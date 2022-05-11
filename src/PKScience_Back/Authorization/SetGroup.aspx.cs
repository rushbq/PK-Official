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
using LogRecord;

public partial class Authorization_SetGroup : SecurityCheck
{
    public string ErrMsg;
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!IsPostBack)
            {
                //[權限判斷]
                if (fn_CheckAuth.CheckAuth("9902", out ErrMsg) == false)
                {
                    Response.Redirect("{0}401.aspx".FormatThis(Application["WebUrl"]));
                    return;
                }

                //** 設定程式編號(重要) **
                if (false == setProgIDs.setID(this.Master, "9900", "9902"))
                {
                    throw new Exception("目前的Masterpage 沒有實作 IProgID,無法傳值");
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
            //[取得資料] - 取得資料
            using (SqlCommand cmd = new SqlCommand())
            {
                //宣告
                StringBuilder SBSql = new StringBuilder();

                //清除參數
                cmd.Parameters.Clear();

                //[SQL] - 資料查詢
                SBSql.AppendLine(" SELECT Guid, Display_Name, Account_Name");
                SBSql.AppendLine(" FROM User_Group ");
                SBSql.AppendLine(" WHERE (Guid = @DataID) ");
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.AddWithValue("DataID", Param_thisID);
                using (DataTable DT = dbConn.LookupDT(cmd, dbConn.DBS.PKSYS, out ErrMsg))
                {
                    if (DT.Rows.Count == 0)
                    {
                        fn_Extensions.JsAlert("查無資料！", "");
                        return;
                    }
                    else
                    {
                        //[填入資料]
                        this.tb_EmpValue.Text = DT.Rows[0]["Guid"].ToString();
                        this.lt_Guid.Text = DT.Rows[0]["Guid"].ToString();
                        this.lt_GroupName.Text = "<a class=\"btn btn-success\"><i class=\"fa fa-users fa-lg\"></i>&nbsp;{0}({1})</a>".FormatThis(
                                DT.Rows[0]["Display_Name"].ToString()
                                , DT.Rows[0]["Account_Name"].ToString()
                            );

                        //[顯示/隱藏]
                        this.hf_flag.Value = "Edit";
                        this.ph_data.Visible = true;
                        this.ph_btns.Visible = true;
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
    protected void btn_Search_Click(object sender, EventArgs e)
    {
        Response.Redirect("{0}Auth/Group/Set/{1}/".FormatThis(
                    Application["WebUrl"]
                    , HttpUtility.UrlEncode(Cryptograph.MD5Encrypt(this.tb_EmpValue.Text, fn_Param.DesKey))
                ));
    }

    /// <summary>
    /// 設定權限
    /// </summary>
    protected void lbtn_Save_Click(object sender, EventArgs e)
    {
        try
        {
            //[欄位檢查] - 權限編號
            string inputValue = this.tb_IDvalues.Text;
            if (string.IsNullOrEmpty(inputValue))
            {
                fn_Extensions.JsAlert("未勾選任何選項，無法存檔", Page_CurrentUrl);
                return;
            }

            //[取得參數值] - 編號組合
            string[] strAry = Regex.Split(inputValue, @"\|{2}");
            var query = from el in strAry
                        select new
                        {
                            Val = el.ToString().Trim()
                        };

            //[資料儲存]
            using (SqlCommand cmd = new SqlCommand())
            {
                StringBuilder SBSql = new StringBuilder();

                //[SQL] - 清除參數設定
                cmd.Parameters.Clear();


                #region >> Log參數處理 <<

                //[宣告參數] - 原權限ID
                List<string> iProgID_Old = new List<string>();

                //[SQL] - 取得原權限ID (記錄LOG用) 
                SBSql.AppendLine(" SELECT Prog_ID FROM User_Group_Rel_Program WHERE (Guid = @Param_Guid); ");
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.AddWithValue("Param_Guid", Param_Guid);
                using (DataTable DT = dbConn.LookupDT(cmd, out ErrMsg))
                {
                    for (int row = 0; row < DT.Rows.Count; row++)
                    {
                        iProgID_Old.Add(DT.Rows[row]["Prog_ID"].ToString());
                    }
                }

                //[宣告參數] - 新權限ID (記錄LOG用) 
                List<string> iProgID_New = new List<string>();
                foreach (var item in query)
                {
                    iProgID_New.Add(item.Val);
                }

                #endregion

                //[SQL] - 清除參數設定
                cmd.Parameters.Clear();
                SBSql.Clear();

                //[SQL] - 清除關聯
                SBSql.AppendLine(" DELETE FROM User_Group_Rel_Program WHERE (Guid = @Param_Guid); ");

                //[SQL] - 資料新增
                int idx = 0;
                foreach (var item in query)
                {
                    idx++;
                    SBSql.AppendLine(" INSERT INTO User_Group_Rel_Program (Guid, Prog_ID) ");
                    SBSql.AppendLine(" VALUES (@Param_Guid, @Prog_ID" + idx + "); ");

                    cmd.Parameters.AddWithValue("Prog_ID" + idx, item.Val);
                }
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.AddWithValue("Param_Guid", Param_Guid);
                if (dbConn.ExecuteSql(cmd, out ErrMsg) == false)
                {
                    fn_Extensions.JsAlert("資料儲存失敗！", "");
                    return;
                }
                else
                {
                    string empNo = this.tb_EmpValue.Text;

                    //寫入Log
                    if (false == fn_Log.Log_AD_withAuth(
                         "Group"
                         , "設定權限"
                         , empNo
                         , "設定群組使用權限".FormatThis(empNo)
                         , Session["Login_GUID"].ToString()
                         , iProgID_Old
                         , iProgID_New
                         , Param_Guid))
                    {
                        fn_Extensions.JsAlert("權限已設定, Log處理失敗", Page_CurrentUrl);
                        return;
                    }
                    else
                    {
                        //執行轉頁
                        Response.Redirect(Page_CurrentUrl);
                    }
                }
            }

        }
        catch (Exception)
        {
            fn_Extensions.JsAlert("系統發生錯誤 - 存檔", "");
            return;
        }
    }

    /// <summary>
    /// 移除權限
    /// </summary>
    protected void lbtn_Remove_Click(object sender, EventArgs e)
    {
        try
        {
            //[資料儲存]
            using (SqlCommand cmd = new SqlCommand())
            {
                StringBuilder SBSql = new StringBuilder();

                //[SQL] - 清除參數設定
                cmd.Parameters.Clear();

                //[SQL] - 清除關聯
                SBSql.AppendLine(" DELETE FROM User_Group_Rel_Program WHERE (Guid = @Param_Guid); ");

                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.AddWithValue("Param_Guid", Param_Guid);
                if (dbConn.ExecuteSql(cmd, out ErrMsg) == false)
                {
                    fn_Extensions.JsAlert("資料儲存失敗！", "");
                    return;
                }
                else
                {
                    ////[XML設定] - 群組
                    //if (fn_CustomUI.XmlAuthGroup(out ErrMsg) == false)
                    //{
                    //    fn_Extensions.JsAlert("群組權限XML產生失敗！", "");
                    //    return;
                    //}
                    //else
                    //{
                    //    string empNo = this.tb_EmpValue.Text;

                    //    //寫入Log
                    //    if (false == fn_Log.Log_AD(
                    //         "Group"
                    //         , "移除權限"
                    //         , empNo
                    //         , "移除此群組的權限"
                    //         , Session["Login_GUID"].ToString()))
                    //    {
                    //        fn_Extensions.JsAlert("權限已移除, Log處理失敗", Page_CurrentUrl);
                    //        return;
                    //    }
                    //    else
                    //    {
                    //        //執行轉頁
                    //        Response.Redirect(Page_CurrentUrl);
                    //    }

                    //}
                    string empNo = this.tb_EmpValue.Text;

                    //寫入Log
                    if (false == fn_Log.Log_AD(
                         "Group"
                         , "移除權限"
                         , empNo
                         , "移除此群組的權限"
                         , Session["Login_GUID"].ToString()))
                    {
                        fn_Extensions.JsAlert("權限已移除, Log處理失敗", Page_CurrentUrl);
                        return;
                    }
                    else
                    {
                        //執行轉頁
                        Response.Redirect(Page_CurrentUrl);
                    }
                }
            }

        }
        catch (Exception)
        {
            fn_Extensions.JsAlert("系統發生錯誤 - 存檔", "");
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

            return DataID.Equals("New") ? "" : Cryptograph.MD5Decrypt(DataID, fn_Param.DesKey);
        }
        set
        {
            this._Param_thisID = value;
        }
    }

    private string _Param_Guid;
    public string Param_Guid
    {
        get
        {
            return this.lt_Guid.Text;
        }
        set
        {
            this._Param_Guid = value;
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
            return "{0}Auth/Group/Set/{1}/".FormatThis(
                Application["WebUrl"]
                , string.IsNullOrEmpty(Param_thisID) ? "New" : HttpUtility.UrlEncode(Cryptograph.MD5Encrypt(Param_thisID, fn_Param.DesKey))
            );
        }
        set
        {
            this._Page_CurrentUrl = value;
        }
    }
    #endregion

}