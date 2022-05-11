<%@ Page Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="PriceList_Index.aspx.cs" Inherits="PriceList" %>

<%@ Register Src="~/myController/Ascx_Navi.ascx" TagName="Ascx_Navi" TagPrefix="ucNavi" %>
<%@ Import Namespace="ExtensionMethods" %>
<asp:Content ID="myCss" ContentPlaceHolderID="CssContent" runat="Server">
    <%: Styles.Render("~/bundles/support-css") %>
</asp:Content>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="Server">
    <div class="container df-container-margin">
        <!-- 路徑導航 Start -->
        <ucNavi:Ascx_Navi ID="Ascx_Navi1" runat="server" Param_CurrID="305" />
        <!-- 路徑導航 End -->
        <!-- Content Start -->
        <div class="page-header df-page-header">
            <h3><%=this.GetLocalResourceObject("text_報價單分類篩選").ToString()%></h3><span style="display:none;"><%=fn_Param.Get_CustID %></span>
        </div>
        <div class="panel panel-info">
            <div class="panel-heading">
                <div class="pull-left">
                    <div class="checkbox">
                        <label>
                            <input type="checkbox" id="chkAll" value="-1" checked="checked" /><b><%=this.GetLocalResourceObject("text_全部分類").ToString()%></b></label>
                    </div>
                </div>
                <div class="pull-right">
                    <a id="goNext" class="btn btn-success"><%=this.GetLocalResourceObject("text_下一步").ToString()%></a>
                    <a href="<%=Application["WebUrl"] %>Report" class="btn btn-default"><i class="fa fa-home" aria-hidden="true"></i>&nbsp;<%=this.GetLocalResourceObject("text_返回").ToString()%></a>
                </div>
                <div class="clearfix"></div>
            </div>
            <asp:ListView ID="lvDataList" runat="server" ItemPlaceholderID="ph_Items" GroupPlaceholderID="ph_Group" GroupItemCount="4">
                <LayoutTemplate>
                    <table class="table table-bordered">
                        <asp:PlaceHolder ID="ph_Group" runat="server" />
                    </table>
                </LayoutTemplate>
                <GroupTemplate>
                    <tr>
                        <asp:PlaceHolder ID="ph_Items" runat="server" />
                    </tr>
                </GroupTemplate>
                <EmptyItemTemplate>
                    <td></td>
                </EmptyItemTemplate>
                <ItemTemplate>
                    <td>
                        <div class="checkbox">
                            <label>
                                <input type="checkbox" id="item_<%#Eval("Class_ID") %>" class="ProdCls" value="<%#Eval("Class_ID") %>" checked="checked" /><%#Eval("Class_Name") %>
                            </label>
                        </div>
                    </td>
                </ItemTemplate>
            </asp:ListView>
        </div>
        <!-- Content End -->
    </div>
</asp:Content>
<asp:Content ID="myScript" ContentPlaceHolderID="ScriptContent" runat="Server">
    <script>
        $(function () {
            /* trigger click */
            $("#goNext").click(function () {
                $("#MainContent_btn_goNext").trigger("click");
            });

            /* 全選按鈕 */
            $("#chkAll").click(function () {
                var eleItem = $("input.ProdCls");
                
                if ($(this).prop("checked")) {
                    eleItem.each(function () {
                        $(this).prop("checked", true);
                    });
                }
                else {
                    eleItem.each(function () {
                        $(this).prop("checked", false);
                    });
                }
            });

            /* 核選方塊Click時檢查 */
            $("input.ProdCls").click(function () {
                var itemChecked = 0;
                var itemLength = $("input.ProdCls").length;
                var eleAll = $("#chkAll");
                var eleItem = $("input.ProdCls");

                eleItem.each(function () {
                    if ($(this).prop("checked")) {
                        itemChecked++;
                    }
                });

                if (itemChecked == itemLength) {
                    eleAll.prop("checked", true);
                } else {
                    eleAll.prop("checked", false);
                }
            });

            /* 下一步 */
            $("#goNext").click(function () {
                var url = '<%=Application["WebUrl"]%>PriceList?Cls=';
                //將核取值存至陣列
                cbxValue = new Array();
                $('input:checkbox:checked[class="ProdCls"]').each(function (i) { cbxValue[i] = this.value; });

                //若無勾選則送出 -1
                if (cbxValue.length == 0) {
                    url += '-1';
                } else {
                    //送出值加入 , 作分隔
                    url += encodeURIComponent(cbxValue.join(','));
                }
                
                //Redirect
                location.href = url;
            });
           
        });
    </script>
</asp:Content>

