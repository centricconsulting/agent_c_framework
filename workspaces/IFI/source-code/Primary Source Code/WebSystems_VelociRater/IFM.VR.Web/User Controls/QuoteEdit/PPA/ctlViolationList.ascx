<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctlViolationList.ascx.vb" Inherits="IFM.VR.Web.ctlViolationList" %>
<%@ Register Src="~/User Controls/QuoteEdit/PPA/ctlViolationItem.ascx" TagPrefix="uc1" TagName="ctlViolationItem" %>

<div runat="server" id="divViolationList" class="standardSubSection">
    <h3>
        <asp:Label ID="lblHeader" runat="server" Text="Violations"></asp:Label><span style="float: right">
            <asp:LinkButton ID="lnkAdd" CssClass="RemovePanelLink" ToolTip="Add a Violation Item" OnClientClick="StopEventPropagation(event);DisableFormOnSaveRemoves();" runat="server">Add Violation Item</asp:LinkButton>
            <asp:LinkButton ID="lnkBtnSave" CssClass="RemovePanelLink" OnClientClick="StopEventPropagation(event);DisableFormOnSaveRemoves();" ToolTip="Save Page" runat="server">Save</asp:LinkButton>
        </span></h3>
    <div id="divViolations" runat="server">
        <asp:Repeater ID="Repeater1" runat="server">
            <ItemTemplate>
                <uc1:ctlViolationItem runat="server" ID="ctlViolationItem" />
            </ItemTemplate>
        </asp:Repeater>
    </div>
</div>
<asp:HiddenField ID="HiddenField1" runat="server" />
<asp:HiddenField ID="HiddenFieldMainAccord" runat="server" />