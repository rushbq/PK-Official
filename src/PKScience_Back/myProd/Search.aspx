<%@ Page Title="產品資料查詢" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="Search.aspx.cs" Inherits="Prod_Search" %>

<%@ Import Namespace="ExtensionMethods" %>
<asp:Content ID="myCss" ContentPlaceHolderID="CssContent" runat="Server">
    <%: Styles.Render("~/bundles/DTpicker-css") %>
</asp:Content>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="Server">
    <!-- Page Header Start -->
    <div class="row">
        <div class="col-md-12">
            <ol class="breadcrumb">
                <li><a href="<%=Application["WebUrl"] %>">首頁</a></li>
                <li class="active">產品資料查詢</li>
            </ol>
        </div>
    </div>
    <!-- Page Header End -->
    <!-- Filter Start -->
    <div class="row">
        <div class="col-sm-8 col-md-10">
            <div class="panel panel-warning">
                <div class="panel-heading">
                    <div class="pull-left">
                        <span class="glyphicon glyphicon-filter"></span>
                        <span>篩選器</span>
                    </div>
                    <div class="pull-right">
                        <a data-toggle="collapse" href="#filter">
                            <span class="glyphicon glyphicon-sort"></span>
                        </a>
                    </div>
                    <div class="clearfix"></div>
                </div>
                <div id="filter" class="collapse in">
                    <div class="panel-body form-horizontal">
                        <!-- Filter Content Start -->
                        <div class="form-group">
                            <div class="col-sm-5">
                                <label>上架狀態</label>
                                <asp:DropDownList ID="ddl_Display" runat="server" CssClass="form-control"></asp:DropDownList>
                            </div>
                            <div class="col-sm-7">
                                <label>關鍵字搜尋(品號/目錄/頁次)</label>
                                <asp:TextBox ID="tb_Keyword" runat="server" CssClass="form-control" placeholder="輸入關鍵字:品號/目錄/頁次" MaxLength="20"></asp:TextBox>
                            </div>
                        </div>
                        <div class="form-group">
                            <div class="col-sm-12">
                                <label>上架日期</label>
                                <div class="form-inline">
                                    <div class="input-group date showDate" data-link-field="MainContent_tb_StartDate">
                                        <asp:TextBox ID="show_sDate" runat="server" CssClass="form-control text-center" ReadOnly="true"></asp:TextBox>
                                        <span class="input-group-addon"><span class="glyphicon glyphicon-remove"></span></span>
                                        <span class="input-group-addon"><span class="glyphicon glyphicon-calendar"></span></span>
                                    </div>
                                    ~
                                    <div class="input-group date showDate" data-link-field="MainContent_tb_EndDate">
                                        <asp:TextBox ID="show_eDate" runat="server" CssClass="form-control text-center" ReadOnly="true"></asp:TextBox>
                                        <span class="input-group-addon"><span class="glyphicon glyphicon-remove"></span></span>
                                        <span class="input-group-addon"><span class="glyphicon glyphicon-calendar"></span></span>
                                    </div>
                                </div>
                                <div>
                                    <asp:TextBox ID="tb_StartDate" runat="server" Style="display: none"></asp:TextBox>
                                    <asp:TextBox ID="tb_EndDate" runat="server" Style="display: none"></asp:TextBox>
                                </div>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-sm-5">
                                <label>產品類別</label>
                                <div class="form-inline">
                                    <asp:DropDownList ID="ddl_ProdClass" runat="server" CssClass="form-control"></asp:DropDownList>
                                </div>
                            </div>
                            <div class="col-sm-3">
                                <label></label>
                                <div class="form-inline">
                                    <asp:CheckBox ID="cb_StopOffer" runat="server" Text="已停售未下架" />&nbsp;&nbsp;
                                    <asp:CheckBox ID="cb_IsHot" runat="server" Text="熱銷推薦" />
                                </div>
                            </div>
                            <div class="col-sm-4">
                                <label></label>
                                <div class="text-right" style="vertical-align: bottom;">
                                    <asp:Button ID="btn_Search" runat="server" CssClass="btn btn-success" Text="開始查詢" ValidationGroup="Search" OnClick="btn_Search_Click" OnClientClick="blockBox2_NoMsg()" />
                                </div>
                            </div>
                        </div>
                        <!-- Filter Content End -->
                    </div>
                </div>
            </div>
        </div>
        <div class="col-sm-4 col-md-2">
            <div class="metro-nav metro-fix-view">
                <div class="metro-nav-block nav-light-blue">
                    <a href="<%=Application["WebUrl"] %>Prod/Edit/" class="text-center">
                        <i class="fa fa-pencil-square-o"></i>
                        <div class="status">產品上架</div>
                    </a>
                </div>
            </div>
        </div>
    </div>

    <!-- Filter End -->
    <!-- Result Start -->
    <div class="row">
        <div class="col-md-12">
            <div class="tab-content">
                <div class="tab-pane active">
                    <div class="panel panel-success">
                        <div class="panel-heading">
                            <div class="pull-left">
                                <span class="glyphicon glyphicon-list"></span>
                            </div>
                            <div class="pull-right">
                                <a data-toggle="collapse" href="#result">
                                    <span class="glyphicon glyphicon-sort"></span>
                                </a>
                            </div>
                            <div class="clearfix"></div>
                        </div>
                        <!-- Table Content Start -->
                        <div id="result" class="table-responsive collapse in">
                            <asp:ListView ID="lvDataList" runat="server" ItemPlaceholderID="ph_Items" OnItemCommand="lvDataList_ItemCommand" OnItemDataBound="lvDataList_ItemDataBound">
                                <LayoutTemplate>
                                    <table class="table table-bordered table-advance table-striped">
                                        <thead>
                                            <tr>
                                                <th style="width: 40%">品號/品名</th>
                                                <th style="width: 20%">上架期間</th>
                                                <th style="width: 20%">狀態</th>
                                                <th style="width: 20%">&nbsp;</th>
                                            </tr>
                                        </thead>
                                        <tbody>
                                            <asp:PlaceHolder ID="ph_Items" runat="server" />
                                        </tbody>
                                        <tfoot>
                                            <tr>
                                                <td colspan="5">
                                                    <asp:Literal ID="lt_Pager" runat="server"></asp:Literal>
                                                </td>
                                            </tr>
                                        </tfoot>
                                    </table>
                                </LayoutTemplate>
                                <ItemTemplate>
                                    <tr>
                                        <td>
                                            <p>
                                                <div class="pull-left"><strong><%#Eval("Model_No") %></strong></div>
                                                <div class="pull-right">
                                                    <label class="label label-default" title="目錄"><%#Eval("Catelog_Vol") %></label>
                                                    <label class="label label-default" title="頁次"><%#Eval("Page") %></label>
                                                </div>
                                                <div class="clearfix"></div>
                                            </p>
                                            <p>
                                                <%#Eval("Model_Name") %>
                                            </p>
                                        </td>
                                        <td class="text-center">
                                            <div><%#Eval("StartTime").ToString().ToDateString("yyyy-MM-dd HH:mm:ss") %></div>
                                            <div><i class="fa fa-chevron-down"></i></div>
                                            <div><%#Eval("EndTime").ToString().ToDateString("yyyy-MM-dd HH:mm:ss") %></div>
                                        </td>
                                        <td class="text-center">
                                            <asp:Label ID="lb_IsNew" runat="server" CssClass="label label-danger" Text="我是新品"></asp:Label>
                                            <asp:Label ID="lb_IsHot" runat="server" CssClass="label label-warning" Text="熱銷推薦"></asp:Label>
                                            <asp:Label ID="lb_IsStop" runat="server" CssClass="label label-default" Text="已停售"></asp:Label>
                                            <asp:Label ID="lb_Status" runat="server"></asp:Label>
                                        </td>
                                        <td class="text-center">
                                            <a href="<%=Application["WebUrl"] %>Prod/Edit/<%# Server.UrlEncode(Cryptograph.MD5Encrypt(Eval("Prod_ID").ToString(), fn_Param.DesKey))%>/" class="btn btn-primary">
                                                <i class="fa fa-pencil fa-lg"></i>
                                            </a>
                                            <asp:HiddenField ID="hf_DataID" runat="server" Value='<%#Eval("Prod_ID") %>' />
                                        </td>
                                    </tr>
                                </ItemTemplate>
                                <EmptyDataTemplate>
                                    <div class="text-center styleReddark" style="padding: 60px 0px 60px 0px;">
                                        <h3><span class="glyphicon glyphicon-exclamation-sign"></span>&nbsp;查無資料</h3>
                                    </div>
                                </EmptyDataTemplate>

                            </asp:ListView>
                        </div>
                        <!-- Table Content End -->
                    </div>
                </div>
            </div>
        </div>
    </div>
    <!-- Result End -->
