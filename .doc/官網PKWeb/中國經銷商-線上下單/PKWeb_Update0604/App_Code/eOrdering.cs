using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace eOrder.Models
{
    /// <summary>
    /// 單頭資料欄位
    /// </summary>
    public class ImportData
    {
        public int SeqNo { get; set; }
        public Guid Data_ID { get; set; }
        public string TraceID { get; set; }
        public string CustID { get; set; }
        public decimal Data_Type { get; set; }
        public decimal Status { get; set; }
        public string Upload_File { get; set; }
        public string Sheet_Name { get; set; }
        public string DB_Name { get; set; }
        public double TotalPrice { get; set; }
        public string Import_Time { get; set; }
        public string Create_Time { get; set; }
        public string Update_Time { get; set; }

        public string CustName { get; set; }
        public string StatusName { get; set; }
        public string Data_TypeName { get; set; }
        public string Remark { get; set; }

        public int LogCnt { get; set; }
        public string InCompleteID { get; set; }

    }


    /// <summary>
    /// 暫存單身資料欄
    /// </summary>
    public class RefTempColumn
    {
        public int Data_ID { get; set; }
        //EXCEL中的品號(未檢查)
        public string ProdID { get; set; }
        public string Cust_ModelNo { get; set; }
        public string ERP_ModelNo { get; set; }
        public int InputCnt { get; set; }
        public int BuyCnt { get; set; }
        public int MOQ { get; set; }
        public int MinQty { get; set; }
        public float UnitPrice { get; set; }
        public string IsPass { get; set; }
        public string doWhat { get; set; }
    }


    /// <summary>
    /// 單身資料欄
    /// </summary>
    public class RefColumn
    {
        public int Data_ID { get; set; }
        public string OrderID { get; set; }
        public string Cust_ModelNo { get; set; }
        public string ERP_ModelNo { get; set; }
        public double ERP_Price { get; set; }
        public int BuyCnt { get; set; }
        public int MOQ { get; set; }
        public int MinQty { get; set; }
        public string Currency { get; set; }
        public string ShipWho { get; set; }
        public string ShipAddr { get; set; }
        public string ShipTel { get; set; }
        public int StockNum { get; set; }
        public string StockStatus { get; set; }
    }


    /// <summary>
    /// Mail
    /// </summary>
    public class MailList
    {
        public string MailAddress { get; set; }
    }
}
