Imports QuickQuote.CommonObjects
Imports QuickQuote.CommonMethods
Imports IFM.VR.Web.Helpers
Imports IFM.VR.Common.Validation

Public Class ctlHomePersonalLossItem
    Inherits System.Web.UI.UserControl
    Implements IVRUI_P

#Region "Declarations"
    Public Event ItemRemoveRequest(index As Int32)
    Public Event SaveRequested(ByVal Index As Integer, ByVal WhichControl As String)

    Dim ClassName As String = "ctlHomePersonalLossItem"
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

    Public Property LossHistoryNumber As Int32
        Get
            If ViewState("vs_LossHistoryNum") Is Nothing Then
                ViewState("vs_LossHistoryNum") = -1
            End If
            Return CInt(ViewState("vs_LossHistoryNum"))
        End Get
        Set(value As Int32)
            ViewState("vs_LossHistoryNum") = value
            'Dim scriptHeaderUpdate As String = "updateInlandMarineHeaderText(""" + lblAccordHeader.ClientID + """,""" + (InlandMarineNumber + 1).ToString() + """," + """" + value.ToString() + """" + "); "
            'ddlCoverage.Attributes.Add("onkeyup", scriptHeaderUpdate)
            'txtIncludedLimit.Attributes.Add("onkeyup", scriptHeaderUpdate)
            'txtIncreasedLimit.Attributes.Add("onkeyup", scriptHeaderUpdate)
            'txtTotalLimit.Attributes.Add("onkeyup", scriptHeaderUpdate)
            'txtDescription.Attributes.Add("onkeyup", scriptHeaderUpdate)
            'txtStorageLocation.Attributes.Add("onkeyup", scriptHeaderUpdate)
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

    Public Sub AttachInlandMarineControlEvents()

    End Sub

    ''' <summary>
    ''' Saves this Inland Marine item
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function Save() As Boolean Implements IVRUI_P.Save
        Dim err As String = ""

        Try
            If Quote Is Nothing Then Return False
            If Quote.Locations(0) Is Nothing Then Return False

            ' SET THE QUOTE OBJECT VALUES
            Quote.LossHistoryRecords(LossHistoryNumber).ClaimNumber = txtClaimNumber.Text
            Quote.LossHistoryRecords(LossHistoryNumber).LossDate = txtLossDate.Text
            Quote.LossHistoryRecords(LossHistoryNumber).TypeOfLossId = ddlTypeOfLoss.SelectedValue
            Quote.LossHistoryRecords(LossHistoryNumber).Amount = txtPaidAmount.Text
            Quote.LossHistoryRecords(LossHistoryNumber).ReserveAmount = txtReserveAmount.Text
            Quote.LossHistoryRecords(LossHistoryNumber).LossHistorySurchargeId = ddlSurcharge.SelectedValue
            Quote.LossHistoryRecords(LossHistoryNumber).LossHistorySourceId = ddlSource.SelectedValue
            Quote.LossHistoryRecords(LossHistoryNumber).Catastrophic = chkCatastrophic.Checked
            Quote.LossHistoryRecords(LossHistoryNumber).LossDescription = txtDescription.Text
            Quote.LossHistoryRecords(LossHistoryNumber).LossHistoryFaultId = ddlFaultIndicator.SelectedValue
            Quote.LossHistoryRecords(LossHistoryNumber).WeatherRelated = chkWeatherRelated.Checked
            If StringHasAnyValue(txtComments.Text) Then
                Quote.LossHistoryRecords(LossHistoryNumber).Comments = txtComments.Text
            Else
                Quote.LossHistoryRecords(LossHistoryNumber).Comments = ""
            End If

            RaiseEvent SaveRequested(LossHistoryNumber, ClassName)

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
            qqHelper.LoadStaticDataOptionsDropDown(Me.ddlSurcharge, QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteLossHistoryRecord, QuickQuoteHelperClass.QuickQuotePropertyName.LossHistorySurchargeId)
            qqHelper.LoadStaticDataOptionsDropDown(Me.ddlTypeOfLoss, QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteLossHistoryRecord, QuickQuoteHelperClass.QuickQuotePropertyName.TypeOfLossId)
            qqHelper.LoadStaticDataOptionsDropDown(Me.ddlSource, QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteLossHistoryRecord, QuickQuoteHelperClass.QuickQuotePropertyName.LossHistorySourceId)
            qqHelper.LoadStaticDataOptionsDropDown(Me.ddlFaultIndicator, QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteLossHistoryRecord, QuickQuoteHelperClass.QuickQuotePropertyName.LossHistoryFaultId)

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
            If Me.Quote IsNot Nothing Then
                If Me.Quote.HasLossHistories AndAlso Me.Quote.LossHistoryRecords IsNot Nothing Then
                    If Quote.LossHistoryRecords.Count >= LossHistoryNumber Then
                        LoadStaticData()
                        If StringHasAnyValue(Quote.LossHistoryRecords(LossHistoryNumber).ClaimNumber) Then txtClaimNumber.Text = Quote.LossHistoryRecords(LossHistoryNumber).ClaimNumber
                        If StringHasAnyValue(Quote.LossHistoryRecords(LossHistoryNumber).LossDate) Then txtLossDate.Text = Quote.LossHistoryRecords(LossHistoryNumber).LossDate
                        If StringHasNumericValue(Quote.LossHistoryRecords(LossHistoryNumber).TypeOfLossId) Then ddlTypeOfLoss.SelectedValue = Quote.LossHistoryRecords(LossHistoryNumber).TypeOfLossId
                        If StringHasNumericValue(Quote.LossHistoryRecords(LossHistoryNumber).Amount) Then txtPaidAmount.Text = Quote.LossHistoryRecords(LossHistoryNumber).Amount
                        If StringHasNumericValue(Quote.LossHistoryRecords(LossHistoryNumber).ReserveAmount) Then txtReserveAmount.Text = Quote.LossHistoryRecords(LossHistoryNumber).ReserveAmount
                        If StringHasNumericValue(Quote.LossHistoryRecords(LossHistoryNumber).LossHistorySurchargeId) Then ddlSurcharge.SelectedValue = Quote.LossHistoryRecords(LossHistoryNumber).LossHistorySurchargeId
                        If StringHasNumericValue(Quote.LossHistoryRecords(LossHistoryNumber).LossHistorySourceId) Then ddlSource.SelectedValue = Quote.LossHistoryRecords(LossHistoryNumber).LossHistorySourceId
                        If Quote.LossHistoryRecords(LossHistoryNumber).Catastrophic Then
                            chkCatastrophic.Checked = True
                        Else
                            chkCatastrophic.Checked = False
                        End If
                        If StringHasAnyValue(Quote.LossHistoryRecords(LossHistoryNumber).LossDescription) Then txtDescription.Text = Quote.LossHistoryRecords(LossHistoryNumber).LossDescription
                        If StringHasNumericValue(Quote.LossHistoryRecords(LossHistoryNumber).LossHistoryFaultId) Then ddlFaultIndicator.SelectedValue = Quote.LossHistoryRecords(LossHistoryNumber).LossHistoryFaultId
                        If Quote.LossHistoryRecords(LossHistoryNumber).WeatherRelated Then
                            chkWeatherRelated.Checked = True
                        Else
                            chkWeatherRelated.Checked = False
                        End If
                        If StringHasAnyValue(Quote.LossHistoryRecords(LossHistoryNumber).Comments) Then txtComments.Text = Quote.LossHistoryRecords(LossHistoryNumber).Comments
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
        Dim MaxLength As Integer = 55

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
            If StringHasAnyValue(txtDescription.Text) Then
                lblAccordHeader.Text = FormatHeaderText("Loss #" & (LossHistoryNumber + 1).ToString() & ": " & txtDescription.Text)
            Else
                lblAccordHeader.Text = FormatHeaderText("Loss #" & (LossHistoryNumber + 1).ToString())
            End If
            Exit Sub
        Catch ex As Exception
            HandleError("UpdateHeaderText", ex)
            Exit Sub
        End Try
    End Sub

    Public Sub ValidateForm() Implements IVRUI_P.ValidateForm
        Try
            ValidationHelper.Clear()

            ' Claim Number
            If String.IsNullOrWhiteSpace(txtClaimNumber.Text) Then
                Me.ValidationHelper.AddError("Missing Claim Number", txtClaimNumber.ClientID)
                With Me.ValidationHelper.GetLastError()
                    .ScriptCollection.Add("$(""#LossHistoryItemDiv"").accordion(""option"",""active""," + LossHistoryNumber.ToString() + ");")
                    .ScriptCollection.Add("DoValidationSummaryErrorJump(""" + .SenderClientId + """);")
                    .ScriptCollection.Add("$(this).css(""color"",""blue"");")
                End With
            End If

            ' Loss Date
            If Not IsDate(txtLossDate.Text) Then
                Me.ValidationHelper.AddError("Invalid Loss Date", txtLossDate.ClientID)
                With Me.ValidationHelper.GetLastError()
                    .ScriptCollection.Add("$(""#LossHistoryItemDiv"").accordion(""option"",""active""," + LossHistoryNumber.ToString() + ");")
                    .ScriptCollection.Add("DoValidationSummaryErrorJump(""" + .SenderClientId + """);")
                    .ScriptCollection.Add("$(this).css(""color"",""blue"");")
                End With
            Else
                If CDate(txtLossDate.Text) < CDate("1/1/1900") OrElse CDate(txtLossDate.Text) > DateTime.Now Then
                    Me.ValidationHelper.AddError("Loss date must be between 1/1/1900 and todays date", txtLossDate.ClientID)
                    With Me.ValidationHelper.GetLastError()
                        .ScriptCollection.Add("$(""#LossHistoryItemDiv"").accordion(""option"",""active""," + LossHistoryNumber.ToString() + ");")
                        .ScriptCollection.Add("DoValidationSummaryErrorJump(""" + .SenderClientId + """);")
                        .ScriptCollection.Add("$(this).css(""color"",""blue"");")
                    End With
                End If
            End If

            ' Type Of Loss
            If ddlTypeOfLoss.SelectedValue Is Nothing OrElse ddlTypeOfLoss.SelectedValue = "" Then
                Me.ValidationHelper.AddError("Type of Loss must be selected", ddlTypeOfLoss.ClientID)
                With Me.ValidationHelper.GetLastError()
                    .ScriptCollection.Add("$(""#LossHistoryItemDiv"").accordion(""option"",""active""," + LossHistoryNumber.ToString() + ");")
                    .ScriptCollection.Add("DoValidationSummaryErrorJump(""" + .SenderClientId + """);")
                    .ScriptCollection.Add("$(this).css(""color"",""blue"");")
                End With
            End If

            ' Paid Amount
            If String.IsNullOrWhiteSpace(txtPaidAmount.Text) Then
                Me.ValidationHelper.AddError("Missing Paid Amount", txtPaidAmount.ClientID)
                With Me.ValidationHelper.GetLastError()
                    .ScriptCollection.Add("$(""#LossHistoryItemDiv"").accordion(""option"",""active""," + LossHistoryNumber.ToString() + ");")
                    .ScriptCollection.Add("DoValidationSummaryErrorJump(""" + .SenderClientId + """);")
                    .ScriptCollection.Add("$(this).css(""color"",""blue"");")
                End With
            End If
            If Not StringHasNumericValue(txtPaidAmount.Text) Then
                Me.ValidationHelper.AddError("Paid Amount must be numeric", txtPaidAmount.ClientID)
                With Me.ValidationHelper.GetLastError()
                    .ScriptCollection.Add("$(""#LossHistoryItemDiv"").accordion(""option"",""active""," + LossHistoryNumber.ToString() + ");")
                    .ScriptCollection.Add("DoValidationSummaryErrorJump(""" + .SenderClientId + """);")
                    .ScriptCollection.Add("$(this).css(""color"",""blue"");")
                End With
            Else
                If CInt(txtPaidAmount.Text) < 1 OrElse CInt(txtPaidAmount.Text) > 10000000 Then
                    Me.ValidationHelper.AddError("Paid Amount must be between 1 and 10,000,000", txtPaidAmount.ClientID)
                    With Me.ValidationHelper.GetLastError()
                        .ScriptCollection.Add("$(""#LossHistoryItemDiv"").accordion(""option"",""active""," + LossHistoryNumber.ToString() + ");")
                        .ScriptCollection.Add("DoValidationSummaryErrorJump(""" + .SenderClientId + """);")
                        .ScriptCollection.Add("$(this).css(""color"",""blue"");")
                    End With
                End If
            End If

            ' Reserve Amount
            If String.IsNullOrWhiteSpace(txtReserveAmount.Text) Then
                Me.ValidationHelper.AddError("Missing Reserve Amount", txtReserveAmount.ClientID)
                With Me.ValidationHelper.GetLastError()
                    .ScriptCollection.Add("$(""#LossHistoryItemDiv"").accordion(""option"",""active""," + LossHistoryNumber.ToString() + ");")
                    .ScriptCollection.Add("DoValidationSummaryErrorJump(""" + .SenderClientId + """);")
                    .ScriptCollection.Add("$(this).css(""color"",""blue"");")
                End With
            End If
            If Not StringHasNumericValue(txtReserveAmount.Text) Then
                Me.ValidationHelper.AddError("Reserve Amount must be numeric", txtReserveAmount.ClientID)
                With Me.ValidationHelper.GetLastError()
                    .ScriptCollection.Add("$(""#LossHistoryItemDiv"").accordion(""option"",""active""," + LossHistoryNumber.ToString() + ");")
                    .ScriptCollection.Add("DoValidationSummaryErrorJump(""" + .SenderClientId + """);")
                    .ScriptCollection.Add("$(this).css(""color"",""blue"");")
                End With
            Else
                If CInt(txtReserveAmount.Text) < 1 OrElse CInt(txtReserveAmount.Text) > 10000000 Then
                    Me.ValidationHelper.AddError("Reserve Amount must be between 1 and 10,000,000", txtReserveAmount.ClientID)
                    With Me.ValidationHelper.GetLastError()
                        .ScriptCollection.Add("$(""#LossHistoryItemDiv"").accordion(""option"",""active""," + LossHistoryNumber.ToString() + ");")
                        .ScriptCollection.Add("DoValidationSummaryErrorJump(""" + .SenderClientId + """);")
                        .ScriptCollection.Add("$(this).css(""color"",""blue"");")
                    End With
                End If
            End If

            ' Surcharge
            If ddlSurcharge.SelectedValue Is Nothing OrElse ddlSurcharge.SelectedValue = "" Then
                Me.ValidationHelper.AddError("Surcharge must be selected", ddlSurcharge.ClientID)
                With Me.ValidationHelper.GetLastError()
                    .ScriptCollection.Add("$(""#LossHistoryItemDiv"").accordion(""option"",""active""," + LossHistoryNumber.ToString() + ");")
                    .ScriptCollection.Add("DoValidationSummaryErrorJump(""" + .SenderClientId + """);")
                    .ScriptCollection.Add("$(this).css(""color"",""blue"");")
                End With
            End If

            ' Source
            If ddlSource.SelectedValue Is Nothing OrElse ddlSource.SelectedValue = "" Then
                Me.ValidationHelper.AddError("Source must be selected", ddlSource.ClientID)
                With Me.ValidationHelper.GetLastError()
                    .ScriptCollection.Add("$(""#LossHistoryItemDiv"").accordion(""option"",""active""," + LossHistoryNumber.ToString() + ");")
                    .ScriptCollection.Add("DoValidationSummaryErrorJump(""" + .SenderClientId + """);")
                    .ScriptCollection.Add("$(this).css(""color"",""blue"");")
                End With
            End If

            ' Catastrophic (checkbox)

            ' Description
            If String.IsNullOrWhiteSpace(txtDescription.Text) Then
                Me.ValidationHelper.AddError("Missing Description", txtDescription.ClientID)
                With Me.ValidationHelper.GetLastError()
                    .ScriptCollection.Add("$(""#LossHistoryItemDiv"").accordion(""option"",""active""," + LossHistoryNumber.ToString() + ");")
                    .ScriptCollection.Add("DoValidationSummaryErrorJump(""" + .SenderClientId + """);")
                    .ScriptCollection.Add("$(this).css(""color"",""blue"");")
                End With
            End If

            ' Fault
            If ddlFaultIndicator.SelectedValue Is Nothing OrElse ddlFaultIndicator.SelectedValue = "" Then
                Me.ValidationHelper.AddError("Fault Indicator must be selected", ddlFaultIndicator.ClientID)
                With Me.ValidationHelper.GetLastError()
                    .ScriptCollection.Add("$(""#LossHistoryItemDiv"").accordion(""option"",""active""," + LossHistoryNumber.ToString() + ");")
                    .ScriptCollection.Add("DoValidationSummaryErrorJump(""" + .SenderClientId + """);")
                    .ScriptCollection.Add("$(this).css(""color"",""blue"");")
                End With
            End If

            ' Weather Related (checkbox)
            ' Comments (not required)

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
        Try
            Dim _script = DirectCast(Me.Page.Master, VelociRater).StartUpScriptManager
            _script.AddScriptLine("$(""#" + lnkRemove.ClientID + """).bind(""click"", function (e) { e.stopPropagation();});")
            _script.AddScriptLine("$(""#" + lnkSave.ClientID + """).bind(""click"", function (e) { e.stopPropagation();});")

            If ddlTypeOfLoss.Items.Count <= 0 Then LoadStaticData()

            Exit Sub
        Catch ex As Exception
            HandleError("PAGE LOAD", ex)
            Exit Sub
        End Try
    End Sub

    Protected Sub lnkSave_Click(ByVal sender As Object, ByVal e As EventArgs) Handles lnkSave.Click
        Try
            ClearErrorMessage()
            ValidateForm()
            If Me.ValidationHelper.HasErrros Then Exit Sub
            Save()
            UpdateHeaderText()
            RaiseEvent SaveRequested(LossHistoryNumber, ClassName)
            ShowMessage("Item Saved")
            Exit Sub
        Catch ex As Exception
            HandleError("lnkBtnSave_Click", ex)
            Exit Sub
        End Try
    End Sub

    Protected Sub lnkRemove_Click(ByVal sender As Object, ByVal e As EventArgs) Handles lnkRemove.Click
        Try

            If Quote IsNot Nothing AndAlso Quote.LossHistoryRecords IsNot Nothing _
                AndAlso Quote.LossHistoryRecords.Count > 0 Then
                Me.Quote.LossHistoryRecords.RemoveAt(LossHistoryNumber)
                RaiseEvent ItemRemoveRequest(LossHistoryNumber)
                Exit Sub
            End If

            Exit Sub
        Catch ex As Exception
            HandleError("lnkRemove_Click", ex)
            Exit Sub
        End Try
    End Sub

#End Region
End Class