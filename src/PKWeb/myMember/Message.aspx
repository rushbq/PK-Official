<%@ Page Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="Message.aspx.cs" Inherits="Message" %>

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

                <%-- 驗證碼過期 --%>
                <asp:PlaceHolder ID="ph_message99" runat="server" Visible="false">
                    <div class="page-title">
                        <div class="header"><%=this.GetLocalResourceObject("title_oops").ToString()%></div>
                    </div>
                    <div style="line-height: 2em; font-size: 16px;">
                        <%=this.GetLocalResourceObject("msg_驗證碼過期").ToString()%>
                    </div>
                    <div style="line-height: 2em;">
                        <%=this.GetLocalResourceObject("msg_try").ToString()%>
                        <ul class="list-inline">
                            <li><a href="<%=Application["WebUrl"] %>ForgotPwd"><i class="fa fa-key"></i>&nbsp;<%=this.GetLocalResourceObject("url_忘記密碼").ToString()%></a></li>
                            <li><a href="<%=Application["WebUrl"] %>"><i class="fa fa-home"></i>&nbsp;<%=this.GetLocalResourceObject("url_回到首頁").ToString()%></a></li>
                        </ul>
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
                        <div class="header"><%=this.GetLocalResourceObject("title_驗證").ToString()%></div>
                    </div>
                    <div style="line-height: 2em; font-size: 16px;"><%=this.GetLocalResourceObject("msg_註冊成功").ToString()%></div>
                </asp:PlaceHolder>


                <%-- 變更密碼成功 --%>
                <asp:PlaceHolder ID="ph_message3" runat="server" Visible="false">
                    <div class="page-title">
                        <div class="header"><%=this.GetLocalResourceObject("title_變更密碼成功").ToString()%></div>
                    </div>
                    <div style="line-height: 2em; font-size: 16px;"><%=this.GetLocalResourceObject("msg_變更密碼成功").ToString()%></div>
                </asp:PlaceHolder>

                <%-- 變更密碼失敗 --%>
                <asp:PlaceHolder ID="ph_message4" runat="server" Visible="false">
                    <div class="page-title">
                        <div class="header"><%=this.GetLocalResourceObject("title_變更密碼失敗").ToString()%></div>
                    </div>
                    <div style="line-height: 2em; font-size: 16px;">
                        <%=this.GetLocalResourceObject("msg_變更密碼失敗").ToString()%><br />
                        <a href="<%=Application["WebUrl"] %>ForgotPwd"><%=this.GetLocalResourceObject("url_密碼驗證").ToString()%></a>。
                    </div>
                </asp:PlaceHolder>

                <%-- 驗證成功 --%>
                <asp:PlaceHolder ID="ph_message5" runat="server" Visible="false">
                    <div class="page-title">
                        <div class="header"><%=this.GetLocalResourceObject("title_驗證成功").ToString()%></div>
                    </div>
                    <div style="line-height: 2em; font-size: 16px;"><%=this.GetLocalResourceObject("msg_驗證成功").ToString()%> </div>
                </asp:PlaceHolder>

                <%-- 登入失敗 --%>
                <asp:PlaceHolder ID="ph_message6" runat="server" Visible="false">
                    <div class="page-title">
                        <div class="header"><%=this.GetLocalResourceObject("title_登入失敗").ToString()%></div>
                    </div>
                    <div style="line-height: 2em; font-size: 16px;">
                        <%=this.GetLocalResourceObject("msg_登入失敗").ToString()%><br />
                        <a href="<%=Application["WebUrl"] %>Login?code=<%=HttpUtility.UrlEncode(Req_Code) %>"><%=this.GetLocalResourceObject("url_返回").ToString()%></a>。
                    </div>
                </asp:PlaceHolder>

                <%-- 補發驗證信 --%>
                <asp:PlaceHolder ID="ph_message7" runat="server" Visible="false">
                    <div class="page-title">
                        <div class="header"><%=this.GetLocalResourceObject("title_驗證").ToString()%></div>
                    </div>
                    <div style="line-height: 2em; font-size: 16px;"><%=this.GetLocalResourceObject("msg_驗證信").ToString()%></div>
                </asp:PlaceHolder>

                <%-- 資料修改成功 --%>
                <asp:PlaceHolder ID="ph_message8" runat="server" Visible="false">
                    <div class="page-title">
                        <div class="header"><%=this.GetLocalResourceObject("title_資料修改成功").ToString()%></div>
                    </div>
                    <div style="line-height: 2em; font-size: 16px;"><%=this.GetLocalResourceObject("msg_資料修改成功").ToString()%> </div>
                    <br />
                    <br />
                    <div>
                        <a class="btn btn-default" href="<%=Application["WebUrl"] %>"><i class="fa fa-home fa-fw"></i>&nbsp;<%=this.GetLocalResourceObject("url_回到首頁").ToString()%></a>
                        <a class="btn btn-default" href="<%=Application["WebUrl"] %>ContactUs"><i class="fa fa-envelope fa-fw"></i>&nbsp;<%=Resources.resPublic.home_聯絡我們%></a>
                    </div>
                </asp:PlaceHolder>

                <%-- 資料修改失敗 --%>
                <asp:PlaceHolder ID="ph_message9" runat="server" Visible="false">
                    <div class="page-title">
                        <div class="header"><%=this.GetLocalResourceObject("title_資料修改失敗").ToString()%></div>
                    </div>
                    <div style="line-height: 2em; font-size: 16px;"><%=this.GetLocalResourceObject("msg_資料修改失敗").ToString()%> </div>
                </asp:PlaceHolder>

                <%-- 授權失敗 --%>
                <asp:PlaceHolder ID="ph_message10" runat="server" Visible="false">
                    <div class="page-title">
                        <div class="header"><%=this.GetLocalResourceObject("title_授權失敗").ToString()%></div>
                    </div>
                    <div style="line-height: 2em; font-size: 16px;"><%=this.GetLocalResourceObject("msg_授權失敗").ToString()%> </div>
                </asp:PlaceHolder>

                <%-- 經銷商申請完成 --%>
                <asp:PlaceHolder ID="ph_message11" runat="server" Visible="false">
                    <div class="page-title">
                        <div class="header"><%=this.GetLocalResourceObject("title_經銷商申請完成").ToString()%></div>
                    </div>
                    <div style="line-height: 2em; font-size: 16px;"><%=this.GetLocalResourceObject("msg_經銷商申請完成").ToString()%></div>
                    <a href="<%=Application["WebUrl"] %>"><i class="fa fa-home"></i>&nbsp;<%=this.GetLocalResourceObject("url_回到首頁").ToString()%></a>
                </asp:PlaceHolder>

                <%-- 經銷商申請失敗 --%>
                <asp:PlaceHolder ID="ph_message12" runat="server" Visible="false">
                    <div class="page-title">
                        <div class="header"><%=this.GetLocalResourceObject("title_經銷商申請失敗").ToString()%></div>
                    </div>
                    <div style="line-height: 2em; font-size: 16px;"><%=this.GetLocalResourceObject("msg_經銷商申請失敗").ToString()%></div>
                    <a href="<%=Application["WebUrl"] %>ContactUs/"><%=this.GetLocalResourceObject("url_與我們聯繫").ToString()%></a>
                </asp:PlaceHolder>

                <%-- 帳號已使用 --%>
                <asp:PlaceHolder ID="ph_message13" runat="server" Visible="false">
                    <div class="page-title">
                        <div class="header"><%=this.GetLocalResourceObject("title_登入失敗").ToString()%></div>
                    </div>
                    <div style="line-height: 2em; font-size: 16px;">
                        <%=this.GetLocalResourceObject("msg_帳號已使用").ToString()%>
                    </div>
                </asp:PlaceHolder>

                <%-- 會員轉換成功 --%>
                <asp:PlaceHolder ID="ph_message14" runat="server" Visible="false">
                    <div class="page-title">
                        <div class="header"><%=this.GetLocalResourceObject("title_驗證成功").ToString()%></div>
                    </div>
                    <div style="line-height: 2em; font-size: 16px;">
                        會員轉換成功，
                        <a href="<%=Application["WebUrl"] %>Login">請重新登入</a>。
                    </div>
                </asp:PlaceHolder>

                <%-- 會員轉換失敗 --%>
                <asp:PlaceHolder ID="ph_message15" runat="server" Visible="false">
                    <div class="page-title">
                        <div class="header"><%=this.GetLocalResourceObject("title_oops").ToString()%></div>
                    </div>
                    <div style="line-height: 2em; font-size: 16px;">
                        會員轉換失敗，
                        <a href="<%=Application["WebUrl"] %>Login">請重新登入</a>。
                    </div>
                    <div style="line-height: 2em; font-size: 16px;">
                        <%=this.GetLocalResourceObject("msg_註冊失敗").ToString()%>,<a href="<%=Application["WebUrl"] %>ContactUs/"><%=this.GetLocalResourceObject("url_與我們聯繫").ToString()%></a>。
                    </div>
                </asp:PlaceHolder>
            </div>
        </div>

    </div>
</asp:Content>
<asp:Content ID="myScript" ContentPlaceHolderID="ScriptContent" runat="Server">
</asp:Content>

