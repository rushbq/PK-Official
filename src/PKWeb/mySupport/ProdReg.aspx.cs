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
using MailMethods;

public partial class ProdReg : SecurityCheck
{

    public string ErrMsg;

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!IsPostBack)
            {
                //** 次標題 **
                this.Page.Title = Resources.resPublic.title_產品保固註冊;

                //填入語系文字
                this.tb_InvoiceNo.Attributes.Add("placeholder", this.GetLocalResourceObject("tip_發票號碼").ToString());
                this.show_sDate.Attributes.Add("placeholder", this.GetLocalResourceObject("tip_購買日期").ToString());
                this.tb_VerifyCode.Attributes.Add("placeholder", this.GetLocalResourceObject("tip_驗證碼").ToString());
                this.tb_myFilterItem.Attributes.Add("placeholder", this.GetLocalResourceObject("tip_產品欄位").ToString());
                this.img_Verify.ImageUrl = Application["WebUrl"] + "myHandler/Ashx_CreateValidImg.ashx";

                //驗證文字
                this.rfv_tb_InvoiceNo.ErrorMessage = this.GetLocalResourceObject("txt_發票號碼").ToString();
                this.rfv_tb_BuyDate.ErrorMessage = this.GetLocalResourceObject("txt_購買日期").ToString();
                this.rfv_tb_VerifyCode.ErrorMessage = this.GetLocalResourceObject("txt_驗證碼").ToString();
                this.rfv_myValues.ErrorMessage = this.GetLocalResourceObject("txt_產品資料").ToString();
                this.ValidationSummary1.HeaderText = this.GetLocalResourceObject("tip_Check").ToString();
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

            //[新增資料]
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
                SBSql.AppendLine("  SELECT ISNULL(MAX(RID) ,0) + 1 FROM Register_Prod ");
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
                SBSql.AppendLine(" INSERT INTO Register_Prod( ");
                SBSql.AppendLine("  RID, Mem_ID, InvoiceNo, BuyDate, RegDate, Create_Time");
                SBSql.AppendLine(" ) VALUES ( ");
                SBSql.AppendLine("  @NewID, @Mem_ID, @InvoiceNo, @BuyDate, @RegDate, GETDATE()");
                SBSql.AppendLine(" )");

                //[SQL] - Command
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.AddWithValue("NewID", NewID);
                cmd.Parameters.AddWithValue("Mem_ID", fn_Param.MemberID);
                cmd.Parameters.AddWithValue("InvoiceNo", this.tb_InvoiceNo.Text);
                cmd.Parameters.AddWithValue("BuyDate", this.tb_BuyDate.Text);
                cmd.Parameters.AddWithValue("RegDate", DateTime.Now.ToShortDateString().ToDateString("yyyy/MM/dd"));
                if (dbConn.ExecuteSql(cmd, out ErrMsg) == false)
                {
                    //失敗
                    Response.Redirect("{0}ContactNoti/2".FormatThis(Application["WebUrl"].ToString()));
                    return;
                }

                //[SQL] - 資料新增, 品號
                if (false == Set_DataRel(NewID.ToString()))
                {
                    //失敗
                    Response.Redirect("{0}ContactNoti/2".FormatThis(Application["WebUrl"].ToString()));
                    return;
                }
                else
                {
                    //產品註冊成功
                    Response.Redirect("{0}ContactNoti/3".FormatThis(Application["WebUrl"].ToString()));
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
    /// 關聯設定
    /// </summary>
    /// <param name="DataID">資料編號</param>
    /// <returns></returns>
    private bool Set_DataRel(string DataID)
    {
        //取得欄位值
        string Get_IDs = this.myValues.Text;

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

            SBSql.AppendLine(" DELETE FROM Register_Prod_Models WHERE (RID = @DataID); ");

            int row = 0;
            foreach (var item in query)
            {
                row++;

                SBSql.AppendLine(" INSERT INTO Register_Prod_Models( ");
                SBSql.AppendLine("  RID, Model_No");
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