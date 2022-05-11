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

public partial class Lot_Result : SecurityCheck
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
                SBSql.AppendLine(" SELECT TOP 1 Base.EventCode, Base.Lot_Name ");
                SBSql.AppendLine("  , Sub.Prize_Model AS id, Sub.Prize_Name AS label, Sub.Prize_Img AS pic");
                SBSql.AppendLine(" FROM Lottery Base ");
                SBSql.AppendLine("  INNER JOIN Lottery_Prize Sub ON Base.Lot_ID = Sub.Lot_ID ");
                SBSql.AppendLine("  INNER JOIN Lottery_Rel_Member Rel ON Sub.Lot_ID = Rel.Lot_ID AND Sub.Lot_PID = Rel.Lot_PID ");
                SBSql.AppendLine(" WHERE (Sub.Lot_ID = @ParentID) AND (Sub.Lot_PID = @DataID) AND (Rel.Mem_ID = @Mem_ID)");

                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.AddWithValue("ParentID", Req_ParentID);
                cmd.Parameters.AddWithValue("DataID", Req_DataID);
                cmd.Parameters.AddWithValue("Mem_ID", fn_Param.MemberID);
                using (DataTable DT = dbConn.LookupDT(cmd, out ErrMsg))
                {
                    if (DT.Rows.Count == 0)
                    {
                        Response.Redirect(Application["WebUrl"].ToString());
                        return;
                    }

                    //基本資料
                    this.lt_subTitle.Text = DT.Rows[0]["Lot_Name"].ToString();
                    this.lt_Name.Text = DT.Rows[0]["label"].ToString();
                    this.lt_Pic.Text = "<img src=\"{0}\" alt=\"Prize\" class=\"img-thumbnail\" style=\"max-height: 270px;\" />".FormatThis(
                        Application["WebUrl"].ToString() + "images/lottery/" + DT.Rows[0]["pic"].ToString()
                        );

                    //Meta資訊
                    meta_Title = DT.Rows[0]["Lot_Name"].ToString();
                    meta_Desc = "{0}{1}{2}【{3}】{4}".FormatThis(
                        "YA!!我在"
                        , DT.Rows[0]["Lot_Name"].ToString()
                        , "抽到了"
                        , DT.Rows[0]["label"].ToString()
                        , "，快一起來抽獎吧!");

                    meta_Url = "{0}Lottery/{1}".FormatThis(
                        Application["WebUrl"].ToString()
                        , HttpUtility.UrlEncode(DT.Rows[0]["EventCode"].ToString())
                        );

                    meta_Image = DT.Rows[0]["pic"].ToString();

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
    /// 取得傳遞參數 - 第一層編號
    /// </summary>
    private string _Req_ParentID;
    public string Req_ParentID
    {
        get
        {
            String DataID = Page.RouteData.Values["ParentID"] == null ? "" : Cryptograph.MD5Decrypt(Page.RouteData.Values["ParentID"].ToString(), DesKey);

            return DataID;
        }
        set
        {
            this._Req_ParentID = value;
        }
    }

    /// <summary>
    /// 取得傳遞參數 - 第二層編號
    /// </summary>
    private string _Req_DataID;
    public string Req_DataID
    {
        get
        {
            String DataID = Page.RouteData.Values["DataID"] == null ? "" : Cryptograph.MD5Decrypt(Page.RouteData.Values["DataID"].ToString(), DesKey);

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