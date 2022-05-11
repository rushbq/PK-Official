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


public partial class Edit_DetailBox : SecurityCheck
{
    public string ErrMsg;
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!IsPostBack)
            {
                //[權限判斷]
                if (fn_CheckAuth.CheckAuth("110", out ErrMsg) == false)
                {
                    fn_Extensions.JsAlert("權限不足", "script:parent.$.fancybox.close();");
                    return;
                }

                //[必要參數判斷]
                if (string.IsNullOrEmpty(Param_parentID) || string.IsNullOrEmpty(Param_thisID))
                {
                    fn_Extensions.JsAlert("錯誤的操作，請重新開啟", "script:parent.$.fancybox.close();");
                    return;
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

                //[SQL] - 資料查詢
                SBSql.AppendLine(" SELECT Base.Block_Desc, Base.Block_Pic, Base.Sort");
                SBSql.AppendLine(" FROM News_Block Base ");
                SBSql.AppendLine(" WHERE (Base.News_ID = @ParentID) AND (Base.Block_ID = @DataID)");

                //[SQL] - Command
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("ParentID", Param_parentID);
                cmd.Parameters.AddWithValue("DataID", Param_thisID);
                using (DataTable DT = dbConn.LookupDT(cmd, out ErrMsg))
                {
                    if (DT.Rows.Count == 0)
                    {
                        fn_Extensions.JsAlert("查無資料！", "");
                        return;
                    }
                    else
                    {
                        //[填入資料]
                        this.tb_Block_Desc.Text = HttpUtility.HtmlDecode(DT.Rows[0]["Block_Desc"].ToString());
                        this.tb_Sort.Text = DT.Rows[0]["Sort"].ToString();

                        //檔案Url
                        string myFile = DT.Rows[0]["Block_Pic"].ToString();
                        this.hf_OldFile.Value = myFile;
                        if (!string.IsNullOrEmpty(myFile))
                        {
                            this.ph_files.Visible = true;
                        }

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
    /// 存檔
    /// </summary>
    protected void btn_BlockSave_Click(object sender, EventArgs e)
    {
        try
        {
            #region "..欄位檢查.."
            StringBuilder SBAlert = new StringBuilder();

            //[參數檢查] - 必填項目
            if (string.IsNullOrEmpty(this.tb_Block_Desc.Text))
            {
                SBAlert.Append("請輸入「區塊內容」\\n");
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
            HttpFileCollection hfc = Request.Files;
            for (int i = 0; i <= hfc.Count - 1; i++)
            {
                HttpPostedFile hpf = hfc[i];
                if (hpf.ContentLength > 0)
                {
                    //[IO] - 取得檔案名稱
                    IOManage.GetFileName(hpf);

                    //判斷副檔名，未符合規格的檔案不上傳
                    if (fn_Extensions.CheckStrWord(IOManage.FileExtend, FileExtLimit, "|", 1))
                    {
                        ITempList.Add(new TempParam(IOManage.FileNewName, IOManage.FileFullName, hpf));
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
            using (SqlCommand cmd = new SqlCommand())
            {
                //宣告
                StringBuilder SBSql = new StringBuilder();

                //[SQL] - 清除參數設定
                cmd.Parameters.Clear();

                //[SQL] - 資料更新
                SBSql.AppendLine(" UPDATE News_Block ");
                SBSql.AppendLine(" SET Block_Desc = @Block_Desc, Block_Pic = @Block_Pic, Sort = @Sort");
                SBSql.AppendLine(" WHERE (News_ID = @ParentID) AND (Block_ID = @DataID)");

                //[SQL] - Command
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.AddWithValue("ParentID", Param_parentID);
                cmd.Parameters.AddWithValue("DataID", Param_thisID);
                cmd.Parameters.AddWithValue("Block_Desc", HttpUtility.HtmlEncode(this.tb_Block_Desc.Text));
                cmd.Parameters.AddWithValue("Block_Pic", (ITempList.Count == 0) ? this.hf_OldFile.Value : ITempList[0].Param_Pic);
                cmd.Parameters.AddWithValue("Sort", this.tb_Sort.Text);
                if (dbConn.ExecuteSql(cmd, out ErrMsg) == false)
                {
                    fn_Extensions.JsAlert("資料更新失敗！", Page_CurrentUrl);
                    return;
                }

                //[IO] - 儲存檔案
                if (ITempList.Count > 0)
                {
                    HttpPostedFile hpf = ITempList[0].Param_hpf;
                    if (hpf.ContentLength > 0)
                    {
                        IOManage.Save(hpf, Param_FileFolder, ITempList[0].Param_Pic, Param_Width, Param_Height);
                    }
                }

                //導向本頁
                Response.Redirect(Page_CurrentUrl);

            }
            #endregion

        }
        catch (Exception)
        {
            fn_Extensions.JsAlert("系統發生錯誤 - 區塊設定存檔", "");
            return;
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
                SBSql.AppendLine(" UPDATE News_Block SET Block_Pic = NULL WHERE (News_ID = @ParentID) AND (Block_ID = @DataID)");

                //[SQL] - Command
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("ParentID", Param_parentID);
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
    /// 取得傳遞參數 - 群組資料編號(GroupID)
    /// </summary>
    private string _Param_GID;
    public string Param_GID
    {
        get
        {
            String DataID = Page.RouteData.Values["GroupID"].ToString();

            return DataID;
        }
        set
        {
            this._Param_GID = value;
        }
    }

    /// <summary>
    /// 取得傳遞參數 - 單頭資料編號(NewsID)
    /// </summary>
    private string _Param_parentID;
    public string Param_parentID
    {
        get
        {
            String DataID = Page.RouteData.Values["ParentID"].ToString();

            return DataID;
        }
        set
        {
            this._Param_parentID = value;
        }
    }

    /// <summary>
    /// 取得傳遞參數 - 資料編號(BlockID)
    /// </summary>
    private string _Param_thisID;
    public string Param_thisID
    {
        get
        {
            String DataID = Page.RouteData.Values["DataID"].ToString();

            return Cryptograph.MD5Decrypt(DataID, fn_Param.DesKey);
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
            return "{0}News/Edit/DetailBox/{1}/{2}/{3}".FormatThis(
                 Application["WebUrl"]
                 , Param_GID
                 , Param_parentID
                 , Cryptograph.MD5Encrypt(Param_thisID, fn_Param.DesKey)
             );
        }
        set
        {
            this._Page_CurrentUrl = value;
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
                : @"{0}News\{1}\".FormatThis(fn_Param.File_DiskUrl, Param_GID);
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
                : @"{0}News/{1}/".FormatThis(fn_Param.File_WebUrl, Param_GID);
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