using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Models.Common
{
    public class BaseRequest
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public string SortDir { get; set; }
        public string SortCol { get; set; }
        public string Keyword { get; set; }
    }
}
