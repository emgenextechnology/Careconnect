﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EBP.Business.Filter
{
    public class FilterRoles : SelectFilterBase
    {
        public string KeyWords { get; set; }

        public string SortOrder { get; set; }
    }
}