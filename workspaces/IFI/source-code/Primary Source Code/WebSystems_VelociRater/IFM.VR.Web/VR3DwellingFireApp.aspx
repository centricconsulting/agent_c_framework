<%@ Page Title="" Language="vb" EnableEventValidation="false" AutoEventWireup="false" MasterPageFile="~/VelociRater.Master" CodeBehind="VR3DwellingFireApp.aspx.vb" Inherits="IFM.VR.Web.VR3DwellingFireApp" %>
<%@ Import Namespace="IFM.VR.Web" %>
<%@ Register Src="~/User Controls/Application/DFR/ctl_WorkFlowManager_DFR_App.ascx" TagPrefix="uc1" TagName="ctl_WorkFlowManager_DFR_App" %>


<asp:Content ID="script" ContentPlaceHolderID="cphBodyScripts" runat="server">
    <script src="<%=ResolveClientUrl("~/js/VrAdditionalInterestSearch.js?dt=" + DirectCast(Me.Master, VelociRater).ScriptDT)%>"></script>
    <script src="<%=ResolveClientUrl("~/js/VrApp.js?dt=" + DirectCast(Me.Master, VelociRater).ScriptDT)%>"></script>    
    <script src="<%=ResolveClientUrl("~/js/VrAllLines.js?dt=" + DirectCast(Me.Master, VelociRater).ScriptDT)%>"></script>
    <script src="<%=ResolveClientUrl("~/js/VrDwellingFireLine.js?dt=" + DirectCast(Me.Master, VelociRater).ScriptDT)%>"></script>
</asp:Content>
<asp:Content ID="cntMain" ContentPlaceHolderID="cphMain" runat="server">
    <uc1:ctl_WorkFlowManager_DFR_App runat="server" id="ctl_WorkFlowManager_DFR_App" />
</asp:Content>



