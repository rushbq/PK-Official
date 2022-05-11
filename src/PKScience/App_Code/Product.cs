using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProductData.Models
{
    /// <summary>
    /// 資料欄位
    /// </summary>
    public class Product
    {
        public int ID { get; set; }
        public string ModelNo { get; set; }
        public string ModelName { get; set; }
        public string ShopUrl { get; set; }
        public string IsNew { get; set; }
        public string ListPic { get; set; }
        public string ListDesc { get; set; }
        public string ShareTitle { get; set; }
        public string ShareDesc { get; set; }
        public string PicGroup { get; set; }
        public int TypeID { get; set; }
        public string TypeName { get; set; }
        public string Url1 { get; set; }
        public string Url2 { get; set; }

        public string FullDesc { get; set; }
        public string ShortDesc { get; set; }
        public string SeoDesc { get; set; }

    }


    /// <summary>
    /// 關鍵字欄位
    /// </summary>
    public class Tags
    {
        public int TagID { get; set; }
        public string TagName { get; set; }
        public string ModelNo { get; set; }
    }


    /// <summary>
    /// 說明書下載欄位
    /// </summary>
    public class Manual
    {
        public string DwFile { get; set; }
        public string DwFileName { get; set; }
        public string LangName { get; set; }
        public Int16 ClassID { get; set; }
    }
}
