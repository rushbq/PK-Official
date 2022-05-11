using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using FAQData.Controllers;
using PKLib_Method.Methods;

public partial class myQA_View : System.Web.UI.Page
{
    public string cdnUrl = fn_Param.CDNUrl;
    public string webUrl = fn_Param.WebUrl;

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!IsPostBack)
            {
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
        FAQRepository _data = new FAQRepository();


        //----- 原始資料:取得所有資料 -----
        var data = _data.GetDetail(Req_DataID, fn_Language.Web_Lang);

        if (data != null && data.Count() > 0)
        {
            var topData = data.FirstOrDefault();

            //填入header資料
            this.lt_Header.Text = topData.TopTitle;
            this.lt_Title.Text = topData.TopTitle;
            this.lt_ClassHeader.Text =
                "<a class=\"link\" href=\"{0}{1}/FAQ-List/?cls={2}\">{3}</a>".FormatThis(
                webUrl
                , Req_Lang
                , topData.TopClassID
                , topData.TopClass);


            //取得產品關聯
            LookupProds(topData.GroupID.ToString());


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
                lt_Img.Text = "<img class=\"responsive-img lazy\" src=\"{0}\" alt=\"no img\" />"
                    .FormatThis(Get_Img);
            }

        }
    }



    /// <summary>
    /// 取得產品關聯
    /// </summary>
    /// <param name="id"></param>
    private void LookupProds(string id)
    {
        //----- 宣告:資料參數 -----
        FAQRepository _data = new FAQRepository();


        //----- 原始資料:取得所有資料 -----
        var data = _data.GetProds(id, fn_Language.Web_Lang);


        //----- 資料整理:繫結 ----- 
        this.lvProdList.DataSource = data;
        this.lvProdList.DataBind();
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
            return "{0}{1}/FAQ/{2}".FormatThis(webUrl, Req_Lang, Req_DataID);
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