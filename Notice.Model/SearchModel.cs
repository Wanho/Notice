using Notice.Data.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notice.Model
{
    public class SearchModel : PageModel
    {
        public SearchModel()
        {
            StartDate = DateTime.Now.AddYears(-1);

            EndDate = DateTime.Now;
        }

        public DateTime StartDate { set; get; }

        public DateTime EndDate { set; get; }

        public string Title { set; get; }

        public string UserId { set; get; }

        public string Area { set; get; }

        public string Word { set; get; }

        public string DateType { set; get; }

        public string SortOrder { set; get; }
        public string SortColumn { set; get; }
        public string SortType { set; get; } = BoardSortType.SortCreateDate.Value;
    }
}
