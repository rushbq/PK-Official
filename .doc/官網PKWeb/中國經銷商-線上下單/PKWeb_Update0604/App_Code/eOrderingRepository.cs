using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using eOrder.Models;
using LinqToExcel;
using PKLib_Method.Methods;

namespace eOrder.Controllers
{
    /// <summary>
    /// 查詢參數
    /// </summary>
    public enum mySearch : int
    {
        DataID = 1,
        Keyword = 2,
        TraceID = 3,
        Status = 4,
        CustID = 5,
        StartDate = 6,
        EndDate = 7
    }



    public class eOrderingRepository
    {
        public string ErrMsg;
        public string CurrentDBName = fn_Param.Get_DBName();

        #region -----// Read //-----

        /// <summary>
        /// 取得所有資料(傳入預設參數)
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// 預設值為(null)
        /// </remarks>
        public IQueryable<ImportData> GetDataList()
        {
            return GetDataList(null);
        }


        /// <summary>
        /// 取得所有資料
        /// </summary>
        /// <param name="search">查詢參數</param>
        /// <returns></returns>
        public IQueryable<ImportData> GetDataList(Dictionary<int, string> search)
        {
            //----- 宣告 -----
            List<ImportData> dataList = new List<ImportData>();

            //----- 資料取得 -----
            using (DataTable DT = LookupRawData(search))
            {
                //LinQ 查詢
                var query = DT.AsEnumerable();

                //資料迴圈
                foreach (var item in query)
                {
                    //加入項目
                    var data = new ImportData
                    {
                        Data_ID = item.Field<Guid>("Data_ID"),
                        TraceID = item.Field<string>("TraceID"),
                        CustID = item.Field<string>("CustID"),
                        Data_Type = item.Field<Int16>("Data_Type"),
                        Status = item.Field<Int16>("Status"),
                        Upload_File = item.Field<string>("Upload_File"),
                        Sheet_Name = item.Field<string>("Sheet_Name"),
                        DB_Name = item.Field<string>("DB_Name"),
                        CustName = item.Field<string>("CustName"),
                        StatusName = item.Field<string>("StatusName"),
                        Data_TypeName = item.Field<string>("Data_TypeName"),
                        LogCnt = item.Field<int>("LogCnt"),
                        InCompleteID = item.Field<string>("InCompleteID"),
                        TotalPrice = item.Field<double>("TotalPrice"),

                        Import_Time = item.Field<DateTime?>("Import_Time").ToString().ToDateString("yyyy/MM/dd HH:mm:ss"),
                        Create_Time = item.Field<DateTime?>("Create_Time").ToString().ToDateString("yyyy/MM/dd HH:mm:ss"),
                        Update_Time = item.Field<DateTime?>("Update_Time").ToString().ToDateString("yyyy/MM/dd HH:mm:ss"),


                    };

                    //將項目加入至集合
                    dataList.Add(data);

                }
            }

            //回傳集合
            return dataList.AsQueryable();
        }


        /// <summary>
        /// 取得匯入資料 - 暫存單身
        /// </summary>
        /// <param name="parentID">上層編號</param>
        /// <returns></returns>
        public IQueryable<RefTempColumn> GetDetailTempList(string parentID)
        {
            //----- 宣告 -----
            List<RefTempColumn> dataList = new List<RefTempColumn>();
            StringBuilder sql = new StringBuilder();

            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                sql.AppendLine(" SELECT *");
                sql.AppendLine(" FROM Order_ImportData_TempDT WITH(NOLOCK)");
                sql.AppendLine(" WHERE (Parent_ID = @ParentID)");
                sql.AppendLine(" ORDER BY IsPass DESC, Data_ID ASC");


                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();
                cmd.Parameters.AddWithValue("ParentID", parentID);


                //----- 資料取得 -----
                using (DataTable DT = dbConn.LookupDT(cmd, out ErrMsg))
                {
                    //LinQ 查詢
                    var query = DT.AsEnumerable();

                    //資料迴圈
                    foreach (var item in query)
                    {
                        //加入項目
                        var data = new RefTempColumn
                        {
                            Data_ID = item.Field<int>("Data_ID"),
                            ProdID = item.Field<string>("ProdID"),
                            Cust_ModelNo = item.Field<string>("Cust_ModelNo"),
                            ERP_ModelNo = item.Field<string>("ERP_ModelNo"),
                            InputCnt = item.Field<int>("InputCnt"),
                            BuyCnt = item.Field<int>("BuyCnt"),
                            MOQ = item.Field<int>("MOQ"),
                            MinQty = item.Field<int>("MinQty"),
                            IsPass = item.Field<string>("IsPass"),
                            doWhat = item.Field<string>("doWhat")
                        };

                        //將項目加入至集合
                        dataList.Add(data);

                    }
                }
            }

            //回傳集合
            return dataList.AsQueryable();
        }


        /// <summary>
        /// 取得匯入資料 - 單身
        /// </summary>
        /// <param name="parentID">上層編號</param>
        /// <returns></returns>
        public IQueryable<RefColumn> GetDetailList(string parentID)
        {
            //----- 宣告 -----
            List<RefColumn> dataList = new List<RefColumn>();
            StringBuilder sql = new StringBuilder();

            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                sql.AppendLine(" SELECT *");
                sql.AppendLine(" FROM Order_ImportData_DT WITH(NOLOCK)");
                sql.AppendLine(" WHERE (Parent_ID = @ParentID)");
                sql.AppendLine(" ORDER BY Data_ID ASC");


                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();
                cmd.Parameters.AddWithValue("ParentID", parentID);


                //----- 資料取得 -----
                using (DataTable DT = dbConn.LookupDT(cmd, out ErrMsg))
                {
                    //LinQ 查詢
                    var query = DT.AsEnumerable();

                    //資料迴圈
                    foreach (var item in query)
                    {
                        //加入項目
                        var data = new RefColumn
                        {
                            Data_ID = item.Field<int>("Data_ID"),
                            OrderID = item.Field<string>("OrderID"),
                            Cust_ModelNo = item.Field<string>("Cust_ModelNo"),
                            ERP_ModelNo = item.Field<string>("ERP_ModelNo"),
                            ERP_Price = item.Field<double>("ERP_Price"),
                            BuyCnt = item.Field<int>("BuyCnt"),
                            MOQ = item.Field<int>("MOQ"),
                            MinQty = item.Field<int>("MinQty"),
                            Currency = item.Field<string>("Currency"),
                            ShipWho = item.Field<string>("ShipWho"),
                            ShipAddr = item.Field<string>("ShipAddr"),
                            ShipTel = item.Field<string>("ShipTel"),
                            StockNum = item.Field<int>("StockNum"),
                            StockStatus = item.Field<string>("StockStatus")
                        };


                        //將項目加入至集合
                        dataList.Add(data);

                    }
                }
            }

