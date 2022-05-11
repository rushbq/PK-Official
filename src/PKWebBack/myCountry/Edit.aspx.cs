using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ExtensionIO;
using ExtensionMethods;
using ExtensionUI;

public partial class Country_Edit : SecurityCheck
{
    public string ErrMsg;
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!IsPostBack)
            {
                //[權限判斷]
                if (fn_CheckAuth.CheckAuth("210", out ErrMsg) == false)
                {
                    Response.Redirect("{0}401.aspx".FormatThis(Application["WebUrl"]));
                    return;
                }

                //** 設定程式編號(重要) **
                if (false == setProgIDs.setID(this.Master, "200", "210"))
                {
                    throw new Exception("目前的Masterpage 沒有實作 IProgID,無法傳值");
                }

                //[取得/檢查參數] - 洲別
                if (fn_CustomUI.Get_Region(this.ddl_AreaCode, "", true, out ErrMsg) == false)
                {
                    this.ddl_AreaCode.Items.Insert(0, new ListItem("選單產生失敗", ""));
                }


                //[取得/檢查參數] - 狀態(預設Y)
                if (fn_CustomUI.Get_PubDisp(this.rbl_Display, "Y", out ErrMsg) == false)
                {
                    this.rbl_Display.Items.Insert(0, new ListItem("選單產生失敗", ""));
                }

                //帶出資料
                LookupData();

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
                SBSql.AppendLine(" SELECT Base.AreaCode, Sub.Country_Code, Sub.Country_Name, Base.Country_Flag ");
                SBSql.AppendLine("     , Base.Display, Lang.LangCode, Lang.LangName ");
                SBSql.AppendLine(" FROM PKSYS.dbo.Param_Language Lang ");
                SBSql.AppendLine("     LEFT JOIN Geocode_CountryName Sub ON Lang.LangCode = Sub.LangCode AND UPPER(Sub.Country_Code) = UPPER(@DataID) ");
                SBSql.AppendLine("     LEFT JOIN Geocode_CountryCode Base ON Base.Country_Code = Sub.Country_Code ");
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.AddWithValue("DataID", Param_thisID);
                using (DataTable DT = dbConn.LookupDT(cmd, out ErrMsg))
                {
                    if (DT.Rows.Count == 0)
                    {
                        fn_Extensions.JsAlert("查無資料！", Page_SearchUrl);
                        return;
                    }
                    else
                    {
                        //取得資料
                        string GetCode = DT.Rows[0]["Country_Code"].ToString();

                        //[填入資料]
                        this.ddl_AreaCode.SelectedValue = DT.Rows[0]["AreaCode"].ToString();
                        this.tb_Country_Code.Text = GetCode;
                        this.rbl_Display.SelectedValue = DT.Rows[0]["Display"].ToString();

                        //檔案Url - 國旗
                        string myFile = DT.Rows[0]["Country_Flag"].ToString();
                        this.hf_OldFile.Value = myFile;
                        if (!string.IsNullOrEmpty(myFile))
                        {
                            this.lt_dwUrl.Text = "<a href=\"{0}\" class=\"btn btn-info zoomPic\" data-gall=\"singlePic\" title=\"\"><i class=\"fa fa-eye\"></i></a>".FormatThis(
                                Param_WebFolder + myFile);

                            this.ph_files.Visible = true;
                        }
                     
                        //Flag設定 & 欄位顯示/隱藏
                        if (!string.IsNullOrEmpty(GetCode))
                        {
                            this.tb_Country_Code.ReadOnly = true;
                            this.hf_flag.Value = "Edit";
                        }
                        else
                        {
                            this.tb_Country_Code.ReadOnly = false;
                            this.hf_flag.Value = "Add";
                        }

                        //帶出語系資料
                        this.lvDataList.DataSource = DT.DefaultView;
                        this.lvDataList.DataBind();
                    }
                }
            }
        }
        catch (Exception)
        {
            throw new Exception("系統發生錯誤 - 資料查詢");
        }
    }


    #endregion -- 資料顯示 End --

    #region -- 資料編輯 Start --
    /// <summary>
    /// 存檔
    /// </summary>
    protected void btn_Save_Click(object sender, EventArgs e)
    {
        try
        {
            #region "..欄位檢查.."
            StringBuilder SBAlert = new StringBuilder();

            //[參數檢查] - 必填項目
            if (fn_Extensions.String_資料長度Byte(this.tb_Country_Code.Text, "1", "2", out ErrMsg) == false)
            {
                SBAlert.Append("「國家區碼」請輸入1 ~ 2個字\\n");
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

            //國旗圖
            HttpPostedFile hpFile = this.fu_Files.PostedFile;
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
                        ITempList.Add(new TempParam(IOManage.FileNewName, this.hf_OldFile.Value, hpFile, "P1"));
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
            string dataID = this.tb_Country_Code.Text.ToUpper();
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

            //[SQL] - 判斷是否重複新增
            SBSql.AppendLine(" IF (SELECT COUNT(*) FROM Geocode_CountryCode WHERE (Country_Code = @Country_Code)) = 0 ");

            //[SQL] - 資料新增
            SBSql.AppendLine(" BEGIN ");
            SBSql.AppendLine(" INSERT INTO Geocode_CountryCode( ");
            SBSql.AppendLine("  AreaCode, Country_Code, Country_Flag, Display");
            SBSql.AppendLine(" ) VALUES ( ");
            SBSql.AppendLine("  @AreaCode, @Country_Code, @Country_Flag, @Display");
            SBSql.AppendLine(" );");
            SBSql.AppendLine(" END ");

            //[SQL] - 各語系新增
            SBSql.AppendLine(" DELETE FROM Geocode_CountryName WHERE (Country_Code = @Country_Code); ");

            for (int row = 0; row < this.lvDataList.Items.Count; row++)
            {
                //[取得參數] 
                string lvParam_ID = ((HiddenField)this.lvDataList.Items[row].FindControl("hf_DataID")).Value;
                string lvParam_Name = ((TextBox)this.lvDataList.Items[row].FindControl("tb_Country_Name")).Text;

                SBSql.AppendLine(" INSERT INTO Geocode_CountryName( ");
                SBSql.AppendLine("  LangCode, Country_Code, Country_Name");
                SBSql.AppendLine(" ) VALUES ( ");
                SBSql.AppendLine("  @LangCode_{0}, @Country_Code, @Country_Name_{0}".FormatThis(row));
                SBSql.AppendLine(" );");

                cmd.Parameters.AddWithValue("LangCode_" + row, lvParam_ID);
                cmd.Parameters.AddWithValue("Country_Name_" + row, lvParam_Name);

            }

            //[SQL] - Command
            cmd.CommandText = SBSql.ToString();
            cmd.Parameters.AddWithValue("AreaCode", this.ddl_AreaCode.SelectedValue);
            cmd.Parameters.AddWithValue("Country_Code", dataID);
            cmd.Parameters.AddWithValue("Country_Flag", pic1);
            cmd.Parameters.AddWithValue("Display", this.rbl_Display.SelectedValue);
            if (dbConn.ExecuteSql(cmd, out ErrMsg) == false)
            {
                fn_Extensions.JsAlert("資料新增失敗！", Page_CurrentUrl);
                return;
            }

            //[IO] - 儲存檔案
            for (int row = 0; row < ITempList.Count; row++)
            {
                HttpPostedFile hpf = ITempList[row].Param_hpf;
                if (hpf.ContentLength > 0)
                {
                    IOManage.Save(hpf, Param_FileFolder, ITempList[row].Param_Pic, Param_Width, Param_Height);
                }
            }

            //更新本頁Url
            string thisUrl = "{0}Country/Edit/{1}/".FormatThis(
                        Application["WebUrl"]
                        , Cryptograph.MD5Encrypt(dataID, Application["DesKey"].ToString())
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
            string pic1 = this.hf_OldFile.Value;

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

            //[SQL] - 資料更新
            SBSql.AppendLine(" UPDATE Geocode_CountryCode ");
            SBSql.AppendLine(" SET AreaCode = @AreaCode ");
            SBSql.AppendLine("  , Country_Flag = @Country_Flag, Display = @Display");
            SBSql.AppendLine(" WHERE (Country_Code = @Country_Code) ");

            //[SQL] - 各語系新增
            SBSql.AppendLine(" DELETE FROM Geocode_CountryName WHERE (Country_Code = @Country_Code); ");

            for (int row = 0; row < this.lvDataList.Items.Count; row++)
            {
                //[取得參數] 
                string lvParam_ID = ((HiddenField)this.lvDataList.Items[row].FindControl("hf_DataID")).Value;
                string lvParam_Name = ((TextBox)this.lvDataList.Items[row].FindControl("tb_Country_Name")).Text;

                SBSql.AppendLine(" INSERT INTO Geocode_CountryName( ");
                SBSql.AppendLine("  LangCode, Country_Code, Country_Name");
                SBSql.AppendLine(" ) VALUES ( ");
                SBSql.AppendLine("  @LangCode_{0}, @Country_Code, @Country_Name_{0}".FormatThis(row));
                SBSql.AppendLine(" );");

                cmd.Parameters.AddWithValue("LangCode_" + row, lvParam_ID);
                cmd.Parameters.AddWithValue("Country_Name_" + row, lvParam_Name);
            }

            //[SQL] - Command
            cmd.CommandText = SBSql.ToString();
            cmd.Parameters.AddWithValue("AreaCode", this.ddl_AreaCode.SelectedValue);
            cmd.Parameters.AddWithValue("Country_Flag", pic1);
            cmd.Parameters.AddWithValue("Display", this.rbl_Display.SelectedValue);
            cmd.Parameters.AddWithValue("Country_Code", Param_thisID);
            if (dbConn.ExecuteSql(cmd, out ErrMsg) == false)
            {
                fn_Extensions.JsAlert("資料更新失敗！", Page_CurrentUrl);
                return;
            }

            //[IO] - 儲存檔案
            for (int row = 0; row < ITempList.Count; row++)
            {
                //刪除原本的檔案
                IOManage.DelFile(Param_FileFolder, ITempList[row].Param_OrgPic);

                //儲存新的檔案
                HttpPostedFile hpf = ITempList[row].Param_hpf;
                if (hpf.ContentLength > 0)
                {
                    IOManage.Save(hpf, Param_FileFolder, ITempList[row].Param_Pic, Param_Width, Param_Height);
                }
            }

            //導向本頁
            Response.Redirect(Page_CurrentUrl);
        }
    }

    /// <summary>
    /// 檔案刪除 
    /// </summary>
    protected void lbtn_DelFile_Click(object sender, EventArgs e)
    {
        try
        {
            using (SqlCommand cmd = new SqlCommand())
            {
                StringBuilder SBSql = new StringBuilder();

                //[SQL] - 資料更新
                SBSql.AppendLine(" UPDATE Geocode_CountryCode SET Country_Flag = NULL WHERE (Country_Code = @DataID) ");

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
                    IOManage.DelFile(Param_FileFolder, this.hf_OldFile.Value);

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
    /// 取得傳遞參數 - 資料編號
    /// </summary>
    private string _Param_thisID;
    public string Param_thisID
    {
        get
        {
            String DataID = Page.RouteData.Values["DataID"].ToString();

            return DataID.Equals("New") ? "" : Cryptograph.MD5Decrypt(DataID, Application["DesKey"].ToString());
        }
        set
        {
            this._Param_thisID = value;
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
            return "{0}Country/Edit/{1}/".FormatThis(
                Application["WebUrl"]
                , string.IsNullOrEmpty(Param_thisID) ? "New" : HttpUtility.UrlEncode(Cryptograph.MD5Encrypt(Param_thisID, Application["DesKey"].ToString()))
            );
        }
        set
        {
            this._Page_CurrentUrl = value;
        }
    }

    /// <summary>
    /// 設定參數 - 列表頁Url
    /// </summary>
    private string _Page_SearchUrl;
    public string Page_SearchUrl
    {
        get
        {
            String Url;
            if (Session["BackListUrl"] == null)
            {
                Url = "{0}Country/Search/".FormatThis(Application["WebUrl"]);
            }
            else
            {
                Url = Session["BackListUrl"].ToString();
            }

            return Url;
        }
        set
        {
            this._Page_SearchUrl = value;
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
                : @"{0}Support\Flag\".FormatThis(Application["File_DiskUrl"]);
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
                : @"{0}Support/Flag/".FormatThis(Application["File_WebUrl"]);
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