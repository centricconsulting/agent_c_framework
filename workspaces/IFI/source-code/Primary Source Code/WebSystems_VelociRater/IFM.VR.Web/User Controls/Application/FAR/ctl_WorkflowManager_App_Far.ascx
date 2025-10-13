<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctl_WorkflowManager_App_Far.ascx.vb" Inherits="IFM.VR.Web.ctl_WorkflowManager_App_Far" %>
<%@ Register Src="~/User Controls/ctlTreeView.ascx" TagPrefix="uc1" TagName="ctlTreeView" %>
<%@ Register Src="~/User Controls/Application/FAR/ctlUWQuestionsFARM.ascx" TagPrefix="uc1" TagName="ctlUWQuestionsFARM" %>
<%@ Register Src="~/User Controls/Application/FAR/ctl_AppSection_Farm.ascx" TagPrefix="uc1" TagName="ctl_AppSection_Farm" %>
<%@ Register Src="~/User Controls/QuoteEdit/FAR/ctlQuoteSummary_Farm.ascx" TagPrefix="uc1" TagName="ctlQuoteSummary_Farm" %>
<%@ Register Src="~/User Controls/Application/ctl_ReturnToQuoteSide.ascx" TagPrefix="uc1" TagName="ctl_ReturnToQuoteSide" %>
<%@ Register Src="~/User Controls/QuoteEdit/FAR/ctlIRPM.ascx" TagPrefix="uc1" TagName="ctlIRPM" %>
<%@ Register Src="~/User Controls/QuoteEdit/ctlDisplayDiamondRatingErrors.ascx" TagPrefix="uc1" TagName="ctlDisplayDiamondRatingErrors" %>
<%@ Register Src="~/User Controls/Application/ctl_AttachmentUpload.ascx" TagPrefix="uc1" TagName="ctl_AttachmentUpload" %>
<%@ Register Src="~/User Controls/Application/ctl_OrderClueAndOrMVR.ascx" TagPrefix="uc1" TagName="ctl_OrderClueAndOrMVR" %>



<uc1:ctlDisplayDiamondRatingErrors runat="server" ID="ctlDisplayDiamondRatingErrors" />

<table style="width: 100%;">
    <tr>
        <td style="width: 250px; vertical-align: top;">
            <uc1:ctlTreeView runat="server" ID="ctlTreeView" />
        </td>
        <td style="vertical-align: top;">
            <div id="divAppEditControls" style="display: none;">
                <uc1:ctl_orderclueandormvr runat="server" id="ctl_OrderClueAndOrMVR" />
                <uc1:ctlUWQuestionsFARM runat="server" ID="ctlUWQuestionsFARM" />
                <uc1:ctl_AttachmentUpload runat="server" ID="ctl_AttachmentUpload" />
                <uc1:ctl_AppSection_Farm runat="server" ID="ctl_AppSection_Farm" />
                <uc1:ctlQuoteSummary_Farm runat="server" ID="ctlQuoteSummary_Farm" />
                <uc1:ctl_ReturnToQuoteSide runat="server" ID="ctl_ReturnToQuoteSide" />
            </div>
            <uc1:ctlIRPM runat="server" ID="ctlIRPM" />
        </td>
    </tr>
</table>