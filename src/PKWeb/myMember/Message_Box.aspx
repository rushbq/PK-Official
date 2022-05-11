<%@ Page Language="C#" MasterPageFile="~/Site_Box.master" AutoEventWireup="true" CodeFile="Message_Box.aspx.cs" Inherits="Message_Box" %>

<asp:Content ID="myCss" ContentPlaceHolderID="CssContent" runat="Server">
    <%: Styles.Render("~/bundles/member-css") %>
</asp:Content>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="Server">
    <div class="container df-container-margin">
        <div class="row">
            <div class="col-xs-12">
                <%-- 其他處理 --%>
                <asp:PlaceHolder ID="ph_message" runat="server" Visible="false">
                    <div class="page-title">
                        <div class="header"><%=this.GetLocalResourceObject("title_oops").ToString()%></div>
                    </div>
                    <div style="line-height: 2em; font-size: 16px;">
                        <%=this.GetLocalResourceObject("msg_發生錯誤").ToString()%><br />
                        <br />
                        <a href="<%=Application["WebUrl"] %>"><i class="fa fa-home"></i>&nbsp;<%=this.GetLocalResourceObject("url_回到首頁").ToString()%></a>
                    </div>
                </asp:PlaceHolder>


                <%-- 註冊失敗 --%>
                <asp:PlaceHolder ID="ph_message1" runat="server" Visible="false">
                    <div class="page-title">
                        <div class="header"><%=this.GetLocalResourceObject("title_註冊失敗").ToString()%></div>
                    </div>
                    <div style="line-height: 2em; font-size: 16px;">
                        <%=this.GetLocalResourceObject("msg_註冊失敗").ToString()%>,<a href="<%=Application["WebUrl"] %>ContactUs/"><%=this.GetLocalResourceObject("url_與我們聯繫").ToString()%></a>。
                    </div>
                </asp:PlaceHolder>

                <%-- 註冊成功 --%>
                <asp:PlaceHolder ID="ph_message2" runat="server" Visible="false">
                    <div class="page-title">
                        <div class="header"><%=this.GetLocalResourceObject("title_註冊成功").ToString()%></div>
                    </div>
                    <div style="line-height: 2em; font-size: 16px;"><%=this.GetLocalResourceObject("msg_註冊成功").ToString()%></div>
                </asp:PlaceHolder>

                <%-- 註冊失敗,Email無法寄送 --%>
                <asp:PlaceHolder ID="ph_message3" runat="server" Visible="false">
                    <div class="page-title">
                        <div class="header"><%=this.GetLocalResourceObject("title_註冊失敗").ToString()%></div>
                    </div>
                    <div style="line-height: 2em; font-size: 16px;">
                        <%=this.GetLocalResourceObject("msg_註冊失敗_MailErr").ToString()%>,<a href="<%=Application["WebUrl"] %>ContactUs/"><%=this.GetLocalResourceObject("url_與我們聯繫").ToString()%></a>。
                    </div>
                </asp:PlaceHolder>
            </div>
        </div>

    </div>
</asp:Content>
<asp:Content ID="myScript" ContentPlaceHolderID="ScriptContent" runat="Server">
</asp:Content>

