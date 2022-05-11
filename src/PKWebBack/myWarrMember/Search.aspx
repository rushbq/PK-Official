<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="Search.aspx.cs" Inherits="myWarrMember_Search" %>

<%@ Import Namespace="ExtensionMethods" %>
<asp:Content ID="myCss" ContentPlaceHolderID="CssContent" runat="Server">
    <%: Styles.Render("~/bundles/DTpicker-css") %>
</asp:Content>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="Server">
    <!-- Page Header Start -->
    <div class="row">
        <div class="col-md-12">
            <div class="page-header">
                <h2>產品保固名單
                    <small>
                        <a>會員管理</a>&nbsp;
                        <i class="fa fa-chevron-right"></i>&nbsp;<span>產品保固名單</span>
                    </small>
                </h2>
            </div>
        </div>
    </div>
    <!-- Page Header End -->
    <!-- Filter Start -->
    <div class="row">
        <div class="col-md-12">
            <div class="panel panel-warning">
                <div class="panel-heading">
                    <i class="fa fa-filter"></i>
                    <span>資料篩選</span>
                </div>
                <div class="panel-body form-horizontal">
                    <div class="form-group">
                        <div class="col-xs-12">
                            <label>日期</label>
                            <div class="form-inline">
                                <asp:DropDownList ID="ddl_dateType" runat="server" CssClass="form-control">
                                    <asp:ListItem Value="1">註冊日期</asp:ListItem>
                                    <asp:ListItem Value="2">購買日期</asp:ListItem>
                                </asp:DropDownList>
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
                    <!-- 各區filter使用form-group,最後一區使用row -->
                    <div class="row">
                        <div class="col-xs-8">
                            <%-- <div class="input-group">
                                <span class="input-group-addon">關鍵字</span>
                                <asp:TextBox ID="tb_Keyword" runat="server" CssClass="form-control" placeholder="主旨" aria-label="主旨" MaxLength="50"></asp:TextBox>
                            </div>--%>
                        </div>
                        <div class="col-xs-4 text-right">
                            <asp:Button ID="btn_Search" runat="server" CssClass="btn btn-success" Text="開始查詢" ValidationGroup="Search" OnClick="btn_Search_Click" OnClientClick="blockBox2_NoMsg()" />
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <!-- Filter End -->
    <!-- Result Start -->
    <div class="row">
        <div class="col-md-12">
            <div class="panel panel-info">
                <div class="panel-heading">
                    <div class="pull-left">
                        <span class="fa fa-th-list"></span>&nbsp;資料列表
                    </div>
                    <div class="pull-right text-right">
                        <asp:Button ID="btn_Excel" runat="server" CssClass="btn btn-primary btn-sm" Text="匯出Excel" CausesValidation="false" OnClick="btn_Excel_Click" />
                    </div>
                    <div class="clearfix"></div>
                </div>
                <!-- Table Content Start -->
                <div style="margin-top: 10px;">
                    <asp:ListView ID="lvDataList" runat="server" ItemPlaceholderID="ph_Items">
                        <LayoutTemplate>
                            <table id="listTable" class="table table-bordered table-striped">
                                <thead>
                                    <tr>
                                        <th class="no-sort">編號</th>
                                        <th class="no-sort">Email / 姓名</th>
                                        <th>發票號碼</th>
                                        <th>註冊日期</th>
                                        <th>購買日期</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    <asp:PlaceHolder ID="ph_Items" runat="server" />
                                </tbody>
                            </table>
                        </LayoutTemplate>
                        <ItemTemplate>
                            <tr>
                                <td class="text-center"><%#Eval("ID") %></td>
                                <td>
                                    <p>
                                        <strong><%#Eval("Email") %></strong>
                                    </p>
                                    <p>
                                        <label class="label label-default"><%#Eval("Country_Name") %></label>
                                        <%#Eval("FirstName") %>&nbsp;<%#Eval("LastName") %>
                                    </p>
                                </td>
                                <td>
                                    <%#Eval("Invoice") %>
                                </td>
                                <td>
                                    <%#Eval("RegDate").ToString().ToDateString("yyyy-MM-dd") %>                                    
                                </td>
                                <td>
                                    <%#Eval("BuyDate").ToString().ToDateString("yyyy-MM-dd") %>                                    
                                </td>
                            </tr>
                        </ItemTemplate>
                        <EmptyDataTemplate>
                            <div class="text-center text-danger" style="padding: 60px 0px 60px 0px;">
                                <h3><i class="fa fa-exclamation-triangle" aria-hidden="true"></i>&nbsp;查無資料</h3>
                            </div>
                        </EmptyDataTemplate>

                    </asp:ListView>
                </div>
                <!-- Table Content End -->
            </div>
        </div>
    </div>
    <!-- Result End -->
</asp:Content>
<asp:Content ID="myScript" ContentPlaceHolderID="ScriptContent" runat="Server">
    <%: Scripts.Render("~/bundles/blockUI-script") %>

    <%-- DatePicker Start --%>
    <%: Scripts.Render("~/bundles/Moment-script") %>
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
    <%-- DataTable Start --%>
    <link href="https://cdn.datatables.net/1.10.11/css/dataTables.bootstrap.min.css" rel="stylesheet" />
    <script src="https://cdn.datatables.net/1.10.11/js/jquery.dataTables.min.js"></script>
    <script src="https://cdn.datatables.net/1.10.11/js/dataTables.bootstrap.min.js"></script>
    <script>
        $(function () {
            $('#listTable').DataTable({
                "searching": true,  //搜尋
                "ordering": true,   //排序
                "paging": true,     //分頁
                "info": false,      //筆數資訊
                //讓不排序的欄位在初始化時不出現排序圖
                "order": [],
                //自訂欄位
                "columnDefs": [{
                    "targets": 'no-sort',
                    "orderable": false,
                }]
            });
        });
    </script>
    <%-- DataTable End --%>
</asp:Content>

