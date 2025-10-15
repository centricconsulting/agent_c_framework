Imports QuickQuote.CommonObjects
Imports IFM.VR.Web.Helpers
Imports QuickQuote.CommonMethods
Imports IFM.PrimativeExtensions

Public Class ctlFarmIncidentalLimits
    Inherits System.Web.UI.UserControl
    Implements IVRUI_P

    Public Event FarmIncidentalExist(state As Boolean)

    Dim qqHelper As New QuickQuote.CommonMethods.QuickQuoteHelperClass

    Protected ReadOnly Property Quote As QuickQuote.CommonObjects.QuickQuoteObject Implements IVRUI_P.Quote
        Get
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

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            Populate()
        End If
    End Sub

    Public Sub LoadStaticData() Implements IVRUI_P.LoadStaticData

    End Sub

    Private Sub WriteCell(sb As StringBuilder, cellText As String)
        Try
            sb.AppendLine("<td>")
            sb.AppendLine(cellText)
            sb.AppendLine("</td>")

            Exit Sub
        Catch ex As Exception
            Exit Sub
        End Try
    End Sub

    Private Sub WriteCell(sb As StringBuilder, cellText As String, styleText As String)
        Try
            sb.AppendLine("<td style=""" + styleText + """>")
            sb.AppendLine(cellText)
            sb.AppendLine("</td>")

            Exit Sub
        Catch ex As Exception
            Exit Sub
        End Try
    End Sub

    Private Sub WriteCell(sb As StringBuilder, cellText As String, styleText As String, cssclass As String)
        Try
            sb.AppendLine("<td style=""" + styleText + """ class=""" + cssclass + """>")
            sb.AppendLine(cellText)
            sb.AppendLine("</td>")

            Exit Sub
        Catch ex As Exception
            Exit Sub
        End Try
    End Sub

    Public Sub Populate() Implements IVRUI_P.Populate
        Dim html As New StringBuilder()
        Dim tot As Decimal = 0
        Dim lim As String = ""
        Dim prem As String = ""
        Dim desc As String = ""
        Dim deduct As String = ""
        Dim GoverningStateQuote = qqHelper.GoverningStateQuote(LocalQuickQuote)
        Dim NumFormatWithCents As String = "$###,###,###.00"

        ToggleFarmIncidental(False)

        Try
            html.AppendLine("<table style=""width: 100%"" class=""table"">")

            ' Header Row is in markup

            Dim FilTable = {(New With {.Coverage = "", .Limit = "", .Deduct = "", .Premium = ""})}.Take(0).ToList()

            If Me.Quote IsNot Nothing Then

                If GoverningStateQuote IsNot Nothing Then
                    If GoverningStateQuote.FarmIncidentalLimits IsNot Nothing AndAlso GoverningStateQuote.FarmIncidentalLimits.Count > 0 Then
                        For Each oc As QuickQuoteFarmIncidentalLimit In GoverningStateQuote.FarmIncidentalLimits
                            Select Case oc.CoverageType
                                Case QuickQuoteFarmIncidentalLimit.QuickQuoteFarmIncidentalLimitType.Farm_Glass_Breakage_in_Cabs
                                    ' This should be generic enough to handle all of these, but just in case, we can
                                    ' break them out for further processing by CoverageType.
                                    If Common.Helpers.FARM.GlassBreakageForCabs.IsGlassBreakageForCabsAvailable(LocalQuickQuote) Then
                                        desc = GetCoverageName(oc.CoverageType)
                                        lim = oc.TotalLimit
                                        deduct = "&nbsp;"
                                        If oc.Premium.TryToGetDouble > 0 Then
                                            tot += oc.Premium.TryToGetDouble
                                            prem = Format(oc.Premium.TryToGetDouble, NumFormatWithCents)
                                        Else
                                            prem = "Included"
                                        End If

                                        FilTable.Add(New With {.Coverage = desc, .Limit = lim, .Deduct = deduct, .Premium = prem})
                                    End If
                                    ToggleFarmIncidental(True)
                                    Exit Select
                                Case QuickQuoteFarmIncidentalLimit.QuickQuoteFarmIncidentalLimitType.Farm_Fire_Department_Service_Charge

                                    Exit Select
                                Case Else
                                    Exit Select
                            End Select
                        Next
                    End If

                    'Build the data table rows
                    ' Coverage Row
                    For Each PPRow In FilTable
                        html.AppendLine("<tr>")
                        ' Description
                        WriteCell(html, PPRow.Coverage, "width: 50%;")

                        html.AppendLine("<td><table style=""width:  100%"" class=""table""><tr style=""vertical-align: bottom"">")

                        ' Limit 
                        WriteCell(html, PPRow.Limit, "width: 54%;text-align:right;")
                        ' Deductable
                        WriteCell(html, PPRow.Deduct, "width: 23%;text-align:right;")
                        ' Premium
                        WriteCell(html, PPRow.Premium, "width: 23%;text-align:right;")

                        html.AppendLine("</tr></table></td>")

                        html.AppendLine("</tr>")
                    Next
                End If
            End If

            html.AppendLine("</table>")

            If tot > 0 Then
                lblTotalPremData.Text = Format(tot, NumFormatWithCents)
            End If

            Me.tblFarmIncidentalLimits.Text = html.ToString()

            Exit Sub
        Catch ex As Exception
            ' Spacer
            Exit Sub
        End Try
    End Sub

    ''' <summary>
    ''' Sums up the premium across states for the passed optional coverage
    ''' </summary>
    ''' <param name="CovCodeId"></param>
    ''' <returns></returns>
    'Private Function SumOptionalCoveragePremium(ByRef TopQuote As QuickQuote.CommonObjects.QuickQuoteObject, ByVal CovCodeId As String) As String
    '    Dim tot As Decimal = 0

    '    For Each sq As QuickQuote.CommonObjects.QuickQuoteObject In IFM.VR.Common.Helpers.MultiState.SubQuotes(TopQuote)
    '        If sq.OptionalCoverages IsNot Nothing Then
    '            For Each oc As QuickQuote.CommonObjects.QuickQuoteOptionalCoverage In sq.OptionalCoverages
    '                If oc.CoverageCodeId = CovCodeId Then
    '                    If IsNumeric(oc.Premium) Then tot += CDec(oc.Premium)
    '                    Exit For
    '                End If
    '            Next
    '        End If
    '    Next

    '    Return tot.ToString
    'End Function

    ''' <summary>
    ''' Sums the premium for the passed Scheduled Personal Property coverage
    ''' Can either be MAIN or EQ
    ''' </summary>
    ''' <param name="MainOrEQ"></param>
    ''' <returns></returns>
    'Private Function SumScheduledPersonalPropertyPremium(ByRef TopQuote As QuickQuote.CommonObjects.QuickQuoteObject, ByVal MainOrEQ As String, ByVal CovDesc As String, Optional ByVal CovLimit As String = Nothing) As String
    '    Dim tot As Decimal = 0

    '    For Each sq As QuickQuote.CommonObjects.QuickQuoteObject In IFM.VR.Common.Helpers.MultiState.SubQuotes(TopQuote)
    '        If sq.ScheduledPersonalPropertyCoverages IsNot Nothing Then
    '            If MainOrEQ.ToUpper = "MAIN" Then
    '                For Each spp As QuickQuote.CommonObjects.QuickQuoteScheduledPersonalPropertyCoverage In sq.ScheduledPersonalPropertyCoverages
    '                    If CovLimit IsNot Nothing AndAlso CovLimit.Trim <> String.Empty Then
    '                        If spp.Description.ToUpper = CovDesc.ToUpper AndAlso spp.IncreasedLimit = CovLimit Then
    '                            If IsNumeric(spp.MainCoveragePremium) Then tot += spp.MainCoveragePremium
    '                        End If
    '                    Else
    '                        If spp.Description.ToUpper = CovDesc.ToUpper Then
    '                            If IsNumeric(spp.MainCoveragePremium) Then tot += spp.MainCoveragePremium
    '                        End If
    '                    End If
    '                Next
    '            ElseIf MainOrEQ.ToUpper = "EQ" Then
    '                For Each spp As QuickQuote.CommonObjects.QuickQuoteScheduledPersonalPropertyCoverage In sq.ScheduledPersonalPropertyCoverages
    '                    If spp.Description.ToUpper = CovDesc.ToUpper Then
    '                        If IsNumeric(spp.EarthquakePremium) Then tot += spp.EarthquakePremium
    '                    End If
    '                Next
    '            End If
    '        End If
    '    Next

    '    Return tot.ToString
    'End Function

    Private Sub ToggleFarmIncidental(state As Boolean)
        RaiseEvent FarmIncidentalExist(state)
    End Sub

    Public Function Save() As Boolean Implements IVRUI_P.Save
        Return False
    End Function

    Public Sub ValidateForm() Implements IVRUI_P.ValidateForm

    End Sub

    Private Function GetCoverageName(coverageType As QuickQuoteFarmIncidentalLimit.QuickQuoteFarmIncidentalLimitType) As String
        Dim coverageName As String = Nothing

        Select Case coverageType
            Case QuickQuoteFarmIncidentalLimit.QuickQuoteFarmIncidentalLimitType.Farm_Glass_Breakage_in_Cabs
                coverageName = "Farm Glass Breakage in Cabs"
            Case Else 'Updated 8/28/2019 for Bug 39727 MLW
                coverageName = String.Empty
        End Select

        Return coverageName
    End Function
End Class