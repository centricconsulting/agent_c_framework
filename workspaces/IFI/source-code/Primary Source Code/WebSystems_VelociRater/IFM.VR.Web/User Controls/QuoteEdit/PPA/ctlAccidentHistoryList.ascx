<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctlAccidentHistoryList.ascx.vb" Inherits="IFM.VR.Web.ctlAccidentHistoryList" %>
<%@ Register Src="~/User Controls/QuoteEdit/PPA/ctlAccidentHistoryItem.ascx" TagPrefix="uc1" TagName="ctlAccidentHistoryItem" %>

<div runat="server" id="divAccidetHistoryList" class="standardSubSection">
    <h3>
        <asp:Label ID="lblHeader" runat="server" Text="Loss History"></asp:Label>

        <span style="float: right">
            <asp:LinkButton ID="lnkAdd" CssClass="RemovePanelLink" ToolTip="Add a Loss History Item" runat="server">Add Loss Item</asp:LinkButton>
            <asp:LinkButton ID="lnkSave" CssClass="RemovePanelLink" ToolTip="Save Page" runat="server">Save</asp:LinkButton>
        </span>
    </h3>

    <div id="divLossHistories" runat="server">
        <asp:Repeater ID="Repeater1" runat="server">
            <ItemTemplate>
                <uc1:ctlAccidentHistoryItem runat="server" ID="ctlAccidentHistoryItem" />
            </ItemTemplate>
        </asp:Repeater>
    </div>
</div>
<div runat="server" id="divHOMClueLink" style="width: 90%; margin-bottom: 15px;">
    <asp:LinkButton ID="lnkClueReport" Style="margin-bottom: 10px;" ToolTip="View CLUE Report" runat="server">View CLUE Report</asp:LinkButton>
</div>

<asp:HiddenField ID="HiddenFieldMainAccord" Value="0" runat="server" />
<asp:HiddenField ID="HiddenField1" Value="0" runat="server" />