<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctl_Master_HOM_APP.ascx.vb" Inherits="IFM.VR.Web.ctl_Master_HOM_APP" %>
<%@ Register Src="~/User Controls/Application/ctlUWQuestions.ascx" TagPrefix="uc1" TagName="ctlUWQuestions" %>
<%@ Register Src="~/User Controls/Application/HOM/ctl_HOM_App_Section.ascx" TagPrefix="uc1" TagName="ctl_HOM_App_Section" %>
<%@ Register Src="~/User Controls/Application/ctl_ReturnToQuoteSide.ascx" TagPrefix="uc1" TagName="ctl_ReturnToQuoteSide" %>
<%@ Register Src="~/User Controls/QuoteEdit/HOM/ctlQuoteSummary_HOM.ascx" TagPrefix="uc1" TagName="ctlQuoteSummary_HOM" %>
<%@ Register Src="~/User Controls/Application/ctl_OrderClueAndOrMVR.ascx" TagPrefix="uc1" TagName="ctl_OrderClueAndOrMVR" %>
<%@ Register Src="~/User Controls/QuoteEdit/ctlDisplayDiamondRatingErrors.ascx" TagPrefix="uc1" TagName="ctlDisplayDiamondRatingErrors" %>
<%@ Register Src="~/User Controls/Application/ctl_AttachmentUpload.ascx" TagPrefix="uc1" TagName="ctl_AttachmentUpload" %>


<uc1:ctlDisplayDiamondRatingErrors runat="server" ID="ctlDisplayDiamondRatingErrors" />
<uc1:ctl_OrderClueAndOrMVR runat="server" ID="ctl_OrderClueAndOrMVR" />
<uc1:ctlUWQuestions runat="server" ID="ctlUWQuestions" />
<uc1:ctl_AttachmentUpload runat="server" ID="ctl_AttachmentUpload" />

<uc1:ctl_HOM_App_Section runat="server" ID="ctl_HOM_App_Section" />
<uc1:ctlQuoteSummary_HOM runat="server" ID="ctlQuoteSummary_HOM" />

<uc1:ctl_ReturnToQuoteSide runat="server" ID="ctl_ReturnToQuoteSide" />