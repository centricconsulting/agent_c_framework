<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="cov_BuildersRisk.ascx.vb" Inherits="IFM.VR.Web.cov_BuildersRisk" %>
<%@ Register Src="~/User Controls/VR Commercial/Quote/CPP/CoverageControls/cov_BuildersRisk_Item.ascx" TagPrefix="uc1" TagName="cov_BuildersRisk_Item" %>

<table id="brSubGroup" runat="server" class="ItemGroup" style="border-collapse: collapse; display: table;">
    <tr>
        <td>
            <div id="divBuildersRiskOption" runat="server">
                <asp:CheckBox ID="chkBuildersRisk" runat="server" class="chkOption" Text="Builders Risk - Scheduled"  />
                <asp:Button ID="clearButton" class="hiddenclearbutton" runat="server" Text=""  style="display: none"/>
            </div>
        </td>
    </tr>
    <tr>
        <td>
            <div id="divBuildersRiskDetail" runat="server" class="chkDetail" style="display: none;">
                <table id="Table2" runat="server" style="border-collapse: collapse; width: 100%; display: table; margin: 5px 15px;">
                    <tr>
                        <td>Deductible</td>
                    </tr>
                    <tr>
                        <td>
                            <asp:DropDownList ID="brDeductible" runat="server" class="form10Em">
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <table class="qs_grid_4_columns">
                                <uc1:cov_BuildersRisk_Item runat="server" ID="cov_BuildersRisk_Item" />
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="3" style="padding-right: 2em; text-align: center;"><span class="informationalText">Your binding authority for this coverage is a maximum per project limit of $500,000 and total catastrophe limit of $1,000,000.</span></td>
                    </tr>
                </table>
            </div>
        </td>
    </tr>

</table>
