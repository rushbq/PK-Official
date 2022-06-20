<%@ Page Title="Tag維護" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="Tags_Search.aspx.cs" Inherits="Tags_Search" %>

<%@ Import Namespace="ExtensionMethods" %>
<asp:Content ID="myCss" ContentPlaceHolderID="CssContent" runat="Server">
</asp:Content>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="Server">
    <!-- Page Header Start -->
    <div class="row">
        <div class="col-md-12">
            <div class="page-header">
                <h4>Tag維護</h4>
            </div>
        </div>
    </div>
    <!-- Page Header End -->
    <!-- Filter Start -->
    <div class="row">
        <div class="col-sm-12 col-md-12">
            <div class="panel panel-warning">
                <div class="panel-heading">
                    <span class="glyphicon glyphicon-filter"></span>
                    <span>篩選器</span>
                </div>
                <div id="filter" class="collapse in">
                    <div class="panel-body">
                        <!-- Filter Content Start -->
                        <div class="form-inline">
                            <div class="form-group">
                                <label class="form-control-static">關鍵字查詢</label>
                            </div>
                            <div class="form-group">
                                <asp:TextBox ID="tb_Keyword" runat="server" CssClass="form-control" placeholder="輸入關鍵字" MaxLength="20"></asp:TextBox>
                            </div>
                            <asp:Button ID="btn_Search" runat="server" CssClass="btn btn-success" Text="開始查詢" ValidationGroup="Search" OnClick="btn_Search_Click" OnClientClick="blockBox2_NoMsg()" />
                            <a href="<%=Application["WebUrl"] %>Config/Tags" class="btn btn-info"><span class="glyphicon glyphicon-refresh"></span></a>
                        </div>
                        <!-- Filter Content End -->
                    </div>
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
                    <span class="glyphicon glyphicon-list"></span>
                    Tag清單&nbsp;<small>(未被使用的Tag才能刪除)</small>
                </div>
                <!-- Table Content Start -->
                <div id="result" class="table-responsive">
                    <asp:ListView ID="lvDataList" runat="server" ItemPlaceholderID="ph_Items" GroupPlaceholderID="ph_GroupItems" GroupItemCount="2" OnItemCommand="lvDataList_ItemCommand" OnItemDataBound="lvDataList_ItemDataBound">
                        <LayoutTemplate>
                            <table class="table table-bordered table-advance table-striped">
                                <thead>
                                    <tr>
                                        <th style="width: 35%">Tag名稱</th>
                                        <th style="width: 15%">&nbsp;</th>
                                        <th style="width: 35%">Tag名稱</th>
                                        <th style="width: 15%">&nbsp;</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    <asp:PlaceHolder ID="ph_GroupItems" runat="server" />
                                </tbody>
                                <tfoot>
                                    <tr>
                                        <td colspan="4">
                                            <asp:Literal ID="lt_Pager" runat="server"></asp:Literal>
                                        </td>
                                    </tr>
                                </tfoot>
                            </table>
                        </LayoutTemplate>
                        <GroupTemplate>
                            <tr>
                                <asp:PlaceHolder ID="ph_Items" runat="server" />
                            </tr>
                        </GroupTemplate>
                        <ItemTemplate>
                            <td>
                                <asp:TextBox ID="tb_Keyword" runat="server" CssClass="form-control myInput" Width="95%" Text='<%#Eval("Tag_Name") %>' MaxLength="50"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="rfv_tb_Keyword" runat="server" ErrorMessage="不可為空白"
                                    ControlToValidate="tb_Keyword" Display="Dynamic" CssClass="text-danger" ValidationGroup="List"></asp:RequiredFieldValidator>
                            </td>
                            <td>
                                <asp:LinkButton ID="lbtn_Edit" CommandName="Edit" runat="server" CssClass="btn btn-primary myEdit" ToolTip="修改" OnClientClick="blockBox1('List', '資料更新中...');">
                                        <i class="fa fa-save fa-lg"></i>
                                </asp:LinkButton>
                                <asp:LinkButton ID="lbtn_Del" CommandName="Del" runat="server" CssClass="btn btn-danger" OnClientClick="return confirm('確定要刪除嗎？')">
                                        <i class="fa fa-trash-o fa-lg"></i>
                                </asp:LinkButton>
                                <asp:HiddenField ID="hf_DataID" runat="server" Value='<%#Eval("Tag_ID") %>' />
                            </td>
                        </ItemTemplate>
                        <EmptyItemTemplate>
                            <td></td>
                            <td></td>
                        </EmptyItemTemplate>
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


            /* 偵測 enter */
            $(".myInput").keypress(function (e) {
                code = (e.keyCode ? e.keyCode : e.which);
                if (code == 13) {

                    return false;
                }
            });

        });
    </script>
</asp:Content>

