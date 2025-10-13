<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctl_RVwatercraft_HOM_App.ascx.vb" Inherits="IFM.VR.Web.ctl_RVwatercraft_HOM_App" %>

<%@ Register Src="~/User Controls/Application/HOM/ctl_RVwatercraft_Hom_App_Item.ascx" TagPrefix="uc1" TagName="ctl_RVwatercraft_Hom_App_Item" %>

<div id="divRVWatercraft" runat="server">
    <h3>RV / WATERCRAFT
                <span style="float: right;">
                    <asp:LinkButton ID="lnkSaveRVWater" CssClass="RemovePanelLink" ToolTip="Save Page" runat="server">Save</asp:LinkButton>
                </span>
    </h3>
    <div id="divInLandMarineContent" runat="server">

        <div runat="server" id="divYouthFul">
            <h3>Youngest Operator in the Household
                    <span style="float: right;">
                        <asp:LinkButton ID="lnkClearYouth" CssClass="RemovePanelLink" ToolTip="Clear Operator" runat="server">Clear</asp:LinkButton>
                        <asp:LinkButton ID="lnkSaveYouth" CssClass="RemovePanelLink" ToolTip="Save Page" runat="server">Save</asp:LinkButton>
                    </span>
            </h3>
            <div>
                <table style="width: 100%;">

                    <tr>
                        <td style="text-align: right">
                            <label for="<%=txtFirstName.ClientID %>">*First Name</label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtFirstName" runat="server" TabIndex="1" MaxLength="50"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td style="text-align: right">
                            <label for="<%=Me.txtLastName.ClientID%>">*Last Name</label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtLastName" runat="server" TabIndex="3" MaxLength="50"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td style="text-align: right">
                            <label for="<%=Me.txtBirthDate.ClientID%>">*Birth Date</label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtBirthDate" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                </table>
            </div>
        </div>

        <div id="divRVItems" runat="server" style="margin-top: 15px;">
            <asp:Repeater ID="rvWaterRepeater" runat="server">
                <ItemTemplate>
                    <uc1:ctl_RVwatercraft_Hom_App_Item runat="server" ID="ctl_RVwatercraft_Hom_App_Item" />
                </ItemTemplate>
            </asp:Repeater>
        </div>

        <asp:HiddenField ID="hiddenActiveRVWatercraft" runat="server" />
        <asp:HiddenField ID="hiddenActiveRVWater" runat="server" />
    </div>
</div>