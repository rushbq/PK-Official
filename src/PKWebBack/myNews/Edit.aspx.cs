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

public partial class News_Edit : SecurityCheck
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
                    Response.Redirect("{0}401.aspx".FormatThis(Application["WebUrl"]));
                    return;
                }

                //** 設定程式編號(重要) **
                if (false == setProgIDs.setID(this.Master, "100", "110"))
                {
                    throw new Exception("目前的Masterpage 沒有實作 IProgID,無法傳值");
                }

                //[取得/檢查參數] - 上架狀態(預設S)
                if (fn_CustomUI.Get_NewsDisp(this.rbl_Display, "S", out ErrMsg) == false)
                {
                    this.rbl_Display.Items.Insert(0, new ListItem("選單產生失敗", ""));
                }


                //[取得/檢查參數] - 區域
                if (fn_CustomUI.Get_Area(this.cbl_Area, new string[] { "1" }, out ErrMsg) == false)
                {
                    this.cbl_Area.Items.Insert(0, new ListItem("選單產生失敗", ""));
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
                SBSql.AppendLine("    , Area.AreaCode ");
                SBSql.AppendLine("    ,( ");
                SBSql.AppendLine("        CASE WHEN ( ");
                SBSql.AppendLine("            SELECT COUNT(*) ");
                SBSql.AppendLine("            FROM PKSYS.dbo.Param_Language Lang ");
                SBSql.AppendLine("            LEFT JOIN News Base ON Lang.LangCode = Base.LangCode AND Base.Group_ID = GP.Group_ID ");
                SBSql.AppendLine("            WHERE (Lang.Display = 'Y') AND (Base.News_ID IS NULL) ");
                SBSql.AppendLine("        ) = 0 THEN 'Y' ELSE 'N' END ");
                SBSql.AppendLine("    ) AS IsDone ");
                SBSql.AppendLine("  , (SELECT Display_Name FROM {0}.dbo.User_Profile WHERE ([Guid] = GP.Create_Who)) AS Create_Name "
                    .FormatThis(fn_SysDB.Param_DB));
                SBSql.AppendLine("  , (SELECT Display_Name FROM {0}.dbo.User_Profile WHERE ([Guid] = GP.Update_Who)) AS Update_Name "
                    .FormatThis(fn_SysDB.Param_DB));
                SBSql.AppendLine("  FROM News_Group GP ");
                SBSql.AppendLine("      INNER JOIN News_Area Area ON GP.Group_ID = Area.Group_ID ");
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
                        this.show_sDate.Text = DT.Rows[0]["StartTime"].ToString().ToDateString("yyyy/MM/dd HH:mm");
                        this.show_eDate.Text = DT.Rows[0]["EndTime"].ToString().ToDateString("yyyy/MM/dd HH:mm");
                        this.tb_StartDate.Text = DT.Rows[0]["StartTime"].ToString().ToDateString("yyyy/MM/dd HH:mm");
                        this.tb_EndDate.Text = DT.Rows[0]["EndTime"].ToString().ToDateString("yyyy/MM/dd HH:mm");
                        this.rbl_Display.SelectedValue = DT.Rows[0]["Display"].ToString();
                        this.rbl_onIndex.SelectedValue = DT.Rows[0]["onIndex"].ToString();
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

                        //區域
                        CheckBoxList cbl = this.cbl_Area;
                        for (int row = 0; row < DT.Rows.Count; row++)
                        {
                            for (int col = 0; col < cbl.Items.Count; col++)
                            {
                                if (cbl.Items[col].Value.Equals(DT.Rows[row]["AreaCode"].ToString()))
                                {
                                    cbl.Items[col].Selected = true;
                                }
                            }

                        }

                        //判斷各語系設定是否完成(remove at 20200520)
                        //if (DT.Rows[0]["IsDone"].Equals("Y"))
                        //{
                        //    this.rbl_Display.Enabled = true;
                        //    this.lb_SetMsg.Text = "(可設定上架狀態)";

                        //}

                        //Flag設定 & 欄位顯示/隱藏
                        this.hf_flag.Value = "Edit";
                        this.ph_Delete.Visible = true;

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

                SBSql.AppendLine(" SELECT Lang.LangName, Lang.LangCode, Base.News_ID ");
                SBSql.AppendLine("     , (CASE WHEN Base.News_ID IS NULL THEN 'N' ELSE 'Y' END) IsSet ");
                SBSql.AppendLine(" FROM PKSYS.dbo.Param_Language Lang ");
                SBSql.AppendLine("     LEFT JOIN News Base ON Lang.LangCode = Base.LangCode AND Base.Group_ID = @DataID ");
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
                        string NewsID = DT.Rows[row]["News_ID"].ToString();

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
                                "{0}News/Edit/Detail/{1}/{2}/{3}".FormatThis(
                                        Application["WebUrl"]
                                        , Cryptograph.MD5Encrypt(Param_thisID, Application["DesKey"].ToString())
                                        , DT.Rows[row]["LangCode"].ToString()
                                        , string.IsNullOrEmpty(NewsID) ? "" : Cryptograph.MD5Encrypt(NewsID, Application["DesKey"].ToString())
                                    )
                                , urlCss
                                , iconCss
                                , DT.Rows[row]["LangName"].ToString()
                            ));
                    }

                    //輸出Html
                    this.lt_NewsSet.Text = html.ToString();
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
            if (fn_Extensions.String_資料長度Byte(this.tb_Group_Name.Text, "1", "150", out ErrMsg) == false)
            {
                SBAlert.Append("「名稱」請輸入1 ~ 75個字\\n");
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
            SBSql.AppendLine("  SELECT ISNULL(MAX(Group_ID) ,0) + 1 FROM News_Group ");
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
            SBSql.AppendLine(" INSERT INTO News_Group( ");
            SBSql.AppendLine("  Group_ID, Group_Name, StartTime, EndTime, Display, Sort, onIndex");
            SBSql.AppendLine("  , Create_Who, Create_Time");
            SBSql.AppendLine(" ) VALUES ( ");
            SBSql.AppendLine("  @NewID, @Group_Name, @StartTime, @EndTime, 'S', @Sort, @onIndex");
            SBSql.AppendLine("  , @Create_Who, GETDATE() ");
            SBSql.AppendLine(" );");

            //區域
            var GetArea = from ListItem item in this.cbl_Area.Items where item.Selected select item.Value;
            if (GetArea.Count() > 0)
            {
                int row = 0;

                foreach (var AreaCode in GetArea)
                {
                    row++;

                    SBSql.AppendLine(" INSERT INTO News_Area( ");
                    SBSql.AppendLine("  Group_ID, AreaCode");
                    SBSql.AppendLine(" ) VALUES ( ");
                    SBSql.AppendLine("  @NewID, @AreaCode_{0} ".FormatThis(row));
                    SBSql.AppendLine(" );");

                    cmd.Parameters.AddWithValue("AreaCode_{0}".FormatThis(row), AreaCode);
                }
            }

            //時間
            DateTime getSTime = Convert.ToDateTime(this.tb_StartDate.Text.ToDateString("yyyy-MM-dd HH:mm"));
            DateTime getETime = string.IsNullOrEmpty(this.tb_EndDate.Text) ? getSTime.AddYears(5) : Convert.ToDateTime(this.tb_EndDate.Text.ToDateString("yyyy-MM-dd HH:mm"));
            cmd.Parameters.AddWithValue("StartTime", getSTime);
            cmd.Parameters.AddWithValue("EndTime", getETime);

            //首頁判斷, 當設定為首頁顯示時
            if (this.rbl_onIndex.SelectedValue.Equals("Y"))
            {
                //將其他資料皆設為N
                SBSql.AppendLine(" UPDATE News_Group SET onIndex = 'N' WHERE (Group_ID <> @NewID); ");
            }
            cmd.Parameters.AddWithValue("onIndex", this.rbl_onIndex.SelectedValue);

            //[SQL] - Command
            cmd.CommandText = SBSql.ToString();
            cmd.Parameters.AddWithValue("NewID", NewID);
            cmd.Parameters.AddWithValue("Group_Name", this.tb_Group_Name.Text.Trim());
            cmd.Parameters.AddWithValue("Sort", this.tb_Sort.Text);
            cmd.Parameters.AddWithValue("Create_Who", Session["Login_GUID"].ToString());
            if (dbConn.ExecuteSql(cmd, out ErrMsg) == false)
            {
                fn_Extensions.JsAlert("資料新增失敗！", Page_CurrentUrl);
                return;
            }

            //更新本頁Url
            string thisUrl = "{0}News/Edit/{1}/".FormatThis(
                        Application["WebUrl"]
                        , Cryptograph.MD5Encrypt(NewID.ToString(), Application["DesKey"].ToString())
                        );


            ////[發送通知信]
            //#region - 寄EMail -
            ////[設定參數] - 建立者(20字)
            //fn_Mail.Create_Who = "PKWeb-System";

            ////[設定參數] - 來源程式/功能
            //fn_Mail.FromFunc = "官網-後台, News";

            ////[設定參數] - 寄件人
            //fn_Mail.Sender = System.Web.Configuration.WebConfigurationManager.AppSettings["SysMail_Sender"];

            ////[設定參數] - 寄件人顯示名稱
            //fn_Mail.SenderName = "屎拜";

            ////[設定參數] - 收件人
            //List<string> emailReceiver = new List<string>();
            //emailReceiver.Add("WebDesigner@mail.prokits.com.tw");
            //fn_Mail.Reciever = emailReceiver;

            ////[設定參數] - 轉寄人群組
            //fn_Mail.CC = null;

            ////[設定參數] - 密件轉寄人群組
            //fn_Mail.BCC = null;

            ////[設定參數] - 郵件主旨
            //fn_Mail.Subject = "[官網後台] News維護,群組編號:{0}".FormatThis(NewID);

            ////[設定參數] - 郵件內容
            //StringBuilder mailBody = new StringBuilder();

            ////內容主體
            //mailBody.Append("<h3>新的News已新增</h3>");
            //mailBody.Append("群組編號：{0}<br/>".FormatThis(NewID));
            //mailBody.Append("識別名稱：{0}<br/>".FormatThis(this.tb_Group_Name.Text.Trim()));
            //mailBody.Append("建立者：{0} - {1}<br/>".FormatThis(Session["Login_UserID"], Session["Login_UserName"]));

            //fn_Mail.MailBody = mailBody;


            ////發送郵件
            //fn_Mail.SendMail();
            //#endregion


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
            SBSql.AppendLine(" UPDATE News_Group ");
            SBSql.AppendLine(" SET Group_Name = @Group_Name ");
            SBSql.AppendLine("  , StartTime = @StartTime, EndTime = @EndTime");
            SBSql.AppendLine("  , Display = @Display, Sort = @Sort, onIndex = @onIndex");
            SBSql.AppendLine("  , Update_Who = @Update_Who, Update_Time = GETDATE() ");
            SBSql.AppendLine(" WHERE (Group_ID = @DataID) ");

            //區域
            var GetArea = from ListItem item in this.cbl_Area.Items where item.Selected select item.Value;
            if (GetArea.Count() > 0)
            {
                //先清除資料
                SBSql.AppendLine(" DELETE FROM News_Area WHERE (Group_ID = @DataID);");

                int row = 0;

                foreach (var AreaCode in GetArea)
                {
                    row++;

                    SBSql.AppendLine(" INSERT INTO News_Area( ");
                    SBSql.AppendLine("  Group_ID, AreaCode");
                    SBSql.AppendLine(" ) VALUES ( ");
                    SBSql.AppendLine("  @DataID, @AreaCode_{0} ".FormatThis(row));
                    SBSql.AppendLine(" );");

                    cmd.Parameters.AddWithValue("AreaCode_{0}".FormatThis(row), AreaCode);
                }
            }

            //時間
            DateTime getSTime = Convert.ToDateTime(this.tb_StartDate.Text.ToDateString("yyyy-MM-dd HH:mm"));
            DateTime getETime = string.IsNullOrEmpty(this.tb_EndDate.Text) ? getSTime.AddYears(5) : Convert.ToDateTime(this.tb_EndDate.Text.ToDateString("yyyy-MM-dd HH:mm"));
            cmd.Parameters.AddWithValue("StartTime", getSTime);
            cmd.Parameters.AddWithValue("EndTime", getETime);

            //首頁判斷, 當設定為首頁顯示時
            if (this.rbl_onIndex.SelectedValue.Equals("Y"))
            {
                //將其他資料皆設為N
                SBSql.AppendLine(" UPDATE News_Group SET onIndex = 'N' WHERE (Group_ID <> @DataID); ");
            }
            cmd.Parameters.AddWithValue("onIndex", this.rbl_onIndex.SelectedValue);

            //[SQL] - Command
            cmd.CommandText = SBSql.ToString();
            cmd.Parameters.AddWithValue("Group_Name", this.tb_Group_Name.Text.Trim());
            cmd.Parameters.AddWithValue("Display", this.rbl_Display.SelectedValue);
            cmd.Parameters.AddWithValue("Sort", this.tb_Sort.Text);
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

                //[SQL] - 資料更新
                SBSql.AppendLine(" DELETE FROM News_Block WHERE (News_ID IN (SELECT News_ID FROM News WHERE (Group_ID = @DataID))); ");
                SBSql.AppendLine(" DELETE FROM News WHERE (Group_ID = @DataID); ");
                SBSql.AppendLine(" DELETE FROM News_Area WHERE (Group_ID = @DataID); ");
                SBSql.AppendLine(" DELETE FROM News_Group WHERE (Group_ID = @DataID); ");

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
                    string fileUrl = @"{0}News\{1}\".FormatThis(Application["File_DiskUrl"], Param_thisID);
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
            return "{0}News/Edit/{1}/".FormatThis(
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
                Url = "{0}News/Search/".FormatThis(Application["WebUrl"]);
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

}