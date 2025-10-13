<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctlInlandMarineIncreasedLimitList.ascx.vb" Inherits="IFM.VR.Web.ctlInlandMarineIncreasedLimitList" %>
<%@ Register Src="~/User Controls/InlandMarine/ctlInlandMarineIncreasedLimit.ascx" TagPrefix="uc1" TagName="ctlInlandMarineIncreasedLimit" %>

<div id="dvInlandMarineList" runat="server" class="div">
    <asp:Repeater ID="IMRepeater" runat="server">
        <ItemTemplate>
            <table style="width: 100%" class="tableBorder">
                <tr>
                    <td>
                        <uc1:ctlInlandMarineIncreasedLimit runat="server" ID="ctlInlandMarineIncreasedLimit" />
                    </td>
                </tr>
            </table>
        </ItemTemplate>
        <AlternatingItemTemplate>
            <table style="width: 100%" class="table">
                <tr>
                    <td style="background-color: lightgray">
                        <uc1:ctlInlandMarineIncreasedLimit runat="server" ID="ctlInlandMarineIncreasedLimit" />
                    </td>
                </tr>
            </table>
        </AlternatingItemTemplate>
    </asp:Repeater>
</div>