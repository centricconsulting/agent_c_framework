Imports QuickQuote.CommonObjects
Imports QuickQuote.CommonMethods
Imports IFM.PrimativeExtensions

Namespace IFM.VR.Common.Helpers.FARM
    Public Class UndergroundServiceLineHelper
        Const UndergroundServiceLineWarningMsg As String = "Underground Service Line coverage is unavailable before 09/01/22. The coverage has been removed."
        Public Shared Sub UpdateUndergroundServiceLine(ByRef Quote As QuickQuoteObject, ByRef CrossDirection As Helper.EnumsHelper.CrossDirectionEnum, Optional ByRef ValidationErrors As List(Of WebValidationItem) = Nothing)
            Dim MyLocation As QuickQuoteLocation = Quote.Locations(0)
            Dim NeedsWarningMessage As Boolean = False
            Select Case CrossDirection
                Case Helper.EnumsHelper.CrossDirectionEnum.FORWARD
                    'Added Coverage Possibility
                Case Helper.EnumsHelper.CrossDirectionEnum.BACK
                    'Remove Coverage Possibility
                    For Each l In Quote.Locations
                        If l.SectionICoverages IsNot Nothing AndAlso l.SectionICoverages.FindAll(Function(sc) sc.CoverageType = QuickQuoteSectionICoverage.SectionICoverageType.UndergroundServiceLine).IsLoaded Then
                            l.SectionICoverages.RemoveAll(Function(sc) sc.CoverageType = QuickQuoteSectionICoverage.SectionICoverageType.UndergroundServiceLine)
                            NeedsWarningMessage = True
                        End If
                    Next
                    If NeedsWarningMessage AndAlso ValidationErrors IsNot Nothing Then
                        Dim i = New WebValidationItem(UndergroundServiceLineWarningMsg)
                        i.IsWarning = True
                        ValidationErrors.Add(i)
                    End If
            End Select
        End Sub

        Public Shared Function isUndergroundServiceLineAvailable(quote As QuickQuoteObject) As Boolean

            If quote IsNot Nothing Then
                Dim qqh As New QuickQuoteHelperClass
                Dim SubQuoteFirst = qqh.MultiStateQuickQuoteObjects(quote).GetItemAtIndex(0)

                Dim IsCorrectLOB As Boolean = quote.LobType = QuickQuoteObject.QuickQuoteLobType.Farm
                Dim IsCorrectProgramType As Boolean = SubQuoteFirst?.ProgramTypeId = "6" 'FO only

                Dim UndergroundServiceLineSettings As NewFlagItem = New NewFlagItem("VR_Far_UndergroundServiceLine_Settings")
                UndergroundServiceLineSettings.OtherQualifiers = IsCorrectProgramType AndAlso IsCorrectLOB
                Return AvailabilityByDateOrVersion.AvailabilityByDateOrVersion(quote, UndergroundServiceLineSettings, AvailabilityByDateOrVersion.VersionTypeToTest.SubquoteFirstVersionId)
            End If
            Return False

        End Function

        Private Function areUndergroundServiceLineRestrictionsMet(quote As QuickQuoteObject, locationIndex As Integer) As Boolean
            If isUndergroundServiceLineAvailable(quote) Then
                If quote IsNot Nothing AndAlso quote.Locations IsNot Nothing AndAlso quote.Locations.Any() Then
                    Dim isFormTypeGood = quote.Locations(locationIndex).FormTypeId <> "17" 'No HO-4
                    Dim isStructureGood = quote.Locations(locationIndex).StructureTypeId <> "2" 'No Mobile Home
                    Return isFormTypeGood AndAlso isStructureGood
                End If
                Return False
            End If
        End Function
    End Class

End Namespace
