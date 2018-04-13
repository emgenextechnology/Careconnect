using Bytescout.Spreadsheet;
using EBP.Business.Database;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace EBP.Scheduler
{
    public partial class ServiceFileFix : Form
    {
        public ServiceFileFix()
        {
            InitializeComponent();
        }

        private void btnFix_Click(object sender, EventArgs e)
        {
            DataTable bloodSalesData = ReadExcelAsDataTable("D:\\ServiceDataFixFiles\\Blood 3.21 All Data.xlsx");
            DataTable bloodServiceFixData = ReadExcelAsDataTable("D:\\ServiceDataFixFiles\\Blood Practice Name Mismatches.xlsx");

            DataTable fixedServiceData = new DataTable();
            fixedServiceData = bloodSalesData.Clone();


            foreach (DataRow salesRow in bloodSalesData.Rows)
            {
                string _practiceName = salesRow["PracticeName"].ToString().Trim();
                string _valueToReplaceWith = string.Empty;

                foreach (DataRow serviceFixDataRow in bloodServiceFixData.Rows)
                {
                    if (_practiceName == serviceFixDataRow["LaurensAccounts"].ToString().Trim())
                    {
                        string _reference = serviceFixDataRow[1].ToString().Trim();
                        foreach (DataRow item in bloodServiceFixData.Rows)
                        {
                            if (_reference == item[4].ToString().Trim())
                            {
                                _valueToReplaceWith = item[3].ToString().Trim();
                                break;
                            }
                        }
                    }
                }
                if (!string.IsNullOrEmpty(_valueToReplaceWith))
                    salesRow["PracticeName"] = _valueToReplaceWith;
                fixedServiceData.Rows.Add(salesRow.ItemArray);
            }

            //foreach (DataRow serviceFixDataRow in bloodServiceFixData.Rows)
            //{
            //    string _reference = serviceFixDataRow[1].ToString().Trim();
            //    string _valueToReplaceWith = string.Empty;
            //    foreach (DataRow item in bloodServiceFixData.Rows)
            //    {
            //        if (_reference == item[4].ToString().Trim())
            //        {
            //            _valueToReplaceWith = item[3].ToString().Trim();
            //            goto exitLoop;
            //        }
            //    }
            //exitLoop:

            //    foreach (DataRow salesRow in bloodSalesData.Rows)
            //    {
            //        if (salesRow["PracticeName"].ToString().Trim() == serviceFixDataRow["LaurensAccounts"].ToString().Trim())
            //        {
            //            salesRow["PracticeName"] = _valueToReplaceWith;
            //        }
            //    }
            //}

            //DataTable fixedServiceData1 = new DataTable();
            //fixedServiceData = bloodSalesData.Clone();

            ////Load Provider names and practice namesto a collection from db (make sure all accounts are imported to db) - dbcollection

            //using (var dbEntities = new CareConnectCrmEntities())
            //{
            //    //Loop through rows in excelDataTable
            //    foreach (DataRow sourceRow in bloodSalesData.Rows)
            //    {
            //        DataRow fixedRow = fixedServiceData.NewRow();

            //        bool isExist = dbEntities.Practices.Any(a => a.PracticeName == sourceRow["PracticeName"] && a.PracticeProviderMappers.Any(b => b.Provider.NPI == sourceRow["ProviderNpi"]));
            //        //check existance of provider name and practce name  in db collection
            //        if (isExist)
            //        {
            //            fixedServiceData.Rows.Add(sourceRow);
            //        }
            //        else
            //        {
            //            //cmpare using algorithm below

            //            //if match replace with name in db

            //            //sourceRow["provider"] = db.Provider.NAme
            //        }

            //        //foreach (DataColumn dcol in excelData.Columns)
            //        //{
            //        //    fixedRow[dcol.ColumnName] = sourceRow[dcol.ColumnName];
            //        //}

            //        //fixedServiceData.Rows.Add(fixedRow);
            //    }
            //}

            ExportDataTableToExcel(bloodSalesData);
        }

        /// <summary>
        /// This method takes DataSet as input paramenter and it exports the same to excel
        /// </summary>
        /// <param name="ds"></param>
        private void ExportDataTableToExcel(DataTable table)
        {
            //Creae an Excel application instance
            Microsoft.Office.Interop.Excel.Application excelApp = new Microsoft.Office.Interop.Excel.Application();

            //Create an Excel workbook instance and open it from the predefined location
            Microsoft.Office.Interop.Excel.Workbook excelWorkBook = excelApp.Workbooks.Open(@"E:\BloodFileFinal2.xlsx");

            //foreach (DataTable table in ds.Tables)
            //{
            //Add a new worksheet to workbook with the Datatable name
            Microsoft.Office.Interop.Excel.Worksheet excelWorkSheet = excelWorkBook.Sheets.Add();
            excelWorkSheet.Name = table.TableName;

            for (int i = 1; i < table.Columns.Count + 1; i++)
            {
                excelWorkSheet.Cells[1, i] = table.Columns[i - 1].ColumnName;
            }

            for (int j = 0; j < table.Rows.Count; j++)
            {
                for (int k = 0; k < table.Columns.Count; k++)
                {
                    excelWorkSheet.Cells[j + 2, k + 1] = table.Rows[j].ItemArray[k].ToString();
                }
            }
            //}

            excelWorkBook.Save();
            excelWorkBook.Close();
            excelApp.Quit();

        }

        public DataTable ReadExcelAsDataTable(string filePath)
        {
            bool firstRowIsHeader = true;
            System.Data.DataTable excelData = new System.Data.DataTable("RowItem");
            DataSet ds = new DataSet("Parent");
            ds.Tables.Add(excelData);
            DataRow dr;

            using (Spreadsheet document = new Spreadsheet())
            {
                document.LoadFromFile(filePath);
                Worksheet oSheet = document.Workbook.Worksheets[0];
                int jValue = oSheet.UsedRangeColumnMax;
                int iValue = oSheet.UsedRangeRowMax;

                for (int j = 0; j <= jValue; j++)
                {
                    excelData.Columns.Add("column" + j, System.Type.GetType("System.String"));
                }

                for (int i = 0; i <= iValue; i++)
                {
                    dr = excelData.NewRow();
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
                                goto exitLoop;
                            }
                        }

                        if (firstRowIsHeader && i == 0)
                            excelData.Columns[j].Caption = strValue;
                        else
                            dr["column" + j] = strValue;
                    }

                    if (!(firstRowIsHeader && i == 0))
                        excelData.Rows.Add(dr);
                }

            exitLoop:

                if (firstRowIsHeader)
                {
                    for (int k = 0; k < excelData.Columns.Count; k++)
                    {
                        excelData.Columns[k].ColumnName = (excelData.Columns[k].Caption.Replace(" ", "").Replace("#", ""));
                    }
                }
            }
            return excelData;
        }

        public int Compute(string s, string t)
        {
            int n = s.Length;
            int m = t.Length;
            int[,] d = new int[n + 1, m + 1];

            // Step 1
            if (n == 0)
            {
                return m;
            }

            if (m == 0)
            {
                return n;
            }

            // Step 2
            for (int i = 0; i <= n; d[i, 0] = i++)
            {
            }

            for (int j = 0; j <= m; d[0, j] = j++)
            {
            }

            // Step 3
            for (int i = 1; i <= n; i++)
            {
                //Step 4
                for (int j = 1; j <= m; j++)
                {
                    // Step 5
                    int cost = (t[j - 1] == s[i - 1]) ? 0 : 1;

                    // Step 6
                    d[i, j] = Math.Min(
                        Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
                        d[i - 1, j - 1] + cost);
                }
            }
            // Step 7
            return d[n, m];
        }

    }
}
