<%@ Page Language="vb" EnableEventValidation="false" AutoEventWireup="false" MasterPageFile="~/VelociRater.Master" CodeBehind="VR3Auto.aspx.vb" Inherits="IFM.VR.Web.VR3Auto" MaintainScrollPositionOnPostback="true" %>

<%@ Import Namespace="IFM.VR.Web" %>

<%@ Register Src="~/User Controls/QuoteEdit/PPA/ctl_Master_Edit.ascx" TagPrefix="uc1" TagName="ctl_Master_Edit" %>
<%@ Register Src="~/User Controls/ctlTreeView.ascx" TagPrefix="uc1" TagName="ctlTreeView" %>

<asp:Content ID="script" ContentPlaceHolderID="cphBodyScripts" runat="server">
    <script src="<%=ResolveClientUrl("~/js/VrAdditionalInterestSearch.js?dt=" + DirectCast(Me.Master, VelociRater).ScriptDT)%>"></script>
    <script src="<%=ResolveClientUrl("~/js/VrMiniClientSearch.js?dt=" + DirectCast(Me.Master, VelociRater).ScriptDT)%>"></script>
    <script src="<%=ResolveClientUrl("~/js/VrAutoLine.js?dt=" + DirectCast(Me.Master, VelociRater).ScriptDT)%>"></script>
    <script src="<%=ResolveClientUrl("~/js/VrApp.js?dt=" + DirectCast(Me.Master, VelociRater).ScriptDT)%>"></script>
    <script>
        var List = ['divInsuredInfo', 'MultiStateSection', 'divDriver', 'divVehicles', 'divPolicyCoverage', 'dvCoverageVehicleList'];
        ifm.vr.ui.DisableContent(List);
    </script>
</asp:Content>

<asp:Content ID="cntMain" ContentPlaceHolderID="cphMain" runat="server">

    <table style="width: 100%;">
        <tr>
            <td style="width: 250px; vertical-align: top;">
                <uc1:ctlTreeView runat="server" ID="ctlTreeView" />
            </td>
            <td style="vertical-align: top;">
                <div>
                    <div id="divQuoteEditControls">
                        <uc1:ctl_Master_Edit runat="server" ID="ctl_Master_Edit" />
                    </div>
                </div>
            </td>
        </tr>
    </table>
</asp:Content>