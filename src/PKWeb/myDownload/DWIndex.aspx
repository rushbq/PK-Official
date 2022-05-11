<%@ Page Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="DWIndex.aspx.cs" Inherits="QAIndex" %>

<%@ Register Src="~/myController/Ascx_Navi.ascx" TagName="Ascx_Navi" TagPrefix="ucNavi" %>
<%@ Import Namespace="ExtensionMethods" %>
<asp:Content ID="myCss" ContentPlaceHolderID="CssContent" runat="Server">
    <%: Styles.Render("~/bundles/support-css") %>
    <%: Styles.Render("~/bundles/distributor-css") %>
</asp:Content>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="Server">
    <div class="container df-container-margin">
        <!-- 路徑導航 Start -->
        <ucNavi:Ascx_Navi ID="Ascx_Navi1" runat="server" Param_CurrID="252" />
        <!-- 路徑導航 End -->
        <div class="form-group input-group">
            <asp:TextBox ID="tb_Keyword" runat="server" CssClass="form-control myEnter" MaxLength="50"></asp:TextBox>
            <div class="input-group-btn">
                <button type="button" class="btn btn-search doSearch"><span class="fa fa-search"></span></button>
            </div>
        </div>
        <!-- Content Start -->
        <!-- 分類標籤找檔案-->
        <div class="content download-tab">
            <ul class="nav nav-tabs" id='nav_tab' role="tablist">
                <li role="presentation" class="nav-item nav-link active">
                    <a href="#normal" rel="external nofollow"
                        data-toggle="tab" aria-selected="true">
                        <h4><%=this.GetLocalResourceObject("txt_Normal").ToString()%></h4>
                    </a></li>
                <%--<li role="presentation" class="nav-item nav-link">
                    <a href="#dealer" rel="external nofollow"
                        data-toggle="tab" aria-selected="false">
                        <h4>經銷商下載</h4>
                    </a>
                </li>--%>
            </ul>
            <div class="tab-content">
                <div class="tab-pane active" id="normal">
                    <div class="row">
                        <asp:Literal ID="lt_ClassItems" runat="server"></asp:Literal>

                    </div>
                </div>
                <asp:PlaceHolder ID="ph_DealerDW" runat="server" Visible="false">
                    <div class="tab-pane fade" id="dealer">
                        <div class="row">
                            <a class="blok-link" href="<%=Application["WebUrl"] %>D-Download/Photo">
                                <div class="col-md-4">
                                    <div class="thumbnail">
                                        <div class="block">
                                            <div class="media-left">
                                                <i class="fa fa-picture-o fa-3x fa-fw text-gray" aria-hidden="true"></i>
                                            </div>
                                            <div class="media-body">
                                                <h5 class="text-gray"><%=Resources.resPublic.bar_產品圖片 %></h5>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </a>
                            <a class="blok-link" href="<%=Application["WebUrl"] %>D-Download/Package">
                                <div class="col-md-4">
                                    <div class="thumbnail">
                                        <div class="block">
                                            <div class="media-left">
                                                <i class="fa fa-archive fa-3x fa-fw text-gray" aria-hidden="true"></i>
                                            </div>
                                            <div class="media-body">
                                                <h5 class="text-gray"><%=Resources.resPublic.bar_包材檔案 %></h5>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </a>
                            <a class="blok-link" href="<%=Application["WebUrl"] %>D-Download/Certification">
                                <div class="col-md-4">
                                    <div class="thumbnail">
                                        <div class="block">
                                            <div class="media-left">
                                                <i class="fa fa-check-square-o fa-3x fa-fw text-gray" aria-hidden="true"></i>
                                            </div>
                                            <div class="media-body">
                                                <h5 class="text-gray"><%=Resources.resPublic.bar_認證資料 %></h5>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </a>
                        </div>
                        <div class="row">
                            <a class="blok-link" href="<%=historyUrl(309) %>" target="_blank">
                                <div class="col-md-4">
                                    <div class="thumbnail">
                                        <div class="block">
                                            <div class="media-left">
                                                <i class="fa fa-clone fa-3x fa-fw text-gray" aria-hidden="true"></i>
                                            </div>
                                            <div class="media-body">
                                                <h5 class="text-gray"><%=Resources.resPublic.bar_形象識別 %></h5>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </a>
                            <a class="blok-link" href="<%=historyUrl(308) %>" target="_blank">
                                <div class="col-md-4">
                                    <div class="thumbnail">
                                        <div class="block">
                                            <div class="media-left">
                                                <i class="fa fa-map-o fa-3x fa-fw text-gray" aria-hidden="true"></i>
                                            </div>
                                            <div class="media-body">
                                                <h5 class="text-gray"><%=Resources.resPublic.bar_燈片圖庫 %></h5>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </a>
                            <a class="blok-link" href="<%=historyUrl(307) %>" target="_blank">
                                <div class="col-md-4">
                                    <div class="thumbnail">
                                        <div class="block">
                                            <div class="media-left">
                                                <i class="fa fa-object-ungroup fa-3x fa-fw text-gray" aria-hidden="true"></i>
                                            </div>
                                            <div class="media-body">
                                                <h5 class="text-gray"><%=Resources.resPublic.bar_銷售工具包 %></h5>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </a>
                        </div>
                    </div>
                </asp:PlaceHolder>
            </div>
        </div>





        <asp:Button ID="btn_Search" runat="server" OnClick="btn_Search_Click" Style="display: none;" />
        <!-- Content End -->
    </div>
</asp:Content>
<asp:Content ID="myScript" ContentPlaceHolderID="ScriptContent" runat="Server">
    <%: Scripts.Render("~/bundles/blockUI-script") %>
    <script>
        $(function () {
            /* Search click */
            $(".doSearch").click(function () {
                blockBox2_NoMsg();
                $("#MainContent_btn_Search").trigger("click");
                event.preventDefault();
            });

            /* Enter 偵測 */
            $(".myEnter").keypress(function (e) {
                code = (e.keyCode ? e.keyCode : e.which);
                if (code == 13) {
                    $("#MainContent_btn_Search").trigger("click");
                    event.preventDefault();
                }
            });
        });
    </script>
</asp:Content>

