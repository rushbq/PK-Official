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
                //** 次標題 **
                this.Page.Title = Resources.resPublic.title_學生期保固註冊;


                //填入語系文字
                this.img_Verify.ImageUrl = Application["WebUrl"] + "myHandler/Ashx_CreateValidImg.ashx";
                this.btn_Submit.Text = this.GetLocalResourceObject("txt_傳送").ToString();
                this.tb_SchoolName.Attributes.Add("placeholder", this.GetLocalResourceObject("tip_填入您的學校").ToString());
                this.tb_SchoolDept.Attributes.Add("placeholder", this.GetLocalResourceObject("tip_填入您的科系").ToString());

                //[取得/檢查參數] - 縣市
                if (fn_CustomUI.Get_City(this.ddl_RegionCode, "", Req_Code, true, out ErrMsg) == false)
                {
                    this.ddl_RegionCode.Items.Insert(0, new ListItem("選單產生失敗", ""));
                }

                //帶入預設資料
                string sDate = DateTime.Now.ToString().ToDateString("yyyy/MM/dd");
                string eDate = DateTime.Now.AddYears(1).ToString().ToDateString("yyyy/MM/dd");
                this.tb_RegDate.Text = sDate;
                this.show_sDate.Text = sDate;
                this.tb_WarrantyDate.Text = eDate;
                this.show_eDate.Text = eDate;

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

                //[SQL] - 資料處理, 會員資料
                SBSql.AppendLine(" SELECT ");
                SBSql.AppendLine("  Base.LastName, Base.FirstName, Base.Mobile");
                SBSql.AppendLine(" FROM Member_Data Base");
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
                        this.tb_LastName.Text = DT.Rows[0]["LastName"].ToString();
                        this.tb_FirstName.Text = DT.Rows[0]["FirstName"].ToString();
                        this.tb_Mobile.Text = DT.Rows[0]["Mobile"].ToString();
                    }
                }

            }
        }
        catch (Exception)
        {

            throw;
        }

    }

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
                SBSql.AppendLine("  LastName = @LastName, FirstName = @FirstName, Mobile = @Mobile");
                SBSql.AppendLine("  , IsWrite = 'Y', Update_Time = GETDATE()");
                SBSql.AppendLine(" WHERE (Mem_ID = @Mem_ID)");

                //[SQL] - 取得新編號
                SBSql.AppendLine(" DECLARE @WID AS INT ");
                SBSql.AppendLine(" SET @WID = (");
                SBSql.AppendLine("  SELECT ISNULL(MAX(WID) ,0) + 1 FROM Member_Warranty ");
                SBSql.AppendLine(" );");

                //[SQL] - 資料處理, 新增保固資料
                SBSql.AppendLine(" INSERT INTO Member_Warranty( ");
                SBSql.AppendLine("  WID, Mem_ID, TID, RegDate, WarrDate");
                SBSql.AppendLine("  , Sch_Name, Sch_Dept");
                SBSql.AppendLine(" ) VALUES (");
                SBSql.AppendLine("  @WID, @Mem_ID, @SchoolID, @RegDate, @WarrDate");
                SBSql.AppendLine("  , @Sch_Name, @Sch_Dept");
                SBSql.AppendLine(" );");

                //[SQL] - Command
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.AddWithValue("Mem_ID", fn_Param.MemberID);
                cmd.Parameters.AddWithValue("LastName", this.tb_LastName.Text.Left(50));
                cmd.Parameters.AddWithValue("FirstName", this.tb_FirstName.Text.Left(50));
                cmd.Parameters.AddWithValue("Mobile", this.tb_Mobile.Text.Left(30));
                cmd.Parameters.AddWithValue("SchoolID", this.tb_DataValue.Text);
                cmd.Parameters.AddWithValue("RegDate", this.tb_RegDate.Text);
                cmd.Parameters.AddWithValue("WarrDate", this.tb_WarrantyDate.Text);
                //其他科系
                if (this.tb_DataValue.Text.Equals("-1"))
                {
                    cmd.Parameters.AddWithValue("Sch_Name", this.tb_SchoolName.Text);
                    cmd.Parameters.AddWithValue("Sch_Dept", this.tb_SchoolDept.Text);
                }
                else
                {
                    cmd.Parameters.AddWithValue("Sch_Name", "");
                    cmd.Parameters.AddWithValue("Sch_Dept", "");
                }

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

    #region -- 參數設定 --
    /// <summary>
    /// 取得傳遞參數 - 國家代碼
    /// </summary>
    private string _Req_Code;
    public string Req_Code
    {
        get
        {
            String DataID = Convert.ToString(Page.RouteData.Values["code"]);
            return fn_stringFormat.Set_FilterHtml(DataID).Left(2);
        }
        set
        {
            this._Req_Code = value;
        }
    }
    #endregion
}