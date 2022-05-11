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

public partial class MemberData : SecurityCheck
{

    public string ErrMsg;

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!IsPostBack)
            {
                //Token驗證
                if (!CheckToken())
                {
                    Response.Redirect("{0}Login".FormatThis(Application["WebUrl"]));
                    return;
                }

                //** 次標題 **
                this.Page.Title = Resources.resPublic.title_會員資料;

                //填入語系文字
                this.btn_Submit.Text = this.GetLocalResourceObject("txt_傳送").ToString();
                this.btn_Join.Text = this.GetLocalResourceObject("txt_傳送").ToString();

                //[取得/檢查參數] - 洲別
                if (fn_CustomUI.Get_Region(this.ddl_AreaCode, "", fn_Language.PKWeb_Lang, true, out ErrMsg) == false)
                {
                    this.ddl_AreaCode.Items.Insert(0, new ListItem("選單產生失敗", ""));
                }

                //讀取資料
                LookupData();

            }

        }
        catch (Exception)
        {

            throw;
        }
    }

    /// <summary>
    /// Token驗證
    /// </summary>
    private bool CheckToken()
    {
        try
        {
            using (SqlCommand cmd = new SqlCommand())
            {
                StringBuilder sbSQL = new StringBuilder();

                //[SQL] - 資料查詢
                sbSQL.AppendLine(" DECLARE @MemID AS Bigint ");
                sbSQL.AppendLine(" SET @MemID = ( ");
                sbSQL.AppendLine("     SELECT Mem_ID ");
                sbSQL.AppendLine("     FROM Member_Token ");
                sbSQL.AppendLine("     WHERE (TokenID = @TokenID) AND (ActType = @ActType) AND (IsUse = 'N') AND (@CurrTS <= TimeoutTS) ");
                sbSQL.AppendLine(" ) ");

                sbSQL.AppendLine(" SELECT @MemID AS MemberID ");

                //[SQL] - Command
                cmd.CommandText = sbSQL.ToString();
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("TokenID", fn_Param.MemberToken);
                cmd.Parameters.AddWithValue("ActType", 3);
                cmd.Parameters.AddWithValue("CurrTS", Cryptograph.GetCurrentTime());

                //取得資料
                using (DataTable DT = dbConn.LookupDT(cmd, out ErrMsg))
                {
                    if (DT.Rows[0]["MemberID"] == DBNull.Value)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
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
    /// 讀取資料
    /// </summary>
    private void LookupData()
    {
        try
        {
            using (SqlCommand cmd = new SqlCommand())
            {
                //宣告
                StringBuilder SBSql = new StringBuilder();

                //清除參數
                cmd.Parameters.Clear();

                //[SQL] - 資料處理, 修改會員資料
                SBSql.AppendLine(" SELECT ");
                SBSql.AppendLine("  Base.Company, Base.LastName, Base.FirstName, Base.Sex, Base.Mem_Type ");
                SBSql.AppendLine("  , Base.Birthday, Base.Country_Code, Base.Address, Base.Tel, Base.Mobile");
                SBSql.AppendLine("  , Base.IM_QQ, Base.IM_Wechat");
                SBSql.AppendLine("  , myCode.AreaCode");
                SBSql.AppendLine(" FROM Member_Data Base");
                SBSql.AppendLine("  LEFT JOIN Geocode_CountryCode myCode ON Base.Country_Code = myCode.Country_Code");
                SBSql.AppendLine(" WHERE (Base.Mem_ID = @Mem_ID)");

                //[SQL] - Command
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.AddWithValue("Mem_ID", fn_Param.MemberID);
                using (DataTable DT = dbConn.LookupDT(cmd, out ErrMsg))
                {
                    if (DT.Rows.Count == 0)
                    {
                        Response.Redirect("{0}Notification".FormatThis(Application["WebUrl"].ToString()));
                        return;
                    }
                    else
                    {
                        //[填入資料]
                        this.tb_Company.Text = DT.Rows[0]["Company"].ToString();
                        this.tb_cfm_Company.Text = DT.Rows[0]["Company"].ToString();
                        this.tb_LastName.Text = DT.Rows[0]["LastName"].ToString();
                        this.tb_FirstName.Text = DT.Rows[0]["FirstName"].ToString();
                        this.rbl_Sex.SelectedValue = DT.Rows[0]["Sex"].ToString();
                        this.show_sDate.Text = DT.Rows[0]["Birthday"].ToString().ToDateString("yyyy-MM-dd");
                        this.tb_Birthday.Text = DT.Rows[0]["Birthday"].ToString().ToDateString("yyyy-MM-dd");
                        this.ddl_AreaCode.SelectedValue = DT.Rows[0]["AreaCode"].ToString();
                        this.tb_DataValue.Text = DT.Rows[0]["Country_Code"].ToString();
                        this.tb_Address.Text = DT.Rows[0]["Address"].ToString();
                        this.tb_Tel.Text = DT.Rows[0]["Tel"].ToString();
                        this.tb_Mobile.Text = DT.Rows[0]["Mobile"].ToString();
                        this.tb_IM_QQ.Text = DT.Rows[0]["IM_QQ"].ToString();
                        this.tb_IM_Wechat.Text = DT.Rows[0]["IM_Wechat"].ToString();

                        //判斷身份:使用者/經銷商
                        if (DT.Rows[0]["Mem_Type"].ToString().Equals("0"))
                        {
                            //this.ph_memberinfo1.Visible = false;
                            //this.ph_memberinfo2.Visible = true;
                        }
                        else
                        {
                            //this.ph_memberinfo1.Visible = true;
                            //this.ph_memberinfo2.Visible = false;
                        }
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
    /// 更新資料
    /// </summary>
    protected void btn_Submit_Click(object sender, EventArgs e)
    {
        try
        {
            using (SqlCommand cmd = new SqlCommand())
            {
                //宣告
                StringBuilder SBSql = new StringBuilder();

                //清除參數
                cmd.Parameters.Clear();

                //[SQL] - 資料處理, 修改會員資料
                SBSql.AppendLine(" UPDATE Member_Data SET ");
                SBSql.AppendLine("  Company = @Company, LastName = @LastName, FirstName = @FirstName");
                SBSql.AppendLine("  , Sex = @Sex, Birthday = @Birthday, Country_Code = @Country_Code");
                SBSql.AppendLine("  , Address = @Address, Tel = @Tel, Mobile = @Mobile, IM_QQ = @IM_QQ, IM_Wechat = @IM_Wechat");
                SBSql.AppendLine("  , IsWrite = 'Y', Update_Time = GETDATE()");
                SBSql.AppendLine(" WHERE (Mem_ID = @Mem_ID)");

                //[SQL] - Command
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.AddWithValue("Mem_ID", fn_Param.MemberID);
                cmd.Parameters.AddWithValue("Company", this.tb_Company.Text.Left(80));
                cmd.Parameters.AddWithValue("LastName", this.tb_LastName.Text.Left(50));
                cmd.Parameters.AddWithValue("FirstName", this.tb_FirstName.Text.Left(50));
                cmd.Parameters.AddWithValue("Sex", this.rbl_Sex.SelectedValue);
                cmd.Parameters.AddWithValue("Birthday", string.IsNullOrEmpty(this.tb_Birthday.Text) ? DBNull.Value : (object)this.tb_Birthday.Text);
                cmd.Parameters.AddWithValue("Country_Code", this.tb_DataValue.Text);
                cmd.Parameters.AddWithValue("Address", this.tb_Address.Text.Left(150));
                cmd.Parameters.AddWithValue("Tel", this.tb_Tel.Text.Left(30));
                cmd.Parameters.AddWithValue("Mobile", this.tb_Mobile.Text.Left(30));
                cmd.Parameters.AddWithValue("IM_QQ", this.tb_IM_QQ.Text.Left(100));
                cmd.Parameters.AddWithValue("IM_Wechat", this.tb_IM_Wechat.Text.Left(100));
                if (false == dbConn.ExecuteSql(cmd, out ErrMsg))
                {
                    #region -- Log記錄 --
                    fn_Log.writeLog(fn_Param.MemberID, "修改會員資料", "1003", "會員資料修改失敗,原因:{0}".FormatThis(ErrMsg));
                    #endregion

                    Response.Redirect("{0}Notification/9".FormatThis(Application["WebUrl"].ToString()));
                    return;
                }
                else
                {
                    #region -- Log記錄 --
                    fn_Log.writeLog(fn_Param.MemberID, "修改會員資料", "1003", "已完成會員資料修改");
                    #endregion

                    Response.Redirect("{0}Notification/8".FormatThis(Application["WebUrl"].ToString()));
                    return;
                }

            }

        }
        catch (Exception)
        {

            throw;
        }
    }

    /// <summary>
    /// 已是經銷商 - 送出審核單
    /// </summary>
    protected void btn_Join_Click(object sender, EventArgs e)
    {
        try
        {
            using (SqlCommand cmd = new SqlCommand())
            {
                //宣告
                StringBuilder SBSql = new StringBuilder();

                //清除參數
                cmd.Parameters.Clear();

                //[SQL] - 資料處理, 修改會員資料
                SBSql.AppendLine(" UPDATE Member_Data SET ");
                SBSql.AppendLine("  Company = @Company, DealerCheck = 'S'");
                SBSql.AppendLine("  , Update_Time = GETDATE()");
                SBSql.AppendLine(" WHERE (Mem_ID = @Mem_ID)");

                //[SQL] - Command
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.AddWithValue("Mem_ID", fn_Param.MemberID);
                cmd.Parameters.AddWithValue("Company", this.tb_cfm_Company.Text.Left(80));
                if (false == dbConn.ExecuteSql(cmd, out ErrMsg))
                {
                    #region -- Log記錄 --
                    fn_Log.writeLog(fn_Param.MemberID, "已是經銷商-送出審核", "1003", "送出審核單失敗,原因:{0}".FormatThis(ErrMsg));
                    #endregion

                    Response.Redirect("{0}Notification/9".FormatThis(Application["WebUrl"].ToString()));
                    return;
                }
                else
                {
                    #region -- Log記錄 --
                    fn_Log.writeLog(fn_Param.MemberID, "已是經銷商-送出審核", "1003", "已送出審核單");
                    #endregion

                    Response.Redirect("{0}Notification/11".FormatThis(Application["WebUrl"].ToString()));
                    return;
                }

            }

        }
        catch (Exception)
        {

            throw;
        }
    }

}