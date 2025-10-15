<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctl_Endo_CTEQList.ascx.vb" Inherits="IFM.VR.Web.ctl_Endo_CTEQList" %>
<%@ Register Src="~/User Controls/Endorsements/CommonControls/ctl_Endo_CTEQ.ascx" TagPrefix="uc1" TagName="ctlContractorsScheduledItem" %>


<div id="divContractorsEquipmentList" runat="server">
    <h3>Contractors Equipment Scheduled
        <span style="float: right;">
            <asp:LinkButton ID="lnkBtnAdd" runat="server" ToolTip="Add Item" CssClass="RemovePanelLink">Add Item</asp:LinkButton>
            <asp:LinkButton ID="lnkBtnSave" runat="server" ToolTip="Save Item" CssClass="RemovePanelLink">Save</asp:LinkButton>
        </span>
    </h3>
    <div>
        Scheduled Amount: &nbsp;&nbsp;&nbsp;
        <asp:TextBox ID="txtScheduledAmount" runat="server" Text="$0" Enabled="false" CssClass="txtCTSched"></asp:TextBox>
        <div runat="server" id="divList">              
            <asp:Repeater ID="Repeater1" runat="server">
                <ItemTemplate>
                    <uc1:ctlContractorsScheduledItem runat="server" ID="ctlScheduledItem" />
                </ItemTemplate>
            </asp:Repeater>
        </div>
        <section class="AmountRemainingAndWarningRow">
            Amount Remaining: &nbsp;&nbsp;&nbsp;
            <asp:TextBox ID="txtAmountRemaining" runat="server" Text="$0" Enabled="false" CssClass="txtCTRemain"></asp:TextBox>
            <br />
            <span class="txtCTWarning" style="color:red; display:none;">Total Scheduled Amount does not match the total scheduled amount entered on the quote.  This will change the quote premium.</span>
        </section>
    </div>
    
    <asp:HiddenField ID="hdnAccord" runat="server" />
    <asp:HiddenField ID="hdnAccordList" runat="server" />
</div>    
