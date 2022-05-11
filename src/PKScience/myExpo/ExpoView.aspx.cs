using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ExpoData.Controllers;
using PKLib_Method.Methods;
using System.Collections;

public partial class myProd_ExpoView : System.Web.UI.Page
{
    public string cdnUrl = fn_Param.CDNUrl;
    public string webUrl = fn_Param.WebUrl;
    public string webName = fn_Param.WebName;

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!IsPostBack)
            {
                //隱藏主頁的meta
                PlaceHolder myMeta = (PlaceHolder)Master.FindControl("ph_MetaInfo");
                myMeta.Visible = false;

                //Get Data
                LookupDataList();

            }

        }
        catch (Exception)
        {

            throw;
        }
    }


    #region -- 資料顯示 --

    /// <summary>
    /// 取得資料
    /// </summary>
    private void LookupDataList()
    {
        //----- 宣告:資料參數 -----
        ExpoRepository _data = new ExpoRepository();


        //----- 原始資料:取得所有資料 -----
        var data = _data.GetOne(Req_DataID, fn_Language.Web_Lang).FirstOrDefault();

        if (data != null)
        {
            meta_Title = data.Title;
            meta_Desc = data.Desc;
            meta_Url = PageUrl;
            meta_Image = data.ListPic;

            this.lt_Header.Text = data.Title;
            this.lt_Content.Text = "<a href=\"{0}\" target=\"_blank\"><img class=\"acivity-img center-align acivity-img responsive-img\" src=\"{1}\" alt=\"\" /></a>"
                .FormatThis(
                    data.Url
                    , data.BigPic
                );
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
            string myData = Page.RouteData.Values["lang"].ToString();

            //若為auto, 就去抓cookie
            return myData.Equals("auto") ? fn_Language.Get_Lang(Request.Cookies["PKScience_Lang"].Value) : myData;
        }
        set
        {
            this._Req_Lang = value;
        }
    }
    private string _Req_Lang;


    /// <summary>
    /// 取得傳遞參數 - 資料編號
    /// </summary>
    private string _Req_DataID;
    public string Req_DataID
    {
        get
        {
            String DataID = Page.RouteData.Values["id"].ToString();

            return string.IsNullOrEmpty(DataID) ? "" : DataID;
        }
        set
        {
            this._Req_DataID = value;
        }
    }


    /// <summary>
    /// 本頁網址
    /// </summary>
    private string _PageUrl;
    public string PageUrl
    {
        get
        {
            return "{0}{1}/Activity/{2}".FormatThis(webUrl, Req_Lang, Req_DataID);
        }
        set
        {
            this._PageUrl = value;
        }
    }


    public string meta_Title
    {
        get;
        set;
    }

    public string meta_Desc
    {
        get;
        set;
    }

    public string meta_Url
    {
        get;
        set;
    }

    public string meta_Image
    {
        get;
        set;
    }

    #endregion
}