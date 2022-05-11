using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ExtensionMethods;
using Newtonsoft.Json.Linq;

public partial class myDealer_ProdPhoto : SecurityCheckDealer
{
    public string ErrMsg;

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!IsPostBack)
            {
                //** 次標題 **
                this.Page.Title = Resources.resPublic.home_經銷商專區;

                #region -- PDF AccessToken --

                //[取得API Token]
                string LoginID = System.Web.Configuration.WebConfigurationManager.AppSettings["API_PDFLoginID"];
                string LoginPwd = Cryptograph.MD5(System.Web.Configuration.WebConfigurationManager.AppSettings["API_PDFLoginPwd"]);

                //Get Token Request
                string Url = "{0}GetAccessToken/".FormatThis(Application["API_WebUrl"]);
                string GetTokenJson = fn_Extensions.WebRequest_POST(
                    Url
                    , "LoginID={0}&LoginPwd={1}".FormatThis(LoginID, LoginPwd));

                if (string.IsNullOrEmpty(GetTokenJson))
                {
                    Response.Write("Token取得失敗");
                }

                //解析Json
                JObject jObject = JObject.Parse(GetTokenJson);

                //填入資料
                this.ViewState["tokenID"] = jObject["tokenID"].ToString();

                #endregion
            }

        }
        catch (Exception)
        {

            throw;
        }
    }


    #region -- 參數設定 --
    /// <summary>
    /// 圖片API網址
    /// </summary>
    private string _Param_ImgUrl;
    public string Param_ImgUrl
    {
        get
        {
            return "{0}myProd/".FormatThis(Application["API_WebUrl"]);
        }
        set
        {
            this._Param_ImgUrl = value;
        }
    }


    /// <summary>
    /// 依不同身份產生token
    /// </summary>
    private string _Token;
    public string Token
    {
        get
        {
            return fn_Extensions.Get_MemberToken(fn_Param.MemberType, fn_Param.MemberID);
        }
        set
        {
            this._Token = value;
        }
    }

    /// <summary>
    /// CDN網址
    /// </summary>
    private string _CDNUrl;
    public string CDNUrl
    {
        get
        {
            return System.Web.Configuration.WebConfigurationManager.AppSettings["CDNUrl"];
        }
        set
        {
            this._CDNUrl = value;
        }
    }
    #endregion
}