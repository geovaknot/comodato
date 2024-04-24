﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="RelatorioAcompanhamentos.aspx.cs" Inherits="_3M.Comodato.Front.RelatorioAcompanhamentos" %>

<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=14.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91" Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <meta http-equiv="X-UA-Compatible" content="IE=10" />
</head>
<body>
    <link href="Content/bootstrap.css" rel="stylesheet" />
    <link href="Content/site.css" rel="stylesheet" />
    <link href="Content/fontawesome-all.css" rel="stylesheet" />
    
    <form id="form1" runat="server">
        <div>
            <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
            <rsweb:ReportViewer ID="ReportViewer1" runat="server" SizeToReportContent="True" BackColor="White" BorderWidth="0px" InteractivityPostBackMode="AlwaysSynchronous" ShowRefreshButton="False" Width="801px" />
        </div>

        <asp:Panel ID="pnlMensagem" runat="server" Visible="false">
            <hgroup>
                <i class="fas fa-exclamation-triangle fa-7x fa-pull-left"></i>
                <h3>Relatório Acompanhamentos</h3>
                <h4>O Relatório solicitado não possui dados a serem exibidos.</h4>
            </hgroup>
        </asp:Panel>
    </form>
</body>
</html>
