<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctl_WorkflowManager_Quote_CPR.ascx.vb" Inherits="IFM.VR.Web.ctl_WorkflowManager_Quote_CPR" %>

<%@ Register Src="~/User Controls/ctlTreeView.ascx" TagPrefix="uc1" TagName="ctlTreeView" %>
<%@ Register Src="~/User Controls/QuoteEdit/ctlIsuredList.ascx" TagPrefix="uc1" TagName="ctlIsuredList" %>
<%@ Register Src="~/User Controls/ctlEffectiveDateChecker.ascx" TagPrefix="uc1" TagName="ctlEffectiveDateChecker" %>
<%@ Register Src="~/User Controls/QuoteEdit/ctlApplicantList.ascx" TagPrefix="uc1" TagName="ctlApplicantList" %>
<%@ Register Src="~/User Controls/ctlUWQuestionsPopup.ascx" TagPrefix="uc1" TagName="ctlUWQuestionsPopup" %>
<%@ Register Src="~/User Controls/QuoteEdit/ctlDisplayDiamondRatingErrors.ascx" TagPrefix="uc1" TagName="ctlDisplayDiamondRatingErrors" %>
<%@ Register Src="~/User Controls/QuoteEdit/ctlRiskGradeSearch.ascx" TagPrefix="uc1" TagName="ctlRiskGradeSearch" %>
<%@ Register Src="~/User Controls/VR Commercial/Quote/CPR/PolicyLevelCoverages/ctl_CPR_Coverages.ascx" TagPrefix="uc1" TagName="ctl_Coverages" %>
<%@ Register Src="~/User Controls/VR Commercial/Quote/CPR/Locations/ctl_CPR_LocationList.ascx" TagPrefix="uc1" TagName="ctl_LocationsList" %>
<%@ Register Src="~/User Controls/VR Commercial/Common/IRPM/ctlCommercial_IRPM.ascx" TagPrefix="uc1" TagName="ctlCommercial_IRPM" %>
<%@ Register Src="~/User Controls/VR Commercial/Common/ctl_Comm_EmailUW.ascx" TagPrefix="uc1" TagName="ctl_Comm_EmailUW" %>
<%@ Register Src="~/User Controls/Application/ctl_AttachmentUpload.ascx" TagPrefix="uc1" TagName="ctl_AttachmentUpload" %>

<%@ Register Src="~/User Controls/VR Commercial/Quote/CPR/ctl_CPR_QuoteSummary.ascx" TagPrefix="uc1" TagName="ctl_CPR_QuoteSummary" %>
<%@ Register Src="~/User Controls/VR Commercial/Quote/CPR/ctl_CPR_PFSummary.ascx" TagPrefix="uc1" TagName="ctl_CPR_PFSummary" %>
<%@ Register Src="~/User Controls/ctlProposalSelection.ascx" TagPrefix="uc1" TagName="ctlProposalSelection" %>
<%@ Register Src="~/User Controls/VR Commercial/Common/ctl_Comm_NewQuoteForClient.ascx" TagPrefix="uc1" TagName="ctl_Comm_NewQuoteForClient" %>
<%@ Register src="~/User Controls/VR Commercial/Common/ctlCommercialDataPrefillEntry.ascx" tagname="ctlCommercialDataPrefillEntry" tagprefix="uc1" %>



<uc1:ctlEffectiveDateChecker runat="server" ID="ctlEffectiveDateChecker" />
<uc1:ctlUWQuestionsPopup runat="server" ID="ctlUWQuestionsPopup" />
<uc1:ctlRiskGradeSearch runat="server" id="ctlRiskGradeSearch" />
<uc1:ctlDisplayDiamondRatingErrors runat="server" ID="ctlDisplayDiamondRatingErrors" />
<uc1:ctlCommercialDataPrefillEntry ID="ctlCommercialDataPrefillEntry" runat="server" />

<table style="width: 100%;">
    <tr>
        <td style="width: 250px; vertical-align: top;">
            <uc1:ctlTreeView runat="server" ID="ctlTreeView" />
        </td>
        <td style="vertical-align: top;">
            <div id="divEditControls" style="display: none;">
                <uc1:ctlIsuredList runat="server" ID="ctlIsuredList" />
                <uc1:ctl_Coverages runat="server" id="ctl_CPR_PolicyCoverages" />
                <uc1:ctl_LocationsList runat="server" id="ctl_CPR_LocationsList"></uc1:ctl_LocationsList>
                <uc1:ctl_AttachmentUpload runat="server" ID="ctl_AttachmentUpload" />
                <uc1:ctl_CPR_QuoteSummary runat="server" id="ctl_CPR_QuoteSummary" />
                <uc1:ctl_CPR_PFSummary runat="server" id="Ctl_CPR_PFSummary"></uc1:ctl_CPR_PFSummary> 
                <uc1:ctlCommercial_IRPM runat="server" ID="ctl_CPR_IRPM" />
                <uc1:ctl_Comm_EmailUW runat="server" ID="ctl_CPR_EmailUW"></uc1:ctl_Comm_EmailUW>
                <uc1:ctlProposalSelection runat="server" ID="ctlProposalSelection" />
                <uc1:ctl_Comm_NewQuoteForClient runat="server" ID="ctl_Comm_NewQuoteForClient" />
            </div>
        </td>
    </tr>
</table>