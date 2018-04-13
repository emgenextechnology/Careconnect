using EBP.Business.Entity.Practice;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace EBP.Business.Entity.Users
{
    public class EntityUsers
    {
        //public DateTime CreatedOn = DateTime.UtcNow;
        //public DateTime? UpdatedOn = DateTime.UtcNow;
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public int? BusinessId { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string HomePhone { get; set; }
        public string WorkEmail { get; set; }
        public string AdditionalPhone { get; set; }
        public string Address { get; set; }

        public string AddressLine1 { get; set; }

        public string AddressLine2 { get; set; }

        public string City { get; set; }

        public string Zip { get; set; }
        public int? StateId { get; set; }
        public string State { get; set; }
        public string UserName { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? LastLoggedInTime { get; set; }
        public string StartDate { get; set; }
        public List<int> RoleIds { get; set; }
        public List<int> DepartmentIds { get; set; }
        public IEnumerable<string> UserRoleNames { get; set; }
        public IEnumerable<string> UserDepartmentName { get; set; }
        public IEnumerable<SelectedItem> UserRoles { get; set; }
        public IEnumerable<SelectedItem> UserDepartments { get; set; }
        public string FilePath
        {
            get
            {
                return
                    string.Format("{0}/{1}/{2}/{3}/{4}.jpg", ConfigurationManager.AppSettings["BaseUrl"] ?? "", "Assets", BusinessId.HasValue ? BusinessId.ToString() : "0", "Users", Id.ToString());

            }
        }
        public bool HasUserImage { get; set; }
    }

    //
    // Summary:
    //     Represents the selected item in an instance of the System.Web.Mvc.SelectList
    //     class.
    public class SelectedItem
    {

        //
        // Summary:
        //     Gets or sets a value that indicates whether this System.Web.Mvc.SelectListItem
        //     is disabled.
        public bool Disabled { get; set; }
        //
        // Summary:
        //     Represents the optgroup HTML element this item is wrapped into. In a select list,
        //     multiple groups with the same name are supported. They are compared with reference
        //     equality.
        public SelectListGroup Group { get; set; }
        //
        // Summary:
        //     Gets or sets a value that indicates whether this System.Web.Mvc.SelectListItem
        //     is selected.
        //
        // Returns:
        //     true if the item is selected; otherwise, false.
        public bool Selected { get; set; }
        //
        // Summary:
        //     Gets or sets the text of the selected item.
        //
        // Returns:
        //     The text.
        public string Text { get; set; }
        //
        // Summary:
        //     Gets or sets the value of the selected item.
        //
        // Returns:
        //     The value.
        public string Value { get; set; }
    }
}
