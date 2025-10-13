<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctl_WorkflowManager_Billing_Update.ascx.vb" Inherits="IFM.VR.Web.ctl_WorkflowManager_Billing_Update" %>
<%@ Register Src="~/User Controls/ctlEffectiveDateChecker.ascx" TagPrefix="uc1" TagName="ctlEffectiveDateChecker" %>
<%@ Register Src="~/User Controls/QuoteEdit/ctlDisplayDiamondRatingErrors.ascx" TagPrefix="uc1" TagName="ctlDisplayDiamondRatingErrors" %>
<%@ Register Src="~/User Controls/Endorsements/ctlBillingInfo.ascx" TagPrefix="uc1" TagName="ctlBillingInfo" %>
<%@ Register Src="~/User Controls/Endorsements/ctlBillingChangeSummary.ascx" TagPrefix="uc1" TagName="ctlBillingChangeSummary" %>
<%@ Register Src="~/User Controls/Application/ctl_AttachmentUpload.ascx" TagPrefix="uc1" TagName="ctl_AttachmentUpload" %>


<uc1:ctlDisplayDiamondRatingErrors runat="server" ID="ctlDisplayDiamondRatingErrors" />
<uc1:ctlEffectiveDateChecker runat="server" ID="ctlEffectiveDateChecker" />

<div id="BillingUpdate" style="display: none;">
    <uc1:ctlBillingInfo runat="server" ID="ctlBillingInfo" />
    <uc1:ctlBillingChangeSummary runat="server" id="ctlBillingChangeSummary" />
    <uc1:ctl_AttachmentUpload runat="server" ID="ctl_AttachmentUpload" />
</div>