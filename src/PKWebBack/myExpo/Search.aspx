<%@ Page Title="展覽活動查詢" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="Search.aspx.cs" Inherits="Expo_Search" %>

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
                <li class="active">展覽活動查詢</li>
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
                        
                    </div>
                    <div class="clearfix"></div>
                </div>
                <div id="filter" class="collapse in">
                    <div class="panel-body">
                        <!-- Filter Content Start -->
                        <div class="row">
                            <div class="col-sm-3">
                                <label>上架狀態</label>
                                <asp:DropDownList ID="ddl_Display" runat="server" CssClass="form-control"></asp:DropDownList>
                            </div>
                            <div class="col-sm-4">
                                <label>識別名稱</label>
                                <asp:TextBox ID="tb_Keyword" runat="server" CssClass="form-control" placeholder="輸入關鍵字" MaxLength="20"></asp:TextBox>
                            </div>
                            <div class="col-sm-3 hide">
                                <label>區域</label>
                                <div class="form-inline">
                                    <asp:CheckBoxList ID="cbl_Area" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow"></asp:CheckBoxList>
                                </div>
                            </div>
                        </div>
                        <div class="row">
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
                            <div class="col-sm-12">
                                <div class="text-right">
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
                    <a href="<%=Application["WebUrl"] %>Expo/Edit/" class="text-center">
                        <i class="fa fa-pencil-square-o"></i>
                        <div class="status">新增資料</div>
                    </a>
                </div>
            </div>
        </div>
    </div>

    <!-- Filter End -->
    <!-- Result Start -->
    <div class="row">
        <div class="col-md-12">
            <div class="panel panel-success">
                <div class="panel-heading">
                    <div class="pull-left">
                        <span class="glyphicon glyphicon-list"></span>
                    </div>
                    <div class="pull-right">
                        
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
                                        <th style="width: 10%">編號</th>
                                        <th style="width: 40%">識別名稱</th>
                                        <th style="width: 20%">上架期間</th>
                                        <th style="width: 15%">狀態</th>
                                        <th style="width: 15%">&nbsp;</th>
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
                                <td class="text-center">
                                    <%#Eval("Group_ID") %>
                                </td>
                                <td>
                                    <div>
                                        <p><%#Eval("Group_Name") %></p>
                                    </div>
                                </td>
                                <td class="text-center">
                                    <%#Eval("StartTime").ToString().ToDateString("yyyy-MM-dd HH:mm") %>
                                    <br />
                                    <i class="fa fa-chevron-down"></i>
                                    <br />
                                    <%#Eval("EndTime").ToString().ToDateString("yyyy-MM-dd HH:mm") %>
                                </td>
                                <td class="text-center">
                                    <asp:Label ID="lb_onIndex" runat="server" CssClass="label label-danger" Text="首頁顯示"></asp:Label>
                                    <asp:Label ID="lb_Status" runat="server"></asp:Label>
                                </td>
                                <td class="text-center">
                                    <a href="<%=Application["WebUrl"] %>Expo/Edit/<%# Server.UrlEncode(Cryptograph.MD5Encrypt(Eval("Group_ID").ToString(), Application["DesKey"].ToString()))%>/" class="btn btn-primary">
                                        <i class="fa fa-pencil fa-lg"></i>
                                    </a>&nbsp;
                                    <asp:LinkButton ID="lbtn_Del" CommandName="Del" runat="server" CssClass="btn btn-danger" OnClientClick="return confirm('是否確定刪除?')">
                                        <i class="fa fa-trash-o fa-lg"></i>
                                    </asp:LinkButton>
                                    <asp:HiddenField ID="hf_DataID" runat="server" Value='<%#Eval("Group_ID") %>' />
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

