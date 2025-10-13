Imports IFM.PrimativeExtensions
Imports IFM.VR.Web.Helpers
Imports QuickQuote.CommonMethods.QuickQuoteHelperClass
Imports IFM.VR.Common.Helpers.CPR
Imports IFM.VR.Common.Helpers.CPP

Public Class ctl_CPR_PropertyInOpenItem
    Inherits VRControlBase

#Region "Declarations"

    Public Property LocationIndex As Int32
        Get
            Return ViewState.GetInt32("vs_LocationIndex", -1)
        End Get
        Set(value As Int32)
            ViewState("vs_LocationIndex") = value
        End Set
    End Property

    Public Property PropertyIndex As Int32
        Get
            Return ViewState.GetInt32("vs_PropertyIndex", -1)
        End Get
        Set(value As Int32)
            ViewState("vs_PropertyIndex") = value
        End Set
    End Property

    Private ReadOnly Property MyProperty As QuickQuote.CommonObjects.QuickQuotePropertyInTheOpenRecord
        Get
            If Me.Quote IsNot Nothing AndAlso Me.Quote.Locations.HasItemAtIndex(Me.LocationIndex) AndAlso Me.Quote.Locations.GetItemAtIndex(Me.LocationIndex).PropertyInTheOpenRecords.HasItemAtIndex(Me.PropertyIndex) Then
                Return Me.Quote.Locations(LocationIndex).PropertyInTheOpenRecords.GetItemAtIndex(Me.PropertyIndex)
            End If
            Return Nothing
        End Get
    End Property

    Private ReadOnly Property MyLocation As QuickQuote.CommonObjects.QuickQuoteLocation
        Get
            If Me.Quote IsNot Nothing AndAlso Me.Quote.Locations.HasItemAtIndex(Me.LocationIndex) Then
                Return Me.Quote.Locations(LocationIndex)
            End If
            Return Nothing
        End Get
    End Property

    Private ReadOnly Property bldgQuote As QuickQuote.CommonObjects.QuickQuoteObject
        Get
            Return Me.SubQuoteForLocation(MyLocation)
        End Get
    End Property

    Protected Overrides ReadOnly Property MyAccordionIndex As Integer
        Get
            Return Me.PropertyIndex
        End Get
    End Property

    Private Structure DropdownDefaults_Structure
        Public COLId As String
        Public COINSId As String
        Public VALId As String
        Public DEDId As String
    End Structure

    Private DropdownDefaults As New DropdownDefaults_Structure()

    Public Event PIODeleteRequested(ByVal LocIndex As Integer, ByVal ItemIndex As Integer)
    'Public Event PIOClearRequested(ByVal LocIndex As Integer, ByVal ItemIndex As Integer)
    'Public Event PIOChanged(ByVal LocIndex As Integer, ByVal ItemIndex As Integer)

#End Region

