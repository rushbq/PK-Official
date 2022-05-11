<%@ WebHandler Language="C#" Class="FileDownload" %>

using System;
using System.Web;
using ExtensionMethods;

public class FileDownload : System.Web.UI.Page, System.Web.SessionState.IReadOnlySessionState, IHttpHandler
{
    public void ProcessRequest(HttpContext context)
    {
        try
        {
            //取得參數
            var routeValues = context.Request.RequestContext.RouteData.Values;

            //Check Token
            if (!Token.Equals(routeValues["token"].ToString()))
            {
                throw new HttpException(401, "Unauthorized");
            }

            //[取得參數] - 完整檔案路徑
            string FullFilePath = Cryptograph.MD5Decrypt(routeValues["Path"].ToString(), context.Application["DesKey"].ToString());

            //[取得參數] - 下載檔名
            string DwFileName = Cryptograph.MD5Decrypt(routeValues["Name"].ToString(), context.Application["DesKey"].ToString());

            //呼叫 webclient 方式做檔案下載
            System.Net.WebClient wc = new System.Net.WebClient();

            byte[] xfile = null;
            xfile = wc.DownloadData(FullFilePath);

            context.Response.ContentType = "application/octet-stream";  //二進位方式
            context.Response.AddHeader("content-disposition", "attachment;filename=" + DwFileName);

            //輸出檔案
            context.Response.BinaryWrite(xfile);
            context.Response.End();
        }
        catch (Exception)
        {
            throw new HttpException(404, "HTTP/1.1 404 Not Found");
        }

    }

    public bool IsReusable { get { return false; } }

    /// <summary>
    /// 依不同身份產生token
    /// </summary>
    private string _Token;
    public string Token
    {
        get
        {
            return fn_Extensions.Get_MemberToken(fn_Param.MemberType, fn_Param.MemberID);
        }
        set
        {
            this._Token = value;
        }
    }

}