using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EBP.NotificationApp.Services
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

    public class Email : MailMessage
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
            //try
            //{
            SmtpClient smtp = new SmtpClient()
            {
                Host = MailConfig.SmtpHost,
                Port = Convert.ToInt32(MailConfig.SmtpPort),
                EnableSsl = true,
                Credentials = new System.Net.NetworkCredential(MailConfig.SmtpUserName, MailConfig.SmtpPassword)
            };

            //SmtpClient smtp = new SmtpClient("localhost");

            smtp.Send(this);
            //}
            //catch (Exception ex)
            //{
            //    throw ex;
            //    ex.Log();
            //}
        }
        public void SendDynamicHTMLEmail(string to, string subject, string MessageBody, string bcc = null, string cc = null)
        {
            try
            {
                new Thread(() =>
                {
                    Thread.CurrentThread.IsBackground = true;

                    Email emailMessage = new Email();

                    if (this.From == null)
                    {
                        if (string.IsNullOrEmpty(CurrentHost))
                            From = new MailAddress(MailConfig.FromEmail, "careconnectsystems.com");
                        else
                            From = new MailAddress("noreply@" + CurrentHost, CurrentHost);
                    }

                    emailMessage.Body = MessageBody;
                    emailMessage.IsBodyHtml = true;
                    if (ConfigurationSettings.AppSettings["IsInDemo"] == "true")
                    {
                        emailMessage.To.Add(ConfigurationSettings.AppSettings["DemoEmailId"]);
                    }
                    else
                    {
                        emailMessage.To.Add(to);
                    }
                    if (!string.IsNullOrEmpty(bcc))
                    {
                        emailMessage.Bcc.Add(bcc);
                    }
                    if (!string.IsNullOrEmpty(cc))
                    {
                        emailMessage.CC.Add(cc);
                    }
                    //emailMessage.From = new MailAddress(ApplicationConfig.CustomerServiceEmail);

                    #region manage bcc for developers

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

                throw ex;
            }



        }
    }
}
