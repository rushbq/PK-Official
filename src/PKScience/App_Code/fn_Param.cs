using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using Newtonsoft.Json.Linq;
using PKLib_Method.Methods;

/// <summary>
/// 常用參數
/// </summary>
public class fn_Param
{

    /// <summary>
    /// 網站名稱
    /// </summary>
    public static string WebName
    {
        get
        {
            return System.Web.Configuration.WebConfigurationManager.AppSettings["WebName"];
        }
        set
        {
            _WebName = value;
        }
    }
    private static string _WebName;


    /// <summary>
    /// 網站網址
    /// </summary>
    public static string WebUrl
    {
        get
        {
            return System.Web.Configuration.WebConfigurationManager.AppSettings["WebUrl"];
        }
        set
        {
            _WebUrl = value;
        }
    }
    private static string _WebUrl;


    /// <summary>
    /// 檔案路徑
    /// </summary>
    public static string FileUrl
    {
        get
        {
            return System.Web.Configuration.WebConfigurationManager.AppSettings["FileUrl"];
        }
        set
        {
            _FileUrl = value;
        }
    }
    private static string _FileUrl;


    /// <summary>
    /// 檔案目錄
    /// </summary>
    public static string FileFolder
    {
        get
        {
            return System.Web.Configuration.WebConfigurationManager.AppSettings["FileFolder"];
        }
        set
        {
            _FileFolder = value;
        }
    }
    private static string _FileFolder;


    /// <summary>
    /// CDN網址
    /// </summary>
    public static string CDNUrl
    {
        get
        {
            return System.Web.Configuration.WebConfigurationManager.AppSettings["CDNUrl"];
        }
        set
        {
            _CDNUrl = value;
        }
    }
    private static string _CDNUrl;


    /// <summary>
    /// API網址
    /// </summary>
    public static string ApiUrl
    {
        get
        {
            return System.Web.Configuration.WebConfigurationManager.AppSettings["ApiUrl"];
        }
        set
        {
            _ApiUrl = value;
        }
    }
    private static string _ApiUrl;

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



    #region -- EurekAPI --


    /// <summary>
    /// 依國家取得購買網址 - View頁使用
    /// </summary>
    /// <param name="modelNo"></param>
    /// <returns></returns>
    public static string ShopUrl(string modelNo)
    {
        string url = fn_Param.Get_BuyUrl(fn_Param.GetCountryCode_byIP());

        return string.IsNullOrEmpty(url) ? "https://shop.prokits.com.tw/" : url.Replace("#品號#", modelNo);
    }


    /// <summary>
    /// 依國家取得購買網址 - List頁使用
    /// </summary>
    /// <remarks>
    /// 取回後要Replace #品號#
    /// </remarks>
    public static string ShopUrl()
    {
        string url = fn_Param.Get_BuyUrl(fn_Param.GetCountryCode_byIP());

        return string.IsNullOrEmpty(url) ? "https://shop.prokits.com.tw/" : url;
    }

    /// <summary>
    /// 回傳國家對應的購買網址
    /// </summary>
    /// <param name="countryCode">國家區碼,TW/CN</param>
    public static string Get_BuyUrl(string countryCode)
    {
        //----- 宣告 -----
        StringBuilder sql = new StringBuilder();
        StringBuilder html = new StringBuilder();
        string ErrMsg;

        //----- 資料查詢 -----
        using (SqlCommand cmd = new SqlCommand())
        {
            //----- SQL 查詢語法 -----
            sql.AppendLine(" SELECT TOP 1 Country_Code, Url");
            sql.AppendLine(" FROM Shop_Redirect WITH(NOLOCK)");
            sql.AppendLine(" WHERE (Country_Code = @Country_Code)");

            //----- SQL 執行 -----
            cmd.CommandText = sql.ToString();
            cmd.Parameters.AddWithValue("Country_Code", countryCode);
            using (DataTable DT = dbConn.LookupDT(cmd, dbConn.DBS.PKWeb, out ErrMsg))
            {
                if (DT.Rows.Count == 0)
                {
                    return "";
                }
                else
                {
                    return DT.Rows[0]["Url"].ToString();
                }
            }
        }

    }

    /// <summary>
    /// 使用API 從外部取得 IP對應的國家別
    /// </summary>
    /// <returns>Country Code (ex:TW)</returns>
    ///<see cref="https://www.apigurus.com/"/>
    public static string GetCountryCode_byIP()
    {
        /*
        * [參考網站] https://www.apigurus.com/
        * [要求網址] http://api.apigurus.com/iplocation/v1.8/locateip?format=json&key=SAKQJ6VTY69G7WFZLJVZ&ip=49.218.100.127
        */
        string ipApiUrl = "http://api.apigurus.com/iplocation/v1.8/locateip?format=json";
        string accessKey = System.Web.Configuration.WebConfigurationManager.AppSettings["IPApi_Key"];
        string clientIP = CustomExtension.GetIP();
        string apiFullUrl = "{0}&key={1}&ip={2}".FormatThis(ipApiUrl, accessKey, clientIP);

        //無法取得IP
        if (string.IsNullOrEmpty(clientIP))
        {
            return "";
        }


        //判斷是否為內部IP, 回傳指定的CountryCode
        string getCode = CheckLocalIP(clientIP);
        if (!string.IsNullOrEmpty(getCode))
        {
            return getCode;
        }


        //API - Get Response
        string response = CustomExtension.WebRequest_byGET(apiFullUrl);

        try
        {
            //Parse Json
            JObject json = JObject.Parse(response);

            //Get Country Code
            string country = json["geolocation_data"]["country_code_iso3166alpha2"].ToString();

            return country;
        }
        catch (Exception)
        {
            return "";
        }

    }


    /// <summary>
    /// 判斷是否為內部IP, 回傳指定國家別
    /// </summary>
    /// <param name="ip"></param>
    /// <returns></returns>
    private static string CheckLocalIP(string ip)
    {
        if (string.IsNullOrEmpty(ip))
        {
            return "";
        }

        //定義指定網段及國家別
        Dictionary<string, string> dicCode = new Dictionary<string, string>();
        dicCode.Add("1921681", "TW");
        dicCode.Add("1921683", "TW");
        dicCode.Add("192168168", "TW");
        dicCode.Add("192168169", "TW");
        dicCode.Add("1721640", "TW");
        dicCode.Add("1721650", "TW");
        dicCode.Add("1921680", "CN");
        dicCode.Add("1921684", "CN");
        dicCode.Add("192168171", "CN");
        dicCode.Add("1721642", "CN");
        dicCode.Add("1721652", "CN");

        //分割字串
        string[] ipAry = Regex.Split(ip, @"\.{1}");

        //取得IP前3段, 並取成一字串
        string combineIP = "{0}{1}{2}".FormatThis(ipAry[0], ipAry[1], ipAry[2]);

        //查詢符合資料並回傳
        var query = dicCode
            .Where(i => i.Key.Equals(combineIP))
            .Select(i => i.Value).FirstOrDefault();

        return query == null ? "" : query.ToString();
    }



    #endregion
}