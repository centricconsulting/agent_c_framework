Imports QuickQuote.CommonMethods.QuickQuoteHelperClass
Imports QuickQuote.CommonObjects
Imports IFM.VR.Web.Helpers

Public Class cov_Transportation
    Inherits VRControlBase

    Private Property selectedOption As Boolean

    Public Overrides Sub LoadStaticData()
        QQHelper.LoadStaticDataOptionsDropDown(Me.trDeductible, QuickQuoteClassName.QuickQuoteObject, QuickQuotePropertyName.TransportationCatastropheDeductibleId, QuickQuoteStaticDataOption.SortBy.TextAscending, Me.Quote.LobType)
        QQHelper.LoadStaticDataOptionsDropDown(Me.trDeductible_FoodManuf, QuickQuoteClassName.QuickQuoteObject, QuickQuotePropertyName.TransportationCatastropheDeductibleId, QuickQuoteStaticDataOption.SortBy.TextAscending, Me.Quote.LobType)
    End Sub

    ''' <summary>
    ''' Returns true if the quote should use the CPP Food Manufacturer's Enhancement format.
    ''' This is true when:
    ''' - The quote LOB is CPP.
    ''' - Food Manufacturers Enhancement is enabled.
    ''' - The quote effective date or the date passed to this function is equal to or greater than
    '''   the food manufacturers enhancement effective date.
    ''' - The quote has the food manufacturers enhancement.
    ''' - If you do not pass an effective date it will compare against the quote effective date.
    ''' </summary>
    ''' <param name="EffDt"></param>
    ''' <returns></returns>
    Private ReadOnly Property UseCPPFoodManufacturersFormat(Optional ByVal EffDt As String = Nothing) As Boolean
        Get
            Dim DateToCheck As String = Nothing
            If Quote IsNot Nothing AndAlso QQHelper.IsValidDateString(Quote.EffectiveDate) Then
                If QQHelper.IsValidDateString(EffDt) Then
                    DateToCheck = EffDt
                Else
                    DateToCheck = Quote.EffectiveDate
                End If
                If Quote.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialPackage _
                        AndAlso CDate(DateToCheck) >= CDate(QuickQuote.CommonMethods.QuickQuoteHelperClass.FoodManufacturers_EffectiveDate) _
                        AndAlso QuoteHasFoodManufacturersEnhancement() _
                        AndAlso (QuickQuote.CommonMethods.QuickQuoteHelperClass.FoodManufacturers_Enabled() = True _
                            OrElse QuickQuote.CommonMethods.QuickQuoteHelperClass.FoodManufacturers_MaintainCoverageWhenDisabled() = True) Then
                    Return True
                End If
            End If
            Return False
        End Get
    End Property

    Private ReadOnly Property QuoteHasFoodManufacturersEnhancement() As Boolean
        Get
            If Quote IsNot Nothing Then
                If QQHelper.IsValidDateString(Quote.EffectiveDate) AndAlso CDate(Quote.EffectiveDate) >= CDate(QuickQuote.CommonMethods.QuickQuoteHelperClass.FoodManufacturers_EffectiveDate) Then
                    If GoverningStateQuote.HasFoodManufacturersEnhancement Then Return True
                End If
            End If
            Return False
        End Get
    End Property

    ''' <summary>
    ''' Handles effective date change.  Same as populate but uses the new effective date to 
    ''' determine the form layout
    ''' </summary>
    ''' <param name="qqTranType"></param>
    ''' <param name="newEffectiveDate"></param>
    ''' <param name="oldEffectiveDate"></param>
    Public Sub HandleEffectiveDateChange(qqTranType As QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteTransactionType, newEffectiveDate As String, oldEffectiveDate As String)
        FormatControlBasedOnEffectiveDate(newEffectiveDate)
        Exit Sub
    End Sub

    ''' <summary>
    ''' Repopulate based on the passed effective date.  
    ''' Needed both for populate and when you cross the effective date for Food Manufacturers, since it uses a different page format for Food Manufacturers vs everything else.
    ''' </summary>
    ''' <param name="EffDate"></param>
    Private Sub FormatControlBasedOnEffectiveDate(ByVal EffDate As String)
        If trDeductible.Items Is Nothing OrElse trDeductible.Items.Count <= 0 Then LoadStaticData()

        ' Display the appropriate Transportation coverage format
        cpSubGroup_FoodManuf.Attributes.Add("style", "display:none")
        cpSubGroup_Normal.Attributes.Add("style", "display:none")
        If UseCPPFoodManufacturersFormat(EffDate) Then
            ' Use the CPP Food Manufacturers layout
            cpSubGroup_FoodManuf.Attributes.Add("style", "display:''")
        Else
            ' Use the normal layout (non-cpp food manufacturers)
            cpSubGroup_Normal.Attributes.Add("style", "display:''")
        End If

        If Quote IsNot Nothing Then
            Dim useFoodManufacturers As Boolean = UseCPPFoodManufacturersFormat(EffDate)
            'Updated 12/15/2021 for CPP Endorsements Task 66800 MLW
            'If useFoodManufacturers = True Then
            If useFoodManufacturers = True AndAlso Not IsQuoteReadOnly() Then
                If QQHelper.IntegerForString(GoverningStateQuote.TransportationAnyOneOwnedVehicleLimit) < 50000 Then
                    GoverningStateQuote.TransportationAnyOneOwnedVehicleLimit = "50,000" ' Always 50,000 for Food Manufacturers
                End If
                If QQHelper.IntegerForString(GoverningStateQuote.TransportationCatastropheDeductibleId) = 0 Then
                    GoverningStateQuote.TransportationCatastropheDeductibleId = "8"      ' $500 is the default
                End If
            End If
            If String.IsNullOrWhiteSpace(GoverningStateQuote.TransportationCatastropheDeductibleId) = False _
            AndAlso (String.IsNullOrWhiteSpace(GoverningStateQuote.TransportationAnyOneOwnedVehicleLimit) = False _
            OrElse String.IsNullOrWhiteSpace(GoverningStateQuote.TransportationAnyOneOwnedVehicleNumberOfVehicles) = False _
            OrElse String.IsNullOrWhiteSpace(GoverningStateQuote.TransportationCatastropheDescription) = False) Then
                ' Quote has transportation
                If useFoodManufacturers Then
                    Me.chkTransportation_FoodManuf.Checked = True
                    Me.chkTransportation_FoodManuf.Enabled = False ' Transportation can NOT be removed when food manufacturers is on the quote
                    Me.divTransportationDetail_FoodManuf.Attributes.Add("style", "display:''")
                    If String.IsNullOrWhiteSpace(GoverningStateQuote.TransportationCatastropheDeductibleId) = False Then
                        'Updated 12/15/2021 for CPP Endorsements Task 66800 MLW
                        If IsQuoteReadOnly() Then
                            WebHelper_Personal.SetDropDownValue_ForceDiamondValue(Me.trDeductible_FoodManuf, GoverningStateQuote.TransportationCatastropheDeductibleId, QuickQuoteClassName.QuickQuoteObject, QuickQuotePropertyName.TransportationCatastropheDeductibleId)
                        Else
                            WebHelper_Personal.SetdropDownFromValue(Me.trDeductible_FoodManuf, GoverningStateQuote.TransportationCatastropheDeductibleId)
                        End If
                    End If
                    Me.txtDescription_FoodManuf.Text = GoverningStateQuote.TransportationCatastropheDescription
                    ' Limit per vehicle, increased limit, total limit per vehicle
                    Me.txtLimitPerVehicle_FoodManuf.Text = "50,000"   ' Always 50,000 for food manufacturers limit
                    If IsNumeric(GoverningStateQuote.TransportationAnyOneOwnedVehicleLimit) _
                        AndAlso CInt(GoverningStateQuote.TransportationAnyOneOwnedVehicleLimit) > 50000 Then
                        Dim IncLim As Integer = CInt(GoverningStateQuote.TransportationAnyOneOwnedVehicleLimit) - 50000
                        txtIncreasedLimit_FoodManuf.Text = Format(CDec(IncLim), "###,###,##0")
                    Else
                        txtIncreasedLimit_FoodManuf.Text = String.Empty
                    End If
                    txtTotalVehicleLimit_FoodManuf.Text = GoverningStateQuote.TransportationAnyOneOwnedVehicleLimit
                    ' Number of vehicles, cat limit
                    Me.txtNumOfVehicles_FoodManuf.Text = GoverningStateQuote.TransportationAnyOneOwnedVehicleNumberOfVehicles
                    Me.txtCatLimit_FoodManuf.Text = GoverningStateQuote.TransportationCatastropheLimit
                Else
                    Me.chkTransportation.Checked = True
                    Me.chkTransportation.Enabled = True
                    If String.IsNullOrWhiteSpace(GoverningStateQuote.TransportationCatastropheDeductibleId) = False Then
                        'Updated 12/15/2021 for CPP Endorsements Task 66800 MLW
                        If IsQuoteReadOnly() Then
                            WebHelper_Personal.SetDropDownValue_ForceDiamondValue(Me.trDeductible, GoverningStateQuote.TransportationCatastropheDeductibleId, QuickQuoteClassName.QuickQuoteObject, QuickQuotePropertyName.TransportationCatastropheDeductibleId)
                        Else
                            WebHelper_Personal.SetdropDownFromValue(Me.trDeductible, GoverningStateQuote.TransportationCatastropheDeductibleId)
                        End If
                    End If
                    Me.txtDescription.Text = GoverningStateQuote.TransportationCatastropheDescription
                    Me.txtLimitPerVehicle.Text = GoverningStateQuote.TransportationAnyOneOwnedVehicleLimit
                    Me.txtNumOfVehicles.Text = GoverningStateQuote.TransportationAnyOneOwnedVehicleNumberOfVehicles
                    Me.txtCatLimit.Text = GoverningStateQuote.TransportationCatastropheLimit
                End If
            Else
                ' Quote does not have transportation
                Me.chkTransportation.Checked = False
                Me.chkTransportation_FoodManuf.Checked = False
            End If
        End If
        If selectedOption Then
            If UseCPPFoodManufacturersFormat(EffDate) Then
                chkTransportation_FoodManuf.Checked = selectedOption
            Else
                chkTransportation.Checked = selectedOption
            End If
        End If
    End Sub

    Public Overrides Sub Populate()
        FormatControlBasedOnEffectiveDate(Quote.EffectiveDate)
        Exit Sub
    End Sub

    Public Overrides Sub AddScriptAlways()
    End Sub

    Public Overrides Sub AddScriptWhenRendered()
        ' CPP Food Manufacturing - calculate total vehicle limit when typing in increased limit
        Me.VRScript.CreateJSBinding(txtIncreasedLimit_FoodManuf.ClientID, "onkeyup", "Cpp.TransportationIncreasedLimitChanged('" & txtLimitPerVehicle_FoodManuf.ClientID & "','" & txtIncreasedLimit_FoodManuf.ClientID & "','" & txtTotalVehicleLimit_FoodManuf.ClientID & "');")
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
    End Sub

    Public Overrides Function Save() As Boolean
        selectedOption = False

        If Quote IsNot Nothing Then
            If UseCPPFoodManufacturersFormat() Then
                ' CPP Food Manufacturing Logic
                If Me.chkTransportation_FoodManuf.Checked Then
                    If Quote IsNot Nothing Then
                        selectedOption = True
                        GoverningStateQuote.TransportationCatastropheDeductibleId = Me.trDeductible_FoodManuf.SelectedValue
                        GoverningStateQuote.TransportationCatastropheDescription = Me.txtDescription_FoodManuf.Text.Trim()
                        ' Per vehicle limit for food manufacturing transportation is 50,000 plus any increased limit
                        If Me.txtIncreasedLimit_FoodManuf.Text.Trim <> String.Empty AndAlso IsNumeric(Me.txtIncreasedLimit_FoodManuf.Text) AndAlso CInt(txtIncreasedLimit_FoodManuf.Text) > 0 Then
                            GoverningStateQuote.TransportationAnyOneOwnedVehicleLimit = (50000 + CInt(Me.txtIncreasedLimit_FoodManuf.Text))
                        Else
                            GoverningStateQuote.TransportationAnyOneOwnedVehicleLimit = "50000"
                        End If
                        GoverningStateQuote.TransportationAnyOneOwnedVehicleNumberOfVehicles = Me.txtNumOfVehicles_FoodManuf.Text.Trim()

                        Me.txtCatLimit_FoodManuf.Text = IFM.Common.InputValidation.InputHelpers.TryToGetDouble(GoverningStateQuote.TransportationAnyOneOwnedVehicleLimit) * IFM.Common.InputValidation.InputHelpers.TryToGetDouble(Me.txtNumOfVehicles_FoodManuf.Text)
                        GoverningStateQuote.TransportationCatastropheLimit = Me.txtCatLimit_FoodManuf.Text.Trim()
                        GoverningStateQuote.TransportationAnyOneOwnedVehicleRate = ".25"
                    End If
                End If
            Else
                ' Non CPP food-manufacturing logic
                If Me.chkTransportation.Checked Then
                    If Quote IsNot Nothing Then
                        selectedOption = True
                        GoverningStateQuote.TransportationCatastropheDeductibleId = Me.trDeductible.SelectedValue
                        GoverningStateQuote.TransportationCatastropheDescription = Me.txtDescription.Text.Trim()
                        GoverningStateQuote.TransportationAnyOneOwnedVehicleLimit = Me.txtLimitPerVehicle.Text.Trim()
                        GoverningStateQuote.TransportationAnyOneOwnedVehicleNumberOfVehicles = Me.txtNumOfVehicles.Text.Trim()

                        Me.txtCatLimit.Text = IFM.Common.InputValidation.InputHelpers.TryToGetDouble(Me.txtLimitPerVehicle.Text) * IFM.Common.InputValidation.InputHelpers.TryToGetDouble(Me.txtNumOfVehicles.Text)
                        GoverningStateQuote.TransportationCatastropheLimit = Me.txtCatLimit.Text.Trim()

                        GoverningStateQuote.TransportationAnyOneOwnedVehicleRate = CIMHelper.TransportationRateTable.GetRateForLimit(IFM.Common.InputValidation.InputHelpers.TryToGetDouble(Me.txtCatLimit.Text))
                    End If
                End If
            End If
        End If

        Return True
    End Function

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        Dim deductibleAmount As Double = 0
        Dim limitPerVehicle As Double = 0
        Dim numVehicles As Double = 0

        MyBase.ValidateControl(valArgs)
        Me.ValidationHelper.GroupName = "Transportation"

        If UseCPPFoodManufacturersFormat() Then
            ' CPP With Food Manufacturers Enhancement
            If chkTransportation_FoodManuf.Checked Then
                deductibleAmount = IFM.Common.InputValidation.InputHelpers.TryToGetDouble(Me.trDeductible_FoodManuf.SelectedItem.ToString)
                limitPerVehicle = IFM.Common.InputValidation.InputHelpers.TryToGetDouble(Me.txtTotalVehicleLimit_FoodManuf.Text)
                numVehicles = IFM.Common.InputValidation.InputHelpers.TryToGetDouble(Me.txtNumOfVehicles_FoodManuf.Text)

                If String.IsNullOrEmpty(trDeductible_FoodManuf.SelectedValue) Then
                    Me.ValidationHelper.AddError("Missing Deductible", trDeductible_FoodManuf.ClientID)
                End If

                If String.IsNullOrEmpty(Me.txtDescription_FoodManuf.Text) Then
                    Me.ValidationHelper.AddError("Missing Cargo description", txtDescription_FoodManuf.ClientID)
                End If

                If String.IsNullOrEmpty(Me.txtNumOfVehicles_FoodManuf.Text) Then
                    Me.ValidationHelper.AddError("Missing Number of Vehicles", txtNumOfVehicles_FoodManuf.ClientID)
                End If

                If String.IsNullOrEmpty(Me.txtLimitPerVehicle_FoodManuf.Text) Then
                    Me.ValidationHelper.AddError("Missing Limit Per Vehicles", txtLimitPerVehicle_FoodManuf.ClientID)
                End If

                If deductibleAmount >= limitPerVehicle Then
                    Me.ValidationHelper.AddError("Deductible amount selected is equal or greater than the Limit. Please adjust either value.", txtLimitPerVehicle_FoodManuf.ClientID)
                End If
                '3.8.127
                If limitPerVehicle > 150000 Then
                    Me.ValidationHelper.AddError("You have selected a limit that exceeds your authority, please contact your underwriter for higher limits.", txtTotalVehicleLimit_FoodManuf.ClientID)
                End If
                '3.8.128
                If (limitPerVehicle * numVehicles) > 150000 Then
                    Me.ValidationHelper.AddError("You have selected a limit that exceeds your authority, please contact your underwriter for higher limits.", txtTotalVehicleLimit_FoodManuf.ClientID)
                End If
            Else
                ' CPP or CPR without Food Manufacturers Enhancement
                If chkTransportation.Checked Then
                    deductibleAmount = IFM.Common.InputValidation.InputHelpers.TryToGetDouble(Me.trDeductible.SelectedItem.ToString)
                    limitPerVehicle = IFM.Common.InputValidation.InputHelpers.TryToGetDouble(Me.txtLimitPerVehicle.Text)
                    numVehicles = IFM.Common.InputValidation.InputHelpers.TryToGetDouble(Me.txtNumOfVehicles.Text)

                    If String.IsNullOrEmpty(trDeductible.SelectedValue) Then
                        Me.ValidationHelper.AddError("Missing Deductible", trDeductible.ClientID)
                    End If

                    If String.IsNullOrEmpty(Me.txtDescription.Text) Then
                        Me.ValidationHelper.AddError("Missing Cargo description", txtDescription.ClientID)
                    End If

                    If String.IsNullOrEmpty(Me.txtNumOfVehicles.Text) Then
                        Me.ValidationHelper.AddError("Missing Number of Vehicles", txtNumOfVehicles.ClientID)
                    End If

                    If String.IsNullOrEmpty(Me.txtLimitPerVehicle.Text) Then
                        Me.ValidationHelper.AddError("Missing Limit Per Vehicles", txtLimitPerVehicle.ClientID)
                    End If

                    If deductibleAmount >= limitPerVehicle Then
                        Me.ValidationHelper.AddError("Deductible amount selected is equal or greater than the Limit. Please adjust either value.", txtLimitPerVehicle.ClientID)
                    End If

                    '3.8.127
                    If limitPerVehicle > 100000 Then
                        Me.ValidationHelper.AddError("You have selected a limit that exceeds your authority, please contact your underwriter for higher limits.", txtLimitPerVehicle.ClientID)
                    End If
                    '3.8.128
                    If (limitPerVehicle * numVehicles) > 100000 Then
                        Me.ValidationHelper.AddError("You have selected a limit that exceeds your authority, please contact your underwriter for higher limits.", txtLimitPerVehicle.ClientID)
                    End If
                End If
            End If

            Me.ValidateChildControls(valArgs)
        End If
    End Sub

    Public Overrides Sub ClearControl()
        GoverningStateQuote.TransportationCatastropheDeductibleId = ""
        GoverningStateQuote.TransportationCatastropheDescription = ""
        GoverningStateQuote.TransportationAnyOneOwnedVehicleLimit = ""
        GoverningStateQuote.TransportationAnyOneOwnedVehicleNumberOfVehicles = ""
        GoverningStateQuote.TransportationCatastropheLimit = ""
        GoverningStateQuote.TransportationAnyOneOwnedVehicleRate = ""

        If UseCPPFoodManufacturersFormat() Then
            txtDescription_FoodManuf.Text = String.Empty
            txtNumOfVehicles_FoodManuf.Text = String.Empty
            txtLimitPerVehicle_FoodManuf.Text = String.Empty
            txtIncreasedLimit_FoodManuf.Text = String.Empty
            txtTotalVehicleLimit_FoodManuf.Text = String.Empty
            txtCatLimit_FoodManuf.Text = String.Empty
        Else
            txtDescription.Text = String.Empty
            txtNumOfVehicles.Text = String.Empty
            txtLimitPerVehicle.Text = String.Empty
            txtCatLimit.Text = String.Empty
        End If
    End Sub

    Public Overrides Function hasSetting() As Boolean
        If UseCPPFoodManufacturersFormat() Then
            Return Me.chkTransportation_FoodManuf.Checked
        Else
            Return Me.chkTransportation.Checked
        End If
    End Function

    Private Sub btnClear_Click(sender As Object, e As EventArgs) Handles clearButton.Click
        If UseCPPFoodManufacturersFormat() Then
            If chkTransportation_FoodManuf.Checked = False Then
                ClearControl()
                Me.Save_FireSaveEvent(False)
                Populate()
            End If
        Else
            If chkTransportation.Checked = False Then
                ClearControl()
                Me.Save_FireSaveEvent(False)
                Populate()
            End If
        End If
    End Sub
End Class