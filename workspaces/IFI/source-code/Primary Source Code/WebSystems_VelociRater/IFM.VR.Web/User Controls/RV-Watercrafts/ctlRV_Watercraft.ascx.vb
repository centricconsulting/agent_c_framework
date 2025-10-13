Imports QuickQuote.CommonObjects
Imports QuickQuote.CommonMethods
Imports IFM.VR.Web.Helpers
Imports IFM.VR.Common.Helpers
Imports IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM
Imports IFM.PrimativeExtensions

Public Class ctlRV_Watercraft
    Inherits VRControlBase

    Private _chc As New CommonHelperClass

    Public Event CommonRefreshRVWater()
    Public Event CommonRaiseRVWSave()
    Public Event CommonRaiseRVWRate()
    Public Event CommonSetActivePanel(activePanel As String)
    Public Event NewVehicleRequested()
    Public Event NewBoatMotorRequested()

    Public ReadOnly Property MyLocation As QuickQuote.CommonObjects.QuickQuoteLocation
        Get
            If Me.Quote IsNot Nothing AndAlso Me.Quote.Locations IsNot Nothing AndAlso Me.Quote.Locations.Any() Then
                Return Quote.Locations(RVWatercraftLocationNumber)
            End If
            Return Nothing
        End Get
    End Property

    Public Property MyRVWaterIndex As Int32
        Get
            If ViewState("vs_RVWaterIndex") IsNot Nothing Then
                Return CInt(ViewState("vs_RVWaterIndex"))
            End If
            Return 0
        End Get
        Set(value As Int32)
            ViewState("vs_RVWaterIndex") = value
        End Set
    End Property

    Protected Overrides ReadOnly Property MyAccordionIndex As Integer
        Get
            Return MyRVWaterIndex
        End Get
    End Property

    Public ReadOnly Property MyVehicle As QuickQuote.CommonObjects.QuickQuoteRvWatercraft
        Get
            If MyLocation IsNot Nothing Then
                If MyLocation.RvWatercrafts IsNot Nothing Then
                    If MyLocation.RvWatercrafts.Count > RVWatercraftNumber Then
                        Return MyLocation.RvWatercrafts(RVWatercraftNumber)
                    End If
                End If
            End If
            Return Nothing
        End Get
    End Property

    Public ReadOnly Property MyVehicleMotor As QuickQuote.CommonObjects.QuickQuoteRvWatercraftMotor
        Get
            If MyLocation IsNot Nothing Then
                If MyLocation.RvWatercrafts IsNot Nothing Then
                    If MyLocation.RvWatercrafts.Count > Me.RVWatercraftNumber Then
                        Return MyLocation.RvWatercrafts(Me.RVWatercraftNumber).RvWatercraftMotors(0)
                    End If
                End If
            End If
            Return Nothing
        End Get
    End Property

    Public Property RVWatercraftNumber As Int32
        Get
            If ViewState("RVWatercraftNumber") Is Nothing Then
                ViewState("RVWatercraftNumber") = 0
            End If
            Return CInt(ViewState("RVWatercraftNumber"))
        End Get
        Set(value As Int32)
            ViewState("RVWatercraftNumber") = value

            Dim scriptHeaderUpdate As String = "updateRVWatercraftHeaderText(""" + lblCommonRVWatercraftHdr.ClientID + """," + value.ToString() + ",""" + txtVehMake.ClientID + """,""" + txtVehModel.ClientID + """,""" + txtVehYear.ClientID + """); "
            Me.txtVehMake.Attributes.Add("onkeyup", scriptHeaderUpdate)
            Me.txtVehModel.Attributes.Add("onkeyup", scriptHeaderUpdate)
            Me.txtVehYear.Attributes.Add("onkeyup", scriptHeaderUpdate)
        End Set
    End Property
    Public Property RVWatercraftLocationNumber As Int32
        Get
            If ViewState("RVWatercraftLocationNumber") Is Nothing Then
                ViewState("RVWatercraftLocationNumber") = 0
            End If
            Return CInt(ViewState("RVWatercraftLocationNumber"))
        End Get
        Set(value As Int32)
            ViewState("RVWatercraftLocationNumber") = value
        End Set
    End Property

    Public Property SelectedCoverageOption As String
        Get
            Return hiddenSelectedCoverage.Value
        End Get
        Set(ByVal value As String)
            hiddenSelectedCoverage.Value = value
        End Set
    End Property

    Public ReadOnly Property CurrentForm As String
        Get
            Try
                'Updated 11/28/17 for HOM Upgrade MLW
                'Return QQHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteLocation, QuickQuoteHelperClass.QuickQuotePropertyName.FormTypeId, Quote.Locations(0).FormTypeId).Substring(0, 4)
                Return QQHelper.GetShortFormName(Quote)
            Catch ex As Exception
                Return ""
            End Try
        End Get
    End Property
    Public ReadOnly Property YouthfulChecked As Boolean 'added 3/17/2020
        Get
            Return Me.chkUnder25Operator.Checked
        End Get
    End Property

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        divContents.Visible = Not HideFromParent

        Me.pnlRvWaterButtons.Visible = Not IsOnAppPage ' Matt A - 8/3/15
        If Not IsPostBack Then
            LoadStaticData()
            RaiseEvent CommonSetActivePanel("false")
            Populate()
        End If
        If IsOnAppPage Or Me.IsQuoteEndorsement Then ' Matt A - 8/4/15
            Me.lblVehSerialNum.Text = "*Serial/Hull Number"
            Me.lblVehMake.Text = "*Manufacturer"
            Me.lblVehModel.Text = "*Model"
        End If

    End Sub

    Public Overrides Sub AddScriptAlways()

    End Sub

    Public Overrides Sub AddScriptWhenRendered()
        Me.VRScript.CreateConfirmDialog(lnkRemove.ClientID, "Remove Item?")
        Me.VRScript.StopEventPropagation(lnkSave.ClientID)
        Me.VRScript.StopEventPropagation(lnkAdd.ClientID)
        Me.VRScript.StopEventPropagation(lnkSaveMotor.ClientID)
        VRScript.CreateConfirmDialog(lnkClearMotor.ClientID, "Clear Motor Information ?")
        'Me.VRScript.StopEventPropagation(lnkClearMotor.ClientID)


        VRScript.AddScriptLine("$(""#" + Me.divMotorInput.ClientID + """).accordion({heightStyle: ""content"", active: " + Me.hiddenMotorInfo.Value + ", collapsible: true, activate: function(event, ui) { ToggleHiddenField('" + Me.hiddenMotorInfo.ClientID + "');   } });")

        Dim scriptHeaderUpdate As String = "updateRVWatercraftHeaderText(""" + lblCommonRVWatercraftHdr.ClientID + """," + Me.RVWatercraftNumber.ToString() + ",""" + Me.txtVehMake.ClientID + """,""" + Me.txtVehModel.ClientID + """,""" + Me.txtVehYear.ClientID + """); "
        Me.txtVehMake.Attributes.Add("onkeyup", scriptHeaderUpdate)
        Me.txtVehModel.Attributes.Add("onkeyup", scriptHeaderUpdate)
        Me.txtVehYear.Attributes.Add("onkeyup", "$(this).val(FormatAsPositiveNumberNoCommaFormatting($(this).val()));" + scriptHeaderUpdate)

        Dim scriptUnder25Checked As String = "toggleRVWaterUnder25Message(""" + chkUnder25Operator.ClientID + """, """ + lblUnder25OperMsg.ClientID + """);"
        chkUnder25Operator.Attributes.Add("onclick", scriptUnder25Checked)
        'If Me.IsOnAppPage = False Then ' Matt A - 8/4/15
        '    Dim scriptUnder25Checked As String = "toggleRVWaterUnder25Message(""" + chkUnder25Operator.ClientID + """, """ + lblUnder25OperMsg.ClientID + """);"
        '    chkUnder25Operator.Attributes.Add("onclick", scriptUnder25Checked)
        'Else
        '    lblUnder25OperMsg.Visible = False
        'End If

        Dim scriptToggleType As String = "toggleRVWatercraftType(""" + ddlVehType.ClientID + """, """ + pnlCoverageOptions.ClientID + """, """ + pnlVehYear.ClientID + """, """ +
            pnlVehLength.ClientID + """, """ + pnlVehCostNew.ClientID + """, """ + pnlMotorType.ClientID + """, """ + ddlMotorType.ClientID + """, """ + lblMotorTypeInOut.ClientID + """, """ +
            pnlPropertyDeductible.ClientID + """, """ + pnlRVWLiabilityOnly.ClientID + """, """ + pnlVehSerialNum.ClientID + """, """ + pnlVehMake.ClientID + """, """ + pnlVehModel.ClientID + """, """ +
            pnlDescription.ClientID + """, """ + divMotorInput.ClientID + """, """ + pnlHorsepowerCCs.ClientID + """, """ + pnlMotorYear.ClientID + """, """ + pnlMotorCostNew.ClientID + """, """ +
            pnlMotorSerialNum.ClientID + """, """ + pnlMotorMake.ClientID + """, """ + pnlMotorModel.ClientID + """, """ + pnlBodilyInjuryLimit.ClientID + """, """ + pnlUnder25Operator.ClientID + """, """ +
            ddlCoverageOptions.ClientID + """, """ + lblCoverageOptionPD.ClientID + """, """ + lblCoverageOptionPDL.ClientID + """, """ + lblCoverageOptionLib.ClientID + """, """ +
            lblMotorTypeIn.ClientID + """, """ + lblMotorTypeOut.ClientID + """, """ + ddlPropertyDeductible.ClientID + """, """ + txtVehYear.ClientID + """, """ + txtVehLength.ClientID + """, """ +
            txtVehCostNew.ClientID + """, """ + ddlBodilyInjuryLimit.ClientID + """, """ + chkUnder25Operator.ClientID + """, """ + txtVehSerialNum.ClientID + """, """ + txtVehMake.ClientID + """, """ +
            txtVehModel.ClientID + """, """ + txtHorsepowerCCs.ClientID + """, """ + txtMotorYear.ClientID + """, """ + txtMotorCostNew.ClientID + """, """ + txtMotorSerialNum.ClientID + """, """ +
            txtMotorMake.ClientID + """, """ + txtMotorModel.ClientID + """, """ + lblUnder25OperMsg.ClientID + """, """ + lblVehCostNew.ClientID + """, """ + lblLimit.ClientID + """, """ +
            hiddenSelectedCoverage.ClientID + """, """ + hiddenSelectedForm.ClientID + """, """ + txtDescription.ClientID + """, """ + lblHorsepowerCCs.ClientID + """, """ + hiddenWatercraftType.ClientID + """, """ + divVehTypeoWatercraftText.ClientID +
            """);"
        Me.VRScript.CreateJSBinding(ddlVehType.ClientID, ctlPageStartupScript.JsEventType.onchange, scriptToggleType) ' Matt A 11-07-2016
        'ddlVehType.Attributes.Add("onchange", scriptToggleType)
        'VRScript.AddScriptLine(scriptToggleType, onlyAllowOnce:=True)

        Dim scriptToggleCoverage As String = "toggleRVWaterCoverageOptions(""" + ddlCoverageOptions.ClientID + """, """ + pnlPropertyDeductible.ClientID + """, """ + pnlVehCostNew.ClientID + """, 
            """ + ddlPropertyDeductible.ClientID + """, """ + txtVehCostNew.ClientID + """, """ + hiddenSelectedCoverage.ClientID + """, """ + ddlVehType.ClientID + """);"
        Me.VRScript.CreateJSBinding(ddlCoverageOptions.ClientID, ctlPageStartupScript.JsEventType.onchange, scriptToggleCoverage) ' Matt A 11-07-2016
        'ddlCoverageOptions.Attributes.Add("onchange", scriptToggleCoverage)
        'VRScript.AddScriptLine(scriptToggleCoverage, onlyAllowOnce:=True)


        If QQHelper.doUseNewVersionOfLOB(Quote, QuickQuoteHelperClass.LOBIFMVersions.HOM2018Upgrade, "7/1/2018") Then
            Dim scriptChkOwnerOtherThanInsured As String = "checkOwnedBySomeoneOtherThanInsured('" & chkOwnerOtherThanInsured.ClientID & "');"
            Me.VRScript.CreateJSBinding(chkOwnerOtherThanInsured.ClientID, ctlPageStartupScript.JsEventType.onchange, scriptChkOwnerOtherThanInsured)
            'VRScript.AddScriptLine(scriptChkOwnerOtherThanInsured, onlyAllowOnce:=True)

            Dim scriptChkCollisionCoverage As String = "checkCollisionCoverage('" & chkCollisionCoverage.ClientID & "');"
            Me.VRScript.CreateJSBinding(chkCollisionCoverage.ClientID, ctlPageStartupScript.JsEventType.onchange, scriptChkCollisionCoverage)
            'VRScript.AddScriptLine(scriptChkCollisionCoverage, onlyAllowOnce:=True)
        End If

        Dim scriptToggleMotor As String = "toggleRVWaterMotorType(""" + ddlMotorType.ClientID + """, """ + divMotorInput.ClientID + """, """ + pnlHorsepowerCCs.ClientID + """, """ +
            pnlMotorYear.ClientID + """, """ + pnlMotorCostNew.ClientID + """, """ + pnlMotorSerialNum.ClientID + """, """ + pnlMotorMake.ClientID + """, """ + pnlMotorModel.ClientID + """, """ +
            txtHorsepowerCCs.ClientID + """, """ + txtMotorYear.ClientID + """, """ + txtMotorCostNew.ClientID + """, """ + txtMotorSerialNum.ClientID + """, """ + txtMotorMake.ClientID + """, """ +
            txtMotorModel.ClientID + """, """ + hiddenSelectedForm.ClientID + """, """ + lblHorsepowerCCs.ClientID + """, """ + hiddenMotorType.ClientID + """, """ + HiddenMotorButton.ClientID + """);"
        Me.VRScript.CreateJSBinding(ddlMotorType.ClientID, ctlPageStartupScript.JsEventType.onchange, scriptToggleMotor) ' Matt A 11-07-2016
        'ddlMotorType.Attributes.Add("onchange", scriptToggleMotor)

        Me.VRScript.CreateJSBinding(txtVehCostNew.ClientID, ctlPageStartupScript.JsEventType.onblur, "$('#{0}').val(ifm.vr.stringFormating.asNumberWithCommas(ifm.vr.numbers.RoundUpBy100But0isBlank($('#{0}').val())));".FormatIFM(txtVehCostNew.ClientID), True) ' Matt A 11-07-2016

        Me.VRScript.CreateJSBinding(txtMotorCostNew.ClientID, ctlPageStartupScript.JsEventType.onblur, "$('#{0}').val(ifm.vr.stringFormating.asNumberWithCommas(ifm.vr.numbers.RoundUpBy100But0isBlank($('#{0}').val())));".FormatIFM(txtMotorCostNew.ClientID), True) ' Matt A 11-07-2016

        'Dim scriptClearMotor As String = "ClearRVWaterMotorEntries(""" + txtHorsepowerCCs.ClientID + """, """ + txtMotorYear.ClientID + """, """ + txtMotorCostNew.ClientID + """, """ +
        '    txtMotorSerialNum.ClientID + """, """ + txtMotorMake.ClientID + """, """ + txtMotorModel.ClientID + """); return false;"
        'lnkClearMotor.Attributes.Add("onclick", scriptClearMotor)

        'Added 04/14/2020 for Home Endorsements Bug 45722 MLW
        Dim scriptDescCount As String = "CountDescLength(""" + txtDescription.ClientID + """, """ + lblMaxCharCount.ClientID + """, """ + hiddenMaxCharCount.ClientID + """);"
        VRScript.CreateJSBinding(Me.txtDescription.ClientID, ctlPageStartupScript.JsEventType.onkeyup, scriptDescCount, True)


        hiddenSelectedForm.Value = CurrentForm
        txtDescription.Attributes.Add("onfocus", "this.select()")
        txtVehYear.Attributes.Add("onfocus", "this.select()")
        txtVehLength.Attributes.Add("onfocus", "this.select()")
        txtVehCostNew.Attributes.Add("onfocus", "this.select()")
        txtVehSerialNum.Attributes.Add("onfocus", "this.select()")
        txtVehMake.Attributes.Add("onfocus", "this.select()")
        txtVehModel.Attributes.Add("onfocus", "this.select()")
        txtHorsepowerCCs.Attributes.Add("onfocus", "this.select()")
        txtMotorYear.Attributes.Add("onfocus", "this.select()")
        txtMotorCostNew.Attributes.Add("onfocus", "this.select()")
        txtMotorSerialNum.Attributes.Add("onfocus", "this.select()")
        txtMotorMake.Attributes.Add("onfocus", "this.select()")
        txtMotorModel.Attributes.Add("onfocus", "this.select()")

        VRScript.AddVariableLine("var isRvWatercraftMotorAvailable = " & RvWaterCraftHelper.IsRvWaterCraftMotorAvailable(Quote).ToString().ToLower & ";")
    End Sub

    Public Overrides Sub LoadStaticData()
        QQHelper.LoadStaticDataOptionsDropDown(Me.ddlVehType, QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteRvWatercraft, QuickQuoteHelperClass.QuickQuotePropertyName.RvWatercraftTypeId, QuickQuoteStaticDataOption.SortBy.None, Me.Quote.LobType)
        QQHelper.LoadStaticDataOptionsDropDown(Me.ddlMotorType, QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteRvWatercraftMotor, QuickQuoteHelperClass.QuickQuotePropertyName.MotorTypeId, QuickQuoteStaticDataOption.SortBy.None, Me.Quote.LobType)
        QQHelper.LoadStaticDataOptionsDropDown(Me.ddlPropertyDeductible, QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteRvWatercraft, QuickQuoteHelperClass.QuickQuotePropertyName.PropertyDeductibleLimitId, QuickQuoteStaticDataOption.SortBy.None, Me.Quote.LobType)
        QQHelper.LoadStaticDataOptionsDropDown(Me.ddlBodilyInjuryLimit, QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteRvWatercraft, QuickQuoteHelperClass.QuickQuotePropertyName.UninsuredMotoristBodilyInjuryLimitId, QuickQuoteStaticDataOption.SortBy.None, Me.Quote.LobType)

        ' Populate Coverage Options
        ddlCoverageOptions.Items.Clear()
        ddlCoverageOptions.Items.Insert(0, "")
        ddlCoverageOptions.Items.Insert(1, "PHYSICAL DAMAGE AND LIABILITY")
        ddlCoverageOptions.Items.Insert(2, "PHYSICAL DAMAGE ONLY")
        ddlCoverageOptions.Items.Insert(3, "LIABILITY ONLY")
    End Sub

    Public Overrides Sub Populate()
        Dim qqhelper As New QuickQuoteHelperClass
        hiddenLOB.Value = Quote.LobType
        hiddenOccupancyCodeID.Value = MyLocation.OccupancyCodeId

        'Added 03/30/2020 for home endorsement task 38919 MLW
        If Me.IsQuoteEndorsement Then
            hiddenIsEndorsementQuote.Value = "true"
        Else
            hiddenIsEndorsementQuote.Value = "false"
        End If

        If IsOnAppPage OrElse IsQuoteEndorsement() OrElse IsQuoteReadOnly() Then
            lblMotorSerialNum.CssClass = "showAsRequired"
            lblMotorMake.CssClass = "showAsRequired"
            lblMotorModel.CssClass = "showAsRequired"
        End If

        If MyLocation IsNot Nothing Then
            If MyLocation.RvWatercrafts IsNot Nothing Then
                LoadStaticData()
                HideAllRVWater()
                Dim RVWaterCnt As Integer = 1

                If MyLocation.RvWatercrafts.Count > 1 Then
                    RVWaterCnt = MyLocation.RvWatercrafts.Count
                End If

                lblCommonRVWatercraftHdr.Text = String.Format("RV / WATERCRAFT #{0} - {1} {2} {3}", RVWatercraftNumber + 1, MyVehicle.Year, MyVehicle.Manufacturer, MyVehicle.Model).ToUpper()

                lblCommonRVWatercraftHdr.Text = IFM.Common.InputValidation.InputHelpers.EllipsisText(lblCommonRVWatercraftHdr.Text, 30)

                If RvWaterCraftHelper.IsRvWaterCraftMotorAvailable(Quote) Then
                    If MyVehicle.RvWatercraftTypeId = "3" Then
                        lblCommonRVWatercraftHdr.Text = String.Format("RV/WATERCRAFT #{0} - BOAT MOTOR ONLY", RVWatercraftNumber + 1).ToUpper()
                        lblCommonRVWatercraftHdr.Text = IFM.Common.InputValidation.InputHelpers.EllipsisText(lblCommonRVWatercraftHdr.Text, 35)
                    End If
                End If

                ' *******************
                ' Vehicle Information
                ' *******************
                hiddenWatercraftType.Value = qqhelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteRvWatercraft, QuickQuoteHelperClass.QuickQuotePropertyName.RvWatercraftTypeId, MyVehicle.RvWatercraftTypeId).ToUpper()
                IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromValue(ddlVehType, MyVehicle.RvWatercraftTypeId)
                txtVehYear.Text = MyVehicle.Year
                txtVehMake.Text = MyVehicle.Manufacturer
                txtVehModel.Text = MyVehicle.Model
                txtVehSerialNum.Text = MyVehicle.SerialNumber
                txtDescription.Text = MyVehicle.Description

                'added 12/5/17 for HOM Upgrade MLW
                hiddenSelectedForm.Value = CurrentForm

                If hiddenSelectedForm.Value <> "ML-2" And hiddenSelectedForm.Value <> "ML-4" Then
                    txtHorsepowerCCs.Text = MyVehicle.HorsepowerCC
                Else
                    If MyVehicle.RvWatercraftMotors IsNot Nothing Then
                        If qqhelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteRvWatercraftMotor, QuickQuoteHelperClass.QuickQuotePropertyName.MotorTypeId, MyVehicle.RvWatercraftMotors(0).MotorTypeId).ToUpper() = "OUTBOARD" Then
                            txtHorsepowerCCs.Text = MyVehicle.HorsepowerCC
                        Else
                            txtHorsepowerCCs.Text = MyVehicle.RatedSpeed
                        End If
                    Else
                        txtHorsepowerCCs.Text = MyVehicle.HorsepowerCC
                    End If
                End If

                If Quote.LobType = QuickQuoteObject.QuickQuoteLobType.Farm Then
                    txtHorsepowerCCs.Text = MyVehicle.RatedSpeed
                End If

                txtVehLength.Text = MyVehicle.Length
                txtVehCostNew.Text = ""
                If MyVehicle.CostNew.NoneAreNullEmptyorWhitespace() AndAlso MyVehicle.CostNew.IsNumeric() AndAlso _chc.NumericStringComparison(CDec(MyVehicle.CostNew), CommonHelperClass.ComparisonOperators.GreaterThan, 0) Then
                    'If MyVehicle.CostNew.Contains("$") Then
                    '    txtVehCostNew.Text = MyVehicle.CostNew.Replace("$", "").Substring(0, MyVehicle.CostNew.Length - 4)
                    'Else
                    '    txtVehCostNew.Text = MyVehicle.CostNew
                    'End If
                    txtVehCostNew.Text = Math.Truncate(CDec(MyVehicle.CostNew))
                End If

                ''
                '' Determine how to keep this blank when just the vehicle type is selected
                ''
                ' Set Coverage Option Dropdown
                If MyVehicle.RvWatercraftTypeId <> "" AndAlso MyVehicle.RvWatercraftTypeId <> "0" Then
                    'Updated 5/15/18 for Bug 26544 MLW - cannot use MyVehicle.PropertyDeductibleLimitId <> "-1" because coverage options do not populate for some vehicle types
                    If qqhelper.doUseNewVersionOfLOB(Quote, QuickQuoteHelperClass.LOBIFMVersions.HOM2018Upgrade, "7/1/2018") Then
                        If MyVehicle.HasLiability And Not MyVehicle.HasLiabilityOnly Then
                            ddlCoverageOptions.SetFromText("PHYSICAL DAMAGE AND LIABILITY")
                            'ddlCoverageOptions.SelectedIndex = 1
                            'SelectedCoverageOption = "PHYSICAL DAMAGE AND LIABILITY"
                        ElseIf Not MyVehicle.HasLiability And Not MyVehicle.HasLiabilityOnly Then
                            ddlCoverageOptions.SetFromText("PHYSICAL DAMAGE ONLY")
                            'ddlCoverageOptions.SelectedIndex = 2
                            'SelectedCoverageOption = "PHYSICAL DAMAGE ONLY"
                        ElseIf MyVehicle.HasLiability And MyVehicle.HasLiabilityOnly Then
                            ddlCoverageOptions.SetFromText("LIABILITY ONLY")
                            'ddlCoverageOptions.SelectedIndex = 3
                            'SelectedCoverageOption = "LIABILITY ONLY"
                        Else
                            ddlCoverageOptions.SetFromText("")
                        End If
                    Else
                        If MyVehicle.HasLiability And Not MyVehicle.HasLiabilityOnly And MyVehicle.PropertyDeductibleLimitId <> "-1" Then
                            ddlCoverageOptions.SetFromText("PHYSICAL DAMAGE AND LIABILITY")
                            'ddlCoverageOptions.SelectedIndex = 1
                            'SelectedCoverageOption = "PHYSICAL DAMAGE AND LIABILITY"
                        ElseIf Not MyVehicle.HasLiability And Not MyVehicle.HasLiabilityOnly And MyVehicle.PropertyDeductibleLimitId <> "-1" Then
                            ddlCoverageOptions.SetFromText("PHYSICAL DAMAGE ONLY")
                            'ddlCoverageOptions.SelectedIndex = 2
                            'SelectedCoverageOption = "PHYSICAL DAMAGE ONLY"
                        ElseIf MyVehicle.HasLiabilityOnly Then
                            ddlCoverageOptions.SetFromText("LIABILITY ONLY")
                            'ddlCoverageOptions.SelectedIndex = 3
                            'SelectedCoverageOption = "LIABILITY ONLY"
                        End If
                    End If
                    'If MyVehicle.HasLiability And Not MyVehicle.HasLiabilityOnly And MyVehicle.PropertyDeductibleLimitId <> "-1" Then
                    '    ddlCoverageOptions.SetFromText("PHYSICAL DAMAGE AND LIABILITY")
                    '    'ddlCoverageOptions.SelectedIndex = 1
                    '    'SelectedCoverageOption = "PHYSICAL DAMAGE AND LIABILITY"
                    'ElseIf Not MyVehicle.HasLiability And Not MyVehicle.HasLiabilityOnly And MyVehicle.PropertyDeductibleLimitId <> "-1" Then
                    '    ddlCoverageOptions.SetFromText("PHYSICAL DAMAGE ONLY")
                    '    'ddlCoverageOptions.SelectedIndex = 2
                    '    'SelectedCoverageOption = "PHYSICAL DAMAGE ONLY"
                    'ElseIf MyVehicle.HasLiabilityOnly Then
                    '    ddlCoverageOptions.SetFromText("LIABILITY ONLY")
                    '    'ddlCoverageOptions.SelectedIndex = 3
                    '    'SelectedCoverageOption = "LIABILITY ONLY"
                    'End If
                Else
                    ddlCoverageOptions.SetFromText("")
                    'SelectedCoverageOption = Nothing
                End If

                If qqhelper.doUseNewVersionOfLOB(Quote, QuickQuoteHelperClass.LOBIFMVersions.HOM2018Upgrade, "7/1/2018") Then
                    chkCollisionCoverage.Checked = MyVehicle.HasCollision 'added 12/15/2017 for HOM2018 Upgrade
                    'Updated 7/5/18 for HOM2011 Upgrade post go-live changes MLW
                    chkOwnerOtherThanInsured.Checked = False
                    'chkOwnerOtherThanInsured.Checked = MyVehicle.OwnerOtherThanInsured 'added 12/15/2017 for HOM2018 Upgrade
                Else
                    chkCollisionCoverage.Checked = False
                    chkOwnerOtherThanInsured.Checked = False
                End If

                'hiddenSelectedCoverage.Value = SelectedCoverageOption
                SelectedCoverageOption = ddlCoverageOptions.SelectedItem.Text

                IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromValue(Me.ddlPropertyDeductible, MyVehicle.PropertyDeductibleLimitId)
                IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromValue(ddlBodilyInjuryLimit, MyVehicle.UninsuredMotoristBodilyInjuryLimitId)

                If Quote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso Quote.Operators IsNot Nothing Then
                    If Me.IsQuoteEndorsement Then
                        'Updated 03/12/2020 for Home Endorsements task 38919 MLW
                        Dim hasYouthful As Boolean = False
                        If MyVehicle.RvWatercraftTypeId <> "" AndAlso (MyVehicle.RvWatercraftTypeId = 1 OrElse MyVehicle.RvWatercraftTypeId = 7) Then '1=Watercraft, 7=Jet Skis/Waverunner 
                            Dim youthfulNums As IEnumerable(Of Int32) = IFM.VR.Common.Helpers.WaterCraftOperatorHelper.GetYouthfulOperatorNums(Me.Quote)
                            If youthfulNums IsNot Nothing AndAlso youthfulNums.Count > 0 AndAlso MyVehicle.AssignedOperatorNums IsNot Nothing AndAlso MyVehicle.AssignedOperatorNums.Count > 0 Then
                                For Each operNum As Integer In MyVehicle.AssignedOperatorNums
                                    If youthfulNums.Contains(operNum) = True Then
                                        hasYouthful = True
                                        Exit For
                                    End If
                                Next
                            End If
                        End If
                        Me.chkUnder25Operator.Checked = hasYouthful
                        'Me.chkUnder25Operator.Checked = IFM.VR.Common.Helpers.WaterCraftOperatorHelper.GetYouthfulOperators(Me.Quote).Any()
                        Me.chkUnder25Operator.Enabled = Not Me.chkUnder25Operator.Checked 'disable if checked
                    Else
                        Dim policyEffDate As Date
                        Dim birthDate As Date
                        Dim span As TimeSpan

                        If MyVehicle.AssignedOperatorNums IsNot Nothing Then
                            Dim youthOper As Boolean = False
                            Dim defaultYouthOper As Boolean = False

                            For Each operNum As Integer In MyVehicle.AssignedOperatorNums
                                Dim assignedOper As QuickQuoteOperator = Nothing

                                Try
                                    assignedOper = Quote.Operators.Find(Function(p) p.OperatorNum = operNum)
                                Catch ex As Exception

                                End Try

                                If assignedOper IsNot Nothing Then
                                    birthDate = assignedOper.Name.BirthDate

                                    If Quote.EffectiveDate = "" Then
                                        policyEffDate = DateTime.Now
                                    Else
                                        policyEffDate = Quote.EffectiveDate
                                    End If

                                    span = policyEffDate.Subtract(birthDate)

                                    If (span.Duration().Days / 365) < 25 Then
                                        youthOper = True

                                        If assignedOper.Name.FirstName.ToUpper() = "YOUTHFUL" And assignedOper.Name.LastName.ToUpper() = "OPERATOR" Then
                                            defaultYouthOper = True
                                        End If
                                    End If
                                End If
                            Next

                            If youthOper Then
                                chkUnder25Operator.Checked = True

                                If defaultYouthOper Then
                                    chkUnder25Operator.Enabled = True
                                    lblUnder25OperMsg.Attributes.Add("style", "display:block;")
                                Else
                                    chkUnder25Operator.Enabled = False
                                    lblUnder25OperMsg.Attributes.Add("style", "display:none;")
                                End If
                            End If
                        End If
                    End If

                End If

                '***************
                ' Motor
                '***************
                If MyVehicle.RvWatercraftMotors IsNot Nothing AndAlso MyVehicle.RvWatercraftMotors.Any() Then
                    hiddenMotorType.Value = qqhelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteRvWatercraftMotor, QuickQuoteHelperClass.QuickQuotePropertyName.MotorTypeId, MyVehicle.RvWatercraftMotors(0).MotorTypeId).ToUpper()
                    If MyVehicle.RvWatercraftMotors(0).MotorTypeId <> "0" And MyVehicle.RvWatercraftMotors(0).MotorTypeId <> "-1" Then
                        'divMotorInput.Attributes.Add("style", "display:block;")
                        IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromValue(ddlMotorType, MyVehicle.RvWatercraftMotors(0).MotorTypeId)
                        txtMotorYear.Text = MyVehicle.RvWatercraftMotors(0).Year
                        txtMotorMake.Text = MyVehicle.RvWatercraftMotors(0).Manufacturer
                        txtMotorModel.Text = MyVehicle.RvWatercraftMotors(0).Model
                        txtMotorSerialNum.Text = MyVehicle.RvWatercraftMotors(0).SerialNumber
                        txtMotorCostNew.Text = ""
                        If MyVehicle.RvWatercraftMotors(0).CostNew.NoneAreNullEmptyorWhitespace() AndAlso MyVehicle.RvWatercraftMotors(0).CostNew.IsNumeric() AndAlso _chc.NumericStringComparison(CDec(MyVehicle.RvWatercraftMotors(0).CostNew), CommonHelperClass.ComparisonOperators.GreaterThan, 0) Then
                            'If MyVehicle.RvWatercraftMotors(0).CostNew.Contains("$") Then
                            '    txtMotorCostNew.Text = MyVehicle.RvWatercraftMotors(0).CostNew.Replace("$", "").Substring(0, MyVehicle.RvWatercraftMotors(0).CostNew.Length - 4)
                            'Else
                            '    txtMotorCostNew.Text = MyVehicle.RvWatercraftMotors(0).CostNew
                            'End If
                            txtMotorCostNew.Text = Math.Truncate(CDec(MyVehicle.RvWatercraftMotors(0).CostNew))
                        End If

                    Else
                        IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromValue(ddlMotorType, MyVehicle.RvWatercraftMotors(0).MotorTypeId)
                    End If
                End If

                RVWatercraftVisibility()
                CoverageOptionVisibility()
                MotorVisibility()
            End If
        End If

        'Me.ctlWatercraftOperatorAssignment.WatercraftIndex = Me.MyRVWaterIndex
        Me.PopulateChildControls()

        If Quote.LobType = QuickQuoteObject.QuickQuoteLobType.Farm AndAlso ddlVehType.SelectedItem.Text.EqualsAny("WATERCRAFT") Then
            divVehTypeoWatercraftText.Attributes.Add("style", "display:block;")
        End If

    End Sub

    Public Overrides Function Save() As Boolean
        If MyLocation IsNot Nothing Then
            If MyLocation.RvWatercrafts IsNot Nothing Then
                If MyVehicle IsNot Nothing Then
                    '********************
                    ' Vehicle Information
                    '********************
                    MyVehicle.RvWatercraftTypeId = ddlVehType.SelectedValue
                    MyVehicle.Year = txtVehYear.Text
                    MyVehicle.Manufacturer = txtVehMake.Text
                    MyVehicle.Model = txtVehModel.Text
                    MyVehicle.SerialNumber = txtVehSerialNum.Text
                    MyVehicle.Length = txtVehLength.Text
                    MyVehicle.CostNew = txtVehCostNew.Text
                    MyVehicle.Description = txtDescription.Text
                    MyVehicle.HasCollision = False
                    MyVehicle.OwnerOtherThanInsured = False

                    Select Case hiddenSelectedCoverage.Value
                        Case "PHYSICAL DAMAGE AND LIABILITY"
                            MyVehicle.PropertyDeductibleLimitId = ddlPropertyDeductible.SelectedValue
                            MyVehicle.HasLiability = True
                            MyVehicle.HasLiabilityOnly = False
                            If QQHelper.doUseNewVersionOfLOB(Quote, QuickQuoteHelperClass.LOBIFMVersions.HOM2018Upgrade, "7/1/2018") Then
                                If MyLocation.OccupancyCodeId <> 4 AndAlso MyLocation.OccupancyCodeId <> 5 Then
                                    MyVehicle.HasCollision = chkCollisionCoverage.Checked
                                    MyVehicle.OwnerOtherThanInsured = chkOwnerOtherThanInsured.Checked
                                End If
                            End If
                            'hiddenSelectedCoverage.Value = "PHYSICAL DAMAGE AND LIABILITY"
                            'SelectedCoverageOption = "PHYSICAL DAMAGE AND LIABILITY"
                            hiddenPhysicalDamage.Value = ddlPropertyDeductible.SelectedValue
                        Case "PHYSICAL DAMAGE ONLY"
                            MyVehicle.PropertyDeductibleLimitId = ddlPropertyDeductible.SelectedValue
                            MyVehicle.HasLiability = False
                            MyVehicle.HasLiabilityOnly = False
                            'hiddenSelectedCoverage.Value = "PHYSICAL DAMAGE ONLY"
                            'SelectedCoverageOption = "PHYSICAL DAMAGE ONLY"
                            hiddenPhysicalDamage.Value = ddlPropertyDeductible.SelectedValue
                        Case "LIABILITY ONLY"
                            IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromValue(Me.ddlPropertyDeductible, "")
                            MyVehicle.HasLiability = True
                            MyVehicle.HasLiabilityOnly = True
                            'hiddenSelectedCoverage.Value = "LIABILITY ONLY"
                            'SelectedCoverageOption = "LIABILITY ONLY"
                            hiddenPhysicalDamage.Value = "-1"
                        Case Else
                            IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromValue(Me.ddlPropertyDeductible, "")
                            'Updated 5/15/18 for Bug 26544 MLW
                            If QQHelper.doUseNewVersionOfLOB(Quote, QuickQuoteHelperClass.LOBIFMVersions.HOM2018Upgrade, "7/1/2018") Then
                                'forces a "" selection for coverage option
                                MyVehicle.HasLiability = False
                                MyVehicle.HasLiabilityOnly = True
                            Else
                                MyVehicle.HasLiability = False
                                MyVehicle.HasLiabilityOnly = False
                            End If
                            'MyVehicle.HasLiability = False
                            'MyVehicle.HasLiabilityOnly = False
                            'hiddenSelectedCoverage.Value = ""
                            'SelectedCoverageOption = ""
                            hiddenPhysicalDamage.Value = "-1"
                    End Select

                    'AutoFill the vehicle's assigned operater with the first Quote Operator, this should be the same person as the first applicant.
                    If QQHelper.doUseNewVersionOfLOB(Quote, QuickQuoteHelperClass.LOBIFMVersions.HOM2018Upgrade, "7/1/2018") Then
                        If MyVehicle.RvWatercraftTypeId.EqualsAny("6", "7", "8", "9", "10", "12") AndAlso (MyVehicle.AssignedOperatorNums Is Nothing OrElse MyVehicle.AssignedOperatorNums.Count = 0) Then
                            If Quote.Applicants IsNot Nothing AndAlso (Quote.Applicants(0) IsNot Nothing OrElse Quote.Applicants(1) IsNot Nothing) Then
                                If MyVehicle.AssignedOperatorNums Is Nothing Then
                                    MyVehicle.AssignedOperatorNums = New List(Of Integer)
                                End If

                                'This does not appear to be needed
                                'If MyVehicle.AddedOperators Is Nothing Then
                                '    MyVehicle.AddedOperators = New List(Of QuickQuoteOperator)
                                'End If

                                If Quote.Operators Is Nothing OrElse Quote.Operators.Count = 0 Then
                                    'Most likely won't be hit. This should have happened on the insured control. If it didn't though, lets make sure the operators list gets initialized and filled with the applicants.
                                    Quote.Operators = New List(Of QuickQuoteOperator)
                                    Quote.CopyPolicyholdersToOperators()
                                End If

                                'MyVehicle.Operators.Add(QQHelper.CloneObject(Quote.Operators(0)))
                                MyVehicle.AssignedOperatorNums.Add(Quote.Operators(0).OperatorNum)
                            End If
                        End If
                    End If

                    If Quote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso chkUnder25Operator.Enabled Then
                        If Me.IsQuoteEndorsement Then
                            Dim youthfulOperators = IFM.VR.Common.Helpers.WaterCraftOperatorHelper.GetYouthfulOperators(Me.Quote)
                            If chkUnder25Operator.Checked Then
                                If Not youthfulOperators.Any() Then
                                    'Updated 03/10/2020 for home endorsement task 38919 MLW 
                                    Dim operatorsControl = Me.FindVrControlsOfType(Of ctlYoungestOperator).FirstOrDefault()
                                    'operatorsControl.Save() 'moved 04/15/2020 for bug 45727 MLW
                                    Dim policyDate As Date
                                    If Quote.EffectiveDate = "" Then
                                        policyDate = DateTime.Now
                                    Else
                                        policyDate = If(IsDate(Quote.EffectiveDate), Quote.EffectiveDate, DateTime.Now)
                                    End If
                                    Dim op As New QuickQuote.CommonObjects.QuickQuoteOperator()
                                    op.Name.BirthDate = policyDate.AddYears(-16).ToShortDateString()
                                    Me.Quote.Operators.Add(op)
                                    operatorsControl.Save() 'moved 04/15/2020 for bug 45727 MLW
                                    operatorsControl.Populate()
                                    youthfulOperators = IFM.VR.Common.Helpers.WaterCraftOperatorHelper.GetYouthfulOperators(Me.Quote)
                                End If
                                Dim youthfulOperatorsNums = IFM.VR.Common.Helpers.WaterCraftOperatorHelper.GetYouthfulOperatorNums(Me.Quote)

                                MyVehicle.AssignedOperatorNums.CreateIfNull()
                                MyVehicle.AssignedOperatorNums = youthfulOperatorsNums.Union(MyVehicle.AssignedOperatorNums).ToList()
                            End If
                        Else
                            If chkUnder25Operator.Checked Then
                                If MyVehicle.AssignedOperatorNums Is Nothing Then
                                    MyVehicle.AssignedOperatorNums = New List(Of Integer)
                                    'Dim assignedOperNum As QuickQuoteRvWatercraft = New QuickQuoteRvWatercraft()
                                End If

                                If Quote.Operators Is Nothing Then
                                    Quote.Operators = New List(Of QuickQuoteOperator)
                                End If

                                If MyVehicle.AddedOperators Is Nothing Then
                                    MyVehicle.AddedOperators = New List(Of QuickQuoteOperator)
                                End If

                                Dim operatorRec As QuickQuoteOperator = New QuickQuoteOperator()

                                Dim policyDate As Date

                                If Quote.EffectiveDate = "" Then
                                    policyDate = DateTime.Now
                                Else
                                    policyDate = If(IsDate(Quote.EffectiveDate), Quote.EffectiveDate, DateTime.Now)
                                End If

                                Dim youthfulDate As Date = policyDate.AddYears(-16)

                                operatorRec.Name.FirstName = "YOUTHFUL"
                                operatorRec.Name.LastName = "OPERATOR"
                                operatorRec.Name.BirthDate = youthfulDate

                                If Quote.Operators.FindAll(Function(p) p.Name.FirstName = "YOUTHFUL" And p.Name.LastName = "OPERATOR").Count <= 0 Then
                                    operatorRec.OperatorNum = Quote.Operators.Count + 1
                                    Quote.Operators.Add(operatorRec)
                                    MyVehicle.AssignedOperatorNums.Add(Quote.Operators.Count)
                                Else
                                    Dim youthOperAssigned As Boolean = False
                                    Dim youthOperNum As Integer = 0

                                    Try
                                        youthOperNum = Quote.Operators.Find(Function(p) p.Name.FirstName = "YOUTHFUL" And p.Name.LastName = "OPERATOR").OperatorNum
                                    Catch ex As Exception
                                        Quote.Operators.Find(Function(p) p.Name.FirstName = "YOUTHFUL" And p.Name.LastName = "OPERATOR").OperatorNum = Quote.Operators.Count
                                        youthOperNum = Quote.Operators.Count
                                    End Try

                                    For Each youth As Integer In MyVehicle.AssignedOperatorNums
                                        youthOperAssigned = True
                                    Next

                                    If Not youthOperAssigned Then
                                        MyVehicle.AssignedOperatorNums.Add(Quote.Operators.Count)
                                    End If
                                End If
                            Else
                                If pnlUnder25Operator.Visible Then
                                    If Quote.Operators IsNot Nothing Then
                                        Dim youthCnt As Integer = 0
                                        For Each item As QuickQuoteOperator In Quote.Operators
                                            If item.Name.FirstName.ToUpper() = "YOUTHFUL" And item.Name.LastName.ToUpper() = "OPERATOR" Then
                                                If MyVehicle.AssignedOperatorNums IsNot Nothing Then
                                                    Dim removeList As New List(Of Int32) ' Matt A - 8-17-2016
                                                    For Each assignDrvr As Integer In MyVehicle.AssignedOperatorNums
                                                        If assignDrvr = item.OperatorNum Then
                                                            removeList.Add(assignDrvr)
                                                            'MyVehicle.AssignedOperatorNums.Remove(assignDrvr) ' Matt A - 8-17-2016 - You can not remove/add and item to a list inside a foreach over that list
                                                        End If
                                                    Next
                                                    removeList.Reverse() ' Matt A - 8-17-2016
                                                    For Each index In removeList ' Matt A - 8-17-2016
                                                        MyVehicle.AssignedOperatorNums.Remove(index)
                                                    Next
                                                End If
                                                'Quote.Operators = Nothing
                                                'MyVehicle.AssignedOperatorNums = Nothing
                                            End If
                                        Next
                                    End If
                                End If
                            End If
                        End If
                    End If

                    '***************
                    ' Motor
                    '***************
                    If MyVehicle.RvWatercraftMotors Is Nothing Then
                        MyVehicle.RvWatercraftMotors = New List(Of QuickQuoteRvWatercraftMotor)
                        Dim rvwm As New QuickQuoteRvWatercraftMotor()
                        MyVehicle.RvWatercraftMotors.Add(rvwm)
                    Else
                        If MyVehicle.RvWatercraftMotors.Count = 0 Then
                            Dim rvwm As New QuickQuoteRvWatercraftMotor()
                            MyVehicle.RvWatercraftMotors.Add(rvwm)
                        End If
                    End If

                    If hiddenSelectedForm.Value <> "ML-2" AndAlso hiddenSelectedForm.Value <> "ML-4" Then
                        If QQHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteRvWatercraftMotor, QuickQuoteHelperClass.QuickQuotePropertyName.MotorTypeId, ddlMotorType.SelectedValue).ToUpper() = "NONE" Then
                            MyVehicle.HorsepowerCC = "1"
                        Else
                            MyVehicle.HorsepowerCC = txtHorsepowerCCs.Text
                        End If
                    Else
                        If QQHelper.doUseNewVersionOfLOB(Quote, QuickQuoteHelperClass.LOBIFMVersions.HOM2018Upgrade, "7/1/2018") Then
                            MyVehicle.HorsepowerCC = txtHorsepowerCCs.Text
                            MyVehicle.RatedSpeed = "1"
                        Else
                            If QQHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteRvWatercraftMotor, QuickQuoteHelperClass.QuickQuotePropertyName.MotorTypeId, ddlMotorType.SelectedValue).ToUpper() <> "OUTBOARD" Then
                                MyVehicle.HorsepowerCC = "1"

                                If QQHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteRvWatercraftMotor, QuickQuoteHelperClass.QuickQuotePropertyName.MotorTypeId, ddlMotorType.SelectedValue).ToUpper() = "NONE" Then
                                    MyVehicle.RatedSpeed = "1"
                                Else
                                    MyVehicle.RatedSpeed = txtHorsepowerCCs.Text
                                End If
                            Else
                                If QQHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteRvWatercraftMotor, QuickQuoteHelperClass.QuickQuotePropertyName.MotorTypeId, ddlMotorType.SelectedValue).ToUpper() = "NONE" Then
                                    MyVehicle.HorsepowerCC = "1"
                                Else
                                    MyVehicle.HorsepowerCC = txtHorsepowerCCs.Text
                                End If
                            End If
                        End If
                    End If

                    If Quote.LobType = QuickQuoteObject.QuickQuoteLobType.Farm Then
                        MyVehicle.RatedSpeed = txtHorsepowerCCs.Text
                    End If

                    Select Case QQHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteRvWatercraft, QuickQuoteHelperClass.QuickQuotePropertyName.RvWatercraftTypeId, ddlVehType.SelectedValue).ToUpper()
                        Case "BOAT MOTOR ONLY"
                            MyVehicle.RvWatercraftMotors(0).MotorTypeId = "2"
                            IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromValue(ddlMotorType, MyVehicle.RvWatercraftMotors(0).MotorTypeId)

                            If txtHorsepowerCCs.Text <> "" Then
                                If Integer.Parse(txtHorsepowerCCs.Text) > 150 Then
                                    MyVehicle.Length = "16"
                                End If
                            End If
                        Case "JET SKIS & WAVERUNNERS"
                            MyVehicle.RvWatercraftMotors(0).MotorTypeId = "1"
                            IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromValue(ddlMotorType, MyVehicle.RvWatercraftMotors(0).MotorTypeId)
                        Case Else
                            MyVehicle.RvWatercraftMotors(0).MotorTypeId = ddlMotorType.SelectedValue
                    End Select

                    MyVehicle.RvWatercraftMotors(0).Year = txtMotorYear.Text
                    MyVehicle.RvWatercraftMotors(0).Manufacturer = txtMotorMake.Text
                    MyVehicle.RvWatercraftMotors(0).Model = txtMotorModel.Text
                    MyVehicle.RvWatercraftMotors(0).SerialNumber = txtMotorSerialNum.Text
                    MyVehicle.RvWatercraftMotors(0).CostNew = txtMotorCostNew.Text

                    ' Property Deductible
                    MyVehicle.PropertyDeductibleLimitId = ddlPropertyDeductible.SelectedValue

                    ' Bodily Injury Limit
                    MyVehicle.UninsuredMotoristBodilyInjuryLimitId = ddlBodilyInjuryLimit.SelectedValue

                    Return True
                End If
            End If
        End If

        Return False
    End Function

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        MyBase.ValidateControl(valArgs)
        MyBase.ValidateControl(valArgs)
        ValidationHelper.GroupName = "RV/Watercraft #" & (RVWatercraftNumber + 1) & " Coverages"
        Dim divRVWCoverages As String = ParentVrControl.ListAccordionDivId
        Dim buildMtrlErr As Boolean = False
        'don't like this but PPA is too far along to change right now
        Dim validationType = Me.DefaultValidationType
        If (Me.IsQuoteEndorsement) Then
            validationType = Validation.ObjectValidation.ValidationItem.ValidationType.endorsement
        End If

        Dim valList = RvWaterCraftValidator.ValidateRvWaterCraft(MyVehicle, validationType, Quote.LobType, CurrentForm, ddlCoverageOptions.SelectedValue, ddlVehType.SelectedValue, quote:=Quote)

        ' Vehicle Validation
        If valList.Any() Then
            For Each v In valList
                Select Case v.FieldId
                    Case RvWaterCraftValidator.VehicleTypeIsNull
                        ValidationHelper.Val_BindValidationItemToControl(ddlVehType.ClientID, v, divRVWCoverages, RVWatercraftNumber.ToString())
                    Case RvWaterCraftValidator.CoverageOptionIsNull
                        ValidationHelper.Val_BindValidationItemToControl(ddlCoverageOptions.ClientID, v, divRVWCoverages, RVWatercraftNumber.ToString())
                    Case RvWaterCraftValidator.PropertyDeductIsNull
                        ValidationHelper.Val_BindValidationItemToControl(ddlPropertyDeductible.ClientID, v, divRVWCoverages, RVWatercraftNumber.ToString())
                    Case RvWaterCraftValidator.YearMissing
                        ValidationHelper.Val_BindValidationItemToControl(txtVehYear.ClientID, v, divRVWCoverages, RVWatercraftNumber.ToString())
                    Case RvWaterCraftValidator.YearRange
                        ValidationHelper.Val_BindValidationItemToControl(txtVehYear.ClientID, v, divRVWCoverages, RVWatercraftNumber.ToString())
                    Case RvWaterCraftValidator.LengthMissing
                        ValidationHelper.Val_BindValidationItemToControl(txtVehLength.ClientID, v, divRVWCoverages, RVWatercraftNumber.ToString())
                    Case RvWaterCraftValidator.LengthNumeric
                        ValidationHelper.Val_BindValidationItemToControl(txtVehLength.ClientID, v, divRVWCoverages, RVWatercraftNumber.ToString())
                    Case RvWaterCraftValidator.LengthInvalid
                        ValidationHelper.Val_BindValidationItemToControl(txtVehLength.ClientID, v, divRVWCoverages, RVWatercraftNumber.ToString())
                    Case RvWaterCraftValidator.LengthMaximum
                        ValidationHelper.Val_BindValidationItemToControl(txtVehLength.ClientID, v, divRVWCoverages, RVWatercraftNumber.ToString())
                    Case RvWaterCraftValidator.CostNewMissing
                        ValidationHelper.Val_BindValidationItemToControl(txtVehCostNew.ClientID, v, divRVWCoverages, RVWatercraftNumber.ToString())
                    Case RvWaterCraftValidator.CostNewNumeric
                        ValidationHelper.Val_BindValidationItemToControl(txtVehCostNew.ClientID, v, divRVWCoverages, RVWatercraftNumber.ToString())
                    Case RvWaterCraftValidator.CostNewInvalid
                        ValidationHelper.Val_BindValidationItemToControl(txtVehCostNew.ClientID, v, divRVWCoverages, RVWatercraftNumber.ToString())
                    Case RvWaterCraftValidator.CostNew_LessThan_Deductible
                        ValidationHelper.Val_BindValidationItemToControl(txtVehCostNew.ClientID, v, divRVWCoverages, RVWatercraftNumber.ToString())
                    Case RvWaterCraftValidator.DescMissing
                        ValidationHelper.Val_BindValidationItemToControl(txtDescription.ClientID, v, divRVWCoverages, RVWatercraftNumber.ToString())
                    Case RvWaterCraftValidator.MotorTypeMissing
                        ValidationHelper.Val_BindValidationItemToControl(ddlMotorType.ClientID, v, divRVWCoverages, RVWatercraftNumber.ToString())
                    Case RvWaterCraftValidator.HPMissing
                        ValidationHelper.Val_BindValidationItemToControl(txtHorsepowerCCs.ClientID, v, divRVWCoverages, RVWatercraftNumber.ToString())
                    Case RvWaterCraftValidator.MotorYearMissing
                        ValidationHelper.Val_BindValidationItemToControl(txtMotorYear.ClientID, v, divRVWCoverages, RVWatercraftNumber.ToString())
                    Case RvWaterCraftValidator.MotorYearInvalid
                        ValidationHelper.Val_BindValidationItemToControl(txtMotorYear.ClientID, v, divRVWCoverages, RVWatercraftNumber.ToString())
                    Case RvWaterCraftValidator.MotorCostNewMissing
                        ValidationHelper.Val_BindValidationItemToControl(txtMotorCostNew.ClientID, v, divRVWCoverages, RVWatercraftNumber.ToString())
                    Case RvWaterCraftValidator.MotorCostNewNumeric
                        ValidationHelper.Val_BindValidationItemToControl(txtMotorCostNew.ClientID, v, divRVWCoverages, RVWatercraftNumber.ToString())
                    Case RvWaterCraftValidator.MotorCostNewInvalid
                        ValidationHelper.Val_BindValidationItemToControl(txtMotorCostNew.ClientID, v, divRVWCoverages, RVWatercraftNumber.ToString())
                    Case RvWaterCraftValidator.MPHInvalid
                        ValidationHelper.Val_BindValidationItemToControl(txtHorsepowerCCs.ClientID, v, divRVWCoverages, RVWatercraftNumber.ToString())

                    Case RvWaterCraftValidator.SerialNumberMissing, RvWaterCraftValidator.SerialNumberInvalid ' Matt A - 8/4/15
                        If ddlVehType.SelectedValue <> "3" Then
                            ValidationHelper.Val_BindValidationItemToControl(txtVehSerialNum.ClientID, v, divRVWCoverages, RVWatercraftNumber.ToString())
                        Else
                            ValidationHelper.Val_BindValidationItemToControl(txtMotorSerialNum.ClientID, v, divRVWCoverages, RVWatercraftNumber.ToString())
                        End If
                    Case RvWaterCraftValidator.ManufacturerMissing, RvWaterCraftValidator.ManufacturerInvalid ' Matt A - 8/4/15
                        If ddlVehType.SelectedValue <> "3" Then
                            ValidationHelper.Val_BindValidationItemToControl(txtVehMake.ClientID, v, divRVWCoverages, RVWatercraftNumber.ToString())
                        Else
                            ValidationHelper.Val_BindValidationItemToControl(txtMotorMake.ClientID, v, divRVWCoverages, RVWatercraftNumber.ToString())
                        End If
                    Case RvWaterCraftValidator.ModelMissing, RvWaterCraftValidator.ModelInvalid ' Matt A - 8/4/15
                        If ddlVehType.SelectedValue <> "3" Then
                            ValidationHelper.Val_BindValidationItemToControl(txtVehModel.ClientID, v, divRVWCoverages, RVWatercraftNumber.ToString())
                        Else
                            ValidationHelper.Val_BindValidationItemToControl(txtMotorModel.ClientID, v, divRVWCoverages, RVWatercraftNumber.ToString())
                        End If
                End Select
            Next
        End If

        ValidateChildControls(valArgs)
    End Sub

    Protected Sub lnkAdd_Click(sender As Object, e As EventArgs) Handles lnkAdd.Click, btnAddRVWater.Click
        RaiseEvent NewVehicleRequested()
        'If MyLocation IsNot Nothing Then
        '    If MyLocation.RvWatercrafts Is Nothing Then
        '        MyLocation.RvWatercrafts = New List(Of QuickQuoteRvWatercraft)
        '    End If

        '    Dim newRVWater As New QuickQuoteRvWatercraft

        '    MyLocation.RvWatercrafts.Add(newRVWater)
        '    Save_FireSaveEvent(False)
        '    RaiseEvent CommonSetActivePanel((MyLocation.RvWatercrafts.Count - 1).ToString())
        '    RaiseEvent CommonRefreshRVWater()
        'End If
    End Sub

    Protected Sub lnkSave_Click(sender As Object, e As EventArgs) Handles lnkSave.Click, btnSaveRVWater.Click
        RaiseEvent CommonRaiseRVWSave()
        RaiseEvent CommonRefreshRVWater()
    End Sub

    Protected Sub lnkRemove_Click(sender As Object, e As EventArgs) Handles lnkRemove.Click
        If Not HideFromParent Then
            Save_FireSaveEvent(False)

            Dim rvWatercraft As QuickQuoteRvWatercraft = MyLocation.RvWatercrafts(RVWatercraftNumber)
            MyLocation.RvWatercrafts.Remove(rvWatercraft)

            RaiseEvent CommonRefreshRVWater()

            Save_FireSaveEvent(New VrControlBaseSaveEventArgs(Me, False, New IFM.VR.Web.VRValidationArgs(DefaultValidationType)))

            RaiseEvent CommonSetActivePanel("false")
        End If
    End Sub

    Protected Sub btnRateRVWater_Click(sender As Object, e As EventArgs) Handles btnRateRVWater.Click
        RaiseEvent CommonRaiseRVWRate()
        RaiseEvent CommonRefreshRVWater()
    End Sub

    Private Sub HideAllRVWater()
        pnlCoverageOptions.Attributes.Add("style", "display:none;")
        ddlCoverageOptions.Attributes.Add("style", "display:none;")
        lblCoverageOptionPD.Attributes.Add("style", "display:none;")
        lblCoverageOptionPDL.Attributes.Add("style", "display:none;")
        lblCoverageOptionLib.Attributes.Add("style", "display:none;")

        pnlPropertyDeductibleTextBox.Attributes.Add("style", "display:none;") 'added 12/01/2017 HOM2018 Upgrade
        pnlOwnerOtherThanInsured.Attributes.Add("style", "display:none;") 'added 12/01/2017 HOM2018 Upgrade
        pnlCollisionCoverage.Attributes.Add("style", "display:none;") 'added 12/01/2017 HOM2018 Upgrade
        'txtPropertyDeductibleValue.Attributes.Add("style", "display:none;") 'added 12/01/2017 HOM2018 Upgrade

        'ddlCoverageOptions.SelectedIndex = "0"
        pnlVehYear.Attributes.Add("style", "display:none;")
        pnlVehLength.Attributes.Add("style", "display:none;")
        pnlVehCostNew.Attributes.Add("style", "display:none;")
        lblVehCostNew.Attributes.Add("style", "display:none;")
        lblLimit.Attributes.Add("style", "display:none;")
        pnlMotorType.Attributes.Add("style", "display:none;")
        ddlMotorType.Attributes.Add("style", "display:none;")
        lblMotorTypeInOut.Attributes.Add("style", "display:none;")
        lblMotorTypeIn.Attributes.Add("style", "display:none;")
        lblMotorTypeOut.Attributes.Add("style", "display:none;")
        pnlPropertyDeductible.Attributes.Add("style", "display:none;")
        pnlRVWLiabilityOnly.Attributes.Add("style", "display:none;")
        pnlBodilyInjuryLimit.Attributes.Add("style", "display:none;")
        pnlUnder25Operator.Attributes.Add("style", "display:none;")
        lblUnder25OperMsg.Attributes.Add("style", "display:none;")
        pnlVehSerialNum.Attributes.Add("style", "display:none;")
        pnlVehMake.Attributes.Add("style", "display:none;")
        pnlVehModel.Attributes.Add("style", "display:none;")
        pnlDescription.Attributes.Add("style", "display:none;")

        ' Motor
        divMotorInput.Attributes.Add("style", "display:none;")
        pnlHorsepowerCCs.Attributes.Add("style", "display:none;")
        pnlMotorYear.Attributes.Add("style", "display:none;")
        pnlMotorCostNew.Attributes.Add("style", "display:none;")
        pnlMotorSerialNum.Attributes.Add("style", "display:none;")
        pnlMotorMake.Attributes.Add("style", "display:none;")
        pnlMotorModel.Attributes.Add("style", "display:none;")
        'ddlMotorType.SelectedValue = "0"
    End Sub

    Private Sub RVWatercraftVisibility()
        Dim displayOwnerOtherThanInsured As Boolean = False
        Dim displayAddCollision As Boolean = False
        Select Case (QQHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteRvWatercraft, QuickQuoteHelperClass.QuickQuotePropertyName.RvWatercraftTypeId, ddlVehType.SelectedValue)).ToUpper()
            Case "WATERCRAFT"
                pnlCoverageOptions.Attributes.Add("style", "display:block;")
                ddlCoverageOptions.Attributes.Add("style", "display:block;")
                pnlVehYear.Attributes.Add("style", "display:block;")
                pnlVehLength.Attributes.Add("style", "display:block;")
                lblVehCostNew.Attributes.Add("style", "display:block;")
                pnlVehCostNew.Attributes.Add("style", "display:block;")
                pnlMotorType.Attributes.Add("style", "display:block;")
                ddlMotorType.Attributes.Add("style", "display:block;")
                pnlBodilyInjuryLimit.Attributes.Add("style", "display:block;")

                ' Check for Farm LOB
                'If hiddenLOB.Value <> "10" Then
                If Quote.LobType <> QuickQuoteObject.QuickQuoteLobType.Farm Then
                    pnlUnder25Operator.Attributes.Add("style", "display:block;")
                Else
                    pnlUnder25Operator.Attributes.Add("style", "display:none;")
                End If

                pnlVehSerialNum.Attributes.Add("style", "display:block;")
                pnlVehMake.Attributes.Add("style", "display:block;")
                pnlVehModel.Attributes.Add("style", "display:block;")

            Case "SAILBOAT"
                pnlCoverageOptions.Attributes.Add("style", "display:block;")
                ddlCoverageOptions.Attributes.Add("style", "display:block;")
                pnlVehYear.Attributes.Add("style", "display:block;")
                pnlVehLength.Attributes.Add("style", "display:block;")
                lblVehCostNew.Attributes.Add("style", "display:block;")
                pnlBodilyInjuryLimit.Attributes.Add("style", "display:block;")
                pnlVehSerialNum.Attributes.Add("style", "display:block;")
                pnlVehMake.Attributes.Add("style", "display:block;")
                pnlVehModel.Attributes.Add("style", "display:block;")

            Case "BOAT MOTOR ONLY"
                pnlCoverageOptions.Attributes.Add("style", "display:block;")

                If Quote.LobType = QuickQuoteObject.QuickQuoteLobType.Farm Then
                    ddlCoverageOptions.Attributes.Add("style", "display:block;")
                    ddlCoverageOptions.Items.RemoveAt(3)
                Else
                    lblCoverageOptionPDL.Attributes.Add("style", "display:block;")
                    ddlCoverageOptions.SetFromText("PHYSICAL DAMAGE AND LIABILITY")
                End If


                pnlPropertyDeductible.Attributes.Add("style", "display:block;")
                pnlMotorType.Attributes.Add("style", "display:block;")
                lblMotorTypeOut.Attributes.Add("style", "display:block;")

                ' Motor
                divMotorInput.Attributes.Add("style", "display:block;")
                pnlHorsepowerCCs.Attributes.Add("style", "display:block;")
                pnlMotorYear.Attributes.Add("style", "display:block;")
                pnlMotorCostNew.Attributes.Add("style", "display:block;")
                pnlMotorSerialNum.Attributes.Add("style", "display:block;")
                pnlMotorMake.Attributes.Add("style", "display:block;")
                pnlMotorModel.Attributes.Add("style", "display:block;")
                ddlMotorType.SelectedIndex = 3

            Case "4 WHEEL ATV"
                pnlCoverageOptions.Attributes.Add("style", "display:block;")
                If Quote.LobType = QuickQuoteObject.QuickQuoteLobType.Farm OrElse QQHelper.doUseNewVersionOfLOB(Quote, QuickQuoteHelperClass.LOBIFMVersions.HOM2018Upgrade, "7/1/2018") Then
                    ddlCoverageOptions.Attributes.Add("style", "display:block;")
                    'Added 5/15/18 for Bug 26544 MLW
                    pnlVehCostNew.Attributes.Add("style", "display:none;")
                Else
                    lblCoverageOptionPD.Attributes.Add("style", "display:block;")
                    ddlCoverageOptions.SetFromText("PHYSICAL DAMAGE ONLY")
                    pnlVehCostNew.Attributes.Add("style", "display:block;") 'moved 5/15/18 for Bug 26544 MLW
                End If

                pnlPropertyDeductible.Attributes.Add("style", "display:block;")
                'lblPropertyDeductible.Attributes.Add("style", "display:block;") 'Added 7/11/18 for HOM2011 Upgrade post implementation changes MLW
                pnlVehYear.Attributes.Add("style", "display:block;")
                lblVehCostNew.Attributes.Add("style", "display:block;")
                'pnlVehCostNew.Attributes.Add("style", "display:block;") 'moved 5/15/18 for Bug 26544 MLW
                pnlVehSerialNum.Attributes.Add("style", "display:block;")
                pnlVehMake.Attributes.Add("style", "display:block;")
                pnlVehModel.Attributes.Add("style", "display:block;")
                ddlMotorType.SelectedIndex = 1

            Case "GOLF CART"
                pnlCoverageOptions.Attributes.Add("style", "display:block;")
                If QQHelper.doUseNewVersionOfLOB(Quote, QuickQuoteHelperClass.LOBIFMVersions.HOM2018Upgrade, "7/1/2018") Then
                    'Changed 10/30/17 for HOM Upgrade
                    pnlPropertyDeductible.Attributes.Add("style", "display:none;")
                    pnlPropertyDeductibleTextBox.Attributes.Add("style", "display:block;")
                    ddlCoverageOptions.Attributes.Add("style", "display:block;")
                    ddlPropertyDeductible.Attributes.Add("style", "display:none;")
                    'lblPropertyDeductible.Attributes.Add("style", "display:none;") 'removed 7/12/18 for HOM2011 Upgrade post implementation changes MLW - was causing label to not appear when other watercraft types selected after Golf Cart saved
                    txtPropertyDeductibleTextBox.Text = ddlPropertyDeductible.SelectedItem.Text
                    'Updated 7/5/18 for HOM2011 Upgrade post go-live changes MLW
                    displayOwnerOtherThanInsured = False
                    'displayOwnerOtherThanInsured = True
                    'Updated 7/6/18 for HOM2011 Upgrade post go-live changes MLW
                    If MyLocation.OccupancyCodeId = "4" OrElse MyLocation.OccupancyCodeId = "5" Then
                        displayAddCollision = False
                    Else
                        displayAddCollision = True
                    End If
                    'displayAddCollision = True
                    'Added 5/15/18 for Bug 26544 MLW
                    pnlVehCostNew.Attributes.Add("style", "display:none;")
                Else
                    pnlPropertyDeductible.Attributes.Add("style", "display:block;")
                    If Quote.LobType = QuickQuoteObject.QuickQuoteLobType.Farm AndAlso ddlVehType.SelectedItem.Text.EqualsAny("GOLF CART") Then
                        pnlPropertyDeductible.Attributes.Add("style", "display:block;")
                        ddlCoverageOptions.Attributes.Add("style", "display:block;")
                        ddlPropertyDeductible.Attributes.Add("style", "display:block;")
                        txtPropertyDeductibleTextBox.Text = ddlPropertyDeductible.SelectedItem.Text
                    Else
                        'If Quote.LobType <> QuickQuoteObject.QuickQuoteLobType.Farm AndAlso ddlCoverageOptions.SelectedItem.Text <> "GOLF CART" Then
                        If hiddenSelectedForm.Value <> "ML-2" And hiddenSelectedForm.Value <> "ML-4" Then
                            lblCoverageOptionPDL.Attributes.Add("style", "display:block;")
                            ddlCoverageOptions.SetFromText("PHYSICAL DAMAGE AND LIABILITY")
                        Else
                            lblCoverageOptionPD.Attributes.Add("style", "display:block;")
                            ddlCoverageOptions.SetFromText("PHYSICAL DAMAGE ONLY")
                        End If
                    End If
                    pnlVehCostNew.Attributes.Add("style", "display:block;") 'moved 5/15/18 for Bug 26544 MLW
                End If

                pnlVehYear.Attributes.Add("style", "display:block;")
                lblVehCostNew.Attributes.Add("style", "display:block;")
                pnlVehSerialNum.Attributes.Add("style", "display:block;")
                pnlVehMake.Attributes.Add("style", "display:block;")
                pnlVehModel.Attributes.Add("style", "display:block;")
                ddlMotorType.SelectedIndex = 1

            Case "OTHER RV" ' Matt A - 3-21-06 for Comparative Rater Project
                pnlCoverageOptions.Attributes.Add("style", "display:block;")

                If QQHelper.doUseNewVersionOfLOB(Quote, QuickQuoteHelperClass.LOBIFMVersions.HOM2018Upgrade, "7/1/2018") Then
                    ddlCoverageOptions.Attributes.Add("style", "display:block;")
                    'Added 5/15/18 for Bug 26544 MLW
                    pnlVehCostNew.Attributes.Add("style", "display:none;")
                ElseIf Quote.LobType = QuickQuoteObject.QuickQuoteLobType.Farm Then
                    ddlCoverageOptions.Attributes.Add("style", "display:block;")
                    pnlVehCostNew.Attributes.Add("style", "display:block;")
                    ddlCoverageOptions.Items.RemoveAt(3) 'No Liability on Farm - Other RV
                Else
                    lblCoverageOptionPD.Attributes.Add("style", "display:block;")
                    ddlCoverageOptions.SetFromText("PHYSICAL DAMAGE ONLY")
                    pnlVehCostNew.Attributes.Add("style", "display:block;") 'moved 5/15/18 for Bug 26544 MLW
                End If

                pnlPropertyDeductible.Attributes.Add("style", "display:block;")
                pnlVehYear.Attributes.Add("style", "display:block;")
                lblVehCostNew.Attributes.Add("style", "display:block;")
                'pnlVehCostNew.Attributes.Add("style", "display:block;") 'moved 5/15/18 for Bug 26544 MLW
                pnlVehSerialNum.Attributes.Add("style", "display:block;")
                pnlVehMake.Attributes.Add("style", "display:block;")
                pnlVehModel.Attributes.Add("style", "display:block;")
                ddlMotorType.SelectedIndex = 1

            Case "BOAT TRAILER",
                "SNOWMOBILE - TRAILER"
                ' Matt A - 3-21-06 for Comparative Rater Project
                pnlCoverageOptions.Attributes.Add("style", "display:block;")
                lblCoverageOptionPD.Attributes.Add("style", "display:block;")
                ddlCoverageOptions.SetFromText("PHYSICAL DAMAGE ONLY")
                pnlPropertyDeductible.Attributes.Add("style", "display:block;")
                pnlVehYear.Attributes.Add("style", "display:block;")
                lblVehCostNew.Attributes.Add("style", "display:block;")
                pnlVehCostNew.Attributes.Add("style", "display:block;")
                pnlVehSerialNum.Attributes.Add("style", "display:block;")
                pnlVehMake.Attributes.Add("style", "display:block;")
                pnlVehModel.Attributes.Add("style", "display:block;")
                ddlMotorType.SelectedIndex = 1

            Case "ACCESSORIES & EQUIPMENT"
                pnlCoverageOptions.Attributes.Add("style", "display:block;")
                lblCoverageOptionPD.Attributes.Add("style", "display:block;")
                ddlCoverageOptions.SetFromText("PHYSICAL DAMAGE ONLY")
                pnlPropertyDeductible.Attributes.Add("style", "display:block;")
                lblLimit.Attributes.Add("style", "display:block;")
                pnlVehCostNew.Attributes.Add("style", "display:block;")
                pnlDescription.Attributes.Add("style", "display:block;")
                ddlMotorType.SelectedIndex = 1

            Case "JET SKIS & WAVERUNNERS"
                pnlCoverageOptions.Attributes.Add("style", "display:block;")
                ddlCoverageOptions.Attributes.Add("style", "display:block;")
                pnlVehYear.Attributes.Add("style", "display:block;")
                lblVehCostNew.Attributes.Add("style", "display:block;")
                pnlVehLength.Attributes.Add("style", "display:block;")
                pnlBodilyInjuryLimit.Attributes.Add("style", "display:block;")

                ' Check for Farm LOB
                'If hiddenLOB.Value <> "10" Then
                If Quote.LobType <> QuickQuoteObject.QuickQuoteLobType.Farm Then
                    pnlUnder25Operator.Attributes.Add("style", "display:block;")
                Else
                    pnlUnder25Operator.Attributes.Add("style", "display:none;")
                End If

                pnlMotorType.Attributes.Add("style", "display:block;")
                lblMotorTypeIn.Attributes.Add("style", "display:block;")
                pnlVehSerialNum.Attributes.Add("style", "display:block;")
                pnlVehMake.Attributes.Add("style", "display:block;")
                pnlVehModel.Attributes.Add("style", "display:block;")
                divMotorInput.Attributes.Add("style", "display:block;")
                pnlHorsepowerCCs.Attributes.Add("style", "display:block;")
                ddlMotorType.SelectedIndex = 2

            Case "SNOWMOBILE - NAMED PERILS",
             "SNOWMOBILE - SPECIAL COVERAGE"
                pnlCoverageOptions.Attributes.Add("style", "display:block;")
                ddlCoverageOptions.Attributes.Add("style", "display:block;")
                pnlVehYear.Attributes.Add("style", "display:block;")
                lblVehCostNew.Attributes.Add("style", "display:block;")
                pnlVehCostNew.Attributes.Add("style", "display:block;")
                pnlVehSerialNum.Attributes.Add("style", "display:block;")
                pnlVehMake.Attributes.Add("style", "display:block;")
                pnlVehModel.Attributes.Add("style", "display:block;")
                pnlDescription.Attributes.Add("style", "display:block;")
                If QQHelper.doUseNewVersionOfLOB(Quote, QuickQuoteHelperClass.LOBIFMVersions.HOM2018Upgrade, "7/1/2018") Then
                    'Updated 7/5/18 for HOM2011 Upgrade post go-live changes MLW
                    displayOwnerOtherThanInsured = False
                    'displayOwnerOtherThanInsured = True
                    'Added 5/16/18 for Bug 26544 MLW
                    pnlPropertyDeductible.Attributes.Add("style", "display:block;")
                End If
        End Select

        If QQHelper.doUseNewVersionOfLOB(Quote, QuickQuoteHelperClass.LOBIFMVersions.HOM2018Upgrade, "7/1/2018") Then
            If MyLocation.OccupancyCodeId.EqualsAny("4", "5") Then
                If ddlCoverageOptions.Attributes("style").Contains("display:block;") Then
                    ddlCoverageOptions.Attributes.Add("style", "display:none;")
                    lblCoverageOptionPD.Attributes.Add("style", "display:block;")
                End If
            Else
                'Updated 7/5/18 for HOM2011 Upgrade post go-live changes MLW
                If ddlCoverageOptions.SelectedItem.Text = "PHYSICAL DAMAGE AND LIABILITY" AndAlso displayAddCollision = True Then
                    pnlCollisionCoverage.Attributes.Add("style", "display:block;")
                ElseIf ddlCoverageOptions.SelectedItem.Text = "PHYSICAL DAMAGE ONLY" AndAlso displayAddCollision = True Then
                    pnlCollisionCoverage.Attributes.Add("style", "display:block;")
                ElseIf ddlCoverageOptions.SelectedItem.Text = "LIABILITY ONLY" AndAlso displayAddCollision = True Then
                    pnlCollisionCoverage.Attributes.Add("style", "display:block;")
                End If
                'If ddlCoverageOptions.SelectedItem.Text = "PHYSICAL DAMAGE AND LIABILITY" Then
                '    If MyVehicle.HasCollision = True Then
                '        pnlCollisionCoverage.Attributes.Add("style", "display:block;")
                '    End If
                '    If MyVehicle.OwnerOtherThanInsured = True Then
                '        pnlOwnerOtherThanInsured.Attributes.Add("style", "display:block;")
                '    End If
                'ElseIf ddlCoverageOptions.SelectedItem.Text = "PHYSICAL DAMAGE ONLY" AndAlso displayOwnerOtherThanInsured = True Then
                '    pnlOwnerOtherThanInsured.Attributes.Add("style", "display:block;")
                'ElseIf ddlCoverageOptions.SelectedItem.Text = "LIABILITY ONLY" AndAlso displayAddCollision = True Then
                '    pnlCollisionCoverage.Attributes.Add("style", "display:block;")
                'End If
            End If
        End If
    End Sub

    Private Sub MotorVisibility()
        If MyLocation IsNot Nothing Then
            If MyLocation.RvWatercrafts IsNot Nothing Then
                If hiddenSelectedForm.Value <> "ML-2" AndAlso hiddenSelectedForm.Value <> "ML-4" Then
                    lblHorsepowerCCs.Text = "*Horsepower/CCs"
                Else
                    If QQHelper.doUseNewVersionOfLOB(Quote, QuickQuoteHelperClass.LOBIFMVersions.HOM2018Upgrade, "7/1/2018") Then
                        lblHorsepowerCCs.Text = "*Horsepower/CCs"
                    Else
                        lblHorsepowerCCs.Text = "*Rated Speed in MPH"
                    End If
                End If

                If Quote.LobType = QuickQuoteObject.QuickQuoteLobType.Farm Then
                    lblHorsepowerCCs.Text = "*Rated Speed in MPH"
                End If

                Select Case QQHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteRvWatercraftMotor, QuickQuoteHelperClass.QuickQuotePropertyName.MotorTypeId, ddlMotorType.SelectedValue).ToUpper()
                    Case "INBOARD",
                        "INBOARD/OUTBOARD"
                        divMotorInput.Attributes.Add("style", "display:block;")
                        pnlHorsepowerCCs.Attributes.Add("style", "display:block;")

                    Case "OUTBOARD"
                        If RvWaterCraftHelper.HasIncludedMotor(MyVehicle) OrElse RvWaterCraftHelper.IsRvWaterCraftMotorAvailable(Quote) = False Then ' always do if Flag off
                            divMotorInput.Attributes.Add("style", "display:block;")
                            pnlHorsepowerCCs.Attributes.Add("style", "display:block;")
                            pnlMotorYear.Attributes.Add("style", "display:block;")
                            pnlMotorCostNew.Attributes.Add("style", "display:block;")
                            pnlMotorSerialNum.Attributes.Add("style", "display:block;")
                            pnlMotorMake.Attributes.Add("style", "display:block;")
                            pnlMotorModel.Attributes.Add("style", "display:block;")
                        End If

                    Case Else
                        divMotorInput.Attributes.Add("style", "display:none;")
                        pnlHorsepowerCCs.Attributes.Add("style", "display:none;")
                        pnlMotorYear.Attributes.Add("style", "display:none;")
                        pnlMotorCostNew.Attributes.Add("style", "display:none;")
                        pnlMotorSerialNum.Attributes.Add("style", "display:none;")
                        pnlMotorMake.Attributes.Add("style", "display:none;")
                        pnlMotorModel.Attributes.Add("style", "display:none;")
                End Select
            End If
        End If
    End Sub

    Protected Sub ddlCoverageOptions_TextChanged(sender As Object, e As EventArgs) Handles ddlCoverageOptions.TextChanged
        CoverageOptionVisibility()
    End Sub

    Private Sub CoverageOptionVisibility()
        Select Case ddlCoverageOptions.SelectedItem.ToString().ToUpper()
            Case "PHYSICAL DAMAGE ONLY"
                If QQHelper.doUseNewVersionOfLOB(Quote, QuickQuoteHelperClass.LOBIFMVersions.HOM2018Upgrade, "7/1/2018") AndAlso ddlVehType.SelectedItem.Text = "GOLF CART" Then
                    pnlPropertyDeductibleTextBox.Attributes.Add("style", "display:block;")
                Else
                    pnlPropertyDeductible.Attributes.Add("style", "display:block;")
                End If

                If (QQHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteRvWatercraft, QuickQuoteHelperClass.QuickQuotePropertyName.RvWatercraftTypeId, ddlVehType.SelectedValue)).ToUpper() <> "ACCESSORIES & EQUIPMENT" And
                    (QQHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteRvWatercraft, QuickQuoteHelperClass.QuickQuotePropertyName.RvWatercraftTypeId, ddlVehType.SelectedValue)).ToUpper() <> "BOAT MOTOR ONLY" Then
                    pnlVehCostNew.Attributes.Add("style", "display:block;")
                End If
            Case "PHYSICAL DAMAGE AND LIABILITY"
                If QQHelper.doUseNewVersionOfLOB(Quote, QuickQuoteHelperClass.LOBIFMVersions.HOM2018Upgrade, "7/1/2018") AndAlso ddlVehType.SelectedItem.Text.EqualsAny("GOLF CART", "SNOWMOBILE - NAMED PERILS", "SNOWMOBILE - SPECIAL COVERAGE") Then
                    If ddlVehType.SelectedItem.Text.EqualsAny("GOLF CART") Then
                        pnlPropertyDeductibleTextBox.Attributes.Add("style", "display:block;")
                    End If
                    If Me.chkCollisionCoverage.Checked = True OrElse Me.chkOwnerOtherThanInsured.Checked = True Then
                        ddlCoverageOptions.Attributes.Add("style", "display:none;")
                        lblCoverageOptionPDL.Attributes.Add("style", "display:block;")
                    End If
                Else
                    pnlPropertyDeductible.Attributes.Add("style", "display:block;")
                End If

                If (QQHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteRvWatercraft, QuickQuoteHelperClass.QuickQuotePropertyName.RvWatercraftTypeId, ddlVehType.SelectedValue)).ToUpper() <> "BOAT MOTOR ONLY" Then
                    pnlVehCostNew.Attributes.Add("style", "display:block;")
                End If
            Case Else
                pnlPropertyDeductibleTextBox.Attributes.Add("style", "display:none;")
                pnlPropertyDeductible.Attributes.Add("style", "display:none;")
                pnlVehCostNew.Attributes.Add("style", "display:none;")
                'Case "LIABILITY ONLY" 'Added 5/14/18 for Bug 26544 MLW
                '    pnlPropertyDeductibleTextBox.Attributes.Add("style", "display:none;")
                '    pnlPropertyDeductible.Attributes.Add("style", "display:none;")
                '    pnlVehCostNew.Attributes.Add("style", "display:none;")
                'Case Else 'Updated 5/14/2018 for Bug 26544 MLW
                '    pnlPropertyDeductibleTextBox.Attributes.Add("style", "display:block;")
                '    pnlPropertyDeductible.Attributes.Add("style", "display:block;")
                '    pnlVehCostNew.Attributes.Add("style", "display:block;")
        End Select
        ' Added for task 79067 and 77972 01/25/22 BD
        If Quote.LobType = QuickQuoteObject.QuickQuoteLobType.Farm Then
            If ddlVehType.SelectedItem.Text.EqualsAny("GOLF CART") AndAlso ddlCoverageOptions.SelectedItem.Text <> "LIABILITY ONLY" Then
                ddlCoverageOptions.Attributes.Add("style", "display:block;")
                pnlPropertyDeductible.Attributes.Add("style", "display:block;")
                ddlPropertyDeductible.Attributes.Add("style", "display:block;")
            End If
        End If

    End Sub

    Protected Sub lnkClearMotor_Click(sender As Object, e As EventArgs) Handles lnkClearMotor.Click
        ClearControl()
    End Sub

    Public Overrides Sub ClearControl()
        MyBase.ClearControl()

        txtHorsepowerCCs.Text = ""
        txtMotorYear.Text = ""
        txtMotorCostNew.Text = ""
        txtMotorSerialNum.Text = ""
        txtMotorMake.Text = ""
        txtMotorModel.Text = ""
    End Sub

    Private Function AreApplicantAndOperatorTheSame(myOperator As QuickQuoteOperator, myApplicant As QuickQuoteApplicant) As Boolean
        Dim returnVar As Boolean = False

        If myOperator IsNot Nothing AndAlso myOperator.Name IsNot Nothing AndAlso myApplicant IsNot Nothing AndAlso myApplicant.Name IsNot Nothing Then
            If myOperator.Name.FirstName.Equals(myApplicant.Name.FirstName, StringComparison.OrdinalIgnoreCase) Then
                If myOperator.Name.MiddleName.Equals(myApplicant.Name.MiddleName, StringComparison.OrdinalIgnoreCase) Then
                    If myOperator.Name.LastName.Equals(myApplicant.Name.LastName, StringComparison.OrdinalIgnoreCase) Then
                        returnVar = True
                    End If
                End If
            End If
        End If

        Return returnVar
    End Function

    Protected Sub lnkSaveMotor_Click(sender As Object, e As EventArgs) Handles lnkSaveMotor.Click
        Save_FireSaveEvent()
        Populate()
    End Sub

    Protected Sub HiddenMotorButton_Click(sender As Object, e As EventArgs) Handles HiddenMotorButton.Click
        If RvWaterCraftHelper.IsRvWaterCraftMotorAvailable(Quote) Then
            ' JS Alert Message about motor change is found in VrAllLines.js under toggleRVWaterMotorType()
            MyVehicle.HorsepowerCC = String.Empty
            ' Request is to set the motortype back to None.
            ddlMotorType.SelectedValue = 0
            If MyVehicle.RvWatercraftMotors.IsLoaded Then
                MyVehicle.RvWatercraftMotors(0).Year = String.Empty
                MyVehicle.RvWatercraftMotors(0).Manufacturer = String.Empty
                MyVehicle.RvWatercraftMotors(0).Model = String.Empty
                MyVehicle.RvWatercraftMotors(0).SerialNumber = String.Empty
                MyVehicle.RvWatercraftMotors(0).CostNew = String.Empty
                MyVehicle.RvWatercraftMotors(0).MotorTypeId = 0
            End If
            RaiseEvent NewBoatMotorRequested()
        End If
    End Sub

End Class