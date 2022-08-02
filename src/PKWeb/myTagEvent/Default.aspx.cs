using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class myTagEvent_Default : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(Req_EventName))
        {
            Response.Redirect(fn_Param.WebUrl);
        }
    }

    /// <summary>
    /// 取得傳遞參數 - Event Name
    /// </summary>
    private string _Req_EventName;
    public string Req_EventName
    {
        get
        {
            var eventName = Page.RouteData.Values["eventName"];
            String DataID = (eventName == null ? "" : eventName.ToString());

            return DataID;
        }
        set
        {
            this._Req_EventName = value;
        }
    }

    public string EventUrl
    {
        get
        {
            switch (Req_EventName.ToUpper())
            {
                case "POWER":
                    return fn_Param.WebUrl + "event/power/index.html";

                default:
                    return "";
            }
        }
    }
}