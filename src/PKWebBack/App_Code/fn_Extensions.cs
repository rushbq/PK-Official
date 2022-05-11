using System;
using System.Linq;
using System.Web;
using System.Text;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Web.UI.WebControls;
using System.Web.UI;
using System.Collections;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace ExtensionMethods
{
    public static class fn_Extensions
    {
        #region -- 一般功能 --
        /// <summary>
        /// 簡化string.format
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static string FormatThis(this string format, params object[] args)
        {
            return string.Format(format, args);
        }

        /// <summary>
        /// 取得Right字串
        /// </summary>
        /// <param name="inputValue">輸入字串</param>
        /// <param name="length">取得長度</param>
        /// <returns>string</returns>
        /// <example>
        /// string str = "12345";
        /// str = str.Right(3);  //345
        /// </example>
        public static string Right(this string inputValue, int length)
        {
            length = Math.Max(length, 0);

            if (inputValue.Length > length)
            {
                return inputValue.Substring(inputValue.Length - length, length);
            }
            else
            {
                return inputValue;
            }
        }

        /// <summary>
        /// 取得Left字串
        /// </summary>
        /// <param name="inputValue">輸入字串</param>
        /// <param name="length">取得長度</param>
        /// <returns>string</returns>
        /// <example>
        /// string str = "12345";
        /// str = str.Left(3);  //123
        /// </example>
        public static string Left(this string inputValue, int length)
        {
            length = Math.Max(length, 0);

            if (inputValue.Length > length)
            {
                return inputValue.Substring(0, length);
            }
            else
            {
                return inputValue;
            }
        }

        /// <summary>
        /// 取得各參數串的值
        /// </summary>
        /// <param name="str">String to process</param>
        /// <param name="OuterSeparator">Separator for each "NameValue"</param>
        /// <param name="NameValueSeparator">Separator for Name/Value splitting</param>
        /// <returns></returns>
        /// <example>
        /// string para = "param1=value1;param2=value2";
        /// NameValueCollection nv = para.ToNameValueCollection(';', '=');
        /// foreach (var item in nv)
        /// {
        ///     Response.Write(item + "<BR>");
        /// }
        /// </example>
        public static NameValueCollection ToNameValueCollection(this String inputValue, Char OuterSeparator, Char NameValueSeparator)
        {
            NameValueCollection nvText = null;
            inputValue = inputValue.TrimEnd(OuterSeparator);
            if (!String.IsNullOrEmpty(inputValue))
            {
                String[] arrStrings = inputValue.TrimEnd(OuterSeparator).Split(OuterSeparator);

                foreach (String s in arrStrings)
                {
                    Int32 posSep = s.IndexOf(NameValueSeparator);
                    String name = s.Substring(0, posSep);
                    String value = s.Substring(posSep + 1);
                    if (nvText == null)
                        nvText = new NameValueCollection();
                    nvText.Add(name, value);
                }
            }
            return nvText;
        }

        /// <summary>
        /// 檢查格式 - 是否為日期
        /// </summary>
        /// <param name="inputValue">日期</param>
        /// <returns>bool</returns>
        /// <example>
        /// string someDate = "2010/1/5";
        /// bool isDate = nonDate.IsDate();
        /// </example>
        public static bool IsDate(this string inputValue)
        {
            if (!string.IsNullOrEmpty(inputValue))
            {
                DateTime dt;
                return (DateTime.TryParse(inputValue, out dt));
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 檢查格式 - 是否為網址
        /// </summary>
        /// <param name="inputValue">網址字串</param>
        /// <returns>bool</returns>
        public static bool IsUrl(this string inputValue)
        {
            return Regex.IsMatch(inputValue, "http(s)?://([\\w-]+\\.)+[\\w-]+(/[\\w- ./?%&amp;=]*)?");
        }

        /// <summary>
        /// 檢查格式 - 是否為座標
        /// </summary>
        /// <param name="Lat">座標-Lat字串</param>
        /// <param name="Lng">座標-Lng字串</param>
        /// <returns>Boolean</returns>
        public static bool IsLatLng(string Lat, string Lng)
        {
            if (IsNumeric(Lat) & IsNumeric(Lng))
            {
                if (Math.Abs(Convert.ToDouble(Lat)) >= 0 & Math.Abs(Convert.ToDouble(Lat)) < 180 & Math.Abs(Convert.ToDouble(Lng)) >= 0 & Math.Abs(Convert.ToDouble(Lng)) < 180)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 檢查格式 - 是否為數字
        /// </summary>
        /// <param name="Expression">輸入值</param>
        /// <returns>bool</returns>
        /// <see cref="http://support.microsoft.com/kb/329488/zh-tw"/>
        public static bool IsNumeric(this object Expression)
        {
            // Variable to collect the Return value of the TryParse method.
            bool isNum;
            // Define variable to collect out parameter of the TryParse method. If the conversion fails, the out parameter is zero.
            double retNum;
            // The TryParse method converts a string in a specified style and culture-specific format to its double-precision floating point number equivalent.
            // The TryParse method does not generate an exception if the conversion fails. If the conversion passes, True is returned. If it does not, False is returned.
            isNum = Double.TryParse(Convert.ToString(Expression), System.Globalization.NumberStyles.Any, System.Globalization.NumberFormatInfo.InvariantInfo, out retNum);

            return isNum;
        }

        /// <summary>
        /// 檢查格式 - EMail
        /// </summary>
        /// <param name="inputValue">Email</param>
        /// <returns>bool</returns>
        public static bool IsEmail(this string inputValue)
        {
            // Return true if strIn is in valid e-mail format.
            return Regex.IsMatch(inputValue,
                   @"^(?("")("".+?""@)|(([0-9a-zA-Z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-zA-Z])@))" +
                   @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-zA-Z][-\w]*[0-9a-zA-Z]\.)+[a-zA-Z]{2,6}))$");
        }

        /// <summary>
        /// 日期格式化
        /// </summary>
        /// <param name="inputValue">日期字串</param>
        /// <param name="formatValue">要輸出的格式</param>
        /// <returns>string</returns>
        public static string ToDateString(this string inputValue, string formatValue)
        {
            if (string.IsNullOrEmpty(inputValue))
            {
                return "";
            }
            else
            {
                return String.Format("{0:" + formatValue + "}", Convert.ToDateTime(inputValue));
            }

        }

        /// <summary>
        /// 日期格式化 - ERP
        /// </summary>
        /// <param name="inputValue">日期字串</param>
        /// <param name="formatValue">日期間隔符號</param>
        /// <returns>string</returns>
        /// <example>原始日期:20101215</example>
        public static string ToDateString_ERP(this string inputValue, string formatValue)
        {
            if (string.IsNullOrEmpty(inputValue))
            {
                return "";
            }
            else
            {
                return String.Format("{1}{0}{2}{0}{3}"
                    , formatValue
                    , inputValue.Substring(0, 4)
                    , inputValue.Substring(4, 2)
                    , inputValue.Substring(6, 2));
            }

        }

        /// <summary>
        /// 產生隨機英數字
        /// </summary>
        /// <param name="VcodeNum">顯示幾碼</param>
        /// <returns>string</returns>
        public static string GetRndNum(int VcodeNum)
        {
            string Vchar = "a,b,c,d,e,f,g,h,i,j,k,l,m,n,o,p,q,r,s,t,u,v,w,x,y,z,A,B,C,D,E,F,G,H,I,J,K,L,M,N,O,P,Q,R,S,T,U,V,W,X,Y,Z,0,1,2,3,4,5,6,7,8,9";
            string[] VcArray = Vchar.Split(',');
            string VNum = ""; //由于字符串很短，就不用StringBuilder了
            int temp = -1; //记录上次随机数值，尽量避免生产几个一样的随机数
            //采用一个简单的算法以保证生成随机数的不同
            Random rand = new Random();
            for (int i = 1; i < VcodeNum + 1; i++)
            {
                if (temp != -1)
                {
                    rand = new Random(i * temp * unchecked((int)DateTime.Now.Ticks));
                }
                int t = rand.Next(VcArray.Length);
                if (temp != -1 && temp == t)
                {
                    return GetRndNum(VcodeNum);
                }
                temp = t;
                VNum += VcArray[t];
            }
            return VNum;
        }

        /// <summary>
        /// 取得IP
        /// </summary>
        /// <returns></returns>
        public static string GetIP()
        {
            string ip;
            string trueIP = string.Empty;
            HttpRequest req = HttpContext.Current.Request;

            //先取得是否有經過代理伺服器
            ip = req.ServerVariables["HTTP_X_FORWARDED_FOR"];

            if (!string.IsNullOrEmpty(ip))
            {
                //將取得的 IP 字串存入陣列
                string[] ipRange = ip.Split(',');

                //比對陣列中的每個 IP
                for (int i = 0; i < ipRange.Length; i++)
                {
                    //剔除內部 IP 及不合法的 IP 後，取出第一個合法 IP
                    if (ipRange[i].Trim().Substring(0, 3) != "10." &&
                        ipRange[i].Trim().Substring(0, 7) != "192.168" &&
                        ipRange[i].Trim().Substring(0, 7) != "172.16." &&
                        CheckIP(ipRange[i].Trim()))
                    {
                        trueIP = ipRange[i].Trim();
                        break;
                    }
                }

            }
            else
            {
                //沒經過代理伺服器，直接使用 ServerVariables["REMOTE_ADDR"]
                //並經過 CheckIP( ) 的驗證
                trueIP = CheckIP(req.ServerVariables["REMOTE_ADDR"]) ?
                    req.ServerVariables["REMOTE_ADDR"] : "";
            }

            return trueIP;
        }

        /// <summary>
        /// 檢查 IP 是否合法
        /// </summary>
        /// <param name="strPattern">需檢測的 IP</param>
        /// <returns>true:合法 false:不合法</returns>
        private static bool CheckIP(string strPattern)
        {
            // 繼承自：System.Text.RegularExpressions
            // regular: ^\d{1,3}[\.]\d{1,3}[\.]\d{1,3}[\.]\d{1,3}$
            Regex regex = new Regex("^\\d{1,3}[\\.]\\d{1,3}[\\.]\\d{1,3}[\\.]\\d{1,3}$");
            Match m = regex.Match(strPattern);

            return m.Success;
        }

        /// <summary>
        /// 建立Url
        /// </summary>
        /// <param name="Uri">網址</param>
        /// <param name="ParamName">參數名稱(Array)(String)</param>
        /// <param name="ParamVal">參數值(Array)(String)</param>
        /// <returns>string</returns>
        public static string CreateUrl(string Uri, Array ParamName, Array ParamVal)
        {
            //判斷網址是否為空
            if (string.IsNullOrEmpty(Uri))
            {
                return "";
            }

            //產生完整網址
            StringBuilder url = new StringBuilder();
            url.Append(Uri);

            //判斷陣列索引數是否相同
            if (ParamName.Length == ParamVal.Length)
            {
                for (int row = 0; row < ParamName.Length; row++)
                {
                    url.Append(string.Format("{0}{1}={2}"
                        , (row == 0) ? "?" : "&"
                        , ParamName.GetValue(row).ToString()
                        , HttpUtility.UrlEncode(ParamVal.GetValue(row).ToString())
                        ));
                }
            }

            return url.ToString();
        }

        /// <summary>
        /// 判斷字串內是否包含某字詞
        /// </summary>
        /// <param name="inputValue">輸入字串</param>
        /// <param name="strPool">要判斷的字詞</param>
        /// <param name="splitSymbol">Array的分割符號</param>
        /// <param name="splitNum">分割符號的數量</param>
        /// <returns></returns>
        /// <example>
        ///     string strTmp = ".jpg||.png||.pdf||.bmp";
        ///     Response.Write(fn_Extensions.CheckStrWord(src, strTmp, "|", 2));        
        /// </example>
        public static bool CheckStrWord(string inputValue, string strPool, string splitSymbol, int splitNum)
        {
            string[] strAry = Regex.Split(strPool, @"\" + splitSymbol + "{" + splitNum + "}");
            foreach (string item in strAry)
            {
                if (inputValue.IndexOf(item.ToString(), StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    return true;
                }
            }

            return false;
        }
        #endregion

        #region -- 字串驗証 --

        //================================= 字串 =================================
        public enum InputType
        {
            英文,
            數字,
            小寫英文,
            小寫英文混數字,
            小寫英文開頭混數字,
            大寫英文,
            大寫英文混數字,
            大寫英文開頭混數字
        }

        /// <summary>
        /// 驗証 - 輸入類型(文字)
        /// </summary>
        /// <param name="value">要驗証的值</param>
        /// <param name="InputType">輸入類型</param>
        /// <param name="minLength">最少字元數</param>
        /// <param name="maxLength">最大字元數</param>
        /// <param name="ErrMsg">錯誤訊息</param>
        /// <returns>Boolean</returns>
        public static bool String_輸入限制(string value, InputType InputType, string minLength, string maxLength
            , out string ErrMsg)
        {
            try
            {
                value = value.Trim();
                ErrMsg = "";

                //判斷輸入限制種類 - InputType
                switch (InputType)
                {
                    case InputType.數字:

                        return IsNumeric(value);

                    case InputType.英文:
                        for (int i = 0; i < value.Length; i++)
                        {
                            if ((System.Char.Parse(value.Substring(i, 1)) < 65 | System.Char.Parse(value.Substring(i, 1)) > 90)
                                & System.Char.Parse(value.Substring(i, 1)) < 97 | System.Char.Parse(value.Substring(i, 1)) > 122)
                            {
                                return false;
                            }
                        }

                        break;

                    case InputType.小寫英文:
                        for (int i = 0; i < value.Length; i++)
                        {
                            if ((System.Char.Parse(value.Substring(i, 1)) < 97 | System.Char.Parse(value.Substring(i, 1)) > 122))
                            {
                                return false;
                            }
                        }

                        break;

                    case InputType.小寫英文混數字:
                        for (int i = 0; i < value.Length; i++)
                        {
                            if ((System.Char.Parse(value.Substring(i, 1)) < 97 | System.Char.Parse(value.Substring(i, 1)) > 122)
                                & (System.Char.Parse(value.Substring(i, 1)) < 48 | System.Char.Parse(value.Substring(i, 1)) > 57))
                            {
                                return false;
                            }
                        }

                        break;

                    case InputType.小寫英文開頭混數字:
                        for (int i = 0; i < value.Length; i++)
                        {
                            if (i == 0)
                            {
                                if ((System.Char.Parse(value.Substring(i, 1)) < 97 | System.Char.Parse(value.Substring(i, 1)) > 122))
                                {
                                    return false;
                                }
                            }
                            else
                            {
                                if ((System.Char.Parse(value.Substring(i, 1)) < 97 | System.Char.Parse(value.Substring(i, 1)) > 122)
                                    & (System.Char.Parse(value.Substring(i, 1)) < 48 | System.Char.Parse(value.Substring(i, 1)) > 57))
                                {
                                    return false;
                                }
                            }
                        }

                        break;

                    case InputType.大寫英文:
                        for (int i = 0; i < value.Length; i++)
                        {
                            if ((System.Char.Parse(value.Substring(i, 1)) < 65 | System.Char.Parse(value.Substring(i, 1)) > 90))
                            {
                                return false;
                            }
                        }

                        break;

                    case InputType.大寫英文混數字:
                        for (int i = 0; i < value.Length; i++)
                        {
                            if ((System.Char.Parse(value.Substring(i, 1)) < 65 | System.Char.Parse(value.Substring(i, 1)) > 90)
                                & (System.Char.Parse(value.Substring(i, 1)) < 48 | System.Char.Parse(value.Substring(i, 1)) > 57))
                            {
                                return false;
                            }
                        }

                        break;

                    case InputType.大寫英文開頭混數字:
                        for (int i = 0; i < value.Length; i++)
                        {
                            if (i == 0)
                            {
                                if ((System.Char.Parse(value.Substring(i, 1)) < 65 | System.Char.Parse(value.Substring(i, 1)) > 90))
                                {
                                    return false;
                                }
                            }
                            else
                            {
                                if ((System.Char.Parse(value.Substring(i, 1)) < 65 | System.Char.Parse(value.Substring(i, 1)) > 90)
                                    & (System.Char.Parse(value.Substring(i, 1)) < 48 | System.Char.Parse(value.Substring(i, 1)) > 57))
                                {
                                    return false;
                                }
                            }
                        }

                        break;
                }

                //檢查字數是不是小於 minLength
                if (IsNumeric(minLength))
                {
                    if (value.Length < Math.Floor(Convert.ToDouble(minLength)))
                    {
                        ErrMsg = "字數小於 minLength：" + Math.Floor(Convert.ToDouble(minLength));
                        return false;
                    }
                }
                //檢查字數是不是大於 maxLength
                if (IsNumeric(maxLength))
                {
                    if (value.Length > Math.Floor(Convert.ToDouble(maxLength)))
                    {
                        ErrMsg = "字數大於 maxLength：" + maxLength;
                        return false;
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                ErrMsg = "Exception：" + ex.Message.ToString();
                return false;

            }
        }

        /// <summary>
        /// 驗証 - 輸入字數(文字)
        /// </summary>
        /// <param name="value">要驗証的值</param>
        /// <param name="minLength">最少字元數</param>
        /// <param name="maxLength">最大字元數</param>
        /// <param name="ErrMsg">錯誤訊息</param>
        /// <returns>Boolean</returns>
        public static bool String_字數(string value, string minLength, string maxLength, out string ErrMsg)
        {
            try
            {
                value = value.Trim();
                ErrMsg = "";

                //檢查字數是不是小於 minLength
                if (IsNumeric(minLength))
                {
                    if (value.Length < Math.Floor(Convert.ToDouble(minLength)))
                    {
                        ErrMsg = "字數小於 minLength：" + Math.Floor(Convert.ToDouble(minLength));
                        return false;
                    }
                }
                //檢查字數是不是大於 maxLength
                if (IsNumeric(maxLength))
                {
                    if (value.Length > Math.Floor(Convert.ToDouble(maxLength)))
                    {
                        ErrMsg = "字數大於 maxLength：" + maxLength;
                        return false;
                    }
                }

                return true;

            }
            catch (Exception ex)
            {
                ErrMsg = "Exception：" + ex.Message.ToString();
                return false;
            }
        }

        /// <summary>
        /// 驗証 - 輸入字數(byte)(文字)
        /// </summary>
        /// <param name="value">要驗証的值</param>
        /// <param name="minLength">最少字元數</param>
        /// <param name="maxLength">最大字元數</param>
        /// <param name="ErrMsg">錯誤訊息</param>
        /// <returns>Boolean</returns>
        public static bool String_資料長度Byte(string value, string minLength, string maxLength, out string ErrMsg)
        {
            try
            {
                value = value.Trim();
                ErrMsg = "";

                double valueByteLength = System.Text.Encoding.Default.GetBytes(value).Length;
                //檢查資料長度(Byte)是不是小於 minLength
                if (IsNumeric(minLength))
                {
                    if (valueByteLength < Math.Floor(Convert.ToDouble(minLength)))
                    {
                        ErrMsg = "資料長度(Byte)小於 minLength：" + Math.Floor(Convert.ToDouble(minLength));
                        return false;
                    }
                }
                //檢查資料長度(Byte)是不是大於 maxLength
                if (IsNumeric(maxLength))
                {
                    if (valueByteLength > Math.Floor(Convert.ToDouble(maxLength)))
                    {
                        ErrMsg = "資料長度(Byte)大於 maxLength：" + maxLength;
                        return false;
                    }
                }

                return true;

            }
            catch (Exception ex)
            {
                ErrMsg = "Exception：" + ex.Message.ToString();
                return false;

            }
        }

        //================================ 日期時間 ==============================
        /// <summary>
        /// 驗証 - 日期
        /// </summary>
        /// <param name="value">要驗証的值</param>
        /// <param name="minDate">最小日期</param>
        /// <param name="maxDate">最大日期</param>
        /// <param name="ErrMsg">錯誤訊息</param>
        /// <returns>Boolean</returns>
        public static bool DateTime_日期(string value, string minDate, string maxDate, out string ErrMsg)
        {
            try
            {
                DateTime dtResult;
                ErrMsg = "";
                value = value.Trim();
                minDate = minDate.Trim();
                maxDate = maxDate.Trim();
                //檢查是不是時間
                if (DateTime.TryParse(value, out dtResult) == false | string.IsNullOrEmpty(value))
                {
                    ErrMsg = "不是日期資料";
                    return false;
                }
                //檢查是不是小於 minDate
                if (DateTime.TryParse(minDate, out dtResult) & !string.IsNullOrEmpty(minDate))
                {
                    if (Convert.ToDateTime(value) < Convert.ToDateTime(minDate))
                    {
                        ErrMsg = "小於 minDate：" + string.Format(minDate, "yyyy-MM-dd");
                        return false;
                    }
                }
                //檢查是不是小於 maxDate
                if (DateTime.TryParse(maxDate, out dtResult) & !string.IsNullOrEmpty(maxDate))
                {
                    if (Convert.ToDateTime(value) > Convert.ToDateTime(maxDate))
                    {
                        ErrMsg = "大於 maxDate：" + string.Format(maxDate, "yyyy-MM-dd");
                        return false;
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                ErrMsg = "Exception：" + ex.Message.ToString();
                return false;

            }
        }

        /// <summary>
        /// 驗証 - 時間
        /// </summary>
        /// <param name="value">要驗証的值</param>
        /// <param name="minDateTime">最小時間</param>
        /// <param name="maxDateTime">最大時間</param>
        /// <param name="ErrMsg">錯誤訊息</param>
        /// <returns>Boolean</returns>
        public static bool DateTime_時間(string value, string minDateTime, string maxDateTime, out string ErrMsg)
        {
            try
            {
                DateTime dtResult;
                ErrMsg = "";
                value = value.Trim();
                minDateTime = minDateTime.Trim();
                maxDateTime = maxDateTime.Trim();
                //檢查是不是時間
                if (DateTime.TryParse(value, out dtResult) == false | string.IsNullOrEmpty(value))
                {
                    ErrMsg = "不是時間資料";
                    return false;
                }
                //檢查是不是小於 minDateTime
                if (DateTime.TryParse(minDateTime, out dtResult) & !string.IsNullOrEmpty(minDateTime))
                {
                    if (Convert.ToDateTime(value) < Convert.ToDateTime(minDateTime))
                    {
                        ErrMsg = "小於 minDateTime：" + string.Format(minDateTime, "yyyy-MM-dd HH:mm:ss.fff");
                        return false;
                    }
                }
                //檢查是不是小於 maxDateTime
                if (DateTime.TryParse(maxDateTime, out dtResult) & !string.IsNullOrEmpty(maxDateTime))
                {
                    if (Convert.ToDateTime(value) > Convert.ToDateTime(maxDateTime))
                    {
                        ErrMsg = "大於 maxDateTime：" + string.Format(maxDateTime, "yyyy-MM-dd HH:mm:ss.fff");
                        return false;
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                ErrMsg = "Exception：" + ex.Message.ToString();
                return false;

            }
        }

        //================================= 數值 =================================
        /// <summary>
        /// 驗証 - 數字(正整數)
        /// </summary>
        /// <param name="value">要驗証的值</param>
        /// <param name="minValue">最小數值</param>
        /// <param name="maxValue">最大數值</param>
        /// <param name="ErrMsg">錯誤訊息</param>
        /// <returns>Boolean</returns>
        public static bool Num_正整數(string value, string minValue, string maxValue, out string ErrMsg)
        {
            try
            {
                value = value.Trim();
                ErrMsg = "";

                //檢查是不是數值
                if (IsNumeric(value) == false)
                {
                    ErrMsg = "不是數值";
                    return false;
                }
                //檢查是不是大於零
                if (Convert.ToDouble(value) < 0)
                {
                    ErrMsg = "小於 0";
                    return false;
                }
                //檢查是不是整數
                if (Convert.ToDouble(value) != Math.Floor(Convert.ToDouble(value)))
                {
                    ErrMsg = "正數非正整數";
                    return false;
                }
                //檢查是不是小於 minValue
                if (IsNumeric(minValue))
                {
                    if (Convert.ToDouble(value) < Convert.ToDouble(minValue))
                    {
                        ErrMsg = "小於 minValue：" + minValue;
                        return false;
                    }
                }
                //檢查是不是大於 maxValue
                if (IsNumeric(maxValue))
                {
                    if (Convert.ToDouble(value) > Convert.ToDouble(maxValue))
                    {
                        ErrMsg = "大於 maxValue：" + maxValue;
                        return false;
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                ErrMsg = "Exception：" + ex.Message.ToString();
                return false;

            }
        }

        /// <summary>
        /// 驗証 - 數字(負整數)
        /// </summary>
        /// <param name="value">要驗証的值</param>
        /// <param name="minValue">最小數值</param>
        /// <param name="maxValue">最大數值</param>
        /// <param name="ErrMsg">錯誤訊息</param>
        /// <returns>Boolean</returns>
        public static bool Num_負整數(string value, string minValue, string maxValue, out string ErrMsg)
        {
            try
            {
                value = value.Trim();
                ErrMsg = "";

                //檢查是不是數值
                if (IsNumeric(value) == false)
                {
                    ErrMsg = "不是數值";
                    return false;
                }
                //檢查是不是大於零
                if (Convert.ToDouble(value) > 0)
                {
                    ErrMsg = "大於 0";
                    return false;
                }
                //檢查是不是整數
                if (Convert.ToDouble(value) != Math.Floor(Convert.ToDouble(value)))
                {
                    ErrMsg = "負數非負整數";
                    return false;
                }
                //檢查是不是小於 minValue
                if (IsNumeric(minValue))
                {
                    if (Convert.ToDouble(value) < Convert.ToDouble(minValue))
                    {
                        ErrMsg = "小於 minValue：" + minValue;
                        return false;
                    }
                }
                //檢查是不是大於 maxValue
                if (IsNumeric(maxValue))
                {
                    if (Convert.ToDouble(value) > Convert.ToDouble(maxValue))
                    {
                        ErrMsg = "大於 maxValue：" + maxValue;
                        return false;
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                ErrMsg = "Exception：" + ex.Message.ToString();
                return false;

            }
        }

        /// <summary>
        /// 驗証 - 數字(正數)
        /// </summary>
        /// <param name="value">要驗証的值</param>
        /// <param name="minValue">最小數值</param>
        /// <param name="maxValue">最大數值</param>
        /// <param name="ErrMsg">錯誤訊息</param>
        /// <returns>Boolean</returns>
        public static bool Num_正數(string value, string minValue, string maxValue, out string ErrMsg)
        {
            try
            {
                value = value.Trim();
                ErrMsg = "";

                //檢查是不是數值
                if (IsNumeric(value) == false)
                {
                    ErrMsg = "不是數值";
                    return false;
                }
                //檢查是不是大於零
                if (Convert.ToDouble(value) < 0)
                {
                    ErrMsg = "小於 0";
                    return false;
                }
                //檢查是不是小於 minValue
                if (IsNumeric(minValue))
                {
                    if (Convert.ToDouble(value) < Convert.ToDouble(minValue))
                    {
                        ErrMsg = "小於 minValue：" + minValue;
                        return false;
                    }
                }
                //檢查是不是大於 maxValue
                if (IsNumeric(maxValue))
                {
                    if (Convert.ToDouble(value) > Convert.ToDouble(maxValue))
                    {
                        ErrMsg = "大於 maxValue：" + maxValue;
                        return false;
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                ErrMsg = "Exception：" + ex.Message.ToString();
                return false;

            }
        }

        /// <summary>
        /// 驗証 - 數字(負數)
        /// </summary>
        /// <param name="value">要驗証的值</param>
        /// <param name="minValue">最小數值</param>
        /// <param name="maxValue">最大數值</param>
        /// <param name="ErrMsg">錯誤訊息</param>
        /// <returns>Boolean</returns>
        public static bool Num_負數(string value, string minValue, string maxValue, out string ErrMsg)
        {
            try
            {
                value = value.Trim();
                ErrMsg = "";

                //檢查是不是數值
                if (IsNumeric(value) == false)
                {
                    ErrMsg = "不是數值";
                    return false;
                }
                //檢查是不是大於零
                if (Convert.ToDouble(value) > 0)
                {
                    ErrMsg = "大於 0";
                    return false;
                }
                //檢查是不是小於 minValue
                if (IsNumeric(minValue))
                {
                    if (Convert.ToDouble(value) < Convert.ToDouble(minValue))
                    {
                        ErrMsg = "小於 minValue：" + minValue;
                        return false;
                    }
                }
                //檢查是不是大於 maxValue
                if (IsNumeric(maxValue))
                {
                    if (Convert.ToDouble(value) > Convert.ToDouble(maxValue))
                    {
                        ErrMsg = "大於 maxValue：" + maxValue;
                        return false;
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                ErrMsg = "Exception：" + ex.Message.ToString();
                return false;

            }
        }


        #endregion

        #region -- 常用功能 --
        /// <summary>
        /// 使用HttpWebRequest取得網頁資料
        /// </summary>
        /// <param name="url">網址</param>
        /// <returns>string</returns>
        public static string WebRequest_GET(string url)
        {
            try
            {
                Encoding myEncoding = Encoding.GetEncoding("UTF-8");
                HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(url);

                //IIS為AD驗証時加入此段 Start
                req.UseDefaultCredentials = true;
                req.PreAuthenticate = true;
                req.Credentials = CredentialCache.DefaultCredentials;
                //IIS為AD驗証時加入此段 End

                req.Method = "GET";
                using (WebResponse wr = req.GetResponse())
                {
                    using (StreamReader myStreamReader = new StreamReader(wr.GetResponseStream(), myEncoding))
                    {
                        return myStreamReader.ReadToEnd();
                    }
                }
            }
            catch (Exception)
            {
                return null;
            }

        }

        /// <summary>
        /// 使用HttpWebRequest POST取得網頁資料
        /// </summary>
        /// <param name="url">網址</param>
        /// <param name="param">參數 (a=123&b=456)</param>
        /// <returns></returns>
        public static string WebRequest_POST(string url, string param, bool ADMode)
        {
            try
            {
                byte[] bs = Encoding.ASCII.GetBytes(param);

                Encoding myEncoding = Encoding.GetEncoding("UTF-8");
                HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(url);

                //IIS為AD驗証時加入此段 Start
                if (ADMode)
                {
                    req.UseDefaultCredentials = true;
                    req.PreAuthenticate = true;
                    req.Credentials = CredentialCache.DefaultCredentials;
                }
                //IIS為AD驗証時加入此段 End

                req.Method = "POST";
                req.ContentType = "application/x-www-form-urlencoded";
                req.ContentLength = bs.Length;

                using (Stream reqStream = req.GetRequestStream())
                {
                    reqStream.Write(bs, 0, bs.Length);
                }
                using (WebResponse wr = req.GetResponse())
                {
                    using (StreamReader myStreamReader = new StreamReader(wr.GetResponseStream(), myEncoding))
                    {
                        return myStreamReader.ReadToEnd();
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// 使用FileStream取得資料
        /// </summary>
        /// <param name="path">磁碟路徑</param>
        /// <returns>string</returns>
        public static string IORequest_GET(string path)
        {
            try
            {
                if (false == System.IO.File.Exists(path)) return "";
                using (FileStream fs = new FileStream(path, FileMode.Open))
                {
                    using (StreamReader sw = new StreamReader(fs, System.Text.Encoding.UTF8))
                    {
                        return sw.ReadToEnd();
                    }
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        #endregion

        #region -- 其他 --
        /// <summary>
        /// Javascript - Alert與Redirect
        /// </summary>
        /// <param name="alertMessage">警示訊息</param>
        /// <param name="hrefUrl">導向頁面或JS語法</param>
        /// <remarks>
        /// 使用JS語法須加入 script: 以判斷
        /// </remarks>
        public static void JsAlert(string alertMessage, string hrefUrl)
        {
            try
            {
                StringBuilder sbJs = new StringBuilder();
                //警示訊息
                if (false == string.IsNullOrEmpty(alertMessage))
                {
                    sbJs.Append(string.Format("alert('{0}');", alertMessage));
                }
                //判斷是頁面還是語法
                if (false == string.IsNullOrEmpty(hrefUrl))
                {
                    if (hrefUrl.IndexOf("script:") != -1)
                    {
                        sbJs.Append(hrefUrl.Replace("script:", ""));
                    }
                    else
                    {
                        sbJs.Append(string.Format("location.href=\'{0}\';", hrefUrl));
                    }
                }
                ScriptManager.RegisterClientScriptBlock((Page)HttpContext.Current.Handler, typeof(string), "js", sbJs.ToString(), true);
                return;
            }
            catch (Exception)
            {
                return;
            }
        }

        /// <summary>
        /// SQL參數組合 - Where IN
        /// </summary>
        /// <param name="listSrc">來源資料(List)</param>
        /// <param name="paramName">參數名稱</param>
        /// <returns>參數字串</returns>
        public static string GetSQLParam(List<string> listSrc, string paramName)
        {
            if (listSrc.Count == 0)
            {
                return "";
            }

            //組合參數字串
            ArrayList aryParam = new ArrayList();
            for (int row = 0; row < listSrc.Count; row++)
            {
                aryParam.Add(string.Format("@{0}{1}", paramName, row));
            }
            //回傳以 , 為分隔符號的字串
            return string.Join(",", aryParam.ToArray());
        }
        #endregion

        
    }

}