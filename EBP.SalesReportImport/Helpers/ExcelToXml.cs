using Bytescout.Spreadsheet;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace EBP.SalesReportImport.Helpers
{

    public class ExcelToXml
    {
        /// <summary>
        ///  Returns the first sheet in the workbook contained in the given Excel fileName as an XML String
        /// </summary>
        /// <param name="ExcelFilePath">Uploaded file reference as filepath</param>
        /// <param name="firstRowIsHeader">Set to true, if the row contains header field names</param>
        /// <param name="RecordCount"></param>
        /// <returns></returns>
        public string GetXMLString(string ExcelFilePath, bool firstRowIsHeader, out int RecordCount)
        {
            try
            {
                System.Data.DataTable dt = new System.Data.DataTable("RowItem");
                DataSet ds = new DataSet("Parent");
                ds.Tables.Add(dt);
                DataRow dr;
                StringBuilder sb = new StringBuilder();
                using (Spreadsheet document = new Spreadsheet())
                {
                    document.LoadFromFile(ExcelFilePath);
                    Worksheet oSheet = document.Workbook.Worksheets[0];
                    int jValue = oSheet.NotEmptyColumnMax;
                    int iValue = RecordCount = oSheet.UsedRangeRowMax;
                    //  get data columns
                    for (int j = 0; j <= jValue; j++)
                    {
                        dt.Columns.Add("column" + j, System.Type.GetType("System.String"));
                    }
                    // get data in cell.
                    // If the user set the firstRowIsHeader flag then save these cell values as column names instead of actual row values.
                    for (int i = 0; i <= iValue; i++)
                    {
                        dr = dt.NewRow();
                        for (int j = 0; j <= jValue; j++)
                        {
                            string strValue = oSheet[i, j].ValueAsString;
                            if (string.IsNullOrEmpty(strValue))
                            {
                                List<string> lstColumnValues = new List<string>();
                                for (int column = 0; column <= jValue; column++)
                                {
                                    lstColumnValues.Add(oSheet[i, column].ValueAsString);
                                }
                                if (lstColumnValues.All(a => string.IsNullOrEmpty(a)))
                                {
                                    RecordCount = i - 1;
                                    goto exitLoop;
                                }
                            }

                            if (firstRowIsHeader && i == 0)
                                dt.Columns[j].Caption = strValue;
                            else
                                dr["column" + j] = strValue;
                        }

                        //Don't add an empty row if we are just reading in header field names
                        if (!(firstRowIsHeader && i == 0))
                            dt.Rows.Add(dr);
                    }
                exitLoop:

                    //Rename the column names to the header field names, if the first row is header.
                    if (firstRowIsHeader)
                    {
                        for (int k = 0; k < dt.Columns.Count; k++)
                        {
                            var columnName = (dt.Columns[k].Caption.Replace(" ", "").Replace("#", ""));
                            if (!string.IsNullOrEmpty(columnName))
                                dt.Columns[k].ColumnName = columnName;
                        }
                    }
                    return ds.GetXml();
                }
            }
            catch (Exception ex)
            {
                RecordCount = 0;
                throw ex;
            }
        }

        /// <summary>
        ///  Returns the first sheet in the workbook contained in the given Excel fileName as an XML String
        /// </summary>
        /// <param name="fileStream">Uploaded file reference as stream</param>
        /// <param name="firstRowIsHeader">Set to true, if the row contains header field names</param>
        /// <param name="RecordCount"></param>
        /// <returns></returns>
        public string GetXMLString(ref Stream fileStream, bool firstRowIsHeader, out int RecordCount)
        {
            try
            {
                System.Data.DataTable dt = new System.Data.DataTable("RowItem");
                DataSet ds = new DataSet("Parent");
                ds.Tables.Add(dt);
                DataRow dr;
                StringBuilder sb = new StringBuilder();
                using (Spreadsheet document = new Spreadsheet())
                {
                    //document.LoadFromFile("D:\\HUBSPIRE\\EMGENEX-BIZ-PORTAL-2016\\EmgenexBusinessPortal\\Assets\\6\\Sales\\Sales-Archives\\Uploads\\20170329101801975.xlsx");
                    document.LoadFromStream(fileStream);
                    Worksheet oSheet = document.Workbook.Worksheets[0];
                    int jValue = oSheet.UsedRangeColumnMax;
                    int iValue = RecordCount = oSheet.UsedRangeRowMax;
                    //  get data columns
                    for (int j = 0; j < jValue; j++)
                    {
                        dt.Columns.Add("column" + j, System.Type.GetType("System.String"));
                    }
                    // get data in cell.
                    // If the user set the firstRowIsHeader flag then save these cell values as column names instead of actual row values.
                    for (int i = 0; i <= iValue; i++)
                    {
                        dr = dt.NewRow();
                        for (int j = 0; j < jValue; j++)
                        {
                            string strValue = oSheet[i, j].ValueAsString;
                            if (string.IsNullOrEmpty(strValue))
                            {
                                List<string> lstColumnValues = new List<string>();
                                for (int column = 0; column <= jValue; column++)
                                {
                                    lstColumnValues.Add(oSheet[i, column].ValueAsString);
                                }
                                if (lstColumnValues.All(a => string.IsNullOrEmpty(a)))
                                {
                                    RecordCount = i - 1;
                                    goto exitLoop;
                                }
                            }

                            if (firstRowIsHeader && i == 0)
                                dt.Columns[j].Caption = strValue;
                            else
                                dr["column" + j] = strValue;
                        }

                        //Don't add an empty row if we are just reading in header field names
                        if (!(firstRowIsHeader && i == 0))
                            dt.Rows.Add(dr);
                    }
                exitLoop:

                    //Rename the column names to the header field names, if the first row is header.
                    if (firstRowIsHeader)
                    {
                        for (int k = 0; k < dt.Columns.Count; k++)
                        {
                            dt.Columns[k].ColumnName = (dt.Columns[k].Caption.Replace(" ", "").Replace("#", ""));
                        }
                    }
                    return ds.GetXml();
                }
            }
            catch (Exception ex)
            {
                RecordCount = 0;
                throw ex;
            }
        }
    }
}