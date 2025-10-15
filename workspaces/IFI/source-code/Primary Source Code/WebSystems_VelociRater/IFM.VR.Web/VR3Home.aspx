<%@ Page Language="vb" EnableEventValidation="false" AutoEventWireup="false" MasterPageFile="~/VelociRater.Master" CodeBehind="VR3Home.aspx.vb" Inherits="IFM.VR.Web.VR3Home" MaintainScrollPositionOnPostback="true" %>

<%@ Import Namespace="IFM.VR.Web" %>
<%@ Register Src="~/User Controls/ctlTreeView.ascx" TagPrefix="uc1" TagName="ctlTreeView" %>
<%@ Register Src="~/User Controls/QuoteEdit/HOM/ctlHomeQuote.ascx" TagPrefix="uc1" TagName="ctlHome" %>

<asp:Content ID="script" ContentPlaceHolderID="cphBodyScripts" runat="server">
      <script src="<%=ResolveClientUrl("~/js/VrAdditionalInterestSearch.js?dt=" + DirectCast(Me.Master, VelociRater).ScriptDT)%>"></script>
    <script src="<%=ResolveClientUrl("~/js/VrApp.js?dt=" + DirectCast(Me.Master, VelociRater).ScriptDT)%>"></script>
    <script src="<%=ResolveClientUrl("~/js/VrMiniClientSearch.js?dt=" + DirectCast(Me.Master, VelociRater).ScriptDT)%>"></script>
    <script src="<%=ResolveClientUrl("~/js/VrProtectionClassLookup.js?dt=" + DirectCast(Me.Master, VelociRater).ScriptDT)%>"></script>
    <script src="<%=ResolveClientUrl("~/js/VrHomeLine.js?dt=" + DirectCast(Me.Master, VelociRater).ScriptDT)%>"></script>
    <script src="<%=ResolveClientUrl("~/js/VrAllLines.js?dt=" + DirectCast(Me.Master, VelociRater).ScriptDT)%>"></script>
    <script>
        var List = ['divInsuredInfo', 'MultiStateSection', 'PropertyAddressContent', 'ResidenceContentDiv', 'MobileHomeContentDiv', 'ProtectionClassContentDiv', 'ProtectionClassContentDivB', 'ProtectionClassContentDivC', 'AdditionalQuestionsContentDiv', 'divAiEntry', 'divLossHistoriesContent', 'dvDeductibleContent', 'dvBaseCoveragesContent', 'divOptionalCoveragesContent', 'divMoreCoverages', 'dvIMCoverages','dvRVWatercraftList', 'dvSectionIAddress'];
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
                    <asp:Panel ID="pnlMain" runat="server">
                        <uc1:ctlHome ID="ctlHomeInput" runat="server" />
                    </asp:Panel>
                </div>
            </td>
        </tr>
    </table>

    <asp:Label ID="lblMsg" runat="server" ForeColor="Red" Font-Bold="true" Text="&nbsp;"></asp:Label>
</asp:Content>