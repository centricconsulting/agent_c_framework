<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctl_CAP_Vehicle.ascx.vb" Inherits="IFM.VR.Web.ctl_CAP_Vehicle" %>
<%@ Register Src="~/User Controls/VR Commercial/Application/CAP/ctl_CAP_App_Vehicle.ascx" TagPrefix="uc1" TagName="ctlVehicle_CAP" %>

<h3>
    <asp:Label ID="lblAccordHeader" runat="server" Text="Vehicle #0 - "></asp:Label>
    <span style="float: right;">
        <asp:LinkButton ID="lnkCopy" runat="server" CssClass="RemovePanelLink" Style="margin-left: 20px" ToolTip="Copy to new Vehicle">Copy Vehicle</asp:LinkButton>
        <asp:LinkButton ID="lnkNew" runat="server" CssClass="RemovePanelLink" Style="margin-left: 20px" ToolTip="Add New Vehicle">Add Vehicle</asp:LinkButton>
        <asp:LinkButton ID="lnkDelete" runat="server" CssClass="RemovePanelLink" Style="margin-left: 20px" ToolTip="Remove Vehicle">Remove Vehicle</asp:LinkButton>
        <asp:LinkButton ID="lnkSave" ToolTip="Save" runat="server" CssClass="RemovePanelLink">Save</asp:LinkButton>
    </span>
</h3>

