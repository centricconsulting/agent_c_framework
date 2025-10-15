<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="VREBillingUpdate.aspx.vb" Inherits="IFM.VR.Web.VREBillingUpdate" %>

<%@ Import Namespace="IFM.VR.Web" %>

<%@ Register Src="~/User Controls/ctlTreeView.ascx" TagPrefix="uc1" TagName="ctlTreeView" %>
<%@ Register Src="~/User Controls/Endorsements/ctl_WorkflowManager_Billing_Update.ascx" TagPrefix="uc1" TagName="ctl_WorkflowManager_Billing_Update" %>

<asp:Content ID="script" ContentPlaceHolderID="cphBodyScripts" runat="server">
    <script src="<%=ResolveClientUrl("~/js/VrApp.js?dt=" + DirectCast(Me.Master, VelociRater).ScriptDT)%>"></script>
</asp:Content>

<asp:Content ID="cntMain" ContentPlaceHolderID="cphMain" runat="server">  
    <table style="width: 100%;">
        <tr>
            <td style="width: 250px; vertical-align: top;">
                <div id="divAboveTree"></div>
                <uc1:ctlTreeView runat="server" ID="ctlTreeView" />
            </td>
            <td style="vertical-align: top;">
                <div id="divBillingUpdateControls">
                    <uc1:ctl_WorkflowManager_Billing_Update runat="server" ID="ctl_WorkflowManager_Billing_Update" />
                </div>
            </td>
        </tr>
    </table>
</asp:Content>

