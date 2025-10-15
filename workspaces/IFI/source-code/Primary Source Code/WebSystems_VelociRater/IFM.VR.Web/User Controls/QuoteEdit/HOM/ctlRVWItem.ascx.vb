Imports QuickQuote.CommonObjects
Imports QuickQuote.CommonMethods
Imports IFM.VR.Web.Helpers

Public Class ctlRVWItem
    Inherits System.Web.UI.UserControl
    Implements IVRUI_P

#Region "Declarations"

    Private Const ClassName As String = "ctlRVWItem"
    Public Event ItemRemoveRequest(index As Int32)
    Public Event SaveRequested(ByVal index As Integer, ByVal WhichControl As String)
    Public Event HomeRVWatercraftChanged()

    Dim _quote As QuickQuoteObject
    Protected ReadOnly Property Quote As QuickQuote.CommonObjects.QuickQuoteObject Implements IVRUI_P.Quote
        Get
            Dim errCreateQSO As String = ""
            If _quote Is Nothing Then
                _quote = VR.Common.QuoteSave.QuoteSaveHelpers.GetQuoteById(QuickQuoteObject.QuickQuoteLobType.HomePersonal, QuoteId, errCreateQSO)
            End If
            Return _quote
        End Get
    End Property

    Protected ReadOnly Property QuoteId As String Implements IVRUI_P.QuoteId
        Get
            If Request.QueryString("quoteid") IsNot Nothing Then
                Return Request.QueryString("quoteid")
            End If
            If Page.RouteData.Values("quoteid") IsNot Nothing Then
                Return Page.RouteData.Values("quoteid").ToString()
            End If
            Return ""
        End Get
    End Property

    Public Property ValidationHelper As ControlValidationHelper Implements IVRUI_P.ValidationHelper
        Get
            If ViewState("vs_valHelp") Is Nothing Then
                ViewState("vs_valHelp") = New ControlValidationHelper
            End If
            Return DirectCast(ViewState("vs_valHelp"), ControlValidationHelper)
        End Get
        Set(value As ControlValidationHelper)
            ViewState("vs_valHelp") = value
        End Set
    End Property

    Public Property RVWatercraftNumber As Int32
        Get
            If ViewState("RVWatercraftNumber") Is Nothing Then
                ViewState("RVWatercraftNumber") = -1
            End If
            Return CInt(ViewState("RVWatercraftNumber"))
        End Get
        Set(value As Int32)
            ViewState("RVWatercraftNumber") = value
        End Set
    End Property

    Public Property ListDivContainerId As String
        Get
            If ViewState("vs_listdivContainerId") IsNot Nothing Then
                Return ViewState("vs_listdivContainerId")
            End If
            Return ""
        End Get
        Set(value As String)
            ViewState("vs_listdivContainerId") = value
        End Set
    End Property

    Public Property ListItemsContainerId As String
        Get
            If ViewState("vs_listitemsdivContainerId") IsNot Nothing Then
                Return ViewState("vs_listitemsdivContainerId")
            End If
            Return ""
        End Get
        Set(value As String)
            ViewState("vs_listitemsdivContainerId") = value
        End Set
    End Property

    Public ReadOnly Property txtMotorYearClientID As String
        Get
            Return Me.txtMotorYear.ClientID
        End Get
    End Property

    Public ReadOnly Property ddlMotorTypeClientID As String
        Get
            Return Me.ddlMotorType.ClientID
        End Get
    End Property

    Public ReadOnly Property txtMotorManufacturerClientID As String
        Get
            Return Me.txtMotorManufacturer.ClientID
        End Get
    End Property

    Public ReadOnly Property txtMotorModelClientID As String
        Get
            Return Me.txtMotorModel.ClientID
        End Get
    End Property

    Public ReadOnly Property txtMotorSerialNumberClientID As String
        Get
            Return Me.txtMotorSerialNumber.ClientID
        End Get
    End Property

    Public ReadOnly Property txtMotorCostNewClientID As String
        Get
            Return Me.txtMotorCostNew.ClientID
        End Get
    End Property
#End Region

