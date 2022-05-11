using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ExtensionMethods;

public partial class LoginFail : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            //輸出錯誤代碼
            this.lb_ErrCode.Text = "{0}<br/>{1}".FormatThis(
                Req_ErrCode
                , fn_Desc.Login.ErrCode(Req_ErrCode)
                );

            //清除Cookie
            HttpCookie myCookie = new HttpCookie("PK_UserSID");
            myCookie.Expires = DateTime.Now.AddDays(-1d);
            Response.Cookies.Add(myCookie);

            HttpCookie myCookie_Remb = new HttpCookie("PK_Remember");
            myCookie_Remb.Expires = DateTime.Now.AddDays(-1d);
            Response.Cookies.Add(myCookie_Remb);
        }
    }

    #region -- 按鈕事件 --
    /// <summary>
    /// 重登
    /// </summary>
    protected void lbtn_Login_Click(object sender, EventArgs e)
    {
        Response.Redirect("{0}ServiceLogin/".FormatThis(Application["WebUrl"]));
    }
    #endregion

    #region -- 參數設定 --
    /// <summary>
    /// 取得傳遞參數 - ErrCode
    /// </summary>
    private string _Req_ErrCode;
    public string Req_ErrCode
    {
        get
        {
            String ErrCode = Convert.ToString(Page.RouteData.Values["ErrCode"]);
            return ErrCode;
        }
        set
        {
            this._Req_ErrCode = value;
        }
    }

    #endregion
}