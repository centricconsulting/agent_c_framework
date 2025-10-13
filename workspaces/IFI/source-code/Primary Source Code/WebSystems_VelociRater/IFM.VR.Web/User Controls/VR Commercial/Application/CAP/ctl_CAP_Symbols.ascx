<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctl_CAP_Symbols.ascx.vb" Inherits="IFM.VR.Web.ctl_CAP_Symbols" %>

<div id="divSymbols" runat="server">
    <h3>
        <asp:Label ID="lblAccordHeader" runat="server" Text="Covered Auto Coverage Symbols"></asp:Label>
            <span style="float: right;">
            <asp:LinkButton ID="lnkSave" ToolTip="Save" runat="server" CssClass="RemovePanelLink">Save</asp:LinkButton>
        </span>
    </h3>
    <div>
        <style>
            .SymbolColumn {
                width:7%;
                text-align:center;
            }
            .SymbolLabelColumn {
                width:40%;
                text-align:right;
            }
            .SymbolHeaderColumn {
                text-align:center;
                text-decoration:underline;
                font-weight:700;
            }
            .SymbolSpacer {
                width:20%;
            }
        </style>
        <table id="tblSymbols" runat="server" style="width:100%;">
            <tr>
                <td class="SymbolLabelColumn">&nbsp;</td>
                <td class="SymbolHeaderColumn">1</td>
                <td class="SymbolHeaderColumn">2</td>
                <td class="SymbolHeaderColumn">3</td>
                <td class="SymbolHeaderColumn">4</td>
                <td class="SymbolHeaderColumn">7</td>
                <td class="SymbolHeaderColumn">8</td>
                <td class="SymbolHeaderColumn">9</td>
            </tr>
            <tr>
                <td class="SymbolLabelColumn">Liability</td>
                <td class="SymbolColumn">
                    <asp:CheckBox ID="chk_Liab_1" runat="server" Text="&nbsp;" AutoPostBack="false" />
                </td>
                <td class="SymbolColumn">
                    <asp:CheckBox ID="chk_Liab_2" runat="server" Text="&nbsp;" AutoPostBack="false" />
                </td>
                <td class="SymbolColumn">
                    <asp:CheckBox ID="chk_Liab_3" runat="server" Text="&nbsp;" AutoPostBack="false" />
                </td>
                <td class="SymbolColumn">
                    <asp:CheckBox ID="chk_Liab_4" runat="server" Text="&nbsp;" AutoPostBack="false" />
                </td>
                <td class="SymbolColumn">
                    <asp:CheckBox ID="chk_Liab_7" runat="server" Text="&nbsp;" AutoPostBack="false" />
                </td>
                <td class="SymbolColumn">
                    <asp:CheckBox ID="chk_Liab_8" runat="server" Text="&nbsp;" AutoPostBack="false" />
                </td>
                <td class="SymbolColumn">
                    <asp:CheckBox ID="chk_Liab_9" runat="server" Text="&nbsp;" AutoPostBack="false" />
                </td>
            </tr>
            <tr>
                <td class="SymbolLabelColumn">Medical Payments</td>
                <td class="SymbolColumn">
                    &nbsp;
                </td>
                <td class="SymbolColumn">
                    <asp:CheckBox ID="chk_MedPay_2" runat="server" Text="&nbsp;" AutoPostBack="false" />
                </td>
                <td class="SymbolColumn">
                    <asp:CheckBox ID="chk_MedPay_3" runat="server" Text="&nbsp;" AutoPostBack="false" />
                </td>
                <td class="SymbolColumn">
                    <asp:CheckBox ID="chk_MedPay_4" runat="server" Text="&nbsp;" AutoPostBack="false" />
                </td>
                <td class="SymbolColumn">
                    <asp:CheckBox ID="chk_MedPay_7" runat="server" Text="&nbsp;" AutoPostBack="false" />
                </td>
                <td class="SymbolColumn">
                    &nbsp;
                </td>
                <td class="SymbolColumn">
                    &nbsp;
                </td>
            </tr>
            <tr id="trUMUIMSymbols">
                <td class="SymbolLabelColumn">Uninsured/Underinsured Motorist</td>
                <td class="SymbolColumn">
                    &nbsp;
                </td>
                <td class="SymbolColumn">
                    <asp:CheckBox ID="chk_UMUIM_2" runat="server" Text="&nbsp;" AutoPostBack="false" />
                </td>
                <td class="SymbolColumn">
                    <asp:CheckBox ID="chk_UMUIM_3" runat="server" Text="&nbsp;" AutoPostBack="false" />
                </td>
                <td class="SymbolColumn">
                    <asp:CheckBox ID="chk_UMUIM_4" runat="server" Text="&nbsp;" AutoPostBack="false" />
                </td>
                <td class="SymbolColumn">
                    <asp:CheckBox ID="chk_UMUIM_7" runat="server" Text="&nbsp;" AutoPostBack="false" />
                </td>
                <td class="SymbolColumn">
                    &nbsp;
                </td>
                <td class="SymbolColumn">
                    &nbsp;
                </td>
            </tr>
            <tr id="trUMSymbols" style="display:none;">
                <td class="SymbolLabelColumn">Uninsured Motorist</td>
                <td class="SymbolColumn">
                    &nbsp;
                </td>
                <td class="SymbolColumn">
                    <asp:CheckBox ID="chk_UM_2" runat="server" Text="&nbsp;" AutoPostBack="false" />
                </td>
                <td class="SymbolColumn">
                    <asp:CheckBox ID="chk_UM_3" runat="server" Text="&nbsp;" AutoPostBack="false" />
                </td>
                <td class="SymbolColumn">
                    <asp:CheckBox ID="chk_UM_4" runat="server" Text="&nbsp;" AutoPostBack="false" />
                </td>
                <td class="SymbolColumn">
                    <asp:CheckBox ID="chk_UM_7" runat="server" Text="&nbsp;" AutoPostBack="false" />
                </td>
                <td class="SymbolColumn">
                    &nbsp;
                </td>
                <td class="SymbolColumn">
                    &nbsp;
                </td>
            </tr>
            <tr id="trUIMSymbols" style="display:none;">
                <td class="SymbolLabelColumn">Underinsured Motorist</td>
                <td class="SymbolColumn">
                    &nbsp;
                </td>
                <td class="SymbolColumn">
                    <asp:CheckBox ID="chk_UIM_2" runat="server" Text="&nbsp;" AutoPostBack="false" />
                </td>
                <td class="SymbolColumn">
                    <asp:CheckBox ID="chk_UIM_3" runat="server" Text="&nbsp;" AutoPostBack="false" />
                </td>
                <td class="SymbolColumn">
                    <asp:CheckBox ID="chk_UIM_4" runat="server" Text="&nbsp;" AutoPostBack="false" />
                </td>
                <td class="SymbolColumn">
                    <asp:CheckBox ID="chk_UIM_7" runat="server" Text="&nbsp;" AutoPostBack="false" />
                </td>
                <td class="SymbolColumn">
                    &nbsp;
                </td>
                <td class="SymbolColumn">
                    &nbsp;
                </td>
            </tr>
            <tr>
                <td class="SymbolLabelColumn">Comprehensive</td>
                <td class="SymbolColumn">
                    &nbsp;
                </td>
                <td class="SymbolColumn">
                    <asp:CheckBox ID="chk_Comp_2" runat="server" Text="&nbsp;" AutoPostBack="false" />
                </td>
                <td class="SymbolColumn">
                    <asp:CheckBox ID="chk_Comp_3" runat="server" Text="&nbsp;" AutoPostBack="false" />
                </td>
                <td class="SymbolColumn">
                    <asp:CheckBox ID="chk_Comp_4" runat="server" Text="&nbsp;" AutoPostBack="false" />
                </td>
                <td class="SymbolColumn">
                    <asp:CheckBox ID="chk_Comp_7" runat="server" Text="&nbsp;" AutoPostBack="false" />
                </td>
                <td class="SymbolColumn">
                    <asp:CheckBox ID="chk_Comp_8" runat="server" Text="&nbsp;" AutoPostBack="false" />
                </td>
                <td class="SymbolColumn">
                    &nbsp;
                </td>
            </tr>
            <tr>
                <td class="SymbolLabelColumn">Collision</td>
                <td class="SymbolColumn">
                    &nbsp;
                </td>
                <td class="SymbolColumn">
                    <asp:CheckBox ID="chk_Coll_2" runat="server" Text="&nbsp;" AutoPostBack="false" />
                </td>
                <td class="SymbolColumn">
                    <asp:CheckBox ID="chk_Coll_3" runat="server" Text="&nbsp;" AutoPostBack="false" />
                </td>
                <td class="SymbolColumn">
                    <asp:CheckBox ID="chk_Coll_4" runat="server" Text="&nbsp;" AutoPostBack="false" />
                </td>
                <td class="SymbolColumn">
                    <asp:CheckBox ID="chk_Coll_7" runat="server" Text="&nbsp;" AutoPostBack="false" />
                </td>
                <td class="SymbolColumn">
                    <asp:CheckBox ID="chk_Coll_8" runat="server" Text="&nbsp;" AutoPostBack="false" />
                </td>
                <td class="SymbolColumn">
                    &nbsp;
                </td>
            </tr>
            <tr>
                <td class="SymbolLabelColumn">Towing and Labor</td>
                <td class="SymbolColumn">
                    &nbsp;
                </td>
                <td class="SymbolColumn">
                    &nbsp;
                </td>
                <td class="SymbolColumn">
                    &nbsp;
                </td>
                <td class="SymbolColumn">
                    &nbsp;
                </td>
                <td class="SymbolColumn">
                    <asp:CheckBox ID="chk_Tow_7" runat="server" Text="&nbsp;" AutoPostBack="false" />
                </td>
                <td class="SymbolColumn">
                    &nbsp;
                </td>
                <td class="SymbolColumn">
                    &nbsp;
                </td>
            </tr>
        </table>
        <br />
        <table id="tblInfo" runat="server" style="width:100%">
            <tr>
                <td class="SymbolSpacer">&nbsp;</td>
                <td style="text-decoration:underline;">Covered Auto Symbols Selection</td>
            </tr>
            <tr>
                <td class="SymbolSpacer">&nbsp;</td>
                <td>1. Any "Auto"</td>
            </tr>
            <tr>
                <td class="SymbolSpacer">&nbsp;</td>
                <td>2. Owned "Autos" Only</td>
            </tr>
            <tr>
                <td class="SymbolSpacer">&nbsp;</td>
                <td>3. Owned Private Passenger "Autos" Only</td>
            </tr>
            <tr>
                <td class="SymbolSpacer">&nbsp;</td>
                <td>4. Owned "Autos" Other Than Private Passenger "Autos" Only</td>
            </tr>
            <tr>
                <td class="SymbolSpacer">&nbsp;</td>
                <td>7. Specifically Described "Autos"</td>
            </tr>
            <tr>
                <td class="SymbolSpacer">&nbsp;</td>
                <td>8. Hired "Autos"</td>
            </tr>
            <tr>
                <td class="SymbolSpacer">&nbsp;</td>
                <td>9. Non-Owned "Autos"</td>
            </tr>
        </table>
    </div>        
    <asp:HiddenField ID="hdnAccord" runat="server" />
</div>