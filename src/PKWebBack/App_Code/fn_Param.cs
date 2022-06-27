using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// 常用參數
/// </summary>
public class fn_Param
{
    public readonly static string UserGuid = HttpContext.Current.Session["Login_GUID"].ToString();

    /// <summary>
    /// DesKey
    /// </summary>
    public static string DesKey
    {
        get
        {
            return System.Web.Configuration.WebConfigurationManager.AppSettings["DesKey"];
        }
        set
        {
            _DesKey = value;
        }
    }
    private static string _DesKey;

    /// <summary>
    /// Webrl
    /// </summary>
    public static string Web_Url
    {
        get
        {
            return System.Web.Configuration.WebConfigurationManager.AppSettings["WebUrl"];
        }
        set
        {
            _Web_Url = value;
        }
    }
    private static string _Web_Url;

    /// <summary>
    /// CDN網址
    /// </summary>
    public static string CDN_Url
    {
        get
        {
            return System.Web.Configuration.WebConfigurationManager.AppSettings["CDNUrl"];
        }
        set
        {
            _CDN_Url = value;
        }
    }
    private static string _CDN_Url;


    /// <summary>
    /// 檔案實體路徑
    /// </summary>
    public static string File_DiskUrl
    {
        get
        {
            return System.Web.Configuration.WebConfigurationManager.AppSettings["File_DiskUrl"];
        }
        set
        {
            _File_DiskUrl = value;
        }
    }
    private static string _File_DiskUrl;


    /// <summary>
    /// 檔案網址
    /// </summary>
    public static string File_WebUrl
    {
        get
        {
            return System.Web.Configuration.WebConfigurationManager.AppSettings["File_WebUrl"];
        }
        set
        {
            _File_WebUrl = value;
        }
    }
    private static string _File_WebUrl;

}