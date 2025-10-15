<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctl_App_PhotographersEquipment.ascx.vb" Inherits="IFM.VR.Web.ctl_App_PhotographersEquipment" %>



<script type="text/javascript">
$(function () {
    $('.txtPhotogSerial').on('keyup', function (event) {
        $(this).val($(this).val().replace(/[^a-z0-9' ']/gi, ''))
    });
});


</script>


<div runat="server" id="divPhotogScheduledEquipment">
    <h3>Scheduled Photography Equipment
        <span style="float: right;">
            <asp:LinkButton ID="lnkSave" runat="server" ToolTip="Save" CssClass="RemovePanelLink">Save</asp:LinkButton>
        </span>
    </h3>
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
                                        *Serial Number
                                    </td>
                                    <td style="text-align:left;width:20%;vertical-align:top;">
                                        <asp:TextBox ID="txtPhotogSerialNum" runat="server" Width="70%" CssClass="txtPhotogSerial"  MaxLength="30"></asp:TextBox>
                                    </td>   

                                    <td style="text-align:right;width:7%;vertical-align:top;">
                                        *Description
                                    </td>
                                    <td style="text-align:left;width:60%;">
                                        <asp:TextBox ID="txtPhotogItemDesc" runat="server" Width="85%" Height="50px" TextMode="MultiLine" MaxLength="250" CssClass="txtPhotogItemDesc" ></asp:TextBox>
                                        <div id="divMaxChars" runat="server"><span style="color:red;">Max Characters: 250</span></div>
                                    </td>
                                    <td id="tdDeleteLink" runat="server" style="text-align:left;width:6%;vertical-align:central;">
                                        <asp:LinkButton ID="lbDeleteItem" runat="server" Text="Delete" CommandName="DELETE" CssClass="lbDeleteItem"></asp:LinkButton>
                                    </td>
                                </tr>
                            </table>
                                        
                        </ItemTemplate>
                    </asp:Repeater>
                </td>
            </tr>
            <tr id="trPhotogAddItemRow" runat="server" class="trPhotogAddItemRow">
                <%--<td style="width:80%;">&nbsp;</td>--%>
                <td colspan="2" style="text-align:left;width:100%;">
                    <asp:Button ID="btnAddNew" runat="server" Text="Add Additional Item" CssClass="StandardSaveButton" />
                    <%--<asp:LinkButton ID="lbPhotogAddItem" runat="server" Text="Add Additional" CssClass="lbPhotogAddItem"></asp:LinkButton>--%>
                </td>
            </tr>
        </table>
        <asp:Label ID="lblMsg" runat="server" Text="&nbsp;" ForeColor="Red"></asp:Label>
        <asp:HiddenField ID="hdnAccord" runat="server" />
    </div>
</div>

