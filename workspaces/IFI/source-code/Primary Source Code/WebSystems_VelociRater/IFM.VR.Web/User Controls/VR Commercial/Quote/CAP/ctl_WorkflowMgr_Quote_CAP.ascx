<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctl_WorkflowMgr_Quote_CAP.ascx.vb" Inherits="IFM.VR.Web.ctl_WorkflowMgr_Quote_CAP" %>
<%@ Register Src="~/User Controls/ctlEffectiveDateChecker.ascx" TagPrefix="uc1" TagName="ctlEffectiveDateChecker" %>
<%@ Register Src="~/User Controls/ctlUWQuestionsPopup.ascx" TagPrefix="uc1" TagName="ctlUWQuestionsPopup" %>
<%@ Register Src="~/User Controls/QuoteEdit/ctlRiskGradeSearch.ascx" TagPrefix="uc1" TagName="ctlRiskGradeSearch" %>
<%@ Register Src="~/User Controls/QuoteEdit/ctlDisplayDiamondRatingErrors.ascx" TagPrefix="uc1" TagName="ctlDisplayDiamondRatingErrors" %>
<%@ Register Src="~/User Controls/ctlTreeView.ascx" TagPrefix="uc1" TagName="ctlTreeView" %>
<%@ Register Src="~/User Controls/QuoteEdit/ctlIsuredList.ascx" TagPrefix="uc1" TagName="ctlIsuredList" %>
<%@ Register Src="~/User Controls/QuoteEdit/HOM/Property/ctlProperty_Address.ascx" TagPrefix="uc1" TagName="ctl_General_Info" %>
<%@ Register Src="~/User Controls/Application/ctl_AttachmentUpload.ascx" TagPrefix="uc1" TagName="ctl_AttachmentUpload" %>
<%@ Register Src="~/User Controls/VR Commercial/Quote/CAP/ctl_CAP_QuoteSummary.ascx" TagPrefix="uc1" TagName="ctl_CAP_QuoteSummary" %>
<%@ Register Src="~/User Controls/VR Commercial/Common/IRPM/ctlCommercial_IRPM.ascx" TagPrefix="uc1" TagName="ctl_Comm_IRPM" %>
<%@ Register Src="~/User Controls/VR Commercial/Common/ctl_Comm_EmailUW.ascx" TagPrefix="uc1" TagName="ctl_Comm_EmailUW" %>
<%@ Register Src="~/User Controls/VR Commercial/Quote/CAP/ctl_CAP_Coverages.ascx" TagPrefix="uc1" TagName="ctl_CAP_Coverages" %>
<%@ Register Src="~/User Controls/VR Commercial/Application/CAP/ctl_CAP_DriverList.ascx" TagPrefix="uc1" TagName="ctl_CAP_DriverList" %>
<%@ Register Src="~/User Controls/VR Commercial/Quote/CAP/ctl_CAP_VehicleList.ascx" TagPrefix="uc1" TagName="ctl_VehicleList" %>
<%@ Register Src="~/User Controls/VR Commercial/Quote/CAP/ctl_CAP_PFSummary.ascx" TagPrefix="uc1" TagName="ctl_CAP_PFSummary" %>
<%@ Register Src="~/User Controls/ctlProposalSelection.ascx" TagPrefix="uc1" TagName="ctlProposalSelection" %>
<%@ Register Src="~/User Controls/VR Commercial/Common/ctl_Comm_NewQuoteForClient.ascx" TagPrefix="uc1" TagName="ctl_Comm_NewQuoteForClient" %>
<%@ Register Src="~/User Controls/Endorsements/ctlPolicyHistory.ascx" TagPrefix="uc1" TagName="ctlPolicyHistory" %>
<%@ Register Src="~/User Controls/Endorsements/ctlBillingInfo.ascx" TagPrefix="uc1" TagName="ctlBillingInfo" %>


<uc1:ctlEffectiveDateChecker runat="server" ID="ctlEffectiveDateChecker" />
<uc1:ctlUWQuestionsPopup runat="server" ID="ctlUWQuestionsPopup" />
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
                <uc1:ctl_General_Info runat="server" ID="ctl_GeneralInfo" />
                <uc1:ctl_CAP_Coverages runat="server" id="ctl_Cap_Covs"></uc1:ctl_CAP_Coverages>
                <uc1:ctl_CAP_DriverList runat="server" ID="ctl_CAP_DriverList" />
                <uc1:ctl_VehicleList runat="server" id="ctl_CAP_VehList"></uc1:ctl_VehicleList>
                <uc1:ctl_AttachmentUpload runat="server" ID="ctl_AttachmentUpload" />
                <uc1:ctl_CAP_QuoteSummary runat="server" id="ctl_CAP_QuoteSummary"></uc1:ctl_CAP_QuoteSummary> 
                <uc1:ctl_CAP_PFSummary runat="server" id="Ctl_CAP_PFSummary"></uc1:ctl_CAP_PFSummary> 
                <uc1:ctl_Comm_IRPM runat="server" ID="ctl_CAP_IRPM" />
                <uc1:ctl_Comm_EmailUW runat="server" ID="ctl_CAP_EmailUW"></uc1:ctl_Comm_EmailUW>
                <uc1:ctlProposalSelection runat="server" ID="ctlProposalSelection" />
                <uc1:ctl_Comm_NewQuoteForClient runat="server" ID="ctl_Comm_NewQuoteForClient" />
                <uc1:ctlPolicyHistory runat="server" ID="ctlPolicyHistory" />
                <uc1:ctlBillingInfo runat="server" ID="ctlBillingInfo" />
            </div>
        </td>
    </tr>
</table>