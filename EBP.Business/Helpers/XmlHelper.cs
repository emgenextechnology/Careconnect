using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml;
using System.Xml.Linq;

namespace EBP.Business.Helpers
{
    public class XmlHelper
    {
        public XmlDocument xmlDocument { get; set; }

        public XmlHelper()
        {
            xmlDocument = new XmlDocument();
        }

        string _xmlString;
        public string xmlString
        {
            get
            {
                return _xmlString;
            }
            set
            {
                xmlDocument.LoadXml(value);
                _xmlString = value;
            }
        }

        public XDocument XmlMapper { get; set; }

        public ExcelColumn ExcelColumn { get; set; }

        public ExcelColumn GetNodeValue(ref XElement xElement, string searchKey, bool nullIfNoNode = true)
        {
            if (xElement.HasElements)
                searchKey = ((string)xElement.Element(searchKey)).Replace(" ", "");

            string[] _searchKeys = searchKey.Split('+');
            ExcelColumn = new Helpers.ExcelColumn();
            ExcelColumn.NodeName = searchKey;

            if (_searchKeys.Length > 1)
            {
                /* Case when node need to be return after concatinate. Eg: Return "FullName" if "FirstName" and "LastName" are in different Fields. Input Format : [Firstg NodeName]+[SecondNodeName] */

                foreach (string item in _searchKeys)
                {
                    if (searchKey == string.Empty)
                    {
                        //_returnVal = null;
                        ExcelColumn.Value = null;
                        continue;
                    }
                    else
                        break;
                }

                //if (_returnVal == null)
                if (ExcelColumn.Value == null)
                    return new Helpers.ExcelColumn();

                foreach (string _searchKey in _searchKeys)
                {
                    if (xmlDocument.DocumentElement.ChildNodes[0] != null)
                    {
                        var element = xmlDocument.DocumentElement.ChildNodes[0].SelectNodes(_searchKey);
                        if (element.Count > 0)
                        {
                            //_returnVal = _returnVal + " " + element[0].InnerText;
                            ExcelColumn.Value = ExcelColumn.Value + " " + element[0].InnerText;
                            ExcelColumn.HasColumn = true;
                        }
                    }
                }

                //return (_returnVal.Trim() != string.Empty) ? _returnVal.Trim() : null;
                ExcelColumn.Value = (ExcelColumn.Value.Trim() != string.Empty) ? ExcelColumn.Value.Trim() : null;
                return ExcelColumn;
            }
            else
            {
                /* Case when a portion of a value need to be taken. Eg: "FullName" is in a single field and need to take "FirstName" Input Format : [NodeName]^[Index] */

                _searchKeys = searchKey.Split('^');
                if (_searchKeys.Length > 1)
                {
                    if (_searchKeys[0] == string.Empty) /* return null if searchkey string is empty */
                        return new Helpers.ExcelColumn();

                    string nodeValue = null;
                    if (xmlDocument.DocumentElement.ChildNodes[0] != null)
                    {
                        var element = xmlDocument.DocumentElement.ChildNodes[0].SelectNodes(_searchKeys[0]);
                        if (element.Count > 0)
                        {
                            nodeValue = element[0].InnerText;
                            ExcelColumn.HasColumn = true;
                        }
                    }

                    if (string.IsNullOrEmpty(nodeValue))
                        return new Helpers.ExcelColumn();

                    int index; int.TryParse(_searchKeys[1], out index);
                    var valueArray = nodeValue.Split(' ');

                    //return index > 0 && valueArray.Count() >= index ? valueArray[index - 1].Trim().Replace(",", "") : null;
                    ExcelColumn.Value = index > 0 && valueArray.Count() >= index ? valueArray[index - 1].Trim().Replace(",", "") : null;
                    return ExcelColumn;

                }
                else
                {
                    if (searchKey == string.Empty) /* return null if searchkey string is empty */
                        return new Helpers.ExcelColumn();

                    string nodeValue = null;
                    if (xmlDocument.DocumentElement.ChildNodes[0] != null)
                    {
                        var element = xmlDocument.DocumentElement.ChildNodes[0].SelectNodes(searchKey);
                        if (element.Count > 0)
                        {
                            nodeValue = element[0].InnerText;
                            ExcelColumn.HasColumn = true;
                        }
                    }

                    ExcelColumn.Value = string.IsNullOrEmpty(nodeValue) ? null : nodeValue.Trim();
                    return ExcelColumn;
                }
            }
        }

        public ExcelColumn GetNodeValue(XElement xElement, string search)
        {
            return GetNodeValue(ref xElement, search, false);
        }

        public void RemoveFirstChild()
        {
            if (this.xmlDocument.DocumentElement.ChildNodes[0] != null)
                this.xmlDocument.DocumentElement.RemoveChild(this.xmlDocument.DocumentElement.ChildNodes[0]);
        }

        public string GetFirstChildAsJson()
        {
            if (this.xmlDocument.DocumentElement.ChildNodes[0] != null)
            {
                return JsonConvert.SerializeXmlNode(this.xmlDocument.DocumentElement.ChildNodes[0]);
            }
            else
                return null;
        }
    }


    public static class DocumentExtensions
    {
        /// <summary>
        /// Convert XDocument to XmlDocument
        /// </summary>
        /// <param name="xdoc">XDocument object as input</param>
        /// <returns></returns>
        public static XmlDocument ToXmlDocument(this XDocument xdoc)
        {
            var xmlDocument = new XmlDocument();
            using (var reader = xdoc.CreateReader())
            {
                xmlDocument.Load(reader);
            }
            return xmlDocument;
        }

        /// <summary>
        /// Convert XmlDocument object to XDocument object
        /// </summary>
        /// <param name="xmlDoc">XmlDocument as input</param>
        /// <returns></returns>
        public static XDocument ToXDocument(this XmlDocument xmlDoc)
        {
            using (var reader = new XmlNodeReader(xmlDoc))
            {
                reader.MoveToContent();
                return XDocument.Load(reader);
            }
        }
    }

    public class ExcelColumn
    {
        public string Value { get; set; }

        public bool HasColumn { get; set; }

        public string NodeName { get; set; }
    }
}
