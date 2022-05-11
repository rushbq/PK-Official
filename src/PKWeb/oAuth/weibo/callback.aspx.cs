using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using API_Weibo;
using ExtensionMethods;

public partial class oAuth_weibo_callback : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            /*
             * 登入頁 -> 判斷是否登入 -> (未登入) -> 點按Weibo Login -> 來到此頁
             * 
             * 1.觸發登入對話框, 取得使用者資料
             * 2.Weibo認證回傳code,使用此code去要求token
             * 3.Weibo回傳token, 判斷帳號是否存在
             *  3-1.存在, 更新token至會員資料
             *  3-2.不存在, 自動註冊, 更新token至會員資料
             *  3-3.此token可用在API取得資料
             *  
             * Weibo Dialog授權頁(Weibo) -> 回傳code -> 使用code取得token(Weibo) -> 回傳token
             *  -> 使用token取得使用者資訊 -> 判斷帳號是否存在 -> 更新會員token -> 登入完成
            */
            /*
             * 20150828 註釋
             * 因微博取得Email需要高級接口權限，但高級接口的申請需要app使用人數達1萬人以上，
             * 故先排除取得Email功能，並使用微博用戶的Guid當作Email帳號。
             * 修改 WB_User.Get_UserProfile 
             */
            try
            {
                //取得App ID, App Secret, Permission, 注意UID
                string AppID, AppSecret, Perms;
                if (false == fn_Extensions.Get_AppInfo(2, out AppID, out AppSecret, out Perms))
                {
                    Response.Redirect(GoUrl("6"));
                    return;
                }

                //進行認證
                string myOperation = string.IsNullOrEmpty(Request.QueryString["op"]) ? "0" : Request.QueryString["op"].ToString();
                switch (myOperation)
                {
                    case "0":
                        //取得code
                        Go_GetAuth(AppID, Perms);

                        break;

                    case "1":
                        //判斷是否回傳Error
                        if (Request.QueryString["error"] != null)
                        {
                            Response.Redirect(GoUrl("10"));
                            return;
                        }

                        //取得Access Token
                        string backUrl = HttpUtility.UrlEncode("{0}oAuth/weibo/callback.aspx?op=1".FormatThis(
                            Application["WebUrl"]));

                        //API調用
                        WB_AccessToken.Get_Token(AppID, AppSecret, backUrl, Request.QueryString["code"]);

                        string myToken = WB_AccessToken.access_token;

                        //判斷是否成功取得token
                        if (string.IsNullOrEmpty(myToken))
                        {
                            //失敗
                            Response.Redirect(GoUrl("10"));
                            break;
                        }
                        else
                        {
                            //UserID
                            string UserID = WB_AccessToken.userId;

                            //使用token取得會員資料
                            WB_User.Get_UserProfile(myToken, UserID);
                            string FirstName = WB_User.first_name;
                            string LastName = WB_User.last_name;
                            string Locale = WB_User.locale;
                            string Email = WB_User.email;

                            //判斷API是否成功調用
                            if (string.IsNullOrEmpty(UserID))
                            {
                                //失敗
                                Response.Redirect(GoUrl("10"));
                                break;
                            }
                         
                            //判斷帳號是否存在
                            if (fn_Member.CheckAccount(Email))
                            {
                                //不存在, 建立帳號, 新增token, 再執行登入
                                if (!fn_Member.AccountProcess(UserID, Email, FirstName, LastName, Locale, myToken, "Weibo", "Create"))
                                {
                                    //失敗
                                    Response.Redirect(GoUrl("6"));
                                    break;
                                }
                            }
                            else
                            {
                                //存在, 更新token, 再執行登入
                                if (!fn_Member.AccountProcess(UserID, Email, FirstName, LastName, Locale, myToken, "Weibo", "Update"))
                                {
                                    //失敗
                                    Response.Redirect(GoUrl("6"));
                                    break;
                                }
                            }

                        }

                        //Login OK
                        //Response.Redirect(Application["WebUrl"].ToString());
                        fn_Extensions.JsAlert("", "script:parent.location.reload()");
                        break;


                    default:
                        Response.Redirect(GoUrl("10"));
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
        string uri = "https://api.weibo.com/oauth2/authorize?client_id={0}&redirect_uri={1}&scope={2}&response_type=code".FormatThis(
                     appID
                     , "{0}oAuth/weibo/callback.aspx?op=1".FormatThis(Application["WebUrl"])
                     , scope
                 );

        Response.Redirect(uri);
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