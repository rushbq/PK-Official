using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using eOrder.Controllers;
using PKLib_Method.Methods;

public partial class myOrder_List : SecurityCheckDealer
{
    public string ErrMsg;

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!IsPostBack)
            {

                //Get Data
                LookupDataList(Req_PageIdx);


                //[取得/檢查參數] - Keyword
                if (!string.IsNullOrEmpty(Req_Keyword))
                {
                    this.filter_Keyword.Text = Req_Keyword;
                }

                //Set LocalResources
                this.filter_Keyword.Attributes.Add("placeholder", this.GetLocalResourceObject("search_訂單編號").ToString());
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
        int RecordsPerPage = 10;    //每頁筆數
        int StartRow = (pageIndex - 1) * RecordsPerPage;    //第n筆開始顯示
        int TotalRow = 0;   //總筆數
        ArrayList PageParam = new ArrayList();  //條件參數
        bool doRedirect = false;    //是否重新導向

        //----- 宣告:資料參數 -----
        eOrderingRepository _data = new eOrderingRepository();
        Dictionary<int, string> search = new Dictionary<int, string>();


        //----- 原始資料:條件篩選 -----

        #region >> 條件篩選 <<

        //[取得/檢查參數] - Keyword
        if (!string.IsNullOrEmpty(Req_Keyword))
        {
            search.Add((int)mySearch.Keyword, Req_Keyword);

            PageParam.Add("keyword=" + Server.UrlEncode(Req_Keyword));
        }

        //設定條件 - 客戶編號
        search.Add((int)mySearch.CustID, fn_Param.Get_CustID);

        #endregion


        //----- 原始資料:取得所有資料 -----
        var query = _data.GetDataList(search);


        //----- 資料整理:取得總筆數 -----
        TotalRow = query.Count();


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
            string thisPage = "{0}?Page={1}{2}".FormatThis(
                PageUrl
                , pageIndex
                , "&" + string.Join("&", PageParam.ToArray()));

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
            Session.Remove("BackListUrl");
        }
        else
        {
            //Literal lt_Pager = (Literal)this.lvDataList.FindControl("lt_Pager");
            lt_Pager.Text = CustomExtension.PageControl(TotalRow, RecordsPerPage, pageIndex, 5, PageUrl, PageParam, false
                , false, CustomExtension.myStyle.Bootstrap);


            //重新整理頁面Url
            string thisPage = "{0}?Page={1}{2}".FormatThis(
                PageUrl
                , pageIndex
                , "&" + string.Join("&", PageParam.ToArray()));


            //暫存頁面Url, 給其他頁使用
            Session["BackListUrl"] = thisPage;

            //填入表頭的多語系文字
            ((Literal)this.lvDataList.FindControl("lt_header1")).Text = this.GetLocalResourceObject("txt_Header1").ToString();
            ((Literal)this.lvDataList.FindControl("lt_header2")).Text = this.GetLocalResourceObject("txt_Header2").ToString();
            ((Literal)this.lvDataList.FindControl("lt_header3")).Text = this.GetLocalResourceObject("txt_Header3").ToString();
        }

    }

    protected void lvDataList_ItemDataBound(object sender, ListViewItemEventArgs e)
    {
        try
        {
            if (e.Item.ItemType == ListViewItemType.DataItem)
            {
                ListViewDataItem dataItem = (ListViewDataItem)e.Item;

                //取得資料:狀態
                string Get_Status = DataBinder.Eval(dataItem.DataItem, "Status").ToString();
                string Get_DataID = DataBinder.Eval(dataItem.DataItem, "Data_ID").ToString();
                string url = "";
                switch (Get_Status)
                {
                    case "1":
                        url = "Step1-1";
                        break;

                    case "2":
                    case "3":
                        //價格會在Step3計算,時間點上會有誤差,所以要進入Step2讓他觸發重算
                        url = "Step2";
                        break;

                    default:
                        url = "View";
                        break;
                }

                Literal lt_Url = (Literal)e.Item.FindControl("lt_Url");
                lt_Url.Text = "<a href=\"{0}EO/{1}/{2}\">More</a>".FormatThis(
                    Application["WebUrl"]
                    , url
                    , Get_DataID
                    );

            }
        }
        catch (Exception)
        {

            throw new Exception("系統發生錯誤 - ItemDataBound！");
        }
    }
    #endregion

    #region -- 按鈕事件 --

    /// <summary>
    /// 關鍵字快查
    /// </summary>
    protected void btn_KeySearch_Click(object sender, EventArgs e)
    {
        //執行查詢
        doSearch();
    }

    /// <summary>
    /// 快查 - 狀態
    /// </summary>
    protected void fast_Status_SelectedIndexChanged(object sender, EventArgs e)
    {
        //執行查詢
        doSearch();
    }

    /// <summary>
    /// 快查 - 時間
    /// </summary>
    protected void fast_DateRange_SelectedIndexChanged(object sender, EventArgs e)
    {
        //執行查詢
        doSearch();
    }


    /// <summary>
    /// 執行查詢
    /// </summary>
    /// <param name="keyword"></param>
    private void doSearch()
    {
        StringBuilder url = new StringBuilder();

        url.Append("{0}?Page=1".FormatThis(PageUrl));


        //[查詢條件] - 關鍵字
        if (!string.IsNullOrEmpty(this.filter_Keyword.Text))
        {
            url.Append("&Keyword=" + Server.UrlEncode(this.filter_Keyword.Text));
        }

        //執行轉頁
        Response.Redirect(url.ToString(), false);
    }

    #endregion


    #region -- 參數設定 --
    /// <summary>
    /// 取得傳遞參數 - PageIdx(目前索引頁)
    /// </summary>
    public int Req_PageIdx
    {
        get
        {
            int data = Request.QueryString["Page"] == null ? 1 : Convert.ToInt32(Request.QueryString["Page"]);
            return data;
        }
        set
        {
            this._Req_PageIdx = value;
        }
    }
    private int _Req_PageIdx;

    /// <summary>
    /// 取得傳遞參數 - Keyword
    /// </summary>
    public string Req_Keyword
    {
        get
        {
            String Keyword = Request.QueryString["Keyword"];
            return (CustomExtension.String_資料長度Byte(Keyword, "1", "50", out ErrMsg)) ? Keyword.Trim() : "";
        }
        set
        {
            this._Req_Keyword = value;
        }
    }
    private string _Req_Keyword;


    /// <summary>
    /// 設定參數 - 本頁Url
    /// </summary>
    public string PageUrl
    {
        get
        {
            return "{0}EO/List".FormatThis(Application["WebUrl"]);
        }
        set
        {
            this._PageUrl = value;
        }
    }
    private string _PageUrl;


    #endregion
}