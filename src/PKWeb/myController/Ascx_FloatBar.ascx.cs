using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using ExtensionMethods;
using Resources;

public partial class Ascx_FloatBar : System.Web.UI.UserControl
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            string WebUrl = Application["WebUrl"].ToString();

            //產生選單
            List<TabMenu> listTab = new List<TabMenu>();
            listTab.Add(new TabMenu("1", "{0}D-Download/Photo".FormatThis(WebUrl), resPublic.bar_產品圖片, "fa-picture-o", false));
            listTab.Add(new TabMenu("2", "{0}D-Download/Package".FormatThis(WebUrl), resPublic.bar_包材檔案, "fa-archive", false));
            listTab.Add(new TabMenu("3", "{0}D-Download/Certification".FormatThis(WebUrl), resPublic.bar_認證資料, "fa-check-square-o", false));
            //listTab.Add(new TabMenu("4", "{0}D-Download/Specification".FormatThis(WebUrl), resPublic.bar_產品規格書, "fa-book", false));
            listTab.Add(new TabMenu("5", historyUrl(309), resPublic.bar_形象識別, "fa-clone", true));
            listTab.Add(new TabMenu("6", historyUrl(308), resPublic.bar_燈片圖庫, "fa-map-o", true));
            listTab.Add(new TabMenu("7", historyUrl(307), resPublic.bar_銷售工具包, "fa-object-ungroup", true));

            //宣告Html
            StringBuilder sbTab = new StringBuilder();

            //判斷要產生的選單類型
            if (Param_MenuType.ToLower().Equals("dropdownmenu"))
            {
                //下拉選單
                sbTab.AppendLine("<div class=\"btn-group\">");
                sbTab.Append("<button type=\"button\" class=\"btn btn-default dropdown-toggle\" data-toggle=\"dropdown\" aria-haspopup=\"true\" aria-expanded=\"false\">{0}&nbsp;<span class=\"caret\"></span></button>"
                    .FormatThis(resPublic.bar_功能項目));

                sbTab.Append("<ul class=\"dropdown-menu dropdown-menu-right\" role=\"menu\">");

                for (int row = 0; row < listTab.Count; row++)
                {
                    sbTab.Append("<li><a href=\"{0}\" target=\"{3}\"><i class=\"fa fa-fw {2}\"></i>{1}</a></li>".FormatThis(
                        listTab[row].TabUrl
                        , listTab[row].TabName
                        , listTab[row].TabIcon
                        , listTab[row].TabNewOpen ? "_blank" : "_self"
                        ));
                }
                sbTab.Append("</ul>");
                sbTab.AppendLine("</div>");
            }
            else
            {
                //浮動選單
                sbTab.AppendLine("<div class=\"list\">");
                sbTab.Append("<ul>");
                sbTab.Append(" <li class=\"open-up\">");
                sbTab.Append(" <a href=\"javascript:void(0)\"><span class=\"title-name\">{0}</span><span class=\"icon arrow\"><i class=\"fa fa-exchange fa-lg fa-fw\"></i></span></a>"
                    .FormatThis(resPublic.bar_功能項目));
                sbTab.Append(" </li>");

                for (int row = 0; row < listTab.Count; row++)
                {
                    sbTab.Append("<li class=\"{0}\"><a href=\"{1}\" title=\"{2}\" target=\"{4}\"><span>{2}</span><span class=\"icon\"><i class=\"fa fa-fw fa-lg text-gray {3}\"></i></span></a></li>".FormatThis(
                        listTab[row].TabIndex.Equals(Param_CurrItem) ? "in" : ""
                        , listTab[row].TabUrl
                        , listTab[row].TabName
                        , listTab[row].TabIcon
                        , listTab[row].TabNewOpen ? "_blank" : "_self"
                        ));
                }

                sbTab.Append("</ul>");
                sbTab.AppendLine("</div>");
            }

            //輸出Html
            this.lt_bar.Text = sbTab.ToString();
        }
    }

    /// <summary>
    /// 舊版經銷商資料網址
    /// </summary>
    /// <param name="menuID"></param>
    /// <returns></returns>
    public string historyUrl(int menuID)
    {
        string getMenuUri = "{0}Redirect.aspx?ActType=log&mu={1}&rt={2}".FormatThis(
                 Application["WebUrl"]
                 , menuID
                 , HttpUtility.UrlEncode(
                     "http://w3.prokits.com.tw/CrossPage/CheckLogin.asp?l={0}&m={1}&dealerID={2}&code={3}".FormatThis(
                         fn_Language.PKWeb_Lang
                         , menuID
                         , Param_DealerID
                         , Param_DealerMD5
                     )
                 )
             );

        return getMenuUri;
    }

    /// <summary>
    /// 經銷商代號
    /// </summary>
    private string _Param_DealerID;
    public string Param_DealerID
    {
        get
        {
            return fn_Member.GetDealerID(fn_Param.MemberID);
        }
        set
        {
            this._Param_DealerID = value;
        }
    }

    private string _Param_DealerMD5;
    public string Param_DealerMD5
    {
        get
        {

            string dateNow = DateTime.Today.ToShortDateString().ToDateString("yyyyMMdd");
            string md5 = Cryptograph.MD5(Param_DealerID + dateNow);

            return md5;
        }
        set
        {
            this._Param_DealerMD5 = value;
        }
    }


    /// <summary>
    /// [參數] - 目前選項
    /// </summary>
    private string _Param_CurrItem;
    public string Param_CurrItem
    {
        get;
        set;
    }

    /// <summary>
    /// [參數] - 選單類型
    /// </summary>
    private string _Param_MenuType;
    public string Param_MenuType
    {
        get;
        set;
    }

    /// <summary>
    /// 選單
    /// </summary>
    public class TabMenu
    {
        /// <summary>
        /// [參數] - Tab位置
        /// </summary>
        private string _TabIndex;
        public string TabIndex
        {
            get { return this._TabIndex; }
            set { this._TabIndex = value; }
        }

        /// <summary>
        /// [參數] - Tab連結
        /// </summary>
        private string _TabUrl;
        public string TabUrl
        {
            get { return this._TabUrl; }
            set { this._TabUrl = value; }
        }

        /// <summary>
        /// [參數] - Tab名稱
        /// </summary>
        private string _TabName;
        public string TabName
        {
            get { return this._TabName; }
            set { this._TabName = value; }
        }


        /// <summary>
        /// [參數] - Tab Icon
        /// </summary>
        private string _TabIcon;
        public string TabIcon
        {
            get { return this._TabIcon; }
            set { this._TabIcon = value; }
        }

        private bool _TabNewOpen;
        public bool TabNewOpen
        {
            get { return this._TabNewOpen; }
            set { this._TabNewOpen = value; }
        }

        /// <summary>
        /// 設定參數值
        /// </summary>
        /// <param name="TabIndex">Tab位置</param>
        /// <param name="TabUrl">Tab連結</param>
        /// <param name="TabName">Tab名稱</param>
        /// <param name="TabIcon">Tab Icon</param>
        /// <param name="TabNewOpen">是否開新視窗</param>
        public TabMenu(string TabIndex, string TabUrl, string TabName, string TabIcon, bool TabNewOpen)
        {
            this._TabIndex = TabIndex;
            this._TabUrl = TabUrl;
            this._TabName = TabName;
            this._TabIcon = TabIcon;
            this._TabNewOpen = TabNewOpen;
        }
    }
}