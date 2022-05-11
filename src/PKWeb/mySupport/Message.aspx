<%@ Page Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="Message.aspx.cs" Inherits="Message" %>

<%@ Register Src="~/myController/Ascx_Navi.ascx" TagName="Ascx_Navi" TagPrefix="ucNavi" %>
<asp:Content ID="myCss" ContentPlaceHolderID="CssContent" runat="Server">
    <%: Styles.Render("~/bundles/member-css") %>
</asp:Content>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="Server">
    <div class="container df-container-margin">
        <!-- 路徑導航 Start -->
        <ucNavi:Ascx_Navi ID="Ascx_Navi1" runat="server" Param_CurrID="253" />
        <!-- 路徑導航 End -->
        <div class="row">
            <div class="col-xs-12">
                <%-- 其他處理 --%>
                <asp:PlaceHolder ID="ph_message" runat="server" Visible="false">
                    <div class="page-title">
                        <div class="header"><%=this.GetLocalResourceObject("title_oops").ToString()%></div>
                    </div>
                    <div style="line-height: 2em; font-size: 16px;">
                        <%=this.GetLocalResourceObject("msg_發生錯誤").ToString()%><br />
                        <a href="<%=Application["WebUrl"] %>"><i class="fa fa-home"></i>&nbsp;<%=this.GetLocalResourceObject("url_回到首頁").ToString()%></a>
                    </div>
                </asp:PlaceHolder>

                <%-- 成功 --%>
                <asp:PlaceHolder ID="ph_message1" runat="server" Visible="false">
                    <div class="page-title">
                        <div class="header"><%=this.GetLocalResourceObject("title_填寫完成").ToString()%></div>
                    </div>
                    <div style="line-height: 2em; font-size: 16px;">
                        <%=this.GetLocalResourceObject("msg_填寫成功").ToString()%>
                    </div>
                </asp:PlaceHolder>

                <%-- 失敗 --%>
                <asp:PlaceHolder ID="ph_message2" runat="server" Visible="false">
                    <div class="page-title">
                        <div class="header"><%=this.GetLocalResourceObject("title_填寫失敗").ToString()%></div>
                    </div>
                    <div style="line-height: 2em; font-size: 16px;">
                        <%=this.GetLocalResourceObject("msg_填寫失敗").ToString()%>
                        <a href="<%=Application["WebUrl"] %>ContactUs/"><%=this.GetLocalResourceObject("url_與我們聯繫").ToString()%></a>。
                    </div>
                </asp:PlaceHolder>

                <%-- 產品註冊成功 --%>
                <asp:PlaceHolder ID="ph_message3" runat="server" Visible="false">
                    <div class="page-title">
                        <div class="header"><%=this.GetLocalResourceObject("title_產品註冊完成").ToString()%></div>
                    </div>
                    <div style="line-height: 2em; font-size: 16px;">
                        <%=this.GetLocalResourceObject("msg_產品註冊成功").ToString()%>
                    </div>
                </asp:PlaceHolder>
            </div>
        </div>

    </div>
</asp:Content>
<asp:Content ID="myScript" ContentPlaceHolderID="ScriptContent" runat="Server">
</asp:Content>

