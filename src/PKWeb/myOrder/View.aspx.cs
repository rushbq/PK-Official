using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using eOrder.Controllers;
using eOrder.Models;
using PKLib_Method.Methods;

public partial class myOrder_View : SecurityCheckDealer
{
    public string ErrMsg;


    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!IsPostBack)
            {
                //檢查ID是否為空
                if (string.IsNullOrEmpty(Req_DataID))
                {
                    Response.Redirect(Application["WebUrl"] + "EO/List");
                    return;
                }

                //取得資料
                LookupData();

            }


        }
        catch (Exception)
        {

            throw;
        }

    }

    #region -- 資料讀取 --

    /// <summary>
    /// 取得資料
    /// </summary>
    private void LookupData()
    {
        //----- 宣告:資料參數 -----
        eOrderingRepository _data = new eOrderingRepository();
        Dictionary<int, string> search = new Dictionary<int, string>();


        //----- 原始資料:條件篩選 -----
        search.Add((int)mySearch.DataID, Req_DataID);
        search.Add((int)mySearch.CustID, fn_Param.Get_CustID);


        //----- 原始資料:取得所有資料 -----
        var query = _data.GetDataList(search).Take(1)
            .Select(fld => new
            {
                TraceID = fld.TraceID,
                TotalPrice = fld.TotalPrice,
                Staus = fld.Status

            }).FirstOrDefault();


        //Check Data
        if (query == null)
        {
            //No data ...redirect
            Response.Redirect(Application["WebUrl"] + "EO/List");
            return;
        }

        //載入單身資料
        LookupDetailData();


        //----- 資料整理:填入資料 -----
        this.lt_TraceID.Text = query.TraceID;
        this.lt_TotalPrice.Text = fn_stringFormat.Money_Format(query.TotalPrice.ToString());
        

        query = null;

    }

    private void LookupDetailData()
    {
        //----- 宣告:資料參數 -----
        eOrderingRepository _data = new eOrderingRepository();


        //----- 原始資料:取得所有資料 -----
        var query = _data.GetDetailList(Req_DataID);

        //----- 資料繫結 -----
        this.lvDataList.DataSource = query;
        this.lvDataList.DataBind();


        //填入表頭的多語系文字
        ((Literal)this.lvDataList.FindControl("lt_Header1")).Text = this.GetLocalResourceObject("txt_Header1").ToString();
        ((Literal)this.lvDataList.FindControl("lt_Header2")).Text = this.GetLocalResourceObject("txt_Header2").ToString();
        ((Literal)this.lvDataList.FindControl("lt_Header3")).Text = this.GetLocalResourceObject("txt_Header3").ToString();
        ((Literal)this.lvDataList.FindControl("lt_Header4")).Text = this.GetLocalResourceObject("txt_Header4").ToString();
        ((Literal)this.lvDataList.FindControl("lt_Header5")).Text = this.GetLocalResourceObject("txt_Header5").ToString();
        ((Literal)this.lvDataList.FindControl("lt_Header6")).Text = this.GetLocalResourceObject("txt_Header6").ToString();
        ((Literal)this.lvDataList.FindControl("lt_Header7")).Text = this.GetLocalResourceObject("txt_Header7").ToString();
    }

    #endregion


    #region -- 參數設定 --

    /// <summary>
    /// 設定參數 - 取得DataID
    /// </summary>
    public string Req_DataID
    {
        get
        {
            String data = Convert.ToString(Page.RouteData.Values["DataID"]);

            return string.IsNullOrEmpty(data) ? "" : data.ToString();
        }
        set
        {
            this._Req_DataID = value;
        }
    }
    private string _Req_DataID;

    #endregion

}