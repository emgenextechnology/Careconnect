using System;
using System.Collections.Generic;
using System.Web.Script.Serialization;

namespace EBP.Business.Entity
{


    /// <summary>
    /// Usage example
    ///   var entity = (from a in entityOptions.Where(c => c.Type == "AgentJustification")
                                        //select new EntitySelectItem
                                        //{
                                        //    Text = a.Text,
                                        //    Value = a.Value,
                                        //}).ToList();
    /// </summary>

    
    public partial class EntitySelectItem
    {
        public EntitySelectItem()
        {
            IsSelected = false;
        }

        public string Text
        {
            get;
            set;
        }

        public int? SortOrder
        {
            get;
            set;
        }

        private object _value = null;

        public object Value
        {
            get {
                if (_value == null)
                {
                    if (this.Id.HasValue)
                        return Id;
                }
                return _value;            
            }
            set { _value = value; }
        }

        //[ScriptIgnore]
        public int? Id
        {
            get;
            set;
        }

        public int? ParentId
        {
            get;
            set;
        }

        bool? _IsSelected = false;
        public bool? IsSelected
        {
            get
            {
                return _IsSelected==null?false:_IsSelected;
            }
            set{
                _IsSelected = value;
            }
        }

        public string Selected
        {
            get
            {
                return IsSelected.Value ? "selected" : "";
            }
        }
    }

    public class PrivilegeModulesmodel
    {
        public string ModuleName { get; set; }

        public IEnumerable<EntitySelectItem> Privileges { get; set; }

        public List<PrivilegeModel> UserPrivileges { get; set; }
    }
    public class PrivilegeModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public bool? Deny { get; set; }

        public int UserId { get; set; }
    }

    public class KeyValue
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }
}