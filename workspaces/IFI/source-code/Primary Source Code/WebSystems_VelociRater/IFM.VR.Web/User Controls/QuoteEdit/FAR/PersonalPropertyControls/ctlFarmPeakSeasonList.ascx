<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctlFarmPeakSeasonList.ascx.vb" Inherits="IFM.VR.Web.ctlFarmPeakSeasonList" %>
<%@ Register Src="~/User Controls/QuoteEdit/FAR/PersonalPropertyControls/ctlFarmPeakSeason.ascx" TagPrefix="uc1" TagName="ctlFarmPeakSeason" %>

<div id="dvPeakSeasonList" runat="server" class="div">
    <asp:Repeater ID="PeakSeasonRepeater" runat="server">
        <ItemTemplate>
            <table style="width: 100%" class="tableBorder">
                <tr>
                    <td style="background-color: whitesmoke">
                        <uc1:ctlFarmPeakSeason runat="server" ID="ctlFarmPeakSeason" />
                    </td>
                </tr>
            </table>
        </ItemTemplate>
        <AlternatingItemTemplate>
            <table style="width: 100%" class="table">
                <tr>
                    <td style="background-color: lightgray">
                        <uc1:ctlFarmPeakSeason runat="server" ID="ctlFarmPeakSeason" />
                    </td>
                </tr>
            </table>
        </AlternatingItemTemplate>
    </asp:Repeater>
    <asp:HiddenField ID="hiddenPeakType" runat="server" />
</div>