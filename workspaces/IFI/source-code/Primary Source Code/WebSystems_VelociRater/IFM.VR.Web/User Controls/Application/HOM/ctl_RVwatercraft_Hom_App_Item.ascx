<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctl_RVwatercraft_Hom_App_Item.ascx.vb" Inherits="IFM.VR.Web.ctl_RVwatercraft_Hom_App_Item" %>

<h3>
    <asp:Label ID="lblHeader" runat="server" Text="Header"></asp:Label>
    <span style="float: right;">
        <asp:LinkButton ID="lnkClearRVWater" CssClass="RemovePanelLink" ToolTip="Clear this RV/Watercraft" runat="server">Clear</asp:LinkButton>
        <asp:LinkButton ID="lnkSaveRVWater" CssClass="RemovePanelLink" ToolTip="Save ALL RV/Watercrafts" runat="server">Save</asp:LinkButton>
    </span>
</h3>
<div>
    <div>
        <table runat="server" id="tblFormData" style="width: 100%">
            <tr>
                <td>
                    <asp:Label ID="lblSerial" AssociatedControlID="txtSerial" runat="server" Text="Serial Number"></asp:Label>
                    <br />
                    <asp:TextBox ID="txtSerial" Width="90" runat="server"></asp:TextBox>
                </td>
                <td>
                    <asp:Label ID="lblManufacturer" AssociatedControlID="txtManufacturer" runat="server" Text="Manufacturer"></asp:Label>
                    <br />
                    <asp:TextBox ID="txtManufacturer" Width="90" runat="server"></asp:TextBox>
                </td>
                <td>
                    <asp:Label ID="lblModel" AssociatedControlID="txtModel" runat="server" Text="Model"></asp:Label>
                    <br />
                    <asp:TextBox ID="txtModel" Width="90" runat="server"></asp:TextBox>
                </td>
            </tr>
        </table>

        <div id="divMotor" runat="server" class="standardSubSection">
            <h3>Boat Motor
                    <span style="float: right;">
                        <asp:LinkButton ID="lnkClearMotor" CssClass="RemovePanelLink" runat="server">Clear</asp:LinkButton>
                        <asp:LinkButton ID="lnkSaveMotor" CssClass="RemovePanelLink" runat="server">Save</asp:LinkButton>
                    </span>
            </h3>
            <div>
                <table id="tblMotorFormData" runat="server" style="width: 100%;">
                    <tr>
                        <td>
                            <label for="<%=txtMotorSerialNumber.ClientID%>">*Serial Number</label>
                            <br />
                            <asp:TextBox ID="txtMotorSerialNumber" Width="90" runat="server"></asp:TextBox>
                        </td>
                        <td>
                            <label for="<%=txtMotorManufacturer.ClientID%>">*Manufacturer</label>
                            <br />
                            <asp:TextBox ID="txtMotorManufacturer" Width="90" runat="server"></asp:TextBox>
                        </td>
                        <td>
                            <label for="<%=txtMotorModel.ClientID%>">*Model</label>
                            <br />
                            <asp:TextBox ID="txtMotorModel" Width="90" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                </table>
            </div>
        </div>

        <asp:Label ID="lblNoInputNeeded" runat="server" Text="No additional information required"></asp:Label>
    </div>
</div>