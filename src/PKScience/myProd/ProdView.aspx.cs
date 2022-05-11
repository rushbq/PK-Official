using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.UI;
using System.Web.UI.WebControls;
using PKLib_Method.Methods;
using ProductData.Controllers;

public partial class myProd_ProdView : System.Web.UI.Page
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
        ProductRepository _data = new ProductRepository();


        //----- 原始資料:取得所有資料 -----
        var data = _data.GetOne(Req_DataID, fn_Language.Web_Lang);


        //----- 資料整理:繫結 ----- 
        this.lvDataList.DataSource = data;
        this.lvDataList.DataBind();


        //填入資料
        var showData = data.FirstOrDefault();
        if (showData != null)
        {
            this.lt_Header.Text = showData.ModelName;
            this.lt_ModalHeader.Text = showData.ModelName;
            this.lt_TypeName.Text = showData.TypeName;
            param_TypeID = showData.TypeID;

            //meta資訊
            meta_Title = string.IsNullOrEmpty(showData.ShareTitle) ? showData.ModelName : showData.ShareTitle;
            meta_Desc = string.IsNullOrEmpty(showData.ShareDesc) ? showData.ShortDesc : showData.ShareDesc;
            meta_Url = PageUrl;
            meta_Image = showData.ListPic;
            meta_Keyword = GetData_Keyword(showData.ModelNo);
            meta_TitleSeo = "{0} {1} | {2}".FormatThis(showData.ModelNo, showData.ModelName, Resources.resPublic.title_All);
            meta_DescSeo = showData.SeoDesc;

            //取得檔案列表
            LookupData_Files();
        }

    }


    protected void lvDataList_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
        {
            //取得資料
            string Get_Status = DataBinder.Eval(e.Item.DataItem, "IsNew").ToString();
            string Get_Url1 = DataBinder.Eval(e.Item.DataItem, "Url1").ToString();
            string Get_Url2 = DataBinder.Eval(e.Item.DataItem, "Url2").ToString();

            //取得控制項, 判斷是否為新品
            PlaceHolder ph_NewMark = (PlaceHolder)e.Item.FindControl("ph_NewMark");
            ph_NewMark.Visible = Get_Status.Equals("Y");

            //判斷是否有資料
            PlaceHolder ph_Url1 = (PlaceHolder)e.Item.FindControl("ph_Url1");
            ph_Url1.Visible = !string.IsNullOrEmpty(Get_Url1);
            PlaceHolder ph_Url2 = (PlaceHolder)e.Item.FindControl("ph_Url2");
            ph_Url2.Visible = !string.IsNullOrEmpty(Get_Url2);

        }

    }


    /// <summary>
    /// 取得下載檔案
    /// </summary>
    /// <remarks>
    ///  remove at 20171122 資訊需求(30-20171120-1967)
    ///  open at 20200323
    /// </remarks>
    private void LookupData_Files()
    {
        //----- 宣告:資料參數 -----
        ProductRepository _data = new ProductRepository();


        //----- 原始資料:取得所有資料 -----
        var data = _data.GetManuals(Req_DataID, fn_Language.Web_Lang);


        //----- 資料整理:繫結 ----- 
        this.dtFiles.DataSource = data;
        this.dtFiles.DataBind();


        //無資料
        if (data.Count() == 0)
        {
            this.lvDataList.Items[0].FindControl("ph_Download").Visible = false;
        }

        _data = null;
    }


    /// <summary>
    /// 取得關鍵字
    /// </summary>
    /// <param name="modelNo"></param>
    /// <returns></returns>
    private string GetData_Keyword(string modelNo)
    {
        // 宣告
        ProductRepository _data = new ProductRepository();

        //取得資料
        var query = _data.GetTags(modelNo).Select(i => i.TagName);

        //回傳
        return string.Join(",", query); ;
    }


    /// <summary>
    /// 取得圖片集
    /// </summary>
    /// <param name="PhotoGroup">圖片集合</param>
    /// <param name="Model_No">品號</param>
    /// <param name="type">Slide or PicList</param>
    /// <remarks>
    /// 圖片要給直接路徑
    /// </remarks>
    public string GetData_AllPic(string PhotoGroup, string Model_No, string type)
    {
        //判斷參數
        if (string.IsNullOrEmpty(Model_No))
        {
            return "";
        }


        //拆解圖片值 "|"
        StringBuilder Photo = new StringBuilder();
        string[] strAry = Regex.Split(PhotoGroup, @"\|{1}");


        if (type.Equals("slide"))
        {
            Photo.Append("<div class=\"flexslider\"><ul class=\"slides\">");
        }
        else
        {
            Photo.Append("<div style=\"display: none\">");
        }

        for (int row = 0; row < strAry.Length; row++)
        {
            if (false == string.IsNullOrEmpty(strAry[row].ToString()))
            {
                //小圖
                string smallPicUrl = "{0}ProductPic/{1}/{2}/{3}".FormatThis(
                    fn_Param.FileUrl, Model_No, "1", "500x500_" + strAry[row].ToString());

                //大圖
                string bigPicUrl = "{0}ProductPic/{1}/{2}/{3}".FormatThis(
                    fn_Param.FileUrl, Model_No, "1", "1000x1000_" + strAry[row].ToString());


                //判斷type, 組合html
                if (type.Equals("slide"))
                {
                    Photo.Append("<li data-thumb=\"{0}\"><a class=\"zoomPic\" data-rel=\"gal-{1}\" href=\"#!\"><img class=\"lazy\" src=\"{0}\" alt=\"\" /></a></li>".FormatThis(
                            smallPicUrl
                            , row
                        ));
                }
                else
                {
                    Photo.Append("<a id=\"gal-{1}\" class=\"venobox\" data-gall=\"myGallery\" href=\"{0}\"></a>".FormatThis(
                            bigPicUrl
                            , row
                        ));
                }
            }
        }

        if (type.Equals("slide"))
        {
            Photo.Append("</ul></div>");
        }
        else
        {
            Photo.Append("</div>");
        }

        return Photo.ToString();
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
            return "{0}{1}/Product/{2}/".FormatThis(webUrl, Req_Lang, Req_DataID);
        }
        set
        {
            this._PageUrl = value;
        }
    }

    public int param_TypeID
    {
        get;
        set;
    }

    public string meta_Title
    {
        get;
        set;
    }

    public string meta_TitleSeo
    {
        get;
        set;
    }

    public string meta_Desc
    {
        get;
        set;
    }

    public string meta_DescSeo
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

    public string meta_Keyword
    {
        get;
        set;
    }

    #endregion


}