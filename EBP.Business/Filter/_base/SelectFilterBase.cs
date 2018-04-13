using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EBP.Business.Filter
{
    public class SelectFilterBase
    {
        int? pageSize = null;
        int? currentPage = null;

        public string SortKey { get; set; }
        public int PageSize
        {
            get
            {
                if (pageSize.HasValue)
                    return pageSize.Value;

                return 10;

            }
            set { pageSize = value; }
        }

        public int CurrentPage
        {
            get
            {
                if (currentPage.HasValue)
                    return currentPage.Value;
                return 1;
            }
            set { currentPage = value; }
        }

        public int Skip { get { return (CurrentPage - 1) * PageSize; } }

        public int Take { get { return PageSize; } }
    }
}
