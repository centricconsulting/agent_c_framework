<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctl_FarmBuilding_Property.ascx.vb" Inherits="IFM.VR.Web.ctl_FarmBuilding_Property" %>

<div runat="server" id="divMain">
    <h3>
        <asp:Label ID="lblAccordHeader" runat="server" Text="Property"></asp:Label>
        <span style="float: right;">
            <asp:LinkButton ID="lnkBtnClear" CssClass="RemovePanelLink" ToolTip="Clear" runat="server">Clear</asp:LinkButton>
            <asp:LinkButton ID="lnkBtnSave" CssClass="RemovePanelLink" ToolTip="Save" runat="server">Save</asp:LinkButton>
        </span>
    </h3>
    <div>
        <table style="width: 100%">
            <tr>
                <td style="width: 250px;">
                    <label for="<%=ddBuilding.ClientID%>">*Building</label></td>
                <td>
                    <asp:DropDownList ID="ddBuilding" runat="server"></asp:DropDownList></td>
            </tr>
            <tr>
                <td>
                    <label for="<%=ddConstruction.ClientID%>">*Construction</label></td>
                <td>
                    <asp:DropDownList ID="ddConstruction" runat="server">
                        <asp:ListItem Value=""></asp:ListItem>
                        <asp:ListItem Value="1">Cement</asp:ListItem>
                        <asp:ListItem Value="2">Frame</asp:ListItem>
                        <asp:ListItem Value="3">Masonry</asp:ListItem>
                        <asp:ListItem Value="4">Mixed</asp:ListItem>
                        <asp:ListItem Value="5">Steel</asp:ListItem>
                    </asp:DropDownList></td>
            </tr>
            <tr runat="server" id="trHeated">
                <td>
                    <label for="<%=chkHeated.ClientID%>">Building is Heated By Solid Fuel Unit</label></td>
                <td>
                    <asp:CheckBox ID="chkHeated" runat="server" /></td>
            </tr>
            <tr>
                <td>
                    <label for="<%=chkHay.ClientID %>">Building Stores Hay</label></td>
                <td>
                    <asp:CheckBox ID="chkHay" runat="server" /></td>
            </tr>
            <tr>
                <td>
                    <label for="<%=txtLimit.ClientID%>">*Building Limit</label></td>
                <td>
                    <asp:TextBox ID="txtLimit" MaxLength="11" Width="60px" runat="server"></asp:TextBox></td>
            </tr>
            <tr runat="server" id="DwellingContentsRow">
                <td>
                    <label for="<%=txtDwellingContentsLimit.ClientID%>">Household Contents Limit</label></td>
                <td>
                    <asp:TextBox ID="txtDwellingContentsLimit" MaxLength="11" Width="60px" runat="server"></asp:TextBox></td>
            </tr>
            <tr>
                <td>
                    <label for="<%=ddDeducible.ClientID%>">*Deductible</label></td>
                <td>
                    <asp:DropDownList ID="ddDeducible" runat="server"></asp:DropDownList></td>
            </tr>
            <tr id="trFarmCoverageEInfoMsg" visible="false" runat="server">
                <td colspan="2" Class="informationalText">
                     The deductible selection applies to all Farm Barns, Buildings and Structures at this location.
                </td>
            </tr>
            <tr>
                <td>
                    <label for="<%=txtYear.ClientID%>">*Year Constructed</label></td>
                <td>
                    <asp:TextBox ID="txtYear" MaxLength="4" Width="50px" runat="server"></asp:TextBox></td>
            </tr>
            <tr>
                <td>
                    <asp:Label ID="lblSquareFeet" runat="server" Text="Square Feet"></asp:Label>
                <td>
                    <asp:TextBox ID="txtsqrFeet" MaxLength="7" Width="50px" runat="server"></asp:TextBox></td>
            </tr>
            <tr>
                <td>
                    <asp:Label ID="lblDimensions" runat="server" Text="*Dimensions"></asp:Label>
                </td>
                <td>
                    <asp:TextBox ID="txtDimensions" runat="server"></asp:TextBox></td>
            </tr>
            <tr>
                <td>
                    <asp:HyperLink ID="lnkBuildingTypeHelp" Target="_blank" runat="server">*Building Type</asp:HyperLink>
                </td>
                <td>
                    <asp:DropDownList ID="ddBuildingType" runat="server">
                    </asp:DropDownList></td>
            </tr>
            <tr>
                <td>
                    <label for="<%=txtDescription.ClientID%>"><asp:Label ID="lblDescriptionRequired" runat="server" visible="false" Text="*" />Description</label></td>
                <td>
                    <asp:TextBox ID="txtDescription" TextMode="MultiLine" runat="server"></asp:TextBox></td>
            </tr>
        </table>
       <div style="position: relative; top: -260px; left: 320px; width: 118px;" runat="server" id="ReplacementCostSection">
            <asp:Button ID="btnReplacementCC" runat="server" Text="Replacement Cost Calculator" CssClass="StandardButtonWrap" Height="40px" Width="120px" />
        </div>
    </div>
</div>
<asp:HiddenField ID="HiddenField1" runat="server" />
<asp:HiddenField ID="hdnHasCosmeticDamagePreexisting" runat="server" />