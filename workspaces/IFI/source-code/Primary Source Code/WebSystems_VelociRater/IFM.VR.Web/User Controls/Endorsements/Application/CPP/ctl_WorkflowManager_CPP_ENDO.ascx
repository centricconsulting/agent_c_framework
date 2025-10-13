<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctl_WorkflowManager_CPP_ENDO.ascx.vb" Inherits="IFM.VR.Web.ctl_WorkflowManager_CPP_Endo" %>
<%@ Register Src="~/User Controls/ctlEffectiveDateChecker.ascx" TagPrefix="uc1" TagName="ctlEffectiveDateChecker" %>
<%@ Register Src="~/User Controls/QuoteEdit/ctlRiskGradeSearch.ascx" TagPrefix="uc1" TagName="ctlRiskGradeSearch" %>
<%@ Register Src="~/User Controls/QuoteEdit/ctlDisplayDiamondRatingErrors.ascx" TagPrefix="uc1" TagName="ctlDisplayDiamondRatingErrors" %>
<%@ Register Src="~/User Controls/ctlTreeView.ascx" TagPrefix="uc1" TagName="ctlTreeView" %>
<%@ Register Src="~/User Controls/QuoteEdit/ctlIsuredList.ascx" TagPrefix="uc1" TagName="ctlIsuredList" %>
<%@ Register Src="~/User Controls/ctlProposalSelection.ascx" TagPrefix="uc1" TagName="ctlProposalSelection" %>
<%@ Register Src="~/User Controls/VR Commercial/Quote/CPP/ctl_CPP_QuoteSummary.ascx" TagPrefix="uc1" TagName="ctl_CPP_QuoteSummary" %>
<%@ Register Src="~/User Controls/VR Commercial/Quote/CPP/ctl_CPP_PFSummary.ascx" TagPrefix="uc1" TagName="ctl_CPP_PFSummary" %>
<%@ Register Src="~/User Controls/Endorsements/Application/CPP/Locations/ctl_CPP_ENDO_Liability_LocationList.ascx" TagPrefix="uc1" TagName="ctl_CPP_ENDO_Liability_LocationList" %>
<%@ Register Src="~/User Controls/Endorsements/Application/CPP/ctl_CPP_ENDO_InlandMarine.ascx" TagPrefix="uc1" TagName="ctl_CPP_ENDO_InlandMarine" %>
<%@ Register Src="~/User Controls/Endorsements/Application/CPP/Locations/ctl_CPR_ENDO_LocationList.ascx" TagPrefix="uc1" TagName="ctl_CPR_ENDO_LocationList" %>
<%@ Register Src="~/User Controls/Application/ctl_AttachmentUpload.ascx" TagPrefix="uc1" TagName="ctl_AttachmentUpload" %>















<uc1:ctlEffectiveDateChecker runat="server" ID="ctlEffectiveDateChecker" />
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
                <uc1:ctl_CPR_ENDO_LocationList runat="server" id="ctl_CPR_ENDO_LocationList" />
                <uc1:ctl_CPP_ENDO_Liability_LocationList runat="server" id="ctl_CPP_ENDO_Liability_LocationList" />
                <uc1:ctl_CPP_ENDO_InlandMarine runat="server" id="ctl_CPP_ENDO_InlandMarine" />
                <%--<uc1:ctl_BOP_ENDO_PolicyLevelCoverages runat="server" ID="ctl_BOP_ENDO_PolicyLevelCoverages" />--%>
                <uc1:ctl_CPP_QuoteSummary runat="server" ID="ctl_CPP_QuoteSummary" />
                <uc1:ctl_CPP_PFSummary runat="server" ID="ctl_CPP_PFSummary" />
                <uc1:ctlProposalSelection runat="server" ID="ctlProposalSelection" />
                <uc1:ctl_AttachmentUpload runat="server" ID="ctl_AttachmentUpload" />
            </div>
        </td>
    </tr>
</table>