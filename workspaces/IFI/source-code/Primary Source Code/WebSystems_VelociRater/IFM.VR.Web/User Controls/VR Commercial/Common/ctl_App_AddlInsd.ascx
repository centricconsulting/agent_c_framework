<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctl_App_AddlInsd.ascx.vb" Inherits="IFM.VR.Web.ctl_App_AddlInsd" %>

<table border="0" id="additionalInsuredsAREA" runat="server" visible="true" width="100%">
    <tr><td colspan="3" class="standardBackColor sectionHeaderBar1 roundedContainer standardShadow gradient">Additional Insureds?&nbsp;&nbsp;<span style="font-size: 12px">(Liability)</span></td></tr>
    <tr><td colspan="3">&nbsp;<%--space--%></td></tr>
    <tr id="addIdgROW" runat="server" visible="true">
        <td colspan="3" align="center">
            <asp:DataGrid ID="DataGrid_additionalInsured" runat="Server" Visible="true" AutoGenerateColumns="False"
            Width="700" CellPadding="3" ItemStyle-VerticalAlign="Top" BorderStyle="None" ItemStyle-Font-Size="Small" HeaderStyle-Font-Size="Small">
            <HeaderStyle CssClass="GridHeaderStyle" />
            <AlternatingItemStyle CssClass="GridAlternatingRowStyle" />
                <Columns>
                    <asp:BoundColumn DataField="Num" Visible="false"></asp:BoundColumn>
                    <asp:BoundColumn DataField="NumType" Visible="false" ItemStyle-BorderColor="White"></asp:BoundColumn>
                    <asp:BoundColumn DataField="Type" HeaderText="Type" ItemStyle-Wrap="true" ItemStyle-BorderColor="White"></asp:BoundColumn>
                    <asp:BoundColumn DataField="Name" HeaderText="Name" ItemStyle-Wrap="true" ItemStyle-BorderColor="White"></asp:BoundColumn>
                    <asp:BoundColumn DataField="DesigPrem" HeaderText="Premises" ItemStyle-Wrap="true" ItemStyle-BorderColor="White"></asp:BoundColumn>
                    <asp:BoundColumn DataField="Desc" HeaderText="Description" ItemStyle-Wrap="true" ItemStyle-BorderColor="White"></asp:BoundColumn>
                    <asp:BoundColumn DataField="Waiver" HeaderText="Waiver" ItemStyle-Wrap="true" ItemStyle-BorderColor="White"></asp:BoundColumn>
                    <asp:ButtonColumn Text="Edit" CommandName="edit" ButtonType="PushButton" Visible="true" ItemStyle-BorderColor="White"></asp:ButtonColumn>
                    <asp:ButtonColumn Text="Delete" CommandName="delete" ButtonType="PushButton" visible="true"></asp:ButtonColumn>
                </Columns>
            </asp:DataGrid>
        </td>
    </tr>

    <tr id="additionalInsuredTYPErow" runat="server" visible="true" align="right" style="height: 50px;"><td align="right">Additional Insured Type&nbsp;</td>
        <td colspan="3" align="left">
            <asp:DropDownList ID="DropDownList_additonalInsuredType" runat="server" Width="510" AutoPostBack="true">
                <asp:ListItem Value="0">Select</asp:ListItem>
                <asp:ListItem Value="21018">Co-Owner of Insured Premises</asp:ListItem>
                <asp:ListItem Value="501">Controlling Interests</asp:ListItem>
                <asp:ListItem Value="21022">Designated Person or Organization</asp:ListItem>
                <asp:ListItem Value="21019">Engineers, Architects or Surveyors</asp:ListItem>
                <asp:ListItem Value="21023">Engineers, Architects or Surveyors Not Engaged by the Named Insured</asp:ListItem>
                <asp:ListItem Value="21020">Lessor of Leased Equipment</asp:ListItem>
                <asp:ListItem Value="21053">Managers or Lessors of Premises</asp:ListItem>
                <asp:ListItem Value="21054">Mortgagee, Assignee Or Receiver</asp:ListItem>
                <asp:ListItem Value="21055">Owner or Other Interests From Whom Land has been Leased</asp:ListItem>
                <asp:ListItem Value="21024">Owners, Lessees or Contractors</asp:ListItem>
                <asp:ListItem Value="21025">Owners, Lessees or Contractors - With Additional Insured Requirement in Construction Contract</asp:ListItem>
                <asp:ListItem Value="21016">State or Political Subdivision - Permits Relating to Premises</asp:ListItem>
                <asp:ListItem Value="21026">State or Political Subdivisions - Permits</asp:ListItem>
                <asp:ListItem Value="21017">Townhouse Associations</asp:ListItem>
                <asp:ListItem Value="21021">Vendors</asp:ListItem>        
            </asp:DropDownList>
        </td>
    </tr>
            
    <tr id="addItypeROW" runat="server" visible="false"><td colspan="2" align="right">Additional Insured Type: <asp:Label ID="Label_addItype" runat="server" Font-Bold="true"></asp:Label></td></tr>
    <tr id="addInameROW" runat="server" visible="false"><td align="right">Name of Person or Organization&nbsp;</td><td align="left"><asp:TextBox ID="TextBox_addInameOfOrg" runat="server" Rows="4" Width="200" TextMode="MultiLine" Wrap="true" MaxLength="245"></asp:TextBox></td></tr>
    <tr id="addIpremisesROW" runat="server" visible="false"><td align="right">Designation of Premises&nbsp;</td><td align="left"><asp:TextBox ID="TextBox_addIdesignation" runat="server" Rows="4" Width="200" TextMode="MultiLine" Wrap="true" MaxLength="245"></asp:TextBox></td></tr>
    <tr id="addIDescROW" runat="server" visible="false"><td align="right">Description&nbsp;</td><td align="left"><asp:TextBox ID="TextBox_addIdesc" runat="server" Rows="4" Width="200" TextMode="MultiLine" Wrap="true" MaxLength="245"></asp:TextBox></td></tr>
    <tr id="addIwaiverROW" runat="server" visible="false"><td align="right">Waiver of Subrogation&nbsp;</td><td align="left"><asp:CheckBox ID="CheckBox_addIwaiver" runat="server" /></td></tr>
    <%--<tr id="addIpremROW" runat="server" visible="false"><td colspan="3"></td><td>Premium</td><td><asp:TextBox ID="TextBox_addIprem" runat="server" MaxLength="7"></asp:TextBox></td></tr>--%>
    
    <tr id="addIbuttonROW" runat="server" visible="false"><td colspan="3" style="text-align: center;"><br /><asp:Button ID="Button_addInsSaveAdd" runat="server" CssClass="roundedContainer StandardButton" Text="Save"></asp:Button></td></tr>
    
</table>
<asp:Label ID="lblMsg" runat="server" ForeColor="Red" Text="&nbsp;"></asp:Label>
