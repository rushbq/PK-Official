using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// 取得區域
/// </summary>
public class fn_Area
{
    /// <summary>
    /// 目前區域 - Cookie
    /// 若Cookie不存在，自動帶預設區域 (全球 = 1)
    /// </summary>
    private static string _PKWeb_Area;
    public static string PKWeb_Area
    {
        get
        {
            //return HttpContext.Current.Request.Cookies["PKWeb_Area"] != null ?
            //  HttpContext.Current.Request.Cookies["PKWeb_Area"].Value.ToString() :
            //  "1";
            return "1";
        }
        private set
        {
            _PKWeb_Area = value;
        }
    }
}