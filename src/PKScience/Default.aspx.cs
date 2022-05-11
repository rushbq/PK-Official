using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using AdvData.Controllers;
using ProductData.Controllers;
using VideoData.Controllers;
using PKLib_Method.Methods;
using NewsData.Controllers;
using ExpoData.Controllers;

public partial class _Default : System.Web.UI.Page
{
    public string cdnUrl = fn_Param.CDNUrl;
    public string webUrl = fn_Param.WebUrl;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            try
            {
                //顯示廣告
                Get_AdvList();

                //顯示影片
                Get_Video();

                //顯示新貨
                Get_NewProdList();

                //最新消息
                Get_News();

                //最新活動
                Get_Expo();


            }
            catch (Exception)
            {

                throw;
            }
        }
    }


    #region -- 取得資料 --

    /// <summary>
    /// 廣告
    /// </summary>
    private void Get_AdvList()
    {
        //----- 宣告:資料參數 -----
        AdvRepository _data = new AdvRepository();

        //----- 原始資料:取得所有資料 -----
        var advList = _data.GetAdvs(searchType.橫幅廣告, fn_Language.Web_Lang);

        //----- 資料整理:繫結 ----- 
        this.lvAdvList.DataSource = advList;
        this.lvAdvList.DataBind();
    }


    /// <summary>
    /// 影片
    /// </summary>
    private void Get_Video()
    {
        //----- 宣告:資料參數 -----
        VideoRepository _data = new VideoRepository();
        Dictionary<int, string> search = new Dictionary<int, string>();

        search.Add((int)VideoData.Controllers.mySearch.IsIndex, "Y");

        //----- 原始資料:取得所有資料 -----
        var dataList = _data.GetVideos(search, fn_Language.Web_Lang, 1);

        //----- 資料整理:繫結 ----- 
        var showData = dataList.FirstOrDefault();
        if (showData != null)
        {
            this.ph_Video.Visible = true;
            this.lt_Video.Text = "<iframe width=\"853\" height=\"480\" src=\"{0}\" frameborder=\"0\" allowfullscreen=\"\"></iframe>"
                .FormatThis(showData.Url);
        }
    }


    /// <summary>
    /// 熱銷商品
    /// </summary>
    private void Get_NewProdList()
    {
        //----- 宣告:資料參數 -----
        ProductRepository _data = new ProductRepository();
        Dictionary<int, string> search = new Dictionary<int, string>();

        search.Add((int)ProductData.Controllers.mySearch.IsHot, "Y");

        //----- 原始資料:取得所有資料 -----
        var prodList = _data.GetProducts(search, fn_Language.Web_Lang, 9);

        //----- 資料整理:繫結 ----- 
        this.lvNewProdList.DataSource = prodList;
        this.lvNewProdList.DataBind();
    }

    protected void lvNewProdList_ItemDataBound(object sender, ListViewItemEventArgs e)
    {
        if (e.Item.ItemType == ListViewItemType.DataItem)
        {
            ListViewDataItem dataItem = (ListViewDataItem)e.Item;

            //取得控制項
            Panel pl_ProdItem = (Panel)e.Item.FindControl("pl_ProdItem");
            int rowIdx = dataItem.DataItemIndex + 1;

            //取得對應顏色
            pl_ProdItem.CssClass = Get_ItemCss(rowIdx);


            ////取得資料
            //string GetDisp = DataBinder.Eval(dataItem.DataItem, "Display").ToString();
            //string GetIsStop = DataBinder.Eval(dataItem.DataItem, "IsStop").ToString();
            //string GetIsNew = DataBinder.Eval(dataItem.DataItem, "IsNew").ToString();

        }
    }

    /// <summary>
    /// 取得對應位置的css顏色
    /// </summary>
    /// <param name="idx"></param>
    /// <returns></returns>
    private string Get_ItemCss(int idx)
    {
        switch (idx)
        {
            case 1:
            case 4:
            case 7:
                return "yellow-section";

            case 2:
            case 5:
            case 8:
                return "soil-section";

            default:
                return "orange-section";
        }
    }


    /// <summary>
    /// 最新消息
    /// </summary>
    private void Get_News()
    {
        //----- 宣告:資料參數 -----
        NewsRepository _data = new NewsRepository();
        Dictionary<int, string> search = new Dictionary<int, string>();

        search.Add((int)NewsData.Controllers.mySearch.onIndex, "Y");

        //----- 原始資料:取得資料 -----
        var dataList = _data.GetNews(search, fn_Language.Web_Lang);

        //----- 資料整理:繫結 ----- 
        this.myNews.DataSource = dataList;
        this.myNews.DataBind();
    }


    /// <summary>
    /// 最新活動
    /// </summary>
    private void Get_Expo()
    {
        //----- 宣告:資料參數 -----
        ExpoRepository _data = new ExpoRepository();
        Dictionary<int, string> search = new Dictionary<int, string>();

        search.Add((int)ExpoData.Controllers.mySearch.onIndex, "Y");

        //----- 原始資料:取得資料 -----
        var dataList = _data.GetExpos(search, fn_Language.Web_Lang);

        //----- 資料整理:繫結 ----- 
        this.myActivity.DataSource = dataList;
        this.myActivity.DataBind();
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
            string myLang = Page.RouteData.Values["lang"] == null ? "auto" : Page.RouteData.Values["lang"].ToString();

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