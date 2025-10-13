<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctlVehicle_PPA.ascx.vb" Inherits="IFM.VR.Web.ctlVehicle_PPA" %>
<%@ Register Src="~/User Controls/Application/PPA/ctlVehicleAdditionalInterestList.ascx" TagPrefix="uc1" TagName="ctlVehicleAdditionalInterestList" %>
<%@ Register Src="~/User Controls/QuoteEdit/PPA/ctlVinLookup.ascx" TagPrefix="uc1" TagName="ctlVinLookup" %>

<h3>
    <asp:Label ID="lblAccordHeader" runat="server" Text="Label"></asp:Label>
    <span style="float: right;">
        <asp:LinkButton ID="lnkBtnReplace" CssClass="RemovePanelLink" ToolTip="Replace Vehicle" runat="server">Replace Vehicle</asp:LinkButton>
        <asp:LinkButton ID="lnkBtnAddvehicle" CssClass="RemovePanelLink" ToolTip="Add New Vehicle" runat="server">Add Vehicle</asp:LinkButton>
        <asp:LinkButton ID="lnkRemove" CssClass="RemovePanelLink" ToolTip="Remove this vehicle." runat="server">Remove</asp:LinkButton>
        <asp:LinkButton ID="lnkBtnSave" CssClass="RemovePanelLink" ToolTip="Save all vehicles." runat="server">Save</asp:LinkButton>
    </span>
</h3>

