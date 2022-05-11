<%@ Page Title="HotProd" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="HotProd.aspx.cs" Inherits="myProd_HotProd" %>


<%@ Import Namespace="ExtensionMethods" %>
<asp:Content ID="myCss" ContentPlaceHolderID="CssContent" runat="Server">
    <%: Styles.Render("~/bundles/product-css") %>
</asp:Content>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="Server">
    <div class="container df-container-margin">
        <div class="page-header">
            <h3><%=Resources.resPublic.title_HotProd_Tool %>
                &nbsp;<small><%=ClassName %></small>
            </h3>
        </div>
        <div>
            <div class="pull-left form-inline">
                <div class="form-group input-group">
                    <asp:TextBox ID="tb_Keyword" runat="server" CssClass="form-control myEnter" MaxLength="50"></asp:TextBox>
                    <div class="input-group-btn">
                        <button type="button" class="btn btn-search doSearch"><span class="fa fa-search"></span></button>
                    </div>
                </div>
                <div class="form-group">
                    <asp:DropDownList ID="ddl_ProdClass" runat="server" CssClass="form-control" AutoPostBack="true" OnTextChanged="btn_Search_Click"></asp:DropDownList>
                </div>
                <asp:Button ID="btn_Search" runat="server" OnClick="btn_Search_Click" Style="display: none;" />

            </div>
            <div class="pull-right hidden-xs">
                <asp:Literal ID="lt_Pager_top" runat="server"></asp:Literal>
            </div>
            <div class="clearfix"></div>
        </div>

        <!-- Content Start -->
        <div class="Products_Info">
            <asp:ListView ID="lvDataList" runat="server" ItemPlaceholderID="ph_Items" OnItemDataBound="lvDataList_ItemDataBound">
                <LayoutTemplate>
                    <div class="row">
                        <asp:PlaceHolder ID="ph_Items" runat="server" />
                    </div>

                    <asp:Literal ID="lt_Pager_footer" runat="server"></asp:Literal>
                </LayoutTemplate>
                <ItemTemplate>
                    <div class="col-xs-6 col-sm-4 col-md-3">
                        <div class="thumbnail">
                            <div class="box">
                                <asp:PlaceHolder ID="ph_NewItem" runat="server" Visible="false">
                                    <div class="label-box">
                                        <div class="new">
                                            <h3><%=this.GetLocalResourceObject("txt_新品").ToString()%></h3>
                                        </div>
                                    </div>
                                </asp:PlaceHolder>
                                <asp:PlaceHolder ID="ph_RecmItem" runat="server" Visible="false">
                                    <div class="label-box">
                                        <div class="new">
                                            <h3><%=this.GetLocalResourceObject("txt_推薦").ToString()%></h3>
                                        </div>
                                    </div>
                                </asp:PlaceHolder>
                                <asp:PlaceHolder ID="ph_Stop" runat="server" Visible="false">
                                    <div class="label-box">
                                        <div class="Discontinued">
                                            <h4><%=this.GetLocalResourceObject("txt_已停售").ToString()%></h4>
                                        </div>
                                    </div>
                                </asp:PlaceHolder>
                                <div class="row">
                                    <div class="col-xs-12 col-sm-12 col-md-12">
                                        <div class="image-container">
                                            <a href="<%=Application["WebUrl"] %>Product/<%#Server.UrlEncode(Eval("ModelNo").ToString()) %>/" title="<%# fn_stringFormat.Set_FilterHtml(Eval("ModelName").ToString()) %>">
                                                <%# Get_Pic(Eval("PhotoGroup").ToString(), Eval("ModelNo").ToString()) %>
                                            </a>
                                        </div>
                                    </div>
                                    <div class="col-xs-12 col-sm-12 col-md-12">
                                        <div class="area-label">
                                            <i class="material-icons glyphicon glyphicon-shopping-cart hidden-xs"></i>&nbsp;
                                            <ul>
                                                <asp:Literal ID="lt_Area" runat="server"></asp:Literal>
                                            </ul>
                                        </div>

                                        <div class="content">
                                            <p class="title"><a href="<%=Application["WebUrl"] %>Product/<%#Server.UrlEncode(Eval("ModelNo").ToString()) %>/"><%#Eval("ModelNo") %></a></p>
                                            <p class="name"><a href="<%=Application["WebUrl"] %>Product/<%#Server.UrlEncode(Eval("ModelNo").ToString()) %>/"><%#Eval("ModelName") %></a></p>
                                        </div>
                                        <!-- 立即購買 -->
                                        <div class="btn-bord">
                                            <div class="btn-box">
                                                <asp:Literal ID="lt_BuyUrl" runat="server"></asp:Literal>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>

                    <!-- /// Remote Modal Start /// -->
                    <div class="modal fade" id="remoteModal-<%#Server.UrlEncode(Eval("ModelNo").ToString()) %>" role="dialog"></div>
                    <!-- /// RemoteModal End /// -->

                </ItemTemplate>
                <EmptyDataTemplate>
                    <div class="exception-info">
                        <div class="icon"><i class="fa fa-exclamation-triangle"></i></div>
                        <div class="message">Oops! <%=Resources.resPublic.txt_查無資料 %></div>
                    </div>
                </EmptyDataTemplate>
            </asp:ListView>
        </div>
        <!-- Content End -->

        <!-- Modal Contact us S -->
        <div class="modal fade" id="myModalContact" role="dialog">
            <div class="modal-dialog modal-dialog-centered" role="document">
                <div class="modal-content">
                    <div class="modal-header">
                        <button type="button" class="close" data-dismiss="modal">&times;</button>
                        <div class="page-title">
                            <div class="header"><%=this.GetLocalResourceObject("txt_商品詢問").ToString()%>【<span id="showTargetModel"></span>】</div>
                        </div>
                        <p class="contact-description"><%=this.GetLocalResourceObject("txt_StopBuy").ToString()%></p>
                    </div>
                    <div class="modal-body">
                        <div class="form-horizontal">
                            <div class="form-group">
                                <label class="col-sm-12 col-md-3"><%=this.GetLocalResourceObject("txt_姓名").ToString()%></label>
                                <div class="col-sm-12 col-md-9">
                                    <asp:TextBox ID="tb_Name" runat="server" CssClass="form-control myForm" autocomplete="off" MaxLength="50"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rfvName" runat="server" Display="Dynamic"
                                        ControlToValidate="tb_Name" ValidationGroup="ProdInq"><div class="alert alert-danger"><%=this.GetLocalResourceObject("tip_Require").ToString()%></div></asp:RequiredFieldValidator>
                                </div>
                            </div>
                            <div class="form-group">
                                <label class="col-sm-12 col-md-3"><%=this.GetLocalResourceObject("txt_Email").ToString()%></label>
                                <div class="col-sm-12 col-md-9">
                                    <asp:TextBox ID="tb_Email" runat="server" CssClass="form-control myForm" type="email" autocomplete="off" MaxLength="100"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rfvEmail" runat="server" Display="Dynamic"
                                        ControlToValidate="tb_Email" ValidationGroup="ProdInq"><div class="alert alert-danger"><%=this.GetLocalResourceObject("tip_Require").ToString()%></div></asp:RequiredFieldValidator>
                                    <asp:RegularExpressionValidator ID="rev_tb_Email" runat="server" ControlToValidate="tb_Email" Display="Dynamic" ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*" ValidationGroup="Add"><div class="alert alert-danger"><%=this.GetLocalResourceObject("tip_Format").ToString()%></div></asp:RegularExpressionValidator>
                                </div>
                            </div>
                            <div class="form-group">
                                <label class="col-sm-12 col-md-3"><%=this.GetLocalResourceObject("txt_Message").ToString()%></label>
                                <div class="col-sm-12 col-md-9">
                                    <asp:TextBox ID="tb_Message" runat="server" CssClass="form-control" autocomplete="off" TextMode="MultiLine" Rows="5" MaxLength="500"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rfvMessage" runat="server" Display="Dynamic"
                                        ControlToValidate="tb_Message" ValidationGroup="ProdInq"><div class="alert alert-danger"><%=this.GetLocalResourceObject("tip_Require").ToString()%></div></asp:RequiredFieldValidator>
                                </div>
                            </div>
                            <div class="form-group">
                                <label class="col-xs-3"><%=this.GetLocalResourceObject("txt_Verify").ToString()%></label>
                                <div class="col-xs-9 form-inline">
                                    <asp:Image ID="img_Verify" runat="server" ImageAlign="Middle" />
                                    <a id="chg-Verify" href="javascript:void(0)"><i class="fa fa-refresh"></i></a>

                                    <asp:TextBox ID="tb_VerifyCode" runat="server" MaxLength="5" CssClass="form-control myForm" rel="nofollow" ValidationGroup="ProdInq"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rfv_tb_VerifyCode" runat="server" Display="Dynamic"
                                        ControlToValidate="tb_VerifyCode" ValidationGroup="ProdInq"><div class="alert alert-danger"><%=this.GetLocalResourceObject("tip_Require").ToString()%></div></asp:RequiredFieldValidator>
                                </div>
                            </div>
                            <div class="form-group">
                                <asp:Button ID="btn_SendContact" runat="server" Text="Submit" CssClass="btn btn-login" OnClick="btn_SendContact_Click" ValidationGroup="ProdInq" />
                                <asp:HiddenField ID="hf_ModelNo" runat="server" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <!-- Modal Contact us E -->

        <!-- Modal unSell S -->
        <div class="modal fade" id="myUnsell" role="dialog">
            <div class="modal-dialog modal-dialog-centered" role="document">
                <div class="modal-content">
                    <div class="modal-header">
                        <button type="button" class="close" data-dismiss="modal">&times;</button>
                        <div class="page-title">
                            <div class="header">Information</div>
                        </div>
                        <p class="contact-description">
                            <%=this.GetLocalResourceObject("txt_未開賣").ToString()%>
                        </p>
                        <div class="modal-footer">
                            <button type="button" class="btn btn-default" data-dismiss="modal">Close</button>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <!-- Modal unSell E -->
    </div>
