<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctl_WorkFlowManager_DFR_App.ascx.vb" Inherits="IFM.VR.Web.ctl_WorkFlowManager_DFR_App" %>
<%@ Register Src="~/User Controls/QuoteEdit/ctlDisplayDiamondRatingErrors.ascx" TagPrefix="uc1" TagName="ctlDisplayDiamondRatingErrors" %>
<%@ Register Src="~/User Controls/ctlTreeView.ascx" TagPrefix="uc1" TagName="ctlTreeView" %>
<%@ Register Src="~/User Controls/Application/ctl_ReturnToQuoteSide.ascx" TagPrefix="uc1" TagName="ctl_ReturnToQuoteSide" %>
<%@ Register Src="~/User Controls/Application/DFR/ctl_DFR_AppSection.ascx" TagPrefix="uc1" TagName="ctl_DFR_AppSection" %>
<%@ Register Src="~/User Controls/QuoteEdit/DFR/ctlQuoteSummary_DFR.ascx" TagPrefix="uc1" TagName="ctlQuoteSummary_DFR" %>
<%@ Register Src="~/User Controls/Application/ctlUWQuestions.ascx" TagPrefix="uc1" TagName="ctlUWQuestions" %>
<%@ Register Src="~/User Controls/Application/ctl_OrderClueAndOrMVR.ascx" TagPrefix="uc1" TagName="ctl_OrderClueAndOrMVR" %>
<%@ Register Src="~/User Controls/Application/ctl_AttachmentUpload.ascx" TagPrefix="uc1" TagName="ctl_AttachmentUpload" %>
<%@ register src="~/User Controls/Application/ctl_UnderwritingQuestionsByLob.ascx" tagprefix="uc1" tagname="ctl_UnderwritingQuestionsByLob" %>






<uc1:ctlDisplayDiamondRatingErrors runat="server" ID="ctlDisplayDiamondRatingErrors" />
<uc1:ctl_OrderClueAndOrMVR runat="server" ID="ctl_OrderClueAndOrMVR" />
<table style="width: 100%;">
    <tr>
        <td style="width: 250px; vertical-align: top;">
            <uc1:ctlTreeView runat="server" ID="ctlTreeView" />
        </td>
        <td style="vertical-align: top;">
            <div id="divAppEditControls" style="display: none;">
                <uc1:ctlUWQuestions runat="server" ID="ctlUWQuestions" />
                <uc1:ctl_UnderwritingQuestionsByLob runat="server" id="ctl_UnderwritingQuestionsByLob" />
                <uc1:ctl_AttachmentUpload runat="server" ID="ctl_AttachmentUpload" />

                <uc1:ctl_DFR_AppSection runat="server" ID="ctl_DFR_AppSection" />
                <uc1:ctlQuoteSummary_DFR runat="server" ID="ctlQuoteSummary_DFR" />                
            </div>
            <uc1:ctl_ReturnToQuoteSide runat="server" ID="ctl_ReturnToQuoteSide" />
        </td>
    </tr>
</table>