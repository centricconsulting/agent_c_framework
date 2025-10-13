<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctl_WorkflowManager_BOP_Quote.ascx.vb" Inherits="IFM.VR.Web.ctl_WorkflowManager_BOP_Quote" %>
<%@ Register Src="~/User Controls/ctlEffectiveDateChecker.ascx" TagPrefix="uc1" TagName="ctlEffectiveDateChecker" %>
<%@ Register Src="~/User Controls/ctlUWQuestionsPopup.ascx" TagPrefix="uc1" TagName="ctlUWQuestionsPopup" %>
<%@ Register Src="~/User Controls/QuoteEdit/ctlRiskGradeSearch.ascx" TagPrefix="uc1" TagName="ctlRiskGradeSearch" %>
<%@ Register Src="~/User Controls/QuoteEdit/ctlDisplayDiamondRatingErrors.ascx" TagPrefix="uc1" TagName="ctlDisplayDiamondRatingErrors" %>
<%@ Register Src="~/User Controls/ctlTreeView.ascx" TagPrefix="uc1" TagName="ctlTreeView" %>
<%@ Register Src="~/User Controls/QuoteEdit/ctlIsuredList.ascx" TagPrefix="uc1" TagName="ctlIsuredList" %>
<%@ Register Src="~/User Controls/VR Commercial/Quote/BOP/PolicyLevelCoverages/ctl_BOP_PolicyLevelCoverages.ascx" TagPrefix="uc1" TagName="ctl_BOP_PolicyLevelCoverages" %>
<%@ Register Src="~/User Controls/VR Commercial/Quote/BOP/Locations/ctl_BOP_LocationList.ascx" TagPrefix="uc1" TagName="ctl_BOP_LocationList" %>
<%@ Register Src="~/User Controls/VR Commercial/Quote/BOP/ctl_BOP_QuoteSummary.ascx" TagPrefix="uc1" TagName="ctl_BOPQuoteSummary" %>
<%@ Register Src="~/User Controls/VR Commercial/Common/IRPM/ctlCommercial_IRPM.ascx" TagPrefix="uc1" TagName="ctl_Comm_IRPM" %>
<%@ Register Src="~/User Controls/VR Commercial/Common/ctl_Comm_EmailUW.ascx" TagPrefix="uc1" TagName="ctl_Comm_EmailUW" %>
<%@ Register Src="~/User Controls/VR Commercial/Quote/BOP/ctl_BOP_PFSummary.ascx" TagPrefix="uc1" TagName="ctl_BOPQuoteSummaryPF" %>
<%@ Register Src="~/User Controls/ctlProposalSelection.ascx" TagPrefix="uc1" TagName="ctlProposalSelection" %>
<%@ Register Src="~/User Controls/VR Commercial/Common/ctl_Comm_NewQuoteForClient.ascx" TagPrefix="uc1" TagName="ctl_Comm_NewQuoteForClient" %>
<%@ Register Src="~/User Controls/Endorsements/ctlPolicyHistory.ascx" TagPrefix="uc1" TagName="ctlPolicyHistory" %>
<%@ Register Src="~/User Controls/Endorsements/ctlBillingInfo.ascx" TagPrefix="uc1" TagName="ctlBillingInfo" %>
<%@ Register Src="~/User Controls/VR Commercial/Common/ctlCommercialDataPrefillEntry.ascx" TagPrefix="uc1" TagName="ctlCommercialDataPrefillEntry" %>



<uc1:ctlEffectiveDateChecker runat="server" ID="ctlEffectiveDateChecker" />
<uc1:ctlUWQuestionsPopup runat="server" ID="ctlUWQuestionsPopup" />
<uc1:ctlRiskGradeSearch runat="server" ID="ctlRiskGradeSearch" />
<uc1:ctlDisplayDiamondRatingErrors runat="server" ID="ctlDisplayDiamondRatingErrors" />
<uc1:ctlCommercialDataPrefillEntry runat="server" ID="ctlCommercialDataPrefillEntry" />

<table style="width: 100%;">
    <tr>
        <td style="width: 250px; vertical-align: top;">
            <uc1:ctlTreeView runat="server" ID="ctlTreeView" />
        </td>
        <td style="vertical-align: top;">
            <div id="divEditControls" style="display: none;">
                <uc1:ctlIsuredList runat="server" ID="ctlIsuredList" />
                <uc1:ctl_BOP_PolicyLevelCoverages runat="server" id="ctl_BOP_PolicyLevelCoverages" />
                <uc1:ctl_BOP_LocationList runat="server" id="ctl_BOP_LocationList" />
                <uc1:ctl_BOPQuoteSummary runat="server" id="ctl_BOP_Quote_Summary"></uc1:ctl_BOPQuoteSummary>
                <uc1:ctl_BOPQuoteSummaryPF runat="server" id="Ctl_PF_BOPQuoteSummary"></uc1:ctl_BOPQuoteSummaryPF>
                <uc1:ctl_Comm_IRPM runat="server" ID="ctl_BOP_IRPM" />
                <uc1:ctl_Comm_EmailUW runat="server" ID="ctl_BOP_EmailUW"></uc1:ctl_Comm_EmailUW>
                <uc1:ctlProposalSelection runat="server" ID="ctlProposalSelection" />
                <uc1:ctl_Comm_NewQuoteForClient runat="server" ID="ctl_Comm_NewQuoteForClient" />
                <uc1:ctlPolicyHistory runat="server" ID="ctlPolicyHistory" />
                <uc1:ctlBillingInfo runat="server" ID="ctlBillingInfo" />
            </div>
        </td>
    </tr>
</table>