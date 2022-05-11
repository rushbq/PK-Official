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
                        //成功
                        this.ph_message1.Visible = true;
                        break;

                    case "2":
                        //失敗
                        this.ph_message2.Visible = true;
                        break;

                    case "3":
                        //產品註冊成功
                        this.ph_message3.Visible = true;
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
            String DataID = Page.RouteData.Values["DataID"].ToString();

            return string.IsNullOrEmpty(DataID) ? "" : DataID;
        }
        set
        {
            this._Req_DataID = value;
        }
    }

    #endregion
}