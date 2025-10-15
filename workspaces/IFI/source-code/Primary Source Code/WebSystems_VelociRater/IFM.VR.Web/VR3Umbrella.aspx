<%@ Page Language="vb" AutoEventWireup="false" EnableEventValidation="false" CodeBehind="VR3Umbrella.aspx.vb" MasterPageFile="~/VelociRater.Master" Inherits="IFM.VR.Web.VR3Umbrella" %>

<%@ Import Namespace="IFM.VR.Web" %>

<%@ Register Src="~/User Controls/QuoteEdit/FUPPUP/ctl_WorkflowManager_Quote_fuppup.ascx" TagPrefix="uc1" TagName="ctl_WorkflowManager_Quote_fuppup" %>

<asp:Content ID="script" ContentPlaceHolderID="cphBodyScripts" runat="server">
    <script src="<%=ResolveClientUrl("~/js/VrMiniClientSearch.js?dt=" + DirectCast(Me.Master, VelociRater).ScriptDT)%>"></script>
    <script src="<%=ResolveClientUrl("~/js/VrFarmLine.js?dt=" + DirectCast(Me.Master, VelociRater).ScriptDT)%>"></script>
    <script src="<%=ResolveClientUrl("~/js/VrAllLines.js?dt=" + DirectCast(Me.Master, VelociRater).ScriptDT)%>"></script>
    <script src="<%=ResolveClientUrl("~/js/VrApplicantSearch.js?dt=" + DirectCast(Me.Master, VelociRater).ScriptDT)%>"></script>
     <script src="<%=ResolveClientUrl("~/js/vrBOP.js?dt=" + DirectCast(Me.Master, VelociRater).ScriptDT)%>"></script>
</asp:Content>

<asp:Content ID="cntMain" ContentPlaceHolderID="cphMain" runat="server">
    <uc1:ctl_WorkflowManager_Quote_fuppup runat="server" id="ctl_WorkflowManager_Quote_fuppup" />
</asp:Content>