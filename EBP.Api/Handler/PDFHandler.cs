using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using iTextSharp;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;
using iTextSharp.text.html.simpleparser;
using HtmlAgilityPack;
using EBP.Business;
using EBP.Api.Handler._base;

namespace EBP.Api.Handler
{
    public class PDFHandler : IHttpHandlerBase
    {
        public override void ProcessPost()
        {
            //ServiceResponse.ContentType = "application/pdf";
            //ServiceResponse.AddHeader("content-disposition", "attachment;filename=Panel.pdf");
            //ServiceResponse.Cache.SetCacheability(HttpCacheability.NoCache);

            //StringWriter stringWriter = new StringWriter();
            ////HtmlTextWriter htmlTextWriter = new HtmlTextWriter(stringWriter);
            ////employeelistDiv.RenderControl(htmlTextWriter);
            //var content = ServiceRequest.Params["content"].ToString();
            //StringReader stringReader = new StringReader(HttpUtility.HtmlDecode(content));
            //Document Doc = new Document(PageSize.A4, 10f, 10f, 100f, 0f);
            //HTMLWorker htmlparser = new HTMLWorker(Doc);
            //PdfWriter.GetInstance(Doc, ServiceResponse.OutputStream);

            //Doc.Open();
            //htmlparser.Parse(stringReader);
            //Doc.Close();
            //ServiceResponse.Write(Doc);
            //ServiceResponse.End();

            string strHtml = ServiceRequest.Params["content"].ToString();
            strHtml = HttpUtility.HtmlDecode(strHtml);
            strHtml = strHtml.Replace("\r\n", "");
            strHtml = strHtml.Replace("\0", "");

            /*****************  Calling A Method Synchronously   ********************/
            // Create an instance of a delegate that wraps CreatePDFFromHTMLFile.
            DelegateCreatePDFFromHTMLFile dlgt = new DelegateCreatePDFFromHTMLFile(this.CreatePDFFromHTMLFile);
            // Call CreatePDFFromHTMLFile using the delegate.
            var response = dlgt(strHtml, "/Print");
            ServiceResponse.Write(response.ToJson());
            //ServiceResponse.Write(CreatePDFFromHTMLFile(strHtml, "/Print").ToJson());
        }
        delegate DataResponse DelegateCreatePDFFromHTMLFile(string HtmlStream, string Path);
        public DataResponse CreatePDFFromHTMLFile(string HtmlStream, string Path)
        {
            DataResponse dr = new DataResponse();
            try
            {

                string domain = System.Configuration.ConfigurationManager.AppSettings["domain"];
                string fileName = DateTime.UtcNow.ToString("yyyyMMddHHmmss") + ".pdf";
                object TargetFile = ServiceContext.Server.MapPath(Path) + "/" + fileName;
                string ModifiedFileName = string.Empty;
                string FinalFileName = string.Empty;

                // To add a Password to PDF -http://aspnettutorialonline.blogspot.com/
                HtmlToPdfBuilder builder = new HtmlToPdfBuilder(iTextSharp.text.PageSize.A4);
                HtmlPdfPage first = builder.AddPage();


                HtmlDocument htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(HtmlStream);

                var node = htmlDoc.DocumentNode.SelectSingleNode("//*[contains(@class,'print')]");
                if (node != null)
                {
                    node.ParentNode.RemoveChild(node, true);
                    HtmlStream = HtmlStream.Replace(node.InnerHtml, "");
                }

                // Set image full path
                HtmlStream = HtmlStream.Replace("<img src=\"/", "<img src=\"" + domain + "/");
                HtmlStream = HtmlStream.Replace("width=\"730\"", "");

                HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
                doc.LoadHtml(HtmlStream);

                doc.DocumentNode.Descendants()
                                .Where(n => n.Name == "script" || n.Name == "style")
                                .ToList()
                                .ForEach(n => n.Remove());

                first.AppendHtml(HtmlStream);
                byte[] file = builder.RenderPdf();
                File.WriteAllBytes(TargetFile.ToString(), file);

                iTextSharp.text.pdf.PdfReader reader = new iTextSharp.text.pdf.PdfReader(TargetFile.ToString());
                ModifiedFileName = TargetFile.ToString();
                ModifiedFileName = ModifiedFileName.Insert(ModifiedFileName.Length - 4, "1");

                string password = "password";
                iTextSharp.text.pdf.PdfEncryptor.Encrypt(reader, new FileStream(ModifiedFileName, FileMode.Append), iTextSharp.text.pdf.PdfWriter.STRENGTH128BITS, password, "", iTextSharp.text.pdf.PdfWriter.AllowPrinting);
                //http://aspnettutorialonline.blogspot.com/
                reader.Close();
                if (File.Exists(TargetFile.ToString()))
                    File.Delete(TargetFile.ToString());
                FinalFileName = ModifiedFileName.Remove(ModifiedFileName.Length - 5, 1);
                File.Copy(ModifiedFileName, FinalFileName);
                if (File.Exists(ModifiedFileName))
                    File.Delete(ModifiedFileName);

                dr.Status = DataResponseStatus.OK;
                dr.Message = "PDF created successfully";
                dr.Content = Path + "/" + fileName;

            }
            catch (Exception ex)
            {
                dr.Status = DataResponseStatus.InternalServerError;
                dr.Message = "Request failed";
                dr.Content = "";
                ex.Log();
                //throw ex;
            }
            return dr;
        }



