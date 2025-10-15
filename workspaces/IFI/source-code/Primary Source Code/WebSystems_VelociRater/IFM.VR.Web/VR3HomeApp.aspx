<%@ Page Language="vb" EnableEventValidation="false" AutoEventWireup="false" CodeBehind="VR3HomeApp.aspx.vb" Inherits="IFM.VR.Web.VR3HomeApp" MasterPageFile="~/VelociRater.Master" %>

<%@ Import Namespace="IFM.VR.Web" %>
<%@ Register Src="~/User Controls/ctlTreeView.ascx" TagPrefix="uc1" TagName="ctlTreeView" %>
<%@ Register Src="~/User Controls/Application/HOM/ctl_Master_HOM_APP.ascx" TagPrefix="uc1" TagName="ctl_Master_HOM_APP" %>

<asp:Content ID="script" ContentPlaceHolderID="cphBodyScripts" runat="server">
    <script src="<%=ResolveClientUrl("~/js/VrAdditionalInterestSearch.js?dt=" + DirectCast(Me.Master, VelociRater).ScriptDT)%>"></script>
    <script src="<%=ResolveClientUrl("~/js/VrApp.js?dt=" + DirectCast(Me.Master, VelociRater).ScriptDT)%>"></script>
    <script src="<%=ResolveClientUrl("~/js/VrHomeLine.js?dt=" + DirectCast(Me.Master, VelociRater).ScriptDT)%>"></script>
    <script src="<%=ResolveClientUrl("~/js/VrAllLines.js?dt=" + DirectCast(Me.Master, VelociRater).ScriptDT)%>"></script>
    <script>
        $(document).ready(function () {
            $("#tblVR3HomeApp_Main").fadeIn();
        });
    </script>
</asp:Content>

<asp:Content ID="cntMain" ContentPlaceHolderID="cphMain" runat="server">

    <table id="tblVR3HomeApp_Main" style="width: 100%; display: none;">
        <tr>
            <td style="width: 250px; vertical-align: top;">
                <uc1:ctlTreeView runat="server" ID="ctlTreeView" />
            </td>
            <td style="vertical-align: top;">
                <div>
                    <asp:Panel ID="pnlMain" runat="server">
                        <uc1:ctl_Master_HOM_APP runat="server" ID="ctl_Master_HOM_APP" />
                    </asp:Panel>
                </div>
            </td>
        </tr>
    </table>

    <asp:Label ID="lblMsg" runat="server" ForeColor="Red" Font-Bold="true" Text="&nbsp;"></asp:Label>
</asp:Content>