using Notice.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Notice.Web
{
    public class ContentBoardModel
    {
        public List<BoardModel> boardModel { get; set; }
        public BoardSearchModel boardSearchModel { get; set; }
    }
}