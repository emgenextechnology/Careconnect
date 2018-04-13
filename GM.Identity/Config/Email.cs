using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace GM.Identity.Config
{
    public static class MailConfig
    {
        public static string FromEmail
        {
            get
            {
                var emailFrom = ConfigurationManager.AppSettings["EmailFrom"];
                if (emailFrom == null)
                    throw new Exception("Set from default from email-id in config file. App settings key \"EmailFrom\"");

                return ConfigurationManager.AppSettings["EmailFrom"];
            }
        }

        public static string SmtpHost
        {
            get
            {
                var emailFrom = ConfigurationManager.AppSettings["SmtpHost"];
                if (emailFrom == null)
                    throw new Exception("Please set App settings key \"SmtpHost\"");

                return ConfigurationManager.AppSettings["SmtpHost"];
            }
        }

        public static string SmtpPort
        {
            get
            {
                var emailFrom = ConfigurationManager.AppSettings["SmtpPort"];
                if (emailFrom == null)
                    throw new Exception("Please set App settings key \"SmtpPort\"");

                return ConfigurationManager.AppSettings["SmtpPort"];
            }
        }

        public static string SmtpUserName
        {
            get
            {
                var emailFrom = ConfigurationManager.AppSettings["SmtpUserName"];
                if (emailFrom == null)
                    throw new Exception("Please set App settings key \"SmtpUserName\"");

                return ConfigurationManager.AppSettings["SmtpUserName"];
            }

        }

        public static string SmtpPassword
        {
            get
            {
                var emailFrom = ConfigurationManager.AppSettings["SmtpPassword"];
                if (emailFrom == null)
                    throw new Exception("Please set App settings key \"SmtpPassword\"");

                return ConfigurationManager.AppSettings["SmtpPassword"];
            }
        }
    }

    public class GMEmail : MailMessage
    {
        public string CurrentHost
        {
            get;
            set;
        }

        public void SendMessage()
        {
            this.IsBodyHtml = true;
            if (this.From == null)
            {
                if (string.IsNullOrEmpty(CurrentHost))
                    From = new MailAddress(MailConfig.FromEmail, "careconnectsystems.com");
                else
                    From = new MailAddress("noreply@" + CurrentHost, CurrentHost);
            }
            try
            {
                SmtpClient smtp = new SmtpClient()
                {
                    Host = MailConfig.SmtpHost,
                    Port = Convert.ToInt32(MailConfig.SmtpPort),
                    EnableSsl = true,
                    Credentials = new System.Net.NetworkCredential(MailConfig.SmtpUserName, MailConfig.SmtpPassword)
                };


                smtp.Send(this);

            }
            catch (Exception ex)
            {
                string message = string.Format("Time: {0}", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt"));
                message += Environment.NewLine;
                message += "-----------------------------------------------------------";
                message += Environment.NewLine;
                message += string.Format("Message: {0}", ex.Message);
                message += Environment.NewLine;
                message += string.Format("StackTrace: {0}", ex.StackTrace);
                message += Environment.NewLine;
                message += string.Format("Source: {0}", ex.Source);
                message += Environment.NewLine;
                message += string.Format("TargetSite: {0}", ex.TargetSite.ToString());
                message += Environment.NewLine;
                message += "-----------------------------------------------------------";
                message += Environment.NewLine;
                string path = ConfigurationManager.AppSettings["LogFolderPath"] + " / ErrorLog/Err-" + DateTime.Now.Ticks + ".txt";
                using (StreamWriter writer = new StreamWriter(path, true))
                {
                    writer.WriteLine(message);
                    writer.Close();
                }
            }
        }

        public void SendDynamicHTMLEmail(string to, string subject, string MessageBody, string bcc = null, string cc = null)
        {
            try
            {
                new Thread(() =>
                {
                    Thread.CurrentThread.IsBackground = true;

                    GMEmail emailMessage = new GMEmail();

                    if (this.From == null)
                    {
                        if (string.IsNullOrEmpty(CurrentHost))
                            From = new MailAddress(MailConfig.FromEmail, "careconnectsystems.com");
                        else
                            From = new MailAddress("noreply@" + CurrentHost, CurrentHost);
                    }

                    emailMessage.Body = MessageBody;
                    emailMessage.IsBodyHtml = true;
                    if (ConfigurationManager.AppSettings["IsInDemo"] == "true")
                    {
                        emailMessage.To.Add(ConfigurationManager.AppSettings["DemoEmailId"]);
                    }
                    else
                    {
                        emailMessage.To.Add(to);

                        #region Manage bcc in parameter

                        if (!string.IsNullOrEmpty(bcc))
                        {
                            var arrayBcc = bcc.Split(',');
                            foreach (var email in arrayBcc)
                            {
                                if (!string.IsNullOrEmpty(email))
                                    emailMessage.Bcc.Add(email);
                            }
                        }

                        #endregion

                        #region Manage cc in parameter

                        if (!string.IsNullOrEmpty(cc))
                        {
                            var arrayBcc = cc.Split(',');
                            foreach (var email in arrayBcc)
                            {
                                if (!string.IsNullOrEmpty(email))
                                    emailMessage.Bcc.Add(email);
                            }
                        }

                        #endregion
                    }

                    #region Manage bcc for developers

                    string _bcc = ConfigurationManager.AppSettings["DevNotification"];
                    if (!string.IsNullOrEmpty(_bcc))
                    {
                        var arrayBcc = _bcc.Split(',');
                        foreach (var email in arrayBcc)
                        {
                            if (!string.IsNullOrEmpty(email))
                                emailMessage.Bcc.Add(email);
                        }
                    }

                    #endregion

                    emailMessage.Subject = subject;

                    /* run your code here */
                    emailMessage.SendMessage();
                }).Start();
            }
            catch (Exception ex)
            {
                //throw ex;
            }
        }
    }
}
