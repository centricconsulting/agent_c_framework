<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctlProperty_Address.ascx.vb" Inherits="IFM.VR.Web.ctlProperty_Address" %>
<%@ Register Src="~/User Controls/Application/FAR/AppSection/Locations/ctl_Farm_Location_Description_List.ascx" TagPrefix="uc1" TagName="ctl_Farm_Location_Description_List" %>


<script type="text/javascript">
    /// Show a confirmation dialog when the address has changed - SAVE LinkButtons
    $(document).ready(function ()
    {
        // Only perform this function against HOM and DFR quotes
        // 2 = Home Personal; 3 = Dwelling Fire Personal
        $("[id*='lnkSave']").each(function (i) {
            $(this).on('click', function (event) {
                //alert(ifm.vr.currentQuote.lobId);
                if ((ifm.vr.currentQuote.lobId == 2 || ifm.vr.currentQuote.lobId == 3) && $('#hdnPCCOrdered').val().toUpperCase() == 'Y')
                {
                    if ($('#<%=txtStreetNum.ClientID %>').val().toUpperCase() != $('#hdnStreetNum').val().toUpperCase() ||
                        $('#<%=txtStreetName.ClientID %>').val().toUpperCase() != $('#hdnStreetName').val().toUpperCase() ||
                        $('#<%=txtAptNum.ClientID %>').val().toUpperCase() != $('#hdnAptNum').val().toUpperCase() ||
                        $('#<%=txtCityName.ClientID %>').val().toUpperCase() != $('#hdnCityName').val().toUpperCase() ||
                        $('#<%=ddStateAbbrev.ClientID %> option:selected').text().toUpperCase() != $('#hdnStateAbbrev').val().toUpperCase() ||
                    $('#<%=txtZipCode.ClientID %>').val().toUpperCase() != $('#hdnZipCode').val().toUpperCase())
                    {
                        var okToContinue = confirm('Revising the location will result in another Protection Class lookup, do you want to continue?');
                        if (okToContinue)
                        {
                            // User clicked OK - save
                            return true;
                        }
                        else 
                        {
                            // User clicked CANCEL - don't save and restore the address values to their
                            // original value.
                            ifm.vr.ui.StopEventPropagation(event);
                            PopulateAddressFromHiddenFields();
                            ifm.ui.EnableEntirePage();
                            return false;
                        }
                    }
                }
            });
        });
    });

    /// Show a confirmation dialog when the address has changed - SUBMIT button
    $(document).ready(function ()
    {
        // Only perform this function against HOM and DFR quotes
        // 2 = Home Personal; 3 = Dwelling Fire Personal
        // Only perform the check when a report has already been ordered.
        $("[id*='ctlProperty_HOM_btnSubmit']").each(function (i) {
            $(this).on('click', function (event) {
                if ((ifm.vr.currentQuote.lobId == 2 || ifm.vr.currentQuote.lobId == 3) && $('#hdnPCCOrdered').val().toUpperCase() == 'Y')
                {
                    if ($('#<%=txtStreetNum.ClientID %>').val().toUpperCase() != $('#hdnStreetNum').val().toUpperCase() ||
                        $('#<%=txtStreetName.ClientID %>').val().toUpperCase() != $('#hdnStreetName').val().toUpperCase() ||
                        $('#<%=txtAptNum.ClientID %>').val().toUpperCase() != $('#hdnAptNum').val().toUpperCase() ||
                        $('#<%=txtCityName.ClientID %>').val().toUpperCase() != $('#hdnCityName').val().toUpperCase() ||
                        $('#<%=ddStateAbbrev.ClientID %> option:selected').text().toUpperCase() != $('#hdnStateAbbrev').val().toUpperCase() ||
                        $('#<%=txtZipCode.ClientID %>').val().toUpperCase() != $('#hdnZipCode').val().toUpperCase()) {
                        var okToContinue = confirm('Revising the location will result in another Protection Class lookup, do you want to continue?');
                        if (okToContinue) {
                            // User clicked OK - save
                            return true;
                        }
                        else {
                            // User clicked CANCEL - don't save and restore the address values to their
                            // original value.
                            ifm.vr.ui.StopEventPropagation(event);
                            PopulateAddressFromHiddenFields();
                            ifm.ui.EnableEntirePage();
                            return false;
                        }
                    }
                }
            });
        });
    });

    /// Show a confirmation dialog when the address has changed - SUBMIT button
    $(document).ready(function ()
    {
        // Only perform this function against HOM and DFR quotes
        // 2 = Home Personal; 3 = Dwelling Fire Personal
        // Only perform the check when a report has already been ordered.
        $("[id*='btnSaveGotoNextSection']").each(function (i) {
            $(this).on('click', function (event) {
                if ((ifm.vr.currentQuote.lobId == 2 || ifm.vr.currentQuote.lobId == 3) && $('#hdnPCCOrdered').val().toUpperCase() == 'Y')
                {
                    if ($('#<%=txtStreetNum.ClientID %>').val().toUpperCase() != $('#hdnStreetNum').val().toUpperCase() ||
                    $('#<%=txtStreetName.ClientID %>').val().toUpperCase() != $('#hdnStreetName').val().toUpperCase() ||
                    $('#<%=txtAptNum.ClientID %>').val().toUpperCase() != $('#hdnAptNum').val().toUpperCase() ||
                    $('#<%=txtCityName.ClientID %>').val().toUpperCase() != $('#hdnCityName').val().toUpperCase() ||
                    $('#<%=ddStateAbbrev.ClientID %> option:selected').text().toUpperCase() != $('#hdnStateAbbrev').val().toUpperCase() ||
                    $('#<%=txtZipCode.ClientID %>').val().toUpperCase() != $('#hdnZipCode').val().toUpperCase()) {
                        var okToContinue = confirm('Revising the location will result in another Protection Class lookup, do you want to continue?');
                        if (okToContinue) {
                            // User clicked OK - save
                            return true;
                        }
                        else {
                            // User clicked CANCEL - don't save and restore the address values to their
                            // original value.
                            ifm.vr.ui.StopEventPropagation(event);
                            PopulateAddressFromHiddenFields();
                            ifm.ui.EnableEntirePage();
                            return false;
                        }
                    }
                }
            });
        });
    });

    // Populates the address fields from the values in the hidden address fields.
    // Used when the user changes their mind about changing the address after the 
    // Verisk re-order warning.
    function PopulateAddressFromHiddenFields()
    {
        // Fill the address text fields with what's in the hidden fields
        $('#<%=txtStreetNum.ClientID %>').val($('#hdnStreetNum').val());
        $('#<%=txtStreetName.ClientID %>').val($('#hdnStreetName').val());
        $('#<%=txtAptNum.ClientID %>').val($('#hdnAptNum').val());
        $('#<%=txtCityName.ClientID %>').val($('#hdnCityName').val());
        $('#<%=txtZipCode.ClientID %>').val($('#hdnZipCode').val());

        // State is a little trickier because it's a dropdown
        var dd = document.getElementById('<%=ddStateAbbrev.ClientID%>');
        if (dd == null)
        {
            alert('dd is null!');
        }
        else 
        {
            for (var i = 0; i < dd.options.length; i++)
            {
                if (dd.options[i].text == $('#hdnStreetNum').val())
                {
                    dd.selectedindex = i;
                    break;
                }
            }
        }
    }
