<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctlFARPersonalLiabilityGL9.ascx.vb" Inherits="IFM.VR.Web.ctlFARPersonalLiabilityGL9" %>

<style type="text/css">
    .AddressTextbox {
        width:175px;
    }
</style>

<div id="PersonalLiabilitySection" runat="server" class="standardSubSection">
    <h3>Personal Liability (GL-9)
        <span style="float: right;">
            <asp:LinkButton ID="lnkClearPersonalLiability" ToolTip="Clear Personal Liability" runat="server" CssClass="RemovePanelLink">Clear</asp:LinkButton>
            <asp:LinkButton ID="lnkSavePersonalLiability" ToolTip="Save Page" runat="server" CssClass="RemovePanelLink">Save</asp:LinkButton>
        </span>
    </h3>

    <div>
        <asp:CheckBox ID="chkPersonalLiabilityCoverage" runat="server" Text="" /> 
        <asp:LinkButton id="lbPersonalLiabilityCoverage" runat="server" Text="Personal Liability Coverage (GL-9)" />

        <div id="divPersonalLiabilityData" runat="server">
            <asp:Repeater ID="rptPersonalLiabilityItems" runat="server">
                <ItemTemplate>
                    <div style="background-color:lightgray"> 
                        <table id="tblPersonalLiabilityInfo" runat="server" style="width:100%;">
                            <tr style="vertical-align:top;">
                                <td style="line-height:1;">First Name</td>
                                <td colspan="2" style="line-height:1;">Last Name</td>
                            </tr>
                            <tr>
                                <td style="width:40%;vertical-align:top;">
                                    <asp:TextBox ID="txtFirstName" Width="90%" MaxLength="100" runat="server"></asp:TextBox>
                                    <asp:TextBox ID="txtSectionIIIndex" runat="server" Width="0px" style="display:none;" ></asp:TextBox>
                                </td>
                                <td style="width:40%;vertical-align:top;">
                                    <asp:TextBox ID="txtLastName" Width="90%" MaxLength="100" runat="server"></asp:TextBox>
                                </td>
                                <td style="width:20%;">
                                    <asp:LinkButton ID="lbEditAddress" runat="server" CommandName="EDIT" OnClientClick="return false;">Edit Address</asp:LinkButton>
                                    &nbsp;
                                    <asp:LinkButton ID="lbDelete" runat="server" CommandName="DELETE">Delete</asp:LinkButton>
                                </td>
                            </tr>
                        </table>
                        <div id="divAddressInfo" runat="server">
                            <table id="tblAddress" runat="server" style="width:100%;">
                                <tr>
                                    <td style="width:40%;">
                                        Street #
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtStreetNumber" runat="server" CssClass="AddressTextbox"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        Street Name
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtStreetName" runat="server" CssClass="AddressTextbox"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        Apt/Suite Number
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtAptSuiteNumber" runat="server" width="33%"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        Zip
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtZip" runat="server" width="33%"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        City
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtCity" runat="server" CssClass="AddressTextbox"></asp:TextBox>
                                        <asp:DropDownList ID="ddCity" CausesValidation="false" Style="display: none" runat="server"></asp:DropDownList>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        State
                                    </td>
                                    <td>
                                        <asp:DropDownList ID="ddState" runat="server" width="20%" ></asp:DropDownList>
                                    </td>
                                </tr>
                            </table>            
                            <br />
                            <div align="center" >
                                <asp:Button ID="btnOK" runat="server" Text="OK" CssClass="StandardSaveButton" CommandName="OK" />
                                &nbsp;
                                &nbsp;
                                <asp:Button ID="btnCancel" runat="server" Text="Cancel" CssClass="StandardSaveButton" CommandName="CANCEL" />
                                <asp:TextBox ID="txtCounty" runat="server" MaxLength="11" style="display:none;" Width="2px"></asp:TextBox>
                            </div>
                        </div>
                    </div>   
                </ItemTemplate>
                <AlternatingItemTemplate>
                    <div>
                        <table id="tblPersonalLiabilityInfo" runat="server" style="width:100%;">
                            <tr style="vertical-align:top;">
                                <td style="line-height:1;">First Name</td>
                                <td colspan="2" style="line-height:1;">Last Name</td>
                            </tr>
                            <tr>
                                <td style="width:40%;vertical-align:top;">
                                    <asp:TextBox ID="txtFirstName" Width="90%" MaxLength="100" runat="server"></asp:TextBox>
                                    <asp:TextBox ID="txtSectionIIIndex" runat="server" Width="0px" style="display:none;" ></asp:TextBox>
                                </td>
                                <td style="width:40%;vertical-align:top;">
                                    <asp:TextBox ID="txtLastName" Width="90%" MaxLength="100" runat="server"></asp:TextBox>
                                </td>
                                <td style="width:20%;">
                                    <asp:LinkButton ID="lbEditAddress" runat="server" CommandName="EDIT" OnClientClick="return false;">Edit Address</asp:LinkButton>
                                    &nbsp;
                                    <asp:LinkButton ID="lbDelete" runat="server" CommandName="DELETE">Delete</asp:LinkButton>
                                </td>
                            </tr>
                        </table>
                        <div id="divAddressInfo" runat="server">
                            <table id="tblAddress" runat="server" style="width:100%;">
                                <tr>
                                    <td style="width:40%;">
                                        Street #
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtStreetNumber" runat="server" CssClass="AddressTextbox"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        Street Name
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtStreetName" runat="server" CssClass="AddressTextbox"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        Apt/Suite Number
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtAptSuiteNumber" runat="server" width="33%"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        Zip
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtZip" runat="server" width="33%"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        City
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtCity" runat="server" CssClass="AddressTextbox"></asp:TextBox>
                                        <asp:DropDownList ID="ddCity" CausesValidation="false" Style="display: none" runat="server"></asp:DropDownList>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        State
                                    </td>
                                    <td>
                                        <asp:DropDownList ID="ddState" runat="server" width="20%" ></asp:DropDownList>
                                    </td>
                                </tr>
                            </table>            
                            <br />
                            <div align="center" >
                                <asp:Button ID="btnOK" runat="server" Text="OK" CssClass="StandardSaveButton" CommandName="OK" />
                                &nbsp;
                                &nbsp;
                                <asp:Button ID="btnCancel" runat="server" Text="Cancel" CssClass="StandardSaveButton" CommandName="CANCEL" />
                                <asp:TextBox ID="txtCounty" runat="server" MaxLength="11" style="display:none;" Width="2px"></asp:TextBox>
                            </div>
                        </div>
                    </div>  
                </AlternatingItemTemplate>
            </asp:Repeater>
            <br />
            <asp:LinkButton ID="lbAddNewPersonalLiability" runat="server" >Add New Personal Liability Coverage (GL-9)</asp:LinkButton>
            <asp:HiddenField ID="hdnAccord" runat="server" />
        </div>
    </div>
</div>

<%--Personal Liability Popup--%>
<div id="dvPersLiabPopup" runat="server" style="display: none">
    <div>
        If liability coverage is written on the GL-610 (Commercial Liability Coverage Form), endorsement GL-9 may be used to provide personal liability coverage for corporate officers and stockholders who do not have personal liability coverage elsewhere.
    </div>

    <div style="text-align: center; margin-left: auto; margin-right: auto; margin-top: 20px; margin-bottom: 20px;">
        <asp:Button ID="btnPLOK" CssClass="StandardButton" runat="server" Text="OK" />
    </div>
</div>

<asp:HiddenField ID="hdnCheckboxValue" runat="server" />