#Region "Methods and Functions"

    Public Overrides Sub AddScriptAlways()

    End Sub

    Public Overrides Sub AddScriptWhenRendered()
        Dim HasBlanketAgreed As String = "False"

        If bldgQuote.HasBlanketBuilding Then
            HasBlanketAgreed = bldgQuote.BlanketBuildingIsAgreedValue.ToString
        ElseIf bldgQuote.HasBlanketContents Then
            HasBlanketAgreed = bldgQuote.BlanketContentsIsAgreedValue.ToString
        ElseIf bldgQuote.HasBlanketBuildingAndContents Then
            HasBlanketAgreed = bldgQuote.BlanketBuildingAndContentsIsAgreedValue.ToString
        End If

        Me.VRScript.StopEventPropagation(Me.lnkSave.ClientID)
        Me.VRScript.CreateConfirmDialog(Me.lnkDelete.ClientID, "Delete?")
        Me.VRScript.CreateConfirmDialog(Me.lnkClear.ClientID, "Clear?")

        ' These variables are needed for the class code lookup script
        Me.VRScript.AddVariableLine("var txtPIOClassCode = '" & txtSpecialClassCode.ClientID & "';")
        Me.VRScript.AddVariableLine("var txtPIODescription = '" & txtDescription.ClientID & "';")
        Me.VRScript.AddVariableLine("var txtPIOID = '" & txtClassCodeId.ClientID & "';")
        Me.VRScript.AddVariableLine("var PIOLookuWindowID = '" & Me.ctl_PIOClassCodeLookup.ClientID & "';")

        'Updated 12/08/2021 for CPP Endorsements Task 66977 MLW
        If Not IsQuoteReadOnly() Then
            PopulateDropdownDefaults()
        End If

        Me.VRScript.CreateJSBinding(Me.chkIncludedInBlanketRating, ctlPageStartupScript.JsEventType.onchange, "Cpr.PIOAgreedAmountOrBlanketCheckboxChanged('" & Me.chkIncludedInBlanketRating.ClientID & "','" & Me.chkAgreedAmount.ClientID & "','" & Me.trBlanketInfoRow.ClientID & "','" & Me.ddCauseOfLoss.ClientID & "','" & Me.ddCoinsurance.ClientID & "','" & Me.ddValuation.ClientID & "','" & Me.ddDeductible.ClientID & "','" & DropdownDefaults.COLId & "','" & DropdownDefaults.COINSId & "','" & DropdownDefaults.VALId & "','" & DropdownDefaults.DEDId & "','" & HasBlanketAgreed & "');")
        Me.VRScript.CreateJSBinding(Me.chkAgreedAmount, ctlPageStartupScript.JsEventType.onchange, "Cpr.PIOAgreedAmountOrBlanketCheckboxChanged('" & Me.chkIncludedInBlanketRating.ClientID & "','" & Me.chkAgreedAmount.ClientID & "','" & Me.trBlanketInfoRow.ClientID & "','" & Me.ddCauseOfLoss.ClientID & "','" & Me.ddCoinsurance.ClientID & "','" & Me.ddValuation.ClientID & "','" & Me.ddDeductible.ClientID & "','" & DropdownDefaults.COLId & "','" & DropdownDefaults.COINSId & "','" & DropdownDefaults.VALId & "','" & DropdownDefaults.DEDId & "','" & HasBlanketAgreed & "');")
        Me.VRScript.CreateJSBinding(chkEarthquake, ctlPageStartupScript.JsEventType.onchange, "Cpr.LocOrPIOCoverageCheckboxChanged('PIOEQ','" & chkEarthquake.ClientID & "');")
        If IsNewCo() Then
            Me.VRScript.CreateJSBinding(chkEarthquake, ctlPageStartupScript.JsEventType.onchange, "Cpr.ToggleEarthquakeDeductible('" & chkEarthquake.ClientID & "','" & trEarthquakeDeductible.ClientID & "');")
        End If
        If LocationWindHailHelper.IsLocationWindHailAvailable(Quote) Then
            Me.VRScript.CreateJSBinding(chkWindHail, ctlPageStartupScript.JsEventType.onchange, "Cpr.WindHailCheckboxChanged('" & chkWindHail.ClientID & "');")
        End If

        Exit Sub
    End Sub

    Public Overrides Sub LoadStaticData()
        If ddValuation.Items Is Nothing OrElse ddValuation.Items.Count <= 0 Then
            ' Valuation
            QQHelper.LoadStaticDataOptionsDropDown(ddValuation, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuotePropertyName.BlanketContentsValuationId, , Quote.LobType)
            '' Deductible - moved to LoadPIODeductible()
            'QQHelper.LoadStaticDataOptionsDropDown(ddDeductible, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteLocation, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuotePropertyName.DeductibleId, , Quote.LobType)
            ' Cause of Loss
            QQHelper.LoadStaticDataOptionsDropDown(ddCauseOfLoss, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuotePropertyName.BlanketContentsCauseOfLossTypeId, , Quote.LobType)
            ' Coinsurance
            QQHelper.LoadStaticDataOptionsDropDown(ddCoinsurance, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuotePropertyName.BlanketContentsCoinsuranceTypeId, , Quote.LobType)
        End If
        If IsNewCo() Then
            QQHelper.LoadStaticDataOptionsDropDownForStateAndCompany(ddEarthquakeDeductible, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuotePropertyInTheOpenRecord, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuotePropertyName.EarthquakeDeductibleId, Quote.QuickQuoteState, Quote.Company, QuickQuote.CommonObjects.QuickQuoteStaticDataOption.SortBy.None, Me.Quote.LobType)
        End If
        LoadPIODeductible()
    End Sub

    Private Sub PopulateDropdownDefaults()
        DropdownDefaults = New DropdownDefaults_Structure()
        Dim COL As String = "3"     ' Default to Special Form Including Theft
        Dim COINS As String = "5"   ' Default to 80%
        Dim VAL As String = "1"     ' Default to Replacement Cost
        Dim DED As String = "9"     ' Default to 500 changed to 1000 per task 62836
        If bldgQuote IsNot Nothing Then
            If IFM.VR.Common.Helpers.BlanketHelper_CPR_CPP.Has_CPRCPP_Blanket(bldgQuote) Then
                ' Get the defaults from the blanket on the policy
                GetBlanketProperties(COL, COINS, VAL, DED)
                DropdownDefaults.COLId = COL
                DropdownDefaults.COINSId = COINS
                DropdownDefaults.VALId = VAL
                DropdownDefaults.DEDId = DED
                Exit Sub
            End If
        End If
    End Sub

    Private Function GetBlanketProperties(ByRef CauseOfLossId As String, ByRef coInsuranceId As String, ByRef ValuationId As String, ByRef DeductibleId As String) As Boolean
        If bldgQuote IsNot Nothing Then
            ' Default Values
            CauseOfLossId = "3" ' Special form including theft
            coInsuranceId = "5" ' 80%
            ValuationId = "1"   ' Replacement Cost
            DeductibleId = "9"  ' 500 changed to 1000 per task 62836

            Dim blanketType As String = IFM.VR.Common.Helpers.BlanketHelper_CPR_CPP.Get_CPRCPP_Blanket_ID(bldgQuote)

            ' Get deductible id
            Select Case blanketType
                Case "1", "2"  ' combined & contents - use the building deductible
                    If RemoveBuildingLevelDeductibleHelper.IsRemoveBuildingLevelDeductibleAvailable(Quote) Then
                        If Quote.Locations IsNot Nothing AndAlso Quote.Locations.HasItemAtIndex(0) AndAlso Quote.Locations(0).DeductibleId <> "" Then
                            ' Use the building zero deductible if it's there
                            DeductibleId = bldgQuote.Locations(0).DeductibleId
                        Else
                            DeductibleId = "9"  ' No deductible on building zero, set to 500 changed to 1000 per task 62836
                        End If
                    Else
                        If Quote.Locations IsNot Nothing AndAlso Quote.Locations.HasItemAtIndex(0) AndAlso Quote.Locations(0).Buildings IsNot Nothing AndAlso Quote.Locations(0).Buildings.HasItemAtIndex(0) AndAlso Quote.Locations(0).Buildings(0).DeductibleId <> "" Then
                            ' Use the building zero deductible if it's there
                            DeductibleId = bldgQuote.Locations(0).Buildings(0).DeductibleId
                        Else
                            DeductibleId = "9"  ' No deductible on building zero, set to 500 changed to 1000 per task 62836
                        End If
                    End If
                    Exit Select
                Case "3" ' Property Only - Use the property deductible
                    If RemoveBuildingLevelDeductibleHelper.IsRemoveBuildingLevelDeductibleAvailable(Quote) Then
                        If Quote.Locations IsNot Nothing AndAlso Quote.Locations.HasItemAtIndex(0) AndAlso Quote.Locations(0).DeductibleId <> "" Then
                            ' Use the building zero deductible if it's there
                            DeductibleId = bldgQuote.Locations(0).DeductibleId
                        Else
                            DeductibleId = "9"  ' No deductible on building zero, set to 500 changed to 1000 per task 62836 BD
                        End If
                    Else
                        If Quote.Locations IsNot Nothing AndAlso Quote.Locations.HasItemAtIndex(0) AndAlso Quote.Locations(0).Buildings IsNot Nothing AndAlso Quote.Locations(0).Buildings.HasItemAtIndex(0) AndAlso Quote.Locations(0).Buildings(0).PersPropCov_DeductibleId <> "" Then
                            ' Use the building zero deductible if it's there
                            DeductibleId = bldgQuote.Locations(0).Buildings(0).PersPropCov_DeductibleId
                        Else
                            DeductibleId = "9"  ' No deductible on building zero, set to 500 changed to 1000 per task 62836 BD
                        End If
                    End If
                    Exit Select
                Case Else
                    ' No blanket - use the deductible on the item if possible
                    If MyProperty.Deductible <> "" Then DeductibleId = MyProperty.DeductibleId
                    Exit Select
            End Select

            ' Get Cause of Loss, Coinsurance & Valuation id's
            Select Case blanketType
                Case "1"  ' Building & Contents
                    If bldgQuote.BlanketBuildingAndContentsCauseOfLossTypeId <> "" Then CauseOfLossId = bldgQuote.BlanketBuildingAndContentsCauseOfLossTypeId
                    If bldgQuote.BlanketBuildingAndContentsCoinsuranceTypeId <> "" Then coInsuranceId = bldgQuote.BlanketBuildingAndContentsCoinsuranceTypeId
                    If bldgQuote.BlanketBuildingAndContentsValuationId <> "" Then ValuationId = bldgQuote.BlanketBuildingAndContentsValuationId
                    Exit Select
                Case "2" ' Building Only
                    If bldgQuote.BlanketBuildingCauseOfLossTypeId <> "" Then CauseOfLossId = bldgQuote.BlanketBuildingCauseOfLossTypeId
                    If bldgQuote.BlanketBuildingCoinsuranceTypeId <> "" Then coInsuranceId = bldgQuote.BlanketBuildingCoinsuranceTypeId
                    If bldgQuote.BlanketBuildingValuationId <> "" Then ValuationId = bldgQuote.BlanketBuildingValuationId
                    Exit Select
                Case "3"  ' Contents Only
                    If bldgQuote.BlanketContentsCauseOfLossTypeId <> "" Then CauseOfLossId = bldgQuote.BlanketContentsCauseOfLossTypeId
                    If bldgQuote.BlanketContentsCoinsuranceTypeId <> "" Then coInsuranceId = bldgQuote.BlanketContentsCoinsuranceTypeId
                    If bldgQuote.BlanketContentsValuationId <> "" Then ValuationId = bldgQuote.BlanketContentsValuationId
                    Exit Select
                Case Else
                    ' No blanket
                    If MyProperty.CauseOfLossTypeId <> "" Then CauseOfLossId = MyProperty.CauseOfLossTypeId
                    If MyProperty.CoinsuranceTypeId <> "" Then coInsuranceId = MyProperty.CoinsuranceTypeId
                    If MyProperty.ValuationId <> "" Then ValuationId = MyProperty.ValuationId
                    Exit Select
            End Select
        End If

        Return True
    End Function

    Private Function QuoteHasAgreedAmount() As Boolean
        If bldgQuote.HasBlanketBuilding Then
            Return bldgQuote.BlanketBuildingIsAgreedValue
        ElseIf bldgQuote.HasBlanketContents Then
            Return bldgQuote.BlanketContentsIsAgreedValue
        ElseIf bldgQuote.HasBlanketBuildingAndContents Then
            Return bldgQuote.BlanketBuildingAndContentsIsAgreedValue
        Else
            Return False
        End If
    End Function

    ''' <summary>
    ''' Returns true if the quote has blanket coverage, false if not
    ''' Sets the passed BlanketNum string to 0=N/A, 1=Contents, 2=Building, 3=Combined
    ''' </summary>
    ''' <param name="BlanketNum"></param>
    ''' <returns></returns>
    Private Function QuoteHasBlanket(ByRef BlanketNum As String) As Boolean
        If bldgQuote.HasBlanketBuilding Then
            BlanketNum = "2"
            Return True
        ElseIf bldgQuote.HasBlanketContents Then
            BlanketNum = "3"
            Return True
        ElseIf bldgQuote.HasBlanketBuildingAndContents Then
            BlanketNum = "1"
            Return True
        Else
            BlanketNum = "0"
            Return False
        End If
    End Function

    Public Overrides Sub Populate()
        Dim err As String = Nothing
        Dim BlanketNum As String = Nothing

        LoadStaticData()
        ClearInputFields()

        divWindHail.Attributes.Add("style", "display:none")

        'Updated 12/08/2021 for CPP Endorsements Task 66977 MLW
        If Not IsQuoteReadOnly() Then
            ' Set the defaults
            SetFromValue(ddCauseOfLoss, "3")
            SetFromValue(ddCoinsurance, "5")
            SetFromValue(ddValuation, "1")
            SetFromValue(ddDeductible, "9") 'Was 500, changed to 1000 per task 62836
        End If

        If MyProperty IsNot Nothing Then
            'Remove Replacement Cost from Valuation if all buildings property classes on location are dwelling classification (0196, 0197, 0198)
            If ValuationACVHelper.IsValuationACVAvailable(Quote) Then
                Dim hasLocationWithAllBuildingsWithDwellingClassCode As Boolean = False
                If MyLocation IsNot Nothing Then
                    hasLocationWithAllBuildingsWithDwellingClassCode = ValuationACVHelper.CheckAllBuildingsHaveDwellingClass(MyLocation)
                    If hasLocationWithAllBuildingsWithDwellingClassCode Then
                        If MyProperty.ValuationId = "1" Then
                            MyProperty.ValuationId = "2" 'Actual Cash Value (ACV)
                            ValuationACVHelper.ShowValuationRCPopupMessage(Me.Page)
                        End If
                        Dim removeRC As ListItem = ddValuation.Items.FindByValue("1")
                        '1=Replacement Cost
                        If removeRC IsNot Nothing Then
                            ddValuation.Items.Remove(removeRC)
                        End If
                    End If
                End If
            End If

            If LocationWindHailHelper.IsLocationWindHailAvailable(Quote) Then
                divWindHail.Attributes.Add("style", "display:''")
                If MyLocation IsNot Nothing Then
                    If IsNullEmptyorWhitespace(MyLocation.WindHailDeductibleLimitId) OrElse MyLocation.WindHailDeductibleLimitId = "0" Then
                        '0 = N/A
                        chkWindHail.Checked = False
                        chkWindHail.Enabled = False
                    Else
                        chkWindHail.Enabled = True
                        Select Case MyProperty.OptionalWindstormOrHailDeductibleId
                            Case "32", "33", "34"
                                '32=1%, 33=2%, 34=5%
                                chkWindHail.Checked = True
                            Case Else
                                chkWindHail.Checked = False
                        End Select
                    End If
                End If
            End If


            UpdateHeader()
            txtClassCodeId.Text = MyProperty.SpecialClassCodeTypeId
            If IsNewCo() AndAlso IsNullEmptyorWhitespace(MyProperty.SpecialClassCodeTypeId) = False Then
                DisablePIOEQDeductibleOption(MyProperty.SpecialClassCodeTypeId)
            End If
            If MyProperty.SpecialClassCodeType.ToUpper <> "N/A" Then txtSpecialClassCode.Text = MyProperty.SpecialClassCodeType Else txtSpecialClassCode.Text = String.Empty
            txtCoverageLimit.Text = MyProperty.Limit
            txtDescription.Text = MyProperty.Description
            SetFromValue(ddValuation, MyProperty.ValuationId, "0")

            If QuoteHasBlanket(BlanketNum) Then
                ' QUOTE HAS BLANKET
                If BlanketNum = "3" Then
                    ' Hide the blanket chckbox if property only is selected
                    divBlanketCheckbox.Attributes.Add("style", "display:none")
                Else
                    ' Show the blanket checkbox
                    divBlanketCheckbox.Attributes.Add("style", "display:''")
                End If
                ' Populate the blanket values
                chkIncludedInBlanketRating.Checked = MyProperty.IncludedInBlanketCoverage
                If chkIncludedInBlanketRating.Checked Then
                    ' When blanket rating is checked disable the Cause of Loss, Coinsurance, Valuation and Deductible dropdowns
                    ddCauseOfLoss.Attributes.Add("disabled", "True")
                    ddCoinsurance.Attributes.Add("disabled", "True")
                    ddValuation.Attributes.Add("disabled", "True")
                    ddDeductible.Attributes.Add("disabled", "True")
                    ' When blanket is checked agreed amount must match what's on the coverages page and is not editable
                    If QuoteHasAgreedAmount() Then
                        chkAgreedAmount.Checked = True
                        chkAgreedAmount.Attributes.Add("disabled", "True")
                    Else
                        chkAgreedAmount.Checked = False
                        chkAgreedAmount.Attributes.Add("disabled", "True")
                    End If
                Else
                    ' When blanket rating is NOT checked ENable the Cause of Loss, Coinsurance, Valuation and Deductible dropdowns
                    ddCauseOfLoss.Attributes.Remove("disabled")
                    ddCoinsurance.Attributes.Remove("disabled")
                    ddValuation.Attributes.Remove("disabled")
                    ddDeductible.Attributes.Remove("disabled")
                    ' When blanket is not checked agreed amopunt is enabled and set to whatever is on the PIO item
                    chkAgreedAmount.Attributes.Remove("disabled")
                    chkAgreedAmount.Checked = MyProperty.IsAgreedValue
                End If
            Else
                ' QUOTE DOES NOT HAVE BLANKET
                ' Hide the blanket checkbox
                divBlanketCheckbox.Attributes.Add("style", "display:none")
                chkIncludedInBlanketRating.Checked = False
                ' When no blanket on quote enable the Cause of Loss, Coinsurance, Valuation and Deductible dropdowns
                ddCauseOfLoss.Attributes.Remove("disabled")
                ddCoinsurance.Attributes.Remove("disabled")
                ddValuation.Attributes.Remove("disabled")
                tdDeductibleInTheOpen.Attributes.Add("style", "display:''")
                ddDeductible.Attributes.Remove("disabled")
                ' When no blanket on quote agreed amopunt is enabled and set to whatever is on the PIO item
                chkAgreedAmount.Attributes.Remove("disabled")
                chkAgreedAmount.Checked = MyProperty.IsAgreedValue
            End If

            If chkIncludedInBlanketRating.Checked OrElse chkAgreedAmount.Checked Then
                trBlanketInfoRow.Attributes.Add("style", "display:''")
            Else
                trBlanketInfoRow.Attributes.Add("style", "display:none")
            End If

            PopulatePIODeductible()
            'WebHelper_Personal.SetDropDownValue_ForceDiamondValue(ddDeductible, MyProperty.DeductibleId, QuickQuoteClassName.QuickQuoteLocation, QuickQuotePropertyName.DeductibleId)
            '''Updated 12/02/2021 for CPP Endorsements Task 67023 MLW
            ''If IsQuoteReadOnly() Then
            ''    WebHelper_Personal.SetDropDownValue_ForceDiamondValue(ddDeductible, MyProperty.DeductibleId, QuickQuoteClassName.QuickQuoteLocation, QuickQuotePropertyName.DeductibleId)
            ''Else
            ''    SetFromValue(ddDeductible, MyProperty.DeductibleId, "0")
            ''End If
            '''SetFromValue(ddDeductible, MyProperty.DeductibleId, "0")

            If chkIncludedInBlanketRating.Checked Then
                If bldgQuote.HasBlanketBuilding Then
                    SetFromValue(ddCauseOfLoss, Quote.BlanketBuildingCauseOfLossTypeId)
                ElseIf bldgQuote.HasBlanketContents Then
                    SetFromValue(ddCauseOfLoss, Quote.BlanketContentsCauseOfLossTypeId)
                Else
                    SetFromValue(ddCauseOfLoss, Quote.BlanketBuildingAndContentsCauseOfLossTypeId)
                End If
            Else
                SetFromValue(ddCauseOfLoss, MyProperty.CauseOfLossTypeId, "0")
            End If

            chkEarthquake.Checked = MyProperty.EarthquakeApplies
            If IsNewCo() AndAlso chkEarthquake.Checked Then
                trEarthquakeDeductible.Attributes.Add("style", "display:''")
                SetFromValue(ddEarthquakeDeductible, MyProperty.EarthquakeDeductibleId, "-1")
            Else
                trEarthquakeDeductible.Attributes.Add("style", "display:none")
            End If
            'Updated 12/02/2021 for CPP Endorsements Task 67023 MLW
            If IsQuoteReadOnly() Then
                WebHelper_Personal.SetDropDownValue_ForceDiamondValue(ddCoinsurance, MyProperty.CoinsuranceTypeId, QuickQuoteClassName.QuickQuoteObject, QuickQuotePropertyName.BlanketContentsCoinsuranceTypeId)
            Else
                SetFromValue(ddCoinsurance, MyProperty.CoinsuranceTypeId, "0")
            End If
            'SetFromValue(ddCoinsurance, MyProperty.CoinsuranceTypeId, "0")

            If RemoveBuildingLevelDeductibleHelper.IsRemoveBuildingLevelDeductibleAvailable(Quote) Then
                tdDeductibleInTheOpen.Attributes.Add("style", "display:none")
                If MyLocation IsNot Nothing Then
                    MyProperty.DeductibleId = MyLocation.DeductibleId
                End If
            Else
                WebHelper_Personal.SetDropDownValue_ForceDiamondValue(ddDeductible, MyProperty.DeductibleId, QuickQuoteClassName.QuickQuoteLocation, QuickQuotePropertyName.DeductibleId)
                tdDeductibleInTheOpen.Attributes.Add("style", "display''")
            End If


        End If

        Me.PopulateChildControls()

        Exit Sub
    End Sub

    Private Sub DisablePIOEQDeductibleOption(specialClassCodeTypeId As String)
        If specialClassCodeTypeId.NotEqualsAny("50", "51", "52", "53", "98", "99", "100", "101") Then 'ids are for swimming pool class codes
            'disable all class codes that are not swimming pools for NewCo only
            If ddEarthquakeDeductible.Items.Contains(New ListItem("5%", "34")) Then
                ddEarthquakeDeductible.Items.FindByValue("34").Attributes.Add("disabled", "disabled")
            End If
        Else
            If ddEarthquakeDeductible.Items.Contains(New ListItem("5%", "34")) Then
                ddEarthquakeDeductible.Items.FindByValue("34").Attributes.Remove("disabled")
            End If
        End If
    End Sub

    Public Overrides Function Save() As Boolean
        If MyProperty IsNot Nothing Then
            'If txtClassCodeId.Text <> "" Then
            MyProperty.SpecialClassCodeTypeId = txtClassCodeId.Text
            MyProperty.Limit = txtCoverageLimit.Text
            MyProperty.Description = txtDescription.Text
            MyProperty.IncludedInBlanketCoverage = chkIncludedInBlanketRating.Checked
            If MyProperty.IncludedInBlanketCoverage Then
                ' INCLUDED IN BLANKET
                ' Since the Cause of Loss, Coinsurance, Deductible and Valuation values are always set to 
                ' specific values when 'Included in Blanket' is checked, we don't need to go through the 
                ' whole hidden-field debacle when changing the values in script, just set everything on save
                ' and we're good.
                Dim dedId As String = ""
                Dim coinsId As String = ""
                Dim valId As String = ""
                Dim colId As String = ""
                GetBlanketProperties(colId, coinsId, valId, dedId)
                MyProperty.CauseOfLossTypeId = colId
                ddCauseOfLoss.SelectedValue = colId
                If RemoveBuildingLevelDeductibleHelper.IsRemoveBuildingLevelDeductibleAvailable(Quote) Then
                    MyProperty.DeductibleId = MyLocation.DeductibleId
                Else
                    MyProperty.DeductibleId = dedId
                    ddDeductible.SelectedValue = dedId
                End If
                MyProperty.CoinsuranceTypeId = coinsId
                ddCoinsurance.SelectedValue = coinsId
                MyProperty.ValuationId = valId
                ddValuation.SelectedValue = valId
                ddCauseOfLoss.Attributes.Add("disabled", "true")
                ddCoinsurance.Attributes.Add("disabled", "true")
                ddValuation.Attributes.Add("disabled", "true")
                ddDeductible.Attributes.Add("disabled", "true")
            Else
                ' NOT INCLUDED IN BLANKET
                MyProperty.CauseOfLossTypeId = ddCauseOfLoss.SelectedValue
                If RemoveBuildingLevelDeductibleHelper.IsRemoveBuildingLevelDeductibleAvailable(Quote) Then
                    MyProperty.DeductibleId = MyLocation.DeductibleId
                Else
                    MyProperty.DeductibleId = ddDeductible.SelectedValue
                End If
                MyProperty.CoinsuranceTypeId = ddCoinsurance.SelectedValue
                MyProperty.ValuationId = ddValuation.SelectedValue
                ddCauseOfLoss.Attributes.Remove("disabled")
                ddCoinsurance.Attributes.Remove("disabled")
                ddValuation.Attributes.Remove("disabled")
                ddDeductible.Attributes.Remove("disabled")
            End If
            If RemoveBuildingLevelDeductibleHelper.IsRemoveBuildingLevelDeductibleAvailable(Quote) Then
                MyProperty.OptionalTheftDeductibleId = MyLocation.DeductibleId ' Added 02/18/2025 BD
                If LocationWindHailHelper.IsLocationWindHailAvailable(Quote) AndAlso chkWindHail.Checked Then
                    MyProperty.OptionalWindstormOrHailDeductibleId = MyLocation.WindHailDeductibleLimitId
                Else
                    'Fixed dollar amount
                    MyProperty.OptionalWindstormOrHailDeductibleId = MyLocation.DeductibleId ' Added 02/18/2025 BD
                End If
            Else
                MyProperty.OptionalTheftDeductibleId = ddDeductible.SelectedValue ' copied from old look & feel
                If LocationWindHailHelper.IsLocationWindHailAvailable(Quote) AndAlso chkWindHail.Checked Then
                    MyProperty.OptionalWindstormOrHailDeductibleId = MyLocation.WindHailDeductibleLimitId
                Else
                    MyProperty.OptionalWindstormOrHailDeductibleId = ddDeductible.SelectedValue ' copied from old look & feel
                End If
            End If
            MyProperty.EarthquakeApplies = chkEarthquake.Checked
            If IsNewCo() AndAlso chkEarthquake.Checked Then
                MyProperty.EarthquakeDeductibleId = ddEarthquakeDeductible.SelectedValue
            Else
                MyProperty.EarthquakeDeductibleId = ""
            End If
            MyProperty.ConstructionTypeId = "1"  ' frame - copied from old look & feel
            MyProperty.RatingTypeId = "3" ' Special Class Rate - copied from old look & feel
            MyProperty.ProtectionClassId = MyLocation.ProtectionClassId 'copied from old look & feel
            MyProperty.IsAgreedValue = chkAgreedAmount.Checked
        End If
        UpdateHeader()

        Me.SaveChildControls()

        Populate()

        Return True
    End Function

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        MyBase.ValidateControl(valArgs)
        Dim accordList As List(Of VRAccordionTogglePair) = Me.MyAccordionList ' just to cache it
        Me.ValidationHelper.GroupName = "Location #" & LocationIndex + 1 & ", Property #" & PropertyIndex + 1.ToString

        If txtClassCodeId.Text = "" Then
            'Me.ValidationHelper.AddError("No Class Code was selected.")
            Me.ValidationHelper.AddError(txtSpecialClassCode, "Missing Special Class Code", accordList)
        End If

        If txtDescription.Text.Trim = "" Then
            Me.ValidationHelper.AddError(txtDescription, "Missing Description", accordList)
        Else
            If txtDescription.Text.Length < 4 Then
                Me.ValidationHelper.AddError(txtDescription, "Description must be more than 4 characters in length", accordList)
            End If
        End If
        If txtCoverageLimit.Text.Trim = "" Then
            Me.ValidationHelper.AddError(txtCoverageLimit, "Missing Coverage Limit", accordList)
        End If

        If IsNewCo() AndAlso chkEarthquake.Checked Then
            If ddEarthquakeDeductible.SelectedValue = "" OrElse ddEarthquakeDeductible.SelectedValue = "-1" OrElse ddEarthquakeDeductible.SelectedValue = "0" Then
                Me.ValidationHelper.AddError(ddEarthquakeDeductible, "Missing Earthquake Deductible Options", accordList)
            End If
        End If

        Me.ValidateChildControls(valArgs)

        Exit Sub
    End Sub

    Private Sub ClearInputFields()
        txtSpecialClassCode.Text = ""
        txtClassCodeId.Text = ""
        txtDescription.Text = ""
        txtCoverageLimit.Text = ""
        chkIncludedInBlanketRating.Checked = False
        chkEarthquake.Checked = False
        chkAgreedAmount.Checked = False
        ' Set dropdowns to default values on clear
        SetFromValue(ddCauseOfLoss, "3")
        SetFromValue(ddCoinsurance, "5")
        SetFromValue(ddValuation, "1")
        SetFromValue(ddDeductible, "9") ' Was 500, changed to 1000 per task 62836

        UpdateHeader()
        Exit Sub
    End Sub

    Public Sub UpdateHeader()
        lblAccordHeader.Text = "Property"
        If MyProperty IsNot Nothing Then
            Dim dsc As String = MyProperty.Description.ToUpper
            If dsc.Length > 20 Then dsc = MyProperty.Description.Substring(0, 20).ToUpper & "..."
            lblAccordHeader.Text = "Property # " & PropertyIndex + 1.ToString & " - " & dsc
        End If
    End Sub

    ''' <summary>
    ''' Called from the PIO list when agreed amount changes on the coverages page
    ''' </summary>
    Public Sub HandleAgreedAmountChange(ByVal newvalue As Boolean)
        If newvalue Then
            If chkIncludedInBlanketRating.Checked Then
                chkAgreedAmount.Checked = True
                chkAgreedAmount.Attributes.Add("disabled", "true")
            End If
        Else
            chkAgreedAmount.Attributes.Remove("disabled")
            chkAgreedAmount.Checked = MyProperty.IsAgreedValue
        End If
    End Sub

    ''' <summary>
    ''' Called from the PIO list when the blanket deductible changes on the coverages page
    ''' </summary>
    Public Sub HandleBlanketDeductibleChange()
        If chkIncludedInBlanketRating.Checked Then
            If bldgQuote IsNot Nothing AndAlso Quote.Locations IsNot Nothing AndAlso Quote.Locations.HasItemAtIndex(0) AndAlso Quote.Locations(0).Buildings IsNot Nothing AndAlso Quote.Locations(0).Buildings.HasItemAtIndex(0) Then
                If RemoveBuildingLevelDeductibleHelper.IsRemoveBuildingLevelDeductibleAvailable(Quote) Then
                    If bldgQuote.HasBlanketBuildingAndContents OrElse bldgQuote.HasBlanketBuilding Then
                        'ddDeductible.SelectedValue = Quote.Locations(0).Buildings(0).DeductibleId
                        WebHelper_Personal.SetDropDownValue_ForceDiamondValue(Me.ddDeductible, Quote.Locations(0).DeductibleId, QuickQuoteClassName.QuickQuoteLocation, QuickQuotePropertyName.DeductibleId)
                    ElseIf bldgQuote.HasBlanketContents Then
                        'ddDeductible.SelectedValue = Quote.Locations(0).Buildings(0).PersPropCov_DeductibleId
                        'WebHelper_Personal.SetDropDownValue_ForceDiamondValue(Me.ddDeductible, Quote.Locations(0).Buildings(0).PersPropCov_DeductibleId, QuickQuoteClassName.QuickQuoteLocation, QuickQuotePropertyName.DeductibleId)
                        WebHelper_Personal.SetDropDownValue_ForceDiamondValue(Me.ddDeductible, Quote.Locations(0).DeductibleId, QuickQuoteClassName.QuickQuoteLocation, QuickQuotePropertyName.PersPropCov_DeductibleId)
                    End If
                Else
                    If bldgQuote.HasBlanketBuildingAndContents OrElse bldgQuote.HasBlanketBuilding Then
                        'ddDeductible.SelectedValue = Quote.Locations(0).Buildings(0).DeductibleId
                        WebHelper_Personal.SetDropDownValue_ForceDiamondValue(Me.ddDeductible, Quote.Locations(0).Buildings(0).DeductibleId, QuickQuoteClassName.QuickQuoteLocation, QuickQuotePropertyName.DeductibleId)
                    ElseIf bldgQuote.HasBlanketContents Then
                        'ddDeductible.SelectedValue = Quote.Locations(0).Buildings(0).PersPropCov_DeductibleId
                        'WebHelper_Personal.SetDropDownValue_ForceDiamondValue(Me.ddDeductible, Quote.Locations(0).Buildings(0).PersPropCov_DeductibleId, QuickQuoteClassName.QuickQuoteLocation, QuickQuotePropertyName.DeductibleId)
                        WebHelper_Personal.SetDropDownValue_ForceDiamondValue(Me.ddDeductible, Quote.Locations(0).Buildings(0).PersPropCov_DeductibleId, QuickQuoteClassName.QuickQuoteBuilding, QuickQuotePropertyName.PersPropCov_DeductibleId)
                    End If
                End If
                PopulateDropdownDefaults()
            End If
            End If
    End Sub

    Public Sub LoadPIODeductible()
        If Quote IsNot Nothing Then
            ' Deductible
            QQHelper.LoadStaticDataOptionsDropDown(ddDeductible, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteLocation, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuotePropertyName.DeductibleId, , Quote.LobType)
            If CPRRemovePropDedBelow1k.IsPropertyDeductibleBelow1kAvailable(Quote) Then
                Dim Item500 = New ListItem("500", "8")
                ddDeductible.Items.Remove(Item500)
            End If
        End If
    End Sub

    Public Sub PopulatePIODeductible()
        If Quote IsNot Nothing AndAlso MyProperty IsNot Nothing Then
            WebHelper_Personal.SetDropDownValue_ForceDiamondValue(ddDeductible, MyProperty.DeductibleId, QuickQuoteClassName.QuickQuoteLocation, QuickQuotePropertyName.DeductibleId)
        End If
    End Sub

