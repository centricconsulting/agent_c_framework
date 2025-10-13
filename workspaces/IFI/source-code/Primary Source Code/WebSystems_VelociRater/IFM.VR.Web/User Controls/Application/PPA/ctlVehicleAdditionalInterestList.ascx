<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctlVehicleAdditionalInterestList.ascx.vb" Inherits="IFM.VR.Web.ctlVehicleAdditionalInterestList" %>
<%@ Register Src="~/User Controls/Application/PPA/ctlVehicleAdditionalInterest.ascx" TagPrefix="uc1" TagName="ctlVehicleAdditionalInterest" %>
<%@ Register Src="~/User Controls/Application/PPA/ctlRecentlyManufacturedVehicle_APP.ascx" TagPrefix="uc1" TagName="AIPopupControl" %>

<div runat="server" id="divAdditionalInterests" class="standardSubSection">
    <h3>
        <asp:Label ID="lblHeader" runat="server" Text="Additional Interests"></asp:Label>
        <span style="float: right">
            <asp:LinkButton ID="lnkBtnAdd" OnClientClick="StopEventPropagation(event);DisableFormOnSaveRemoves();" ToolTip="Add Additional Interest" CssClass="RemovePanelLink EndoAIClickTarget" runat="server">Add Additional Interest</asp:LinkButton>
            <asp:LinkButton ID="lnkBtnSave" OnClientClick="StopEventPropagation(event);DisableFormOnSaveRemoves();" CssClass="RemovePanelLink" runat="server">Save</asp:LinkButton></span>
    </h3>
    <div>
        <div runat="server" id="divAdditionalInterestItems">
            <asp:Repeater ID="Repeater1" runat="server">
                <ItemTemplate>
                    <uc1:ctlVehicleAdditionalInterest runat="server" ID="ctlVehicleAdditionalInterest" />
                </ItemTemplate>
            </asp:Repeater>
        </div>
        <div id="divEndorsementMaxTransactionsMessage" runat="server" style="width: 100%; text-align: center;font-weight:bold;color:blue;" visible="false">
            <br />Only 3 additional interests can be added or deleted per transaction, contact your underwriter for changes involving more than 3.
        </div>
        <div id="divEndorsementButtons" runat="server" visible="false">
            <br /><asp:Button ID="btnAddAdditionalInterest" runat="server" CssClass="StandardSaveButton" Text="Add Additional Interest"/>
        </div>
    </div>
</div>
<uc1:AIPopupControl ID="ctlAIPopup" runat="server"/>
<asp:HiddenField ID="hiddenAdditionalInterest" runat="server" />
<asp:HiddenField ID="hiddenAdditionalInterestItems" runat="server" />