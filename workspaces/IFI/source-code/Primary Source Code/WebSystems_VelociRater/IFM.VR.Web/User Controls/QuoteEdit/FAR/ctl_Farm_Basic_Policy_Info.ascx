<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctl_Farm_Basic_Policy_Info.ascx.vb" Inherits="IFM.VR.Web.ctl_Farm_Basic_Policy_Info" %>


<script type="text/javascript">
    var _hobbyId = "100";
    $(document).ready(function () {

        $("#<%= rdoPolicyType.ClientID%>").change(function () { ShowHideCommercialLiability(); });
        $("#<%=Me.radioListCommOrPersonal.ClientId%>").find("input").click(function () { ShowHideCommercialLiability(); });

        $("#<%=chkIsHobbyFarm.ClientId%>").click(function () {
            if ($(this).prop('checked')) {
                $("#<%=Me.radioListCommOrPersonal.ClientId%>").find("input[value=1]").prop("checked", true);
                $("#<%=Me.radioListCommOrPersonal.ClientId%>").find("input").attr('disabled', 'disabled');

                $("#<%=rdoPolicyType.ClientID%>").find("input").prop("checked", false);
                $("#<%=rdoPolicyType.ClientID%>").find("input").prop("disabled", true);

                if (!isFarmCopy) {
                    $("#<%=Me.radioListFarmActivity.ClientID%>").find("input").prop("checked", false);
                }
                //$("#<%=Me.radioListFarmActivity.ClientID%>").find("input[value=" + _hobbyId + "]").prop("checked", true);
                $("#<%=Me.radioListFarmActivity.ClientID%>").find("input").attr('disabled', 'disabled');
            }
            else {
                $("#<%=Me.radioListCommOrPersonal.ClientId%>").find("input").removeAttr('disabled');
                $("#<%=Me.radioListFarmActivity.ClientID%>").find("input").removeAttr('disabled');
                $("#<%=rdoPolicyType.ClientID%>").find("input").removeAttr("disabled");
                $("#<%=Me.radioListFarmActivity.ClientID%>").find("input[value=" + _hobbyId + "]").attr('disabled', 'disabled');
                $("#<%=Me.radioListFarmActivity.ClientID%>").find("input[value=" + _hobbyId + "]").prop("checked", false);
            }
            if (isFarmCopy) {
                $("#<%=Me.radioListCommOrPersonal.ClientId%>").find("input[value=1]").prop("checked", true);
                $("#<%=Me.radioListCommOrPersonal.ClientId%>").find("input").attr('disabled', 'disabled');

                $("#<%=Me.rdoPolicyType.ClientId%>").find("input[value=7]").prop("checked", false).attr('disabled', 'disabled');
                $("#<%=Me.rdoPolicyType.ClientId%>").find("input[value=8]").prop("checked", false).attr('disabled', 'disabled');
            }
            ShowHideCommercialLiability();
        });

        if ($("#<%=chkIsHobbyFarm.ClientId%>").prop('checked')) {
            $("#<%=Me.radioListCommOrPersonal.ClientId%>").find("input[value=1]").prop("checked", true);
            $("#<%=Me.radioListCommOrPersonal.ClientId%>").find("input").attr('disabled', 'disabled');

            $("#<%=rdoPolicyType.ClientID%>").find("input[value=1]").prop("checked", true);
            $("#<%=rdoPolicyType.ClientId%>").find("input").attr('disabled', 'disabled');

            $("#<%=Me.radioListFarmActivity.ClientID%>").find("input[value=" + _hobbyId + "]").prop("checked", true);
            $("#<%=Me.radioListFarmActivity.ClientID%>").find("input").attr('disabled', 'disabled');
        }
        else {
            $("#<%=Me.radioListCommOrPersonal.ClientId%>").find("input").removeAttr('disabled');
            $("#<%=Me.radioListFarmActivity.ClientID%>").find("input").removeAttr('disabled');
            $("#<%=Me.radioListFarmActivity.ClientID%>").find("input[value=" + _hobbyId + "]").attr('disabled', 'disabled');
            $("#<%=Me.radioListFarmActivity.ClientID%>").find("input[value=" + _hobbyId + "]").prop("checked", false);
        }
        if (isFarmCopy) {
            $("#<%=Me.radioListCommOrPersonal.ClientId%>").find("input[value=1]").prop("checked", true);
            $("#<%=Me.radioListCommOrPersonal.ClientId%>").find("input").attr('disabled', 'disabled');

            $("#<%=Me.rdoPolicyType.ClientId%>").find("input[value=7]").prop("checked", false).attr('disabled', 'disabled');
            $("#<%=Me.rdoPolicyType.ClientId%>").find("input[value=8]").prop("checked", false).attr('disabled', 'disabled');
        }
        

        
        ShowHideCommercialLiability();        
        
    });

    function ShowHideCommercialLiability()
    {
        if($("#<%=Me.radioListCommOrPersonal.ClientId%>").find("input[value=1]").prop("checked") & $("#<%=rdoPolicyType.ClientID%>").val() == "8")
        {
            $("#div_CommercialLiability").show();
        }
        else
        {
            $("#div_CommercialLiability").hide();
            $("#<%=Me.chkCommercialLiability%>").prop("checked",false);
        }
    }

    function FarmBasicInfoComplete() {
        var nameTypeSelected = $("#<%=me.radioListCommOrPersonal.ClientId%>").find("input:checked").length == 1;
        var policyTypeSelected = $("#<%=rdoPolicyType.ClientID%>").find("input:checked").length == 1;
        var farmActivitySelected = $("#<%=Me.radioListFarmActivity.ClientID%>").find("input:checked").length == 1;
        if (!(nameTypeSelected & (policyTypeSelected | $("#<%=chkIsHobbyFarm.ClientId%>").prop("checked")) & (farmActivitySelected | $("#<%=chkIsHobbyFarm.ClientId%>").prop("checked")))) {
            if (nameTypeSelected == false)
                alert('Selection of Personal or Commercial is required');
            if ($("#<%=chkIsHobbyFarm.ClientId%>").prop("checked") == false && policyTypeSelected == false)
                alert('Selection of Policy Type is required');
            if ($("#<%=chkIsHobbyFarm.ClientId%>").prop("checked") == false && farmActivitySelected == false)
                alert('Selection of Principal Farming Activity is required');

            //alert('Form is incomplete.');
            return false;
        }
        else {
            // server will not accept values from disabled radio buttons so enable them before post back
            $("#<%=me.radioListCommOrPersonal.ClientId%>").find("input").removeAttr('disabled');
            $("#<%=rdoPolicyType.ClientID%>").find("input").removeAttr('disabled');
            $("#<%=Me.radioListFarmActivity.ClientID%>").find("input").removeAttr('disabled');
            // disable the form with an overlay to prevent changes during post back
            EditModeaDiv('divBasicInfoPopup', true);
            return true;
        }
    }


