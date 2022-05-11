using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ExtensionMethods;
using ExtensionUI;

public partial class ReportList : SecurityCheckDealer
{
    public string ErrMsg;

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!IsPostBack)
            {
                //** 次標題 **
                this.Page.Title = Resources.resPublic.home_經銷商專區;

                //取得資料
                LookupData();


                //EC專屬報表 - 寫死的
                this.ph_EC.Visible = Get_CustID.Equals("1180401");
            }

        }
        catch (Exception)
        {

            throw;
        }
    }

    #region -- 資料顯示 --
    /// <summary>
    /// 顯示資料
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

                //[SQL] - 資料查詢
                SBSql.Append(" SELECT Base.Prog_ID AS ID, Base.Prog_Name_{0} AS Label".FormatThis(fn_Language.Param_Lang));
                SBSql.Append(" FROM Program Base WITH(NOLOCK)");
                SBSql.Append("  INNER JOIN PKSYS.dbo.Customer_Report Rel WITH(NOLOCK) ON Base.Prog_ID = Rel.Prog_ID");
                SBSql.Append(" WHERE (Rel.Cust_ERPID = @CustID) AND (Base.Display = 'Y')");
                SBSql.Append(" ORDER BY Base.Sort");

                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("CustID", Get_CustID);
                using (DataTable DT = dbConn.LookupDT(cmd, dbConn.DBS.Report, out ErrMsg))
                {
                    //DataBind            
                    this.lvDataList.DataSource = DT.DefaultView;
                    this.lvDataList.DataBind();
                }
            }
        }
        catch (Exception)
        {
            throw;
        }
    }


    protected void lvDataList_ItemCommand(object sender, ListViewCommandEventArgs e)
    {
        if (e.Item.ItemType == ListViewItemType.DataItem)
        {
            //取得資料
            string Get_DataID = ((HiddenField)e.Item.FindControl("hf_DataID")).Value;

            ////自定隱藏Form
            //fn_FormPost resp = new fn_FormPost();

            ////Url
            //resp.Url = "{0}Report/{1}".FormatThis(Application["WebUrl"].ToString(), Cryptograph.MD5Encrypt(Get_DataID, Application["DesKey"].ToString()));


            ////年份區間 (DropDownList)
            ///*
            //* 參數名: para_sYear;para_eYear
            //* 屬性設定: para_sYear;para_eYear
            //* SQL: (column >= @para_sYear OR @para_sYear = '') AND (column <= @para_eYear OR @para_eYear = '')
            //*/
            //DropDownList sYear = (DropDownList)e.Item.FindControl("sYear");
            //DropDownList eYear = (DropDownList)e.Item.FindControl("eYear");
            //resp.Add("values_sYear", sYear.SelectedValue);
            //resp.Add("values_eYear", sYear.SelectedValue);


            ////送出表單
            //resp.Post();

            string url = "{0}Report/{1}/".FormatThis(Application["WebUrl"].ToString(), Cryptograph.MD5Encrypt(Get_DataID, Application["DesKey"].ToString()));

            DropDownList sYear = (DropDownList)e.Item.FindControl("sYear");
            DropDownList eYear = (DropDownList)e.Item.FindControl("eYear");
            Response.Redirect("{0}?values_sYear={1}&values_eYear={2}".FormatThis(
                url, sYear.SelectedValue, eYear.SelectedValue
                ));

        }
    }

    protected void lvDataList_ItemDataBound(object sender, ListViewItemEventArgs e)
    {
        if (e.Item.ItemType == ListViewItemType.DataItem)
        {
            ListViewDataItem dataItem = (ListViewDataItem)e.Item;
            //預設年份
            string currYear = DateTime.Now.Year.ToString();

            //取得控制項
            DropDownList sYear = (DropDownList)e.Item.FindControl("sYear");
            if (false == fn_CustomUI.Get_Years(sYear, currYear, false, -4, 0, out ErrMsg))
            {
                sYear.Items.Insert(0, new ListItem("選單產生失敗", ""));
            }
            DropDownList eYear = (DropDownList)e.Item.FindControl("eYear");
            if (false == fn_CustomUI.Get_Years(eYear, currYear, false, -4, 0, out ErrMsg))
            {
                eYear.Items.Insert(0, new ListItem("選單產生失敗", ""));
            }

        }
    }

    #endregion

    #region -- 參數設定 --

    /// <summary>
    /// 設定參數 - 客戶編號
    /// </summary>
    public string Get_CustID
    {
        get
        {
            String DataID = fn_Member.GetDealerID(fn_Param.MemberID);

            return string.IsNullOrEmpty(DataID) ? "" : DataID;
        }
        set
        {
            this._Get_CustID = value;
        }
    }
    private string _Get_CustID;

    /// <summary>
    /// 本頁網址
    /// </summary>
    public string Page_CurrentUrl
    {
        get
        {
            return "{0}Report".FormatThis(Application["WebUrl"].ToString());
        }
        set
        {
            this._Page_CurrentUrl = value;
        }
    }
    private string _Page_CurrentUrl;
    #endregion
}