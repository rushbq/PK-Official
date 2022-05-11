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
using ExtensionUI;

public partial class mySupport_DealerView : System.Web.UI.Page
{
    public string ErrMsg;

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!IsPostBack)
            {
                //取得資料
                LookupData();


                //[取得/檢查參數] - 縣市
                if (fn_CustomUI.Get_City(ddl_RegionCode, Req_City, Req_DataID, true, out ErrMsg))
                {
                    if (ddl_RegionCode.Items.Count > 0)
                    {
                        ddl_RegionCode.Items[0].Text = "-- ALL --";
                    }
                }
                else
                {
                    ddl_RegionCode.Visible = false;
                }
            }

        }
        catch (Exception)
        {

            throw;
        }
    }

    #region -- 資料顯示 --
    /// <summary>
    /// 資料顯示
    /// </summary>
    private void LookupData()
    {
        try
        {
            //[取得資料] - 取得資料
            using (SqlCommand cmd = new SqlCommand())
            {
                //宣告
                StringBuilder SBSql = new StringBuilder();

                //清除參數
                cmd.Parameters.Clear();

                //[SQL] - 資料查詢
                SBSql.AppendLine(" SELECT Sub.Country_Name, Base.Dealer_ID");
                SBSql.AppendLine("  , Base.Dealer_Name, Base.Dealer_Contact, Base.Dealer_Location, Base.Dealer_Lat, Base.Dealer_Lng");
                SBSql.AppendLine("  , Base.Dealer_Fax, Base.Dealer_Email, Base.Dealer_Website");
                SBSql.AppendLine("  , (SELECT COUNT(*) FROM Dealer_Tel Tel WHERE (Tel.Dealer_ID = Base.Dealer_ID)) AS Tel_Cnt");
                SBSql.AppendLine("  , (SELECT COUNT(*) FROM Dealer_Photos Photo WHERE (Photo.Dealer_ID = Base.Dealer_ID)) AS Photo_Cnt");
                SBSql.AppendLine(" FROM Dealer Base WITH (NOLOCK) ");
                SBSql.AppendLine("  INNER JOIN Geocode_CountryName Sub WITH (NOLOCK) ON Base.Country_Code = Sub.Country_Code ");
                SBSql.AppendLine(" WHERE (LOWER(Sub.LangCode) = LOWER(@LangCode)) AND (Base.Display = 'Y') ");
                SBSql.AppendLine("  AND (LOWER(Sub.Country_Code) = LOWER(@Country_Code))");

                //Filter:City
                if (!string.IsNullOrWhiteSpace(Req_City))
                {
                    SBSql.Append(" AND (Base.Region_Code = @Region_Code)");

                    cmd.Parameters.AddWithValue("Region_Code", Req_City);
                }

                SBSql.AppendLine(" ORDER BY Base.Sort");
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.AddWithValue("LangCode", fn_Language.PKWeb_Lang);
                cmd.Parameters.AddWithValue("Country_Code", Req_DataID);
                using (DataTable DT = dbConn.LookupDT(cmd, out ErrMsg))
                {
                    if (DT.Rows.Count > 0)
                    {
                        string Country_Name = DT.Rows[0]["Country_Name"].ToString();
                        this.lt_Title.Text = Country_Name;

                        //** 次標題 **
                        this.Page.Title = "{0} | {1}".FormatThis(Country_Name, Resources.resPublic.title_銷售據點);
                    }
                    else
                    {
                        //** 次標題 **
                        this.Page.Title = Resources.resPublic.title_銷售據點;
                    }

                    this.lvDataList.DataSource = DT.DefaultView;
                    this.lvDataList.DataBind();
                }

            }
        }
        catch (Exception)
        {
            throw new Exception("系統發生錯誤 - WhereToBuy");
        }
    }


    protected void lvDataList_ItemDataBound(object sender, ListViewItemEventArgs e)
    {
        if (e.Item.ItemType == ListViewItemType.DataItem)
        {
            //取得資料
            string GetID = DataBinder.Eval(e.Item.DataItem, "Dealer_ID").ToString();
            int Tel_Cnt = Convert.ToInt16(DataBinder.Eval(e.Item.DataItem, "Tel_Cnt"));
            int Photo_Cnt = Convert.ToInt16(DataBinder.Eval(e.Item.DataItem, "Photo_Cnt"));

            //取得控制項
            Literal lt_phone = (Literal)e.Item.FindControl("lt_phone");
            Literal lt_photos = (Literal)e.Item.FindControl("lt_photos");

            //取得電話
            if (Tel_Cnt > 0)
            {
                lt_phone.Text = GetData_Phones(GetID);
            }

            //取得圖片集
            if (Photo_Cnt > 0)
            {
                lt_photos.Text = GetData_Photos(GetID);
            }

            //指定顯示:中國顯示百度地圖的Url
            if (Req_DataID.ToUpper().Equals("CN"))
            {
                PlaceHolder ph_CN = (PlaceHolder)e.Item.FindControl("ph_CN");
                ph_CN.Visible = true;
            }
        }
    }

    private string GetData_Phones(string id)
    {
        //[取得資料] - 取得資料
        using (SqlCommand cmd = new SqlCommand())
        {
            //宣告
            StringBuilder SBSql = new StringBuilder();

            //清除參數
            cmd.Parameters.Clear();

            //[SQL] - 資料查詢
            SBSql.AppendLine(" SELECT Base.Tel");
            SBSql.AppendLine(" FROM Dealer_Tel Base WITH (NOLOCK) ");
            SBSql.AppendLine(" WHERE (Base.Dealer_ID = @DataID) ");
            cmd.CommandText = SBSql.ToString();
            cmd.Parameters.AddWithValue("DataID", id);
            using (DataTable DT = dbConn.LookupDT(cmd, out ErrMsg))
            {
                if (DT.Rows.Count == 0)
                {
                    return "";
                }

                //組合html
                StringBuilder html = new StringBuilder();

                html.Append("<p><i class=\"fa fa-phone fa-fw\"></i>");
                for (int row = 0; row < DT.Rows.Count; row++)
                {
                    if (row > 0) html.Append(" / ");

                    html.Append(DT.Rows[row]["Tel"].ToString());

                }
                html.Append("</p>");

                return html.ToString();
            }

        }
    }

    private string GetData_Photos(string id)
    {
        //[取得資料] - 取得資料
        using (SqlCommand cmd = new SqlCommand())
        {
            //宣告
            StringBuilder SBSql = new StringBuilder();

            //清除參數
            cmd.Parameters.Clear();

            //[SQL] - 資料查詢
            SBSql.AppendLine(" SELECT Base.Pic_File");
            SBSql.AppendLine(" FROM Dealer_Photos Base WITH (NOLOCK) ");
            SBSql.AppendLine(" WHERE (Base.Dealer_ID = @DataID) ");
            SBSql.AppendLine(" ORDER BY Base.Sort ");
            cmd.CommandText = SBSql.ToString();
            cmd.Parameters.AddWithValue("DataID", id);
            using (DataTable DT = dbConn.LookupDT(cmd, out ErrMsg))
            {
                if (DT.Rows.Count == 0)
                {
                    return "";
                }

                //組合html
                StringBuilder html = new StringBuilder();

                //按鈕
                html.Append("<div class=\"btn-group\">");
                html.Append("<a class=\"btn btn-default zoomPic\" data-gall=\"thumb{1}\" href=\"{0}\"><i class=\"fa fa-camera-retro fa-fw fa-2x\"></i></a>".FormatThis(
                          fn_stringFormat.ashx_Pic("{0}Support/Dealer/{1}/{2}".FormatThis(Param_FileWebFolder, id, DT.Rows[0]["Pic_File"].ToString()))
                         , id
                    ));
                html.Append("</div>");

                //圖片集(隱藏)
                html.Append("<div class=\"hidden\">");

                for (int row = 1; row < DT.Rows.Count; row++)
                {
                    html.Append("<a class=\"zoomPic\" data-gall=\"thumb{1}\" href=\"{0}\" title=\"\"></a>".FormatThis(
                         fn_stringFormat.ashx_Pic("{0}Support/Dealer/{1}/{2}".FormatThis(Param_FileWebFolder, id, DT.Rows[row]["Pic_File"].ToString()))
                         , id
                        ));

                }
                html.Append("</div>");

                return html.ToString();

            }

        }
    }

    /// <summary>
    /// 顯示聯絡資料
    /// </summary>
    /// <param name="icon">icon名</param>
    /// <param name="value">值</param>
    /// <param name="isUrl">是否為網址</param>
    /// <param name="url">網址</param>
    /// <returns></returns>
    protected string showInfo(string icon, string value, bool isUrl, string url)
    {
        if (string.IsNullOrEmpty(value))
        {
            return "";
        }
        else
        {
            return "<p><i class=\"fa fa-fw {0}\"></i>{1}</p>".FormatThis(
               icon
               , (!isUrl) ? value : "<a href=\"{0}\" target=\"_blank\">{1}</a>".FormatThis(
                       url
                       , value
                   )
               );
        }
    }

    protected void ddl_RegionCode_SelectedIndexChanged(object sender, EventArgs e)
    {
        //this page url
        string url = "{0}WhereToBuy/{1}".FormatThis(Application["WebUrl"], Req_DataID);

        if (ddl_RegionCode.SelectedIndex > 0)
        {
            string _code = ddl_RegionCode.SelectedValue;
            url += "?r=" + _code;

        }

        //redirect
        Response.Redirect(url);
    }


    #endregion


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
    /// 取得傳遞參數 - 地區/城市
    /// </summary>
    private string _Req_City;
    public string Req_City
    {
        get
        {
            String DataID = Request.QueryString["r"] == null ? "" : Request.QueryString["r"].ToString();

            return DataID;
        }
        set
        {
            this._Req_City = value;
        }
    }

    /// <summary>
    /// [參數] - 檔案Web資料夾
    /// </summary>
    private string _Param_FileWebFolder;
    public string Param_FileWebFolder
    {
        get
        {
            return System.Web.Configuration.WebConfigurationManager.AppSettings["File_Folder"];
        }
        set
        {
            this._Param_FileWebFolder = value;
        }
    }
    #endregion

}