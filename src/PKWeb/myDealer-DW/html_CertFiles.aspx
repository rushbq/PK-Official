<%@ Page Language="C#" AutoEventWireup="true" CodeFile="html_CertFiles.aspx.cs" Inherits="html_CertFiles" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>附件List</title>
    <asp:PlaceHolder runat="server">
        <%: Styles.Render("~/bundles/css") %>
    </asp:PlaceHolder>
</head>
<body>
    <form id="form1" runat="server">
        <div class="modal-header text-left">
            <button type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
            <h4 class="modal-title text-green">
                <asp:Literal ID="lt_ID" runat="server"></asp:Literal></h4>
            <h4>
                <asp:Literal ID="lt_Label" runat="server"></asp:Literal></h4>
        </div>
        <div class="modal-body">
            <asp:Panel ID="pl_warning" runat="server" CssClass="alert alert-danger" Visible="false">
                <i class="fa fa-exclamation-triangle" aria-hidden="true"></i>&nbsp;<%=this.GetLocalResourceObject("txt_警告").ToString()%>
            </asp:Panel>
            <table class="table table-striped">
                <thead>
                    <tr>
                        <th><%=this.GetLocalResourceObject("title_證書類別").ToString()%></th>
                        <th><%=this.GetLocalResourceObject("title_證書").ToString()%></th>
                        <th><%=this.GetLocalResourceObject("title_測試報告").ToString()%></th>
                        <th><%=this.GetLocalResourceObject("title_自我宣告").ToString()%></th>
                        <th><%=this.GetLocalResourceObject("title_自我檢測").ToString()%></th>
                    </tr>
                </thead>
                <tbody>
                    <asp:ListView ID="lvDataList" runat="server" ItemPlaceholderID="ph_Items" OnItemDataBound="lvDataList_ItemDataBound">
                        <LayoutTemplate>

                            <asp:PlaceHolder ID="ph_Items" runat="server" />

                        </LayoutTemplate>
                        <ItemTemplate>
                            <tr>
                                <th scope="row"><%#Eval("ClassName") %></th>
                                <td>
                                    <asp:Literal ID="lt_CEFile" runat="server"></asp:Literal>
                                </td>
                                <td>
                                    <asp:Literal ID="lt_TestReport" runat="server"></asp:Literal>
                                </td>
                                <td>
                                    <asp:Literal ID="lt_SelfCE" runat="server"></asp:Literal>
                                </td>
                                <td>
                                    <asp:Literal ID="lt_SelfCheck" runat="server"></asp:Literal>
                                </td>
                            </tr>
                        </ItemTemplate>
                    </asp:ListView>
                </tbody>
            </table>
        </div>
        <div class="modal-footer">
            <button type="button" class="btn btn-primary" data-dismiss="modal">Close</button>
        </div>
    </form>
</body>
</html>
