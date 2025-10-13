<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctlPropertyCoverages.ascx.vb" Inherits="IFM.VR.Web.ctlPropertyCoverages" %>

<script type="text/javascript">
    // Information Popup
    function InitBldTypePopupInfo(divName, popTitle) {
        $("#" + divName).dialog({
            title: popTitle,
            width: 600,
            draggable: true,
            autoOpen: true,
            modal: true,
            dialogClass: "no-close"
        });

        DisplayBldTypePopupInfo(divName);
    }

    function DisplayBldTypePopupInfo(divName) {
        $("#" + divName).dialog('open');
    }

    function CloseBldTypePopupInfo(divName) {
        $("#" + divName).dialog('close');
    }
</script>

<style type="text/css">
    .table {
        border-collapse: collapse;
        width: 100%;
    }

        .table td {
            margin: 0;
            padding: 0;
        }
</style>

<div id="dvFarmBuildingProperty" runat="server" class="standardSubSection" onkeydown="return (event.keyCode!=13)">
    <h3>
        <asp:Label ID="lblMainHeader" runat="server" Text="Property"></asp:Label>
        <span style="float: right;">
            <asp:LinkButton ID="lnkClearProperty" runat="server" OnClientClick="var confirmed = confirm('Clear ALL Location Data?'); if(confirmed)DisableFormOnSaveRemoves(); return confirmed;" ToolTip="Reset Property to Default Values" CssClass="RemovePanelLink">Clear</asp:LinkButton>
            <asp:LinkButton ID="lnkSaveProperty" runat="server" ToolTip="Save Property" CssClass="RemovePanelLink">Save</asp:LinkButton>
        </span>
    </h3>
    <div>
        <table id="tbProperty" runat="server" style="width: 100%" class="table">
            <tr>
                <td id="tdBuilding" runat="server" style="width: 50%">
                    <div>
                        <asp:Label ID="lblBldgReq" runat="server" Text="*"></asp:Label>
                        <asp:Label ID="lblBuilding" runat="server" Text="Building"></asp:Label>
                        <br />
                        <asp:DropDownList ID="ddlBuilding" runat="server"></asp:DropDownList>
                    </div>
                </td>
                <td id="tdConstruction" runat="server">
                    <div>
                        <asp:Label ID="lblConstReq" runat="server" Text="*"></asp:Label>
                        <asp:Label ID="lblConstruction" runat="server" Text="Construction"></asp:Label>
                        <br />
                        <asp:DropDownList ID="ddlConstruction" runat="server"></asp:DropDownList>
                    </div>
                </td>
            </tr>
            <tr>
                <td id="tdBuildingHeat" runat="server" style="width: 50%">
                    <div>
                        <asp:CheckBox ID="chkBuildingHeat" runat="server" />
                        <asp:Label ID="lblBuildingHeat" runat="server" Text="Building is Heated"></asp:Label>
                    </div>
                </td>
                <td id="tdHayBuilding" runat="server">
                    <div>
                        <asp:CheckBox ID="chkHayBuilding" runat="server" />
                        <asp:Label ID="lblHayBulding" runat="server" Text="Building Stores Hay"></asp:Label>
                    </div>
                </td>
            </tr>
            <tr>
                <td id="tdLimit" runat="server" style="width: 50%">
                    <div>
                        <asp:Label ID="lblLimitReq" runat="server" Text="*"></asp:Label>
                        <asp:Label ID="lblLimit" runat="server" Text="Limit"></asp:Label>
                        <br />
                        <asp:TextBox ID="txtLimit" runat="server"></asp:TextBox>
                    </div>
                </td>
                <td id="tdDeductible" runat="server">
                    <div>
                        <asp:Label ID="lblDeductReq" runat="server" Text="*"></asp:Label>
                        <asp:Label ID="lblDeductible" runat="server" Text="Deductible"></asp:Label>
                        <br />
                        <asp:DropDownList ID="ddlDeductible" runat="server"></asp:DropDownList>
                    </div>
                </td>
            </tr>
            <tr>
                <td id="tdConstructedYear" runat="server" style="width: 50%">
                    <div>
                        <asp:Label ID="lblConstructedReq" runat="server" Text="*"></asp:Label>
                        <asp:Label ID="lblConstructed" runat="server" Text="Year Constructed"></asp:Label>
                        <br />
                        <asp:TextBox ID="txtConstructed" runat="server"></asp:TextBox>
                    </div>
                </td>
                <td id="tdSqFeet" runat="server">
                    <div>
                        <asp:Label ID="lblSqFeet" runat="server" Text="Square Feet"></asp:Label>
                        <br />
                        <asp:TextBox ID="txtSqFeet" runat="server"></asp:TextBox>
                    </div>
                </td>
            </tr>
            <tr>
                <td id="tdDimensions" runat="server" style="width: 50%">
                    <div>
                        <asp:Label ID="lblDimensionsReq" runat="server" Text="*"></asp:Label>
                        <asp:Label ID="lblDimensions" runat="server" Text="Dimensions"></asp:Label>
                        <br />
                        <asp:TextBox ID="txtDimensions" runat="server"></asp:TextBox>
                    </div>
                </td>
                <td id="tdBuildType" runat="server">
                    <div>
                        <asp:Label ID="lblBuildTypeReq" runat="server" Text="*"></asp:Label>
                        <asp:LinkButton ID="lbtnBuildtype" runat="server">Build Type</asp:LinkButton>
                        <br />
                        <asp:DropDownList ID="ddlBuildType" runat="server"></asp:DropDownList>
                    </div>
                </td>
            </tr>
        </table>
    </div>
    <asp:HiddenField ID="hiddenBuildingProperty" runat="server" />
