<%@ Page Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="QAListContent.aspx.cs" Inherits="QAListContent" %>

<%@ Import Namespace="ExtensionMethods" %>
<asp:Content ID="myCss" ContentPlaceHolderID="CssContent" runat="Server">
    <%: Styles.Render("~/bundles/product-css") %>
    <%: Styles.Render("~/bundles/support-css") %>
</asp:Content>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="Server">
    <div class="container df-container-margin">
        <!-- 路徑導航 -->
        <div>
            <ol class="breadcrumb">
                <li><a href="<%=Application["WebUrl"] %>QA/List"><%=Resources.resPublic.title_常見問題 %></a></li>
                <li><%=Req_ModelNo %></li>
            </ol>
        </div>

        <!-- Content Start -->
        <div class="technical_faq_result df_technical_faq_result_padding row">
            <!--商品 Start-->
            <div class="contact_area df_border_right col-xs-12 col-sm-5 col-md-4 col-lg-4">
                <div class="thumbnail">
                    <div class="row">
                        <div class="col-xs-12 col-sm-12 col-md-12">
                            <div class="faq-image-container">
                                <asp:Literal ID="lt_ModelPic" runat="server"></asp:Literal>
                            </div>
                        </div>
                        <div class=" col-xs-12 col-sm-12 col-md-12">
                            <div class="content">
                                <p class="title">
                                    <asp:Literal ID="lt_ModelNo" runat="server"></asp:Literal>
                                </p>
                                <p class="name">
                                    <asp:Literal ID="lt_ModelName" runat="server"></asp:Literal>
                                </p>
                            </div>
                            <!-- 常見問題/商品特色按鈕 -->
                            <div class="btn-box">
                                <a href="<%=Application["WebUrl"] %>ContactUs/" class="btn btn-contact hidden-xs"><%=Resources.resPublic.title_技術諮詢 %></a>
                                <a href="<%=Application["WebUrl"] %>Product/<%=Server.UrlEncode(Req_ModelNo) %>/" class="btn btn-more hidden-xs"><%=this.GetLocalResourceObject("tip_產品資訊").ToString() %></a>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <!-- 商品 End -->
            <!-- 問題列表 Start -->
            <ul class="col-xs-12 col-sm-7 col-md-8 col-lg-8 frequently_questions_area">
                <asp:ListView ID="lvDataList" runat="server" ItemPlaceholderID="ph_Items">
                    <LayoutTemplate>
                        <asp:PlaceHolder ID="ph_Items" runat="server" />
                    </LayoutTemplate>
                    <ItemTemplate>
                        <li class="topic"><a href="<%#Application["WebUrl"] %>QA/View/<%=HttpUtility.UrlEncode(Req_ModelNo) %>/<%# Cryptograph.MD5Encrypt(Eval("FAQ_ID").ToString(), DesKey)%>"><%#Eval("FAQ_Title") %></a></li>
                    </ItemTemplate>
                    <EmptyDataTemplate>
                        <div class="exception-info">
                            <div class="icon"><i class="fa fa-exclamation-triangle"></i></div>
                            <div class="message">Oops! <%=Resources.resPublic.txt_查無資料 %></div>
                        </div>
                    </EmptyDataTemplate>
                </asp:ListView>

                <li>
                    <a href="<%=Application["WebUrl"] %>QA/List/1/<%=HttpUtility.UrlEncode(Req_Keyword) %>/" class="btn btn-back"><%=this.GetLocalResourceObject("btn_回搜尋列表頁").ToString() %></a>
                </li>
            </ul>
            <!-- 問題列表 End -->

        </div>
        <!-- Content End -->
    </div>

</asp:Content>
<asp:Content ID="myScript" ContentPlaceHolderID="ScriptContent" runat="Server">
    <script>
        $(function () {

        });
    </script>
</asp:Content>

