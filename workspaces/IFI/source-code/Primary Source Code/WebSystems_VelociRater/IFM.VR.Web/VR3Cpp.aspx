<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="VR3CPP.aspx.vb" Inherits="IFM.VR.Web.VR3CPPQuote" EnableEventValidation="false" MasterPageFile="~/VelociRater.Master" %>

<%@ Import Namespace="IFM.VR.Web" %>
<%@ Register Src="~/User Controls/VR Commercial/Quote/CPP/ctl_WorkflowManager_Quote_CPP.ascx" TagPrefix="uc1" TagName="ctl_WorkflowManager_Quote_CPP" %>


<asp:Content ID="script" ContentPlaceHolderID="cphBodyScripts" runat="server">
    <script src="<%=ResolveClientUrl("~/js/VrMiniClientSearch.js?dt=" + DirectCast(Me.Master, VelociRater).ScriptDT)%>"></script>    
    <script src="<%=ResolveClientUrl("~/js/VrAllLines.js?dt=" + DirectCast(Me.Master, VelociRater).ScriptDT)%>"></script>
    <script src="<%=ResolveClientUrl("~/js/VrApplicantSearch.js?dt=" + DirectCast(Me.Master, VelociRater).ScriptDT)%>"></script>    
    <script src="<%=ResolveClientUrl("~/js/VrRiskGrade.js?dt=" + DirectCast(Me.Master, VelociRater).ScriptDT)%>"></script>
    <script src="<%=ResolveClientUrl("~/js/vrCPR.js?dt=" + DirectCast(Me.Master, VelociRater).ScriptDT)%>"></script>
    <script src="<%=ResolveClientUrl("~/js/vrCPP.js?dt=" + DirectCast(Me.Master, VelociRater).ScriptDT)%>"></script>
    <script src="<%=ResolveClientUrl("~/js/vrCGL.js?dt=" + DirectCast(Me.Master, VelociRater).ScriptDT)%>"></script>
    <script src="<%=ResolveClientUrl("~/js/vrBOP.js?dt=" + DirectCast(Me.Master, VelociRater).ScriptDT)%>"></script>
    <script src="<%=ResolveClientUrl("~/js/VrGlClassCodes.js?dt=" + DirectCast(Me.Master, VelociRater).ScriptDT)%>"></script>
    <script src="<%=ResolveClientUrl("~/js/VrClassCodes.js?dt=" + DirectCast(Me.Master, VelociRater).ScriptDT)%>"></script>
    <script src="<%=ResolveClientUrl("~/js/VrProtectionClassLookup.js?dt=" + DirectCast(Me.Master, VelociRater).ScriptDT)%>"></script>
    <script src="<%=ResolveClientUrl("~/js/VrApp.js?dt=" + DirectCast(Me.Master, VelociRater).ScriptDT)%>"></script>
    <script src="<%=ResolveClientUrl("~/js/VrAdditionalInterestSearch.js?dt=" + DirectCast(Me.Master, VelociRater).ScriptDT)%>"></script>
    <script>
        var List = ['divInsuredInfo', 'MultiStateSection', 'divGenInfo', 'CrimeDiv', 'divLocationList', 'divAdditionalInterests', 'divContents', 'divGenInfo', 'divDeductibles', 'divCoverages', 'InlandMarineDiv', 'divIMAssignedAdditionalInterests'];
        ifm.vr.ui.DisableContent(List);
    </script>
</asp:Content>

<asp:Content ID="cntMain" ContentPlaceHolderID="cphMain" runat="server">
    <uc1:ctl_WorkflowManager_Quote_CPP runat="server" id="ctl_WorkflowManager_Quote_CPP" />
</asp:Content>