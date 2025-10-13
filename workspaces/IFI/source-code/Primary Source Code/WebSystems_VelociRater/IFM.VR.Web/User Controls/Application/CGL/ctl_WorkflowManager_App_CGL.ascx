<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctl_WorkflowManager_App_CGL.ascx.vb" Inherits="IFM.VR.Web.ctl_WorkflowManager_App_CGL" %>

<%@ Register Src="~/User Controls/ctlTreeView.ascx" TagPrefix="uc1" TagName="ctlTreeView" %>
<%@ Register Src="~/User Controls/QuoteEdit/ctlDisplayDiamondRatingErrors.ascx" TagPrefix="uc1" TagName="ctlDisplayDiamondRatingErrors" %>
<%@ Register Src="~/User Controls/Application/ctl_ReturnToQuoteSide.ascx" TagPrefix="uc1" TagName="ctl_ReturnToQuoteSide" %>
<%@ Register Src="~/User Controls/Application/CGL/ctl_AppSection_CGL.ascx" TagPrefix="uc1" TagName="ctl_AppSection_CGL" %>
<%@ Register Src="~/User Controls/QuoteEdit/CGL/ctl_Summary_CGL.ascx" TagPrefix="uc1" TagName="ctl_Summary_CGL" %>



<uc1:ctlDisplayDiamondRatingErrors runat="server" ID="ctlDisplayDiamondRatingErrors" />

<table style="width: 100%;">
    <tr>
        <td style="width: 250px; vertical-align: top;">
            <uc1:ctlTreeView runat="server" ID="ctlTreeView" />
        </td>
        <td style="vertical-align: top;">
            <div id="divAppEditControls" style="display: none;">
                <uc1:ctl_AppSection_CGL runat="server" id="ctl_AppSection_CGL" />
                <uc1:ctl_Summary_CGL runat="server" id="ctl_Summary_CGL" />                
            </div>
            <uc1:ctl_ReturnToQuoteSide runat="server" ID="ctl_ReturnToQuoteSide" />
        </td>
    </tr>
</table>