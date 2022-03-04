using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Notice.Model;

namespace Notice.Web
{
    public class ContentBoardModel
    {
        public List<BoardItemModel> boardItems { get; set; }
        public BoardSearchModel boardSearch { get; set; }
    }
}