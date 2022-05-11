<%@ Page Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="Message.aspx.cs" Inherits="Message" %>

<%@ Register Src="~/myController/Ascx_Navi.ascx" TagName="Ascx_Navi" TagPrefix="ucNavi" %>
<asp:Content ID="myCss" ContentPlaceHolderID="CssContent" runat="Server">
    <%: Styles.Render("~/bundles/member-css") %>

    <%-- 自訂meta(中繼標記) --%>
    <meta name="robots" content="noindex" />
    <meta name="googlebot" content="noindex" />
</asp:Content>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="Server">
    <div class="container df-container-margin">
        <!-- 路徑導航 Start -->
        <ucNavi:Ascx_Navi ID="Ascx_Navi1" runat="server" Param_CurrID="606" />
        <!-- 路徑導航 End -->
        <div class="row">
            <div class="col-xs-12">
                <%-- 其他處理 --%>
                <asp:PlaceHolder ID="ph_message" runat="server" Visible="false">
                    <div class="page-title">
                        <div class="header"><%=this.GetLocalResourceObject("title_oops").ToString()%></div>
                    </div>
                    <div style="line-height: 2em; font-size: 16px;">
                        <%=this.GetLocalResourceObject("msg_頁面不存在").ToString()%><br />
                        <%=Req_LastUrl %>
                        <p></p>
                        <%=this.GetLocalResourceObject("msg_發生錯誤").ToString()%><br />
                        <a href="<%=Application["WebUrl"] %>"><i class="fa fa-home"></i>&nbsp;<%=this.GetLocalResourceObject("url_回到首頁").ToString()%></a>
                    </div>
                </asp:PlaceHolder>

            </div>
        </div>

    </div>
</asp:Content>
<asp:Content ID="myScript" ContentPlaceHolderID="ScriptContent" runat="Server">
</asp:Content>

