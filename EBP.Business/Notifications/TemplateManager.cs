using EBP.Business.Enums;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace EBP.Business.Notifications
{
    public class TemplateManager
    {
        public static string Forgotpassword(string rootPath, string url, int currentBusinessId, string businessName)
        {
            string HtmlTemplate = string.Empty,
                templatePath = rootPath + "/Email/forgotpassword.html",
                logoUrl = string.Format("{0}/Assets/{1}/Logo_{1}.jpg", ConfigurationManager.AppSettings["PortalUrl"] ?? ConfigurationManager.AppSettings["BaseUrl"] ?? "https://crm.careconnectsystems.com", currentBusinessId);
            HtmlTemplate = File.ReadAllText(templatePath);
            HtmlTemplate = HttpUtility.HtmlDecode(HtmlTemplate);
            HtmlTemplate = HtmlTemplate.Replace("<%LogoUrl%>", logoUrl);

            CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;
            TextInfo textInfo = cultureInfo.TextInfo;

            HtmlTemplate = HtmlTemplate.Replace("<%BusinessName%>", textInfo.ToTitleCase(businessName));
            HtmlTemplate = HtmlTemplate.Replace("<%link%>", url);
            HtmlTemplate = HtmlTemplate.Replace("<%Year%>", DateTime.UtcNow.Year.ToString());

            return HtmlTemplate;
        }

        public static string NewPracticeToManager(string rootPath, string services, string providers, string address, string managerName, string repName, string createdByName, string practiceName, string returnUrl, NotificationTargetType targetType, int currentBusinessId, string relativeUrl)
        {
            string HtmlTemplate,
                templatePath = templatePath = rootPath + "/Email/NewPracticeToManager.html",
                NotificationType = "Practice",
                logoUrl = string.Format("{0}/Assets/{1}/Logo_{1}.jpg", ConfigurationManager.AppSettings["PortalUrl"] ?? ConfigurationManager.AppSettings["BaseUrl"] ?? "https://crm.careconnectsystems.com", currentBusinessId),
                notificationSettings = string.Format("{0}/{1}/{2}", ConfigurationManager.AppSettings["PortalUrl"] ?? ConfigurationManager.AppSettings["BaseUrl"] ?? "https://crm.careconnectsystems.com", relativeUrl, "NotificationSettings");

            switch (targetType)
            {
                case NotificationTargetType.Lead:
                    NotificationType = "Lead";
                    break;
                case NotificationTargetType.Account:
                    NotificationType = "Account";
                    break;
            }

            HtmlTemplate = File.ReadAllText(templatePath);
            HtmlTemplate = HttpUtility.HtmlDecode(HtmlTemplate);

            HtmlTemplate = HtmlTemplate.Replace("<%LogoUrl%>", logoUrl);
            HtmlTemplate = HtmlTemplate.Replace("<%NotificationSettings%>", notificationSettings);
            HtmlTemplate = HtmlTemplate.Replace("<%NotificationType%>", NotificationType.ToLower());
            HtmlTemplate = HtmlTemplate.Replace("<%PracticeName%>", practiceName);
            HtmlTemplate = HtmlTemplate.Replace("<%ReturnUrl%>", returnUrl);
            HtmlTemplate = HtmlTemplate.Replace("<%ManagerName%>", managerName);
            HtmlTemplate = HtmlTemplate.Replace("<%RepName%>", repName);
            HtmlTemplate = HtmlTemplate.Replace("<%CreatedByName%>", createdByName);

            string displayAddress = "none";
            if (!string.IsNullOrEmpty(address))
                displayAddress = string.IsNullOrEmpty(address.Replace(" ", "").Replace(",", "")) ? "none" : "table-row";
            HtmlTemplate = HtmlTemplate.Replace("<%DisplayAddress%>", displayAddress);

            HtmlTemplate = HtmlTemplate.Replace("<%Address%>", address);

            string displayProvider = "none";
            if (!string.IsNullOrEmpty(providers))
                displayProvider = string.IsNullOrEmpty(providers.Replace(" ", "").Replace(",", "")) ? "none" : "table-row";
            HtmlTemplate = HtmlTemplate.Replace("<%DisplayProvider%>", displayProvider);

            HtmlTemplate = HtmlTemplate.Replace("<%Providers%>", providers);

            string displayServicer = "none";
            if (!string.IsNullOrEmpty(services))
                displayServicer = string.IsNullOrEmpty(services.Replace(" ", "").Replace(",", "")) ? "none" : "table-row";
            HtmlTemplate = HtmlTemplate.Replace("<%DisplayService%>", displayServicer);

            HtmlTemplate = HtmlTemplate.Replace("<%Services%>", services);
            HtmlTemplate = HtmlTemplate.Replace("<%Year%>", DateTime.UtcNow.Year.ToString());

            return HtmlTemplate;
        }

        public static string NewPracticeToRep(string rootPath, string services, string providers, string address, string repName, string createdByName, string practiceName, string returnUrl, NotificationTargetType targetType, int currentBusinessId, string relativeUrl)
        {
            string NotificationType = "Practice",
                logoUrl = string.Format("{0}/Assets/{1}/Logo_{1}.jpg", ConfigurationManager.AppSettings["PortalUrl"] ?? ConfigurationManager.AppSettings["BaseUrl"] ?? "https://crm.careconnectsystems.com", currentBusinessId),
                notificationSettings = string.Format("{0}/{1}/{2}", ConfigurationManager.AppSettings["PortalUrl"] ?? ConfigurationManager.AppSettings["BaseUrl"] ?? "https://crm.careconnectsystems.com", relativeUrl, "NotificationSettings");

            switch (targetType)
            {
                case NotificationTargetType.Lead:
                    NotificationType = "Lead";
                    break;
                case NotificationTargetType.Account:
                    NotificationType = "Account";
                    break;
            }

            string HtmlTemplate = string.Empty,
                templatePath = templatePath = rootPath + "/Email/NewPracticeToRep.html";
            HtmlTemplate = File.ReadAllText(templatePath);
            HtmlTemplate = HttpUtility.HtmlDecode(HtmlTemplate);

            HtmlTemplate = HtmlTemplate.Replace("<%LogoUrl%>", logoUrl);
            HtmlTemplate = HtmlTemplate.Replace("<%NotificationSettings%>", notificationSettings);
            HtmlTemplate = HtmlTemplate.Replace("<%NotificationType%>", NotificationType.ToLower());
            HtmlTemplate = HtmlTemplate.Replace("<%PracticeName%>", practiceName);
            HtmlTemplate = HtmlTemplate.Replace("<%RepName%>", repName);
            HtmlTemplate = HtmlTemplate.Replace("<%CreatedByName%>", createdByName);

            string displayAddress = "none";
            if (!string.IsNullOrEmpty(address))
                displayAddress = string.IsNullOrEmpty(address.Replace(" ", "").Replace(",", "")) ? "none" : "table-row";
            HtmlTemplate = HtmlTemplate.Replace("<%DisplayAddress%>", displayAddress);

            HtmlTemplate = HtmlTemplate.Replace("<%Address%>", address);

            string displayProvider = "none";
            if (!string.IsNullOrEmpty(providers))
                displayProvider = string.IsNullOrEmpty(providers.Replace(" ", "").Replace(",", "")) ? "none" : "table-row";
            HtmlTemplate = HtmlTemplate.Replace("<%DisplayProvider%>", displayProvider);

            HtmlTemplate = HtmlTemplate.Replace("<%Providers%>", providers);

            string displayServicer = "none";
            if (!string.IsNullOrEmpty(services))
                displayServicer = string.IsNullOrEmpty(services.Replace(" ", "").Replace(",", "")) ? "none" : "table-row";
            HtmlTemplate = HtmlTemplate.Replace("<%DisplayService%>", displayServicer);

            HtmlTemplate = HtmlTemplate.Replace("<%Services%>", services);
            HtmlTemplate = HtmlTemplate.Replace("<%Year%>", DateTime.UtcNow.Year.ToString());

            return HtmlTemplate;
        }

        public static string NewAccountAddress(string rootPath, string services, string providers, string address, string createdUser, string practiceName, string repName, int currentBusinessId, string relativeUrl)
        {
            string HtmlTemplate = string.Empty,
                templatePath = rootPath + "/Email/NewAccountAddress.html",
                logoUrl = string.Format("{0}/Assets/{1}/Logo_{1}.jpg", ConfigurationManager.AppSettings["PortalUrl"] ?? ConfigurationManager.AppSettings["BaseUrl"] ?? "https://crm.careconnectsystems.com", currentBusinessId),
                notificationSettings = string.Format("{0}/{1}/{2}", ConfigurationManager.AppSettings["PortalUrl"] ?? ConfigurationManager.AppSettings["BaseUrl"] ?? "https://crm.careconnectsystems.com", relativeUrl, "NotificationSettings");
            HtmlTemplate = File.ReadAllText(templatePath);
            HtmlTemplate = HttpUtility.HtmlDecode(HtmlTemplate);

            HtmlTemplate = HtmlTemplate.Replace("<%LogoUrl%>", logoUrl);
            HtmlTemplate = HtmlTemplate.Replace("<%NotificationSettings%>", notificationSettings);
            HtmlTemplate = HtmlTemplate.Replace("<%CreatedUserName%>", createdUser);
            HtmlTemplate = HtmlTemplate.Replace("<%RepName%>", repName);
            HtmlTemplate = HtmlTemplate.Replace("<%AccountName%>", practiceName);
            HtmlTemplate = HtmlTemplate.Replace("<%Address%>", address);
            HtmlTemplate = HtmlTemplate.Replace("<%Providers%>", providers);
            HtmlTemplate = HtmlTemplate.Replace("<%Services%>", services);
            HtmlTemplate = HtmlTemplate.Replace("<%Year%>", DateTime.UtcNow.Year.ToString());

            return HtmlTemplate;
        }

        public static string NewBusinessCreate(string rootPath,string BusinessName, string UserName, string Password, string FirstName, string ReturnUrl, int currentBusinessId, string relativeUrl)
        {
            string HtmlTemplate = string.Empty,
                templatePath = rootPath + "/Email/NewBusiness.html";
            HtmlTemplate = File.ReadAllText(templatePath);
            HtmlTemplate = HttpUtility.HtmlDecode(HtmlTemplate);

            HtmlTemplate = HtmlTemplate.Replace("<%BusinessName%>", BusinessName);
            HtmlTemplate = HtmlTemplate.Replace("<%UserName%>", UserName);
            HtmlTemplate = HtmlTemplate.Replace("<%Password%>", Password);
            HtmlTemplate = HtmlTemplate.Replace("<%FirstName%>", FirstName);
            HtmlTemplate = HtmlTemplate.Replace("<%ReturnUrl%>", ReturnUrl);          

            return HtmlTemplate;
        }

        public static string NewUserCreate(string rootPath, string BusinessName, string UserName, string Password, string FirstName,string UserDepartments,string ReturnUrl)
        {
            string HtmlTemplate = string.Empty,
                templatePath = rootPath + "/Email/NewUser.html";
            HtmlTemplate = File.ReadAllText(templatePath);
            HtmlTemplate = HttpUtility.HtmlDecode(HtmlTemplate);

            HtmlTemplate = HtmlTemplate.Replace("<%BusinessName%>", BusinessName);
            HtmlTemplate = HtmlTemplate.Replace("<%UserName%>", UserName);
            HtmlTemplate = HtmlTemplate.Replace("<%Password%>", Password);
            HtmlTemplate = HtmlTemplate.Replace("<%FirstName%>", FirstName);
            HtmlTemplate = HtmlTemplate.Replace("<%UserDepartments%>", UserDepartments);
            HtmlTemplate = HtmlTemplate.Replace("<%ReturnUrl%>", ReturnUrl);
            return HtmlTemplate;
        }

        public static string AddPrivilegesToUser(string rootPath, string Subject, string AssignedUserName, string UserPrivileges,string CreatedUsername,string ReturnUrl)
        {
            string HtmlTemplate = string.Empty,
                templatePath = rootPath + "/Email/AddprivilegesToUser.html";
            HtmlTemplate = File.ReadAllText(templatePath);
            HtmlTemplate = HttpUtility.HtmlDecode(HtmlTemplate);

            HtmlTemplate = HtmlTemplate.Replace("<%Subject%>", Subject);
            HtmlTemplate = HtmlTemplate.Replace("<%AssignedUserName%>", AssignedUserName);
            HtmlTemplate = HtmlTemplate.Replace("<%UserPrivileges%>", UserPrivileges);
            HtmlTemplate = HtmlTemplate.Replace("<%CreatedUsername%>", CreatedUsername);
            HtmlTemplate = HtmlTemplate.Replace("<%ReturnUrl%>", ReturnUrl);
            return HtmlTemplate;
        }
        public static string NewTask(string rootPath, string toUserName, string AssignedUsers, string createdByName,
            string taskTitle, string dueOn, string description, string practiceName, string priority, string returnUrl, bool IsWatcher, int currentBusinessId, string relativeUrl)
        {
            string HtmlTemplate = string.Empty,
                templatePath = "",
                logoUrl = string.Format("{0}/Assets/{1}/Logo_{1}.jpg", ConfigurationManager.AppSettings["PortalUrl"] ?? ConfigurationManager.AppSettings["BaseUrl"] ?? "https://crm.careconnectsystems.com", currentBusinessId),
                notificationSettings = string.Format("{0}/{1}/{2}", ConfigurationManager.AppSettings["PortalUrl"] ?? ConfigurationManager.AppSettings["BaseUrl"] ?? "https://crm.careconnectsystems.com", relativeUrl, "NotificationSettings");
            if (IsWatcher)
            {
                templatePath = rootPath + "/Email/NewTaskToWatcher.html";
                HtmlTemplate = File.ReadAllText(templatePath);
                HtmlTemplate = HttpUtility.HtmlDecode(HtmlTemplate);
                HtmlTemplate = HtmlTemplate.Replace("<%AssignedUsers%>", AssignedUsers);
            }
            else
            {
                templatePath = rootPath + "/Email/NewTaskToAssignedUser.html";
                HtmlTemplate = File.ReadAllText(templatePath);
                HtmlTemplate = HttpUtility.HtmlDecode(HtmlTemplate);
            }

            string displayPractice = "none";
            if (!string.IsNullOrEmpty(practiceName))
                displayPractice = string.IsNullOrEmpty(practiceName.Replace(" ", "").Replace(",", "")) ? "none" : "table-row";

            HtmlTemplate = HtmlTemplate.Replace("<%LogoUrl%>", logoUrl);
            HtmlTemplate = HtmlTemplate.Replace("<%NotificationSettings%>", notificationSettings);
            HtmlTemplate = HtmlTemplate.Replace("<%TaskDescription%>", StripHTML(description));
            HtmlTemplate = HtmlTemplate.Replace("<%PracticeName%>", practiceName);
            HtmlTemplate = HtmlTemplate.Replace("<%DueOn%>", dueOn);
            HtmlTemplate = HtmlTemplate.Replace("<%Priority%>", priority);
            HtmlTemplate = HtmlTemplate.Replace("<%DisplayPractice%>", displayPractice);
            HtmlTemplate = HtmlTemplate.Replace("<%CreatedByName%>", createdByName);
            HtmlTemplate = HtmlTemplate.Replace("<%TaskTitle%>", taskTitle);
            HtmlTemplate = HtmlTemplate.Replace("<%ReturnUrl%>", returnUrl);
            HtmlTemplate = HtmlTemplate.Replace("<%ToUserName%>", toUserName);
            HtmlTemplate = HtmlTemplate.Replace("<%Year%>", DateTime.UtcNow.Year.ToString());

            return HtmlTemplate;
        }

        public static string UpdateOrDeleteTask(string rootPath, string toUserName, string assignedUsers, string createdByName,
            string taskTitle, string dueOn, string description, string practiceName, string priority, string status, string returnUrl, bool isWatcher, int currentBusinessId, string relativeUrl)
        {
            string HtmlTemplate = string.Empty,
                message = string.Format("The following task has been deleted by {0}", createdByName),
                templatePath = "",
                title = "A Task Has Been Deleted",
                logoUrl = string.Format("{0}/Assets/{1}/Logo_{1}.jpg", ConfigurationManager.AppSettings["PortalUrl"] ?? ConfigurationManager.AppSettings["BaseUrl"] ?? "https://crm.careconnectsystems.com", currentBusinessId),
                notificationSettings = string.Format("{0}/{1}/{2}", ConfigurationManager.AppSettings["PortalUrl"] ?? ConfigurationManager.AppSettings["BaseUrl"] ?? "https://crm.careconnectsystems.com", relativeUrl, "NotificationSettings");
            templatePath = rootPath + "/Email/TaskUpdateOrDelete.html";
            HtmlTemplate = File.ReadAllText(templatePath);
            HtmlTemplate = HttpUtility.HtmlDecode(HtmlTemplate);
            string displayPractice = "none",
                displayReturnUrl = "none",
                displayAssignedUsers = "block";
            if (!string.IsNullOrEmpty(practiceName))
                displayPractice = string.IsNullOrEmpty(practiceName.Replace(" ", "").Replace(",", "")) ? "none" : "table-row";
            if (!string.IsNullOrEmpty(returnUrl))
            {
                displayReturnUrl = "table-row";
                message = string.Format("The task status has been updated by {0}", createdByName);
                title = "Task Status Updated";
            }

            HtmlTemplate = HtmlTemplate.Replace("<%LogoUrl%>", logoUrl);
            HtmlTemplate = HtmlTemplate.Replace("<%NotificationSettings%>", notificationSettings);
            HtmlTemplate = HtmlTemplate.Replace("<%TaskDescription%>", StripHTML(description));
            HtmlTemplate = HtmlTemplate.Replace("<%PracticeName%>", practiceName);
            HtmlTemplate = HtmlTemplate.Replace("<%DueOn%>", dueOn);
            HtmlTemplate = HtmlTemplate.Replace("<%Priority%>", priority);
            HtmlTemplate = HtmlTemplate.Replace("<%Title%>", title);
            HtmlTemplate = HtmlTemplate.Replace("<%DisplayPractice%>", displayPractice);
            HtmlTemplate = HtmlTemplate.Replace("<%Message%>", message);
            HtmlTemplate = HtmlTemplate.Replace("<%CreatedByName%>", createdByName);
            HtmlTemplate = HtmlTemplate.Replace("<%TaskTitle%>", taskTitle);
            HtmlTemplate = HtmlTemplate.Replace("<%ReturnUrl%>", returnUrl);
            HtmlTemplate = HtmlTemplate.Replace("<%DisplayReturnUrl%>", displayReturnUrl);
            HtmlTemplate = HtmlTemplate.Replace("<%ToUserName%>", toUserName);
            HtmlTemplate = HtmlTemplate.Replace("<%Status%>", status);

            if (!isWatcher)
            {
                assignedUsers = null;
                displayAssignedUsers = "none";
            }

            HtmlTemplate = HtmlTemplate.Replace("<%AssignedUsers%>", assignedUsers);
            HtmlTemplate = HtmlTemplate.Replace("<%DisplayAssignedUsers%>", displayAssignedUsers);
            HtmlTemplate = HtmlTemplate.Replace("<%Year%>", DateTime.UtcNow.Year.ToString());

            return HtmlTemplate;
        }

        public static string NewNote(string rootPath, string toUserName, string createdByName, string taskTitle, string returnUrl, string emailBody, int currentBusinessId, string relativeUrl)
        {

            string HtmlTemplate = string.Empty,
                templatePath = "",
                logoUrl = string.Format("{0}/Assets/{1}/Logo_{1}.jpg", ConfigurationManager.AppSettings["PortalUrl"] ?? ConfigurationManager.AppSettings["BaseUrl"] ?? "https://crm.careconnectsystems.com", currentBusinessId),
                notificationSettings = string.Format("{0}/{1}/{2}", ConfigurationManager.AppSettings["PortalUrl"] ?? ConfigurationManager.AppSettings["BaseUrl"] ?? "https://crm.careconnectsystems.com", relativeUrl, "NotificationSettings");
            templatePath = rootPath + "/Email/NewTaskNote.html";
            HtmlTemplate = File.ReadAllText(templatePath);
            HtmlTemplate = HttpUtility.HtmlDecode(HtmlTemplate);

            HtmlTemplate = HtmlTemplate.Replace("<%LogoUrl%>", logoUrl);
            HtmlTemplate = HtmlTemplate.Replace("<%NotificationSettings%>", notificationSettings);
            HtmlTemplate = HtmlTemplate.Replace("<%TaskTitle%>", taskTitle);
            HtmlTemplate = HtmlTemplate.Replace("<%ReturnUrl%>", returnUrl);
            HtmlTemplate = HtmlTemplate.Replace("<%ToUserName%>", toUserName);
            HtmlTemplate = HtmlTemplate.Replace("<%CreatedByName%>", createdByName);
            HtmlTemplate = HtmlTemplate.Replace("<%EmailBody%>", emailBody);
            HtmlTemplate = HtmlTemplate.Replace("<%Year%>", DateTime.UtcNow.Year.ToString());
            return HtmlTemplate;
        }

        static string StripHTML(string inputString)
        {
            return Regex.Replace(inputString, "<.*?>", string.Empty);
        }
    }
}
