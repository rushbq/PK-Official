using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class myInfo_Profile : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!IsPostBack)
            {
                //** 次標題 **
                this.Page.Title = Resources.resPublic.title_關於寶工;
                Ascx_Navi1.Param_CustomName = Resources.resPublic.txt_CompanyName;
            }

        }
        catch (Exception)
        {

            throw;
        }
    }
}