<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctlApplicant.ascx.vb" Inherits="IFM.VR.Web.ctlApplicant" %>

<h3>
    <asp:Label ID="lblInsuredTitle" runat="server" Text="Label"></asp:Label>
    <span style="float: right;">
        <asp:LinkButton ID="lnkAdd" CssClass="RemovePanelLink" runat="server">Add New Applicant</asp:LinkButton>
        <asp:LinkButton ID="lnkRemove" ToolTip="Clear this applicant" CssClass="RemovePanelLink" runat="server">Remove</asp:LinkButton>
        <asp:LinkButton ID="lnkSave" CssClass="RemovePanelLink" runat="server">Save</asp:LinkButton>
    </span>
</h3>
<div runat="server" id="divInsuredInfo">
    <div class="insured" style="float: left; vertical-align: top; text-align: left;">
        <ul>
            <li>
                <label for="<%=txtFirstName.ClientID %>">*First Name:</label><br />
                <asp:TextBox ID="txtFirstName" runat="server" TabIndex="101" MaxLength="50" autofocus></asp:TextBox>
            </li>
            <li>
                <label for="<%=Me.txtMiddleName.ClientID%>">Middle Name:</label><br />
                <asp:TextBox ID="txtMiddleName" runat="server" TabIndex="102" MaxLength="50"></asp:TextBox>
            </li>
            <li>
                <label for="<%=Me.txtLastName.ClientID%>">*Last Name:</label><br />
                <asp:TextBox ID="txtLastName" runat="server" TabIndex="103" MaxLength="50"></asp:TextBox>
            </li>

            <li>
                <label for="<%=Me.txtSSN.ClientID%>">SSN:</label><br />
                <asp:TextBox ID="txtSSN" runat="server" TabIndex="106" MaxLength="11"></asp:TextBox>
            </li>
            <li>
                <label for="<%=Me.txtBirthDate.ClientID%>">*Birth Date:</label><br />
                <asp:TextBox ID="txtBirthDate" runat="server" MaxLength="10" TabIndex="107"></asp:TextBox>
            </li>
        </ul>
        <ul style="display: none">
            <li>
                <label for="<%=Me.txtEmail.ClientID%>">Email:</label><br />
                <asp:TextBox ID="txtEmail" runat="server" TabIndex="108" MaxLength="100"></asp:TextBox>
            </li>
            <li>
                <label for="<%=Me.txtPhone.ClientID%>">Phone:</label><br />
                <asp:TextBox ID="txtPhone" runat="server" TabIndex="109" MaxLength="13"></asp:TextBox>
                <label for="<%=Me.txtPhoneExt.ClientID%>">Ext:</label><asp:TextBox ID="txtPhoneExt" runat="server" TabIndex="1010" onkeyup="$(this).val(FormatAsPositiveNumberNoCommaFormatting($(this).val()));" MaxLength="5" Width="50px"></asp:TextBox>
            </li>
            <li>
                <label for="<%=Me.ddPhoneType.ClientID%>">Phone Type:</label>
                <br />
                <asp:DropDownList ID="ddPhoneType" TabIndex="1011" runat="server"></asp:DropDownList>
            </li>
        </ul>
    </div>

    <div runat="server" id="tblAddressInfo" class="insured" style="vertical-align: top; text-align: left; width: 275px; float: right">
        <ul>
            <li>
                <input id="Button1" type="button" onclick="ApplicantSearch.CopyPh1AddressToApplicant(<%=Me.ApplicantIndex%>);" value="Copy from Policyholder #1" class="StandardSaveButton" />
            </li>
            <li>
                <label for="<%=Me.txtStreetNum.ClientID%>">Street #:</label><br />
                <asp:TextBox ID="txtStreetNum" runat="server" TabIndex="1012" MaxLength="35"></asp:TextBox></li>

            <li>
                <label for="<%=Me.txtStreetName.ClientID%>">Street Name:</label><br />
                <asp:TextBox ID="txtStreetName" runat="server" TabIndex="1013" MaxLength="65"></asp:TextBox></li>

            <li>
                <label for="<%=Me.txtAptNum.ClientID%>">Apt./Suite #:</label><br />
                <asp:TextBox ID="txtAptNum" runat="server" TabIndex="1014" MaxLength="30"></asp:TextBox></li>

            <li>
                <label for="<%=Me.txtPOBox.ClientID%>">P.O. Box:</label><br />
                <asp:TextBox ID="txtPOBox" runat="server" TabIndex="1015" MaxLength="10"></asp:TextBox></li>

            <li>
                <label for="<%=Me.txtZipCode.ClientID%>">*ZIP:</label><br />
                <asp:TextBox ID="txtZipCode" runat="server" TabIndex="1016" MaxLength="10"></asp:TextBox></li>

            <li>
                <label for="<%=Me.ddCityName.ClientID%>">*City:</label><br />
                <asp:DropDownList ID="ddCityName" CausesValidation="false" runat="server"></asp:DropDownList>
                <asp:TextBox ID="txtCityName" runat="server" TabIndex="1017" MaxLength="25"></asp:TextBox>
            </li>

            <li>
                <label for="<%=Me.ddStateAbbrev.ClientID%>">*State:</label><br />
                <asp:DropDownList ID="ddStateAbbrev" runat="server" TabIndex="1018"></asp:DropDownList></li>

            <li style="display: none;">
                <label for="<%=Me.txtGaragedCounty.ClientID%>">*County:</label><br />
                <asp:TextBox ID="txtGaragedCounty" runat="server" TabIndex="1019" MaxLength="11"></asp:TextBox></li>
        </ul>

        <div style="display: none; margin-left: 10px; width: 190px; border: 1.5px solid lightblue; padding: 10px;" class="roundedContainer" runat="server" id="divAddressMessage">
            You must enter the Street # and Street Name or the P.O. Box field.
        </div>
    </div>
</div>