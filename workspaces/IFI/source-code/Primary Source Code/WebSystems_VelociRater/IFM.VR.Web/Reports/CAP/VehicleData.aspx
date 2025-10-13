<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="VehicleData.aspx.vb" Inherits="IFM.VR.Web.VehicleData" %>
<%@ Import Namespace="IFM.VR.Web" %>
<%@ Register Src="~/Reports/CAP/ctl_PF_VehicleData.ascx" TagPrefix="uc1" TagName="ctl_PF_VehicleData" %>

<%--<asp:Content ID="cntMain" ContentPlaceHolderID="cphMain" runat="server">
    <uc1:ctl_PF_VehicleData runat="server" id="ctl_PF_VehicleData" />
</asp:Content>--%>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">  
<head id="Head1" runat="server">
    <link href="~/styles/vr.css" rel="stylesheet" type="text/css" />
    <title>Vehicle Data Report</title>
</head>
<body>
    <uc1:ctl_PF_VehicleData runat="server" id="ctl_PF_VehicleData1" />
</body>
</html>