        public override void ProcessGet()
        {
            bool IsSuccess = false;
            if (RequestUrl.Contains("print.pdf"))
            {

                string pid = string.Empty;
                if (ServiceRequest.QueryString.Get("pid") != null)
                {
                    pid = ServiceRequest.QueryString.Get("pid").ToString();
                }
                if (!string.IsNullOrEmpty(pid))
                {
                    // Read html content from template
                    string template = string.Empty;
                    StreamReader sr;
                    try
                    {
                        sr = File.OpenText(ServiceContext.Server.MapPath("/Assets/cors/Agreement.html"));
                        template = sr.ReadToEnd();
                        sr.Close();
                    }
                    catch
                    {

                    }
                    if (!string.IsNullOrEmpty(template))
                    {
                        // Replace texts
                        //template = template.Replace("<%Name%>", "jithesh");

                        Document pdfDoc = new Document(PageSize.A4, 10, 10, 10, 10);
                        string destination = ServiceContext.Server.MapPath("/Assets/Media/Property/");
                        try
                        {
                            PdfWriter.GetInstance(pdfDoc, new FileStream(destination + pid + "/" + pid + "_" + DateTime.UtcNow.ToString("yyyyMMdd") + ".pdf", FileMode.Create));
                            pdfDoc.Open();
                            var parsedHtmlElements = HTMLWorker.ParseToList(new StringReader(template), null);
                            foreach (var htmlElement in parsedHtmlElements)
                                pdfDoc.Add(htmlElement as IElement);
                            pdfDoc.Close();
                            IsSuccess = true;

                        }
                        catch
                        {
                        }
                    }
                }

            }
            if (IsSuccess)
                PostResponse("Created");
            else
                PostResponse("Failed");
        }
        public override void ProcessDelete()
        {
            throw new NotImplementedException();
        }
    }


    //handle Image relative and absolute URL's
    public class ImageHander : IImageProvider
    {
        public string BaseUri;
        public iTextSharp.text.Image GetImage(string src,
        IDictionary<string, string> h,
        ChainedProperties cprops,
        IDocListener doc)
        {
            string imgPath = string.Empty;

            if (src.ToLower().Contains("http://") == false)
            {
                imgPath = "http://hr2014.com/" + src;
            }
            else
            {
                imgPath = src;
            }

            return iTextSharp.text.Image.GetInstance(imgPath);


        }
    }
}