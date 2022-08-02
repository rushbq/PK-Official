using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using ExtensionMethods;
using Newtonsoft.Json.Linq;

/// <summary>
/// 自訂參數
/// </summary>
public class fn_Param
{
    public static string WebUrl
    {
        get
        {
            return System.Web.Configuration.WebConfigurationManager.AppSettings["WebUrl"];
        }
        private set
        {
            _WebUrl = value;
        }
    }
    private static string _WebUrl;

    public static string CDNUrl
    {
        get
        {
            return System.Web.Configuration.WebConfigurationManager.AppSettings["CDNUrl"];
        }
        private set
        {
            _CDNUrl = value;
        }
    }
    private static string _CDNUrl;

    public static string RefUrl
    {
        get
        {
            return System.Web.Configuration.WebConfigurationManager.AppSettings["RefUrl"];
        }
        private set
        {
            _RefUrl = value;
        }
    }
    private static string _RefUrl;


    public static string Get_TagUrl(string tagName)
    {
        return "{0}Tags/{1}/".FormatThis(WebUrl, HttpUtility.UrlEncode(tagName));
    }

    /// <summary>
    /// 本站資料庫名(目前為線上下單使用)
    /// </summary>
    /// <returns></returns>
    /// <remarks>
    /// 判斷是否為測試模式, 帶不同的DB Name
    /// </remarks>
    public static string Get_DBName()
    {
        string testMode = System.Web.Configuration.WebConfigurationManager.AppSettings["EDITestMode"];

        //判斷是否為測試模試
        if (testMode.Equals("Y"))
        {
            //測試資料庫
            //return "PKWebTest";
            return "PKWebTest";
        }
        else
        {
            //正式資料庫
            return "PKWeb";
        }
    }

    /// <summary>
    /// 回傳國家對應的購買網址
    /// </summary>
    /// <param name="countryCode">國家區碼,TW/CN</param>
    public static string[] Get_BuyUrl(string countryCode)
    {
        //----- 宣告 -----
        StringBuilder sql = new StringBuilder();
        StringBuilder html = new StringBuilder();
        string ErrMsg;

        //----- 資料查詢 -----
        using (SqlCommand cmd = new SqlCommand())
        {
            //----- SQL 查詢語法 -----
            sql.AppendLine(" SELECT Country_Code, Url");
            sql.AppendLine(" FROM Shop_Redirect WITH(NOLOCK)");
            sql.AppendLine(" WHERE (Country_Code = @Country_Code)");

            //----- SQL 執行 -----
            cmd.CommandText = sql.ToString();
            cmd.Parameters.AddWithValue("Country_Code", countryCode);
            using (DataTable DT = dbConn.LookupDT(cmd, out ErrMsg))
            {
                if (DT.Rows.Count > 0)
                {
                    string url = DT.Rows[0]["Url"].ToString();

                    //判斷資料筆數是否為多筆, 若是則跳出Frame_GoBuy頁面 (參數id/name/area)
                    if (DT.Rows.Count == 1)
                    {
                        //單筆(直接導向)
                        return new string[] { "direct", url };
                    }
                    else
                    {
                        //多筆(跳小視窗)
                        return new string[] { "frame"
                            , "{0}Ajax_Data/Frame_GoBuy.aspx?area={1}".FormatThis(
                            System.Web.Configuration.WebConfigurationManager.AppSettings["WebUrl"]
                            , countryCode) };
                    }
                }

                //無資料(不顯示購買連結)
                return new string[] { "none", "" };
            }
        }

    }


    /// <summary>
    /// 組合要導向的網址
    /// </summary>
    /// <param name="url">帶#參數#的網址</param>
    /// <param name="modelNo">品號</param>
    /// <param name="modelName">品名</param>
    /// <returns></returns>
    /// <remarks>
    /// 先使用 Get_BuyUrl 取得網址
    /// </remarks>
    public static string Get_BuyRedirectUrl(string type, string url, string countryCode, string modelNo, string modelName)
    {
        //&id={2}&name={3}
        switch (type.ToLower())
        {
            case "direct":
                //直接導向Url
                string directUrl = url.Replace("#品號#", modelNo);
                //Log Url (LogUrl + 直接導向Url)
                string logUrl = "{0}Redirect.aspx?ActType=buy&id={1}&data={2}&rt={3}".FormatThis(
                    System.Web.Configuration.WebConfigurationManager.AppSettings["WebUrl"]
                    , HttpUtility.UrlEncode(modelNo)
                    , countryCode
                    , HttpUtility.UrlEncode(directUrl)
                    );
                return logUrl;


            case "frame":
                //多筆, 開啟frame小視窗
                return "{0}&id={1}&name={2}".FormatThis(
                    url
                    , HttpUtility.UrlEncode(modelNo)
                    , HttpUtility.UrlEncode(modelName));


            default:
                return "";
        }
    }


