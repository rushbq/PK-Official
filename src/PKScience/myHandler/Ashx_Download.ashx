<%@ WebHandler Language="C#" Class="Ashx_Download" %>

using System;
using System.Web;
using PKLib_Method.Methods;

public class Ashx_Download : IHttpHandler, System.Web.SessionState.IReadOnlySessionState
{

    public void ProcessRequest(HttpContext context)
    {
        try
        {
            //取得參數
            string dwFolder = context.Request["f"].Trim();
            string realFile = context.Request["r"].Trim();
            string dwFileName = context.Request["d"].Trim();

            //檢查NULL
            if (string.IsNullOrEmpty(dwFolder) || string.IsNullOrEmpty(realFile) || string.IsNullOrEmpty(dwFileName))
            {
                throw new HttpException(401, "Unauthorized");
            }

            //設定FTP連線參數
            FtpMethod _ftp = new FtpMethod(Ftp_UserName, Ftp_UserPwd, Ftp_Url);

            //ftp路徑 + folder + 檔名
            _ftp.FTP_doDownload(dwFolder, realFile, dwFileName);

            _ftp = null;


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
    private string Ftp_Url
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
    /// FTP UserName
    /// </summary>
    private string _Ftp_UserName;
    private string Ftp_UserName
    {
        get
        {
            return System.Web.Configuration.WebConfigurationManager.AppSettings["FTP_Username"];
        }
        set
        {
            this._Ftp_UserName = value;
        }
    }

    /// <summary>
    /// FTP UserPwd
    /// </summary>
    private string _Ftp_UserPwd;
    private string Ftp_UserPwd
    {
        get
        {
            return System.Web.Configuration.WebConfigurationManager.AppSettings["FTP_Password"];
        }
        set
        {
            this._Ftp_UserPwd = value;
        }
    }

}