</script>

<asp:HiddenField ID="hdnStreetNum" runat="server" ClientIDMode="Static" />
<asp:HiddenField ID="hdnStreetName" runat="server" ClientIDMode="Static" />
<asp:HiddenField ID="hdnAptNum" runat="server" ClientIDMode="Static" />
<asp:HiddenField ID="hdnCityName" runat="server" ClientIDMode="Static" />
<asp:HiddenField ID="hdnStateAbbrev" runat="server" ClientIDMode="Static" />
<asp:HiddenField ID="hdnZipCode" runat="server" ClientIDMode="Static" />
<asp:HiddenField ID="hdnPCCOrdered" runat="server" ClientIDMode="Static" />

<div id="AddressDiv" runat="server" class="standardSubSection">
    <h3>
        <asp:Label ID="lblAccordHeader" runat="server" Text="Address"></asp:Label>
        <span style="float: right;">
            <asp:LinkButton ID="lnkClearAddress" runat="server" ToolTip="Clear Address" CssClass="RemovePanelLink" OnClientClick="return confirm('Clear Address?');">Clear</asp:LinkButton>
            <asp:LinkButton ID="lnkSaveAddress" runat="server" ToolTip="Save Page" CssClass="RemovePanelLink">Save</asp:LinkButton>
        </span>
    </h3>
    <div id="PropertyAddressContent">
        <section style="clear: both;">
            <div runat="server" id="divAddressInfoText" class="informationalText">
                If adding a location without the legal address please add the township in the Street # and the range for the Street Name. Example:  Street # 20W TWP, Street Name:  25N Range
            </div>
        </section>
        <section>
            <div class="insured" style="float: left; vertical-align: top; text-align: left;">
                <ul>
                    <li>
                        <div runat="server" id="tdAcerage">
                            <div style="display: inline-block;">
                                <label for="<%=txtAcerage.ClientID%>">*Acres:</label>
                                <br />
                                <asp:TextBox ID="txtAcerage" TabIndex="1" runat="server"></asp:TextBox>
                                <br />
                                <asp:Label ForeColor="Blue" Font-Bold="true" ID="lblAdditionalAcres" runat="server" Text=""></asp:Label>
                            </div>
                        </div>
                    </li>
                    <li>
                        <asp:Label ID="lblStreetNum" AssociatedControlID="txtStreetNum" runat="server" Text="*Street #:"></asp:Label>
                        <%--<label for="<%=Me.txtStreetNum.ClientID%>">*Street #:</label>--%><br />
                        <asp:TextBox ID="txtStreetNum" runat="server" TabIndex="3" MaxLength="35"></asp:TextBox>
                    </li>
                    <li>
                        <asp:Label ID="lblStreetName" AssociatedControlID="txtStreetName" runat="server" Text="*Street Name:"></asp:Label>
                        <%--<label for="<%=Me.txtStreetName.ClientID%>">*Street Name:</label>--%><br />
                        <asp:TextBox ID="txtStreetName" runat="server" TabIndex="4" MaxLength="65"></asp:TextBox>
                    </li>
                    <li>
                        <div runat="server" id="divAptNum">
                            <label for="<%=Me.txtAptNum.ClientID%>">Apt./Suite #:</label><br />
                            <asp:TextBox ID="txtAptNum" runat="server" TabIndex="5" MaxLength="30"></asp:TextBox>
                        </div>
                    </li>
                    <li>
                        <div runat="server" id="tdSection">
                            <label for="<%=txtSection.ClientID%>">*Section:</label>
                            <br />
                            <asp:TextBox ID="txtSection" TabIndex="6" MaxLength="15" runat="server"></asp:TextBox>
                        </div>
                    </li>
                    <li>
                        <div runat="server" id="tdTownship">
                            <label for="<%=txtTownship.ClientID%>">*Township:</label>
                            <br />
                            <asp:TextBox ID="txtTownship" TabIndex="7" MaxLength="15" runat="server"></asp:TextBox>
                        </div>
                    </li>
                <li><div runat="server" id="tdRange">
                            <label for="<%=txtRange.ClientID%>">*Range:</label>
                            <br />
                            <asp:TextBox ID="txtRange" MaxLength="15" TabIndex="8" runat="server"></asp:TextBox>
                        </div>
                    </li>
                    <%-- NEW MGB 2/27/17 - for BOP --%>
                    <li>
                        <div runat="server" id="divNumAmusementAreas">
                            <label for="<%=txtNumberOfAmusementAreas.ClientID%>">Number of Amusement Areas:</label>
                            <br />
                            <asp:TextBox ID="txtNumberOfAmusementAreas" MaxLength="2" TabIndex="9" runat="server" onkeypress='return event.charCode >= 48 && event.charCode <= 57'></asp:TextBox>
                        </div>
                    </li>
                    <%-- NEW MGB 2/27/17 - for BOP --%>
                    <li>
                        <div runat="server" id="divNumberOfPlaygrounds">
                            <label for="<%=txtNumberOfPlaygrounds.ClientID%>">Number of Playgrounds:</label>
                            <br />
                            <asp:TextBox ID="txtNumberOfPlaygrounds" MaxLength="2" TabIndex="10" runat="server" onkeypress='return event.charCode >= 48 && event.charCode <= 57'></asp:TextBox>
                        </div>
                    </li>
                    <%-- NEW MGB 2/27/17 - for BOP --%>
                    <li>
                        <div runat="server" id="divNumberOfSwimmingPools">
                            <label for="<%=txtRange.ClientID%>">Number of Swimming Pools:</label>
                            <br />
                            <asp:TextBox ID="txtNumberOfSwimmingPools" MaxLength="2" TabIndex="11" runat="server" onkeypress='return event.charCode >= 48 && event.charCode <= 57'></asp:TextBox>
                        </div>
                    </li>
                    <%-- NEW MGB 2/27/17 - for CAP --%>
                    <li>
                        <div runat="server" id="divPOBox">
                            <label for="<%=txtPOBox.ClientID%>">P.O. Box:</label>
                            <br />
                            <asp:TextBox ID="txtPOBox" MaxLength="10" TabIndex="11" runat="server" onkeypress='return event.charCode >= 48 && event.charCode <= 57'></asp:TextBox>
                        </div>
                    </li>
                    <%-- NEW MGB 8/09/17 - for WCP --%>
                    <li>
                        <div runat="server" id="divNumberOfEmployees">
                            <label for="<%=txtNumberOfEmployees.ClientID%>">*Number of Employees:</label>
                            <br />
                            <asp:TextBox ID="txtNumberOfEmployees" MaxLength="4" TabIndex="12" runat="server" onkeypress='return event.charCode >= 48 && event.charCode <= 57'></asp:TextBox>
                        </div>
                    </li>
                    <%-- NEW MGB 4/29/19 - for KY WCP --%>
                    <li>

                        <div runat="server" id="divNoOwnedLocation">
                            <br />
                            <asp:CheckBox ID="chkNoOwnedLocation" runat="server" Text="No owned location in this state" />
                        </div>
                    </li>

                </ul>
            </div>

            <div class="insured" style="vertical-align: top; text-align: left; width: 235px; float: right">
                <ul>
                    <li>
                        <asp:Button ID="btnCopyAddress" CssClass="StandardButton" TabIndex="-1" Style="margin-top: -2px;" runat="server" Text="Copy Address from Policyholder" />
                    </li>
                    <asp:UpdatePanel ID="up2" runat="server">
                        <ContentTemplate>
                            <li>
                                <label for="<%=Me.txtZipCode.ClientID%>">*ZIP:</label><br />
                                <asp:TextBox ID="txtZipCode" runat="server" TabIndex="13" MaxLength="10"></asp:TextBox>
                            </li>
                            <li>
                                <label for="<%=Me.ddCityName.ClientID%>">*City:</label><br />
                            <asp:DropDownList ID="ddCityName" CausesValidation="false" runat="server" ></asp:DropDownList>
                            <asp:TextBox ID="txtCityName" runat="server" TabIndex="14" MaxLength="25" ></asp:TextBox>
                            </li>
                            <li>
                                <label for="<%=Me.ddStateAbbrev.ClientID%>">*State:</label><br />
                                <asp:DropDownList ID="ddStateAbbrev" runat="server" TabIndex="15">
                                </asp:DropDownList>
                            </li>
                            <li>
                                <label for="<%=Me.txtGaragedCounty.ClientID%>">*County:</label><br />
                            <asp:TextBox ID="txtGaragedCounty" runat="server" TabIndex="16" MaxLength="11" ></asp:TextBox>
                            </li>
                            <%-- NEW MGB 11/28/17 - for CPR --%>
                            <li>
                                <div runat="server" id="divFeetToHydrant">
                                    <label for="<%=txtFeetToHydrant.ClientID%>">Feet to Hydrant</label>
                                    <br />
                                <asp:TextBox ID="txtFeetToHydrant" MaxLength="5" TabIndex="17" runat="server" style="width:30%;" AutoPostBack="true" onkeypress='return event.charCode >= 48 && event.charCode <= 57'></asp:TextBox>
                                </div>
                            </li>
                            <%-- NEW MGB 11/28/17 - for CPR --%>
                            <li>
                                <div runat="server" id="divMilesToFireDept">
                                    <label for="<%=txtMilesToFireDept.ClientID%>">Miles to Fire Department</label>
                                    <br />
                                <asp:TextBox ID="txtMilesToFireDept" MaxLength="2" TabIndex="18" runat="server" style="width:30%;" AutoPostBack="true" onkeypress='return event.charCode >= 48 && event.charCode <= 57'></asp:TextBox>
                                </div>
                            </li>
                            <%-- NEW MGB 11/28/17 - for CPR --%>
                            <li>
                                <div runat="server" id="divProtectionClass">
                                    <label for="<%=ddProtectionClass.ClientID%>">*Protection Class</label>
                                    <br />
                                <asp:DropDownList ID="ddProtectionClass" runat="server" TabIndex="19" style="width:100%;"></asp:DropDownList>
                                </div>
                            </li>
                            <%-- CAH 9/11/2024 - for Verisk API call --%>
                            <li>
                                <div runat="server" id="divVeriskFeetToHydrant">
                                    <label for="<%=ddlVeriskFeetToHydrant.ClientID%>">*Feet to Hydrant</label><br />
                                    <asp:DropDownList ID="ddlVeriskFeetToHydrant" runat="server" TabIndex="15"></asp:DropDownList>
                                </div>
                            </li>
                            <%-- CAH 9/11/2024 - for Verisk API call --%>
                            <li>
                                <div runat="server" id="divVeriskProtectionClass">
                                    <label for="<%=txtVeriskProtectionClass.ClientID%>">Protection Class</label>
                                    <br />
                                <asp:TextBox ID="txtVeriskProtectionClass" runat="server" TabIndex="16" MaxLength="11" ></asp:TextBox>
                                </div>
                            </li>
                        </ContentTemplate>
                        <Triggers>
                        <asp:AsyncPostBackTrigger ControlID ="txtCityName" EventName ="TextChanged" />
                        <asp:AsyncPostBackTrigger ControlID ="txtGaragedCounty" EventName ="TextChanged" />
                        <asp:AsyncPostBackTrigger ControlID ="txtZipCode" EventName ="TextChanged" />
                        <asp:AsyncPostBackTrigger ControlID ="ddCityName" EventName ="SelectedIndexChanged" />
                        <asp:AsyncPostBackTrigger ControlID ="txtFeetToHydrant" EventName ="TextChanged" />
                        <asp:AsyncPostBackTrigger ControlID ="txtMilesToFireDept" EventName ="TextChanged" />
                        </Triggers>
                    </asp:UpdatePanel>
                    <li>
                        <div runat="server" id="tdTownshipName">
                            <label for="<%=ddTownshipName.ClientID%>">Township Name:</label>
                            <br />
                            <asp:DropDownList ID="ddTownshipName" TabIndex="20" runat="server" Height="21px">
                            </asp:DropDownList>
                            <asp:HiddenField ID="hdnTownshipName" Value="" runat="server" />
                        </div>
                    </li>
                    <li>
                        <div runat="server" id="tdDescription">
                            <label for="<%=txtDescrption.ClientID%>">*Location Description:</label>
                            <br />
                            <asp:TextBox ID="txtDescrption" TextMode="MultiLine" MaxLength="255" TabIndex="21" Width="180px" runat="server"></asp:TextBox>
                        </div>
                    </li>
                </ul>
            </div>
        </section>
        <section  style="clear: both">
            <div runat="server" id="divBlanketAcreage" visible="false">

                
                <asp:CheckBox ID="chkBlanketAcreage" runat="server" Text="Blanket Acreage" />
                <span runat="server" id="divtxtTotalBlanketAcreage">
                
                    <label  style="margin-left:30px""  tabindex="12" for="<%=txtTotalBlanketAcreage.ClientID%>">Total Blanket Acreage:</label>
                    <asp:TextBox ID="txtTotalBlanketAcreage" MaxLength="10" TabIndex="12" runat="server" onkeypress='return (event.charCode >= 48 && event.charCode <= 57) || event.charCode == 46'></asp:TextBox>
                </span>
            </div>
        </section>
        <section style="clear: both; margin-top:20px">
            <uc1:ctl_Farm_Location_Description_List runat="server" ID="ctl_Farm_Location_Description_List" Visible="false" />
        </section>


    </div>

</div>
<asp:HiddenField ID="hiddenAddressIsActive" runat="server" />

