using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EBP.Business.Entity
{
    public class DataPager
    {
        public int? Take { get; set; }

        public int? Skip { get; set; }

        public int TotalCount { get; set; }

        public int TotalPage
        {
            get
            {
                var totalpage = 1;
                if (Take.HasValue && Take != 0)
                {
                    totalpage = TotalCount / Take.Value;

                    if (TotalCount % Take.Value > 0)
                        totalpage++;
                }
                return totalpage;
            }
        }

    }
}
