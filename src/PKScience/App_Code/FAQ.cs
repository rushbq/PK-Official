using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAQData.Models
{
    /// <summary>
    /// 資料欄位
    /// </summary>
    public class FAQ
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public int ClassID { get; set; }
        public string ClassName { get; set; }
        public Int16 ClassSort { get; set; }

    }

    public class FAQDetail
    {
        public int GroupID { get; set; }
        public int ID { get; set; }
        public string TopTitle { get; set; }
        public string TopClass { get; set; }
        public int TopClassID { get; set; }
        public string Desc { get; set; }
        public string Pic { get; set; }
    }

    public class FAQProd
    {
        public string ModelNo { get; set; }
        public string ModelName { get; set; }
    }
}

