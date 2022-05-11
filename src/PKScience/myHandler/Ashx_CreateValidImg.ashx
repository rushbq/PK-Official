<%@ WebHandler Language="C#" Class="Create_ValidImg" %>

using System;
using System.Web;

public class Create_ValidImg : IHttpHandler, System.Web.SessionState.IReadOnlySessionState
{

    public void ProcessRequest(HttpContext context)
    {
        fn_ValidImg.DrawImage();
    }

    public bool IsReusable
    {
        get
        {
            return false;
        }
    }

}