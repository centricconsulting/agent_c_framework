<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctlFarmLocation.ascx.vb" Inherits="IFM.VR.Web.ctlFarmLocation" %>
<%@ Register Src="~/User Controls/QuoteEdit/HOM/Property/ctlProperty_Address.ascx" TagPrefix="uc1" TagName="ctlProperty_Address" %>
<%@ Register Src="~/User Controls/QuoteEdit/HOM/Property/ctlResidence.ascx" TagPrefix="uc1" TagName="ctlResidence" %>
<%@ Register Src="~/User Controls/QuoteEdit/HOM/Property/ctlPropertyAdditionalQuestions.ascx" TagPrefix="uc1" TagName="ctlPropertyAdditionalQuestions" %>
<%@ Register Src="~/User Controls/QuoteEdit/FAR/FarmBuildingControls/ctl_FarBuildingList.ascx" TagPrefix="uc1" TagName="ctl_FarBuildingList" %>
<%@ Register Src="~/User Controls/QuoteEdit/FAR/LocationControls/ctlFARPersonalLiabilityGL9.ascx" TagPrefix="uc1" TagName="ctlFarmPersonalLiabilityGL9" %>

    <h3>
        <asp:Table runat="server" Width="100%" CssClass="tableBorder">
            <asp:TableRow>
                <asp:TableCell>
                    <asp:Label ID="lblMainHeader" runat="server" CssClass="labelFrmtEllips"></asp:Label>
                </asp:TableCell>
                <asp:TableCell HorizontalAlign="Right">
                    <asp:LinkButton ID="lnkNewLocation" runat="server" ToolTip="Add New Location" CssClass="RemovePanelLink">Add New</asp:LinkButton>
                    <asp:LinkButton ID="lnkDeleteLocation" runat="server" OnClick="OnConfirm" OnClientClick="ConfirmDialog('Location')" ToolTip="Delete Location" CssClass="RemovePanelLink" Visible="false">Delete</asp:LinkButton>
                    <asp:LinkButton ID="lnkClearLocation" runat="server" ToolTip="Clear Location" CssClass="RemovePanelLink">Clear</asp:LinkButton>
                    <asp:LinkButton ID="lnkSaveLocation" runat="server" ToolTip="Save Location" CssClass="RemovePanelLink">Save</asp:LinkButton>
                </asp:TableCell>
            </asp:TableRow>
        </asp:Table>
    </h3>
    <div id="farmLocationDataContainer" runat="server">
        <uc1:ctlPropertyAdditionalQuestions runat="server" ID="ctlPropertyAdditionalQuestions" />
        <uc1:ctlProperty_Address runat="server" ID="ctlProperty_Address" />
        <uc1:ctlFarmPersonalLiabilityGL9 runat="server" ID="ctlPersLiabGL9" />
        <uc1:ctlResidence runat="server" ID="ctlResidence" />
        <div id="dvInitRes" runat="server">
            <h3 id="h3InitResidence" runat="server">
                <asp:Label ID="lblInitResidenceHdr" runat="server" Text="Dwelling"></asp:Label>
                <span style="float: right;">
                    <asp:LinkButton ID="lnkAddNewResidence" runat="server" ToolTip="Add New Dwelling" CssClass="RemovePanelLink">Add Dwelling</asp:LinkButton>
                </span>
            </h3>
        </div>
        <asp:HiddenField ID="hiddenActiveResidence" runat="server" />

        <uc1:ctl_FarBuildingList runat="server" ID="ctl_FarBuildingList" />

        <div style="width: 100%; text-align: center; margin-top: 10px;">
            <div style="float: left;">
                <asp:Button ID="btnAddLocation" ToolTip="Add a new Location" runat="server" OnClientClick="DisableFormOnSaveRemoves();StopEventPropagation(event);" CssClass="StandardSaveButton" Text="Add Another Location" />
            </div>
            <asp:Button ID="btnSubmit" ToolTip="Save All Location Information" OnClientClick="DisableFormOnSaveRemoves();" runat="server" CssClass="StandardSaveButton" Text="Save Location" />
        </div>
        <asp:HiddenField ID="hiddenLocation" runat="server" />
    </div>
