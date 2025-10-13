<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctlRV_WatercraftList.ascx.vb" Inherits="IFM.VR.Web.ctlRV_WatercraftList" %>
<%@ Register Src="~/User Controls/RV-Watercrafts/ctlRV_Watercraft.ascx" TagPrefix="uc1" TagName="ctlRV_Watercraft" %>

<div id="dvRVWatercraftList" runat="server">
    <asp:Repeater ID="rvWaterRepeater" runat="server">
        <ItemTemplate>
            <uc1:ctlRV_Watercraft runat="server" ID="ctlRV_Watercraft" />
        </ItemTemplate>
    </asp:Repeater>
</div>
<asp:HiddenField ID="hiddenActiveRVWatercraft" runat="server" />