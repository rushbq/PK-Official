<%@ WebHandler Language="C#" Class="Ashx_DealerDownload" %>

using System;
using System.Web;
using ExtensionMethods;

public class Ashx_DealerDownload : IHttpHandler, System.Web.SessionState.IReadOnlySessionState
{

    public void ProcessRequest(HttpContext context)
    {
        try
        {
            //取得參數
            string dwFolder = context.Request["f"].Trim();  //資料夾
            string realFile = context.Request["r"].Trim();  //真實檔名
            string dwFileName = context.Request["d"].Trim();    //下載檔名
            string getToken = context.Request["t"].Trim();  //token

            //檢查Token
            if (!Token.Equals(getToken))
            {
                throw new HttpException(401, "Unauthorized");
            }

            //file路徑 + folder + 檔名
            string FullFilePath = "{0}{1}/{2}".FormatThis(
                    System.Web.Configuration.WebConfigurationManager.AppSettings["File_WebUrl"]
                    , dwFolder
                    , realFile
                );


            //呼叫 webclient 方式做檔案下載
            System.Net.WebClient wc = new System.Net.WebClient();

            byte[] xfile = null;
            xfile = wc.DownloadData(FullFilePath);

            context.Response.ContentType = "application/octet-stream";  //二進位方式
            context.Response.AddHeader("content-disposition", "attachment;filename=" + HttpUtility.UrlEncode(dwFileName));

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

    ///// <summary>
    ///// FTP路徑
    ///// </summary>
    //private string _Ftp_Url;
    //public string Ftp_Url
    //{
    //    get
    //    {
    //        return System.Web.Configuration.WebConfigurationManager.AppSettings["FTP_Url"];
    //    }
    //    set
    //    {
    //        this._Ftp_Url = value;
    //    }
    //}

    ///// <summary>
    ///// DesKey
    ///// </summary>
    //private string _DesKey;
    //public string DesKey
    //{
    //    get
    //    {
    //        return HttpContext.Current.Application["DesKey"].ToString();
    //    }
    //    set
    //    {
    //        this._DesKey = value;
    //    }
    //}

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