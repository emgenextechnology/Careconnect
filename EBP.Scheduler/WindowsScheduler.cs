using EBP.Business;
using EBP.Business.Helpers;
using EBP.Business.Repository;
using EBP.Scheduler.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using System.Configuration;
using EBP.BoxImports;
using EBP.FtpImports;

namespace EBP.Scheduler
{
    public partial class WindowsScheduler : Form
    {
        string BasePath = ConfigurationManager.AppSettings["BasePath"],
            XmlMapper = ConfigurationManager.AppSettings["XmlMapper"];
        private string pathTimeFrequency = string.Empty;
        private string pathOfLogFile = string.Empty;
        private string basePath = string.Empty;
        private BoxSettings boxSettings = null;

        public WindowsScheduler()
        {
            InitializeComponent();
        }

        private void WindowsScheduler_Load(object sender, EventArgs e)
        {

            DialogResult dialogueResult = MessageBox.Show("Do you want to start the scheduler?", "Confirm", MessageBoxButtons.YesNo);
            if (dialogueResult == DialogResult.Yes)
            {
                //Import Files from Box -- Start
                ClientSettingsSection clientSettingsSection = (ClientSettingsSection)ConfigurationManager.GetSection("EBP.BoxSalesReportImport");

                pathTimeFrequency = clientSettingsSection.Settings.Get("TimeFrequency").Value.ValueXml.InnerText;
                pathOfLogFile = clientSettingsSection.Settings.Get("LogFile").Value.ValueXml.InnerText;
                basePath = clientSettingsSection.Settings.Get("BasePath").Value.ValueXml.InnerText;

                boxSettings = new BoxSettings
                {
                    ClientID = clientSettingsSection.Settings.Get("CLIENT_ID").Value.ValueXml.InnerText,
                    ClientSecret = clientSettingsSection.Settings.Get("CLIENT_SECRET").Value.ValueXml.InnerText,
                    EnterpriceId = clientSettingsSection.Settings.Get("ENTERPRISE_ID").Value.ValueXml.InnerText,
                    JwtPrivateKeyPassword = clientSettingsSection.Settings.Get("JWT_PRIVATE_KEY_PASSWORD").Value.ValueXml.InnerText,
                    JwtPublicKeyId = clientSettingsSection.Settings.Get("JWT_PUBLIC_KEY_ID").Value.ValueXml.InnerText,
                    PrivateKey = clientSettingsSection.Settings.Get("PRIVATE_KEY").Value.ValueXml.InnerText,
                };

                ImportBoxFiles importbox = new ImportBoxFiles(boxSettings);
                importbox.ImportFiles(BasePath);
                //Import Files from Box -- End

                //Import Files from Box -- Start
                ImportFtpFiles importFtp = new ImportFtpFiles();
                importFtp.ImportFiles(basePath);
                //Import Files from Box -- End

                ImportSales();
                var timer = new System.Windows.Forms.Timer();

                timer.Interval = (int)(float.Parse(ConfigurationManager.AppSettings["ImportInterval"]) * 60000);
                timer.Tick += timer1_Tick;
                timer.Start();
            }

            //Application.Exit();
        }