<div id="divVehicleMainTable" runat="server">
    <style type="text/css">
        .CAPVEH_Column1 {
            width: 33%;
        }

        .CAPVEH_Column2 {
            width: 33%;
        }

        .CAPVEH_Column3 {
            width: 33%;
        }

        .CAPVEHCOV_LabelColumn {
            width: 30%;
            text-align: right;
        }

        .CAPVEHCOV_DataColumn {
            width: 70%;
            text-align: left;
        }
        .CAPCustomPaintJobOrWrap {
            padding-left: 20px;
        }
    </style>
    <table id="tblBasicVehicleInfo" runat="server" style="width: 100%;">
        <tr id="trVinLookupMessage" runat="server">
            <td colspan="3" style="width: 100%; text-align: center; font-weight: bold;" class="informationalText">Using the VIN lookup will result in a more accurate quote. 
                Quotes entered manually are subject to change based on VIN validation.
            </td>
        </tr>
        <tr>
            <td class="CAPVEH_Column1">
                <asp:Label ID="lblVIN" runat="server">VIN</asp:Label>
                <br />
                <asp:TextBox ID="txtVIN" TabIndex="1" runat="server" MaxLength="17"></asp:TextBox>
            </td>
            <td class="CAPVEH_Column2" style="vertical-align: bottom; padding-bottom: 2px;">
                <span>
                    <asp:Button ID="btnVinLookup" TabIndex="2" runat="server" ToolTip="VIN or Year/Make/Model Lookup" CssClass="StandardButton" Text="Lookup" Width="73px" />
                </span>
                <br /> <br />
                <span>
                    <asp:Button ID="btnVinReset" TabIndex="2" runat="server" ToolTip="VIN or Year/Make/Model Reset" CssClass="StandardButton" Text="Reset" Width="73px" />
                </span>
            </td>
            <td class="CAPVEH_Column3">
                <label id="lblCostNew" runat="server" for="<%=txtCostNew.ClientID%>">Cost New</label>
                <br />
                <asp:TextBox ID="txtCostNew" runat="server" TabIndex="6"></asp:TextBox>
                <br />
                <span class="informationalText" runat="server" style="float: left; font-weight: bold;">* to change cost new contact UW
                </span>
            </td>
        </tr>
        <tr>
            <td class="CAPVEH_Column1">
                <label for="<%=txtVehicleYear.ClientID%>">*Year</label>
                <br />
                <asp:TextBox ID="txtVehicleYear" runat="server" MaxLength="4" TabIndex="3"></asp:TextBox>
            </td>
            <td class="CAPVEH_Column2">&nbsp;
            </td>
            <td class="CAPVEH_Column3" rowspan="2">
                <label for="<%=ddlVehicleRatingType.ClientID%>">*Vehicle Rating Type</label>
                <br />
                <asp:DropDownList ID="ddlVehicleRatingType" runat="server" Width="100%" TabIndex="7">
                    <asp:ListItem></asp:ListItem>
                    <asp:ListItem Value="1">Private Passenger Type</asp:ListItem>
                    <asp:ListItem Value="9">Truck, Trailer, Tractor</asp:ListItem>
                    <asp:ListItem Value="7">Funeral Director</asp:ListItem>
                </asp:DropDownList>
            </td>
        </tr>
        <tr>
            <td class="CAPVEH_Column1">
                <label for="<%=txtVehicleMake.ClientID%>">*Make</label>
                <br />
                <asp:TextBox ID="txtVehicleMake" runat="server" TabIndex="4"></asp:TextBox>
            </td>
            <td class="CAPVEH_Column2">&nbsp;
            </td>
        </tr>
        <tr>
            <td class="CAPVEH_Column1">
                <label for="<%=txtVehicleModel.ClientID%>">*Model</label>
                <br />
                <asp:TextBox ID="txtVehicleModel" runat="server" TabIndex="5"></asp:TextBox>
            </td>
            <td class="CAPVEH_Column2">&nbsp;
            </td>
            <td class="CAPVEH_Column3">
                <label for="<%=txtClassCode.ClientID%>">Class Code</label>
                <br />
                <asp:TextBox ID="txtClassCode" runat="server" TabIndex="8"></asp:TextBox>
            </td>
        </tr>
    </table>

    <div runat="server" id="divVinLookup" style="margin-top: 30px; width: 100%; display: none;">
        <h3><span style="float: right;"><a href="#" onclick="StopEventPropagation(event); $(this).parent().parent().parent().hide();">Close</a></span>Vin/Model Lookup</h3>
        <div runat="server" style="overflow-x: auto; max-height: 220px;" id="divVinLookupContents">
        </div>
    </div>
    <asp:HiddenField ID="hiddenVinLookup" Value="0" runat="server" />
    <asp:HiddenField ID="HiddenLookupWasFired" Value="0" runat="server" />
    <asp:HiddenField ID="hdnValidVin" Value="False" runat="server" />
    <asp:HiddenField ID="hdnOtherThanCollisionSymbol" Value="" runat="server" />
    <asp:HiddenField ID="hdnCollisionSymbol" Value="" runat="server" />
    <asp:HiddenField ID="hdnLiabilitySymbol" Value="" runat="server" />
    <asp:HiddenField ID="hdnOtherThanCollisionOverride" Value="" runat="server" />
    <asp:HiddenField ID="hdnCollisionOverride" Value="" runat="server" />
    <asp:HiddenField ID="hdnLiabilityOverride" Value="" runat="server" />
    <%--VIN Lookup Validation Popup--%>
    <div id="divVinLookupValidation" runat="server" style="display: none;">
        <div>
            <div style="font-weight: bold;">Errors: </div>
            <br />
            VIN could not be validated. Please confirm.
        </div>

        <div style="text-align: center; margin-left: auto; margin-right: auto; margin-top: 20px; margin-bottom: 20px;">
            <asp:Button ID="btnVLVOK" CssClass="StandardButton" runat="server" Text="OK" />
        </div>
    </div>

    <!-- CLASS CODE LOOKUP SECTION -->
    <div id="divClassCodeLookup" runat="server">
        <h3>
            <asp:Label ID="lblCCLookupHeader" runat="server" Text="Class Code Lookup"></asp:Label>
            <span style="float: right;">
                <asp:LinkButton ID="lnkSave_ClassCode" ToolTip="Save" runat="server" CssClass="RemovePanelLink">Save</asp:LinkButton>
            </span>
        </h3>
        <table id="tblClassCodeLookup" runat="server" style="width: 100%;">
            <tr id="trUseCodeRow" runat="server">
                <td class="CAPVEH_Column1">
                    <%--Use Code--%>
                     <asp:HyperLink ID="lbUseCode" href="javascript:;" runat="server">Use Code</asp:HyperLink>
                </td>
                <td class="CAPVEH_Column2">
                    <asp:DropDownList ID="ddlUseCode" runat="server" Width="100%">
                        <asp:ListItem></asp:ListItem>
                        <asp:ListItem Value="20">Business</asp:ListItem>
                        <asp:ListItem Value="21">Personal</asp:ListItem>
                        <asp:ListItem Value="22">Antique Auto</asp:ListItem>
                        <asp:ListItem Value="23">Farm</asp:ListItem>
                        <asp:ListItem Value="28">Service</asp:ListItem>
                        <asp:ListItem Value="29">Retail</asp:ListItem>
                        <asp:ListItem Value="30">Commercial</asp:ListItem>
                        <asp:ListItem Value="2">Limousine</asp:ListItem>
                        <asp:ListItem Value="31">Hearse or Flower Car</asp:ListItem>
                    </asp:DropDownList>
                </td>
                <td class="CAPVEH_Column3">&nbsp;
                </td>
            </tr>
            <tr id="trOperatorTypeRow" runat="server">
                <td class="CAPVEH_Column1">Operator Type</td>
                <td class="CAPVEH_Column2">
                    <asp:DropDownList ID="ddlOperatorType" runat="server" Width="100%">
                        <asp:ListItem></asp:ListItem>
                        <asp:ListItem Value="1">No Operator Licensed less than 5 years</asp:ListItem>
                        <asp:ListItem Value="2">Licensed less than 5 years, not owner or principal operator</asp:ListItem>
                        <asp:ListItem Value="3">Owner or principal operator licensed less than 5 years</asp:ListItem>
                    </asp:DropDownList>
                </td>
                <td class="CAPVEH_Column3">&nbsp;
                </td>
            </tr>
            <tr id="trOperatorUseRow" runat="server">
                <td class="CAPVEH_Column1">Operator Use</td>
                <td class="CAPVEH_Column2">
                    <asp:DropDownList ID="ddlOperatorUse" runat="server" Width="100%">
                        <asp:ListItem></asp:ListItem>
                        <asp:ListItem Value="1">Not driven to work or school</asp:ListItem>
                        <asp:ListItem Value="2">To or from work less than 15 miles</asp:ListItem>
                        <asp:ListItem Value="3">To or from work 15 miles or more</asp:ListItem>
                    </asp:DropDownList>
                </td>
                <td class="CAPVEH_Column3">&nbsp;
                </td>
            </tr>
            <tr id="trSizeRow" runat="server">
                <td class="CAPVEH_Column1">Size</td>
                <td class="CAPVEH_Column2">
                    <asp:DropDownList ID="ddlSize" runat="server" Width="100%">
                        <asp:ListItem></asp:ListItem>
                        <asp:ListItem Value="18">Light Truck &lt; or equal 10,000 Pounds GVW</asp:ListItem>
                        <asp:ListItem Value="19">Medium Truck 10,001 to 20,000 Pounds GVW</asp:ListItem>
                        <asp:ListItem Value="20">Heavy Truck 20,001 to 45,000 Pounds GVW</asp:ListItem>
                        <asp:ListItem Value="21">Extra Heavy Truck &gt; 45,000 Pounds GVW</asp:ListItem>
                        <asp:ListItem Value="22">Heavy Truck-Tractors &lt; or equal 45,000 Pounds GVW</asp:ListItem>
                        <asp:ListItem Value="23">Extra Heavy Truck-Tractors &gt; 45,000 Pounds GVW</asp:ListItem>
                        <asp:ListItem Value="30">Trailer Types</asp:ListItem>
                    </asp:DropDownList>
                    <asp:HiddenField ID="hdnSize" Value="" runat="server" />
                </td>
                <td class="CAPVEH_Column3">&nbsp;
                </td>
            </tr>
            <tr id="trTrailerTypeRow" runat="server">
                <td class="CAPVEH_Column1">Trailer Type</td>
                <td class="CAPVEH_Column2">
                    <asp:DropDownList ID="ddlTrailerType" runat="server" Width="100%">
                        <asp:ListItem></asp:ListItem>
                        <asp:ListItem Value="0">N/A</asp:ListItem>
                        <asp:ListItem Value="2">Semitrailer</asp:ListItem>
                        <asp:ListItem Value="3">Trailer</asp:ListItem>
                        <asp:ListItem Value="4">Service or Utility Trailer < 2,001 load capacity</asp:ListItem>
                    </asp:DropDownList>
                </td>
                <td class="CAPVEH_Column3">&nbsp;
                </td>
            </tr>
            <tr id="trRadiusRow" runat="server">
                <td class="CAPVEH_Column1">Radius</td>
                <td class="CAPVEH_Column2">
                    <asp:DropDownList ID="ddlRadius" runat="server" Width="100%">
                        <asp:ListItem></asp:ListItem>
                        <asp:ListItem Value="1">Local, up to 50 miles</asp:ListItem>
                        <asp:ListItem Value="2">Intermediate, 51 to 200 miles</asp:ListItem>
                    </asp:DropDownList>
                </td>
                <td class="CAPVEH_Column3">&nbsp;
                </td>
            </tr>
            <tr id="trSecondaryClassRow" runat="server">
                <td class="CAPVEH_Column1">Secondary Class</td>
                <td class="CAPVEH_Column2">
                    <asp:DropDownList ID="ddlSecondaryClass" runat="server" Width="100%">
                        <asp:ListItem></asp:ListItem>
                        <asp:ListItem Value="26">Food Delivery</asp:ListItem>
                        <asp:ListItem Value="27">Farmers</asp:ListItem>
                        <asp:ListItem Value="28">Dump and Transit Mix</asp:ListItem>
                        <asp:ListItem Value="29">Contractors</asp:ListItem>
                        <asp:ListItem Value="30">Not Otherwise Specified</asp:ListItem>
                    </asp:DropDownList>
                </td>
                <td class="CAPVEH_Column3">
                    <div id="divSeasonalFarmUse" runat="server">
                        <asp:CheckBox ID="chkSeasonalFarmUse" runat="server" Text="Seasonal Farm Use?" />
                    </div>
                    <div id="divDumping" runat="server">
                        <asp:CheckBox ID="chkDumpingOperations" runat="server" Text="Dumping Operations" />
                    </div>
                </td>
            </tr>
            <tr id="trSecondaryClassTypeRow" runat="server">
                <td class="CAPVEH_Column1">Secondary Class Type</td>
                <td class="CAPVEH_Column2">
                    <asp:DropDownList ID="ddlSecondaryClassType" runat="server" Width="100%">
                        <asp:ListItem Value=""></asp:ListItem>
                        <asp:ListItem Value="1">Common Carrier</asp:ListItem>
                        <asp:ListItem Value="2">Contract Carrier (Not chemicals or Iron and Steel)</asp:ListItem>
                        <asp:ListItem Value="3">Contract Carrier Chemicals</asp:ListItem>
                        <asp:ListItem Value="4">Contract Iron and Steel</asp:ListItem>
                        <asp:ListItem Value="5">Exempt Carrier (other than livestock)</asp:ListItem>
                        <asp:ListItem Value="6">Exempt carrier livestock</asp:ListItem>
                        <asp:ListItem Value="7">Carrier</asp:ListItem>
                        <asp:ListItem Value="8">Tow Truck for Hire</asp:ListItem>
                        <asp:ListItem Value="9">All Other</asp:ListItem>
                        <asp:ListItem Value="10">Canneries</asp:ListItem>
                        <asp:ListItem Value="11">Fish and Seafood</asp:ListItem>
                        <asp:ListItem Value="12">Frozen Foods</asp:ListItem>
                        <asp:ListItem Value="13">Fruit and Vegetables</asp:ListItem>
                        <asp:ListItem Value="14">Meat or Poultry</asp:ListItem>
                        <asp:ListItem Value="15">Armored Cars</asp:ListItem>
                        <asp:ListItem Value="16">Film</asp:ListItem>
                        <asp:ListItem Value="17">Magazines and Newspapers</asp:ListItem>
                        <asp:ListItem Value="18">Mail and Parcel Post</asp:ListItem>
                        <asp:ListItem Value="19">Automobile Dismantler</asp:ListItem>
                        <asp:ListItem Value="20">Building Wrecking Operations</asp:ListItem>
                        <asp:ListItem Value="21">Garbage</asp:ListItem>
                        <asp:ListItem Value="22">Junk Dealer</asp:ListItem>
                        <asp:ListItem Value="23">Individual or Family Owned Corporation (not hauling livestock)</asp:ListItem>
                        <asp:ListItem Value="24">Livestock</asp:ListItem>
                        <asp:ListItem Value="25">Excavating</asp:ListItem>
                        <asp:ListItem Value="26">Sand and Gravel (other than quarrying)</asp:ListItem>
                        <asp:ListItem Value="27">Mining</asp:ListItem>
                        <asp:ListItem Value="28">Quarrying</asp:ListItem>
                        <asp:ListItem Value="29">Building - Commercial</asp:ListItem>
                        <asp:ListItem Value="30">Building - Private Dwelling</asp:ListItem>
                        <asp:ListItem Value="31">Repair or Service</asp:ListItem>
                        <asp:ListItem Value="32">Street and Road</asp:ListItem>
                        <asp:ListItem Value="33">Logging and Lumbering</asp:ListItem>
                    </asp:DropDownList>
                </td>
                <td class="CAPVEH_Column3">&nbsp;
                </td>
            </tr>
            <tr>
                <td class="CAPVEH_Column1">Class Code</td>
                <td class="CAPVEH_Column2">
                    <asp:Label ID="lblClassCode" runat="server"></asp:Label>
                </td>
                <td class="CAPVEH_Column3">&nbsp;
                </td>
            </tr>
        </table>
        <asp:HiddenField ID="hdnCC" runat="server" />
    </div>

    <!-- GARAGING ADDRESS SECTION -->
    <div id="divGaragingAddress" runat="server">
        <h3>
            <asp:Label ID="lblGaragingAddr" runat="server" Text="Vehicle Garaging Address"></asp:Label>
            <span style="float: right;">
                <asp:LinkButton ID="lnkSave_Garaging" ToolTip="Save" runat="server" CssClass="RemovePanelLink">Save</asp:LinkButton>
            </span>
        </h3>
        <table id="tblGaragingAddress" runat="server" style="width: 100%;">
            <tr>
                <td class="CAPVEH_Column1">&nbsp;</td>
                <td colspan="2">
                    <asp:CheckBox ID="chkUseGaragingAddress" runat="server" Text="Use Primary Garaging Address" AutoPostBack="true" />
                </td>
                <%--<td>&nbsp;</td>--%>
            </tr>
            <tr>
                <td class="CAPVEH_Column1">&nbsp;</td>
                <td class="CAPVEH_Column2">
                    <label for="<%=txtZip.ClientID%>">*ZIP</label>
                    <br />
                    <asp:TextBox ID="txtZip" runat="server" TabIndex="0"></asp:TextBox>
                </td>
                <td>&nbsp;</td>
            </tr>
            <tr>
                <td class="CAPVEH_Column1">&nbsp;</td>
                <td class="CAPVEH_Column2">
                    <label for="<%=txtCity.ClientID%>">*City</label>
                    <br />
                    <asp:TextBox ID="txtCity" runat="server" TabIndex="0"></asp:TextBox>
                </td>
                <td>&nbsp;</td>
            </tr>
            <tr>
                <td class="CAPVEH_Column1">&nbsp;</td>
                <td class="CAPVEH_Column2">
                    <label for="<%=txtState.ClientID%>">*State</label>
                    <br />
                    <asp:TextBox ID="txtState" runat="server" Width="35%"></asp:TextBox>
                </td>
                <td>&nbsp;</td>
            </tr>
            <tr>
                <td class="CAPVEH_Column1">&nbsp;</td>
                <td class="CAPVEH_Column2">
                    <label for="<%=txtCounty.ClientID%>">*County</label>
                    <br />
                    <asp:TextBox ID="txtCounty" runat="server" TabIndex="0"></asp:TextBox>
                </td>
                <td>&nbsp;</td>
            </tr>
        </table>
        <asp:HiddenField ID="hdnGaraging" runat="server" />
    </div>
    <uc1:ctlVehicle_CAP runat="server" ID="ctlVehicle" />
    <!-- VEHICLE COVERAGES SECTION -->
    <div id="divVehicleCoverages" runat="server">
        <h3>
            <asp:Label ID="lblVehCovs" runat="server" Text="Vehicle Coverages"></asp:Label>
            <span style="float: right;">
                <asp:LinkButton ID="lnkSave_VehCovs" ToolTip="Save" runat="server" CssClass="RemovePanelLink">Save</asp:LinkButton>
            </span>
        </h3>
        <table id="tblVehicleCoverages" runat="server" style="width: 100%;">
            <tr id="trLiabilityUMUIM" runat="server">
                <td>
                    <asp:CheckBox ID="chkLiabilityUMUIM" runat="server" Text="Liability UM/UIM" TabIndex="0" />
                </td>
            </tr>
            <tr id="trLiability" runat="server" style="display: none;">
                <td>
                    <asp:CheckBox ID="chkLiability" runat="server" Text="Liability" TabIndex="0" />
                </td>
            </tr>
            <tr>
                <td>
                    <asp:CheckBox ID="chkMedicalPayments" runat="server" Text="Medical Payments" TabIndex="0" />
                </td>
            </tr>
            <tr id="trUM" runat="server" style="display: none;">
                <td>
                    <asp:CheckBox ID="chkUM" runat="server" Text="UM" TabIndex="0" />
                </td>
            </tr>
            <tr id="trUMPDRow" runat="server" style="display: none;">
                <td>
                    <asp:CheckBox ID="chkUMPD" runat="server" Text="UMPD" TabIndex="0" />
                </td>
            </tr>
            <tr id="trUMPDLimitRow" runat="server" style="display: none;">
                <td>
                    <table style="width: 100%;">
                        <tr>
                            <td class="CAPVEHCOV_LabelColumn">Coverage Limit</td>
                            <td class="CAPVEHCOV_DataColumn">
                                <asp:TextBox ID="txtUMPDLimit" runat="server" Width="40%" ReadOnly="true" BackColor="LightGray" Text="15,000" TabIndex="0"></asp:TextBox>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr id="trUIM" runat="server" style="display: none;">
                <td>
                    <asp:CheckBox ID="chkUIM" runat="server" Text="UIM" TabIndex="0" />
                </td>
            </tr>
            <tr>
                <td>
                    <asp:CheckBox ID="chkComprehensive" runat="server" Text="Comprehensive" />
                </td>
            </tr>
            <tr id="trComprehensiveDeductibleRow" runat="server">
                <td>
                    <table style="width: 100%;">
                        <tr>
                            <td class="CAPVEHCOV_LabelColumn">Deductible</td>
                            <td class="CAPVEHCOV_DataColumn">
                                <asp:DropDownList ID="ddlComprehensiveDeductible" runat="server" Width="20%"></asp:DropDownList>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr id="trCustomPaintJobOrWrapComprehensive" style="display: none" runat="server">
                <td colspan="2" class="CAPCustomPaintJobOrWrap">
                    <asp:CheckBox ID="chkCustomPaintJobOrWrapComprehensive" runat="server" Text="Does this vehicle include a wrap or custom paint job?" />
                </td>
            </tr>
            <tr>
                <td>
                    <asp:CheckBox ID="chkCollision" runat="server" Text="Collision" />
                </td>
            </tr>
            <tr id="trCollisionDeductibleRow" runat="server">
                <td>
                    <table style="width: 100%;">
                        <tr>
                            <td class="CAPVEHCOV_LabelColumn">Deductible</td>
                            <td class="CAPVEHCOV_DataColumn">
                                <asp:DropDownList ID="ddlCollisionDeductible" runat="server" Width="20%"></asp:DropDownList>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr id="trCustomPaintJobOrWrapCollision" style="display: none" runat="server">
                <td colspan="2" class ="CAPCustomPaintJobOrWrap">
                    <asp:CheckBox ID="chkCustomPaintJobOrWrapCollision" runat="server" Text="Does this vehicle include a wrap or custom paint job?" />
                </td>
            </tr>
            <tr>
                <td>
                    <asp:CheckBox ID="chkRentalReimbursement" runat="server" Text="Rental Reimbursement (Included in Enhancement Endorsement)" />
                </td>
            </tr>
            <tr id="trRentalReimbursementDataRow" runat="server">
                <td>
                    <table style="width: 100%;">
                        <tr>
                            <td class="CAPVEHCOV_LabelColumn">Number of Days</td>
                            <td class="CAPVEHCOV_DataColumn">
                                <asp:TextBox ID="txtNumberOfDays" runat="server" Width="10%"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="CAPVEHCOV_LabelColumn">Daily Reimbursement</td>
                            <td class="CAPVEHCOV_DataColumn">
                                <asp:TextBox ID="txtDailyReimbursement" runat="server" Width="10%"></asp:TextBox>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:CheckBox ID="chkTowingAndLabor" runat="server" Text="Towing and Labor (Included in Enhancement Endorsement)" />
                </td>
            </tr>
        </table>
        <asp:HiddenField ID="hdnOriginalCostNew" runat="server" />
        <asp:HiddenField ID="hdnVehCov" runat="server" />
    </div>

    <asp:Button ID="btnAddVehicle" runat="server" Text="Add Vehicle" CssClass="StandardSaveButton" />

</div>
