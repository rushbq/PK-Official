using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VideoData.Models
{
    /// <summary>
    /// 資料欄位
    /// </summary>
    public class Video
    {
        public int ID { get; set; }
        public string Url { get; set; }
        public string ListPic { get; set; }
        public string ModelName { get; set; }
        public string ModelNo { get; set; }
    }

}

