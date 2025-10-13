<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctlRVWItem.ascx.vb" Inherits="IFM.VR.Web.ctlRVWItem" %>
<%@ Register Src="~/User Controls/QuoteEdit/HOM/ctlRVWatercraftOperatorItem.ascx" TagPrefix="uc1" TagName="ctlRVWOperator" %>

<div id="RVWInputDiv">
    <asp:Panel ID="pnlRVWInput" runat="server">
        <div>
            <table>
                <tr>
                    <td class="LabelColumn">
                        <asp:Label ID="Label1" runat="server" Text="Type"></asp:Label>
                    </td>
                    <td class="DataColumn">
                        <asp:DropDownList ID="ddlVehType" runat="server" Width="150px" TabIndex="1"></asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td class="LabelColumn">
                        <asp:Label ID="Label2" runat="server" Text="Year"></asp:Label>
                    </td>
                    <td class="DataColumn">
                        <asp:TextBox ID="txtVehYear" runat="server" Width="150px" TabIndex="2"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="LabelColumn">
                        <asp:Label ID="Label3" runat="server" Text="Manufacturer"></asp:Label>
                    </td>
                    <td class="DataColumn">
                        <asp:TextBox ID="txtVehManufacturer" runat="server" Width="150px" TabIndex="3"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="LabelColumn">
                        <asp:Label ID="Label4" runat="server" Text="Model"></asp:Label>
                    </td>
                    <td class="DataColumn">
                        <asp:TextBox ID="txtVehModel" runat="server" Width="150px" TabIndex="4"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="LabelColumn">
                        <asp:Label ID="Label5" runat="server" Text="Serial Number"></asp:Label>
                    </td>
                    <td class="DataColumn">
                        <asp:TextBox ID="txtVehSerialNumber" runat="server" Width="150px" TabIndex="5"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="LabelColumn">
                        <asp:Label ID="Label6" runat="server" Text="Horsepower/CCs"></asp:Label>
                    </td>
                    <td class="DataColumn">
                        <asp:TextBox ID="txtVehHorsepowerCCs" runat="server" Width="150px" TabIndex="6"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="LabelColumn">
                        <asp:Label ID="Label7" runat="server" Text="Length in Feet"></asp:Label>
                    </td>
                    <td class="DataColumn">
                        <asp:TextBox ID="txtVehLength" runat="server" Width="150px" TabIndex="7"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="LabelColumn">
                        <asp:Label ID="Label8" runat="server" Text="Rated Speed"></asp:Label>
                    </td>
                    <td class="DataColumn">
                        <asp:TextBox ID="txtVehRatedSpeed" runat="server" Width="150px" TabIndex="8"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="LabelColumn">
                        <asp:Label ID="Label9" runat="server" Text="Cost New"></asp:Label>
                    </td>
                    <td class="DataColumn">
                        <asp:TextBox ID="txtVehCostNew" runat="server" Width="150px" TabIndex="9"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="LabelColumn">
                        <asp:Label ID="Label10" runat="server" Text="Description"></asp:Label>
                    </td>
                    <td class="DataColumn">
                        <asp:TextBox ID="txtVehDescription" runat="server" Width="150px" TextMode="MultiLine" Height="30px" TabIndex="10"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="LabelColumn">
                        <asp:Label ID="Label11" runat="server" Text="Premium"></asp:Label>
                    </td>
                    <td class="DataColumn">
                        <asp:TextBox ID="txtVehPremium" runat="server" Width="150px" TabIndex="11"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="LabelColumn">
                        <asp:Label ID="Label13" runat="server" Text="Owner Other Than Insured"></asp:Label>
                    </td>
                    <td class="DataColumn">
                        <asp:CheckBox ID="chkVehOwnerOtherThanInsured" runat="server" Width="150px" Text="&nbsp" TabIndex="12"></asp:CheckBox>
                    </td>
                </tr>
                <tr>
                    <td class="LabelColumn">
                        <asp:Label ID="Label22" runat="server" Text="Name"></asp:Label>
                    </td>
                    <td class="DataColumn">
                        <asp:TextBox ID="txtVehOwnerOtherThanInsuredName" runat="server" Width="150px" TabIndex="13"></asp:TextBox>
                    </td>
                </tr>
            </table>
        </div>

        <div id="MotorInputDiv" runat="server">
            <h3>Motor</h3>
            <div>
                <table>
                    <tr>
                        <td class="LabelColumn">
                            <asp:Label ID="Label12" runat="server" Text="Type"></asp:Label>
                        </td>
                        <td class="DataColumn">
                            <asp:DropDownList ID="ddlMotorType" runat="server" Width="100%" TabIndex="14" onchange="javascript:ddlMotor_Changed();"></asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td class="LabelColumn">
                            <asp:Label ID="label15" runat="server" Text="Year"></asp:Label>
                        </td>
                        <td class="DataColumn">
                            <asp:TextBox ID="txtMotorYear" runat="server" TabIndex="15"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="LabelColumn">
                            <asp:Label ID="label16" runat="server" Text="Manufacturer"></asp:Label>
                        </td>
                        <td class="DataColumn">
                            <asp:TextBox ID="txtMotorManufacturer" runat="server" TabIndex="16"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="LabelColumn">
                            <asp:Label ID="label17" runat="server" Text="Model"></asp:Label>
                        </td>
                        <td class="DataColumn">
                            <asp:TextBox ID="txtMotorModel" runat="server" TabIndex="17"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="LabelColumn">
                            <asp:Label ID="label18" runat="server" Text="Serial Number"></asp:Label>
                        </td>
                        <td class="DataColumn">
                            <asp:TextBox ID="txtMotorSerialNumber" runat="server" TabIndex="18"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="LabelColumn">
                            <asp:Label ID="label19" runat="server" Text="Cost New"></asp:Label>
                        </td>
                        <td class="DataColumn">
                            <asp:TextBox ID="txtMotorCostNew" runat="server" TabIndex="19"></asp:TextBox>
                        </td>
                    </tr>
                </table>
            </div>
        </div>

        <div id="OperatorInputDiv" runat="server">
            <uc1:ctlRVWOperator ID="ctlRVWOperators" runat="server" TabIndex="20" />
        </div>

        <div id="RVWatercraftCoveragesDiv" runat="server">
            <h3><a href="#">RV/Watercraft Level Coverages</a></h3>
            <div>
                <asp:Panel ID="pnlRVWCoverages" runat="server">
                    <asp:Label ID="label102" runat="server" Text="RV/Watercraft Property Coverage" Font-Bold="true"></asp:Label>
                    <table>
                        <tr>
                            <td class="LabelColumn">
                                <asp:Label ID="label101" runat="server" Text="Limit"></asp:Label>
                            </td>
                            <td class="DataColumn">
                                <asp:TextBox ID="txtPropertyLimit" runat="server" Width="150px" ReadOnly="true" TabIndex="21"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="LabelColumn">
                                <asp:Label ID="label23" runat="server" Text="Deductible"></asp:Label>
                            </td>
                            <td class="DataColumn">
                                <asp:DropDownList ID="ddlPropertyDeductible" runat="server" Width="150px" TabIndex="22"></asp:DropDownList>
                            </td>
                        </tr>
                        <tr>
                            <td class="LabelColumn">
                                <asp:Label ID="label24" runat="server" Text="Premium"></asp:Label>
                            </td>
                            <td class="DataColumn">
                                <asp:TextBox ID="txtPropertyPremium" runat="server" Width="150px" ReadOnly="true" TabIndex="23"></asp:TextBox>
                            </td>
                        </tr>
                    </table>
                    <br />
                    <asp:Label ID="label25" runat="server" Text="RV/Watercraft Uninsured Motorist Bodily Injury" Font-Bold="true"></asp:Label>
                    <table>
                        <tr>
                            <td class="LabelColumn">
                                <asp:Label ID="label27" runat="server" Text="Limit"></asp:Label>
                            </td>
                            <td class="DataColumn">
                                <asp:DropDownList ID="ddlBodilyInjuryLimit" runat="server" Width="100px" TabIndex="24"></asp:DropDownList>
                            </td>
                        </tr>
                        <tr>
                            <td class="LabelColumn">
                                <asp:Label ID="label28" runat="server" Text="Premium"></asp:Label>
                            </td>
                            <td class="DataColumn">
                                <asp:TextBox ID="txtBodilyInjuryPremium" runat="server" Width="75px" ReadOnly="true" TabIndex="25"></asp:TextBox>
                            </td>
                        </tr>
                    </table>
                    <br />
                    <table>
                        <tr>
                            <td>
                                <asp:Label ID="label30" runat="server" Text="RV/Watercraft Liability"></asp:Label>
                            </td>
                            <td>
                                <asp:CheckBox ID="chkRVWLiability" runat="server" Text="&nbsp;" TabIndex="26" />
                            </td>
                            <td>
                                <asp:Label ID="label29" runat="server" Text="Premium"></asp:Label>
                            </td>
                            <td>
                                <asp:TextBox ID="txtRVWLiabilityPremium" runat="server" Width="100px" ReadOnly="true" TabIndex="27"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:Label ID="label26" runat="server" Text="Liability Only"></asp:Label>
                            </td>
                            <td>
                                <asp:CheckBox ID="chkRVWLiabilityOnly" runat="server" Text="&nbsp;" TabIndex="28" />
                            </td>
                            <td>
                                <asp:Label ID="label31" runat="server" Text="Premium"></asp:Label>
                            </td>
                            <td>
                                <asp:TextBox ID="txtRVWLiabilityOnlyPremium" runat="server" Width="100px" ReadOnly="true" TabIndex="29"></asp:TextBox>
                            </td>
                        </tr>
                    </table>
                </asp:Panel>
            </div>
        </div>
        <%-- Watercraft Coverages Div --%>
        <br />
        <div align="center">
            <asp:Label ID="lblMsg" runat="server" ForeColor="Red" Font-Bold="true" Text="&nbsp;"></asp:Label>
        </div>
    </asp:Panel>
</div>