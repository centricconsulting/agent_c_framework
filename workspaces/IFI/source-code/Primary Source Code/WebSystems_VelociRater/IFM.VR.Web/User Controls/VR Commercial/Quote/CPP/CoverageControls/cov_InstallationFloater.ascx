<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="cov_InstallationFloater.ascx.vb" Inherits="IFM.VR.Web.cov_InstallationFloater" %>

<table id="cpSubGroup" runat="server" class="ItemGroup" style="border-collapse: collapse; display: table;width: 100%;">
    <tr>
        <td>
            <div id="divInstallationFloaterOption" runat="server">
                <asp:CheckBox ID="chkInstallationFloater" runat="server" class="chkOption" Text="Installation Floater" />
                <asp:Button ID="clearButton" class="hiddenclearbutton" runat="server" Text=""  style="display: none"/>
            </div>
        </td>
    </tr>
    <tr>
        <td>
            <div id="divInstallationFloaterDetail" runat="server" class="chkDetail" style="display: none;">
                <table id="Table2" runat="server" style="border-collapse: collapse; width: 100%; display: table; margin: 5px 15px;" class="qs_grid_4_columns">
                    <tr>
                        <td>Deductible</td>
                        <td>Coinsurance</td>
                        <td colspan="2"></td>
                    </tr>
                    <tr>
                        <td>
                            <asp:DropDownList ID="ifDeductible" runat="server" class="form8Em">
                            </asp:DropDownList>
                        </td>
                        
                        <td>
                            <asp:DropDownList ID="ifCoinsurance" runat="server" class="form8Em">
                            </asp:DropDownList>
                        </td>
                        <td colspan="2"></td>
                    </tr>
                        <tr ID="withoutPackageLabel" runat="server" Visible="false">
                            <td>*Jobsite Limit</td>
                            <td>*Catastrophe Limit</td>
                        </tr>
                        <tr ID="withoutPackage" runat="server" Visible="false">
                            <td>
                                <asp:TextBox ID="txtJobSiteLimit" onkeyup='$(this).val(FormatAsNumber($(this).val()," ,[A-Za-z],$,-"));' runat="server"></asp:TextBox>
                            </td>
                            <td>
                                <asp:TextBox ID="txtCatLimitWithout" onkeyup='$(this).val(FormatAsNumber($(this).val()," ,[A-Za-z],$,-"));'  runat="server" Enabled="false"></asp:TextBox>
                            </td>
                        </tr>
                        <tr  ID="withPackageLabel" runat="server" Visible="false">
                            <td>Included in Jobsite Limit</td>
                            <td>Increased Limit</td>
                            <td>*Catastrophe Limit</td>
                        </tr>
                        <tr ID="withPackage" runat="server" Visible="false">
                            <td>
                               <asp:TextBox ID="txtIncludedJobSiteLmit" Enabled="false" runat="server" Text="10,000"></asp:TextBox>
                            </td>
                            <td>
                                <asp:TextBox ID="txtIncreasedLimit" onkeyup='$(this).val(FormatAsNumber($(this).val()," ,[A-Za-z],$,-"));' runat="server"></asp:TextBox>
                            </td>
                            <td>
                                <asp:TextBox ID="txtCatLimitWith" onkeyup='$(this).val(FormatAsNumber($(this).val()," ,[A-Za-z],$,-"));'  runat="server" Enabled="false"></asp:TextBox>
                            </td>
                        </tr>
                    <tr>
                        <td colspan="4" style="text-align: center;"><span class="informationalText">This coverage includes Storage, Transit and Testing coverage with limits of $5,000 each.    If the Contractors Enhancement is selected, these limits are increased to $10,000.</span></td>
                    </tr>
                    <tr>
                        <td colspan="4" style="text-align: center;"><span class="informationalText">If higher limits are desired, contact your underwriter. </span></td>
                    </tr>
                    <tr>
                        <td colspan="4" style="text-align: center;"><span class="informationalText">Your binding authority for this coverage is a maximum per project limit of $100,000 and a total catastrophe limit of $300,000.</span></td>
                    </tr>
                </table>
            </div>
        </td>
    </tr>

</table>