</div>

<div id="dvFarmBuildingCoverage" runat="server" class="standardSubSection" onkeydown="return (event.keyCode!=13)">
    <h3>
        <asp:Label ID="lblFarmCoverage" runat="server" Text="Coverage"></asp:Label>
        <span style="float: right;">
            <asp:LinkButton ID="lnkClearCoverage" runat="server" OnClientClick="var confirmed = confirm('Clear ALL Location Data?'); if(confirmed)DisableFormOnSaveRemoves(); return confirmed;" ToolTip="Reset Coverage to Default Values" CssClass="RemovePanelLink">Clear</asp:LinkButton>
            <asp:LinkButton ID="lnkSaveCoverage" runat="server" ToolTip="Save Coverage" CssClass="RemovePanelLink">Save</asp:LinkButton>
        </span>
    </h3>
    <div>
        <asp:Label ID="lblOptLiabCoverage" runat="server" Text="OPTIONAL LIABILITY COVERAGES" Font-Bold="True"></asp:Label>
        <br />
        <asp:CheckBox ID="chkContractGrow" runat="server" />
        <div id="dvContractGrowLimit" runat="server" style="display: none">
            <asp:DropDownList ID="ddlContractGrow" runat="server"></asp:DropDownList>
        </div>
        <br />
        <asp:Label ID="lblOptPropCoverage" runat="server" Text="OPTIONAL PROPERTY COVERAGES" Font-Bold="True"></asp:Label>
        <div id="dvPerils" runat="server">
            <asp:CheckBox ID="chkPerils" runat="server" />&nbsp;
            <asp:Label ID="lblPerils" runat="server" Text="Additional Perils"></asp:Label>
        </div>
        <div id="dvEarthContents" runat="server">
            <asp:CheckBox ID="chkEarthContents" runat="server" />&nbsp;
            <asp:Label ID="lblEarthContents" runat="server" Text="Earthquake (Contents)"></asp:Label>
        </div>
        <div id="dvEarthStruct" runat="server">
            <asp:CheckBox ID="chkEarthStruct" runat="server" />&nbsp;
            <asp:Label ID="lblEarthStruct" runat="server" Text="Earthquake (Structures)"></asp:Label>
        </div>
        <div id="dvReplacement" runat="server">
            <asp:CheckBox ID="chkReplacement" runat="server" />&nbsp;
            <asp:Label ID="lblReplacement" runat="server" Text="Replacement Cost"></asp:Label>
        </div>
        <div id="dvSpecialForm" runat="server">
            <asp:CheckBox ID="chkSpecialForm" runat="server" />&nbsp;
            <asp:Label ID="lblSpecialForm" runat="server" Text="Special Form"></asp:Label>
        </div>
        <div id="dvLossIncome" runat="server">
            <asp:CheckBox ID="chkLossIncome" runat="server" />&nbsp;
            <asp:Label ID="lblLossIncome" runat="server" Text="Loss of Income"></asp:Label>
            <div id="dvLossIncomeLimit" runat="server" style="display: none">
                <asp:TextBox ID="txtLossIncomeLimit" runat="server"></asp:TextBox>
            </div>
        </div>
        <div id="dvTheft" runat="server">
            <asp:CheckBox ID="chkTheft" runat="server" />&nbsp;
            <asp:Label ID="lblTheft" runat="server" Text="Theft of Building Materials"></asp:Label>
        </div>
        <div id="dvLivestock" runat="server">
            <asp:CheckBox ID="chkLivestock" runat="server" />&nbsp;
            <asp:Label ID="lblLivestock" runat="server" Text="Suffocation of Livestock"></asp:Label>
            <div id="dvLivestockLimit" runat="server" style="display: none">
                <asp:TextBox ID="txtLivestockLimit" runat="server"></asp:TextBox>
            </div>
        </div>
        <div id="dvMineSub" runat="server">
            <asp:CheckBox ID="chkMineSub" runat="server" />&nbsp;
            <asp:Label ID="lblMineSub" runat="server" Text="Mine Subsidence"></asp:Label>
        </div>
    </div>
    <asp:HiddenField ID="hiddenBuildingCoverage" runat="server" />
