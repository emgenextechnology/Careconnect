using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace EBP.WindowsService
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            //Debugger.Break();
            //System.Diagnostics.Debugger.Launch();
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new BoxSalesReportImportService(),
                new FtpSalesReportImportService(),
                new SalesReportImportService()
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}
