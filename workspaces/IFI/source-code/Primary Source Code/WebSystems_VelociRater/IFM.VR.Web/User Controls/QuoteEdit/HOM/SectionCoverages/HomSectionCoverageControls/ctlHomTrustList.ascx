<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctlHomTrustList.ascx.vb" Inherits="IFM.VR.Web.ctlHomTrustList" %>
<%@ Register Src="~/User Controls/QuoteEdit/HOM/SectionCoverages/HomSectionCoverageControls/ctlHomTrust.ascx" TagPrefix="uc1" TagName="ctlHomTrust" %>
<%@ Register Src="~/User Controls/QuoteEdit/HOM/SectionCoverages/HomSectionCoverageControls/ctlHomAdditionalInterestAddress.ascx" TagPrefix="uc1" TagName="ctlHomAdditionalInterestAddress" %>

<table style="width:100%">
    <tr>
        <td runat="server" id="tdName" style="vertical-align:top;">
            <asp:Label ID="lblTrustName" runat="server">*Trust Name</asp:Label><br />
            <asp:TextBox ID="txtTrustName" Width="125" MaxLength="250" TextMode="MultiLine" runat="server"></asp:TextBox>
            <asp:TextBox ID="txtAiId" ToolTip="This should be hidden. AI ID" runat="server"></asp:TextBox>
            <asp:TextBox ID="txtIsEditable" Style="display: none;" ToolTip="This should be hidden. Was created this session." runat="server"></asp:TextBox>                            
            <asp:Label ID="lblExpanderText" runat="server" Text="Label"></asp:Label>
        </td>
        <td runat="server" id="tdAddress" style="vertical-align:top;">
            <asp:Label ID="lblTrustAddress" runat="server" Text="*Trust Address:"></asp:Label>
                <uc1:ctlHomAdditionalInterestAddress runat="server" id="ctlHomAdditionalInterestAddress" />
        </td>
    </tr>
</table>

<asp:Repeater ID="Repeater1" runat="server">
    <ItemTemplate>
        <uc1:ctlHomTrust runat="server" id="ctlHomTrust" />
        </ItemTemplate>
    <AlternatingItemTemplate>
        <div style="background-color:lightgray;">
            <uc1:ctlHomTrust runat="server" id="ctlHomTrust" />
        </div>        
    </AlternatingItemTemplate>
</asp:Repeater>
<div>
    <div style="float:right"><asp:LinkButton ID="lnkAddAddress" runat="server">Add Another Trustee</asp:LinkButton></div>
</div>

