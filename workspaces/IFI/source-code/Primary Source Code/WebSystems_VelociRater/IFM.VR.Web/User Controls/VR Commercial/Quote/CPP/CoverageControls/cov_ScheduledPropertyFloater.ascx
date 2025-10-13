<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="cov_ScheduledPropertyFloater.ascx.vb" Inherits="IFM.VR.Web.cov_ScheduledPropertyFloater" %>
<%@ Register Src="~/User Controls/VR Commercial/Quote/CPP/CoverageControls/cov_ScheduledPropertyFloater_Item.ascx" TagPrefix="uc1" TagName="cov_ScheduledPropertyFloater_Item" %>

<table id="spSubGroup" runat="server" class="ItemGroup" style="border-collapse: collapse; display: table;">
    <tr>
        <td>
            <div id="divScheduledPropertyFloaterOption" runat="server">
                <asp:CheckBox ID="chkScheduledPropertyFloater" runat="server" class="chkOption" Text="Scheduled Property Floater" />
                <asp:Button ID="clearButton" class="hiddenclearbutton" runat="server" Text=""  style="display: none"/>
            </div>
        </td>
    </tr>
    <tr>
        <td>
            <div id="divScheduledPropertyFloaterDetail" runat="server" class="chkDetail" style="display: none;">
                <table id="Table2" runat="server" style="border-collapse: collapse; width: 100%; display: table; margin: 5px 15px;">
                    <tr>
                        <td>Deductible</td>
                        <td>Coinsurance</td>
                    </tr>
                    <tr>
                        <td>
                            <asp:DropDownList ID="spDeductible" runat="server" class="form10Em">
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:DropDownList ID="spCoinsurance" runat="server" class="form10Em">
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <uc1:cov_ScheduledPropertyFloater_Item runat="server" ID="cov_ScheduledPropertyFloater_Item" />
                        </td>
                    </tr>
                    <tr>
                        <td colspan="3" style="padding-right: 2em; text-align: center;"><span class="informationalText">Your binding authority for this coverage is a maximum limit of $50,000.</span></td>
                    </tr>
                </table>
            </div>
        </td>
    </tr>
</table>
