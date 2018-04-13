using EBP.FtpImports;
using System;
using System.Configuration;
using System.IO;
using System.ServiceProcess;
using System.Timers;

namespace EBP.WindowsService
{
    partial class FtpSalesReportImportService : ServiceBase
    {
        private string pathTimeFrequency = string.Empty;
        private string pathOfLogFile = string.Empty;
        private string basePath = string.Empty;

        //INITIALIZE THE TIMER
        private Timer timer = new Timer();
        public FtpSalesReportImportService()
        {
            this.ServiceName = "FtpSalesReportImportService";
            this.CanShutdown = true;
            try
            {
                ClientSettingsSection clientSettingsSection = (ClientSettingsSection)ConfigurationManager.GetSection("EBP.FtpSalesReportImport");

                pathTimeFrequency = clientSettingsSection.Settings.Get("TimeFrequency").Value.ValueXml.InnerText;
                pathOfLogFile = clientSettingsSection.Settings.Get("LogFile").Value.ValueXml.InnerText;
                basePath = clientSettingsSection.Settings.Get("BasePath").Value.ValueXml.InnerText;
                TraceService("FtpSalesReportImportService", false);
                InitializeComponent();
            }
            catch (Exception ex)
            {
                TraceService("ERROR: " + ex.Message, false);
            }
        }

        protected override void OnStart(string[] args)
        {
           // System.Diagnostics.Debugger.Launch();
            //ADD THIS LINE TO TEXT FILE DURING START OF SERVICE
            TraceService("Service Started: " + DateTime.Now.ToString(), false);

            //HANDLE ELAPSED EVENT
            timer.Elapsed += new ElapsedEventHandler(OnElapsedTime);

            //THIS STATEMENT IS USED TO SET INTERVAL TO 1 MINUTE (= 60,000 MILLISECONDS)
            timer.Interval = TimeSpan.FromMinutes(Convert.ToDouble(pathTimeFrequency)).TotalMilliseconds;

            //ENABLING THE TIMER
            timer.Enabled = true;
        }

        protected override void OnStop()
        {
            timer.Enabled = false;
            TraceService("Service Stopped: " + DateTime.Now.ToString(), false);
        }

        private void OnElapsedTime(object source, ElapsedEventArgs e)
        {
            //System.Diagnostics.Debugger.Launch();
            //Import Files from Ftp -- Start
            try
            {
                ImportFtpFiles importFtp = new ImportFtpFiles();
                importFtp.ImportFiles(basePath);
            }
            catch (Exception ex)
            {
                TraceService("Error:OnElapsedTime: " + ex.Message, false);
            }
            //Import Files from Ftp -- End
        }

        public void TraceService(string message, bool isLogInDB)
        {
            string directory = Path.GetDirectoryName(pathOfLogFile);
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            //SET UP A FILESTREAM
            FileStream fileStream = new FileStream(pathOfLogFile, FileMode.OpenOrCreate, FileAccess.Write);

            //SET UP A STREAMWRITER FOR ADDING TEXT
            StreamWriter streamWriter = new StreamWriter(fileStream);

            //FIND THE END OF THE UNDERLYING FILESTREAM
            streamWriter.BaseStream.Seek(0, SeekOrigin.End);

            //ADD THE TEXT
            streamWriter.WriteLine(message);

            //ADD THE TEXT TO THE UNDERLYING FILESTREAM
            streamWriter.Flush();

            //CLOSE THE WRITER
            streamWriter.Close();
        }
    }
}
