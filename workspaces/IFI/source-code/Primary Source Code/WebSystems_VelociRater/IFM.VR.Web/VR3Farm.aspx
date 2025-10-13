<%@ Page Language="vb" AutoEventWireup="false" EnableEventValidation="false" CodeBehind="VR3Farm.aspx.vb" MasterPageFile="~/VelociRater.Master" Inherits="IFM.VR.Web.VR3Farm" %>

<%@ Import Namespace="IFM.VR.Web" %>

<%@ Register Src="~/User Controls/QuoteEdit/FAR/ctl_WorkflowManager_Quote_Farm.ascx" TagPrefix="uc1" TagName="ctl_WorkflowManager_Quote_Farm" %>

<asp:Content ID="script" ContentPlaceHolderID="cphBodyScripts" runat="server">
    <script src="<%=ResolveClientUrl("~/js/VrAdditionalInterestSearch.js?dt=" + DirectCast(Me.Master, VelociRater).ScriptDT)%>"></script>
    <script src="<%=ResolveClientUrl("~/js/VrMiniClientSearch.js?dt=" + DirectCast(Me.Master, VelociRater).ScriptDT)%>"></script>
    <script src="<%=ResolveClientUrl("~/js/VrFarmLine.js?dt=" + DirectCast(Me.Master, VelociRater).ScriptDT)%>"></script>
    <script src="<%=ResolveClientUrl("~/js/VrAllLines.js?dt=" + DirectCast(Me.Master, VelociRater).ScriptDT)%>"></script>
    <script src="<%=ResolveClientUrl("~/js/VrApplicantSearch.js?dt=" + DirectCast(Me.Master, VelociRater).ScriptDT)%>"></script>
     <script src="<%=ResolveClientUrl("~/js/vrBOP.js?dt=" + DirectCast(Me.Master, VelociRater).ScriptDT)%>"></script>
    <script src="<%=ResolveClientUrl("~/js/VrApp.js?dt=" + DirectCast(Me.Master, VelociRater).ScriptDT)%>"></script>
    <script>
        var List = ['divInsuredInfo', 'MultiStateSection', 'dvFarmPolicyCoverages', 'dvFarmLocations', 'dvInlandMarineInput','dvFarmPersProp'];
        ifm.vr.ui.DisableContent(List);
    </script>
</asp:Content>

<asp:Content ID="cntMain" ContentPlaceHolderID="cphMain" runat="server">
    <uc1:ctl_WorkflowManager_Quote_Farm runat="server" id="ctl_WorkflowManager_Quote_Farm" />
</asp:Content>