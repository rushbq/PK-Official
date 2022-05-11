using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using PKLib_Method.Methods;

public partial class Site : System.Web.UI.MasterPage
{
    public string cdnUrl = fn_Param.CDNUrl;
    public string webUrl = fn_Param.WebUrl;
    public string webName = fn_Param.WebName;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            try
            {
                //判斷 & 轉換語系
                Check_Lang();

                //agree btn
                this.lbtn_Agree.Text = Resources.resPublic.btn_Yes;
                /*
                * 判斷COOKIE, cookieAgree是否存在
                */
                HttpCookie cAgree = Request.Cookies["PKScience_CkAgree"];
                this.ph_CookiePrivacy.Visible = cAgree == null ? true : false;

                /*
                 * 20191219:禁用en, (國外版權問題)
                 * 20211124:要求開放 (#163769086456)
                 */
                //if (Req_Lang.ToLower().Equals("en"))
                //{
                //    Response.Redirect(fn_Param.WebUrl + "tw");
                //}
            }
            catch (Exception)
            {
                throw;
            }
        }
    }

    /// <summary>
    /// Cookie privacy agree (PKScience_CkAgree)
    /// </summary>
    protected void lbtn_Agree_Click(object sender, EventArgs e)
    {
        /*
         * 設定COOKIE, cookieAgree是否存在, 並設為6個月
         */
        Response.Cookies.Add(new HttpCookie("PKScience_CkAgree", DateTime.Now.ToString().ToDateString("yyyyMMddhhmmss")));
        Response.Cookies["PKScience_CkAgree"].Expires = DateTime.Now.AddMonths(6);

        //redirect
        Response.Redirect("{0}{1}/PrivacyCookies".FormatThis(webUrl, Req_Lang));
    }


    #region -- 語系處理 --

    /// <summary>
    /// 判斷 & 轉換語系
    /// </summary>
    private void Check_Lang()
    {
        //取得目前語系cookie
        HttpCookie cLang = Request.Cookies["PKScience_Lang"];
        //將傳來的參數,轉換成完整語系參數
        string langCode = fn_Language.Get_LangCode(Req_Lang);

        //判斷傳入語系是否與目前語系相同, 若不同則執行語系變更
        if (!cLang.Value.ToUpper().Equals(langCode.ToUpper()))
        {
            //重新註冊cookie
            Response.Cookies.Remove("PKScience_Lang");
            Response.Cookies.Add(new HttpCookie("PKScience_Lang", langCode));
            Response.Cookies["PKScience_Lang"].Expires = DateTime.Now.AddYears(1);

            //語系變換
            System.Globalization.CultureInfo currentInfo = new System.Globalization.CultureInfo(langCode);
            System.Threading.Thread.CurrentThread.CurrentCulture = currentInfo;
            System.Threading.Thread.CurrentThread.CurrentUICulture = currentInfo;

            //redirect
            Response.Redirect(Request.Url.AbsoluteUri);
        }
    }



    #endregion


    #region -- 參數設定 --

    /// <summary>
    /// 取得傳遞參數 - 語系
    /// </summary>
    public string Req_Lang
    {
        get
        {
            string myLang = Page.RouteData.Values["lang"] == null ? "" : Page.RouteData.Values["lang"].ToString();

            //若為auto, 就去抓cookie
            return myLang.Equals("auto") ? fn_Language.Get_Lang(Request.Cookies["PKScience_Lang"].Value) : myLang;
        }
        set
        {
            this._Req_Lang = value;
        }
    }
    private string _Req_Lang;




    #endregion

}
