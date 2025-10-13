<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctl_WorkFlowManager_DFR_Quote.ascx.vb" Inherits="IFM.VR.Web.ctl_WorkFlowManager_DFR_Quote" %>
<%@ Register Src="~/User Controls/ctlEffectiveDateChecker.ascx" TagPrefix="uc1" TagName="ctlEffectiveDateChecker" %>
<%@ Register Src="~/User Controls/ctlUWQuestionsPopup.ascx" TagPrefix="uc1" TagName="ctlUWQuestionsPopup" %>
<%@ Register Src="~/User Controls/QuoteEdit/ctlDisplayDiamondRatingErrors.ascx" TagPrefix="uc1" TagName="ctlDisplayDiamondRatingErrors" %>
<%@ Register Src="~/User Controls/ctlTreeView.ascx" TagPrefix="uc1" TagName="ctlTreeView" %>
<%@ Register Src="~/User Controls/QuoteEdit/ctlIsuredList.ascx" TagPrefix="uc1" TagName="ctlIsuredList" %>
<%@ Register Src="~/User Controls/QuoteEdit/HOM/Property/ctlProperty_HOM.ascx" TagPrefix="uc1" TagName="ctlProperty_HOM" %>
<%@ Register Src="~/User Controls/QuoteEdit/DFR/Residence/ctlDFRResidenceCoverages.ascx" TagPrefix="uc1" TagName="ctlDFRResidenceCoverages" %>
<%@ Register Src="~/User Controls/QuoteEdit/DFR/ctlQuoteSummary_DFR.ascx" TagPrefix="uc1" TagName="ctlQuoteSummary_DFR" %>
<%@ Register Src="~/User Controls/Endorsements/ctlBillingInfo.ascx" TagPrefix="uc1" TagName="ctlBillingInfo" %>
<%@ Register Src="~/User Controls/Endorsements/ctlPrintHistory.ascx" TagPrefix="uc1" TagName="ctlPrintHistory" %>
<%@ Register Src="~/User Controls/Endorsements/ctlPolicyHistory.ascx" TagPrefix="uc1" TagName="ctlPolicyHistory" %>
<%@ Register Src="~/User Controls/Application/ctl_AttachmentUpload.ascx" TagPrefix="uc1" TagName="ctl_AttachmentUpload" %>


<uc1:ctlEffectiveDateChecker runat="server" ID="ctlEffectiveDateChecker" />
<uc1:ctlUWQuestionsPopup runat="server" ID="ctlUWQuestionsPopup" />
<uc1:ctlDisplayDiamondRatingErrors runat="server" ID="ctlDisplayDiamondRatingErrors" />

<table style="width: 100%;">
    <tr>
        <td style="width: 250px; vertical-align: top;">
            <uc1:ctlTreeView runat="server" ID="ctlTreeView" />
        </td>
        <td style="vertical-align: top;">
            <div id="divAppEditControls" style="display: none;">
                <uc1:ctlIsuredList runat="server" ID="ctlIsuredList" />
                <uc1:ctlProperty_HOM runat="server" ID="ctlProperty_HOM" />
                <uc1:ctlDFRResidenceCoverages runat="server" ID="ctlDFRResidenceCoverages" />
                <uc1:ctlQuoteSummary_DFR runat="server" ID="ctlQuoteSummary_DFR" />
                <uc1:ctlBillingInfo runat="server" ID="ctlBillingInfo" />
                <uc1:ctlPrintHistory runat="server" ID="ctlPrintHistory" />
                <uc1:ctlPolicyHistory runat="server" ID="ctlPolicyHistory" />
                <uc1:ctl_AttachmentUpload runat="server" ID="ctl_AttachmentUpload" />
            </div>
        </td>
    </tr>
</table>