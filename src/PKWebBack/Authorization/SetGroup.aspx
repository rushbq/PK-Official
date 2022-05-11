<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="SetGroup.aspx.cs" Inherits="Authorization_SetGroup" %>

<asp:Content ID="myCss" ContentPlaceHolderID="CssContent" runat="Server">
    <%: Styles.Render("~/bundles/zTree-css") %>
    <%: Styles.Render("~/bundles/JQ-UI-css") %>
</asp:Content>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="Server">
    <!-- Page Header Start -->
    <div class="row">
        <div class="col-md-12">
            <div class="page-header">
                <h1>權限設定 - 群組</h1>
            </div>
            <ol class="breadcrumb">
                <li><a href="<%=Application["WebUrl"] %>">首頁</a></li>
                <li><a>帳號 & 權限</a></li>
                <li class="active">群組權限</li>
            </ol>
        </div>
    </div>
    <!-- Page Header End -->
    <div class="row">
        <div class="col-sm-9 col-md-10">
            <div class="panel panel-info">
                <div class="panel-heading">
                    <div class="pull-left">
                        <span class="glyphicon glyphicon-edit"></span>
                        <span>資料維護</span>
                    </div>
                    <div class="pull-right">
                        <a data-toggle="collapse" href="#data">
                            <span class="glyphicon glyphicon-sort"></span>
                        </a>
                    </div>
                    <div class="clearfix"></div>
                </div>
                <div id="data" class="collapse in">
                    <div class="panel-body">
                        <!-- Content Start -->
                        <div class="row">
                            <div class="col-md-12">
                                <asp:TextBox ID="tb_Group" runat="server" CssClass="form-control" placeholder="輸入群組關鍵字"></asp:TextBox>
                                <span class="help-block">(輸入關鍵字, 選擇項目後會自動帶出資料)</span>

                                <asp:Literal ID="lt_GroupName" runat="server"></asp:Literal>
                            </div>
                            <div class="hidden-field">
                                <asp:TextBox ID="tb_EmpValue" runat="server" Style="display: none;" ToolTip="群組Guid, Autocomplete暫存欄"></asp:TextBox>
                                <asp:Literal ID="lt_Guid" runat="server" Visible="false"></asp:Literal>
                                <asp:Button ID="btn_Search" runat="server" Text="Search" OnClick="btn_Search_Click" Style="display: none;" />
                                <asp:HiddenField ID="hf_flag" runat="server" Value="Add" />
                            </div>
                        </div>
                        <hr />
                        <asp:PlaceHolder ID="ph_data" runat="server" Visible="false">
                            <div class="row">
                                <div class="col-md-12">
                                    <div>
                                        <input type="button" id="showAll" class="btn btn-info" value="展開" />
                                        <input type="button" id="hideAll" class="btn btn-warning" value="折疊" />
                                    </div>
                                    <ul id="myTree" class="ztree">
                                    </ul>

                                    <asp:TextBox ID="tb_IDvalues" runat="server" Style="display: none"></asp:TextBox>
                                </div>
                            </div>
                        </asp:PlaceHolder>
                        <!-- Content End -->
                    </div>
                </div>
            </div>
        </div>
        <!-- buttons -->
        <div class="col-sm-3 col-md-2">
            <asp:PlaceHolder ID="ph_btns" runat="server" Visible="false">
                <div class="FormBtn-fixPos">
                    <div class="metro-nav metro-fix-view">
                        <div class="metro-nav-block nav-block-blue">
                            <asp:LinkButton ID="lbtn_Save" runat="server" CssClass="text-center" ValidationGroup="Add" OnClientClick="getCbValue('myTree', 'MainContent_tb_IDvalues');blockBox1('Add','資料處理中...')" OnClick="lbtn_Save_Click">
                            <i class="glyphicon glyphicon-floppy-save"></i>
                            <div class="status">設定權限</div>
                            </asp:LinkButton>
                        </div>
                        <div class="metro-nav-block nav-block-red">
                            <asp:LinkButton ID="lbtn_Remove" runat="server" CssClass="text-center" CausesValidation="false" OnClientClick="return confirm('是否確定移除權限?')" OnClick="lbtn_Remove_Click">
                            <i class="glyphicon glyphicon-floppy-remove"></i>
                            <div class="status">移除權限</div>
                            </asp:LinkButton>
                        </div>
                    </div>
                </div>
            </asp:PlaceHolder>
        </div>
    </div>
