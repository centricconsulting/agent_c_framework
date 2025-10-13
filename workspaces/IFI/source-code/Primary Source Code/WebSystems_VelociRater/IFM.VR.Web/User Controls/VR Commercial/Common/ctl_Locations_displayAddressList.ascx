<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctl_Locations_displayAddressList.ascx.vb" Inherits="IFM.VR.Web.ctl_Locations_displayAddress" %>
<%@ Register Src="~/User Controls/VR Commercial/Common/ctl_Location_displayAddress.ascx" TagPrefix="uc1" TagName="ctl_Location_displayAddress" %>


<div id="div_master_wcp_app" runat="server">
    <div>
        <div id="divWorkplaces" runat="server">
            <h3>
                <asp:Label ID="lblWorkplacesAccordHeader" runat="server" Text="Locations"></asp:Label>
                 <span style="float: right;">
                     <%--<asp:LinkButton ID="lbAddNewWorkplace" CssClass="RemovePanelLink" ToolTip="Add New Location" runat="server">Add New Location</asp:LinkButton>--%>
                     <asp:LinkButton ID="lbSaveWorkplaces" CssClass="RemovePanelLink" ToolTip="Save" runat="server">Save</asp:LinkButton>
                 </span>
            </h3>
            <div runat="server" id="divWorkplacesList">
                <asp:Repeater ID="rptWorkplaces" runat="server">
                    <ItemTemplate>
                        <uc1:ctl_Location_displayAddress runat="server" ID="ctl_Location_displayAddress" />
                    </ItemTemplate>
                </asp:Repeater>
            </div>
        </div>
    </div>
</div>
<asp:HiddenField ID="hdn_master_wcp_app" runat="server" />
<asp:HiddenField ID="hdnAccordWorkplace" runat="server" />
<asp:HiddenField ID="hdnAccordWorkplaceList" runat="server" />
