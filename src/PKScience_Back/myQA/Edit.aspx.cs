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
using ExtensionIO;
using ExtensionMethods;
using ExtensionUI;

public partial class FAQ_Edit : SecurityCheck
{
    public string ErrMsg;
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!IsPostBack)
            {
                //[權限判斷]
                if (fn_CheckAuth.CheckAuth("620", out ErrMsg) == false)
                {
                    Response.Redirect("{0}401.aspx".FormatThis(Application["WebUrl"]));
                    return;
                }

                //** 設定程式編號(重要) **
                if (false == setProgIDs.setID(this.Master, "600", "620"))
                {
                    throw new Exception("目前的Masterpage 沒有實作 IProgID,無法傳值");
                }

                //[取得/檢查參數] - 類別
                if (fn_CustomUI.Get_FAQClass(this.ddl_Class, "", true, out ErrMsg) == false)
                {
                    this.ddl_Class.Items.Insert(0, new ListItem("選單產生失敗", ""));
                }

                //[取得/檢查參數] - 上架狀態 
                if (fn_CustomUI.Get_PubDisp(this.rbl_Display, "N", out ErrMsg) == false)
                {
                    this.rbl_Display.Items.Insert(0, new ListItem("選單產生失敗", ""));
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
                SBSql.AppendLine(" SELECT GP.* ");
                SBSql.AppendLine("    ,( ");
                SBSql.AppendLine("        CASE WHEN ( ");
                SBSql.AppendLine("            SELECT COUNT(*) ");
                SBSql.AppendLine("            FROM PKSYS.dbo.Param_Language Lang ");
                SBSql.AppendLine("            LEFT JOIN FAQ Base ON Lang.LangCode = Base.LangCode AND Base.Group_ID = GP.Group_ID ");
                SBSql.AppendLine("            WHERE (Lang.Display = 'Y') AND (Base.FAQ_ID IS NULL) ");
                SBSql.AppendLine("        ) = 0 THEN 'Y' ELSE 'N' END ");
                SBSql.AppendLine("    ) AS IsDone ");
                SBSql.AppendLine("  , (SELECT Display_Name FROM {0}.dbo.User_Profile WHERE ([Guid] = GP.Create_Who)) AS Create_Name "
                    .FormatThis(fn_SysDB.Param_DB));
                SBSql.AppendLine("  , (SELECT Display_Name FROM {0}.dbo.User_Profile WHERE ([Guid] = GP.Update_Who)) AS Update_Name "
                    .FormatThis(fn_SysDB.Param_DB));
                SBSql.AppendLine("  FROM FAQ_Group GP ");
                SBSql.AppendLine(" WHERE (GP.Group_ID = @DataID) ");
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
                        this.lb_DataID.Text = DT.Rows[0]["Group_ID"].ToString();
                        this.tb_Group_Name.Text = DT.Rows[0]["Group_Name"].ToString();
                        this.ddl_Class.SelectedValue = DT.Rows[0]["Class_ID"].ToString();
                        this.rbl_Display.SelectedValue = DT.Rows[0]["Display"].ToString();
                        this.tb_Sort.Text = DT.Rows[0]["Sort"].ToString();

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
                        this.ph_Delete.Visible = true;

                        //帶出關聯資料
                        LookupData_Detail();

                        //帶出品號資料
                        LookupData_Rel_ModelNo();
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

                SBSql.AppendLine(" SELECT Lang.LangName, Lang.LangCode, Base.FAQ_ID ");
                SBSql.AppendLine("     , (CASE WHEN Base.FAQ_ID IS NULL THEN 'N' ELSE 'Y' END) IsSet ");
                SBSql.AppendLine(" FROM PKSYS.dbo.Param_Language Lang ");
                SBSql.AppendLine("     LEFT JOIN FAQ Base ON Lang.LangCode = Base.LangCode AND Base.Group_ID = @DataID ");
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
                        string FAQID = DT.Rows[row]["FAQ_ID"].ToString();

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
                                "{0}QA/Edit/Detail/{1}/{2}/{3}".FormatThis(
                                        Application["WebUrl"]
                                        , Cryptograph.MD5Encrypt(Param_thisID,fn_Param.DesKey)
                                        , DT.Rows[row]["LangCode"].ToString()
                                        , string.IsNullOrEmpty(FAQID) ? "" : Cryptograph.MD5Encrypt(FAQID,fn_Param.DesKey)
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


    /// <summary>
    /// 顯示品號關聯資料
    /// </summary>
    private void LookupData_Rel_ModelNo()
    {
        try
        {
            using (SqlCommand cmd = new SqlCommand())
            {
                //[SQL] - 清除參數設定
                cmd.Parameters.Clear();

                //[SQL] - 資料查詢
                StringBuilder SBSql = new StringBuilder();
                SBSql.AppendLine(" SELECT Model_No ");
                SBSql.AppendLine(" FROM FAQ_Rel_ModelNo ");
                SBSql.AppendLine(" WHERE (Group_ID = @DataID) ");
                SBSql.AppendLine(" ORDER BY Model_No ");
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.AddWithValue("DataID", Param_thisID);
                using (DataTable DT = dbConn.LookupDT(cmd, out ErrMsg))
                {
                    if (DT.Rows.Count > 0)
                    {
                        StringBuilder itemHtml = new StringBuilder();

                        for (int row = 0; row < DT.Rows.Count; row++)
                        {
                            //取得參數
                            string TagID = DT.Rows[row]["Model_No"].ToString();
                            string TagName = DT.Rows[row]["Model_No"].ToString();

                            //組合Html
                            itemHtml.AppendLine("<li id=\"li_{0}\" style=\"padding-top:5px;\">".FormatThis(row));
                            itemHtml.Append("<input type=\"hidden\" class=\"item_ID\" value=\"{0}\" />".FormatThis(TagID));
                            itemHtml.Append("<input type=\"hidden\" class=\"item_Name\" value=\"{0}\" />".FormatThis(TagName));
                            itemHtml.Append("<a href=\"javascript:Delete_Item('{0}');\" class=\"btn btn-success\">{1}&nbsp;<i class=\"fa fa-trash\"></i></a>"
                                .FormatThis(row, TagName));
                            itemHtml.AppendLine("</li>");
                        }

                        this.lt_myItems.Text = itemHtml.ToString();

                    }
                }
            }
        }
        catch (Exception)
        {
            fn_Extensions.JsAlert("系統發生錯誤 - 品號資料！", "");
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
            if (fn_Extensions.String_資料長度Byte(this.tb_Group_Name.Text, "1", "150", out ErrMsg) == false)
            {
                SBAlert.Append("「名稱」請輸入1 ~ 75個字\\n");
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

            //[SQL] - 取得新編號
            SBSql.AppendLine(" DECLARE @NewID AS INT ");
            SBSql.AppendLine(" SET @NewID = (");
            SBSql.AppendLine("  SELECT ISNULL(MAX(Group_ID) ,0) + 1 FROM FAQ_Group ");
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
            SBSql.AppendLine(" INSERT INTO FAQ_Group( ");
            SBSql.AppendLine("  Group_ID, Group_Name, Class_ID, Display, Sort");
            SBSql.AppendLine("  , Create_Who, Create_Time");
            SBSql.AppendLine(" ) VALUES ( ");
            SBSql.AppendLine("  @NewID, @Group_Name, @Class_ID, @Display, @Sort");
            SBSql.AppendLine("  , @Create_Who, GETDATE() ");
            SBSql.AppendLine(" );");

            //[SQL] - Command
            cmd.CommandText = SBSql.ToString();
            cmd.Parameters.AddWithValue("NewID", NewID);
            cmd.Parameters.AddWithValue("Group_Name", this.tb_Group_Name.Text.Trim());
            cmd.Parameters.AddWithValue("Class_ID", this.ddl_Class.SelectedValue);
            cmd.Parameters.AddWithValue("Display", this.rbl_Display.SelectedValue);
            cmd.Parameters.AddWithValue("Sort", this.tb_Sort.Text);
            cmd.Parameters.AddWithValue("Create_Who", Session["Login_GUID"].ToString());

            if (dbConn.ExecuteSql(cmd, out ErrMsg) == false)
            {
                fn_Extensions.JsAlert("資料新增失敗！", Page_CurrentUrl);
                return;
            }

            //更新本頁Url
            string thisUrl = "{0}QA/Edit/{1}/".FormatThis(
                        Application["WebUrl"]
                        , Cryptograph.MD5Encrypt(NewID.ToString(),fn_Param.DesKey)
                        );


            //[SQL] - 資料新增, 品號
            if (false == Set_DataRel(NewID.ToString()))
            {
                fn_Extensions.JsAlert("品號關聯設定失敗！", thisUrl);
                return;
            }
            else
            {
                //導向本頁
                Response.Redirect(thisUrl);
            }
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
            SBSql.AppendLine(" UPDATE FAQ_Group ");
            SBSql.AppendLine(" SET Group_Name = @Group_Name, Class_ID = @Class_ID ");
            SBSql.AppendLine("  , Display = @Display, Sort = @Sort");
            SBSql.AppendLine("  , Update_Who = @Update_Who, Update_Time = GETDATE() ");
            SBSql.AppendLine(" WHERE (Group_ID = @DataID) ");

            //[SQL] - Command
            cmd.CommandText = SBSql.ToString();
            cmd.Parameters.AddWithValue("Group_Name", this.tb_Group_Name.Text.Trim());
            cmd.Parameters.AddWithValue("Class_ID", this.ddl_Class.SelectedValue);
            cmd.Parameters.AddWithValue("Display", this.rbl_Display.SelectedValue);
            cmd.Parameters.AddWithValue("Sort", this.tb_Sort.Text);
            cmd.Parameters.AddWithValue("Update_Who", Session["Login_GUID"].ToString());
            cmd.Parameters.AddWithValue("DataID", Param_thisID);
            if (dbConn.ExecuteSql(cmd, out ErrMsg) == false)
            {
                fn_Extensions.JsAlert("資料更新失敗！", Page_CurrentUrl);
                return;
            }

            //[SQL] - 資料新增, 品號
            if (false == Set_DataRel(Param_thisID))
            {
                fn_Extensions.JsAlert("品號關聯設定失敗！", Page_CurrentUrl);
                return;
            }
            else
            {
                //導向本頁
                Response.Redirect(Page_CurrentUrl);
            }
        }
    }

    /// <summary>
    /// 資料刪除
    /// </summary>
    protected void lbtn_Delete_Click(object sender, EventArgs e)
    {
        try
        {
            using (SqlCommand cmd = new SqlCommand())
            {
                StringBuilder SBSql = new StringBuilder();

                //[SQL] - 資料刪除
                SBSql.AppendLine(" DELETE FROM FAQ_Rel_ModelNo WHERE (Group_ID = @DataID); ");
                SBSql.AppendLine(" DELETE FROM FAQ_Block WHERE (FAQ_ID IN (SELECT FAQ_ID FROM FAQ WHERE (Group_ID = @DataID))); ");
                SBSql.AppendLine(" DELETE FROM FAQ WHERE (Group_ID = @DataID); ");
                SBSql.AppendLine(" DELETE FROM FAQ_Group WHERE (Group_ID = @DataID); ");

                //[SQL] - Command
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("DataID", Param_thisID);
                if (dbConn.ExecuteSql(cmd, out ErrMsg) == false)
                {
                    fn_Extensions.JsAlert("刪除失敗！", Page_CurrentUrl);
                    return;
                }
                else
                {
                    //刪除整個Folder
                    string fileUrl = @"{0}FAQ\{1}\".FormatThis(fn_Param.File_DiskUrl, Param_thisID);
                    IOManage.DelFolder(fileUrl);

                    //導向列表
                    Response.Redirect(Page_SearchUrl);
                }

            }

        }
        catch (Exception)
        {
            fn_Extensions.JsAlert("系統發生錯誤 - 資料刪除", "");
            return;
        }

    }


    /// <summary>
    /// 關聯設定
    /// </summary>
    /// <param name="DataID">資料編號</param>
    /// <returns></returns>
    private bool Set_DataRel(string DataID)
    {
        //取得欄位值
        string Get_IDs = this.val_Items.Text;

        //判斷是否為空
        if (string.IsNullOrEmpty(Get_IDs))
        {
            return true;
        }

        //取得陣列資料
        string[] strAry_ID = Regex.Split(Get_IDs, @"\,{1}");

        //宣告暫存清單
        List<TempParam_Item> ITempList = new List<TempParam_Item>();

        //存入暫存清單
        for (int row = 0; row < strAry_ID.Length; row++)
        {
            ITempList.Add(new TempParam_Item(strAry_ID[row]));
        }

        //過濾重複資料
        var query = from el in ITempList
                    group el by new
                    {
                        ID = el.tmp_ID
                    } into gp
                    select new
                    {
                        ID = gp.Key.ID
                    };

        //處理資料
        using (SqlCommand cmd = new SqlCommand())
        {
            //宣告
            StringBuilder SBSql = new StringBuilder();

            //[SQL] - 清除參數設定
            cmd.Parameters.Clear();

            SBSql.AppendLine(" DELETE FROM FAQ_Rel_ModelNo WHERE (Group_ID = @DataID); ");

            int row = 0;
            foreach (var item in query)
            {
                row++;

                SBSql.AppendLine(" INSERT INTO FAQ_Rel_ModelNo( ");
                SBSql.AppendLine("  Group_ID, Model_No");
                SBSql.AppendLine(" ) VALUES ( ");
                SBSql.AppendLine("  @DataID, @Model_No_{0}".FormatThis(row));
                SBSql.AppendLine(" ); ");

                cmd.Parameters.AddWithValue("Model_No_" + row, item.ID);
            }

            //[SQL] - Command
            cmd.CommandText = SBSql.ToString();
            cmd.Parameters.AddWithValue("DataID", DataID);
            if (dbConn.ExecuteSql(cmd, out ErrMsg) == false)
            {
                return false;
            }
            else
            {
                return true;
            }

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

            return DataID.Equals("New") ? "" : Cryptograph.MD5Decrypt(DataID,fn_Param.DesKey);
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
            return "{0}QA/Edit/{1}/".FormatThis(
                Application["WebUrl"]
                , string.IsNullOrEmpty(Param_thisID) ? "New" : HttpUtility.UrlEncode(Cryptograph.MD5Encrypt(Param_thisID,fn_Param.DesKey))
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
                Url = "{0}QA/Search/".FormatThis(Application["WebUrl"]);
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


    #region -- 暫存參數設定 --

    /// <summary>
    /// 暫存參數
    /// </summary>
    public class TempParam_Item
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
        /// 設定參數值
        /// </summary>
        /// <param name="tmp_ID">編號</param>
        public TempParam_Item(string tmp_ID)
        {
            this._tmp_ID = tmp_ID;
        }
    }

    #endregion
}