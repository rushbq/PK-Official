<%@ Page Title="常見問題內容設定" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="Edit_Sub.aspx.cs" Inherits="FAQ_Edit_Sub" ValidateRequest="false" %>

<%@ Import Namespace="ExtensionMethods" %>
<asp:Content ID="myCss" ContentPlaceHolderID="CssContent" runat="Server">
    <%: Styles.Render("~/bundles/fancybox-css") %>
    <%: Styles.Render("~/bundles/JQ-UI-css") %>
    <style>
        .dragMe {
            cursor: all-scroll;
        }

        /* 強迫設定 ckeditor 高度*/
        .cke_contents {
            height: 400px !important;
        }
    </style>
</asp:Content>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="Server">
    <!-- Page Header Start -->
    <div class="row">
        <div class="col-md-12">
            <ol class="breadcrumb">
                <li><a href="<%=Application["WebUrl"] %>">首頁</a></li>
                <li><a href="<%=Page_LastUrl %>">
                    <asp:Literal ID="lt_NavSubject" runat="server">單頭資料</asp:Literal></a></li>
                <li class="active">語系內容設定</li>
            </ol>
        </div>
    </div>
    <!-- Page Header End -->
    <!-- Form Start -->
    <div class="row">
        <div class="col-sm-9 col-md-10">
            <div class="panel panel-info">
                <div class="panel-heading">
                    <div class="pull-left">
                        <span class="glyphicon glyphicon-edit"></span>
                        <span>基本設定</span>
                    </div>
                    <div class="pull-right">
                    </div>
                    <div class="clearfix"></div>
                </div>
                <div id="data" class="collapse in">
                    <div class="panel-body">
                        <!-- Content Start -->
                        <div class="form-horizontal">
                            <div class="form-group">
                                <label class="control-label col-sm-3 col-md-3 col-lg-2">識別名稱</label>
                                <div class="col-sm-9 col-md-9 col-lg-10">
                                    <p class="form-control-static">
                                        <strong>
                                            <asp:Label ID="lb_Group_Name" runat="server" CssClass="styleGreen"></asp:Label></strong>
                                    </p>
                                </div>
                            </div>
                            <div class="form-group">
                                <label class="control-label col-sm-3 col-md-3 col-lg-2">目前語系</label>
                                <div class="col-sm-9 col-md-9 col-lg-10">
                                    <p class="form-control-static">
                                        <strong>
                                            <asp:Label ID="lb_Lang" runat="server" CssClass="styleBlue"></asp:Label></strong>
                                    </p>
                                </div>
                            </div>

                            <div class="form-group">
                                <label class="control-label col-sm-3 col-md-3 col-lg-2">主標題 <em>*</em></label>
                                <div class="col-sm-9 col-md-9 col-lg-10">
                                    <asp:TextBox ID="tb_FAQ_Title" runat="server" CssClass="form-control tip" placeholder="主標題" MaxLength="200" ToolTip="字數上限 100 字"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rfv_tb_FAQ_Title" runat="server" ErrorMessage="請填寫「主標題」" ControlToValidate="tb_FAQ_Title" Display="Dynamic" ValidationGroup="Add" CssClass="styleRed help-block"></asp:RequiredFieldValidator>
                                </div>
                            </div>

                            <div class="form-group">
                                <div class="col-sm-12 text-right">
                                    <asp:Button ID="btn_Base_Save" runat="server" Text="儲存設定" CssClass="btn btn-primary" OnClick="btn_Save_Click" ValidationGroup="Add" OnClientClick="blockBox1('Add', '資料處理中...');" />
                                </div>
                            </div>
                            <asp:HiddenField ID="hf_flag" runat="server" Value="Add" />
                            <asp:ValidationSummary ID="ValidationSummary1" runat="server" ValidationGroup="Add" ShowMessageBox="true" ShowSummary="false" />
                        </div>
                        <!-- Content End -->
                    </div>
                </div>
            </div>
        </div>
        <!-- buttons -->
        <div class="col-sm-3 col-md-2">
            <div class="FormBtn-fixPos">
                <div class="metro-nav metro-fix-view">
                    <div class="metro-nav-block nav-block-green">
                        <a href="<%=Page_LastUrl %>" class="text-center">
                            <i class="glyphicon glyphicon-circle-arrow-left"></i>
                            <div class="status">返回單頭資料</div>
                        </a>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <!-- Form End -->
    <!-- 區塊內容 Start -->
    <asp:PlaceHolder ID="ph_myBlock" runat="server" Visible="false">
        <!-- Block Add Start -->
        <div class="row">
            <div class="col-sm-9 col-md-10">
                <div class="panel panel-info">
                    <div class="panel-heading">
                        <div class="pull-left">
                            <i class="fa fa-cube"></i>
                            <span>區塊新增</span>
                        </div>
                        <div class="pull-right">
                        </div>
                        <div class="clearfix"></div>
                    </div>
                    <div id="block" class="collapse in">
                        <div class="panel-body">
                            <!-- Content Start -->
                            <div class="form-horizontal">
                                <div class="form-group">
                                    <label class="control-label col-sm-3 col-md-3 col-lg-2">區塊標題 <em>*</em></label>
                                    <div class="col-sm-9 col-md-9 col-lg-10">
                                        <asp:TextBox ID="tb_Block_Title" runat="server" CssClass="form-control tip" placeholder="區塊標題" MaxLength="500" ToolTip="字數上限 250 字"></asp:TextBox>
                                        <asp:RequiredFieldValidator ID="rfv_tb_Block_Title" runat="server" ErrorMessage="請填寫「區塊標題」" ControlToValidate="tb_Block_Title" Display="Dynamic" ValidationGroup="BlockAdd" CssClass="styleRed help-block"></asp:RequiredFieldValidator>
                                    </div>
                                </div>
                                <div class="form-group">
                                    <label class="control-label col-sm-3 col-md-3 col-lg-2">區塊內容 <em>*</em></label>
                                    <div class="col-sm-9 col-md-9 col-lg-10">
                                        <asp:TextBox ID="tb_Block_Desc" runat="server" TextMode="MultiLine" CssClass="ckeditor"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="form-group">
                                    <label class="control-label col-sm-3 col-md-3 col-lg-2" for="MainContent_tb_Sort">自訂排序</label>
                                    <div class="col-sm-5 col-md-5 col-lg-5">
                                        <asp:TextBox ID="tb_Sort" runat="server" MaxLength="3" CssClass="form-control" placeholder="排序" Width="70px" Style="text-align: center;">999</asp:TextBox>
                                        <asp:RequiredFieldValidator ID="rfv_tb_Sort" runat="server" ErrorMessage="請輸入「排序」"
                                            Display="Dynamic" ControlToValidate="tb_Sort" ValidationGroup="BlockAdd" CssClass="styleRed help-block"></asp:RequiredFieldValidator>
                                        <asp:RangeValidator ID="rv_tb_Sort" runat="server" ErrorMessage="請輸入1 ~ 999 的數字"
                                            Display="Dynamic" Type="Integer" MaximumValue="999" MinimumValue="1" ControlToValidate="tb_Sort"
                                            ValidationGroup="BlockAdd" CssClass="styleRed help-block"></asp:RangeValidator>
                                    </div>
                                    <div class="col-sm-4 col-md-4 col-lg-5 text-right">
                                        <asp:Button ID="btn_Block_Save" runat="server" Text="新增區塊" CssClass="btn btn-primary" OnClick="btn_BlockSave_Click" ValidationGroup="BlockAdd" OnClientClick="blockBox1('BlockAdd', '資料處理中...');" />
                                        <asp:ValidationSummary ID="ValidationSummary2" runat="server" ValidationGroup="BlockAdd" ShowMessageBox="true" ShowSummary="false" />
                                    </div>
                                </div>
                            </div>
                            <!-- Content End -->
                        </div>
                    </div>
                    <!-- Table Content End -->
                </div>
            </div>
            <div class="col-sm-3 col-md-2"></div>
        </div>
        <!-- Block Add End -->
        <!-- Block List Start -->
        <div class="row">
            <div class="col-sm-9 col-md-10">
                <div class="panel panel-info">
                    <div class="panel-heading">
                        <div class="pull-left">
                            <span class="glyphicon glyphicon-list"></span>
                            <span>區塊預覽</span>
                        </div>
                        <div class="pull-right">
                        </div>
                        <div class="clearfix"></div>
                    </div>
                    <div id="blocklist" class="collapse in">
                        <div class="panel-body">
                            <div class="row">
                                <div class="col-xs-8">
                                    <div class="well well-sm">
                                        1. 此頁排版僅供參考，實際畫面呈現以前台為主。<br />
                                        2. 拖曳區塊可自訂排序，排序完成後請按下「儲存版面排序」。
                                    </div>
                                </div>
                                <div class="col-xs-4 text-right">
                                    <asp:Button ID="btn_SaveBlock1" runat="server" Text="↓ 儲存版面排序 ↓" CssClass="btn btn-success" OnClick="btn_SaveSort_Click" ValidationGroup="List" />
                                </div>
                            </div>

                            <asp:ListView ID="lvDataList" runat="server" ItemPlaceholderID="ph_Items" OnItemCommand="lvDataList_ItemCommand">
                                <LayoutTemplate>
                                    <div id="draggableList">
                                        <asp:PlaceHolder ID="ph_Items" runat="server" />
                                    </div>
                                </LayoutTemplate>
                                <ItemTemplate>
                                    <div class="blockItem">
                                        <div class="bq-callout orange">
                                            <div class="row">
                                                <div class="col-sm-9">
                                                    <h4>
                                                        <%#Eval("Block_Title") %>
                                                    </h4>
                                                    <div>
                                                        <%# HttpUtility.HtmlDecode(Eval("Block_Desc").ToString()) %>
                                                    </div>
                                                </div>
                                                <div class="col-sm-3 text-right" style="padding-right: 20px;">
                                                    <a class="btn btn-info tip dragMe" title="拖動我可排序"><i class="fa fa-arrows"></i></a>&nbsp;&nbsp;
                                                    <a href="<%=Application["WebUrl"] %>FAQ/Edit/DetailBox/<%#Param_thisID %>/<%# Cryptograph.MD5Encrypt(Eval("Block_ID").ToString(), Application["DesKey"].ToString()) %>" class="btn btn-primary tip myBox" title="區塊修改"><i class="fa fa-pencil"></i></a>&nbsp;
                                                    <asp:LinkButton ID="lbtn_Del" CommandName="Del" runat="server" CssClass="btn btn-danger tip" OnClientClick="return confirm('是否確定刪除!?')" ToolTip="刪除此筆"><i class="fa fa-trash-o"></i></asp:LinkButton>
                                                </div>
                                            </div>
                                        </div>

                                        <asp:HiddenField ID="hf_DataID" runat="server" Value='<%#Eval("Block_ID") %>' />
                                        <asp:TextBox ID="tb_Sort" runat="server" Text='<%#Eval("Sort") %>' CssClass="tb-sortid hidden"></asp:TextBox>
                                    </div>
                                </ItemTemplate>
                                <EmptyDataTemplate>
                                    <div class="text-center styleReddark" style="margin: 0 auto;">
                                        <h3><span class="glyphicon glyphicon-exclamation-sign"></span>&nbsp;尚未新增區塊資料</h3>
                                    </div>
                                </EmptyDataTemplate>
                            </asp:ListView>

                            <div class="text-right">
                                <asp:Button ID="btn_SaveBlock2" runat="server" Text="↑ 儲存版面排序 ↑" CssClass="btn btn-success" OnClick="btn_SaveSort_Click" ValidationGroup="List" />
                            </div>
                        </div>
                    </div>
                    <!-- Table Content End -->
                </div>
            </div>
            <div class="col-sm-3 col-md-2"></div>
        </div>
        <!-- Block List End -->
    </asp:PlaceHolder>
    <!-- 區塊內容 End -->
