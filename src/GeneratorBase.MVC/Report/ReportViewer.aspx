<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ReportViewer.aspx.cs" Inherits="GeneratorBase.MVC.Report.ReportViewer" EnableViewState="true" %>
<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=9.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a" Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <rsweb:ReportViewer ID="MyReportViewer" ShowParameterPrompts="false" ShowFindControls="false"
            ShowPageNavigationControls="false" ShowPrintButton="false" ShowZoomControl="false"
            ShowRefreshButton="false" Width="100%" Height="100%" AsyncRendering="false" runat="server" ProcessingMode="Remote">
        </rsweb:ReportViewer>
    </form>
</body>
</html>
