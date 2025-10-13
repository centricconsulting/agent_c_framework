<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctlDriverList.ascx.vb" Inherits="IFM.VR.Web.ctlDriverList" %>
<%@ Register Src="~/User Controls/QuoteEdit/PPA/ctlDriver_PPA.ascx" TagPrefix="uc1" TagName="ctlDriver_PPA" %>
<%@ Register Src="~/User Controls/QuoteEdit/PPA/ctlLossHistoryGeneric.ascx" TagPrefix="uc1" TagName="ctlLossHistoryGeneric" %>


<div id="DriverListControlDivTopMost" runat="server">
    <asp:Panel ID="Panel1" runat="server" DefaultButton="btnSubmit">
        <div id="divDriver" runat="server">
            <asp:Repeater ID="Repeater1" runat="server">
                <ItemTemplate>
                    <uc1:ctlDriver_PPA runat="server" ID="ctlDriver_PPAControl" />
                </ItemTemplate>
            </asp:Repeater>
        </div>
        <uc1:ctlLossHistoryGeneric runat="server" id="ctlLossHistoryGeneric" />
        <div id="divActionButtons" runat="server" style="margin-top: 20px; width: 100%; text-align: center;">
            <div style="float: left;">
                <asp:Button ID="bnAddDriver" ToolTip="Add a Driver" OnClientClick="DisableFormOnSaveRemoves();" runat="server" CssClass="StandardSaveButton" Text="Add Driver" />
            </div>

            <asp:Button ID="btnSubmit" ToolTip="Save all Driver Information" OnClientClick="DisableFormOnSaveRemoves();" runat="server" CssClass="StandardSaveButton" Text="Save Drivers" />
            <asp:Button ID="btnSaveAndGotoVehicles" ToolTip="Save all Driver Information and Navigate to the Vehicles Page" OnClientClick="DisableFormOnSaveRemoves();" runat="server" CssClass="StandardSaveButton" Text="Vehicles Page" />
            <asp:Button ID="btnRateDriver" runat="server" OnClientClick="DisableFormOnSaveRemoves();" CssClass="StandardSaveButton" ToolTip="Rate Quote" Text="Rate this Quote"/>
        </div>
        <div id="divEndorsementButtons" runat="server" class="standardSubSection center" style="width: 100%; text-align: center;" visible="false">
            <asp:Button ID="btnMakeAChange" runat="server" OnClientClick="DisableFormOnSaveRemoves();" CssClass="StandardSaveButton" ToolTip="Make a Change to this policy" Text="Make A Change"/>
            <asp:Button ID="btnViewGotoVehicles" OnClientClick="DisableFormOnSaveRemoves();" CssClass="StandardSaveButton" runat="server" Text="Vehicles Page" />
        </div>
    </asp:Panel>
</div>
<asp:HiddenField ID="hiddenActiveDriver" runat="server" />