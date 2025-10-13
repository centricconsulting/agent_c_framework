<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctlHomTrust.ascx.vb" Inherits="IFM.VR.Web.ctlHomTrust" %>
<%@ Register Src="~/User Controls/QuoteEdit/HOM/SectionCoverages/HomSectionCoverageControls/ctlHomAdditionalInterestAddress.ascx" TagPrefix="uc1" TagName="ctlHomAdditionalInterestAddress" %>

<div runat="server" id="divTrustee" style="display:block;">
<table style="width:100%">
    <tr>
        <td runat="server" id="tdName" style="vertical-align:top;">
            <div runat="server" id="divTrusteeName" style="display:block;">
                <asp:Label ID="lblName" runat="server">*Name</asp:Label><br />
                <asp:TextBox ID="txtName" Width="125" MaxLength="50" runat="server"></asp:TextBox>
                <asp:TextBox ID="txtAiId" ToolTip="This should be hidden. AI ID" runat="server"></asp:TextBox>
                <asp:TextBox ID="txtIsEditable" Style="display: none;" ToolTip="This should be hidden. Was created this session." runat="server"></asp:TextBox>                            
                <asp:Label ID="lblExpanderText" runat="server" Text="Label"></asp:Label>
            </div>
        </td>
        <td runat="server" id="tdAddress" style="vertical-align:top;">
            <asp:Label ID="lblTrustAddress" runat="server" Text="*Trustee Address:"></asp:Label>
            <uc1:ctlHomAdditionalInterestAddress runat="server" id="ctlHomAdditionalInterestAddress" />
        </td>
    </tr>
    <tr>
        <td runat="server" id="tdBlank"></td>
        <td style="text-align:center;"><asp:LinkButton ID="lnkDelete" runat="server">Delete</asp:LinkButton></td>
    </tr>
</table>
</div>
<asp:LinkButton ID="lnkViewEdit" runat="server">View/Edit Trustee</asp:LinkButton>

