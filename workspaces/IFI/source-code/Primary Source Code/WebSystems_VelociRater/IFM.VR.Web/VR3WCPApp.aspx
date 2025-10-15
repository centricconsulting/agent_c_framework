<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="VR3WCPApp.aspx.vb" Inherits="IFM.VR.Web.VR3WCPApp" EnableEventValidation="false" MasterPageFile="~/VelociRater.Master" %>

<%@ Import Namespace="IFM.VR.Web" %>
<%@ Register Src="~/User Controls/ctlTreeView.ascx" TagPrefix="uc1" TagName="ctlTreeView" %>
<%@ Register Src="~/User Controls/VR Commercial/Application/WCP/ctl_WorkflowMgr_App_WCP.ascx" TagPrefix="uc1" TagName="ctl_WorkflowMgr_App_WCP" %>

<asp:Content ID="script" ContentPlaceHolderID="cphBodyScripts" runat="server">
    <%--<script src="<%=ResolveClientUrl("~/js/vrBOP.js?dt=" + DirectCast(Me.Master, VelociRater).ScriptDT)%>"></script>--%>
    <script src="<%=ResolveClientUrl("~/js/vrCAP.js?dt=" + DirectCast(Me.Master, VelociRater).ScriptDT)%>"></script>
    <script src="<%=ResolveClientUrl("~/js/VrAdditionalInterestSearch.js?dt=" + DirectCast(Me.Master, VelociRater).ScriptDT)%>"></script>
    <script src="<%=ResolveClientUrl("~/js/VrApp.js?dt=" + DirectCast(Me.Master, VelociRater).ScriptDT)%>"></script>
    <script src="<%=ResolveClientUrl("~/js/VrAllLines.js?dt=" + DirectCast(Me.Master, VelociRater).ScriptDT)%>"></script>
    <script src="<%=ResolveClientUrl("~/js/VrPersonal.js?dt=" + DirectCast(Me.Master, VelociRater).ScriptDT)%>"></script> <!-- Needed for zip code lookup MGB -->
</asp:Content>

<asp:Content ID="cntMain" ContentPlaceHolderID="cphMain" runat="server">
    <uc1:ctl_WorkflowMgr_App_WCP runat="server" id="ctl_WorkflowMgr_App_WCP" />
</asp:Content>

