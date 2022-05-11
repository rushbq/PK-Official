using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ExtensionIO;
using ExtensionMethods;
using ExtensionUI;


public partial class Promo_Edit_Sub : SecurityCheck
{
    public string ErrMsg;
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!IsPostBack)
            {
                //[權限判斷]
                if (fn_CheckAuth.CheckAuth("320", out ErrMsg) == false)
                {
                    Response.Redirect("{0}401.aspx".FormatThis(Application["WebUrl"]));
                    return;
                }

                //** 設定程式編號(重要) **
                if (false == setProgIDs.setID(this.Master, "300", "320"))
                {
                    throw new Exception("目前的Masterpage 沒有實作 IProgID,無法傳值");
                }

                //[必要參數判斷]
                if (string.IsNullOrEmpty(Param_parentID))
                {
                    fn_Extensions.JsAlert("你不乖!快去吃乖乖", "script:history.back(-1);");
                    return;
                }

                //帶出基本資料
                LookupData_Base();

                //[參數判斷] - 判斷是否有資料編號
                if (!string.IsNullOrEmpty(Param_thisID))
                {
                    LookupData();

                    //顯示區塊
                    this.ph_NewsBlock.Visible = true;
                }

            }
        }
        catch (Exception)
        {

            throw;
        }

    }

    #region -- 資料顯示 --
    /// <summary>
    /// 基本資料顯示
    /// </summary>
    private void LookupData_Base()
    {
        try
        {
            //[取得資料] - 取得資料
            using (SqlCommand cmd = new SqlCommand())
            {
                //宣告
                StringBuilder SBSql = new StringBuilder();

                //清除參數
                cmd.Parameters.Clear();

                //[SQL] - 資料查詢
                SBSql.AppendLine(" SELECT GP.Group_Name");
                SBSql.AppendLine("  , (SELECT LangName FROM PKSYS.dbo.Param_Language WHERE (LOWER(LangCode) = LOWER(@LangCode))) AS Lang_Name");
                SBSql.AppendLine(" FROM  Promo_Group GP ");
                SBSql.AppendLine(" WHERE (GP.Group_ID = @parentID)");
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.AddWithValue("parentID", Param_parentID);
                cmd.Parameters.AddWithValue("LangCode", Param_LangCode);
                using (DataTable DT = dbConn.LookupDT(cmd, out ErrMsg))
                {
                    if (DT.Rows.Count == 0)
                    {
                        fn_Extensions.JsAlert("查無資料！", Page_LastUrl);
                        return;
                    }
                    else
                    {
                        //[填入資料]
                        this.lt_NavSubject.Text = DT.Rows[0]["Group_Name"].ToString();
                        this.lb_Group_Name.Text = DT.Rows[0]["Group_Name"].ToString();
                        this.lb_Lang.Text = DT.Rows[0]["Lang_Name"].ToString();
                    }
                }
            }
        }
        catch (Exception)
        {
            throw new Exception("系統發生錯誤 - 基本資料顯示");
        }
    }

    /// <summary>
    /// 資料顯示
    /// </summary>
    private void LookupData()
    {
        try
        {
            //[取得資料] - 取得資料
            using (SqlCommand cmd = new SqlCommand())
            {
                //宣告
                StringBuilder SBSql = new StringBuilder();

                //清除參數
                cmd.Parameters.Clear();

                //[SQL] - 資料查詢
                SBSql.AppendLine(" SELECT Base.Promo_ID, Base.Promo_Title, Base.Promo_Desc, Base.Promo_Icon, Base.Promo_Pic");
                SBSql.AppendLine(" FROM Promo Base ");
                SBSql.AppendLine(" WHERE (Base.Promo_ID = @DataID)");
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.AddWithValue("DataID", Param_thisID);
                using (DataTable DT = dbConn.LookupDT(cmd, out ErrMsg))
                {
                    if (DT.Rows.Count == 0)
                    {
                        fn_Extensions.JsAlert("查無資料！", Page_LastUrl);
                        return;
                    }
                    else
                    {
                        //[填入資料]
                        this.tb_Promo_Title.Text = DT.Rows[0]["Promo_Title"].ToString();
                        this.tb_Promo_Desc.Text = DT.Rows[0]["Promo_Desc"].ToString();

                        //檔案Url - 小圖
                        string myFile = DT.Rows[0]["Promo_Icon"].ToString();
                        this.hf_OldFile.Value = myFile;
                        if (!string.IsNullOrEmpty(myFile))
                        {
                            this.lt_dwUrl.Text = "<a href=\"{0}\" class=\"btn btn-info zoomPic\" data-gall=\"singlePic\" title=\"\"><i class=\"fa fa-eye\"></i></a>".FormatThis(
                                Param_WebFolder + myFile);

                            this.ph_files.Visible = true;
                        }

                        //檔案Url - 大圖
                        string myFile_Big = DT.Rows[0]["Promo_Pic"].ToString();
                        this.hf_OldFile_Big.Value = myFile_Big;
                        if (!string.IsNullOrEmpty(myFile_Big))
                        {
                            this.lt_dwUrl_Big.Text = "<a href=\"{0}\" class=\"btn btn-info zoomPic\" data-gall=\"singlePic\" title=\"\"><i class=\"fa fa-eye\"></i></a>".FormatThis(
                                Param_WebFolder + myFile_Big);

                            this.ph_files_Big.Visible = true;
                        }

                        //Flag設定 & 欄位顯示/隱藏
                        this.hf_flag.Value = "Edit";


                        //顯示區塊
                        LookupData_Block();
                    }
                }
            }
        }
        catch (Exception)
        {
            throw new Exception("系統發生錯誤 - 資料顯示");
        }
    }

    /// <summary>
    /// 區塊資料顯示
    /// </summary>
    private void LookupData_Block()
    {
        try
        {
            using (SqlCommand cmd = new SqlCommand())
            {
                //[SQL] - 清除參數設定
                cmd.Parameters.Clear();

                //[SQL] - 資料查詢
                StringBuilder SBSql = new StringBuilder();
                SBSql.AppendLine(" SELECT Base.Block_ID, Base.Block_Desc, Base.Block_Pic, Base.Sort ");
                SBSql.AppendLine("  , Base.Model_No, Prod.Model_Name_{0} AS ModelName".FormatThis(Param_LangCode.Replace("-", "_")));
                SBSql.AppendLine(" FROM Promo_Block Base");
                SBSql.AppendLine("  INNER JOIN ProductCenter.dbo.Prod_Item Prod ON UPPER(Base.Model_No) = UPPER(RTRIM(Prod.Model_No))");
                SBSql.AppendLine(" WHERE (Base.Promo_ID = @DataID) ");
                SBSql.AppendLine(" ORDER BY Base.Sort, Base.Block_ID ASC ");
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.AddWithValue("DataID", Param_thisID);
                using (DataTable DT = dbConn.LookupDT(cmd, out ErrMsg))
                {
                    if (DT.Rows.Count == 0)
                    {
                        this.btn_SaveBlock1.Visible = false;
                        this.btn_SaveBlock2.Visible = false;
                    }

                    //DataBind            
                    this.lvDataList.DataSource = DT.DefaultView;
                    this.lvDataList.DataBind();
                }
            }
        }
        catch (Exception)
        {
            throw new Exception("系統發生錯誤 - 區塊資料！");
        }
    }


    protected void lvDataList_ItemCommand(object sender, ListViewCommandEventArgs e)
    {
        try
        {
            if (e.Item.ItemType == ListViewItemType.DataItem)
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    StringBuilder SBSql = new StringBuilder();
                    cmd.Parameters.Clear();

                    //[取得參數] - 編號
                    string GetDataID = ((HiddenField)e.Item.FindControl("hf_DataID")).Value;
                    //[取得參數] - 檔案名稱
                    string GetThisFile = ((HiddenField)e.Item.FindControl("hf_OldFile")).Value;

                    //[SQL] - 刪除資料
                    SBSql.AppendLine(" DELETE FROM Promo_Block WHERE (Promo_ID = @DataID) AND (Block_ID = @Param_ID) ");
                    cmd.CommandText = SBSql.ToString();
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("DataID", Param_thisID);
                    cmd.Parameters.AddWithValue("Param_ID", GetDataID);
                    if (dbConn.ExecuteSql(cmd, out ErrMsg) == false)
                    {
                        fn_Extensions.JsAlert("刪除失敗！", "");
                    }
                    else
                    {
                        //刪除檔案
                        IOManage.DelFile(Param_FileFolder, GetThisFile);

                        //頁面跳至本頁
                        Response.Redirect(Page_CurrentUrl);
                    }
                }
            }
        }
        catch (Exception)
        {

            throw;
        }

    }

    protected void lvDataList_ItemDataBound(object sender, ListViewItemEventArgs e)
    {
        try
        {
            if (e.Item.ItemType == ListViewItemType.DataItem)
            {
                ListViewDataItem dataItem = (ListViewDataItem)e.Item;

                //取得參數資料
                string myFile = DataBinder.Eval(dataItem.DataItem, "Block_Pic").ToString();
                string myCont = DataBinder.Eval(dataItem.DataItem, "Block_Desc").ToString();
                string myModelNo = DataBinder.Eval(dataItem.DataItem, "Model_No").ToString();
                string myModelName = DataBinder.Eval(dataItem.DataItem, "ModelName").ToString();

                //取得控制項
                Literal lt_FileThumb = (Literal)e.Item.FindControl("lt_FileThumb");
                Literal lt_Desc = (Literal)e.Item.FindControl("lt_Desc");

                if (!string.IsNullOrEmpty(myFile))
                {
                    //下載路徑 
                    string downloadPath = "{0}myHandler/Ashx_FileDownload.ashx?OrgiName={1}&FilePath={2}".FormatThis(
                        Application["WebUrl"]
                        , Server.UrlEncode(myFile)
                        , Server.UrlEncode(Cryptograph.Encrypt(Param_WebFolder + myFile)));

                    //顯示縮圖
                    lt_FileThumb.Text = "<div class=\"col-sm-2\"><div class=\"thumbnail\"><img src=\"{0}\" alt=\"{1}\" /></div></div>"
                        .FormatThis(downloadPath, myFile);

                    //顯示內文
                    lt_Desc.Text = "<div class=\"col-sm-7\"><p><strong>{0}</strong><br><strong>{1}</strong></p><p>{2}</p></div>".FormatThis(
                        myModelNo
                        , myModelName
                        , fn_stringFormat.Set_StringOutput(myCont.Replace("\r\n", "<br/>"), 300, fn_stringFormat.WordType.Bytes, true));
                }
                else
                {
                    //顯示內文
                    lt_Desc.Text = "<div class=\"col-sm-9\"><p>{0}</p></div>".FormatThis(
                        fn_stringFormat.Set_StringOutput(myCont.Replace("\r\n", "<br/>"), 400, fn_stringFormat.WordType.Bytes, true));

                }
            }
        }
        catch (Exception)
        {

            throw new Exception("系統發生錯誤 - ItemDataBound！");
        }
    }

    #endregion -- 資料顯示 End --

    #region -- 資料編輯 Start --
    /// <summary>
    /// 基本設定存檔
    /// </summary>
    protected void btn_Save_Click(object sender, EventArgs e)
    {
        try
        {
            #region "..欄位檢查.."
            StringBuilder SBAlert = new StringBuilder();

            //[參數檢查] - 必填項目
            if (fn_Extensions.String_資料長度Byte(this.tb_Promo_Title.Text, "1", "150", out ErrMsg) == false)
            {
                SBAlert.Append("「標題」請輸入1 ~ 75個字\\n");
            }
            //[參數檢查] - 必填項目
            if (fn_Extensions.String_資料長度Byte(this.tb_Promo_Desc.Text, "1", "500", out ErrMsg) == false)
            {
                SBAlert.Append("「標題」請輸入1 ~ 250個字\\n");
            }

            //[JS] - 判斷是否有警示訊息
            if (string.IsNullOrEmpty(SBAlert.ToString()) == false)
            {
                fn_Extensions.JsAlert(SBAlert.ToString(), "");
                return;
            }
            #endregion

            #region --檔案處理--
            //副檔名檢查參數
            int errExt = 0;

            //[IO] - 暫存檔案名稱
            List<TempParam> ITempList = new List<TempParam>();

            //小圖
            HttpPostedFile hpFile = this.fu_Files.PostedFile;
            if (hpFile != null)
            {
                if (hpFile.ContentLength > 0)
                {
                    //[IO] - 取得檔案名稱
                    IOManage.GetFileName(hpFile);

                    //判斷副檔名，未符合規格的檔案不上傳
                    if (fn_Extensions.CheckStrWord(IOManage.FileExtend, FileExtLimit, "|", 1))
                    {
                        //暫存檔案資訊
                        ITempList.Add(new TempParam(IOManage.FileNewName, this.hf_OldFile.Value, hpFile, "P1"));
                    }
                    else
                    {
                        errExt++;
                    }

                }
            }

            //大圖
            HttpPostedFile hpFile_Big = this.fu_Files_Big.PostedFile;
            if (hpFile_Big != null)
            {
                if (hpFile_Big.ContentLength > 0)
                {
                    //[IO] - 取得檔案名稱
                    IOManage.GetFileName(hpFile_Big);

                    //判斷副檔名，未符合規格的檔案不上傳
                    if (fn_Extensions.CheckStrWord(IOManage.FileExtend, FileExtLimit, "|", 1))
                    {
                        //暫存檔案資訊
                        ITempList.Add(new TempParam(IOManage.FileNewName, this.hf_OldFile_Big.Value, hpFile_Big, "P2"));
                    }
                    else
                    {
                        errExt++;
                    }

                }
            }

            //未符合檔案規格的警示訊息
            if (errExt > 0)
            {
                fn_Extensions.JsAlert("上傳內容含有不正確的副檔名\\n請重新挑選!!", "");
                return;
            }
            #endregion

            #region "..資料儲存.."
            //判斷是新增 or 修改
            switch (this.hf_flag.Value.ToUpper())
            {
                case "ADD":
                    Add_Data(ITempList);
                    break;

                case "EDIT":
                    Edit_Data(ITempList);
                    break;

                default:
                    throw new Exception("走錯路囉!");
            }
            #endregion

        }
        catch (Exception)
        {
            fn_Extensions.JsAlert("系統發生錯誤 - 存檔", "");
            return;
        }

    }

    /// <summary>
    /// 資料新增
    /// </summary>
    private void Add_Data(List<TempParam> ITempList)
    {
        using (SqlCommand cmd = new SqlCommand())
        {
            //宣告
            StringBuilder SBSql = new StringBuilder();
            int NewID;
            string pic1 = "", pic2 = "";

            //取得圖片參數
            var queryPic = from el in ITempList
                           select new
                           {
                               NewPic = el.Param_Pic,
                               PicKind = el.Param_FileKind
                           };
            foreach (var item in queryPic)
            {
                if (item.PicKind.Equals("P1"))
                {
                    pic1 = item.NewPic;
                }

                if (item.PicKind.Equals("P2"))
                {
                    pic2 = item.NewPic;
                }
            }

            //[SQL] - 清除參數設定
            cmd.Parameters.Clear();

            //[SQL] - 取得新編號
            SBSql.AppendLine(" DECLARE @NewID AS INT ");
            SBSql.AppendLine(" SET @NewID = (");
            SBSql.AppendLine("  SELECT ISNULL(MAX(Promo_ID) ,0) + 1 FROM Promo ");
            SBSql.AppendLine(" );");
            SBSql.AppendLine(" SELECT @NewID AS NewID");

            cmd.CommandText = SBSql.ToString();
            using (DataTable DT = dbConn.LookupDT(cmd, out ErrMsg))
            {
                NewID = Convert.ToInt32(DT.Rows[0]["NewID"]);
            }

            //--- 開始新增資料 ---
            //[SQL] - 清除參數設定
            cmd.Parameters.Clear();
            SBSql.Clear();

            //[SQL] - 資料新增
            SBSql.AppendLine(" INSERT INTO Promo( ");
            SBSql.AppendLine("  Group_ID, Promo_ID, LangCode");
            SBSql.AppendLine("  , Promo_Title, Promo_Desc, Promo_Icon, Promo_Pic");
            SBSql.AppendLine("  , Create_Who, Create_Time");
            SBSql.AppendLine(" ) VALUES ( ");
            SBSql.AppendLine("  @Group_ID, @NewID, @LangCode");
            SBSql.AppendLine("  , @Promo_Title, @Promo_Desc, @Promo_Icon, @Promo_Pic");
            SBSql.AppendLine("  , @Create_Who, GETDATE() ");
            SBSql.AppendLine(" )");

            //[SQL] - Command
            cmd.CommandText = SBSql.ToString();
            cmd.Parameters.AddWithValue("Group_ID", Param_parentID);
            cmd.Parameters.AddWithValue("NewID", NewID);
            cmd.Parameters.AddWithValue("LangCode", Param_LangCode);
            cmd.Parameters.AddWithValue("Promo_Title", this.tb_Promo_Title.Text.Trim());
            cmd.Parameters.AddWithValue("Promo_Desc", this.tb_Promo_Desc.Text.Trim());
            cmd.Parameters.AddWithValue("Promo_Icon", pic1);
            cmd.Parameters.AddWithValue("Promo_Pic", pic2);
            cmd.Parameters.AddWithValue("Create_Who", Session["Login_GUID"].ToString());
            if (dbConn.ExecuteSql(cmd, out ErrMsg) == false)
            {
                fn_Extensions.JsAlert("資料新增失敗！", Page_CurrentUrl);
                return;
            }

            //[IO] - 儲存檔案
            for (int row = 0; row < ITempList.Count; row++)
            {
                HttpPostedFile hpf = ITempList[row].Param_hpf;
                if (hpf.ContentLength > 0)
                {
                    IOManage.Save(hpf, Param_FileFolder, ITempList[row].Param_Pic, Param_Width, Param_Height);
                }
            }

            //更新本頁Url
            string thisUrl = "{0}Promo/Edit/Detail/{1}/{2}/{3}/".FormatThis(
                        Application["WebUrl"]
                        , Cryptograph.MD5Encrypt(Param_parentID, Application["DesKey"].ToString())
                        , Param_LangCode
                        , Cryptograph.MD5Encrypt(NewID.ToString(), Application["DesKey"].ToString())
                    );
            //導向本頁
            Response.Redirect(thisUrl);

        }

    }

    /// <summary>
    /// 資料修改
    /// </summary>
    private void Edit_Data(List<TempParam> ITempList)
    {
        using (SqlCommand cmd = new SqlCommand())
        {
            //宣告
            StringBuilder SBSql = new StringBuilder();
            string pic1 = this.hf_OldFile.Value, pic2 = this.hf_OldFile_Big.Value;

            //取得圖片參數
            var queryPic = from el in ITempList
                           select new
                           {
                               NewPic = el.Param_Pic,
                               PicKind = el.Param_FileKind
                           };
            foreach (var item in queryPic)
            {
                if (item.PicKind.Equals("P1"))
                {
                    pic1 = item.NewPic;
                }

                if (item.PicKind.Equals("P2"))
                {
                    pic2 = item.NewPic;
                }
            }

            //--- 開始更新資料 ---
            //[SQL] - 清除參數設定
            cmd.Parameters.Clear();

            //[SQL] - 資料更新
            SBSql.AppendLine(" UPDATE Promo ");
            SBSql.AppendLine(" SET Promo_Title = @Promo_Title, Promo_Desc = @Promo_Desc ");
            SBSql.AppendLine("  , Promo_Icon = @Promo_Icon, Promo_Pic = @Promo_Pic");
            SBSql.AppendLine("  , Update_Who = @Update_Who, Update_Time = GETDATE() ");
            SBSql.AppendLine(" WHERE (Promo_ID = @DataID) ");

            //[SQL] - Command
            cmd.CommandText = SBSql.ToString();
            cmd.Parameters.AddWithValue("DataID", Param_thisID);
            cmd.Parameters.AddWithValue("Promo_Title", this.tb_Promo_Title.Text.Trim());
            cmd.Parameters.AddWithValue("Promo_Desc", this.tb_Promo_Desc.Text.Trim());
            cmd.Parameters.AddWithValue("Promo_Icon", pic1);
            cmd.Parameters.AddWithValue("Promo_Pic", pic2);
            cmd.Parameters.AddWithValue("Update_Who", Session["Login_GUID"].ToString());
            if (dbConn.ExecuteSql(cmd, out ErrMsg) == false)
            {
                fn_Extensions.JsAlert("資料更新失敗！", Page_CurrentUrl);
                return;
            }

            //[IO] - 儲存檔案
            for (int row = 0; row < ITempList.Count; row++)
            {
                //刪除原本的檔案
                IOManage.DelFile(Param_FileFolder, ITempList[row].Param_OrgPic);

                //儲存新的檔案
                HttpPostedFile hpf = ITempList[row].Param_hpf;
                if (hpf.ContentLength > 0)
                {
                    IOManage.Save(hpf, Param_FileFolder, ITempList[row].Param_Pic, Param_Width, Param_Height);
                }
            }

            //導向本頁
            Response.Redirect(Page_CurrentUrl);
        }
    }

    /// <summary>
    /// 檔案刪除 - Icon
    /// </summary>
    protected void lbtn_DelFile_Click(object sender, EventArgs e)
    {
        try
        {
            using (SqlCommand cmd = new SqlCommand())
            {
                StringBuilder SBSql = new StringBuilder();

                //[SQL] - 資料更新
                SBSql.AppendLine(" UPDATE Promo SET Promo_Icon = NULL WHERE (Promo_ID = @DataID) ");

                //[SQL] - Command
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("DataID", Param_thisID);
                if (dbConn.ExecuteSql(cmd, out ErrMsg) == false)
                {
                    fn_Extensions.JsAlert("檔案刪除失敗！", Page_CurrentUrl);
                    return;
                }
                else
                {
                    //刪除檔案
                    IOManage.DelFile(Param_FileFolder, this.hf_OldFile.Value);

                    //導向列表
                    Response.Redirect(Page_CurrentUrl);
                }

            }

        }
        catch (Exception)
        {
            fn_Extensions.JsAlert("系統發生錯誤 - 刪除檔案", "");
            return;
        }

    }

    /// <summary>
    /// 檔案刪除 - Big Pic
    /// </summary>
    protected void lbtn_DelFile_Big_Click(object sender, EventArgs e)
    {
        try
        {
            using (SqlCommand cmd = new SqlCommand())
            {
                StringBuilder SBSql = new StringBuilder();

                //[SQL] - 資料更新
                SBSql.AppendLine(" UPDATE Promo SET Promo_Pic = NULL WHERE (Promo_ID = @DataID) ");

                //[SQL] - Command
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("DataID", Param_thisID);
                if (dbConn.ExecuteSql(cmd, out ErrMsg) == false)
                {
                    fn_Extensions.JsAlert("檔案刪除失敗！", Page_CurrentUrl);
                    return;
                }
                else
                {
                    //刪除檔案
                    IOManage.DelFile(Param_FileFolder, this.hf_OldFile_Big.Value);

                    //導向列表
                    Response.Redirect(Page_CurrentUrl);
                }

            }

        }
        catch (Exception)
        {
            fn_Extensions.JsAlert("系統發生錯誤 - 刪除檔案", "");
            return;
        }

    }

    #endregion -- 資料編輯 End --

    #region -- 資料編輯(區塊) Start --

    /// <summary>
    /// 區塊設定存檔
    /// </summary>
    protected void btn_BlockSave_Click(object sender, EventArgs e)
    {
        try
        {
            #region "..欄位檢查.."
            StringBuilder SBAlert = new StringBuilder();

            //[參數檢查] - 必填項目
            if (fn_Extensions.String_資料長度Byte(this.tb_Block_Desc.Text, "1", "1000", out ErrMsg) == false)
            {
                SBAlert.Append("「區塊內容」請輸入1 ~ 500個字\\n");
            }

            //[JS] - 判斷是否有警示訊息
            if (string.IsNullOrEmpty(SBAlert.ToString()) == false)
            {
                fn_Extensions.JsAlert(SBAlert.ToString(), "");
                return;
            }
            #endregion

            #region --檔案處理--
            //副檔名檢查參數
            int errExt = 0;

            //[IO] - 暫存檔案名稱
            List<TempParam> ITempList = new List<TempParam>();
            HttpFileCollection hfc = Request.Files;
            for (int i = 0; i <= hfc.Count - 1; i++)
            {
                HttpPostedFile hpf = hfc[i];
                if (hpf.ContentLength > 0)
                {
                    //[IO] - 取得檔案名稱
                    IOManage.GetFileName(hpf);

                    //判斷副檔名，未符合規格的檔案不上傳
                    if (fn_Extensions.CheckStrWord(IOManage.FileExtend, FileExtLimit, "|", 1))
                    {
                        ITempList.Add(new TempParam(IOManage.FileNewName, IOManage.FileFullName, hpf, "Prod"));
                    }
                    else
                    {
                        errExt++;
                    }
                }
            }

            //未符合檔案規格的警示訊息
            if (errExt > 0)
            {
                fn_Extensions.JsAlert("上傳內容含有不正確的副檔名\\n請重新挑選!!", "");
                return;
            }
            #endregion

            #region "..資料儲存.."
            using (SqlCommand cmd = new SqlCommand())
            {
                //宣告
                StringBuilder SBSql = new StringBuilder();

                //--- 開始新增資料 ---
                //[SQL] - 清除參數設定
                cmd.Parameters.Clear();
                SBSql.Clear();

                //[SQL] - 資料新增
                SBSql.AppendLine(" DECLARE @NewID AS INT ");
                SBSql.AppendLine(" SET @NewID = (");
                SBSql.AppendLine("  SELECT ISNULL(MAX(Block_ID) ,0) + 1 FROM Promo_Block WHERE (Promo_ID = @Promo_ID)");
                SBSql.AppendLine(" );");

                SBSql.AppendLine(" INSERT INTO Promo_Block( ");
                SBSql.AppendLine("  Promo_ID, Block_ID");
                SBSql.AppendLine("  , Model_No, Block_Desc, Block_Pic, Sort");
                SBSql.AppendLine(" ) VALUES ( ");
                SBSql.AppendLine("  @Promo_ID, @NewID");
                SBSql.AppendLine("  , @Model_No, @Block_Desc, @Block_Pic, @Sort");
                SBSql.AppendLine(" )");

                //[SQL] - Command
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.AddWithValue("Promo_ID", Param_thisID);
                cmd.Parameters.AddWithValue("Model_No", this.val_Model_No.Text);
                cmd.Parameters.AddWithValue("Block_Desc", this.tb_Block_Desc.Text);
                cmd.Parameters.AddWithValue("Block_Pic", (ITempList.Count == 0) ? "" : ITempList[0].Param_Pic);
                cmd.Parameters.AddWithValue("Sort", this.tb_Sort.Text);
                if (dbConn.ExecuteSql(cmd, out ErrMsg) == false)
                {
                    Response.Write(ErrMsg);
                    fn_Extensions.JsAlert("區塊新增失敗！", Page_CurrentUrl);
                    return;
                }

                //[IO] - 儲存檔案
                if (ITempList.Count > 0)
                {
                    HttpPostedFile hpf = ITempList[0].Param_hpf;
                    if (hpf.ContentLength > 0)
                    {
                        IOManage.Save(hpf, Param_FileFolder, ITempList[0].Param_Pic, Param_Width, Param_Height);
                    }
                }

                //導向本頁
                Response.Redirect(Page_CurrentUrl);

            }
            #endregion

        }
        catch (Exception)
        {
            fn_Extensions.JsAlert("系統發生錯誤 - 區塊設定存檔", "");
            return;
        }

    }

    /// <summary>
    /// 儲存版面排序
    /// </summary>
    protected void btn_SaveSort_Click(object sender, EventArgs e)
    {
        try
        {
            if (this.lvDataList.Items.Count == 0)
            {
                fn_Extensions.JsAlert("請先新增區塊資料", "");
                return;
            }

            using (SqlCommand cmd = new SqlCommand())
            {
                //宣告
                StringBuilder SBSql = new StringBuilder();

                //[SQL] - 清除參數設定
                cmd.Parameters.Clear();

                for (int row = 0; row < lvDataList.Items.Count; row++)
                {
                    //[取得參數] - 編號
                    string lvParam_ID = ((HiddenField)this.lvDataList.Items[row].FindControl("hf_DataID")).Value;
                    //[取得參數] - 排序
                    string lvParam_Sort = ((TextBox)this.lvDataList.Items[row].FindControl("tb_Sort")).Text;

                    SBSql.AppendLine(" UPDATE Promo_Block SET Sort = @lvParam_Sort_{0}".FormatThis(row));
                    SBSql.AppendLine(" WHERE (Promo_ID = @Promo_ID) AND (Block_ID = @lvParam_ID_{0}) ".FormatThis(row));

                    cmd.Parameters.AddWithValue("lvParam_ID_" + row, lvParam_ID);
                    cmd.Parameters.AddWithValue("lvParam_Sort_" + row, lvParam_Sort);
                }

                //[SQL] - Command
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.AddWithValue("Promo_ID", Param_thisID);
                if (dbConn.ExecuteSql(cmd, out ErrMsg) == false)
                {
                    fn_Extensions.JsAlert("儲存版面失敗！", "");
                }
                else
                {
                    //導向本頁
                    Response.Redirect(Page_CurrentUrl);
                }
            }
        }
        catch (Exception)
        {
            fn_Extensions.JsAlert("系統發生錯誤 - 儲存版面排序", "");
            return;
        }
    }

    #endregion -- 資料編輯(區塊) End --

    #region -- 參數設定 --
    /// <summary>
    /// 取得傳遞參數 - 單頭資料編號
    /// </summary>
    private string _Param_parentID;
    public string Param_parentID
    {
        get
        {
            String DataID = Page.RouteData.Values["ParentID"].ToString();

            return Cryptograph.MD5Decrypt(DataID, Application["DesKey"].ToString());
        }
        set
        {
            this._Param_parentID = value;
        }
    }

    /// <summary>
    /// 取得傳遞參數 - 資料編號
    /// </summary>
    private string _Param_thisID;
    public string Param_thisID
    {
        get
        {
            String DataID = Page.RouteData.Values["DataID"].ToString();

            return DataID.Equals("New") ? "" : Cryptograph.MD5Decrypt(DataID, Application["DesKey"].ToString());
        }
        set
        {
            this._Param_thisID = value;
        }
    }

    /// <summary>
    /// 取得傳遞參數 - 語系
    /// </summary>
    private string _Param_LangCode;
    public string Param_LangCode
    {
        get
        {
            String LangCode = Page.RouteData.Values["LangCode"].ToString();

            return LangCode;
        }
        set
        {
            this._Param_LangCode = value;
        }
    }

    /// <summary>
    /// 設定參數 - 本頁Url
    /// </summary>
    private string _Page_CurrentUrl;
    public string Page_CurrentUrl
    {
        get
        {
            return "{0}Promo/Edit/Detail/{1}/{2}/{3}/".FormatThis(
                Application["WebUrl"]
                , Cryptograph.MD5Encrypt(Param_parentID, Application["DesKey"].ToString())
                , Param_LangCode
                , string.IsNullOrEmpty(Param_thisID) ? "New" : HttpUtility.UrlEncode(Cryptograph.MD5Encrypt(Param_thisID, Application["DesKey"].ToString()))
            );
        }
        set
        {
            this._Page_CurrentUrl = value;
        }
    }

    /// <summary>
    /// 設定參數 - 上一頁Url
    /// </summary>
    private string _Page_LastUrl;
    public string Page_LastUrl
    {
        get
        {
            return "{0}Promo/Edit/{1}/".FormatThis(
                Application["WebUrl"]
                , Cryptograph.MD5Encrypt(Param_parentID, Application["DesKey"].ToString()));
        }
        set
        {
            this._Page_LastUrl = value;
        }
    }

    /// <summary>
    /// [參數] - 檔案資料夾路徑
    /// </summary>
    private string _Param_FileFolder;
    public string Param_FileFolder
    {
        get
        {
            return this._Param_FileFolder != null
                ? this._Param_FileFolder
                : @"{0}Promo\{1}\".FormatThis(Application["File_DiskUrl"], Param_parentID);
        }
        set
        {
            this._Param_FileFolder = value;
        }
    }

    /// <summary>
    /// [參數] - 檔案Web資料夾路徑
    /// </summary>
    private string _Param_WebFolder;
    public string Param_WebFolder
    {
        get
        {
            return this._Param_WebFolder != null
                ? this._Param_WebFolder
                : @"{0}Promo/{1}/".FormatThis(Application["File_WebUrl"], Param_parentID);
        }
        set
        {
            this._Param_WebFolder = value;
        }
    }
    #endregion

    #region -- 上傳參數 --

    /// <summary>
    /// 限制上傳的副檔名
    /// </summary>
    private string _FileExtLimit;
    public string FileExtLimit
    {
        get
        {
            return "jpg|png";
        }
        set
        {
            this._FileExtLimit = value;
        }
    }

    /// <summary>
    /// 圖片設定寬度
    /// </summary>
    private int _Param_Width;
    public int Param_Width
    {
        get
        {
            return 1920;
        }
        set
        {
            this._Param_Width = value;
        }
    }
    /// <summary>
    /// 圖片設定高度
    /// </summary>
    private int _Param_Height;
    public int Param_Height
    {
        get
        {
            return 765;
        }
        set
        {
            this._Param_Height = value;
        }
    }


    /// <summary>
    /// 暫存參數
    /// </summary>
    public class TempParam
    {
        /// <summary>
        /// [參數] - 圖片檔名
        /// </summary>
        private string _Param_Pic;
        public string Param_Pic
        {
            get { return this._Param_Pic; }
            set { this._Param_Pic = value; }
        }

        /// <summary>
        /// [參數] - 圖片原始名稱
        /// </summary>
        private string _Param_OrgPic;
        public string Param_OrgPic
        {
            get { return this._Param_OrgPic; }
            set { this._Param_OrgPic = value; }
        }

        /// <summary>
        /// [參數] - 圖片類別
        /// </summary>
        private string _Param_FileKind;
        public string Param_FileKind
        {
            get { return this._Param_FileKind; }
            set { this._Param_FileKind = value; }
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
        /// <param name="Param_Pic">系統檔名</param>
        /// <param name="Param_OrgPic">原始檔名</param>
        /// <param name="Param_hpf">上傳檔案</param>
        /// <param name="Param_FileKind">檔案類別</param>
        public TempParam(string Param_Pic, string Param_OrgPic, HttpPostedFile Param_hpf, string Param_FileKind)
        {
            this._Param_Pic = Param_Pic;
            this._Param_OrgPic = Param_OrgPic;
            this._Param_hpf = Param_hpf;
            this._Param_FileKind = Param_FileKind;
        }

    }
    #endregion
}