using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ExtensionMethods;
using ExtensionUI;

public partial class QAList : System.Web.UI.Page
{
    public string ErrMsg;

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!IsPostBack)
            {
                //** 次標題 **
                this.Page.Title = Resources.resPublic.title_常見問題;

                this.tb_QAwords1.Attributes.Add("placeholder", this.GetLocalResourceObject("tip_關鍵字").ToString());
                tb_QAwords1.Text = Req_Keyword;

                //取得資料
                LookupDataList(Req_PageIdx);

                //** 設定程式編號(目前頁面所在功能位置) **
                if (false == setProgIDs.setID(this.Master, "200"))
                {
                    throw new Exception("目前的Masterpage 沒有實作 IProgID,無法傳值");
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
    /// 副程式 - 取得資料列表 (分頁)
    /// </summary>
    /// <param name="pageIndex">目前頁數</param>
    private void LookupDataList(int pageIndex)
    {
        //[參數宣告] - 共用參數
        SqlCommand cmd = new SqlCommand();
        SqlCommand cmdTotalCnt = new SqlCommand();
        try
        {
            //[參數宣告] - 設定本頁Url(末端無須加 "/")
            this.ViewState["Page_Url"] = "{0}QA/List".FormatThis(
                    Application["WebUrl"]);
            ArrayList Params = new ArrayList();

            //[參數宣告] - 筆數/分頁設定
            int PageSize = 12;  //每頁筆數
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
            SBSql.AppendLine("    SELECT GP.Model_No");
            SBSql.AppendLine("    , RTRIM(myData.Model_No) AS ModelNo, RTRIM(myData.Model_Name_{0}) AS ModelName".FormatThis(fn_Language.Param_Lang));
            //圖片(判斷圖片中心 2->1->3->4->5->7->8->9)
            SBSql.AppendLine("      , (SELECT TOP 1 (ISNULL(Pic02,'') + '|' + ISNULL(Pic01,'') + '|' + ISNULL(Pic03,'') + '|' + ISNULL(Pic04,'') ");
            SBSql.AppendLine("          + '|' + ISNULL(Pic05,'') + '|' + ISNULL(Pic07,'') + '|' + ISNULL(Pic08,'') + '|' + ISNULL(Pic09,'')) AS PicGroup");
            SBSql.AppendLine("          FROM [ProductCenter].dbo.ProdPic_Photo WITH (NOLOCK) WHERE (ProdPic_Photo.Model_No = myData.Model_No)");
            SBSql.AppendLine("      ) AS PhotoGroup ");
            SBSql.AppendLine("    , ROW_NUMBER() OVER (ORDER BY GP.IsNew DESC, GP.Sort ASC, GP.EndTime DESC) AS RowRank ");
            SBSql.AppendLine("    FROM Prod GP ");
            SBSql.AppendLine("      INNER JOIN [ProductCenter].dbo.Prod_Item myData WITH (NOLOCK) ON GP.Model_No = myData.Model_No ");
            SBSql.AppendLine("    WHERE (GP.Display = 'Y') ");
            SBSql.AppendLine("      AND (GETDATE() >= GP.StartTime) AND (GETDATE() <= GP.EndTime)");

            //SBSql.AppendLine("      AND (GP.Model_No IN (SELECT Model_No FROM FAQ_Rel_ModelNo WITH (NOLOCK)))");
            //排除無資料
            SBSql.AppendLine("  AND (GP.Model_No IN (");
            SBSql.Append(" SELECT Rel.Model_No");
            SBSql.Append(" FROM FAQ_Group GP");
            SBSql.Append("  INNER JOIN FAQ Base ON GP.Group_ID = Base.Group_ID");
            SBSql.Append("  INNER JOIN FAQ_Rel_ModelNo Rel ON GP.Group_ID = Rel.Group_ID");
            SBSql.Append(" WHERE (UPPER(Base.LangCode) = UPPER(@LangCode))");
            SBSql.AppendLine("  ))");


            #region "..查詢條件.."

            //[查詢條件] - 關鍵字
            if (!string.IsNullOrEmpty(Req_Keyword))
            {
                SBSql.Append(" AND (");
                SBSql.Append("  (myData.Model_No LIKE '%' + @Keyword + '%')");
                SBSql.Append("  OR (myData.Model_Name_zh_TW LIKE '%' + @Keyword + '%')");
                SBSql.Append("  OR (myData.Model_Name_zh_CN LIKE '%' + @Keyword + '%')");
                SBSql.Append("  OR (myData.Model_Name_en_US LIKE '%' + @Keyword + '%')");
                SBSql.Append("  OR EXISTS (");
                SBSql.Append("   SELECT Rel.Model_No FROM Prod_Rel_Tags Rel WITH (NOLOCK) INNER JOIN Prod_Tags Tag WITH (NOLOCK) ON Rel.Tag_ID = Tag.Tag_ID");
                SBSql.Append("   WHERE (Rel.Model_No = GP.Model_No) AND (Tag.Tag_Name LIKE '%' + @Keyword + '%')");
                SBSql.Append("  )");
                SBSql.Append("  OR EXISTS (");
                SBSql.Append("   SELECT Base.FAQ_ID");
                SBSql.Append("    FROM FAQ_Group FAQ_GP WITH (NOLOCK)");
                SBSql.Append("    INNER JOIN FAQ Base WITH (NOLOCK) ON FAQ_GP.Group_ID = Base.Group_ID");
                SBSql.Append("    INNER JOIN FAQ_Block Block WITH (NOLOCK) ON Base.FAQ_ID = Block.FAQ_ID");
                SBSql.Append("    INNER JOIN FAQ_Rel_ModelNo Rel WITH (NOLOCK) ON FAQ_GP.Group_ID = Rel.Group_ID");
                SBSql.Append("   WHERE (Rel.Model_No = GP.Model_No) AND (UPPER(Base.LangCode) = UPPER(@LangCode)) AND (");
                SBSql.Append("    (Block.Block_Title LIKE '%' + @Keyword + '%')");
                //SBSql.Append("     OR (Block.Block_Desc LIKE '%' + @Keyword + '%')");
                SBSql.Append("   )");
                SBSql.Append("  )");
                SBSql.Append(" )");

                cmd.Parameters.AddWithValue("Keyword", Req_Keyword);


                Params.Add(Server.UrlEncode(Req_Keyword));
            }

            #endregion

            SBSql.AppendLine(" ) AS TBL ");
            SBSql.AppendLine(" WHERE (RowRank >= @BG_ITEM) AND (RowRank <= @ED_ITEM)");
            SBSql.AppendLine(" ORDER BY RowRank ");

            //[SQL] - Command
            cmd.CommandText = SBSql.ToString();
            cmd.Parameters.AddWithValue("BG_ITEM", BgItem);
            cmd.Parameters.AddWithValue("ED_ITEM", EdItem);
            cmd.Parameters.AddWithValue("LangCode", fn_Language.PKWeb_Lang);

            #endregion

            #region - [SQL] 計算筆數 -
            //[SQL] - 計算資料總數
            SBSql.Clear();
            SBSql.AppendLine(" SELECT COUNT(*) AS TOTAL_CNT ");
            SBSql.AppendLine(" FROM Prod GP ");
            SBSql.AppendLine("   INNER JOIN [ProductCenter].dbo.Prod_Item myData WITH (NOLOCK) ON GP.Model_No = myData.Model_No ");
            SBSql.AppendLine(" WHERE (GP.Display = 'Y') ");
            SBSql.AppendLine("   AND (GETDATE() >= GP.StartTime) AND (GETDATE() <= GP.EndTime)");
            //SBSql.AppendLine("   AND (GP.Model_No IN (SELECT Model_No FROM FAQ_Rel_ModelNo WITH (NOLOCK)))");
            //排除無資料
            SBSql.AppendLine("  AND (GP.Model_No IN (");
            SBSql.Append(" SELECT Rel.Model_No");
            SBSql.Append(" FROM FAQ_Group GP");
            SBSql.Append("  INNER JOIN FAQ Base ON GP.Group_ID = Base.Group_ID");
            SBSql.Append("  INNER JOIN FAQ_Rel_ModelNo Rel ON GP.Group_ID = Rel.Group_ID");
            SBSql.Append(" WHERE (UPPER(Base.LangCode) = UPPER(@LangCode))");
            SBSql.AppendLine("  ))");

            #region "..查詢條件.."
            //[查詢條件] - 關鍵字
            if (!string.IsNullOrEmpty(Req_Keyword))
            {
                SBSql.Append(" AND (");
                SBSql.Append("  (myData.Model_No LIKE '%' + @Keyword + '%')");
                SBSql.Append("  OR (myData.Model_Name_zh_TW LIKE '%' + @Keyword + '%')");
                SBSql.Append("  OR (myData.Model_Name_zh_CN LIKE '%' + @Keyword + '%')");
                SBSql.Append("  OR (myData.Model_Name_en_US LIKE '%' + @Keyword + '%')");
                SBSql.Append("  OR EXISTS (");
                SBSql.Append("   SELECT Rel.Model_No FROM Prod_Rel_Tags Rel WITH (NOLOCK) INNER JOIN Prod_Tags Tag WITH (NOLOCK) ON Rel.Tag_ID = Tag.Tag_ID");
                SBSql.Append("   WHERE (Rel.Model_No = GP.Model_No) AND (Tag.Tag_Name LIKE '%' + @Keyword + '%')");
                SBSql.Append("  )");
                SBSql.Append("  OR EXISTS (");
                SBSql.Append("   SELECT Base.FAQ_ID");
                SBSql.Append("    FROM FAQ_Group FAQ_GP WITH (NOLOCK)");
                SBSql.Append("    INNER JOIN FAQ Base WITH (NOLOCK) ON FAQ_GP.Group_ID = Base.Group_ID");
                SBSql.Append("    INNER JOIN FAQ_Block Block WITH (NOLOCK) ON Base.FAQ_ID = Block.FAQ_ID");
                SBSql.Append("    INNER JOIN FAQ_Rel_ModelNo Rel WITH (NOLOCK) ON FAQ_GP.Group_ID = Rel.Group_ID");
                SBSql.Append("   WHERE (Rel.Model_No = GP.Model_No) AND (UPPER(Base.LangCode) = UPPER(@LangCode)) AND (");
                SBSql.Append("    (Block.Block_Title LIKE '%' + @Keyword + '%')");
                //SBSql.Append("     OR (Block.Block_Desc LIKE '%' + @Keyword + '%')");
                SBSql.Append("   )");
                SBSql.Append("  )");
                SBSql.Append(" )");

                cmdTotalCnt.Parameters.AddWithValue("Keyword", Req_Keyword);
            }

            #endregion

            //[SQL] - Command
            cmdTotalCnt.CommandText = SBSql.ToString();
            cmdTotalCnt.Parameters.AddWithValue("LangCode", fn_Language.PKWeb_Lang);

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
                    lt_Pager.Text = fn_CustomUI.PageControl(TotalRow, PageSize, pageIndex, 5, this.ViewState["Page_Url"].ToString(), Params, true, true);
                }

                //[頁數判斷] - 目前頁數大於總頁數, 則導向第一頁
                if (pageIndex > TotalRow && TotalRow > 0)
                {
                    Response.Redirect("{0}/{1}/{2}".FormatThis(
                            this.ViewState["Page_Url"]
                            , 1
                            , (string.IsNullOrEmpty(Req_Keyword)) ? "ALL" : HttpUtility.UrlEncode(Req_Keyword)));
                }
                else
                {
                    //重新取得頁面Url
                    this.ViewState["Page_Url"] = "{0}/{1}/{2}".FormatThis(
                        this.ViewState["Page_Url"]
                        , pageIndex
                        , (string.IsNullOrEmpty(Req_Keyword)) ? "ALL" : HttpUtility.UrlEncode(Req_Keyword));

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


    /// <summary>
    /// 取得產品圖
    /// </summary>
    /// <param name="PhotoGroup">圖片集合</param>
    /// <param name="Model_No">品號</param>
    /// <returns></returns>
    protected string Get_Pic(string PhotoGroup, string Model_No)
    {
        //判斷參數
        if (string.IsNullOrEmpty(Model_No))
        {
            return "";
        }

        //拆解圖片值 "|"
        string Photo = "";
        string[] strAry = Regex.Split(PhotoGroup, @"\|{1}");
        for (int row = 0; row < strAry.Length; row++)
        {
            if (false == string.IsNullOrEmpty(strAry[row].ToString()))
            {
                Photo = strAry[row].ToString();
                break;
            }
        }

        //判斷是否有圖片
        if (string.IsNullOrEmpty(Photo))
        {
            return "<img data-original=\"{0}images/NoPic.png\" src=\"{0}js/lazyload/grey.gif\" class=\"lazy img-responsive fixImg\" alt=\"\" />".FormatThis(
                Application["WebUrl"]);
        }
        else
        {
            //實際檔案資料夾路徑
            string fileRealPath = string.Format("ProductPic/{0}/{1}/{2}"
                , Model_No
                , "1"
                , Photo);

            string downloadPath = "<img data-original=\"{0}\" src=\"{1}js/lazyload/grey.gif\" class=\"lazy img-responsive fixImg\" alt=\"\" />".FormatThis(
                   Application["File_WebUrl"] + fileRealPath
                    , Application["WebUrl"]
                );

            return downloadPath;
        }
    }
    #endregion

    #region -- 按鈕事件 --
    /// <summary>
    /// 查詢
    /// </summary>
    protected void btn_Search_Click(object sender, EventArgs e)
    {
        try
        {
            string myUrl = "{0}QA/List".FormatThis(Application["WebUrl"]);


            //[查詢條件] - 關鍵字1
            if (!string.IsNullOrEmpty(this.tb_QAwords1.Text))
            {
                myUrl = "{0}QA/List/1/{1}".FormatThis(
                        Application["WebUrl"], Server.UrlEncode(fn_stringFormat.Set_FilterHtml(this.tb_QAwords1.Text))
                    );
            }
            
            //執行轉頁
            Response.Redirect(myUrl, false);

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
    /// 取得傳遞參數 - Keyword
    /// </summary>
    private string _Req_Keyword;
    public string Req_Keyword
    {
        get
        {
            String myData = Convert.ToString(Page.RouteData.Values["myData"]);
            return (myData.ToUpper().Equals("ALL")) ? "" : myData;
        }
        set
        {
            this._Req_Keyword = value;
        }
    }

    ///// <summary>
    ///// 取得產品網址,使用API
    ///// </summary>
    //private string _ProdUrl;
    //public string ProdUrl
    //{
    //    get
    //    {
    //        string url = "http://go.prokits.com.tw/";

    //        switch (fn_Area.PKWeb_Area)
    //        {
    //            case "2":
    //                return url + "ProdB/";

    //            case "3":
    //                return url + "ProdC/";

    //            default:
    //                return url + "ProdA/";
    //        }
    //    }
    //    set
    //    {
    //        this._ProdUrl = value;
    //    }
    //}
    #endregion
}