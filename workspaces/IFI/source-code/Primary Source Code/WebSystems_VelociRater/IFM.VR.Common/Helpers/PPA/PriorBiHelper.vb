Imports QuickQuote.CommonMethods
Imports QuickQuote.CommonObjects
Imports IFM.PrimativeExtensions
Imports System.Linq

Namespace IFM.VR.Common.Helpers.PPA
    Public Class PriorBiHelper


        Friend Shared Function FindPriorBI(prefillVehicles As IEnumerable(Of Diamond.Common.Objects.ThirdParty.SAQImportedVehicle)) As String
            Dim priorBiCoverageCodeLimitId As Int32 = 0
            If prefillVehicles IsNot Nothing Then
                For Each v In prefillVehicles
                    If priorBiCoverageCodeLimitId = 0 Then ' only care about the first value you get that isn't null,empty, whitepsace
                        priorBiCoverageCodeLimitId = ParsePrioBiChoicePointValue(v.ReportedCoverageLimit).ToString()
                    End If
                Next
            End If
            Return priorBiCoverageCodeLimitId
        End Function

        Private Shared Function ParsePrioBiChoicePointValue(ReportedCoverageLimit As String) As Int32
            If (ReportedCoverageLimit.IsNullEmptyorWhitespace() = False) Then
                Dim splitLimit As String() = ReportedCoverageLimit.Split("/")
                If splitLimit.Length = 3 Then
                    Dim perPersonLimit = Text.RegularExpressions.Regex.Replace(splitLimit(0), "[^0-9.]", "").TryToGetInt32()
                    Dim perOccurrenceLimit = Text.RegularExpressions.Regex.Replace(splitLimit(1), "[^0-9.]", "").TryToGetInt32()
                    Return FindPriorBiCoverageCodeIdFromStaticData(perPersonLimit, perOccurrenceLimit)
                Else
                    ' probably combined single limit
                    Dim perOccurrenceLimit = Text.RegularExpressions.Regex.Replace(ReportedCoverageLimit, "[^0-9.]", "").TryToGetInt32()
                    Return FindPriorBiCoverageCodeIdFromStaticData(0, perOccurrenceLimit)
                End If
            End If
            Return 0
        End Function

        Private Shared Function FindPriorBiCoverageCodeIdFromStaticData(perPersonLimit As Int32, perOccurrenceLimit As Int32) As Int32
            Dim qqHelper As New QuickQuoteHelperClass
            Dim limits = qqHelper.GetStaticDataOptions(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuoteHelperClass.QuickQuotePropertyName.PriorBiCoverageCodeLimitId)

            If (perPersonLimit < 1) Then
                'CSL
                For Each item In limits
                    Dim txt = Text.RegularExpressions.Regex.Replace(item.Text, "[^0-9.]", "")
                    If (txt = perOccurrenceLimit.ToString()) Then
                        Return item.Value.TryToGetInt32()
                    End If
                Next
                'static data didn't have a CSL that matched so find first split limit with an perOccurrence that matches
                For Each item In limits
                    Dim txt = item.Text.Split("/")
                    If txt.Length > 1 Then
                        Dim txt_perPersonLimit = Text.RegularExpressions.Regex.Replace(txt(0), "[^0-9.]", "").TryToGetInt32()
                        Dim txt_perOccurrenceLimit = Text.RegularExpressions.Regex.Replace(txt(1), "[^0-9.]", "").TryToGetInt32()
                        If (txt_perPersonLimit = txt_perOccurrenceLimit = perOccurrenceLimit.ToString()) Then
                            Return item.Value.TryToGetInt32()
                        End If
                    End If
                Next
            Else
                'Split Limit
                For Each item In limits
                    Dim txt = item.Text.Split("/")
                    If txt.Length > 1 Then
                        Dim txt_perPersonLimit = Text.RegularExpressions.Regex.Replace(txt(0), "[^0-9.]", "").TryToGetInt32()
                        Dim txt_perOccurrenceLimit = Text.RegularExpressions.Regex.Replace(txt(1), "[^0-9.]", "").TryToGetInt32()
                        If (txt_perPersonLimit = perPersonLimit AndAlso txt_perOccurrenceLimit = perOccurrenceLimit.ToString()) Then
                            Return item.Value.TryToGetInt32()
                        End If
                    End If
                Next
            End If
            Return 0
        End Function




        Public Shared Function HasPrefillPriorBi(policyId As String) As Boolean
            Dim foundPriorBi = False
            Dim priorBiValue As String = String.Empty
            Using sq As New SQLselectObject
                Dim param As New SqlClient.SqlParameter("@policy_Id", policyId)
                Dim saqPrefillDS As System.Data.DataSet
                saqPrefillDS = sq.GetDataset(System.Configuration.ConfigurationManager.AppSettings("connDiamond"), "assp_ChoicePoint_LoadSAQAndPrefillData", param)
                If saqPrefillDS.Tables.Count > 1 AndAlso saqPrefillDS.Tables(2).Rows.Count > 0 Then
                    For Each row As DataRow In saqPrefillDS.Tables(2).Rows
                        'makes sure that there is a record and that it is parsable to a coveragecodelimitid
                        If String.IsNullOrWhiteSpace(row("reported_coverage_limit").ToString) = False AndAlso ParsePrioBiChoicePointValue(row("reported_coverage_limit").ToString()) > 0 Then
                            Dim pBi As String = row("reported_coverage_limit").ToString()
                            foundPriorBi = True
                            Exit For
                        End If
                    Next
                End If
            End Using

            Return foundPriorBi
        End Function
    End Class

End Namespace