<div class="ctlVehicle_PPA_container">
    
    <div runat="server" ID="divBasicInfo">
        <table style="width: 100%" title="Basic Information">

            <tr>
                <td style="width: 260px;">
                    <label for="<%=Me.txtVinNumber.ClientID%>">*VIN:</label>
                    <br />
                    <asp:TextBox ID="txtVinNumber" TabIndex="1" runat="server" ToolTip="If Trailer and no Vin is available enter 'NONE'" MaxLength="30" autofocus></asp:TextBox>
                </td>
                <td>
                    <asp:CheckBox AutoPostBack="false" TabIndex="6" ID="chkVehicleHasALienholderOrLease" Text=" Vehicle has a lienholder or lease" ToolTip="Check if the vheicle has a lienholder or lease.  Improves the accuracy of the quote." runat="server" />
                    <br />
                    <asp:CheckBox AutoPostBack="true" TabIndex="7" ID="chkNamedNonOwner" Text=" Named Non-Owner Non-Specific Vehicle" ToolTip="Extends Personal Liability coverage when a driver is borrowing a vehicle but does not own it." runat="server" />
                    <br />
                    <asp:CheckBox AutoPostBack="true" TabIndex="8" ID="chkExtenedNonOwned" Text=" Extended Non-Owned" ToolTip="Extends Personal Liability coverage to a non-owned vehicle used regularly.  Example: Company car" runat="server" />
                </td>
            </tr>
            <tr>
                <td>
                    <span style="float: right; margin-right: 20px;">
                        <br />
                        <span>
                            <asp:TextBox ID="txtSymbol" ForeColor="Gray" onkeydown="return false;" ToolTip="Auto Symbols - Use Lookup below to determine these values" runat="server" Width="70px"></asp:TextBox>
                        </span>
                    </span>
                    <label for="<%=Me.txtYear.ClientID%>">*Year:</label>
                    <br />
                    <asp:TextBox ID="txtYear" TabIndex="2" runat="server" MaxLength="4" onkeyup='$(this).val(FormatAsNumericDigitsOnly($(this).val()));'></asp:TextBox>
                </td>
                <td>
                    <label for="<%=Me.ddBodyType.ClientID%>">*Body Type:</label>
                    <br />
                    <asp:DropDownList ID="ddBodyType" TabIndex="9" runat="server"></asp:DropDownList></td>
            </tr>
            <tr>
                <td>
                    <label for="<%=Me.txtMake.ClientID%>">*Make:</label>
                    <br />
                    <asp:TextBox ID="txtMake" TabIndex="3" runat="server" MaxLength="50"></asp:TextBox>
                    <asp:DropDownList ID="ddPerformance" Width="80" runat="server" Style="display: none;">
                    </asp:DropDownList>
                </td>
                <td>
                    <label for="<%=Me.ddUse.ClientID%>">Use</label><br />
                    <asp:DropDownList ID="ddUse" TabIndex="10" runat="server"></asp:DropDownList>
                </td>
            </tr>
            <tr>

                <td>
                    <label for="<%=Me.txtModel.ClientID%>">*Model:</label>
                    <br />
                    <span style="float: right; margin-right: 20px;">
                        <button id="btnVinLookup" tabindex="5" runat="server" title="VIN or Year/Make/Model Lookup" class="StandardButton" type="button" style="width:73px;">Lookup</button>
                    </span>
                    <asp:TextBox ID="txtModel" TabIndex="4" runat="server" MaxLength="50"></asp:TextBox>
                </td>
                <td>
                    <div runat="server" id="divAnnualMileage">
                        <label for="<%=Me.ddAnnualMileage.ClientID%>">*Annual Mileage</label>
                        <br />
                        <asp:DropDownList ID="ddAnnualMileage" runat="server"></asp:DropDownList>
                    </div>
                    <div>
                        <label for="<%=Me.txtCostNew.ClientID%>">*Cost New:</label>
                        <br />
                        <asp:TextBox ID="txtCostNew" onkeyup='$(this).val(FormatAsCurrency($(this).val(),""));' runat="server" MaxLength="11"></asp:TextBox>
                    </div>
                </td>
            </tr>
        </table>
    </div>

    <div runat="server" id="divVinLookup" style="margin-top: 30px; width: 100%; display: none;">
        <h3><span style="float: right;"><a href="#" onclick="StopEventPropagation(event); $(this).parent().parent().parent().hide();">Close</a></span>Vin/Model Lookup</h3>
        <div runat="server" style="overflow-x: auto; max-height: 220px;" id="divVinLookupContents">
        </div>
    </div>

    <div class="hidden">
        <uc1:ctlVinLookup runat="server" ID="ctlVinLookup" />
    </div>

    <div runat="server" id="divDriverAssignment" class="standardSubSection">
        <h3 id="h3DriverAssignment" runat="server">Driver Assignment<span style="float: right;">
            <asp:LinkButton ID="lnkBtnDriverAssSave" ToolTip="Save all Vehicles" runat="server">Save</asp:LinkButton></span></h3>
        <div>
            <table style="width: 100%;" title="Driver Assignments" runat="server" id="tblDriverAssignment">
                <tr>
                    <td style="width: 230px;">
                        <label for="<%=Me.ddPrincipalDriver.ClientID%>">*Principal Driver</label></td>
                    <td>
                        <asp:DropDownList ID="ddPrincipalDriver" Width="400" runat="server"></asp:DropDownList></td>
                </tr>
                <tr id="trOdriver1" runat="server">
                    <td>
                        <label for="<%=Me.ddOccDriver1.ClientID%>">Occasional Driver 1</label></td>
                    <td>
                        <asp:DropDownList ID="ddOccDriver1" Width="400" runat="server"></asp:DropDownList></td>
                </tr>
                <tr id="trOdriver2" runat="server">
                    <td>
                        <label for="<%=Me.ddOccDriver2.ClientID%>">Occasional Driver 2</label></td>
                    <td>
                        <asp:DropDownList ID="ddOccDriver2" Width="400" runat="server"></asp:DropDownList></td>
                </tr>
                <tr id="trOdriver3" runat="server">
                    <td>
                        <label for="<%=Me.ddOccDriver3.ClientID%>">Occasional Driver 3</label></td>
                    <td>
                        <asp:DropDownList ID="ddOccDriver3" Width="400" runat="server"></asp:DropDownList></td>
                </tr>
            </table>
        </div>
    </div>
    <asp:HiddenField ID="hiddenDriverAssignment" Value="0" runat="server" />
    <asp:HiddenField ID="hiddenVinLookup" Value="0" runat="server" />

    <div runat="server" id="divAutoSpecificdata" class="standardSubSection">
        <h3>Auto Specific Data <span style="float: right;">
            <asp:LinkButton ID="lnkBtnSpecificSave" ToolTip="Save all Vehicles" runat="server">Save</asp:LinkButton></span></h3>
        <div>
            <table style="width: 100%;" title="Auto Specific Data">

                <tr>
                    <td>
                        <label for="<%=Me.ddAirBags.ClientID%>">Airbags</label></td>
                    <td>
                        <asp:DropDownList ID="ddAirBags" runat="server"></asp:DropDownList></td>
                </tr>
                <tr>
                    <td>
                        <label for="<%=Me.ddAntiTheft.ClientID%>">Anti-Theft</label></td>
                    <td>
                        <asp:DropDownList ID="ddAntiTheft" runat="server"></asp:DropDownList></td>
                </tr>
            </table>
        </div>
    </div>
    <asp:HiddenField ID="hiddenAutoSpecificdata" Value="false" runat="server" />

    <div runat="server" id="divOtherVehicleinfo" class="standardSubSection">
        <h3>Other Vehicle Information <span style="float: right;">
            <asp:LinkButton ID="lnkBtnSaveOtherInfo" ToolTip="Save all Vehicles" runat="server">Save</asp:LinkButton></span></h3>
        <div>
            <table style="width: 100%" title="Other Vehicle Info">
                <tr id="trCustomEquipment" runat="server">
                    <td style="width: 230px;">
                        <label for="<%=Me.txtCustomEquipment.ClientID%>">Custom Equipment</label></td>
                    <td colspan="3">
                        <asp:TextBox ID="txtCustomEquipment" onkeyup='$(this).val(FormatAsCurrency($(this).val(),""));' runat="server"></asp:TextBox></td>
                </tr>
                <tr>
                    <td style="width: 230px;">
                        <label for="<%=Me.ddMotorCyleType.ClientID%>">*Motorcycle Type</label></td>
                    <td colspan="3">
                        <asp:DropDownList ID="ddMotorCyleType" runat="server"></asp:DropDownList></td>
                </tr>
                <tr>
                    <td>
                        <label for="<%=Me.txtHorsePower.ClientID%>">*Horsepower/CC's</label></td>
                    <td>
                        <asp:TextBox ID="txtHorsePower" onblur='$(this).val(FormatAsNumber($(this).val()));' runat="server" MaxLength="5"></asp:TextBox></td>
                    <td style="width: 230px;"><span runat="server" id="spanReqStatedAmt">*</span>
                        <label for="<%=Me.txtStatedAmt.ClientID%>">Stated Amount</label></td>
                    <td>
                        <asp:TextBox ID="txtStatedAmt" onkeyup='$(this).val(FormatAsCurrency($(this).val(),""));' runat="server" MaxLength="11"></asp:TextBox></td>
                </tr>
                <tr>
                    <td><span runat="server" id="spanReqActualCashVal">*</span>
                        <label for="<%=Me.txtCashValue.ClientID%>">Actual Cash Value</label></td>
                    <td>
                        <asp:TextBox ID="txtCashValue" onkeyup='$(this).val(FormatAsCurrency($(this).val(),""));' runat="server" MaxLength="11"></asp:TextBox></td>
                    <td></td>
                    <td title="NOTE: Custom Equipment value greater than $0 will require Underwriting review prior to issuance.">&nbsp;</td>
                </tr>
            </table>        
            <div class="informationalText" style="font-size:smaller;margin-left:25px;margin-right:25px;">
                Policy provides $1,500 included coverage. If total of Custom Equipment exceeds $1,500 please enter in the total amount of Custom Equipment. (eg total Custom Equipment is $3,500 which includes $1,500. Place $3,500 in Custom Equipment box) Claim Settlement is at Actual Cash Value. Documentation is required for coverage.
            </div>
        </div>
    </div>
    <asp:HiddenField ID="hiddenOtherInfo" Value="0" runat="server" />

    <div runat="server" id="divGaraged" class="standardSubSection">
        <h3>Garaged Information  <span style="float: right;">
            <asp:LinkButton ID="lnkBtnGaragedSave" ToolTip="Save all Vehicles" runat="server">Save</asp:LinkButton></span></h3>
        <div>
            <table style="width: 100%" title="Garaged Information">
                <tr id="trCopyGaragingAddress" runat="server">
                    <td colspan="2">
                        <asp:Button ID="btnCopyGaragingAddress" runat="server" Style="margin-left: 20px;" Text="Copy Garaging Address" CssClass="StandardButton" />
                    </td>
                </tr>
                <tr>
                    <td style="width: 230px;">
                        <label for="<%=Me.txtGaragedStreetNum.ClientID%>">Street #</label></td>
                    <td>
                        <asp:TextBox ID="txtGaragedStreetNum" runat="server" MaxLength="35"></asp:TextBox></td>
                </tr>
                <tr>
                    <td>
                        <label for="<%=Me.txtGaragedStreet.ClientID%>">Street Name</label></td>
                    <td>
                        <asp:TextBox ID="txtGaragedStreet" runat="server" MaxLength="65"></asp:TextBox></td>
                </tr>
                <tr>
                    <td>
                        <label for="<%=Me.txtGaragedApt.ClientID%>">Apt/Suite Number</label></td>
                    <td>
                        <asp:TextBox ID="txtGaragedApt" runat="server" MaxLength="30"></asp:TextBox></td>
                </tr>
                <tr style="display: none;">
                    <td>
                        <label for="<%=Me.txtGaragedOtherInfo.ClientID%>">Other Info</label></td>
                    <td>
                        <asp:TextBox ID="txtGaragedOtherInfo" runat="server" MaxLength="100"></asp:TextBox></td>
                </tr>
                <tr>
                    <td>
                        <label for="<%=Me.txtGaragedCity.ClientID%>">City</label></td>
                    <td>
                        <asp:TextBox ID="txtGaragedCity" runat="server" MaxLength="25"></asp:TextBox></td>
                </tr>
                <tr>
                    <td>
                        <label for="<%=Me.ddGaragedState.ClientID%>">State</label></td>
                    <td>
                        <asp:DropDownList ID="ddGaragedState" runat="server"></asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td>
                        <label for="<%=Me.txtGaragedZip.ClientID%>">Zip</label></td>
                    <td>
                        <asp:TextBox ID="txtGaragedZip" onblur="$(this).val(formatPostalcode($(this).val()));" runat="server" MaxLength="10"></asp:TextBox></td>
                </tr>
                <tr>
                    <td>
                        <label for="<%=Me.txtGaragedCounty.ClientID%>">County</label></td>
                    <td>
                        <asp:TextBox ID="txtGaragedCounty" runat="server" MaxLength="11"></asp:TextBox></td>
                </tr>
            </table>
        </div>
    </div>
    <uc1:ctlVehicleAdditionalInterestList runat="server" ID="ctlVehicleAdditionalInterestList" />
    <asp:HiddenField ID="hiddenGaraged" Value="false" runat="server" />
    <asp:HiddenField ID="hidden_VehicleListActive" runat="server" />
    <asp:HiddenField ID="hdnHasReplacedVehicle" runat="server" />
    <asp:HiddenField ID="HiddenLookupWasFired" Value="0" runat="server" />
</div>