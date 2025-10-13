<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctlIMRVWatercraft.ascx.vb" Inherits="IFM.VR.Web.ctlIMRVWatercraft" %>
<%@ Register Src="~/User Controls/InlandMarine/ctlInlandMarine.ascx" TagPrefix="uc1" TagName="ctlInlandMarine" %>
<%@ Register Src="~/User Controls/RV-Watercrafts/ctlRV_WatercraftList.ascx" TagPrefix="uc1" TagName="ctlRV_WatercraftList" %>
<%@ Register Src="~/User Controls/RV-Watercrafts/ctlYoungestOperator.ascx" TagPrefix="uc1" TagName="ctlYoungestOperator" %>


<uc1:ctlInlandMarine runat="server" ID="ctlInlandMarine" />
<div id="dvInitRVWatercraft" runat="server" class="standardSubSection" onkeydown="return (event.keyCode!=13)">
    <h3 id="h3RVWaterHdr" runat="server">
        <asp:Label ID="lblRVWatercraftHdr" runat="server" Text="RV / WATERCRAFT"></asp:Label>
        <span style="float: right;">
            <asp:LinkButton ID="lnkAddRVWater" CssClass="RemovePanelLink" ToolTip="Add New RV/Watercraft" runat="server">Add RV/WC</asp:LinkButton>
        </span>
    </h3>
</div>
<uc1:ctlRV_WatercraftList runat="server" ID="ctlRV_WatercraftList" />
<br />
<div id="dvYoungestOperator" runat="server"><uc1:ctlYoungestOperator runat="server" ID="ctlYoungestOperator" /></div>
<br />
<asp:Label ID="lblUnavailable" runat="server" Text="Inland Marine and RV/Watercraft not available for Commercial Farm quotes without Personal Liability Coverage GL-9" Font-Bold="True"></asp:Label>

<asp:HiddenField ID="hiddenRVWatercraft" runat="server" />

<div id="divEndorsementButtons" runat="server" class="standardSubSection center" style="width: 100%; text-align: center;" visible="false">
    <asp:Button ID="btnMakeAChange" runat="server" OnClientClick="DisableFormOnSaveRemoves();" CssClass="StandardSaveButton" ToolTip="Make a Change to this policy" Text="Make A Change" />
    <asp:Button ID="btnBillingInfo" runat="server" Text="Billing Information Page" OnClientClick="DisableFormOnSaveRemoves();"  CssClass="StandardSaveButton" ToolTip="Billing Information Page" />
</div>