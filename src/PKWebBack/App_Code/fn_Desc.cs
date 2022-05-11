using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// 各功能的中英文描述轉換
/// </summary>
public class fn_Desc
{
    /// <summary>
    /// Login
    /// </summary>
    public class Login
    {
        /// <summary>
        /// 登入錯誤碼
        /// </summary>
        /// <param name="inputValue">輸入值</param>
        /// <returns>string</returns>
        public static string ErrCode(string inputValue)
        {
            switch (inputValue.ToUpper())
            {
                case "1001":
                    return "未登入網域或帳號未建立";

                case "1002":
                    return "帳號未建立";

                case "2001":
                    return "帳密錯誤或帳號未建立";

                case "2002":
                    return "帳號已被停用";

                default:
                    return "請確認帳密是否正確";
            }
        }

    }

    public class MemberInfo
    {

        /// <summary>
        /// 會員身份
        /// </summary>
        /// <param name="inputValue"></param>
        /// <returns></returns>
        public static string MemberType(string inputValue)
        {
            //檢查 - 是否為空白字串
            if (string.IsNullOrEmpty(inputValue))
                return "";

            switch (inputValue.ToUpper())
            {
                case "0":
                    return "<i class=\"fa fa-user fa-fw\"></i>&nbsp;一般使用者";

                case "1":
                    return "<i class=\"fa fa-users fa-fw\"></i>&nbsp;經銷商";

                default:
                    return "";
            }
        }

        /// <summary>
        /// 經銷商審核狀態
        /// </summary>
        /// <param name="inputValue">輸入值</param>
        /// <returns>string</returns>
        public static string DealerStatus(string inputValue)
        {
            //檢查 - 是否為空白字串
            if (string.IsNullOrEmpty(inputValue))
                return "";

            switch (inputValue.ToUpper())
            {
                case "Y":
                    return "已核准";

                case "N":
                    return "未申請";

                case "S":
                    return "待核准";

                case "R":
                    return "駁回申請";

                default:
                    return "";
            }
        }
    }


    /// <summary>
    /// 共用類
    /// </summary>
    public class PubAll
    {
        /// <summary>
        /// 是否
        /// </summary>
        /// <param name="inputValue">輸入值</param>
        /// <returns>string</returns>
        public static string YesNo(string inputValue)
        {
            //檢查 - 是否為空白字串
            if (string.IsNullOrEmpty(inputValue))
                return "";

            switch (inputValue.ToUpper())
            {
                case "Y":
                    return "是";

                case "N":
                    return "否";

                default:
                    return "";
            }
        }

        /// <summary>
        /// News Type
        /// </summary>
        /// <param name="inputValue">輸入值</param>
        /// <returns>string</returns>
        public static string NewsType(string inputValue)
        {
            //檢查 - 是否為空白字串
            if (string.IsNullOrEmpty(inputValue))
                return "";

            switch (inputValue.ToUpper())
            {
                case "1":
                    return "News";

                case "2":
                    return "Event";

                default:
                    return "";
            }
        }

        /// <summary>
        /// 性別
        /// </summary>
        /// <param name="inputValue"></param>
        /// <returns></returns>
        public static string Sex(string inputValue)
        {
            //檢查 - 是否為空白字串
            if (string.IsNullOrEmpty(inputValue))
                return "";

            switch (inputValue.ToUpper())
            {
                case "1":
                    return "男";

                case "2":
                    return "女";

                default:
                    return "";
            }
        }

    }

}