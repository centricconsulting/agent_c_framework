<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctlCommercialDataPrefillEntry.ascx.vb" Inherits="IFM.VR.Web.ctlCommercialDataPrefillEntry" %>

<%--<asp:UpdatePanel ID="upCommDataPrefill" runat="server">
    <ContentTemplate>--%>
        <div runat="server" id="divCommDataPrefillPopup">
            <div style="padding:20px; text-align:center;" class="informationalText">
                Please enter all the property location addresses below so that we can retrieve detailed information regarding the risk.
            </div>
            <div runat="server" id="PolicyholderSection">
                <h3>Policyholder Information</h3>
                <div>
                    <div class="insured" style="float: left; vertical-align: top; text-align: left;">
                        <ul style="width:190px;">
                            <li>
                                <label for="<%=Me.txtBusinessName.ClientID%>">*Name:</label><br />
                                <asp:TextBox ID="txtBusinessName" runat="server" Width="180px" MaxLength="50"></asp:TextBox>
                            </li>
                            <li>
                                <label for="<%=Me.txtStreetNum.ClientID%>">Street #:</label><br />
                                <asp:TextBox ID="txtStreetNum" runat="server" MaxLength="35"></asp:TextBox>
                            </li>
                            <li>
                                <label for="<%=Me.txtStreetName.ClientID%>">Street Name:</label><br />
                                <asp:TextBox ID="txtStreetName" runat="server" MaxLength="65"></asp:TextBox>
                            </li>
                            <li>
                                <label for="<%=Me.txtAptNum.ClientID%>">Apt./Suite #:</label><br />
                                <asp:TextBox ID="txtAptNum" runat="server" MaxLength="30"></asp:TextBox>
                            </li>
                        </ul>
                        <asp:TextBox ID="txtDBA" runat="server" MaxLength="50" Width="200px" CssClass="hidden"></asp:TextBox>
                        <asp:DropDownList ID="ddBusinessType" runat="server" CssClass="hidden"></asp:DropDownList>
                        <asp:DropDownList ID="ddTaxIDType" runat="server" CssClass="hidden">
                            <asp:ListItem Text="" Value=""></asp:ListItem>
                            <asp:ListItem Text="FEIN" Value="2"></asp:ListItem>
                            <asp:ListItem Text="SSN" Value="1"></asp:ListItem>
                        </asp:DropDownList>
                        <asp:TextBox ID="txtFEIN" runat="server" MaxLength="11" CssClass="hidden"></asp:TextBox>
                        <asp:TextBox ID="txtEmail" runat="server" MaxLength="100" CssClass="hidden"></asp:TextBox>
                        <input type="hidden" runat="server" id="hdnEmailTypeId" />
                        <asp:TextBox ID="txtPhone" runat="server" MaxLength="13" CssClass="hidden"></asp:TextBox>
                        <asp:TextBox ID="txtPhoneExt" runat="server" MaxLength="5" Width="50px" CssClass="hidden"></asp:TextBox>
                        <asp:DropDownList ID="ddPhoneType" runat="server" CssClass="hidden"></asp:DropDownList>
                        <asp:TextBox ID="txtOtherEntity" runat="server" MaxLength="250" Width="200px" CssClass="hidden"></asp:TextBox>
                        <asp:TextBox ID="txtBusinessStarted" runat="server" CssClass="hidden"></asp:TextBox>
                        <asp:TextBox ID="txtYearsOfExperience" runat="server" onkeypress='return event.charCode >= 48 && event.charCode <= 57' MaxLength="3" CssClass="hidden"></asp:TextBox>
                        <asp:TextBox ID="txtDescriptionOfOperations" runat="server" CssClass="hidden"></asp:TextBox>
                        <asp:TextBox ID="txtOther" runat="server" CssClass="hidden"></asp:TextBox>
                        <asp:TextBox ID="txtClientId" runat="server" CssClass="hidden"></asp:TextBox>
                        <asp:HiddenField runat="server" ID="hdnHasPrefill" />
                        <asp:HiddenField runat="server" ID="hdnPrefillOrderInfo" />
                        <asp:HiddenField runat="server" ID="hdnHasChangedSinceLoad" />
                        <asp:HiddenField runat="server" ID="hdnHasChangedSinceLastCheck" />
                    </div>
                    <div class="insured" style="vertical-align: top; text-align: left; width: 200px; float: right">
                        <ul style="width:150px;">
                            <li>
                                <label for="<%=Me.txtPOBox.ClientID%>">P.O. Box:</label><br />
                                <asp:TextBox ID="txtPOBox" runat="server" MaxLength="10"></asp:TextBox>
                            </li>
                            <li>
                                <label for="<%=Me.txtZipCode.ClientID%>">*ZIP:</label><br />
                                <asp:TextBox ID="txtZipCode" runat="server" MaxLength="10"></asp:TextBox>
                            </li>
                            <li>
                                <label for="<%=Me.ddCityName.ClientID%>">*City:</label><br />
                                <asp:DropDownList ID="ddCityName" CausesValidation="false" runat="server"></asp:DropDownList>
                                <asp:TextBox ID="txtCityName" Width="90px" runat="server" MaxLength="25"></asp:TextBox>
                            </li>
                            <li>
                                <label for="<%=Me.ddStateAbbrev.ClientID%>">*State:</label><br />
                                <asp:DropDownList ID="ddStateAbbrev" runat="server"></asp:DropDownList>
                            </li>
                        </ul>
                        <asp:TextBox ID="txtGaragedCounty" runat="server" MaxLength="11" CssClass="hidden"></asp:TextBox>

                        <div style="display: none; margin-left: 10px; width: 190px; border: 1.5px solid lightblue; padding: 10px;" class="roundedContainer" runat="server" id="divAddressMessage">
                            You must enter the Street # and Street Name or the P.O. Box field.
                        </div>
                    </div>
                </div>
            </div>
            <div runat="server" id="LocationSection">
                <h3>Location Information</h3>
                <div runat="server" id="LocationRepeaterSection">
                    <asp:Repeater ID="rptLocations" runat="server">
                        <ItemTemplate>
                            <div runat="server" id="IndividualLocationRepeaterItemSection">
                            <h3>
                                Location # <asp:Label runat="server" ID="lblLocNum" Text='<%# DataBinder.Eval(Container.DataItem, "locNum") %>'></asp:Label>
                                <span style="float: right;">
                                    <asp:LinkButton ID="lbCopyPolicyholder" CssClass="RemovePanelLink" ToolTip="Copy Policyholder Address to Location" runat="server" OnClick="lbCopyPolicyholder_Click">Copy Policyholder</asp:LinkButton>
                                    <asp:LinkButton ID="lbDeleteLocation" CssClass="RemovePanelLink" ToolTip="Delete Location" runat="server" OnClick="lbDeleteLocation_Click">Delete</asp:LinkButton>
                                </span>
                                <asp:Label runat="server" ID="lblIsPreExistingLoc" Visible="false" Text='<%# DataBinder.Eval(Container.DataItem, "isPreExisting") %>'></asp:Label>
                            </h3>
                            <div>
                                <div class="insured" style="float: left; vertical-align: top; text-align: left;">
                                    <ul style="width:150px;">
                                        <li>
                                            <label runat="server" id="lblLocStreetNum">Street #:</label><br />
                                            <asp:TextBox ID="txtLocStreetNum" runat="server" MaxLength="35" Text='<%# DataBinder.Eval(Container.DataItem, "streetNum") %>'></asp:TextBox>
                                        </li>
                                        <li>
                                            <label runat="server" id="lblLocStreetName">Street Name:</label><br />
                                            <asp:TextBox ID="txtLocStreetName" runat="server" MaxLength="65" Text='<%# DataBinder.Eval(Container.DataItem, "streetName") %>'></asp:TextBox>
                                        </li>
                                        <li>
                                            <label runat="server" id="lblLocAptNum">Apt./Suite #:</label><br />
                                            <asp:TextBox ID="txtLocAptNum" runat="server" MaxLength="30" Text='<%# DataBinder.Eval(Container.DataItem, "aptNum") %>'></asp:TextBox>
                                        </li>
                                    </ul>
                                </div>
                                <div class="insured" style="vertical-align: top; text-align: left; width: 200px; float: right">
                                    <ul style="width:150px;">
                                        <li>
                                            <label runat="server" id="lblLocZipCode">*ZIP:</label><br />
                                            <asp:TextBox ID="txtLocZipCode" runat="server" MaxLength="10" Text='<%# DataBinder.Eval(Container.DataItem, "zipCode") %>'></asp:TextBox>
                                        </li>
                                        <li>
                                            <label runat="server" id="lblLocCityName">*City:</label><br />
                                            <asp:DropDownList ID="ddLocCityName" CausesValidation="false" runat="server"></asp:DropDownList>
                                            <asp:TextBox ID="txtLocCityName" Width="90px" runat="server" MaxLength="25" Text='<%# DataBinder.Eval(Container.DataItem, "city") %>'></asp:TextBox>
                                        </li>
                                        <li>
                                            <label runat="server" id="lblLocStateAbbrev">*State:</label><br />
                                            <asp:Label runat="server" ID="lblLocState" Visible="false" Text='<%# DataBinder.Eval(Container.DataItem, "state") %>'></asp:Label>
                                            <asp:DropDownList ID="ddLocStateAbbrev" runat="server"></asp:DropDownList>
                                        </li>
                                    </ul>
                                    <asp:TextBox ID="txtLocGaragedCounty" runat="server" MaxLength="11" CssClass="hidden" Text='<%# DataBinder.Eval(Container.DataItem, "county") %>'></asp:TextBox>
                                </div>
                            </div>
                            <asp:HiddenField runat="server" ID="hdnIndividualLocationRepeaterItemSection" Value='<%# DataBinder.Eval(Container.DataItem, "accordionFlag") %>' />
                            <asp:HiddenField runat="server" ID="hdnLocHasPrefill" Value='<%# DataBinder.Eval(Container.DataItem, "hasPrefill") %>' />
                            <asp:HiddenField runat="server" ID="hdnLocPrefillOrderInfo" Value='<%# DataBinder.Eval(Container.DataItem, "prefillOrderInfo") %>' />
                            <asp:HiddenField runat="server" ID="hdnLocHasChangedSinceLoad" Value='<%# DataBinder.Eval(Container.DataItem, "hasChangedSinceLoad") %>' />
                            <asp:HiddenField runat="server" ID="hdnLocHasChangedSinceLastCheck" Value='<%# DataBinder.Eval(Container.DataItem, "hasChangedSinceLastCheck") %>' />
                            </div>
                        </ItemTemplate>
                    </asp:Repeater>
                </div>
            </div>
            <div class="standardSubSection center" style="width: 100%; text-align: center;">
                <div style="float:left; text-align:left;">
                    <asp:Button runat="server" ID="btnCancelPrefill" CssClass="StandardButton" Text="Cancel" Visible="false" />
                    <asp:Button runat="server" ID="btnAddLoc" CssClass="StandardSaveButton" Text="Add Location" />
                </div>
                <div style="float:right; text-align:left;">
                    <asp:Button runat="server" ID="btnSavePrefillEntry" CssClass="StandardSaveButton" Text="Continue" />
                </div>
            </div>
            <asp:Label runat="server" ID="lblFocusClientId" Visible="false"></asp:Label>
            <asp:Label runat="server" ID="lblGovStateId" Visible="false"></asp:Label>
            <asp:Label runat="server" ID="lblLocNumForLastAction" Visible="false"></asp:Label>
            <asp:Label runat="server" ID="lblLocNumWithFocus" Visible="false"></asp:Label>

            <asp:HiddenField runat="server" ID="hdnPolicyholderSection" />
            <asp:HiddenField runat="server" ID="hdnLocationSection" />
        </div>
   <%-- </ContentTemplate>
</asp:UpdatePanel>--%>
