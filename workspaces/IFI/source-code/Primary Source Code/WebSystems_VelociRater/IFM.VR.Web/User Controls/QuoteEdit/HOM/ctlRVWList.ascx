<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctlRVWList.ascx.vb" Inherits="IFM.VR.Web.ctlRVWList" %>
<%@ Register Src="~/User Controls/QuoteEdit/HOM/ctlRVWItem.ascx" TagPrefix="uc1" TagName="ctlRVWItem" %>

<div id="divRVWatercraftList" runat="server">
    <div id="divRVWatercraftListItems">
        <asp:Repeater ID="rptRVW" runat="server">
            <ItemTemplate>
                <uc1:ctlRVWItem runat="server" ID="ctlRVWItem" />
            </ItemTemplate>
        </asp:Repeater>
    </div>
    <div style="width: 100%; text-align: center;">
        <asp:HiddenField ID="hiddenActiveRVW" Value="0" runat="server" />
    </div>
</div>