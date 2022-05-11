using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ExtensionMethods;
using Newtonsoft.Json.Linq;

/*
 * 使用Facebook Graph API
 * 取得欄位時，注意相關權限是否已得到授權
 */
namespace API_Facebook
{
    /// <summary>
    /// 取得人員資料
    /// 需要授權:public_profile,email
    /// </summary>
    /// <example>
    /// FB_User.Get_UserProfile(token, "1086533281");
    /// Response.Write(FB_User.userId);
    /// </example>
    /// <see cref="https://developers.facebook.com/docs/graph-api/reference/user"/>
    public class FB_User
    {
        public static void Get_UserProfile(string token, string id)
        {
            //API網址
            string uri = "{0}{2}?access_token={1}&fields={3}".FormatThis(
                    Pub_Param.facebook_Url
                    , token
                    , string.IsNullOrEmpty(id) ? "me" : id
                    , "first_name,last_name,email"
                );

            //取得遠端Json
            string GetJson = fn_Extensions.WebRequest_GET(uri);

            //判斷是否為空(網址有誤或未授權)
            if (string.IsNullOrEmpty(GetJson))
            {
                return;
            }

            //解析Json
            JObject jObject = JObject.Parse(GetJson);

            //填入資料
            userId = jObject["id"].ToString();
            email = (jObject["email"] == null) ? jObject["id"].ToString() + "@facebook.com" : jObject["email"].ToString();
            first_name = jObject["first_name"].ToString();
            last_name = jObject["last_name"].ToString();
            locale = "FB"; //locale 已停用
        }

        public static string userId { get; set; }
        public static string email { get; set; }
        public static string first_name { get; set; }
        public static string last_name { get; set; }
        public static string locale { get; set; }
    }


    /// <summary>
    /// 共用參數
    /// </summary>
    public class Pub_Param
    {
        private static string _facebook_Url;
        public static string facebook_Url
        {
            get
            {
                return "https://graph.facebook.com/";
            }
            set
            {
                _facebook_Url = value;
            }
        }
    }


}

