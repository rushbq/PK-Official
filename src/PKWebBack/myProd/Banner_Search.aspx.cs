using ExtensionMethods;
using ExtensionUI;
using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Prod_BannerSearch : SecurityCheck
{
    public string ErrMsg;

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!IsPostBack)
            {
                //[權限判斷]
                if (fn_CheckAuth.CheckAuth("450", out ErrMsg) == false)
                {
                    Response.Redirect("{0}401.aspx".FormatThis(Application["WebUrl"]));
                    return;
                }

                //** 設定程式編號(重要) **
                if (false == setProgIDs.setID(this.Master, "400", "450"))
                {
                    throw new Exception("目前的Masterpage 沒有實作 IProgID,無法傳值");
                }

                //[取得/檢查參數] - 類型:工具/玩具
                if (fn_CustomUI.Get_ProdType(this.ddl_Type, Req_pType, true, out ErrMsg) == false)
                {
                    this.ddl_Type.Items.Insert(0, new ListItem("選單產生失敗", ""));
                }

                //[取得/檢查參數] - 產品類別
                if (fn_CustomUI.Get_ProdAllClass(this.ddl_ProdClass, Req_ProdClass, true, out ErrMsg) == false)
                {
                    this.ddl_ProdClass.Items.Insert(0, new ListItem("選單產生失敗", ""));
                }

                //[帶出Tab]
                this.lt_Tabs.Text = Get_Tab();
                if (!Req_Area.Equals("C"))
                {
                    ddl_ProdClass.Enabled = false;
                }

                //[帶出資料]
                LookupDataList(Req_PageIdx);

            }
        }
        catch (Exception)
        {

            throw;
        }
    }

    #region -- 資料取得 --
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
            this.ViewState["Page_Url"] = Application["WebUrl"] + "Prod/BannerSearch";
            ArrayList Params = new ArrayList();

            //[參數宣告] - 筆數/分頁設定
            int PageSize = 10;  //每頁筆數
            int TotalRow = 0;  //總筆數
            int BgItem = (pageIndex - 1) * PageSize + 1;  //開始筆數
            int EdItem = BgItem + (PageSize - 1);  //結束筆數 

            //[SQL] - 清除參數設定
            cmd.Parameters.Clear();
            cmdTotalCnt.Parameters.Clear();
            string sql;

            #region - [SQL] 資料顯示 -
            sql = @"
                ;WITH TblProdClass AS (
	                SELECT ProdAllClass.*
	                FROM (
		                SELECT RTRIM(Cls.Class_ID) AS ID, Cls.Class_Name_zh_TW AS Label, '工具' AS GroupLabel, Cls.Sort
		                FROM [ProductCenter].dbo.Prod_Class Cls WITH (NOLOCK)
		                WHERE (LEFT(RTRIM(Cls.Class_ID),1) = '2') AND (Cls.Display = 'Y') AND (Cls.Display_PKWeb = 'Y')
		                UNION ALL
		                SELECT RTRIM(Cls.Class_ID) AS ID, Cls.Class_Name_zh_TW AS Label, '玩具' AS GroupLabel, 1000 + Cls.Sort
		                FROM [ProductCenter].dbo.ProdToy_Class Cls WITH (NOLOCK)
		                WHERE (Cls.Display = 'Y') AND (Cls.Display_PKWeb = 'Y')
	                ) AS ProdAllClass
                )
                SELECT Tbl.* 
                FROM (
	                SELECT Base.SeqNo, Base.Subject, Base.Content1
	                , Base.LangCode
	                , Base.功能區塊
	                , Base.產品類型
	                , Base.關聯編號
	                , cls1.Class_Name AS 'desc功能區塊'
	                , cls2.Class_Name AS 'desc產品類型'
	                , prodCls.Label AS 'desc類別名稱'
	                , ROW_NUMBER() OVER (ORDER BY Base.產品類型, Base.關聯編號) AS RowRank
	                FROM [產品宣傳] Base
	                 INNER JOIN [產品宣傳參數] cls1 ON Base.功能區塊 = cls1.Class_MenuID AND cls1.Class_Type = 'A'
	                 INNER JOIN [產品宣傳參數] cls2 ON Base.產品類型 = cls2.Class_MenuID AND cls2.Class_Type = 'B'
	                 LEFT JOIN TblProdClass prodCls ON Base.關聯編號 = prodCls.ID
	                WHERE (1=1)";


            #region "..查詢條件.."
            //[查詢條件] - 類型
            if (!string.IsNullOrEmpty(Req_pType))
            {
                sql += " AND (Base.產品類型 = @pType)";

                cmd.Parameters.AddWithValue("pType", Req_pType);
                Params.Add("pType=" + Server.UrlEncode(Req_pType));
            }

            //[查詢條件] - 產品類別
            if (!string.IsNullOrEmpty(Req_ProdClass))
            {
                sql += " AND (Base.關聯編號 = @ProdClass)";

                cmd.Parameters.AddWithValue("ProdClass", Req_ProdClass);
                Params.Add("ProdClass=" + Server.UrlEncode(Req_ProdClass));
            }

            //[查詢條件] - Tab
            if (!string.IsNullOrEmpty(Req_Area))
            {
                sql += " AND (Base.功能區塊 = @Area)";

                cmd.Parameters.AddWithValue("Area", Req_Area);
                Params.Add("Area=" + Server.UrlEncode(Req_Area));
            }

            #endregion

            sql += " ) AS Tbl";
            sql += " WHERE (RowRank >= @BG_ITEM) AND (RowRank <= @ED_ITEM) ORDER BY RowRank";

            //[SQL] - Command
            cmd.CommandText = sql;
            cmd.Parameters.AddWithValue("BG_ITEM", BgItem);
            cmd.Parameters.AddWithValue("ED_ITEM", EdItem);

            #endregion

            #region - [SQL] 計算筆數 -
            sql = @"
                 SELECT COUNT(Base.SeqNo) AS TOTAL_CNT 
                FROM [產品宣傳] Base 
                INNER JOIN [產品宣傳參數] cls1 ON Base.功能區塊 = cls1.Class_MenuID AND cls1.Class_Type = 'A'
                INNER JOIN [產品宣傳參數] cls2 ON Base.產品類型 = cls2.Class_MenuID AND cls2.Class_Type = 'B'
                WHERE (1 = 1) ";

            #region "..查詢條件.."

            //[查詢條件] - 類型
            if (!string.IsNullOrEmpty(Req_pType))
            {
                sql += " AND (Base.產品類型 = @pType)";

                cmdTotalCnt.Parameters.AddWithValue("pType", Req_pType);
            }

            //[查詢條件] - 產品類別
            if (!string.IsNullOrEmpty(Req_ProdClass))
            {
                sql += " AND (Base.關聯編號 = @ProdClass)";

                cmdTotalCnt.Parameters.AddWithValue("ProdClass", Req_ProdClass);
            }

            //[查詢條件] - Tab
            if (!string.IsNullOrEmpty(Req_Area))
            {
                sql += " AND (Base.功能區塊 = @Area)";

                cmdTotalCnt.Parameters.AddWithValue("Area", Req_Area);
            }

            #endregion

            //[SQL] - Command
            cmdTotalCnt.CommandText = sql;
            #endregion

            //[SQL] - 取得資料
            using (DataTable DT = dbConn.LookupDTwithPage(cmd, cmdTotalCnt, out TotalRow, out ErrMsg))
            {
                //DataBind            
                this.lvDataList.DataSource = DT.DefaultView;
                this.lvDataList.DataBind();

                //Page
                if (DT.Rows.Count > 0)
                {
                    //顯示分頁, 需在DataBind之後
                    Literal lt_Pager = (Literal)this.lvDataList.FindControl("lt_Pager");
                    lt_Pager.Text = fn_CustomUI.PageControl(TotalRow, PageSize, pageIndex, 5, this.ViewState["Page_Url"].ToString(), Params, true);
                }

                //[頁數判斷] - 目前頁數大於總頁數, 則導向第一頁
                //計算總頁數
                int TotalPage = (TotalRow / PageSize);
                if (TotalRow % PageSize > 0)
                {
                    TotalPage++;
                }
                if (pageIndex > TotalPage && TotalPage > 0)
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

                    //暫存頁面Url, 給其他頁使用
                    Session["BackListUrl"] = this.ViewState["Page_Url"];
                }

            }
        }
        catch (Exception ex)
        {
            throw new Exception("系統發生錯誤 - 讀取資料." + ex.Message.ToString());
        }
        finally
        {
            if (cmd != null)
                cmd.Dispose();
            if (cmdTotalCnt != null)
                cmdTotalCnt.Dispose();
        }
    }

    protected void lvDataList_ItemCommand(object sender, ListViewCommandEventArgs e)
    {
        try
        {
            if (e.Item.ItemType == ListViewItemType.DataItem)
            {
                ////取得Key值
                //string Get_DataID = ((HiddenField)e.Item.FindControl("hf_DataID")).Value;

                //using (SqlCommand cmd = new SqlCommand())
                //{
                //    //刪除資料
                //    StringBuilder SBSql = new StringBuilder();
                //    SBSql.AppendLine(" DELETE FROM Prod_Rel_Tags WHERE (Model_No = @Param_ID); ");
                //    SBSql.AppendLine(" DELETE FROM Prod_Rel_CertIcon WHERE (Model_No = @Param_ID); ");
                //    SBSql.AppendLine(" DELETE FROM Prod WHERE (Prod_ID = @Param_ID); ");

                //    cmd.CommandText = SBSql.ToString();
                //    cmd.Parameters.Clear();
                //    cmd.Parameters.AddWithValue("Param_ID", Get_DataID);
                //    if (dbConn.ExecuteSql(cmd, out ErrMsg) == false)
                //    {
                //        fn_Extensions.JsAlert("資料處理失敗", "");
                //        return;
                //    }
                //    else
                //    {
                //        //導向列表頁
                //        fn_Extensions.JsAlert("", this.ViewState["Page_Url"].ToString());
                //    }
                //}
            }
        }
        catch (Exception)
        {
            throw new Exception("系統發生錯誤 - ItemCommand");
        }

    }

    protected void lvDataList_ItemDataBound(object sender, ListViewItemEventArgs e)
    {
        try
        {
            if (e.Item.ItemType == ListViewItemType.DataItem)
            {
                //ListViewDataItem dataItem = (ListViewDataItem)e.Item;

                ////取得資料, 判斷狀態
                //string GetDisp = DataBinder.Eval(dataItem.DataItem, "Display").ToString();

            }
        }
        catch (Exception)
        {

            throw new Exception("系統發生錯誤 - ItemDataBound！");
        }
    }


    /// <summary>
    /// 取得Tab:功能區塊
    /// </summary>
    /// <returns></returns>
    private string Get_Tab()
    {
        try
        {
            var query = fn_CustomUI.Get_宣傳參數("A", out ErrMsg).AsEnumerable();
            if (!query.Any())
            {
                return "";
            }

            StringBuilder html = new StringBuilder();

            html.AppendLine("<ul class=\"nav nav-tabs\">");

            foreach (var item in query)
            {
                string GetID = item.Field<string>("ID");
                string GetLabel = item.Field<string>("Label");

                html.AppendLine("<li class=\"{2}\"><a href=\"{0}\">{1}</a></li>".FormatThis(
                        Application["WebUrl"] + "Prod/BannerSearch/?srh=1&area=" + GetID
                        , GetLabel
                        , Req_Area.Equals(GetID) ? "active" : ""
                    ));
            }

            html.AppendLine("</ul>");

            return html.ToString();
        }
        catch (Exception)
        {

            throw;
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
            StringBuilder SBUrl = new StringBuilder();
            SBUrl.Append("{0}Prod/BannerSearch/?srh=1".FormatThis(Application["WebUrl"]));

            //[查詢條件] - pType
            if (this.ddl_Type.SelectedIndex > 0)
            {
                SBUrl.Append("&pType=" + Server.UrlEncode(this.ddl_Type.SelectedValue));
            }

            //[查詢條件] - ProdClass
            if (this.ddl_ProdClass.SelectedIndex > 0)
            {
                SBUrl.Append("&ProdClass=" + Server.UrlEncode(this.ddl_ProdClass.SelectedValue));
            }

            //[查詢條件] - Tab
            SBUrl.Append("&Area=" + Server.UrlEncode(Req_Area));

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
    /// 取得傳遞參數 - Area
    /// </summary>
    private string _Req_Area;
    public string Req_Area
    {
        get
        {
            String ReqData = Request.QueryString["Area"];
            return (fn_Extensions.String_資料長度Byte(ReqData, "1", "2", out ErrMsg)) ? fn_stringFormat.Set_FilterHtml(ReqData).Trim() : "A";
        }
        set
        {
            this._Req_Area = value;
        }
    }

    /// <summary>
    /// 取得傳遞參數 - pType
    /// </summary>
    private string _Req_pType;
    public string Req_pType
    {
        get
        {
            String Disp = Request.QueryString["pType"];
            return (fn_Extensions.String_資料長度Byte(Disp, "1", "4", out ErrMsg)) ? fn_stringFormat.Set_FilterHtml(Disp).Trim() : "";
        }
        set
        {
            this._Req_pType = value;
        }
    }

    /// <summary>
    /// 取得傳遞參數 - ProdClass
    /// </summary>
    private string _Req_ProdClass;
    public string Req_ProdClass
    {
        get
        {
            String ReqData = Request.QueryString["ProdClass"];
            return (fn_Extensions.String_資料長度Byte(ReqData, "1", "4", out ErrMsg)) ? fn_stringFormat.Set_FilterHtml(ReqData).Trim() : "";
        }
        set
        {
            this._Req_ProdClass = value;
        }
    }

    #endregion

}