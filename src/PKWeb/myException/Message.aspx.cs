using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ExtensionMethods;

public partial class Message : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!IsPostBack)
            {
                switch (Req_DataID)
                {

                    default:
                        this.ph_message.Visible = true;
                        break;
                }


            }

        }
        catch (Exception)
        {
            throw;
        }
    }

    #region -- 參數設定 --
    /// <summary>
    /// 取得傳遞參數 - 資料編號
    /// </summary>
    private string _Req_DataID;
    public string Req_DataID
    {
        get
        {
            String DataID = Page.RouteData.Values["DataID"].ToString();

            return string.IsNullOrEmpty(DataID) ? "" : DataID;
        }
        set
        {
            this._Req_DataID = value;
        }
    }

    /// <summary>
    /// 取得傳遞參數 - 上一頁網址
    /// </summary>
    private string _Req_LastUrl;
    public string Req_LastUrl
    {
        get
        {
            //上一頁網址不存在= 空白, 存在= 解密
            //把多餘的字串取代(Url+error.aspx?404;)
            String url = string.IsNullOrEmpty(Request.QueryString["u"])
                ? ""
                : "<em class=\"text-gray\"><u>{0}</u></em>".FormatThis(
                    Cryptograph.MD5Decrypt(Request.QueryString["u"].ToString(), Application["DesKey"].ToString())
                        .Replace(Application["WebUrl"] + "error.aspx?404;", "")
                        .Replace(":80","")
                    );

            //判斷是否為正確的網址
            if (false == fn_Extensions.IsUrl(url)) url = "";

            return url;
        }
        set
        {
            this._Req_LastUrl = value;
        }
    }
    #endregion
}