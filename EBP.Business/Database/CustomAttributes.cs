using EBP.Business.Helpers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EBP.Business.Database
{
    public partial class Provider
    {
        public int AddressIndex { get; set; }
    }
    public partial class PracticeProviderLocationMapper
    {
        public int AddressIndex { get; set; }
    }
    public partial class PracticeProviderMapper
    {
        public int AddressIndex { get; set; }
    }
    public partial class Address
    {
        public int AddressIndex { get; set; }
    }
    public partial class ReportMaster
    {

        public string LocationId { get; set; }

        public bool HasPatientFirstNameColumn { get; set; }

        public bool HasPatientLastNameColumn { get; set; }

        public bool HasSpecimenCollectionDateColumn { get; set; }

        public bool HasSpecimenReceivedDateColumn { get; set; }

        public bool HasReportedDateColumn { get; set; }

        public bool HasPracticeNameColumn { get; set; }

        public bool HasProviderFirstNameColumn { get; set; }

        public bool HasProviderLastNameColumn { get; set; }

        public bool HasProviderNpiColumn { get; set; }

        public bool HasRepFirstNameColumn { get; set; }

        public bool HasRepLastNameColumn { get; set; }

        public bool HasPatientIdColumn { get; set; }

        public bool HasLocationIdColumn { get; set; }

        public bool HasSpecimenIdColumn { get; set; }

        public List<Database.SalesImportMessage> ColumnsMissingMessages = new List<SalesImportMessage>();

        public ExcelColumn ObjPatientFirstName
        {
            set
            {
                this.PatientFirstName = value.Value;
                this.HasPatientFirstNameColumn = value.HasColumn;
                if (!string.IsNullOrEmpty(value.NodeName) && !value.HasColumn)
                    ColumnsMissingMessages.Add(new Database.SalesImportMessage
                    {
                        NodeName = value.NodeName,
                        Message = "First Name column is missing in excel file",
                        IsFinance = false,
                        IsTechnical = true
                    });
            }
        }

        public ExcelColumn ObjPatientLastName
        {
            set
            {
                this.PatientLastName = value.Value;
                this.HasPatientLastNameColumn = value.HasColumn;
                if (!string.IsNullOrEmpty(value.NodeName) && !value.HasColumn)
                    ColumnsMissingMessages.Add(new Database.SalesImportMessage
                    {
                        NodeName = value.NodeName,
                        Message = "Last Name column is missing in excel file",
                        IsFinance = false,
                        IsTechnical = true
                    });
            }
        }

        public ExcelColumn ObjSpecimenCollectionDate
        {
            set
            {
                this.SpecimenCollectionDate = value.Value.ToDateTime();
                this.HasSpecimenCollectionDateColumn = value.HasColumn;
                if (!string.IsNullOrEmpty(value.NodeName) && !value.HasColumn)
                    ColumnsMissingMessages.Add(new Database.SalesImportMessage
                    {
                        NodeName = value.NodeName,
                        Message = "Specimen Collection Date column is missing in excel file",
                        IsFinance = false,
                        IsTechnical = true
                    });
            }
        }

        public ExcelColumn ObjSpecimenReceivedDate
        {
            set
            {
                this.SpecimenReceivedDate = value.Value.ToDateTime();
                this.HasSpecimenReceivedDateColumn = value.HasColumn;
                if (!string.IsNullOrEmpty(value.NodeName) && !value.HasColumn)
                    ColumnsMissingMessages.Add(new Database.SalesImportMessage
                    {
                        NodeName = value.NodeName,
                        Message = "Specimen Received Date column is missing in excel file",
                        IsFinance = false,
                        IsTechnical = true
                    });
            }
        }

        public ExcelColumn ObjReportedDate
        {
            set
            {
                this.ReportedDate = value.Value.ToDateTime();
                this.HasReportedDateColumn = value.HasColumn;
                if (!string.IsNullOrEmpty(value.NodeName) && !value.HasColumn)
                    ColumnsMissingMessages.Add(new Database.SalesImportMessage
                    {
                        NodeName = value.NodeName,
                        Message = "Reported Date column is missing in excel file",
                        IsFinance = false,
                        IsTechnical = true
                    });
            }
        }

        ExcelColumn _ObjPracticeName;
        public ExcelColumn ObjPracticeName
        {
            get
            {
                return _ObjProviderFirstName;
            }
            set
            {
                this.PracticeName = value.Value;
                this.HasPracticeNameColumn = value.HasColumn;
                _ObjPracticeName = value;
                if (!string.IsNullOrEmpty(value.NodeName) && !value.HasColumn)
                    ColumnsMissingMessages.Add(new Database.SalesImportMessage
                    {
                        NodeName = value.NodeName,
                        Message = "First Name column is missing in excel file",
                        IsFinance = false,
                        IsTechnical = true
                    });
            }
        }

        ExcelColumn _ObjProviderFirstName;
        public ExcelColumn ObjProviderFirstName
        {
            get
            {
                return _ObjProviderFirstName;
            }
            set
            {
                this.ProviderFirstName = value.Value;
                this.HasProviderFirstNameColumn = value.HasColumn;
                _ObjProviderFirstName = value;
                if (!string.IsNullOrEmpty(value.NodeName) && !value.HasColumn)
                    ColumnsMissingMessages.Add(new Database.SalesImportMessage
                    {
                        NodeName = value.NodeName,
                        Message = "Provider First Name column is missing in excel file",
                        IsFinance = false,
                        IsTechnical = true
                    });
            }
        }

        ExcelColumn _ObjProviderLastName;
        public ExcelColumn ObjProviderLastName
        {
            get
            {
                return _ObjProviderLastName;
            }
            set
            {
                this.ProviderLastName = value.Value;
                this.HasProviderLastNameColumn = value.HasColumn;
                _ObjProviderLastName = value;
                if (!string.IsNullOrEmpty(value.NodeName) && !value.HasColumn)
                    ColumnsMissingMessages.Add(new Database.SalesImportMessage
                    {
                        NodeName = value.NodeName,
                        Message = "Provider Last Name column is missing in excel file",
                        IsFinance = false,
                        IsTechnical = true
                    });
            }
        }

        ExcelColumn _ObjProviderNpi;
        public ExcelColumn ObjProviderNpi
        {
            get
            {
                return _ObjProviderNpi;
            }
            set
            {
                this.ProviderNpi = value.Value;
                this.HasProviderNpiColumn = value.HasColumn;
                _ObjProviderNpi = value;
                if (!string.IsNullOrEmpty(value.NodeName) && !value.HasColumn)
                    ColumnsMissingMessages.Add(new Database.SalesImportMessage
                    {
                        NodeName = value.NodeName,
                        Message = "Provider NPI column is missing in excel file",
                        IsFinance = false,
                        IsTechnical = true
                    });
            }
        }

        public ExcelColumn ObjRepFirstName
        {
            set
            {
                this.RepFirstName = value.Value;
                this.HasRepFirstNameColumn = value.HasColumn;
                if (!string.IsNullOrEmpty(value.NodeName) && !value.HasColumn)
                    ColumnsMissingMessages.Add(new Database.SalesImportMessage
                    {
                        NodeName = value.NodeName,
                        Message = "Rep First Name column is missing in excel file",
                        IsFinance = false,
                        IsTechnical = true
                    });
            }
        }

        public ExcelColumn ObjRepLastName
        {
            set
            {
                this.RepLastName = value.Value;
                this.HasRepLastNameColumn = value.HasColumn;
                if (!string.IsNullOrEmpty(value.NodeName) && !value.HasColumn)
                    ColumnsMissingMessages.Add(new Database.SalesImportMessage
                    {
                        NodeName = value.NodeName,
                        Message = "Rep Last Name column is missing in excel file",
                        IsFinance = false,
                        IsTechnical = true
                    });
            }
        }

        public ExcelColumn ObjPatientId
        {
            set
            {
                this.PatientId = value.Value.ToInt() ?? 0;
                this.HasPatientIdColumn = value.HasColumn;
                if (!string.IsNullOrEmpty(value.NodeName) && !value.HasColumn)
                    ColumnsMissingMessages.Add(new Database.SalesImportMessage
                    {
                        NodeName = value.NodeName,
                        Message = "Patient ID column is missing in excel file",
                        IsFinance = false,
                        IsTechnical = true
                    });
            }
        }

        public ExcelColumn ObjLocationId
        {
            set
            {
                this.LocationId = value.Value;
                this.HasLocationIdColumn = value.HasColumn;
                if (!string.IsNullOrEmpty(value.NodeName) && !value.HasColumn)
                    ColumnsMissingMessages.Add(new Database.SalesImportMessage
                    {
                        NodeName = value.NodeName,
                        Message = "Location ID column is missing in excel file",
                        IsFinance = false,
                        IsTechnical = true
                    });
            }
        }

        public ExcelColumn ObjSpecimenId
        {
            set
            {
                this.SpecimenId = value.Value;
                this.HasSpecimenIdColumn = value.HasColumn;
                if (!string.IsNullOrEmpty(value.NodeName) && !value.HasColumn)
                    ColumnsMissingMessages.Add(new Database.SalesImportMessage
                    {
                        NodeName = value.NodeName,
                        Message = "Specimen ID column is missing in excel file",
                        IsFinance = false,
                        IsTechnical = true
                    });
            }
        }

        public List<string> ReportLogMessages
        {
            get
            {
                List<string> _errorMessaages = new List<string>();
                if (this.PracticeId == null || this.PracticeId <= 0)
                    _errorMessaages.Add("Practice not resolved");
                if (this.ProviderId == null || this.ProviderId <= 0)
                    _errorMessaages.Add("Provider not resolved");
                if (this.RepId == null || this.RepId <= 0)
                    _errorMessaages.Add("Rep not resolved");

                if (this.LogMessages.Count > 0)
                    _errorMessaages = LogMessages.Concat(_errorMessaages).ToList();

                return _errorMessaages;
            }
        }

        private List<string> _logMessages;
        public List<string> LogMessages
        {
            private get
            {
                return _logMessages;
            }
            set
            {
                _logMessages = value;
            }
        }

        public int NumberOfPractices { get; set; }
    }

    public partial class ReportFinance
    {
        public string LocationId { get; set; }

        public bool HasBilledDateColumn { get; set; }

        public bool HasPaidDateColumn { get; set; }

        public bool HasChargesColumn { get; set; }

        public bool HasPaidAmountColumn { get; set; }

        public bool HasAdjustAmountColumn { get; set; }

        public bool HasAdjustReasonColumn { get; set; }

        public List<Database.SalesImportMessage> ColumnsMissingMessages = new List<SalesImportMessage>();

        public ExcelColumn ObjBilledDate
        {
            set
            {
                this.BilledDate = value.Value.ToDateTime();
                this.HasBilledDateColumn = value.HasColumn;
                if (!string.IsNullOrEmpty(value.NodeName) && !value.HasColumn)
                    ColumnsMissingMessages.Add(new Database.SalesImportMessage
                    {
                        NodeName = value.NodeName,

                        Message = "Billed Date column is missing in excel file",
                        IsFinance = false,
                        IsTechnical = true
                    });
            }
        }

        public ExcelColumn ObjPaidDate
        {
            set
            {
                this.PaidDate = value.Value.ToDateTime();
                this.HasPaidDateColumn = value.HasColumn;
                if (!string.IsNullOrEmpty(value.NodeName) && !value.HasColumn)
                    ColumnsMissingMessages.Add(new Database.SalesImportMessage
                    {
                        NodeName = value.NodeName,
                        Message = "Paid Date column is missing in excel file",
                        IsFinance = false,
                        IsTechnical = true
                    });
            }
        }

        public ExcelColumn ObjCharges
        {
            set
            {
                this.Charges = value.Value.ToFloat();
                this.HasChargesColumn = value.HasColumn;
                if (!string.IsNullOrEmpty(value.NodeName) && !value.HasColumn)
                    ColumnsMissingMessages.Add(new Database.SalesImportMessage
                    {
                        NodeName = value.NodeName,
                        Message = "Charges column is missing in excel file",
                        IsFinance = false,
                        IsTechnical = true
                    });
            }
        }

        public ExcelColumn ObjPaidAmount
        {
            set
            {
                this.PaidAmount = value.Value.ToFloat();
                this.HasPaidAmountColumn = value.HasColumn;
                if (!string.IsNullOrEmpty(value.NodeName) && !value.HasColumn)
                    ColumnsMissingMessages.Add(new Database.SalesImportMessage
                    {
                        NodeName = value.NodeName,
                        Message = "Paid Amount column is missing in excel file",
                        IsFinance = false,
                        IsTechnical = true
                    });
            }
        }

        public ExcelColumn ObjAdjustAmount
        {
            set
            {
                this.AdjustAmount = value.Value.ToFloat();
                this.HasAdjustAmountColumn = value.HasColumn;
                if (!string.IsNullOrEmpty(value.NodeName) && !value.HasColumn)
                    ColumnsMissingMessages.Add(new Database.SalesImportMessage
                    {
                        NodeName = value.NodeName,
                        Message = "Adjust Amount column is missing in excel file",
                        IsFinance = false,
                        IsTechnical = true
                    });
            }
        }

        public ExcelColumn ObjAdjustReason
        {
            set
            {
                this.AdjustReason = value.Value;
                this.HasAdjustReasonColumn = value.HasColumn;
                if (!string.IsNullOrEmpty(value.NodeName) && !value.HasColumn)
                    ColumnsMissingMessages.Add(new Database.SalesImportMessage
                    {
                        NodeName = value.NodeName,
                        Message = "Adjust Reason column is missing in excel file",
                        IsFinance = false,
                        IsTechnical = true
                    });
            }
        }

    }

    public static class Helpers
    {
        /// <summary>
        /// Integer Parser to manage non numeric input
        /// </summary>
        /// <param name="val">Value to parse to integer</param>
        /// <returns>returns an integer value even if the input value is a non numeric data</returns>
        public static int? ToInt(this string val)
        {
            int i;
            return int.TryParse(val, out i) ? (int?)i : null;
        }

        /// <summary>
        /// Float Parser to manage non numeric input
        /// </summary>
        /// <param name="val">Value to parse to float</param>
        /// <returns>returns a float value even if the input value is a non numeric data</returns>
        public static float? ToFloat(this string val)
        {
            float i;
            if (!string.IsNullOrEmpty(val))
                val = Regex.Replace(val, "[ ](?=[ ])|[^-.,A-Za-z0-9 ]+", "");
            return float.TryParse(val, out i) ? (float?)i : null;
        }

        /// <summary>
        /// Date Parser
        /// </summary>
        /// <param name="date">data as string</param>
        /// <returns>return back a nullable date, If input value is not a valid date string the method will return null</returns>
        public static DateTime? ToDateTime(this string date)
        {
            if (string.IsNullOrEmpty(date))
                return null;
            else
            {
                string[] dateFormats = { "yyyyMMddHHmmss",
                                           "yyyyMMddHHmm",
                                           "dd-MM-yyyy",
                                           "MM/dd/yyyy",
                                           "M/d/yyyy",
                                           "MM/dd/yy",
                                           "M/d/yy",
                                           "M/d/yyyy HH:mm",
                                           "M/d/yy H:mm",
                                           "M/d/yyyy H:mm",
                                           "M-d-yy H:mm",
                                           "M-d-yy",
                                           "M-d-yyyy H:mm",
                                           "M-d-yyyy",
                                           "yyyy-MM-dd",
                                           "dd-MM-yyyy HH:mm",
                                           "dd-MM-yyyy HH:mm:ss",
                                           "yyyy-MM-dd HH:mm:ss",
                                           "dd-MMM" };
                DateTime dt;
                if (DateTime.TryParseExact(date.Replace("+", " +"), dateFormats, CultureInfo.GetCultureInfo("en-US"), DateTimeStyles.None, out dt))
                    return dt;
                else
                    return null;
            }
        }
    }
}
