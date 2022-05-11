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

public partial class myDealer_ProdCert : SecurityCheckDealer
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