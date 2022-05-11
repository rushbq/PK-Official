<%@ Page Language="C#" MasterPageFile="~/Site_Box.master" AutoEventWireup="true" CodeFile="Lot_NameList.aspx.cs" Inherits="Lot_NameList" %>

<asp:Content ID="myCss" ContentPlaceHolderID="CssContent" runat="Server">
</asp:Content>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="Server">
    <div class="container">
        <div class="page-header">
            <h3>中獎名單&nbsp;<small><asp:Literal ID="lt_subTitle" runat="server"></asp:Literal></small></h3>
        </div>
        <div>
            <div class="pull-left form-inline">
                <div class="form-group input-group">
                    <asp:TextBox ID="tb_Keyword" runat="server" CssClass="form-control myEnter" MaxLength="50"></asp:TextBox>
                    <div class="input-group-btn">
                        <button type="button" class="btn btn-success doSearch"><span class="fa fa-search"></span></button>
                        <a class="btn btn-default" href="<%=Application["WebUrl"] %>Winner/<%=Req_EncID %>"><span class="fa fa-times"></span></a>
                    </div>
                </div>

                <asp:Button ID="btn_Search" runat="server" OnClick="btn_Search_Click" Style="display: none;" />
            </div>
            <div class="pull-right hidden-xs">
                <asp:Literal ID="lt_Pager_top" runat="server"></asp:Literal>
            </div>
            <div class="clearfix"></div>
        </div>

        <!-- Content Start -->
        <asp:ListView ID="lvDataList" runat="server" ItemPlaceholderID="ph_Items" OnItemDataBound="lvDataList_ItemDataBound" OnItemCommand="lvDataList_ItemCommand">
            <LayoutTemplate>
                <table class="table table-striped">
                    <thead>
                        <tr>
                            <th>#</th>
                            <th>會員名稱</th>
                            <th>獎項</th>
                            <th>確認</th>
                        </tr>
                    </thead>
                    <tbody>
                        <asp:PlaceHolder ID="ph_Items" runat="server" />
                    </tbody>
                </table>

                <asp:Literal ID="lt_Pager_footer" runat="server"></asp:Literal>
            </LayoutTemplate>
            <ItemTemplate>
                <tr>
                    <th scope="row">
                        <%#Container.DataItemIndex + 1 %>
                    </th>
                    <td>
                        <label class="label label-danger"><%#Eval("FirstName") %><%#Eval("LastName") %></label>
                        <div style="padding-top: 8px;"><%#Eval("Mem_Account") %></div>
                    </td>
                    <td>
                        <label class="label label-default"><%#Eval("id") %></label>
                        <div style="padding-top: 8px;"><%#Eval("label") %></div>
                    </td>
                    <td>
                        <asp:LinkButton ID="lbtn_Confirm" CommandName="doConfirm" runat="server" CssClass="btn btn-success" ToolTip="確認領取"><i class="fa fa-check-circle"></i></asp:LinkButton>
                        <asp:LinkButton ID="lbtn_Undo" CommandName="doUndo" runat="server" CssClass="btn btn-warning" ToolTip="回復狀態"><i class="fa fa-undo"></i></asp:LinkButton>
                    </td>
                </tr>
                <asp:HiddenField ID="hf_Lot_PID" runat="server" Value='<%#Eval("Lot_PID") %>' />
                <asp:HiddenField ID="hf_Mem_ID" runat="server" Value='<%#Eval("Mem_ID") %>' />
            </ItemTemplate>
            <EmptyDataTemplate>
                <div class="exception-info">
                    <div class="icon"><i class="fa fa-exclamation-triangle"></i></div>
                    <div class="message">Oops! <%=Resources.resPublic.txt_查無資料 %></div>
                </div>
            </EmptyDataTemplate>
        </asp:ListView>

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
        });
    </script>
</asp:Content>

