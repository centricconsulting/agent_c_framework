<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="VR3CAPApp.aspx.vb" Inherits="IFM.VR.Web.VR3CAPApp" EnableEventValidation="false" MasterPageFile="~/VelociRater.Master" %>

<%@ Import Namespace="IFM.VR.Web" %>
<%@ Register Src="~/User Controls/ctlTreeView.ascx" TagPrefix="uc1" TagName="ctlTreeView" %>
<%@ Register Src="~/User Controls/VR Commercial/Application/CAP/ctl_WorkflowMgr_App_CAP.ascx" TagPrefix="uc1" TagName="ctl_WorkflowMgr_App_CAP" %>

<asp:Content ID="script" ContentPlaceHolderID="cphBodyScripts" runat="server">
    <%--<script src="<%=ResolveClientUrl("~/js/vrBOP.js?dt=" + DirectCast(Me.Master, VelociRater).ScriptDT)%>"></script>--%>
    <script src="<%=ResolveClientUrl("~/js/vrCAP.js?dt=" + DirectCast(Me.Master, VelociRater).ScriptDT)%>"></script>
    <script src="<%=ResolveClientUrl("~/js/VrAdditionalInterestSearch.js?dt=" + DirectCast(Me.Master, VelociRater).ScriptDT)%>"></script>
    <script src="<%=ResolveClientUrl("~/js/VrApp.js?dt=" + DirectCast(Me.Master, VelociRater).ScriptDT)%>"></script>
    <script src="<%=ResolveClientUrl("~/js/VrAllLines.js?dt=" + DirectCast(Me.Master, VelociRater).ScriptDT)%>"></script>
</asp:Content>

<asp:Content ID="cntMain" ContentPlaceHolderID="cphMain" runat="server">
    <uc1:ctl_WorkflowMgr_App_CAP runat="server" id="ctl_WorkflowMgr_App_CAP" />
</asp:Content>

