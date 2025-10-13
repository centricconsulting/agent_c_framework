Imports QuickQuote.CommonObjects
Imports IFM.VR.Web.Helpers
Imports QuickQuote.CommonMethods
Imports IFM.VR.Common.Helpers.FARM
'Imports System.Data.SqlClient
'Imports System.Configuration.ConfigurationManager

Public Class ctlOtherCoverages
    Inherits System.Web.UI.UserControl
    Implements IVRUI_P

    Private Const ClassName As String = "ctlQuoteSummary_Farm"
    Dim qqHelper As New QuickQuote.CommonMethods.QuickQuoteHelperClass
    Dim QuickQuote As QuickQuoteObject

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

    Public Property CoverageName() As String
        Get
            Return ViewState("vs_CoverageName")
        End Get
        Set(ByVal value As String)
            ViewState("vs_CoverageName") = value
        End Set
    End Property

    Public Property CoverageRowNum() As String
        Get
            Return ViewState("vs_CoverageRowNum")
        End Get
        Set(ByVal value As String)
            ViewState("vs_CoverageRowNum") = value
        End Set
    End Property

    Public Property CoverageCode() As DataTable
        Get
            Return Session("sess_CoverageCode")
        End Get
        Set(ByVal value As DataTable)
            Session("sess_CoverageCode") = value
        End Set
    End Property

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub

    Public Sub LoadStaticData() Implements IVRUI_P.LoadStaticData

    End Sub

    Public Sub Populate() Implements IVRUI_P.Populate
        If LocalQuickQuote.Locations(RowNumber).SectionICoverages(CoverageRowNum).CoverageCodeId <> "554" Then
            'Dim drCoverageCode = CoverageCode.Select("coveragecode_id='" & LocalQuickQuote.Locations(RowNumber).SectionICoverages(CoverageRowNum).CoverageCodeId & "'")
            'If drCoverageCode.Length > 0 Then
            If CosmeticDamageExHelper.IsCosmeticDamageExAvailable(Quote) Then
                lblOtherPropCov.Text = FarmSummaryHelper.GetCoverageCodeCaption(LocalQuickQuote.Locations(RowNumber).SectionICoverages(CoverageRowNum).CoverageCodeId, LocalQuickQuote, LocalQuickQuote.Locations(RowNumber))
            Else
                lblOtherPropCov.Text = FarmSummaryHelper.GetCoverageCodeCaption(LocalQuickQuote.Locations(RowNumber).SectionICoverages(CoverageRowNum).CoverageCodeId)

            End If
            'lblOtherPropCov.Text = drCoverageCode(drCoverageCode.Length - 1)("caption")
            lblOtherLimit.Text = LocalQuickQuote.Locations(RowNumber).SectionICoverages(CoverageRowNum).TotalLimit

            If Not qqHelper.IsZeroAmount(LocalQuickQuote.Locations(RowNumber).SectionICoverages(CoverageRowNum).TotalLimit) Then
                lblOtherLimit.Text = LocalQuickQuote.Locations(RowNumber).SectionICoverages(CoverageRowNum).TotalLimit
            Else
                lblOtherLimit.Text = "N/A"
            End If

            If Not qqHelper.IsZeroAmount(LocalQuickQuote.Locations(RowNumber).SectionICoverages(CoverageRowNum).Premium) Then
                lblOtherPrem.Text = LocalQuickQuote.Locations(RowNumber).SectionICoverages(CoverageRowNum).Premium
            Else
                lblOtherPrem.Text = "Included"
            End If
            'End If
        End If
    End Sub

    Public Function Save() As Boolean Implements IVRUI_P.Save
        Return False
    End Function

    Public Sub ValidateForm() Implements IVRUI_P.ValidateForm

    End Sub

    'Private Function GetCoverageCodeCaption(ByVal CoverageCode_Id As String) As String
    '    Dim conn As New SqlConnection()
    '    Dim cmd As New SqlCommand()
    '    Dim da As New SqlDataAdapter()
    '    Dim tbl As New DataTable()

    '    Try
    '        conn = New SqlConnection(AppSettings("connDiamond"))
    '        conn.Open()
    '        cmd.Connection = conn
    '        cmd.CommandType = CommandType.Text
    '        cmd.CommandText = "SELECT coveragecodeversion_id, caption FROM CoverageCodeVersion WHERE CoverageCode_Id = " & CoverageCode_Id & " ORDER BY CoverageCodeVersion_id DESC"
    '        da.SelectCommand = cmd
    '        da.Fill(tbl)
    '        If tbl Is Nothing OrElse tbl.Rows.Count <= 0 Then Return ""

    '        Return tbl(tbl.Rows.Count - 1)("caption").ToString()
    '    Catch ex As Exception
    '        HandleError(ClassName, "GetCoverageCodeCaption", ex, lblMsg)
    '        Return ""
    '    Finally
    '        If conn.State = ConnectionState.Open Then conn.Close()
    '        conn.Dispose()
    '        cmd.Dispose()
    '        tbl.Dispose()
    '        da.Dispose()
    '    End Try
    'End Function
End Class