<%@ Page Title="" Language="vb" EnableEventValidation="false" AutoEventWireup="false" MasterPageFile="~/VelociRater.Master" CodeBehind="VR3Bop.aspx.vb" Inherits="IFM.VR.Web.VR3Bop" %>


<%@ Import Namespace="IFM.VR.Web" %>
<%@ Register Src="~/User Controls/VR Commercial/Quote/BOP/ctl_WorkflowManager_BOP_Quote.ascx" TagPrefix="uc1" TagName="ctl_WorkflowManager_BOP_Quote" %>




<asp:Content ID="script" ContentPlaceHolderID="cphBodyScripts" runat="server">
    <style>
          .ctl_BOP_Location_Coverages_DetailLabel {
        width:50%;
        text-align:left;
    }
    .ctl_BOP_Location_Coverages_DataLabel {
        width:50%;
        text-align:left;
    }
    </style>

    <script src="<%=ResolveClientUrl("~/js/VrMiniClientSearch.js?dt=" + DirectCast(Me.Master, VelociRater).ScriptDT)%>"></script>    
    <script src="<%=ResolveClientUrl("~/js/VrAllLines.js?dt=" + DirectCast(Me.Master, VelociRater).ScriptDT)%>"></script>
    <script src="<%=ResolveClientUrl("~/js/VrApplicantSearch.js?dt=" + DirectCast(Me.Master, VelociRater).ScriptDT)%>"></script>    
    <script src="<%=ResolveClientUrl("~/js/VrRiskGrade.js?dt=" + DirectCast(Me.Master, VelociRater).ScriptDT)%>"></script>
<%--    <script src="<%=ResolveClientUrl("~/js/BOPBuilding.js?dt=" + DirectCast(Me.Master, VelociRater).ScriptDT)%>"></script>--%>
    <script src="<%=ResolveClientUrl("~/js/vrBOP.js?dt=" + DirectCast(Me.Master, VelociRater).ScriptDT)%>"></script>
    <script src="<%=ResolveClientUrl("~/js/VrProtectionClassLookup.js?dt=" + DirectCast(Me.Master, VelociRater).ScriptDT)%>"></script>
    <script src="<%=ResolveClientUrl("~/js/VrApp.js?dt=" + DirectCast(Me.Master, VelociRater).ScriptDT)%>"></script>
    <script src="<%=ResolveClientUrl("~/js/VrAdditionalInterestSearch.js?dt=" + DirectCast(Me.Master, VelociRater).ScriptDT)%>"></script>
    <script>
        var List = ['divInsuredInfo', 'MultiStateSection', 'divBOPGeneralInformation', 'divBOPPolicyLevelCoverages', 'divAdditionalInterests', 'divContents'];
        ifm.vr.ui.DisableContent(List);
    </script>
</asp:Content>
<asp:Content ID="cntMain" ContentPlaceHolderID="cphMain" runat="server">
    <uc1:ctl_WorkflowManager_BOP_Quote runat="server" id="ctl_WorkflowManager_BOP_Quote" />
</asp:Content>
