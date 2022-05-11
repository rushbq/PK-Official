using System;
using System.Web;
using System.Collections.Generic;
using System.Collections;
using System.Security.Principal;
using System.Collections.Specialized;
using ExtensionMethods;

public class SecurityCheckDealer : System.Web.UI.Page
{
    protected override void OnLoad(System.EventArgs e)
    {
        try
        {
            //[檢查參數] 會員編號是否為空 / 身份是否為經銷商(經銷商=1)
            if (string.IsNullOrEmpty(fn_Param.MemberID) || !fn_Param.MemberType.Equals("1"))
            {
                //清除Session
                Session.Clear();

                //導向登入頁
                Response.Redirect("{0}Login?u={1}".FormatThis(
                    Application["WebUrl"].ToString()
                    , Cryptograph.MD5Encrypt(Request.Url.AbsoluteUri, Application["DesKey"].ToString())
                    ));

            }
            else
            {
                base.OnLoad(e);
            }
        }
        catch (Exception)
        {
            throw;
        }

    }

}