</asp:Content>
<asp:Content ID="myScript" ContentPlaceHolderID="ScriptContent" runat="Server">
    <%: Scripts.Render("~/bundles/JQ-UI-script") %>
    <%: Scripts.Render("~/bundles/blockUI-script") %>
    <%: Scripts.Render("~/bundles/CKEditor-script") %>

    <script>
        $(function () {
            /* tooltip */
            $('.tip').tooltip({
                html: true,
                //trigger: 'focus',  /* click | hover | focus */
                placement: 'bottom' /*  top | bottom | left | right | auto */
            });

            //滑動到指定內容區 Start
            <% if (Param_thisID != "")
               { %>
            //取得元素
            var _thisID = $('#block');

            //滑動至指定ID
            $('html, body').animate({
                scrollTop: $(_thisID).offset().top - 100
            }, 600);

            //將基本設定收合
            //$('#data').collapse('hide');
            <%}%>
            //滑動到指定內容區 End

            /* jQuery sortable Start */
            var panelList = $('#draggableList');

            panelList.sortable({
                handle: '.dragMe',
                update: function () {
                    $('.blockItem', panelList).each(function (index, elem) {
                        var $listItem = $(elem),
                            newIndex = $listItem.index();

                        //填入排序
                        var newSort = newIndex + 1;
                        $(this).find('.tb-sortid').val(newSort);

                    });
                }
            });
            /* jQuery sortable End */
        });

    </script>

    <%-- FancyBox Start --%>
    <%: Scripts.Render("~/bundles/fancybox-script") %>
    <script>
        $(function () {
            /* fancybox - 編輯視窗 */
            $(".myBox").fancybox({
                type: 'iframe',
                fitToView: true,
                autoSize: true,
                closeClick: false,
                openEffect: 'elastic', // 'elastic', 'fade' or 'none'
                closeEffect: 'none',
                afterClose: function () { // 關閉後自動reload
                    parent.location.reload(true);
                }
            });
        });
    </script>
    <%-- FancyBox End --%>
</asp:Content>
