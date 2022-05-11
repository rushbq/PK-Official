using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ExtensionMethods;
using ExtensionUI;

public partial class Prod_Edit : SecurityCheck
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

                //[取得/檢查參數] - 上架狀態(預設S)
                if (fn_CustomUI.Get_NewsDisp(this.rbl_Display, "S", out ErrMsg) == false)
                {
                    this.rbl_Display.Items.Insert(0, new ListItem("選單產生失敗", ""));
                }
                //[取得/檢查參數] - 商品類別
                if (fn_CustomUI.Get_TypeClass(this.rbl_Class, "", out ErrMsg) == false)
                {
                    this.rbl_Class.Items.Insert(0, new ListItem("選單產生失敗", ""));
                }

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
    /// 資料顯示
    /// </summary>
    private void LookupData()
    {
        try
        {
            //[取得/檢查參數] - 系統編號
            if (string.IsNullOrEmpty(Param_thisID))
            {
                fn_Extensions.JsAlert("參數傳遞錯誤！", Page_SearchUrl);
                return;
            }

            //[取得資料] - 取得資料
            using (SqlCommand cmd = new SqlCommand())
            {
                //宣告
                StringBuilder SBSql = new StringBuilder();

                //清除參數
                cmd.Parameters.Clear();

                //[SQL] - 資料查詢
                SBSql.AppendLine(" SELECT GP.*, Sub.Model_Name_zh_TW AS ModelName ");
                SBSql.AppendLine("  , (SELECT Display_Name FROM {0}.dbo.User_Profile WHERE ([Guid] = GP.Create_Who)) AS Create_Name "
                    .FormatThis(fn_SysDB.Param_DB));
                SBSql.AppendLine("  , (SELECT Display_Name FROM {0}.dbo.User_Profile WHERE ([Guid] = GP.Update_Who)) AS Update_Name "
                    .FormatThis(fn_SysDB.Param_DB));
                SBSql.AppendLine("  , Sub.Catelog_Vol, Sub.Page");
                SBSql.AppendLine("  FROM Prod GP ");
                SBSql.AppendLine("      LEFT JOIN [ProductCenter].dbo.Prod_Item Sub ON GP.Model_No = Sub.Model_No");
                SBSql.AppendLine(" WHERE (GP.Prod_ID = @DataID) ");

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
                        //[填入資料]
                        string Model_No = DT.Rows[0]["Model_No"].ToString();
                        this.lb_DataID.Text = DT.Rows[0]["Prod_ID"].ToString();
                        this.tb_Model_No.Text = Model_No;
                        this.hf_ModelNo.Value = Model_No;
                        this.hf_myItemVal.Value = Model_No;
                        this.lb_ModelName.Text = DT.Rows[0]["ModelName"].ToString();
                        this.rbl_Class.SelectedValue = DT.Rows[0]["Class_ID"].ToString();
                        this.show_sDate.Text = DT.Rows[0]["StartTime"].ToString().ToDateString("yyyy/MM/dd HH:mm");
                        this.show_eDate.Text = DT.Rows[0]["EndTime"].ToString().ToDateString("yyyy/MM/dd HH:mm");
                        this.tb_StartDate.Text = DT.Rows[0]["StartTime"].ToString().ToDateString("yyyy/MM/dd HH:mm");
                        this.tb_EndDate.Text = DT.Rows[0]["EndTime"].ToString().ToDateString("yyyy/MM/dd HH:mm");
                        this.tb_ShopUrl.Text = DT.Rows[0]["ShopUrl"].ToString();
                        this.rbl_Display.SelectedValue = DT.Rows[0]["Display"].ToString();
                        this.tb_Sort.Text = DT.Rows[0]["Sort"].ToString();
                        this.lb_Vol.Text = DT.Rows[0]["Catelog_Vol"].ToString();
                        this.lb_Page.Text = DT.Rows[0]["Page"].ToString();
                        this.rbl_IsHot.SelectedValue = DT.Rows[0]["IsHot"].ToString();

                        //維護資訊
                        this.lt_CreateInfo.Text = "<p class=\"form-control-static help-block\">Created on <code>{0}</code> by <code>{1}</code></p>"
                            .FormatThis(
                                DT.Rows[0]["Create_Time"].ToString().ToDateString("yyyy-MM-dd HH:mm")
                                , DT.Rows[0]["Create_Name"].ToString()
                            );
                        if (!string.IsNullOrEmpty(DT.Rows[0]["Update_Time"].ToString()))
                        {
                            this.lt_UpdateInfo.Text = "<p class=\"form-control-static help-block\">Last updated on <code>{0}</code> by <code>{1}</code></p>"
                                .FormatThis(
                                    DT.Rows[0]["Update_Time"].ToString().ToDateString("yyyy-MM-dd HH:mm")
                                    , DT.Rows[0]["Update_Name"].ToString()
                                );
                        }

                        //Flag設定 & 欄位顯示/隱藏
                        this.hf_flag.Value = "Edit";
                        this.ph_ReNew.Visible = true;
                        this.tb_Model_No.ReadOnly = true;

                        //帶出關聯資料
                        LookupData_Detail();
                    }
                }
            }
        }
        catch (Exception)
        {
            throw new Exception("系統發生錯誤 - 資料查詢");
        }
    }


    /// <summary>
    /// 顯示關聯資料
    /// </summary>
    private void LookupData_Detail()
    {
        try
        {
            using (SqlCommand cmd = new SqlCommand())
            {
                //[SQL] - 清除參數設定
                cmd.Parameters.Clear();

                //[SQL] - 資料查詢
                StringBuilder SBSql = new StringBuilder();

                SBSql.AppendLine(" SELECT Lang.LangName, Lang.LangCode, Base.Info_ID ");
                SBSql.AppendLine("     , (CASE WHEN Base.Info_ID IS NULL THEN 'N' ELSE 'Y' END) IsSet ");
                SBSql.AppendLine(" FROM PKSYS.dbo.Param_Language Lang ");
                SBSql.AppendLine("     LEFT JOIN Prod_Info Base ON Lang.LangCode = Base.LangCode AND Base.Prod_ID = @DataID ");
                SBSql.AppendLine(" WHERE (Lang.Display = 'Y') ");
                SBSql.AppendLine(" ORDER BY Lang.Sort ");
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.AddWithValue("DataID", Param_thisID);
                using (DataTable DT = dbConn.LookupDT(cmd, out ErrMsg))
                {
                    StringBuilder html = new StringBuilder();

                    for (int row = 0; row < DT.Rows.Count; row++)
                    {
                        string urlCss, iconCss;
                        string DataUID = DT.Rows[row]["Info_ID"].ToString();

                        //判斷是否設定
                        if (DT.Rows[row]["IsSet"].ToString().Equals("Y"))
                        {
                            urlCss = "btn btn-success";
                            iconCss = "fa fa-lg fa-check";
                        }
                        else
                        {
                            urlCss = "btn btn-danger";
                            iconCss = "fa fa-lg fa-remove";
                        }

                        //組合Html
                        html.AppendLine("<a href=\"{0}\" class=\"{1}\"><i class=\"{2}\"></i>&nbsp;{3}</a>".FormatThis(
                                "{0}Prod/Edit/Detail/{1}/{2}/{3}".FormatThis(
                                        Application["WebUrl"]
                                        , Cryptograph.MD5Encrypt(Param_thisID, fn_Param.DesKey)
                                        , DT.Rows[row]["LangCode"].ToString()
                                        , string.IsNullOrEmpty(DataUID) ? "" : Cryptograph.MD5Encrypt(DataUID, fn_Param.DesKey)
                                    )
                                , urlCss
                                , iconCss
                                , DT.Rows[row]["LangName"].ToString()
                            ));
                    }

                    //輸出Html
                    this.lt_DataSetStatus.Text = html.ToString();
                }
            }
        }
        catch (Exception)
        {
            fn_Extensions.JsAlert("系統發生錯誤 - 關聯！", "");
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
            if (fn_Extensions.String_資料長度Byte(this.hf_myItemVal.Value, "1", "40", out ErrMsg) == false)
            {
                SBAlert.Append("「品號」輸入不正確\\n");
            }

            //日期區間
            DateTime getSTime = Convert.ToDateTime(this.tb_StartDate.Text.ToDateString("yyyy-MM-dd HH:mm"));
            DateTime getETime = string.IsNullOrEmpty(this.tb_EndDate.Text) ? getSTime.AddYears(5) : Convert.ToDateTime(this.tb_EndDate.Text.ToDateString("yyyy-MM-dd HH:mm"));
            if (getSTime >= getETime)
            {
                SBAlert.Append("「上架日期」\\n{0} ~ {1}\\n不覺得哪裡怪怪的嗎?\\n".FormatThis(getSTime, getETime));
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
                    //判斷重複
                    if (CheckInUse(this.hf_myItemVal.Value))
                    {
                        fn_Extensions.JsAlert("資料重複新增,請確認品號是否正確\\n", "");
                        return;
                    }

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
    /// 判斷重複新增 (品號)
    /// </summary>
    /// <param name="modelNo">品號</param>
    /// <returns>
    /// true = 重複, 不可通過
    /// </returns>
    private bool CheckInUse(string modelNo)
    {
        try
        {
            using (SqlCommand cmd = new SqlCommand())
            {
                StringBuilder SBSql = new StringBuilder();

                SBSql.AppendLine(" SELECT Model_No FROM Prod WHERE (Model_No = @Model_No) ");

                //[SQL] - Command
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("Model_No", modelNo);
                using (DataTable DT = dbConn.LookupDT(cmd, out ErrMsg))
                {
                    if (DT.Rows.Count > 0)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }

            }

        }
        catch (Exception)
        {
            throw;
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
            SBSql.AppendLine("  SELECT ISNULL(MAX(Prod_ID) ,0) + 1 FROM Prod ");
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
            SBSql.AppendLine(" INSERT INTO Prod( ");
            SBSql.AppendLine("  Prod_ID, Model_No, StartTime, EndTime, ShopUrl, Display, Sort, Class_ID, IsHot");
            SBSql.AppendLine("  , Create_Who, Create_Time");
            SBSql.AppendLine(" ) VALUES ( ");
            SBSql.AppendLine("  @NewID, @Model_No, @StartTime, @EndTime, @ShopUrl, @Display, @Sort, @Class_ID, @IsHot");
            SBSql.AppendLine("  , @Create_Who, GETDATE() ");
            SBSql.AppendLine(" );");


            //[SQL] - Command
            cmd.CommandText = SBSql.ToString();
            cmd.Parameters.AddWithValue("NewID", NewID);
            cmd.Parameters.AddWithValue("Model_No", this.hf_myItemVal.Value.Trim());
            //時間
            DateTime getSTime = Convert.ToDateTime(this.tb_StartDate.Text.ToDateString("yyyy-MM-dd HH:mm"));
            DateTime getETime = string.IsNullOrEmpty(this.tb_EndDate.Text) ? getSTime.AddYears(5) : Convert.ToDateTime(this.tb_EndDate.Text.ToDateString("yyyy-MM-dd HH:mm"));
            cmd.Parameters.AddWithValue("StartTime", getSTime);
            cmd.Parameters.AddWithValue("EndTime", getETime);

            cmd.Parameters.AddWithValue("ShopUrl", this.tb_ShopUrl.Text);
            cmd.Parameters.AddWithValue("Display", this.rbl_Display.SelectedValue);
            cmd.Parameters.AddWithValue("Sort", this.tb_Sort.Text);
            cmd.Parameters.AddWithValue("Class_ID", this.rbl_Class.SelectedValue);
            cmd.Parameters.AddWithValue("IsHot", this.rbl_IsHot.SelectedValue);
            cmd.Parameters.AddWithValue("Create_Who", Session["Login_GUID"].ToString());
            if (dbConn.ExecuteSql(cmd, out ErrMsg) == false)
            {
                fn_Extensions.JsAlert("資料新增失敗！", Page_CurrentUrl);
                return;
            }

            //更新本頁Url
            string thisUrl = "{0}Prod/Edit/{1}/".FormatThis(
                        Application["WebUrl"]
                        , Cryptograph.MD5Encrypt(NewID.ToString(), fn_Param.DesKey)
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
            SBSql.AppendLine(" UPDATE Prod ");
            SBSql.AppendLine(" SET ");
            SBSql.AppendLine("  StartTime = @StartTime, EndTime = @EndTime");
            SBSql.AppendLine("  , ShopUrl = @ShopUrl, Display = @Display, Sort = @Sort, Class_ID = @Class_ID, IsHot = @IsHot");
            SBSql.AppendLine("  , Update_Who = @Update_Who, Update_Time = GETDATE() ");
            SBSql.AppendLine(" WHERE (Prod_ID = @DataID) ");


            //[SQL] - Command
            cmd.CommandText = SBSql.ToString();

            //時間
            DateTime getSTime = Convert.ToDateTime(this.tb_StartDate.Text.ToDateString("yyyy-MM-dd HH:mm"));
            DateTime getETime = string.IsNullOrEmpty(this.tb_EndDate.Text) ? getSTime.AddYears(5) : Convert.ToDateTime(this.tb_EndDate.Text.ToDateString("yyyy-MM-dd HH:mm"));
            cmd.Parameters.AddWithValue("StartTime", getSTime);
            cmd.Parameters.AddWithValue("EndTime", getETime);

            cmd.Parameters.AddWithValue("ShopUrl", this.tb_ShopUrl.Text);
            cmd.Parameters.AddWithValue("Display", this.rbl_Display.SelectedValue);
            cmd.Parameters.AddWithValue("Sort", this.tb_Sort.Text);
            cmd.Parameters.AddWithValue("Class_ID", this.rbl_Class.SelectedValue);
            cmd.Parameters.AddWithValue("IsHot", this.rbl_IsHot.SelectedValue);
            cmd.Parameters.AddWithValue("Update_Who", Session["Login_GUID"].ToString());
            cmd.Parameters.AddWithValue("DataID", Param_thisID);
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

            return DataID.Equals("New") ? "" : Cryptograph.MD5Decrypt(DataID, fn_Param.DesKey);
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
            return "{0}Prod/Edit/{1}/".FormatThis(
                Application["WebUrl"]
                , string.IsNullOrEmpty(Param_thisID) ? "New" : HttpUtility.UrlEncode(Cryptograph.MD5Encrypt(Param_thisID, fn_Param.DesKey))
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
                Url = "{0}Prod/Search/".FormatThis(Application["WebUrl"]);
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

    #endregion


    #region -- 暫存參數 --
    /// <summary>
    /// 暫存參數
    /// </summary>
    public class TempParam
    {
        /// <summary>
        /// [參數] - 編號
        /// </summary>
        private string _tmp_ID;
        public string tmp_ID
        {
            get { return this._tmp_ID; }
            set { this._tmp_ID = value; }
        }

        /// <summary>
        /// [參數] - 名稱
        /// </summary>
        private string _tmp_Name;
        public string tmp_Name
        {
            get { return this._tmp_Name; }
            set { this._tmp_Name = value; }
        }


        /// <summary>
        /// 設定參數值
        /// </summary>
        /// <param name="tmp_ID">編號</param>
        /// <param name="tmp_Name">名稱</param>
        public TempParam(string tmp_ID, string tmp_Name)
        {
            this._tmp_ID = tmp_ID;
            this._tmp_Name = tmp_Name;
        }
    }
    #endregion
}