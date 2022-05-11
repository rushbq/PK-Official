using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using eOrder.Controllers;
using eOrder.Models;
using PKLib_Method.Methods;

public partial class myOrder_Step1 : SecurityCheckDealer
{
    public string ErrMsg;

    //設定FTP連線參數
    private FtpMethod _ftp = new FtpMethod(fn_FTP.myFtp_Username, fn_FTP.myFtp_Password, fn_FTP.myFtp_ServerUrl);

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!IsPostBack)
            {
                //取得資料
                LookupData();

                //Set LocalResources
                this.rb_DataType1.Text = this.GetLocalResourceObject("rdo_自有品號").ToString();
                this.rb_DataType2.Text = this.GetLocalResourceObject("rdo_寶工品號").ToString();
            }


        }
        catch (Exception)
        {

            throw;
        }

    }

    #region -- 資料讀取 --

    /// <summary>
    /// 取得資料
    /// </summary>
    private void LookupData()
    {
        //----- 宣告:資料參數 -----
        eOrderingRepository _data = new eOrderingRepository();
        Dictionary<int, string> search = new Dictionary<int, string>();


        //----- 原始資料:條件篩選 -----
        search.Add((int)mySearch.CustID, fn_Param.Get_CustID);


        //----- 原始資料:取得所有資料 -----
        var query = _data.GetDataList(search)
            .Select(fld => new
            {
                TraceID = fld.TraceID,
                InCompleteID = fld.InCompleteID
            }).FirstOrDefault();

        if (query != null)
        {
            if (string.IsNullOrEmpty(query.InCompleteID))
            {
                this.ph_InComplete.Visible = false;
            }
            else
            {
                this.ph_InComplete.Visible = true;

                //判斷有未完成訂單,顯示URL
                this.lt_UrlStep2.Text = "<a href=\"{0}EO/Step1-1/{1}\" class=\"btn btn-danger\">{2}</a>".FormatThis(
                  Application["WebUrl"]
                  , query.InCompleteID
                  , this.GetLocalResourceObject("tip3").ToString());

                //未完成訂單的編號
                this.hf_OldDataID.Value = query.InCompleteID;
                this.hf_OldTraceID.Value = query.TraceID;
            }
        }

        query = null;

    }


    #endregion


    #region -- 按鈕事件 --

    /// <summary>
    /// 下一步
    /// 建立新訂單, 若有未完成舊訂單, 刪除
    /// </summary>
    protected void btn_Next_Click(object sender, EventArgs e)
    {
        //判斷是否有必填欄位
        HttpPostedFile hpFile = this.file_Upload.PostedFile;
        if (hpFile.ContentLength == 0)
        {
            string chkField = "{0}:{1}".FormatThis(Resources.resPublic.tip_Require, this.GetLocalResourceObject("txt_選擇Excel").ToString());
            CustomExtension.AlertMsg(chkField, "");
            return;
        }

        /*
         * 判斷是否有舊的ID,
         * 刪除檔案及資料
         */
        #region -- 刪除 --
        string lastDataID = this.hf_OldDataID.Value;
        string lastTraceID = this.hf_OldTraceID.Value;
        if (!string.IsNullOrEmpty(this.hf_OldDataID.Value))
        {
            //----- 宣告:資料參數 -----
            eOrderingRepository _data = new eOrderingRepository();

            //----- 方法:刪除資料 -----
            if (false == _data.Delete(lastDataID))
            {
                this.ph_errMessage.Visible = true;
                return;
            }
            else
            {
                //刪除整個Folder檔案
                _ftp.FTP_DelFolder(UploadFolder(lastTraceID));

            }
        }
        #endregion


        //資料處理-建立新資料
        string[] myData = Add_Data();

        //取得回傳參數
        string DataID = myData[0];
        string ProcCode = myData[1];
        string Message = myData[2];

        //判斷是否處理成功
        if (!ProcCode.Equals("200"))
        {
            this.lt_UploadMessage.Text = Message;
            this.ph_errMessage.Visible = true;
            return;
        }
        else
        {
            //導至下一步
            Response.Redirect("{0}EO/Step1-1/{1}".FormatThis(
                Application["WebUrl"]
                , DataID));
            return;
        }

    }

    #endregion


    #region -- 資料編輯 Start --

    /// <summary>
    /// 資料新增
    /// </summary>
    /// <returns></returns>
    private string[] Add_Data()
    {
        //回傳參數初始化
        string DataID = "";
        string ProcCode = "0";
        string Message = "";

        //----- TraceID
        string myTraceID = NewTraceID();

        #region -- 檔案處理 --

        //宣告
        List<IOTempParam> ITempList = new List<IOTempParam>();
        Random rnd = new Random();


        //取得上傳檔案集合
        HttpFileCollection hfc = Request.Files;


        //--- 檔案檢查 ---
        for (int idx = 0; idx <= hfc.Count - 1; idx++)
        {
            //取得個別檔案
            HttpPostedFile hpf = hfc[idx];

            if (hpf.ContentLength > FileSizeLimit)
            {
                //[提示]
                //Message = "檔案大小超出限制, 每個檔案大小限制為 {0} MB".FormatThis(FileSizeLimit);
                Message = "File size limit is {0} MB.".FormatThis(FileSizeLimit);
                return new string[] { DataID, ProcCode, Message };
            }

            if (hpf.ContentLength > 0)
            {
                //取得原始檔名
                string OrgFileName = Path.GetFileName(hpf.FileName);
                //取得副檔名
                string FileExt = Path.GetExtension(OrgFileName).ToLower();
                if (false == CustomExtension.CheckStrWord(FileExt, FileExtLimit, "|", 1))
                {
                    //[提示]
                    //Message = "檔案副檔名不符規定, 僅可上傳副檔名為 {0}".FormatThis(FileExtLimit.Replace("|", ", "));
                    Message = "The extension is limited to {0}.".FormatThis(FileExtLimit.Replace("|", ", "));
                    return new string[] { DataID, ProcCode, Message };
                }
            }
        }


        //--- 檔案暫存List ---
        for (int idx = 0; idx <= hfc.Count - 1; idx++)
        {
            //取得個別檔案
            HttpPostedFile hpf = hfc[idx];

            if (hpf.ContentLength > 0)
            {
                //取得原始檔名
                string OrgFileName = Path.GetFileName(hpf.FileName);
                //取得副檔名
                string FileExt = Path.GetExtension(OrgFileName).ToLower();

                //設定檔名, 重新命名
                string myFullFile = String.Format(@"{0:yyMMddHHmmssfff}{1}{2}"
                    , DateTime.Now
                    , rnd.Next(0, 99)
                    , FileExt);


                //判斷副檔名, 未符合規格的檔案不上傳
                if (CustomExtension.CheckStrWord(FileExt, FileExtLimit, "|", 1))
                {
                    //設定暫存-檔案
                    ITempList.Add(new IOTempParam(myFullFile, OrgFileName, hpf));
                }
            }
        }

        #endregion


        #region -- 儲存檔案 --

        if (ITempList.Count > 0)
        {
            int errCnt = 0;
            //判斷資料夾, 不存在則建立
            _ftp.FTP_CheckFolder(UploadFolder(myTraceID));

            //暫存檔案List
            for (int row = 0; row < ITempList.Count; row++)
            {
                //取得個別檔案
                HttpPostedFile hpf = ITempList[row].Param_hpf;

                //執行上傳
                if (false == _ftp.FTP_doUpload(hpf, UploadFolder(myTraceID), ITempList[row].Param_FileName))
                {
                    errCnt++;
                }
            }

            if (errCnt > 0)
            {
                Message = "檔案上傳失敗, 失敗筆數為 {0} 筆, 請重新整理後再上傳...{1}".FormatThis(errCnt, myTraceID);
                return new string[] { DataID, ProcCode, Message };
            }
        }

        #endregion


        #region -- 資料處理 --

        //----- 宣告:資料參數 -----
        eOrderingRepository _data = new eOrderingRepository();


        //----- 設定:資料欄位 -----
        string guid = CustomExtension.GetGuid();

        var data = new ImportData
        {
            Data_ID = new Guid(guid),
            TraceID = myTraceID,
            CustID = fn_Param.Get_CustID,
            Data_Type = Get_rbType(),
            Upload_File = ITempList[0].Param_FileName
        };


        //----- 方法:建立資料 -----      
        if (!_data.Create(data))
        {
            Message = "資料建立失敗";
            return new string[] { DataID, ProcCode, Message };
        }
        else
        {
            DataID = guid;
            ProcCode = "200";
            return new string[] { DataID, ProcCode, Message };
        }


        #endregion
    }


    /// <summary>
    /// 取得品號來源
    /// </summary>
    /// <returns></returns>
    private decimal Get_rbType()
    {
        return this.rb_DataType1.Checked ? 1 : 2;
    }

    #endregion -- 資料編輯 End --


    #region -- 參數設定 --

    /// <summary>
    /// 當TraceID重複時, 重新產生
    /// </summary>
    /// <returns></returns>
    private string NewTraceID()
    {
        Random myBaseRnd = new Random(Guid.NewGuid().GetHashCode());

        //產生TraceID
        long getTS = Cryptograph.GetCurrentTime();

        //亂數
        int getRnd = myBaseRnd.Next(1, 999);

        //日時分秒
        string getTimeString = DateTime.Now.ToString("ddHHmmss");


        return "{0}{1}".FormatThis(getTS + getRnd, getTimeString);
    }


    /// <summary>
    /// 本站Ref檔案路徑
    /// </summary>
    /// <returns></returns>
    public string refUrl()
    {
        return "{0}{1}".FormatThis(
            System.Web.Configuration.WebConfigurationManager.AppSettings["File_WebUrl"]
            , System.Web.Configuration.WebConfigurationManager.AppSettings["File_Folder"]);
    }


    #endregion


    #region -- 上傳參數 --
    /// <summary>
    /// 上傳目錄(+TraceID)
    /// </summary>
    /// <param name="traceID"></param>
    /// <returns></returns>
    private string UploadFolder(string traceID)
    {
        return "{0}OrderFiles/{1}/".FormatThis(System.Web.Configuration.WebConfigurationManager.AppSettings["File_Folder"], traceID);
    }

    /// <summary>
    /// 限制上傳的副檔名
    /// </summary>
    private string _FileExtLimit;
    public string FileExtLimit
    {
        get
        {
            return "xls|xlsx";
        }
        set
        {
            this._FileExtLimit = value;
        }
    }

    /// <summary>
    /// 限制上傳的檔案大小(1MB = 1024000), 5MB
    /// </summary>
    private int _FileSizeLimit;
    public int FileSizeLimit
    {
        get
        {
            return 5120000;
        }
        set
        {
            this._FileSizeLimit = value;
        }
    }

    /// <summary>
    /// 暫存參數
    /// </summary>
    public class IOTempParam
    {
        /// <summary>
        /// [參數] - 檔名
        /// </summary>
        private string _Param_FileName;
        public string Param_FileName
        {
            get { return this._Param_FileName; }
            set { this._Param_FileName = value; }
        }

        /// <summary>
        /// [參數] -原始檔名
        /// </summary>
        private string _Param_OrgFileName;
        public string Param_OrgFileName
        {
            get { return this._Param_OrgFileName; }
            set { this._Param_OrgFileName = value; }
        }


        private HttpPostedFile _Param_hpf;
        public HttpPostedFile Param_hpf
        {
            get { return this._Param_hpf; }
            set { this._Param_hpf = value; }
        }

        /// <summary>
        /// 設定參數值
        /// </summary>
        /// <param name="Param_FileName">系統檔名</param>
        /// <param name="Param_OrgFileName">原始檔名</param>
        /// <param name="Param_hpf">上傳檔案</param>
        /// <param name="Param_FileKind">檔案類別</param>
        public IOTempParam(string Param_FileName, string Param_OrgFileName, HttpPostedFile Param_hpf)
        {
            this._Param_FileName = Param_FileName;
            this._Param_OrgFileName = Param_OrgFileName;
            this._Param_hpf = Param_hpf;
        }

    }
    #endregion
}