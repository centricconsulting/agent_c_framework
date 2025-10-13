<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctl_CPR_App_Location.ascx.vb" Inherits="IFM.VR.Web.ctl_CPR_App_Location" %>
<%@ Register Src="~/User Controls/VR Commercial/Application/CPR/ctl_CPR_App_BuildingList.ascx" TagPrefix="uc1" TagName="ctl_CPR_App_BuildingList" %>

<h3>
    <asp:Label ID="lblAccordHeader" runat="server" Text="Location #0 - "></asp:Label>
        <span style="float: right;">
        <asp:LinkButton ID="lnkSave" ToolTip="Save" runat="server" CssClass="RemovePanelLink">Save</asp:LinkButton>
    </span>
</h3>
<div>
    <style>
        .LOCLabelColumn {
            width:32%;
            text-align:left;
        }
        .LOCDataColumn {
            /*width:10%;*/
            text-align:left;
        }
        .LOCUITextBox {
            width:75%;
        }
    </style>
    <uc1:ctl_CPR_App_BuildingList runat="server" ID="ctl_CPR_App_BuildingList" />
</div>

