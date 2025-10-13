<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctlHomAdditionalResidence.ascx.vb" Inherits="IFM.VR.Web.ctlHomAdditionalResidence" %>
<%@ Register Src="~/User Controls/QuoteEdit/HOM/SectionCoverages/HomSectionCoverageControls/ctlHomSectionCoverageAddress.ascx" TagPrefix="uc1" TagName="ctlHomSectionCoverageAddress" %>


<table style="width:100%">
    <tr>
        <td runat="server" id="tdDescription"><asp:Label ID="lblDescription" runat="server">Description</asp:Label><!-- Updated 7/10/19 asterisk for Home Endorsements Project Task 38905 MLW --></td>
        <td>Number of Families</td>
        <td></td>
        <td></td>
    </tr>
    <tr>
        <td runat="server" id="tdDescription1"><asp:TextBox ID="txtDescription" Width="100" MaxLength="250" runat="server"></asp:TextBox></td>
        <td>
            <asp:DropDownList ID="ddNumOfFamilies" Width="100" runat="server"></asp:DropDownList>
            <asp:TextBox ID="txtNumOfFamilies" Text="1" runat="server"></asp:TextBox>
        </td>
        <td><asp:LinkButton ID="lnkAddAdress" runat="server">Add Address</asp:LinkButton>
        </td>
        <td><asp:LinkButton ID="lnkDelete" runat="server">Delete</asp:LinkButton></td>
    </tr>
</table>
<div runat="server" id="divAddress">
    <uc1:ctlHomSectionCoverageAddress runat="server" id="ctlHomSectionCoverageAddress" />
    </div>