</asp:Content>
<asp:Content ID="myScript" ContentPlaceHolderID="ScriptContent" runat="Server">
    <%: Scripts.Render("~/bundles/JQ-UI-script") %>
    <%: Scripts.Render("~/bundles/blockUI-script") %>
    <%: Scripts.Render("~/bundles/zTree-script") %>
    <%-- zTree Start --%>
    <script>
        //zTree 設定
        var setting = {
            view: {
                dblClickExpand: false
            },
            callback: {
                onClick: MMonClick
            },
            check: {
                enable: true
            },
            data: {
                simpleData: {
                    enable: true
                }
            }
        };

        //Event - onClick
        function MMonClick(e, treeId, treeNode) {
            var zTree = $.fn.zTree.getZTreeObj("myTree");
            zTree.expandNode(treeNode);
        }

        //宣告節點
        var zNodes;

        //取得資料
        function getAuthList() {
            $.ajax({
                async: false,
                cache: false,
                type: 'POST',
                dataType: "json",
                url: "<%=Application["WebUrl"]%>Ajax_Data/Json_GetAuthList.aspx",
                data: {
                    DataType: 'Group',
                    Guid: '<%=Param_Guid %>'
                },
                error: function () {
                    alert('樹狀選單載入失敗!');
                },
                success: function (data) {
                    zNodes = data;
                }
            });
            //載入zTree
            $.fn.zTree.init($("#myTree"), setting, zNodes);
        }

        // 所有節點的收合(true = 展開, false = 折疊)
        function expandAll(objbool) {
            var treeObj = $.fn.zTree.getZTreeObj("myTree");
            treeObj.expandAll(objbool);
        }

        /* 取值(zTree名稱, 要放值的欄位名) */
        function getCbValue(eleName, valName) {

            var treeObj = $.fn.zTree.getZTreeObj(eleName);
            var nodes = treeObj.getCheckedNodes(true);
            var ids = "";
            for (var i = 0; i < nodes.length; i++) {
                //加入分隔符號("||")
                if (ids != "") {
                    ids += "||"
                }
                //取得id值
                ids += nodes[i].id;
            }
            //輸出組合完畢的字串值
            document.getElementById(valName).value = ids;
            return true;
        }

        //Load
        $(document).ready(function () {
            <%if (!string.IsNullOrEmpty(Param_Guid))
              {%>
            getAuthList();
            <%}%>

            //顯示所有節點
            $('#showAll').click(function () {
                expandAll(true);
            });

            //隱藏所有節點
            $('#hideAll').click(function () {
                expandAll(false);
            });
        });

    </script>
    <%-- zTree End --%>
    <script>
        $(function () {
            /* Autocomplete - Tags */
            $("#MainContent_tb_Group").autocomplete({
                minLength: 1,  //至少要輸入 n 個字元
                source: function (request, response) {
                    $.ajax({
                        url: "<%=Application["WebUrl"]%>Ajax_Data/AC_ADGroups.aspx",
                        data: {
                            q: request.term
                        },
                        type: "POST",
                        dataType: "json",
                        success: function (data) {
                            if (data != null) {
                                response($.map(data, function (item) {
                                    return {
                                        label: item.label,
                                        value: item.label,
                                        id: item.id
                                    }
                                }));
                            }
                        }
                    });
                },
                select: function (event, ui) {
                    //$("#tb_TagName").val(ui.item.value);
                    $("#MainContent_tb_EmpValue").val(ui.item.id);

                    //觸發事件, Click Search
                    blockBox2_NoMsg();
                    $('#MainContent_btn_Search').trigger("click");
                }
            });

        });
    </script>
</asp:Content>