</div>

<div id="dvBldTypeInfoPopup" style="display: none">
    <div>
        <asp:Label ID="lblType1" runat="server" Text="Type 1 Minimum Requirements" Font-Bold="True" Font-Underline="True"></asp:Label>
        <br />
        1. Building must be of superior construction, free of any visible defect, and insured for at least 80 percent of the building's actual cash value
        <br />
        2. Must be one story
        <br />
        3. Foundation under all exterior walls must be continuous and of mortared masonry or concrete construction. Type 1 buildings may be considered as meeting the foundation requirements when such buildings are framed on poles of minimum diameter of six inches at ground line, if set a minimum of four feet below ground level. Poles must be pressure-treated with wood preservative
        <br />
        4. Buildings must be fully enclosed with no open sheds attached
        <br />
        5. No hay storage
        <br />
        6. Minimum limit--$10,000
        <br />
        <br />
        <asp:Label ID="lblType2" runat="server" Text="Type 2 Minimum Requirements" Font-Bold="True" Font-Underline="True"></asp:Label>
        <br />
        1. Buildings must have above-average construction, show maintenance with no structural defects and be insured for at least 60 percent of the building's actual cash value
        <br />
        2. Building must have continuous, mortared masonry or concrete foundation under all exterior walls (or the two longest walls in granaries or corn cribs). Buildings framed on poles of minimum six inch diameter at ground line, set a minimum of four feet below ground level, may be considered as satisfying this requirement when the poles have been pressure-treated with wood preservative
        <br />
        3. Open sheds will show as Type 2 Open
        <br />
        4. Minimum limit--$5,000
        <br />
        <br />
        <asp:Label ID="lblType3" runat="server" Text="Type 3 Minimum Requirements" Font-Bold="True" Font-Underline="True"></asp:Label>
        <br />
        1. Buildings of average construction and repair and insured for less than 60 percent of the building's actual cash value
        <br />
        2. Buildings occupied or constructed primarily for:
        <br />
        &emsp;a. Grain grinding
        <br />
        &emsp;b. Grain cleaning
        <br />
        &emsp;c. Feed mixing
        <br />
        &emsp;d. Alfalfa and hay chopping
        <br />
        &emsp;e. Tobacco Barns
        <br />
        3. Hoop buildings. (Type 2 rating is appropriate if the hoop building is built with concrete knee walls)
    </div>

    <div style="text-align: center; margin-left: auto; margin-right: auto; margin-top: 20px; margin-bottom: 20px;">
        <asp:Button ID="btnBldTypeOK" CssClass="StandardButton" runat="server" Text="OK" />
    </div>
</div>