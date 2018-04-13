namespace EBP.WindowsService
{
    partial class ProjectInstaller
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.serviceProcessInstaller1 = new System.ServiceProcess.ServiceProcessInstaller();
            this.boxServiceInstaller = new System.ServiceProcess.ServiceInstaller();
            this.ftpServiceInstaller = new System.ServiceProcess.ServiceInstaller();
            this.salesParsingServiceInstaller = new System.ServiceProcess.ServiceInstaller();
            // 
            // serviceProcessInstaller1
            // 
            this.serviceProcessInstaller1.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
            this.serviceProcessInstaller1.Password = null;
            this.serviceProcessInstaller1.Username = null;
            // 
            // boxServiceInstaller
            // 
            this.boxServiceInstaller.ServiceName = "BoxSalesReportImportService";
            this.boxServiceInstaller.DisplayName = "EBP.BoxImportService";
            this.boxServiceInstaller.StartType = System.ServiceProcess.ServiceStartMode.Automatic;

            // 
            // ftpServiceInstaller
            // 
            this.ftpServiceInstaller.ServiceName = "FtpSalesReportImportService";
            this.ftpServiceInstaller.DisplayName = "EBP.FtpImportService";
            this.ftpServiceInstaller.StartType = System.ServiceProcess.ServiceStartMode.Automatic;

            // 
            // saleParsingServiceInstaller
            // 
            this.salesParsingServiceInstaller.ServiceName = "SalesReportImportService";
            this.salesParsingServiceInstaller.DisplayName = "EBP.SalesParsingService";
            this.salesParsingServiceInstaller.StartType = System.ServiceProcess.ServiceStartMode.Automatic;

            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.serviceProcessInstaller1,
            this.boxServiceInstaller,
            this.ftpServiceInstaller,
            this.salesParsingServiceInstaller});

        }

        #endregion
        private System.ServiceProcess.ServiceInstaller boxServiceInstaller;
        private System.ServiceProcess.ServiceInstaller ftpServiceInstaller;
        private System.ServiceProcess.ServiceInstaller salesParsingServiceInstaller;
        private System.ServiceProcess.ServiceProcessInstaller serviceProcessInstaller1;
    }
}