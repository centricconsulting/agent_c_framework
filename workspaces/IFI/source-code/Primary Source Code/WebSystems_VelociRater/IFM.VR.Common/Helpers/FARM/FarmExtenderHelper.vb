Imports QuickQuote.CommonMethods
Imports QuickQuote.CommonObjects
Imports IFM.PrimativeExtensions

Namespace IFM.VR.Common.Helpers.FARM
    Public Class FarmExtenderHelper
        Private Shared _FarmExtenderSettings As NewFlagItem
        Public Shared ReadOnly Property FarmExtenderSettings() As NewFlagItem
            Get
                If _FarmExtenderSettings Is Nothing Then
                    _FarmExtenderSettings = New NewFlagItem("VR_Far_FarmExtender_Settings")
                End If
                Return _FarmExtenderSettings
            End Get
        End Property

        Const FarmExtenderNotAvailableMsg As String = "Updated Farm Extender Endorsement is not available prior to 11/01/2023. Coverage will be amended to the existing Farm Extender Endorsement for quotes or endorsements amended to an effective date prior to 11/01/2023."
        Const FarmExtenderAvailableMsg As String = "Updated Farm Extender Endorsement is available effective 11/01/2023. Coverage will be amended to the Updated Farm Extender Endorsement for quotes or endorsements amended to an effective date of 11/01/2023 or greater."

        Public Shared Function FarmExtenderEnabled() As Boolean
            Return FarmExtenderSettings.EnabledFlag
        End Function

        Public Shared Function FarmExtenderEffDate() As Date
            Return FarmExtenderSettings.GetStartDateOrDefault("11/1/2023")
        End Function

        Public Shared Sub UpdateFarmExtender(ByRef Quote As QuickQuoteObject, ByRef CrossDirection As Helper.EnumsHelper.CrossDirectionEnum, Optional ByRef ValidationErrors As List(Of WebValidationItem) = Nothing)
            Dim FarmExtenderMsg As String = ""
            Dim NeedsWarningMessage As Boolean = False
            Dim qqhelper = New QuickQuoteHelperClass()
            Dim SubQuoteFirst = qqhelper.MultiStateQuickQuoteObjects(Quote).GetItemAtIndex(0)
            If SubQuoteFirst IsNot Nothing AndAlso SubQuoteFirst.HasFarmExtender Then
                Dim GoverningStateQuote As QuickQuote.CommonObjects.QuickQuoteObject = qqhelper.GoverningStateQuote(quote)
                Select Case CrossDirection
                    Case Helper.EnumsHelper.CrossDirectionEnum.FORWARD
                        FarmExtenderMsg = FarmExtenderAvailableMsg
                        NeedsWarningMessage =  True
                        'Add Extra Expense coverage with 5k included, no increase, and 5k total. If already on quote, reset included limit to 5k, increased limit to 0, total limit 5k
                        If GoverningStateQuote IsNot Nothing Then
                            If GoverningStateQuote.OptionalCoverages Is Nothing Then
                                GoverningStateQuote.OptionalCoverages = New List(Of QuickQuoteOptionalCoverage)
                            End If
                            If GoverningStateQuote.OptionalCoverages.FindAll(Function(p) p.CoverageType = QuickQuoteOptionalCoverage.QuickQuoteOptionalCoverageType.Farm_F_G_Extra_Expense).Count <= 0 Then
                                Dim optionalCoverage As QuickQuoteOptionalCoverage = New QuickQuoteOptionalCoverage()
                                optionalCoverage.CoverageType = QuickQuoteOptionalCoverage.QuickQuoteOptionalCoverageType.Farm_F_G_Extra_Expense
                                optionalCoverage.IncludedLimit = "5,000"
                                optionalCoverage.IncreasedLimit = "0"
                                If optionalCoverage.Coverage IsNot Nothing then
                                    optionalCoverage.Coverage.ManualLimitAmount = "5,000"
                                End If
                                GoverningStateQuote.OptionalCoverages.Add(optionalCoverage)
                            Else
                                Dim updateExtraExpense As QuickQuoteOptionalCoverage = GoverningStateQuote.OptionalCoverages.Find(Function(p) p.CoverageType = QuickQuoteOptionalCoverage.QuickQuoteOptionalCoverageType.Farm_F_G_Extra_Expense)
                                updateExtraExpense.IncludedLimit = "5,000"
                                updateExtraExpense.IncreasedLimit = "0"
                                If updateExtraExpense.Coverage IsNot Nothing then
                                    updateExtraExpense.Coverage.ManualLimitAmount = "5,000"
                                End If
                            End If
                        End If
                    Case Helper.EnumsHelper.CrossDirectionEnum.BACK
                        FarmExtenderMsg = FarmExtenderNotAvailableMsg
                        NeedsWarningMessage = True
                        'remove Extra Expense coverage
                        If GoverningStateQuote IsNot Nothing Then
                            Dim extraExpense As QuickQuoteOptionalCoverage = GoverningStateQuote.OptionalCoverages.Find(Function(p) p.CoverageType = QuickQuoteOptionalCoverage.QuickQuoteOptionalCoverageType.Farm_F_G_Extra_Expense)
                            If extraExpense IsNot Nothing Then
                                GoverningStateQuote.OptionalCoverages.Remove(extraExpense)
                            End If
                        End If
                End Select
                If NeedsWarningMessage = True Then
                    Dim i = New WebValidationItem(FarmExtenderMsg)
                    i.IsWarning = True
                    ValidationErrors.Add(i)
                End If
            End If
        End Sub

        Public Shared Function IsFarmExtenderAvailable(quote As QuickQuoteObject) As Boolean
            If quote IsNot Nothing Then
                Return AvailabilityByDateOrVersion.AvailabilityByDateOrVersion(quote, FarmExtenderSettings, AvailabilityByDateOrVersion.VersionTypeToTest.SubquoteFirstVersionId)
            End If
            Return False
        End Function
    End Class
End Namespace
