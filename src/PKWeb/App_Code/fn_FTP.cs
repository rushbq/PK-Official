using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;

/// <summary>
/// fn_FTP 的摘要描述
/// </summary>
public class fn_FTP
{
    #region -- FTP功能 --

    /// <summary>
    /// 從FTP下載檔案
    /// </summary>
    /// <param name="fullPath">完整路徑</param>
    /// <param name="dwFileName">另存新檔的檔名</param>
    public static void FTP_doDownload(string fullPath, string dwFileName)
    {
        try
        {
            //取得FTP協定
            FtpWebRequest dwRequest = (FtpWebRequest)WebRequest.Create(fullPath);

            //完成後,連線關閉
            dwRequest.KeepAlive = false;
            dwRequest.UseBinary = true;

            //method = 下載
            dwRequest.Method = WebRequestMethods.Ftp.DownloadFile;
            dwRequest.Credentials = new NetworkCredential(myFtp_Username, myFtp_Password);

            //dwRequest.Timeout = (60000 * 1);  // (60000 * 1) 一分鐘

            //取得FTP伺服器回應
            using (FtpWebResponse dwResponse = (FtpWebResponse)dwRequest.GetResponse())
            {
                //取得FTP伺服器回傳的資料流
                using (Stream responseStream = dwResponse.GetResponseStream())
                {
                    //取得資料流
                    using (StreamReader reader = new StreamReader(responseStream))
                    {
                        string fileName = Path.GetFileName(dwRequest.RequestUri.AbsolutePath);
                        if (fileName.Length == 0)
                        {
                            throw new Exception(reader.ReadToEnd());
                        }
                        else
                        {
                            int Length = 2048;
                            byte[] buffer = new byte[Length];
                            int read = 0;

                            using (MemoryStream ms = new MemoryStream())
                            {
                                while ((read = responseStream.Read(buffer, 0, Length)) > 0)
                                {
                                    ms.Write(buffer, 0, read);
                                }
                                ms.Flush();
                                HttpContext.Current.Response.Clear();
                                HttpContext.Current.Response.ContentType = "application/octet-stream";

                                // 設定強制下載標頭
                                HttpContext.Current.Response.AddHeader("Content-Disposition", string.Format("attachment; filename=" + dwFileName));
                                // 輸出檔案
                                HttpContext.Current.Response.BinaryWrite(ms.ToArray());
                            }
                        }
                    }

                }

            }


        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    /// <summary>
    /// 判斷檔案是否存在
    /// </summary>
    /// <param name="uploadFolder">資料夾名稱</param>
    /// <param name="fileName">檔案名稱</param>
    /// <returns></returns>
    public static bool FTP_CheckFile(string uploadFolder, string fileName)
    {
        try
        {
            string myUrl = myFtp_ServerUrl + uploadFolder + @"/" + fileName;

            //宣告
            FtpWebRequest ftp = (FtpWebRequest)FtpWebRequest.Create(myUrl);

            //登入
            ftp.Credentials = new NetworkCredential(myFtp_Username, myFtp_Password);

            //取得檔案大小
            ftp.Method = WebRequestMethods.Ftp.GetFileSize;

            //取得FTP回應
            FtpWebResponse myResponse = (FtpWebResponse)ftp.GetResponse();

            string result = string.Empty;

            using (Stream datastream = myResponse.GetResponseStream())
            {
                StreamReader sr = new StreamReader(datastream);
                result = sr.ReadToEnd();
                sr.Close();
            }

            myResponse.Close();

            return true;
        }
        catch (Exception)
        {
            return false;
        }

    }

    /// <summary>
    /// 列出資料夾內的檔案
    /// </summary>
    /// <param name="uploadFolder">資料夾名稱</param>
    /// <returns></returns>
    public static List<string> ListFiles(string uploadFolder)
    {
        string myUrl = myFtp_ServerUrl + uploadFolder;

        //宣告
        FtpWebRequest ftp = (FtpWebRequest)FtpWebRequest.Create(myUrl);

        //登入
        ftp.Credentials = new NetworkCredential(myFtp_Username, myFtp_Password);

        //顯示資料夾內容檔案
        ftp.Method = WebRequestMethods.Ftp.ListDirectory;

        //取得FTP回應
        FtpWebResponse myResponse = (FtpWebResponse)ftp.GetResponse();

        List<string> result = new List<string>();

        using (Stream datastream = myResponse.GetResponseStream())
        {
            StreamReader reader = new StreamReader(datastream);

            while (!reader.EndOfStream)
            {
                result.Add(reader.ReadLine());
            }

            reader.Close();
        }

        return result;
    }

    #endregion

    #region -- FTP參數 --
    /// <summary>
    /// FTP帳號
    /// </summary>
    private static string _myFtp_Username;
    public static string myFtp_Username
    {
        get
        {
            return System.Web.Configuration.WebConfigurationManager.AppSettings["FTP_Username"];
        }
        set
        {
            _myFtp_Username = value;
        }
    }

    /// <summary>
    /// FTP密碼
    /// </summary>
    private static string _myFtp_Password;
    public static string myFtp_Password
    {
        get
        {
            return System.Web.Configuration.WebConfigurationManager.AppSettings["FTP_Password"];
        }
        set
        {
            _myFtp_Password = value;
        }
    }

    /// <summary>
    /// FTP伺服器路徑
    /// </summary>
    private static string _myFtp_ServerUrl;
    public static string myFtp_ServerUrl
    {
        get
        {
            return System.Web.Configuration.WebConfigurationManager.AppSettings["FTP_Url"];
        }
        set
        {
            _myFtp_ServerUrl = value;
        }
    }
    #endregion

}