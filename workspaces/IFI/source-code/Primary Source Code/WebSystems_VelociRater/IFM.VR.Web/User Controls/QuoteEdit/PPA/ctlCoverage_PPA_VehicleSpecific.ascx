<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctlCoverage_PPA_VehicleSpecific.ascx.vb" Inherits="IFM.VR.Web.ctlCoverage_PPA_VehicleSpecific" %>
<%@ Register Src="~/User Controls/Application/PPA/ctlCoverage_PPA_ScheduledItemsList.ascx" TagPrefix="uc1" TagName="ctlCoverage_PPA_ScheduledItemsList" %>


<%--<div id="dvVehicleSpecific" runat="server">--%>
    <h3 id="divAccordHeader" runat="server">
        <asp:Panel ID="pnlAccordHeader" runat="server">
        <%--<asp:LinkButton ID="lnkAccordHeader" runat="server"></asp:LinkButton>--%>
        <asp:Label ID="lblAccordHeader" runat="server"></asp:Label>
        <span style="float: right;">
            <asp:LinkButton ID="lnkBtnClear" CssClass="RemovePanelLink" OnClientClick="var confirmed = confirm('Clear this Vehicles Coverages?'); if(confirmed)DisableFormOnSaveRemoves(); return confirmed;" ToolTip="Reset Vehicle Coverages to Default Values" runat="server">Clear</asp:LinkButton>
            <asp:LinkButton ID="lnkBtnSave" CssClass="RemovePanelLink" ToolTip="Save all vehicles." runat="server">Save</asp:LinkButton>
        </span>
        </asp:Panel>
    </h3>
    <div>
        <div id="dvLiabDisclaim" runat="server" style="display:none">
            <asp:Label ID="lblLiabDisclaim" runat="server" Text="Policy Liability above extends from towing vehicle. The above coverages do not exist independently on a trailer, and no premiums for them will be applied to this trailer. See Auto Manual for additional questions." ForeColor="Red"></asp:Label>
        </div>
        <div id="dvCompDisclaim" runat="server" style="display:none">
            <asp:Label ID="lblCompDisclaim" runat="server" Text="Policy Liability above does not apply to vehicles with Comp Only coverage, and no premiums for them will be applied to this vehicle. See Auto Manual for additional questions." ForeColor="Red"></asp:Label>
        </div>
        <table style="width: 100%; vertical-align: top">
            <tr>
                <td style="width: 60%; vertical-align: top">
                    <asp:Panel ID="pnlCompChk" runat="server">
                        <table style="width: 100%">
                            <tr>
                                <td style="width: 30%">
                                    <asp:Label ID="txtPolicy" runat="server" Text="Vehicle Plan"></asp:Label>
                                </td>
                                <td>
                                    <asp:DropDownList ID="ddPolicy" Width="245px" runat="server"></asp:DropDownList>
                                </td>
                            </tr>
                        </table>
                    </asp:Panel>
                </td>
                <td align="right" style="width:20%">
                    <asp:Button ID="btnCopyCoverage" runat="server" Text="Use Previous Vehicle" ToolTip="Previous Vehicle MUST be saved before copying" />
                </td>
            </tr>
            <tr>
                <td colspan="2">
                    <div id="dvDisclaim" runat="server" style="display:none">
                        <asp:Label ID="lblDisclaim" runat="server" Text="In order to avoid potential claim settlement problems, Indiana Farmers must insure all eligible private passenger vehicles owned by the named insured."></asp:Label>
                    </div>
                </td>
            </tr>
        </table>
        <table style="width: 100%; vertical-align: top" title="Vehicle Coverages">
            <tr>
                <td style="width: 50%; vertical-align: top">
                    <div id="dvComp" runat="server">
                        <table style="width: 100%">
                            <tr>
                                <td style="width: 70%">
                                    <asp:Label ID="lblComp" runat="server" Text="Comprehensive"></asp:Label>
                                </td>
                                <td>
                                    <asp:DropDownList ID="ddComprehensive" Width="80px" runat="server"></asp:DropDownList>
                                </td>
                            </tr>
                        </table>
                    </div>
                    <div id="dvColl" runat="server">
                        <table style="width: 100%">
                            <tr>
                                <td style="width: 70%">
                                    <asp:Label ID="lblCollision" runat="server" Text="Collision"></asp:Label>
                                </td>
                                <td>
                                    <asp:DropDownList ID="ddCollision" Width="80px" runat="server"></asp:DropDownList>
                                </td>
                            </tr>
                        </table>
                    </div>
                    <div id="dvTowing" runat="server">
                        <table style="width: 100%">
                            <tr>
                                <td style="width: 70%">
                                    <asp:Label ID="lblTowing" runat="server" Text="Towing and Labor"></asp:Label>
                                </td>
                                <td>
                                    <asp:DropDownList ID="ddTowing" Width="80px" runat="server"></asp:DropDownList>
                                </td>
                            </tr>
                        </table>
                    </div>
                    <div id="dvTransportation" runat="server">
                        <table style="width: 100%">
                            <tr>
                                <td style="width: 70%">
                                    <asp:Label ID="lblTransporation" runat="server" Text="Transportation Expense" ToolTip="20/600 is offered on the base policy"></asp:Label>
                                </td>
                                <td>
                                    <asp:DropDownList ID="ddTransportation" Width="80px" runat="server"></asp:DropDownList>
                                </td>
                            </tr>
                        </table>
                    </div>
                    <div id="dvTravel" runat="server">
                        <table style="width: 100%">
                            <tr>
                                <td style="width: 70%">
                                    <asp:Label ID="lblInterruptionOfTravel" runat="server" Text="Add Interruption of Travel Coverage" ToolTip="Coverage up to $300 is provided for towing and emergency road services, meals, safety riding apparel, lodging and transportation following an interruption of your trip more than 100 miles from your residence. This interruption must be the result of a covered loss. No deductible applies to this coverage."></asp:Label>
                                </td>
                                <td>
                                    <asp:CheckBox ID="chkInterruptionOfTravel" runat="server" Enabled="false" />
                                </td>
                            </tr>
                        </table>
                    </div>
                    <div id="dvRadio" runat="server">
                        <table style="width: 100%">
                            <tr>
                                <td style="width: 70%">
                                    <asp:Label ID="lblRadio" runat="server" Text="Sound Equipment" ToolTip="Sound producing equipment increase limited with 1,000 added on base policy. Must have Comp and Collision to add endorsement. See form PP 03 13 for more information."></asp:Label>
                                </td>
                                <td>
                                    <asp:DropDownList ID="ddRadio" Width="80px" runat="server"></asp:DropDownList>
                                </td>
                            </tr>
                        </table>
                    </div>
                </td>
                <td style="vertical-align: top">
                    <div id="dvAVEquip" runat="server">
                        <table style="width: 100%">
                            <tr>
                                <td style="width: 70%">
                                    <asp:Label ID="lblAudioVisual" runat="server" Text="Audio, Visual, and Data Equipment" ToolTip="Audio, video, and data electronic equipment excluding radar and laser detectors. See form PP 03 13 for more information"></asp:Label>
                                </td>
                                <td>
                                    <asp:DropDownList ID="ddAudioVisual" runat="server" Width="80px"></asp:DropDownList>
                                </td>
                            </tr>
                        </table>
                    </div>
                    <div id="dvMedia" runat="server">
                        <table style="width: 100%">
                            <tr>
                                <td style="width: 70%">
                                    <asp:Label ID="lblMedia" runat="server" Text="Tapes, Records, Discs and Other Media" ToolTip="$200 included in base policy, but $400 available"></asp:Label>
                                </td>
                                <td>
                                    <asp:DropDownList ID="ddMedia" Width="80px" runat="server"></asp:DropDownList>
                                </td>
                            </tr>
                        </table>
                    </div>
                    <div id="dvCostNew" runat="server">
                        <table style="width: 100%">
                            <tr>
                                <td style="width: 70%">
                                    <%--<asp:Label ID="lblCostNew" runat="server" Text="Cost New"></asp:Label>--%>
                                    <label for="<%=Me.txtCostNew.ClientID%>">Cost New:</label>
                                </td>
                                <td>
                                    <%--<asp:TextBox ID="txtCostNew" Width="80px" runat="server"></asp:TextBox>--%>
                                    <asp:TextBox ID="txtCostNew" onkeyup='$(this).val(FormatAsCurrency($(this).val(),""));' runat="server" MaxLength="11"></asp:TextBox>
                                </td>
                            </tr>
                        </table>
                    </div>
                    <div id="dvLoanLease" runat="server">
                        <table style="width: 100%">
                            <tr>
                                <td style="width: 70%">
                                    <asp:Label ID="lblAutoLoanLease" runat="server" Text="Loan/Lease" ToolTip="Only available if vehicle is within 5 model years of current at the time the vehicle is added to a policy"></asp:Label>
                                </td>
                                <td>
                                    <asp:CheckBox ID="chkAutoLoanLease" runat="server" />
                                </td>
                            </tr>
                        </table>
                    </div>
                    <div id="dvUMPD" runat="server">
                        <table style="width: 100%">
                            <tr>
                                <td style="width: 70%">
                                    <asp:Label ID="lblUMPD" runat="server" Text="UMPD"></asp:Label>
                                </td>
                                <td>
                                    <asp:CheckBox ID="chkUMPD" runat="server" />
                                </td>
                            </tr>
                            <tr id="trUMPDLimitTB" runat="server">
                                <td style="width: 70%;padding-left:20px;">
                                    <asp:Label ID="lblUMPDLimit" runat="server" Text="Limit"></asp:Label>
                                </td>
                                <td>
                                    <asp:TextBox ID="txtUMPDLimit" runat="server" Width="60px" ForeColor="Gray" ReadOnly="true"></asp:TextBox>
                                </td>
                            </tr>
                            <tr id="trUMPDLimitDD" runat="server">
                                <td style="width: 70%;padding-left:20px;">
                                    <asp:Label ID="lblUMPDLimitDD" runat="server" Text="Limit"></asp:Label>
                                </td>
                                <td>
                                    <asp:DropDownList ID="ddUMPD" Width="80px" runat="server" class="UMPD_IL_LimitMatch"></asp:DropDownList>
                                </td>
                            </tr>
                            <tr id="trUMPDLimitMsg" runat="server">
                                <td colspan="2" class="informationalText" style="width:100%;padding-left:20px;">All vehicles with UMPD coverage have the same limit.</td>
                            </tr>
                        </table>
                    </div>
                    <div id="dvPolution" runat="server">
                        <table style="width: 100%">
                            <tr>
                                <td style="width: 70%">
                                    <asp:Label ID="lblPollution" runat="server" Text="Pollution Liability"></asp:Label>
                                </td>
                                <td>
                                    <asp:CheckBox ID="chkPollution" runat="server" />
                                </td>
                            </tr>
                        </table>
                    </div>
                    <div id="dvMotorcycle" runat="server">
                        <table style="width: 100%">
                            <tr>
                                <td style="width: 70%">
                                    <asp:Label ID="lblMotorEquip" runat="server" Text="Motorcycle Customized Equipment" ToolTip="Coverage calculated in cost per $3,000. Enter in full amount of needed coverage"></asp:Label>
                                </td>
                                <td>
                                    <asp:TextBox ID="txtMotorEquip" runat="server" Width="80px">0</asp:TextBox>
                                </td>
                            </tr>
                        </table>
                    </div>
                </td>
            </tr>
        </table>
        <uc1:ctlCoverage_PPA_ScheduledItemsList runat="server" ID="ctlCoverage_PPA_ScheduledItemsList" visible="false"/>

        <asp:HiddenField ID="hiddenVehicleCoverage" runat="server" />
        <asp:HiddenField ID="hiddenVehicleYear" runat="server" />
        <asp:HiddenField ID="hiddenVehicleType" runat="server" />
        <asp:HiddenField ID="hiddenVehicleUse" runat="server" />
        <asp:HiddenField ID="hiddenNNO" runat="server" />
        <asp:HiddenField ID="hiddenVehicleNumber" runat="server" Value="0" />
        <asp:HiddenField ID="hiddenVehicleCoveragePlan" runat="server" />
        <asp:HiddenField ID="hiddenActiveVehicle" runat="server" />
    </div>
<%--</div>--%>