Imports QuickQuote.CommonObjects
Imports IFM.VR.Web.Helpers
Imports QuickQuote.CommonMethods

Public Class ctlDwellingList
    Inherits System.Web.UI.UserControl
    Implements IVRUI_P

    Public Event ToggleAddlDwelling(status As Boolean)

    Protected ReadOnly Property Quote As QuickQuote.CommonObjects.QuickQuoteObject Implements IVRUI_P.Quote
        Get
            'Dim errCreateQSO As String = ""
            'If IsAppPageMode Then
            '    Return VR.Common.QuoteSave.QuoteSaveHelpers.GetRatedQuoteById_NOSESSION(Me.QuoteId, errCreateQSO, QuickQuoteObject.QuickQuoteLobType.Farm, QuickQuoteXML.QuickQuoteSaveType.AppGap)
            'Else
            '    Return VR.Common.QuoteSave.QuoteSaveHelpers.GetRatedQuoteById_NOSESSION(Me.QuoteId, errCreateQSO, QuickQuoteObject.QuickQuoteLobType.Farm)
            'End If
            Return New QuickQuote.CommonObjects.QuickQuoteObject
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

    Public Property LocalQuickQuote() As QuickQuote.CommonObjects.QuickQuoteObject
        Get
            Return Session("sess_LocalQuickQuote")
        End Get
        Set(ByVal value As QuickQuote.CommonObjects.QuickQuoteObject)
            Session("sess_LocalQuickQuote") = value
        End Set
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
            If TypeOf Me.Page Is VR3FarmApp Then
                Return True
            End If
            Return False
        End Get
    End Property

    Private _addlDwellingExist As Boolean
    Public Property AddlDwellingExist() As Boolean
        Get
            Return _addlDwellingExist
        End Get
        Set(ByVal value As Boolean)
            _addlDwellingExist = value
        End Set
    End Property


    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            Populate()
        End If
    End Sub

    Private Sub dlAddlDwelling_ItemDataBound(sender As Object, e As DataListItemEventArgs) Handles dlAddlDwelling.ItemDataBound
        If LocalQuickQuote IsNot Nothing AndAlso LocalQuickQuote.Locations IsNot Nothing Then
            Dim itemIdx As Integer = e.Item.ItemIndex + 1
            If itemIdx < LocalQuickQuote.Locations.Count Then
                Dim addlDwelling As ctlDwelling = e.Item.FindControl("ctlDwelling")
                addlDwelling.RowNumber = itemIdx
                addlDwelling.UseAddlDwelling = True
                addlDwelling.Populate()
            End If
        End If
    End Sub

    Protected Sub ValidateForm() Implements IVRUI_P.ValidateForm

    End Sub

    Protected Sub Populate() Implements IVRUI_P.Populate
        If LocalQuickQuote.Locations(0).ProgramTypeId <> "6" And LocalQuickQuote.Locations(0).ProgramTypeId <> "8" Then
        Else
            dlAddlDwelling.DataSource = Nothing
            If LocalQuickQuote IsNot Nothing Then
                Dim filterAddlDwelling As List(Of QuickQuoteLocation) = New List(Of QuickQuoteLocation)
                For dwellingIdx As Integer = 1 To LocalQuickQuote.Locations.Count - 1
                    filterAddlDwelling.Add(LocalQuickQuote.Locations(dwellingIdx))

                    If Not AddlDwellingExist Then
                        If LocalQuickQuote.Locations(dwellingIdx).FormTypeId <> "" And LocalQuickQuote.Locations(dwellingIdx).FormTypeId <> "13" Then
                            AddlDwellingExist = True
                        End If
                    End If
                Next

                dlAddlDwelling.DataSource = filterAddlDwelling
                dlAddlDwelling.DataBind()
                RaiseEvent ToggleAddlDwelling(AddlDwellingExist)
            End If
        End If
    End Sub

    Protected Sub LoadStaticData() Implements IVRUI_P.LoadStaticData

    End Sub

    Protected Function Save() As Boolean Implements IVRUI_P.Save
        Return False
    End Function
End Class