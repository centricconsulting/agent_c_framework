Imports QuickQuote.CommonObjects
Imports QuickQuote.CommonMethods
Imports IFM.VR.Web.Helpers

Public Class ctlRVWatercraftMotorItem
    Inherits System.Web.UI.UserControl
    Implements IVRUI_P

#Region "Declarations"
    Private Const ClassName As String = "ctlRVWatercraftItem"
    Public Event ItemRemoveRequest(index As Int32)

    Public Property ValidationHelper As ControlValidationHelper Implements IVRUI_P.ValidationHelper

    Private _quote As QuickQuoteObject
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

    Private _motornumber As Int32 = -1
    Public Property MotorNumber As Int32
        Get
            Return _motornumber
        End Get
        Set(value As Int32)
            _motornumber = value
        End Set
    End Property

    Private _rvwatercraftnumber As Int32 = -1
    Public Property RVWatercraftNumber As Int32
        Get
            Return _rvwatercraftnumber
        End Get
        Set(value As Int32)
            _rvwatercraftnumber = value
        End Set
    End Property

#End Region

#Region "Methods and Functions"
    Private Sub HandleError(ByVal RoutineName As String, ByVal exc As Exception)
        lblMsg.Text = ClassName & ":" & RoutineName & ": " & exc.Message
    End Sub

    Private Sub ClearErrorMessage()
        lblMsg.Text = "&nbsp;"
    End Sub

    Private Sub LoadStaticData() Implements IVRUI_P.LoadStaticData
        Dim qqHelper As New QuickQuoteHelperClass
        Try
            qqHelper.LoadStaticDataOptionsDropDown(Me.ddlMotorType, QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteRvWatercraft, QuickQuoteHelperClass.QuickQuotePropertyName.MotorTypeId, QuickQuoteStaticDataOption.SortBy.None, Me.Quote.LobType)
            Exit Sub
        Catch ex As Exception
            HandleError("LoadStaticData", ex)
            Exit Sub
        End Try
    End Sub

    Public Sub Populate() Implements IVRUI_P.Populate
        Try
            If Quote IsNot Nothing AndAlso Quote.Locations IsNot Nothing AndAlso Quote.Locations(0).RvWatercrafts IsNot Nothing _
                AndAlso Quote.Locations(0).RvWatercrafts(RVWatercraftNumber) IsNot Nothing AndAlso
                Quote.Locations(0).RvWatercrafts(RVWatercraftNumber).RvWatercraftMotors IsNot Nothing AndAlso
                Quote.Locations(0).RvWatercrafts(RVWatercraftNumber).RvWatercraftMotors(MotorNumber) IsNot Nothing Then
                If Quote.Locations(0).RvWatercrafts(RVWatercraftNumber).RvWatercraftMotors.Count >= MotorNumber Then
                    If IFM.Common.InputValidation.InputHelpers.StringHasNumericValue(Quote.Locations(0).RvWatercrafts(RVWatercraftNumber).RvWatercraftMotors(MotorNumber).MotorTypeId) Then ddlMotorType.SelectedValue = Quote.Locations(0).RvWatercrafts(RVWatercraftNumber).RvWatercraftMotors(MotorNumber).MotorTypeId
                    If IFM.Common.InputValidation.InputHelpers.StringHasAnyValue(Quote.Locations(0).RvWatercrafts(RVWatercraftNumber).RvWatercraftMotors(MotorNumber).Year) Then txtYear.Text = Quote.Locations(0).RvWatercrafts(RVWatercraftNumber).RvWatercraftMotors(MotorNumber).Year
                    If IFM.Common.InputValidation.InputHelpers.StringHasAnyValue(Quote.Locations(0).RvWatercrafts(RVWatercraftNumber).RvWatercraftMotors(MotorNumber).Manufacturer) Then txtManufacturer.Text = Quote.Locations(0).RvWatercrafts(RVWatercraftNumber).RvWatercraftMotors(MotorNumber).Manufacturer
                    If IFM.Common.InputValidation.InputHelpers.StringHasAnyValue(Quote.Locations(0).RvWatercrafts(RVWatercraftNumber).RvWatercraftMotors(MotorNumber).Model) Then txtModel.Text = Quote.Locations(0).RvWatercrafts(RVWatercraftNumber).RvWatercraftMotors(MotorNumber).Model
                    If IFM.Common.InputValidation.InputHelpers.StringHasAnyValue(Quote.Locations(0).RvWatercrafts(RVWatercraftNumber).RvWatercraftMotors(MotorNumber).SerialNumber) Then txtSerialNumber.Text = Quote.Locations(0).RvWatercrafts(RVWatercraftNumber).RvWatercraftMotors(MotorNumber).SerialNumber
                    If IFM.Common.InputValidation.InputHelpers.StringHasNumericValue(Quote.Locations(0).RvWatercrafts(RVWatercraftNumber).RvWatercraftMotors(MotorNumber).CostNew) Then txtCostNew.Text = Quote.Locations(0).RvWatercrafts(RVWatercraftNumber).RvWatercraftMotors(MotorNumber).CostNew
                End If
            End If

            Exit Sub
        Catch ex As Exception
            HandleError("Populate", ex)
            Exit Sub
        End Try
    End Sub

    Public Function Save() As Boolean Implements IVRUI_P.Save
        Try
            Return True
        Catch ex As Exception
            HandleError("Save", ex)
            Return False
        End Try
    End Function

    Public Sub ValidateForm() Implements IVRUI_P.ValidateForm
        Try

        Catch ex As Exception
            HandleError("ValidateForm", ex)
            Exit Sub
        End Try
    End Sub
#End Region

#Region "Events"

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try
            If Not IsPostBack Then
                LoadStaticData()
                Populate()
            End If
        Catch ex As Exception
            HandleError("PAGE LOAD", ex)
            Exit Sub
        End Try
    End Sub
#End Region

End Class