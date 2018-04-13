using EBP.NotificationApp.Entity;
using EBP.NotificationApp.Entity._base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EBP.NotificationApp.Entity._base
{
    public class EntityList<T>
    {
        DataPager pager = null;
        private List<T> _Model;
        public List<T> List
        {
            get
            {
                if (_Model == null)
                    _Model = new List<T>();

                return _Model;
            }
            set { _Model = value; }
        }
        public DataPager Pager
        {
            get
            {
                if (pager == null) pager = new DataPager();

                return pager;
            }
            set { pager = value; }
        }
        public string SortAscBy { get; set; }
        public string SortDescBy { get; set; }
    }






}


public static class ToNameArrayExt
{
    public static string[] ToNameArray(this EntityList<EntitySelectItem> model)
    {
        if (model == null)
            return new string[] { };

        else
            return model.List.Select(a => a.Text).ToArray();
    }
}
