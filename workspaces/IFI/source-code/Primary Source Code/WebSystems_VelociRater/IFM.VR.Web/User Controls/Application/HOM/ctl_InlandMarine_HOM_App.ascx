<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctl_InlandMarine_HOM_App.ascx.vb" Inherits="IFM.VR.Web.ctl_InlandMarine_HOM_App" %>
<%@ Register Src="~/User Controls/InlandMarine/ctlInlandMarine.ascx" TagPrefix="uc1" TagName="ctlInlandMarine" %>


<%--<%@ Register Src="~/User Controls/QuoteEdit/HOM/ctlInlandMarineItem.ascx" TagPrefix="uc1" TagName="ctlInlandMarineItem" %>--%>

<div id="divInlandMarine" runat="server">
    <h3>
        <asp:Label ID="lblInlandMarineHdr" runat="server" Text="INLAND MARINE"></asp:Label>
        <asp:Label ID="lblIMChosen" runat="server" Text="(0)"></asp:Label>
        <span style="float: right;">
            <asp:LinkButton ID="lnkClearInland" runat="server" CssClass="RemovePanelLink" ToolTip="Clear Inland Marine">Clear</asp:LinkButton>
            <asp:LinkButton ID="lnkSaveinland" runat="server" CssClass="RemovePanelLink" ToolTip="Save Page">Save</asp:LinkButton>
        </span>
    </h3>
    <div>
        <uc1:ctlInlandMarine runat="server" ID="ctlInlandMarine" />
        <%--<uc1:ctlInlandMarineItem runat="server" ID="ctlInlandMarineItem" />--%>
        <asp:HiddenField ID="hiddenActiveInlandMarine" runat="server" />
    </div>
</div>