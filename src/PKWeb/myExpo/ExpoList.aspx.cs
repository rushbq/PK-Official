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

public partial class myExpo_ExpoList : System.Web.UI.Page
{
    public string ErrMsg;

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!IsPostBack)
            {
                //** 次標題 **
                this.Page.Title = Resources.resPublic.title_展覽活動;

                //取得年份
                LookupData_ExpoYear();

                //取得資料
                LookupData_Expo();
            }

        }
        catch (Exception)
        {

            throw;
        }
    }

    #region -- 資料顯示 --
    /// <summary>
    /// 取得年份
    /// </summary>
    private void LookupData_ExpoYear()
    {
        try
        {
            //[取得資料] - 取得資料
            using (SqlCommand cmd = new SqlCommand())
            {
                //宣告
                StringBuilder SBSql = new StringBuilder();
                StringBuilder Html = new StringBuilder();

                //清除參數
                cmd.Parameters.Clear();

                //[SQL] - 資料查詢
                SBSql.AppendLine(" SELECT YEAR(myData.Expo_PubDate) AS myYear ");
                SBSql.AppendLine(" FROM Expo_Group GP ");
                SBSql.AppendLine("  INNER JOIN Expo_Area Area ON GP.Group_ID = Area.Group_ID ");
                SBSql.AppendLine("  INNER JOIN Expo myData ON GP.Group_ID = myData.Group_ID ");
                SBSql.AppendLine(" WHERE (Area.AreaCode = @AreaCode) ");
                SBSql.AppendLine("  AND (GETDATE() >= GP.StartTime) AND (GETDATE() <= GP.EndTime)");
                SBSql.AppendLine("  AND (GP.Display = 'Y') AND (LOWER(myData.LangCode) = LOWER(@LangCode))");
                SBSql.AppendLine(" GROUP BY YEAR(myData.Expo_PubDate) ");
                SBSql.AppendLine(" ORDER BY 1 DESC ");

                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.AddWithValue("AreaCode", fn_Area.PKWeb_Area);
                cmd.Parameters.AddWithValue("LangCode", fn_Language.PKWeb_Lang);
                using (DataTable DT = dbConn.LookupDT(cmd, out ErrMsg))
                {
                    if (DT.Rows.Count == 0)
                    {
                        //沒有資料, 帶入今年
                        // 下拉選單
                        string thisYear = Convert.ToString(DateTime.Now.Year);
                        this.ddl_Year.Items.Insert(0, new ListItem(thisYear, thisYear));

                        // 項目連結
                        Html.AppendLine("<li><a href=\"{0}Expo/{1}\">{1}</a></li>".FormatThis(
                                Application["WebUrl"]
                                , thisYear
                            ));
                    }
                    else
                    {
                        // 下拉選單
                        this.ddl_Year.DataValueField = "myYear";
                        this.ddl_Year.DataTextField = "myYear";
                        this.ddl_Year.DataSource = DT.DefaultView;
                        this.ddl_Year.DataBind();
                        this.ddl_Year.SelectedValue = Req_Year;

                        // 項目連結
                        for (int row = 0; row < DT.Rows.Count; row++)
                        {
                            Html.AppendLine("<li><a href=\"{0}Expo/{1}\">{1}</a></li>".FormatThis(
                               Application["WebUrl"]
                               , DT.Rows[row]["myYear"].ToString()
                           ));
                        }
                    }

                    //填入年份資料
                    this.lt_listYear.Text = Html.ToString();
                }


            }
        }
        catch (Exception)
        {
            throw new Exception("系統發生錯誤 - 年份");
        }
    }

    protected void ddl_Year_SelectedIndexChanged(object sender, EventArgs e)
    {
        Response.Redirect("{0}Expo/{1}".FormatThis(
                Application["WebUrl"]
                , this.ddl_Year.SelectedValue
            ));
    }

    /// <summary>
    /// Expo 資料顯示
    /// </summary>
    private void LookupData_Expo()
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
                SBSql.AppendLine(" SELECT myData.Expo_ID, myData.Group_ID, myData.Expo_Title, myData.Expo_Desc, myData.Expo_PubDate, myData.Expo_Pic ");
                SBSql.AppendLine("  , myData.Expo_Location, myData.Expo_Website, myData.Expo_BoothPic");
                SBSql.AppendLine("  , myData.Expo_Lat, myData.Expo_Lng");
                SBSql.AppendLine(" FROM Expo_Group GP ");
                SBSql.AppendLine("  INNER JOIN Expo_Area Area ON GP.Group_ID = Area.Group_ID ");
                SBSql.AppendLine("  INNER JOIN Expo myData ON GP.Group_ID = myData.Group_ID ");
                SBSql.AppendLine(" WHERE (Area.AreaCode = @AreaCode) ");
                SBSql.AppendLine("  AND (GETDATE() >= GP.StartTime) AND (GETDATE() <= GP.EndTime)");
                SBSql.AppendLine("  AND (GP.Display = 'Y') AND (LOWER(myData.LangCode) = LOWER(@LangCode))");
                SBSql.AppendLine("  AND (YEAR(myData.Expo_PubDate) = @Req_Year)");
                SBSql.AppendLine(" ORDER BY GP.Sort ASC, myData.Expo_PubDate DESC ");

                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.AddWithValue("AreaCode", fn_Area.PKWeb_Area);
                cmd.Parameters.AddWithValue("LangCode", fn_Language.PKWeb_Lang);
                cmd.Parameters.AddWithValue("Req_Year", Req_Year);
                using (DataTable DT = dbConn.LookupDT(cmd, out ErrMsg))
                {
                    this.lvDataList.DataSource = DT.DefaultView;
                    this.lvDataList.DataBind();
                }

            }
        }
        catch (Exception)
        {
            throw;
            throw new Exception("系統發生錯誤 - Expo");
        }
    }


    protected void lvDataList_ItemDataBound(object sender, ListViewItemEventArgs e)
    {
        if (e.Item.ItemType == ListViewItemType.DataItem)
        {
            //取得資料
            string GetID = DataBinder.Eval(e.Item.DataItem, "Expo_ID").ToString();
            string GetGroupID = DataBinder.Eval(e.Item.DataItem, "Group_ID").ToString();

            /** 圖片顯示 **/
            string GetPic = DataBinder.Eval(e.Item.DataItem, "Expo_Pic").ToString();
            if (!string.IsNullOrEmpty(GetPic))
            {
                //取得控制項
                Literal lt_Pic = (Literal)e.Item.FindControl("lt_Pic");

                //顯示Html
                lt_Pic.Text = "<img data-original=\"{0}\" src=\"{1}js/lazyload/grey.gif\" class=\"lazy fixImg\" alt=\"\" />".FormatThis(
                        fn_stringFormat.ashx_Pic("{0}Expo/{1}/{2}".FormatThis(Param_FileWebFolder, GetGroupID, GetPic))
                        , Application["WebUrl"]
                    );
            }

            /** 網站顯示 **/
            string GetWebsite = DataBinder.Eval(e.Item.DataItem, "Expo_Website").ToString();
            if (!string.IsNullOrEmpty(GetWebsite))
            {
                //取得控制項
                Literal lt_Website = (Literal)e.Item.FindControl("lt_Website");

                //顯示Html
                lt_Website.Text = "<a href=\"{0}\" class=\"btn btn-default btn-sm\" target=\"_blank\">{1}</a>".FormatThis(
                        GetWebsite
                        , this.GetLocalResourceObject("txt_網站")
                    );
            }

            /** 攤位圖顯示 **/
            string GetBoothPic = DataBinder.Eval(e.Item.DataItem, "Expo_BoothPic").ToString();
            if (!string.IsNullOrEmpty(GetBoothPic))
            {
                //取得控制項
                Literal lt_Booth = (Literal)e.Item.FindControl("lt_Booth");

                //顯示Html
                lt_Booth.Text = "<a href=\"{0}\" class=\"btn btn-default btn-sm zoomPic\" title=\"{1}\">{1}</a>".FormatThis(
                         fn_stringFormat.ashx_Pic("{0}Expo/{1}/{2}".FormatThis(Param_FileWebFolder, GetGroupID, GetBoothPic))
                        , this.GetLocalResourceObject("txt_攤位圖")
                    );
            }
        }
    }
    #endregion

    #region -- 參數設定 --
    /// <summary>
    /// 取得傳遞參數 - 年份
    /// </summary>
    private string _Req_Year;
    public string Req_Year
    {
        get
        {
            string getData = Page.RouteData.Values["Year"].ToString();
            return getData;
        }
        set
        {
            this._Req_Year = value;
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