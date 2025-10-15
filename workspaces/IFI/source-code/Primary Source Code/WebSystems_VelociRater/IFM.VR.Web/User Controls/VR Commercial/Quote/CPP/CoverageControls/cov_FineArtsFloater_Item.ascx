<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="cov_FineArtsFloater_Item.ascx.vb" Inherits="IFM.VR.Web.cov_FineArtsFloater_Item" %>
<%@ Register Src="~/User Controls/VR Commercial/Quote/CPP/CoverageControls/cov_FineArtsFloater_Item_Details.ascx" TagPrefix="uc1" TagName="cov_FineArtsFloater_Item_Details" %>


<asp:Repeater ID="faRepeater" runat="server">
    <ItemTemplate>
        <div class="ItemGroup">
            <asp:CheckBox ID="chkApply" runat="server" class="chkOption2" />
            Location:
            <asp:Label ID="txtAddress" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "Address")%>' Style="margin-right: 13px;"></asp:Label>
            <br />
            <span style="padding-left: 20px;">Building #<asp:Label ID="txtBuildingNum" Text='<%# DataBinder.Eval(Container.DataItem, "BuildingIndex") + 1%>' runat="server"></asp:Label></span>
            <div style="display: none; margin-bottom: 25px;" class="chkDetail">
                <div runat="server" id="divItemDetails" style="padding-left: 20px; padding-top: 10px;">
                    <table class="qs_grid_4_columns">
                        <uc1:cov_FineArtsFloater_Item_Details runat="server" id="cov_FineArtsFloater_Item_Details" />
                    </table>
                </div>
            </div>
        </div>
    </ItemTemplate>
</asp:Repeater>

