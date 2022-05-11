using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NewsData.Controllers;
using PKLib_Method.Methods;
using System.Collections;

public partial class myNews_NewsView : System.Web.UI.Page
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
        NewsRepository _data = new NewsRepository();


        //----- 原始資料:取得所有資料 -----
        var data = _data.GetDetail(Req_DataID);

        if (data != null)
        {
            var topData = data.FirstOrDefault();

            meta_Title = topData.TopTitle;
            meta_Desc = topData.TopDesc;
            meta_Url = PageUrl;
            meta_Image = topData.ListPic;

            this.lt_Header.Text = topData.TopTitle;

            topData = null;
        }


        //----- 資料整理:繫結 ----- 
        this.lvDataList.DataSource = data;
        this.lvDataList.DataBind();
    }


    protected void lvDataList_ItemDataBound(object sender, ListViewItemEventArgs e)
    {
        if (e.Item.ItemType == ListViewItemType.DataItem)
        {
            ListViewDataItem dataItem = (ListViewDataItem)e.Item;

            //判斷是否有圖片
            string Get_Img = DataBinder.Eval(dataItem.DataItem, "Pic").ToString();

            //取得控制項
            if (!string.IsNullOrEmpty(Get_Img))
            {
                Literal lt_Img = (Literal)e.Item.FindControl("lt_Img");
                lt_Img.Text = "<img class=\"news-img responsive-img lazy\" src=\"{0}\" alt=\"News Image\" />"
                    .FormatThis(Get_Img);
            }

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
            return "{0}{1}/NewsPage/{2}".FormatThis(webUrl, Req_Lang, Req_DataID);
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