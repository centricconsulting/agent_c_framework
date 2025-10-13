<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctl_ENDO_App_PhotographersEquipment.ascx.vb" Inherits="IFM.VR.Web.ctl_ENDO_App_PhotographersEquipment" %>

<div runat="server" id="divPhotogScheduledEquipment">
    <h3>Scheduled Photography Equipment</h3>
    <div>
        <table id="tblPhotogItems" runat="server" style="width:100%;">
            <tr id="trPhotogItemsTotalRow" runat="server" class="trPhotogItemsTotalRow">
                <td colspan="2" style="text-align:left;">
                    <asp:Label ID="lblPhotographerItemsTotal" runat="server" Text="Total of All Scheduled Limits: " CssClass="lblPhotogTotal"></asp:Label>
                </td>
            </tr>
            <tr><td colspan="2">&nbsp;</td></tr>
            <tr id="trPhotogScheduledItemsListRow" runat="server" class="trPhotogScheduledItemsListRow">
                <td colspan="2">
                    <asp:Repeater ID="rptPhotogScheduledItems" runat="server" OnItemCommand="rptPhotogScheduledItems_ItemCommand" >
                        <ItemTemplate>
                            <table style="width:100%;">
                                <tr>
                                    <td style="text-align:right;width:7%;vertical-align:top;">
                                        *Limit
                                    </td>
                                    <td style="text-align:left;width:20%;vertical-align:top;">
                                        <asp:TextBox ID="txtPhotogItemLimit" runat="server" Width="70%" CssClass="txtPhotogLimit"></asp:TextBox>
                                    </td>
                                    <td style="text-align:right;width:7%;vertical-align:top;">
                                        *Description
                                    </td>
                                    <td style="text-align:left;width:60%;">
                                        <asp:TextBox ID="txtPhotogItemDesc" runat="server" Width="100%" Height="50px" TextMode="MultiLine" MaxLength="250" CssClass="txtPhotogItemDesc" ></asp:TextBox>
                                    </td>                                   
                                </tr>
                            </table>                                        
                        </ItemTemplate>
                    </asp:Repeater>
                </td>
            </tr>            
        </table>
        <asp:Label ID="lblMsg" runat="server" Text="&nbsp;" ForeColor="Red"></asp:Label>
        <asp:HiddenField ID="hdnAccord" runat="server" />
    </div>
</div>
