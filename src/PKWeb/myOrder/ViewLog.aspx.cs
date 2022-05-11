using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using eOrder.Controllers;
using eOrder.Models;
using PKLib_Method.Methods;

public partial class myOrder_ViewLog : SecurityCheckDealer
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
        LookupData_Log();


        //----- 資料整理:填入資料 -----
        this.lt_TraceID.Text = query.TraceID;
        

        query = null;

    }

    /// <summary>
    /// Log
    /// </summary>
    private void LookupData_Log()
    {
        //----- 宣告:資料參數 -----
        eOrderingRepository _data = new eOrderingRepository();


        //----- 原始資料:取得基本資料 -----
        var query = _data.GetLogList(Req_DataID);


        //----- 資料整理:繫結 ----- 
        this.lv_LogList.DataSource = query;
        this.lv_LogList.DataBind();


        //release
        query = null;

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