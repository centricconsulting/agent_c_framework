<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctlReproductiveEquip.ascx.vb" Inherits="IFM.VR.Web.ctlReproductiveEquip" %>
<%@ Register Src="~/User Controls/QuoteEdit/FAR/PersonalPropertyControls/ctlFarmScheduledLimit.ascx" TagPrefix="uc1" TagName="ctlFarmScheduledLimit" %>

<div id="dvReproductiveList" runat="server" class="div">
    <asp:Repeater ID="PersPropRepeater" runat="server">
        <ItemTemplate>
            <table style="width: 100%" class="tableBorder">
                <tr>
                    <td>
                        <uc1:ctlFarmScheduledLimit runat="server" ID="ctlFarmScheduledLimit" />
                    </td>
                </tr>
            </table>
        </ItemTemplate>
        <AlternatingItemTemplate>
            <table style="width: 100%" class="table">
                <tr>
                    <td style="background-color: lightgray">
                        <uc1:ctlFarmScheduledLimit runat="server" ID="ctlFarmScheduledLimit" />
                    </td>
                </tr>
            </table>
        </AlternatingItemTemplate>
    </asp:Repeater>
</div>