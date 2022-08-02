<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="myTagEvent_Default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="CssContent" runat="Server">
    <style>
        body {
            /*position: absolute;*/
            overflow-y: scroll;
            overflow-x: hidden;
        }

        ifHtml {
            overflow-y: auto;
            background-color: transparent;
        }

        ::-webkit-scrollbar {
            width: 10px;
            height: 10px;
            display: none;
        }

        ::-webkit-scrollbar-button:start:decrement,
        ::-webkit-scrollbar-button:end:increment {
            height: 30px;
            background-color: transparent;
        }

        ::-webkit-scrollbar-track-piece {
            background-color: #3b3b3b;
            -webkit-border-radius: 16px;
        }

        ::-webkit-scrollbar-thumb:vertical {
            height: 50px;
            background-color: #666;
            border: 1px solid #eee;
            -webkit-border-radius: 6px;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="Server">
    <div class="embed-responsive embed-responsive-4by3">
        <iframe id="ifHtml" src="<%:EventUrl %>" width="100%"></iframe>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ScriptContent" runat="Server">
</asp:Content>

