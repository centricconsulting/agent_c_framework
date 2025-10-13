Imports QuickQuote.CommonObjects
Imports IFM.VR.Web.Helpers
Imports QuickQuote.CommonMethods
Imports System.Globalization

Public Class ctlPeakSeasons
    Inherits System.Web.UI.UserControl
    Implements IVRUI_P

    Dim qqHelper As New QuickQuote.CommonMethods.QuickQuoteHelperClass

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

    Public Property RowNumber As Int32
        Get
            If ViewState("vs_rowNumber") Is Nothing Then
                ViewState("vs_rowNumber") = 0
            End If
            Return CInt(ViewState("vs_rowNumber"))
        End Get
        Set(value As Int32)
            ViewState("vs_rowNumber") = value
        End Set
    End Property

    Public Property PeakSeasonNumber As Int32
        Get
            If ViewState("vs_peakSeasonNumber") Is Nothing Then
                ViewState("vs_peakSeasonNumber") = 0
            End If
            Return CInt(ViewState("vs_peakSeasonNumber"))
        End Get
        Set(value As Int32)
            ViewState("vs_peakSeasonNumber") = value
        End Set
    End Property

    Public Property PeakSeasonList() As DataTable
        Get
            Return ViewState("vs_PeakSeason")
        End Get
        Set(ByVal value As DataTable)
            ViewState("vs_PeakSeason") = value
        End Set
    End Property

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            Populate()
        End If
    End Sub

    Public Sub LoadStaticData() Implements IVRUI_P.LoadStaticData

    End Sub

    Public Sub Populate() Implements IVRUI_P.Populate
        ' Coverage Table
        Dim dtPeakSeason As New DataTable
        dtPeakSeason.Columns.Add("Description", System.Type.GetType("System.String"))
        dtPeakSeason.Columns.Add("StartDate", System.Type.GetType("System.String"))
        dtPeakSeason.Columns.Add("EndDate", System.Type.GetType("System.String"))
        dtPeakSeason.Columns.Add("Limit", System.Type.GetType("System.String"))
        dtPeakSeason.Columns.Add("Premium", System.Type.GetType("System.String"))

        Try
            Dim drPeakSeason = PeakSeasonList.Select("PersPropRowNum='" & RowNumber & "'")
            If drPeakSeason.Length > 0 Then
                tblPeakseason.Attributes.Add("style", "display:block;")
                For Each peakSeason In drPeakSeason
                    Dim newRow As DataRow = dtPeakSeason.NewRow
                    newRow.Item("Description") = peakSeason("Description")
                    Dim effDate As DateTime = peakSeason("StartDate")
                    newRow.Item("StartDate") = effDate.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture)
                    Dim expDate As DateTime = peakSeason("EndDate")
                    newRow.Item("EndDate") = expDate.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture)
                    newRow.Item("Limit") = peakSeason("Limit")
                    newRow.Item("Premium") = peakSeason("Premium")
                    dtPeakSeason.Rows.Add(newRow)
                Next
            Else
                tblPeakseason.Attributes.Add("style", "display:none;")
            End If

            dgPeakSeason.DataSource = dtPeakSeason
            dgPeakSeason.DataBind()
        Catch ex As Exception

        End Try
    End Sub

    Public Function Save() As Boolean Implements IVRUI_P.Save
        Return False
    End Function

    Public Sub ValidateForm() Implements IVRUI_P.ValidateForm

    End Sub
End Class