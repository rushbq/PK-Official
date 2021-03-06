using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// 取得語系
/// </summary>
public class fn_Language
{
    /// <summary>
    /// 目前語系 - Cookie
    /// 若Cookie不存在，自動帶預設語系 en-US
    /// </summary>
    private static string _PKReport_Lang;
    public static string PKReport_Lang
    {
        get
        {
            return HttpContext.Current.Request.Cookies["PKReport_Lang"] != null ?
              HttpContext.Current.Request.Cookies["PKReport_Lang"].Value.ToString() :
              "en-US";
        }
        private set
        {
            _PKReport_Lang = value;
        }
    }

    /// <summary>
    /// 參數用語系, "-" 改 "_"
    /// DB語系欄位需開成像 xxx_zh_TW 的欄位名
    /// </summary>
    private static string _Param_Lang;
    public static string Param_Lang
    {
        get
        {
            return PKReport_Lang.Replace("-", "_");
        }
        private set
        {
            _Param_Lang = value;
        }
    }
}