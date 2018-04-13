using Newtonsoft.Json;
using EBP.NotificationApp.Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Web;

namespace EBP.NotificationApp.Helper.Notification
{
    public class NotificationHelper
    {
        public static bool pushMessage(string deviceID, ApplePushPayLoad aps, string certificatePath)
        {
           
            try
            {
                int port = 2195;
                //String hostname = "gateway.sandbox.push.apple.com";

                String hostname = "gateway.push.apple.com";
                var certificatePassword = ConfigurationManager.AppSettings["AppleCertificatePassword"];
                //    var certificatePassword = System.Configuration.ConfigurationManager.AppSettings["AppleCertificatePassword"].ToString();        
                X509Certificate2 clientCertificate = new X509Certificate2(System.IO.File.ReadAllBytes(certificatePath), ConfigurationManager.AppSettings["AppleCertificatePassword"]);
                X509Certificate2Collection certificatesCollection = new X509Certificate2Collection(clientCertificate);

                TcpClient client = new TcpClient(hostname, port);
                SslStream sslStream = new SslStream(client.GetStream(), false, new RemoteCertificateValidationCallback(ValidateServerCertificate), null);

                sslStream.AuthenticateAsClient(hostname, certificatesCollection, SslProtocols.Default, false);
                MemoryStream memoryStream = new MemoryStream();
                BinaryWriter writer = new BinaryWriter(memoryStream);
                writer.Write((byte)0);
                writer.Write((byte)0);
                writer.Write((byte)32);

                writer.Write(HexStringToByteArray(deviceID.ToUpper()));
                String payload = JsonConvert.SerializeObject(aps);
                writer.Write((byte)0);
                writer.Write((byte)payload.Length);
                byte[] b1 = System.Text.Encoding.UTF8.GetBytes(payload);
                writer.Write(b1);
                writer.Flush();
                byte[] array = memoryStream.ToArray();
                sslStream.Write(array);
                sslStream.Flush();
                client.Close();
                return true;
            }
            catch (Exception ex)
            {
                new EventsLog().Write("Error: "+ ex.Message);
                ex.Log();
                //new Exception("Push failed:- \tDeviceId:" + toDeviceId + "\tUser: " + ToUserId + "\tMessage:" + Message).Log();
                //throw ex;
            }
            return false;
        }


        // The following method is invoked by the RemoteCertificateValidationDelegate.
        public static bool ValidateServerCertificate(
              object sender,
              X509Certificate certificate,
              X509Chain chain,
              SslPolicyErrors sslPolicyErrors)
        {
            if (sslPolicyErrors == SslPolicyErrors.None)
                return true;

            Console.WriteLine("Certificate error: {0}", sslPolicyErrors);

            // Do not allow this client to communicate with unauthenticated servers.
            return false;
        }

        public static byte[] HexStringToByteArray(string hex)
        {
            return Enumerable.Range(0, hex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                             .ToArray();
        }

    }
}