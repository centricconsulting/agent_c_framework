<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctl_WorkflowManager_App_CPP.ascx.vb" Inherits="IFM.VR.Web.ctl_WorkflowManager_App_CPP" %>

<%@ Register Src="~/User Controls/ctlTreeView.ascx" TagPrefix="uc1" TagName="ctlTreeView" %>
<%@ Register Src="~/User Controls/QuoteEdit/ctlDisplayDiamondRatingErrors.ascx" TagPrefix="uc1" TagName="ctlDisplayDiamondRatingErrors" %>
<%@ Register Src="~/User Controls/Application/ctl_ReturnToQuoteSide.ascx" TagPrefix="uc1" TagName="ctl_ReturnToQuoteSide" %>
<%@ Register Src="~/User Controls/VR Commercial/Application/CPP/ctl_AppSection_CPP.ascx" TagPrefix="uc1" TagName="ctl_AppSection_CPP" %>
<%@ Register Src="~/User Controls/VR Commercial/Quote/CPP/ctl_CPP_QuoteSummary.ascx" TagPrefix="uc1" TagName="ctl_CPP_QuoteSummary" %>


<%@ Register Src="~/User Controls/Application/ctl_AttachmentUpload.ascx" TagPrefix="uc1" TagName="ctl_AttachmentUpload" %>
<%@ Register Src="~/User Controls/VR Commercial/Common/DocumentPrinting/ctlCommercial_DocPrint.ascx" TagPrefix="uc1" TagName="ctlCommercial_DocPrint" %>
<%@ Register Src="~/User Controls/VR Commercial/Quote/CPP/ctl_CPP_PFSummary.ascx" TagPrefix="uc1" TagName="ctl_CPP_PFSummary" %>
<%@ Register Src="~/User Controls/VR Commercial/Application/CPP/ctlCommercialUWQuestionList_CPP.ascx" TagPrefix="uc1" TagName="ctlCommercialUWQuestionList_CPP" %>
<%@ Register Src="~/User Controls/VR Commercial/Common/IRPM_CPP/ctlCommercial_IRPM_CPP.ascx" TagPrefix="uc1" TagName="ctlCommercial_IRPM_CPP" %>






<uc1:ctlDisplayDiamondRatingErrors runat="server" ID="ctlDisplayDiamondRatingErrors" />

<table style="width: 100%;">
    <tr>
        <td style="width: 250px; vertical-align: top;">
            <uc1:ctlTreeView runat="server" ID="ctlTreeView" />
        </td>
        <td style="vertical-align: top;">
            <div id="divAppEditControls" style="display: none;">
                <uc1:ctlCommercialUWQuestionList_CPP runat="server" id="ctlCommercialUWQuestionList_CPP" />
                <uc1:ctl_AppSection_CPP runat="server" id="ctl_AppSection_CPP" />
                <uc1:ctl_CPP_QuoteSummary runat="server" id="ctl_CPP_QuoteSummary" /> 
                <uc1:ctl_CPP_PFSummary runat="server" id="Ctl_CPP_PFSummary"></uc1:ctl_CPP_PFSummary>
                <uc1:ctlCommercial_IRPM_CPP runat="server" ID="ctlCommercial_IRPM_CPP" />
                <uc1:ctlCommercial_DocPrint runat="server" ID="ctlCommercial_DocPrint" />
                <uc1:ctl_AttachmentUpload runat="server" ID="ctl_AttachmentUpload" />
            </div>
            <div class="standardSubSection" id="wf_returnButton" runat="server">
                <div style="float: left; margin-bottom: 4px;">
                    <uc1:ctl_ReturnToQuoteSide runat="server" ID="ctl_ReturnToQuoteSide" />
                </div>
            </div>
        </td>
    </tr>
</table>