    /// <summary>
    /// 從cookie取得國家代碼
    /// 代碼於Global.asax時產生
    /// </summary>
    /// <returns></returns>
    public static string GetCountryCode_fromCookie()
    {
        //讀取cookie
        HttpCookie myInfo = HttpContext.Current.Request.Cookies["PKWeb_FromRegion"];
        if (myInfo == null)
        {
            return "";
        }

        return myInfo.Value;
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
        * [要求網址] http://api.apigurus.com/iplocation/v1.8/locateip?format=json&key=SAK9FCE78Z7Z6Z6925KZ&ip=49.218.100.127
        */
        string ipApiUrl = "http://api.apigurus.com/iplocation/v1.8/locateip?format=json";
        string accessKey = System.Web.Configuration.WebConfigurationManager.AppSettings["IPApi_Key"];
        string clientIP = fn_Extensions.GetIP();
        string apiFullUrl = "{0}&key={1}&ip={2}".FormatThis(ipApiUrl, accessKey, clientIP);

        //無法取得IP
        if (string.IsNullOrEmpty(clientIP))
        {
            return "UnKnown";
        }


        //判斷是否為內部IP, 回傳指定的CountryCode
        string getCode = CheckLocalIP(clientIP);
        if (!string.IsNullOrEmpty(getCode))
        {
            return getCode;
        }


        //API - Get Response
        string response = fn_Extensions.WebRequest_GET(apiFullUrl);

        try
        {
            //Parse Json
            JObject json = JObject.Parse(response);

            //Get Country Code
            string country = json["geolocation_data"]["country_code_iso3166alpha2"].ToString();

            return country;
        }
        catch (Exception ex)
        {
            return ex.Message.ToString() + "__" + response;
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

        /*
            台灣
            192.168.1
            192.168.3
            192.168.168
            192.168.169
            172.16.40
            172.16.50

            上海
            192.168.0
            192.168.4
            192.168.171
            172.16.42
            172.16.52
         */

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


    /// <summary>
    /// 更新&觸發 良興&寶工的同步會員
    /// </summary>
    /// <param name="email"></param>
    /// <returns></returns>
    public static bool SetEclifeMemberStatus(string sDate, string eDate, string email)
    {
        try
        {
            //Key & Url
            string apiKey = fn_Param.EcLife_ApiKey;
            string url = "https://api-proskit.eclife.com.tw/v1/getProskitMember/";
            string token = fn_Param.EcLife_Token;

            //Post Data
            Dictionary<string, string> postHeaders = new Dictionary<string, string>();
            Dictionary<string, string> postParams = new Dictionary<string, string>();

            postHeaders.Add("token", token);

            if (!string.IsNullOrEmpty(sDate))
            {
                postParams.Add("sdate", sDate);
            }

            if (!string.IsNullOrEmpty(eDate))
            {
                postParams.Add("edate", eDate);
            }

            if (!string.IsNullOrEmpty(email))
            {
                postParams.Add("email", email);
            }

            //Get Api Response
            string result = fn_Extensions.WebRequest_POST(false, url, postParams, postHeaders);

            //解析Json
            JObject jData = JObject.Parse(result);

            //Get data
            string status = jData["status"].ToString();

            //
            return true;

        }
        catch (Exception)
        {
            return false;
        }


    }

    /// <summary>
    /// 良興ApiKey
    /// </summary>
    public static string EcLife_ApiKey
    {
        get
        {
            return System.Web.Configuration.WebConfigurationManager.AppSettings["EcLife_ApiKey"];
        }
        private set
        {
            _EcLife_ApiKey = value;
        }
    }
    private static string _EcLife_Token;


    /// <summary>
    /// 良興Api Token
    /// </summary>
    public static string EcLife_Token
    {
        get
        {
            return System.Web.Configuration.WebConfigurationManager.AppSettings["EcLife_Token"];
        }
        private set
        {
            _EcLife_Token = value;
        }
    }


    private static string _EcLife_ApiKey;
    /// <summary>
    /// FB AppID
    /// </summary>
    public static string FB_AppID
    {
        get
        {
            return System.Web.Configuration.WebConfigurationManager.AppSettings["FB_AppID"];
        }
        private set
        {
            _FB_AppID = value;
        }
    }
    private static string _FB_AppID;


    private static string GetMemberInfo(string cookieParam)
    {
        //讀取cookie
        HttpCookie myInfo = HttpContext.Current.Request.Cookies["PKWeb_MemberInfo"];
        if (myInfo == null)
        {
            return "";
        }
        else
        {
            if (myInfo.Values[cookieParam] == null)
            {
                return "";
            }
            else
            {
                return myInfo.Values[cookieParam];
            }
        }
    }

    /// <summary>
    /// 會員編號
    /// </summary>
    public static string MemberID
    {
        get
        {
            return Cryptograph.MD5Decrypt(GetMemberInfo("MemberID"), HttpContext.Current.Application["DesKey"].ToString());
        }
        private set
        {
            _MemberID = value;
        }
    }
    private static string _MemberID;

    /// <summary>
    /// 會員帳號
    /// </summary>
    public static string MemberAcct
    {
        get
        {
            return Cryptograph.MD5Decrypt(GetMemberInfo("MemberAcct"), HttpContext.Current.Application["DesKey"].ToString());
        }
        private set
        {
            _MemberAcct = value;
        }
    }
    private static string _MemberAcct;

    /// <summary>
    /// 變更密碼使用的會員ID
    /// </summary>
    public static string MemberID_ChgPwd
    {
        get
        {
            return GetMemberInfo("MemberID_ChgPwd");
        }
        private set
        {
            _MemberID_ChgPwd = value;
        }
    }
    private static string _MemberID_ChgPwd;

    /// <summary>
    /// 會員姓名
    /// </summary>
    public static string MemberName
    {
        get
        {
            return Cryptograph.MD5Decrypt(GetMemberInfo("MemberName"), HttpContext.Current.Application["DesKey"].ToString());
        }
        private set
        {
            _MemberName = value;
        }
    }
    private static string _MemberName;

    /// <summary>
    /// 會員型態
    /// </summary>
    /// <remarks>
    /// 0:一般使用者 / 1:經銷商
    /// </remarks>
    public static string MemberType
    {
        get
        {
            return GetMemberInfo("MemberType");
        }
        private set
        {
            _MemberType = value;
        }
    }
    private static string _MemberType;

    /// <summary>
    /// 記住我
    /// </summary>
    public static string RememberMe
    {
        get
        {
            return GetMemberInfo("RememberMe");
        }
        private set
        {
            _RememberMe = value;
        }
    }
    private static string _RememberMe;

    /// <summary>
    /// 是否填資料
    /// </summary>
    public static string IsWrite
    {
        get
        {
            return GetMemberInfo("IsWrite");
        }
        private set
        {
            _IsWrite = value;
        }
    }
    private static string _IsWrite;

    /// <summary>
    /// 會員Token
    /// </summary>
    public static string MemberToken
    {
        get
        {
            return GetMemberInfo("Token");
        }
        private set
        {
            _MemberToken = value;
        }
    }
    private static string _MemberToken;



    /// <summary>
    /// 客戶代號
    /// </summary>
    public static string Get_CustID
    {
        get
        {
            String DataID = fn_Member.GetDealerID(fn_Param.MemberID);

            return string.IsNullOrEmpty(DataID) ? "" : DataID;
        }
        set
        {
            _Get_CustID = value;
        }
    }
    private static string _Get_CustID;


    /// <summary>
    /// 系統收件箱
    /// </summary>
    public static string SysMail_Inform
    {
        get
        {
            return System.Web.Configuration.WebConfigurationManager.AppSettings["SysMail_Inform"];
        }
        private set
        {
            _SysMail_Inform = value;
        }
    }
    private static string _SysMail_Inform;
}