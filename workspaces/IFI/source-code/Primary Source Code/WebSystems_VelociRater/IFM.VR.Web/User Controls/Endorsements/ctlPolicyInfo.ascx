<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctlPolicyInfo.ascx.vb" Inherits="IFM.VR.Web.ctlPolicyInfo" %>
<style>
    input {
        width: 200px;
    }
    .inputArea {
    width: 300px;
}

    textarea {
        width: 300px;
    }

    .divNames li, .divEndorsementInfo li {
        margin-top: 5px;
    }

    .insuredDetail {
        vertical-align: top; 
        text-align: left; 
        width: 235px; 
        float: right;
        
    }

    .insuredDetail li {
        margin-top: 5px;
    }
</style>
<div runat="server" id="divPolicyInfo">
    <h3>
        <asp:Label ID="lblInsuredTitle" runat="server" Text="Label"></asp:Label>
        <span style="float: right;">
            <%--<asp:LinkButton ID="lnkRemove" ToolTip="Clear this policyholder" CssClass="RemovePanelLink" runat="server">Clear</asp:LinkButton>--%>
            <%--<asp:LinkButton ID="lnkSave" CssClass="RemovePanelLink" runat="server">Save</asp:LinkButton>--%>
        </span>
    </h3>

    <div runat="server" id="divInsuredInfo">
        <div class="insured" style="float: left; vertical-align: top; text-align: left;">
            <div runat="server" id="divNames" class="divNames">
                <ul>
                    <li>
                        <asp:Label ID="lblPH1Name" runat="server">Policyholder 1 Name:</asp:Label><br />
                        <asp:TextBox ID="txtPH1Name" runat="server" TabIndex="-1" MaxLength="50"></asp:TextBox>
                    </li>
                    <li>
                        <asp:Label ID="lblPH2Name" runat="server">Policyholder 2 Name: </asp:Label><br />
                        <asp:TextBox ID="txtPh2Name" runat="server" TabIndex="-1" MaxLength="50"></asp:TextBox>
                    </li>
                </ul>
            </div>
            <div runat="server" id="divEndorsementInfo" class="divEndorsementInfo">
                <ul>
                    <li class="inputArea">
                        <label for="<%=Me.txtTransEffectDate.ClientID%>" class="informationalTextRed">*Transaction Effective Date:</label><br />
                        <asp:TextBox ID="txtTransEffectDate" runat="server" TabIndex="1" MaxLength="50" class="informationTextYellowBackground"></asp:TextBox>
                        <asp:Label ID="lblTransEffDateRenewal" runat="server" Visible="False" CssClass="informationalText"><br />Payment plan change must be for the renewal term.</asp:Label>
                    </li>
                    <li id="typeOfTransaction" runat="server">
                        <label for="<%=Me.ddlTypeOfEndorsement.ClientID%>" class="informationalTextRed">*Type of Endorsement</label><br />
                        <asp:DropDownList ID="ddlTypeOfEndorsement" runat="server" TabIndex="2" class="informationTextYellowBackground w300">
                        </asp:DropDownList>
                    </li>
                    <li id="remarksArea" runat="server">
                        <label for="<%=Me.txtRemarks.ClientID%>" class="informationalTextRed">*Remarks</label><br />
                        <asp:TextBox ID="txtRemarks" runat="server" TabIndex="2" MaxLength="255" TextMode="multiline" Rows="5" cols="30" class="informationTextYellowBackground"></asp:TextBox>
                    </li>
                </ul>
            </div>
            <div id="divRemarksRules">
                <div id="divRemarksPersonal" runat="server">
                    <span>Remarks Help:</span><br />
                    The first seven characters must:
                    <ul>
                        <li>be seven alphanumeric characters and spaces</li>
                        <li>Not be the same letter or number</li>
                        <li>Not have special characters</li>
                    </ul>              
                    The entire remark must:
                     <ul>
                        <li>be between 7 and 255 characters long</li>
                    </ul>
                </div>
                <div id="divRemarksCommercial" runat="server"> 
                    <asp:Label ID="lblAuthority" runat="server"></asp:Label>
                    <br />
                    <ol runat="server" id="ListOfChanges"></ol>
                    <asp:Label ID="lblLobLimitText" runat="server"></asp:Label>
                    <br />
                    <br />
                    <asp:Label ID="lblOtherChangeText" runat="server"></asp:Label>
                </div>
            </div>
        </div>

        <div runat="server" id="rightColumn" class="insured insuredDetail">
            <ul>
                <li>
                    <label for="<%=Me.txtPolicyTerm.ClientID%>">Policy Term:</label><br />
                    <asp:TextBox ID="txtPolicyTerm" runat="server" TabIndex="-1"></asp:TextBox></li>

                <li>
                    <label for="<%=Me.txtStreetNum.ClientID%>">Street #:</label><br />
                    <asp:TextBox ID="txtStreetNum" runat="server" TabIndex="-1"></asp:TextBox></li>

                <li>
                    <label for="<%=Me.txtStreetName.ClientID%>">Street Name:</label><br />
                    <asp:TextBox ID="txtStreetName" runat="server" TabIndex="-1"></asp:TextBox></li>

                <li>
                    <label for="<%=Me.txtAptNum.ClientID%>">Apt./Suite #:</label><br />
                    <asp:TextBox ID="txtAptNum" runat="server" TabIndex="-1"></asp:TextBox></li>

                <li>
                    <label for="<%=Me.txtPOBox.ClientID%>">P.O. Box:</label><br />
                    <asp:TextBox ID="txtPOBox" runat="server" TabIndex="-1"></asp:TextBox></li>

                <li>
                    <label for="<%=Me.txtZipCode.ClientID%>">ZIP:</label><br />
                    <asp:TextBox ID="txtZipCode" runat="server" TabIndex="-1"></asp:TextBox></li>

                <li>
                    <label for="<%=Me.txtCityName.ClientID%>">City:</label><br />
                    <asp:TextBox ID="txtCityName" runat="server" TabIndex="-1"></asp:TextBox>
                </li>

                <li>
                    <label for="<%=Me.txtStateAbbrev.ClientID%>">State:</label><br />
                    <asp:TextBox ID="txtStateAbbrev" runat="server" TabIndex="-1"></asp:TextBox></li>

                <li>
                    <label for="<%=Me.txtGaragedCounty.ClientID%>">County:</label><br />
                    <asp:TextBox ID="txtGaragedCounty" runat="server" TabIndex="-1"></asp:TextBox></li>
            </ul>


        </div>

        
    </div>
</div>
<br />
<div id="divMaxTransactionsMessage" runat="server" style="color:red;">Only two transactions can be processed in one business day, 
    please contact your underwriter for more than two transactions per business day.</div>
<br />
<div id="divActionButtons" runat="server" class="standardSubSection center" style="width: 100%; text-align: center;">
    <asp:Button ID="btnSubmit" runat="server" OnClientClick="DisableFormOnSaveRemoves();" CssClass="StandardSaveButton" ToolTip="Begins new endorsement." Text="Next" />
    <asp:Button ID="btnCancel" CssClass="StandardSaveButton" runat="server" Text="Cancel" /><br />
</div>
    <asp:HiddenField ID="hdnAccordGenInfo" runat="server" />
<%--<script src="js/vr.core.js"></script>--%>
<script>
        var List = ['divNames', 'rightColumn'];
    ifm.vr.ui.DisableContent(List);
</script>

