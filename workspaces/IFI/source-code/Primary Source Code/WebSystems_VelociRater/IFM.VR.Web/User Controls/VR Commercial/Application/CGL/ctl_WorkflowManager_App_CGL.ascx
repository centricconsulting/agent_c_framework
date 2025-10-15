<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctl_WorkflowManager_App_CGL.ascx.vb" Inherits="IFM.VR.Web.ctl_WorkflowManager_App_CGL" %>

<%@ Register Src="~/User Controls/ctlTreeView.ascx" TagPrefix="uc1" TagName="ctlTreeView" %>
<%@ Register Src="~/User Controls/QuoteEdit/ctlDisplayDiamondRatingErrors.ascx" TagPrefix="uc1" TagName="ctlDisplayDiamondRatingErrors" %>
<%@ Register Src="~/User Controls/Application/ctl_ReturnToQuoteSide.ascx" TagPrefix="uc1" TagName="ctl_ReturnToQuoteSide" %>
<%@ Register Src="~/User Controls/VR Commercial/Application/CGL/ctl_AppSection_CGL.ascx" TagPrefix="uc1" TagName="ctl_AppSection_CGL" %>
<%@ Register Src="~/User Controls/VR Commercial/Quote/CGL/ctl_CGL_QuoteSummary.ascx" TagPrefix="uc1" TagName="ctl_CGL_QuoteSummary" %>


<%@ Register Src="~/User Controls/Application/ctl_AttachmentUpload.ascx" TagPrefix="uc1" TagName="ctl_AttachmentUpload" %>
<%@ Register Src="~/User Controls/VR Commercial/Common/IRPM/ctlCommercial_IRPM.ascx" TagPrefix="uc1" TagName="ctlCommercial_IRPM" %>
<%@ Register Src="~/User Controls/VR Commercial/Common/DocumentPrinting/ctlCommercial_DocPrint.ascx" TagPrefix="uc1" TagName="ctlCommercial_DocPrint" %>
<%@ Register Src="~/User Controls/VR Commercial/Quote/CGL/ctl_CGL_PFSummary.ascx" TagPrefix="uc1" TagName="ctl_CGL_PFSummary" %>

<%@ Register Src="~/User Controls/VR Commercial/Application/BOP/ctlCommercialUWQuestionList.ascx" TagPrefix="uc1" TagName="ctlCommercialUWQuestionList" %>



<uc1:ctlDisplayDiamondRatingErrors runat="server" ID="ctlDisplayDiamondRatingErrors" />

<table style="width: 100%;">
    <tr>
        <td style="width: 250px; vertical-align: top;">
            <uc1:ctlTreeView runat="server" ID="ctlTreeView" />
        </td>
        <td style="vertical-align: top;">
            <div id="divAppEditControls" style="display: none;">
                <uc1:ctlCommercialUWQuestionList runat="server" id="ctlCommercialUWQuestionList" />
                <uc1:ctl_AppSection_CGL runat="server" id="ctl_AppSection_CGL" />
                <uc1:ctl_CGL_QuoteSummary runat="server" id="ctl_CGL_QuoteSummary" /> 
                <uc1:ctl_CGL_PFSummary runat="server" id="Ctl_CGL_PFSummary"></uc1:ctl_CGL_PFSummary>
                <uc1:ctlCommercial_IRPM runat="server" id="ctlCommercial_IRPM" />
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