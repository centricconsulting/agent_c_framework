<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="VRHomeTEST_MGB.aspx.vb" MasterPageFile="~/VelociRater.Master" Inherits="IFM.VR.Web.VRHomeTEST_MGB" %>
<%--<%@ Register Src="~/User Controls/VR Commercial/Application/BOP/ctl_AppSection_BOP.ascx" TagPrefix="UC1" TagName="BOPAPP" %>--%>
<%@ Register Src="~/User Controls/VR Commercial/Quote/CPP/ctl_CPP_QuoteSummary.ascx" TagPrefix="uc1" TagName="CPPSummary" %>
<%--<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">--%>
<asp:Content ID="script" ContentPlaceHolderID="cphBodyScripts" runat="server">   
        <div>
            <uc1:CPPSummary runat="server" id="CPPQuoteSummary"></uc1:CPPSummary>
            <%--<UC1:BOPAPP runat="server" ID="ctlBOPApp" />--%>
        </div>
</asp:Content>
    <%--</form>
</body>
</html>--%>
