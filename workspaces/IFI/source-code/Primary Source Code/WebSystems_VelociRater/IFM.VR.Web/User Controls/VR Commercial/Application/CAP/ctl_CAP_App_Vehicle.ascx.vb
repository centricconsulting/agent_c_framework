Imports QuickQuote.CommonObjects
Imports QuickQuote.CommonMethods
Imports IFM.VR.Web.Helpers
Imports IFM.VR.Validation.ObjectValidation.PersLines.LOB.PPA
Imports QuickQuote.CommonMethods.QuickQuoteHelperClass
Imports QuickQuote.CommonObjects.QuickQuoteStaticDataOption
Imports QuickQuote.CommonObjects.QuickQuoteObject
Imports IFM.VR.Web.Helpers.WebHelper_Personal
Imports IFM.Common.InputValidation.InputHelpers
Imports IFM.PrimativeExtensions
Imports IFM.VR.Common.Helpers.CAP

Public Class ctl_CAP_App_Vehicle
    Inherits VRControlBase

    Public Event VehicleChanged(index As Int32)

    Public Event NeedToRepopulateTopLevelAIs() 'added 6/14/2021 for CAP Endorsements Task 52974 MLW

    Public ReadOnly CAPEndorsementsDictionaryName = "CAPEndorsementsDetails" 'Added 06/01/2021 for CAP Endorsements Task 52974 MLW
    'Added 06/01/2021 for CAP Endorsements Task 52974 MLW
    Private Property _devDictionaryHelper As DevDictionaryHelper.DevDictionaryHelper
    Public ReadOnly Property ddh() As DevDictionaryHelper.DevDictionaryHelper
        Get
            If _devDictionaryHelper Is Nothing Then
                If Quote IsNot Nothing AndAlso String.IsNullOrWhiteSpace(CAPEndorsementsDictionaryName) = False Then
                    _devDictionaryHelper = New DevDictionaryHelper.DevDictionaryHelper(Quote, CAPEndorsementsDictionaryName, Quote.LobType)
                End If
            End If
            Return _devDictionaryHelper
        End Get
    End Property

    Public ReadOnly Property MyVehicle As QuickQuoteVehicle
        Get
            If Me.Quote IsNot Nothing Then
                Return Me.Quote.Vehicles.GetItemAtIndex(Me.VehicleIndex)
            End If
            Return Nothing
        End Get
    End Property

    Public Property VehicleIndex As Int32
        Get
            If ViewState("vs_vehicleNum") Is Nothing Then
                ViewState("vs_vehicleNum") = -1
            End If
            Return CInt(ViewState("vs_vehicleNum"))
        End Get
        Set(value As Int32)
            ViewState("vs_vehicleNum") = value
        End Set
    End Property

    Protected Overrides ReadOnly Property MyAccordionIndex As Integer ' New for accordion logic Matt A - 7/14/15
        Get
            Return Me.VehicleIndex
        End Get
    End Property

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            LoadStaticData()
        End If
    End Sub

    Public Overrides Sub AddScriptAlways()

    End Sub

    Public Overrides Sub AddScriptWhenRendered()
        Me.VRScript.StopEventPropagation(lnkSave.ClientID)
        Me.VRScript.CreateAccordion(Me.divCAP.ClientID, Me.hdnAccord, "0")

        'Added 07/22/2021 for CAP Endorsements Task 53030 MLW
        If IsOnAppPage() Then
            Dim versionId As String = Quote.VersionId
            Dim policyId As String = Quote.PolicyId
            Dim policyImageNumber As String = Quote.PolicyImageNum
            Dim vehicleNum As String = "0"
            If MyVehicle IsNot Nothing Then
                vehicleNum = MyVehicle.VehicleNum
            End If
            Dim vrVehicleNum As String = VehicleIndex + 1 'Added 08/23/2021 for Bug 64413 MLW
            Dim isRACASymbolAvailable As Boolean = RACASymbolHelper.IsRACASymbolsAvailable(Quote)
            Me.VRScript.CreateJSBinding(Me.txtVinNumber, ctlPageStartupScript.JsEventType.onchange, "Cap.AppGapVINLookup('" & Me.txtVinNumber.ClientID & "','" & Me.txtVinNumber.ClientID & "','" & policyId & "','" & policyImageNumber & "','" & vehicleNum & "','" & vrVehicleNum & "','" & versionId & "','" & divVinLookupValidation.ClientID & "','" & hdnVehicleYear.ClientID & "','" & hdnVehicleMake.ClientID & "','" & hdnVehicleModel.ClientID & "','" & hdnVehicleSize.ClientID & "','" & hdnVehicleCostNew.ClientID & "','" & hdnVehicleClassCode.ClientID & "','" & hdnValidVin.ClientID & "','" & hdnRateType.ClientID & "','" & hdnUseCode.ClientID & "','" & hdnOperator.ClientID & "','" & hdnOperatorType.ClientID & "','" & hdnTrailerType.ClientID & "','" & hdnRadius.ClientID & "','" & hdnSecClass.ClientID & "','" & hdnSecClassType.ClientID & "','" & isRACASymbolAvailable.ToString() & "','" & hdnOtherThanCollisionSymbol.ClientID & "','" & hdnCollisionSymbol.ClientID & "','" & hdnLiabilitySymbol.ClientID & "','" & hdnOtherThanCollisionOverride.ClientID & "','" & hdnCollisionOverride.ClientID & "','" & hdnLiabilityOverride.ClientID + "'); return false;")

            ' The OK button on the VIN Lookup Validation popup
            btnVLVOK.Attributes.Add("onclick", "Cap.CloseVINValidationPopup('" & divVinLookupValidation.ClientID & "'); return false;")
        End If
    End Sub

    Public Overrides Sub LoadStaticData()
        'Removed 08/11/2021 for Bug 63943 MLW - causes selection of drop down to not work, defaults to first item every time regardless of having an AI.TypeId
        ''Added 02/22/2021 for CAP Endorsements Task 52974 MLW
        'QQHelper.LoadStaticDataOptionsDropDown(Me.ddlLossPayeeType, QuickQuoteClassName.QuickQuoteAdditionalInterest, QuickQuotePropertyName.TypeId, , Me.Quote.LobType)
    End Sub

    Public Overrides Sub Populate()
        LoadStaticData() 'Added 02/22/2021 for CAP Endorsements Task 52974 MLW

        'Updated 12/22/2020 for CAP Endorsements Task 52972 MLW 52974 MLW
        If IsOnAppPage OrElse IsQuoteReadOnly() OrElse (IsQuoteEndorsement() AndAlso (TypeOfEndorsement() = "Add/Delete Vehicle" OrElse TypeOfEndorsement() = "Add/Delete Additional Interest")) Then
            If MyVehicle IsNot Nothing Then
                LoadLossPayeeDDL()
                'Updated 05/05/2021 for CAP Endorsements bug 61606 MLW
                If IsQuoteReadOnly() OrElse IsQuoteEndorsement() Then
                    Dim txt As String
                    Dim lastFourVin As String = MyVehicle.Vin.ToUpper
                    If Not String.IsNullOrWhiteSpace(MyVehicle.Vin) AndAlso MyVehicle.Vin.Length > 4 Then
                        lastFourVin = MyVehicle.Vin.Substring(MyVehicle.Vin.Length - 4).ToUpper()
                    End If
                    txt = MyVehicle.Year & " " & MyVehicle.Make & " " & MyVehicle.Model & " " & lastFourVin
                    If IsNullEmptyorWhitespace(txt) Then
                        txt = "0"
                    End If
                    lblAccordHeader.Text = txt
                Else
                    lblAccordHeader.Text = "Vehicle #" & VehicleIndex + 1.ToString & " " & MyVehicle.Year & " " & MyVehicle.Make & " " & MyVehicle.Model
                End If
                'lblAccordHeader.Text = "Vehicle #" & VehicleIndex + 1.ToString & " " & MyVehicle.Year & " " & MyVehicle.Make & " " & MyVehicle.Model

                txtVinNumber.Text = MyVehicle.Vin

                Dim ableToFindAiInDropdown As Boolean = False 'added 5/23/2021 for CAP Endorsements Task 52974 MLW
                If MyVehicle.AdditionalInterests IsNot Nothing AndAlso MyVehicle.AdditionalInterests.Count > 0 Then
                    Dim AI As QuickQuote.CommonObjects.QuickQuoteAdditionalInterest = MyVehicle.AdditionalInterests(0)
                    If ddlLossPayeeName.Items IsNot Nothing AndAlso ddlLossPayeeName.Items.Count > 0 AndAlso ddlLossPayeeName.Items.FindByValue(AI.ListId) IsNot Nothing Then 'added IF 5/23/2021
                        ableToFindAiInDropdown = True 'added 5/23/2021 for CAP Endorsements Task 52974 MLW
                        ddlLossPayeeName.SelectedValue = AI.ListId
                        'Updated 02/22/2021 for CAP Endorsements Task 52974 MLW
                        If IsQuoteReadOnly() OrElse IsQuoteEndorsement() Then
                            IFM.VR.Web.Helpers.WebHelper_Personal.SetDropDownValue_ForceDiamondValue(ddlLossPayeeType, AI.TypeId, QuickQuoteClassName.QuickQuoteAdditionalInterest, QuickQuotePropertyName.TypeId)
                        Else
                            ddlLossPayeeType.SelectedValue = AI.TypeId
                        End If
                        'ddlLossPayeeType.SelectedValue = AI.TypeId
                        If AI.ATIMA = False AndAlso AI.ISAOA = False Then
                            ddlATIMA.SelectedValue = 0
                        ElseIf AI.ATIMA = True AndAlso AI.ISAOA = False Then
                            ddlATIMA.SelectedValue = 1
                        ElseIf AI.ATIMA = False AndAlso AI.ISAOA = True Then
                            ddlATIMA.SelectedValue = 2
                        ElseIf AI.ATIMA = True AndAlso AI.ISAOA = True Then
                            ddlATIMA.SelectedValue = 3
                        End If
                    End If
                End If
                If ableToFindAiInDropdown = False Then 'added 5/23/2021 for CAP Endorsements Task 52974 MLW
                    'note: either vehicle didn't have AIs to begin with or AI was likely just removed at top-level; just reset values
                    ddlLossPayeeName.ClearSelection()
                    ddlLossPayeeType.ClearSelection()
                    ddlATIMA.ClearSelection()
                End If
            End If

            'Added 12/08/2020 for CAP Endorsements Task 52974 MLW
            If IsQuoteEndorsement() Then
                txtVinNumber.Enabled = False
                If Not IsNewVehicleOnEndorsement(MyVehicle) AndAlso TypeOfEndorsement() <> "Add/Delete Additional Interest" Then
                    lnkSave.Visible = False
                    ddlLossPayeeName.Enabled = False
                    ddlLossPayeeType.Enabled = False
                    ddlATIMA.Enabled = False
                ElseIf TypeOfEndorsement() = "Add/Delete Additional Interest" Then
                    txtVinNumber.Enabled = False
                    Dim transactionCount As Integer = ddh.GetEndorsementAdditionalInterestTransactionCount()
                    If transactionCount >= 3 Then
                        Dim hasEndorsementAIVehicleAssignmentChange As Boolean = DoesEndorsementHaveAIVehicleAssignmentChange(MyVehicle.DisplayNum)
                        'Want to allow vehicles who's AI assignment has changed during the endorsement quote to be unlocked so they can undo the assignment if they want. All other vehicles have their options locked once the max AI transactions reach 3.
                        If hasEndorsementAIVehicleAssignmentChange = False Then
                            ddlLossPayeeName.Enabled = False
                            ddlLossPayeeType.Enabled = False
                            ddlATIMA.Enabled = False
                        End If
                    Else
                        ddlLossPayeeName.Enabled = True
                        ddlLossPayeeType.Enabled = True
                        ddlATIMA.Enabled = True
                    End If
                End If
            End If
            'Added 07/07/2021 for CAP Endorsements task 53028 MLW
            If IsQuoteReadOnly() OrElse IsQuoteEndorsement() Then
                trVinMessage.Visible = False
            End If
            'Added 08/02/2021 for CAP Endorsements Tasks 53030 MLW
            If IsOnAppPage Then
                hdnVehicleNum.Value = MyVehicle.VehicleNum
                hdnVehicleYear.Value = MyVehicle.Year
                hdnVehicleMake.Value = MyVehicle.Make
                hdnVehicleModel.Value = MyVehicle.Model
                hdnVehicleSize.Value = MyVehicle.SizeTypeId
                hdnVehicleCostNew.Value = MyVehicle.CostNew
                hdnRateType.Value = MyVehicle.VehicleRatingTypeId
                hdnUseCode.Value = MyVehicle.UseCodeTypeId
                hdnOperator.Value = MyVehicle.OperatorTypeId
                hdnOperatorType.Value = MyVehicle.OperatorUseTypeId
                hdnTrailerType.Value = MyVehicle.TrailerTypeId
                hdnRadius.Value = MyVehicle.RadiusTypeId
                hdnSecClass.Value = MyVehicle.SecondaryClassTypeId
                hdnSecClassType.Value = MyVehicle.SecondaryClassUsageTypeId
                hdnVehicleClassCode.Value = MyVehicle.ClassCode
                hdnValidVin.Value = GetValidVinValue(VehicleIndex)
            End If
            PopulateRACASymbols()
        End If
        Exit Sub
    End Sub

    Public Sub PopulateRACASymbols()
        If RACASymbolHelper.IsRACASymbolsAvailable(Quote) AndAlso Quote.QuoteTransactionType = QuickQuoteObject.QuickQuoteTransactionType.NewBusinessQuote Then
            If MyVehicle.VehicleSymbols IsNot Nothing AndAlso MyVehicle.VehicleSymbols.Count > 1 Then
                PopulateRACASymbolsByType("1", Me.hdnOtherThanCollisionSymbol, Me.hdnOtherThanCollisionOverride)
                PopulateRACASymbolsByType("2", Me.hdnCollisionSymbol, Me.hdnCollisionOverride)
                PopulateRACASymbolsByType("3", Me.hdnLiabilitySymbol, Me.hdnLiabilityOverride)
            End If
        End If
    End Sub

    Public Sub PopulateRACASymbolsByType(typeId As String, hdnSymbol As HiddenField, hdnOverride As HiddenField)
        Dim symbol As String = (From s In MyVehicle.VehicleSymbols Where s.VehicleSymbolCoverageTypeId = typeId Select s.SystemGeneratedSymbol).FirstOrDefault()
        Dim symbolOverride As String = (From s In MyVehicle.VehicleSymbols Where s.VehicleSymbolCoverageTypeId = typeId Select s.UserOverrideSymbol).FirstOrDefault()
        If symbol IsNot Nothing Then
            hdnSymbol.Value = symbol.Trim()
        End If
        If symbolOverride IsNot Nothing Then
            hdnOverride.Value = symbolOverride.Trim()
        End If
    End Sub

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        'Updated 12/22/2020 for CAP Endorsements Task 52972 MLW
        If IsOnAppPage OrElse (IsQuoteEndorsement() AndAlso TypeOfEndorsement() = "Add/Delete Vehicle" AndAlso IsNewVehicleOnEndorsement(MyVehicle)) Then
            MyBase.ValidateControl(valArgs)
            'Updated 06/02/2021 for CAP Endorsements Task 52974 MLW
            If IsQuoteEndorsement() Then
                If IsNullEmptyorWhitespace(MyVehicle.Year) AndAlso IsNullEmptyorWhitespace(MyVehicle.Make) AndAlso IsNullEmptyorWhitespace(MyVehicle.Model) AndAlso IsNullEmptyorWhitespace(MyVehicle.Vin) Then
                    Me.ValidationHelper.GroupName = "Vehicle 0"
                Else
                    Me.ValidationHelper.GroupName = String.Format("{0} {1} {2} {3}", MyVehicle.Year, MyVehicle.Make, MyVehicle.Model, MyVehicle.Vin)
                End If
            Else
                Me.ValidationHelper.GroupName = String.Format("Vehicle #{0}", Me.VehicleIndex + 1)
            End If
            'Me.ValidationHelper.GroupName = String.Format("Vehicle #{0}", Me.VehicleIndex + 1)

            Dim accordList As List(Of VRAccordionTogglePair) = Me.MyAccordionList ' just to cache it

            'VIN Validation - Added 07/08/2021 for CAP Endorsements Tasks 53028 and 53030 MLW
            If Not IsQuoteEndorsement() Then
                Dim valItems = Validation.ObjectValidation.CommLines.LOB.CAP.VINValidator.VINValidation(Me.VehicleIndex, Me.Quote, valArgs.ValidationType)
                If valItems.Any() Then
                    For Each v In valItems
                        Select Case v.FieldId
                            Case Validation.ObjectValidation.CommLines.LOB.CAP.VINValidator.VehicleVIN
                                Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtVinNumber, v, accordList)
                        End Select
                    Next
                End If
            End If
            'Moved to VINValidator 07/08/2021 for CAP Endorsements Task 53028 and 53030 MLW            
            'If txtVinNumber.Text.Trim = "" Then
            '    Me.ValidationHelper.AddError(txtVinNumber, "Missing VIN", accordList)
            'End If

            'Added 08/05/2021 for CAP Endorsements Task 53030 MLW
            If IsOnAppPage AndAlso hdnVehicleClassCode.Value.Trim = "" Then
                Me.ValidationHelper.AddError(txtVinNumber, "Invalid class code due to VIN Lookup", accordList)
            End If
        End If
    End Sub

    Public Overrides Function Save() As Boolean
        'Updated 12/22/2020 for CAP Endorsements Task 52972 MLW
        If IsOnAppPage OrElse (IsQuoteEndorsement() AndAlso ((TypeOfEndorsement() = "Add/Delete Vehicle" AndAlso IsNewVehicleOnEndorsement(MyVehicle)) OrElse TypeOfEndorsement() = "Add/Delete Additional Interest")) Then
            If MyVehicle IsNot Nothing Then
                Dim removedCopiedAiFromTopLevel As Boolean = False 'added 6/14/2021 for CAP Endorsements Task 52974 MLW

                'Updated 07/08/2021 for CAP Endorsements Task 53028 and 53030 MLW
                If Not IsQuoteEndorsement() Then
                    MyVehicle.Vin = txtVinNumber.Text
                End If
                'MyVehicle.Vin = txtVinNumber.Text

                'Added 08/02/2021 for CAP Endorsements Tasks 53030 MLW
                If IsOnAppPage Then
                    MyVehicle.Year = hdnVehicleYear.Value
                    MyVehicle.Make = hdnVehicleMake.Value
                    MyVehicle.Model = hdnVehicleModel.Value
                    MyVehicle.SizeTypeId = hdnVehicleSize.Value
                    MyVehicle.CostNew = hdnVehicleCostNew.Value
                    MyVehicle.VehicleRatingTypeId = hdnRateType.Value
                    MyVehicle.UseCodeTypeId = hdnUseCode.Value
                    MyVehicle.OperatorTypeId = hdnOperator.Value
                    MyVehicle.OperatorUseTypeId = hdnOperatorType.Value
                    MyVehicle.TrailerTypeId = hdnTrailerType.Value
                    MyVehicle.RadiusTypeId = hdnRadius.Value
                    MyVehicle.SecondaryClassTypeId = hdnSecClass.Value
                    MyVehicle.SecondaryClassUsageTypeId = hdnSecClassType.Value
                    MyVehicle.ClassCode = hdnVehicleClassCode.Value
                    'save ValidVin to DevDictionary - indicates that the VIN lookup was used and that the VIN used in the lookup was a valid VIN. If not a valid VIN the cost new and size fields remain enabled, user will not be able to rate and a message to use the VIN lookup will appear.
                    'This is for new business and endorsements, using VR vehicleNum, value not stored in Diamond
                    Dim vinDDH As DevDictionaryHelper.DevDictionaryHelper = New DevDictionaryHelper.DevDictionaryHelper(Quote, "ValidVIN", Quote.LobType)
                    vinDDH.AddToMasterValueDictionary(VehicleIndex + 1, hdnValidVin.Value)
                End If

                If RACASymbolHelper.IsRACASymbolsAvailable(Quote) AndAlso Quote.QuoteTransactionType = QuickQuoteObject.QuickQuoteTransactionType.NewBusinessQuote Then
                    If MyVehicle.VehicleSymbols Is Nothing Then
                        MyVehicle.VehicleSymbols = New List(Of QuickQuoteVehicleSymbol)
                    End If
                    RACASymbolHelper.AddUpdateRACASymbols(Me.MyVehicle, "1", Me.hdnOtherThanCollisionSymbol.Value, Me.hdnOtherThanCollisionOverride.Value)
                    RACASymbolHelper.AddUpdateRACASymbols(Me.MyVehicle, "2", Me.hdnCollisionSymbol.Value, Me.hdnCollisionOverride.Value)
                    RACASymbolHelper.AddUpdateRACASymbols(Me.MyVehicle, "3", Me.hdnLiabilitySymbol.Value, Me.hdnLiabilityOverride.Value)
                End If

                Dim updateEndorsementRemarks As Boolean = False 'added 06/07/2021 for CAP Endorsements Task 52974 MLW
                Dim repopulateAIControl As Boolean = False 'added 06/24/2021 for CAP Endorsements Task 52974 MLW
                If ddlLossPayeeName.SelectedIndex > 0 Then
                    If MyVehicle.AdditionalInterests Is Nothing Then
                        MyVehicle.AdditionalInterests = New List(Of QuickQuoteAdditionalInterest)
                    End If
                    Dim aiToUse As QuickQuoteAdditionalInterest = Nothing
                    If MyVehicle.AdditionalInterests.Count = 0 Then
                        aiToUse = New QuickQuoteAdditionalInterest
                        MyVehicle.AdditionalInterests.Add(aiToUse)
                    End If
                    If MyVehicle.AdditionalInterests.Count > 0 Then
                        If aiToUse Is Nothing Then
                            aiToUse = MyVehicle.AdditionalInterests(0)
                        End If
                        Dim isSameListId As Boolean = False
                        With aiToUse
                            If .ListId = Me.ddlLossPayeeName.SelectedValue Then
                                isSameListId = True
                            Else
                                isSameListId = False 'redundant
                            End If
                            .ListId = Me.ddlLossPayeeName.SelectedValue
                            .TypeId = Me.ddlLossPayeeType.SelectedValue
                            Select Case Me.ddlATIMA.SelectedValue
                                Case "3"
                                    .ATIMA = True
                                    .ISAOA = True
                                Case "1"
                                    .ATIMA = True
                                    .ISAOA = False
                                Case "2"
                                    .ISAOA = True
                                    .ATIMA = False
                                Case Else '0
                                    .ATIMA = False
                                    .ISAOA = False
                            End Select
                            'If isSameListId = False Then 'will just run every time
                            Dim sourceAI As QuickQuoteAdditionalInterest = Nothing 'Added 3/25/2021 for CAP Endorsements task 52974 MLW
                            'If Me.Quote IsNot Nothing AndAlso Me.Quote.AdditionalInterests IsNot Nothing AndAlso Me.Quote.AdditionalInterests.Count > 0 Then
                            '    'Dim sourceAI As QuickQuoteAdditionalInterest = QuickQuoteHelperClass.QuickQuoteAdditionalInterestForListId(Me.Quote.AdditionalInterests, .ListId, cloneAI:=False, firstOrLastItem:=QuickQuoteHelperClass.FirstOrLast.First, matchType:=QuickQuoteHelperClass.TextMatchType.IntegerOrText_IgnoreCasing, returnNewIfNothing:=False)
                            '    'Updated 3/25/2021 for CAP Endorsements task 52974 MLW
                            '    sourceAI = QuickQuoteHelperClass.QuickQuoteAdditionalInterestForListId(Me.Quote.AdditionalInterests, .ListId, cloneAI:=False, firstOrLastItem:=QuickQuoteHelperClass.FirstOrLast.First, matchType:=QuickQuoteHelperClass.TextMatchType.IntegerOrText_IgnoreCasing, returnNewIfNothing:=False)
                            '    'If sourceAI IsNot Nothing Then 'moved below 3/25/2021
                            '    '    QQHelper.CopyQuickQuoteAdditionalInterestNameAddressEmailsAndPhones(sourceAI, aiToUse)
                            '    'End If
                            'End If
                            'updated 5/23/2021 to just use MyAiList property
                            Dim aiList As List(Of QuickQuoteAdditionalInterest) = MyAiList
                            If aiList IsNot Nothing AndAlso aiList.Count > 0 Then
                                sourceAI = QuickQuoteHelperClass.QuickQuoteAdditionalInterestForListId(aiList, .ListId, cloneAI:=False, firstOrLastItem:=QuickQuoteHelperClass.FirstOrLast.First, matchType:=QuickQuoteHelperClass.TextMatchType.IntegerOrText_IgnoreCasing, returnNewIfNothing:=False)
                            End If
                            'Added 3/25/2021 for CAP Endorsements task 52974 MLW; removed 5/23/2021 - if it's not in the top-level list, then it's just been deleted
                            'If sourceAI Is Nothing Then
                            '    If Me.Quote.Vehicles IsNot Nothing AndAlso Me.Quote.Vehicles.Count > 0 Then
                            '        For Each v As QuickQuoteVehicle In Me.Quote.Vehicles
                            '            If v IsNot Nothing AndAlso v.AdditionalInterests IsNot Nothing AndAlso v.AdditionalInterests.Count > 0 Then
                            '                sourceAI = QuickQuoteHelperClass.QuickQuoteAdditionalInterestForListId(v.AdditionalInterests, .ListId, cloneAI:=False, firstOrLastItem:=QuickQuoteHelperClass.FirstOrLast.First, matchType:=QuickQuoteHelperClass.TextMatchType.IntegerOrText_IgnoreCasing, returnNewIfNothing:=False)
                            '                If sourceAI IsNot Nothing Then
                            '                    Exit For
                            '                End If
                            '            End If
                            '        Next
                            '    End If
                            'End If
                            If sourceAI IsNot Nothing Then
                                QQHelper.CopyQuickQuoteAdditionalInterestNameAddressEmailsAndPhones(sourceAI, aiToUse)

                                'Added 05/19/2021 for CAP Endorsements task 52974 MLW
                                'DevDictionary tracks the add and delete transactions. Used to find transaction counts (max is 3) and formulate remarks in tree/Dec.
                                'Add transactions count when AI added to the master AI list in the AI control or in this control as an AI assigned to vehicle.
                                'Delete transactions count when an AI is deleted from the master AI list in the AI control or in this control when an AI is unassigned from a vehicle.
                                If isSameListId = False AndAlso (IsQuoteEndorsement() AndAlso TypeOfEndorsement() = "Add/Delete Additional Interest") Then
                                    'This is an AI Add entry to the DevDictionary.
                                    'There is a new AI listId assigned to the vehicle.

                                    Dim MyAdditionalInterest As QuickQuoteAdditionalInterest = MyVehicle.AdditionalInterests(0)
                                    Dim vrVehicleNum As String = VehicleIndex + 1
                                    Dim diaVehicleNum As String = MyVehicle.DisplayNum
                                    If String.IsNullOrWhiteSpace(diaVehicleNum) OrElse diaVehicleNum.Contains("-") Then
                                        diaVehicleNum = vrVehicleNum
                                    End If

                                    'When the listId changes (isSameListId = False), the user either assigned a new AI (from N/A - unassigned) to the vehicle or switched it (from one AI to another).
                                    'Handling of unassigning an AI (setting drop down to N/A) is in the Else statement for the ddlLossPayeeName.SelectedIndex > 0 (if statement at top)
                                    'Remove previously deleted AI/Veh assignment for assigned AI
                                    If RemovedPreviousAIVehicleAssignmentByAIListIdAndListType(MyAdditionalInterest, diaVehicleNum, DevDictionaryHelper.DevDictionaryHelper.deleteItem) Then
                                        updateEndorsementRemarks = True
                                    End If

                                    'When switching a vehicle's assigned AI, we need to have a DevDictionary AI delete entry for the unassigned AI.
                                    'had AI assigned to vehicle when endorsement created, find it & add to delete list
                                    Dim aiNumOriginallyAssignedToVehicle As String = PolicyAIAssignedToVehicle(diaVehicleNum)
                                    If aiNumOriginallyAssignedToVehicle <> "" AndAlso aiNumOriginallyAssignedToVehicle <> MyAdditionalInterest.ListId Then
                                        'Add AI delete record for old AI
                                        If SetOriginallyAssignedAIToEndorsementDelete(aiNumOriginallyAssignedToVehicle, diaVehicleNum) = True Then
                                            updateEndorsementRemarks = True
                                        End If

                                        'Remove any previously added AI/Veh assignment for vehicle - can only have one AI/Vehicle association at a time. Will add new assignment next.
                                        If RemovedAnyPreviouslyAddedAIVehicleEndorsementAssigments(diaVehicleNum) = True Then
                                            updateEndorsementRemarks = True
                                            'Note: Do not need to check if Quote.TransactionReasonId needs to be set back to 10168 because we are adding the new vehicle AI assignment next
                                        End If

                                        'add new AI record for the new AI and vehicle.
                                        If AddNewAIVehicleEndorsementAssignment(MyAdditionalInterest, diaVehicleNum) = True Then
                                            updateEndorsementRemarks = True
                                            Quote.TransactionReasonId = 10169 'Endorsement Change Dec and Full Revised Dec
                                        End If

                                    ElseIf aiNumOriginallyAssignedToVehicle = MyAdditionalInterest.ListId Then
                                        'delete the add entry for the original AI - if switching back to original AI assignment
                                        If RemovedAnyPreviouslyAddedAIVehicleEndorsementAssigments(diaVehicleNum) = True Then
                                            updateEndorsementRemarks = True
                                            'If there are any AIs that have been added to the DevDictionary (regardless of being assigned to a vehicle), keep the Quote.TransactionReasonId set to 10169
                                            'Otherwise, set it back to 10168. 10169 Full dec is for added vehicles and added AIs only.
                                            If HasAddedAIInDevDictionary() = False Then
                                                Quote.TransactionReasonId = 10168 'Endorsement Change Dec Only
                                            End If
                                        End If
                                        'when the transaction count goes from 3 to 1 (from assigning a vehicle back to the AI it was originally assigned when the endorsement was created - switching between AIs counts as 2 transactions, delete of old, add of new), need to repopulate the AI control so the max 3 transaction message hides and add AI button shows
                                        Dim transCount = ddh.GetEndorsementAdditionalInterestTransactionCount()
                                        If transCount >= 1 Then
                                            repopulateAIControl = True
                                        End If
                                    Else
                                        'Remove any previously added AI/Veh assignment for vehicle - can only have one AI/Vehicle association at a time. Will add the new assignment next.
                                        If RemovedAnyPreviouslyAddedAIVehicleEndorsementAssigments(diaVehicleNum) = True Then
                                            updateEndorsementRemarks = True
                                            'Note: Do not need to check if Quote.TransactionReasonId needs to be set back to 10168 because we are adding the new vehicle AI assignment next
                                        End If

                                        'add new AI Add entry for the new AI
                                        If AddNewAIVehicleEndorsementAssignment(MyAdditionalInterest, diaVehicleNum) = True Then
                                            updateEndorsementRemarks = True
                                            Quote.TransactionReasonId = 10169 'Endorsement Change Dec and Full Revised Dec
                                        End If
                                    End If

                                End If
                            Else 'added 5/23/2021 for CAP Endorsements Task 52974 MLW
                                'it must've just been removed at the top-level; go ahead and remove from vehicle
                                If MyVehicle.AdditionalInterests.Contains(aiToUse) = True Then
                                    If IsQuoteEndorsement() AndAlso TypeOfEndorsement() = "Add/Delete Additional Interest" AndAlso IsNewAdditionalInterestOnEndorsement(aiToUse) Then
                                        If RemovedPreviousAIVehicleAssignmentByAIListIdAndListType(aiToUse, MyVehicle.DisplayNum, DevDictionaryHelper.DevDictionaryHelper.addItem) = True Then
                                            updateEndorsementRemarks = True
                                            'If there are any AIs that have been added to the DevDictionary (regardless of being assigned to a vehicle), keep the Quote.TransactionReasonId set to 10169
                                            'Otherwise, set it back to 10168. 10169 Full dec is for added vehicles and added AIs only.
                                            If HasAddedAIInDevDictionary() = False Then
                                                Quote.TransactionReasonId = 10168 'Endorsement Change Dec Only
                                            End If
                                        End If
                                    End If
                                    MyVehicle.AdditionalInterests.Remove(aiToUse)
                                    'added 5/24/2021 to clear AI selections... in case another Save happens right after
                                    ddlLossPayeeName.ClearSelection()
                                    ddlLossPayeeType.ClearSelection()
                                    ddlATIMA.ClearSelection()
                                End If
                            End If
                        End With
                    End If
                Else
                    Dim aiListIdBeingRemoved As Integer = 0 'added 6/14/2021 for CAP Endorsements Task 52974 MLW

                    'Added 05/27/2021 for CAP Endorsements Task 52974 MLW
                    If IsQuoteEndorsement() AndAlso TypeOfEndorsement() = "Add/Delete Additional Interest" AndAlso MyVehicle.AdditionalInterests IsNot Nothing AndAlso MyVehicle.AdditionalInterests.Count > 0 Then
                        'This is an AI Delete entry to the DevDictionary
                        'The user unassigned the vehicle's AI (set to N/A)

                        Dim diaVehicleNum As Integer = MyVehicle.DisplayNum
                        Dim MyAdditionalInterest As QuickQuoteAdditionalInterest = MyVehicle.AdditionalInterests(0)

                        'Since we are unassigning the AI, we do not want any Add entries to exist (for this vehicle unless only one vehicle was associate to the AI. If there is only one vehicle, we remove the entire AI Add entry.)
                        'An add entry might exist if the user assigned the vehicle to an AI before deciding to unassign it. We are undoing their AI Add entry.
                        'Remove any previously added AI/Veh assignment for vehicle
                        If RemovedAnyPreviouslyAddedAIVehicleEndorsementAssigments(diaVehicleNum) = True Then
                            updateEndorsementRemarks = True
                            'when the transaction count goes from 3 to 2, need to repopulate the AI control so the max 3 transaction message hides and add AI button shows (this is in the AI control).
                            Dim transCount = ddh.GetEndorsementAdditionalInterestTransactionCount()
                            If transCount = 2 Then
                                repopulateAIControl = True
                            End If
                        End If

                        'When the vehicle had an AI assigned when the endorsement was created, we need to make sure that a DevDictionary Delete entry is recorded for that original AI.
                        'had AI assigned to vehicle when endorsement created, find it & add to delete list
                        Dim aiNumOriginallyAssignedToVehicle As String = PolicyAIAssignedToVehicle(diaVehicleNum)
                        If aiNumOriginallyAssignedToVehicle <> "" Then
                            'Add AI delete record for old AI
                            If SetOriginallyAssignedAIToEndorsementDelete(aiNumOriginallyAssignedToVehicle, diaVehicleNum) = True Then
                                updateEndorsementRemarks = True
                            End If
                        End If

                        'added 6/14/2021 for CAP Endorsements Task 52974 MLW
                        If MyAdditionalInterest IsNot Nothing AndAlso MyAdditionalInterest.HasValidAdditionalInterestListId = True Then
                            aiListIdBeingRemoved = QQHelper.IntegerForString(MyAdditionalInterest.ListId)
                            'This is needed so we know whether or not to update the AI list at the top level.
                        End If
                    End If

                    'added 6/8/2017
                    QQHelper.DisposeAdditionalInterests(MyVehicle.AdditionalInterests)

                    'added 6/14/2021 for CAP Endorsements Task 52974 MLW
                    If aiListIdBeingRemoved > 0 Then
                        'This is where we evaluate whether or not to remove the AI from the top level.
                        Dim aiExistsOnAnotherVehicle As Boolean = False
                        Dim identifiedResults As QuickQuoteAdditionalInterestRelatedResults = Nothing
                        QQHelper.IdentifySpecificQuickQuoteAdditionalInterestFromQuoteBasedOnLob(Me.Quote, aiListIdBeingRemoved, identifiedResults:=identifiedResults, shouldRemove:=False, includeTopLevel:=True) 'This gets a list of AIs at the vehicle level (Diamond has AIs at vehicle level. VR has them at the Quote level as a master list and vehicle level as a vehicle AI assignment.)
                        If identifiedResults IsNot Nothing Then
                            If identifiedResults.VehiclesAndAdditionalInterests IsNot Nothing AndAlso identifiedResults.VehiclesAndAdditionalInterests.Count > 0 Then
                                For Each vai As QuickQuoteVehicleAndAdditionalInterests In identifiedResults.VehiclesAndAdditionalInterests
                                    If vai IsNot Nothing AndAlso vai.Vehicle IsNot Nothing AndAlso vai.AdditionalInterests IsNot Nothing AndAlso vai.AdditionalInterests.Count > 0 Then
                                        aiExistsOnAnotherVehicle = True
                                        Exit For
                                    End If
                                Next
                            End If
                            If aiExistsOnAnotherVehicle = False AndAlso identifiedResults.TopLevelAdditionalInterests IsNot Nothing AndAlso identifiedResults.TopLevelAdditionalInterests.Count > 0 AndAlso identifiedResults.TopLevelAdditionalInterests(0) IsNot Nothing AndAlso identifiedResults.TopLevelAdditionalInterests(0).OriginalSourceAI IsNot Nothing Then
                                If MyAiList IsNot Nothing AndAlso MyAiList.Count > 0 Then
                                    QuickQuoteHelperClass.RemoveQuickQuoteAdditionalInterestsForListId(MyAiList, aiListIdBeingRemoved.ToString)
                                    removedCopiedAiFromTopLevel = True
                                End If
                            End If
                        End If
                    End If
                End If

                'Added 06/07/2021 for CAP Endorsements Task 52974 MLW
                Dim transactionCount As Integer = 0
                If updateEndorsementRemarks = True Then
                    Dim endorsementsRemarksHelper = New EndorsementsRemarksHelper(ddh)
                    Dim updatedRemarks As String = endorsementsRemarksHelper.UpdateRemarks(endorsementsRemarksHelper.RemarksType.AdditionalInterest)
                    Quote.TransactionRemark = updatedRemarks 'Shows on treeview and on dec

                    transactionCount = ddh.GetEndorsementAdditionalInterestTransactionCount()
                End If

                RaiseEvent VehicleChanged(VehicleIndex)

                'added 6/14/2021 for CAP Endorsements Task 52974 MLW
                If removedCopiedAiFromTopLevel = True OrElse transactionCount >= 3 OrElse repopulateAIControl = True Then
                    RaiseEvent NeedToRepopulateTopLevelAIs()
                End If

                Return True
            End If
        End If
        Return False
    End Function

    'Added 07/02/2021 for CAP Endorsements Task 52974 MLW
    Private Function HasAddedAIInDevDictionary() As Boolean
        'Used to determine if any Add entries exist in the DevDictionary for any AIs. This is used to update the Quote.TransactionReasonId which determines which dec is in the print history.
        'Any add AIs need Quote.TransactionReasonId = 10169. All other AI transactions are 10168.
        Dim hasAddedAI As Boolean = False
        Dim aiList = ddh.GetAdditionalInterestDictionary
        For Each ai In aiList
            If ai.addOrDelete = DevDictionaryHelper.DevDictionaryHelper.addItem Then
                hasAddedAI = True
                Exit For
            End If
        Next
        Return hasAddedAI
    End Function

    'Added 06/28/2021 for CAP Endorsements Task 52974 MLW
    Private Function RemovedPreviousAIVehicleAssignmentByAIListIdAndListType(myAdditionalInterest As QuickQuoteAdditionalInterest, diaVehicleNum As Integer, addOrDelete As String) As Boolean
        'Need to remove any AI Add entries prior to updating the AI for the new vehicle/AI assigment. Need to remove any AI Delete entries prior to udpating the AI for the AI unassignment from the vehicle.
        'The ListId is for the AI being assigned (for Add entries) or unassigned (for Delete entries). The ListId is not for the original vehicle/AI assignment when switching between AIs.
        Dim hasChanged As Boolean = False
        Dim strVehicleNumsForAIList As String = ""
        Dim aiExistsInList As Boolean = False
        Dim aiVehicleList As List(Of Integer) = New List(Of Integer)
        'Find if the AI exists in the list and get its vehicle number list if it is found.
        If ddh.GetAdditionalInterestDictionary IsNot Nothing AndAlso ddh.GetAdditionalInterestDictionary.Count > 0 Then
            For Each additionalInterest As DevDictionaryHelper.AdditionalInterestInfo In ddh.GetAdditionalInterestDictionary
                If additionalInterest.addOrDelete = addOrDelete Then
                    If additionalInterest.diaAINumber = myAdditionalInterest.ListId Then
                        aiExistsInList = True
                        strVehicleNumsForAIList = additionalInterest.VehicleNumList
                        Exit For
                    End If
                End If
            Next
        End If
        If aiExistsInList Then
            If Not (IsNullEmptyorWhitespace(strVehicleNumsForAIList)) AndAlso strVehicleNumsForAIList <> "0" Then
                aiVehicleList = strVehicleNumsForAIList.Split(","c).[Select](Function(n) Integer.Parse(n)).ToList()
                For Each deletedAIVehNum As Integer In aiVehicleList
                    If deletedAIVehNum = diaVehicleNum Then
                        aiVehicleList.Remove(deletedAIVehNum)
                        If aiVehicleList.Count > 0 Then
                            Dim listType As DevDictionaryHelper.DevDictionaryHelper.DevDictionaryListType = DevDictionaryHelper.DevDictionaryHelper.DevDictionaryListType.None
                            Select Case addOrDelete
                                Case DevDictionaryHelper.DevDictionaryHelper.addItem
                                    listType = DevDictionaryHelper.DevDictionaryHelper.DevDictionaryListType.Add
                                Case DevDictionaryHelper.DevDictionaryHelper.deleteItem
                                    listType = DevDictionaryHelper.DevDictionaryHelper.DevDictionaryListType.Delete
                            End Select
                            'Updates the DevDictionary AI entry to not include assigned/unassigned vehicle
                            ddh.UpdateDevDictionaryAdditionalInterestList(listType, myAdditionalInterest, aiVehicleList)
                        Else
                            Dim removeListType As DevDictionaryHelper.DevDictionaryHelper.DevDictionaryListType = DevDictionaryHelper.DevDictionaryHelper.DevDictionaryListType.None
                            Select Case addOrDelete
                                Case DevDictionaryHelper.DevDictionaryHelper.addItem
                                    removeListType = DevDictionaryHelper.DevDictionaryHelper.DevDictionaryListType.RemoveAdd
                                Case DevDictionaryHelper.DevDictionaryHelper.deleteItem
                                    removeListType = DevDictionaryHelper.DevDictionaryHelper.DevDictionaryListType.RemoveDelete
                            End Select
                            'Removes the entire DevDictionary entry for this AI (when the vehicle was the last vehicle in the vehicle number list)
                            ddh.UpdateDevDictionaryAdditionalInterestList(removeListType, myAdditionalInterest, aiVehicleList)
                        End If
                        hasChanged = True
                        Exit For
                    End If
                Next
            End If
        End If
        Return hasChanged
    End Function

    'Added 06/22/2021 for CAP Endorsements Task 52974 MLW
    Private Function SetOriginallyAssignedAIToEndorsementDelete(aiNumOriginallyAssignedToVehicle As String, diaVehicleNum As Integer) As Boolean
        'This is for unassigning the vehicle from an AI or for switching between AIs when the vehicle had an AI assigned when the endorsement was created.
        'Will update or add an AI Delete entry to the DevDictionary.
        'DevDictionary AI Delete entry example: 78412==JP MORGAN CHASE BANK==1,2,3==DEL
        'AI ListId == Lienholder name == vehicle number list == list type (an add or delete)
        Dim hasChanged As Boolean = False
        Dim oldAIIndex As Integer = GetOldAIIndex(aiNumOriginallyAssignedToVehicle) 'Finds the index for the AI (based on its ListId passed in) from the quote level Additional Interest list
        If oldAIIndex > -1 Then
            Dim myOldAdditionalInterest As QuickQuoteAdditionalInterest = Quote.AdditionalInterests(oldAIIndex) 'Finds the AI object in the quote level AI list
            'see if old AI exists in DevDictionary delete list - if exists, it also gets its associated vehicle number list
            Dim strDeletedVehicleNumsForOldAIList As String = ""
            Dim oldAIExistsInDeleteList As Boolean = False
            Dim oldAIVehicleDeleteList As List(Of Integer) = New List(Of Integer)
            If ddh.GetAdditionalInterestDictionary IsNot Nothing AndAlso ddh.GetAdditionalInterestDictionary.Count > 0 Then
                For Each additionalInterest As DevDictionaryHelper.AdditionalInterestInfo In ddh.GetAdditionalInterestDictionary
                    If additionalInterest.addOrDelete = DevDictionaryHelper.DevDictionaryHelper.deleteItem Then
                        If additionalInterest.diaAINumber = aiNumOriginallyAssignedToVehicle Then
                            oldAIExistsInDeleteList = True
                            strDeletedVehicleNumsForOldAIList = additionalInterest.VehicleNumList
                            Exit For
                        End If
                    End If
                Next
            End If
            If oldAIExistsInDeleteList Then
                Dim vehicleExistsInOldAIDeleteVehicleNumList As Boolean = False
                If Not (IsNullEmptyorWhitespace(strDeletedVehicleNumsForOldAIList)) AndAlso strDeletedVehicleNumsForOldAIList <> "0" Then
                    oldAIVehicleDeleteList = strDeletedVehicleNumsForOldAIList.Split(","c).[Select](Function(n) Integer.Parse(n)).ToList()
                    For Each oldAIVehNum As Integer In oldAIVehicleDeleteList
                        If oldAIVehNum = diaVehicleNum Then
                            vehicleExistsInOldAIDeleteVehicleNumList = True
                            Exit For
                        End If
                    Next
                    'append vehicle num to existing list
                    If vehicleExistsInOldAIDeleteVehicleNumList = False Then
                        oldAIVehicleDeleteList.Add(diaVehicleNum)
                        oldAIVehicleDeleteList.Sort()
                        ddh.UpdateDevDictionaryAdditionalInterestList(DevDictionaryHelper.DevDictionaryHelper.DevDictionaryListType.Delete, myOldAdditionalInterest, oldAIVehicleDeleteList)
                        hasChanged = True
                    End If
                Else
                    'no vehicle list yet for the old AI delete entry
                    oldAIVehicleDeleteList.Add(diaVehicleNum)
                    ddh.UpdateDevDictionaryAdditionalInterestList(DevDictionaryHelper.DevDictionaryHelper.DevDictionaryListType.Delete, myOldAdditionalInterest, oldAIVehicleDeleteList)
                    hasChanged = True
                End If
            Else
                'no delete entry yet for the old AI, add new delete record with new vehicle number for the Old AI
                oldAIVehicleDeleteList.Add(diaVehicleNum)
                ddh.UpdateDevDictionaryAdditionalInterestList(DevDictionaryHelper.DevDictionaryHelper.DevDictionaryListType.Delete, myOldAdditionalInterest, oldAIVehicleDeleteList)
                hasChanged = True
            End If
        End If
        Return hasChanged
    End Function

    'Added 06/22/2021 for CAP Endorsements Task 52974 MLW
    Private Function RemovedAnyPreviouslyAddedAIVehicleEndorsementAssigments(diaVehicleNum As String) As Object
        'This will update the DevDictionary AI Add entry that the vehicle is listed in in the AI's vehicle number list
        'DevDictionary AI Add entry example: 59078==FORUM CREDIT UNION==3,4==ADD
        'AI ListId == Lienholder name == vehicle number list == list type (an add or delete)
        Dim hasChanged As Boolean = False
        Dim strAddedVehicleNumsForAIList As String = ""
        Dim aiVehicleAddList As List(Of Integer) = New List(Of Integer)
        Dim myPreviousAdditionalInterestIndex As Integer = -1
        Dim strAddedVehicleNumsForPreviousAIList As String = ""
        Dim vehicleExistsInAIAddVehicleNumList As Boolean = False
        'see if the AI exists in DevDictionary add list - if exists, it also gets its associated vehicle number list
        If ddh.GetAdditionalInterestDictionary IsNot Nothing AndAlso ddh.GetAdditionalInterestDictionary.Count > 0 Then
            For Each additionalInterest As DevDictionaryHelper.AdditionalInterestInfo In ddh.GetAdditionalInterestDictionary
                If additionalInterest.addOrDelete = DevDictionaryHelper.DevDictionaryHelper.addItem Then
                    If Not (IsNullEmptyorWhitespace(additionalInterest.VehicleNumList)) AndAlso additionalInterest.VehicleNumList <> "0" Then
                        aiVehicleAddList = additionalInterest.VehicleNumList.Split(","c).[Select](Function(n) Integer.Parse(n)).ToList()
                        For Each addAIVehNum As Integer In aiVehicleAddList
                            If addAIVehNum = diaVehicleNum Then
                                vehicleExistsInAIAddVehicleNumList = True
                                myPreviousAdditionalInterestIndex = GetOldAIIndex(additionalInterest.diaAINumber)
                                strAddedVehicleNumsForPreviousAIList = additionalInterest.VehicleNumList
                                Exit For
                            End If
                        Next
                    End If
                    If vehicleExistsInAIAddVehicleNumList = True Then
                        Exit For
                    End If
                End If
            Next
        End If
        If vehicleExistsInAIAddVehicleNumList AndAlso myPreviousAdditionalInterestIndex > -1 Then
            Dim myPreviousAdditionalInterest As QuickQuoteAdditionalInterest = Quote.AdditionalInterests(myPreviousAdditionalInterestIndex)
            If aiVehicleAddList IsNot Nothing AndAlso aiVehicleAddList.Count > 0 Then
                aiVehicleAddList.Remove(diaVehicleNum)
                If aiVehicleAddList.Count = 0 AndAlso Not IsNewAdditionalInterestOnEndorsement(myPreviousAdditionalInterest) Then
                    'remove AI Add entry - want to do this if it is the last vehicle being removed from the list and the AI is an existing AI on the endorsement. 
                    'New AIs added to the endorsement keep their DevDictionary AI Add entry because adding a new AI to the master list counts as an add even if no vehicles are assigned.
                    ddh.UpdateDevDictionaryAdditionalInterestList(DevDictionaryHelper.DevDictionaryHelper.DevDictionaryListType.RemoveAdd, myPreviousAdditionalInterest, aiVehicleAddList)
                    hasChanged = True
                Else
                    'remove vehicle from AI Add entry - keeps the AI Add entry, but updates the vehicle number list to not include the assigned/unassigned vehicle
                    ddh.UpdateDevDictionaryAdditionalInterestList(DevDictionaryHelper.DevDictionaryHelper.DevDictionaryListType.Add, myPreviousAdditionalInterest, aiVehicleAddList)
                    hasChanged = True
                End If
            End If
        End If
        Return hasChanged
    End Function

    'Added 06/17/2021 for CAP Endorsements Task 52974 MLW
    Private Function GetOldAIIndex(aiNumOriginallyAssignedToVehicle As String) As Integer
        'Used when we need to know the index of the AI so we can get the entire AI object to be used later to update the DevDictionary
        Dim oldAIIndex As Integer = -1
        Dim i As Integer = 0
        For Each qai In Quote.AdditionalInterests
            If qai.ListId = aiNumOriginallyAssignedToVehicle Then
                oldAIIndex = i
            End If
            i += 1
        Next
        Return oldAIIndex
    End Function

    'Added 06/17/2021 for CAP Endorsements Task 52974 MLW
    Private Function AddNewAIVehicleEndorsementAssignment(myAdditionalInterest As QuickQuoteAdditionalInterest, diaVehicleNum As String) As Boolean
        'This will add a new AI Add entry to the DevDictionary. This happens anytime a vehicle is assigned an AI from the drop down when the drop down was previously N/A or if the vehicle's AI is switched from one AI to another.
        'DevDictionary AI Add entry example: 59078==FORUM CREDIT UNION==3,4==ADD
        'AI ListId == Lienholder name == vehicle number list == list type (an add or delete)
        Dim hasChanged As Boolean = False
        Dim aiExistsInAddList As Boolean = False
        Dim strAddAIVehicleList As String = ""
        Dim aiVehicleList As List(Of Integer) = New List(Of Integer)
        'see if the AI exists in DevDictionary add list - if exists, it also gets its associated vehicle number list
        If ddh.GetAdditionalInterestDictionary IsNot Nothing AndAlso ddh.GetAdditionalInterestDictionary.Count > 0 Then
            For Each additionalInterest As DevDictionaryHelper.AdditionalInterestInfo In ddh.GetAdditionalInterestDictionary
                If additionalInterest.addOrDelete = DevDictionaryHelper.DevDictionaryHelper.addItem Then
                    If additionalInterest.diaAINumber = myAdditionalInterest.ListId Then
                        aiExistsInAddList = True
                        strAddAIVehicleList = additionalInterest.VehicleNumList
                        Exit For
                    End If
                End If
            Next
        End If

        If aiExistsInAddList = True Then
            Dim vehicleExistsInAIVehicleNumList As Boolean = False
            If Not (IsNullEmptyorWhitespace(strAddAIVehicleList)) AndAlso strAddAIVehicleList <> "0" Then
                Dim originalVehicleAIList As List(Of Integer) = strAddAIVehicleList.Split(","c).[Select](Function(n) Integer.Parse(n)).ToList()
                For Each origAIVehNum As Integer In originalVehicleAIList
                    If origAIVehNum = diaVehicleNum Then
                        vehicleExistsInAIVehicleNumList = True
                        Exit For
                    End If
                Next
                If vehicleExistsInAIVehicleNumList = False Then
                    'add vehicle num to already existing vehicle list
                    aiVehicleList = originalVehicleAIList
                    aiVehicleList.Add(diaVehicleNum)
                    aiVehicleList.Sort()
                    ddh.UpdateDevDictionaryAdditionalInterestList(DevDictionaryHelper.DevDictionaryHelper.DevDictionaryListType.Add, myAdditionalInterest, aiVehicleList)
                    hasChanged = True
                End If
            Else
                'no list, add vehicle list to AI add record
                aiVehicleList.Add(diaVehicleNum)
                ddh.UpdateDevDictionaryAdditionalInterestList(DevDictionaryHelper.DevDictionaryHelper.DevDictionaryListType.Add, myAdditionalInterest, aiVehicleList)
                hasChanged = True
            End If
        Else
            'Add new AI add record to include this new AI with the vehicle number
            aiVehicleList.Add(diaVehicleNum)
            ddh.UpdateDevDictionaryAdditionalInterestList(DevDictionaryHelper.DevDictionaryHelper.DevDictionaryListType.Add, myAdditionalInterest, aiVehicleList)
            hasChanged = True
        End If

        Return hasChanged
    End Function

    'Added 06/17/2021 for CAP Endorsements Task 52974 MLW
    Private Function DoesEndorsementHaveAIVehicleAssignmentChange(diaVehicleNum As Integer) As Boolean
        'This is used to determine whether or not to show the ctl_CAP_App_Vehicle section disabled/locked when the max 3 AI transactions are met.
        'If the vehicle had an AI assignment change on the endorsement, it shall remain unlocked so the user can undo their change if needed.
        'This is only for Add/Delete Additional Interest. Add/Delete Vehicle does not allow changes to existing vehicles. New vehicles can have an AI assigned, but are not tracked in the DevDictioanry.
        'It finds the vehicle number in any AI list (does not matter if add or delete) and evaluates true if found.
        Dim hasChange As Boolean = False
        If ddh.GetAdditionalInterestDictionary IsNot Nothing AndAlso ddh.GetAdditionalInterestDictionary.Count > 0 Then
            For Each additionalInterest As DevDictionaryHelper.AdditionalInterestInfo In ddh.GetAdditionalInterestDictionary
                If Not (IsNullEmptyorWhitespace(additionalInterest.VehicleNumList)) AndAlso additionalInterest.VehicleNumList <> "0" Then
                    Dim aiVehicleList As List(Of Integer) = additionalInterest.VehicleNumList.Split(","c).[Select](Function(n) Integer.Parse(n)).ToList()
                    For Each aiVehNum As Integer In aiVehicleList
                        If aiVehNum = diaVehicleNum Then
                            hasChange = True
                            Exit For
                        End If
                    Next
                    If hasChange = True Then
                        Exit For
                    End If
                End If
            Next
        End If
        Return hasChange
    End Function

    'Added 06/16/2021 for CAP Endorsements Task 52974
    Private Function PolicyAIAssignedToVehicle(diaVehicleNum As String) As String
        'Gets the AI listId that the vehicle was originally assigned to when the endorsement was created.
        'DevDictionary data is vehicleNum And AI listId. Ex: 1==167037&&2==43686
        Dim ddhelper As DevDictionaryHelper.DevDictionaryHelper = New DevDictionaryHelper.DevDictionaryHelper(Quote, "CAPEndorsements_Original_Vehicle_AIs", Quote.LobType)
        Dim originalAIAssignedToVehicle As String = ddhelper.GetValueFromMasterValueDictionaryByKey(diaVehicleNum)
        Return originalAIAssignedToVehicle
    End Function

    Protected Sub lnkSave_Click(sender As Object, e As EventArgs) Handles lnkSave.Click
        Me.Save_FireSaveEvent(True)
    End Sub

    Private Sub LoadLossPayeeDDL()
        Dim li As New ListItem()
        ddlLossPayeeName.Items.Clear()
        li.Text = "N/A"
        li.Value = ""
        ddlLossPayeeName.Items.Add(li)
        'If Quote IsNot Nothing AndAlso Quote.AdditionalInterests IsNot Nothing Then
        '    For Each ai As QuickQuote.CommonObjects.QuickQuoteAdditionalInterest In Quote.AdditionalInterests
        '        li = New ListItem
        '        li.Text = ai.Name.DisplayName
        '        li.Value = ai.ListId
        '        ddlLossPayeeName.Items.Add(li)
        '    Next
        'End If
        'Updated 3/25/2021 for CAP Endorsements task 52974 MLW
        If Quote IsNot Nothing Then
            Dim aiList As List(Of QuickQuoteAdditionalInterest) = MyAiList
            If aiList IsNot Nothing AndAlso aiList.Count > 0 Then
                For Each ai As QuickQuote.CommonObjects.QuickQuoteAdditionalInterest In aiList
                    If ai IsNot Nothing AndAlso ai.HasValidAdditionalInterestListId = True Then 'added IF 6/9/2021
                        Dim trucatedDisplay As String = ai.Name.DisplayName
                        If Not(IsNullEmptyorWhitespace(trucatedDisplay)) AndAlso trucatedDisplay.Length > 25 Then trucatedDisplay = trucatedDisplay.Substring(0, 25)
                        li = New ListItem
                        li.Text = trucatedDisplay
                        li.Value = ai.ListId
                        ddlLossPayeeName.Items.Add(li)
                    End If
                Next
            End If
        End If
    End Sub
    'Added 3/25/2021 from ctlVehicleAdditionalInterestList
    Public Property MyAiList As List(Of QuickQuoteAdditionalInterest)
        Get
            Dim AiList As List(Of QuickQuoteAdditionalInterest) = Nothing
            Select Case Me.Quote.LobType
                Case QuickQuoteObject.QuickQuoteLobType.AutoPersonal
                    Dim vehicle As QuickQuote.CommonObjects.QuickQuoteVehicle = Nothing
                    If Me.Quote.Vehicles IsNot Nothing AndAlso Me.Quote.Vehicles.Count > Me.VehicleIndex Then
                        vehicle = Me.Quote.Vehicles(Me.VehicleIndex)
                        If vehicle IsNot Nothing Then
                            AiList = vehicle.AdditionalInterests
                        End If
                    End If
                Case QuickQuoteObject.QuickQuoteLobType.HomePersonal, QuickQuoteObject.QuickQuoteLobType.DwellingFirePersonal
                    AiList = Me.Quote.Locations(0).AdditionalInterests
                Case QuickQuoteObject.QuickQuoteLobType.Farm
                    AiList = Me.Quote.Locations(0).AdditionalInterests
                Case QuickQuoteObject.QuickQuoteLobType.CommercialBOP, QuickQuoteObject.QuickQuoteLobType.CommercialAuto, QuickQuoteObject.QuickQuoteLobType.CommercialProperty, QuickQuoteObject.QuickQuoteLobType.CommercialPackage
                    AiList = Quote.AdditionalInterests
                    ''Added 3/25/2021 for CAP Endorsements task 52974 MLW
                    'If Me.Quote.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialAuto Then
                    '    Dim aiListIds As List(Of Integer) = Nothing
                    '    If Me.Quote.AdditionalInterests IsNot Nothing AndAlso Me.Quote.AdditionalInterests.Count > 0 Then
                    '        For Each ai As QuickQuoteAdditionalInterest In Me.Quote.AdditionalInterests
                    '            If ai IsNot Nothing Then
                    '                QuickQuoteHelperClass.AddUniqueIntegerToIntegerList(QQHelper.IntegerForString(ai.ListId), aiListIds, positiveOnly:=True)
                    '            End If
                    '        Next
                    '    End If
                    '    If Me.Quote.Vehicles IsNot Nothing AndAlso Me.Quote.Vehicles.Count > 0 Then
                    '        For Each v As QuickQuoteVehicle In Me.Quote.Vehicles
                    '            If v IsNot Nothing AndAlso v.AdditionalInterests IsNot Nothing AndAlso v.AdditionalInterests.Count > 0 Then
                    '                For Each vai As QuickQuoteAdditionalInterest In v.AdditionalInterests
                    '                    If vai IsNot Nothing AndAlso vai.HasValidAdditionalInterestListId = True Then
                    '                        If aiListIds Is Nothing OrElse aiListIds.Count = 0 OrElse aiListIds.Contains(QQHelper.IntegerForString(vai.ListId)) = False Then
                    '                            'If AiList Is Nothing Then
                    '                            '    AiList = New List(Of QuickQuoteAdditionalInterest)
                    '                            'End If
                    '                            'AiList.Add(QQHelper.CloneObject(vai))
                    '                            'QuickQuoteHelperClass.AddUniqueIntegerToIntegerList(QQHelper.IntegerForString(vai.ListId), aiListIds, positiveOnly:=True)
                    '                            Dim copiedAI As QuickQuoteAdditionalInterest = QQHelper.CloneObject(vai)
                    '                            If copiedAI IsNot Nothing Then
                    '                                If AiList Is Nothing Then
                    '                                    AiList = New List(Of QuickQuoteAdditionalInterest)
                    '                                End If
                    '                                copiedAI.Num = "" 'clearing out in case it came over from source AI
                    '                                AiList.Add(copiedAI)
                    '                                QuickQuoteHelperClass.AddUniqueIntegerToIntegerList(QQHelper.IntegerForString(vai.ListId), aiListIds, positiveOnly:=True)
                    '                            End If
                    '                        End If
                    '                    End If
                    '                Next
                    '            End If
                    '        Next
                    '    End If
                    'End If
            End Select
            Return AiList
        End Get
        Set(value As List(Of QuickQuoteAdditionalInterest))
            Select Case Me.Quote.LobType
                Case QuickQuoteObject.QuickQuoteLobType.AutoPersonal
                    Dim vehicle As QuickQuote.CommonObjects.QuickQuoteVehicle = Nothing
                    If Me.Quote.Vehicles IsNot Nothing AndAlso Me.Quote.Vehicles.Count > Me.VehicleIndex Then
                        vehicle = Me.Quote.Vehicles(Me.VehicleIndex)
                        If vehicle IsNot Nothing Then
                            vehicle.AdditionalInterests = value
                        End If
                    End If
                Case QuickQuoteObject.QuickQuoteLobType.HomePersonal, QuickQuoteObject.QuickQuoteLobType.DwellingFirePersonal
                    Me.Quote.Locations(0).AdditionalInterests = value
                Case QuickQuoteObject.QuickQuoteLobType.Farm
                    Me.Quote.Locations(0).AdditionalInterests = value
                Case QuickQuoteObject.QuickQuoteLobType.CommercialBOP, QuickQuoteObject.QuickQuoteLobType.CommercialAuto, QuickQuoteObject.QuickQuoteLobType.CommercialProperty, QuickQuoteObject.QuickQuoteLobType.CommercialPackage
                    Quote.AdditionalInterests = value
            End Select
        End Set
    End Property

    'Added 08/05/2021 for CAP Endorsements Tasks 53030 MLW
    Private Function GetValidVinValue(vehicleIndex As Integer) As String
        'ValidVIN is used to know whether or not a VIN lookup was performed and if the lookup returned valid VIN results
        'VIN Lookup is required on CAP, using this to know whether or not to disable the rate button and show a route to UW button
        Dim validVin As String
        Dim vinDDH As DevDictionaryHelper.DevDictionaryHelper = New DevDictionaryHelper.DevDictionaryHelper(Quote, "ValidVIN", Quote.LobType)
        validVin = vinDDH.GetValueFromMasterValueDictionaryByKey(vehicleIndex + 1)
        If IsNullEmptyorWhitespace(validVin) Then
            'If not yet in the DevDictionary, assume no lookup was ever done
            validVin = "False"
        End If
        Return validVin
    End Function

End Class