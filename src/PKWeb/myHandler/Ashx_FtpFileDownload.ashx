<%@ WebHandler Language="C#" Class="Ashx_FtpFileDownload" %>

using System;
using System.Web;
using ExtensionMethods;

public class Ashx_FtpFileDownload : IHttpHandler, System.Web.SessionState.IReadOnlySessionState
{

    public void ProcessRequest(HttpContext context)
    {
        try
        {
            //取得參數
            string dwFolder = context.Request["f"].Trim();
            string realFile = context.Request["r"].Trim();
            string dwFileName = context.Request["d"].Trim();
            string getToken = context.Request["t"].Trim();

            //檢查Token
            if (!Token.Equals(getToken))
            {
                throw new HttpException(401, "Unauthorized");
            }

            //ftp路徑 + folder + 檔名
            fn_FTP.FTP_doDownload("{0}{1}/{2}".FormatThis(
                    Ftp_Url
                    , Cryptograph.MD5Decrypt(dwFolder, DesKey)
                    , Cryptograph.MD5Decrypt(realFile, DesKey)
                 ), dwFileName);

        }
        catch (Exception)
        {
            throw new HttpException(404, "HTTP/1.1 404 Not Found");
        }

    }

    public bool IsReusable { get { return false; } }

    /// <summary>
    /// FTP路徑
    /// </summary>
    private string _Ftp_Url;
    public string Ftp_Url
    {
        get
        {
            return System.Web.Configuration.WebConfigurationManager.AppSettings["FTP_Url"];
        }
        set
        {
            this._Ftp_Url = value;
        }
    }

    /// <summary>
    /// DesKey
    /// </summary>
    private string _DesKey;
    public string DesKey
    {
        get
        {
            return HttpContext.Current.Application["DesKey"].ToString();
        }
        set
        {
            this._DesKey = value;
        }
    }

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