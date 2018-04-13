using Bytescout.Spreadsheet;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace EBP.Api.Helpers
{

    public class ExcelToXml
    {
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
                            if (firstRowIsHeader && i == 0)
                                dt.Columns[j].Caption = strValue;
                            else
                                dr["column" + j] = strValue;
                        }
                        //Don't add an empty row if we are just reading in header field names
                        if (!(firstRowIsHeader && i == 0))
                            dt.Rows.Add(dr);
                    }

                    //Rename the column names to the header field names, if the first row is header.
                    if (firstRowIsHeader)
                    {
                        for (int k = 0; k < dt.Columns.Count; k++)
                        {
                            dt.Columns[k].ColumnName = (dt.Columns[k].Caption.Replace(" ", ""));
                        }
                    }
                    return ds.GetXml();
                }
            }
            catch
            {
                RecordCount = 0;
                throw;
            }
        }
    }
}