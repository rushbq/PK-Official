using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using API_Facebook;
using ExtensionMethods;
using Newtonsoft.Json.Linq;

public partial class oAuth_facebook_callback : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            /*
             * FB登入參考= https://developers.facebook.com/docs/facebook-login/manually-build-a-login-flow
             * 
             * 登入頁 -> 判斷是否登入 -> (未登入) -> 點按FB Login -> 來到此頁
             * 
             * 1.觸發登入對話框, 取得使用者資料
             * 2.fb認證回傳code,使用此code去要求token
             * 3.fb回傳token, 判斷帳號是否存在
             *  3-1.存在, 更新token至會員資料
             *  3-2.不存在, 自動註冊, 更新token至會員資料
             *  3-3.此token可用在API取得資料
             *  
             * FB Dialog授權頁(FB) -> 回傳code -> 使用code取得token(FB) -> 回傳token
             *  -> 使用token取得使用者資訊 -> 判斷帳號是否存在 -> 更新會員token -> 登入完成
            */
            try
            {
                //取得App ID, App Secret, Permission (從db取得Social_Apps)
                string AppID, AppSecret, Perms;
                if (false == fn_Extensions.Get_AppInfo(1, out AppID, out AppSecret, out Perms))
                {
                    Response.Redirect(GoUrl("6"));
                    return;
                }

                //進行認證
                string myOperation = string.IsNullOrEmpty(Request.QueryString["code"]) ? "0" : Request.QueryString["code"].ToString();
                switch (myOperation)
                {
                    case "0":
                        //取得code
                        Go_GetAuth(AppID, Perms);

                        break;

                    default:
                        //判斷是否回傳Error
                        if (Request.QueryString["error_code"] != null)
                        {
                            Response.Redirect(GoUrl("10"));
                            return;
                        }

                        //取得Access Token
                        string myToken = Get_NewToken(AppID, AppSecret, Request.QueryString["code"]);
                    

                        //判斷是否成功取得token
                        if (string.IsNullOrEmpty(myToken))
                        {
                            Response.Redirect(GoUrl("10"));
                            break;
                        }
                        else
                        {
                            //API調用
                            FB_User.Get_UserProfile(myToken, "me");
                            string UserID = FB_User.userId;
                            string Email = FB_User.email;
                            string FirstName = FB_User.first_name;
                            string LastName = FB_User.last_name;
                            string Locale = FB_User.locale;

                            //判斷API是否成功調用
                            if (string.IsNullOrEmpty(UserID))
                            {
                                Response.Redirect(GoUrl("10"));
                                break;
                            }

                            //判斷帳號是否存在
                            if (fn_Member.CheckAccount(Email))
                            {
                                //不存在, 建立帳號, 新增token, 再執行登入
                                if (!fn_Member.AccountProcess(UserID, Email, FirstName, LastName, Locale, myToken, "Facebook", "Create"))
                                {
                                    Response.Redirect(GoUrl("6"));
                                    break;
                                }
                            }
                            else
                            {
                                //判斷是否用社群登入
                                if (false == fn_Member.CheckSocialAccount(UserID))
                                {
                                    Response.Redirect(GoUrl("13"));
                                    break;
                                }
                                else
                                {
                                    //存在, 更新token, 再執行登入
                                    if (!fn_Member.AccountProcess(UserID, Email, FirstName, LastName, Locale, myToken, "Facebook", "Update"))
                                    {
                                        Response.Redirect(GoUrl("6"));
                                        break;
                                    }
                                }
                            }

                        }

                        //Login OK
                        Response.Redirect(Application["WebUrl"].ToString() + "Login");
                        break;

                }
            }
            catch (Exception)
            {

                throw;
            }
        }
    }


    /// <summary>
    /// 前往授權頁,取得Code
    /// </summary>
    /// <param name="appID"></param>
    /// <param name="scope"></param>
    void Go_GetAuth(string appID, string scope)
    {
        string uri = "https://www.facebook.com/dialog/oauth?client_id={0}&redirect_uri={1}&scope={2}&state=1".FormatThis(
                        appID
                        , "{0}oAuth/facebook/callback.aspx".FormatThis(Application["WebUrl"])
                        , scope
                    );

        Response.Redirect(uri);
    }

    /// <summary>
    /// 取得Access Token
    /// </summary>
    /// <param name="appID"></param>
    /// <param name="appSecret"></param>
    /// <param name="code"></param>
    /// <returns></returns>
    /// <remarks>
    /// FB回傳格式:
    /// {
    ///  "access_token": {access-token}, 
    ///  "token_type": {type},
    ///  "expires_in":  {seconds-til-expiration}
    ///}
    /// </remarks>
    string Get_NewToken(string appID, string appSecret, string code)
    {
        string uri = "https://graph.facebook.com/oauth/access_token?client_id={0}&client_secret={1}&redirect_uri={2}&code={3}".FormatThis(
                     appID
                     , appSecret
                     , "{0}oAuth/facebook/callback.aspx".FormatThis(Application["WebUrl"])
                     , code
                 );

        //取得遠端Json
        string GetJson = fn_Extensions.WebRequest_GET(uri);

        //解析Json
        JObject jObject = JObject.Parse(GetJson);

        //回傳token
        return jObject["access_token"].ToString();
    }

    /// <summary>
    /// 訊息頁
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    private string GoUrl(string id)
    {
        return "{0}Notification/{1}".FormatThis(
            Application["WebUrl"].ToString()
            , id
            );
    }

}