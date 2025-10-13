Imports QuickQuote.CommonObjects
Imports QuickQuote.CommonMethods
Imports IFM.VR.Web.Helpers
Imports IFM.VR.Validation.ObjectValidation.PersLines.LOB.Common
Imports QuickQuote.CommonMethods.QuickQuoteHelperClass
Imports QuickQuote.CommonObjects.QuickQuoteStaticDataOption
Imports IFM.VR.Web.Helpers.WebHelper_Personal
Imports IFM.Common.InputValidation.InputHelpers
Imports IFM.PrimativeExtensions

Public Class ctlAccidentHistoryItem
    Inherits VRControlBase

    Public Event ItemRemoveRequest(index As Int32)

    Public ReadOnly Property MyDriver As QuickQuoteDriver
        Get
            If Me.Quote IsNot Nothing Then
                'updated 8/15/2018 to use GoverningStateQuote instead of Quote
                Return Me.GoverningStateQuote.Drivers.GetItemAtIndex(Me.DriverIndex)
            End If
            Return Nothing
        End Get
    End Property

    Public ReadOnly Property MyAccident As QuickQuoteLossHistoryRecord
        Get
            If Me.Quote IsNot Nothing Then
                Select Case Me.Quote.LobType
                    Case QuickQuoteObject.QuickQuoteLobType.AutoPersonal
                        If Me.MyDriver IsNot Nothing Then
                            Return Me.MyDriver.LossHistoryRecords.GetItemAtIndex(Me.AccidentIndex)
                        End If
                    Case QuickQuoteObject.QuickQuoteLobType.HomePersonal, QuickQuoteObject.QuickQuoteLobType.DwellingFirePersonal
                        Return IFM.VR.Common.Helpers.HOM.LossHistoryHelper_HOM.GetAllHOMLosses(Me.Quote).GetItemAtIndex(Me.AccidentIndex)
                    Case QuickQuoteObject.QuickQuoteLobType.CommercialBOP, QuickQuoteObject.QuickQuoteLobType.CommercialAuto, QuickQuoteObject.QuickQuoteLobType.WorkersCompensation, QuickQuoteObject.QuickQuoteLobType.CommercialGeneralLiability, QuickQuoteObject.QuickQuoteLobType.CommercialProperty, QuickQuoteObject.QuickQuoteLobType.CommercialPackage
                        'updated 8/15/2018 to use GoverningStateQuote instead of Quote
                        Return GoverningStateQuote.LossHistoryRecords.GetItemAtIndex(Me.AccidentIndex)
                End Select

            End If

            Return Nothing
        End Get
    End Property

    Public Property AccidentIndex As Int32
        Get
            Return ViewState.GetInt32("vs_accidentNum", -1)
        End Get
        Set(value As Int32)
            ViewState("vs_accidentNum") = value
        End Set
    End Property

    Public Property DriverIndex As Int32
        Get
            Return ViewState.GetInt32("vs_driverNum", -1)
        End Get
        Set(value As Int32)
            ViewState("vs_driverNum") = value
        End Set
    End Property

    Protected Overrides ReadOnly Property MyAccordionIndex As Integer ' New for accordion logic Matt A - 7/14/15
        Get
            Return Me.AccidentIndex
        End Get
    End Property

    'added 6/18/2020
    Public ReadOnly Property ShouldControlBeUsed As Boolean
        Get
            If IsOnAppPage = False AndAlso Me.Quote IsNot Nothing AndAlso Me.Quote.LobType = QuickQuoteObject.QuickQuoteLobType.AutoPersonal AndAlso QuickQuoteHelperClass.PPA_CheckDictionaryKeyToOrderClueAtQuoteRate() = True AndAlso Me.Quote.QuoteTransactionType <> QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote AndAlso Me.Quote.QuoteTransactionType <> QuickQuoteObject.QuickQuoteTransactionType.ReadOnlyImage Then
                Return False
            Else
                Return True
            End If
        End Get
    End Property

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Quote IsNot Nothing Then
            Select Case Quote.LobType
                Case QuickQuoteObject.QuickQuoteLobType.CommercialBOP, QuickQuoteObject.QuickQuoteLobType.CommercialAuto, QuickQuoteObject.QuickQuoteLobType.WorkersCompensation, QuickQuoteObject.QuickQuoteLobType.CommercialGeneralLiability, QuickQuoteObject.QuickQuoteLobType.CommercialProperty, QuickQuoteObject.QuickQuoteLobType.CommercialPackage
                    Me.lnkRemove.Attributes.Add("style", "display:none")
                    Me.txtLossDate.Enabled = True
                    Me.ddFaultIndicator.Enabled = True
                    Me.txtAmountOfLoss.Enabled = True
                    Me.ddLostType.Enabled = True
                    Exit Select
                Case Else
                    If IsOnAppPage OrElse IsQuoteEndorsement() OrElse IsQuoteReadOnly() Then
                        Me.lnkRemove.Attributes.Add("style", "display:none")
                        Me.txtLossDate.Enabled = False
                        Me.ddFaultIndicator.Enabled = False
                        Me.txtAmountOfLoss.Enabled = False
                        Me.ddLostType.Enabled = False
                    End If
                    Exit Select
            End Select
        End If
    End Sub

    Public Overrides Sub AddScriptAlways()

    End Sub

    Public Overrides Sub AddScriptWhenRendered()
        Me.VRScript.CreateDatePicker(txtLossDate.ClientID, True)
        Me.VRScript.CreateTextboxMask(txtLossDate, "99/99/9999")
    End Sub

    Public Overrides Sub LoadStaticData()
        'added IF 6/18/2020
        If ShouldControlBeUsed = True Then
            If Me.ddLostType.Items.Count = 0 Then
                Select Case Me.Quote.LobType
                    Case QuickQuoteObject.QuickQuoteLobType.AutoPersonal
                        QQHelper.LoadStaticDataOptionsDropDown(Me.ddLostType, QuickQuoteClassName.QuickQuoteLossHistoryRecord, QuickQuotePropertyName.TypeOfLossId, SortBy.TextAscending, Me.Quote.LobType)
                        QQHelper.LoadStaticDataOptionsDropDown(Me.ddFaultIndicator, QuickQuoteClassName.QuickQuoteLossHistoryRecord, QuickQuotePropertyName.LossHistoryFaultId, SortBy.TextAscending, Me.Quote.LobType)
                    Case QuickQuoteObject.QuickQuoteLobType.HomePersonal
                        QQHelper.LoadStaticDataOptionsDropDown(Me.ddLostType, QuickQuoteClassName.QuickQuoteLossHistoryRecord, QuickQuotePropertyName.TypeOfLossId, SortBy.TextAscending, Me.Quote.LobType)
                    Case QuickQuoteObject.QuickQuoteLobType.DwellingFirePersonal
                        QQHelper.LoadStaticDataOptionsDropDown(Me.ddLostType, QuickQuoteClassName.QuickQuoteLossHistoryRecord, QuickQuotePropertyName.TypeOfLossId, SortBy.TextAscending, Me.Quote.LobType)
                        Me.ddLostType.Items(Me.ddLostType.Items.IndexOf(Me.ddLostType.Items.FindByValue("146"))).Text = "FALLING OBJECTS"
                    Case QuickQuoteObject.QuickQuoteLobType.CommercialBOP, QuickQuoteObject.QuickQuoteLobType.CommercialAuto, QuickQuoteObject.QuickQuoteLobType.WorkersCompensation, QuickQuoteObject.QuickQuoteLobType.CommercialGeneralLiability, QuickQuoteObject.QuickQuoteLobType.CommercialProperty, QuickQuoteObject.QuickQuoteLobType.CommercialPackage
                        QQHelper.LoadStaticDataOptionsDropDown(Me.ddLostType, QuickQuoteClassName.QuickQuoteLossHistoryRecord, QuickQuotePropertyName.TypeOfLossId, SortBy.TextAscending, Me.Quote.LobType)
                End Select
                'do not show at fault so no need to load it

            End If
        End If
    End Sub

    Public Overrides Sub Populate()
        'added IF 6/18/2020
        If ShouldControlBeUsed = True Then
            If MyAccident IsNot Nothing Then

                LoadStaticData()

                If IsDate(MyAccident.LossDate) Then
                    Me.txtLossDate.Text = MyAccident.LossDate.ReturnEmptyIfDefaultDiamondDate
                Else
                    ' just keep whatever is there now
                End If

                If String.IsNullOrWhiteSpace(MyAccident.Amount) Then
                    Me.txtAmountOfLoss.Text = ""
                Else
                    Me.txtAmountOfLoss.Text = MyAccident.Amount.TryToFormatAsCurreny(False)
                End If

                'IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromValue(Me.ddLostType, accident.TypeOfLossId)

                'get the text in case you have to do a force
                Dim typeOfLossType As String = QQHelper.GetStaticDataTextForValue(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteLossHistoryRecord, QuickQuoteHelperClass.QuickQuotePropertyName.TypeOfLossId, MyAccident.TypeOfLossId)

                SetdropDownFromValue_ForceSeletion(Me.ddLostType, MyAccident.TypeOfLossId, typeOfLossType)

                ' do not show fault on HOM
                Select Case Me.Quote.LobType
                    Case QuickQuoteObject.QuickQuoteLobType.AutoPersonal
                        Me.trFaultRow.Visible = True
                        Me.ddFaultIndicator.SetFromValue(MyAccident.LossHistoryFaultId)
                    Case QuickQuoteObject.QuickQuoteLobType.HomePersonal, QuickQuoteObject.QuickQuoteLobType.DwellingFirePersonal
                        ' hide that table row
                        Me.trFaultRow.Visible = False
                    Case QuickQuoteObject.QuickQuoteLobType.CommercialBOP, QuickQuoteObject.QuickQuoteLobType.CommercialAuto, QuickQuoteObject.QuickQuoteLobType.WorkersCompensation, QuickQuoteObject.QuickQuoteLobType.CommercialGeneralLiability, QuickQuoteObject.QuickQuoteLobType.CommercialProperty, QuickQuoteObject.QuickQuoteLobType.CommercialPackage
                        ' hide that table row
                        Me.trFaultRow.Visible = False
                End Select

                ' Show Remove link if needed
                Select Case Quote.LobType
                    Case QuickQuoteObject.QuickQuoteLobType.CommercialAuto, QuickQuoteObject.QuickQuoteLobType.CommercialBOP, QuickQuoteObject.QuickQuoteLobType.WorkersCompensation, QuickQuoteObject.QuickQuoteLobType.CommercialGeneralLiability, QuickQuoteObject.QuickQuoteLobType.CommercialProperty, QuickQuoteObject.QuickQuoteLobType.CommercialPackage
                        Me.lnkRemove.Attributes.Add("style", "display:''")
                        Exit Select
                    Case Else
                        Me.lnkRemove.Attributes.Add("style", "display:none")
                        Exit Select
                End Select

                ' Show loss description on Commercial Lines
                Select Case Quote.LobType
                    Case QuickQuoteObject.QuickQuoteLobType.CommercialBOP, QuickQuoteObject.QuickQuoteLobType.CommercialAuto, QuickQuoteObject.QuickQuoteLobType.WorkersCompensation, QuickQuoteObject.QuickQuoteLobType.CommercialGeneralLiability, QuickQuoteObject.QuickQuoteLobType.CommercialProperty, QuickQuoteObject.QuickQuoteLobType.CommercialPackage
                        trLossDescription.Attributes.Add("style", "display:''")
                        txtLossDescription.Text = MyAccident.LossDescription
                        Exit Select
                    Case Else
                        trLossDescription.Attributes.Add("style", "display:none")
                        Exit Select
                End Select

                Me.lblAccordianHeader.Text = "Loss History #{0} - {1}".FormatIFM(Me.AccidentIndex + 1, typeOfLossType)

                If IsOnAppPage = False AndAlso Quote.QuoteTransactionType = QuickQuoteObject.QuickQuoteTransactionType.NewBusinessQuote Then
                    Select Case MyAccident.LossHistorySourceId
                        Case "1", "4", "7", "8"
                            Me.txtLossDate.Enabled = False
                            Me.txtAmountOfLoss.Enabled = False
                            Me.ddLostType.Enabled = False
                            Me.ddFaultIndicator.Enabled = False
                            Me.txtLossDescription.Enabled = False
                    End Select
                End If
            End If
        End If
    End Sub

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        'added IF 6/18/2020
        If ShouldControlBeUsed = True Then
            MyBase.ValidateControl(valArgs)
            Select Case Me.Quote.LobType
                Case QuickQuoteObject.QuickQuoteLobType.AutoPersonal
                    Me.ValidationHelper.GroupName = String.Format("Driver #{0}  - Loss History #{1}", (Me.DriverIndex + 1), (Me.AccidentIndex + 1))
                Case QuickQuoteObject.QuickQuoteLobType.HomePersonal, QuickQuoteObject.QuickQuoteLobType.DwellingFirePersonal
                    Me.ValidationHelper.GroupName = String.Format("Property - Loss History #{0}", (Me.AccidentIndex + 1))
                Case QuickQuoteObject.QuickQuoteLobType.CommercialBOP
                    Me.ValidationHelper.GroupName = String.Format("Commercial BOP  - Loss History #{0}", (Me.AccidentIndex + 1))
            End Select

            Select Case Quote.LobType
                Case QuickQuoteObject.QuickQuoteLobType.CommercialBOP, QuickQuoteObject.QuickQuoteLobType.CommercialAuto, QuickQuoteObject.QuickQuoteLobType.WorkersCompensation, QuickQuoteObject.QuickQuoteLobType.CommercialGeneralLiability, QuickQuoteObject.QuickQuoteLobType.CommercialProperty, QuickQuoteObject.QuickQuoteLobType.CommercialPackage
                    Dim accordList As List(Of VRAccordionTogglePair) = Me.MyAccordionList ' just to cache it

                    ' Type of Loss
                    Select Case Quote.LobType
                        Case QuickQuoteObject.QuickQuoteLobType.WorkersCompensation
                            ' Workers Comp doesn't have a blank loss type as the first item in the list so it's index can't be 0
                            If ddLostType.SelectedIndex < 0 Then Me.ValidationHelper.AddError(ddLostType, "Missing Loss Type", accordList)
                            Exit Select
                        Case Else
                            ' BOP & CAP have a blank loss type item as the first item in the list, so 0 is a valid index
                            If ddLostType.SelectedIndex <= 0 Then Me.ValidationHelper.AddError(ddLostType, "Missing Loss Type", accordList)
                            Exit Select
                    End Select

                    ' Loss Date
                    If txtLossDate.Text.Trim = "" OrElse Not IsDate(txtLossDate.Text) Then
                        Me.ValidationHelper.AddError(txtLossDate, "Missing Loss Date", accordList)
                    End If

                    ' Loss Amount
                    If txtAmountOfLoss.Text.Trim = "" Then
                        Me.ValidationHelper.AddError(txtAmountOfLoss, "Missing Loss Amount", accordList)
                    Else
                        If Not IsNumeric(txtAmountOfLoss.Text) OrElse CDec(txtAmountOfLoss.Text) <= 0 Then
                            Me.ValidationHelper.AddError(txtAmountOfLoss, "Invalid Amount", accordList)
                        End If
                    End If

                    ' Loss Description
                    If txtLossDescription.Text.Trim = "" Then
                        Me.ValidationHelper.AddError(txtLossDescription, "Missing Loss Description", accordList)
                    End If


                    ' Old
                    'Dim valItems = LossValidator.ValidateLoss(Me.DriverIndex, Me.AccidentIndex, Me.Quote)
                    'If valItems.Any() Then
                    '    For Each v In valItems
                    '        Select Case v.FieldId
                    '            Case LossValidator.LossType
                    '                Me.ValidationHelper.Val_BindValidationItemToControl(Me.ddLostType, v, accordList)
                    '            Case LossValidator.LossDate
                    '                Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtLossDate, v, accordList)
                    '            Case LossValidator.LossFault
                    '                Me.ValidationHelper.Val_BindValidationItemToControl(Me.ddFaultIndicator, v, accordList)
                    '            Case LossValidator.LossAmount
                    '                Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtAmountOfLoss, v, accordList)
                    '        End Select
                    '    Next
                    'End If
                    Exit Select
                Case Else
                    If IsOnAppPage = False AndAlso IsQuoteEndorsement() = False AndAlso IsQuoteReadOnly() = False Then
                        Dim accordList As List(Of VRAccordionTogglePair) = Me.MyAccordionList ' just to cache it
                        Dim valItems = LossValidator.ValidateLoss(Me.DriverIndex, Me.AccidentIndex, Me.Quote)
                        If valItems.Any() Then
                            For Each v In valItems
                                Select Case v.FieldId
                                    Case LossValidator.LossType
                                        Me.ValidationHelper.Val_BindValidationItemToControl(Me.ddLostType, v, accordList)
                                    Case LossValidator.LossDate
                                        Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtLossDate, v, accordList)
                                    Case LossValidator.LossFault
                                        Me.ValidationHelper.Val_BindValidationItemToControl(Me.ddFaultIndicator, v, accordList)
                                    Case LossValidator.LossAmount
                                        Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtAmountOfLoss, v, accordList)
                                End Select
                            Next
                        End If
                    End If
                    Exit Select
            End Select
        End If
    End Sub

    Public Overrides Function Save() As Boolean
        Dim okayToSaveLossHistoryItem As Boolean = True
        'added IF 6/18/2020
        If ShouldControlBeUsed = True Then
            'Updated 02/25/2020 for Home Endorsements Bug 44548 MLW
            If Not Me.IsQuoteEndorsement Then
                If MyAccident IsNot Nothing Then
                    Select Case MyAccident.LossHistorySourceId
                        Case "1", "4", "7", "8"
                            'Do not save
                            okayToSaveLossHistoryItem = False
                        Case Else
                            'Save
                            okayToSaveLossHistoryItem = True
                    End Select
                    If okayToSaveLossHistoryItem Then
                        MyAccident.LossDate = Me.txtLossDate.Text
                        MyAccident.TypeOfLossId = Me.ddLostType.SelectedValue
                        Select Case Me.Quote.LobType
                            Case QuickQuoteObject.QuickQuoteLobType.AutoPersonal
                                MyAccident.LossHistoryFaultId = Me.ddFaultIndicator.SelectedValue
                                Exit Select
                            Case QuickQuoteObject.QuickQuoteLobType.CommercialBOP, QuickQuoteObject.QuickQuoteLobType.CommercialAuto, QuickQuoteObject.QuickQuoteLobType.WorkersCompensation, QuickQuoteObject.QuickQuoteLobType.CommercialGeneralLiability, QuickQuoteObject.QuickQuoteLobType.CommercialProperty, QuickQuoteObject.QuickQuoteLobType.CommercialPackage
                                MyAccident.LossDescription = txtLossDescription.Text
                                Exit Select
                            Case Else
                                'do not save fault
                        End Select

                        MyAccident.Amount = Me.txtAmountOfLoss.Text

                        ' Added for Bug 28868 - MGB 4/4/2019
                        ' Determine if surcharge applies
                        Dim SurchargeApplies As Boolean = False
                        Dim LossCutoff3Yrs As DateTime = CDate(Quote.EffectiveDate).AddYears(-3).Date
                        ' Must be within 3 years of effective date
                        If IsDate(MyAccident?.LossDate) AndAlso CDate(MyAccident?.LossDate) >= LossCutoff3Yrs Then
                            ' Loss amount must be greater than $1000
                            If CDec(MyAccident.Amount) > 1000 Then
                                ' Must be type PD Liability(2), Bodily Injury(1), CSL (not in VR), Collision(4), or Med Pay(3)
                                If MyAccident.TypeOfLossId = 1 OrElse MyAccident.TypeOfLossId = 2 OrElse MyAccident.TypeOfLossId = 3 OrElse MyAccident.TypeOfLossId = 4 Then
                                    SurchargeApplies = True
                                End If
                            End If
                        End If

                        If SurchargeApplies Then
                            MyAccident.LossHistorySourceId = 3    ' Manual
                            MyAccident.LossHistorySurchargeId = 1 ' Surcharge
                        Else
                            MyAccident.LossHistorySourceId = 0    ' N/A
                            MyAccident.LossHistorySurchargeId = 2 ' No Surcharge
                        End If
                    End If
                    Return True
                End If
            End If
        End If
        Return False
    End Function

    Protected Sub lnkRemove_Click(sender As Object, e As EventArgs) Handles lnkRemove.Click
        Select Case Quote.LobType
            Case QuickQuoteObject.QuickQuoteLobType.CommercialBOP, QuickQuoteObject.QuickQuoteLobType.CommercialAuto, QuickQuoteObject.QuickQuoteLobType.WorkersCompensation, QuickQuoteObject.QuickQuoteLobType.CommercialGeneralLiability, QuickQuoteObject.QuickQuoteLobType.CommercialProperty, QuickQuoteObject.QuickQuoteLobType.CommercialPackage
                If Me.Quote IsNot Nothing Then
                    'Save state as it is now - remove - then re-populate
                    Me.Save_FireSaveEvent(New VrControlBaseSaveEventArgs(Me, False, False, New IFM.VR.Web.VRValidationArgs(DefaultValidationType))) ' will not save to database it will be saved later
                    'need to decide where this loss record is so you delete the right index from the right list
                    Dim policyLevelLossesCount As Int32 = 0
                    'updated 8/17/2018 to use GoverningStateQuote instead of Quote
                    If Me.GoverningStateQuote.LossHistoryRecords IsNot Nothing Then
                        policyLevelLossesCount = Me.GoverningStateQuote.LossHistoryRecords.Count
                    End If
                    If Me.AccidentIndex < policyLevelLossesCount Then
                        ' you have your list and index
                        'updated 8/17/2018 to use GoverningStateQuote instead of Quote
                        Me.GoverningStateQuote.LossHistoryRecords.RemoveAt(AccidentIndex)
                        RaiseEvent ItemRemoveRequest(Me.AccidentIndex)
                    End If
                End If
                Exit Select
            Case Else
                If IsOnAppPage = False Then
                    If Me.Quote IsNot Nothing Then
                        'Save state as it is now - remove - then re-populate
                        Me.Save_FireSaveEvent(New VrControlBaseSaveEventArgs(Me, False, False, New IFM.VR.Web.VRValidationArgs(DefaultValidationType))) ' will not save to database it will be saved later
                        Select Case Me.Quote.LobType
                            Case QuickQuoteObject.QuickQuoteLobType.AutoPersonal
                                'updated 8/17/2018 to use GoverningStateQuote instead of Quote
                                If Me.GoverningStateQuote.Drivers IsNot Nothing Then
                                    Me.GoverningStateQuote.Drivers(Me.DriverIndex).LossHistoryRecords.RemoveAt(AccidentIndex)
                                    RaiseEvent ItemRemoveRequest(Me.AccidentIndex)
                                End If
                            Case QuickQuoteObject.QuickQuoteLobType.HomePersonal, QuickQuoteObject.QuickQuoteLobType.DwellingFirePersonal
                                'need to decide where this loss record is so you delete the right index from the right list
                                Dim policyLevelLossesCount As Int32 = 0
                                'updated 8/17/2018 to use GoverningStateQuote instead of Quote
                                If Me.GoverningStateQuote.LossHistoryRecords IsNot Nothing Then
                                    policyLevelLossesCount = Me.GoverningStateQuote.LossHistoryRecords.Count
                                End If
                                If Me.AccidentIndex < policyLevelLossesCount Then
                                    ' you have your list and index
                                    'updated 8/17/2018 to use GoverningStateQuote instead of Quote
                                    Me.GoverningStateQuote.LossHistoryRecords.RemoveAt(AccidentIndex)
                                    RaiseEvent ItemRemoveRequest(Me.AccidentIndex)
                                Else
                                    ' keep going
                                    'updated 8/17/2018 to use GoverningStateQuote instead of Quote
                                    If Me.GoverningStateQuote.Applicants IsNot Nothing Then
                                        Dim appNum1LossCount As Int32 = 0
                                        If Me.GoverningStateQuote.Applicants.Count > 0 Then
                                            If Me.GoverningStateQuote.Applicants(0).LossHistoryRecords IsNot Nothing Then
                                                appNum1LossCount = Me.GoverningStateQuote.Applicants(0).LossHistoryRecords.Count()
                                            End If
                                        End If
                                        Dim appNum2LossCount As Int32 = 0
                                        'updated 8/17/2018 to use GoverningStateQuote instead of Quote
                                        If Me.GoverningStateQuote.Applicants.Count > 1 Then
                                            If Me.GoverningStateQuote.Applicants(1).LossHistoryRecords IsNot Nothing Then
                                                appNum2LossCount = Me.GoverningStateQuote.Applicants(1).LossHistoryRecords.Count()
                                            End If
                                        End If

                                        If AccidentIndex < policyLevelLossesCount + appNum1LossCount Then
                                            ' is in the applicant #1 list
                                            Dim ri As Int32 = appNum1LossCount - ((policyLevelLossesCount + appNum1LossCount) - AccidentIndex)
                                            'updated 8/17/2018 to use GoverningStateQuote instead of Quote
                                            Me.GoverningStateQuote.Applicants(0).LossHistoryRecords.RemoveAt(ri)
                                            RaiseEvent ItemRemoveRequest(Me.AccidentIndex)
                                        Else
                                            If AccidentIndex < policyLevelLossesCount + appNum1LossCount + appNum2LossCount Then
                                                ' is in the applicant #2 list
                                                Dim removeIndex As Int32 = appNum2LossCount - ((policyLevelLossesCount + appNum1LossCount + appNum2LossCount) - AccidentIndex)
                                                'updated 8/17/2018 to use GoverningStateQuote instead of Quote
                                                Me.GoverningStateQuote.Applicants(1).LossHistoryRecords.RemoveAt(removeIndex)
                                                RaiseEvent ItemRemoveRequest(Me.AccidentIndex)
                                            Else
                                                ' you got me it ;) - should be less than this
                                            End If
                                        End If

                                    End If
                                End If
                        End Select
                    End If
                End If
                Exit Select
        End Select


    End Sub

    Public Overrides Sub ClearControl()
        If Me.txtAmountOfLoss.Enabled Then
            Me.txtAmountOfLoss.Text = ""
        End If
        If Me.txtLossDate.Enabled Then
            Me.txtLossDate.Text = ""
        End If
        If Me.ddFaultIndicator.Enabled Then
            Me.ddFaultIndicator.SelectedIndex = -1
        End If
        If Me.ddLostType.Enabled Then
            Me.ddLostType.SelectedIndex = -1
        End If
        If Me.txtLossDescription.Enabled Then
            Me.txtLossDescription.Text = ""
        End If
        MyBase.ClearControl()
    End Sub

End Class