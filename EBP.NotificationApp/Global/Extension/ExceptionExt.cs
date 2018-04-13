using EBP.NotificationApp.Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

public static class ExceptionExt
{
    public static int Log(this Exception ex, string attachment = "", bool isSales = false)
    {
        try
        {
            var fileName = "Log/" + DateTime.Now.Year+DateTime.Now.Month+DateTime.Now.Day + ".txt";
            FileInfo file = new FileInfo(fileName);
            if (!file.Exists)
            {
                var fs = file.Create();
                fs.Dispose();
            }

            //using (System.IO.StreamWriter logWriter = new System.IO.StreamWriter("EventLog.txt"))
            //{
            //    logWriter.WriteLine(ex.Message + ": @"+DateTime.Now.ToString());
            //}
            File.AppendAllText(fileName,
                   ex.Message + ": @" + DateTime.Now.ToString() + Environment.NewLine);

        }
        catch (Exception ex1)
        {
            var a = ex1.Message;
        }

        //enter error in log table        
        HttpContext currentContext = System.Web.HttpContext.Current;
        var ip = GetIP();
        bool FromLocal = false;
        if (ip != null)
        {
            var ipSplit = ip.Contains(":") ? ip.Split(':') : ip.Split('.');
            FromLocal = ipSplit[ipSplit.Length - 1] == "1";
        }
        if (ip != null && !FromLocal)
        {
            var user = currentContext != null ? currentContext.User.Identity.Name : "0000";

            var mail = new Email();
            mail.Subject = "Careconnect Log : Push notification";

            if (isSales)
            {
                mail.Subject = "Sales Parsing Report";
                string emailTo = ConfigurationManager.AppSettings["ReportLogNotification"];
                if (!string.IsNullOrEmpty(emailTo))
                {
                    var emailArray = emailTo.Split(',');
                    if (emailArray != null && emailArray.Length > 0)
                    {
                        foreach (var email in emailArray)
                        {
                            if (!string.IsNullOrEmpty(email))
                                mail.To.Add(email);
                        }
                    }
                }
            }
            else
            {
                mail.To.Add("kiran@hubspire.com");
            }

            #region manage bcc for developers

            string _bcc = ConfigurationManager.AppSettings["DevNotification"];
            if (!string.IsNullOrEmpty(_bcc))
            {
                var arrayBcc = _bcc.Split(',');
                foreach (var email in arrayBcc)
                {
                    if (!string.IsNullOrEmpty(email))
                        mail.Bcc.Add(email);
                }
            }

            #endregion

            var innerException = "";

            if (ex.InnerException != null)
                innerException = ex.InnerException.Message;


            mail.Body = "Message: " + ex.Message + "<br> User: " + user + "<br>IP: " + ip + "<br>" + innerException;

            StackTrace stackTrace = new StackTrace(true);
            if (stackTrace.FrameCount > 0)
            {
                StackFrame stackFrame = stackTrace.GetFrame(1);
                mail.Body += "<br>Method\t: " + stackFrame.GetMethod().Name +
                    "<br>Line Number\t: " + stackTrace.GetFrame(1).GetFileLineNumber() +
                    "<br>Full Name\t: " + stackFrame.GetMethod().ReflectedType.FullName +
                    "<br>Base Url\t: " + ConfigurationManager.AppSettings["BaseUrl"] ?? "UnKnown";
            }

            if (attachment != "")
            {
                //var attachmentInfo = new System.IO.FileInfo(attachment);
                mail.Attachments.Add(new System.Net.Mail.Attachment(attachment));
            }

            mail.SendMessage();
        }

        return 1;
    }

    public static String GetIP()
    {
        String ip = null;
        HttpContext currentContext = System.Web.HttpContext.Current;

        if (currentContext != null)
        {
            ip = currentContext.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

            if (string.IsNullOrEmpty(ip))
            {
                ip = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            }
        }

        return ip;
    }
}
