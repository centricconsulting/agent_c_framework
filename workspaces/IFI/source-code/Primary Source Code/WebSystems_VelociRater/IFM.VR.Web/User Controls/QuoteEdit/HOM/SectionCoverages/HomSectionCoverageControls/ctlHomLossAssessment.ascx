<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctlHomLossAssessment.ascx.vb" Inherits="IFM.VR.Web.ctlHomLossAssessment" %>
<%@ Register Src="~/User Controls/QuoteEdit/HOM/SectionCoverages/HomSectionCoverageControls/ctlHomSectionCoverageAddress.ascx" TagPrefix="uc1" TagName="ctlHomSectionCoverageAddress" %>

<table style="width:100%">
    <tr>
        <td>
            <label for="<%=txtIncludedLimit.ClientID%>">Included Limit</label>
            <br />
            <asp:TextBox ID="txtIncludedLimit" Width="100" runat="server"></asp:TextBox>
        </td>
        <td>
            <label for="<%=ddIncreasedLimit.ClientID%>">Increased Limit</label>
            <br />
            <asp:DropDownList ID="ddIncreasedLimit" Width="100" runat="server"></asp:DropDownList>
        </td>
        <td>
            <label for="<%=txtTotalLimit.ClientID%>">Total Limit</label>
            <br />
            <asp:TextBox ID="txtTotalLimit" Width="100" runat="server"></asp:TextBox>
        </td>
        <td><asp:LinkButton ID="lnkAddAdress" runat="server">Add Address</asp:LinkButton></td>
        <td><asp:LinkButton ID="lnkDelete" runat="server">Delete</asp:LinkButton></td>
    </tr>
</table>
<div runat="server" id="divAddress">
    <uc1:ctlHomSectionCoverageAddress runat="server" id="ctlHomSectionCoverageAddress" />
</div>
