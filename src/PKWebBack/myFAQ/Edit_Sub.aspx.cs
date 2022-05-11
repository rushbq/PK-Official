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


public partial class FAQ_Edit_Sub : SecurityCheck
{
    public string ErrMsg;
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!IsPostBack)
            {
                //[權限判斷]
                if (fn_CheckAuth.CheckAuth("230", out ErrMsg) == false)
                {
                    Response.Redirect("{0}401.aspx".FormatThis(Application["WebUrl"]));
                    return;
                }

                //** 設定程式編號(重要) **
                if (false == setProgIDs.setID(this.Master, "200", "230"))
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

                    //顯示區塊
                    this.ph_myBlock.Visible = true;
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
                SBSql.AppendLine(" FROM  FAQ_Group GP ");
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
                SBSql.AppendLine(" SELECT Base.FAQ_ID, Base.FAQ_Title");
                SBSql.AppendLine(" FROM FAQ Base ");
                SBSql.AppendLine(" WHERE (Base.FAQ_ID = @DataID)");
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
                        this.tb_FAQ_Title.Text = DT.Rows[0]["FAQ_Title"].ToString();


                        //Flag設定 & 欄位顯示/隱藏
                        this.hf_flag.Value = "Edit";

                        //顯示區塊
                        LookupData_Block();
                    }
                }
            }
        }
        catch (Exception)
        {
            throw new Exception("系統發生錯誤 - 資料顯示");
        }
    }

    /// <summary>
    /// 區塊資料顯示
    /// </summary>
    private void LookupData_Block()
    {
        try
        {
            using (SqlCommand cmd = new SqlCommand())
            {
                //[SQL] - 清除參數設定
                cmd.Parameters.Clear();

                //[SQL] - 資料查詢
                StringBuilder SBSql = new StringBuilder();
                SBSql.AppendLine(" SELECT Block_ID, Block_Title, Block_Desc, Sort ");
                SBSql.AppendLine(" FROM FAQ_Block ");
                SBSql.AppendLine(" WHERE (FAQ_ID = @DataID) ");
                SBSql.AppendLine(" ORDER BY Sort, Block_ID ASC ");
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.AddWithValue("DataID", Param_thisID);
                using (DataTable DT = dbConn.LookupDT(cmd, out ErrMsg))
                {
                    if (DT.Rows.Count == 0)
                    {
                        this.btn_SaveBlock1.Visible = false;
                        this.btn_SaveBlock2.Visible = false;
                    }

                    //DataBind            
                    this.lvDataList.DataSource = DT.DefaultView;
                    this.lvDataList.DataBind();
                }
            }
        }
        catch (Exception)
        {
            throw new Exception("系統發生錯誤 - 區塊資料！");
        }
    }


    protected void lvDataList_ItemCommand(object sender, ListViewCommandEventArgs e)
    {
        try
        {
            if (e.Item.ItemType == ListViewItemType.DataItem)
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    StringBuilder SBSql = new StringBuilder();
                    cmd.Parameters.Clear();

                    //[取得參數] - 編號
                    string GetDataID = ((HiddenField)e.Item.FindControl("hf_DataID")).Value;

                    //[SQL] - 刪除資料
                    SBSql.AppendLine(" DELETE FROM FAQ_Block WHERE (FAQ_ID = @DataID) AND (Block_ID = @Param_ID) ");
                    cmd.CommandText = SBSql.ToString();
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("DataID", Param_thisID);
                    cmd.Parameters.AddWithValue("Param_ID", GetDataID);
                    if (dbConn.ExecuteSql(cmd, out ErrMsg) == false)
                    {
                        fn_Extensions.JsAlert("刪除失敗！", "");
                    }
                    else
                    {
                        //頁面跳至本頁
                        Response.Redirect(Page_CurrentUrl);
                    }
                }
            }
        }
        catch (Exception)
        {

            throw;
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
            if (fn_Extensions.String_資料長度Byte(this.tb_FAQ_Title.Text, "1", "200", out ErrMsg) == false)
            {
                SBAlert.Append("「主標題」請輸入1 ~ 100個字\\n");
            }

            //[JS] - 判斷是否有警示訊息
            if (string.IsNullOrEmpty(SBAlert.ToString()) == false)
            {
                fn_Extensions.JsAlert(SBAlert.ToString(), "");
                return;
            }
            #endregion

            #region "..資料儲存.."
            //判斷是新增 or 修改
            switch (this.hf_flag.Value.ToUpper())
            {
                case "ADD":
                    Add_Data();
                    break;

                case "EDIT":
                    Edit_Data();
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
    private void Add_Data()
    {
        using (SqlCommand cmd = new SqlCommand())
        {
            //宣告
            StringBuilder SBSql = new StringBuilder();
            int NewID;

            //[SQL] - 清除參數設定
            cmd.Parameters.Clear();

            //[SQL] - 取得新編號
            SBSql.AppendLine(" DECLARE @NewID AS INT ");
            SBSql.AppendLine(" SET @NewID = (");
            SBSql.AppendLine("  SELECT ISNULL(MAX(FAQ_ID) ,0) + 1 FROM FAQ ");
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
            SBSql.AppendLine(" INSERT INTO FAQ( ");
            SBSql.AppendLine("  Group_ID, FAQ_ID, LangCode");
            SBSql.AppendLine("  , FAQ_Title");
            SBSql.AppendLine("  , Create_Who, Create_Time");
            SBSql.AppendLine(" ) VALUES ( ");
            SBSql.AppendLine("  @Group_ID, @NewID, @LangCode");
            SBSql.AppendLine("  , @FAQ_Title");
            SBSql.AppendLine("  , @Create_Who, GETDATE() ");
            SBSql.AppendLine(" )");



            //[SQL] - Command
            cmd.CommandText = SBSql.ToString();
            cmd.Parameters.AddWithValue("Group_ID", Param_parentID);
            cmd.Parameters.AddWithValue("NewID", NewID);
            cmd.Parameters.AddWithValue("LangCode", Param_LangCode);
            cmd.Parameters.AddWithValue("FAQ_Title", this.tb_FAQ_Title.Text.Trim());
            cmd.Parameters.AddWithValue("Create_Who", Session["Login_GUID"].ToString());
            if (dbConn.ExecuteSql(cmd, out ErrMsg) == false)
            {
                fn_Extensions.JsAlert("資料新增失敗！", Page_CurrentUrl);
                return;
            }

            //更新本頁Url
            string thisUrl = "{0}FAQ/Edit/Detail/{1}/{2}/{3}/".FormatThis(
                        Application["WebUrl"]
                        , Cryptograph.MD5Encrypt(Param_parentID, Application["DesKey"].ToString())
                        , Param_LangCode
                        , Cryptograph.MD5Encrypt(NewID.ToString(), Application["DesKey"].ToString())
                    );

            //導向本頁
            Response.Redirect(thisUrl);
        }

    }

    /// <summary>
    /// 資料修改
    /// </summary>
    private void Edit_Data()
    {
        using (SqlCommand cmd = new SqlCommand())
        {
            //宣告
            StringBuilder SBSql = new StringBuilder();

            //--- 開始更新資料 ---
            //[SQL] - 清除參數設定
            cmd.Parameters.Clear();

            //[SQL] - 資料更新
            SBSql.AppendLine(" UPDATE FAQ ");
            SBSql.AppendLine(" SET FAQ_Title = @FAQ_Title ");
            SBSql.AppendLine("  , Update_Who = @Update_Who, Update_Time = GETDATE() ");
            SBSql.AppendLine(" WHERE (FAQ_ID = @DataID) ");

            //[SQL] - Command
            cmd.CommandText = SBSql.ToString();
            cmd.Parameters.AddWithValue("DataID", Param_thisID);
            cmd.Parameters.AddWithValue("FAQ_Title", this.tb_FAQ_Title.Text.Trim());
            cmd.Parameters.AddWithValue("Update_Who", Session["Login_GUID"].ToString());
            if (dbConn.ExecuteSql(cmd, out ErrMsg) == false)
            {
                fn_Extensions.JsAlert("資料更新失敗！", Page_CurrentUrl);
                return;
            }

            //導向本頁
            Response.Redirect(Page_CurrentUrl);
        }
    }

    #endregion -- 資料編輯 End --

    #region -- 資料編輯(區塊) Start --

    /// <summary>
    /// 區塊設定存檔
    /// </summary>
    protected void btn_BlockSave_Click(object sender, EventArgs e)
    {
        try
        {
            #region "..資料儲存.."
            using (SqlCommand cmd = new SqlCommand())
            {
                //宣告
                StringBuilder SBSql = new StringBuilder();

                //--- 開始新增資料 ---
                //[SQL] - 清除參數設定
                cmd.Parameters.Clear();
                SBSql.Clear();

                //[SQL] - 資料新增
                SBSql.AppendLine(" DECLARE @NewID AS INT ");
                SBSql.AppendLine(" SET @NewID = (");
                SBSql.AppendLine("  SELECT ISNULL(MAX(Block_ID) ,0) + 1 FROM FAQ_Block WHERE (FAQ_ID = @FAQ_ID)");
                SBSql.AppendLine(" );");

                SBSql.AppendLine(" INSERT INTO FAQ_Block( ");
                SBSql.AppendLine("  FAQ_ID, Block_ID");
                SBSql.AppendLine("  , Block_Title, Block_Desc,Sort");
                SBSql.AppendLine(" ) VALUES ( ");
                SBSql.AppendLine("  @FAQ_ID, @NewID");
                SBSql.AppendLine("  , @Block_Title, @Block_Desc, @Sort");
                SBSql.AppendLine(" )");

                //[SQL] - Command
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.AddWithValue("FAQ_ID", Param_thisID);
                cmd.Parameters.AddWithValue("Block_Title", this.tb_Block_Title.Text);
                cmd.Parameters.AddWithValue("Block_Desc", HttpUtility.HtmlEncode(this.tb_Block_Desc.Text));
                cmd.Parameters.AddWithValue("Sort", this.tb_Sort.Text);
                if (dbConn.ExecuteSql(cmd, out ErrMsg) == false)
                {
                    fn_Extensions.JsAlert("區塊新增失敗！", Page_CurrentUrl);
                    return;
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
    /// 儲存版面排序
    /// </summary>
    protected void btn_SaveSort_Click(object sender, EventArgs e)
    {
        try
        {
            if (this.lvDataList.Items.Count == 0)
            {
                fn_Extensions.JsAlert("請先新增區塊資料", "");
                return;
            }

            using (SqlCommand cmd = new SqlCommand())
            {
                //宣告
                StringBuilder SBSql = new StringBuilder();

                //[SQL] - 清除參數設定
                cmd.Parameters.Clear();

                for (int row = 0; row < lvDataList.Items.Count; row++)
                {
                    //[取得參數] - 編號
                    string lvParam_ID = ((HiddenField)this.lvDataList.Items[row].FindControl("hf_DataID")).Value;
                    //[取得參數] - 排序
                    string lvParam_Sort = ((TextBox)this.lvDataList.Items[row].FindControl("tb_Sort")).Text;

                    SBSql.AppendLine(" UPDATE FAQ_Block SET Sort = @lvParam_Sort_{0}".FormatThis(row));
                    SBSql.AppendLine(" WHERE (FAQ_ID = @FAQ_ID) AND (Block_ID = @lvParam_ID_{0}) ".FormatThis(row));

                    cmd.Parameters.AddWithValue("lvParam_ID_" + row, lvParam_ID);
                    cmd.Parameters.AddWithValue("lvParam_Sort_" + row, lvParam_Sort);
                }

                //[SQL] - Command
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.AddWithValue("FAQ_ID", Param_thisID);
                if (dbConn.ExecuteSql(cmd, out ErrMsg) == false)
                {
                    fn_Extensions.JsAlert("儲存版面失敗！", "");
                }
                else
                {
                    //導向本頁
                    Response.Redirect(Page_CurrentUrl);
                }
            }
        }
        catch (Exception)
        {
            fn_Extensions.JsAlert("系統發生錯誤 - 儲存版面排序", "");
            return;
        }
    }

    #endregion -- 資料編輯(區塊) End --

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

            return Cryptograph.MD5Decrypt(DataID, Application["DesKey"].ToString());
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

            return DataID.Equals("New") ? "" : Cryptograph.MD5Decrypt(DataID, Application["DesKey"].ToString());
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
            return "{0}FAQ/Edit/Detail/{1}/{2}/{3}/".FormatThis(
                Application["WebUrl"]
                , Cryptograph.MD5Encrypt(Param_parentID, Application["DesKey"].ToString())
                , Param_LangCode
                , string.IsNullOrEmpty(Param_thisID) ? "New" : HttpUtility.UrlEncode(Cryptograph.MD5Encrypt(Param_thisID, Application["DesKey"].ToString()))
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
            return "{0}FAQ/Edit/{1}/".FormatThis(
                Application["WebUrl"]
                , Cryptograph.MD5Encrypt(Param_parentID, Application["DesKey"].ToString()));
        }
        set
        {
            this._Page_LastUrl = value;
        }
    }


    #endregion

}