<%@ WebHandler Language="C#" Class="FileDownload" %>

using System;
using System.Web;

public class FileDownload : IHttpHandler
{
    public void ProcessRequest(HttpContext context)
    {
        try
        {
            //[取得參數] - 完整檔案路徑
            string FullFilePath = Cryptograph.Decrypt(HttpContext.Current.Request["FilePath"]).Trim();

            //[取得參數] - 原始檔名
            string OrgFileName = fn_stringFormat.Set_ClearFileName(HttpContext.Current.Request["OrgiName"].Trim());
            OrgFileName = Convert.ToChar(34) + HttpUtility.UrlPathEncode(OrgFileName) + Convert.ToChar(34);

            //呼叫 webclient 方式做檔案下載
            System.Net.WebClient wc = new System.Net.WebClient();

            //判斷站台是否為AD驗證
            string IsAD = System.Web.Configuration.WebConfigurationManager.AppSettings["AD_IsUse"];
            if (IsAD.Equals("Y"))
            {
                wc.UseDefaultCredentials = true;
                wc.Credentials = System.Net.CredentialCache.DefaultCredentials;
            }

            byte[] xfile = null;
            xfile = wc.DownloadData(FullFilePath);

            context.Response.ContentType = "application/octet-stream";  //二進位方式
            context.Response.AddHeader("content-disposition", "attachment;filename=" + OrgFileName);
            
            //輸出檔案
            context.Response.BinaryWrite(xfile);
            context.Response.End();
        }
        catch (Exception)
        {
            throw new Exception("處理失敗");
        }

    }

    public bool IsReusable { get { return false; } }

}