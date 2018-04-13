using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EBP.Business.Filter
{
    public class FilterMarketing : SelectFilterBase
    {
        public FilterMarketing()
        {
            DocumentTypeIds = new int[] { };
            CategoryIds = new int[] { };
            UserIds = new int[] { };
        }

        public int[] _DocumentTypeIds;
        public int[] _CategoryIds;
        public int[] _UserIds;
        public int[] DocumentTypeIds
        {
            get
            {
                if ((_DocumentTypeIds == null || _DocumentTypeIds.Count() == 0) && DocumentTypeId.HasValue)
                    _DocumentTypeIds = new int[] { DocumentTypeId.Value };

                return _DocumentTypeIds;
            }


            set { _DocumentTypeIds = value; }
        }
        public int[] CategoryIds
        {
            get
            {
                if ((_CategoryIds == null || _CategoryIds.Count() == 0) && CategoryId.HasValue)
                    _CategoryIds = new int[] { CategoryId.Value };

                return _CategoryIds;
            }


            set { _CategoryIds = value; }
        }
        public int[] UserIds
        {
            get
            {
                if ((_UserIds == null || _UserIds.Count() == 0) && UserId.HasValue)
                    _UserIds = new int[] { UserId.Value };

                return _UserIds;
            }


            set { _UserIds = value; }
        }

        public int? DocumentTypeId { get; set; }

        public int? CategoryId { get; set; }

        public int? UserId { get; set; }

        public string Keyword { get; set; }
    }
}
