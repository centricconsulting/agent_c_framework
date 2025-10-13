<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/VelociRater.Master" CodeBehind="VR3CglApp.aspx.vb" Inherits="IFM.VR.Web.VR3CglApp" %>


<%@ Import Namespace="IFM.VR.Web" %>
<%@ Register Src="~/User Controls/VR Commercial/Application/CGL/ctl_WorkflowManager_App_CGL.ascx" TagPrefix="uc1" TagName="ctl_WorkflowManager_App_CGL" %>


<asp:Content ID="Content1" ContentPlaceHolderID="cphBodyScripts" runat="server">
    <script src="<%=ResolveClientUrl("~/js/VrAdditionalInterestSearch.js?dt=" + DirectCast(Me.Master, VelociRater).ScriptDT)%>"></script>
    <script src="<%=ResolveClientUrl("~/js/VrApp.js?dt=" + DirectCast(Me.Master, VelociRater).ScriptDT)%>"></script>
    <script src="<%=ResolveClientUrl("~/js/VrAllLines.js?dt=" + DirectCast(Me.Master, VelociRater).ScriptDT)%>"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphMain" runat="server">
    <uc1:ctl_WorkflowManager_App_CGL runat="server" id="ctl_WorkflowManager_App_CGL" />
</asp:Content>
