<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctl_App_Master_Edit.ascx.vb" Inherits="IFM.VR.Web.ctl_App_Master_Edit" %>

<%@ Register Src="~/User Controls/Application/ctlUWQuestions.ascx" TagPrefix="uc1" TagName="ctlUWQuestions" %>
<%@ Register Src="~/User Controls/QuoteEdit/PPA/ctlQsummary_PPA.ascx" TagPrefix="uc1" TagName="ctlQsummary_PPA" %>
<%@ Register Src="~/User Controls/Application/ctl_ReturnToQuoteSide.ascx" TagPrefix="uc1" TagName="ctl_ReturnToQuoteSide" %>
<%@ Register Src="~/User Controls/QuoteEdit/ctlDisplayDiamondRatingErrors.ascx" TagPrefix="uc1" TagName="ctlDisplayDiamondRatingErrors" %>
<%@ Register Src="~/User Controls/Application/ctl_OrderClueAndOrMVR.ascx" TagPrefix="uc1" TagName="ctl_OrderClueAndOrMVR" %>
<%@ Register Src="~/User Controls/Application/PPA/ctl_App_Section.ascx" TagPrefix="uc1" TagName="ctl_App_Section" %>
<%@ Register Src="~/User Controls/Application/ctl_AttachmentUpload.ascx" TagPrefix="uc1" TagName="ctl_AttachmentUpload" %>
<%@ Register Src="~/User Controls/Application/PPA/ctlUWQuestions_PPA.ascx" TagPrefix="uc1" TagName="ctlUWQuestions_PPA" %>



<div id="divMasterAppEdit" style="display: none;">
    <h3 style="display: none;">Application</h3>
    <div>
        <uc1:ctlDisplayDiamondRatingErrors runat="server" ID="ctlDisplayDiamondRatingErrors" />
        <uc1:ctl_OrderClueAndOrMVR runat="server" ID="ctl_OrderClueAndOrMVR" />
        <uc1:ctlUWQuestions runat="server" ID="ctlUWQuestions" />
        <uc1:ctlUWQuestions_PPA runat="server" id="ctlUWQuestions_PPA" Visible="false"/>
        <uc1:ctl_App_Section runat="server" ID="ctl_App_Section" />
        <uc1:ctl_AttachmentUpload runat="server" ID="ctl_AttachmentUpload" />
        <uc1:ctlQsummary_PPA runat="server" ID="ctlQsummary_PPA" />

        <div class="standardSubSection">
            <div style="float: left;">
                <uc1:ctl_ReturnToQuoteSide runat="server" ID="ctl_ReturnToQuoteSide" />
            </div>
        </div>
    </div>
</div>

<asp:HiddenField ID="hiddenIsActive_MasterEdit" Value="0" runat="server" />