</asp:Content>
<asp:Content ID="myScript" ContentPlaceHolderID="ScriptContent" runat="Server">
    <%: Scripts.Render("~/bundles/blockUI-script") %>
    <script>
        $(function () {
            //換頁時, 滑動到內容區
            <% if (Req_PageIdx != 1)
               { %>
            //取得元素
            var _thisID = $('#result');

            //滑動至指定ID
            $('html, body').animate({
                scrollTop: $(_thisID).offset().top - 110
            }, 600);
            <%}%>

        });
    </script>
    <%-- DatePicker Start --%>
    <%: Scripts.Render("~/bundles/DTpicker-script") %>
    <script>
        $(function () {
            $('.showDate').datetimepicker({
                format: 'yyyy/mm/dd',   //目前欄位格式
                //linkField: 'dtp_input2',    //鏡像欄位對應
                linkFormat: 'yyyy/mm/dd',   //鏡像欄位格式
                todayBtn: true,     //顯示today
                todayHighlight: true,   //將today設置高亮
                autoclose: true,    //選擇完畢後自動關閉
                startView: 4,    //選擇器開啟後，顯示的視圖(4:10年 ; 3:12月 ; 2:該月 ; 1:該日全時段 ; 0:該時段的各個時間,預設5分間隔)
                maxView: 4,
                minView: 2,
                forceParse: false
                //showMeridian: true, //顯示AM/PM
            });
        });

    </script>
    <%-- DatePicker End --%>
</asp:Content>

