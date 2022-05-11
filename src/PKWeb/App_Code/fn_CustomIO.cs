using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace ExtensionIO
{
    /// <summary>
    /// 自訂的IO處理
    /// </summary>
    public class fn_CustomIO
    {
        /// <summary>
        /// IO - 判斷目標資料夾是否存在
        /// </summary>
        /// <param name="folder">目標資料夾</param>
        /// <returns>bool</returns>
        public static bool CheckFolder(string folder)
        {
            try
            {
                DirectoryInfo CheckFolder = new DirectoryInfo(folder);
                if (CheckFolder.Exists == false)
                    CheckFolder.Create();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }


    public static class IOManage
    {
        //副檔名
        private static string _FileExtend;
        public static string FileExtend
        {
            get;
            private set;
        }
        //檔案名稱 - 原始檔名
        private static string _FileFullName;
        public static string FileFullName
        {
            get;
            private set;
        }
        //檔案名稱 - 系統命名
        private static string _FileNewName;
        public static string FileNewName
        {
            get;
            private set;
        }
        //檔案真實路徑
        private static string _FileRealName;
        private static string FileRealName
        {
            get;
            set;
        }
        //處理訊息
        private static string _Message;
        public static string Message
        {
            get;
            private set;
        }

        private static int idx = 0;

        /// <summary>
        /// 取得相關檔案名稱
        /// </summary>
        /// <param name="hpFile">FileUpload</param>
        public static void GetFileName(HttpPostedFile hpFile)
        {
            try
            {
                if (hpFile.ContentLength != 0)
                {
                    //[IO] - 檔案真實路徑
                    FileRealName = hpFile.FileName;
                    //[IO] - 取得副檔名(.xxx)
                    FileExtend = Path.GetExtension(FileRealName);
                    //[IO] - 檔案重新命名
                    idx += 1;
                    FileNewName = String.Format("{0:yyyyMMddHHmmssfff}", DateTime.Now) + Convert.ToString(idx) + FileExtend;
                    //[IO] - 取得完整檔名
                    FileFullName = Path.GetFileName(FileRealName);

                    Message = "OK";
                }
                else
                {
                    FileExtend = null;
                    FileFullName = null;
                    FileNewName = null;
                    FileRealName = null;
                    Message = "";
                }
            }
            catch (Exception)
            {
                Message = "系統發生錯誤 - GetFileName";
            }
        }

        /// <summary>
        /// 儲存檔案
        /// </summary>
        /// <param name="hpFile">FileUpload</param>
        /// <param name="FileFolder">資料夾路徑</param>
        /// <param name="newFileName">檔案名稱</param>
        public static void Save(HttpPostedFile hpFile, string FileFolder, string newFileName)
        {
            try
            {
                if (string.IsNullOrEmpty(newFileName) == false || hpFile.ContentLength != 0)
                {
                    //判斷資料夾是否存在
                    if (fn_CustomIO.CheckFolder(FileFolder))
                    {
                        hpFile.SaveAs(FileFolder + newFileName);
                        Message = "OK";
                    }
                    else
                    {
                        Message = "資料夾無法建立，檔案上傳失敗。";
                    }
                }
                else
                {
                    Message = "";
                }
            }
            catch (Exception)
            {
                Message = "系統發生錯誤 - Save";
            }

        }

        /// <summary>
        /// 儲存檔案, 使用縮圖
        /// </summary>
        /// <param name="hpFile">FileUpload</param>
        /// <param name="FileFolder">資料夾路徑</param>
        /// <param name="newFileName">檔案名稱</param>
        /// <param name="intWidth">指定寬度</param>
        /// <param name="intHeight">指定高度</param>
        public static void Save(HttpPostedFile hpFile, string FileFolder, string newFileName, int intWidth, int intHeight)
        {
            try
            {
                if (string.IsNullOrEmpty(newFileName) == false || hpFile.ContentLength != 0)
                {
                    string fileUrl = FileFolder + newFileName;

                    //判斷資料夾是否存在
                    if (fn_CustomIO.CheckFolder(FileFolder))
                    {
                        //儲存原始圖檔
                        hpFile.SaveAs(fileUrl);
                        //產生縮圖並覆蓋原始圖檔
                        renderThumb(fileUrl, fileUrl, intWidth, intHeight);

                        Message = "OK";
                    }
                    else
                    {
                        Message = "資料夾無法建立，檔案上傳失敗。";
                    }
                }
                else
                {
                    Message = "";
                }
            }
            catch (Exception)
            {
                Message = "系統發生錯誤 - Save";
            }

        }

        /// <summary>
        /// 產生並儲存縮圖
        /// </summary>
        /// <param name="inputImg">來源圖檔路徑(磁碟路徑)</param>
        /// <param name="outputImg">輸出圖檔路徑(磁碟路徑)</param>
        /// <param name="w">寬</param>
        /// <param name="h">高</param>
        /// <returns></returns>
        /// <remarks></remarks>
        private static void renderThumb(string inputImg, string outputImg, int w, int h)
        {
            int width = 0;
            int height = 0;

            System.Drawing.Image image = new System.Drawing.Bitmap(inputImg);

            //取得圖檔寬高
            width = image.Width;
            height = image.Height;

            //重新設定寬高 (等比例)
            if (!(width < w & height < h))
            {
                if (width > height)
                {
                    h = w * height / width;
                }
                else
                {
                    w = h * width / height;
                }
            }
            else
            {
                h = height;
                w = width;
            }

            //產生縮圖
            System.Drawing.Bitmap img = new System.Drawing.Bitmap(w, h);
            System.Drawing.Graphics graphic = System.Drawing.Graphics.FromImage(img);
            //將品質設定為HighQuality
            graphic.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            graphic.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            graphic.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            //重畫縮圖
            graphic.DrawImage(image, 0, 0, w, h);

            image.Dispose();

            //輸出縮圖, 格式為Png
            img.Save(outputImg, System.Drawing.Imaging.ImageFormat.Png);

            img.Dispose();
            graphic.Dispose();

        }

        /// <summary>
        /// 刪除檔案
        /// </summary>
        /// <param name="FileFolder">資料夾路徑</param>
        /// <param name="oldFileName">檔案名稱</param>
        public static void DelFile(string FileFolder, string oldFileName)
        {
            try
            {
                if (string.IsNullOrEmpty(FileFolder) || string.IsNullOrEmpty(oldFileName))
                {
                    Message = "傳入參數空白";
                    return;
                }
                FileInfo FileDelete = new FileInfo(FileFolder + oldFileName);
                if (FileDelete.Exists)
                    FileDelete.Delete();

                Message = "OK";
            }
            catch (Exception)
            {
                Message = "系統發生錯誤 - DelFile";
            }
        }

        /// <summary>
        /// 刪除資料夾
        /// </summary>
        /// <param name="FileFolder">資料夾名稱</param>
        public static void DelFolder(string FileFolder)
        {
            try
            {
                if (string.IsNullOrEmpty(FileFolder))
                {
                    Message = "傳入參數空白";
                    return;
                }

                string[] strTemp = null;
                int idx = 0;
                // 刪除檔案
                strTemp = Directory.GetFiles(FileFolder);
                for (idx = 0; idx < strTemp.Length; idx++)
                {
                    if (File.Exists(strTemp[idx]))
                        File.Delete(strTemp[idx]);
                }
                // 刪除子目錄
                strTemp = Directory.GetDirectories(FileFolder);
                for (idx = 0; idx < strTemp.Length; idx++)
                {
                    //呼叫 DelFolder
                    DelFolder(strTemp[idx]);
                }
                // 刪除該目錄
                System.IO.Directory.Delete(FileFolder);

                Message = "OK";
            }
            catch (Exception)
            {
                Message = "系統發生錯誤 - DelFolder";
            }
        }
    }

}
