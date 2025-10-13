<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctl_CAP_DriverList.ascx.vb" Inherits="IFM.VR.Web.ctl_CAP_DriverList" %>
<%@ Register src="~/User Controls/VR Commercial/Application/CAP/ctl_CAP_Driver.ascx" TagPrefix="uc1" TagName="ctl_Driver" %>

    <div id="divMainList" runat="server">
        <h3 id="hdrDrivers" runat="server">
            <asp:Label ID="lblAccordHeader" runat="server" Text="Drivers (0)"></asp:Label>
                <span style="float: right;">
                <asp:LinkButton ID="lnkNew" runat="server" CssClass="RemovePanelLink" Style="margin-left: 20px" ToolTip="Add a Driver">Add Driver</asp:LinkButton>
                <asp:LinkButton ID="lnkSave" ToolTip="Save" runat="server" CssClass="RemovePanelLink">Save</asp:LinkButton>
            </span>
        </h3>
        <div>
            <div id="divList" runat="server">
                <asp:Repeater ID="Repeater1" runat="server">
                    <ItemTemplate>
                        <uc1:ctl_Driver runat="server" id="ctl_CAP_Driver" />
                    </ItemTemplate>
                </asp:Repeater>
            </div>            
        <asp:HiddenField ID="hdnAccordList" runat="server" />
        </div>        
        <asp:HiddenField ID="hdnAccord" runat="server" />
    </div>
    <div id="divEndorsementMaxTransactionsMessage" runat="server" class="" style="width: 100%; text-align: center;font-weight:bold;color:blue;" visible="false">
        <br />Only 3 drivers can be added or deleted per transaction, contact your underwriter for changes involving more than 3.
    </div>
    <div id="divActionButtons" runat="server" style="margin-top: 20px; width: 100%; text-align:center;">
        <asp:Button ID="btnAddDriver" runat="server" Text="Add Driver" CssClass="StandardSaveButton" />
        <asp:Button ID="btnSaveVehicles" runat="server" Text="Save Drivers" CssClass="StandardSaveButton" />
        <asp:Button ID="btnVehicles" runat="server" Text="Vehicles Page" CssClass="StandardSaveButton" />
        <asp:Button ID="btnSaveAndRate" runat="server" Text="Rate This Quote" CssClass="StandardSaveButton" />
    </div>
    <div id="divEndorsementButtons" runat="server" class="standardSubSection center" style="width: 100%; text-align: center;" visible="false">
        <asp:Button ID="btnMakeAChange" runat="server" OnClientClick="DisableFormOnSaveRemoves();" CssClass="StandardSaveButton" ToolTip="Make a Change to this policy" Text="Make A Change"/>
        <asp:Button ID="btnViewVehicles" OnClientClick="DisableFormOnSaveRemoves();" CssClass="StandardSaveButton" runat="server" Text="Vehicles Page" />
    </div>