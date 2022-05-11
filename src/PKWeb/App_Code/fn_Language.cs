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
    public static string PKWeb_Lang
    {
        get
        {
            return HttpContext.Current.Request.Cookies["PKWeb_Lang"] != null ?
              HttpContext.Current.Request.Cookies["PKWeb_Lang"].Value.ToString() :
              "en-US";
        }
        private set
        {
            _PKWeb_Lang = value;
        }
    }
    private static string _PKWeb_Lang;


    /// <summary>
    /// 參數用語系, "-" 改 "_"
    /// DB語系欄位需開成像 xxx_zh_TW 的欄位名
    /// </summary>
    public static string Param_Lang
    {
        get
        {
            return PKWeb_Lang.Replace("-", "_");
        }
        private set
        {
            _Param_Lang = value;
        }
    }
    private static string _Param_Lang;
}