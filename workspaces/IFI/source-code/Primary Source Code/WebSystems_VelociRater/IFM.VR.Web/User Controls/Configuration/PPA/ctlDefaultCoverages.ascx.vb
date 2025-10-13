Imports QuickQuote.CommonObjects
Imports IFM.VR.Web.Helpers
Imports QuickQuote.CommonMethods

Public Class ctlDefaultCoverages
    Inherits System.Web.UI.UserControl
    Implements IVRUI_P

    Dim qqHelper As New QuickQuoteHelperClass
    Dim QuickQuote As QuickQuoteObject

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim _script = DirectCast(Me.Page.Master, VelociRater).StartUpScriptManager
        _script.AddScriptLine("$(""#" + Me.divUserDefaultCoverage.ClientID + """).accordion({heightStyle: ""content"", active: 0, collapsible: false, activate: function(event, ui) { ToggleHiddenField('" + Me.hiddenDriverAssignment.ClientID + "');   } });")
        _script.AddScriptLine("$(""#" + Me.divUserDefaultCoverage.ClientID + """).find("">:first-child"").find("">:first-child"").hide();")
    End Sub

    Public Sub LoadStaticData() Implements IVRUI_P.LoadStaticData
        qqHelper.LoadStaticDataOptionsDropDown(ddlComp, QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuoteHelperClass.QuickQuotePropertyName.ComprehensiveDeductibleLimitId, QuickQuoteStaticDataOption.SortBy.None, Me.Quote.LobType)
        qqHelper.LoadStaticDataOptionsDropDown(ddlColl, QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuoteHelperClass.QuickQuotePropertyName.CollisionDeductibleLimitId, QuickQuoteStaticDataOption.SortBy.None, Me.Quote.LobType)
        qqHelper.LoadStaticDataOptionsDropDown(ddlSLL, QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuoteHelperClass.QuickQuotePropertyName.Liability_UM_UIM_LimitId, QuickQuoteStaticDataOption.SortBy.None, Me.Quote.LobType)
        qqHelper.LoadStaticDataOptionsDropDown(ddlBI, QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuoteHelperClass.QuickQuotePropertyName.BodilyInjuryLiabilityLimitId, QuickQuoteStaticDataOption.SortBy.None, Me.Quote.LobType)
        qqHelper.LoadStaticDataOptionsDropDown(ddlPD, QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuoteHelperClass.QuickQuotePropertyName.PropertyDamageLimitId, QuickQuoteStaticDataOption.SortBy.None, Me.Quote.LobType)
        qqHelper.LoadStaticDataOptionsDropDown(ddlMedical, QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuoteHelperClass.QuickQuotePropertyName.MedicalPaymentsLimitId, QuickQuoteStaticDataOption.SortBy.None, Me.Quote.LobType)
        'Updated 10/4/18 for multi state MLW
        Select Case (Quote.QuickQuoteState)
            Case QuickQuoteHelperClass.QuickQuoteState.Illinois
                qqHelper.LoadStaticDataOptionsDropDownForState(ddlUIMSL, QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuoteHelperClass.QuickQuotePropertyName.UninsuredCombinedSingleLimitId, Quote.QuickQuoteState, QuickQuoteStaticDataOption.SortBy.None, Me.Quote.LobType)
                qqHelper.LoadStaticDataOptionsDropDownForState(ddlUIMBI, QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuoteHelperClass.QuickQuotePropertyName.UninsuredBodilyInjuryLimitId, Quote.QuickQuoteState, QuickQuoteStaticDataOption.SortBy.None, Me.Quote.LobType)
            Case Else
                qqHelper.LoadStaticDataOptionsDropDown(ddlUIMSL, QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuoteHelperClass.QuickQuotePropertyName.UninsuredCombinedSingleLimitId, QuickQuoteStaticDataOption.SortBy.None, Me.Quote.LobType)
                qqHelper.LoadStaticDataOptionsDropDown(ddlUIMBI, QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuoteHelperClass.QuickQuotePropertyName.UninsuredMotoristLiabilityLimitId, QuickQuoteStaticDataOption.SortBy.None, Me.Quote.LobType)
                qqHelper.LoadStaticDataOptionsDropDown(ddlUMPD, QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuoteHelperClass.QuickQuotePropertyName.UninsuredMotoristPropertyDamageLimitId, QuickQuoteStaticDataOption.SortBy.None, Me.Quote.LobType)
                qqHelper.LoadStaticDataOptionsDropDown(ddlUMPDDed, QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuoteHelperClass.QuickQuotePropertyName.UninsuredMotoristPropertyDamageDeductibleLimitId, QuickQuoteStaticDataOption.SortBy.None, Me.Quote.LobType)
        End Select
        qqHelper.LoadStaticDataOptionsDropDown(ddlRental, QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuoteHelperClass.QuickQuotePropertyName.TransportationExpenseLimitId, QuickQuoteStaticDataOption.SortBy.None, Me.Quote.LobType)
        qqHelper.LoadStaticDataOptionsDropDown(ddlTowing, QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuoteHelperClass.QuickQuotePropertyName.TowingAndLaborDeductibleLimitId, QuickQuoteStaticDataOption.SortBy.None, Me.Quote.LobType)
        qqHelper.LoadStaticDataOptionsDropDown(ddlMedia, QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuoteHelperClass.QuickQuotePropertyName.TapesAndRecordsLimitId, QuickQuoteStaticDataOption.SortBy.None, Me.Quote.LobType)

        'Populate Interruption of Travel
        ddlTraveInterrupt.Items.Add("Yes")
        ddlTraveInterrupt.Items.Add("No")

        'Populate Radio Dropdown
        ddlSoundEquip.Items.Add("[1,000 INC]")
        ddlSoundEquip.Items.Add("1,500")
        ddlSoundEquip.Items.Add("2,000")
        ddlSoundEquip.Items.Add("2,500")
        ddlSoundEquip.Items.Add("3,000")

        ddlSoundEquip.SelectedIndex = 0

        'Populate A/V/D Equipment
        ddlAVEquip.Items.Add("N/A")
        ddlAVEquip.Items.Add("500")
        ddlAVEquip.Items.Add("1,000")
        ddlAVEquip.Items.Add("1,500")
        ddlAVEquip.Items.Add("2,000")
        ddlAVEquip.Items.Add("2,500")
        ddlAVEquip.Items.Add("3,000")

        ddlAVEquip.SelectedIndex = 0

        'Populate Pollution Liability
        ddlPollution.Items.Add("Yes")
        ddlPollution.Items.Add("No")

        ' Added items here so they could be added & removed from the dropdown list
        ddlMedia.Items.Insert(0, "N/A")
        ddlMedia.Items.Insert(1, "200")
    End Sub

    Public Sub Populate() Implements IVRUI_P.Populate

    End Sub

    Public Sub DefaultCoverage_Populate(defaultCoverages As List(Of DefaultCoverage.DefaultCoverageGrid), ByRef currentRow As Integer)
        LoadStaticData()

        lblVehType.Text = defaultCoverages(currentRow).VehicleType

        'Set Values
        IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromValue(ddlComp, qqHelper.GetStaticDataValueForText(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuoteHelperClass.QuickQuotePropertyName.ComprehensiveDeductibleLimitId, defaultCoverages(currentRow).Comprehensive, Me.Quote.LobType))
        IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromValue(ddlColl, qqHelper.GetStaticDataValueForText(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuoteHelperClass.QuickQuotePropertyName.CollisionDeductibleLimitId, defaultCoverages(currentRow).Collison, Me.Quote.LobType))
        IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromValue(ddlSLL, qqHelper.GetStaticDataValueForText(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuoteHelperClass.QuickQuotePropertyName.Liability_UM_UIM_LimitId, defaultCoverages(currentRow).SingleLimitLib, Me.Quote.LobType))
        IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromValue(ddlBI, qqHelper.GetStaticDataValueForText(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuoteHelperClass.QuickQuotePropertyName.BodilyInjuryLiabilityLimitId, defaultCoverages(currentRow).BodilyInjury, Me.Quote.LobType))
        IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromValue(ddlPD, qqHelper.GetStaticDataValueForText(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuoteHelperClass.QuickQuotePropertyName.PropertyDamageLimitId, defaultCoverages(currentRow).PropertyDamage, Me.Quote.LobType))
        IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromValue(ddlMedical, qqHelper.GetStaticDataValueForText(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuoteHelperClass.QuickQuotePropertyName.MedicalPaymentsLimitId, defaultCoverages(currentRow).MedicalPayments, Me.Quote.LobType))
        'Updated 10/4/18 for multi state MLW
        Select Case (Quote.QuickQuoteState)
            Case QuickQuoteHelperClass.QuickQuoteState.Illinois
                IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromValue(ddlUIMSL, qqHelper.GetStaticDataValueForTextAndState(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuoteHelperClass.QuickQuotePropertyName.UninsuredCombinedSingleLimitId, Quote.QuickQuoteState, defaultCoverages(currentRow).UmUimSSl, Me.Quote.LobType))
                IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromValue(ddlUIMBI, qqHelper.GetStaticDataValueForTextAndState(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuoteHelperClass.QuickQuotePropertyName.UninsuredBodilyInjuryLimitId, Quote.QuickQuoteState, defaultCoverages(currentRow).UmUmiBi, Me.Quote.LobType))
            Case Else
                IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromValue(ddlUIMSL, qqHelper.GetStaticDataValueForText(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuoteHelperClass.QuickQuotePropertyName.UninsuredCombinedSingleLimitId, defaultCoverages(currentRow).UmUimSSl, Me.Quote.LobType))
                IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromValue(ddlUIMBI, qqHelper.GetStaticDataValueForText(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuoteHelperClass.QuickQuotePropertyName.UninsuredMotoristLiabilityLimitId, defaultCoverages(currentRow).UmUmiBi, Me.Quote.LobType))
                IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromValue(ddlUMPD, qqHelper.GetStaticDataValueForText(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuoteHelperClass.QuickQuotePropertyName.UninsuredMotoristPropertyDamageLimitId, defaultCoverages(currentRow).UmPd, Me.Quote.LobType))
                IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromValue(ddlUMPDDed, qqHelper.GetStaticDataValueForText(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuoteHelperClass.QuickQuotePropertyName.UninsuredMotoristPropertyDamageDeductibleLimitId, defaultCoverages(currentRow).UmDeductible, Me.Quote.LobType))
        End Select

        If IFM.VR.Common.Helpers.PPA.TransportationExpenseHelper.IsTransportationExpenseAvailable(Quote) = False Then
            If defaultCoverages(currentRow).Rent = "Yes" Then
                IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromValue(ddlRental, qqHelper.GetStaticDataValueForText(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuoteHelperClass.QuickQuotePropertyName.TransportationExpenseLimitId, "30/900", Me.Quote.LobType))
            Else
                IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromValue(ddlRental, qqHelper.GetStaticDataValueForText(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuoteHelperClass.QuickQuotePropertyName.TransportationExpenseLimitId, "[20/600 INC]", Me.Quote.LobType))
            End If
        Else
            If defaultCoverages(currentRow).Rent = "Yes" Then
                IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromValue(ddlRental, qqHelper.GetStaticDataValueForText(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuoteHelperClass.QuickQuotePropertyName.TransportationExpenseLimitId, "50/1500", Me.Quote.LobType))
            Else
                IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromValue(ddlRental, qqHelper.GetStaticDataValueForText(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuoteHelperClass.QuickQuotePropertyName.TransportationExpenseLimitId, "[40/1200 INC]", Me.Quote.LobType))
            End If
        End If

        IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromValue(ddlTowing, qqHelper.GetStaticDataValueForText(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuoteHelperClass.QuickQuotePropertyName.TowingAndLaborDeductibleLimitId, defaultCoverages(currentRow).Towing, Me.Quote.LobType))

        If defaultCoverages(currentRow).Travel = "Yes" Then
            ddlTraveInterrupt.SelectedIndex = 0
        Else
            ddlTraveInterrupt.SelectedIndex = 1
        End If

        If defaultCoverages(currentRow).Pollution = "Yes" Then
            ddlPollution.SelectedIndex = 0
        Else
            ddlPollution.SelectedIndex = 1
        End If

        'Field specific changes based on Vehicle Type
        Select Case defaultCoverages(currentRow).VehicleType
            Case "Car"
                pnlTravelInterruptData.Enabled = False
            Case "Trk/Mini/SUV"
                pnlTravelInterruptData.Enabled = False
            Case "Motorcycle"
                pnlSoundData.Enabled = False
                pnlAVEquipData.Enabled = False
            Case Else   'All Other Types
                pnlTravelInterruptData.Enabled = False
        End Select

        currentRow += 1
    End Sub

    Public ReadOnly Property Quote As QuickQuoteObject Implements IVRUI_P.Quote
        Get
            Dim errCreateQSO As String = ""
            If String.IsNullOrWhiteSpace(ReadOnlyPolicyIdAndImageNum) = False Then 'added IF 2/18/2019; original logic in ELSE
                Return VR.Common.QuoteSave.QuoteSaveHelpers.GetReadOnlyImageFromAnywhere(ReadOnlyPolicyId, ReadOnlyPolicyImageNum, expectedLobType:=QuickQuoteObject.QuickQuoteLobType.AutoPersonal, errorMessage:=errCreateQSO)
            ElseIf String.IsNullOrWhiteSpace(EndorsementPolicyIdAndImageNum) = False Then
                Return VR.Common.QuoteSave.QuoteSaveHelpers.GetEndorsementQuoteFromAnywhere(EndorsementPolicyId, EndorsementPolicyImageNum, expectedLobType:=QuickQuoteObject.QuickQuoteLobType.AutoPersonal, errorMessage:=errCreateQSO)
            Else
                Return VR.Common.QuoteSave.QuoteSaveHelpers.GetQuoteById(QuickQuoteObject.QuickQuoteLobType.AutoPersonal, QuoteId, errCreateQSO)
            End If
        End Get
    End Property

    Public ReadOnly Property QuoteId As String Implements IVRUI_P.QuoteId
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

    'added 2/18/2019
    Private _EndorsementPolicyId As Integer = 0
    Public ReadOnly Property EndorsementPolicyId As Integer
        Get
            If _EndorsementPolicyId <= 0 Then
                LoadEndorsementPolicyIdAndImageNum()
            End If
            Return _EndorsementPolicyId
        End Get
    End Property
    Private _EndorsementPolicyImageNum As Integer = 0
    Public ReadOnly Property EndorsementPolicyImageNum As Integer
        Get
            If _EndorsementPolicyImageNum <= 0 Then
                LoadEndorsementPolicyIdAndImageNum()
            End If
            Return _EndorsementPolicyImageNum
        End Get
    End Property
    Public ReadOnly Property EndorsementPolicyIdAndImageNum As String
        Get
            Dim strEndorsementPolicyIdAndImageNum As String = ""
            If Request IsNot Nothing AndAlso Request.QueryString IsNot Nothing AndAlso Request.QueryString("EndorsementPolicyIdAndImageNum") IsNot Nothing AndAlso String.IsNullOrWhiteSpace(Request.QueryString("EndorsementPolicyIdAndImageNum").ToString) = False Then
                strEndorsementPolicyIdAndImageNum = Request.QueryString("EndorsementPolicyIdAndImageNum").ToString
            ElseIf Page IsNot Nothing AndAlso Page.RouteData IsNot Nothing AndAlso Page.RouteData.Values IsNot Nothing AndAlso Page.RouteData.Values("EndorsementPolicyIdAndImageNum") IsNot Nothing AndAlso String.IsNullOrWhiteSpace(Page.RouteData.Values("EndorsementPolicyIdAndImageNum").ToString) = False Then
                strEndorsementPolicyIdAndImageNum = Page.RouteData.Values("EndorsementPolicyIdAndImageNum").ToString
            End If
            Return strEndorsementPolicyIdAndImageNum
        End Get
    End Property
    Private Sub LoadEndorsementPolicyIdAndImageNum(Optional ByVal reset As Boolean = False)
        If reset = True Then
            _EndorsementPolicyId = 0
            _EndorsementPolicyImageNum = 0
        End If
        Dim strEndorsementPolicyIdAndImageNum As String = EndorsementPolicyIdAndImageNum
        If String.IsNullOrWhiteSpace(strEndorsementPolicyIdAndImageNum) = False Then
            Dim intList As List(Of Integer) = QuickQuoteHelperClass.ListOfIntegerFromString(strEndorsementPolicyIdAndImageNum, delimiter:="|", positiveOnly:=True)
            If intList IsNot Nothing AndAlso intList.Count > 0 Then
                _EndorsementPolicyId = intList(0)
                If intList.Count > 1 Then
                    _EndorsementPolicyImageNum = intList(1)
                End If
            End If
        End If
    End Sub
    Private _ReadOnlyPolicyId As Integer = 0
    Public ReadOnly Property ReadOnlyPolicyId As Integer
        Get
            If _ReadOnlyPolicyId <= 0 Then
                LoadReadOnlyPolicyIdAndImageNum()
            End If
            Return _ReadOnlyPolicyId
        End Get
    End Property
    Private _ReadOnlyPolicyImageNum As Integer = 0
    Public ReadOnly Property ReadOnlyPolicyImageNum As Integer
        Get
            If _ReadOnlyPolicyImageNum <= 0 Then
                LoadReadOnlyPolicyIdAndImageNum()
            End If
            Return _ReadOnlyPolicyImageNum
        End Get
    End Property
    Public ReadOnly Property ReadOnlyPolicyIdAndImageNum As String
        Get
            Dim strReadOnlyPolicyIdAndImageNum As String = ""
            If Request IsNot Nothing AndAlso Request.QueryString IsNot Nothing AndAlso Request.QueryString("ReadOnlyPolicyIdAndImageNum") IsNot Nothing AndAlso String.IsNullOrWhiteSpace(Request.QueryString("ReadOnlyPolicyIdAndImageNum").ToString) = False Then
                strReadOnlyPolicyIdAndImageNum = Request.QueryString("ReadOnlyPolicyIdAndImageNum").ToString
            ElseIf Page IsNot Nothing AndAlso Page.RouteData IsNot Nothing AndAlso Page.RouteData.Values IsNot Nothing AndAlso Page.RouteData.Values("ReadOnlyPolicyIdAndImageNum") IsNot Nothing AndAlso String.IsNullOrWhiteSpace(Page.RouteData.Values("ReadOnlyPolicyIdAndImageNum").ToString) = False Then
                strReadOnlyPolicyIdAndImageNum = Page.RouteData.Values("ReadOnlyPolicyIdAndImageNum").ToString
            End If
            Return strReadOnlyPolicyIdAndImageNum
        End Get
    End Property
    Private Sub LoadReadOnlyPolicyIdAndImageNum(Optional ByVal reset As Boolean = False)
        If reset = True Then
            _ReadOnlyPolicyId = 0
            _ReadOnlyPolicyImageNum = 0
        End If
        Dim strReadOnlyPolicyIdAndImageNum As String = ReadOnlyPolicyIdAndImageNum
        If String.IsNullOrWhiteSpace(strReadOnlyPolicyIdAndImageNum) = False Then
            Dim intList As List(Of Integer) = QuickQuoteHelperClass.ListOfIntegerFromString(strReadOnlyPolicyIdAndImageNum, delimiter:="|", positiveOnly:=True)
            If intList IsNot Nothing AndAlso intList.Count > 0 Then
                _ReadOnlyPolicyId = intList(0)
                If intList.Count > 1 Then
                    _ReadOnlyPolicyImageNum = intList(1)
                End If
            End If
        End If
    End Sub

    Public Function Save() As Boolean Implements IVRUI_P.Save
        Return False
    End Function

    Public Sub ValidateForm() Implements IVRUI_P.ValidateForm

    End Sub

    Public Property ValidationHelper As ControlValidationHelper Implements IVRUI_P.ValidationHelper
End Class