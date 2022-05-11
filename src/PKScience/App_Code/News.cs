using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NewsData.Models
{
    /// <summary>
    /// 資料欄位
    /// </summary>
    public class News
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public string Desc { get; set; }
        public string ListPic { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
    }

    public class NewsDetail
    {
        public int ID { get; set; }
        public string TopTitle { get; set; }
        public string TopDesc { get; set; }
        public string ListPic { get; set; }
        public string Desc { get; set; }
        public string Pic { get; set; }
    }
}

