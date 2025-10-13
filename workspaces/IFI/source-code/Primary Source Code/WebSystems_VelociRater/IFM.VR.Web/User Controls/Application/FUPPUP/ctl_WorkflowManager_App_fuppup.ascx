<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctl_WorkflowManager_App_fuppup.ascx.vb" Inherits="IFM.VR.Web.ctl_WorkflowManager_App_fuppup" %>
<%@ Register Src="~/User Controls/ctlTreeView.ascx" TagPrefix="uc1" TagName="ctlTreeView" %>
<%@ Register Src="~/User Controls/Application/ctl_ReturnToQuoteSide.ascx" TagPrefix="uc1" TagName="ctl_ReturnToQuoteSide" %>
<%@ Register Src="~/User Controls/QuoteEdit/ctlDisplayDiamondRatingErrors.ascx" TagPrefix="uc1" TagName="ctlDisplayDiamondRatingErrors" %>
<%@ Register Src="~/User Controls/Application/ctl_AttachmentUpload.ascx" TagPrefix="uc1" TagName="ctl_AttachmentUpload" %>
<%@ Register Src="~/User Controls/Application/FUPPUP/ctl_AppSection_fuppup.ascx" TagPrefix="uc1" TagName="ctl_AppSection_fuppup" %>
<%@ Register Src="~/User Controls/QuoteEdit/FUPPUP/ctl_FUPPUP_QuoteSummary.ascx" TagPrefix="uc1" TagName="ctl_FUPPUP_QuoteSummary" %>
<%@ Register Src="~/User Controls/QuoteEdit/FUPPUP/ctl_FUPPUP_PFSummary.ascx" TagPrefix="uc1" TagName="ctl_FUPPUP_PFSummary" %>





<uc1:ctlDisplayDiamondRatingErrors runat="server" ID="ctlDisplayDiamondRatingErrors" />

<table style="width: 100%;">
    <tr>
        <td style="width: 250px; vertical-align: top;">
            <uc1:ctlTreeView runat="server" ID="ctlTreeView" />
        </td>
        <td style="vertical-align: top;">
            <div id="divAppEditControls" style="display: none;">
                <uc1:ctl_AppSection_fuppup runat="server" id="ctl_AppSection_fuppup" />
                <uc1:ctl_FUPPUP_QuoteSummary runat="server" id="ctl_FUPPUP_QuoteSummary" />
                <uc1:ctl_FUPPUP_PFSummary runat="server" ID="ctl_FUPPUP_PFSummary" />
                <uc1:ctl_AttachmentUpload runat="server" ID="ctl_AttachmentUpload" />
                <uc1:ctl_ReturnToQuoteSide runat="server" ID="ctl_ReturnToQuoteSide" />
            </div>
        </td>
    </tr>
</table>