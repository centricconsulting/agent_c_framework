<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctl_WorkflowMgr_Quote_WCP.ascx.vb" Inherits="IFM.VR.Web.ctl_WorkflowMgr_Quote_WCP" %>
<%@ Register Src="~/User Controls/ctlEffectiveDateChecker.ascx" TagPrefix="uc1" TagName="ctlEffectiveDateChecker" %>
<%@ Register Src="~/User Controls/ctlUWQuestionsPopup.ascx" TagPrefix="uc1" TagName="ctlUWQuestionsPopup" %>
<%@ Register Src="~/User Controls/QuoteEdit/ctlRiskGradeSearch.ascx" TagPrefix="uc1" TagName="ctlRiskGradeSearch" %>
<%@ Register Src="~/User Controls/QuoteEdit/ctlDisplayDiamondRatingErrors.ascx" TagPrefix="uc1" TagName="ctlDisplayDiamondRatingErrors" %>
<%@ Register Src="~/User Controls/ctlTreeView.ascx" TagPrefix="uc1" TagName="ctlTreeView" %>
<%@ Register Src="~/User Controls/QuoteEdit/ctlIsuredList.ascx" TagPrefix="uc1" TagName="ctlIsuredList" %>
<%@ Register Src="~/User Controls/QuoteEdit/HOM/Property/ctlProperty_Address.ascx" TagPrefix="uc1" TagName="ctl_General_Info" %>
<%@ Register Src="~/User Controls/Application/ctl_AttachmentUpload.ascx" TagPrefix="uc1" TagName="ctl_AttachmentUpload" %>
<%@ Register Src="~/User Controls/VR Commercial/Quote/WCP/ctl_WCP_QuoteSummary.ascx" TagPrefix="uc1" TagName="ctl_WCP_QuoteSummary" %>
<%@ Register Src="~/User Controls/VR Commercial/Common/IRPM/ctlCommercial_IRPM.ascx" TagPrefix="uc1" TagName="ctlCommercial_IRPM" %>
<%@ Register Src="~/User Controls/VR Commercial/Common/ctl_Comm_EmailUW.ascx" TagPrefix="uc1" TagName="ctl_Comm_EmailUW" %>
<%@ Register Src="~/User Controls/VR Commercial/Quote/WCP/ctl_WCP_Coverages.ascx" TagPrefix="uc1" TagName="ctl_WCP_Coverages" %>
<%@ Register Src="~/User Controls/VR Commercial/Quote/WCP/ctl_WCP_PFSummary.ascx" TagPrefix="uc1" TagName="ctl_WCP_PFSummary" %>
<%@ Register Src="~/User Controls/ctlProposalSelection.ascx" TagPrefix="uc1" TagName="ctlProposalSelection" %>
<%@ Register Src="~/User Controls/VR Commercial/Common/ctl_Comm_NewQuoteForClient.ascx" TagPrefix="uc1" TagName="ctl_Comm_NewQuoteForClient" %>



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
                <uc1:ctl_WCP_Coverages runat="server" id="ctl_WCPCoverages"></uc1:ctl_WCP_Coverages>
                <uc1:ctl_AttachmentUpload runat="server" ID="ctl_AttachmentUpload" />
                <uc1:ctl_WCP_QuoteSummary runat="server" id="ctl_WCP_QuoteSummary"></uc1:ctl_WCP_QuoteSummary> 
                <uc1:ctl_WCP_PFSummary runat="server" id="Ctl_WCP_PFSummary"></uc1:ctl_WCP_PFSummary> 
                <uc1:ctlCommercial_IRPM runat="server" ID="ctl_WCP_IRPM" />
                <uc1:ctl_Comm_EmailUW runat="server" ID="ctl_WCP_EmailUW"></uc1:ctl_Comm_EmailUW>
                <uc1:ctlProposalSelection runat="server" ID="ctlProposalSelection" />
                <uc1:ctl_Comm_NewQuoteForClient runat="server" ID="ctl_Comm_NewQuoteForClient" />
            </div>
        </td>
    </tr>
</table>