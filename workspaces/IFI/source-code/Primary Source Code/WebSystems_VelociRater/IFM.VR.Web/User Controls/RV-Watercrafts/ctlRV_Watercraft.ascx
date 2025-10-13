<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctlRV_Watercraft.ascx.vb" Inherits="IFM.VR.Web.ctlRV_Watercraft" %>




<h3>
    <asp:Label ID="lblCommonRVWatercraftHdr" runat="server"></asp:Label>
    <span style="float: right;">
        <asp:LinkButton ID="lnkAdd" runat="server" ToolTip="Add RV/Watercraft" CssClass="RemovePanelLink">Add RV/WC</asp:LinkButton>
        <asp:LinkButton ID="lnkRemove" CssClass="RemovePanelLink" ToolTip="Remove this RV/Watercraft" runat="server">Remove</asp:LinkButton>
        <asp:LinkButton ID="lnkSave" CssClass="RemovePanelLink" ToolTip="Save ALL" runat="server">Save</asp:LinkButton>
    </span>
</h3>
<div runat="server" id="divContents">
    <table>
        <tr>
            <td>
                <asp:Panel ID="pnlVehType" runat="server">
                    <asp:Label ID="lblVehType" runat="server" Text="*Type"></asp:Label>
                    <br />
                    <asp:DropDownList ID="ddlVehType" runat="server"></asp:DropDownList>
                </asp:Panel>
            </td>
            <td></td>
            <tr>
                <td colspan="2">
                    <section style="clear: both;">
                        <div runat="server" id="divVehTypeoWatercraftText" class="informationalText" style="display: none">
                            If Farm All Star is included and there is an Outboard Motor which exceeds 50 HP, separate Liability coverage is not needed
                        </div>
                    </section>
                </td>
                <td></td>
            </tr>
        </tr>
    </table>
    <asp:Panel ID="pnlVehicleInfo" runat="server">
        <table style="width: 100%">
            <tr>
                <td style="vertical-align: top">
                    <asp:Panel ID="pnlCoverageOptions" runat="server">
                        <asp:Label ID="lblCoverageOptions" runat="server" Text="*Coverage Options" Font-Bold="true" />
                        <br />
                        <asp:DropDownList ID="ddlCoverageOptions" runat="server" />
                        <asp:Label ID="lblCoverageOptionPDL" runat="server" Text="Physical Damage and Liability" />
                        <asp:Label ID="lblCoverageOptionPD" runat="server" Text="Physical Damage Only" />
                        <asp:Label ID="lblCoverageOptionLib" runat="server" Text="LIABILITY ONLY" />
                        <br />
                    </asp:Panel>
                    <asp:Panel ID="pnlPropertyDeductible" runat="server">
                        <asp:Label ID="lblPropertyDeductible" runat="server" Text="*Physical Damage Deductible" />
                        <br />
                        <asp:DropDownList ID="ddlPropertyDeductible" runat="server" Width="90px" />
                        <br />
                        <br />
                    </asp:Panel>
                    <asp:Panel ID="pnlPropertyDeductibleTextBox" runat="server">
                        <asp:Label ID="lblPropertyDeductibleTextBox" runat="server" Text="Physical Damage Deductible" />
                        <br />
                        <asp:TextBox ID="txtPropertyDeductibleTextBox" runat="server" Width="90px" Enabled="false" />
                        <br />
                        <br />
                    </asp:Panel>
                    <asp:Panel ID="pnlVehYear" runat="server">
                        <asp:Label ID="lblVehYear" runat="server" Text="*Year"></asp:Label>
                        <br />
                        <asp:TextBox ID="txtVehYear" runat="server" Width="90px" onkeyup='$(this).val(FormatAsNumericDigitsOnly($(this).val()));' MaxLength="4"></asp:TextBox>
                        <br />
                        <br />
                    </asp:Panel>
                    <asp:Panel ID="pnlVehLength" runat="server">
                        <asp:Label ID="lblVehLength" runat="server" Text="*Length in Feet"></asp:Label>
                        <br />
                        <asp:TextBox ID="txtVehLength" runat="server" Width="90px" onkeyup='$(this).val(FormatAsNumericDigitsOnly($(this).val()));' MaxLength="2"></asp:TextBox>
                        <br />
                        <br />
                    </asp:Panel>
                    <asp:Panel ID="pnlVehCostNew" runat="server">
                        <asp:Label ID="lblVehCostNew" runat="server" Text="*Cost New"></asp:Label>
                        <asp:Label ID="lblLimit" runat="server" Text="*Limit"></asp:Label>
                        <asp:TextBox ID="txtVehCostNew" runat="server" Width="90px" onkeyup='$(this).val(ifm.vr.stringFormating.asNumberWithCommas($(this).val()));' MaxLength="7"></asp:TextBox>
                        <br />
                        <br />
                    </asp:Panel>
                    <asp:Panel ID="pnlMotorType" runat="server">
                        <asp:Label ID="lblMotorType" runat="server" Text="*Motor Type"></asp:Label>
                        <asp:Button ID="HiddenMotorButton" runat="server" Text="Hidden" Style="display: none;" />
                        <br />
                        <asp:DropDownList ID="ddlMotorType" runat="server"></asp:DropDownList>
                        <asp:Label ID="lblMotorTypeOut" runat="server" Width="90px" Text="Outboard"></asp:Label>
                        <asp:Label ID="lblMotorTypeIn" runat="server" Width="90px" Text="Inboard"></asp:Label>
                        <asp:Label ID="lblMotorTypeInOut" runat="server" Width="90px" Text="Inboard/Outboard"></asp:Label>
                        <br />
                    </asp:Panel>
                    <asp:Panel ID="pnlRVWLiabilityOnly" runat="server">
                        <asp:Label ID="lblRVWLiabilityOnly" runat="server" Text="Liability Only"></asp:Label>
                        <br />
                        <asp:CheckBox ID="chkRVWLiabilityOnly" runat="server" />
                        <br />
                        <br />
                    </asp:Panel>
                </td>
                <td style="vertical-align: top">
                    <asp:Panel ID="pnlBodilyInjuryLimit" runat="server">
                        <asp:Label ID="lblBodilyInjuryLimit" runat="server" Text="Uninsured Watercraft Bodily Injury Limit"></asp:Label>
                        <br />
                        <asp:DropDownList ID="ddlBodilyInjuryLimit" runat="server"></asp:DropDownList>
                        <br />
                        <br />
                    </asp:Panel>
                    <asp:Panel ID="pnlUnder25Operator" runat="server">
                        <table style="width: 100%">
                            <tr style="vertical-align: top">
                                <td>
                                    <asp:CheckBox ID="chkUnder25Operator" runat="server" />
                                    <asp:Label ID="lblUnder25Operator" runat="server" Text=" Any operators of this item under 25?"></asp:Label>
                                </td>
                            </tr>
                            <tr style="vertical-align: top">
                                <td style="min-height: 14px; height: 14px">
                                    <asp:Label ID="lblUnder25OperMsg" runat="server" Text="Name and DOB required to issue" Font-Size="11px" ForeColor="Red"></asp:Label>
                                </td>
                            </tr>
                        </table>
                        <br />
                    </asp:Panel>
                    <asp:Panel ID="pnlVehSerialNum" runat="server">
                        <asp:Label ID="lblVehSerialNum" runat="server" Text="Serial/Hull Number"></asp:Label>
                        <br />
                        <asp:TextBox ID="txtVehSerialNum" runat="server" Width="90px" MaxLength="30"></asp:TextBox>
                        <br />
                        <br />
                    </asp:Panel>
                    <asp:Panel ID="pnlVehMake" runat="server">
                        <asp:Label ID="lblVehMake" runat="server" Text="Manufacturer"></asp:Label>
                        <br />
                        <asp:TextBox ID="txtVehMake" runat="server" Width="90px" MaxLength="30"></asp:TextBox>
                        <br />
                        <br />
                    </asp:Panel>
                    <asp:Panel ID="pnlVehModel" runat="server">
                        <asp:Label ID="lblVehModel" runat="server" Text="Model"></asp:Label>
                        <br />
                        <asp:TextBox ID="txtVehModel" runat="server" Width="90px" MaxLength="30"></asp:TextBox>
                        <br />
                    </asp:Panel>
                    <asp:Panel ID="pnlOwnerOtherThanInsured" runat="server">
                        <br />
                        <asp:CheckBox ID="chkOwnerOtherThanInsured" runat="server" Text="Owned by someone other than insured" />
                    </asp:Panel>
                    <asp:Panel ID="pnlCollisionCoverage" runat="server">
                        <br />
                        <asp:CheckBox ID="chkCollisionCoverage" runat="server" Text="Add collision coverage" />
                    </asp:Panel>
                </td>
            </tr>
        </table>
    </asp:Panel>
    <asp:Panel ID="pnlDescription" runat="server">
        <asp:Label ID="lblDescription" runat="server" Text="*Description"></asp:Label>
        <br />
        <asp:TextBox ID="txtDescription" runat="server" TextMode="MultiLine" Width="100%"></asp:TextBox>
        <br />
        <asp:Label ID="lblMaxChar" runat="server" Text="Max Characters:" Font-Bold="True" Font-Size="XX-Small" ForeColor="Red" Visible="false"></asp:Label>&nbsp;
        <asp:Label ID="lblMaxCharCount" runat="server" Font-Bold="True" Font-Size="XX-Small" ForeColor="Red" Visible="false"></asp:Label>
    </asp:Panel>

    <div id="divMotorInput" runat="server">
        <h3 id="h3Motor" runat="server">Motor
            <span style="float: right;">
                <asp:LinkButton ID="lnkClearMotor" CssClass="RemovePanelLink" ToolTip="Clear Motor" runat="server">Clear</asp:LinkButton>
                <asp:LinkButton ID="lnkSaveMotor" CssClass="RemovePanelLink" ToolTip="Save ALL RV/Watercraft's" runat="server" Style="margin-left: 20px;">Save</asp:LinkButton>
            </span>
        </h3>
        <div>
            <table style="width: 100%">
                <tr>
                    <td style="vertical-align: top">
                        <asp:Panel ID="pnlHorsepowerCCs" runat="server">
                            <asp:Label ID="lblHorsepowerCCs" runat="server"></asp:Label>
                            <br />
                            <asp:TextBox ID="txtHorsepowerCCs" runat="server" Width="90px" onkeyup='$(this).val(FormatAsNumericDigitsOnly($(this).val()));' MaxLength="3"></asp:TextBox>
                            <br />
                            <br />
                        </asp:Panel>
                        <asp:Panel ID="pnlMotorYear" runat="server">
                            <asp:Label ID="lblMotorYear" runat="server" Text="*Year"></asp:Label>
                            <br />
                            <asp:TextBox ID="txtMotorYear" runat="server" Width="90px" onkeyup='$(this).val(FormatAsNumericDigitsOnly($(this).val()));' MaxLength="4"></asp:TextBox>
                            <br />
                            <br />
                        </asp:Panel>
                        <asp:Panel ID="pnlMotorCostNew" runat="server">
                            <asp:Label ID="lblMotorCostNew" runat="server" Text="*Cost New"></asp:Label>
                            <br />
                            <asp:TextBox ID="txtMotorCostNew" runat="server" Width="90px" onkeyup='$(this).val(FormatAsNumericDigitsOnly($(this).val()));' MaxLength="7"></asp:TextBox>
                            <br />
                        </asp:Panel>
                    </td>
                    <td style="vertical-align: top">
                        <asp:Panel ID="pnlMotorSerialNum" runat="server">
                            <asp:Label ID="lblMotorSerialNum" runat="server" Text="Serial Number"></asp:Label>
                            <br />
                            <asp:TextBox ID="txtMotorSerialNum" runat="server" Width="90px" MaxLength="30"></asp:TextBox>
                            <br />
                            <br />
                        </asp:Panel>
                        <asp:Panel ID="pnlMotorMake" runat="server">
                            <asp:Label ID="lblMotorMake" runat="server" Text="Manufacturer"></asp:Label>
                            <br />
                            <asp:TextBox ID="txtMotorMake" runat="server" Width="90px" MaxLength="30"></asp:TextBox>
                            <br />
                            <br />
                        </asp:Panel>
                        <asp:Panel ID="pnlMotorModel" runat="server">
                            <asp:Label ID="lblMotorModel" runat="server" Text="Model"></asp:Label>
                            <br />
                            <asp:TextBox ID="txtMotorModel" runat="server" Width="90px" MaxLength="30"></asp:TextBox>
                            <br />
                            <br />
                        </asp:Panel>
                    </td>
                </tr>
            </table>
        </div>
    </div>
    <asp:HiddenField ID="hiddenMotorInfo" Value="0" runat="server" />

    <%--<div runat="server" id="divYouthFul">
            <h3>Youngest Operator in the Household
                    <span style="float: right;">
                        <asp:LinkButton ID="lnkClearYouth" CssClass="RemovePanelLink" ToolTip="Clear Operator" runat="server">Clear</asp:LinkButton>
                        <asp:LinkButton ID="lnkSaveYouth" CssClass="RemovePanelLink" ToolTip="Save Page" runat="server">Save</asp:LinkButton>
                    </span>
            </h3>
            <div>
                <table style="width: 100%;">

                    <tr>
                        <td style="text-align: right">
                            <label for="<%=txtFirstName.ClientID %>">*First Name</label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtFirstName" runat="server" TabIndex="1" MaxLength="50"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td style="text-align: right">
                            <label for="<%=Me.txtLastName.ClientID%>">*Last Name</label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtLastName" runat="server" TabIndex="3" MaxLength="50"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td style="text-align: right">
                            <label for="<%=Me.txtBirthDate.ClientID%>">*Birth Date</label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtBirthDate" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                </table>
            </div>
        </div>--%>

    <asp:Panel ID="pnlRvWaterButtons" runat="server">
        <table style="width: 100%">
            <tr>
                <td colspan="2"></td>
            </tr>
            <tr>
                <td>
                    <asp:Button ID="btnAddRVWater" runat="server" Text="Add RV/WC" CssClass="StandardSaveButton" />
                </td>
                <td>
                    <asp:Button ID="btnSaveRVWater" runat="server" Text="Save RV/WC Coverages" CssClass="StandardSaveButton" />
                </td>
                <td>
                    <asp:Button ID="btnRateRVWater" runat="server" Text="Rate this Quote" CssClass="StandardSaveButton" />
                </td>
            </tr>
        </table>
    </asp:Panel>
    <asp:HiddenField ID="hiddenSelectedCoverage" runat="server" />
    <asp:HiddenField ID="hiddenPhysicalDamage" runat="server" />
    <asp:HiddenField ID="hiddenSelectedForm" runat="server" />
    <asp:HiddenField ID="hiddenMotorType" runat="server" />
    <asp:HiddenField ID="hiddenWatercraftType" runat="server" />
    <asp:HiddenField ID="hiddenLOB" runat="server" />
    <asp:HiddenField ID="hiddenOccupancyCodeID" runat="server" />
    <asp:HiddenField ID="hiddenIsEndorsementQuote" runat="server" />
    <asp:HiddenField ID="hiddenMaxCharCount" runat="server" />
</div>
