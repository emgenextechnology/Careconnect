using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace EBP.DataMigration
{
    public partial class Datamigrator : Form
    {
        private BackgroundWorker myWorker = new BackgroundWorker();
        StringBuilder sb = new StringBuilder();//Declare a new string builder to store the result.

        public Datamigrator()
        {
            InitializeComponent();

            myWorker.DoWork += new DoWorkEventHandler(myWorker_DoWork);
            myWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(myWorker_RunWorkerCompleted);
            myWorker.ProgressChanged += new ProgressChangedEventHandler(myWorker_ProgressChanged);
            myWorker.WorkerReportsProgress = true;
            myWorker.WorkerSupportsCancellation = true;
        }

        private void FnDataMigrator(int tableIndex, int businessId)
        {

            string ErrorLog = string.Empty;

            var dbNewEntities = new Database.New.CRMStagingEntities();
            var dbOldEntities = new Database.Old.EPMDevEntities();
            var oldUsers = dbOldEntities.Users.ToList();

            switch (tableIndex)
            {
                case 31:// Exceptional Case 

                    #region AddressFix

                    try
                    {
                        using (dbNewEntities = new Database.New.CRMStagingEntities())
                        {
                            var objUsers = dbNewEntities.Users.ToList();

                            foreach (var item in objUsers)
                            {
                                var objOldUser = item.OldId.HasValue ? oldUsers.FirstOrDefault(a => a.UserId == item.OldId) : null;
                                if (objOldUser != null)
                                {
                                    int? oldAddressId = objOldUser.Address_Id;

                                    using (dbOldEntities = new Database.Old.EPMDevEntities())
                                    {
                                        var objLocation = oldAddressId.HasValue ? dbOldEntities.tblLocations.FirstOrDefault(a => a.Address_Id == oldAddressId) : null;
                                        int stateId = dbNewEntities.LookupStates.FirstOrDefault(a => a.OldId == objLocation.State_Id).Id;

                                        var objUserProfile = item.UserProfiles.FirstOrDefault();
                                        if (objUserProfile != null)
                                        {
                                            objUserProfile.AddressLine1 = objLocation != null ? objLocation.Line_1 : null;
                                            objUserProfile.AddressLine2 = objLocation != null ? objLocation.Line_2 : null;
                                            objUserProfile.City = objLocation != null ? objLocation.City : null;
                                            objUserProfile.StateId = stateId;
                                            objUserProfile.Zip = objLocation != null ? objLocation.Zip : null;
                                        }
                                    }
                                }
                            }

                            dbNewEntities.SaveChanges();
                            sb.Append(string.Format("'User Profile' Table Successfully Updated.") + Environment.NewLine);
                        }
                    }
                    catch (Exception ex)
                    {
                        sb.Append("'User Profile for Address'" + Environment.NewLine);
                        sb.Append(string.Format(ex.Message) + Environment.NewLine);
                        if (ex.InnerException != null) sb.Append(string.Format(ex.InnerException.Message) + Environment.NewLine);
                    }


                    #endregion

                    break;

                case 1:

                    #region Users

                    try
                    {
                        foreach (var item in oldUsers)
                        {
                            using (dbNewEntities = new Database.New.CRMStagingEntities())
                            {
                                string username = item.Email;
                                // businessId + "-" + item.UserName;
                                var objuser = dbNewEntities.Users.FirstOrDefault(a => (a.OldId == item.UserId || a.UserName == username) && a.BusinessId == businessId);
                                bool isExists = (objuser != null);
                                var objUser = dbNewEntities.Users.FirstOrDefault(a => a.OldId == item.CreatedBy);
                                int createdBy = objUser != null ? objUser.Id : 1;

                                var objAddress = dbOldEntities.Addresses.FirstOrDefault(a => a.AddressId == item.Address_Id);
                                string addressLine1 = null, addressLine2 = null, city = null, zip = null;
                                int? stateId = null;

                                if (objAddress != null)
                                {
                                    addressLine1 = objAddress.Line1;
                                    addressLine2 = objAddress.Line2;
                                    city = objAddress.City;
                                    zip = objAddress.Zip;

                                    var objState = dbNewEntities.LookupStates.FirstOrDefault(a => a.OldId == objAddress.State);
                                    stateId = objState != null ? (int?)objState.Id : null;
                                }

                                if (!isExists)
                                    dbNewEntities.Users.Add(new Database.New.User
                                    {
                                        BusinessId = businessId,
                                        FirstName = item.FirstName,
                                        MiddleName = item.Middle_Initial,
                                        LastName = item.LastName,
                                        UserName = username,
                                        PasswordHash = "AP3mION9ko8ipOeHV6b3PANj2Bmhngo4tHaOjofK8+/3mdSabaZ66RAcoDYxvGWrwQ==",
                                        SecurityStamp = "9bc825eb-5d21-43ef-a831-c4d11add08ab",
                                        Email = item.Email,
                                        PhoneNumber = item.Phone_Number,
                                        IsActive = item.Status_Id == 1 ? true : false,
                                        EmailConfirmed = false,
                                        PhoneNumberConfirmed = false,
                                        TwoFactorEnabled = false,
                                        LockoutEnabled = true,
                                        AccessFailedCount = 0,
                                        OldId = item.UserId,
                                        UserProfiles = new List<Database.New.UserProfile> { 
                                        new Database.New.UserProfile {
                                            Startdate = item.Start_Date,
                                            WorkEmail = item.Work_Email,
                                            HomePhone = item.Home_Phone,
                                            AdditionalPhone = item.Phone_Number,
                                            AddressLine1 = addressLine1,
                                            AddressLine2 = addressLine2,
                                            City = city,
                                            StateId = stateId,
                                            Zip = zip,
                                            CreatedOn = item.CreatedOn,
                                            CreatedBy = createdBy
                                        }
                                    }
                                    });
                                else if (objuser.OldId != item.UserId)
                                {
                                    objuser.OldId = item.UserId;
                                }
                                else
                                {
                                    ErrorLog = string.Format("User already Exists with Username : {0}", objuser.UserName) + Environment.NewLine;
                                    sb.Append(ErrorLog);//Append the result to the string builder
                                }
                                dbNewEntities.SaveChanges();
                            }
                        }
                        sb.Append(string.Format("'User' Table Successfully Imported.") + Environment.NewLine);
                    }

                    catch (Exception ex)
                    {
                        sb.Append("'User'" + Environment.NewLine);
                        sb.Append(string.Format(ex.Message) + Environment.NewLine);
                        if (ex.InnerException != null) sb.Append(string.Format(ex.InnerException.Message) + Environment.NewLine);
                    }

                    #endregion

                    break;

                case 2:

                    #region Departments

                    try
                    {
                        var oldDepartments = dbOldEntities.tblGroups.ToList();

                        foreach (var item in oldDepartments)
                        {
                            var objDepartments = dbNewEntities.Departments.FirstOrDefault(a => a.OldId == item.Group_Id && a.BusinessId == businessId);
                            bool isExists = (objDepartments != null);
                            var objUser = dbNewEntities.Users.FirstOrDefault(a => a.OldId == item.Created_By);
                            var objUserGroup = dbOldEntities.Users.FirstOrDefault(a => a.Group_Id == item.Group_Id);
                            int createdBy = objUser != null ? objUser.Id : 1;

                            if (!isExists && objUserGroup != null)
                                dbNewEntities.Departments.Add(new Database.New.Department
                                {
                                    BusinessId = businessId,
                                    CreatedBy = createdBy,
                                    CreatedOn = item.Created_On == null ? DateTime.UtcNow : item.Created_On.Value,
                                    DepartmentName = item.Group_Name,
                                    Description = item.Description,
                                    IsActive = item.Status_Id == 1 ? true : false,
                                    OldId = item.Group_Id,
                                    UserDepartments = new List<Database.New.UserDepartment> { 
                            new Database.New.UserDepartment{
                                CreatedBy = createdBy,
                                CreatedOn = item.Created_On == null ? DateTime.UtcNow : item.Created_On.Value,
                                UserId = dbNewEntities.Users.FirstOrDefault(a=>a.OldId==objUserGroup.UserId).Id
                            }
                        }
                                });
                        }

                        dbNewEntities.SaveChanges();
                        sb.Append(string.Format("'Departments' Table Successfully Imported.") + Environment.NewLine);
                    }
                    catch (Exception ex)
                    {
                        sb.Append("'Departments'" + Environment.NewLine);
                        sb.Append(string.Format(ex.Message) + Environment.NewLine);
                        if (ex.InnerException != null) sb.Append(string.Format(ex.InnerException.Message) + Environment.NewLine);
                    }

                    #endregion

                    break;

                case 3:

                    #region Roles

                    try
                    {
                        var oldRoles = dbOldEntities.Roles.ToList();

                        foreach (var item in oldRoles)
                        {
                            var objRoles = dbNewEntities.Roles.FirstOrDefault(a => a.OldId == item.Role_Id && a.BusinessId == businessId);
                            bool isExists = (objRoles != null);
                            var objUser = dbNewEntities.Users.FirstOrDefault(a => a.OldId == item.Created_By);
                            int createdBy = objUser != null ? objUser.Id : 1;

                            if (!isExists)
                                dbNewEntities.Roles.Add(new Database.New.Role
                                {
                                    BusinessId = businessId,
                                    CreatedBy = createdBy,
                                    CreatedOn = item.Created_On == null ? DateTime.UtcNow : item.Created_On.Value,
                                    Name = item.Role_Name,
                                    Description = item.Description,
                                    IsActive = item.Status_Id == 1 ? true : false,
                                    OldId = item.Role_Id
                                });
                        }

                        dbNewEntities.SaveChanges();
                        sb.Append(string.Format("'Roles' Table Successfully Imported.") + Environment.NewLine);
                    }
                    catch (Exception ex)
                    {
                        sb.Append("'Roles'" + Environment.NewLine);
                        sb.Append(string.Format(ex.Message) + Environment.NewLine);
                        if (ex.InnerException != null) sb.Append(string.Format(ex.InnerException.Message) + Environment.NewLine);
                    }

                    #endregion

                    break;

                case 4:

                    #region UserRoles

                    try
                    {
                        foreach (var item in oldUsers)
                        {
                            var user = dbNewEntities.Users.Where(a => a.OldId == item.UserId && a.BusinessId == businessId).FirstOrDefault();
                            if (user != null)
                            {
                                var role = dbNewEntities.Roles.FirstOrDefault(a => a.OldId == item.Role_Id);
                                if (role != null)
                                {
                                    user.Roles.Add(role);
                                }
                            }
                        }

                        dbNewEntities.SaveChanges();
                        sb.Append(string.Format("'User Roles' Table Successfully Imported.") + Environment.NewLine);
                    }
                    catch (Exception ex)
                    {
                        sb.Append("'User Roles'" + Environment.NewLine);
                        sb.Append(string.Format(ex.Message) + Environment.NewLine);
                        if (ex.InnerException != null) sb.Append(string.Format(ex.InnerException.Message) + Environment.NewLine);
                    }

                    #endregion

                    break;

                case 5:

                    #region LookupDegree

                    try
                    {
                        var oldDegrees = dbOldEntities.Degrees.ToList();

                        foreach (var item in oldDegrees)
                        {
                            var objDegrees = dbNewEntities.LookupDegrees.FirstOrDefault(a => a.OldId == item.DegreeId);
                            bool isExists = (objDegrees != null);
                            var objUser = dbNewEntities.Users.FirstOrDefault(a => a.OldId == item.CreatedBy);
                            int createdBy = objUser != null ? objUser.Id : 1;

                            if (!isExists)
                                dbNewEntities.LookupDegrees.Add(new Database.New.LookupDegree
                                {
                                    CreatedBy = createdBy,
                                    CreatedOn = item.CreatedOn,
                                    DegreeName = item.DegreeName,
                                    IsActive = item.IsActive,
                                    OldId = item.DegreeId
                                });
                        }

                        dbNewEntities.SaveChanges();
                        sb.Append(string.Format("'LookupDegree' Table Successfully Imported.") + Environment.NewLine);
                    }
                    catch (Exception ex)
                    {
                        sb.Append("'LookupDegree'" + Environment.NewLine);
                        sb.Append(string.Format(ex.Message) + Environment.NewLine);
                        if (ex.InnerException != null) sb.Append(string.Format(ex.InnerException.Message) + Environment.NewLine);
                    }

                    #endregion

                    break;

                case 6:

                    #region Providers

                    try
                    {
                        var oldProvider = dbOldEntities.Providers.ToList();

                        foreach (var item in oldProvider)
                        {
                            var objProviders = dbNewEntities.Providers.FirstOrDefault(a => a.OldId == item.ProviderId);
                            bool isExists = (objProviders != null);
                            var objUser = dbNewEntities.Users.FirstOrDefault(a => a.OldId == item.CreatedBy);
                            var objDegree = dbNewEntities.LookupDegrees.FirstOrDefault(a => a.OldId == item.Degree);
                            int createdBy = objUser != null ? objUser.Id : 1;

                            if (!isExists)
                            {
                                int newProviderId = 0;
                                var objNewProvider = dbNewEntities.Providers.FirstOrDefault(a => a.NPI == item.NPI);
                                if (objNewProvider != null)
                                {
                                    newProviderId = objNewProvider.Id;
                                }
                                else
                                {
                                    var objProvider = dbNewEntities.Providers.Add(new Database.New.Provider
                                    {
                                        CreatedBy = createdBy,
                                        CreatedOn = item.CreateOn,
                                        IsActive = item.IsActive,
                                        FirstName = item.FirstName,
                                        MiddleName = item.MiddleName,
                                        LastName = item.LastName,
                                        NPI = item.NPI,
                                        DegreeId = objDegree != null ? (int?)objDegree.Id : null,
                                        ProviderEmail = item.ProviderEmail,
                                        OldId = item.ProviderId,
                                    });

                                    if (dbNewEntities.SaveChanges() > 0)
                                    {
                                        newProviderId = objProvider.Id;
                                    }
                                }

                                if (newProviderId > 0)
                                {
                                    dbNewEntities.TempProviderMigrationDatas.Add(new Database.New.TempProviderMigrationData
                                    {
                                        NewId = newProviderId,
                                        NPI = item.NPI,
                                        OldId = item.ProviderId
                                    });
                                    dbNewEntities.SaveChanges();
                                }
                            }
                        }

                        sb.Append(string.Format("'Providers' Table Successfully Imported.") + Environment.NewLine);
                    }
                    catch (Exception ex)
                    {
                        sb.Append("'Providers'" + Environment.NewLine);
                        sb.Append(string.Format(ex.Message) + Environment.NewLine);
                        if (ex.InnerException != null) sb.Append(string.Format(ex.InnerException.Message) + Environment.NewLine);
                    }

                    #endregion

                    break;

                case 7:

                    #region LookupPracticeType

                    try
                    {
                        var oldPracticeTypes = dbOldEntities.PracticeTypes.ToList();

                        foreach (var item in oldPracticeTypes)
                        {
                            var objPracticeTypes = dbNewEntities.LookupPracticeTypes.FirstOrDefault(a => a.OldId == item.PracticeTypeId);
                            bool isExists = (objPracticeTypes != null);
                            var objUser = dbNewEntities.Users.FirstOrDefault(a => a.OldId == item.CreatedBy);
                            int createdBy = objUser != null ? objUser.Id : 1;

                            if (!isExists)
                                dbNewEntities.LookupPracticeTypes.Add(new Database.New.LookupPracticeType
                                {
                                    CreatedBy = createdBy,
                                    CreatedOn = item.CreatedOn,
                                    IsActive = item.IsActive,
                                    PracticeType = item.PracticeType1,
                                    OldId = item.PracticeTypeId
                                });
                        }

                        dbNewEntities.SaveChanges();
                        sb.Append(string.Format("'LookupPracticeType' Table Successfully Imported.") + Environment.NewLine);
                    }
                    catch (Exception ex)
                    {
                        sb.Append("'LookupPracticeType'" + Environment.NewLine);
                        sb.Append(string.Format(ex.Message) + Environment.NewLine);
                        if (ex.InnerException != null) sb.Append(string.Format(ex.InnerException.Message) + Environment.NewLine);
                    }

                    #endregion

                    break;

                case 8:

                    #region RepsGroups

                    try
                    {
                        var oldRepGroups = dbOldEntities.RepGroups.ToList();

                        foreach (var item in oldRepGroups)
                        {
                            int oldRepGroupId = 0;
                            int.TryParse(item.RepGroupId, out oldRepGroupId);
                            var objRepGroups = dbNewEntities.RepGroups.FirstOrDefault(a => a.OldId == oldRepGroupId && a.BusinessId == businessId);
                            bool isExists = (objRepGroups != null);
                            var objUser = dbNewEntities.Users.FirstOrDefault(a => a.OldId == item.CreatedBy);
                            int createdBy = objUser != null ? objUser.Id : 1;
                            int businessAdminId = dbNewEntities.Users.Where(a => a.BusinessId == businessId && a.Roles.Any(r => r.Name == "BusinessAdmin")).FirstOrDefault().Id;

                            var objGroupManager = dbNewEntities.Users.FirstOrDefault(a => a.OldId == item.ManagerName);
                            int managerId = objGroupManager == null ? businessAdminId : objGroupManager.Id;

                            if (!isExists)//&& managerId > 0)
                                dbNewEntities.RepGroups.Add(new Database.New.RepGroup
                                {
                                    BusinessId = businessId,
                                    CreatedBy = createdBy,
                                    CreatedOn = item.CreatedOn,
                                    IsActive = item.Status == 1 ? true : false,
                                    RepGroupName = item.RepGroupName,
                                    Description = item.Decription,
                                    ManagerId = managerId,
                                    OldId = oldRepGroupId
                                });
                        }


                        //dbNewEntities.RepGroups.Add(new Database.New.RepGroup
                        //{
                        //    BusinessId = businessId,
                        //    CreatedBy = 1,
                        //    CreatedOn = DateTime.UtcNow,
                        //    IsActive = true,
                        //    RepGroupName = "W2",
                        //    Description = "Emgenex Employees",
                        //    ManagerId = ManagerId,
                        //    OldId = 100001
                        //});

                        //dbNewEntities.RepGroups.Add(new Database.New.RepGroup
                        //{
                        //    BusinessId = businessId,
                        //    CreatedBy = 1,
                        //    CreatedOn = DateTime.UtcNow,
                        //    IsActive = true,
                        //    RepGroupName = "1099",
                        //    Description = "Independent",
                        //    ManagerId = ManagerId,
                        //    OldId = 200002
                        //});

                        dbNewEntities.SaveChanges();
                        sb.Append(string.Format("'RepGroups' Table Successfully Imported.") + Environment.NewLine);
                    }
                    catch (Exception ex)
                    {
                        sb.Append("'RepGroups'" + Environment.NewLine);
                        sb.Append(string.Format(ex.Message) + Environment.NewLine);
                        if (ex.InnerException != null) sb.Append(string.Format(ex.InnerException.Message) + Environment.NewLine);
                    }

                    #endregion

                    break;

                case 9:

                    #region Reps

                    try
                    {
                        var oldReps = dbOldEntities.Reps.ToList();

                        foreach (var item in oldReps)
                        {
                            int oldRepGroupId = 0;
                            int.TryParse(item.RepGroupsId, out oldRepGroupId);
                            var objReps = dbNewEntities.Reps.FirstOrDefault(a => a.OldId == item.RepId);
                            bool isExists = (objReps != null);
                            var objCreatedUser = dbNewEntities.Users.FirstOrDefault(a => a.OldId == item.CreatedBy);
                            var objUser = dbNewEntities.Users.FirstOrDefault(a => a.OldId == item.User_Id);
                            var objRepGroup = dbNewEntities.RepGroups.FirstOrDefault(a => a.OldId == oldRepGroupId);
                            int createdBy = objCreatedUser != null ? objCreatedUser.Id : 1;
                            int userId = objUser != null ? (int)objUser.Id : 0;

                            if (!isExists && objRepGroup != null && userId > 0)
                                dbNewEntities.Reps.Add(new Database.New.Rep
                                {
                                    CreatedBy = createdBy,
                                    CreatedOn = item.CreatedOn,
                                    IsActive = true,
                                    UserId = userId,
                                    RepGroupId = objRepGroup != null ? (int?)objRepGroup.Id : null,
                                    SignonDate = item.SignonDate,
                                    OldId = item.RepId
                                });
                        }

                        dbNewEntities.SaveChanges();
                        sb.Append(string.Format("'Reps' Table Successfully Imported.") + Environment.NewLine);
                    }
                    catch (Exception ex)
                    {
                        sb.Append("'Reps'" + Environment.NewLine);
                        sb.Append(string.Format(ex.Message) + Environment.NewLine);
                        if (ex.InnerException != null) sb.Append(string.Format(ex.InnerException.Message) + Environment.NewLine);
                    }

                    #endregion

                    break;

                case 10:

                    #region LookupPracticeSpecialityType

                    try
                    {
                        var oldPracticeSpecialityType = dbOldEntities.PracticeSpecialityTypes.ToList();

                        foreach (var item in oldPracticeSpecialityType)
                        {
                            var objPracticeSpecialityTypes = dbNewEntities.LookupPracticeSpecialityTypes.FirstOrDefault(a => a.OldId == item.PracticeSpecialityTypeId || a.PracticeSpecialityType == item.PracticeSpecialityType1);
                            bool isExists = (objPracticeSpecialityTypes != null);
                            var objUser = dbNewEntities.Users.FirstOrDefault(a => a.OldId == item.CreatedBy);
                            int createdBy = objUser != null ? objUser.Id : 1;

                            if (!isExists)
                                dbNewEntities.LookupPracticeSpecialityTypes.Add(new Database.New.LookupPracticeSpecialityType
                                {
                                    CreatedBy = createdBy,
                                    CreatedOn = item.CreatedOn,
                                    IsActive = item.IsActive,
                                    PracticeSpecialityType = item.PracticeSpecialityType1,
                                    OldId = item.PracticeSpecialityTypeId
                                });
                            else
                                objPracticeSpecialityTypes.OldId = item.PracticeSpecialityTypeId;
                        }

                        dbNewEntities.SaveChanges();
                        sb.Append(string.Format("'LookupPracticeSpecialityType' Table Successfully Imported.") + Environment.NewLine);
                    }
                    catch (Exception ex)
                    {
                        sb.Append("'LookupPracticeSpecialityType'" + Environment.NewLine);
                        sb.Append(string.Format(ex.Message) + Environment.NewLine);
                        if (ex.InnerException != null) sb.Append(string.Format(ex.InnerException.Message) + Environment.NewLine);
                    }

                    #endregion

                    break;

                case 11:

                    #region LookupEnrolledServices

                    try
                    {
                        var oldEnrolledServices = dbOldEntities.Services.ToList();

                        foreach (var item in oldEnrolledServices)
                        {
                            var objEnrolledServices = dbNewEntities.LookupEnrolledServices.FirstOrDefault(a => a.OldId == item.ServiceId || a.ServiceName == item.ServiceName && a.BusinessId == businessId);
                            bool isExists = (objEnrolledServices != null);
                            var objUser = dbNewEntities.Users.FirstOrDefault(a => a.OldId == item.CreatedBy);
                            int createdBy = objUser != null ? objUser.Id : 1;

                            if (!isExists)
                                dbNewEntities.LookupEnrolledServices.Add(new Database.New.LookupEnrolledService
                                {
                                    BusinessId = businessId,
                                    ServiceDecription = item.ServiceDecription,
                                    CreatedBy = createdBy,
                                    CreatedOn = item.CreatedOn,
                                    IsActive = item.Status,
                                    ServiceName = item.ServiceName,
                                    OldId = item.ServiceId
                                });
                            else
                                objEnrolledServices.OldId = item.ServiceId;
                        }

                        dbNewEntities.SaveChanges();
                        sb.Append(string.Format("'LookupEnrolledServices' Table Successfully Imported.") + Environment.NewLine);
                    }
                    catch (Exception ex)
                    {
                        sb.Append("'LookupEnrolledServices'" + Environment.NewLine);
                        sb.Append(string.Format(ex.Message) + Environment.NewLine);
                        if (ex.InnerException != null) sb.Append(string.Format(ex.InnerException.Message) + Environment.NewLine);
                    }

                    #endregion

                    break;

                case 12:

                    #region Practices

                    try
                    {
                        var oldPractice = dbOldEntities.Practices.ToList();

                        foreach (var item in oldPractice)
                        {
                            string practiceSpecialityType = null;
                            int oldPrefMethodofContact = int.Parse(item.PrefMethodofContact ?? "0");
                            var objPractice = dbNewEntities.Practices.FirstOrDefault(a => a.OldId == item.PraticeId && a.BusinessId == businessId);
                            bool isExists = (objPractice != null);
                            var objUser = dbNewEntities.Users.FirstOrDefault(a => a.OldId == item.CreatedBy);
                            var objUpdatedUser = dbNewEntities.Users.FirstOrDefault(a => a.OldId == item.UpdatedBy);
                            var objPracticeType = dbNewEntities.LookupPracticeTypes.FirstOrDefault(a => a.OldId == item.PracticeType);


                            var objRepUser = dbNewEntities.Users.FirstOrDefault(a => a.OldId == item.RepNameId);
                            int repUserId = objRepUser == null ? 0 : objRepUser.Id;

                            var objReps = dbNewEntities.Reps.FirstOrDefault(a => a.UserId == repUserId);
                            int repId = objReps == null ? 0 : objReps.Id;

                            // Case : Other Practice Speciality Type
                            if (item.PracticeSpecialityType == 0)
                            {
                                var objPracticeContacts = dbOldEntities.PracticeContacts.FirstOrDefault(a => a.PracticeId == item.PraticeId);
                                practiceSpecialityType = (objPracticeContacts != null && !string.IsNullOrEmpty(objPracticeContacts.PracticeOther)) ? objPracticeContacts.PracticeOther : null;
                            }

                            int createdBy = objUser != null ? objUser.Id : 1;
                            int? updatedBy = objUpdatedUser != null ? (int?)objUpdatedUser.Id : null;

                            if (!isExists && repId > 0)
                                dbNewEntities.Practices.Add(new Database.New.Practice
                                {
                                    CreatedBy = createdBy,
                                    CreatedOn = item.CreateOn,
                                    UpdatedBy = item.UpdatedOn != null ? updatedBy : null,
                                    UpdatedOn = item.UpdatedOn,
                                    OldId = item.PraticeId,
                                    PracticeName = item.PracticeName,
                                    PracticeTypeId = objPracticeType != null ? (int?)objPracticeType.Id : null,
                                    PracticeSpecialityType = practiceSpecialityType,
                                    ContactPreferenceId = oldPrefMethodofContact > 0 ? (int?)oldPrefMethodofContact : null,
                                    RepId = repId,
                                    ReportDeliveryPreference = item.PracEmail,
                                    ReportDeliveryFax = item.PracFax,
                                    FaxNumber = item.Faxno,
                                    BusinessId = businessId
                                });
                            else if (repId <= 0)
                            {
                                sb.Append(string.Format("Practice Not Imported. Details : PracticeId-{0} Message:Rep doesnot exists", item.PraticeId) + Environment.NewLine);
                            }
                        }

                        dbNewEntities.SaveChanges();
                        sb.Append(string.Format("'Practices' Table Successfully Imported.") + Environment.NewLine);
                    }
                    catch (Exception ex)
                    {
                        sb.Append("'Practices'" + Environment.NewLine);
                        sb.Append(string.Format(ex.Message) + Environment.NewLine);
                        if (ex.InnerException != null) sb.Append(string.Format(ex.InnerException.Message) + Environment.NewLine);
                    }

                    #endregion

                    break;

                case 13:

                    #region PracticePracticeSpecialityMappers

                    try
                    {
                        var objPractices = dbNewEntities.Practices.Where(a => a.PracticeSpecialityType == null);

                        if (objPractices != null)
                            foreach (var item in objPractices)
                            {
                                if (item.OldId > 0)
                                {
                                    var objMultySpecType = dbOldEntities.Practice_SpecialityType.Where(a => a.PracticeId == item.OldId).Select(a => new
                                    {
                                        practiceId = a.PracticeId,
                                        specialityId = a.SpecialityId
                                    });

                                    if (objMultySpecType != null && objMultySpecType.Count() > 0)
                                    {
                                        foreach (var specType in objMultySpecType)
                                        {
                                            var objPracticeSpecialityType = dbNewEntities.LookupPracticeSpecialityTypes.FirstOrDefault(a => a.OldId == specType.specialityId);
                                            if (objPracticeSpecialityType != null)
                                            {
                                                int practiceSpecialityId = objPracticeSpecialityType.Id;
                                                dbNewEntities.PracticePracticeSpecialityMappers.Add(new Database.New.PracticePracticeSpecialityMapper
                                                {
                                                    PracticeId = item.Id,
                                                    PracticeSpecialityId = practiceSpecialityId,
                                                    CreatedBy = item.CreatedBy,
                                                    CreatedOn = item.CreatedOn
                                                });
                                            }
                                        }
                                    }
                                    else
                                    {
                                        var objSingleSpecType = dbOldEntities.Practices.FirstOrDefault(a => a.PraticeId == item.OldId && a.PracticeSpecialityType > 0);
                                        if (objSingleSpecType != null)
                                        {
                                            var objPracticeSpecialityType = dbNewEntities.LookupPracticeSpecialityTypes.FirstOrDefault(a => a.OldId == objSingleSpecType.PracticeSpecialityType);
                                            if (objPracticeSpecialityType != null)
                                            {
                                                dbNewEntities.PracticePracticeSpecialityMappers.Add(new Database.New.PracticePracticeSpecialityMapper
                                                {
                                                    PracticeId = item.Id,
                                                    PracticeSpecialityId = objPracticeSpecialityType.Id,
                                                    CreatedBy = item.CreatedBy,
                                                    CreatedOn = item.CreatedOn
                                                });
                                            }
                                        }
                                    }
                                }
                            }

                        dbNewEntities.SaveChanges();
                        sb.Append(string.Format("'PracticePracticeSpecialityMapper' Table Successfully Imported.") + Environment.NewLine);
                    }
                    catch (Exception ex)
                    {
                        sb.Append("'PracticePracticeSpecialityMapper'" + Environment.NewLine);
                        sb.Append(string.Format(ex.Message) + Environment.NewLine);
                        if (ex.InnerException != null) sb.Append(string.Format(ex.InnerException.Message) + Environment.NewLine);
                    }

                    #endregion

                    break;

                case 14:

                    #region PracticeServiceMapper

                    try
                    {
                        var objPracticesWithOldIdPSM = dbNewEntities.Practices.Where(a => a.OldId > 0).Select(a => new
                        {
                            Id = a.Id,
                            OldId = a.OldId,
                            CreatedOn = a.CreatedOn,
                            CreatedBy = a.CreatedBy
                        });

                        if (objPracticesWithOldIdPSM != null)
                            foreach (var item in objPracticesWithOldIdPSM)
                            {
                                var objPracticeServices = dbOldEntities.Practice_serviceId.Where(a => a.PracticeID == item.OldId).Select(a => new
                                {
                                    PracticeId = a.PracticeID,
                                    serviceId = a.EnrolledSercvicedId
                                });

                                if (objPracticeServices != null && objPracticeServices.Count() > 0)
                                {
                                    foreach (var practiceService in objPracticeServices)
                                    {
                                        var objLookupEnrolledServices = dbNewEntities.LookupEnrolledServices.FirstOrDefault(a => a.OldId == practiceService.serviceId);
                                        if (objLookupEnrolledServices != null)
                                        {
                                            int enrolledServiceId = objLookupEnrolledServices.Id;
                                            dbNewEntities.PracticeServiceMappers.Add(new Database.New.PracticeServiceMapper
                                            {
                                                PracticeId = item.Id,
                                                EnrolledServiceId = enrolledServiceId,
                                                CreatedOn = item.CreatedOn,
                                                CreatedBy = item.CreatedBy
                                            });
                                        }
                                    }
                                }
                            }

                        dbNewEntities.SaveChanges();
                        sb.Append(string.Format("'PracticeServiceMapper' Table Successfully Imported.") + Environment.NewLine);
                    }
                    catch (Exception ex)
                    {
                        sb.Append("'PracticeServiceMapper'" + Environment.NewLine);
                        sb.Append(string.Format(ex.Message) + Environment.NewLine);
                        if (ex.InnerException != null)
                            sb.Append(string.Format(ex.InnerException.Message) + Environment.NewLine);
                    }

                    #endregion

                    break;

                case 15:

                    #region LookupCountry

                    try
                    {
                        var oldCountry = dbOldEntities.Countries.ToList();

                        foreach (var item in oldCountry)
                        {
                            var objCountry = dbNewEntities.LookupCountries.FirstOrDefault(a => a.OldId == item.CountryId || a.CountryName == item.CountryName);
                            bool isExists = (objCountry != null);
                            var objUser = dbNewEntities.Users.FirstOrDefault(a => a.OldId == item.CreatedBy);
                            int createdBy = objUser != null ? objUser.Id : 1;

                            if (!isExists)
                                dbNewEntities.LookupCountries.Add(new Database.New.LookupCountry
                                {
                                    CreatedBy = createdBy,
                                    CreatedOn = item.CreatedOn,
                                    CountryCode = item.CountryCode,
                                    CountryName = item.CountryName,
                                    IsActive = item.IsActive,
                                    OldId = item.CountryId
                                });
                            else
                                objCountry.OldId = item.CountryId;
                        }

                        dbNewEntities.SaveChanges();
                        sb.Append(string.Format("'LookupCountry' Table Successfully Imported.") + Environment.NewLine);
                    }
                    catch (Exception ex)
                    {
                        sb.Append("'LookupCountry'" + Environment.NewLine);
                        sb.Append(string.Format(ex.Message) + Environment.NewLine);

                        if (ex.InnerException != null) sb.Append(string.Format(ex.InnerException.Message) + Environment.NewLine);
                    }

                    #endregion

                    break;

                case 16:

                    #region LookupState

                    try
                    {
                        var oldStates = dbOldEntities.States.ToList();

                        foreach (var item in oldStates)
                        {
                            var objStates = dbNewEntities.LookupStates.FirstOrDefault(a => a.OldId == item.StateId || a.StateName == item.StateName);
                            bool isExists = (objStates != null);
                            var objUser = dbNewEntities.Users.FirstOrDefault(a => a.OldId == item.CreatedBy);
                            int createdBy = objUser != null ? objUser.Id : 1;
                            var objCountry = dbNewEntities.LookupCountries.FirstOrDefault(a => a.OldId == item.CountryId);
                            int countryId = objCountry == null ? 0 : objCountry.Id;

                            if (!isExists && countryId > 0)
                                dbNewEntities.LookupStates.Add(new Database.New.LookupState
                                {
                                    CreatedBy = createdBy,
                                    CreatedOn = item.CreatedOn,
                                    CountryId = countryId,
                                    StateCode = item.StateCode,
                                    StateName = item.StateName.Trim(),
                                    IsActive = item.IsActive,
                                    OldId = item.StateId
                                });
                            else
                                objStates.OldId = item.StateId;
                        }

                        dbNewEntities.SaveChanges();
                        sb.Append(string.Format("'LookupState' Table Successfully Imported.") + Environment.NewLine);
                    }
                    catch (Exception ex)
                    {
                        sb.Append("'LookupState'" + Environment.NewLine);
                        sb.Append(string.Format(ex.Message) + Environment.NewLine);
                        if (ex.InnerException != null) sb.Append(string.Format(ex.InnerException.Message) + Environment.NewLine);
                    }

                    #endregion

                    break;

                case 17:

                    #region Address

                    try
                    {
                        var oldAddresses = dbOldEntities.Addresses.ToList();

                        foreach (var item in oldAddresses)
                        {
                            var objAddresses = dbNewEntities.Addresses.FirstOrDefault(a => a.OldId == item.AddressId);
                            bool isExists = (objAddresses != null);
                            var objUser = dbNewEntities.Users.FirstOrDefault(a => a.OldId == item.CreatedBy);
                            int createdBy = objUser != null ? objUser.Id : 1;
                            var objState = dbNewEntities.LookupStates.FirstOrDefault(a => a.OldId == item.State);
                            int stateId = objState == null ? 0 : objState.Id;

                            if (!isExists)
                                dbNewEntities.Addresses.Add(new Database.New.Address
                                {
                                    CreatedBy = createdBy,
                                    CreatedOn = item.CreatedOn,
                                    AddressTypeId = item.AddressTypeId == 2 ? 1 : 2,
                                    Line1 = item.Line1,
                                    Line2 = item.Line2,
                                    City = item.City,
                                    StateId = stateId,
                                    Zip = item.Zip,
                                    OldId = item.AddressId
                                });
                        }

                        dbNewEntities.SaveChanges();
                        sb.Append(string.Format("'Address' Table Successfully Imported.") + Environment.NewLine);
                    }
                    catch (Exception ex)
                    {
                        sb.Append("'Address'" + Environment.NewLine);
                        sb.Append(string.Format(ex.Message) + Environment.NewLine);
                        if (ex.InnerException != null) sb.Append(string.Format(ex.InnerException.Message) + Environment.NewLine);
                    }

                    #endregion

                    break;

                case 18:

                    #region Phone

                    try
                    {
                        var oldPhones = dbOldEntities.Phones.ToList();

                        foreach (var item in oldPhones)
                        {
                            var objPhones = dbNewEntities.Phones.FirstOrDefault(a => a.OldId == item.PhoneId);
                            bool isExists = (objPhones != null);
                            var objUser = dbNewEntities.Users.FirstOrDefault(a => a.OldId == item.CreatedBy);
                            int createdBy = objUser != null ? objUser.Id : 1;
                            var objAddress = dbNewEntities.Addresses.FirstOrDefault(a => a.OldId == item.AddressId);
                            int addressId = objAddress == null ? 0 : objAddress.Id;

                            if (!isExists && addressId > 0)
                                dbNewEntities.Phones.Add(new Database.New.Phone
                                {
                                    CreatedBy = createdBy,
                                    CreatedOn = item.CreatedOn,
                                    OldId = item.PhoneId,
                                    PhoneTypeId = item.PhoneTypeId == 3 ? 1 : item.PhoneTypeId,
                                    PhoneNumber = item.PhoneNumber,
                                    Extension = item.Extension,
                                    AddressId = addressId
                                });
                        }

                        dbNewEntities.SaveChanges();
                        sb.Append(string.Format("'Phone' Table Successfully Imported.") + Environment.NewLine);
                    }
                    catch (Exception ex)
                    {
                        sb.Append("'Phone'" + Environment.NewLine);
                        sb.Append(string.Format(ex.Message) + Environment.NewLine);
                        if (ex.InnerException != null) sb.Append(string.Format(ex.InnerException.Message) + Environment.NewLine);
                    }

                    #endregion

                    break;

                case 19:

                    #region PracticeAddressMapper

                    try
                    {
                        var objPracticesWithOldIdPAM = dbNewEntities.Practices.Where(a => a.OldId > 0).Select(a => new
                        {
                            Id = a.Id,
                            OldId = a.OldId,
                            CreatedOn = a.CreatedOn,
                            CreatedBy = a.CreatedBy
                        });

                        if (objPracticesWithOldIdPAM != null)
                            foreach (var item in objPracticesWithOldIdPAM)
                            {
                                var objPracticeAddressesPAM = dbOldEntities.PracticeContacts.Where(a => a.PracticeId == item.OldId).Select(a => new
                                {
                                    PracticeId = a.PracticeId,
                                    AddressId = a.AddressId,
                                    ManagerName = a.ManagerName
                                });

                                if (objPracticeAddressesPAM != null && objPracticeAddressesPAM.Count() > 0)
                                {
                                    foreach (var practiceAddress in objPracticeAddressesPAM)
                                    {
                                        var objAddress = dbNewEntities.Addresses.FirstOrDefault(a => a.OldId == practiceAddress.AddressId);
                                        int addressId = objAddress == null ? 0 : objAddress.Id;
                                        if (addressId > 0)
                                        {
                                            dbNewEntities.PracticeAddressMappers.Add(new Database.New.PracticeAddressMapper
                                            {
                                                PracticeId = item.Id,
                                                AddressId = addressId,
                                                ManagerName = practiceAddress.ManagerName,
                                                CreatedOn = item.CreatedOn,
                                                CreatedBy = item.CreatedBy
                                            });
                                        }
                                    }
                                }
                            }

                        dbNewEntities.SaveChanges();
                        sb.Append(string.Format("'PracticeAddressMapper' Table Successfully Imported.") + Environment.NewLine);
                    }
                    catch (Exception ex)
                    {
                        sb.Append("'PracticeAddressMapper'" + Environment.NewLine);
                        sb.Append(string.Format(ex.Message) + Environment.NewLine);
                        if (ex.InnerException != null) sb.Append(string.Format(ex.InnerException.Message) + Environment.NewLine);
                    }

                    #endregion

                    break;

                case 20:

                    #region Practice Contact

                    try
                    {
                        var objPracticeAddresses = dbNewEntities.PracticeAddressMappers.ToList();

                        foreach (var item in objPracticeAddresses)
                        {
                            var oldPracticeContact = dbOldEntities.PracticeContacts.FirstOrDefault(a => a.AddressId == item.Address.OldId);
                            if (oldPracticeContact != null)
                            {
                                var objPracticeContact = dbNewEntities.PracticeContacts.FirstOrDefault(a => a.PracticeId == item.PracticeId && a.AddressId == item.AddressId);
                                bool isExists = (objPracticeContact != null);
                                var objUser = dbNewEntities.Users.FirstOrDefault(a => a.OldId == oldPracticeContact.CreatedBy);
                                int createdBy = objUser != null ? objUser.Id : 1;
                                if (!isExists)
                                    dbNewEntities.PracticeContacts.Add(new Database.New.PracticeContact
                                    {
                                        PracticeId = item.PracticeId,
                                        AddressId = item.AddressId,
                                        BillingContact = oldPracticeContact.BillingContact,
                                        BillingContactPhone = oldPracticeContact.BillingContactPhone,
                                        BillingContactEmail = oldPracticeContact.BillingContactEmail,
                                        ManagerName = oldPracticeContact.ManagerName,
                                        ManagerEmail = oldPracticeContact.ManagerEmail,
                                        ManagerPhone = oldPracticeContact.ManagerPhone,
                                        officedayshrs = oldPracticeContact.officedayshrs,
                                        CreatedBy = createdBy,
                                        CreatedOn = oldPracticeContact.CreateOn
                                    });
                            }
                        }

                        dbNewEntities.SaveChanges();
                        sb.Append(string.Format("'PracticeContact' Table Successfully Imported.") + Environment.NewLine);
                    }
                    catch (Exception ex)
                    {
                        sb.Append("'PracticeContact'" + Environment.NewLine);
                        sb.Append(string.Format(ex.Message) + Environment.NewLine);
                        if (ex.InnerException != null) sb.Append(string.Format(ex.InnerException.Message) + Environment.NewLine);
                    }

                    #endregion

                    break;

                case 21:

                    #region PracticeProvicerMapper

                    try
                    {
                        var objPracticesWithOldId = dbNewEntities.Practices.Where(a => a.OldId > 0).Select(a => new
                        {
                            Id = a.Id,
                            OldId = a.OldId,
                            CreatedOn = a.CreatedOn,
                            CreatedBy = a.CreatedBy
                        });

                        if (objPracticesWithOldId != null)
                            foreach (var item in objPracticesWithOldId)
                            {
                                var objProviders = dbOldEntities.Providers.Where(a => a.PracticeId == item.OldId).Select(a => new
                                {
                                    ProviderId = a.ProviderId,
                                });

                                if (objProviders != null && objProviders.Count() > 0)
                                {
                                    foreach (var provider in objProviders)
                                    {
                                        var objProvider = dbNewEntities.TempProviderMigrationDatas.FirstOrDefault(a => a.OldId == provider.ProviderId);
                                        int providerId = objProvider == null ? 0 : objProvider.NewId.Value;
                                        if (providerId > 0)
                                        {
                                            var objUser = dbNewEntities.Users.FirstOrDefault(a => a.OldId == item.CreatedBy);
                                            int createdBy = objUser != null ? objUser.Id : 1;

                                            //if()
                                            dbNewEntities.PracticeProviderMappers.Add(new Database.New.PracticeProviderMapper
                                            {
                                                PracticeId = item.Id,
                                                ProviderId = providerId,
                                                CreatedOn = item.CreatedOn,
                                                CreatedBy = createdBy
                                            });
                                        }
                                    }
                                }
                            }

                        dbNewEntities.SaveChanges();
                        sb.Append(string.Format("'PracticeProviderMapper' Table Successfully Imported.") + Environment.NewLine);
                    }
                    catch (Exception ex)
                    {
                        sb.Append("'PracticeProviderMapper'" + Environment.NewLine);
                        sb.Append(string.Format(ex.Message) + Environment.NewLine);
                        if (ex.InnerException != null) sb.Append(string.Format(ex.InnerException.Message) + Environment.NewLine);
                    }

                    #endregion

                    break;

                case 22:

                    #region PracticeProviderLocationMapper

                    try
                    {
                        var oldPracticeProverAddressess = dbOldEntities.PracticeProviderLocations.ToList();

                        foreach (var item in oldPracticeProverAddressess)
                        {
                            var objProvider = dbNewEntities.TempProviderMigrationDatas.FirstOrDefault(a => a.OldId == item.ProviderId);
                            int providerId = objProvider == null ? 0 : objProvider.NewId.Value;

                            var objAddress = dbNewEntities.Addresses.FirstOrDefault(a => a.OldId == item.AddressId);
                            int addressId = objAddress == null ? 0 : objAddress.Id;

                            var objPractice = dbNewEntities.Practices.FirstOrDefault(a => a.OldId == item.PracticeId);
                            int practiceId = objPractice == null ? 0 : objPractice.Id;

                            var objUser = dbNewEntities.Users.FirstOrDefault(a => a.OldId == item.CreatedBy);
                            int createdBy = objUser != null ? objUser.Id : 1;

                            if (providerId > 0 && addressId > 0 && practiceId > 0)
                                dbNewEntities.PracticeProviderLocationMappers.Add(new Database.New.PracticeProviderLocationMapper
                                {
                                    PracticeId = practiceId,
                                    ProviderId = providerId,
                                    AddressId = addressId,
                                    CreatedBy = createdBy,
                                    CreatedOn = item.CreatedOn
                                });

                        }

                        dbNewEntities.SaveChanges();
                        sb.Append(string.Format("'PracticeProviderLocationMapper' Table Successfully Imported.") + Environment.NewLine);
                    }
                    catch (Exception ex)
                    {
                        sb.Append("'PracticeProviderLocationMapper'" + Environment.NewLine);
                        sb.Append(string.Format(ex.Message) + Environment.NewLine);
                        if (ex.InnerException != null) sb.Append(string.Format(ex.InnerException.Message) + Environment.NewLine);
                    }

                    #endregion

                    break;

                case 23:

                    #region LookupLeadSources

                    try
                    {
                        var oldLeadSources = dbOldEntities.LeadSources.ToList();

                        foreach (var item in oldLeadSources)
                        {
                            var objLeadSources = dbNewEntities.LookupLeadSources.FirstOrDefault(a => a.OldId == item.LeadSourceId || a.LeadSource == item.LeadSource1);
                            bool isExists = (objLeadSources != null);
                            var objUser = dbNewEntities.Users.FirstOrDefault(a => a.OldId == item.CreatedBy);
                            int createdBy = objUser != null ? objUser.Id : 1;

                            if (!isExists)
                                dbNewEntities.LookupLeadSources.Add(new Database.New.LookupLeadSource
                                {
                                    CreatedBy = createdBy,
                                    CreatedOn = item.CreatedOn,
                                    IsActive = item.IsActive,
                                    OldId = item.LeadSourceId,
                                    LeadSource = item.LeadSource1,
                                });
                            else
                                objLeadSources.OldId = item.LeadSourceId;
                        }

                        dbNewEntities.SaveChanges();
                        sb.Append(string.Format("'LookupLeadSources' Table Successfully Imported.") + Environment.NewLine);
                    }
                    catch (Exception ex)
                    {
                        sb.Append("'LookupLeadSources'" + Environment.NewLine);
                        sb.Append(string.Format(ex.Message) + Environment.NewLine);
                        if (ex.InnerException != null) sb.Append(string.Format(ex.InnerException.Message) + Environment.NewLine);
                    }

                    #endregion

                    break;

                case 24:

                    #region Leads

                    try
                    {
                        var objPracticesLeads = dbNewEntities.Practices.Where(a => a.OldId > 0 && a.BusinessId == businessId).ToList();

                        foreach (var item in objPracticesLeads)
                        {
                            var oldLead = dbOldEntities.Leads.FirstOrDefault(a => a.PracticeId == item.OldId);

                            if (oldLead != null)
                            {
                                var objLead = dbNewEntities.Leads.FirstOrDefault(a => a.OldId == oldLead.LeadId && a.BusinessId == businessId);
                                bool isExists = (objLead != null);
                                var objUser = dbNewEntities.Users.FirstOrDefault(a => a.OldId == oldLead.CreatedBy);
                                int createdBy = objUser != null ? objUser.Id : 1;
                                var objUpdatedUser = dbNewEntities.Users.FirstOrDefault(a => a.OldId == item.UpdatedBy);
                                int? updatedBy = objUpdatedUser != null ? (int?)objUpdatedUser.Id : null;
                                var objleadSource = dbNewEntities.LookupLeadSources.FirstOrDefault(a => a.OldId == oldLead.LeadSource);
                                if (!isExists)
                                {
                                    bool isAccount = dbOldEntities.Accounts.Any(a => a.LeadId == oldLead.LeadId);
                                    dbNewEntities.Leads.Add(new Database.New.Lead
                                    {
                                        PracticeId = item.Id,
                                        LeadSourceId = objleadSource == null ? null : (int?)objleadSource.Id,
                                        RepId = item.RepId,
                                        LeadServiceIntrest = oldLead.LeadServiceIntrest,
                                        IsActive = oldLead.Status.HasValue ? oldLead.Status.Value : true,
                                        LeadStatus = isAccount ? 2 : 1,
                                        OtherLeadSource = oldLead.OtherLeadSource,
                                        IsConverted = isAccount,
                                        BusinessId = businessId,
                                        CreatedBy = createdBy,
                                        CreatedOn = item.CreatedOn,
                                        UpdatedBy = item.UpdatedOn != null ? updatedBy : null,
                                        UpdatedOn = item.UpdatedOn,
                                        OldId = oldLead.LeadId
                                    });
                                }
                            }
                        }

                        dbNewEntities.SaveChanges();
                        sb.Append(string.Format("'Leads' Table Successfully Imported.") + Environment.NewLine);
                    }
                    catch (Exception ex)
                    {
                        sb.Append("'Leads'" + Environment.NewLine);
                        sb.Append(string.Format(ex.Message) + Environment.NewLine);
                        if (ex.InnerException != null)
                            sb.Append(string.Format(ex.InnerException.Message) + Environment.NewLine);
                    }

                    #endregion

                    break;

                case 25:

                    #region Accounts

                    try
                    {
                        var objLeads = dbNewEntities.Leads.Where(a => a.OldId > 0 && a.IsConverted == true && a.BusinessId == businessId).ToList();

                        foreach (var item in objLeads)
                        {
                            var objAccount = dbNewEntities.Accounts.FirstOrDefault(a => a.LeadId == item.Id && a.BusinessId == businessId);
                            bool isExists = (objAccount != null);
                            var oldAccount = dbOldEntities.Accounts.FirstOrDefault(a => a.LeadId == item.OldId);
                            if (!isExists)
                            {
                                if (oldAccount != null)
                                {
                                    var objUser = dbNewEntities.Users.FirstOrDefault(a => a.OldId == oldAccount.CreatedBy);
                                    int createdBy = objUser != null ? objUser.Id : 1;
                                    var objUpdatedUser = dbNewEntities.Users.FirstOrDefault(a => a.OldId == item.UpdatedBy);
                                    int? updatedBy = objUpdatedUser != null ? (int?)objUpdatedUser.Id : null;
                                    dbNewEntities.Accounts.Add(new Database.New.Account
                                    {
                                        BusinessId = businessId,
                                        OldId = oldAccount.AccountId,
                                        LeadId = item.Id,
                                        EnrolledDate = oldAccount.EnrollDate,
                                        IsActive = oldAccount.AccountStatus == 1 ? true : false,
                                        CreatedBy = createdBy,
                                        CreatedOn = oldAccount.CreatedOn,
                                        UpdatedBy = item.UpdatedOn != null ? updatedBy : null,
                                        UpdatedOn = item.UpdatedOn,
                                    });
                                }
                            }
                            else
                                if (oldAccount != null)
                                {
                                    objAccount.OldId = oldAccount.AccountId;
                                }
                        }

                        dbNewEntities.SaveChanges();
                        sb.Append(string.Format("'Accounts' Table Successfully Imported.") + Environment.NewLine);
                    }
                    catch (Exception ex)
                    {
                        sb.Append("'Accounts'" + Environment.NewLine);
                        sb.Append(string.Format(ex.Message) + Environment.NewLine);
                        if (ex.InnerException != null)
                            sb.Append(string.Format(ex.InnerException.Message) + Environment.NewLine);
                    }

                    #endregion

                    break;

                case 26:

                    #region Notes for Practice only - (Task -  Not Done)

                    try
                    {
                        var oldNotes = dbOldEntities.Notes.ToList();

                        foreach (var item in oldNotes)
                        {
                            var objNote = dbNewEntities.Notes.FirstOrDefault(a => a.OldId == item.NotesId);
                            bool isExists = (objNote != null);
                            var objUser = dbNewEntities.Users.FirstOrDefault(a => a.OldId == item.CreatedBy);
                            int createdBy = objUser != null ? objUser.Id : 1;
                            int parentId = 0;
                            if (item.NotesTypeId == null && item.PracticeId != null)
                            {
                                var objPractice = dbNewEntities.Practices.FirstOrDefault(a => a.OldId == item.PracticeId);
                                parentId = objPractice == null ? 0 : objPractice.Id;
                            }
                            else
                            {
                                // Manage after insert tasks
                            }

                            if (!isExists && parentId > 0 && item.PracticeId != null)
                                dbNewEntities.Notes.Add(new Database.New.Note
                                {
                                    OldId = item.NotesId,
                                    CreatedBy = createdBy,
                                    CreatedOn = item.CreatedOn,
                                    Description = item.NotesDescription,
                                    ParentTypeId = item.NotesTypeId ?? 1,
                                    ParentId = parentId
                                });
                        }

                        dbNewEntities.SaveChanges();
                        sb.Append(string.Format("'Notes' Table for Practices Successfully Imported.", PerformHeavyOperation(1), Environment.NewLine));
                    }
                    catch (Exception ex)
                    {
                        sb.Append("'Notes'" + Environment.NewLine);
                        sb.Append(string.Format(ex.Message) + Environment.NewLine);
                        if (ex.InnerException != null) sb.Append(string.Format(ex.InnerException.Message) + Environment.NewLine);
                    }

                    #endregion

                    break;

                case 27:

                    #region Rep Service Mapper

                    try
                    {
                        var objNewPractices = dbNewEntities.Practices.ToList();

                        foreach (var item in objNewPractices)
                        {
                            var objPracticeServiceMapper = dbNewEntities.PracticeServiceMappers.Where(a => a.PracticeId == item.Id).ToList();

                            if (objPracticeServiceMapper != null && objPracticeServiceMapper.Count > 0 && item.RepId.HasValue)
                            {
                                foreach (var objMapper in objPracticeServiceMapper)
                                {
                                    using (var tempDbNewEntities = new Database.New.CRMStagingEntities())
                                    {
                                        if (!tempDbNewEntities.RepServiceMappers.Any(a => a.ServiceId == objMapper.EnrolledServiceId && a.RepId == item.RepId))
                                        {
                                            //if (item.RepId.HasValue && !tempDbNewEntities.RepServiceMappers.Any(a => a.RepId == item.RepId.Value && a.ServiceId == objMapper.EnrolledServiceId))
                                            //{
                                            tempDbNewEntities.RepServiceMappers.Add(new Database.New.RepServiceMapper
                                            {
                                                RepId = item.RepId.Value,
                                                ServiceId = objMapper.EnrolledServiceId,
                                                CreatedBy = item.CreatedBy,
                                                CreatedOn = item.CreatedOn,
                                            });
                                            tempDbNewEntities.SaveChanges();
                                            //}
                                        }
                                    }
                                }
                            }
                        }
                        sb.Append(string.Format("'RepServiceMapper' Table Successfully Imported.") + Environment.NewLine);
                    }
                    catch (Exception ex)
                    {
                        sb.Append("'RepServiceMapper'" + Environment.NewLine);
                        sb.Append(string.Format(ex.Message) + Environment.NewLine);
                        if (ex.InnerException != null)
                            sb.Append(string.Format(ex.InnerException.Message) + Environment.NewLine);
                    }

                    #endregion

                    break;

                case 28:

                    #region ReportMaster

                    try
                    {
                        ////Compounding      -   Rxc
                        ////Pharmacogenitics -   Pgx
                        ////Toxicology       -   Txi

                        var oldReport = dbOldEntities.Reports.Where(a => a.ReportTypeId == "Toxicology").OrderBy(a => a.RId).ToList();
                        int RxcId = 0, PgxId = 0, TxiId = 0;

                        var objRxc = dbNewEntities.LookupEnrolledServices.FirstOrDefault(a => a.ServiceName.Trim() == "Compounding" && a.BusinessId == businessId);
                        if (objRxc == null)
                        {
                            var objResult = dbNewEntities.LookupEnrolledServices.Add(new Database.New.LookupEnrolledService
                            {
                                BusinessId = businessId,
                                ServiceName = "Compounding",
                                IsActive = true,
                                CreatedOn = DateTime.UtcNow,
                                CreatedBy = 1,
                            });

                            dbNewEntities.SaveChanges();

                            RxcId = objResult.Id;
                        }
                        else
                            RxcId = objRxc.Id;

                        var objPgx = dbNewEntities.LookupEnrolledServices.FirstOrDefault(a => a.ServiceName == "Pharmacogenetics" && a.BusinessId == businessId);
                        if (objPgx == null)
                        {
                            var objResult = dbNewEntities.LookupEnrolledServices.Add(new Database.New.LookupEnrolledService
                            {
                                BusinessId = businessId,
                                ServiceName = "Pharmacogenetics",
                                IsActive = true,
                                CreatedOn = DateTime.UtcNow,
                                CreatedBy = 1,
                            });

                            dbNewEntities.SaveChanges();

                            PgxId = objResult.Id;
                        }
                        else
                            PgxId = objPgx.Id;

                        var objTxi = dbNewEntities.LookupEnrolledServices.FirstOrDefault(a => a.ServiceName == "Toxicology" && a.BusinessId == businessId);
                        if (objTxi == null)
                        {
                            var objResult = dbNewEntities.LookupEnrolledServices.Add(new Database.New.LookupEnrolledService
                            {
                                BusinessId = businessId,
                                ServiceName = "Toxicology",
                                IsActive = true,
                                CreatedOn = DateTime.UtcNow,
                                CreatedBy = 1,
                            });

                            dbNewEntities.SaveChanges();

                            TxiId = objResult.Id;
                        }
                        else
                            TxiId = objTxi.Id;

                        foreach (var item in oldReport)
                        {
                            using (dbNewEntities = new Database.New.CRMStagingEntities())
                            {

                                int serviceId = 0, scriptId = 0, repId = 0;
                                DateTime? specimenReceivedDate = null, specimenCollectedDate = null, specimenReportedDate = null;
                                string npiNumber = null, repFirstName = null, repLastName = null; //specimenId = null, 

                                if (item.RepId > 0)
                                {
                                    var objUser = dbNewEntities.Users.FirstOrDefault(a => a.OldId == item.RepId);
                                    int userId = objUser == null ? 0 : objUser.Id;

                                    var objRep = dbNewEntities.Reps.FirstOrDefault(a => a.UserId == userId);
                                    if (objRep != null)
                                    {
                                        repId = objRep.Id;
                                        repFirstName = objRep.User2.FirstName;
                                        repLastName = objRep.User2.LastName;
                                    }
                                }

                                switch (item.ReportTypeId)
                                {
                                    case "Rxc":
                                        serviceId = RxcId;
                                        var objRxcReport = dbOldEntities.RxcReports.FirstOrDefault(a => a.ScriptId == item.ReportId);
                                        if (objRxcReport != null)
                                        {
                                            scriptId = objRxcReport.ScriptId;
                                            DateTime rdt;
                                            if (DateTime.TryParse(objRxcReport.DateReceived, out rdt))
                                                specimenReceivedDate = rdt;

                                            DateTime rpdt;
                                            if (DateTime.TryParse(objRxcReport.DateFilled, out rpdt))
                                                specimenReportedDate = rpdt;
                                        }
                                        break;
                                    case "Pgx":
                                        serviceId = PgxId;
                                        var objPgxReport = dbOldEntities.PgxReport1.FirstOrDefault(a => a.ScriptId == item.ReportId);
                                        if (objPgxReport != null)
                                        {
                                            scriptId = objPgxReport.ScriptId;
                                            DateTime rdt;
                                            if (DateTime.TryParse(objPgxReport.ReceivedDt, out rdt))
                                                specimenReceivedDate = rdt;

                                            DateTime cdt;
                                            if (DateTime.TryParse(objPgxReport.CollectedDt, out cdt))
                                                specimenCollectedDate = cdt;

                                            DateTime rpdt;
                                            if (DateTime.TryParse(objPgxReport.Reported, out rpdt))
                                                specimenReportedDate = rpdt;

                                            //specimenId = objPgxReport.SpecimenID;
                                            npiNumber = objPgxReport.NpiNumber;
                                        }
                                        break;
                                    case "Toxicology":
                                        serviceId = TxiId;
                                        var objTxcReport = dbOldEntities.Toxicologies.FirstOrDefault(a => a.ScriptId == item.ReportId);
                                        if (objTxcReport != null)
                                        {
                                            scriptId = objTxcReport.ScriptId;
                                            DateTime rdt;
                                            if (DateTime.TryParse(objTxcReport.ReceivedDt, out rdt))
                                                specimenReceivedDate = rdt;

                                            DateTime cdt;
                                            if (DateTime.TryParse(objTxcReport.CollectedDt, out cdt))
                                                specimenCollectedDate = cdt;

                                            DateTime rpdt;
                                            if (DateTime.TryParse(objTxcReport.Reported, out rpdt))
                                                specimenReportedDate = rpdt;

                                            //specimenId = objTxcReport.SpecimenID;
                                            npiNumber = objTxcReport.NpiNumber;
                                        }
                                        break;
                                }

                                if (scriptId > 0 && serviceId > 0)
                                {
                                    int providerId = item.ProviderId.HasValue ? item.ProviderId.Value : 0;

                                    //int providerId = 0;
                                    //if (objPatientReport != null)
                                    //{

                                    //providerId = item.ProviderId.HasValue ? item.ProviderId.Value : 0;
                                    //switch (item.ReportTypeId)
                                    //{
                                    //    case "Pgx":
                                    //    case "Toxicology":
                                    //        providerId = item.ProviderId.HasValue ? item.ProviderId.Value : 0;
                                    //        break;
                                    //    default:
                                    //        int.TryParse(objPatientReport.ProviderID, out providerId);
                                    //        break;
                                    //}
                                    //}


                                    int practiceId = item.PracticeId == null ? 0 : item.PracticeId.Value;
                                    if (practiceId > 0)
                                    {
                                        var objPractice = dbNewEntities.Practices.FirstOrDefault(a => a.OldId == practiceId);
                                        practiceId = objPractice == null ? 0 : objPractice.Id;
                                    }

                                    string providerFirstName = null, providerLastName = null;
                                    List<string> providerName = null;
                                    if (item.ProviderName.Contains(","))
                                    {
                                        providerName = item.ProviderName.Trim().Split(',').ToList();
                                        providerFirstName = providerName[0];
                                        providerName.RemoveAt(0);
                                        providerLastName = string.Join(" ", providerName);
                                    }
                                    else if (item.ProviderName.Contains(" "))
                                    {
                                        providerName = item.ProviderName.Trim().Split(' ').ToList();
                                        providerFirstName = providerName[0];
                                        providerName.RemoveAt(0);
                                        providerLastName = string.Join(" ", providerName);
                                    }
                                    else
                                        providerFirstName = item.ProviderName.Trim();

                                    int patientId = 0;
                                    string patientFirstName = null, patientLastName = null;

                                    var objPatientReport = dbOldEntities.PatientPgxreportMaps.FirstOrDefault(a => a.ScriptID.Trim() == scriptId.ToString());
                                    switch (item.ReportTypeId)
                                    {
                                        case "Pgx":
                                        case "Toxicology":
                                            objPatientReport = dbOldEntities.PatientPgxreportMaps.FirstOrDefault(a => a.ScriptID.Trim() == item.RId.ToString());
                                            break;
                                    }

                                    if (item.ReportTypeId == "Rxc" && objPatientReport != null)
                                    {
                                        int.TryParse(objPatientReport.ProviderID, out providerId);
                                    }

                                    if (providerId > 0)
                                    {
                                        var objProvider = dbNewEntities.TempProviderMigrationDatas.FirstOrDefault(a => a.OldId == providerId);
                                        providerId = objProvider == null ? 0 : objProvider.NewId.Value;
                                    }

                                    if (objPatientReport != null)
                                    {
                                        switch (item.ReportTypeId)
                                        {
                                            case "Pgx":
                                                patientId = objPatientReport.PatientIdFix.HasValue ? objPatientReport.PatientIdFix.Value : 0;
                                                break;
                                            default:
                                                int.TryParse(objPatientReport.PatientID, out patientId);
                                                break;
                                        }
                                    }
                                    if (patientId > 0)
                                    {
                                        var objPatient = dbOldEntities.Patients.FirstOrDefault(a => a.Pid == patientId);
                                        if (objPatient != null)
                                        {
                                            patientFirstName = !string.IsNullOrEmpty(objPatient.PatientFirstName) ? objPatient.PatientFirstName : null;
                                            patientLastName = !string.IsNullOrEmpty(objPatient.PatientLastName) ? objPatient.PatientLastName : null;
                                            int.TryParse(objPatient.PatientID, out patientId);
                                        }
                                    }

                                    var objUser = dbNewEntities.Users.FirstOrDefault(a => a.OldId == item.CreatedBy);
                                    int createdBy = objUser != null ? objUser.Id : 1;

                                    if (!dbNewEntities.ReportMasters.Any(a => a.OldId == item.RId))
                                        dbNewEntities.ReportMasters.Add(new Database.New.ReportMaster
                                        {
                                            BusinessId = businessId,
                                            PatientId = patientId,
                                            ServiceId = serviceId,
                                            PracticeId = practiceId > 0 ? (int?)practiceId : null,
                                            ProviderId = providerId > 0 ? (int?)providerId : null,
                                            PracticeName = item.PracticeName,
                                            ProviderFirstName = providerFirstName,
                                            ProviderLastName = providerLastName,
                                            PatientFirstName = patientFirstName,
                                            PatientLastName = patientLastName,
                                            SpecimenReceivedDate = specimenReceivedDate != DateTime.MinValue ? specimenReceivedDate : null,
                                            SpecimenCollectionDate = specimenCollectedDate != DateTime.MinValue ? specimenCollectedDate : null,
                                            ReportedDate = specimenReportedDate != DateTime.MinValue ? specimenReportedDate : null,
                                            RepId = repId > 0 ? (int?)repId : null,
                                            RepFirstName = repFirstName,
                                            RepLastName = repLastName,
                                            CreatedOn = item.CreatedOn.HasValue ? item.CreatedOn.Value : DateTime.UtcNow,
                                            CreatedBy = createdBy,
                                            OldId = item.RId,
                                            OldReportId = item.ReportId
                                        });
                                }
                                try
                                {
                                    dbNewEntities.SaveChanges();
                                }
                                catch (Exception ex)
                                {
                                    sb.Append(string.Format("'ReportMaster' Table Failed With RID : {0}", item.RId, Environment.NewLine));
                                    sb.Append("'Error Details'" + Environment.NewLine);
                                    sb.Append(string.Format(ex.Message) + Environment.NewLine);
                                    if (ex.InnerException != null)
                                    {
                                        sb.Append(string.Format(ex.InnerException.Message) + Environment.NewLine);
                                        if (ex.InnerException.InnerException != null) sb.Append(string.Format(ex.InnerException.InnerException.Message) + Environment.NewLine);
                                    }
                                }
                            }
                        }

                        //dbNewEntities.SaveChanges();
                        sb.Append(string.Format("'ReportMaster' Table Successfully Generated.", PerformHeavyOperation(1), Environment.NewLine));
                    }
                    catch (System.Data.Entity.Validation.DbEntityValidationException e)
                    {
                        foreach (var eve in e.EntityValidationErrors)
                        {
                            foreach (var ve in eve.ValidationErrors)
                            {
                                sb.Append(string.Format("- Property: \"{0}\", Error: \"{1}\"", ve.PropertyName, ve.ErrorMessage) + Environment.NewLine);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        sb.Append("'ReportMaster'" + Environment.NewLine);
                        sb.Append(string.Format(ex.Message) + Environment.NewLine);
                        if (ex.InnerException != null)
                        {
                            sb.Append(string.Format(ex.InnerException.Message) + Environment.NewLine);
                            if (ex.InnerException.InnerException != null) sb.Append(string.Format(ex.InnerException.InnerException.Message) + Environment.NewLine);
                        }
                    }

                    #endregion

                    break;

                case 29:

                    #region Report Columns

                    try
                    {
                        var objReportMaster = dbNewEntities.ReportMasters.Where(a => a.OldId > 0).ToList();

                        foreach (var item in objReportMaster)
                        {
                            switch (item.LookupEnrolledService.ServiceName)
                            {
                                case "Compounding":

                                    string[] RxcReportColumns = { "Rxno", "ScriptType", "ScriptNumber", "Formulations", "Status", "RxcPlan", "InsPaid" };
                                    foreach (var column in RxcReportColumns)
                                    {
                                        bool isReportColumnsExists = dbNewEntities.ReportColumns.Any(a =>
                                            a.BusinessId == businessId &&
                                            a.ServiceId == item.ServiceId &&
                                            a.ColumnName == column.Replace(" ", "")
                                            );

                                        if (!isReportColumnsExists)
                                        {
                                            dbNewEntities.ReportColumns.Add(new Database.New.ReportColumn
                                            {
                                                ColumnName = column.Replace(" ", ""),
                                                BusinessId = businessId,
                                                ServiceId = item.ServiceId,
                                                CreatedOn = DateTime.UtcNow,
                                                CreatedBy = 1
                                            });
                                            dbNewEntities.SaveChanges();
                                        }
                                    }

                                    break;
                                case "Pharmacogenetics":

                                    string[] PgxReportColumns = { "InsurancePlan", "InsuarancePlanType", "PayerName", "ReimbursementAmt", "SpecimenId" };
                                    foreach (var column in PgxReportColumns)
                                    {
                                        bool isReportColumnsExists = dbNewEntities.ReportColumns.Any(a =>
                                            a.BusinessId == businessId &&
                                            a.ServiceId == item.ServiceId &&
                                            a.ColumnName == column.Replace(" ", "")
                                            );

                                        if (!isReportColumnsExists)
                                        {
                                            dbNewEntities.ReportColumns.Add(new Database.New.ReportColumn
                                            {
                                                ColumnName = column.Replace(" ", ""),
                                                BusinessId = businessId,
                                                ServiceId = item.ServiceId,
                                                CreatedOn = DateTime.UtcNow,
                                                CreatedBy = 1
                                            });
                                            dbNewEntities.SaveChanges();
                                        }
                                    }

                                    break;
                                case "Toxicology":

                                    string[] TxiReportColumns = { "InsurancePlan", "InsuarancePlanType", "PayerName", "SpecimenId" };
                                    foreach (var column in TxiReportColumns)
                                    {
                                        bool isReportColumnsExists = dbNewEntities.ReportColumns.Any(a =>
                                            a.BusinessId == businessId &&
                                            a.ServiceId == item.ServiceId &&
                                            a.ColumnName == column.Replace(" ", "")
                                            );

                                        if (!isReportColumnsExists)
                                        {
                                            dbNewEntities.ReportColumns.Add(new Database.New.ReportColumn
                                            {
                                                ColumnName = column.Replace(" ", ""),
                                                BusinessId = businessId,
                                                ServiceId = item.ServiceId,
                                                CreatedOn = DateTime.UtcNow,
                                                CreatedBy = 1
                                            });
                                            dbNewEntities.SaveChanges();
                                        }
                                    }

                                    break;
                            }
                        }
                        sb.Append(string.Format("'ReportColumns' Table Successfully Imported.") + Environment.NewLine);
                    }
                    catch (Exception ex)
                    {
                        sb.Append("'ReportColumns'" + Environment.NewLine);
                        sb.Append(string.Format(ex.Message) + Environment.NewLine);
                        if (ex.InnerException != null) sb.Append(string.Format(ex.InnerException.Message) + Environment.NewLine);
                    }

                    #endregion

                    break;

                case 30:

                    #region Report Column Values

                    try
                    {
                        var objReportMasters = dbNewEntities.ReportMasters.Where(a => a.BusinessId == businessId && a.OldId > 0 && a.IsColumnValuesImported != true).ToList();
                        var objReportColumns = dbNewEntities.ReportColumns.Where(a => a.BusinessId == businessId).ToList();

                        foreach (var reportMaster in objReportMasters)
                        {
                            using (dbNewEntities = new Database.New.CRMStagingEntities())
                            {
                                foreach (var reportColumn in objReportColumns)
                                {
                                    var reportColumValue = new Database.New.ReportColumnValue
                                    {
                                        ReportId = reportMaster.Id,
                                        ColumnId = reportColumn.Id
                                    };

                                    switch (reportMaster.LookupEnrolledService.ServiceName)
                                    {
                                        case "Compounding":

                                            if (reportColumn.LookupEnrolledService.ServiceName == "Compounding")
                                            {
                                                var oldRxcRepColVal = dbOldEntities.RxcReports.FirstOrDefault(a => a.ScriptId == reportMaster.OldReportId);
                                                if (oldRxcRepColVal != null)
                                                {
                                                    reportColumValue.CreatedOn = oldRxcRepColVal.CreatedOn.HasValue ? oldRxcRepColVal.CreatedOn.Value : DateTime.UtcNow;

                                                    var objRxcUser = dbNewEntities.Users.FirstOrDefault(a => a.OldId == oldRxcRepColVal.CreatedBy);
                                                    int rxcCreatedBy = objRxcUser != null ? objRxcUser.Id : 1;
                                                    reportColumValue.CreatedBy = rxcCreatedBy;

                                                    switch (reportColumn.ColumnName)
                                                    {
                                                        case "Rxno":
                                                            reportColumValue.Value = oldRxcRepColVal.Rxno;
                                                            break;

                                                        case "ScriptType":
                                                            reportColumValue.Value = oldRxcRepColVal.ScriptType;
                                                            break;

                                                        case "ScriptNumber":
                                                            reportColumValue.Value = oldRxcRepColVal.ScriptNumber;
                                                            break;

                                                        case "Formulations":
                                                            reportColumValue.Value = oldRxcRepColVal.Formulations;
                                                            break;

                                                        case "Status":
                                                            reportColumValue.Value = oldRxcRepColVal.Status;
                                                            break;

                                                        case "RxcPlan":
                                                            reportColumValue.Value = oldRxcRepColVal.RxcPlan;
                                                            break;

                                                        case "InsPaid":
                                                            reportColumValue.Value = oldRxcRepColVal.InsPaid;
                                                            break;
                                                    }

                                                    reportColumValue.Value = string.IsNullOrEmpty(reportColumValue.Value) ? null : reportColumValue.Value;

                                                    if (!dbNewEntities.ReportColumnValues.Any(a => a.ReportId == reportColumValue.ReportId && a.ColumnId == reportColumValue.ColumnId && a.Value == reportColumValue.Value))
                                                        dbNewEntities.ReportColumnValues.Add(reportColumValue);
                                                }
                                            }

                                            break;

                                        case "Pharmacogenetics":

                                            if (reportColumn.LookupEnrolledService.ServiceName == "Pharmacogenetics")
                                            {
                                                var oldPgxRepColVal = dbOldEntities.PgxReport1.FirstOrDefault(a => a.ScriptId == reportMaster.OldReportId);
                                                if (oldPgxRepColVal != null)
                                                {
                                                    reportColumValue.CreatedOn = oldPgxRepColVal.CreatedOn.HasValue ? oldPgxRepColVal.CreatedOn.Value : DateTime.UtcNow;

                                                    var objPgxUser = dbNewEntities.Users.FirstOrDefault(a => a.OldId == oldPgxRepColVal.CreatedBy);
                                                    int pgxCreatedBy = objPgxUser != null ? objPgxUser.Id : 1;
                                                    reportColumValue.CreatedBy = pgxCreatedBy;

                                                    switch (reportColumn.ColumnName)
                                                    {
                                                        case "InsurancePlan":
                                                            reportColumValue.Value = oldPgxRepColVal.InsurancePlan;
                                                            break;

                                                        case "InsuarancePlanType":
                                                            reportColumValue.Value = oldPgxRepColVal.InsuarancePlanType;
                                                            break;

                                                        case "PayerName":
                                                            reportColumValue.Value = oldPgxRepColVal.PayerName;
                                                            break;

                                                        case "ReimbursementAmt":
                                                            reportColumValue.Value = oldPgxRepColVal.ReimbursementAmt;
                                                            break;

                                                        case "SpecimenId":
                                                            reportColumValue.Value = oldPgxRepColVal.SpecimenID;
                                                            break;
                                                    }

                                                    reportColumValue.Value = string.IsNullOrEmpty(reportColumValue.Value) ? null : reportColumValue.Value;

                                                    if (!dbNewEntities.ReportColumnValues.Any(a => a.ReportId == reportColumValue.ReportId && a.ColumnId == reportColumValue.ColumnId && a.Value == reportColumValue.Value))
                                                        dbNewEntities.ReportColumnValues.Add(reportColumValue);
                                                }
                                            }
                                            break;

                                        case "Toxicology":

                                            if (reportColumn.LookupEnrolledService.ServiceName == "Toxicology")
                                            {
                                                var oldTxiRepColVal = dbOldEntities.Toxicologies.FirstOrDefault(a => a.ScriptId == reportMaster.OldReportId);
                                                if (oldTxiRepColVal != null)
                                                {
                                                    reportColumValue.CreatedOn = oldTxiRepColVal.CreatedOn.HasValue ? oldTxiRepColVal.CreatedOn.Value : DateTime.UtcNow;

                                                    var objTxiUser = dbNewEntities.Users.FirstOrDefault(a => a.OldId == oldTxiRepColVal.CreatedBy);
                                                    int txiCreatedBy = objTxiUser != null ? objTxiUser.Id : 1;
                                                    reportColumValue.CreatedBy = txiCreatedBy;

                                                    switch (reportColumn.ColumnName)
                                                    {
                                                        case "InsurancePlan":
                                                            reportColumValue.Value = oldTxiRepColVal.InsurancePlan;
                                                            break;

                                                        case "InsuarancePlanType":
                                                            reportColumValue.Value = oldTxiRepColVal.InsuarancePlanType;
                                                            break;

                                                        case "PayerName":
                                                            reportColumValue.Value = oldTxiRepColVal.PayerName;
                                                            break;

                                                        case "SpecimenId":
                                                            reportColumValue.Value = oldTxiRepColVal.SpecimenID;
                                                            break;
                                                    }

                                                    reportColumValue.Value = string.IsNullOrEmpty(reportColumValue.Value) ? null : reportColumValue.Value;

                                                    if (!dbNewEntities.ReportColumnValues.Any(a => a.ReportId == reportColumValue.ReportId && a.ColumnId == reportColumValue.ColumnId && a.Value == reportColumValue.Value))
                                                        dbNewEntities.ReportColumnValues.Add(reportColumValue);
                                                }
                                            }
                                            break;
                                    }
                                }

                                dbNewEntities.ReportMasters.Find(reportMaster.Id).IsColumnValuesImported = true;

                                //reportMaster.IsColumnValuesImported = true;
                                dbNewEntities.SaveChanges();
                            }
                        }

                        //dbNewEntities.SaveChanges();
                        sb.Append(string.Format(Environment.NewLine + "'ReportColumnValues' Table Successfully Imported.") + Environment.NewLine);
                    }
                    catch (System.Data.Entity.Validation.DbEntityValidationException e)
                    {
                        foreach (var eve in e.EntityValidationErrors)
                        {
                            foreach (var ve in eve.ValidationErrors)
                            {
                                sb.Append(string.Format("- Property: \"{0}\", Error: \"{1}\"", ve.PropertyName, ve.ErrorMessage) + Environment.NewLine);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        sb.Append("'ReportColumnValues'" + Environment.NewLine);
                        sb.Append(string.Format(ex.Message) + Environment.NewLine);
                        if (ex.InnerException != null) sb.Append(string.Format(ex.InnerException.Message) + Environment.NewLine);
                    }

                    #endregion

                    break;
            }
        }

        protected void myWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker sendingWorker = (BackgroundWorker)sender;//Capture the BackgroundWorker that fired the event
            object[] arrObjects = (object[])e.Argument;//Collect the array of objects the we recived from the main thread

            int businessId = (int)arrObjects[0];//Get the numeric value from inside the objects array, don't forget to cast


            for (int i = 25; i <= 25; i++)//Start a for loop //run upto the total number of tables
            {
                if (!sendingWorker.CancellationPending)//At each iteration of the loop, //check if there is a cancellation request pending 
                {
                    sendingWorker.ReportProgress(i);
                    FnDataMigrator(i, businessId);
                    sendingWorker.ReportProgress(i + 100);
                }
                else
                {
                    e.Cancel = true;    //If a cancellation request is pending, assign this flag a value of true
                    break;              // If a cancellation request is pending, break to exit the loop
                }
            }

            e.Result = sb.ToString();   // Send our result to the main thread!
        }

        protected void myWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (!e.Cancelled && e.Error == null)    //Check if the worker has been cancelled or if an error occured
            {
                string result = (string)e.Result;   //Get the result from the background thread
                txtResult.Text = result;    //Display the result to the user
                lblStatus.Text = "Done";
            }
            else if (e.Cancelled)
            {
                lblStatus.Text = "User Cancelled";
            }
            else
            {
                lblStatus.Text = "An error has occured";
            }
            btnStart.Enabled = true;    //Reneable the start button
        }

        protected void myWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            string[] Tables = new string[] { "Users", "Departments", "Roles", "UserRoles", "LookupDegree", "Providers", "LookupPracticeTypes", "RepGroups", "Reps","LookupPracticeSpecialityTypes", 
            "LookupEnrolledServices", "Practices", "PracticePracticeSpecialityMapper", "PracticeServiceMapper", "LookupCountry", "LookupStates","Address", "Phone", "PracticeAddressMapper", "PracticeContact", 
            "PracticeProviderMapper", "PracticeProviderLocationMapper", "LookupLeadSources", "Leads", "Accounts", "Notes","RepServiceMapper","ReportMaster","ReportColumns","ReportColumnValue", "Users"};
            //Show the progress to the user based on the input we got from the background worker
            string msg = string.Empty;

            if (e.ProgressPercentage < 100)//give specified error number for each error and catch it here
            {
                msg = string.Format("Migrating {0} table started", Tables[e.ProgressPercentage - 1]) + Environment.NewLine;
                sb.Append(string.Format(msg, PerformHeavyOperation(1), Environment.NewLine));//Append the result to the string builder
            }
            if (e.ProgressPercentage > 100)//give specified error number for each error and catch it here
            {
                msg = string.Format("{0} table completed.", Tables[e.ProgressPercentage - 101]) + Environment.NewLine + Environment.NewLine;

                sb.Append(string.Format(msg, PerformHeavyOperation(1), Environment.NewLine));//Append the result to the string builder
            }

            lblStatus.Text = msg;
            txtResult.Text = txtResult.Text + msg;
        }

        private int PerformHeavyOperation(int i)
        {
            System.Threading.Thread.Sleep(100);
            return i * 1000;
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            var businessId = Convert.ToInt32(cmbBusiness.SelectedValue);
            if (businessId <= 0)
            {
                MessageBox.Show("Please select the company");
                return;
            }
            //    int numericValue = (int)0;//Capture the user input
            object[] arrObjects = new object[] { businessId };//Declare the array of objects
            if (!myWorker.IsBusy)//Check if the worker is already in progress
            {
                btnStart.Enabled = false;//Disable the Start button
                myWorker.RunWorkerAsync(arrObjects);//Call the background worker
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            myWorker.CancelAsync();//Issue a cancellation request to stop the background worker
        }

        private void Datamigrator_Load(object sender, EventArgs e)
        {
            var dbEntities = new Database.New.CRMStagingEntities();
            var dataSource = dbEntities.BusinessMasters.Select(a => new { Name = a.BusinessName, Id = a.Id });

            cmbBusiness.DataSource = dataSource.ToList();
            cmbBusiness.DisplayMember = "Name";
            cmbBusiness.ValueMember = "Id";
        }
    }
}