#End Region

#Region "Events"

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Me.ctl_PIOClassCodeLookup.txtClassCodeId = Me.txtSpecialClassCode.ClientID
        Me.ctl_PIOClassCodeLookup.txtID = Me.txtClassCodeId.ClientID
        If IsPostBack AndAlso IsNewCo() AndAlso IsNullEmptyorWhitespace(txtClassCodeId.Text) = False Then
            DisablePIOEQDeductibleOption(txtClassCodeId.Text)
        End If
        Exit Sub
    End Sub

    Private Sub lnkSave_Click(sender As Object, e As EventArgs) Handles lnkSave.Click
        Me.Save_FireSaveEvent()
        Exit Sub
    End Sub

    Private Sub lnkClear_Click(sender As Object, e As EventArgs) Handles lnkClear.Click
        ClearInputFields()
        If IsNewCo() Then
            ddEarthquakeDeductible.SelectedIndex = -1
        End If
        Save_FireSaveEvent(False)
    End Sub

    Private Sub lnkDelete_Click(sender As Object, e As EventArgs) Handles lnkDelete.Click
        RaiseEvent PIODeleteRequested(LocationIndex, PropertyIndex)
    End Sub

    Private Sub btnClassCodeLookup_Click(sender As Object, e As EventArgs) Handles btnClassCodeLookup.Click
        Me.ctl_PIOClassCodeLookup.Show()
    End Sub

#End Region


End Class