</script>

<style>
    #<%=me.radioListFarmActivity.clientid%> td {
        width: 125px;
    }
</style>

<div id="divBasicInfoPopup">
    <div>
        <div id="divHobbyFarm" runat="server" style="margin-bottom: 15px;">
            <asp:CheckBox ID="chkIsHobbyFarm" Text="" runat="server" />
            <a id="lnkHobbyFarmPopup" href="#">Hobby Farm?</a>
        </div>

        <div style="margin-bottom: 15px;">
            Liability Type:
        <asp:RadioButtonList ID="radioListCommOrPersonal" runat="server">
            <asp:ListItem Text="Personal" Value="1"></asp:ListItem>
            <asp:ListItem Text="Commercial" Value="2"></asp:ListItem>
        </asp:RadioButtonList>
        </div>

        <%--<div>--%>
        <table style="width:100%">
            <tr>
                <td>
                    Policy Type:
                </td>
            </tr>
            <tr>
                <td style="width:150px">
            <%--<br />--%>
                    <asp:RadioButtonList ID="rdoPolicyType" runat="server">
                        <asp:ListItem Value="6" Text="Farmowners"></asp:ListItem>
                        <asp:ListItem Value="7" Text="Select-O-Matic"></asp:ListItem>
                        <asp:ListItem Value="8" Text="Farm Liability"></asp:ListItem>
                    </asp:RadioButtonList>
                    <%--<asp:DropDownList Style="margin-left: 8px;" ID="ddPolicyType" runat="server"></asp:DropDownList>--%>
                </td>
                <td style="vertical-align:top">
                    <a id='lnkPopupPolicyType'href="#" title="Policy Type help">Help</a>
                </td>
            </tr>
        </table>
        <%--</div>--%>

        <div id="div_CommercialLiability" style="margin-top:10px;">            
            <asp:CheckBox ID="chkCommercialLiability" runat="server" Text="Commercial Liability?" />
        </div>

        <div style="margin-top: 15px; padding: 5px; border: 1px solid black; width: 225px;" >
            Principal Farming Activity:
        <br />
            <asp:RadioButtonList ID="radioListFarmActivity" runat="server" >
                <asp:ListItem Text="Dairy" value="1"></asp:ListItem>
                <asp:ListItem Text="Field Crops" Value="2"></asp:ListItem>
                <asp:ListItem Text="Fruit" Value="3"></asp:ListItem>
                <asp:ListItem Text="Greenhouses" Value="4"></asp:ListItem>
                <asp:ListItem Text="Horses" Value="5"></asp:ListItem>
                <asp:ListItem Text="Livestock" Value="6"></asp:ListItem>
                <asp:ListItem Text="Poultry" Value="7"></asp:ListItem>
                <asp:ListItem Text="Swine" Value="8"></asp:ListItem>
                <asp:ListItem Text="Vegetables" Value="9"></asp:ListItem>
                <%--<asp:ListItem Text="Hobby" Value="100"></asp:ListItem>--%>
            </asp:RadioButtonList>
        </div>
    </div>

    <div style="text-align: center; margin-left: auto; margin-right: auto; margin-top: 20px; margin-bottom: 20px;">        
        <asp:Button ID="btnCancel"  CssClass="StandardButton" runat="server" Text="Cancel" />
        <asp:Button ID="btnContinue" CssClass="StandardButton" OnClientClick="javascript:return FarmBasicInfoComplete();" runat="server" Text="Continue to quote" />
    </div>
