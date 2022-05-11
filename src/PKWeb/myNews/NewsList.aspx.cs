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

public partial class myNews_NewsList : System.Web.UI.Page
{
    public string ErrMsg;

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!IsPostBack)
            {
                //** 次標題 **
                this.Page.Title = Resources.resPublic.title_消息公告;

                //取得年份
                LookupData_NewsYear();

                //取得資料
                LookupData_News();
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
    private void LookupData_NewsYear()
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
                SBSql.AppendLine(" SELECT YEAR(myData.News_PubDate) AS myYear ");
                SBSql.AppendLine(" FROM News_Group GP ");
                SBSql.AppendLine("  INNER JOIN News_Area Area ON GP.Group_ID = Area.Group_ID ");
                SBSql.AppendLine("  INNER JOIN News myData ON GP.Group_ID = myData.Group_ID ");
                SBSql.AppendLine(" WHERE (Area.AreaCode = @AreaCode) ");
                SBSql.AppendLine("  AND (GETDATE() >= GP.StartTime) AND (GETDATE() <= GP.EndTime)");
                SBSql.AppendLine("  AND (GP.Display = 'Y') AND (LOWER(myData.LangCode) = LOWER(@LangCode))");
                SBSql.AppendLine(" GROUP BY YEAR(myData.News_PubDate) ");
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

                        // 項目連結
                        Html.AppendLine("<li><a href=\"{0}News/{1}\">{1}</a></li>".FormatThis(
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
                            Html.AppendLine("<li><a href=\"{0}News/{1}\">{1}</a></li>".FormatThis(
                               Application["WebUrl"]
                               , DT.Rows[row]["myYear"].ToString()
                           ));
                        }
                    }

                    //填入年份資料
                    this.lt_listYear.Text = Html.ToString();

                    this.ddl_Year.Items.Insert(0, new ListItem("↓↓↓↓↓", Req_Year));
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
        Response.Redirect("{0}News/{1}".FormatThis(
                Application["WebUrl"]
                , this.ddl_Year.SelectedValue
            ));
    }

    /// <summary>
    /// News 資料顯示
    /// </summary>
    private void LookupData_News()
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
                SBSql.AppendLine(" SELECT myData.News_ID, myData.Group_ID, myData.News_Title, myData.News_Desc, myData.News_PubDate, myData.News_Pic ");
                SBSql.AppendLine(" FROM News_Group GP ");
                SBSql.AppendLine("  INNER JOIN News_Area Area ON GP.Group_ID = Area.Group_ID ");
                SBSql.AppendLine("  INNER JOIN News myData ON GP.Group_ID = myData.Group_ID ");
                SBSql.AppendLine(" WHERE (Area.AreaCode = @AreaCode) ");
                SBSql.AppendLine("  AND (GETDATE() >= GP.StartTime) AND (GETDATE() <= GP.EndTime)");
                SBSql.AppendLine("  AND (GP.Display = 'Y') AND (LOWER(myData.LangCode) = LOWER(@LangCode))");
                SBSql.AppendLine("  AND (YEAR(myData.News_PubDate) = @Req_Year)");
                SBSql.AppendLine(" ORDER BY GP.Sort ASC, myData.News_PubDate DESC ");

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
            throw new Exception("系統發生錯誤 - News");
        }
    }


    protected void lvDataList_ItemDataBound(object sender, ListViewItemEventArgs e)
    {
        if (e.Item.ItemType == ListViewItemType.DataItem)
        {
            /** 圖片顯示 **/
            //取得資料
            string GetPic = DataBinder.Eval(e.Item.DataItem, "News_Pic").ToString();
            string GetID = DataBinder.Eval(e.Item.DataItem, "News_ID").ToString();
            string GetGroupID = DataBinder.Eval(e.Item.DataItem, "Group_ID").ToString();

            if (!string.IsNullOrEmpty(GetPic))
            {

                //取得控制項
                Literal lt_Pic = (Literal)e.Item.FindControl("lt_Pic");

                StringBuilder html = new StringBuilder();

                html.AppendLine("<img src=\"{0}\" class=\"card-img-top\" alt=\"\" width=\"100%\" />".FormatThis(
                        fn_stringFormat.ashx_Pic("{0}News/{1}/{2}".FormatThis(Param_FileWebFolder, GetGroupID, GetPic))
                    ));

                //顯示Html
                lt_Pic.Text = html.ToString();
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