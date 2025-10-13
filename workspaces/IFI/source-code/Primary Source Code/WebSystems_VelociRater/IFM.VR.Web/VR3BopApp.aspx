<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/VelociRater.Master" EnableEventValidation="false" CodeBehind="VR3BopApp.aspx.vb" Inherits="IFM.VR.Web.VR3BopApp" %>
<%@ Import Namespace="IFM.VR.Web" %>
<%@ Register Src="~/User Controls/ctlTreeView.ascx" TagPrefix="uc1" TagName="ctlTreeView" %>
<%@ Register Src="~/User Controls/VR Commercial/Application/BOP/ctl_WorkflowManager_App_BOP.ascx" TagPrefix="uc1" TagName="ctl_WorkflowManager_App_BOP" %>

<asp:Content ID="script" ContentPlaceHolderID="cphBodyScripts" runat="server">
    <script src="<%=ResolveClientUrl("~/js/vrBOP.js?dt=" + DirectCast(Me.Master, VelociRater).ScriptDT)%>"></script>
    <script src="<%=ResolveClientUrl("~/js/VrAdditionalInterestSearch.js?dt=" + DirectCast(Me.Master, VelociRater).ScriptDT)%>"></script>
    <script src="<%=ResolveClientUrl("~/js/VrApp.js?dt=" + DirectCast(Me.Master, VelociRater).ScriptDT)%>"></script>
    <script src="<%=ResolveClientUrl("~/js/VrAllLines.js?dt=" + DirectCast(Me.Master, VelociRater).ScriptDT)%>"></script>
</asp:Content>

<asp:Content ID="cntMain" ContentPlaceHolderID="cphMain" runat="server">
    <uc1:ctl_WorkflowManager_App_BOP runat="server" id="ctl_WorkflowManager_App_BOP" />
</asp:Content>

