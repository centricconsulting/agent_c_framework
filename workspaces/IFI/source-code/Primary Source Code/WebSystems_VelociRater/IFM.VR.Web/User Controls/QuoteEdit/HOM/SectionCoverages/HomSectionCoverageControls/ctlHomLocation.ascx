<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctlHomLocation.ascx.vb" Inherits="IFM.VR.Web.ctlHomLocation" %>
<%@ Register Src="~/User Controls/QuoteEdit/HOM/SectionCoverages/HomSectionCoverageControls/ctlHomSectionCoverageAddress.ascx" TagPrefix="uc1" TagName="ctlHomSectionCoverageAddress" %>

<table style="width:100%">
    <tr>
        <td width="200px"></td>
        <td><asp:LinkButton ID="lnkAddAdress" runat="server">Add Address</asp:LinkButton></td>
        <td><asp:LinkButton ID="lnkDelete" runat="server">Delete</asp:LinkButton></td>
    </tr>
</table>
<div runat="server" id="divAddress">
    <uc1:ctlHomSectionCoverageAddress runat="server" id="ctlHomSectionCoverageAddress" />
</div>
