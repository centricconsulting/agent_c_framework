<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctlFarmScheduledLimit.ascx.vb" Inherits="IFM.VR.Web.ctlFarmScheduledLimit" %>
<%@ Register Src="~/User Controls/QuoteEdit/FAR/PersonalPropertyControls/ctlFarmPeakSeasonList.ascx" TagPrefix="uc1" TagName="ctlFarmPeakSeasonList" %>

<div id="dvSchedLimit" runat="server" class="div">
    <table style="width: 100%" class="tableBorder">
        <tr>
            <td style="width: 15px"></td>
            <td style="width: 100px; vertical-align: central" class="CovTableColumn">
                <asp:TextBox ID="txtSched_LimitData" runat="server" CssClass="CovTableItem" onkeyup='$(this).val(FormatAsNumericDigitsOnly($(this).val()));'></asp:TextBox>
            </td>
            <td style="vertical-align: central">
                <asp:TextBox ID="txtSched_Description" runat="server" TextMode="MultiLine" Width="175px" MaxLength="250" ToolTip="250 Max Characters"></asp:TextBox>
            </td>
            <td style="width: 100px">
                <asp:LinkButton ID="lnkDelete" runat="server" CssClass="RemovePanelLink" OnClick="OnConfirm">Delete </asp:LinkButton>
            </td>
        </tr>
    </table>
    <div id="dvRentedOrBorrowedEquipMsg" class="informationalText" runat="server" visible="false">
            <p>
                If Farm Extender Applied - The Farm Extender Endorsement includes Rented or Borrowed Equipment coverage with a limit of $150,000.
            </p>
        </div>
</div>
<div id="dvPeakSeason" runat="server" class="div" style="display: none">
    <table style="width: 100%" class="tableBorder">
        <tr>
            <td style="width: 5px"></td>
            <td style="width: 100px; vertical-align: central" class="CovTableColumn">
                <asp:CheckBox ID="chkPeakCovF" runat="server" AutoPostBack="true" />
                <asp:Label ID="lblPeakCovF" runat="server" Text=" Peak Season (F)"></asp:Label>
            </td>
        </tr>
    </table>
    <div id="dvPeakCovF" runat="server" style="display: none">
        <table style="width: 100%">
            <tr>
                <td style="width: 50px"></td>
                <td>
                    <asp:Label ID="lblPeakCovFLimit" runat="server" Text="Limit" Width="65px"></asp:Label>
                </td>
                <td>
                    <asp:Label ID="lblPeakCovFStart" runat="server" Text="Start Date" Width="75px"></asp:Label>
                </td>
                <td>
                    <asp:Label ID="lblPeakCovFEnd" runat="server" Text="End Date" Width="75px"></asp:Label>
                </td>
                <td>
                    <asp:Label ID="lblPeakCovFDesc" runat="server" Text="Description"></asp:Label>
                </td>
                <td style="width: 120px"></td>
            </tr>
        </table>
        <uc1:ctlFarmPeakSeasonList runat="server" ID="ctlFarmPeakSeasonList" />
        <div align="right">
            <asp:LinkButton ID="lnkAddPeakCovF" runat="server" CssClass="RemovePanelLink" Style="margin-right: 20px">Add New Peak Season</asp:LinkButton>
        </div>
        <div id="PeakNotice" class="informationalText" runat="server" visible="false">
            <p>
                Note: Existing and newly added Peak Season coverage will automatically be split if it is outside of the policy term after rating.
            </p>
        </div>
        <hr />
    </div>
    <asp:HiddenField ID="hiddenIncreasedLimit" runat="server" Value="0" />
    <asp:HiddenField ID="hiddenPeakExists" runat="server" />
    <asp:HiddenField ID="hiddenPeakType" runat="server" />
</div>
<div id="divEarthquakeScheduled" runat="server" class="div">
    <table style="width: 100%" class="tableBorder">
        <tr>
            <td style="width: 5px"></td>
            <td style="width: 100px; vertical-align: central" class="CovTableColumn">
                <asp:CheckBox ID="chkEarthquakeScheduled" runat="server" AutoPostBack="true" />
                <asp:Label ID="lblEarthquakeScheduled" runat="server" Text=" Earthquake"></asp:Label>
            </td>
        </tr>
    </table>
</div>