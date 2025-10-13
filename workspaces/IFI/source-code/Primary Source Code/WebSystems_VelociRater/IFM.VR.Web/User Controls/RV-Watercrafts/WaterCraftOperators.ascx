<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="WaterCraftOperators.ascx.vb" Inherits="IFM.VR.Web.WaterCraftOperators" %>
<%@ Register Src="~/User Controls/RV-Watercrafts/WaterCraftOperatorItem.ascx" TagPrefix="uc1" TagName="WaterCraftOperatorItem" %>


<div runat="server" id="divOperatorsList" >
    <h3>
        <asp:Label ID="lblHeader" runat="server" Text="Violations"></asp:Label><span style="float: right">
            <asp:LinkButton ID="lnkAdd" CssClass="RemovePanelLink" ToolTip="Add a Violation Item" OnClientClick="StopEventPropagation(event);DisableFormOnSaveRemoves();" runat="server">Add Operator</asp:LinkButton>
            <asp:LinkButton ID="lnkBtnSave" CssClass="RemovePanelLink" OnClientClick="StopEventPropagation(event);DisableFormOnSaveRemoves();" ToolTip="Save Page" runat="server">Save</asp:LinkButton>
        </span></h3>
    <div id="divViolations" runat="server">
        <asp:Repeater ID="Repeater1" runat="server">
            <ItemTemplate>
                <uc1:WaterCraftOperatorItem runat="server" ID="WaterCraftOperatorItem" />
            </ItemTemplate>
        </asp:Repeater>
    </div>
</div>
<asp:HiddenField ID="HiddenField1" runat="server" />
<asp:HiddenField ID="HiddenFieldMainAccord" runat="server" />