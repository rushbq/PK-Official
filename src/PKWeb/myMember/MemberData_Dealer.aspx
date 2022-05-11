<%@ Page Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="MemberData_Dealer.aspx.cs" Inherits="MemberData_Dealer" %>

<%@ Register Src="~/myController/Ascx_Navi.ascx" TagName="Ascx_Navi" TagPrefix="ucNavi" %>
<asp:Content ID="myCss" ContentPlaceHolderID="CssContent" runat="Server">
    <%: Styles.Render("~/bundles/member-css") %>
</asp:Content>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="Server">
    <div class="container df-container-margin">
        <!-- 路徑導航 Start -->
        <ucNavi:Ascx_Navi ID="Ascx_Navi1" runat="server" Param_CurrID="620" />
        <!-- 路徑導航 End -->
        <div class="row distributo_sheet">
            <div class="distributo_sheet_area col-sm-12">
                <div class="page-title">
                    <div class="header"><%=this.GetLocalResourceObject("title_DataSheet").ToString()%></div>
                </div>
                <form class="form-horizontal">
                    <div class="row form-group">
                        <label class="col-sm-4 control-label text-right"><%=this.GetLocalResourceObject("txt_CompName").ToString()%></label>
                        <div class="col-sm-8 has-error">
                            <asp:TextBox ID="CompName" runat="server" CssClass="form-control" MaxLength="200"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfv_CompName" runat="server" Display="Dynamic"
                                ControlToValidate="CompName" ValidationGroup="Add"><div class="alert alert-danger"><%=this.GetLocalResourceObject("tip_Require").ToString()%></div></asp:RequiredFieldValidator>
                        </div>
                    </div>
                    <div class="row form-group">
                        <label class="col-sm-4 control-label text-right"><%=this.GetLocalResourceObject("txt_CompCaptital").ToString()%></label>
                        <div class="col-sm-8">
                            <asp:TextBox ID="CompCaptital" runat="server" CssClass="form-control" MaxLength="100"></asp:TextBox>
                        </div>
                    </div>
                    <div class="row form-group">
                        <label class="col-sm-4 control-label text-right"><%=this.GetLocalResourceObject("txt_CEO").ToString()%></label>
                        <div class="col-sm-8">
                            <asp:TextBox ID="CEO" runat="server" CssClass="form-control" MaxLength="50"></asp:TextBox>
                        </div>
                    </div>
                    <div class="row form-group">
                        <label class="col-sm-4 control-label text-right"><%=this.GetLocalResourceObject("txt_Tel").ToString()%></label>
                        <div class="col-sm-8 has-error">
                            <asp:TextBox ID="Tel" runat="server" CssClass="form-control" MaxLength="50"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfv_Tel" runat="server" Display="Dynamic"
                                ControlToValidate="Tel" ValidationGroup="Add"><div class="alert alert-danger"><%=this.GetLocalResourceObject("tip_Require").ToString()%></div></asp:RequiredFieldValidator>
                        </div>
                    </div>
                    <div class="row form-group">
                        <label class="col-sm-4 control-label text-right"><%=this.GetLocalResourceObject("txt_Fax").ToString()%></label>
                        <div class="col-sm-8 has-error">
                            <asp:TextBox ID="Fax" runat="server" CssClass="form-control" MaxLength="50"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfv_Fax" runat="server" Display="Dynamic"
                                ControlToValidate="Fax" ValidationGroup="Add"><div class="alert alert-danger"><%=this.GetLocalResourceObject("tip_Require").ToString()%></div></asp:RequiredFieldValidator>
                        </div>
                    </div>
                    <div class="row form-group">
                        <label class="col-sm-4 control-label text-right"><%=this.GetLocalResourceObject("txt_WebSite").ToString()%></label>
                        <div class="col-sm-8">
                            <asp:TextBox ID="WebSite" runat="server" CssClass="form-control" MaxLength="200"></asp:TextBox>
                            <asp:RegularExpressionValidator ID="rev_WebSite" runat="server" ValidationExpression="http(s)?://([\w-]+\.)+[\w-]+(/[\w- ./?%&=]*)?" ControlToValidate="WebSite" Display="Dynamic" ValidationGroup="Add"><div class="alert alert-danger"><%=this.GetLocalResourceObject("tip_Format").ToString()%></div></asp:RegularExpressionValidator>
                        </div>
                    </div>
                    <div class="row form-group">
                        <label class="col-sm-4 control-label text-right"><%=this.GetLocalResourceObject("txt_Address").ToString()%></label>
                        <div class="col-sm-8 has-error">
                            <asp:TextBox ID="Address" runat="server" CssClass="form-control" MaxLength="200"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfv_Address" runat="server" Display="Dynamic"
                                ControlToValidate="Address" ValidationGroup="Add"><div class="alert alert-danger"><%=this.GetLocalResourceObject("tip_Require").ToString()%></div></asp:RequiredFieldValidator>
                        </div>
                    </div>
                    <div class="row form-group">
                        <label class="col-sm-4 control-label text-right"><%=this.GetLocalResourceObject("txt_City").ToString()%></label>
                        <div class="col-sm-8 has-error">
                            <asp:TextBox ID="City" runat="server" CssClass="form-control" MaxLength="50"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfv_City" runat="server" Display="Dynamic"
                                ControlToValidate="City" ValidationGroup="Add"><div class="alert alert-danger"><%=this.GetLocalResourceObject("tip_Require").ToString()%></div></asp:RequiredFieldValidator>
                        </div>
                    </div>
                    <div class="row form-group">
                        <label class="col-sm-4 control-label text-right"><%=this.GetLocalResourceObject("txt_State").ToString()%></label>
                        <div class="col-sm-8 has-error">
                            <asp:TextBox ID="State" runat="server" CssClass="form-control" MaxLength="50"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfv_State" runat="server" Display="Dynamic"
                                ControlToValidate="State" ValidationGroup="Add"><div class="alert alert-danger"><%=this.GetLocalResourceObject("tip_Require").ToString()%></div></asp:RequiredFieldValidator>
                        </div>
                    </div>
                    <div class="row form-group">
                        <label class="col-sm-4 control-label text-right"><%=this.GetLocalResourceObject("txt_ZIP").ToString()%></label>
                        <div class="col-sm-8">
                            <asp:TextBox ID="ZIP" runat="server" CssClass="form-control" MaxLength="10"></asp:TextBox>
                            <%--   <asp:RequiredFieldValidator ID="rfv_ZIP" runat="server" Display="Dynamic"
                                ControlToValidate="ZIP" ValidationGroup="Add"><div class="alert alert-danger"><%=this.GetLocalResourceObject("tip_Require").ToString()%></div></asp:RequiredFieldValidator>--%>
                        </div>
                    </div>
                    <div class="row form-group">
                        <label class="col-sm-4 control-label text-right"><%=this.GetLocalResourceObject("txt_Country").ToString()%></label>
                        <div class="col-sm-8 has-error">
                            <asp:TextBox ID="Country" runat="server" CssClass="form-control" MaxLength="40"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfv_Country" runat="server" Display="Dynamic"
                                ControlToValidate="Country" ValidationGroup="Add"><div class="alert alert-danger"><%=this.GetLocalResourceObject("tip_Require").ToString()%></div></asp:RequiredFieldValidator>
                        </div>
                    </div>
                    <div class="row form-group">
                        <label class="col-sm-4 control-label text-right"><%=this.GetLocalResourceObject("txt_EmpNum1").ToString()%></label>
                        <div class="col-sm-1">
                            <asp:TextBox ID="EmpNum1" runat="server" CssClass="form-control" MaxLength="5"></asp:TextBox>
                            <asp:CompareValidator ID="cv_EmpNum1" runat="server" ControlToValidate="EmpNum1"
                                Display="Dynamic" Operator="DataTypeCheck" Type="Integer" ValidationGroup="Add"><div class="alert alert-danger"><%=this.GetLocalResourceObject("tip_Format").ToString()%></div></asp:CompareValidator>
                        </div>
                        <label class="col-sm-1 control-label text-right"><%=this.GetLocalResourceObject("txt_EmpNum2").ToString()%></label>
                        <div class="col-sm-1">
                            <asp:TextBox ID="EmpNum2" runat="server" CssClass="form-control" MaxLength="5"></asp:TextBox>
                            <asp:CompareValidator ID="cv_EmpNum2" runat="server" ControlToValidate="EmpNum2"
                                Display="Dynamic" Operator="DataTypeCheck" Type="Integer" ValidationGroup="Add"><div class="alert alert-danger"><%=this.GetLocalResourceObject("tip_Format").ToString()%></div></asp:CompareValidator>
                        </div>
                        <label class="col-sm-1 control-label text-right"><%=this.GetLocalResourceObject("txt_EmpNum3").ToString()%></label>
                        <div class="col-sm-1">
                            <asp:TextBox ID="EmpNum3" runat="server" CssClass="form-control" MaxLength="5"></asp:TextBox>
                            <asp:CompareValidator ID="cv_EmpNum3" runat="server" ControlToValidate="EmpNum3"
                                Display="Dynamic" Operator="DataTypeCheck" Type="Integer" ValidationGroup="Add"><div class="alert alert-danger"><%=this.GetLocalResourceObject("tip_Format").ToString()%></div></asp:CompareValidator>
                        </div>
                        <label class="col-sm-1 control-label text-right"><%=this.GetLocalResourceObject("txt_EmpNum4").ToString()%></label>
                        <div class="col-sm-1">
                            <asp:TextBox ID="EmpNum4" runat="server" CssClass="form-control" MaxLength="5"></asp:TextBox>
                            <asp:CompareValidator ID="cv_EmpNum4" runat="server" ControlToValidate="EmpNum4"
                                Display="Dynamic" Operator="DataTypeCheck" Type="Integer" ValidationGroup="Add"><div class="alert alert-danger"><%=this.GetLocalResourceObject("tip_Format").ToString()%></div></asp:CompareValidator>
                        </div>
                    </div>
                    <div class="row form-group">
                        <label class="col-sm-4 control-label text-right"><%=this.GetLocalResourceObject("txt_CP_Name1").ToString()%></label>
                        <div class="col-sm-8 has-error">
                            <asp:TextBox ID="CP_Name1" runat="server" CssClass="form-control" MaxLength="100"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfv_CP_Name1" runat="server" Display="Dynamic"
                                ControlToValidate="CP_Name1" ValidationGroup="Add"><div class="alert alert-danger"><%=this.GetLocalResourceObject("tip_Require").ToString()%></div></asp:RequiredFieldValidator>
                        </div>
                    </div>
                    <div class="row form-group">
                        <label class="col-sm-4 control-label text-right"><%=this.GetLocalResourceObject("txt_CP_Dept1").ToString()%></label>
                        <div class="col-sm-8 has-error">
                            <asp:TextBox ID="CP_Dept1" runat="server" CssClass="form-control" MaxLength="50"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfv_CP_Dept1" runat="server" Display="Dynamic"
                                ControlToValidate="CP_Dept1" ValidationGroup="Add"><div class="alert alert-danger"><%=this.GetLocalResourceObject("tip_Require").ToString()%></div></asp:RequiredFieldValidator>
                        </div>
                    </div>
                    <div class="row form-group">
                        <label class="col-sm-4 control-label text-right"><%=this.GetLocalResourceObject("txt_CP_Line1").ToString()%></label>
                        <div class="col-sm-8">
                            <asp:TextBox ID="CP_Line1" runat="server" CssClass="form-control" MaxLength="50"></asp:TextBox>
                        </div>
                    </div>
                    <div class="row form-group">
                        <label class="col-sm-4 control-label text-right"><%=this.GetLocalResourceObject("txt_CP_Email1").ToString()%></label>
                        <div class="col-sm-8 has-error">
                            <asp:TextBox ID="CP_Email1" runat="server" CssClass="form-control" MaxLength="200"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfv_CP_Email1" runat="server" Display="Dynamic"
                                ControlToValidate="CP_Email1" ValidationGroup="Add"><div class="alert alert-danger"><%=this.GetLocalResourceObject("tip_Require").ToString()%></div></asp:RequiredFieldValidator>
                            <asp:RegularExpressionValidator ID="rev_CP_Email1" runat="server" ControlToValidate="CP_Email1" Display="Dynamic" ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*" ValidationGroup="Add"><div class="alert alert-danger"><%=this.GetLocalResourceObject("tip_Format").ToString()%></div></asp:RegularExpressionValidator>
                        </div>
                    </div>
                    <div class="row form-group">
                        <label class="col-sm-4 control-label text-right"><%=this.GetLocalResourceObject("txt_CP_Name2").ToString()%></label>
                        <div class="col-sm-8">
                            <asp:TextBox ID="CP_Name2" runat="server" CssClass="form-control" MaxLength="100"></asp:TextBox>
                        </div>
                    </div>
                    <div class="row form-group">
                        <label class="col-sm-4 control-label text-right"><%=this.GetLocalResourceObject("txt_CP_Dept2").ToString()%></label>
                        <div class="col-sm-8">
                            <asp:TextBox ID="CP_Dept2" runat="server" CssClass="form-control" MaxLength="50"></asp:TextBox>
                        </div>
                    </div>
                    <div class="row form-group">
                        <label class="col-sm-4 control-label text-right"><%=this.GetLocalResourceObject("txt_CP_Line2").ToString()%></label>
                        <div class="col-sm-8">
                            <asp:TextBox ID="CP_Line2" runat="server" CssClass="form-control" MaxLength="50"></asp:TextBox>
                        </div>
                    </div>
                    <div class="row form-group">
                        <label class="col-sm-4 control-label text-right"><%=this.GetLocalResourceObject("txt_CP_Email2").ToString()%></label>
                        <div class="col-sm-8">
                            <asp:TextBox ID="CP_Email2" runat="server" CssClass="form-control" MaxLength="200"></asp:TextBox>
                            <asp:RegularExpressionValidator ID="rev_CP_Email2" runat="server" ControlToValidate="CP_Email2" Display="Dynamic" ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*" ValidationGroup="Add"><div class="alert alert-danger"><%=this.GetLocalResourceObject("tip_Format").ToString()%></div></asp:RegularExpressionValidator>
                        </div>
                    </div>
                    <asp:PlaceHolder ID="ph_BusinessType" runat="server">
                        <div class="row form-group">
                            <label class="col-sm-4 control-label text-right"><%=this.GetLocalResourceObject("txt_BusinessType").ToString()%></label>
                            <div class="col-sm-8">
                                <asp:CheckBoxList ID="cb_BusinessType" runat="server" RepeatDirection="Horizontal" RepeatLayout="Table" RepeatColumns="2"></asp:CheckBoxList>
                                <div>
                                    <asp:TextBox ID="BusinessType_Other" runat="server" CssClass="form-control" MaxLength="50" placeholder="Other..."></asp:TextBox>
                                </div>
                            </div>
                        </div>
                    </asp:PlaceHolder>
                    <div class="row form-group">
                        <label class="col-sm-4 control-label text-right"><%=this.GetLocalResourceObject("txt_BranchOffice").ToString()%></label>
                        <div class="col-sm-8">
                            <asp:TextBox ID="BranchOffice" runat="server" CssClass="form-control" MaxLength="50"></asp:TextBox>
                        </div>
                    </div>
                    <div class="row form-group">
                        <label class="col-sm-4 control-label text-right"><%=this.GetLocalResourceObject("txt_Annual").ToString()%></label>
                        <div class="col-sm-8">
                            <asp:TextBox ID="Annual" runat="server" CssClass="form-control" MaxLength="10"></asp:TextBox>
                        </div>
                    </div>

                    <!-- eng field start -->
                    <asp:PlaceHolder ID="ph_FieldOfEnglish" runat="server">
                        <div class="row form-group">
                            <label class="col-sm-4 control-label text-right">Establish tool product line since year</label>
                            <div class="col-sm-8">
                                <asp:TextBox ID="ProductYear" runat="server" CssClass="form-control" MaxLength="10"></asp:TextBox>
                            </div>
                        </div>
                        <div class="row form-group">
                            <label class="col-sm-4 control-label text-right">Annual turn over of tools product in US$</label>
                            <div class="col-sm-8">
                                <asp:TextBox ID="AnnualProduct" runat="server" CssClass="form-control" MaxLength="10"></asp:TextBox>
                            </div>
                        </div>
                        <div class="row form-group">
                            <label class="col-sm-4 control-label text-right">Agent / Distributor for famous brands</label>
                            <div class="col-sm-8">
                                <asp:TextBox ID="AgentBrands" runat="server" CssClass="form-control" MaxLength="50"></asp:TextBox>
                            </div>
                        </div>
                        <div class="row form-group">
                            <label class="col-sm-4 control-label text-right">Main market &amp; ratio</label>
                            <div class="col-sm-8">
                                <asp:TextBox ID="MainMarket" runat="server" CssClass="form-control" TextMode="MultiLine"></asp:TextBox>
                            </div>
                        </div>
                        <div class="row form-group">
                            <label class="col-sm-4 control-label text-right">Major product line &amp; ratio of annual turn over</label>
                            <div class="col-sm-8">
                                <asp:TextBox ID="MajorProduct" runat="server" CssClass="form-control" TextMode="MultiLine"></asp:TextBox>
                            </div>
                        </div>

                        <asp:PlaceHolder ID="ph_SupplierLocation" runat="server">
                            <div class="row form-group">
                                <label class="col-sm-4 control-label text-right">Supplier location &amp; ratio</label>
                                <div class="col-sm-8">
                                    <asp:CheckBoxList ID="cb_SupplierLocation" runat="server" RepeatDirection="Vertical" RepeatLayout="Flow"></asp:CheckBoxList>
                                    <asp:TextBox ID="SupplierLocation_Other" runat="server" CssClass="form-control" MaxLength="50" placeholder="Other..."></asp:TextBox>
                                </div>
                            </div>
                        </asp:PlaceHolder>
                        <asp:PlaceHolder ID="ph_BusinessField" runat="server">
                            <div class="form-group">
                                <label class="col-sm-4 control-label text-right">Business Field &amp; ratio (percentage)</label>
                                <div class="col-sm-8">
                                    <div class="row">
                                        <div class="col-xs-9 col-sm-9">
                                            <label>Computer, Communication &amp; Consumer Industry</label>
                                        </div>
                                        <div class="col-xs-3 col-sm-3">
                                            <div class="form-group">
                                                <div class="input-group">
                                                    <asp:TextBox ID="BusniessField_Per1" runat="server" CssClass="form-control" MaxLength="8"></asp:TextBox>
                                                    <div class="input-group-addon">%</div>
                                                </div>
                                                <asp:RegularExpressionValidator ID="rev_BusniessField_Per1" runat="server"
                                                    ControlToValidate="BusniessField_Per1" Display="Dynamic" ValidationExpression="^[0-9]+\.{0,1}[0-9]{0,2}$" ValidationGroup="Add"><div class="alert alert-danger"><%=this.GetLocalResourceObject("tip_Format").ToString()%></div></asp:RegularExpressionValidator>
                                            </div>
                                        </div>
                                        <div class="add-box-border">
                                            <div style="padding-left: 10px;">
                                                <asp:CheckBoxList ID="cb_BusinessFieldA" runat="server" RepeatDirection="Vertical" RepeatLayout="Flow"></asp:CheckBoxList>

                                            </div>
                                        </div>
                                    </div>
                                    <div class="row" style="padding-top: 20px;">
                                        <div class="col-xs-9 col-sm-9">
                                            <label>Machinery &amp; Hardware</label>
                                        </div>
                                        <div class="col-sm-3 col-sm-3">
                                            <div class="form-group">
                                                <div class="input-group">
                                                    <asp:TextBox ID="BusniessField_Per2" runat="server" CssClass="form-control" MaxLength="8"></asp:TextBox>
                                                    <div class="input-group-addon">%</div>
                                                </div>
                                                <asp:RegularExpressionValidator ID="rev_BusniessField_Per2" runat="server"
                                                    ControlToValidate="BusniessField_Per2" Display="Dynamic" ValidationExpression="^[0-9]+\.{0,1}[0-9]{0,2}$" ValidationGroup="Add"><div class="alert alert-danger"><%=this.GetLocalResourceObject("tip_Format").ToString()%></div></asp:RegularExpressionValidator>
                                            </div>
                                        </div>
                                        <div class="add-box-border df-margin-top">
                                            <div style="padding-left: 10px;">
                                                <asp:CheckBoxList ID="cb_BusinessFieldB" runat="server" RepeatDirection="Vertical" RepeatLayout="Flow"></asp:CheckBoxList>

                                            </div>
                                        </div>
                                    </div>
                                    <div class="row" style="padding-top: 20px;">
                                        <div class="col-xs-9 col-sm-9">
                                            <label>Construction Engineering</label>
                                        </div>
                                        <div class="col-sm-3 col-sm-3">
                                            <div class="form-group">
                                                <div class="input-group">
                                                    <asp:TextBox ID="BusniessField_Per3" runat="server" CssClass="form-control" MaxLength="8"></asp:TextBox>
                                                    <div class="input-group-addon">%</div>
                                                </div>
                                                <asp:RegularExpressionValidator ID="rev_BusniessField_Per3" runat="server"
                                                    ControlToValidate="BusniessField_Per3" Display="Dynamic" ValidationExpression="^[0-9]+\.{0,1}[0-9]{0,2}$" ValidationGroup="Add"><div class="alert alert-danger"><%=this.GetLocalResourceObject("tip_Format").ToString()%></div></asp:RegularExpressionValidator>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </asp:PlaceHolder>
                    </asp:PlaceHolder>
                    <!-- eng field end -->

                    <!-- chinese field start -->
                    <asp:PlaceHolder ID="ph_FieldOfChinese" runat="server">
                        <asp:PlaceHolder ID="ph_CompType" runat="server">
                            <div class="row form-group">
                                <label class="col-sm-4 control-label text-right"><%=this.GetLocalResourceObject("txt_CompType").ToString()%></label>
                                <div class="col-sm-8">
                                    <asp:CheckBoxList ID="cb_CompType" runat="server" RepeatDirection="Horizontal" RepeatLayout="Table" RepeatColumns="2"></asp:CheckBoxList>
                                </div>
                            </div>
                        </asp:PlaceHolder>
                        <asp:PlaceHolder ID="ph_BusinessItem" runat="server">
                            <div class="row form-group">
                                <label class="col-sm-4 control-label text-right"><%=this.GetLocalResourceObject("txt_BusinessItem").ToString()%></label>
                                <div class="col-sm-8">
                                    <asp:CheckBoxList ID="cb_BusinessItem" runat="server" RepeatDirection="Horizontal" RepeatLayout="Table" RepeatColumns="2"></asp:CheckBoxList>
                                </div>
                            </div>
                        </asp:PlaceHolder>
                        <asp:PlaceHolder ID="ph_SalesTarget" runat="server">
                            <div class="row form-group">
                                <label class="col-sm-4 control-label text-right"><%=this.GetLocalResourceObject("txt_SalesTarget").ToString()%></label>
                                <div class="col-sm-8">
                                    <asp:CheckBoxList ID="cb_SalesTarget" runat="server" RepeatDirection="Horizontal" RepeatLayout="Table" RepeatColumns="2"></asp:CheckBoxList>
                                </div>
                            </div>
                        </asp:PlaceHolder>
                    </asp:PlaceHolder>
                    <!-- chinese field end -->

                    <div class="text-right">
                        <asp:ValidationSummary ID="ValidationSummary1" runat="server" ValidationGroup="Add" ShowMessageBox="true" HeaderText="Please check the form." />
                        <asp:Button ID="btn_Submit" runat="server" CssClass="btn btn-success" OnClick="btn_Submit_Click" OnClientClick="blockBox1('Add','Processing...')" ValidationGroup="Add" />
                    </div>
                </form>
            </div>
        </div>

    </div>
</asp:Content>
<asp:Content ID="myScript" ContentPlaceHolderID="ScriptContent" runat="Server">
    <%: Scripts.Render("~/bundles/blockUI-script") %>
</asp:Content>

