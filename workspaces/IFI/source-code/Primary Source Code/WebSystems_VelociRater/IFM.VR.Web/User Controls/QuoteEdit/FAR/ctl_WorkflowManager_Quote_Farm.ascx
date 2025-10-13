<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctl_WorkflowManager_Quote_Farm.ascx.vb" Inherits="IFM.VR.Web.ctl_WorkflowManager_Quote_Farm" %>

<%@ Register Src="~/User Controls/ctlTreeView.ascx" TagPrefix="uc1" TagName="ctlTreeView" %>
<%@ Register Src="~/User Controls/QuoteEdit/ctlIsuredList.ascx" TagPrefix="uc1" TagName="ctlIsuredList" %>
<%@ Register Src="~/User Controls/ctlEffectiveDateChecker.ascx" TagPrefix="uc1" TagName="ctlEffectiveDateChecker" %>
<%@ Register Src="~/User Controls/QuoteEdit/ctlApplicantList.ascx" TagPrefix="uc1" TagName="ctlApplicantList" %>
<%@ Register Src="~/User Controls/QuoteEdit/FAR/ctl_Farm_Basic_Policy_Info.ascx" TagPrefix="uc1" TagName="ctl_Farm_Basic_Policy_Info" %>
<%@ Register Src="~/User Controls/ctlUWQuestionsPopup.ascx" TagPrefix="uc1" TagName="ctlUWQuestionsPopup" %>
<%@ Register Src="~/User Controls/QuoteEdit/FAR/ctl_Insured_Applicant_Manager.ascx" TagPrefix="uc1" TagName="ctl_Insured_Applicant_Manager" %>
<%@ Register Src="~/User Controls/QuoteEdit/FAR/ctlFarmPolicyCoverages.ascx" TagPrefix="uc1" TagName="ctlFarmPolicyCoverages" %>
<%@ Register Src="~/User Controls/QuoteEdit/FAR/ctlIRPM.ascx" TagPrefix="uc1" TagName="ctlIRPM" %>
<%@ Register Src="~/User Controls/QuoteEdit/FAR/ctlFarmLocationCoverage.ascx" TagPrefix="uc1" TagName="ctlFarmLocationCoverage" %>
<%@ Register Src="~/User Controls/QuoteEdit/FAR/ctlQuoteSummary_Farm.ascx" TagPrefix="uc1" TagName="ctlQuoteSummary_Farm" %>
<%@ Register Src="~/User Controls/QuoteEdit/FAR/ctlFarmPersonalProperty.ascx" TagPrefix="uc1" TagName="ctlFarmPersonalProperty" %>
<%@ Register Src="~/User Controls/QuoteEdit/ctlIMRVWatercraft.ascx" TagPrefix="uc1" TagName="ctlIMRVWatercraft" %>
<%@ Register Src="~/User Controls/QuoteEdit/ctlDisplayDiamondRatingErrors.ascx" TagPrefix="uc1" TagName="ctlDisplayDiamondRatingErrors" %>
<%@ Register Src="~/User Controls/VR Commercial/Common/ctl_Comm_NewQuoteForClient.ascx" TagPrefix="uc1" TagName="ctl_Comm_NewQuoteForClient" %>
<%@ Register Src="~/User Controls/Endorsements/ctlPolicyHistory.ascx" TagPrefix="uc1" TagName="ctlPolicyHistory" %>
<%@ Register Src="~/User Controls/Endorsements/ctlBillingInfo.ascx" TagPrefix="uc1" TagName="ctlBillingInfo" %>
<%@ Register Src="~/User Controls/Application/ctl_AttachmentUpload.ascx" TagPrefix="uc1" TagName="ctl_AttachmentUpload" %>






<uc1:ctlEffectiveDateChecker runat="server" ID="ctlEffectiveDateChecker" />
<uc1:ctl_Farm_Basic_Policy_Info runat="server" id="ctl_Farm_Basic_Policy_Info" />

<uc1:ctlUWQuestionsPopup runat="server" ID="ctlUWQuestionsPopup" />
<uc1:ctlDisplayDiamondRatingErrors runat="server" ID="ctlDisplayDiamondRatingErrors" />
<table style="width: 100%;">
    <tr>
        <td style="width: 250px; vertical-align: top;">
            <uc1:ctlTreeView runat="server" ID="ctlTreeView" />
        </td>
        <td style="vertical-align: top;">
            <div id="divAppEditControls" style="display: none;">
                <uc1:ctl_Insured_Applicant_Manager runat="server" id="ctl_Insured_Applicant_Manager" />
                <uc1:ctlFarmPolicyCoverages runat="server" id="ctlFarmPolicyCoverages" />
                <uc1:ctlFarmLocationCoverage runat="server" id="ctlFarmLocationCoverage" />
                <uc1:ctlQuoteSummary_Farm runat="server" ID="ctlQuoteSummary_Farm" />
                <uc1:ctlIRPM runat="server" ID="ctlIRPM" />
                <uc1:ctlfarmpersonalproperty runat="server" id="ctlFarmPersonalProperty" />
                <uc1:ctlIMRVWatercraft runat="server" id="ctlIMRVWatercraft" />
                <uc1:ctl_Comm_NewQuoteForClient runat="server" ID="ctl_Comm_NewQuoteForClient" />
                <uc1:ctlPolicyHistory runat="server" ID="ctlPolicyHistory" />
                <uc1:ctlBillingInfo runat="server" ID="ctlBillingInfo" />
                <uc1:ctl_AttachmentUpload runat="server" ID="ctl_AttachmentUpload" />
            </div>
        </td>
    </tr>
</table>