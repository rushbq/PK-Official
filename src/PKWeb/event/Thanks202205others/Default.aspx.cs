using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ExtensionMethods;

public partial class event_Default : System.Web.UI.Page
{
    public string eventFolder = "Thanks202205others";
    public string eventTitle = "寶工感恩月登錄抽獎活動";

    protected void Page_Load(object sender, EventArgs e)
    {
        //隱藏主頁的meta
        PlaceHolder myMeta = (PlaceHolder)Master.FindControl("ph_MetaInfo");
        myMeta.Visible = false;

    }

    protected void btn_Go_Click(object sender, EventArgs e)
    {
        try
        {
            //取得當下的TS
            string GetNowTS = Cryptograph.GetCurrentTime().ToString();
            //轉成MD5
            string EncTS = Cryptograph.MD5Encrypt(GetNowTS, Application["DesKey"].ToString());

            //產生Url
            string url = "{0}event/{1}/EventReg.aspx?token={2}&utm_source={3}&utm_medium={4}&utm_campaign={5}".FormatThis(
                 Application["WebUrl"].ToString()
                 , eventFolder
                 , EncTS
                 , Req_utm_source
                 , Req_utm_medium
                 , Req_utm_campaign
                );

            //Go Url
            Response.Redirect(url);

        }
        catch (Exception)
        {

            throw;
        }
    }
    /*
     utm_source=BN
     utm_medium=Eclife
     utm_campaign=202205thanks
     */

    ///// <summary>
    ///// 取得傳遞參數 - 來源平台
    ///// </summary>
    //private string _Req_FromWhere;
    //public string Req_FromWhere
    //{
    //    get
    //    {
    //        string GetReqVal = string.IsNullOrWhiteSpace(Request.QueryString["from"])
    //            ? "Web"
    //            : fn_stringFormat.Set_FilterHtml(Request.QueryString["from"]);

    //        return GetReqVal;
    //    }
    //    set
    //    {
    //        this._Req_FromWhere = value;
    //    }
    //}


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
}