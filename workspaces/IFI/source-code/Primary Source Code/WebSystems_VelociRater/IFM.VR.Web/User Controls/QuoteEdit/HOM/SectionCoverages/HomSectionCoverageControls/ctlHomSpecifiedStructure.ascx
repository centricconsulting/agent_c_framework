<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctlHomSpecifiedStructure.ascx.vb" Inherits="IFM.VR.Web.ctlHomSpecifiedStructure" %>
<%@ Register Src="~/User Controls/QuoteEdit/HOM/SectionCoverages/HomSectionCoverageControls/ctlHomSectionCoverageAddress.ascx" TagPrefix="uc1" TagName="ctlHomSectionCoverageAddress" %>




<table style="width: 100%">
    <tr>
        <td runat="server" id="tdLimit1"><asp:Label ID="lblLimit" runat="server">Limit</asp:Label><!-- Updated 7/10/19 asterisk for Home Endorsements Project Task 38903 MLW --></td>
        <td><asp:Label ID="lblDescription" runat="server">Description</asp:Label><!-- Updated 7/10/19 asterisk for Home Endorsements Project Task 38910 MLW --></td>
        <td runat="server" ID="tdConst1">Construction Type                
        </td>
        <td></td>
        <td></td>
    </tr>
    <tr style="vertical-align:top;">
        <td runat="server" id="tdLimit">
            <asp:TextBox ID="txtLimit" Width="100" runat="server"></asp:TextBox>
        </td>
        <td>
            <asp:TextBox ID="txtDescription" Width="100" MaxLength="250" runat="server"></asp:TextBox>
            <!-- Added 7/8/19 max chars for Home Endorsements Project Task 38910 MLW --><br />
            <asp:Label ID="lblMaxChar" runat="server" Text="Max Characters:" Font-Bold="True" Font-Size="XX-Small" ForeColor="Red"></asp:Label>&nbsp;
            <asp:Label ID="lblMaxCharCount" runat="server" Font-Bold="True" Font-Size="XX-Small" ForeColor="Red"></asp:Label>
            <asp:HiddenField ID="hiddenMaxCharCount" runat="server" />
        </td>
        <td runat="server" ID="tdConst2">
            <asp:DropDownList ID="ddConstructionType" Width="100" runat="server"></asp:DropDownList>
        </td>
        <td>
            <asp:LinkButton ID="lnkAddAdress" runat="server">Add Address</asp:LinkButton>
        </td>
        <td>
            <asp:LinkButton ID="lnkDelete" runat="server">Delete</asp:LinkButton>
        </td>
    </tr>    
</table>
<div runat="server" id="divAddress">    
    <uc1:ctlHomSectionCoverageAddress runat="server" ID="ctlHomSectionCoverageAddress" />
</div>
