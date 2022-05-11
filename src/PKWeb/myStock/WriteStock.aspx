<%@ Page Title="包材庫存盤點" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="WriteStock.aspx.cs" Inherits="myStock_WriteStock" %>

<asp:Content ID="myCss" ContentPlaceHolderID="CssContent" runat="Server">
    <%: Styles.Render("~/bundles/product-css") %>
</asp:Content>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="Server">
    <div class="container">
        <div class="page-header">
            <h3>
                <asp:Literal ID="lt_SupName" runat="server"></asp:Literal>
                &nbsp;<small>包材庫存盤點</small>
            </h3>
        </div>
        <!-- Content Start -->
        <div class="table-responsive">
            <table class="table table-bordered table-hover">
                <thead>
                    <tr class="active">
                        <th rowspan="2">品號</th>
                        <th rowspan="2">品名</th>
                        <th colspan="2" class="text-center">實盤數量(請填寫)</th>
                        <asp:PlaceHolder ID="ph_stockHeader" runat="server">
                            <th rowspan="2" class="text-center">寶工庫存數量</th>
                        </asp:PlaceHolder>
                    </tr>
                    <tr class="active">
                        <th class="text-center">未包裝數量</th>
                        <th class="text-center">已包裝未出貨數量</th>
                    </tr>
                </thead>

                <asp:ListView ID="lvDataList" runat="server" ItemPlaceholderID="ph_Items" OnItemDataBound="lvDataList_ItemDataBound">
                    <LayoutTemplate>
                        <tbody>
                            <asp:PlaceHolder ID="ph_Items" runat="server" />
                        </tbody>
                    </LayoutTemplate>
                    <ItemTemplate>
                        <tr>
                            <td class="text-danger">
                                <strong><%#Eval("ModelNo") %></strong>
                            </td>
                            <td class="text-muted">
                                <%#Eval("ModelName") %>
                            </td>
                            <td>
                                <asp:TextBox ID="tb_InputQty1" runat="server" CssClass="myEnter form-control" MaxLength="6" min="0" placeholder="填寫數字" autocomplete="off" type="number" step="1"></asp:TextBox>
                            </td>
                            <td>
                                <asp:TextBox ID="tb_InputQty2" runat="server" CssClass="myEnter form-control" MaxLength="6" min="0" placeholder="填寫數字" autocomplete="off" type="number" step="1"></asp:TextBox>
                            </td>
                            <asp:PlaceHolder ID="ph_stockBody" runat="server">
                                <td class="text-center">
                                    <%#Eval("StockNum") %>
                                </td>
                            </asp:PlaceHolder>
                            <asp:HiddenField ID="hf_DataID" runat="server" Value='<%#Eval("Data_ID") %>' />
                        </tr>
                    </ItemTemplate>
                </asp:ListView>

            </table>
        </div>
        <div class="text-right">
            <asp:Button ID="btn_Save" runat="server" Text="填寫完畢" CssClass="btn btn-lg btn-success" OnClientClick="return confirm('確定送出表單?\n送出後就不能修改.')" OnClick="btn_Save_Click" />
            <asp:HiddenField ID="hf_Parent_ID" runat="server" />
            <asp:HiddenField ID="hf_Token" runat="server" />
        </div>
        <!-- Content End -->
    </div>

</asp:Content>
<asp:Content ID="myScript" ContentPlaceHolderID="ScriptContent" runat="Server">
    <script>
        $(function () {
            //focus then select text
            $("input").focus(function () {
                $(this).select();
            });


            /* Enter 偵測 */
            $(".myEnter").keypress(function (e) {
                code = (e.keyCode ? e.keyCode : e.which);
                if (code == 13) {
                    $("#MainContent_btn_Save").trigger("click");
                    event.preventDefault();
                }
            });
        });
    </script>
</asp:Content>

