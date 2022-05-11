using System;
using System.Collections;
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

public partial class mySupport_PVList : System.Web.UI.Page
{
    public string ErrMsg;

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!IsPostBack)
            {
                //** 次標題 **
                this.Page.Title = Resources.resPublic.title_產品影片;

                this.btn_Search.Text = Resources.resPublic.btn_查詢;

                //[取得/檢查參數] - 產品類別(Toy)
                if (fn_CustomUI.Get_ProdToyClass(this.ddl_ProdClass, Req_ClassID, fn_Language.Param_Lang, true, "ALL", out ErrMsg) == false)
                {
                    this.ddl_ProdClass.Items.Insert(0, new ListItem("empty item", ""));
                }

                //[取得/檢查參數] - Keyword
                if (!string.IsNullOrEmpty(Req_Keyword))
                {
                    this.tb_Keyword.Text = Req_Keyword;
                }

                //取得資料
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
    /// 副程式 - 取得資料列表 (分頁)
    /// </summary>
    /// <param name="pageIndex">目前頁數</param>
    private void LookupDataList(int pageIndex)
    {
        string ErrMsg;

        //[參數宣告] - 共用參數
        SqlCommand cmd = new SqlCommand();
        SqlCommand cmdTotalCnt = new SqlCommand();
        try
        {
            //[參數宣告] - 設定本頁Url(末端無須加 "/")
            this.ViewState["Page_Url"] = "{0}Video/{1}".FormatThis(Application["WebUrl"], Req_ClassType);

            ArrayList Params = new ArrayList();

            //[參數宣告] - 筆數/分頁設定
            int PageSize = 8;  //每頁筆數
            int TotalRow = 0;  //總筆數
            int BgItem = (pageIndex - 1) * PageSize + 1;  //開始筆數
            int EdItem = BgItem + (PageSize - 1);  //結束筆數 

            //[SQL] - 清除參數設定
            cmd.Parameters.Clear();
            cmdTotalCnt.Parameters.Clear();

            //[SQL] - 資料查詢
            StringBuilder SBSql = new StringBuilder();

            #region - [SQL] 資料顯示 -
            SBSql.AppendLine(" SELECT TBL.* ");
            SBSql.AppendLine(" FROM ( ");
            SBSql.AppendLine("    SELECT GP.Group_ID");
            SBSql.AppendLine("      , myData.PV_ID, myData.PV_Title, myData.PV_PubDate, myData.PV_Pic, myData.PV_UriSrc, myData.PV_Uri");
            SBSql.AppendLine("      , ROW_NUMBER() OVER (ORDER BY myData.PV_PubDate DESC) AS RowRank ");
            SBSql.AppendLine("    FROM PV_Group GP ");
            SBSql.AppendLine("      INNER JOIN PV_Area Area ON GP.Group_ID = Area.Group_ID ");
            SBSql.AppendLine("      INNER JOIN PV myData ON GP.Group_ID = myData.Group_ID ");
            SBSql.AppendLine("    WHERE (GP.Display = 'Y') AND (GP.ClassType = @ClassType)");
            SBSql.AppendLine("      AND (Area.AreaCode = @AreaCode) AND (LOWER(myData.LangCode) = LOWER(@LangCode))");

            #region "..查詢條件.."

            //[查詢條件] - Toy關聯
            if (!string.IsNullOrEmpty(Req_ClassID))
            {
                SBSql.Append(" AND GP.Group_ID IN (");
                SBSql.Append("  SELECT Group_ID");
                SBSql.Append("  FROM PV_Group_Rel_ModelNo Rel");
                SBSql.Append("   INNER JOIN [ProductCenter].dbo.ProdToy_Class_Rel_ModelNo Itm ON Rel.Model_No = Itm.Model_No");
                if (!string.IsNullOrEmpty(Req_ClassID) && (!Req_ClassID.ToUpper().Equals("ALL")))
                {
                    SBSql.Append("  WHERE (Itm.Class_ID = @Class_ID)");
                }

                SBSql.Append(" )");
                cmd.Parameters.AddWithValue("Class_ID", Req_ClassID);

                Params.Add("c=" + Server.UrlEncode(Req_ClassID));
            }


            //[查詢條件] - 關鍵字
            if (!string.IsNullOrEmpty(Req_Keyword))
            {
                SBSql.Append(" AND ( ");
                SBSql.Append("  (myData.PV_Title LIKE '%' + @Keyword + '%') ");
                SBSql.Append(" ) ");
                cmd.Parameters.AddWithValue("Keyword", Req_Keyword);

                Params.Add("k=" + Server.UrlEncode(Req_Keyword));
            }

            //[查詢條件] - 品號(其他地方來的)
            if (!string.IsNullOrEmpty(Req_ModelNo))
            {
                SBSql.Append(" AND GP.Group_ID IN (");
                SBSql.Append("  SELECT Group_ID FROM PV_Group_Rel_ModelNo WHERE (UPPER(Model_No) = UPPER(@Model_No))");
                SBSql.Append(" )");
                cmd.Parameters.AddWithValue("Model_No", Req_ModelNo);

                Params.Add("m=" + Server.UrlEncode(Req_ModelNo));
            }
            #endregion

            SBSql.AppendLine(" ) AS TBL ");
            SBSql.AppendLine(" WHERE (RowRank >= @BG_ITEM) AND (RowRank <= @ED_ITEM)");
            SBSql.AppendLine(" ORDER BY RowRank ");

            //[SQL] - Command
            cmd.CommandText = SBSql.ToString();
            cmd.Parameters.AddWithValue("BG_ITEM", BgItem);
            cmd.Parameters.AddWithValue("ED_ITEM", EdItem);
            cmd.Parameters.AddWithValue("AreaCode", fn_Area.PKWeb_Area);
            cmd.Parameters.AddWithValue("LangCode", fn_Language.PKWeb_Lang);
            cmd.Parameters.AddWithValue("ClassType", Req_ClassType.ToUpper().Equals("TOY") ? "B" : "A");

            #endregion


            #region - [SQL] 計算筆數 -
            //[SQL] - 計算資料總數
            SBSql.Clear();
            SBSql.AppendLine(" SELECT COUNT(*) AS TOTAL_CNT ");
            SBSql.AppendLine(" FROM PV_Group GP ");
            SBSql.AppendLine("   INNER JOIN PV_Area Area ON GP.Group_ID = Area.Group_ID ");
            SBSql.AppendLine("   INNER JOIN PV myData ON GP.Group_ID = myData.Group_ID ");
            SBSql.AppendLine(" WHERE (GP.Display = 'Y') AND (GP.ClassType = @ClassType)");
            SBSql.AppendLine("   AND (Area.AreaCode = @AreaCode) AND (LOWER(myData.LangCode) = LOWER(@LangCode))");

            #region "..查詢條件.."

            ////[查詢條件] - 產品類別
            //if (!string.IsNullOrEmpty(Req_ClassID))
            //{
            //    SBSql.Append(" AND GP.Group_ID IN (");
            //    SBSql.Append("  SELECT Group_ID");
            //    SBSql.Append("  FROM PV_Group_Rel_ModelNo Rel");
            //    SBSql.Append("   INNER JOIN [ProductCenter].dbo.Prod_Item Itm ON Rel.Model_No = Itm.Model_No");
            //    SBSql.Append("  WHERE (Itm.Class_ID = @Class_ID)");
            //    SBSql.Append(" )");
            //    cmdTotalCnt.Parameters.AddWithValue("Class_ID", Req_ClassID);
            //}
            //[查詢條件] - Toy關聯
            if (!string.IsNullOrEmpty(Req_ClassID))
            {
                SBSql.Append(" AND GP.Group_ID IN (");
                SBSql.Append("  SELECT Group_ID");
                SBSql.Append("  FROM PV_Group_Rel_ModelNo Rel");
                SBSql.Append("   INNER JOIN [ProductCenter].dbo.ProdToy_Class_Rel_ModelNo Itm ON Rel.Model_No = Itm.Model_No");
                if (!string.IsNullOrEmpty(Req_ClassID) && (!Req_ClassID.ToUpper().Equals("ALL")))
                {
                    SBSql.Append("  WHERE (Itm.Class_ID = @Class_ID)");
                }

                SBSql.Append(" )");
                cmdTotalCnt.Parameters.AddWithValue("Class_ID", Req_ClassID);
            }

            //[查詢條件] - 關鍵字
            if (!string.IsNullOrEmpty(Req_Keyword))
            {
                SBSql.Append(" AND ( ");
                SBSql.Append("  (myData.PV_Title LIKE '%' + @Keyword + '%') ");
                SBSql.Append(" ) ");
                cmdTotalCnt.Parameters.AddWithValue("Keyword", Req_Keyword);
            }

            //[查詢條件] - 品號(其他地方來的)
            if (!string.IsNullOrEmpty(Req_ModelNo))
            {
                SBSql.Append(" AND GP.Group_ID IN (");
                SBSql.Append("  SELECT Group_ID FROM PV_Group_Rel_ModelNo WHERE (UPPER(Model_No) = UPPER(@Model_No))");
                SBSql.Append(" )");
                cmdTotalCnt.Parameters.AddWithValue("Model_No", Req_ModelNo);
            }
            #endregion

            //[SQL] - Command
            cmdTotalCnt.CommandText = SBSql.ToString();
            cmdTotalCnt.Parameters.AddWithValue("AreaCode", fn_Area.PKWeb_Area);
            cmdTotalCnt.Parameters.AddWithValue("LangCode", fn_Language.PKWeb_Lang);
            cmdTotalCnt.Parameters.AddWithValue("ClassType", Req_ClassType.ToUpper().Equals("TOY") ? "B" : "A");
            #endregion


            //[SQL] - 取得資料
            using (DataTable DT = dbConn.LookupDTwithPage(cmd, cmdTotalCnt, out TotalRow, out ErrMsg))
            {
                //DataBind            
                this.lvDataList.DataSource = DT.DefaultView;
                this.lvDataList.DataBind();

                if (DT.Rows.Count > 0)
                {
                    //顯示分頁, 需在DataBind之後
                    Literal lt_Pager = (Literal)this.lvDataList.FindControl("lt_Pager");
                    lt_Pager.Text = fn_CustomUI.PageControl(TotalRow, PageSize, pageIndex, 5, this.ViewState["Page_Url"].ToString(), Params, true);
                }

                //[頁數判斷] - 目前頁數大於總頁數, 則導向第一頁
                if (pageIndex > TotalRow && TotalRow > 0)
                {
                    Response.Redirect("{0}/{1}/{2}".FormatThis(
                            this.ViewState["Page_Url"]
                            , 1
                            , "?" + string.Join("&", Params.ToArray())));
                }
                else
                {
                    //重新整理頁面Url
                    this.ViewState["Page_Url"] = "{0}/{1}/{2}".FormatThis(
                        this.ViewState["Page_Url"]
                        , pageIndex
                        , "?" + string.Join("&", Params.ToArray()));

                }

            }
        }
        catch (Exception)
        {
            throw new Exception("系統發生錯誤 - 讀取資料");
        }

        finally
        {
            if (cmd != null)
                cmd.Dispose();
            if (cmdTotalCnt != null)
                cmdTotalCnt.Dispose();
        }
    }

    protected void lvDataList_ItemDataBound(object sender, ListViewItemEventArgs e)
    {
        if (e.Item.ItemType == ListViewItemType.DataItem)
        {
            //取得資料
            string GetID = DataBinder.Eval(e.Item.DataItem, "PV_ID").ToString();
            string GetGroupID = DataBinder.Eval(e.Item.DataItem, "Group_ID").ToString();

            /** 圖片顯示 **/
            string GetPic = DataBinder.Eval(e.Item.DataItem, "PV_Pic").ToString();
            if (!string.IsNullOrEmpty(GetPic))
            {
                //取得控制項
                Literal lt_Pic = (Literal)e.Item.FindControl("lt_Pic");

                //顯示Html
                lt_Pic.Text = "<img data-original=\"{0}\" src=\"{1}js/lazyload/grey.gif\" class=\"lazy fixImg img-responsive\" alt=\"\" />".FormatThis(
                        fn_stringFormat.ashx_Pic("{0}PV/{1}/{2}".FormatThis(Param_FileWebFolder, GetGroupID, GetPic))
                        , Application["WebUrl"]
                    );
            }
        }
    }

    ///// <summary>
    ///// 影片類型
    ///// </summary>
    ///// <param name="uri">網址</param>
    ///// <param name="uriName">說明</param>
    ///// <returns></returns>
    //protected string ShowUrlType(string inpuVal)
    //{
    //    //判斷是否為空
    //    if (string.IsNullOrEmpty(inpuVal))
    //    {
    //        return "";
    //    }

    //    switch (inpuVal)
    //    {
    //        case "1":
    //            //Youtube
    //            return "youtube";

    //        case "2":
    //            //優酷
    //            return "iframe";

    //        case "3":
    //            //Vimeo
    //            return "vimeo";

    //        default:
    //            return "";
    //    }

    //}
    #endregion

    #region -- 按鈕事件 --
    /// <summary>
    /// 查詢
    /// </summary>
    protected void btn_Search_Click(object sender, EventArgs e)
    {
        try
        {
            StringBuilder SBUrl = new StringBuilder();
            SBUrl.Append("{0}Video/{1}/?s=1".FormatThis(
                    Application["WebUrl"], Req_ClassType));

            //[查詢條件] - 分類
            if (this.ddl_ProdClass.SelectedIndex > 0)
            {
                SBUrl.Append("&c=" + Server.UrlEncode(this.ddl_ProdClass.SelectedValue));
            }

            //[查詢條件] - 關鍵字
            if (!string.IsNullOrEmpty(this.tb_Keyword.Text))
            {
                SBUrl.Append("&k=" + Server.UrlEncode(fn_stringFormat.Set_FilterHtml(this.tb_Keyword.Text)));
            }

            //[查詢條件] - 從別的地方來的 品號
            //if (!string.IsNullOrEmpty(Req_ModelNo))
            //{
            //    SBUrl.Append("&m=" + Server.UrlEncode(Req_ModelNo));
            //}

            //執行轉頁
            Response.Redirect(SBUrl.ToString(), false);

        }
        catch (Exception)
        {
            throw;
        }
    }
    #endregion

    #region -- 參數設定 --
    /// <summary>
    /// 取得傳遞參數 - PageIdx(目前索引頁)
    /// </summary>
    private int _Req_PageIdx;
    public int Req_PageIdx
    {
        get
        {
            int PageID = Convert.ToInt32(Page.RouteData.Values["PageID"]);
            return PageID;
        }
        set
        {
            this._Req_PageIdx = value;
        }
    }

    /// <summary>
    /// 取得傳遞參數 - ClassID
    /// </summary>
    private string _Req_ClassID;
    public string Req_ClassID
    {
        get
        {
            String ClassID = Request.QueryString["c"];
            return (fn_Extensions.String_資料長度Byte(ClassID, "1", "6", out ErrMsg)) ? fn_stringFormat.Set_FilterHtml(ClassID).Trim() : "";
        }
        set
        {
            this._Req_ClassID = value;
        }
    }


    /// <summary>
    /// 取得傳遞參數 - 區塊
    /// </summary>
    private string _Req_ClassType;
    public string Req_ClassType
    {
        get
        {
            return "Toy";
        }
        set
        {
            this._Req_ClassType = value;
        }
    }

    /// <summary>
    /// 取得傳遞參數 - Keyword
    /// </summary>
    private string _Req_Keyword;
    public string Req_Keyword
    {
        get
        {
            String Keyword = Request.QueryString["k"];
            return (fn_Extensions.String_資料長度Byte(Keyword, "1", "40", out ErrMsg)) ? fn_stringFormat.Set_FilterHtml(Keyword).Trim() : "";
        }
        set
        {
            this._Req_Keyword = value;
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
    /// 取得傳遞參數 - 品號
    /// </summary>
    private string _Req_ModelNo;
    public string Req_ModelNo
    {
        get
        {
            String Keyword = Request.QueryString["m"];
            return (fn_Extensions.String_資料長度Byte(Keyword, "1", "40", out ErrMsg)) ? fn_stringFormat.Set_FilterHtml(Keyword).Trim() : "";
        }
        set
        {
            this._Req_ModelNo = value;
        }
    }
    #endregion
}