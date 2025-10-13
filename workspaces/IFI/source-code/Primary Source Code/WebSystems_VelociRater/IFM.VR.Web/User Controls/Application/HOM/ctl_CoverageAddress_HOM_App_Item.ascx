<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctl_CoverageAddress_HOM_App_Item.ascx.vb" Inherits="IFM.VR.Web.ctl_CoverageAddress_HOM_App_Item" %>
<div id="dvSectionIAddress" runat="server">
    <div id="divBar" runat="server" class="standardSubSection">
        <hr /><asp:Label ID="lblAdditionalInterests" runat="server" Visible="false" Text="Address:"></asp:Label>
        <br />
    </div>
    <table style="width: 100%;">
        <tr>
            <td class="SingleFieldRowCell" style="width: 50%;">
                <asp:Label ID="lblStreetNum" AssociatedControlID="txtStreetNum" runat="server" Text="*Street #"></asp:Label>
            </td>
            <td>
                <asp:TextBox ID="txtStreetNum" runat="server" MaxLength="35"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td class="SingleFieldRowCell" style="width: 50%;">
                <asp:Label ID="lblStreetName" AssociatedControlID="txtStreetName" runat="server" Text="*Street Name"></asp:Label>
            </td>
            <td>
                <asp:TextBox ID="txtStreetName" runat="server" MaxLength="65"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td class="SingleFieldRowCell" style="width: 50%;">
                <asp:Label ID="lblAptSuite" AssociatedControlID="txtAptSuite" runat="server" Text="Apt/Suite Number"></asp:Label>
            </td>
            <td>
                <asp:TextBox ID="txtAptSuite" runat="server" MaxLength="65"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td class="SingleFieldRowCell" style="width: 50%;">
                <asp:Label ID="lblZipCode" AssociatedControlID="txtZipCode" runat="server" Text="*ZIP"></asp:Label>
            </td>
            <td>
                <asp:TextBox ID="txtZipCode" onblur="$(this).val(formatPostalcode($(this).val()));" runat="server" MaxLength="10"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td class="SingleFieldRowCell" style="width: 50%;">
                <asp:Label ID="lblCity" AssociatedControlID="txtCityName" runat="server" Text="*City"></asp:Label>
            </td>
            <td>
                <asp:DropDownList ID="ddCityName" CausesValidation="false" Style="display: none" runat="server"></asp:DropDownList>
                <asp:TextBox ID="txtCityName" runat="server" MaxLength="25"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td class="SingleFieldRowCell" style="width: 50%;">
                <asp:Label ID="txtState" AssociatedControlID="ddStateAbbrev" runat="server" Text="*State"></asp:Label>
            </td>
            <td>
                <asp:DropDownList ID="ddStateAbbrev" runat="server"></asp:DropDownList>
            </td>
        </tr>
        <tr>
            <td class="SingleFieldRowCell" style="width: 50%;">
                <asp:Label ID="lblCounty" AssociatedControlID="txtCounty" runat="server" Text="*County"></asp:Label>
            </td>
            <td>
                <asp:TextBox ID="txtCounty" runat="server" MaxLength="11"></asp:TextBox>
            </td>
        </tr>
    </table>
    <table style="width: 100%">
        <tr>
            <td align="center">
                <asp:Button ID="btnOK" runat="server" Text="OK" CssClass="StandardSaveButton" />
                &nbsp;&nbsp;
                        <asp:Button ID="btnCancel" runat="server" Text="Cancel" CssClass="StandardSaveButton" />
            </td>
        </tr>
    </table>

</div>