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
using Newtonsoft.Json.Linq;

public partial class _Default : System.Web.UI.Page
{
    public string ErrMsg;
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!IsPostBack)
            {
                //載入四格廣告
                LookupData_Adv();

                //載入News
                LookupData_News();

                //載入展覽活動
                LookupData_Expo();

                //載入產品分類
                LookupData_Cate();

            }

        }
        catch (Exception)
        {

            throw;
        }
    }


    #region -- 資料顯示 --
    /// <summary>
    /// Adv四格廣告資料顯示
    /// </summary>
    private void LookupData_Adv()
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
                SBSql.AppendLine(" SELECT TOP 4 myData.Adv_ID, myData.Group_ID, myData.Adv_Title, myData.Adv_Pic, myData.Adv_Uri, GP.Adv_Target ");
                SBSql.AppendLine(" FROM Adv_Group GP ");
                SBSql.AppendLine(" INNER JOIN Adv_Area Area ON GP.Group_ID = Area.Group_ID ");
                SBSql.AppendLine(" INNER JOIN Adv myData ON GP.Group_ID = myData.Group_ID ");
                SBSql.AppendLine(" WHERE (Area.AreaCode = @AreaCode) ");
                SBSql.AppendLine("  AND (GETDATE() >= GP.StartTime) AND (GETDATE() <= GP.EndTime)");
                SBSql.AppendLine("  AND (GP.Display = 'Y') AND (LOWER(myData.LangCode) = LOWER(@LangCode))");
                SBSql.AppendLine("  AND (GP.Adv_Position = 2)");
                SBSql.AppendLine(" ORDER BY GP.Sort ASC, GP.EndTime DESC");

                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.AddWithValue("AreaCode", fn_Area.PKWeb_Area);
                cmd.Parameters.AddWithValue("LangCode", fn_Language.PKWeb_Lang);
                using (DataTable DT = dbConn.LookupDT(cmd, out ErrMsg))
                {
                    if (DT.Rows.Count == 0)
                    {
                        this.ph_Block_Adv.Visible = false;
                        return;
                    }
                    //宣告
                    StringBuilder html = new StringBuilder();

                    for (int row = 0; row < DT.Rows.Count; row++)
                    {
                        //取得參數
                        string GetGroupID = DT.Rows[row]["Group_ID"].ToString();
                        string GetPic = DT.Rows[row]["Adv_Pic"].ToString();
                        string GetUri = DT.Rows[row]["Adv_Uri"].ToString();
                        string GetTitle = DT.Rows[row]["Adv_Title"].ToString();
                        string GetTarget = DT.Rows[row]["Adv_Target"].ToString();
                        string ShowPic = fn_stringFormat.show_Pic("{0}Adv/{1}/{2}".FormatThis(Param_FileWebFolder, GetGroupID, GetPic));

                        //廣告圖片
                        //html.Append("<a href=\"{1}\" target=\"{3}\" class=\"promo-item\"><div><img src=\"{0}\" alt=\"{2}\" /><h2>{2}</h2></div></a>".FormatThis(
                        //        ShowPic
                        //        , GetUri
                        //        , GetTitle
                        //        , GetTarget));
                        //20200513, 加入ga click事件分析
                        html.Append("<a href=\"{1}\" target=\"{3}\" class=\"promo-item\" onclick=\"captureOutboundLink('{1}','{2}');\"><div><img src=\"{0}\" alt=\"{2}\" /><h2>{2}</h2></div></a>".FormatThis(
                                ShowPic
                                , GetUri
                                , GetTitle
                                , GetTarget));

                        if (row == 1)
                        {
                            html.Append("<div class=\"line visible-xs\"></div>");
                        }
                    }

                    //顯示Html
                    this.lt_BlockAdv.Text = html.ToString();

                }

            }
        }
        catch (Exception)
        {
            throw new Exception("系統發生錯誤 - Adv");
        }
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
                SBSql.AppendLine(" SELECT TOP 1 myData.News_ID, myData.Group_ID, myData.News_Title, myData.News_Desc, myData.News_PubDate, myData.News_Pic ");
                SBSql.AppendLine(" FROM News_Group GP ");
                SBSql.AppendLine(" INNER JOIN News_Area Area ON GP.Group_ID = Area.Group_ID ");
                SBSql.AppendLine(" INNER JOIN News myData ON GP.Group_ID = myData.Group_ID ");
                SBSql.AppendLine(" WHERE (Area.AreaCode = @AreaCode) ");
                SBSql.AppendLine("  AND (GETDATE() >= GP.StartTime) AND (GETDATE() <= GP.EndTime)");
                SBSql.AppendLine("  AND (GP.Display = 'Y') AND (LOWER(myData.LangCode) = LOWER(@LangCode))");
                SBSql.AppendLine("  AND (GP.onIndex = 'Y')");

                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.AddWithValue("AreaCode", fn_Area.PKWeb_Area);
                cmd.Parameters.AddWithValue("LangCode", fn_Language.PKWeb_Lang);
                using (DataTable DT = dbConn.LookupDT(cmd, out ErrMsg))
                {
                    this.myNews.DataSource = DT.DefaultView;
                    this.myNews.DataBind();
                }

            }
        }
        catch (Exception)
        {
            throw new Exception("系統發生錯誤 - News");
        }
    }


    protected void myNews_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
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

                html.AppendLine("<div class=\"pull-left col-xs-6 image-box\">");
                html.AppendLine("<a href=\"{0}\"><img data-original=\"{1}\" src=\"{2}js/lazyload/grey.gif\" class=\"img-responsive fixImg lazy\" alt=\"News\" /></a>".FormatThis(
                        "{0}News/View/{1}/".FormatThis(Application["WebUrl"], Cryptograph.MD5Encrypt(GetID, Application["DesKey"].ToString()))
                        , fn_stringFormat.show_Pic("{0}News/{1}/{2}".FormatThis(Param_FileWebFolder, GetGroupID, GetPic))
                        , Application["WebUrl"]
                    ));
                html.AppendLine("</div>");

                //顯示Html
                lt_Pic.Text = html.ToString();
            }
        }
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
                SBSql.AppendLine(" SELECT TOP 1 myData.Expo_ID, myData.Group_ID, myData.Expo_Title, myData.Expo_Desc, myData.Expo_PubDate, myData.Expo_Pic ");
                SBSql.AppendLine(" FROM Expo_Group GP ");
                SBSql.AppendLine(" INNER JOIN Expo_Area Area ON GP.Group_ID = Area.Group_ID ");
                SBSql.AppendLine(" INNER JOIN Expo myData ON GP.Group_ID = myData.Group_ID ");
                SBSql.AppendLine(" WHERE (Area.AreaCode = @AreaCode) ");
                SBSql.AppendLine("  AND (GETDATE() >= GP.StartTime) AND (GETDATE() <= GP.EndTime)");
                SBSql.AppendLine("  AND (GP.Display = 'Y') AND (LOWER(myData.LangCode) = LOWER(@LangCode))");
                SBSql.AppendLine("  AND (GP.onIndex = 'Y')");

                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.AddWithValue("AreaCode", fn_Area.PKWeb_Area);
                cmd.Parameters.AddWithValue("LangCode", fn_Language.PKWeb_Lang);
                using (DataTable DT = dbConn.LookupDT(cmd, out ErrMsg))
                {
                    this.myExpo.DataSource = DT.DefaultView;
                    this.myExpo.DataBind();
                }

            }
        }
        catch (Exception)
        {
            throw new Exception("系統發生錯誤 - Expo");
        }
    }


    protected void myExpo_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
        {
            /** 圖片顯示 **/
            //取得資料
            string GetPic = DataBinder.Eval(e.Item.DataItem, "Expo_Pic").ToString();
            string GetID = DataBinder.Eval(e.Item.DataItem, "Expo_ID").ToString();
            string GetGroupID = DataBinder.Eval(e.Item.DataItem, "Group_ID").ToString();
            string GetPubDate = DataBinder.Eval(e.Item.DataItem, "Expo_PubDate").ToString();

            if (!string.IsNullOrEmpty(GetPic))
            {

                //取得控制項
                Literal lt_Pic = (Literal)e.Item.FindControl("lt_Pic");

                StringBuilder html = new StringBuilder();

                html.AppendLine("<div class=\"pull-left col-xs-6 image-box\">");
                html.AppendLine("<a href=\"{0}\"><img data-original=\"{1}\" src=\"{2}js/lazyload/grey.gif\" class=\"img-responsive fixImg lazy\" alt=\"Expo\" /></a>".FormatThis(
                        "{0}Expo/{1}".FormatThis(Application["WebUrl"], GetPubDate.ToDateString("yyyy"))
                        , fn_stringFormat.show_Pic("{0}Expo/{1}/{2}".FormatThis(Param_FileWebFolder, GetGroupID, GetPic))
                        , Application["WebUrl"]
                    ));
                html.AppendLine("</div>");
                
                //顯示Html
                lt_Pic.Text = html.ToString();
            }
        }
    }

    /// <summary>
    /// 產品分類 資料顯示
    /// </summary>
    /// <remarks>
    /// 若要新增語系，注意產品中心的架構設定需變動
    /// </remarks>
    private void LookupData_Cate()
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
                SBSql.AppendLine(" SELECT RTRIM(Class_ID) AS Class_ID, Class_Name_{0} AS Class_Name, LinkUrl ".FormatThis(fn_Language.Param_Lang));
                SBSql.AppendLine(" FROM Prod_Class WITH(NOLOCK) ");
                SBSql.AppendLine(" WHERE (LEFT(RTRIM(Class_ID),1) = '2') AND (Display = 'Y') AND (Display_PKWeb = 'Y') ");
                SBSql.AppendLine(" ORDER BY Sort, Class_ID ");
                cmd.CommandText = SBSql.ToString();
                using (DataTable DT = dbConn.LookupDT(cmd, dbConn.DBS.Product, out ErrMsg))
                {
                    //填入項目
                    StringBuilder html = new StringBuilder();
                    html.AppendLine("<ul>");

                    for (int row = 0; row < DT.Rows.Count; row++)
                    {
                        string ClassID = DT.Rows[row]["Class_ID"].ToString();
                        string ClassName = DT.Rows[row]["Class_Name"].ToString();
                        string LinkUrl = DT.Rows[row]["LinkUrl"].ToString();

                        html.Append("<li><a href=\"{0}\" {3}><span class=\"cate-icon cate-icon-{1}\"></span><div class=\"Cate_Text\">{2}</div></a></li>"
                               .FormatThis(
                                    string.IsNullOrEmpty(LinkUrl) ? Application["WebUrl"] + "Products/" + ClassID : LinkUrl
                                   , ClassID
                                   , ClassName
                                   , string.IsNullOrEmpty(LinkUrl) ? "" : "target=\"_blank\""
                               ));

                    }

                    html.AppendLine("</ul>");

                    //輸出Html
                    this.lt_Catelist.Text = html.ToString();

                }

            }
        }
        catch (Exception)
        {
            throw new Exception("系統發生錯誤 - Categories");
        }

    }

    #endregion


    #region -- 參數設定 --
    /// <summary>
    /// [參數] - 檔案Web根目錄路徑
    /// </summary>
    private string _Param_FileWebUrl;
    public string Param_FileWebUrl
    {
        get
        {
            return Application["File_WebUrl"].ToString();
        }
        set
        {
            this._Param_FileWebUrl = value;
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

    /// <summary>
    /// 電子目錄網址
    /// </summary>
    private string _Catalog_Url;
    public string Catalog_Url
    {
        get
        {
            switch (fn_Language.Param_Lang.ToLower())
            {
                case "zh_tw":
                    return "https://e-catalog.prokits.com.tw/tw";

                case "zh_cn":
                    return "https://e-catalog.prokits.com.tw/cn";

                default:
                    return "https://e-catalog.prokits.com.tw/en";
            }

        }
        set
        {
            this._Catalog_Url = value;
        }
    }

    /// <summary>
    /// Facebook網址
    /// </summary>
    private string _Facebook_Url;
    public string Facebook_Url
    {
        get
        {
            switch (fn_Language.Param_Lang)
            {
                case "zh_tw":
                    return "https://www.facebook.com/pages/ProsKit-in-Taiwan/193925007355955";

                case "zh_cn":
                    return "https://www.facebook.com/pages/ProsKit-in-China/294294700592807";

                default:
                    return "https://www.facebook.com/pages/ProsKit-Global/139913052778640";
            }

        }
        set
        {
            this._Facebook_Url = value;
        }
    }


    /// <summary>
    /// DesKey
    /// </summary>
    private string _DesKey;
    public string DesKey
    {
        get
        {
            return Application["DesKey"].ToString();
        }
        set
        {
            this._DesKey = value;
        }
    }

    /// <summary>
    /// 依不同身份產生token
    /// </summary>
    private string _Token;
    public string Token
    {
        get
        {
            return fn_Extensions.Get_MemberToken(fn_Param.MemberType, fn_Param.MemberID);
        }
        set
        {
            this._Token = value;
        }
    }


    /// <summary>
    /// CDN Url
    /// </summary>
    private string _CDN_Url;
    public string CDN_Url
    {
        get
        {
            return System.Web.Configuration.WebConfigurationManager.AppSettings["CDNUrl"];
        }
        set
        {
            this._CDN_Url = value;
        }
    }
    #endregion
}