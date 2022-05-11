using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AdvData.Models
{
    /// <summary>
    /// 資料欄位
    /// </summary>
    public class Adv
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public string Target { get; set; }
        public string Url { get; set; }
        public string ImgUrl { get; set; }
    }
}

