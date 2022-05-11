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

public partial class Lot_Index : System.Web.UI.Page
{
    public string ErrMsg;

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!IsPostBack)
            {
                //** 次標題 **
                this.Page.Title = Resources.resPublic.title_抽獎活動;

                if (string.IsNullOrEmpty(fn_Param.MemberID))
                {
                    this.pl_NotLogin.Visible = true;
                    this.pl_Data.Visible = false;
                    return;
                }

                //讀取資料
                LookupData();

                //隱藏主頁的meta
                PlaceHolder myMeta = (PlaceHolder)Master.FindControl("ph_MetaInfo");
                myMeta.Visible = false;
            }

        }
        catch (Exception)
        {

            throw;
        }
    }

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
                SBSql.AppendLine(" SELECT TOP 1 Base.Lot_ID, Base.Lot_Name, Base.EventCode ");
                SBSql.AppendLine(" FROM Lottery Base ");
                SBSql.AppendLine("  INNER JOIN Lottery_Prize Sub ON Base.Lot_ID = Sub.Lot_ID ");
                SBSql.AppendLine(" WHERE (Base.EventCode = @DataID) AND (Base.Display = 'Y')");
                SBSql.AppendLine("  AND (GETDATE() >= Base.StartTime) AND (GETDATE() <= Base.EndTime)");

                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.AddWithValue("DataID", Req_DataID);
                using (DataTable DT = dbConn.LookupDT(cmd, out ErrMsg))
                {
                    if (DT.Rows.Count == 0)
                    {
                        this.pl_Data.Visible = false;
                        this.pl_NoData.Visible = true;
                        return;
                    }

                    //基本資料
                    this.pl_Data.Visible = true;
                    this.pl_NoData.Visible = false;
                    this.lt_subTitle.Text = DT.Rows[0]["Lot_Name"].ToString();
                    this.hf_DataID.Value = DT.Rows[0]["Lot_ID"].ToString();

                    //Meta資訊
                    meta_Title = "{0}-{1}".FormatThis(
                        Application["WebName"]
                        , DT.Rows[0]["Lot_Name"].ToString());

                    meta_Desc = "{0}{1}{2}".FormatThis(
                        "我在"
                        , DT.Rows[0]["Lot_Name"].ToString()
                        , "，快一起來抽獎吧!");

                    meta_Url = "{0}Lottery/{1}".FormatThis(
                        Application["WebUrl"].ToString()
                        , HttpUtility.UrlEncode(DT.Rows[0]["EventCode"].ToString())
                        );

                    meta_Image = "{0}images/logo_1200.png".FormatThis(
                        Application["WebUrl"].ToString());
                }

            }
        }
        catch (Exception)
        {
            throw;
        }
    }

    /// <summary>
    /// 開始抽獎
    /// </summary>
    protected void btn_GetGift_Click(object sender, EventArgs e)
    {
        using (SqlCommand cmd = new SqlCommand())
        {
            //宣告
            StringBuilder SBSql = new StringBuilder();

            // -----檢查是否已抽過獎 -----
            //[SQL] - 資料查詢
            SBSql.AppendLine(" SELECT COUNT(*) AS Cnt ");
            SBSql.AppendLine(" FROM Lottery_Rel_Member ");
            SBSql.AppendLine(" WHERE (Lot_ID = @DataID) AND (Mem_ID = @Mem_ID)");

            //[SQL] - Command
            cmd.CommandText = SBSql.ToString();
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("Mem_ID", fn_Param.MemberID);
            cmd.Parameters.AddWithValue("DataID", this.hf_DataID.Value);

            //判斷
            using (DataTable DT = dbConn.LookupDT(cmd, out ErrMsg))
            {
                if (Convert.ToInt16(DT.Rows[0]["Cnt"]) > 0)
                {
                    this.pl_Data.Visible = false;
                    this.pl_NotAgain.Visible = true;

                    return;
                }
            }

            // -----開始抽獎 -----
            SBSql.Clear();

            //[SQL] - 資料檢查 & 更新
            SBSql.AppendLine(" DECLARE @PID AS int, @ErrCnt AS int ");
            SBSql.AppendLine(" SET @PID = ( ");
            SBSql.AppendLine("     SELECT TOP 1 Sub.Lot_PID ");
            SBSql.AppendLine("     FROM Lottery Base ");
            SBSql.AppendLine("     INNER JOIN Lottery_Prize Sub ON Base.Lot_ID = Sub.Lot_ID ");
            SBSql.AppendLine("     WHERE (Sub.Lot_ID = @DataID) AND ((Base.Display = 'Y') AND (GETDATE() >= Base.StartTime) AND (GETDATE() <= Base.EndTime)) ");
            SBSql.AppendLine("         AND (Sub.Qty >= 1) ");
            SBSql.AppendLine("     ORDER BY NEWID() ");
            SBSql.AppendLine(" ) ");
            SBSql.AppendLine(" IF @PID IS NULL ");
            SBSql.AppendLine("     SET @ErrCnt = 1 ");
            SBSql.AppendLine(" ELSE ");
            SBSql.AppendLine("     SET @ErrCnt = 0 ");

            SBSql.AppendLine(" IF @ErrCnt = 0 ");
            SBSql.AppendLine("  BEGIN ");
            SBSql.AppendLine("     UPDATE Lottery_Prize ");
            SBSql.AppendLine("     SET Qty = Qty - 1 ");
            SBSql.AppendLine("     WHERE (Lot_ID = @DataID) AND (Lot_PID = @PID) AND (Qty >= 1) ");

            SBSql.AppendLine("     INSERT INTO Lottery_Rel_Member(Lot_ID, Lot_PID, Mem_ID) VALUES (@DataID, @PID, @Mem_ID) ");
            SBSql.AppendLine("  END ");
            SBSql.AppendLine(" SELECT @PID AS myPID, @ErrCnt AS myErrCnt ");

            //[SQL] - Command
            cmd.CommandText = SBSql.ToString();
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("Mem_ID", fn_Param.MemberID);
            cmd.Parameters.AddWithValue("DataID", this.hf_DataID.Value);
            using (DataTable DT = dbConn.LookupDT(cmd, out ErrMsg))
            {
                if (Convert.ToInt16(DT.Rows[0]["myErrCnt"]) > 0)
                {
                    this.pl_Data.Visible = false;
                    this.pl_NoData.Visible = true;

                    return;
                }
                else
                {
                    Response.Redirect("{0}Prize/{1}/{2}".FormatThis(
                        Application["WebUrl"]
                        , Cryptograph.MD5Encrypt(this.hf_DataID.Value, DesKey)
                        , Cryptograph.MD5Encrypt(DT.Rows[0]["myPID"].ToString(), DesKey)
                        ));
                }
            }
        }
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
            String DataID = Page.RouteData.Values["code"] == null ? "" : Page.RouteData.Values["code"].ToString();

            return DataID;
        }
        set
        {
            this._Req_DataID = value;
        }
    }

    /// <summary>
    /// DesKey
    /// </summary>
    private string _DesKey;
    public string DesKey
    {
        get
        {
            return Application["DesKey"].ToString();
        }
        set
        {
            this._DesKey = value;
        }
    }

    public string meta_Title
    {
        get;
        set;
    }

    public string meta_Desc
    {
        get;
        set;
    }

    public string meta_Url
    {
        get;
        set;
    }

    public string meta_Image
    {
        get;
        set;
    }
    #endregion
}