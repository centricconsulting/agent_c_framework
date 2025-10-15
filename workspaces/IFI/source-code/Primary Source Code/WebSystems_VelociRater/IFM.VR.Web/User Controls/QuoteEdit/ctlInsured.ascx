<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctlInsured.ascx.vb" Inherits="IFM.VR.Web.ctlInsured" %>


<h3>
    <asp:Label ID="lblInsuredTitle" runat="server" Text="Label"></asp:Label>
    <span style="float: right;">
        <asp:LinkButton ID="lnkRemove" ToolTip="Clear this policyholder" CssClass="RemovePanelLink" runat="server">Clear</asp:LinkButton>
        <asp:LinkButton ID="lnkSave" CssClass="RemovePanelLink" runat="server">Save</asp:LinkButton>
    </span>
</h3>

<div runat="server" id="divInsuredInfo">
    <div id="trCopyFromDriver" runat="server">
        <input style="margin-left: 20px;" class="StandardButton" id="btnCopyFromDriver" type="button" title="Allows you to use the name part of a driver to create policyholder #2." value="Copy from a Driver" onclick="GetDriverCopyDropDown($(this).attr('id')); $(this).next().focus();" />
    </div>
    <center>
        <div id="divPrefillInfoText" runat="server" class="informationalText">
            Certain fields might be pre-populated with data from our database service.  You can modify the prefilled data, but please be aware any changes are subject to review by your underwriter.
        </div>
    </center>
    <div class="insured" style="float: left; vertical-align: top; text-align: left;">
        <div runat="server" id="divPersName">
            <ul>
                <li>
                    <label for="<%=txtFirstName.ClientID %>">*First Name:</label><br />
                    <asp:TextBox ID="txtFirstName" runat="server" TabIndex="1" MaxLength="50" autofocus></asp:TextBox>
                </li>
                <li>
                    <label for="<%=Me.txtMiddleName.ClientID%>">Middle Name:</label><br />
                    <asp:TextBox ID="txtMiddleName" runat="server" TabIndex="2" MaxLength="50"></asp:TextBox>
                </li>
                <li>
                    <label for="<%=Me.txtLastName.ClientID%>">*Last Name:</label><br />
                    <asp:TextBox ID="txtLastName" runat="server" TabIndex="3" MaxLength="50"></asp:TextBox>
                </li>
                <li>
                    <label for="<%=Me.ddSuffix.ClientID%>">Suffix:</label><br />
                    <asp:DropDownList ID="ddSuffix" runat="server" TabIndex="4">
                    </asp:DropDownList>
                </li>
                <li>
                    <label for="<%=Me.ddSex.ClientID%>">*Gender:</label><br />
                    <asp:DropDownList ID="ddSex" runat="server" TabIndex="5">
                    </asp:DropDownList>
                </li>
                <li>
                    <label for="<%=Me.txtSSN.ClientID%>">SSN:</label><br />
                    <asp:TextBox ID="txtSSN" runat="server" TabIndex="6" MaxLength="11"></asp:TextBox>
                </li>
                <li>
                    <label for="<%=Me.txtBirthDate.ClientID%>">*Birth Date:</label><br />
                    <asp:TextBox ID="txtBirthDate" runat="server" MaxLength="10" TabIndex="7"></asp:TextBox>
                </li>
            </ul>
        </div>
        <div runat="server" id="divCommName">
            <ul>
                <li>
                    <label for="<%=Me.txtBusinessName.ClientID%>">*Name:</label><br />
                    <asp:TextBox ID="txtBusinessName" runat="server" Width="200px" MaxLength="50" TabIndex="1" autofocus></asp:TextBox>
                </li>
                <li runat="server" id="dbaNameFields">
                    <label for="<%=Me.txtDBA.ClientID%>">DBA Name:</label><br />
                    <asp:TextBox ID="txtDBA" runat="server" MaxLength="50" Width="200px" TabIndex="2"></asp:TextBox>
                </li>
                <li>
                    <label for="<%=Me.ddBusinessType.ClientID%>">*Legal Entity:</label><br />
                    <asp:DropDownList ID="ddBusinessType" runat="server" TabIndex="3"></asp:DropDownList>
                </li>
                <li id="liOtherEntity" runat="server" style="display: none;">
                    <label for="<%=Me.txtOtherEntity.ClientID%>">*Other Legal Entity:</label><br />
                    <asp:TextBox ID="txtOtherEntity" runat="server" MaxLength="250" Width="200px" TabIndex="4"></asp:TextBox>
                </li>
                <li id="liTaxIdType" runat="server">
                    <label for="<%=Me.ddTaxIDType.ClientID%>">Tax ID Type:</label><br />
                    <asp:DropDownList ID="ddTaxIDType" runat="server" TabIndex="4">
                        <asp:ListItem Text="" Value=""></asp:ListItem>
                        <asp:ListItem Text="FEIN" Value="2"></asp:ListItem>
                        <asp:ListItem Text="SSN" Value="1"></asp:ListItem>
                    </asp:DropDownList>
                </li>
                <li id="liFEIN" runat="server" style="display: none;">
                    <label for="<%=Me.txtFEIN.ClientID%>">TIN:</label><br />
                    <asp:TextBox ID="txtFEIN" runat="server" TabIndex="5" MaxLength="11"></asp:TextBox>
                </li>
                <li id="liSSNBusiness" runat="server" style="display: none;">
                    <label for="<%=Me.txtSSNBusiness.ClientID%>">SSN:</label><br />
                    <asp:TextBox ID="txtSSNBusiness" runat="server" TabIndex="6" MaxLength="11"></asp:TextBox>
                </li>
                <li id="liBusinessStarted" runat="server">
                    <label for="<%=Me.txtBusinessStarted.ClientID%>">Business Started Date:</label><br />
                    <asp:TextBox ID="txtBusinessStarted" runat="server" TabIndex="7"></asp:TextBox>
                </li>
                <li id="liYearsExperience" runat="server">
                    <label for="<%=Me.txtYearsOfExperience.ClientID%>">Years of Experience:</label><br />
                    <asp:TextBox ID="txtYearsOfExperience" runat="server" onkeypress='return event.charCode >= 48 && event.charCode <= 57' MaxLength="3" TabIndex="8"></asp:TextBox>
                </li>
                <li id="liDescriptionOfOperations" runat="server">
                    <label for="<%=Me.txtDescriptionOfOperations.ClientID%>">Description of Operations:</label><br />
                    <asp:TextBox ID="txtDescriptionOfOperations" runat="server" TabIndex="9"></asp:TextBox>
                </li>
            </ul>
        </div>

        <ul>
            <li>
                <asp:Label ID="lblEmail" runat="server" AssociatedControlID="txtEmail" Text="Email:"></asp:Label><br />
                <asp:TextBox ID="txtEmail" runat="server" TabIndex="9" MaxLength="100"></asp:TextBox>
                <input type="hidden" runat="server" id="hdnEmailTypeId" /><%--added 10/7/2017--%>
            </li>
            <li>
                <asp:Label ID="lblPhone" AssociatedControlID="txtPhone" runat="server" Text="Phone:"></asp:Label>
                <br />
                <asp:TextBox ID="txtPhone" runat="server" TabIndex="10" MaxLength="13"></asp:TextBox>
                <label for="<%=Me.txtPhoneExt.ClientID%>">Ext:</label><asp:TextBox ID="txtPhoneExt" runat="server" TabIndex="11" MaxLength="5" Width="50px"></asp:TextBox>
            </li>
            <li>
                <label for="<%=Me.ddPhoneType.ClientID%>">Phone Type:</label>
                <br />
                <asp:DropDownList ID="ddPhoneType" TabIndex="12" runat="server"></asp:DropDownList>
            </li>
        </ul>
    </div>

    <div runat="server" id="tblAddressInfo" class="insured" style="vertical-align: top; text-align: left; width: 235px; float: right">
        <ul>
            <li>
                <label for="<%=Me.txtStreetNum.ClientID%>">Street #:</label><br />
                <asp:TextBox ID="txtStreetNum" runat="server" TabIndex="13" MaxLength="35"></asp:TextBox></li>

            <li>
                <label for="<%=Me.txtStreetName.ClientID%>">Street Name:</label><br />
                <asp:TextBox ID="txtStreetName" runat="server" TabIndex="14" MaxLength="65"></asp:TextBox></li>

            <li>
                <label for="<%=Me.txtAptNum.ClientID%>">Apt./Suite #:</label><br />
                <asp:TextBox ID="txtAptNum" runat="server" TabIndex="15" MaxLength="30"></asp:TextBox></li>
            <li>
                <label for="<%=Me.txtCO.ClientID%>">Other:</label><br />
                <div id="OtherSection" runat="server">
                    <asp:TextBox ID="txtCO" runat="server" TabIndex="15" MaxLength="30"></asp:TextBox><br />
                </div>
                <div id="OtherPrefixSection" runat="server">
                    <asp:DropDownList ID="OtherPrefix" runat="server" TabIndex="4">
                            <asp:ListItem Text="C/O" Value="1"></asp:ListItem>
                            <asp:ListItem Text="ATTN" Value="2"></asp:ListItem>
                    </asp:DropDownList>
                    <asp:TextBox ID="txtPrefixCO" runat="server" TabIndex="15" MaxLength="30" style="width: 76px;"></asp:TextBox>
                </div>
            </li>

            <li>
                <label for="<%=Me.txtPOBox.ClientID%>">P.O. Box:</label><br />
                <asp:TextBox ID="txtPOBox" runat="server" TabIndex="16" MaxLength="10"></asp:TextBox></li>

            <li>
                <label for="<%=Me.txtZipCode.ClientID%>">*ZIP:</label><br />
                <asp:TextBox ID="txtZipCode" runat="server" TabIndex="17" MaxLength="10"></asp:TextBox></li>

            <li>
                <label for="<%=Me.ddCityName.ClientID%>">*City:</label><br />
                <asp:DropDownList ID="ddCityName" CausesValidation="false" runat="server"></asp:DropDownList>
                <asp:TextBox ID="txtCityName" Width="90px" runat="server" TabIndex="18" MaxLength="25"></asp:TextBox>
            </li>

            <li>
                <label for="<%=Me.ddStateAbbrev.ClientID%>">*State:</label><br />
                <asp:DropDownList ID="ddStateAbbrev" runat="server" TabIndex="19"></asp:DropDownList></li>

            <li>
                <label for="<%=Me.txtGaragedCounty.ClientID%>">*County:</label><br />
                <asp:TextBox ID="txtGaragedCounty" runat="server" TabIndex="20" MaxLength="11"></asp:TextBox></li>
        </ul>

        <div style="display: none; margin-left: 10px; width: 190px; border: 1.5px solid lightblue; padding: 10px;" class="roundedContainer" runat="server" id="divAddressMessage">
            You must enter the Street # and Street Name or the P.O. Box field.
        </div>
    </div>
</div>
