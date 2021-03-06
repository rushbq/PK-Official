using ExtensionIO;
using ExtensionMethods;
using ExtensionUI;
using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Tags_Search : SecurityCheck
{
    public string ErrMsg;

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!IsPostBack)
            {
                //[權限判斷]
                if (fn_CheckAuth.CheckAuth("440", out ErrMsg) == false)
                {
                    Response.Redirect("{0}401.aspx".FormatThis(Application["WebUrl"]));
                    return;
                }

                //** 設定程式編號(重要) **
                if (false == setProgIDs.setID(this.Master, "400", "440"))
                {
                    throw new Exception("目前的Masterpage 沒有實作 IProgID,無法傳值");
                }

                //[取得/檢查參數] - Keyword
                if (!string.IsNullOrEmpty(Req_Keyword))
                {
                    this.tb_Keyword.Text = Req_Keyword;
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
            //[參數宣告] - 設定本頁Url(末端不用加 "/")
            this.ViewState["Page_Url"] = "{0}Config/Tags".FormatThis(Application["WebUrl"]);
            ArrayList Params = new ArrayList();

            //[參數宣告] - 筆數/分頁設定
            int PageSize = 14;  //每頁筆數
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
            SBSql.AppendLine("    SELECT ");
            SBSql.AppendLine("      Base.Tag_ID, Base.Tag_Name, Base.Tag_Pic");
            SBSql.AppendLine("      , (SELECT COUNT(*) FROM Prod_Rel_Tags Rel WHERE (Base.Tag_ID = Rel.Tag_ID)) AS ItemCnt");
            SBSql.AppendLine("      , ROW_NUMBER() OVER (ORDER BY Base.Tag_Name) AS RowRank ");
            SBSql.AppendLine("    FROM Prod_Tags Base ");
            SBSql.AppendLine("    WHERE (1 = 1) ");

            #region "..查詢條件.."

            //[查詢條件] - 關鍵字
            if (!string.IsNullOrEmpty(Req_Keyword))
            {
                SBSql.Append(" AND ( ");
                SBSql.Append("  (Base.Tag_Name LIKE '%' + @Keyword + '%') ");
                SBSql.Append(" ) ");
                cmd.Parameters.AddWithValue("Keyword", Req_Keyword);

                Params.Add("Keyword=" + Server.UrlEncode(Req_Keyword));
            }

            #endregion

            SBSql.AppendLine(" ) AS TBL ");
            SBSql.AppendLine(" WHERE (RowRank >= @BG_ITEM) AND (RowRank <= @ED_ITEM)");
            SBSql.AppendLine(" ORDER BY RowRank ");

            //[SQL] - Command
            cmd.CommandText = SBSql.ToString();
            cmd.Parameters.AddWithValue("BG_ITEM", BgItem);
            cmd.Parameters.AddWithValue("ED_ITEM", EdItem);

            #endregion

            #region - [SQL] 計算筆數 -
            //[SQL] - 計算資料總數
            SBSql.Clear();
            SBSql.AppendLine(" SELECT COUNT(*) AS TOTAL_CNT ");
            SBSql.AppendLine(" FROM Prod_Tags Base ");
            SBSql.AppendLine(" WHERE (1 = 1) ");

            #region "..查詢條件.."

            //[查詢條件] - 關鍵字
            if (!string.IsNullOrEmpty(Req_Keyword))
            {
                SBSql.Append(" AND ( ");
                SBSql.Append("  (Base.Tag_Name LIKE '%' + @Keyword + '%') ");
                SBSql.Append(" ) ");

                cmdTotalCnt.Parameters.AddWithValue("Keyword", Req_Keyword);
            }

            #endregion

            //[SQL] - Command
            cmdTotalCnt.CommandText = SBSql.ToString();
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
                    //Session["BackListUrl"] = this.ViewState["Page_Url"];
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

    protected void lvDataList_ItemCommand(object sender, ListViewCommandEventArgs e)
    {
        try
        {
            if (e.Item.ItemType == ListViewItemType.DataItem)
            {
                //Params
                StringBuilder SBSql = new StringBuilder();
                string Get_DataID = ((HiddenField)e.Item.FindControl("hf_DataID")).Value;
                string Get_Oldpic = ((HiddenField)e.Item.FindControl("hf_OldFile")).Value;
                TextBox tb_Keyword = (TextBox)e.Item.FindControl("tb_Keyword");
                FileUpload fu_Banner = (FileUpload)e.Item.FindControl("fu_Banner");
                string GetNewFileName = "";

                //判斷命令名稱
                switch (e.CommandName.ToLower())
                {
                    case "edit":
                        //post file
                        HttpPostedFile GetFile = fu_Banner.PostedFile;
                        if (GetFile.ContentLength > 0)
                        {
                            //[IO] - 取得檔案名稱
                            IOManage.GetFileName(GetFile);
                            GetNewFileName = IOManage.FileNewName;

                            //判斷副檔名，未符合規格的檔案不上傳
                            if (!fn_Extensions.CheckStrWord(IOManage.FileExtend, FileExtLimit, "|", 1))
                            {
                                fn_Extensions.JsAlert("檔案格式不正確，請使用 " + FileExtLimit, "");
                                return;
                            }

                            //Upload File
                            IOManage.Save(GetFile, Param_FileFolder, GetNewFileName);

                            if (!IOManage.Message.Equals("OK"))
                            {
                                fn_Extensions.JsAlert("圖片上傳失敗", "");
                                return;
                            }
                        }

                        SBSql.Append("UPDATE Prod_Tags SET Tag_Name = @Tag_Name, Tag_Pic = @Tag_Pic WHERE (Tag_ID = @Data_ID)");

                        break;

                    case "delpic":
                        IOManage.DelFile(Param_FileFolder, Get_Oldpic);
                        SBSql.Append("UPDATE Prod_Tags SET Tag_Pic = NULL WHERE (Tag_ID = @Data_ID)");

                        break;


                    case "del":
                        SBSql.Append("DELETE FROM Prod_Tags WHERE (Tag_ID = @Data_ID)");

                        break;

                }

                //執行SQL
                if (!string.IsNullOrEmpty(SBSql.ToString()))
                {

                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.CommandText = SBSql.ToString();
                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("Tag_Name", tb_Keyword.Text);
                        cmd.Parameters.AddWithValue("Tag_Pic", string.IsNullOrWhiteSpace(GetNewFileName) ? Get_Oldpic : GetNewFileName);
                        cmd.Parameters.AddWithValue("Data_ID", Get_DataID);
                        if (dbConn.ExecuteSql(cmd, out ErrMsg) == false)
                        {
                            fn_Extensions.JsAlert("資料處理失敗", "");
                            return;
                        }
                        else
                        {
                            Response.Redirect(this.ViewState["Page_Url"].ToString());
                        }
                    }
                }

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
                ListViewDataItem dataItem = (ListViewDataItem)e.Item;

                //判斷是否已被使用, 未被使用才能刪除
                int Get_ItemCnt = Convert.ToInt32(DataBinder.Eval(dataItem.DataItem, "ItemCnt"));
                LinkButton lbtn_Del = (LinkButton)e.Item.FindControl("lbtn_Del");
                lbtn_Del.Visible = Get_ItemCnt.Equals(0);

                //Tag Pic
                string Tag_Pic = DataBinder.Eval(dataItem.DataItem, "Tag_Pic").ToString();
                ((PlaceHolder)e.Item.FindControl("ph_NewUpload")).Visible = string.IsNullOrWhiteSpace(Tag_Pic);
                ((PlaceHolder)e.Item.FindControl("ph_ViewUpload")).Visible = !string.IsNullOrWhiteSpace(Tag_Pic);

            }
        }
        catch (Exception)
        {

            throw new Exception("系統發生錯誤 - ItemDataBound！");
        }
    }

    protected void lvDataList_ItemEditing(object sender, ListViewEditEventArgs e)
    {

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
            SBUrl.Append("{0}Config/Tags/?srh=1".FormatThis(Application["WebUrl"]));

            //[查詢條件] - 關鍵字
            if (!string.IsNullOrEmpty(this.tb_Keyword.Text))
            {
                SBUrl.Append("&Keyword=" + Server.UrlEncode(fn_stringFormat.Set_FilterHtml(this.tb_Keyword.Text)));
            }

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
    /// 取得傳遞參數 - Keyword
    /// </summary>
    private string _Req_Keyword;
    public string Req_Keyword
    {
        get
        {
            String Keyword = Request.QueryString["Keyword"];
            return (fn_Extensions.String_資料長度Byte(Keyword, "1", "40", out ErrMsg)) ? fn_stringFormat.Set_FilterHtml(Keyword).Trim() : "";
        }
        set
        {
            this._Req_Keyword = value;
        }
    }


    #endregion


    #region -- 上傳參數 --
    /// <summary>
    /// [參數] - 檔案資料夾路徑
    /// </summary>
    private string _Param_FileFolder;
    public string Param_FileFolder
    {
        get
        {
            return this._Param_FileFolder != null
                ? this._Param_FileFolder
                : @"{0}Tag\".FormatThis(Application["File_DiskUrl"]);
        }
        set
        {
            this._Param_FileFolder = value;
        }
    }

    /// <summary>
    /// [參數] - 檔案Web資料夾路徑
    /// </summary>
    private string _Param_WebFolder;
    public string Param_WebFolder
    {
        get
        {
            return this._Param_WebFolder != null
                ? this._Param_WebFolder
                : @"{0}Tag/".FormatThis(Application["File_WebUrl"]);
        }
        set
        {
            this._Param_WebFolder = value;
        }
    }

    /// <summary>
    /// 限制上傳的副檔名
    /// </summary>
    private string _FileExtLimit;
    public string FileExtLimit
    {
        get
        {
            return "jpg|png";
        }
        set
        {
            this._FileExtLimit = value;
        }
    }

    /// <summary>
    /// 圖片設定寬度
    /// </summary>
    private int _Param_Width;
    public int Param_Width
    {
        get
        {
            return 1280;
        }
        set
        {
            this._Param_Width = value;
        }
    }
    /// <summary>
    /// 圖片設定高度
    /// </summary>
    private int _Param_Height;
    public int Param_Height
    {
        get
        {
            return 1024;
        }
        set
        {
            this._Param_Height = value;
        }
    }


    /// <summary>
    /// 暫存參數
    /// </summary>
    public class TempParam
    {
        /// <summary>
        /// [參數] - 圖片檔名
        /// </summary>
        private string _Param_Pic;
        public string Param_Pic
        {
            get { return this._Param_Pic; }
            set { this._Param_Pic = value; }
        }

        /// <summary>
        /// [參數] - 圖片原始名稱
        /// </summary>
        private string _Param_OrgPic;
        public string Param_OrgPic
        {
            get { return this._Param_OrgPic; }
            set { this._Param_OrgPic = value; }
        }

        /// <summary>
        /// [參數] - 圖片類別
        /// </summary>
        private string _Param_FileKind;
        public string Param_FileKind
        {
            get { return this._Param_FileKind; }
            set { this._Param_FileKind = value; }
        }

        private HttpPostedFile _Param_hpf;
        public HttpPostedFile Param_hpf
        {
            get { return this._Param_hpf; }
            set { this._Param_hpf = value; }
        }

        /// <summary>
        /// 設定參數值
        /// </summary>
        /// <param name="Param_Pic">系統檔名</param>
        /// <param name="Param_OrgPic">原始檔名</param>
        /// <param name="Param_hpf">上傳檔案</param>
        public TempParam(string Param_Pic, string Param_OrgPic, HttpPostedFile Param_hpf)
        {
            this._Param_Pic = Param_Pic;
            this._Param_OrgPic = Param_OrgPic;
            this._Param_hpf = Param_hpf;
        }

    }
    #endregion

}