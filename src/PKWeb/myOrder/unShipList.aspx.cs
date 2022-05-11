using System;
using System.Data;
using System.Web.UI.WebControls;
using eOrder.Controllers;
using ExtensionUI;
using ExtensionMethods;

public partial class myOrder_unShipList : SecurityCheckDealer
{
    public string ErrMsg;
    public string CustID = fn_Param.Get_CustID;

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!IsPostBack)
            {
                //Get Data
                LookupDataList();

                this.lbtn_Export.Text = "<span class=\"fa fa-file-excel-o\"></span>&nbsp;" + this.GetLocalResourceObject("txt_匯出EXCEL").ToString();
            }

        }
        catch (Exception)
        {

            throw;
        }
    }

    #region -- 資料顯示 --
    /// <summary>
    /// 取得資料
    /// </summary>
    private void LookupDataList()
    {
        //----- 宣告:資料參數 -----
        eOrderingRepository _data = new eOrderingRepository();

        //----- 原始資料:取得所有資料 -----
        var query = _data.GetUnShipDetail(CustID, out ErrMsg);

        //----- 資料繫結 -----
        this.lvDataList.DataSource = query;
        this.lvDataList.DataBind();

        //填入表頭的多語系文字
        if (query != null)
        {
            ((Literal)this.lvDataList.FindControl("lt_header1")).Text = this.GetLocalResourceObject("txt_Header1").ToString();
            ((Literal)this.lvDataList.FindControl("lt_header2")).Text = this.GetLocalResourceObject("txt_Header2").ToString();
            ((Literal)this.lvDataList.FindControl("lt_header3")).Text = this.GetLocalResourceObject("txt_Header3").ToString();
            ((Literal)this.lvDataList.FindControl("lt_header4")).Text = this.GetLocalResourceObject("txt_Header4").ToString();
            ((Literal)this.lvDataList.FindControl("lt_header5")).Text = this.GetLocalResourceObject("txt_Header5").ToString();
            ((Literal)this.lvDataList.FindControl("lt_header6")).Text = this.GetLocalResourceObject("txt_Header6").ToString();
            ((Literal)this.lvDataList.FindControl("lt_header7")).Text = this.GetLocalResourceObject("txt_Header7").ToString();
            ((Literal)this.lvDataList.FindControl("lt_header8")).Text = this.GetLocalResourceObject("txt_Header8").ToString();
        }

    }

    #endregion

    /// <summary>
    /// Export
    /// </summary>
    protected void lbtn_Export_Click(object sender, EventArgs e)
    {
        //----- 宣告:資料參數 -----
        eOrderingRepository _data = new eOrderingRepository();

        //----- 原始資料:取得所有資料 -----
        var query = _data.GetUnShipDetail(CustID, out ErrMsg);

        //將IQueryable轉成DataTable
        DataTable myDT = fn_CustomUI.LINQToDataTable(query);

        //重新命名欄位標頭
        myDT.Columns["OrderDate"].ColumnName = this.GetLocalResourceObject("txt_Header1").ToString();
        myDT.Columns["ModelNo"].ColumnName = this.GetLocalResourceObject("txt_Header2").ToString();
        myDT.Columns["BuyCnt"].ColumnName = this.GetLocalResourceObject("txt_Header3").ToString();
        myDT.Columns["UnShipCnt"].ColumnName = this.GetLocalResourceObject("txt_Header4").ToString();
        myDT.Columns["UnitPrice"].ColumnName = this.GetLocalResourceObject("txt_Header5").ToString();
        myDT.Columns["UnShipPrice"].ColumnName = this.GetLocalResourceObject("txt_Header6").ToString();
        myDT.Columns["PreShipDate"].ColumnName = this.GetLocalResourceObject("txt_Header7").ToString();
        myDT.Columns["OrderFullID"].ColumnName = this.GetLocalResourceObject("txt_Header8").ToString();

 
        //release
        query = null;

        //匯出Excel
        fn_CustomUI.ExportExcel(
            myDT
            , "DataOutput-{0}.xlsx".FormatThis(DateTime.Now.ToShortDateString().ToDateString("yyyyMMdd")));
    }
}