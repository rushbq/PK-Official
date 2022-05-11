using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using PKLib_Method.Methods;
using ProductData.Controllers;

public partial class myProd_ProdSearch : System.Web.UI.Page
{
    public string cdnUrl = fn_Param.CDNUrl;
    public string webUrl = fn_Param.WebUrl;
    public string shopUrl = fn_Param.ShopUrl();
    public string ErrMsg;

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!IsPostBack)
            {
                //[取得/檢查參數] - Keyword
                if (!string.IsNullOrEmpty(Req_Keyword))
                {
                    this.lt_Keyword.Text = Req_Keyword;
                }

                //Get Data
                LookupDataList(Req_PageIdx);

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
    /// <param name="pageIndex"></param>
    private void LookupDataList(int pageIndex)
    {
        //----- 宣告:分頁參數 -----
        int RecordsPerPage = 6;    //每頁筆數
        int StartRow = (pageIndex - 1) * RecordsPerPage;    //第n筆開始顯示
        int TotalRow = 0;   //總筆數
        ArrayList PageParam = new ArrayList();  //條件參數
        bool doRedirect = false;    //是否重新導向

        //----- 宣告:資料參數 -----
        ProductRepository _data = new ProductRepository();
        Dictionary<int, string> search = new Dictionary<int, string>();


        //----- 原始資料:條件篩選 -----

        #region >> 條件篩選 <<

        //[取得/檢查參數] - Keyword
        if (!string.IsNullOrEmpty(Req_Keyword))
        {
            search.Add((int)mySearch.Keyword, Req_Keyword);

            PageParam.Add("k=" + Server.UrlEncode(Req_Keyword));
        }

        #endregion


        //----- 原始資料:取得所有資料 -----
        var query = _data.GetProducts(search, fn_Language.Web_Lang, 0);


        //----- 資料整理:取得總筆數 -----
        TotalRow = query.Count();
        this.lt_Count.Text = TotalRow.ToString();

        //----- 資料整理:頁數判斷 -----

        #region >> 頁數判斷 <<

        if (pageIndex > TotalRow && TotalRow > 0)
        {
            pageIndex = 1;

            doRedirect = true;
        }

        if (StartRow >= TotalRow && TotalRow > 0)
        {
            //當指定page的資料數已不符合計算出來的數量時, 重新導向
            //當前頁數-1
            pageIndex = pageIndex - 1;

            doRedirect = true;
        }

        if (doRedirect)
        {
            //重新整理頁面Url
            string thisPage = "{0}{1}".FormatThis(
                PageUrl
                , pageIndex);

            //重新導向
            Response.Redirect(thisPage);
        }

        #endregion


        //----- 資料整理:選取每頁顯示筆數 -----
        var data = query.Skip(StartRow).Take(RecordsPerPage);

        //----- 資料整理:繫結 ----- 
        this.lvDataList.DataSource = data;
        this.lvDataList.DataBind();

        //----- 資料整理:顯示分頁(放在DataBind之後) ----- 
        if (query.Count() == 0)
        {
            //Session.Remove("BackListUrl");
        }
        else
        {
            Literal lt_Pager = (Literal)this.lvDataList.FindControl("lt_Pager");
            lt_Pager.Text = CustomExtension.PageControl(TotalRow, RecordsPerPage, pageIndex, 5, PageUrl, PageParam, true
                , false, CustomExtension.myStyle.Goole);


            //重新整理頁面Url
            string thisPage = "{0}{1}".FormatThis(
                 PageUrl
                 , pageIndex);


            //暫存頁面Url, 給其他頁使用
            //Session["BackListUrl"] = thisPage;
        }

    }

    protected void lvDataList_ItemDataBound(object sender, ListViewItemEventArgs e)
    {
        try
        {
            if (e.Item.ItemType == ListViewItemType.DataItem)
            {
                ListViewDataItem dataItem = (ListViewDataItem)e.Item;

                //判斷是否為New
                string Get_Status = DataBinder.Eval(dataItem.DataItem, "IsNew").ToString();

                //取得控制項
                PlaceHolder ph_NewMark = (PlaceHolder)e.Item.FindControl("ph_NewMark");
                ph_NewMark.Visible = Get_Status.Equals("Y");

            }
        }
        catch (Exception)
        {

            throw new Exception("系統發生錯誤 - ItemDataBound！");
        }
    }


    public string ShopUrl(string modelNo)
    {
        string url = shopUrl.Replace("#品號#", modelNo);

        return url;
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
    /// 取得傳遞參數 - Page (目前索引頁)
    /// </summary>
    public int Req_PageIdx
    {
        get
        {
            int myData = Page.RouteData.Values["page"] == null ? 1 : Convert.ToInt16(Page.RouteData.Values["page"]);

            return myData;
        }
        set
        {
            this._Req_PageIdx = value;
        }
    }
    private int _Req_PageIdx;


    /// <summary>
    /// 設定參數 - 本頁Url
    /// </summary>
    public string PageUrl
    {
        get
        {
            return "{0}{1}/Search".FormatThis(webUrl, Req_Lang);
        }
        set
        {
            this._PageUrl = value;
        }
    }
    private string _PageUrl;


    /// <summary>
    /// 取得傳遞參數 - Keyword
    /// </summary>
    private string _Req_Keyword;
    public string Req_Keyword
    {
        get
        {
            String Keyword = Request.QueryString["k"];
            return (CustomExtension.String_資料長度Byte(Keyword, "1", "50", out ErrMsg)) ? Keyword.Trim() : "";
        }
        set
        {
            this._Req_Keyword = value;
        }
    }
    #endregion
}