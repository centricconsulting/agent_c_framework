<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="VR3CAPQuote.aspx.vb" Inherits="IFM.VR.Web.VR3CAPQuote" EnableEventValidation="false" MasterPageFile="~/VelociRater.Master" %>

<%@ Import Namespace="IFM.VR.Web" %>
<%@ Register Src="~/User Controls/VR Commercial/Quote/CAP/ctl_WorkflowMgr_Quote_CAP.ascx" TagPrefix="uc1" TagName="ctl_WorkflowManager_Quote_CAP" %>

<asp:Content ID="script" ContentPlaceHolderID="cphBodyScripts" runat="server">
    <style>
          .ctl_CAP_Location_Coverages_DetailLabel {
        width:50%;
        text-align:left;
    }
    .ctl_CAP_Location_Coverages_DataLabel {
        width:50%;
        text-align:left;
    }
    </style>

    <script src="<%=ResolveClientUrl("~/js/VrMiniClientSearch.js?dt=" + DirectCast(Me.Master, VelociRater).ScriptDT)%>"></script>    
    <script src="<%=ResolveClientUrl("~/js/VrAllLines.js?dt=" + DirectCast(Me.Master, VelociRater).ScriptDT)%>"></script>
    <script src="<%=ResolveClientUrl("~/js/VrApplicantSearch.js?dt=" + DirectCast(Me.Master, VelociRater).ScriptDT)%>"></script>    
    <script src="<%=ResolveClientUrl("~/js/VrRiskGrade.js?dt=" + DirectCast(Me.Master, VelociRater).ScriptDT)%>"></script>
    <script src="<%=ResolveClientUrl("~/js/vrCAP.js?dt=" + DirectCast(Me.Master, VelociRater).ScriptDT)%>"></script>
    <script src="<%=ResolveClientUrl("~/js/vrBOP.js?dt=" + DirectCast(Me.Master, VelociRater).ScriptDT)%>"></script>
    <script src="<%=ResolveClientUrl("~/js/VrProtectionClassLookup.js?dt=" + DirectCast(Me.Master, VelociRater).ScriptDT)%>"></script>
    <script src="<%=ResolveClientUrl("~/js/VrApp.js?dt=" + DirectCast(Me.Master, VelociRater).ScriptDT)%>"></script>
    <script src="<%=ResolveClientUrl("~/js/VrAdditionalInterestSearch.js?dt=" + DirectCast(Me.Master, VelociRater).ScriptDT)%>"></script>
    <script>
        var List = ['divInsuredInfo', 'MultiStateSection', 'PropertyAddressContent', 'divPolicyLevelCoverages', 'divMainList', 'divVehicleMainTable', 'divClassCodeLookup', 'divGaragingAddress', 'divVehicleCoverages', 'divAdditionalInterests', 'divAiEntry', 'divCAPDriver', 'divViolationList'];
        ifm.vr.ui.DisableContent(List);
    </script>
</asp:Content>
<asp:Content ID="cntMain" ContentPlaceHolderID="cphMain" runat="server">
    <uc1:ctl_WorkflowManager_Quote_CAP runat="server" id="ctl_WorkflowManager_Quote_CAP" />
</asp:Content>
