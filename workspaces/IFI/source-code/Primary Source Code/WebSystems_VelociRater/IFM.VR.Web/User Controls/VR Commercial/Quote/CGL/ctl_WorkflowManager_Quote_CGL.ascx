<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctl_WorkflowManager_Quote_CGL.ascx.vb" Inherits="IFM.VR.Web.ctl_WorkflowManager_Quote_CGL" %>

<%@ Register Src="~/User Controls/ctlTreeView.ascx" TagPrefix="uc1" TagName="ctlTreeView" %>
<%@ Register Src="~/User Controls/QuoteEdit/ctlIsuredList.ascx" TagPrefix="uc1" TagName="ctlIsuredList" %>
<%@ Register Src="~/User Controls/ctlEffectiveDateChecker.ascx" TagPrefix="uc1" TagName="ctlEffectiveDateChecker" %>
<%@ Register Src="~/User Controls/QuoteEdit/ctlApplicantList.ascx" TagPrefix="uc1" TagName="ctlApplicantList" %>
<%@ Register Src="~/User Controls/ctlUWQuestionsPopup.ascx" TagPrefix="uc1" TagName="ctlUWQuestionsPopup" %>
<%@ Register Src="~/User Controls/QuoteEdit/ctlDisplayDiamondRatingErrors.ascx" TagPrefix="uc1" TagName="ctlDisplayDiamondRatingErrors" %>
<%@ Register Src="~/User Controls/QuoteEdit/ctlRiskGradeSearch.ascx" TagPrefix="uc1" TagName="ctlRiskGradeSearch" %>
<%@ Register Src="~/User Controls/VR Commercial/Quote/CGL/PolicyLevelCoverages/ctl_CGL_PolicyLevelCoverages.ascx" TagPrefix="uc1" TagName="ctl_CGL_PolicyLevelCoverages" %>
<%@ Register Src="~/User Controls/VR Commercial/Quote/CGL/Locations/ctl_CGL_LocationsWF.ascx" TagPrefix="uc1" TagName="ctl_CGL_LocationsWF" %>
<%@ Register Src="~/User Controls/VR Commercial/Quote/CGL/ctl_CGL_QuoteSummary.ascx" TagPrefix="uc1" TagName="ctl_CGL_QuoteSummary" %>
<%@ Register Src="~/User Controls/VR Commercial/Common/IRPM/ctlCommercial_IRPM.ascx" TagPrefix="uc1" TagName="ctlCommercial_IRPM" %>
<%@ Register Src="~/User Controls/VR Commercial/Common/ctl_Comm_EmailUW.ascx" TagPrefix="uc1" TagName="ctl_Comm_EmailUW" %>
<%@ Register Src="~/User Controls/VR Commercial/Quote/CGL/ctl_CGL_PFSummary.ascx" TagPrefix="uc1" TagName="ctl_CGL_PFSummary" %>
<%@ Register Src="~/User Controls/Application/ctl_AttachmentUpload.ascx" TagPrefix="uc1" TagName="ctl_AttachmentUpload" %>
<%@ Register Src="~/User Controls/ctlProposalSelection.ascx" TagPrefix="uc1" TagName="ctlProposalSelection" %>
<%@ Register Src="~/User Controls/VR Commercial/Common/ctl_Comm_NewQuoteForClient.ascx" TagPrefix="uc1" TagName="ctl_Comm_NewQuoteForClient" %>




<uc1:ctlEffectiveDateChecker runat="server" ID="ctlEffectiveDateChecker" />
<uc1:ctlUWQuestionsPopup runat="server" ID="ctlUWQuestionsPopup" />
<uc1:ctlRiskGradeSearch runat="server" id="ctlRiskGradeSearch" />
<uc1:ctlDisplayDiamondRatingErrors runat="server" ID="ctlDisplayDiamondRatingErrors" />

<table style="width: 100%;">
    <tr>
        <td style="width: 250px; vertical-align: top;">
            <uc1:ctlTreeView runat="server" ID="ctlTreeView" />
        </td>
        <td style="vertical-align: top;">
            <div id="divEditControls" style="display: none;">
                <uc1:ctlIsuredList runat="server" ID="ctlIsuredList" />
                <uc1:ctl_CGL_PolicyLevelCoverages runat="server" id="ctl_CGL_PolicyLevelCoverages" />
                <uc1:ctl_CGL_LocationsWF runat="server" id="ctl_CGL_LocationsWF" />
                <uc1:ctl_AttachmentUpload runat="server" ID="ctl_AttachmentUpload" />
                <uc1:ctl_CGL_QuoteSummary runat="server" id="ctl_CGL_QuoteSummary" />
                <uc1:ctl_CGL_PFSummary runat="server" id="Ctl_CGL_PFSummary"></uc1:ctl_CGL_PFSummary> 
                <uc1:ctlCommercial_IRPM runat="server" ID="ctl_CGL_IRPM" />
                <uc1:ctl_Comm_EmailUW runat="server" ID="ctl_CGL_EmailUW"></uc1:ctl_Comm_EmailUW>
                <uc1:ctlProposalSelection runat="server" ID="ctlProposalSelection" />
                <uc1:ctl_Comm_NewQuoteForClient runat="server" ID="ctl_Comm_NewQuoteForClient" />
            </div>
        </td>
    </tr>
</table>