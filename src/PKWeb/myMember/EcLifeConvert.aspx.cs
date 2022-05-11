using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ExtensionMethods;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public partial class Message : System.Web.UI.Page
{
    public string ErrMsg;

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!IsPostBack)
            {
                //** 次標題 **
                this.Page.Title = Resources.resPublic.title_訊息通知;

                //Response.Write(Req_DataID);
                //Response.Write("<br>" + Cryptograph.MD5Decrypt(Req_TS, Application["DesKey"].ToString()));
                //Response.Write("<br>" + Cryptograph.GetCurrentTime());


            }

        }
        catch (Exception)
        {

            throw;
        }
    }

    protected void lbtn_Convert_Click(object sender, EventArgs e)
    {
        try
        {
            //判斷是否逾時
            Int64 timeOutTS = Convert.ToInt64(Cryptograph.MD5Decrypt(Req_TS, Application["DesKey"].ToString()));
            Int64 timeNow = Cryptograph.GetCurrentTime();

            if (timeOutTS < timeNow)
            {
                this.ph_Msg.Visible = true;
                this.ph_View.Visible = false;
                return;
            }


            //檢查來源編號是否正確
            string memberID = Cryptograph.MD5Decrypt(Req_DataID, Application["DesKey"].ToString());

            //檢查無誤後，回傳Json會員資料
            string memberData = Check_Member(memberID);
            if (string.IsNullOrEmpty(memberData))
            {
                //不存在
                Response.Redirect(GoUrl("6"));
                return;
            }

            //開始轉換會員
            if (Tranfer_Member(memberData))
            {
                //轉換成功
                Response.Redirect(GoUrl("14"));
                return;
            }
            else
            {
                //轉換失敗
                Response.Redirect(GoUrl("15"));
                return;
            }

        }
        catch (Exception)
        {
            this.ph_Msg.Visible = true;
            this.ph_View.Visible = false;
        }

    }


    /// <summary>
    /// 檢查來源編號是否正確
    /// </summary>
    /// <param name="memberID"></param>
    /// <returns>json</returns>
    private string Check_Member(string memberID)
    {
        using (SqlCommand cmd = new SqlCommand())
        {
            //宣告
            StringBuilder SBSql = new StringBuilder();

            SBSql.AppendLine(" SELECT cno, ModifyDate, RegisterDate, Name, Email, Mobile, Sex, password, Birthday, LiveCityName, LiveStateName, LiveState, LiveAddress");
            SBSql.AppendLine(" FROM EcLife_MemberData WITH(NOLOCK)");
            SBSql.AppendLine(" WHERE (cno = @memberID) AND (Sync_Status = 'N')");

            //[SQL] - Command
            cmd.CommandText = SBSql.ToString();
            cmd.Parameters.AddWithValue("memberID", memberID);

            using (DataTable DT = dbConn.LookupDT(cmd, out ErrMsg))
            {
                if (DT.Rows.Count == 0)
                {
                    return "";
                }
                else
                {
                    return JsonConvert.SerializeObject(DT, Formatting.Indented);
                }
            }
        }
    }


    /// <summary>
    /// 轉換會員
    /// </summary>
    /// <param name="memberData">良興會員json</param>
    /// <returns></returns>
    private bool Tranfer_Member(string memberData)
    {
        //取得良興會員資料
        JArray aryObj = JArray.Parse(memberData);

        //Get Data Columns
        var member = aryObj
            .Select(i => new
            {
                cno = i["cno"].ToString(),
                ModifyDate = i["ModifyDate"].ToString(),
                RegisterDate = i["RegisterDate"].ToString(),
                Name = i["Name"].ToString(),
                Email = i["Email"].ToString(),
                Mobile = i["Mobile"].ToString(),
                Sex = i["Sex"].ToString(),
                password = i["password"].ToString(),
                Birthday = i["Birthday"].ToString(),
                LiveCityName = i["LiveCityName"].ToString(),
                LiveStateName = i["LiveStateName"].ToString(),
                LiveState = i["LiveState"].ToString(),
                LiveAddress = i["LiveAddress"].ToString()
            }).FirstOrDefault();


        //轉換會員
        using (SqlCommand cmd = new SqlCommand())
        {
            //宣告
            StringBuilder SBSql = new StringBuilder();

            //[SQL] - 取得新編號
            SBSql.AppendLine(" DECLARE @NewID AS INT ");
            SBSql.AppendLine(" SET @NewID = (");
            SBSql.AppendLine("  SELECT ISNULL(MAX(Mem_ID) ,0) + 1 FROM Member_Data ");
            SBSql.AppendLine(" );");
            
            //[SQL] - 資料處理, 新增會員資料
            SBSql.AppendLine(" INSERT INTO Member_Data( ");
            SBSql.AppendLine("  Mem_ID, Mem_Account, Mem_Pwd, Mem_Type, LastName");
            SBSql.AppendLine("  , Display, Sex, Birthday, Country_Code, Address, Mobile, ComeFrom");
            SBSql.AppendLine(" ) VALUES (");
            SBSql.AppendLine("  @NewID, @Mem_Account, @Mem_Pwd, 0, @LastName");
            SBSql.AppendLine("  , 'Y', @Sex, @Birthday, 'TW', @Address, @Mobile, 'EcLife'");
            SBSql.AppendLine(" );");


            //[SQL] - 資料處理, Update 良興會員狀態
            SBSql.AppendLine(" UPDATE EcLife_MemberData SET Sync_Status = 'Y'");
            SBSql.AppendLine(" WHERE (cno = @cno)");

            //[SQL] - Command
            cmd.CommandText = SBSql.ToString();
            cmd.Parameters.AddWithValue("cno", member.cno);
            cmd.Parameters.AddWithValue("Mem_Account", member.Email);
            cmd.Parameters.AddWithValue("Mem_Pwd", member.password);
            cmd.Parameters.AddWithValue("LastName", member.Name);
            cmd.Parameters.AddWithValue("Sex", member.Sex);
            cmd.Parameters.AddWithValue("Birthday", string.IsNullOrEmpty(member.Birthday) ? DBNull.Value : (object)member.Birthday.ToDateString("yyyy/MM/dd"));
            cmd.Parameters.AddWithValue("Address", "({0}) {1}{2}{3}".FormatThis(
                member.LiveState
                , member.LiveCityName
                , member.LiveStateName
                , member.LiveAddress));
            cmd.Parameters.AddWithValue("Mobile", member.Mobile);

            return dbConn.ExecuteSql(cmd, out ErrMsg);
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


    #region -- 參數設定 --
    /// <summary>
    /// 取得傳遞參數 - 資料編號
    /// </summary>
    private string _Req_DataID;
    public string Req_DataID
    {
        get
        {
            String DataID = Page.RouteData.Values["DataID"] == null ? "" : Page.RouteData.Values["DataID"].ToString();

            return string.IsNullOrEmpty(DataID) ? "" : DataID;
        }
        set
        {
            this._Req_DataID = value;
        }
    }


    /// <summary>
    /// 取得傳遞參數 - ts
    /// </summary>
    private string _Req_TS;
    public string Req_TS
    {
        get
        {
            string GetCode = fn_stringFormat.Set_FilterHtml(Request.QueryString["ts"]);

            return GetCode;
        }
        set
        {
            this._Req_TS = value;
        }
    }
    #endregion

}