</asp:Content>
<asp:Content ID="myScript" ContentPlaceHolderID="ScriptContent" runat="Server">
    <%: Scripts.Render("~/bundles/lazyload-script") %>
    <%: Scripts.Render("~/bundles/blockUI-script") %>
    <script>
        $(function () {
            /* lazyload */
            $("img.lazy").lazyload({
                effect: "fadeIn"
            });

            /* Search click */
            $(".doSearch").click(function () {
                blockBox2_NoMsg();
                $("#MainContent_btn_Search").trigger("click");
            });

            /* Enter 偵測 */
            $(".myEnter").keypress(function (e) {
                code = (e.keyCode ? e.keyCode : e.which);
                if (code == 13) {
                    blockBox2_NoMsg();
                    $("#MainContent_btn_Search").trigger("click");
                    event.preventDefault();
                }
            });
            $(".myForm").keypress(function (e) {
                code = (e.keyCode ? e.keyCode : e.which);
                if (code == 13) {
                    $("#MainContent_btn_SendContact").trigger("click");
                    //取消DOM的預設功能事件
                    event.preventDefault();
                }
            });

            /* MODAL - Contact us by Model */
            $(".doContact").click(function () {
                //get model
                var _model = $(this).attr("data-id");

                //show model
                $("#showTargetModel").text(_model);
                $("#MainContent_hf_ModelNo").val(_model);
            });


            /* 驗證碼 */
            $('#chg-Verify').click(function () {
                document.getElementById('MainContent_img_Verify').src = '<%=Application["WebUrl"] %>myHandler/Ashx_CreateValidImg.ashx?r=' + Math.random();
            });

            /* MODAL - frame go buy */
            $(".doRemoteUrl").on("click", function () {
                var id = $(this).attr("data-id");   //品號
                var _name = $(this).attr("data-name");   //品名
                var _target = $(this).attr("data-target");

                //load html
                var url = '<%=Application["WebUrl"]%>' + "Ajax_Data/Frame_GoBuy.aspx?area=CN&id=" + encodeURIComponent(id) + "&name=" + _name;
                var datablock = $(_target);
                datablock.empty();
                datablock.load(url);

            });
        });
    </script>
</asp:Content>

