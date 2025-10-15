<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctlHomFarmLand.ascx.vb" Inherits="IFM.VR.Web.ctlHomFarmLand" %>
<%@ Register Src="~/User Controls/QuoteEdit/HOM/SectionCoverages/HomSectionCoverageControls/ctlHomSectionCoverageAddress.ascx" TagPrefix="uc1" TagName="ctlHomSectionCoverageAddress" %>


<table style="width:100%">
                        <tr>
                            <td><asp:Label ID="lblDescription" runat="server">Description</asp:Label><!-- Updated 7/10/19 asterisk for Home Endorsements Project Task 38907 MLW --></td>
                            <td></td>
                            <td></td>
                        </tr>
                        <tr>
                            <td>
                                <asp:TextBox ID="txtDescription" TextMode="MultiLine" Width="300" runat="server"></asp:TextBox>
                <br /><!-- Added 1/18/18 max chars for HOM Upgrade MLW -->
                <asp:Label ID="lblMaxChar" runat="server" Text="Max Characters:" Font-Bold="True" Font-Size="XX-Small" ForeColor="Red"></asp:Label>&nbsp;
                <asp:Label ID="lblMaxCharCount" runat="server" Font-Bold="True" Font-Size="XX-Small" ForeColor="Red"></asp:Label>

                            </td>
                            <td>
                                <asp:LinkButton ID="lnkAddAdress" runat="server">Add Address</asp:LinkButton></td>
                            <td>
                                <asp:LinkButton ID="lnkDelete" runat="server">Delete</asp:LinkButton></td>
                        </tr>
                    </table>
                <asp:HiddenField ID="hiddenIncreasedLimit" runat="server" Value="0" /><!-- Added 1/18/18 max chars for HOM Upgrade MLW -->
                <asp:HiddenField ID="hiddenMaxCharCount" runat="server" />
<div runat="server" id="divAddress">
    <uc1:ctlHomSectionCoverageAddress runat="server" ID="ctlHomSectionCoverageAddress" />
    </div>