using Notice.Data.Core;
using System;

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

        public string CreateID { set; get; }

        public string Area { set; get; }

        public string Word { set; get; }

        public string DateType { set; get; }

        public SortOrder SortOrder { set; get; } = SortOrder.Desc;
        public string SortColumn { set; get; }
        public string SortType { set; get; }
    }
}
