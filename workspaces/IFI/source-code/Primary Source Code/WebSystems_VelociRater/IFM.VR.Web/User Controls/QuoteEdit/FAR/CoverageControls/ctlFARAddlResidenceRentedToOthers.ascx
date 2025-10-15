<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctlFARAddlResidenceRentedToOthers.ascx.vb" Inherits="IFM.VR.Web.ctlFARAddlResidenceRentedToOthers" %>

<style type="text/css">
    .AddressTextbox {
        width:150px;
    }
</style>

<asp:CheckBox ID="chkAddlResidenceRentedToOthers" runat="server" Text="&nbsp;&nbsp;Additional Residence Rented to Others (GL-73)" />

<div id="divAddlResidenceData" runat="server">
    <asp:Repeater ID="rptAddlResidenceItems" runat="server">
        <ItemTemplate>
            <div style="background-color:lightgray">
                <table id="tblAddlResidenceInfo" runat="server" style="width:100%;">
                    <tr>
                        <td runat="server" id="tdDescription" style="width:50%;">Description</td>
                        <td colspan="2">Number of Families</td>
                    </tr>
                    <tr>
                        <td runat="server" id="tdDescription1">
                            <asp:TextBox ID="txtDescription" Width="200px" MaxLength="250" runat="server"></asp:TextBox>
                            <asp:TextBox ID="txtSectionIIIndex" runat="server" Width="1px" style="display:none;" ></asp:TextBox>
                        </td>
                        <td>
                            <asp:DropDownList ID="ddNumOfFamilies" Width="100px" runat="server">
                                <asp:ListItem Text="1" Value="1"></asp:ListItem>
                                <asp:ListItem Text="2" Value="2"></asp:ListItem>
                                <asp:ListItem Text="3" Value="3"></asp:ListItem>
                                <asp:ListItem Text="4" Value="4"></asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:LinkButton ID="lbEditAddress" runat="server" CommandName="EDIT" OnClientClick="return false;">Edit Address</asp:LinkButton>
                            &nbsp;
                            <asp:LinkButton ID="lbDelete" runat="server" CommandName="DELETE">Delete</asp:LinkButton>
                        </td>
                    </tr>
                </table>
                <div id="divAddressInfo" runat="server">
                    <table id="tblAddress" runat="server" style="width:100%;">
                        <tr>
                            <td style="width:50%;"></td>
                            <td>
                                <asp:Button ID="btnCopyAddress" CssClass="StandardButton" TabIndex="-1" Style="margin-top: -2px;" runat="server" Text="Copy Address from Location" CommandName="CopyAddress" />                
                            </td>
                        </tr>
                        <tr>
                            <td style="width:50%;">
                                Street #
                            </td>
                            <td>
                                <asp:TextBox ID="txtStreetNumber" runat="server" CssClass="AddressTextbox"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td style="width:50%;">
                                Street Name
                            </td>
                            <td>
                                <asp:TextBox ID="txtStreetName" runat="server" CssClass="AddressTextbox"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td style="width:50%;">
                                Apt/Suite Number
                            </td>
                            <td>
                                <asp:TextBox ID="txtAptSuiteNumber" runat="server" CssClass="AddressTextbox"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td style="width:50%;">
                                Zip
                            </td>
                            <td>
                                <asp:TextBox ID="txtZip" runat="server" CssClass="AddressTextbox"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td style="width:50%;">
                                City
                            </td>
                            <td>
                                <asp:TextBox ID="txtCity" runat="server" CssClass="AddressTextbox"></asp:TextBox>
                                <asp:DropDownList ID="ddCity" CausesValidation="false" Style="display: none" runat="server"></asp:DropDownList>
                            </td>
                        </tr>
                        <tr>
                            <td style="width:50%;">
                                State
                            </td>
                            <td>
                                <asp:DropDownList ID="ddState" runat="server" CssClass="AddressTextbox" ></asp:DropDownList>
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
                <table id="tblAddlResidenceInfo" runat="server" style="width:100%;">
                    <tr>
                        <td runat="server" id="tdDescription" style="width:50%;">Description</td>
                        <td colspan="2">Number of Families</td>
                    </tr>
                    <tr>
                        <td runat="server" id="tdDescription1">
                            <asp:TextBox ID="txtDescription" Width="200px" MaxLength="250" runat="server"></asp:TextBox>
                            <asp:TextBox ID="txtSectionIIIndex" runat="server" Width="1px" style="display:none;" ></asp:TextBox>
                        </td>
                        <td>
                            <asp:DropDownList ID="ddNumOfFamilies" Width="100px" runat="server">
                                <asp:ListItem Text="1" Value="1"></asp:ListItem>
                                <asp:ListItem Text="2" Value="2"></asp:ListItem>
                                <asp:ListItem Text="3" Value="3"></asp:ListItem>
                                <asp:ListItem Text="4" Value="4"></asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:LinkButton ID="lbEditAddress" runat="server" CommandName="EDIT" OnClientClick="return false;">Edit Address</asp:LinkButton>
                            &nbsp;
                            <asp:LinkButton ID="lbDelete" runat="server" CommandName="DELETE">Delete</asp:LinkButton>
                        </td>
                    </tr>
                </table>
                <div id="divAddressInfo" runat="server">
                    <table id="tblAddress" runat="server" style="width:100%;">
                        <tr>
                            <td style="width:50%;"></td>
                            <td>
                                <asp:Button ID="btnCopyAddress" CssClass="StandardButton" TabIndex="-1" Style="margin-top: -2px;" runat="server" Text="Copy Address from Location" CommandName="CopyAddress" />                
                            </td>
                        </tr>
                        <tr>
                            <td style="width:50%;">
                                Street #
                            </td>
                            <td>
                                <asp:TextBox ID="txtStreetNumber" runat="server" CssClass="AddressTextbox"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td style="width:50%;">
                                Street Name
                            </td>
                            <td>
                                <asp:TextBox ID="txtStreetName" runat="server" CssClass="AddressTextbox"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td style="width:50%;">
                                Apt/Suite Number
                            </td>
                            <td>
                                <asp:TextBox ID="txtAptSuiteNumber" runat="server" CssClass="AddressTextbox"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td style="width:50%;">
                                Zip
                            </td>
                            <td>
                                <asp:TextBox ID="txtZip" runat="server" CssClass="AddressTextbox"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td style="width:50%;">
                                City
                            </td>
                            <td>
                                <asp:TextBox ID="txtCity" runat="server" CssClass="AddressTextbox"></asp:TextBox>
                                <asp:DropDownList ID="ddCity" CausesValidation="false" Style="display: none" runat="server"></asp:DropDownList>
                            </td>
                        </tr>
                        <tr>
                            <td style="width:50%;">
                                State
                            </td>
                            <td>
                                <asp:DropDownList ID="ddState" runat="server" CssClass="AddressTextbox" ></asp:DropDownList>
                            </td>
                        </tr>
                    </table>            
                    <br />
                    <div align="center">
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
    <asp:LinkButton ID="lbAddNewResidence" runat="server" >Add New Additional Residence Rented to Others</asp:LinkButton>
</div>

