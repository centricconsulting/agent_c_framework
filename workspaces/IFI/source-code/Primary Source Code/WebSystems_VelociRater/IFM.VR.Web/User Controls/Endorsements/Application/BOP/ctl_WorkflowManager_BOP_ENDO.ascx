<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctl_WorkflowManager_BOP_ENDO.ascx.vb" Inherits="IFM.VR.Web.ctl_WorkflowManager_BOP_Endo" %>
<%@ Register Src="~/User Controls/ctlEffectiveDateChecker.ascx" TagPrefix="uc1" TagName="ctlEffectiveDateChecker" %>
<%@ Register Src="~/User Controls/QuoteEdit/ctlRiskGradeSearch.ascx" TagPrefix="uc1" TagName="ctlRiskGradeSearch" %>
<%@ Register Src="~/User Controls/QuoteEdit/ctlDisplayDiamondRatingErrors.ascx" TagPrefix="uc1" TagName="ctlDisplayDiamondRatingErrors" %>
<%@ Register Src="~/User Controls/ctlTreeView.ascx" TagPrefix="uc1" TagName="ctlTreeView" %>
<%@ Register Src="~/User Controls/QuoteEdit/ctlIsuredList.ascx" TagPrefix="uc1" TagName="ctlIsuredList" %>
<%@ Register Src="~/User Controls/VR Commercial/Quote/BOP/ctl_BOP_QuoteSummary.ascx" TagPrefix="uc1" TagName="ctl_BOPQuoteSummary" %>
<%@ Register Src="~/User Controls/VR Commercial/Common/IRPM/ctlCommercial_IRPM.ascx" TagPrefix="uc1" TagName="ctl_Comm_IRPM" %>
<%@ Register Src="~/User Controls/VR Commercial/Quote/BOP/ctl_BOP_PFSummary.ascx" TagPrefix="uc1" TagName="ctl_BOPQuoteSummaryPF" %>
<%@ Register Src="~/User Controls/ctlProposalSelection.ascx" TagPrefix="uc1" TagName="ctlProposalSelection" %>
<%@ Register Src="~/User Controls/Endorsements/Application/BOP/PolicyLevelCoverages/ctl_BOP_ENDO_PolicyLevelCoverages.ascx" TagPrefix="uc1" TagName="ctl_BOP_ENDO_PolicyLevelCoverages" %>
<%@ Register Src="~/User Controls/Endorsements/Application/BOP/Locations/ctl_BOP_ENDO_LocationList.ascx" TagPrefix="uc1" TagName="ctl_BOP_ENDO_LocationList" %>










<uc1:ctlEffectiveDateChecker runat="server" ID="ctlEffectiveDateChecker" />
<uc1:ctlRiskGradeSearch runat="server" ID="ctlRiskGradeSearch" />
<uc1:ctlDisplayDiamondRatingErrors runat="server" ID="ctlDisplayDiamondRatingErrors" />

<table style="width: 100%;">
    <tr>
        <td style="width: 250px; vertical-align: top;">
            <uc1:ctlTreeView runat="server" ID="ctlTreeView" />
        </td>
        <td style="vertical-align: top;">
            <div id="divEditControls" style="display: none;">
                <uc1:ctlIsuredList runat="server" ID="ctlIsuredList" />
                <uc1:ctl_BOP_ENDO_PolicyLevelCoverages runat="server" ID="ctl_BOP_ENDO_PolicyLevelCoverages" />
                <uc1:ctl_BOP_ENDO_LocationList runat="server" ID="ctl_BOP_ENDO_LocationList" />
                <uc1:ctl_BOPQuoteSummary runat="server" id="ctl_BOP_Quote_Summary"></uc1:ctl_BOPQuoteSummary>
                <uc1:ctl_BOPQuoteSummaryPF runat="server" id="Ctl_PF_BOPQuoteSummary"></uc1:ctl_BOPQuoteSummaryPF>
                <uc1:ctlProposalSelection runat="server" ID="ctlProposalSelection" />
            </div>
        </td>
    </tr>
</table>