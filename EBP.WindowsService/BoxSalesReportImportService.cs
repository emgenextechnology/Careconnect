using EBP.BoxImports;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace EBP.WindowsService
{
    partial class BoxSalesReportImportService : ServiceBase
    {
        public DateTime LastRunTime { get; set; }
        public DateTime NextRuntime { get; set; }
        private string pathTimeFrequency = string.Empty;
        private string pathOfLogFile = string.Empty;
        private string triggerTime = string.Empty;
        private string basePath = string.Empty;
        private BoxSettings boxSettings = null; 
        //INITIALIZE THE TIMER
        private Timer timer = new Timer();

        public BoxSalesReportImportService()
        {
            this.ServiceName = "BoxSalesReportImportService";
            this.CanShutdown = true;

            ClientSettingsSection clientSettingsSection = (ClientSettingsSection)ConfigurationManager.GetSection("EBP.BoxSalesReportImport");
            
            pathTimeFrequency = clientSettingsSection.Settings.Get("TimeFrequency").Value.ValueXml.InnerText;
            pathOfLogFile = clientSettingsSection.Settings.Get("LogFile").Value.ValueXml.InnerText;
            basePath = clientSettingsSection.Settings.Get("BasePath").Value.ValueXml.InnerText;
            TraceService("BoxSalesReportImportService", false);
            InitializeComponent();
            try
            {
                boxSettings = new BoxSettings
                {

                    ClientID = clientSettingsSection.Settings.Get("CLIENT_ID").Value.ValueXml.InnerText,
                    ClientSecret = clientSettingsSection.Settings.Get("CLIENT_SECRET").Value.ValueXml.InnerText,
                    EnterpriceId = clientSettingsSection.Settings.Get("ENTERPRISE_ID").Value.ValueXml.InnerText,
                    JwtPrivateKeyPassword = clientSettingsSection.Settings.Get("JWT_PRIVATE_KEY_PASSWORD").Value.ValueXml.InnerText,
                    JwtPublicKeyId = clientSettingsSection.Settings.Get("JWT_PUBLIC_KEY_ID").Value.ValueXml.InnerText,
                    PrivateKey = clientSettingsSection.Settings.Get("PRIVATE_KEY").Value.ValueXml.InnerText,

                };
            }
            catch (Exception ex)
            {
                TraceService("ERROR: " + ex.Message, false);
            }
        }

        protected override void OnStart(string[] args)
        {
            //System.Diagnostics.Debugger.Launch();
            //ADD THIS LINE TO TEXT FILE DURING START OF SERVICE
            TraceService("Service Started: @" + DateTime.Now.ToString(), false);

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
            TraceService("Service Stopped: @" + DateTime.Now.ToString(), false);
        }

        private void OnElapsedTime(object source, ElapsedEventArgs e)
        {
            //System.Diagnostics.Debugger.Launch();
            //Import Files from Box -- Start
            try
            {
                TraceService("box import triggered @" + DateTime.Now.ToString(), false);
                ImportBoxFiles importbox = new ImportBoxFiles(boxSettings);
                importbox.ImportFiles(basePath);
            }catch(Exception ex)
            {
                TraceService("Error:OnElapsedTime: " + ex.Message, false);
            }
            //Import Files from Box -- End
        }

        public void TraceService(string message, bool isLogInDB)
        {
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
