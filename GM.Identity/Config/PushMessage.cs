using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Web.Hosting;

namespace GM.Identity.Config
{
   public class PushMessage
    {
       public  void pushMessage(string deviceID, ApplePushPayLoad aps)
       {
           int port = 2195;
           //String hostname = "gateway.sandbox.push.apple.com";

           String hostname = "gateway.push.apple.com";
           var certificatePassword = System.Configuration.ConfigurationManager.AppSettings["AppleCertificatePassword"].ToString();
           String certificatePath = new DirectoryInfo(HostingEnvironment.ApplicationPhysicalPath) + ConfigurationManager.AppSettings["AppleCertificate"];//System.Web.Hosting.HostingEnvironment.MapPath(ConfigurationManager.AppSettings["AppleCertificate"]);
           X509Certificate2 clientCertificate = new X509Certificate2(System.IO.File.ReadAllBytes(certificatePath), ConfigurationManager.AppSettings["AppleCertificatePassword"]);
           X509Certificate2Collection certificatesCollection = new X509Certificate2Collection(clientCertificate);

           TcpClient client = new TcpClient(hostname, port);
           SslStream sslStream = new SslStream(client.GetStream(), false, new RemoteCertificateValidationCallback(ValidateServerCertificate), null);

           //try
           //{
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

           //}
           //catch (System.Security.Authentication.AuthenticationException ex)
           //{
           //    ex.Log();
           //    client.Close();
           //}
           //catch (Exception e)
           //{
           //    e.Log();
           //    client.Close();
           //}
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
   public class aps
   {
       public string alert { get; set; }
       public string sound { get; set; }
       public int badge { get; set; }
   }
   public class ApplePushPayLoad
   {
       public aps aps { get; set; }
       public string EventId { get; set; }
       public int Type { get; set; }

       public int? UserId { get; set; }
   }
}
