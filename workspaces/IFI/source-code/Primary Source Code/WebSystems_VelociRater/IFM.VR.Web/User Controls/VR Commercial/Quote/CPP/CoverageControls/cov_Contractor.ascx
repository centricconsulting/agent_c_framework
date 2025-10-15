<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="cov_Contractor.ascx.vb" Inherits="IFM.VR.Web.cov_Contractor" %>
<%@ Register Src="~/User Controls/VR Commercial/Quote/CPP/CoverageControls/cov_Contractor_Item.ascx" TagPrefix="uc1" TagName="cov_Contractor_Item" %>

<table id="ceSubGroup" runat="server" class="ItemGroup" style="border-collapse: collapse; display: table;">
    <tr>
        <td>
            <div id="divContractorOption" runat="server">
                <asp:CheckBox ID="chkContractorsEquipment" runat="server" class="chkOption" Text="Contractors Equipment" />
                <asp:Button ID="clearButton" class="hiddenclearbutton" runat="server" Text=""  style="display: none"/>
            </div>
        </td>
    </tr>
    <tr>
        <td>
            <div id="divContractorDetail" runat="server" class="chkDetail" style="display: none;">
                <table id="Table2" runat="server" style="border-collapse: collapse; width: 100%; display: table; margin: 5px 15px;" class="qs_grid_4_columns">
                    <tr>
                        <td>
                            <table class="qs_grid_4_columns">
                                <tbody>
                                    <tr>
                                        <td>Deductible</td>
                                        <td>CoInsurance</td>
                                        <td></td>
                                        <td></td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <asp:DropDownList ID="ceDeductible" runat="server" class="form10Em">
                                            </asp:DropDownList>
                                        </td>
                                        <td>
                                            <asp:DropDownList ID="ceCoinsurance" runat="server" class="form10Em">
                                            </asp:DropDownList>
                                        </td>
                                        <td></td>
                                        <td></td>
                                    </tr>
                                </tbody>

                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="4"> 
                            <uc1:cov_Contractor_Item runat="server" ID="cov_Contractor_Item" />
                        </td>
                    </tr>
                    <tr>
                        <td class="ItemGroup" colspan="4">
                            <asp:CheckBox ID="chkUnscheduledTools" runat="server" Text="Unscheduled Tools" class="chkOption2"/>
                            <div id="divCeUnscheduledTools" runat="server" style="display: none; padding-top: 5px; padding-left: 20px;" class="chkDetail">
                                <table class="" style="width: 100%">
                                    <tr>
                                        <td style="width:138px">*Limit</td>
                                        <td></td>
                                        <td></td>
                                        <td></td>
                                    </tr>
                                    <tr>
                                        <td style="width:138px">
                                            <asp:TextBox ID="txtUnscheduledToolsLimit" onkeyup='$(this).val(FormatAsNumber($(this).val()," ,[A-Za-z],$,-"));' runat="server"></asp:TextBox>
                                        </td>
                                        <td colspan="3">
                                            <span class="informationalText coverageMessage" id="coverageMessage" runat="server" visible="true">(This coverage includes your Unscheduled Tools, Employee Tools and Equipment Leased/Rented from Others)</span>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td  colspan="4" style="padding-right: 2em; text-align: center;"><br />
                                            <span class="informationalText">Maximum per tool limit for Unscheduled Tools is $2,000. If higher limits are desired, contact your underwriter.</span>
                                        </td>
                                    </tr>
                                </table>
                            </div>
                        </td>
                    </tr>


                    <tr>
                        <td  colspan="4" style="padding-right: 2em; text-align: center;"><br />
                            <span class="informationalText">Your binding authority for Contractors Equipment is a maximum limit of $1,000,000 with a per item maximum of $500,000.</span>
                        </td>
                    </tr>
                </table>
            </div>
        </td>
    </tr>

</table>
