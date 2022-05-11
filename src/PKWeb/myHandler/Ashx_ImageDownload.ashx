<%@ WebHandler Language="C#" Class="ImageDownload" %>

using System;
using System.Web;

public class ImageDownload : IHttpHandler
{
    public void ProcessRequest(HttpContext context)
    {
        try
        {
            //[取得參數] - 完整檔案路徑
            string FullFilePath = System.Web.Configuration.WebConfigurationManager.AppSettings["File_WebUrl"]
                + Cryptograph.Decrypt(context.Request["FilePath"]).Trim();

            //呼叫 webclient 方式做檔案下載
            System.Net.WebClient wc = new System.Net.WebClient();
            byte[] xfile = null;
            xfile = wc.DownloadData(FullFilePath);

            context.Response.ContentType = "image/jpg";
            
            //輸出檔案
            context.Response.BinaryWrite(xfile);
            context.Response.End();
        }
        catch (Exception)
        {
            //throw new Exception("參數錯誤.");
        }

    }

    public bool IsReusable { get { return false; } }

}