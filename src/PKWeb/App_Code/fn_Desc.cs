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


    /// <summary>
    /// 共用類
    /// </summary>
    public class PubAll
    {
        /// <summary>
        /// 語系名稱描述
        /// </summary>
        /// <param name="inputValue">輸入值</param>
        /// <returns>string</returns>
        public static string LangName(string inputValue)
        {
            //檢查 - 是否為空白字串
            if (string.IsNullOrEmpty(inputValue))
                return "";

            switch (inputValue.ToUpper())
            {
                case "ZH-TW":
                    return "繁體中文";

                case "ZH-CN":
                    return "简体中文";

                default:
                    return "English";

            }
        }

        /// <summary>
        /// 區域名稱描述
        /// </summary>
        /// <param name="inputValue"></param>
        /// <returns></returns>
        public static string AreaName(string inputValue)
        {
            //檢查 - 是否為空白字串
            if (string.IsNullOrEmpty(inputValue))
                return "";

            switch (inputValue.ToUpper())
            {
                case "2":
                    return "台灣";

                case "3":
                    return "中国";

                default:
                    return "Global";

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