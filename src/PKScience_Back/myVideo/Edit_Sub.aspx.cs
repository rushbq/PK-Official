using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using ExtensionIO;
using ExtensionMethods;


public partial class Video_Edit_Sub : SecurityCheck
{
    public string ErrMsg;
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!IsPostBack)
            {
                //[權限判斷]
                if (fn_CheckAuth.CheckAuth("510", out ErrMsg) == false)
                {
                    Response.Redirect("{0}401.aspx".FormatThis(Application["WebUrl"]));
                    return;
                }

                //** 設定程式編號(重要) **
                if (false == setProgIDs.setID(this.Master, "500", "510"))
                {
                    throw new Exception("目前的Masterpage 沒有實作 IProgID,無法傳值");
                }

                //[必要參數判斷]
                if (string.IsNullOrEmpty(Param_parentID))
                {
                    fn_Extensions.JsAlert("你不乖!快去吃乖乖", "script:history.back(-1);");
                    return;
                }

                //帶出基本資料
                LookupData_Base();

                //[參數判斷] - 判斷是否有資料編號
                if (!string.IsNullOrEmpty(Param_thisID))
                {
                    LookupData();
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
    /// 基本資料顯示
    /// </summary>
    private void LookupData_Base()
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
                SBSql.AppendLine(" SELECT GP.Group_Name");
                SBSql.AppendLine("  , (SELECT LangName FROM PKSYS.dbo.Param_Language WHERE (LOWER(LangCode) = LOWER(@LangCode))) AS Lang_Name");
                SBSql.AppendLine(" FROM Movies_Group GP ");
                SBSql.AppendLine(" WHERE (GP.Group_ID = @parentID)");
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.AddWithValue("parentID", Param_parentID);
                cmd.Parameters.AddWithValue("LangCode", Param_LangCode);
                using (DataTable DT = dbConn.LookupDT(cmd, out ErrMsg))
                {
                    if (DT.Rows.Count == 0)
                    {
                        fn_Extensions.JsAlert("查無資料！", Page_LastUrl);
                        return;
                    }
                    else
                    {
                        //[填入資料]
                        this.lt_NavSubject.Text = DT.Rows[0]["Group_Name"].ToString();
                        this.lb_Group_Name.Text = DT.Rows[0]["Group_Name"].ToString();
                        this.lb_Lang.Text = DT.Rows[0]["Lang_Name"].ToString();
                    }
                }
            }
        }
        catch (Exception)
        {
            throw new Exception("系統發生錯誤 - 基本資料顯示");
        }
    }

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
                SBSql.AppendLine(" SELECT Base.Data_ID, Base.Data_Title, Base.Data_PubDate, Base.Data_ListPic, Base.Data_UrlSource, Base.Data_Url");
                SBSql.AppendLine(" FROM Movies Base ");
                SBSql.AppendLine(" WHERE (Base.Data_ID = @DataID)");
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.AddWithValue("DataID", Param_thisID);
                using (DataTable DT = dbConn.LookupDT(cmd, out ErrMsg))
                {
                    if (DT.Rows.Count == 0)
                    {
                        fn_Extensions.JsAlert("查無資料！", Page_LastUrl);
                        return;
                    }
                    else
                    {
                        //[填入資料]
                        this.tb_Data_Title.Text = DT.Rows[0]["Data_Title"].ToString();

                        string pubDate = DT.Rows[0]["Data_PubDate"].ToString().ToDateString("yyyy/MM/dd");
                        this.show_PubDate.Text = pubDate;
                        this.tb_PubDate.Text = pubDate;
                        this.rbl_Data_UrlSource.SelectedValue = DT.Rows[0]["Data_UrlSource"].ToString();
                        this.tb_Data_Url.Text = DT.Rows[0]["Data_Url"].ToString();

                        //檔案Url - 小圖
                        string myFile = DT.Rows[0]["Data_ListPic"].ToString();
                        this.hf_OldFile_Small.Value = myFile;
                        if (!string.IsNullOrEmpty(myFile))
                        {
                            this.lt_dwUrl_Small.Text = "<a href=\"{0}\" class=\"btn btn-info zoomPic\" data-gall=\"singlePic\" title=\"\"><i class=\"fa fa-eye\"></i></a>".FormatThis(
                                Param_WebFolder + myFile);

                            this.ph_files_Small.Visible = true;
                        }


                        //Flag設定 & 欄位顯示/隱藏
                        this.hf_flag.Value = "Edit";

                    }
                }
            }
        }
        catch (Exception)
        {
            throw new Exception("系統發生錯誤 - 資料顯示");
        }
    }

    #endregion -- 資料顯示 End --


    #region -- 資料編輯 Start --
    /// <summary>
    /// 基本設定存檔
    /// </summary>
    protected void btn_Save_Click(object sender, EventArgs e)
    {
        try
        {
            #region "..欄位檢查.."
            StringBuilder SBAlert = new StringBuilder();

            //[參數檢查] - 必填項目
            if (fn_Extensions.String_資料長度Byte(this.tb_Data_Title.Text, "1", "150", out ErrMsg) == false)
            {
                SBAlert.Append("「標題」請輸入1 ~ 70個字\\n");
            }

            //[JS] - 判斷是否有警示訊息
            if (string.IsNullOrEmpty(SBAlert.ToString()) == false)
            {
                fn_Extensions.JsAlert(SBAlert.ToString(), "");
                return;
            }
            #endregion

            #region --檔案處理--
            //副檔名檢查參數
            int errExt = 0;

            //[IO] - 暫存檔案名稱
            List<TempParam> ITempList = new List<TempParam>();

            //小圖
            HttpPostedFile hpFile = this.fu_Files_Small.PostedFile;
            if (hpFile != null)
            {
                if (hpFile.ContentLength > 0)
                {
                    //[IO] - 取得檔案名稱
                    IOManage.GetFileName(hpFile);

                    //判斷副檔名，未符合規格的檔案不上傳
                    if (fn_Extensions.CheckStrWord(IOManage.FileExtend, FileExtLimit, "|", 1))
                    {
                        //暫存檔案資訊
                        ITempList.Add(new TempParam(IOManage.FileNewName, this.hf_OldFile_Small.Value, hpFile, "P1"));
                    }
                    else
                    {
                        errExt++;
                    }

                }
            }


            //未符合檔案規格的警示訊息
            if (errExt > 0)
            {
                fn_Extensions.JsAlert("上傳內容含有不正確的副檔名\\n請重新挑選!!", "");
                return;
            }
            #endregion

            #region "..資料儲存.."
            //判斷是新增 or 修改
            switch (this.hf_flag.Value.ToUpper())
            {
                case "ADD":
                    Add_Data(ITempList);
                    break;

                case "EDIT":
                    Edit_Data(ITempList);
                    break;

                default:
                    throw new Exception("走錯路囉!");
            }
            #endregion

        }
        catch (Exception)
        {
            fn_Extensions.JsAlert("系統發生錯誤 - 存檔", "");
            return;
        }

    }

    /// <summary>
    /// 資料新增
    /// </summary>
    private void Add_Data(List<TempParam> ITempList)
    {
        using (SqlCommand cmd = new SqlCommand())
        {
            //宣告
            StringBuilder SBSql = new StringBuilder();
            int NewID;
            string pic1 = "";

            //取得圖片參數
            var queryPic = from el in ITempList
                           select new
                           {
                               NewPic = el.Param_Pic,
                               PicKind = el.Param_FileKind
                           };
            foreach (var item in queryPic)
            {
                if (item.PicKind.Equals("P1"))
                {
                    pic1 = item.NewPic;
                }

            }

            //[SQL] - 清除參數設定
            cmd.Parameters.Clear();

            //[SQL] - 取得新編號
            SBSql.AppendLine(" DECLARE @NewID AS INT ");
            SBSql.AppendLine(" SET @NewID = (");
            SBSql.AppendLine("  SELECT ISNULL(MAX(Data_ID) ,0) + 1 FROM Movies ");
            SBSql.AppendLine(" );");
            SBSql.AppendLine(" SELECT @NewID AS NewID");

            cmd.CommandText = SBSql.ToString();
            using (DataTable DT = dbConn.LookupDT(cmd, out ErrMsg))
            {
                NewID = Convert.ToInt32(DT.Rows[0]["NewID"]);
            }

            //--- 開始新增資料 ---
            //[SQL] - 清除參數設定
            cmd.Parameters.Clear();
            SBSql.Clear();

            //[SQL] - 資料新增
            SBSql.AppendLine(" INSERT INTO Movies( ");
            SBSql.AppendLine("  Group_ID, Data_ID, LangCode");
            SBSql.AppendLine("  , Data_Title, Data_PubDate, Data_ListPic, Data_UrlSource, Data_Url");
            SBSql.AppendLine("  , Create_Who, Create_Time");
            SBSql.AppendLine(" ) VALUES ( ");
            SBSql.AppendLine("  @Group_ID, @NewID, @LangCode");
            SBSql.AppendLine("  , @Data_Title, @Data_PubDate, @Data_ListPic, @Data_UrlSource, @Data_Url");
            SBSql.AppendLine("  , @Create_Who, GETDATE() ");
            SBSql.AppendLine(" )");

            //[SQL] - Command
            cmd.CommandText = SBSql.ToString();
            cmd.Parameters.AddWithValue("Group_ID", Param_parentID);
            cmd.Parameters.AddWithValue("NewID", NewID);
            cmd.Parameters.AddWithValue("LangCode", Param_LangCode);
            cmd.Parameters.AddWithValue("Data_Title", this.tb_Data_Title.Text.Trim());
            cmd.Parameters.AddWithValue("Data_PubDate", this.tb_PubDate.Text);
            cmd.Parameters.AddWithValue("Data_ListPic", pic1);
            cmd.Parameters.AddWithValue("Data_UrlSource", this.rbl_Data_UrlSource.SelectedValue);
            cmd.Parameters.AddWithValue("Data_Url", this.tb_Data_Url.Text);
            cmd.Parameters.AddWithValue("Create_Who", Session["Login_GUID"].ToString());
            if (dbConn.ExecuteSql(cmd, out ErrMsg) == false)
            {
                fn_Extensions.JsAlert("資料新增失敗！", Page_CurrentUrl);
                return;
            }

            //取得圖片指定長寬
            int[] mySizeSmall = GetImgSize("small");

            //[IO] - 儲存檔案
            for (int row = 0; row < ITempList.Count; row++)
            {
                HttpPostedFile hpf = ITempList[row].Param_hpf;
                string fileKind = ITempList[row].Param_FileKind;
                int width, height;

                if (hpf.ContentLength > 0)
                {
                    width = mySizeSmall[0];
                    height = mySizeSmall[1];

                    IOManage.Save(hpf, Param_FileFolder, ITempList[row].Param_Pic, width, height);
                }
            }

            //更新本頁Url
            string thisUrl = "{0}Video/Edit/Detail/{1}/{2}/{3}/".FormatThis(
                        Application["WebUrl"]
                        , Cryptograph.MD5Encrypt(Param_parentID, fn_Param.DesKey)
                        , Param_LangCode
                        , Cryptograph.MD5Encrypt(NewID.ToString(), fn_Param.DesKey)
                    );

            //導向本頁
            Response.Redirect(thisUrl);


        }

    }

    /// <summary>
    /// 資料修改
    /// </summary>
    private void Edit_Data(List<TempParam> ITempList)
    {
        using (SqlCommand cmd = new SqlCommand())
        {
            //宣告
            StringBuilder SBSql = new StringBuilder();
            string pic1 = this.hf_OldFile_Small.Value;

            //取得圖片參數
            var queryPic = from el in ITempList
                           select new
                           {
                               NewPic = el.Param_Pic,
                               PicKind = el.Param_FileKind
                           };
            foreach (var item in queryPic)
            {
                if (item.PicKind.Equals("P1"))
                {
                    pic1 = item.NewPic;
                }

            }

            //--- 開始更新資料 ---
            //[SQL] - 清除參數設定
            cmd.Parameters.Clear();

            //[SQL] - 資料更新
            SBSql.AppendLine(" UPDATE Movies ");
            SBSql.AppendLine(" SET Data_Title = @Data_Title, Data_PubDate = @Data_PubDate ");
            SBSql.AppendLine("  , Data_ListPic = @Data_ListPic, Data_UrlSource = @Data_UrlSource, Data_Url = @Data_Url");
            SBSql.AppendLine("  , Update_Who = @Update_Who, Update_Time = GETDATE() ");
            SBSql.AppendLine(" WHERE (Data_ID = @DataID) ");

            //[SQL] - Command
            cmd.CommandText = SBSql.ToString();

            cmd.Parameters.AddWithValue("DataID", Param_thisID);
            cmd.Parameters.AddWithValue("Data_Title", this.tb_Data_Title.Text.Trim());
            cmd.Parameters.AddWithValue("Data_PubDate", this.tb_PubDate.Text);
            cmd.Parameters.AddWithValue("Data_ListPic", pic1);
            cmd.Parameters.AddWithValue("Data_UrlSource", this.rbl_Data_UrlSource.SelectedValue);
            cmd.Parameters.AddWithValue("Data_Url", this.tb_Data_Url.Text);
            cmd.Parameters.AddWithValue("Update_Who", Session["Login_GUID"].ToString());
            if (dbConn.ExecuteSql(cmd, out ErrMsg) == false)
            {
                fn_Extensions.JsAlert("資料更新失敗！", Page_CurrentUrl);
                return;
            }

            //取得圖片指定長寬
            int[] mySizeSmall = GetImgSize("small");

            //[IO] - 儲存檔案
            for (int row = 0; row < ITempList.Count; row++)
            {
                //刪除原本的檔案
                IOManage.DelFile(Param_FileFolder, ITempList[row].Param_OrgPic);

                //儲存新的檔案
                HttpPostedFile hpf = ITempList[row].Param_hpf;
                string fileKind = ITempList[row].Param_FileKind;
                int width, height;

                if (hpf.ContentLength > 0)
                {
                    width = mySizeSmall[0];
                    height = mySizeSmall[1];

                    IOManage.Save(hpf, Param_FileFolder, ITempList[row].Param_Pic, width, height);
                }
            }

            //導向本頁
            Response.Redirect(Page_CurrentUrl);
        }
    }

    /// <summary>
    /// 檔案刪除 - 小圖
    /// </summary>
    protected void lbtn_DelFile_Small_Click(object sender, EventArgs e)
    {
        try
        {
            using (SqlCommand cmd = new SqlCommand())
            {
                StringBuilder SBSql = new StringBuilder();

                //[SQL] - 資料更新
                SBSql.AppendLine(" UPDATE Movies SET Data_ListPic = NULL WHERE (Data_ID = @DataID) ");

                //[SQL] - Command
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("DataID", Param_thisID);
                if (dbConn.ExecuteSql(cmd, out ErrMsg) == false)
                {
                    fn_Extensions.JsAlert("檔案刪除失敗！", Page_CurrentUrl);
                    return;
                }
                else
                {
                    //刪除檔案
                    IOManage.DelFile(Param_FileFolder, this.hf_OldFile_Small.Value);

                    //導向列表
                    Response.Redirect(Page_CurrentUrl);
                }

            }

        }
        catch (Exception)
        {
            fn_Extensions.JsAlert("系統發生錯誤 - 刪除檔案", "");
            return;
        }

    }


    #endregion -- 資料編輯 End --


    #region -- 參數設定 --
    /// <summary>
    /// 取得傳遞參數 - 單頭資料編號
    /// </summary>
    private string _Param_parentID;
    public string Param_parentID
    {
        get
        {
            String DataID = Page.RouteData.Values["ParentID"].ToString();

            return Cryptograph.MD5Decrypt(DataID, fn_Param.DesKey);
        }
        set
        {
            this._Param_parentID = value;
        }
    }

    /// <summary>
    /// 取得傳遞參數 - 資料編號
    /// </summary>
    private string _Param_thisID;
    public string Param_thisID
    {
        get
        {
            String DataID = Page.RouteData.Values["DataID"].ToString();

            return DataID.Equals("New") ? "" : Cryptograph.MD5Decrypt(DataID, fn_Param.DesKey);
        }
        set
        {
            this._Param_thisID = value;
        }
    }

    /// <summary>
    /// 取得傳遞參數 - 語系
    /// </summary>
    private string _Param_LangCode;
    public string Param_LangCode
    {
        get
        {
            String LangCode = Page.RouteData.Values["LangCode"].ToString();

            return LangCode;
        }
        set
        {
            this._Param_LangCode = value;
        }
    }

    /// <summary>
    /// 設定參數 - 本頁Url
    /// </summary>
    private string _Page_CurrentUrl;
    public string Page_CurrentUrl
    {
        get
        {
            return "{0}Video/Edit/Detail/{1}/{2}/{3}/".FormatThis(
                Application["WebUrl"]
                , Cryptograph.MD5Encrypt(Param_parentID, fn_Param.DesKey)
                , Param_LangCode
                , string.IsNullOrEmpty(Param_thisID) ? "New" : HttpUtility.UrlEncode(Cryptograph.MD5Encrypt(Param_thisID, fn_Param.DesKey))
            );
        }
        set
        {
            this._Page_CurrentUrl = value;
        }
    }

    /// <summary>
    /// 設定參數 - 上一頁Url
    /// </summary>
    private string _Page_LastUrl;
    public string Page_LastUrl
    {
        get
        {
            return "{0}Video/Edit/{1}/".FormatThis(
                Application["WebUrl"]
                , Cryptograph.MD5Encrypt(Param_parentID, fn_Param.DesKey));
        }
        set
        {
            this._Page_LastUrl = value;
        }
    }

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
                : @"{0}Video\{1}\".FormatThis(fn_Param.File_DiskUrl, Param_parentID);
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
                : @"{0}Video/{1}/".FormatThis(fn_Param.File_WebUrl, Param_parentID);
        }
        set
        {
            this._Param_WebFolder = value;
        }
    }

    #endregion

    #region -- 上傳參數 --

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
    /// 圖片Size
    /// </summary>
    /// <param name="sizeType"></param>
    /// <returns></returns>
    private int[] GetImgSize(string sizeType)
    {
        switch (sizeType.ToLower())
        {
            case "small":
                return new int[] { 530, 530 };

            default:
                return new int[] { 1060, 1000 };
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
        /// <param name="Param_FileKind">檔案類別</param>
        public TempParam(string Param_Pic, string Param_OrgPic, HttpPostedFile Param_hpf, string Param_FileKind)
        {
            this._Param_Pic = Param_Pic;
            this._Param_OrgPic = Param_OrgPic;
            this._Param_hpf = Param_hpf;
            this._Param_FileKind = Param_FileKind;
        }

    }
    #endregion
}