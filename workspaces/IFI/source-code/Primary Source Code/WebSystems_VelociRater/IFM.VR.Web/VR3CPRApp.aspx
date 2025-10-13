<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="VR3CPRApp.aspx.vb" Inherits="IFM.VR.Web.VR3CPRApp" EnableEventValidation="false" MasterPageFile="~/VelociRater.Master" %>

<%@ Import Namespace="IFM.VR.Web" %>
<%@ Register Src="~/User Controls/ctlTreeView.ascx" TagPrefix="uc1" TagName="ctlTreeView" %>
<%@ Register Src="~/User Controls/VR Commercial/Application/CPR/ctl_WorkflowManager_App_CPR.ascx" TagPrefix="uc1" TagName="ctl_WorkflowManager_App_CPR" %>

<asp:Content ID="script" ContentPlaceHolderID="cphBodyScripts" runat="server">
    <%--<script src="<%=ResolveClientUrl("~/js/vrBOP.js?dt=" + DirectCast(Me.Master, VelociRater).ScriptDT)%>"></script>--%>
    <script src="<%=ResolveClientUrl("~/js/vrCPR.js?dt=" + DirectCast(Me.Master, VelociRater).ScriptDT)%>"></script>
    <script src="<%=ResolveClientUrl("~/js/vrCPP.js?dt=" + DirectCast(Me.Master, VelociRater).ScriptDT)%>"></script>
    <script src="<%=ResolveClientUrl("~/js/VrAdditionalInterestSearch.js?dt=" + DirectCast(Me.Master, VelociRater).ScriptDT)%>"></script>
    <script src="<%=ResolveClientUrl("~/js/VrApp.js?dt=" + DirectCast(Me.Master, VelociRater).ScriptDT)%>"></script>
    <script src="<%=ResolveClientUrl("~/js/VrAllLines.js?dt=" + DirectCast(Me.Master, VelociRater).ScriptDT)%>"></script>
    <script src="<%=ResolveClientUrl("~/js/VrPersonal.js?dt=" + DirectCast(Me.Master, VelociRater).ScriptDT)%>"></script> <!-- Needed for zip code lookup MGB -->
</asp:Content>

<asp:Content ID="cntMain" ContentPlaceHolderID="cphMain" runat="server">
    <uc1:ctl_WorkflowManager_App_CPR runat="server" id="ctl_WorkflowMgr_App_CPR" />
</asp:Content>

