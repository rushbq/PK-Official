using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ExtensionMethods;
using Newtonsoft.Json.Linq;

/*
 * 使用微博 API
 * 取得欄位時，注意相關權限是否已得到授權
 */
namespace API_Weibo
{
    /// <summary>
    /// 取得AccessToken資訊
    /// </summary>
    /// <see cref="http://open.weibo.com/wiki/Oauth2/access_token"/>
    public class WB_AccessToken
    {
        public static void Get_Token(string appID, string appSecret, string backUrl, string code)
        {
            //API網址
            string uri = "{0}oauth2/access_token".FormatThis(Pub_Param.weibo_Url);
            string param = "client_id={0}&client_secret={1}&redirect_uri={2}&code={3}&grant_type=authorization_code".FormatThis(
                         appID
                         , appSecret
                         , backUrl
                         , code
                     ); ;

            //取得遠端Json
            string GetJson = fn_Extensions.WebRequest_POST(uri, param);

            //判斷是否為空(網址有誤或未授權)
            if (string.IsNullOrEmpty(GetJson))
            {
                return;
            }

            //解析Json
            JObject jObject = JObject.Parse(GetJson);

            //填入資料
            access_token = jObject["access_token"].ToString();
            expires_in = jObject["expires_in"].ToString();
            userId = jObject["uid"].ToString();
        }

        public static string access_token { get; set; }

        /// <summary>
        /// 生命週期(單位:秒數)
        /// </summary>
        public static string expires_in { get; set; }

        /// <summary>
        /// 用戶UID
        /// </summary>
        public static string userId { get; set; }
    }


    /// <summary>
    /// 取得人員資料
    /// 需要授權:email
    /// </summary>
    /// <example>
    /// WB_User.Get_UserProfile(token, "1086533281");
    /// Response.Write(WB_User.userId);
    /// </example>
    public class WB_User
    {
        /// <summary>
        /// 取得人員
        /// </summary>
        /// <param name="token"></param>
        /// <param name="id"></param>
        /// <see cref="http://open.weibo.com/wiki/2/users/show"/>
        public static void Get_UserProfile(string token, string id)
        {
            //API網址
            string uri = "{0}2/users/show.json?access_token={1}&uid={2}".FormatThis(
                    Pub_Param.weibo_Url
                    , token
                    , id
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
            first_name = "";
            last_name = jObject["name"].ToString();
            locale = "";

            //Email權限未申請成功, 先用Guid配置一個假的Email
            email = userId + "@weibo.com";
        }

        ///// <summary>
        ///// 取得email
        ///// </summary>
        ///// <param name="token"></param>
        ///// <see cref="http://open.weibo.com/wiki/2/account/profile/email"/>
        //public static void Get_Email(string token)
        //{
        //    //https://api.weibo.com/2/account/profile/email.json
        //}

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
        private static string _weibo_Url;
        public static string weibo_Url
        {
            get
            {
                return "https://api.weibo.com/";
            }
            set
            {
                _weibo_Url = value;
            }
        }
    }


}