</div>

<div id="dibHobbyFarmInfoPopup">
    <p>This is a special program designed for those clients who have a small farm, but farming operations are not the primary source of their income. Hobby Farms not eligible for IRPM credit or debit.
    </p>
    <p>
<b>Eligible: </b><br />
1.	The Named Insured must be an individual(s). <br />
2.	The dwelling must be owner-occupied as the insured's principal residence and used exclusively for private residential purposes. <br />
3.	The dwelling must meet the eligibility requirements for a Type I classification and be insured for 100% of replacement cost. <br />
<span style="margin-left:30px;">o	Mobile/Manufactured homes on permanent foundations which are ten years or newer and classed Type II per company guidelines should be submitted to UW for review of acceptability for hobby farm.</span> <br />
4.	Written as a <b>Farmowners</b> policy (must comply with all applicable underwriting requirements.)<br />
    </p>
    <p>
<b>Ineligible: </b><br />
1.	Any risk that is eligible to be written on an Indiana Farmers <b>Homeowners</b> policy. <br />
2.	Any risk with two or more losses in the past three years. <br />
3.	Any risk with property (whether insured or not) on more than one location. <br />
4.	Any risk having more than 5 horses (these must be for pleasure use only - boarding of horses is not acceptable under the hobby farm). <br />
5.	Mobile/manufactured homes older than 10 years or not on a permanent foundation. <br />
6.	Premises in excess of 150 acres. <br />
7.	Gross receipts exceeding $25,000 annually. <br />
8.	Outbuildings valued in excess of $50,000 (per building). <br />
9.	Blanket or scheduled equipment that exceeds $50,000. <br />
    </p>
</div>

<div id="divFarmFormTypeHelp">
    <p><b>Farmowners </b>- Owner occupied farm with a Coverage A (owner occupied) dwelling.  May be commercial or personal liability.  May be in a personal name or a commercial name.  May include farm personal property as well as farm buildings and all available endorsements offered by the company. </p>

    <p><b>Select O Matic </b>- Farm policy with no Coverage A dwelling (no owner occupied dwelling).  Will be written with commercial (premises only) liability.  May be personal name or commercial name.  May include rental dwellings, outbuildings, farm personal property and most available endorsement offered by the company.  Must have support with Indiana Farmers such as auto, commercial, or personal insurance. </p>

    <p><b>Farm Liability </b>- Written on commercial form (premises liability only).  Must be farm ground only (owner or tenant farmed), no dwellings or outbuildings, no structures.  No commercial exposure.  No hunting or ATV riding.  No recreational use of the land.  Must have support with Indiana Farmers such as auto, commercial, or personal insurance.  </p>

</div>

<div id="divHobbyFarmHelp">
    <p>
        Please refer to Hobby Farm guidelines.  Any policy bound as a Hobby Farm that does not qualify will be moved to Farmowners prior to issue and this will increase the rating.<asp:HiddenField ID="hddnDivFarmGuidelines" runat="server" />
    </p>
    <p class="informationalTextRed txtCnt">Underwriting approval is required for FO-4 for Hobby Farm policies. Please contact your Underwriter for review.</p>
</div>