        private void ImportSales()
        {
            try
            {
                int BusinessId, ServiceId;

                string[] directories = Directory.GetDirectories(BasePath, "*", SearchOption.TopDirectoryOnly);
                FileInfo fileInfo;

                foreach (string directory in directories)
                {
                    fileInfo = new FileInfo(directory);
                    if (int.TryParse(fileInfo.Name, out BusinessId))
                    {
                        string salesPath = Path.Combine(BasePath, directory, "Sales");
                        if (Directory.Exists(salesPath))
                        {
                            string[] salesDirectories = Directory.GetDirectories(salesPath, "*", SearchOption.TopDirectoryOnly);
                            foreach (string salesDirectory in salesDirectories)
                            {
                                fileInfo = new FileInfo(salesDirectory);
                                if (fileInfo.Name != "Sales-Archives" && fileInfo.Name != "Sales-Not-Processed")
                                {
                                    List<string> filesList = Directory.GetFiles(Path.Combine(salesPath, fileInfo.Name), "*.xls?", SearchOption.AllDirectories).ToList();
                                    string serviceName = fileInfo.Name;
                                    ServiceId = new RepositorySales().GetServiceId(serviceName, BusinessId);
                                    if (ServiceId > 0 && filesList.Count > 0)
                                    {
                                        SaveSalesReports(filesList, BusinessId, ServiceId, serviceName, 1, directory);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);
                ex.Log();
            }
        }

        private void SaveSalesReports(List<string> filesList, int businessId, int serviceId, string serviceName, int userId = 1, string directory = "")
        {
            #region Save records to database

            try
            {
                DataResponse response = new DataResponse();
                XmlHelper xmlHelper = new XmlHelper();
                RepositorySales repositorySales = new RepositorySales();
                FileInfo fileInfo;

                if (filesList != null && filesList.Count > 0)
                {
                    foreach (var filePath in filesList)
                    {
                        string logPath = Path.Combine(BasePath, directory, "Logs", "Sales");
                        if (!Directory.Exists(logPath))
                            Directory.CreateDirectory(logPath);
                        string logFilePath = Path.Combine(logPath, string.Format("{0}.txt", DateTime.Now.ToString("MMddyyhhmmssttfff")));

                        if (!File.Exists(logFilePath))
                        {
                            FileStream fs = File.Create(logFilePath);
                            fs.Dispose();
                        }

                        StringBuilder logString = new StringBuilder();
                        using (System.IO.StreamWriter logWriter = new System.IO.StreamWriter(logFilePath))
                        {
                            logString.Clear();
                            logString.AppendLine("Parsing starts @ " + DateTime.Now);
                            logString.AppendLine(string.Format("Total Files \t:\t{0}\t\nService Name \t:\t{1}", filesList.Count(), serviceName));
                            logString.AppendLine("BusinessId \t:\t" + businessId);

                            string mapperFilePath = Path.Combine(BasePath, businessId.ToString(), "Sales", serviceName, "ReportColumnMapper.xml");

                            if (!File.Exists(mapperFilePath))
                            {
                                logString.Append(string.Format("XML mapper file is missing for the service \"{0}\"", serviceName));
                                continue;
                            }

                            xmlHelper.XmlMapper = XDocument.Load(mapperFilePath);

                            int RecordCount;
                            xmlHelper.xmlString = new ExcelToXml().GetXMLString(filePath, true, out RecordCount);

                            logString.AppendLine(string.Format("File Name \t:\t{0} \t Starts @{1}", Path.GetFileName(filePath), DateTime.UtcNow));

                            logWriter.WriteLine(logString);
                            logString.Clear();

                            int importSummeryId;
                            response = repositorySales.Insert(xmlHelper, RecordCount, businessId, userId, serviceId, false, out importSummeryId, logWriter, incomingFileName: Path.GetFileName(filePath));

                            string destinationPath = Path.Combine(BasePath, businessId.ToString(), "Sales", "Sales-Archives", serviceName);
                            if (response.Status != DataResponseStatus.OK)
                            {
                                logString.AppendLine("ERROR occured in file");
                                destinationPath = Path.Combine(BasePath, businessId.ToString(), "Sales", "Sales-Not-Processed", serviceName);
                            }
                            else
                            {
                                logString.AppendLine("File Successfully Processed");
                            }

                            if (!Directory.Exists(destinationPath))
                                Directory.CreateDirectory(destinationPath);
                            fileInfo = new FileInfo(filePath);
                            string fileName = fileInfo.Name,
                                destinationFilePath = Path.Combine(destinationPath, fileName),
                                fileNameWithoutExt = Path.GetFileNameWithoutExtension(fileName),
                                extension = Path.GetExtension(fileName);
                            int count = 1;
                        isExist:
                            if (File.Exists(destinationFilePath))
                            {
                                fileName = string.Format("{0}{1}{2}", fileNameWithoutExt, count, extension);
                                destinationFilePath = Path.Combine(Path.GetDirectoryName(destinationFilePath), fileName);
                                count++;
                                goto isExist;
                            }

                            File.Move(filePath, destinationFilePath);

                            logString.AppendLine("Parsing completed @ " + DateTime.Now);
                            logString.AppendLine("File moved to :" + destinationFilePath);

                            if (importSummeryId > 0)
                            {
                                repositorySales.UpdateImportSummery(importSummeryId, Path.Combine(GetRightPartOfPath(destinationFilePath, "Assets"), fileName));
                            }

                            logWriter.WriteLine(logString);
                        }
                        new Exception(logString.ToString()).Log(logFilePath, true);
                    }
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);
                ex.Log();
            }

            #endregion
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            ImportSales();
        }

        private static string GetRightPartOfPath(string path, string startAfterPart)
        {
            // use the correct seperator for the environment
            var pathParts = path.Split(Path.DirectorySeparatorChar);

            // this assumes a case sensitive check. If you don't want this, you may want to loop through the pathParts looking
            // for your "startAfterPath" with a StringComparison.OrdinalIgnoreCase check instead
            int startAfter = Array.IndexOf(pathParts, startAfterPart);

            if (startAfter == -1)
            {
                // path path not found
                return null;
            }

            // try and work out if last part was a directory - if not, drop the last part as we don't want the filename
            var lastPartWasDirectory = pathParts[pathParts.Length - 1].EndsWith(Path.DirectorySeparatorChar.ToString());
            return string.Join(
                Path.DirectorySeparatorChar.ToString(),
                pathParts, startAfter,
                pathParts.Length - startAfter - (lastPartWasDirectory ? 0 : 1));
        }
    }
}
