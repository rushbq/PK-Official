using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Message : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!IsPostBack)
            {
                //** 次標題 **
                this.Page.Title = Resources.resPublic.title_訊息通知;

                switch (Req_DataID)
                {
                    case "1":
                        //註冊失敗
                        this.ph_message1.Visible = true;
                        break;


                    case "2":
                        //註冊成功
                        this.ph_message2.Visible = true;
                        break;

                    case "3":
                        //密碼變更成功
                        this.ph_message3.Visible = true;
                        break;

                    case "4":
                        //密碼變更失敗
                        this.ph_message4.Visible = true;
                        break;

                    case "5":
                        //驗證成功
                        this.ph_message5.Visible = true;
                        break;

                    case "6":
                        //登入失敗
                        this.ph_message6.Visible = true;
                        break;

                    case "7":
                        //補發驗證信
                        this.ph_message7.Visible = true;
                        break;

                    case "8":
                        //資料修改成功
                        this.ph_message8.Visible = true;
                        break;

                    case "9":
                        //資料修改失敗
                        this.ph_message9.Visible = true;
                        break;

                    case "10":
                        //授權失敗
                        this.ph_message10.Visible = true;
                        break;

                    case "11":
                        //經銷商申請成功
                        this.ph_message11.Visible = true;
                        break;

                    case "12":
                        //經銷商申請失敗
                        this.ph_message12.Visible = true;
                        break;

                    case "13":
                        //帳號已使用
                        this.ph_message13.Visible = true;
                        break;


                    case "14":
                        //Eclife會員轉換成功
                        this.ph_message14.Visible = true;
                        break;

                    case "15":
                        //Eclife會員轉換失敗
                        this.ph_message15.Visible = true;
                        break;


                    case "99":
                        //驗證碼過期
                        this.ph_message99.Visible = true;
                        break;

                    default:
                        this.ph_message.Visible = true;
                        break;
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
    /// 取得傳遞參數 - 活動code
    /// </summary>
    private string _Req_Code;
    public string Req_Code
    {
        get
        {
            string GetCode = fn_stringFormat.Set_FilterHtml(Request.QueryString["code"]);

            return GetCode;
        }
        set
        {
            this._Req_Code = value;
        }
    }
    #endregion
}