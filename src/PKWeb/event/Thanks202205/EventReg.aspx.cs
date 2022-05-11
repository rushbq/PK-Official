using ExtensionMethods;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.UI.WebControls;

public partial class EventReg : System.Web.UI.Page
{

    public string ErrMsg;
    public string DataID = "0dc02318-4235-41d1-b3f2-dcb09f2f998c";
    public string eventFolder = "Thanks202205";
    public string eventTitle = "寶工感恩月登錄抽獎活動";

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!IsPostBack)
            {
                //Token驗證
                CheckToken();

                //檢查活動是否有效
                CheckDataValid();

                //產生驗證碼
                this.img_Verify.ImageUrl = Application["WebUrl"] + "myHandler/Ashx_CreateValidImg.ashx";

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
    private void CheckToken()
    {
        try
        {
            /*
                  填寫頁檢查:
                   A = 傳入TS + 15, 頁面可逾期秒數=15秒
                   B = 目前TS
                   A < B -> 轉回說明頁
                */
            if (string.IsNullOrWhiteSpace(Req_Token))
            {
                //返回說明頁
                Response.Redirect(BackUrl);
            }

            Int64 timeOutTS = Convert.ToInt64(Cryptograph.MD5Decrypt(Req_Token, Application["DesKey"].ToString())) + 15;
            Int64 timeNow = Cryptograph.GetCurrentTime();

            if (timeOutTS < timeNow)
            {
                //返回說明頁
                Response.Redirect(BackUrl);
            }
        }
        catch (Exception)
        {
            Response.Redirect(BackUrl);
        }

    }


    /// <summary>
    /// 檢查活動是否有效
    /// </summary>
    private void CheckDataValid()
    {
        try
        {
            //資料查詢
            using (SqlCommand cmd = new SqlCommand())
            {
                string sql = @"
                    SELECT EventName, Display, StartTime, EndTime
                    FROM [PKMember].dbo.Event_Base
                    WHERE (EventGUID = @id)
                     AND (StartTime <= GETDATE()) AND (EndTime >= GETDATE())
                     AND (Display = 'Y')";

                //[SQL] - Command
                cmd.CommandText = sql;
                cmd.Parameters.AddWithValue("id", DataID);
                using (DataTable DT = dbConn.LookupDT(cmd, out ErrMsg))
                {
                    if (DT.Rows.Count == 0)
                    {
                        fn_Extensions.JsAlert("活動已結束，感謝您的支持。", BackUrl);
                        return;
                    }
                    else
                    {
                        //show button
                        btn_Submit.Visible = true;

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
    /// 送出資料
    /// </summary>
    protected void btn_Submit_Click(object sender, EventArgs e)
    {
        //[檢查驗證碼]
        string ImgCheckCode = Request.Cookies["ImgCheckCode"].Value;
        if (!this.tb_VerifyCode.Text.ToUpper().Equals(ImgCheckCode))
        {
            this.tb_VerifyCode.Text = "";
            fn_Extensions.JsAlert("驗證碼輸入不正確", "");
            return;
        }


        try
        {
            string IsPass = "N";
            //購買產品-複選
            var selectedValues = from ListItem item in cbl_BuyTypes.Items where item.Selected select item.Value;
            string getBuyTypes = "";
            if (selectedValues.Count() > 0)
            {
                getBuyTypes = selectedValues.Aggregate((x, y) => x + "," + y);
            }

            //SQL處理
            using (SqlCommand cmd = new SqlCommand())
            {
                string sql = @"
                    DECLARE @NewID AS INT, @IsPass AS CHAR(1)
                    SET @NewID = (SELECT ISNULL(MAX(DataID) ,0) + 1 FROM [PKMember].dbo.Event_Detail WHERE ParentID = @ParentID)

                    IF (SELECT COUNT(*) FROM [PKMember].dbo.Event_Detail WHERE (ParentID = @ParentID) AND (InvoiceNo = @InvoiceNo)) > 0
                     --發票號碼重複
                     SELECT 'N' AS IsPass

                    ELSE
                     BEGIN 
                        INSERT INTO [PKMember].dbo.Event_Detail (
	                     ParentID, DataID
	                     , FullName, PhoneNumber, Email, InvoiceNo, InvoicePrice
	                     , Birthday, Sex, LineID
	                     , Create_Time
	                     , FromWhere, BuyWhere, BuyTypes
	                    ) VALUES (
	                     @ParentID, @NewID
	                     , @FullName, @PhoneNumber, @Email, @InvoiceNo, @InvoicePrice
	                     , @Birthday, @Sex, @LineID
	                     , GETDATE()
	                     , @FromWhere, @BuyWhere, @BuyTypes
	                    )

	                    SELECT 'Y' AS IsPass

                     END";

                //[SQL] - Command
                cmd.CommandText = sql;
                cmd.Parameters.AddWithValue("ParentID", DataID);
                cmd.Parameters.AddWithValue("FullName", tb_FullName.Text.Left(20));
                cmd.Parameters.AddWithValue("PhoneNumber", tb_PhoneNumber.Text.Left(10));
                cmd.Parameters.AddWithValue("Email", tb_Email.Text.Left(100));
                cmd.Parameters.AddWithValue("InvoiceNo", tb_InvoiceNo.Text.ToUpper().Left(20));
                cmd.Parameters.AddWithValue("InvoicePrice", string.IsNullOrWhiteSpace(tb_InvoicePrice.Text) ? 0 : Convert.ToDouble(tb_InvoicePrice.Text));
                cmd.Parameters.AddWithValue("Birthday", string.IsNullOrWhiteSpace(tb_Birthday.Text) ? DBNull.Value : (object)tb_Birthday.Text);
                cmd.Parameters.AddWithValue("Sex", string.IsNullOrWhiteSpace(rbl_Sex.SelectedValue) ? DBNull.Value : (object)rbl_Sex.SelectedValue);
                cmd.Parameters.AddWithValue("LineID", tb_Line.Text.Left(120));
                cmd.Parameters.AddWithValue("FromWhere", "A");
                cmd.Parameters.AddWithValue("BuyWhere", string.IsNullOrWhiteSpace(rbl_BuyWhere.SelectedValue) ? DBNull.Value : (object)rbl_BuyWhere.SelectedValue);
                cmd.Parameters.AddWithValue("BuyTypes", getBuyTypes);

                using (DataTable DT = dbConn.LookupDT(cmd, out ErrMsg))
                {
                    IsPass = DT.Rows[0]["IsPass"].ToString();
                }

            }

            //Check
            if (IsPass.Equals("N"))
            {
                fn_Extensions.JsAlert("此發票號碼已登錄!", "");
                return;
            }

            //OK, 導回說明頁
            fn_Extensions.JsAlert("登錄完成，感謝您的參與，祝您中獎", BackUrl);

        }
        catch (Exception)
        {

            throw;
        }
    }

    #region -- 參數設定 --

    ///// <summary>
    ///// 取得傳遞參數 - 來源平台(A=Web;B=fb;C=Line)
    ///// </summary>
    //private string _Req_FromWhere;
    //public string Req_FromWhere
    //{
    //    get
    //    {
    //        string GetReqVal = fn_stringFormat.Set_FilterHtml(Request.QueryString["from"]);
    //        switch (GetReqVal.ToUpper())
    //        {
    //            case "FB":
    //                return "B";

    //            case "LINE":
    //                return "C";

    //            default:
    //                return "A";
    //        }
    //    }
    //    set
    //    {
    //        this._Req_FromWhere = value;
    //    }
    //}

    /// <summary>
    /// 取得傳遞參數 - Token
    /// </summary>
    private string _Req_Token;
    public string Req_Token
    {
        get
        {
            string GetReqVal = fn_stringFormat.Set_FilterHtml(Request.QueryString["token"]);

            return GetReqVal;
        }
        set
        {
            this._Req_Token = value;
        }
    }

    public string BackUrl
    {
        get
        {
            return "{0}event/{1}/".FormatThis(Application["WebUrl"], eventFolder);
        }
    }


    private string _Req_utm_source;
    public string Req_utm_source
    {
        get
        {
            string GetReqVal = string.IsNullOrWhiteSpace(Request.QueryString["utm_source"])
                ? "BN"
                : fn_stringFormat.Set_FilterHtml(Request.QueryString["utm_source"]).Left(10);

            return GetReqVal;
        }
        set
        {
            this._Req_utm_source = value;
        }
    }


    private string _Req_utm_medium;
    public string Req_utm_medium
    {
        get
        {
            string GetReqVal = string.IsNullOrWhiteSpace(Request.QueryString["utm_medium"])
                ? ""
                : fn_stringFormat.Set_FilterHtml(Request.QueryString["utm_medium"]).Left(10);

            return GetReqVal;
        }
        set
        {
            this._Req_utm_medium = value;
        }
    }


    private string _Req_utm_campaign;
    public string Req_utm_campaign
    {
        get
        {
            string GetReqVal = string.IsNullOrWhiteSpace(Request.QueryString["utm_campaign"])
                ? ""
                : fn_stringFormat.Set_FilterHtml(Request.QueryString["utm_campaign"]).Left(20);

            return GetReqVal;
        }
        set
        {
            this._Req_utm_campaign = value;
        }
    }
    #endregion

}