            //回傳集合
            return dataList.AsQueryable();
        }


        /// <summary>
        /// 取得通知信收件人清單
        /// </summary>
        /// <returns></returns>
        public IQueryable<MailList> GetMailList()
        {
            //----- 宣告 -----
            List<MailList> dataList = new List<MailList>();
            StringBuilder sql = new StringBuilder();

            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                sql.AppendLine(" SELECT MailID, MailName, MailAddress");
                sql.AppendLine(" FROM Order_MailList WITH(NOLOCK)");


                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();


                //----- 資料取得 -----
                using (DataTable DT = dbConn.LookupDT(cmd, out ErrMsg))
                {
                    //LinQ 查詢
                    var query = DT.AsEnumerable();

                    //資料迴圈
                    foreach (var item in query)
                    {
                        //加入項目
                        var data = new MailList
                        {
                            MailAddress = item.Field<string>("MailAddress")
                        };


                        //將項目加入至集合
                        dataList.Add(data);

                    }
                }
            }

            //回傳集合
            return dataList.AsQueryable();
        }

        ///// <summary>
        ///// 取得Log資料
        ///// </summary>
        ///// <param name="dataID">ID</param>
        ///// <returns></returns>
        //public IQueryable<RefLog> GetLogList(string dataID)
        //{
        //    //----- 宣告 -----
        //    List<RefLog> dataList = new List<RefLog>();
        //    StringBuilder sql = new StringBuilder();

        //    //----- 資料查詢 -----
        //    using (SqlCommand cmd = new SqlCommand())
        //    {
        //        //----- SQL 查詢語法 -----
        //        sql.AppendLine(" SELECT *");
        //        sql.AppendLine(" FROM Order_ImportData_Log WITH(NOLOCK)");
        //        sql.AppendLine(" WHERE (Data_ID = @Data_ID)");
        //        sql.AppendLine(" ORDER BY Create_Time DESC");


        //        //----- SQL 執行 -----
        //        cmd.CommandText = sql.ToString();
        //        cmd.Parameters.AddWithValue("Data_ID", dataID);


        //        //----- 資料取得 -----
        //        using (DataTable DT = dbConn.LookupDT(cmd, out ErrMsg))
        //        {
        //            //LinQ 查詢
        //            var query = DT.AsEnumerable();

        //            //資料迴圈
        //            foreach (var item in query)
        //            {
        //                //加入項目
        //                var data = new RefLog
        //                {
        //                    Log_ID = item.Field<Int64>("Log_ID"),
        //                    TraceID = item.Field<string>("TraceID"),
        //                    Log_Desc = item.Field<string>("Log_Desc"),
        //                    Create_Time = item.Field<DateTime?>("Create_Time").ToString().ToDateString("yyyy/MM/dd HH:mm")
        //                };

        //                //將項目加入至集合
        //                dataList.Add(data);

        //            }
        //        }
        //    }

        //    //回傳集合
        //    return dataList.AsQueryable();
        //}


        ///// <summary>
        ///// 反查ERP 訂單/銷貨單資料
        ///// </summary>
        ///// <param name="search">查詢參數</param>
        ///// <returns></returns>
        //public IQueryable<ERPData> GetERPData(Dictionary<int, string> search)
        //{
        //    //----- 宣告 -----
        //    List<ERPData> dataList = new List<ERPData>();
        //    StringBuilder sql = new StringBuilder();

        //    //----- 資料查詢 -----
        //    using (SqlCommand cmd = new SqlCommand())
        //    {
        //        //----- SQL 查詢語法 -----
        //        sql.AppendLine(" SELECT ");
        //        //--訂單資訊
        //        sql.AppendLine(" COPTC.TC001, COPTC.TC002, COPTD.TD004, COPTD.TD005, ROUND(COPTD.TD008, 0) TD008");
        //        //--銷貨單資訊
        //        sql.AppendLine(" , COPTH.TH001, COPTH.TH002, COPTH.TH004, COPTH.TH005, COPTH.TH007, ROUND(COPTH.TH008, 0) TH008");
        //        //--原始資料
        //        sql.AppendLine(" , DT.OrderID, DT.ERP_ModelNo ModelNo");
        //        sql.AppendLine(" FROM Order_ImportData Base");
        //        sql.AppendLine("  INNER JOIN Order_ImportData_DT DT ON Base.Data_ID = DT.Parent_ID");

        //        //COPTC:訂單單頭 ,條件:OrderID = TC012, CustID = TC004");
        //        sql.AppendLine("  INNER JOIN [ProUnion].dbo.COPTC WITH(NOLOCK) ON DT.OrderID COLLATE Chinese_Taiwan_Stroke_BIN = COPTC.TC012");
        //        sql.AppendLine("     AND Base.CustID COLLATE Chinese_Taiwan_Stroke_BIN = COPTC.TC004");

        //        //COPTD:訂單單身, 條件:ERP_ModelNo = TD004, COPTC.TC001 = COPTD.TD001, COPTC.TC002 = COPTD.TD002");
        //        sql.AppendLine("  INNER JOIN [ProUnion].dbo.COPTD WITH(NOLOCK) ON COPTC.TC001 = COPTD.TD001 AND COPTC.TC002 = COPTD.TD002");
        //        sql.AppendLine("     AND DT.ERP_ModelNo COLLATE Chinese_Taiwan_Stroke_BIN = COPTD.TD004");

        //        //COPTH:銷貨單單身, 條件:COPTH.TH014 = COPTC.TC001, COPTH.TH015 = COPTC.TC002, (訂單序號)COPTH.TH016 = COPTD.TD003");
        //        sql.AppendLine("  LEFT JOIN [ProUnion].dbo.COPTH WITH(NOLOCK) ON COPTH.TH014 = COPTC.TC001 AND COPTH.TH015 = COPTC.TC002 AND COPTH.TH016 = COPTD.TD003");

        //        //Filter
        //        sql.AppendLine(" WHERE (DT.IsPass = 'Y') AND (COPTC.TC200 = 'Y')");


        //        /* Search */
        //        if (search != null)
        //        {
        //            foreach (var item in search)
        //            {
        //                switch (item.Key)
        //                {
        //                    case (int)mySearch.DataID:
        //                        if (!string.IsNullOrEmpty(item.Value))
        //                        {
        //                            sql.Append(" AND (Base.Data_ID = @DataID)");

        //                            cmd.Parameters.AddWithValue("DataID", item.Value);
        //                        }

        //                        break;

        //                    case (int)mySearch.TraceID:
        //                        if (!string.IsNullOrEmpty(item.Value))
        //                        {
        //                            sql.Append(" AND (Base.TraceID = @TraceID)");

        //                            cmd.Parameters.AddWithValue("TraceID", item.Value);
        //                        }

        //                        break;
        //                }
        //            }
        //        }

        //        sql.AppendLine(" ORDER BY COPTC.TC001, COPTC.TC002, COPTD.TD004");

        //        //----- SQL 執行 -----
        //        cmd.CommandText = sql.ToString();


        //        //----- 資料取得 -----
        //        using (DataTable DT = dbConn.LookupDT(cmd, out ErrMsg))
        //        {
        //            //LinQ 查詢
        //            var query = DT.AsEnumerable();

        //            //資料迴圈
        //            foreach (var item in query)
        //            {
        //                //加入項目
        //                var data = new ERPData
        //                {
        //                    OrderID = item.Field<string>("OrderID"),
        //                    ModelNo = item.Field<string>("ModelNo"),

        //                    TC001 = item.Field<string>("TC001"),
        //                    TC002 = item.Field<string>("TC002"),
        //                    TD004 = item.Field<string>("TD004"),    //品號
        //                    TD005 = item.Field<string>("TD005"),    //品名
        //                    TD008 = Convert.ToInt16(item.Field<Decimal?>("TD008")),  //數量

        //                    TH001 = item.Field<string>("TH001"),
        //                    TH002 = item.Field<string>("TH002"),
        //                    TH004 = item.Field<string>("TH004"),    //品號
        //                    TH005 = item.Field<string>("TH005"),    //品名
        //                    TH007 = item.Field<string>("TH007"),    //庫別
        //                    TH008 = Convert.ToInt16(item.Field<Decimal?>("TH008"))   //數量
        //                };

        //                //將項目加入至集合
        //                dataList.Add(data);

        //            }
        //        }
        //    }

        //    //回傳集合
        //    return dataList.AsQueryable();
        //}


        ///// <summary>
        ///// EDI轉入失敗記錄
        ///// </summary>
        ///// <param name="traceID">平台追蹤編號</param>
        ///// <returns></returns>
        //public IQueryable<EDILog> GetEDILog(string traceID)
        //{
        //    //----- 宣告 -----
        //    List<EDILog> dataList = new List<EDILog>();
        //    StringBuilder sql = new StringBuilder();

        //    //----- 資料查詢 -----
        //    using (SqlCommand cmd = new SqlCommand())
        //    {
        //        //----- SQL 查詢語法 -----
        //        sql.AppendLine(" SELECT RTRIM(XA006) OrderID, RTRIM(XA011) ModelNo, XA019 Why");
        //        sql.AppendLine(" FROM DSCSYS.dbo.EDIXA WITH(NOLOCK)");
        //        sql.AppendLine(" WHERE (XA025 = @TraceID)");

        //        //----- SQL 執行 -----
        //        cmd.CommandText = sql.ToString();
        //        cmd.Parameters.AddWithValue("TraceID", traceID);


        //        //----- 資料取得 -----
        //        using (DataTable DT = dbConn.LookupDT(cmd, out ErrMsg))
        //        {
        //            //LinQ 查詢
        //            var query = DT.AsEnumerable();

        //            //資料迴圈
        //            foreach (var item in query)
        //            {
        //                //加入項目
        //                var data = new EDILog
        //                {
        //                    OrderID = item.Field<string>("OrderID"),
        //                    ModelNo = item.Field<string>("ModelNo"),
        //                    Why = item.Field<string>("Why")
        //                };

        //                //將項目加入至集合
        //                dataList.Add(data);

        //            }
        //        }
        //    }

        //    //回傳集合
        //    return dataList.AsQueryable();
        //}


        /// <summary>
        /// 取得Excel內容
        /// </summary>
        /// <param name="filePath">完整磁碟路徑</param>
        /// <param name="sheetName">工作表名稱</param>
        /// <returns></returns>
        /// <example>
        /// <table id="listTable" class="stripe" cellspacing="0" width="100%" style="width:100%;">
        ///     <asp:Literal ID="lt_tbBody" runat="server"></asp:Literal>
        /// </table>
        /// </example>
        /// <see cref="LinqToExcel.dll"/>
        public StringBuilder GetExcel_Html(string filePath, string sheetName)
        {
            try
            {
                //宣告
                StringBuilder html = new StringBuilder();

                //[Excel] - 取得原始資料
                var excelFile = new ExcelQueryFactory(filePath);

                //[HTML] - 取得欄位, 輸出標題欄 (GetColumnNames)
                var queryCols = excelFile.GetColumnNames(sheetName);

                html.Append("<thead>");
                html.Append("<tr>");
                foreach (var col in queryCols)
                {
                    html.Append("<th>{0}</th>".FormatThis(col.ToString()));
                }
                html.Append("</tr>");
                html.Append("</thead>");


                //[HTML] - 取得欄位值, 輸出內容欄 (Worksheet)
                var queryVals = excelFile.Worksheet(sheetName);

                html.Append("<tbody>");
                foreach (var val in queryVals)
                {
                    //內容迴圈
                    html.Append("<tr>");

                    int myCol = 0;
                    foreach (var col in queryCols)
                    {
                        html.Append("<td>{0}</td>".FormatThis(val[col]));

                        myCol++;
                    }

                    html.Append("</tr>");
                }

                html.Append("</tbody>");

                //output
                return html;
            }
            catch (Exception)
            {
                throw new Exception("請檢查Excel格式是否正確!!");
            }
        }


        /// <summary>
        /// 取得Excel資料欄位
        /// </summary>
        /// <param name="filePath">完整磁碟路徑</param>
        /// <param name="sheetName">工作表名稱</param>
        /// <param name="dataType">1:客戶品號, 2:寶工品號</param>
        /// <param name="traceID">trace id</param>
        /// <returns></returns>
        public IQueryable<RefTempColumn> GetExcel_DT(string filePath, string sheetName, string traceID)
        {
            try
            {
                //----- 宣告 -----
                List<RefTempColumn> dataList = new List<RefTempColumn>();

                //[Excel] - 取得原始資料
                var excelFile = new ExcelQueryFactory(filePath);
                var queryVals = excelFile.Worksheet(sheetName);

                //宣告各內容參數
                string myProdID = "";
                int myBuyCnt = 1;

                //資料迴圈
                foreach (var val in queryVals)
                {
                    myProdID = val[0];
                    myBuyCnt = string.IsNullOrEmpty(val[1]) ? 0 : Convert.ToInt16(val[1]);


                    //加入項目
                    var data = new RefTempColumn
                    {
                        ProdID = myProdID,
                        BuyCnt = myBuyCnt,
                    };

                    //將項目加入至集合
                    dataList.Add(data);

                }


                //回傳集合
                return dataList.AsQueryable();
            }
            catch (Exception)
            {

                throw new Exception("請檢查Excel格式是否正確!!");
            }
        }

        #endregion


        #region -----// Create //-----

        /// <summary>
        /// 建立基本資料 - Step1執行
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public bool Create(ImportData instance)
        {
            //----- 宣告 -----
            StringBuilder sql = new StringBuilder();

            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                sql.AppendLine(" INSERT INTO Order_ImportData( ");
                sql.AppendLine("  Data_ID, TraceID, CustID, Data_Type, Status, Upload_File");
                sql.AppendLine("  , Create_Time, DB_Name");
                sql.AppendLine(" ) VALUES (");
                sql.AppendLine("  @Data_ID, @TraceID, @CustID, @Data_Type, 1, @Upload_File");
                sql.AppendLine("  , GETDATE()");
                sql.AppendLine("  , (SELECT TOP 1 Corp.DB_Name FROM [PKSYS].[dbo].[Customer] Cust");
                sql.AppendLine("     INNER JOIN [PKSYS].[dbo].[Param_Corp] Corp ON Cust.DBS = Corp.Corp_ID");
                sql.AppendLine("     WHERE (Cust.MA001 = @CustID) AND (Cust.DBS = Cust.DBC))");
                sql.AppendLine(" );");


                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();
                cmd.Parameters.AddWithValue("Data_ID", instance.Data_ID);
                cmd.Parameters.AddWithValue("TraceID", instance.TraceID);
                cmd.Parameters.AddWithValue("CustID", instance.CustID);
                cmd.Parameters.AddWithValue("Data_Type", instance.Data_Type);
                cmd.Parameters.AddWithValue("Upload_File", instance.Upload_File);

                return dbConn.ExecuteSql(cmd, out ErrMsg);
            }

        }


        /// <summary>
        /// 建立暫存Table, 更新主檔欄位 - Step1.1執行
        /// 成功後執行Check_Temp
        /// 1) 若為客戶品號則UPDATE PK品號
        /// 2) 若為PK品號則檢查是否正確
        /// 3) 取得分量計價資料(myPrc_GetProdQtyInfo)
        /// </summary>
        /// <param name="baseData"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public bool Create_Temp(ImportData baseData, IQueryable<RefTempColumn> query, out string ErrMsg)
        {
            //----- 宣告 -----
            StringBuilder sql = new StringBuilder();

            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                sql.AppendLine(" DELETE FROM Order_ImportData_TempDT WHERE (Parent_ID = @DataID);");
                sql.AppendLine(" DELETE FROM Order_ImportData_DT WHERE (Parent_ID = @DataID);");
                sql.AppendLine(" UPDATE Order_ImportData SET Status = 2, Import_Time = GETDATE(), Sheet_Name = @Sheet_Name, Update_Time = GETDATE() WHERE (Data_ID = @DataID);");

                sql.AppendLine(" DECLARE @NewID AS INT ");

                int row = 0;

                foreach (var item in query)
                {
                    if (!string.IsNullOrEmpty(item.ProdID))
                    {
                        sql.AppendLine(" SET @NewID = (");
                        sql.AppendLine("  SELECT ISNULL(MAX(Data_ID), 0) + 1 AS NewID ");
                        sql.AppendLine("  FROM Order_ImportData_TempDT ");
                        sql.AppendLine("  WHERE Parent_ID = @DataID ");
                        sql.AppendLine(" )");

                        sql.AppendLine(" INSERT INTO Order_ImportData_TempDT( ");
                        sql.AppendLine("  Parent_ID, Data_ID, ProdID, InputCnt, BuyCnt");
                        sql.AppendLine(" ) VALUES (");
                        sql.AppendLine("  @DataID, @NewID, @ProdID_{0}, @InputCnt_{0}, @BuyCnt_{0}".FormatThis(row));
                        sql.AppendLine(" );");

                        cmd.Parameters.AddWithValue("ProdID_" + row, item.ProdID);
                        cmd.Parameters.AddWithValue("InputCnt_" + row, item.BuyCnt);
                        cmd.Parameters.AddWithValue("BuyCnt_" + row, item.BuyCnt);

                        row++;
                    }
                }

                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();
                cmd.Parameters.AddWithValue("DataID", baseData.Data_ID);
                cmd.Parameters.AddWithValue("Sheet_Name", baseData.Sheet_Name);

                return dbConn.ExecuteSql(cmd, out ErrMsg);
            }

        }


        /// <summary>
        /// 更新暫存Table, 品號 (Step1.1)
        /// </summary>
        /// <param name="baseData"></param>
        /// <param name="ErrMsg"></param>
        /// <returns></returns>
        /// <remarks>
        /// 此處會JOIN到ERP的資料庫, 故使用PKSYS的帳號來JOIN
        /// </remarks>
        public bool Check_Temp1(ImportData baseData, out string ErrMsg)
        {
            //----- 宣告 -----
            StringBuilder sql = new StringBuilder();

            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----

                string dbName = baseData.DB_Name;

                //--Update來源為客戶品號的資料
                sql.AppendLine(" UPDATE {0}.dbo.Order_ImportData_TempDT".FormatThis(CurrentDBName));
                sql.AppendLine(" SET IsPass = 'Y', Cust_ModelNo = RTRIM(ERPData.MG003), ERP_ModelNo = RTRIM(ERPData.MG002)");
                sql.AppendLine(" FROM [{0}].dbo.COPMG AS ERPData".FormatThis(dbName));
                sql.AppendLine(" WHERE (Parent_ID = @DataID)");
                sql.AppendLine("  AND (ERPData.MG001 = @CustID) AND (Order_ImportData_TempDT.ProdID COLLATE Chinese_Taiwan_Stroke_BIN = ERPData.MG003);");

                //--Update來源為寶工品號的資料(排除淘汰商品)
                sql.AppendLine(" UPDATE {0}.dbo.Order_ImportData_TempDT".FormatThis(CurrentDBName));
                sql.AppendLine(" SET IsPass = 'Y', ERP_ModelNo = RTRIM(ERPData.MB001)");
                sql.AppendLine(" FROM [{0}].dbo.INVMB AS ERPData".FormatThis(dbName));
                sql.AppendLine(" WHERE (Parent_ID = @DataID) AND (ERPData.MB008 <> '4005')");
                sql.AppendLine("  AND (Order_ImportData_TempDT.ProdID COLLATE Chinese_Taiwan_Stroke_BIN = ERPData.MB001);");

                //--將查無品號的寫入原因(9001:查無寶工品號)
                sql.AppendLine(" UPDATE {0}.dbo.Order_ImportData_TempDT".FormatThis(CurrentDBName));
                sql.AppendLine(" SET doWhat = '9001:查無寶工品號'");
                sql.AppendLine(" WHERE (IsPass = 'N') AND (Parent_ID = @DataID);");


                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();
                cmd.Parameters.AddWithValue("DataID", baseData.Data_ID);
                cmd.Parameters.AddWithValue("CustID", baseData.CustID);

                //### 上線後修改此處 ###
                return dbConn.ExecuteSql(cmd, dbConn.DBS.PKSYS, out ErrMsg);
                //return dbConn.ExecuteSql(cmd, dbConn.DBS.TestPKSYS, out ErrMsg);
            }

        }


        /// <summary>
        /// 更新暫存Table, MOQ/MinQty/UnitPrice (Step1.1)
        /// </summary>
        /// <param name="baseData"></param>
        /// <param name="ErrMsg"></param>
        /// <returns></returns>
        public bool Check_Temp2(ImportData baseData, out string ErrMsg)
        {
            //----- 宣告 -----
            StringBuilder sql = new StringBuilder();
            string custID = baseData.CustID;
            string dbName = baseData.DB_Name;
            string dataID = baseData.Data_ID.ToString();

            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                sql.AppendLine(" SELECT ERP_ModelNo ");
                sql.AppendLine(" FROM Order_ImportData_TempDT");
                sql.AppendLine(" WHERE (IsPass = 'Y') AND (ERP_ModelNo IS NOT NULL) AND (Parent_ID = @DataID);");

                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();
                cmd.Parameters.AddWithValue("DataID", baseData.Data_ID);

                //取得品號
                ArrayList itemAry = new ArrayList();
                using (DataTable DT = dbConn.LookupDT(cmd, out ErrMsg))
                {
                    for (int row = 0; row < DT.Rows.Count; row++)
                    {
                        itemAry.Add(DT.Rows[row]["ERP_ModelNo"].ToString());
                    }
                }

                //判斷有無品號
                if (itemAry.Count == 0)
                {
                    ErrMsg = "品號未正確對應,若為客戶品號請確認[COPMG客戶品號資料檔]已建立";
                    return false;
                }


                //Clear
                sql.Clear();
                cmd.Parameters.Clear();

                //呼叫SP [myPrc_GetProdQtyInfo], Update 暫存檔
                DataTable erpData = GetSpQtyInfo(custID, itemAry, dbName, out ErrMsg);
                if (erpData == null)
                {
                    ErrMsg = "無法取得ERP客戶報價資料,custID={0},DB={1}".FormatThis(custID, dbName);
                    return false;
                }

                for (int row = 0; row < erpData.Rows.Count; row++)
                {
                    sql.AppendLine(" UPDATE Order_ImportData_TempDT ");
                    sql.AppendLine(" SET MOQ = {0}, MinQty = {1}, UnitPrice = {2}".FormatThis(
                         Convert.ToInt32(erpData.Rows[row]["InBoxQty"])
                         , Convert.ToInt32(erpData.Rows[row]["minQty"])
                         , Convert.ToDouble(erpData.Rows[row]["DefaultPrice"])
                        ));
                    sql.AppendLine(" WHERE (ERP_ModelNo = N'{0}') AND (Parent_ID = @DataID)".FormatThis(erpData.Rows[row]["ModelNo"]));
                }

                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();
                cmd.Parameters.AddWithValue("DataID", dataID);

                return dbConn.ExecuteSql(cmd, out ErrMsg);

            }

        }


        /// <summary>
        /// 取得分量計價資料(取第2筆的資料) (Step1.1)
        /// </summary>
        /// <param name="CustID"></param>
        /// <param name="aryModelNo"></param>
        /// <param name="DBS"></param>
        /// <param name="ErrMsg"></param>
        /// <returns></returns>
        private DataTable GetSpQtyInfo(string CustID, ArrayList aryModelNo, string DBS, out string ErrMsg)
        {
            //Array轉換字串
            string strModelNo = (aryModelNo.Count > 0) ? string.Join(",", aryModelNo.ToArray()) : "";

            //查詢StoreProcedure (myPrc_GetProdQtyInfo)
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.Parameters.Clear();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "myPrc_GetProdQtyInfo";
                cmd.Parameters.AddWithValue("CustIDs", CustID);
                cmd.Parameters.AddWithValue("ModelNos", string.IsNullOrEmpty(strModelNo) ? DBNull.Value : (Object)strModelNo);
                cmd.Parameters.AddWithValue("DBS", DBS);
                //取得回傳值, 輸出參數
                SqlParameter Msg = cmd.Parameters.Add("@Msg", SqlDbType.NVarChar, 200);
                Msg.Direction = ParameterDirection.Output;

                using (DataTable DT = dbConn.LookupDT(cmd, dbConn.DBS.PKSYS, out ErrMsg))
                {
                    if (DT.Rows.Count == 0)
                    {
                        ErrMsg = "查無資料";
                        return null;
                    }

                    //SQL回傳訊息
                    ErrMsg = Msg.Value.ToString();

                    //回傳資料集
                    return DT;
                }
            }
        }


        /// <summary>
        /// 建立單身資料, 取得價格及庫存, 設定要給EDI的訂單編號 (Step2)
        /// </summary>
        /// <param name="baseData"></param>
        /// <param name="ErrMsg"></param>
        /// <returns></returns>
        public bool Create_Detail(ImportData baseData, out string ErrMsg)
        {
            //***** 取得必要參數 ******
            #region -- 處理1 --

            //----- 宣告 -----
            StringBuilder sql = new StringBuilder();
            string dataID = baseData.Data_ID.ToString();
            string traceID = baseData.TraceID;
            string CustID, DBName, StockType, ModelNos, Qtys;
            string ShipWho, ShipAddr, ShipTel;


            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                sql.AppendLine(" SELECT Base.CustID, Base.DB_Name, CustDT.SWID, SW.StockType");
                sql.AppendLine("  , RTRIM(Cust.MA002) ShipWho, RTRIM(Cust.MA006) ShipTel, RTRIM(Cust.MA027) ShipAddr");
                sql.AppendLine(" FROM {0}.dbo.Order_ImportData Base WITH(NOLOCK)".FormatThis(CurrentDBName));
                sql.AppendLine("  INNER JOIN Customer Cust WITH(NOLOCK) ON Base.CustID = Cust.MA001");
                sql.AppendLine("  INNER JOIN Customer_Data CustDT WITH(NOLOCK) ON Cust.MA001 = CustDT.Cust_ERPID");
                sql.AppendLine("  INNER JOIN ShippingWarehouse SW WITH(NOLOCK) ON CustDT.SWID = SW.SWID");
                sql.AppendLine(" WHERE (Base.Data_ID = @DataID)");


                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("DataID", dataID);

                //### 上線後修改此處 ###
                //using (DataTable DT = dbConn.LookupDT(cmd, dbConn.DBS.TestPKSYS, out ErrMsg))
                using (DataTable DT = dbConn.LookupDT(cmd, dbConn.DBS.PKSYS, out ErrMsg))
                {
                    if (DT.Rows.Count == 0)
                    {
                        ErrMsg = "無法取得出貨庫別";
                        return false;
                    }

                    //填入資料
                    CustID = DT.Rows[0]["CustID"].ToString();
                    DBName = DT.Rows[0]["DB_Name"].ToString();
                    StockType = DT.Rows[0]["StockType"].ToString();
                    ShipWho = DT.Rows[0]["ShipWho"].ToString();
                    ShipAddr = DT.Rows[0]["ShipAddr"].ToString();
                    ShipTel = DT.Rows[0]["ShipTel"].ToString();
                }
            }

            #endregion


            //***** 取得要取價的品號 ModelNos, Qtys (from TempDT) ***** 
            #region -- 處理2 --

            List<String> iModelNo = new List<String>();
            List<String> iQty = new List<String>();

            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //清除參數
                sql.Clear();
                cmd.Parameters.Clear();

                //----- SQL 查詢語法 -----
                sql.AppendLine(" SELECT ERP_ModelNo AS ModelNo, BuyCnt");
                sql.AppendLine(" FROM Order_ImportData_TempDT WITH(NOLOCK)");
                sql.AppendLine(" WHERE (IsPass = 'Y') AND (Parent_ID = @DataID)");

                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();
                cmd.Parameters.AddWithValue("DataID", dataID);

                using (DataTable DT = dbConn.LookupDT(cmd, out ErrMsg))
                {
                    if (DT.Rows.Count == 0)
                    {
                        ErrMsg = "無法取得品號資料";
                        return false;
                    }

                    //填入資料
                    for (int row = 0; row < DT.Rows.Count; row++)
                    {
                        iModelNo.Add(DT.Rows[row]["ModelNo"].ToString());
                        iQty.Add(DT.Rows[row]["BuyCnt"].ToString());
                    }

                    ModelNos = string.Join(",", iModelNo.ToArray());
                    Qtys = string.Join(",", iQty.ToArray());
                }
            }

            #endregion


            //***** 將資料寫入正式單身Table ***** 
            #region -- 處理3 --

            //先取得價格及庫存(from WebService)
            DataTable ErpDT = GetErpData(CustID, DBName, ModelNos, Qtys, StockType, out ErrMsg);
            if (ErpDT == null)
            {
                ErrMsg = "無法取得ERP價格及庫存(Step2)..." + ErrMsg;
                return false;
            }

            using (SqlCommand cmd = new SqlCommand())
            {
                //清除參數
                sql.Clear();
                cmd.Parameters.Clear();

                //----- SQL 查詢語法 -----
                sql.AppendLine(" DELETE FROM Order_ImportData_DT WHERE (Parent_ID = @DataID)");
                sql.AppendLine(" DECLARE @NewID AS INT");

                //迴圈
                for (int row = 0; row < ErpDT.Rows.Count; row++)
                {
                    sql.AppendLine(" SET @NewID = (");
                    sql.AppendLine("  SELECT ISNULL(MAX(Data_ID), 0) + 1 AS NewID");
                    sql.AppendLine("  FROM Order_ImportData_DT");
                    sql.AppendLine("  WHERE (Parent_ID = @DataID)");
                    sql.AppendLine(" );");

                    sql.AppendLine(" INSERT INTO Order_ImportData_DT(");
                    sql.AppendLine("  Parent_ID, Data_ID");
                    sql.AppendLine("  , OrderID, ERP_ModelNo, ERP_Price, StockStatus");
                    sql.AppendLine("  , BuyCnt, Currency, StockNum");
                    sql.AppendLine("  , ShipWho, ShipAddr, ShipTel");
                    sql.AppendLine(" ) VALUES (");
                    sql.AppendLine("  @DataID, @NewID");
                    sql.AppendLine("  , @OrderID_{0}, @ERP_ModelNo_{0}, @ERP_Price_{0}, @StockStatus_{0}".FormatThis(row));
                    sql.AppendLine("  , @BuyCnt_{0}, @Currency_{0}, @StockNum_{0}".FormatThis(row));
                    sql.AppendLine("  , @ShipWho, @ShipAddr, @ShipTel");
                    sql.AppendLine(" );");

                    /*
                    * OrderID:訂單編號取用主檔的TraceID, 並判斷庫存, 足夠補1, 不足補2, 成為19碼的訂單編號
                    * StockStatus:判斷庫存, 足夠=1, 不足=2
                    */
                    //庫存量
                    Int32 stockNum = Convert.ToInt32(ErpDT.Rows[row]["StockNum"]);
                    //購買量
                    Int32 buyCnt = Convert.ToInt32(ErpDT.Rows[row]["BuyQty"]);
                    //目前庫存判斷(1:銷貨單 / 2:訂單)
                    string stockStatus = (stockNum - buyCnt) > 0 ? "1" : "2";


                    cmd.Parameters.AddWithValue("OrderID_" + row, traceID + stockStatus); //訂單編號(EDI判斷用)
                    cmd.Parameters.AddWithValue("ERP_ModelNo_" + row, ErpDT.Rows[row]["ModelNo"]);
                    cmd.Parameters.AddWithValue("ERP_Price_" + row, Convert.ToDouble(ErpDT.Rows[row]["SpQtyPrice"]));
                    cmd.Parameters.AddWithValue("StockStatus_" + row, stockStatus); //庫存狀態
                    cmd.Parameters.AddWithValue("BuyCnt_" + row, Convert.ToInt32(ErpDT.Rows[row]["BuyQty"]));
                    cmd.Parameters.AddWithValue("Currency_" + row, ErpDT.Rows[row]["Currency"]);
                    cmd.Parameters.AddWithValue("StockNum_" + row, stockNum);

                }


                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();
                cmd.Parameters.AddWithValue("DataID", dataID);
                cmd.Parameters.AddWithValue("ShipWho", ShipWho);
                cmd.Parameters.AddWithValue("ShipAddr", ShipAddr);
                cmd.Parameters.AddWithValue("ShipTel", ShipTel);

                if (false == dbConn.ExecuteSql(cmd, out ErrMsg))
                {
                    ErrMsg = "單身資料表寫入失敗";
                    return false;
                }
            }

            #endregion


            //***** 將Temp部份欄位回寫至正式DT ***** 
            #region -- 處理4 --

            using (SqlCommand cmd = new SqlCommand())
            {
                //清除參數
                sql.Clear();
                cmd.Parameters.Clear();

                //----- SQL 查詢語法 -----
                //Update單頭狀態/總金額
                sql.AppendLine(" UPDATE Order_ImportData SET Status = 3, Update_Time = GETDATE() ");
                sql.AppendLine(" , TotalPrice = (SELECT SUM(BuyCnt * ERP_Price) FROM Order_ImportData_DT WHERE (Parent_ID = @DataID))");
                sql.AppendLine(" WHERE (Data_ID = @DataID);");

                //Update單身欄位
                sql.AppendLine(" UPDATE Order_ImportData_DT");
                sql.AppendLine(" SET Cust_ModelNo = Tmp.Cust_ModelNo, MOQ = Tmp.MOQ, MinQty = Tmp.MinQty");
                sql.AppendLine(" FROM Order_ImportData_TempDT Tmp");
                sql.AppendLine(" WHERE (Order_ImportData_DT.Parent_ID = @DataID)");
                sql.AppendLine("  AND (Tmp.Parent_ID = @DataID)");
                sql.AppendLine("  AND (Tmp.ERP_ModelNo = Order_ImportData_DT.ERP_ModelNo)");


                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();
                cmd.Parameters.AddWithValue("DataID", dataID);
                cmd.Parameters.AddWithValue("ShipWho", ShipWho);
                cmd.Parameters.AddWithValue("ShipAddr", ShipAddr);
                cmd.Parameters.AddWithValue("ShipTel", ShipTel);

                if (false == dbConn.ExecuteSql(cmd, out ErrMsg))
                {
                    ErrMsg = "單身資料表更新失敗";
                    return false;
                }
            }

            #endregion

            //OK
            return true;

        }


        /// <summary>
        /// 建立EDI資料 - (Step3)
        /// </summary>
        /// <param name="baseData"></param>
        /// <param name="ErrMsg"></param>
        /// <returns></returns>
        public bool Create_EDI(ImportData baseData, out string ErrMsg)
        {
            /*
             * 清空暫存TempDT
             */
            //----- 宣告 -----
            StringBuilder sql = new StringBuilder();

            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法, 建立EDI欄位 -----
                sql.AppendLine(" SELECT");
                sql.AppendLine("  Corp.Corp_ID AS XA001");  //公司別(代號)

                //型態別,1:銷貨 2:訂單 B:退貨
                sql.AppendLine("  , DT.StockStatus AS XA002");

                //單別
                sql.AppendLine("  , (SELECT Param_Value FROM PKSYS.dbo.Param_Public WHERE (Param_Kind = 'EDI_單別') AND (UPPER(Param_Name) = UPPER(Corp.DB_Name))) AS XA003");
                sql.AppendLine("  , LEFT(DT.OrderID, 40) AS XA006");    //平台單號 (訂單號)
                sql.AppendLine("  , Base.CustID AS XA007"); //客戶代號
                sql.AppendLine("  , RTRIM(Cust.MA016) AS XA008");   //ERP業務人員ID
                sql.AppendLine("  , DT.Currency AS XA009"); //幣別
                sql.AppendLine("  , CONVERT(VARCHAR(8), Base.Create_Time, 112) AS XA010");  //單據日期(抓建立日)
                sql.AppendLine("  , LEFT(DT.ERP_ModelNo, 40) AS XA011");    //品號
                sql.AppendLine("  , DT.BuyCnt AS XA012");   //數量
                sql.AppendLine("  , DT.ERP_Price AS XA013");    //單價
                sql.AppendLine("  , SW.StockType AS XA014");    //庫別
                sql.AppendLine("  , LEFT(DT.ShipAddr, 250) AS XA015");  //出貨地址
                //計算預交日的欄位
                sql.AppendLine("  , (SELECT Param_Value FROM PKSYS.dbo.Param_Public WHERE (Param_Kind = 'EDI_預交日_有庫存') AND (UPPER(Param_Name) = UPPER(Corp.DB_Name))) AS StockYes");
                sql.AppendLine("  , (SELECT Param_Value FROM PKSYS.dbo.Param_Public WHERE (Param_Kind = 'EDI_預交日_無庫存') AND (UPPER(Param_Name) = UPPER(Corp.DB_Name))) AS StockNo");
                sql.AppendLine("  , CONVERT(VARCHAR(8), Base.Create_Time, 112) AS XA017");  //生效日(抓建立日)
                sql.AppendLine("  , '1' AS XA020"); //贈備品(預設值)
                sql.AppendLine("  , 0 AS XA021");   //贈備品量(無贈品)
                sql.AppendLine("  , LEFT(DT.ShipWho, 30) AS XA022");    //收貨人
                sql.AppendLine("  , LEFT(DT.ShipTel, 20) AS XA023");    //電話
                sql.AppendLine("  , '官網線上下單' AS XA024");  //來源平台
                sql.AppendLine("  , LEFT(Base.TraceID, 20) AS XA025");  //原始單號(填入追蹤編號)
                sql.AppendLine("  , '' AS XA026"); //SKU碼(無)
                sql.AppendLine("  , '2' AS XA027"); //是否開發票(預設值)
                sql.AppendLine("  , '1' AS XA028"); //折讓或銷退(預設值)
                sql.AppendLine("  , '' AS XA031");  //單頭備註
                sql.AppendLine("  , '' AS XA032"); //單身備註
                sql.AppendLine("  , '' AS XA033");  //銷退原因
                sql.AppendLine("  , DT.StockNum");  //預交日計算用欄位
                sql.AppendLine(" FROM Order_ImportData Base WITH(NOLOCK)");
                sql.AppendLine("  INNER JOIN Order_ImportData_DT DT WITH(NOLOCK) ON Base.Data_ID = DT.Parent_ID");
                sql.AppendLine("  INNER JOIN PKSYS.dbo.Customer Cust WITH(NOLOCK) ON Base.CustID = Cust.MA001 AND Cust.DBS = Cust.DBC");
                sql.AppendLine("  INNER JOIN PKSYS.dbo.Param_Corp Corp WITH(NOLOCK) ON Cust.DBC = Corp.Corp_ID");
                sql.AppendLine("  INNER JOIN PKSYS.dbo.Customer_Data CustDT WITH(NOLOCK) ON Cust.MA001 = CustDT.Cust_ERPID");
                sql.AppendLine("  INNER JOIN PKSYS.dbo.ShippingWarehouse SW WITH(NOLOCK) ON CustDT.SWID = SW.SWID");
                sql.AppendLine(" WHERE (Base.Data_ID = @DataID);");


                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();
                cmd.Parameters.AddWithValue("DataID", baseData.Data_ID);


                /*
                 * 重新整理欄位, 計算預交日
                 * 庫存 > 0:預交日計算使用<StockYes>
                 * 庫存 < 0:預交日計算使用<StockNo>
                 * 將計算好的預交日, 插入DataSet
                 */
                using (DataTable DT = dbConn.LookupDT(cmd, out ErrMsg))
                {
                    if (DT.Rows.Count == 0)
                    {
                        ErrMsg = "沒有可匯入的資料,請確認Step3.";
                        return false;
                    }

                    //[設定參數], 預交日數
                    int BookDays_StockYes = Convert.ToInt16(DT.Rows[0]["StockYes"]);
                    int BookDays_StockNo = Convert.ToInt16(DT.Rows[0]["StockNo"]);

                    //[新增欄位] - 預交日
                    DT.Columns.Add("XA016", typeof(string));

                    for (int row = 0; row < DT.Rows.Count; row++)
                    {
                        //取得庫存量
                        int StockNum = Convert.ToInt32(DT.Rows[row]["StockNum"]);

                        //庫存判斷
                        int StockDay;
                        if (StockNum > 0)
                        {
                            StockDay = BookDays_StockYes;
                        }
                        else
                        {
                            StockDay = BookDays_StockNo;
                        }

                        //設定預交日
                        string myBookDay = CustomExtension.GetWorkDate(DateTime.Now, StockDay).ToShortDateString().ToDateString("yyyyMMdd");
                        //回寫欄位:XA016
                        DT.Rows[row]["XA016"] = myBookDay;
                    }

                    //呼叫Webservice - ws_EDI
                    ws_EDI.ws_EDI EDI = new ws_EDI.ws_EDI();

                    //傳送DataTable 給EDI
                    DT.TableName = "MyEDITable";

                    //轉成DataSet
                    DataSet myDS = new DataSet();
                    myDS.Tables.Add(DT);

                    //回傳DT (資料DataSet, Token, 測試模試(Y/N))
                    return EDI.Insert(myDS, TokenID, System.Web.Configuration.WebConfigurationManager.AppSettings["EDITestMode"], out ErrMsg);

                }
            }

        }


        /// <summary>
        /// 建立Log
        /// </summary>
        /// <param name="baseData"></param>
        /// <param name="desc">描述</param>
        /// <param name="ErrMsg"></param>
        /// <returns></returns>
        public bool Create_Log(ImportData baseData, string desc, out string ErrMsg)
        {
            //----- 宣告 -----
            StringBuilder sql = new StringBuilder();

            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                sql.AppendLine(" DECLARE @NewID AS INT ");
                sql.AppendLine(" SET @NewID = (");
                sql.AppendLine("  SELECT ISNULL(MAX(Log_ID), 0) + 1 AS NewID FROM Order_ImportData_Log");
                sql.AppendLine(" )");

                sql.AppendLine(" INSERT INTO Order_ImportData_Log( ");
                sql.AppendLine("  Log_ID, Data_ID, TraceID, Log_Desc, Create_Time");
                sql.AppendLine(" ) VALUES (");
                sql.AppendLine("  @NewID, @DataID, @TraceID, @Log_Desc, GETDATE()");
                sql.AppendLine(" );");


                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();
                cmd.Parameters.AddWithValue("DataID", baseData.Data_ID);
                cmd.Parameters.AddWithValue("TraceID", baseData.TraceID);
                cmd.Parameters.AddWithValue("Log_Desc", desc.Left(1500));

                return dbConn.ExecuteSql(cmd, out ErrMsg);
            }

        }


        #endregion


        #region -----// Delete //-----

        /// <summary>
        /// 刪除資料
        /// </summary>
        /// <param name="dataID"></param>
        /// <returns></returns>
        public bool Delete(string dataID)
        {
            //----- 宣告 -----
            StringBuilder sql = new StringBuilder();

            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                sql.AppendLine(" DELETE FROM Order_ImportData_TempDT WHERE (Parent_ID = @DataID);");
                sql.AppendLine(" DELETE FROM Order_ImportData_DT WHERE (Parent_ID = @DataID);");
                sql.AppendLine(" DELETE FROM Order_ImportData WHERE (Data_ID = @DataID);");

                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();
                cmd.Parameters.AddWithValue("DataID", dataID);

                return dbConn.ExecuteSql(cmd, out ErrMsg);
            }
        }


        /// <summary>
        /// 刪除單筆 - Step2
        /// </summary>
        /// <param name="parentID"></param>
        /// <param name="dataID"></param>
        /// <returns></returns>
        public bool Delete_Item(string parentID, string dataID)
        {
            //----- 宣告 -----
            StringBuilder sql = new StringBuilder();

            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                sql.AppendLine(" DELETE FROM Order_ImportData_TempDT");
                sql.AppendLine(" WHERE (Parent_ID = @ParentID) AND (Data_ID = @DataID)");


                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();
                cmd.Parameters.AddWithValue("ParentID", parentID);
                cmd.Parameters.AddWithValue("DataID", dataID);

                return dbConn.ExecuteSql(cmd, out ErrMsg);
            }
        }


        /// <summary>
        /// 清空暫存 - Step3
        /// </summary>
        /// <param name="dataID"></param>
        /// <returns></returns>
        public bool Delete_Temp(string dataID)
        {
            //----- 宣告 -----
            StringBuilder sql = new StringBuilder();

            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                sql.AppendLine(" DELETE FROM Order_ImportData_TempDT WHERE (Parent_ID = @DataID);");

                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();
                cmd.Parameters.AddWithValue("DataID", dataID);

                return dbConn.ExecuteSql(cmd, out ErrMsg);
            }
        }


        #endregion


        #region -----// Update //-----
        /// <summary>
        /// 更新數量 - Step2
        /// </summary>
        /// <param name="parentID"></param>
        /// <param name="dataID"></param>
        /// <param name="qty"></param>
        /// <returns></returns>
        public bool Update_Qty(string parentID, string dataID, int qty)
        {
            //----- 宣告 -----
            StringBuilder sql = new StringBuilder();

            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                sql.AppendLine(" UPDATE Order_ImportData_TempDT");
                sql.AppendLine(" SET BuyCnt = @BuyCnt");
                sql.AppendLine(" WHERE (Parent_ID = @ParentID) AND (Data_ID = @DataID)");


                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();
                cmd.Parameters.AddWithValue("ParentID", parentID);
                cmd.Parameters.AddWithValue("DataID", dataID);
                cmd.Parameters.AddWithValue("BuyCnt", qty);

                return dbConn.ExecuteSql(cmd, out ErrMsg);
            }
        }


        /// <summary>
        /// 更新所有數量 - Step2(按下一步時)
        /// </summary>
        /// <param name="parentID"></param>
        /// <param name="query"></param>
        /// <param name="ErrMsg"></param>
        /// <returns></returns>
        public bool Update_AllQty(string parentID, IQueryable<RefTempColumn> query, out string ErrMsg)
        {
            //----- 宣告 -----
            StringBuilder sql = new StringBuilder();
            int row = 0;

            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                foreach (var item in query)
                {
                    //----- SQL 查詢語法 -----
                    sql.AppendLine(" UPDATE Order_ImportData_TempDT");
                    sql.AppendLine(" SET BuyCnt = @BuyCnt_{0}".FormatThis(row));
                    sql.AppendLine(" WHERE (Parent_ID = @ParentID) AND (Data_ID = @DataID_{0});".FormatThis(row));

                    cmd.Parameters.AddWithValue("BuyCnt_" + row, item.BuyCnt);
                    cmd.Parameters.AddWithValue("DataID_" + row, item.Data_ID);

                    row++;
                }


                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();
                cmd.Parameters.AddWithValue("ParentID", parentID);


                return dbConn.ExecuteSql(cmd, out ErrMsg);
            }
        }


        /// <summary>
        /// 狀態更新為完成 - Step3 EDI完成後執行
        /// </summary>
        /// <param name="dataID"></param>
        /// <param name="ErrMsg"></param>
        /// <returns></returns>
        public bool Update_Status(string dataID, out string ErrMsg)
        {
            //----- 宣告 -----
            StringBuilder sql = new StringBuilder();

            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                sql.AppendLine(" UPDATE Order_ImportData SET Status = 4, Update_Time = GETDATE() WHERE (Data_ID = @DataID)");

                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();
                cmd.Parameters.AddWithValue("DataID", dataID);

                return dbConn.ExecuteSql(cmd, out ErrMsg);
            }
        }

        #endregion


        #region -- 取得原始資料 --

        /// <summary>
        /// 取得原始資料
        /// </summary>
        /// <param name="search">查詢</param>
        /// <returns></returns>
        private DataTable LookupRawData(Dictionary<int, string> search)
        {
            //----- 宣告 -----
            StringBuilder sql = new StringBuilder();

            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                sql.AppendLine("SELECT Tbl.* FROM (");
                sql.AppendLine(" SELECT Base.Data_ID, Base.TraceID, Base.CustID, Base.Data_Type, Base.Status");
                sql.AppendLine("   , Base.Upload_File, Base.Sheet_Name, Base.DB_Name, Base.TotalPrice");
                sql.AppendLine("   , Base.Import_Time, Base.Create_Time, Base.Update_Time");
                sql.AppendLine("   , ClsSt.Class_Name AS StatusName");
                sql.AppendLine("   , (SELECT TOP 1 RTRIM(MA002) FROM PKSYS.dbo.Customer WITH(NOLOCK) WHERE (MA001 = Base.CustID) AND (DBS = DBC)) AS CustName");
                sql.AppendLine("   , (CASE Base.Data_Type WHEN 1 THEN '客戶品號' ELSE '寶工品號' END) AS Data_TypeName");
                sql.AppendLine("   , (SELECT COUNT(*) FROM Order_ImportData_Log WHERE (Data_ID = Base.Data_ID)) AS LogCnt");
                sql.AppendLine("   , ISNULL((SELECT TOP 1 CONVERT(VARCHAR(40), Data_ID) FROM Order_ImportData WHERE (Status <> 4) AND (CustID = Base.CustID)), '') AS InCompleteID");
                sql.AppendLine(" FROM Order_ImportData Base");
                sql.AppendLine("  INNER JOIN Order_RefClass ClsSt ON Base.Status = ClsSt.Class_ID");
                sql.AppendLine(" WHERE (ClsSt.Lang = '{0}') ".FormatThis(fn_Language.PKWeb_Lang));

                /* Search */
                if (search != null)
                {
                    string filterDateType = "";

                    foreach (var item in search)
                    {
                        switch (item.Key)
                        {
                            case (int)mySearch.DataID:
                                if (!string.IsNullOrEmpty(item.Value))
                                {
                                    sql.Append(" AND (Base.Data_ID = @DataID)");

                                    cmd.Parameters.AddWithValue("DataID", item.Value);
                                }

                                break;

                            case (int)mySearch.Keyword:
                                if (!string.IsNullOrEmpty(item.Value))
                                {
                                    sql.Append(" AND (");
                                    sql.Append("    (UPPER(RTRIM(Base.TraceID)) LIKE '%' + UPPER(@Keyword) + '%')");
                                    sql.Append(" )");

                                    cmd.Parameters.AddWithValue("Keyword", item.Value);
                                }

                                break;

                            case (int)mySearch.TraceID:
                                if (!string.IsNullOrEmpty(item.Value))
                                {
                                    sql.Append(" AND (Base.TraceID = @TraceID)");

                                    cmd.Parameters.AddWithValue("TraceID", item.Value);
                                }

                                break;

                            case (int)mySearch.Status:
                                if (!string.IsNullOrEmpty(item.Value))
                                {
                                    sql.Append(" AND (Base.Status = @Status)");

                                    cmd.Parameters.AddWithValue("Status", item.Value);
                                }

                                break;

                            case (int)mySearch.CustID:
                                if (!string.IsNullOrEmpty(item.Value))
                                {
                                    sql.Append(" AND (UPPER(RTRIM(Base.CustID)) = @CustID)");

                                    cmd.Parameters.AddWithValue("CustID", item.Value);
                                }

                                break;


                            case (int)mySearch.StartDate:
                                if (!string.IsNullOrEmpty(item.Value))
                                {
                                    sql.Append(" AND ({0} >= @sDate)".FormatThis(filterDateType));

                                    cmd.Parameters.AddWithValue("sDate", item.Value.ToDateString("yyyy/MM/dd 00:00:00"));
                                }

                                break;

                            case (int)mySearch.EndDate:
                                if (!string.IsNullOrEmpty(item.Value))
                                {
                                    sql.Append(" AND ({0} <= @eDate)".FormatThis(filterDateType));

                                    cmd.Parameters.AddWithValue("eDate", item.Value.ToDateString("yyyy/MM/dd 23:59:59"));
                                }

                                break;


                        }
                    }
                }


                sql.AppendLine(") AS Tbl ");
                sql.AppendLine(" ORDER BY Tbl.Status, Tbl.Create_Time DESC");

                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();


                //----- 回傳資料 -----
                return dbConn.LookupDT(cmd, out ErrMsg);
            }
        }



        /// <summary>
        /// 取得ERP資料, 價格/庫存
        /// </summary>
        /// <param name="custID">客戶代號</param>
        /// <param name="DBS">資料庫名</param>
        /// <param name="valModelNo">品號(多筆)</param>
        /// <param name="valQty">數量(多筆)</param>
        /// <param name="StockType">庫別</param>
        /// <param name="ErrMsg"></param>
        /// <returns></returns>
        public DataTable GetErpData(string custID, string DBS, string valModelNo, string valQty, string StockType, out string ErrMsg)
        {
            //-- 取得價格 --
            DataTable myDTPrice = GetPrice(custID, DBS, valModelNo, valQty, out ErrMsg);
            if (myDTPrice == null)
            {
                return null;
            }

            //-- 取得庫存 --
            DataTable myDTStock = GetStock(DBS, StockType, valModelNo, out ErrMsg);
            if (myDTStock == null)
            {
                return null;
            }

            //-- 合併Datatable --
            DataTable dtResult = new DataTable();
            dtResult.Columns.Add("ModelNo", typeof(string));
            dtResult.Columns.Add("Model_Name_{0}".FormatThis(fn_Language.Param_Lang), typeof(string));
            dtResult.Columns.Add("Currency", typeof(string));
            dtResult.Columns.Add("UnitPrice", typeof(double));
            dtResult.Columns.Add("SpQty", typeof(int));
            dtResult.Columns.Add("SpQtyPrice", typeof(double));
            dtResult.Columns.Add("BuyQty", typeof(int));
            dtResult.Columns.Add("StockNum", typeof(int));

            var result = from dataRows1 in myDTPrice.AsEnumerable()
                         join dataRows2 in myDTStock.AsEnumerable()
                         on dataRows1.Field<string>("ModelNo") equals dataRows2.Field<string>("ModelNo") into myMix
                         from r in myMix.DefaultIfEmpty()
                         select dtResult.LoadDataRow(new object[]
             {
                dataRows1.Field<string>("ModelNo"),
                dataRows1.Field<string>("Model_Name_{0}".FormatThis(fn_Language.Param_Lang)),
                dataRows1.Field<string>("Currency"),
                dataRows1.Field<double>("UnitPrice"),
                dataRows1.Field<int>("SpQty"),
                dataRows1.Field<double>("SpQtyPrice"),
                dataRows1.Field<int?>("BuyQty"),
                r == null ? 0 : r.Field<int>("StockNum")
             }, false);

            result.CopyToDataTable();


            return dtResult;
        }


        /// <summary>
        ///  取得報價
        /// </summary>
        /// <param name="custID">客戶代號</param>
        /// <param name="DBS">資料庫名稱</param>
        /// <param name="valModelNo">品號</param>
        /// <param name="valQty">輸入數量</param>
        /// <returns></returns>
        private DataTable GetPrice(string custID, string DBS, string valModelNo, string valQty, out string ErrMsg)
        {
            //檢查是否有空值
            if (string.IsNullOrEmpty(custID) || string.IsNullOrEmpty(valModelNo) || string.IsNullOrEmpty(valQty))
            {
                ErrMsg = "資料未提供齊全";
                return null;
            }

            //判斷是否為空
            if (string.IsNullOrEmpty(valModelNo))
            {
                ErrMsg = "無法取得產品清單";
                return null;
            }

            //取得陣列資料
            string[] strAry_ID = Regex.Split(valModelNo, @"\,{1}");
            string[] strAry_Qty = Regex.Split(valQty, @"\,{1}");

            //宣告暫存清單
            List<TempParam_Item> ITempList = new List<TempParam_Item>();

            //存入暫存清單
            for (int row = 0; row < strAry_ID.Length; row++)
            {
                ITempList.Add(new TempParam_Item(strAry_ID[row], Convert.ToInt16(strAry_Qty[row])));
            }

            //過濾重複資料
            var query = from el in ITempList
                        group el by new
                        {
                            ID = el.tmp_ID,
                            Qty = el.tmp_Qty
                        } into gp
                        select new
                        {
                            ID = gp.Key.ID,
                            Qty = gp.Key.Qty
                        };


            #region -- 取得價格 --
            //設定參數
            ArrayList aryCustID = new ArrayList();
            aryCustID.Add(custID);

            ArrayList aryDBS = new ArrayList();
            aryDBS.Add(DBS);

            ArrayList aryModelNo = new ArrayList();
            ArrayList aryQty = new ArrayList();

            //宣告WebService
            ws_GetPrice.ws_GetPrice ws_GetPrice = new ws_GetPrice.ws_GetPrice();

            foreach (var item in query)
            {
                //品號
                aryModelNo.Add(item.ID);

                //數量
                aryQty.Add(item.Qty);
            }

            //取得價格資料 (DataTable)
            DataTable DT_Price = ws_GetPrice.GetQuotePrice(aryCustID.ToArray(), aryModelNo.ToArray(), aryDBS.ToArray(), aryQty.ToArray(), TokenID, out ErrMsg);
            if (DT_Price == null)
            {
                //無法取得價格
                ErrMsg = "無法取得價格";
                return null;
            }

            return DT_Price;

            #endregion

        }

        /// <summary>
        /// 取得庫存 - 多筆
        /// </summary>
        /// <param name="DBS"></param>
        /// <param name="StockType"></param>
        /// <param name="valModelNo"></param>
        /// <param name="ErrMsg"></param>
        /// <returns></returns>
        private DataTable GetStock(string DBS, string StockType, string valModelNo, out string ErrMsg)
        {
            //判斷是否為空
            if (string.IsNullOrEmpty(valModelNo))
            {
                ErrMsg = "無法取得產品清單";
                return null;
            }

            //取得陣列資料
            string[] strAry_ID = Regex.Split(valModelNo, @"\,{1}");

            //宣告暫存清單
            List<TempParam_Item> ITempList = new List<TempParam_Item>();

            //存入暫存清單
            for (int row = 0; row < strAry_ID.Length; row++)
            {
                ITempList.Add(new TempParam_Item(strAry_ID[row]));
            }

            //過濾重複資料
            var query = from el in ITempList
                        group el by new
                        {
                            ID = el.tmp_ID
                        } into gp
                        select new
                        {
                            ID = gp.Key.ID
                        };


            #region -- 取得庫存 --
            //設定參數
            ArrayList aryStockType = new ArrayList();
            aryStockType.Add(StockType);

            ArrayList aryDBS = new ArrayList();
            aryDBS.Add(DBS);

            ArrayList aryModelNo = new ArrayList();

            //宣告WebService

            ws_GetStock.ws_GetERPData ws_GetData = new ws_GetStock.ws_GetERPData();

            foreach (var item in query)
            {
                //品號
                aryModelNo.Add(item.ID);
            }

            //取得庫存資料 (DataTable)
            DataTable DT_Stock = ws_GetData.GetStock(aryStockType.ToArray(), aryModelNo.ToArray(), aryDBS.ToArray(), TokenID, out ErrMsg);
            if (DT_Stock == null)
            {
                ErrMsg = "無法取得庫存";
                return null;
            }

            return DT_Stock;

            #endregion
        }

        #endregion



        /// <summary>
        /// 暫存參數
        /// </summary>
        public class TempParam_Item
        {
            /// <summary>
            /// [參數] - 編號
            /// </summary>
            private string _tmp_ID;
            public string tmp_ID
            {
                get { return this._tmp_ID; }
                set { this._tmp_ID = value; }
            }

            /// <summary>
            /// [參數] - 數量
            /// </summary>
            private int _tmp_Qty;
            public int tmp_Qty
            {
                get { return this._tmp_Qty; }
                set { this._tmp_Qty = value; }
            }

            /// <summary>
            /// 設定參數值
            /// </summary>
            /// <param name="tmp_ID">編號</param>
            public TempParam_Item(string tmp_ID)
            {
                this._tmp_ID = tmp_ID;
            }

            /// <summary>
            /// 設定參數值
            /// </summary>
            /// <param name="tmp_ID">編號</param>
            /// <param name="tmp_Qty">數量</param>
            public TempParam_Item(string tmp_ID, int tmp_Qty)
            {
                this._tmp_ID = tmp_ID;
                this._tmp_Qty = tmp_Qty;
            }
        }

        /// <summary>
        /// 系統通用Token
        /// </summary>
        private string _TokenID;
        public string TokenID
        {
            get
            {
                return System.Web.Configuration.WebConfigurationManager.AppSettings["API_TokenID"];
            }
            set
            {
                this._TokenID = value;
            }
        }
    }
}
