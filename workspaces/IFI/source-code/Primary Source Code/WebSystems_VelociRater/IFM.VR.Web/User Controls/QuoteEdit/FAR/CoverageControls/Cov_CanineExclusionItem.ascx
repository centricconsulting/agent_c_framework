<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="Cov_CanineExclusionItem.ascx.vb" Inherits="IFM.VR.Web.Cov_CanineExclusionItem" %>

<table style="width:100%">
    <tr  style="padding-top:20px;">
        <td runat="server" id="tdName" style="padding-top:5px;"><asp:Label ID="lblName" runat="server">*Canine Name</asp:Label></td>
        <td runat="server" id="tdDescription" style="padding-top:5px;"><asp:Label ID="lblDescription" runat="server">*Canine Description</asp:Label></td>
    </tr>
    <tr>
        <td runat="server" id="tdName1"><asp:TextBox ID="txtName" Width="100" MaxLength="250" runat="server"></asp:TextBox></td>
        <td runat="server" id="tdDescription1" style="padding-top:20px;"> <asp:TextBox ID="txtDescription" TextMode="MultiLine" Width="250" runat="server" Rows="1"></asp:TextBox>
                <br />
                <asp:Label ID="lblMaxChar" runat="server" Text="Max Characters:" Font-Bold="True" Font-Size="XX-Small" ForeColor="Red"></asp:Label>&nbsp;
                <asp:Label ID="lblMaxCharCount" runat="server" Font-Bold="True" Font-Size="XX-Small" ForeColor="Red"></asp:Label></td>

        <td><asp:LinkButton ID="lnkDelete" runat="server">Delete</asp:LinkButton></td>
    </tr>
</table>
<asp:HiddenField ID="hiddenIncreasedLimit" runat="server" Value="0" />
<asp:HiddenField ID="hiddenMaxCharCount" runat="server" />