using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notice.Model
{
    public class RangeModel
    {
        public int RowStart { set; get; }

        public int RowEnd { set; get; }
    }

    public class PageModel
    {
        public long TotalCount { set; get; } = 0;
        public int PageSize { set; get; } = 10;
        public int PageNumber { set; get; } = 1;


        public RangeModel GetRange()
        {
            return new RangeModel
            {
                RowStart = (PageNumber - 1) * PageSize,
                RowEnd = PageNumber * PageSize
            };
        }
    }
}
