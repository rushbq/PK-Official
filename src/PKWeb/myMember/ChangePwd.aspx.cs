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
using MailMethods;

public partial class ChangePwd : System.Web.UI.Page
{

    public string ErrMsg;

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            //判斷是否已通過驗證
            if (string.IsNullOrEmpty(fn_Param.MemberID_ChgPwd))
            {
                //驗證碼過期
                Response.Redirect(GoUrl("99"));
                return;
            }

            if (!IsPostBack)
            {
                //** 次標題 **
                this.Page.Title = Resources.resPublic.title_變更密碼;

                //填入語系文字
                this.tb_Password.Attributes.Add("placeholder", this.GetLocalResourceObject("tip_您的密碼").ToString());
                this.tb_CfmPassword.Attributes.Add("placeholder", this.GetLocalResourceObject("tip_您的確認密碼").ToString());
                this.tb_VerifyCode.Attributes.Add("placeholder", this.GetLocalResourceObject("tip_驗證碼").ToString());
                this.img_Verify.ImageUrl = Application["WebUrl"] + "myHandler/Ashx_CreateValidImg.ashx";
                this.btn_Submit.Text = this.GetLocalResourceObject("txt_傳送").ToString();
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
            //[檢查驗證碼]
            string ImgCheckCode = Request.Cookies["ImgCheckCode"].Value;
            if (!this.tb_VerifyCode.Text.ToUpper().Equals(ImgCheckCode))
            {
                this.tb_VerifyCode.Text = "";
                fn_Extensions.JsAlert("{0} {1}".FormatThis(
                        this.GetLocalResourceObject("txt_驗證碼").ToString()
                        , this.GetLocalResourceObject("tip_error").ToString()
                        )
                    , "");
                return;
            }

            //[檢查密碼]
            string GetPwd = this.tb_Password.Text;
            string GetCfmPwd = this.tb_CfmPassword.Text;
            if (!GetPwd.Equals(GetCfmPwd))
            {
                fn_Extensions.JsAlert("{0}".FormatThis(
                        this.GetLocalResourceObject("tip_密碼不一致").ToString()
                        )
                    , "");
                return;
            }


            //變更密碼
            if (UpdatePassword(GetPwd))
            {
                //成功, 觸發良興同步
                string email = fn_Member.GetMemberAccount(fn_Param.MemberID_ChgPwd);
                fn_Param.SetEclifeMemberStatus("", "", email);

                //導至訊息頁
                Response.Redirect(GoUrl("3"));
                return;
            }
            else
            {
                //失敗
                Response.Redirect(GoUrl("4"));
                return;
            }

        }
        catch (Exception)
        {

            throw;
        }
    }

    /// <summary>
    /// 會員資料 - 變更密碼
    /// </summary>
    /// <param name="newPwd">password</param>
    /// <returns></returns>
    private bool UpdatePassword(string newPwd)
    {
        try
        {
            using (SqlCommand cmd = new SqlCommand())
            {
                //宣告
                StringBuilder SBSql = new StringBuilder();

                //清除參數
                cmd.Parameters.Clear();

                //[SQL] - 資料處理, 變更密碼
                SBSql.AppendLine(" UPDATE Member_Data ");
                SBSql.AppendLine(" SET Mem_Pwd = @Mem_Pwd, Update_Time = GETDATE()");
                SBSql.AppendLine(" WHERE (Mem_ID = @Mem_ID) ");

                //[SQL] - Command
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.AddWithValue("Mem_Pwd", Cryptograph.MD5(newPwd).ToLower());
                cmd.Parameters.AddWithValue("Mem_ID", fn_Param.MemberID_ChgPwd);

                return dbConn.ExecuteSql(cmd, out ErrMsg);

            }
        }
        catch (Exception)
        {
            throw new Exception("系統發生錯誤 - 變更密碼");
        }
    }

    /// <summary>
    /// 訊息頁
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    private string GoUrl(string id)
    {
        return "{0}Notification/{1}".FormatThis(
            Application["WebUrl"].ToString()
            , id
            );
    }
}