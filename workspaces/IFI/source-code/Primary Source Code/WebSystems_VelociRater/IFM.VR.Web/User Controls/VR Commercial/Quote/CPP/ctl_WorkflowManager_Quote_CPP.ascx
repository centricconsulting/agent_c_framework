<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctl_WorkflowManager_Quote_CPP.ascx.vb" Inherits="IFM.VR.Web.ctl_WorkflowManager_Quote_CPP" %>

<%@ Register Src="~/User Controls/ctlTreeView.ascx" TagPrefix="uc1" TagName="ctlTreeView" %>
<%@ Register Src="~/User Controls/QuoteEdit/ctlIsuredList.ascx" TagPrefix="uc1" TagName="ctlIsuredList" %>
<%@ Register Src="~/User Controls/ctlEffectiveDateChecker.ascx" TagPrefix="uc1" TagName="ctlEffectiveDateChecker" %>
<%@ Register Src="~/User Controls/QuoteEdit/ctlApplicantList.ascx" TagPrefix="uc1" TagName="ctlApplicantList" %>
<%@ Register Src="~/User Controls/ctlUWQuestionsPopup.ascx" TagPrefix="uc1" TagName="ctlUWQuestionsPopup" %>
<%@ Register Src="~/User Controls/QuoteEdit/ctlDisplayDiamondRatingErrors.ascx" TagPrefix="uc1" TagName="ctlDisplayDiamondRatingErrors" %>
<%@ Register Src="~/User Controls/QuoteEdit/ctlRiskGradeSearch.ascx" TagPrefix="uc1" TagName="ctlRiskGradeSearch" %>
<%@ Register Src="~/User Controls/VR Commercial/Common/IRPM_CPP/ctlCommercial_IRPM_CPP.ascx" TagPrefix="uc1" TagName="ctlCommercial_IRPM_CPP" %>
<%@ Register Src="~/User Controls/VR Commercial/Common/ctl_Comm_EmailUW.ascx" TagPrefix="uc1" TagName="ctl_Comm_EmailUW" %>
<%@ Register Src="~/User Controls/Application/ctl_AttachmentUpload.ascx" TagPrefix="uc1" TagName="ctl_AttachmentUpload" %>
<%@ Register Src="~/User Controls/VR Commercial/Quote/CPP/PolicyCoverages/ctl_CPP_PropertyCoverages.ascx" TagPrefix="uc1" TagName="ctl_CPP_CPR_Covs" %>
<%@ Register Src="~/User Controls/VR Commercial/Quote/CPP/PolicyCoverages/ctl_CPP_LiabilityCoverages.ascx" TagPrefix="uc1" TagName="ctl_CPP_CGL_Covs" %>
<%@ Register Src="~/User Controls/VR Commercial/Quote/CPP/Locations/ctl_CPP_Property_Locations.ascx" TagPrefix="uc1" TagName="ctl_CPP_CPR_Locs" %>
<%@ Register Src="~/User Controls/VR Commercial/Quote/CPP/ctl_CPP_InlandMarine.ascx" TagPrefix="uc1" TagName="ctl_CPP_InlandMarine" %>

<%@ Register Src="~/User Controls/VR Commercial/Quote/CPP/Locations/ctl_CPP_Liability_LocationList.ascx" TagPrefix="uc1" TagName="ctl_CPP_CGL_Locs" %>
<%@ Register Src="~/User Controls/VR Commercial/Quote/CPP/ctl_CPP_QuoteSummary.ascx" TagPrefix="uc1" TagName="ctl_CPP_Summary" %>
<%@ Register Src="~/User Controls/VR Commercial/Quote/CPP/ctl_CPP_Crime.ascx" TagPrefix="uc1" TagName="ctl_CPP_Crime" %>
<%@ Register Src="~/User Controls/ctlProposalSelection.ascx" TagPrefix="uc1" TagName="ctlProposalSelection" %>
<%@ Register Src="~/User Controls/VR Commercial/Common/ctl_Comm_NewQuoteForClient.ascx" TagPrefix="uc1" TagName="ctl_Comm_NewQuoteForClient" %>
<%@ Register Src="~/User Controls/VR Commercial/Quote/CPP/ctl_CPP_PFSummary.ascx" TagPrefix="uc1" TagName="ctl_CPP_PFSummary" %>
<%@ Register Src="~/User Controls/Endorsements/ctlPolicyHistory.ascx" TagPrefix="uc1" TagName="ctlPolicyHistory" %>
<%@ Register Src="~/User Controls/Endorsements/ctlBillingInfo.ascx" TagPrefix="uc1" TagName="ctlBillingInfo" %>
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
                <uc1:ctl_CPP_CPR_Covs id="ctl_CPR_Coverages" runat="server" />
                <uc1:ctl_CPP_CGL_Covs id="ctl_CGL_Coverages" runat="server" />
                <uc1:ctl_CPP_InlandMarine runat="server" ID="ctl_CPP_InlandMarine" />
                <uc1:ctl_CPP_CPR_Locs ID="ctl_CPR_Locations" runat="server" />
                <uc1:ctl_CPP_CGL_Locs ID="ctl_CPP_CGL_Locations" runat="server" />
                <uc1:ctl_AttachmentUpload runat="server" ID="ctl_AttachmentUpload" />
                <uc1:ctl_CPP_Summary runat="server" id="ctl_CPP_QuoteSummary" />
                <uc1:ctl_CPP_PFSummary runat="server" id="Ctl_CPP_PFSummary"></uc1:ctl_CPP_PFSummary>
                <uc1:ctlCommercial_IRPM_CPP runat="server" ID="ctlCommercial_IRPM_CPP" />
                <uc1:ctl_Comm_EmailUW runat="server" ID="ctl_CPP_EmailUW" />
                <uc1:ctl_CPP_Crime runat="server" ID="ctl_CPP_Crime" />
                <uc1:ctlProposalSelection runat="server" ID="ctlProposalSelection" />
                <uc1:ctl_Comm_NewQuoteForClient runat="server" ID="ctl_Comm_NewQuoteForClient" />
                <uc1:ctlBillingInfo runat="server" ID="ctlBillingInfo" />
                <uc1:ctlPolicyHistory runat="server" ID="ctlPolicyHistory" />              
            </div>
        </td>
    </tr>
</table>