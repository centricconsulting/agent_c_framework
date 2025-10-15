<%@ Page Title="" Language="vb" EnableEventValidation="false" AutoEventWireup="false" MasterPageFile="~/VelociRater.Master" CodeBehind="VR3DwellingFire.aspx.vb" Inherits="IFM.VR.Web.VR3DwellingFire" %>
<%@ Import Namespace="IFM.VR.Web" %>
<%@ Register Src="~/User Controls/QuoteEdit/DFR/ctl_WorkFlowManager_DFR_Quote.ascx" TagPrefix="uc1" TagName="ctl_WorkFlowManager_DFR_Quote" %>

<asp:Content ID="script" ContentPlaceHolderID="cphBodyScripts" runat="server">
    <script src="<%=ResolveClientUrl("~/js/VrAdditionalInterestSearch.js?dt=" + DirectCast(Me.Master, VelociRater).ScriptDT)%>"></script>
    <script src="<%=ResolveClientUrl("~/js/VrApp.js?dt=" + DirectCast(Me.Master, VelociRater).ScriptDT)%>"></script>
    <script src="<%=ResolveClientUrl("~/js/VrMiniClientSearch.js?dt=" + DirectCast(Me.Master, VelociRater).ScriptDT)%>"></script>
    <script src="<%=ResolveClientUrl("~/js/VrAllLines.js?dt=" + DirectCast(Me.Master, VelociRater).ScriptDT)%>"></script>
    <script src="<%=ResolveClientUrl("~/js/VrDwellingFireLine.js?dt=" + DirectCast(Me.Master, VelociRater).ScriptDT)%>"></script>
    <script src="<%=ResolveClientUrl("~/js/VrProtectionClassLookup.js?dt=" + DirectCast(Me.Master, VelociRater).ScriptDT)%>"></script>
    <script>
        var List = ['divInsuredInfo', 'PropertyAddressContent', 'ResidenceContentDiv', 'ProtectionClassContentDiv', 'ProtectionClassContentDivB', 'ProtectionClassContentDivC', 'divAiEntry', 'divLossHistoriesContent', 'dvDeductible', 'dvBaseCovContainer', 'dvCoverageContainer'];
        ifm.vr.ui.DisableContent(List);
    </script>
</asp:Content>
<asp:Content ID="cntMain" ContentPlaceHolderID="cphMain" runat="server">
    <uc1:ctl_WorkFlowManager_DFR_Quote runat="server" id="ctl_WorkFlowManager_DFR_Quote" />
</asp:Content>
