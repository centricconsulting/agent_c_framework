<%@ Page Language="vb" EnableEventValidation="false" AutoEventWireup="false" MasterPageFile="~/VelociRater.Master" CodeBehind="VR3AutoApp.aspx.vb" Inherits="IFM.VR.Web.VR3AutoApp" %>

<%@ Import Namespace="IFM.VR.Web" %>

<%@ Register Src="~/User Controls/ctlTreeView.ascx" TagPrefix="uc1" TagName="ctlTreeView" %>
<%@ Register Src="~/User Controls/Application/PPA/ctl_App_Master_Edit.ascx" TagPrefix="uc1" TagName="ctl_App_Master_Edit" %>

<asp:Content ID="script" ContentPlaceHolderID="cphBodyScripts" runat="server">
    <script src="<%=ResolveClientUrl("~/js/VrAdditionalInterestSearch.js?dt=" + DirectCast(Me.Master, VelociRater).ScriptDT)%>"></script>
    <script src="<%=ResolveClientUrl("~/js/VrAutoLine.js?dt=" + DirectCast(Me.Master, VelociRater).ScriptDT)%>"></script>
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
                <div id="divAppEditControls">
                    <uc1:ctl_App_Master_Edit runat="server" ID="ctl_App_Master_Edit" />
                </div>
            </td>
        </tr>
    </table>
</asp:Content>