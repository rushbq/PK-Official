using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ExpoData.Models
{
    /// <summary>
    /// 資料欄位
    /// </summary>
    public class Expo
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public string SubTitle { get; set; }
        public string Desc { get; set; }
        public string Url { get; set; }
        public string ListPic { get; set; }
        public string BigPic { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
    }
}

