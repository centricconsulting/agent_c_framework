Imports IFM.VR.Web.Helpers
Imports QuickQuote.CommonObjects
Imports QuickQuote.CommonMethods

Public Class ctlQsummary_PPA_Vehicle_List
    Inherits System.Web.UI.UserControl
    Implements IVRUI_P

    Protected ReadOnly Property Quote As QuickQuote.CommonObjects.QuickQuoteObject Implements IVRUI_P.Quote
        Get
            Return DirectCast(Me.Page.Master, VelociRater).GetRatedQuotefromCache() '6-3-14 - Matt
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

    Public ReadOnly Property IsAppPageMode As Boolean
        Get
            If TypeOf Me.Page Is VR3AutoApp Then
                Return True
            End If
            Return False
        End Get
    End Property

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim _script = DirectCast(Me.Page.Master, VelociRater).StartUpScriptManager
        '_script.AddScriptLine("$(""#" + Me.lnkPrint.ClientID + """).bind(""click"", function (e) { e.stopPropagation();return true; });")

    End Sub

    Public Sub LoadStaticData() Implements IVRUI_P.LoadStaticData

    End Sub

    Public Sub Populate() Implements IVRUI_P.Populate

        If Quote IsNot Nothing Then
            If Quote.Vehicles IsNot Nothing Then
                Me.Repeater1.DataSource = Quote.Vehicles
                Me.Repeater1.DataBind()
            End If
        End If
    End Sub

    Public Function Save() As Boolean Implements IVRUI_P.Save
        Return True
    End Function

    Public Sub ValidateForm() Implements IVRUI_P.ValidateForm
        Me.ValidationHelper.GroupName = String.Format("Quote Summary Vehicle List")

        Me.ValidationHelper.Clear()
    End Sub

    Private Sub Repeater1_ItemDataBound(sender As Object, e As RepeaterItemEventArgs) Handles Repeater1.ItemDataBound
        Dim vehicleControl As ctlQsummary_PPA_Vehicle = e.Item.FindControl("ctlQsummary_PPA_Vehicle")
        vehicleControl.VehicleNumber = e.Item.ItemIndex
        vehicleControl.Populate()
    End Sub
End Class