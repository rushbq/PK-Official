using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class error : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        Response.StatusCode = 404;

        //導向錯誤顯示頁
        Response.Redirect(Application["WebUrl"] + "myExp/?u=" +
            Cryptograph.MD5Encrypt(Request.Url.AbsoluteUri, Application["DesKey"].ToString())
            );

    }
}