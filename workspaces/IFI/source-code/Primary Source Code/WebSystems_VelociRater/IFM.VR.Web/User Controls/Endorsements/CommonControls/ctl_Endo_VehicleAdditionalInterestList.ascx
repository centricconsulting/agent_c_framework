<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctl_Endo_VehicleAdditionalInterestList.ascx.vb" Inherits="IFM.VR.Web.ctl_Endo_VehicleAdditionalInterestList" %>
<%@ Register Src="~/User Controls/Endorsements/CommonControls/ctl_Endo_VehicleAdditionalInterest.ascx" TagPrefix="uc1" TagName="ctl_Endo_VehicleAdditionalInterest" %>



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
                    <uc1:ctl_Endo_VehicleAdditionalInterest runat="server" ID="ctl_Endo_VehicleAdditionalInterest" />
                </ItemTemplate>
            </asp:Repeater>
        </div>
        <div id="divEndorsementMaxTransactionsMessage" runat="server" style="width: 100%; text-align: center; font-weight: bold; color: blue;" visible="false">
            <br />
            Only 3 additional interests can be added or deleted per transaction, contact your underwriter for changes involving more than 3.
        </div>
        <div id="divEndorsementButtons" runat="server" visible="false">
            <asp:Button ID="btnAddAdditionalInterest" runat="server" CssClass="StandardSaveButton" Text="Add Additional Interest" />
        </div>
    </div>

</div>
<%--<div id="divEndorsementMaxTransactionsMessage" runat="server" style="width: 100%; text-align: center;font-weight:bold;color:blue;" visible="false">
    <br />Only 3 additional interests can be added or deleted per transaction, contact your underwriter for changes involving more than 3.
</div>
<div id="divEndorsementButtons" runat="server" visible="false">
    <asp:Button ID="btnAddAdditionalInterest" runat="server" CssClass="StandardSaveButton" Text="Add Additional Interest"/>
</div>--%>
<asp:HiddenField ID="hiddenAdditionalInterest" runat="server" />
<asp:HiddenField ID="hiddenAdditionalInterestItems" runat="server" />