#Region "Methods & Functions"

    Private Sub HandleError(ByVal RoutineName As String, ByVal exc As Exception)
        ShowMessage(ClassName & ":" & RoutineName & ": " & exc.Message)
    End Sub

    Private Sub ClearErrorMessage()
        lblMsg.Text = "&nbsp;"
    End Sub

    Private Sub ShowMessage(ByVal Msg As String)
        lblMsg.Text = Msg
    End Sub

    ''' <summary>
    '''
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub SaveDetected() Handles ctlRVWOperators.SaveRequested
        Try
            RaiseEvent SaveRequested(RVWatercraftNumber, "ctlRVWatercraftOperatorItem")
            Exit Sub
        Catch ex As Exception
            HandleError("SaveDetected", ex)
            Exit Sub
        End Try
    End Sub

    Public Sub AttachRVWatercraftControlEvents()

    End Sub

    ''' <summary>
    ''' Saves this Inland Marine item
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function Save() As Boolean Implements IVRUI_P.Save
        Dim err As String = ""
        Dim cov As QuickQuoteCoverage = Nothing

        Try
            If Quote Is Nothing Then Return False
            If Quote.Locations(0) Is Nothing Then Return False

            ClearErrorMessage()

            ' SET THE QUOTE OBJECT VALUES
            '***************
            ' Vehicle
            '***************
            Quote.Locations(0).RvWatercrafts(RVWatercraftNumber).RvWatercraftTypeId = ddlVehType.SelectedValue
            Quote.Locations(0).RvWatercrafts(RVWatercraftNumber).Year = txtVehYear.Text
            Quote.Locations(0).RvWatercrafts(RVWatercraftNumber).Manufacturer = txtVehManufacturer.Text
            Quote.Locations(0).RvWatercrafts(RVWatercraftNumber).Model = txtVehModel.Text
            Quote.Locations(0).RvWatercrafts(RVWatercraftNumber).SerialNumber = txtVehSerialNumber.Text
            Quote.Locations(0).RvWatercrafts(RVWatercraftNumber).HorsepowerCC = txtVehHorsepowerCCs.Text
            Quote.Locations(0).RvWatercrafts(RVWatercraftNumber).Length = txtVehLength.Text
            Quote.Locations(0).RvWatercrafts(RVWatercraftNumber).RatedSpeed = txtVehRatedSpeed.Text
            Quote.Locations(0).RvWatercrafts(RVWatercraftNumber).CostNew = txtVehCostNew.Text
            Quote.Locations(0).RvWatercrafts(RVWatercraftNumber).Description = txtVehDescription.Text
            ' Premium is not on object
            Quote.Locations(0).RvWatercrafts(RVWatercraftNumber).OwnerOtherThanInsured = chkVehOwnerOtherThanInsured.Checked
            If chkVehOwnerOtherThanInsured.Checked Then
                Dim nm As New QuickQuoteName()
                Quote.Locations(0).RvWatercrafts(RVWatercraftNumber).Name.DisplayName = txtVehOwnerOtherThanInsuredName.Text
            End If

            '***************
            ' Motor
            '***************
            If Quote.Locations(0).RvWatercrafts(RVWatercraftNumber).RvWatercraftMotors Is Nothing Then
                Quote.Locations(0).RvWatercrafts(RVWatercraftNumber).RvWatercraftMotors = New List(Of QuickQuoteRvWatercraftMotor)
                Dim rvwm As New QuickQuoteRvWatercraftMotor()
                Quote.Locations(0).RvWatercrafts(RVWatercraftNumber).RvWatercraftMotors.Add(rvwm)
            End If
            Quote.Locations(0).RvWatercrafts(RVWatercraftNumber).RvWatercraftMotors(0).MotorTypeId = ddlMotorType.SelectedValue
            Quote.Locations(0).RvWatercrafts(RVWatercraftNumber).RvWatercraftMotors(0).Year = txtMotorYear.Text
            Quote.Locations(0).RvWatercrafts(RVWatercraftNumber).RvWatercraftMotors(0).Manufacturer = txtMotorManufacturer.Text
            Quote.Locations(0).RvWatercrafts(RVWatercraftNumber).RvWatercraftMotors(0).Model = txtMotorModel.Text
            Quote.Locations(0).RvWatercrafts(RVWatercraftNumber).RvWatercraftMotors(0).SerialNumber = txtMotorSerialNumber.Text
            Quote.Locations(0).RvWatercrafts(RVWatercraftNumber).RvWatercraftMotors(0).CostNew = txtMotorCostNew.Text

            '***************
            ' Operators
            '***************
            ' TODO: Add operators save logic

            '***************
            ' Coverages
            '***************
            ' Property Limit - CANNOT BE SET

            ' Property Deductible
            Quote.Locations(0).RvWatercrafts(RVWatercraftNumber).PropertyDeductibleLimitId = ddlPropertyDeductible.SelectedValue

            ' Bodily Injury Limit
            Quote.Locations(0).RvWatercrafts(RVWatercraftNumber).UninsuredMotoristBodilyInjuryLimitId = ddlBodilyInjuryLimit.SelectedValue

            ' Liability
            Quote.Locations(0).RvWatercrafts(RVWatercraftNumber).HasLiability = chkRVWLiability.Checked

            ' Liability Only
            Quote.Locations(0).RvWatercrafts(RVWatercraftNumber).HasLiabilityOnly = chkRVWLiabilityOnly.Checked

            UpdateHeaderText()

            Return True
        Catch ex As Exception
            HandleError("Save", ex)
            Return False
        End Try
    End Function

    ''' <summary>
    ''' Loads the dropdown controls
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub LoadStaticData() Implements IVRUI_P.LoadStaticData
        Dim qqHelper As New QuickQuoteHelperClass
        Try
            qqHelper.LoadStaticDataOptionsDropDown(Me.ddlVehType, QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteRvWatercraft, QuickQuoteHelperClass.QuickQuotePropertyName.RvWatercraftTypeId, QuickQuoteStaticDataOption.SortBy.None, Me.Quote.LobType)
            qqHelper.LoadStaticDataOptionsDropDown(Me.ddlMotorType, QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteRvWatercraftMotor, QuickQuoteHelperClass.QuickQuotePropertyName.MotorTypeId, QuickQuoteStaticDataOption.SortBy.None, Me.Quote.LobType)
            qqHelper.LoadStaticDataOptionsDropDown(Me.ddlPropertyDeductible, QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteRvWatercraft, QuickQuoteHelperClass.QuickQuotePropertyName.PropertyDeductibleLimitId, QuickQuoteStaticDataOption.SortBy.None, Me.Quote.LobType)
            qqHelper.LoadStaticDataOptionsDropDown(Me.ddlBodilyInjuryLimit, QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteRvWatercraft, QuickQuoteHelperClass.QuickQuotePropertyName.UninsuredMotoristBodilyInjuryLimitId, QuickQuoteStaticDataOption.SortBy.None, Me.Quote.LobType)
            Exit Sub
        Catch ex As Exception
            HandleError("LoadStaticData", ex)
            Exit Sub
        End Try
    End Sub

    ''' <summary>
    ''' Populates the form with Inland Marine data
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub Populate() Implements IVRUI_P.Populate
        Try
            If Quote IsNot Nothing AndAlso Quote.Locations IsNot Nothing AndAlso Quote.Locations(0).RvWatercrafts IsNot Nothing Then
                If RVWatercraftNumber >= 0 AndAlso Quote.Locations(0).RvWatercrafts.Count >= RVWatercraftNumber Then
                    LoadStaticData()

                    ' Vehicle Info
                    If IFM.Common.InputValidation.InputHelpers.StringHasNumericValue(Quote.Locations(0).RvWatercrafts(RVWatercraftNumber).RvWatercraftTypeId) Then ddlVehType.SelectedValue = Quote.Locations(0).RvWatercrafts(RVWatercraftNumber).RvWatercraftTypeId
                    If IFM.Common.InputValidation.InputHelpers.StringHasNumericValue(Quote.Locations(0).RvWatercrafts(RVWatercraftNumber).Year) Then txtVehYear.Text = Quote.Locations(0).RvWatercrafts(RVWatercraftNumber).Year
                    If IFM.Common.InputValidation.InputHelpers.StringHasAnyValue(Quote.Locations(0).RvWatercrafts(RVWatercraftNumber).Manufacturer) Then txtVehManufacturer.Text = Quote.Locations(0).RvWatercrafts(RVWatercraftNumber).Manufacturer
                    If IFM.Common.InputValidation.InputHelpers.StringHasAnyValue(Quote.Locations(0).RvWatercrafts(RVWatercraftNumber).Model) Then txtVehModel.Text = Quote.Locations(0).RvWatercrafts(RVWatercraftNumber).Model
                    If IFM.Common.InputValidation.InputHelpers.StringHasAnyValue(Quote.Locations(0).RvWatercrafts(RVWatercraftNumber).SerialNumber) Then txtVehSerialNumber.Text = Quote.Locations(0).RvWatercrafts(RVWatercraftNumber).SerialNumber
                    If IFM.Common.InputValidation.InputHelpers.StringHasNumericValue(Quote.Locations(0).RvWatercrafts(RVWatercraftNumber).HorsepowerCC) Then txtVehHorsepowerCCs.Text = Quote.Locations(0).RvWatercrafts(RVWatercraftNumber).HorsepowerCC
                    If IFM.Common.InputValidation.InputHelpers.StringHasNumericValue(Quote.Locations(0).RvWatercrafts(RVWatercraftNumber).Length) Then txtVehLength.Text = Quote.Locations(0).RvWatercrafts(RVWatercraftNumber).Length
                    If IFM.Common.InputValidation.InputHelpers.StringHasNumericValue(Quote.Locations(0).RvWatercrafts(RVWatercraftNumber).RatedSpeed) Then txtVehRatedSpeed.Text = Quote.Locations(0).RvWatercrafts(RVWatercraftNumber).RatedSpeed
                    If IFM.Common.InputValidation.InputHelpers.StringHasNumericValue(Quote.Locations(0).RvWatercrafts(RVWatercraftNumber).CostNew) Then txtVehCostNew.Text = Quote.Locations(0).RvWatercrafts(RVWatercraftNumber).CostNew
                    If IFM.Common.InputValidation.InputHelpers.StringHasAnyValue(Quote.Locations(0).RvWatercrafts(RVWatercraftNumber).Description) Then txtVehDescription.Text = Quote.Locations(0).RvWatercrafts(RVWatercraftNumber).Description
                    txtVehPremium.Text = "?"
                    If Quote.Locations(0).RvWatercrafts(RVWatercraftNumber).OwnerOtherThanInsured Then
                        chkVehOwnerOtherThanInsured.Checked = True
                    Else
                        chkVehOwnerOtherThanInsured.Checked = False
                    End If
                    txtVehOwnerOtherThanInsuredName.Text = "?"

                    ' Motors
                    If Quote.Locations(0).RvWatercrafts(RVWatercraftNumber).RvWatercraftMotors IsNot Nothing AndAlso Quote.Locations(0).RvWatercrafts(RVWatercraftNumber).RvWatercraftMotors.Count > 0 Then
                        Dim myMotor As QuickQuoteRvWatercraftMotor = Quote.Locations(0).RvWatercrafts(RVWatercraftNumber).RvWatercraftMotors(0)
                        If IFM.Common.InputValidation.InputHelpers.StringHasNumericValue(myMotor.MotorTypeId) Then ddlMotorType.SelectedValue = myMotor.MotorTypeId
                        If IFM.Common.InputValidation.InputHelpers.StringHasNumericValue(myMotor.Year) Then txtMotorYear.Text = myMotor.Year
                        If IFM.Common.InputValidation.InputHelpers.StringHasAnyValue(myMotor.Manufacturer) Then txtMotorManufacturer.Text = myMotor.Manufacturer
                        If IFM.Common.InputValidation.InputHelpers.StringHasAnyValue(myMotor.Model) Then txtMotorModel.Text = myMotor.Model
                        If IFM.Common.InputValidation.InputHelpers.StringHasAnyValue(myMotor.SerialNumber) Then txtMotorSerialNumber.Text = myMotor.SerialNumber
                        If IFM.Common.InputValidation.InputHelpers.StringHasNumericValue(myMotor.CostNew) Then txtMotorCostNew.Text = myMotor.CostNew
                    End If

                    ' Coverages
                    ' Watercraft property coverage
                    If IFM.Common.InputValidation.InputHelpers.StringHasNumericValue(Quote.Locations(0).RvWatercrafts(RVWatercraftNumber).PropertyDeductibleLimitId) Then
                        ddlPropertyDeductible.SelectedValue = Quote.Locations(0).RvWatercrafts(RVWatercraftNumber).PropertyDeductibleLimitId
                    Else
                        ddlPropertyDeductible.SelectedIndex = 0
                    End If
                    ' Watercraft Uninsured Motorist Bodily Injury
                    If IFM.Common.InputValidation.InputHelpers.StringHasNumericValue(Quote.Locations(0).RvWatercrafts(RVWatercraftNumber).UninsuredMotoristBodilyInjuryLimitId) Then
                        ddlBodilyInjuryLimit.SelectedValue = Quote.Locations(0).RvWatercrafts(RVWatercraftNumber).UninsuredMotoristBodilyInjuryLimitId
                    Else
                        ddlBodilyInjuryLimit.SelectedIndex = 0
                    End If
                    ' Watercraft Liability
                    If Quote.Locations(0).RvWatercrafts(RVWatercraftNumber).HasLiability Then
                        chkRVWLiability.Checked = True
                    Else
                        chkRVWLiability.Checked = False
                    End If
                    ' Watercraft Liability Only
                    If Quote.Locations(0).RvWatercrafts(RVWatercraftNumber).HasLiabilityOnly Then
                        chkRVWLiabilityOnly.Checked = True
                    Else
                        chkRVWLiabilityOnly.Checked = False
                    End If
                End If
            End If

            UpdateHeaderText()

            Exit Sub
        Catch ex As Exception
            HandleError("Populate", ex)
            Exit Sub
        End Try
    End Sub

    ''' <summary>
    ''' Formats the header text to it's maximum length.
    ''' Set the MaxLength value to what it needs to be.
    ''' </summary>
    ''' <param name="txt"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function FormatHeaderText(ByVal txt As String) As String
        Dim MaxLength As Integer = 60

        Try
            txt = txt.Trim()
            If txt.Length < MaxLength Then Return txt

            Return txt.Substring(0, MaxLength - 3) & "..."
        Catch ex As Exception
            HandleError("FormatHeaderText", ex)
            Return ""
        End Try
    End Function

    Private Sub UpdateHeaderText()
        Try
            'If StringHasAnyValue(txtVehDescription.Text) Then
            '    lblAccordHeader.Text = FormatHeaderText("RV/Watercraft #" & (RVWatercraftNumber + 1).ToString() & ": " & txtVehDescription.Text)
            'Else
            '    lblAccordHeader.Text = FormatHeaderText("RV/Watercraft #" & (RVWatercraftNumber + 1).ToString())
            'End If
            Exit Sub
        Catch ex As Exception
            HandleError("UpdateHeaderText", ex)
            Exit Sub
        End Try
    End Sub

    Public Sub ValidateForm() Implements IVRUI_P.ValidateForm
        Try
            Me.ValidationHelper.GroupName = String.Format("RV/Watercraft Item #{0}", RVWatercraftNumber + 1)
            Me.ValidationHelper.Clear()

            '***************************
            ' VEHICLE SECTION
            '***************************
            ' Coverage Code
            If ddlVehType.SelectedValue Is Nothing OrElse ddlVehType.SelectedValue = "" Then
                Me.ValidationHelper.AddError("Vehicle Type must be selected", ddlVehType.ClientID)
                With Me.ValidationHelper.GetLastError()
                    .ScriptCollection.Add("$(""#VehicleInputDiv"").accordion(""option"",""active""," + RVWatercraftNumber.ToString() + ");")
                    .ScriptCollection.Add("DoValidationSummaryErrorJump(""" + .SenderClientId + """);")
                    .ScriptCollection.Add("$(this).css(""color"",""blue"");")
                End With
            End If

            ' Vehicle Year
            If String.IsNullOrWhiteSpace(txtVehYear.Text) Then
                Me.ValidationHelper.AddError("Missing Vehicle Year", txtVehYear.ClientID)
                With Me.ValidationHelper.GetLastError()
                    .ScriptCollection.Add("$(""#VehicleInputDiv"").accordion(""option"",""active""," + RVWatercraftNumber.ToString() + ");")
                    .ScriptCollection.Add("DoValidationSummaryErrorJump(""" + .SenderClientId + """);")
                    .ScriptCollection.Add("$(this).css(""color"",""blue"");")
                End With
            End If
            If Not IFM.Common.InputValidation.InputHelpers.StringHasNumericValue(txtVehYear.Text) Then
                Me.ValidationHelper.AddError("Vehicle Year must be numeric", txtVehYear.ClientID)
                With Me.ValidationHelper.GetLastError()
                    .ScriptCollection.Add("$(""#VehicleInputDiv"").accordion(""option"",""active""," + RVWatercraftNumber.ToString() + ");")
                    .ScriptCollection.Add("DoValidationSummaryErrorJump(""" + .SenderClientId + """);")
                    .ScriptCollection.Add("$(this).css(""color"",""blue"");")
                End With
            Else
                If CInt(txtVehYear.Text) < 1800 OrElse CInt(txtVehYear.Text) > DateTime.Now.Year Then
                    Me.ValidationHelper.AddError("Vehicle Year must be between 1/1/1800 and the current year", txtVehYear.ClientID)
                    With Me.ValidationHelper.GetLastError()
                        .ScriptCollection.Add("$(""#VehicleInputDiv"").accordion(""option"",""active""," + RVWatercraftNumber.ToString() + ");")
                        .ScriptCollection.Add("DoValidationSummaryErrorJump(""" + .SenderClientId + """);")
                        .ScriptCollection.Add("$(this).css(""color"",""blue"");")
                    End With
                End If
            End If

            ' Vehicle Manufacturer
            If String.IsNullOrWhiteSpace(txtVehManufacturer.Text) Then
                Me.ValidationHelper.AddError("Missing Vehicle Manufacturer", txtVehManufacturer.ClientID)
                With Me.ValidationHelper.GetLastError()
                    .ScriptCollection.Add("$(""#VehicleInputDiv"").accordion(""option"",""active""," + RVWatercraftNumber.ToString() + ");")
                    .ScriptCollection.Add("DoValidationSummaryErrorJump(""" + .SenderClientId + """);")
                    .ScriptCollection.Add("$(this).css(""color"",""blue"");")
                End With
            End If

            ' Vehicle Model
            If String.IsNullOrWhiteSpace(txtVehModel.Text) Then
                Me.ValidationHelper.AddError("Missing Vehicle Model", txtVehModel.ClientID)
                With Me.ValidationHelper.GetLastError()
                    .ScriptCollection.Add("$(""#VehicleInputDiv"").accordion(""option"",""active""," + RVWatercraftNumber.ToString() + ");")
                    .ScriptCollection.Add("DoValidationSummaryErrorJump(""" + .SenderClientId + """);")
                    .ScriptCollection.Add("$(this).css(""color"",""blue"");")
                End With
            End If

            ' Vehicle Serial Number
            If String.IsNullOrWhiteSpace(txtVehSerialNumber.Text) Then
                Me.ValidationHelper.AddError("Missing Vehicle Serial Number", txtVehSerialNumber.ClientID)
                With Me.ValidationHelper.GetLastError()
                    .ScriptCollection.Add("$(""#VehicleInputDiv"").accordion(""option"",""active""," + RVWatercraftNumber.ToString() + ");")
                    .ScriptCollection.Add("DoValidationSummaryErrorJump(""" + .SenderClientId + """);")
                    .ScriptCollection.Add("$(this).css(""color"",""blue"");")
                End With
            End If

            ' Vehicle Horsepower
            If String.IsNullOrWhiteSpace(txtVehHorsepowerCCs.Text) Then
                Me.ValidationHelper.AddError("Missing Vehicle Horsepower", txtVehHorsepowerCCs.ClientID)
                With Me.ValidationHelper.GetLastError()
                    .ScriptCollection.Add("$(""#VehicleInputDiv"").accordion(""option"",""active""," + RVWatercraftNumber.ToString() + ");")
                    .ScriptCollection.Add("DoValidationSummaryErrorJump(""" + .SenderClientId + """);")
                    .ScriptCollection.Add("$(this).css(""color"",""blue"");")
                End With
            End If
            If Not IFM.Common.InputValidation.InputHelpers.StringHasNumericValue(txtVehHorsepowerCCs.Text) Then
                Me.ValidationHelper.AddError("Vehicle Horsepower must be numeric", txtVehHorsepowerCCs.ClientID)
                With Me.ValidationHelper.GetLastError()
                    .ScriptCollection.Add("$(""#VehicleInputDiv"").accordion(""option"",""active""," + RVWatercraftNumber.ToString() + ");")
                    .ScriptCollection.Add("DoValidationSummaryErrorJump(""" + .SenderClientId + """);")
                    .ScriptCollection.Add("$(this).css(""color"",""blue"");")
                End With
            Else
                If CInt(txtVehHorsepowerCCs.Text) < 1 OrElse CInt(txtVehHorsepowerCCs.Text) > 1000 Then
                    Me.ValidationHelper.AddError("Vehicle Horsepower must be between 1 and 1000", txtVehHorsepowerCCs.ClientID)
                    With Me.ValidationHelper.GetLastError()
                        .ScriptCollection.Add("$(""#VehicleInputDiv"").accordion(""option"",""active""," + RVWatercraftNumber.ToString() + ");")
                        .ScriptCollection.Add("DoValidationSummaryErrorJump(""" + .SenderClientId + """);")
                        .ScriptCollection.Add("$(this).css(""color"",""blue"");")
                    End With
                End If
            End If

            ' Vehicle Length
            If String.IsNullOrWhiteSpace(txtVehLength.Text) Then
                Me.ValidationHelper.AddError("Missing Vehicle Length", txtVehLength.ClientID)
                With Me.ValidationHelper.GetLastError()
                    .ScriptCollection.Add("$(""#VehicleInputDiv"").accordion(""option"",""active""," + RVWatercraftNumber.ToString() + ");")
                    .ScriptCollection.Add("DoValidationSummaryErrorJump(""" + .SenderClientId + """);")
                    .ScriptCollection.Add("$(this).css(""color"",""blue"");")
                End With
            End If
            If Not IFM.Common.InputValidation.InputHelpers.StringHasNumericValue(txtVehLength.Text) Then
                Me.ValidationHelper.AddError("Vehicle Length must be numeric", txtVehLength.ClientID)
                With Me.ValidationHelper.GetLastError()
                    .ScriptCollection.Add("$(""#VehicleInputDiv"").accordion(""option"",""active""," + RVWatercraftNumber.ToString() + ");")
                    .ScriptCollection.Add("DoValidationSummaryErrorJump(""" + .SenderClientId + """);")
                    .ScriptCollection.Add("$(this).css(""color"",""blue"");")
                End With
            Else
                If CInt(txtVehLength.Text) < 1 OrElse CInt(txtVehLength.Text) > 500 Then
                    Me.ValidationHelper.AddError("Vehicle Length must be between 1 and 500", txtVehLength.ClientID)
                    With Me.ValidationHelper.GetLastError()
                        .ScriptCollection.Add("$(""#VehicleInputDiv"").accordion(""option"",""active""," + RVWatercraftNumber.ToString() + ");")
                        .ScriptCollection.Add("DoValidationSummaryErrorJump(""" + .SenderClientId + """);")
                        .ScriptCollection.Add("$(this).css(""color"",""blue"");")
                    End With
                End If
            End If

            ' Vehicle Speed
            If String.IsNullOrWhiteSpace(txtVehRatedSpeed.Text) Then
                Me.ValidationHelper.AddError("Missing Vehicle Rated Speed", txtVehRatedSpeed.ClientID)
                With Me.ValidationHelper.GetLastError()
                    .ScriptCollection.Add("$(""#VehicleInputDiv"").accordion(""option"",""active""," + RVWatercraftNumber.ToString() + ");")
                    .ScriptCollection.Add("DoValidationSummaryErrorJump(""" + .SenderClientId + """);")
                    .ScriptCollection.Add("$(this).css(""color"",""blue"");")
                End With
            End If
            If Not IFM.Common.InputValidation.InputHelpers.StringHasNumericValue(txtVehRatedSpeed.Text) Then
                Me.ValidationHelper.AddError("Vehicle speed must be numeric", txtVehRatedSpeed.ClientID)
                With Me.ValidationHelper.GetLastError()
                    .ScriptCollection.Add("$(""#VehicleInputDiv"").accordion(""option"",""active""," + RVWatercraftNumber.ToString() + ");")
                    .ScriptCollection.Add("DoValidationSummaryErrorJump(""" + .SenderClientId + """);")
                    .ScriptCollection.Add("$(this).css(""color"",""blue"");")
                End With
            Else
                If CInt(txtVehRatedSpeed.Text) < 1 OrElse CInt(txtVehRatedSpeed.Text) > 500 Then
                    Me.ValidationHelper.AddError("Vehicle speed must be between 1 and 250", txtVehRatedSpeed.ClientID)
                    With Me.ValidationHelper.GetLastError()
                        .ScriptCollection.Add("$(""#VehicleInputDiv"").accordion(""option"",""active""," + RVWatercraftNumber.ToString() + ");")
                        .ScriptCollection.Add("DoValidationSummaryErrorJump(""" + .SenderClientId + """);")
                        .ScriptCollection.Add("$(this).css(""color"",""blue"");")
                    End With
                End If
            End If

            ' Vehicle Cost New
            If String.IsNullOrWhiteSpace(txtVehCostNew.Text) Then
                Me.ValidationHelper.AddError("Missing Vehicle Cost New", txtVehCostNew.ClientID)
                With Me.ValidationHelper.GetLastError()
                    .ScriptCollection.Add("$(""#VehicleInputDiv"").accordion(""option"",""active""," + RVWatercraftNumber.ToString() + ");")
                    .ScriptCollection.Add("DoValidationSummaryErrorJump(""" + .SenderClientId + """);")
                    .ScriptCollection.Add("$(this).css(""color"",""blue"");")
                End With
            End If
            If Not IFM.Common.InputValidation.InputHelpers.StringHasNumericValue(txtVehCostNew.Text) Then
                Me.ValidationHelper.AddError("Vehicle Cost New must be numeric", txtVehCostNew.ClientID)
                With Me.ValidationHelper.GetLastError()
                    .ScriptCollection.Add("$(""#VehicleInputDiv"").accordion(""option"",""active""," + RVWatercraftNumber.ToString() + ");")
                    .ScriptCollection.Add("DoValidationSummaryErrorJump(""" + .SenderClientId + """);")
                    .ScriptCollection.Add("$(this).css(""color"",""blue"");")
                End With
            Else
                If CInt(txtVehCostNew.Text) < 1 OrElse CInt(txtVehCostNew.Text) > 10000000 Then
                    Me.ValidationHelper.AddError("Vehicle Cost New must be between 1 and 10,000,000", txtVehCostNew.ClientID)
                    With Me.ValidationHelper.GetLastError()
                        .ScriptCollection.Add("$(""#VehicleInputDiv"").accordion(""option"",""active""," + RVWatercraftNumber.ToString() + ");")
                        .ScriptCollection.Add("DoValidationSummaryErrorJump(""" + .SenderClientId + """);")
                        .ScriptCollection.Add("$(this).css(""color"",""blue"");")
                    End With
                End If
            End If

            ' Vehicle Description
            If String.IsNullOrWhiteSpace(txtVehDescription.Text) Then
                Me.ValidationHelper.AddError("Missing Vehicle Description", txtVehDescription.ClientID)
                With Me.ValidationHelper.GetLastError()
                    .ScriptCollection.Add("$(""#VehicleInputDiv"").accordion(""option"",""active""," + RVWatercraftNumber.ToString() + ");")
                    .ScriptCollection.Add("DoValidationSummaryErrorJump(""" + .SenderClientId + """);")
                    .ScriptCollection.Add("$(this).css(""color"",""blue"");")
                End With
            End If

            ' Vehicle Premium - NOT REQUIRED
            'If String.IsNullOrWhiteSpace(txtVehPremium.Text) Then
            '    Me.ValidationHelper.AddError("Missing Vehicle Premium", txtVehPremium.ClientID)
            '    With Me.ValidationHelper.GetLastError()
            '        .ScriptCollection.Add("$(""#VehicleInputDiv"").accordion(""option"",""active""," + RVWatercraftNumber.ToString() + ");")
            '        .ScriptCollection.Add("DoValidationSummaryErrorJump(""" + .SenderClientId + """);")
            '        .ScriptCollection.Add("$(this).css(""color"",""blue"");")
            '    End With
            'End If
            'If Not StringHasNumericValue(txtVehPremium.Text) Then
            '    Me.ValidationHelper.AddError("Vehicle Premium must be numeric", txtVehPremium.ClientID)
            '    With Me.ValidationHelper.GetLastError()
            '        .ScriptCollection.Add("$(""#VehicleInputDiv"").accordion(""option"",""active""," + RVWatercraftNumber.ToString() + ");")
            '        .ScriptCollection.Add("DoValidationSummaryErrorJump(""" + .SenderClientId + """);")
            '        .ScriptCollection.Add("$(this).css(""color"",""blue"");")
            '    End With
            'Else
            '    If CInt(txtVehCostNew.Text) < 1 OrElse CInt(txtVehPremium.Text) > 100000 Then
            '        Me.ValidationHelper.AddError("Vehicle Premium must be between 1 and 100,000", txtVehPremium.ClientID)
            '        With Me.ValidationHelper.GetLastError()
            '            .ScriptCollection.Add("$(""#VehicleInputDiv"").accordion(""option"",""active""," + RVWatercraftNumber.ToString() + ");")
            '            .ScriptCollection.Add("DoValidationSummaryErrorJump(""" + .SenderClientId + """);")
            '            .ScriptCollection.Add("$(this).css(""color"",""blue"");")
            '        End With
            '    End If
            'End If

            ' Vehicle Owner Other than Insured
            If chkVehOwnerOtherThanInsured.Checked Then
                If String.IsNullOrWhiteSpace(txtVehOwnerOtherThanInsuredName.Text) Then
                    Me.ValidationHelper.AddError("Missing Owner Other Than Insured Name", txtVehOwnerOtherThanInsuredName.ClientID)
                    With Me.ValidationHelper.GetLastError()
                        .ScriptCollection.Add("$(""#VehicleInputDiv"").accordion(""option"",""active""," + RVWatercraftNumber.ToString() + ");")
                        .ScriptCollection.Add("DoValidationSummaryErrorJump(""" + .SenderClientId + """);")
                        .ScriptCollection.Add("$(this).css(""color"",""blue"");")
                    End With
                End If
            End If

            '***************************
            ' MOTOR SECTION
            '***************************
            ' Motor Type
            If ddlMotorType.SelectedValue Is Nothing OrElse ddlMotorType.SelectedValue = "" Then
                Me.ValidationHelper.AddError("Motor Type must be selected", ddlMotorType.ClientID)
                With Me.ValidationHelper.GetLastError()
                    .ScriptCollection.Add("$(""#MotorInputDiv"").accordion(""option"",""active""," + RVWatercraftNumber.ToString() + ");")
                    .ScriptCollection.Add("DoValidationSummaryErrorJump(""" + .SenderClientId + """);")
                    .ScriptCollection.Add("$(this).css(""color"",""blue"");")
                End With
            End If

            ' !! Only validate the rest of the motor fields if a value other than "NONE" has been selected
            If ddlMotorType.SelectedValue <> "0" Then  ' 0 = NONE

                ' Motor Year
                If String.IsNullOrWhiteSpace(txtMotorYear.Text) Then
                    Me.ValidationHelper.AddError("Missing Motor Year", txtMotorYear.ClientID)
                    With Me.ValidationHelper.GetLastError()
                        .ScriptCollection.Add("$(""#MotorInputDiv"").accordion(""option"",""active""," + RVWatercraftNumber.ToString() + ");")
                        .ScriptCollection.Add("DoValidationSummaryErrorJump(""" + .SenderClientId + """);")
                        .ScriptCollection.Add("$(this).css(""color"",""blue"");")
                    End With
                End If
                If Not IFM.Common.InputValidation.InputHelpers.StringHasNumericValue(txtMotorYear.Text) Then
                    Me.ValidationHelper.AddError("Motor Year must be numeric", txtMotorYear.ClientID)
                    With Me.ValidationHelper.GetLastError()
                        .ScriptCollection.Add("$(""#MotorInputDiv"").accordion(""option"",""active""," + RVWatercraftNumber.ToString() + ");")
                        .ScriptCollection.Add("DoValidationSummaryErrorJump(""" + .SenderClientId + """);")
                        .ScriptCollection.Add("$(this).css(""color"",""blue"");")
                    End With
                Else
                    If CInt(txtMotorYear.Text) < 1800 OrElse CInt(txtMotorYear.Text) > DateTime.Now.Year Then
                        Me.ValidationHelper.AddError("Motor Year must be between 1/1/1800 and the current year", txtMotorYear.ClientID)
                        With Me.ValidationHelper.GetLastError()
                            .ScriptCollection.Add("$(""#MotorInputDiv"").accordion(""option"",""active""," + RVWatercraftNumber.ToString() + ");")
                            .ScriptCollection.Add("DoValidationSummaryErrorJump(""" + .SenderClientId + """);")
                            .ScriptCollection.Add("$(this).css(""color"",""blue"");")
                        End With
                    End If
                End If

                ' Motor Manufacturer
                If String.IsNullOrWhiteSpace(txtMotorManufacturer.Text) Then
                    Me.ValidationHelper.AddError("Missing Motor Year", txtMotorManufacturer.ClientID)
                    With Me.ValidationHelper.GetLastError()
                        .ScriptCollection.Add("$(""#MotorInputDiv"").accordion(""option"",""active""," + RVWatercraftNumber.ToString() + ");")
                        .ScriptCollection.Add("DoValidationSummaryErrorJump(""" + .SenderClientId + """);")
                        .ScriptCollection.Add("$(this).css(""color"",""blue"");")
                    End With
                End If

                ' Motor Model
                If String.IsNullOrWhiteSpace(txtMotorModel.Text) Then
                    Me.ValidationHelper.AddError("Missing Motor Year", txtMotorModel.ClientID)
                    With Me.ValidationHelper.GetLastError()
                        .ScriptCollection.Add("$(""#MotorInputDiv"").accordion(""option"",""active""," + RVWatercraftNumber.ToString() + ");")
                        .ScriptCollection.Add("DoValidationSummaryErrorJump(""" + .SenderClientId + """);")
                        .ScriptCollection.Add("$(this).css(""color"",""blue"");")
                    End With
                End If

                ' Motor Serial Number
                If String.IsNullOrWhiteSpace(txtMotorSerialNumber.Text) Then
                    Me.ValidationHelper.AddError("Missing Motor Year", txtMotorSerialNumber.ClientID)
                    With Me.ValidationHelper.GetLastError()
                        .ScriptCollection.Add("$(""#MotorInputDiv"").accordion(""option"",""active""," + RVWatercraftNumber.ToString() + ");")
                        .ScriptCollection.Add("DoValidationSummaryErrorJump(""" + .SenderClientId + """);")
                        .ScriptCollection.Add("$(this).css(""color"",""blue"");")
                    End With
                End If

                ' Motor Cost New
                If String.IsNullOrWhiteSpace(txtMotorCostNew.Text) Then
                    Me.ValidationHelper.AddError("Missing Motor Cost New", txtMotorCostNew.ClientID)
                    With Me.ValidationHelper.GetLastError()
                        .ScriptCollection.Add("$(""#MotorInputDiv"").accordion(""option"",""active""," + RVWatercraftNumber.ToString() + ");")
                        .ScriptCollection.Add("DoValidationSummaryErrorJump(""" + .SenderClientId + """);")
                        .ScriptCollection.Add("$(this).css(""color"",""blue"");")
                    End With
                End If
                If Not IFM.Common.InputValidation.InputHelpers.StringHasNumericValue(txtMotorCostNew.Text) Then
                    Me.ValidationHelper.AddError("Motor Cost New must be numeric", txtMotorCostNew.ClientID)
                    With Me.ValidationHelper.GetLastError()
                        .ScriptCollection.Add("$(""#MotorInputDiv"").accordion(""option"",""active""," + RVWatercraftNumber.ToString() + ");")
                        .ScriptCollection.Add("DoValidationSummaryErrorJump(""" + .SenderClientId + """);")
                        .ScriptCollection.Add("$(this).css(""color"",""blue"");")
                    End With
                Else
                    If CInt(txtVehCostNew.Text) < 1 OrElse CInt(txtMotorCostNew.Text) > 100000 Then
                        Me.ValidationHelper.AddError("Motor Cost New must be between 1 and 100,000", txtMotorCostNew.ClientID)
                        With Me.ValidationHelper.GetLastError()
                            .ScriptCollection.Add("$(""#MotorInputDiv"").accordion(""option"",""active""," + RVWatercraftNumber.ToString() + ");")
                            .ScriptCollection.Add("DoValidationSummaryErrorJump(""" + .SenderClientId + """);")
                            .ScriptCollection.Add("$(this).css(""color"",""blue"");")
                        End With
                    End If
                End If
            End If

            '***************************
            ' OPERATOR SECTION
            '***************************
            ' TODO: Operators

            '***************************
            ' COVERAGES SECTION
            '***************************
            ' Property Limit - NOT REQUIRED
            'If String.IsNullOrWhiteSpace(txtPropertyLimit.Text) Then
            '    Me.ValidationHelper.AddError("Missing Property Limit", txtPropertyLimit.ClientID)
            '    With Me.ValidationHelper.GetLastError()
            '        .ScriptCollection.Add("$(""#RVWatercraftCoveragesDiv"").accordion(""option"",""active""," + RVWatercraftNumber.ToString() + ");")
            '        .ScriptCollection.Add("DoValidationSummaryErrorJump(""" + .SenderClientId + """);")
            '        .ScriptCollection.Add("$(this).css(""color"",""blue"");")
            '    End With
            'End If
            'If Not StringHasNumericValue(txtPropertyLimit.Text) Then
            '    Me.ValidationHelper.AddError("Property Limit must be numeric", txtPropertyLimit.ClientID)
            '    With Me.ValidationHelper.GetLastError()
            '        .ScriptCollection.Add("$(""#RVWatercraftCoveragesDiv"").accordion(""option"",""active""," + RVWatercraftNumber.ToString() + ");")
            '        .ScriptCollection.Add("DoValidationSummaryErrorJump(""" + .SenderClientId + """);")
            '        .ScriptCollection.Add("$(this).css(""color"",""blue"");")
            '    End With
            'Else
            '    If CInt(txtVehCostNew.Text) < 1 OrElse CInt(txtPropertyLimit.Text) > 1000000 Then
            '        Me.ValidationHelper.AddError("Property Limit must be between 1 and 1,000,000", txtPropertyLimit.ClientID)
            '        With Me.ValidationHelper.GetLastError()
            '            .ScriptCollection.Add("$(""#RVWatercraftCoveragesDiv"").accordion(""option"",""active""," + RVWatercraftNumber.ToString() + ");")
            '            .ScriptCollection.Add("DoValidationSummaryErrorJump(""" + .SenderClientId + """);")
            '            .ScriptCollection.Add("$(this).css(""color"",""blue"");")
            '        End With
            '    End If
            'End If

            ' Property Deductible
            If ddlPropertyDeductible.SelectedValue Is Nothing OrElse ddlPropertyDeductible.SelectedValue = "" Then
                Me.ValidationHelper.AddError("Property Deductible must be selected", ddlPropertyDeductible.ClientID)
                With Me.ValidationHelper.GetLastError()
                    .ScriptCollection.Add("$(""#RVWatercraftCoveragesDiv"").accordion(""option"",""active""," + RVWatercraftNumber.ToString() + ");")
                    .ScriptCollection.Add("DoValidationSummaryErrorJump(""" + .SenderClientId + """);")
                    .ScriptCollection.Add("$(this).css(""color"",""blue"");")
                End With
            End If
            ' Property Premium is not stored

            ' Bodily Injury
            If ddlBodilyInjuryLimit.SelectedValue Is Nothing OrElse ddlBodilyInjuryLimit.SelectedValue = "" Then
                Me.ValidationHelper.AddError("Bodily Injury Limit must be selected", ddlBodilyInjuryLimit.ClientID)
                With Me.ValidationHelper.GetLastError()
                    .ScriptCollection.Add("$(""#RVWatercraftCoveragesDiv"").accordion(""option"",""active""," + RVWatercraftNumber.ToString() + ");")
                    .ScriptCollection.Add("DoValidationSummaryErrorJump(""" + .SenderClientId + """);")
                    .ScriptCollection.Add("$(this).css(""color"",""blue"");")
                End With
            End If
            ' Bodily Injury Premium is not stored

            ' Liability is a checkbox
            ' Liability Only is a checkbox

            Dim valSum As ctlValidationSummary = DirectCast(Me.Page.Master, VelociRater).ValidationSummary
            valSum.InsertValidationControl(Me.ValidationHelper)

            Exit Sub
        Catch ex As Exception
            HandleError("ValidateForm", ex)
            Exit Sub
        End Try
    End Sub

#End Region

#Region "Events"

    ''' <summary>
    ''' PAGE LOAD
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim js As String = Nothing
        Try
            Dim _script = DirectCast(Me.Page.Master, VelociRater).StartUpScriptManager

            _script.AddScriptLine("$(""#" + Me.MotorInputDiv.ClientID + """).accordion({heightStyle: ""content"", collapsible: true});")
            _script.AddScriptLine("$(""#" + Me.OperatorInputDiv.ClientID + """).accordion({heightStyle: ""content"", collapsible: true});")
            _script.AddScriptLine("$(""#" + Me.RVWatercraftCoveragesDiv.ClientID + """).accordion({heightStyle: ""content"", collapsible: true});")

            Dim ctlop As ctlRVWatercraftOperatorItem = Me.OperatorInputDiv.FindControl("ctlRVWOperators")
            ctlop.RVWatercraftNumber = Me.RVWatercraftNumber

            ' When a motor type of 'NONE' is selected, this script will clear out the rest of the motor fields
            js = "<script type='text/javascript'> "
            js = js & "function ddlMotor_Changed() {"
            js = js & "var ddl = document.getElementById('" & Me.ddlMotorTypeClientID & "');"
            js = js & "if (ddl.options[ddl.selectedIndex].value == '0') {"
            js = js & "document.getElementById('" & Me.txtMotorYearClientID & "').value = '';"
            js = js & "document.getElementById('" & Me.txtMotorModelClientID & "').value = '';"
            js = js & "document.getElementById('" & Me.txtMotorManufacturerClientID & "').value = '';"
            js = js & "document.getElementById('" & Me.txtMotorSerialNumberClientID & "').value = '';"
            js = js & "document.getElementById('" & Me.txtMotorCostNewClientID & "').value = '';"
            js = js & "}}</script>"
            Page.RegisterClientScriptBlock("js_" & DateTime.Now, js)

            Exit Sub
        Catch ex As Exception
            HandleError("PAGE LOAD", ex)
            Exit Sub
        End Try
    End Sub

    'Protected Sub lnkSave_Click(ByVal sender As Object, ByVal e As EventArgs) Handles lnkSave.Click
    '    Try
    '        ClearErrorMessage()
    '        ValidateForm()
    '        If ValidationHelper.HasErrros Then Exit Sub
    '        Save()
    '        RaiseEvent SaveRequested(RVWatercraftNumber, ClassName)
    '        Populate()
    '        ShowMessage("Item Saved")
    '        Exit Sub
    '    Catch ex As Exception
    '        HandleError("lnkSave_Click", ex)
    '        Exit Sub
    '    End Try
    'End Sub

    'Protected Sub lnkRemove_Click(ByVal sender As Object, ByVal e As EventArgs) Handles lnkRemove.Click
    '    Try
    '        If Quote IsNot Nothing AndAlso Quote.Locations IsNot Nothing _
    '            AndAlso Quote.Locations.Count > 0 _
    '            AndAlso Quote.Locations(0).RvWatercrafts IsNot Nothing _
    '            AndAlso Quote.Locations(0).RvWatercrafts.Count > 0 _
    '            AndAlso Quote.Locations(0).RvWatercrafts(RVWatercraftNumber) IsNot Nothing Then
    '            Me.Quote.Locations(0).RvWatercrafts.RemoveAt(RVWatercraftNumber)
    '            RaiseEvent ItemRemoveRequest(RVWatercraftNumber)
    '            Exit Sub
    '        End If

    '        Exit Sub
    '    Catch ex As Exception
    '        HandleError("lnkRemove_Click", ex)
    '        Exit Sub
    '    End Try
    'End Sub

#End Region
End Class