<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctl_App_ContractorsScheduledEquipmentList.ascx.vb" Inherits="IFM.VR.Web.ctl_App_ContractorsScheduledEquipmentList" %>
<%@ Register Src="~/User Controls/VR Commercial/Common/ctl_App_ContractorsScheduledEquipmentItem.ascx" TagPrefix="uc1" TagName="ctlContractorsScheduledItem"  %>

<div id="divContractorsEquipmentList" runat="server">
    <h3>Contractors Equipment Scheduled
        <span style="float: right;">
            <asp:LinkButton ID="lnkBtnAdd" runat="server" ToolTip="Add Building Classification" CssClass="RemovePanelLink">Add New</asp:LinkButton>
        </span>
    </h3>
    <div runat="server" id="divList">
            <asp:Repeater ID="Repeater1" runat="server">
                <ItemTemplate>
                    <uc1:ctlContractorsScheduledItem runat="server" ID="ctlScheduledItem" />
                </ItemTemplate>
            </asp:Repeater>
    </div>
    <asp:HiddenField ID="hdnAccord" runat="server" />
    <asp:HiddenField ID="hdnAccordList" runat="server" />
</div>    
