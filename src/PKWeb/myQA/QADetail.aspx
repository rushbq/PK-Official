<%@ Page Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="QADetail.aspx.cs" Inherits="QADetail" %>

<%@ Import Namespace="ExtensionMethods" %>
<asp:Content ID="myCss" ContentPlaceHolderID="CssContent" runat="Server">
    <%: Styles.Render("~/bundles/product-css") %>
    <%: Styles.Render("~/bundles/support-css") %>

    <%-- 自訂meta --%>
    <meta name="description" content="<%=meta_Desc %>" />
    <meta property="og:type" content="website" />
    <meta property="og:url" content="<%=meta_Url %>" />
    <meta property="og:title" content="<%=meta_Title %>" />
    <meta property="og:description" content="<%=meta_Desc %>" />
    <meta property="og:image" content="<%=meta_Image %>" />
</asp:Content>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="Server">
    <div class="container df-container-margin">
        <!-- 路徑導航 -->
        <div>
            <ol class="breadcrumb">
                <li><a href="<%=Application["WebUrl"] %>QA/List"><%=Resources.resPublic.title_常見問題 %></a></li>
                <li><%=Req_ModelNo %></li>
                <li>
                    <asp:Literal ID="lt_QATitle" runat="server"></asp:Literal>
                </li>
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
                <!-- 問題主題連結 Start -->
                <div class="primary_nav">
                    <p class="glyphicon glyphicon-question-sign" aria-hidden="true">Topics</p>
                    <asp:Literal ID="lt_CapList" runat="server"></asp:Literal>
                </div>
            </div>
            <!-- 商品 End -->

            <!-- 內容區塊 Start -->
            <div class="col-xs-12 col-sm-7 col-md-8 col-lg-8">
                <div class="frequently_questions_area">
                    <!-- 答案列表 Start -->
                    <div class="frequently_questions_contents">
                        <div class="primary_article">
                            <asp:ListView ID="lvDataList" runat="server" ItemPlaceholderID="ph_Items">
                                <LayoutTemplate>
                                    <div class="accordion-style panel-group" id="DataInfo">
                                        <asp:PlaceHolder ID="ph_Items" runat="server" />
                                    </div>
                                </LayoutTemplate>
                                <ItemTemplate>
                                    <div class="panel panel-default">
                                        <div class="panel-heading" role="tab">
                                            <h4 class="panel-title">
                                                <a class="accordion-toggle <%# Get_CurrentClass(Eval("dtDefault").ToString(), "header", "collapsed") %>" data-toggle="collapse" data-parent="#DataInfo" href="#ptab<%#Eval("Block_ID") %>">
                                                    <%#Eval("Block_Title") %>
                                                </a>
                                            </h4>
                                        </div>
                                        <div id="ptab<%#Eval("Block_ID") %>" class="panel-collapse collapse <%# Get_CurrentClass(Eval("dtDefault").ToString(), "content", "in") %>">
                                            <div class="panel-body">
                                                <%# HttpUtility.HtmlDecode(Eval("Block_Desc").ToString())%>
                                            </div>
                                        </div>
                                    </div>
                                </ItemTemplate>
                                <EmptyDataTemplate>
                                    <div class="exception-info">
                                        <div class="icon"><i class="fa fa-exclamation-triangle"></i></div>
                                        <div class="message">Oops! <%=Resources.resPublic.txt_查無資料 %></div>
                                    </div>
                                </EmptyDataTemplate>
                            </asp:ListView>
                        </div>

                        <a href="<%=Application["WebUrl"]%>QA/Content/<%=Server.UrlEncode(Req_ModelNo) %>/" class="btn btn-back"><%=this.GetLocalResourceObject("btn_Back").ToString() %></a>
                    </div>

                    <!-- 答案列表 End -->
                    <!-- feedback -->
                    <div class="feedback">
                        <div class="owner_info">
                            <asp:PlaceHolder ID="ph_feedTitle" runat="server">
                                <%=this.GetLocalResourceObject("feed_Title").ToString() %>
                            </asp:PlaceHolder>
                            <asp:Panel ID="fb_choice" runat="server" CssClass="feedback_bt_group">
                                <asp:Button ID="btn_Yes" runat="server" OnClick="btn_Yes_Click" CssClass="btn btn-yes df-btn-width" />
                                <button type="button" class="btn btn-no df-btn-width" id="fb_no"><%=this.GetLocalResourceObject("btn_No").ToString() %></button>
                            </asp:Panel>

                            <!-- 按"是"按鈕後顯示以下 Start -->
                            <asp:Panel ID="fb_OK" runat="server" Visible="false" CssClass="feedback_title">
                                <strong><%=this.GetLocalResourceObject("feed_OK").ToString() %></strong>
                            </asp:Panel>
                            <!-- 按"是"按鈕後顯示以下 End -->

                            <!-- 按"否"按鈕後顯示以下 Start-->
                            <div class="message_box" style="display: none;" id="fb_myMsg">
                                <%=this.GetLocalResourceObject("feed_Message").ToString() %>
                                <asp:TextBox ID="tb_feedMsg" runat="server" CssClass="form-control form-control-margin" Rows="3" TextMode="MultiLine" MaxLength="1500"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="rfv_tb_feedMsg" runat="server" ControlToValidate="tb_feedMsg" Display="Dynamic" ValidationGroup="feed">
                                <div class="alert alert-danger"><%=this.GetLocalResourceObject("tip_Require").ToString()%></div>
                                </asp:RequiredFieldValidator>
                                <asp:Button ID="btn_SendMsg" runat="server" CssClass="btn btn-success df-btn-width" ValidationGroup="feed" OnClick="btn_SendMsg_Click" />
                            </div>
                            <!--按"否"按鈕後顯示以下 End -->

                        </div>
                    </div>
                </div>

            </div>
            <!-- 內容區塊 End -->
        </div>
        <!-- Content End -->
    </div>

</asp:Content>
<asp:Content ID="myScript" ContentPlaceHolderID="ScriptContent" runat="Server">
    <script>
        $(function () {
            var eleChoice = $("#MainContent_fb_choice");
            var eleMsg = $("#fb_myMsg");

            //Feed Click No
            $("#fb_no").click(function () {
                eleChoice.hide();
                eleMsg.show();
            });

            //自動新增圖片自適應的class
            $(".panel-body img").addClass("img-responsive");
        });
    </script>
</asp:Content>

