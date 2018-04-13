using EBP.Business.Entity.Practice;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace EBP.Business.Entity.Rep
{
    public class EntityRep
    {
        public int? Id { get; set; }

        public string FirstName { get; set; }

        public string MiddleName { get; set; }

        public string LastName { get; set; }

        public string Username { get; set; }

        public string Name
        {
            get
            {
                return string.Format("{0} {1}", this.FirstName, this.LastName);
            }
        }

        public int RepGroup { get; set; }

        public int? GroupId { get; set; }

        public string GroupName { get; set; }

        public string ManagerName
        {
            get
            {
                string managerName = null;
                if (Managers != null && Managers.Count() > 0)
                {
                    managerName = string.Join(", ", Managers.Select(a => a.Name));
                }

                return managerName;
            }
        }

        public int ManagersCount
        {
            get
            {
                return Managers != null ? Managers.Count() : 0;
            }
        }

        public string RepEmail { get; set; }

        public IEnumerable<Manager> Managers { get; set; }
    }

    public class Manager
    {
        public int Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string UserName { get; set; }

        public string MiddleName { get; set; }

        public string Email { get; set; }

        public string Name
        {
            get
            {
                return string.Format("{0} {1} {2}", this.FirstName, this.MiddleName, this.LastName);
            }
        }
    }

    public class EntityReps
    {
        public DateTime CreatedOn = DateTime.UtcNow;

        public DateTime? UpdatedOn = DateTime.UtcNow;

        public int Id { get; set; }

        public int UserId { get; set; }

        public string RepName { get; set; }

        public string RepGroupName { get; set; }

        public int? RepGroupId { get; set; }

        public bool IsActive { get; set; }

        public DateTime? SignonDate { get; set; }

        public int CreatedBy { get; set; }

        public int? UpdatedBy { get; set; }

        public int? OldId { get; set; }

        //public List<int> ServiceIds { get; set; }
        public IEnumerable<string> ServiceNames { get; set; }
        public IEnumerable<SelectedItem> SelectedServiceNames { get; set; }
        public List<int> RepGroupManagerIds { get; set; }
        public IEnumerable<string> RepGroupManagerNames { get; set; }
        public int DirectorId { get; set; }
        public string DirectorName
        {
            get
            {
                if (DirectorNames == null || DirectorNames.Count() <= 0)
                    return null;
                else
                    return string.Join(", ", this.DirectorNames);
            }
        }
        public IEnumerable<string> DirectorNames { get; set; }

        public string CreatedByName { get; set; }

        public string UpdatedByName { get; set; }
        List<int> _serviceIds;
        public List<int> ServiceIds
        {
            get
            {
                if (_serviceIds != null && _serviceIds.Count > 0)
                    return _serviceIds;
                List<int> selectedIds = new List<int>();
                if (SelectedServiceNames != null && SelectedServiceNames.Count() > 0)
                {
                    foreach (var item in SelectedServiceNames)
                    {
                        if (item.Selected == true)
                        {
                            selectedIds.Add(Convert.ToInt32(item.Value));
                        }
                    }
                    return selectedIds;
                }
                return selectedIds;
            }
            set
            {
                _serviceIds = value;
            }
        }
    }

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
