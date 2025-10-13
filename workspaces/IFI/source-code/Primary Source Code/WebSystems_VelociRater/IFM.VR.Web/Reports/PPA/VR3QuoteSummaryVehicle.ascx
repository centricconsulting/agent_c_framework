<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="VR3QuoteSummaryVehicle.ascx.vb" Inherits="IFM.VR.Web.VR3QuoteSummaryVehicle" %>

<style type="text/css">
    P.breakhere {
        page-break-before: always;
    }
    td.ILPoliceTrainingFee {
        height:35px;
    }
</style>

<%--<link href="~/styles/vr.css" rel="stylesheet" type="text/css" />--%>

<p class="breakhere"></p>
<table style="width: 100%; vertical-align: top; font-family: Calibri; font-size: medium">
    <tr>
        <td id="VehicleHdrRow" runat="server" style="vertical-align: top">
            <table style="width: 100%">
                <tr>
                    <td>
                        <hr />
                        <table style="width: 100%">
                            <tr>
                                <td style="vertical-align: top">
                                    <asp:Label ID="lblVehicleHdr" runat="server" Text="Vehicle Information"></asp:Label>
                                </td>
                            </tr>
                        </table>
                        <hr />
                        <table style="width: 100%">
                            <tr>
                                <td style="vertical-align: top">
                                    <asp:Label ID="lblVehYear" runat="server" Text="Year"></asp:Label>
                                    <br />
                                    <asp:Label ID="lblVehMake" runat="server" Text="Make"></asp:Label>
                                    <br />
                                    <asp:Label ID="lblVehModel" runat="server" Text="Model"></asp:Label>
                                    <br />
                                    <asp:Label ID="lblVehVIN" runat="server" Text="VIN"></asp:Label>
                                    <br />
                                    <asp:Label ID="lblVehTerr" runat="server" Text="Territory"></asp:Label>
                                    <br />
                                    <asp:Label ID="lblVehSym" runat="server" Text="Symbol" Visible="false"></asp:Label>
                                    <div runat="server" id="divCVehclass1">                                        
                                        <asp:Label ID="lblVehClass" runat="server" Text="Class"></asp:Label>
                                    </div> 
                                    <div>
                                        <asp:Label ID="lblVehType" runat="server" Text="Type"></asp:Label>
                                    </div>                                                                       
                                    
                                </td>
                            </tr>
                        </table>
                        <hr />
                        <table style="width: 100%">
                            <tr>
                                <td style="vertical-align: top">
                                    <asp:Label ID="lblCoveragesHdr" runat="server" Text="Coverages"></asp:Label>
                                </td>
                            </tr>
                        </table>
                        <hr />
                        <table style="width: 100%">
                            <tr>
                                <td style="vertical-align: top">
                                    <asp:Panel ID="pnlSLLRow" runat="server" Visible="false">
                                        <table>
                                            <tr>
                                                <td>
                                                    <asp:Label ID="lblSLLRow" runat="server" Text="Single Limit Liability"></asp:Label>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <asp:Label ID="lblUMUIMCSLRow" runat="server" Text="UM/UIM CSL"></asp:Label>
                                                </td>
                                            </tr>
                                            <tr id="trUMPDRow" runat="server">
                                                <td>
                                                    <asp:Label ID="lblUMPDRow" runat="server" Text="Uninsured Property Damage"></asp:Label>
                                                </td>
                                            </tr>
                                        </table>
                                    </asp:Panel>
                                    <asp:Panel ID="pnlSplitRow" runat="server" Visible="false">
                                        <table>
                                            <tr>
                                                <td>
                                                    <asp:Label ID="lblBodilyRow" runat="server" Text="Bodily Injury"></asp:Label>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <asp:Label ID="lblPropertyRow" runat="server" Text="Property Damage"></asp:Label>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <asp:Label ID="lblUMBodilyRow" runat="server" Text="UM/UIM Bodily Injury"></asp:Label>
                                                </td>
                                            </tr>
                                            <tr id="trUMPropertyRow" runat="server">
                                                <td>
                                                    <asp:Label ID="lblUMPropertyRow" runat="server" Text="Uninsured Property Damage"></asp:Label>
                                                </td>
                                            </tr>
                                        </table>
                                    </asp:Panel>
                                    <table>
                                        <tr>
                                            <td>
                                                <asp:Label ID="lblMedicalRow" runat="server" Text="Medical Payments"></asp:Label>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <asp:Label ID="lblCompRow" runat="server" Text="Comprehensive"></asp:Label>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <asp:Label ID="lblCollRow" runat="server" Text="Collision"></asp:Label>
                                            </td>
                                        </tr>
                                        <tr id="trPoliceTrainingFeeRow" runat="server" visible="false">
                                            <td class="ILPoliceTrainingFee">
                                                <asp:Label ID="lblPoliceTrainingFeeRow" runat="server" Text="Illinois State Police Training<br />and Academy Fund"></asp:Label>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                        </table>
                        <hr />
                        <table style="width: 100%">
                            <tr>
                                <td style="vertical-align: top">
                                    <asp:Label ID="lblOptCoverageSection" runat="server" Text="Optional Coverages"></asp:Label>
                                </td>
                            </tr>
                        </table>
                        <hr />
                        <table style="width: 100%">
                            <tr>
                                <td style="vertical-align: top">
                                    <table>
                                        <tr>
                                            <td>
                                                <asp:Label ID="lblTowingRow" runat="server" Text="Towing"></asp:Label>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <asp:Label ID="lblRentalRow" runat="server" Text="Rental"></asp:Label>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <asp:Label ID="lblTripInterruptRow" runat="server" Text="Trip Interruption"></asp:Label>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <asp:Label ID="lblSoundEquipRow" runat="server" Text="Sound Equipment"></asp:Label>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <asp:Label ID="lblAudioVisualRow" runat="server" Text="Audio,Visual,Data,Other Media"></asp:Label>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <asp:Label ID="lblTapesRow" runat="server" Text="Tapes,Records,Discs"></asp:Label>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <asp:Label ID="lblLoanRow" runat="server" Text="Loan/Lease"></asp:Label>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <asp:Label ID="lblPollutionRow" runat="server" Text="Pollution Liability"></asp:Label>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <asp:Label ID="lblCustomEquipRow" runat="server" Text="Motorcycle Custom Equipment"></asp:Label>
                                            </td>
                                        </tr>
                                        <tr id="trCustomEquipmentRow" runat="server">
                                            <td>
                                                <asp:Label ID="lblCustomEquipmentRow" runat="server" Text="Customized Equipment"></asp:Label>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                        </table>
                       <%-- <hr />
                        <table style="width: 100%">
                            <tr>
                                <td id="VehDiscountsHdr" runat="server" style="vertical-align: top">
                                    <asp:Label ID="lblDiscountsHdr" runat="server" Text="Discounts" Width="20%"></asp:Label>
                                </td>
                            </tr>
                        </table>
                        <hr />
                        <table style="width: 100%">
                            <tr>
                                <td style="vertical-align: top">
                                    <asp:Label ID="lblTheftRow" runat="server" Text="Anti-Theft"></asp:Label>
                                    <br />
                                    <asp:Label ID="lblRestraintRow" runat="server" Text="Passive Restraint(Med Pay Only)"></asp:Label>
                                </td>
                            </tr>
                        </table>--%>
                        <hr />
                        <table style="width: 100%">
                            <tr>
                                <td align="right" style="vertical-align: top">
                                    <asp:Label ID="lblVehicleTotalRow" runat="server" Text="Vehicle Total"></asp:Label>
                                </td>
                            </tr>
                        </table>
                        <hr />
                    </td>
                </tr>
            </table>
        </td>
        <td></td>
        <td style="vertical-align: top">
            <table style="width: 100%">
                <tr>
                    <td>
                        <hr />
                        <table style="width: 100%">
                            <tr>
                                <td style="vertical-align: top">
                                    <asp:Label ID="lblVehNo" runat="server" Text="Vehicle #"></asp:Label>
                                    &nbsp;
                                    <asp:Label ID="lblVehicle" runat="server"></asp:Label>
                                </td>
                            </tr>
                        </table>
                        <hr />
                        <table style="width: 100%">
                            <tr>
                                <td style="vertical-align: top">
                                    <asp:Label ID="lblYear" runat="server"></asp:Label>
                                    <br />
                                    <asp:Label ID="lblMake" runat="server"></asp:Label>
                                    <br />
                                    <asp:Label ID="lblModel" runat="server"></asp:Label>
                                    <br />
                                    <asp:Label ID="lblVIN" runat="server"></asp:Label>
                                    <br />
                                    <asp:Label ID="lblTerritory" runat="server"></asp:Label>
                                    <br />
                                    <asp:Label ID="lblSymbol" runat="server" Visible="false"></asp:Label>
                                    <div runat="server" id="divCVehclass2">
                                        <asp:Label ID="lblClass" runat="server"></asp:Label>
                                    </div>                                    
                                    <div>
                                        <asp:Label ID="lblType" runat="server"></asp:Label>
                                    </div>
                                    
                                </td>
                            </tr>
                        </table>
                        <hr />
                        <table style="width: 100%">
                            <tr>
                                <td style="vertical-align: top; width: 75%">
                                    <asp:Label ID="lblDeduct" runat="server" Text="Limit/Deductible"></asp:Label>
                                </td>
                                <td></td>
                                <td></td>
                                <td style="vertical-align: top; width: 40%">
                                    <asp:Label ID="lblPremium" runat="server" Text="Premium"></asp:Label>
                                </td>
                            </tr>
                        </table>
                        <hr />
                        <table style="width: 100%">
                            <tr>
                                <td style="vertical-align: top; width: 75%">
                                    <asp:Panel ID="pnlSSL" runat="server" Visible="false">
                                        <table>
                                            <tr>
                                                <td>
                                                    <asp:Label ID="lblSSL" runat="server"></asp:Label>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <asp:Label ID="lblUMUIMCSL" runat="server"></asp:Label>
                                                </td>
                                            </tr>
                                            <tr id="trUMPD" runat="server">
                                                <td>
                                                    <asp:Label ID="lblUMPD" runat="server"></asp:Label>
                                                </td>
                                            </tr>
                                        </table>
                                    </asp:Panel>
                                    <asp:Panel ID="pnlSplit" runat="server" Visible="false">
                                        <table>
                                            <tr>
                                                <td>
                                                    <asp:Label ID="lblBodily" runat="server"></asp:Label>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <asp:Label ID="lblProperty" runat="server"></asp:Label>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <asp:Label ID="lblUMBodily" runat="server"></asp:Label>
                                                </td>
                                            </tr>
                                            <tr id="trUMProperty" runat="server">
                                                <td>
                                                    <asp:Label ID="lblUMProperty" runat="server"></asp:Label>
                                                </td>
                                            </tr>
                                        </table>
                                    </asp:Panel>
                                    <table>
                                        <tr>
                                            <td>
                                                <asp:Label ID="lblMedical" runat="server"></asp:Label>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <asp:Label ID="lblComp" runat="server"></asp:Label>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <asp:Label ID="lblColl" runat="server"></asp:Label>
                                            </td>
                                        </tr>
                                        <tr id="trPoliceTrainingFee" runat="server" visible="false">
                                            <td class="ILPoliceTrainingFee">
                                                <asp:Label ID="lblPoliceTrainingFee" runat="server" Text="N/A"></asp:Label>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                                <td></td>
                                <td></td>
                                <td align="right" style="vertical-align: top">
                                    <asp:Panel ID="pnlSSLPrem" runat="server" Visible="false">
                                        <table>
                                            <tr>
                                                <td align="right">
                                                    <asp:Label ID="lblSSLPrem" runat="server"></asp:Label>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="right">
                                                    <asp:Label ID="lblUMUIMCSLPrem" runat="server"></asp:Label>
                                                </td>
                                            </tr>
                                            <tr id="trUMPDPrem" runat="server">
                                                <td align="right">
                                                    <asp:Label ID="lblUMPDPrem" runat="server"></asp:Label>
                                                </td>
                                            </tr>
                                        </table>
                                    </asp:Panel>
                                    <asp:Panel ID="pnlSplitPrem" runat="server" Visible="false">
                                        <table>
                                            <tr>
                                                <td align="right">
                                                    <asp:Label ID="lblBodilyPrem" runat="server"></asp:Label>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="right">
                                                    <asp:Label ID="lblPropertyPrem" runat="server"></asp:Label>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="right">
                                                    <asp:Label ID="lblUMBodilyPrem" runat="server"></asp:Label>
                                                </td>
                                            </tr>
                                            <tr id="trUMPropertyPrem" runat="server">
                                                <td align="right">
                                                    <asp:Label ID="lblUMPropertyPrem" runat="server"></asp:Label>
                                                </td>
                                            </tr>
                                        </table>
                                    </asp:Panel>
                                    <table>
                                        <tr>
                                            <td align="right">
                                                <asp:Label ID="lblMedicalPrem" runat="server"></asp:Label>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="right">
                                                <asp:Label ID="lblCompPrem" runat="server"></asp:Label>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="right">
                                                <asp:Label ID="lblCollPrem" runat="server"></asp:Label>
                                            </td>
                                        </tr>
                                        <tr id="trPoliceTrainingFeePrem" runat="server" visible="false">
                                            <td align="right" class="ILPoliceTrainingFee">
                                                <asp:Label ID="lblPoliceTrainingFeePrem" runat="server"></asp:Label>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                        </table>
                        <hr />
                        <table style="width: 100%">
                            <tr>
                                <td style="vertical-align: top; width: 75%; min-height: 21px; height: 21px"></td>
                                <td style="vertical-align: top"></td>
                            </tr>
                        </table>
                        <hr />
                        <table style="width: 100%">
                            <tr>
                                <td style="vertical-align: top; width: 75%">
                                    <table>
                                        <tr>
                                            <td>
                                                <asp:Label ID="lblTowing" runat="server"></asp:Label>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <asp:Label ID="lblRental" runat="server"></asp:Label>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <asp:Label ID="lblTrip" runat="server"></asp:Label>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <asp:Label ID="lblSound" runat="server"></asp:Label>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <asp:Label ID="lblAudio" runat="server"></asp:Label>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <asp:Label ID="lblTapes" runat="server"></asp:Label>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <asp:Label ID="lblLoan" runat="server"></asp:Label>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <asp:Label ID="lblPollution" runat="server"></asp:Label>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <asp:Label ID="lblCustom" runat="server"></asp:Label>
                                            </td>
                                        </tr>
                                        <tr id="trCustomEquipmentLabel" runat="server">
                                            <td>
                                                <asp:Label ID="lblCustomEquipment" runat="server"></asp:Label>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                                <td></td>
                                <td></td>
                                <td align="right" style="vertical-align: top">
                                    <table>
                                        <tr>
                                            <td align="right">
                                                <asp:Label ID="lblTowPrem" runat="server"></asp:Label>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="right">
                                                <asp:Label ID="lblRentPrem" runat="server"></asp:Label>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="right">
                                                <asp:Label ID="lblTripPrem" runat="server"></asp:Label>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="right">
                                                <asp:Label ID="lblSoundPrem" runat="server"></asp:Label>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="right">
                                                <asp:Label ID="lblAudioPrem" runat="server"></asp:Label>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="right">
                                                <asp:Label ID="lblTapesPrem" runat="server"></asp:Label>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="right">
                                                <asp:Label ID="lblLoanPrem" runat="server"></asp:Label>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="right">
                                                <asp:Label ID="lblPollutionPrem" runat="server"></asp:Label>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="right">
                                                <asp:Label ID="lblCustomPrem" runat="server"></asp:Label>
                                            </td>
                                        </tr>
                                        <tr id="trCustomEquipmentPrem" runat="server">
                                            <td align="right">
                                                <asp:Label ID="lblCustomEquipmentPrem" runat="server"></asp:Label>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                        </table>
                        <%--<hr />
                        <table style="width: 100%">
                            <tr>
                                <td style="vertical-align: top">
                                    <asp:Label ID="lblDiscHdr" runat="server" Text="Percentage Applied"></asp:Label>
                                </td>
                            </tr>
                        </table>
                        <hr />
                        <table style="width: 100%">
                            <tr>
                                <td style="vertical-align: top">
                                    <asp:Label ID="lblTheft" runat="server"></asp:Label>
                                    <br />
                                    <asp:Label ID="lblRestraint" runat="server"></asp:Label>
                                </td>
                            </tr>
                        </table>--%>
                        <hr />
                        <table style="width: 100%">
                            <tr>
                                <td align="right" style="vertical-align: top">
                                    <asp:Label ID="lblVehicleTotal" runat="server"></asp:Label>
                                </td>
                            </tr>
                        </table>
                        <hr />
                    </td>
                </tr>
            </table>
        </td>
    </tr>
</table>