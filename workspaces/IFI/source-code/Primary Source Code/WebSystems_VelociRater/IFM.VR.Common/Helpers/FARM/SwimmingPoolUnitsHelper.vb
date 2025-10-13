Imports QuickQuote.CommonObjects
Imports QuickQuote.CommonMethods
Imports IFM.PrimativeExtensions

Namespace IFM.VR.Common.Helpers.FARM
    Public Class SwimmingPoolUnitsHelper
        Private Shared _SwimmingPoolSettings As NewFlagItem
        Public Shared ReadOnly Property SwimmingPoolSettings() As NewFlagItem
            Get
                If _SwimmingPoolSettings Is Nothing Then
                    _SwimmingPoolSettings = New NewFlagItem("VR_Far_SwimmingPoolUnits_Settings")
                End If
                Return _SwimmingPoolSettings
            End Get
        End Property

        Const SwimWarningAddedMsg As String = "If Swimming Pools are on property, please update on all locations necessary. See the quote locations page if any are present."
        Const SwimWarningRemovedMsg As String = "Swimming Pools surcharge is unavailable before 09/01/22. The coverage has been removed."
        Public Shared Sub UpdateSwimmingPoolUnits(ByRef Quote As QuickQuoteObject, ByRef CrossDirection As Helper.EnumsHelper.CrossDirectionEnum, Optional ByRef ValidationErrors As List(Of WebValidationItem) = Nothing)
            Dim qqh As New QuickQuoteHelperClass
            Dim NeedsWarningMessage As Boolean = False
            Select Case CrossDirection
                Case Helper.EnumsHelper.CrossDirectionEnum.FORWARD
                    'Added Coverage Possibility
                    'Always show for pools
                    If ValidationErrors IsNot Nothing Then
                        Dim i = New WebValidationItem(SwimWarningAddedMsg)
                        i.IsWarning = True
                        ValidationErrors.Add(i)
                    End If
                Case Helper.EnumsHelper.CrossDirectionEnum.BACK
                    'Remove Coverage Possibility
                    For Each l In Quote.Locations
                        If l.SwimmingPoolHotTubSurcharge = True Then NeedsWarningMessage = True
                        l.SwimmingPoolHotTubSurcharge_NumberOfUnits = 0
                        l.SwimmingPoolHotTubSurcharge = False
                    Next
                    If NeedsWarningMessage AndAlso ValidationErrors IsNot Nothing Then
                        Dim i = New WebValidationItem(SwimWarningRemovedMsg)
                        i.IsWarning = True
                        ValidationErrors.Add(i)
                    End If
            End Select
        End Sub

        Public Shared Function isSwimmingPoolUnitsAvailable(quote As QuickQuoteObject) As Boolean
            If quote IsNot Nothing Then
                Dim qqh As New QuickQuoteHelperClass
                Dim SubQuoteFirst = qqh.MultiStateQuickQuoteObjects(quote).GetItemAtIndex(0)

                Dim IsCorrectLOB As Boolean = quote.LobType = QuickQuoteObject.QuickQuoteLobType.Farm
                Dim IsCorrectProgramType As Boolean = SubQuoteFirst?.ProgramTypeId = "6" 'FO only

                SwimmingPoolSettings.OtherQualifiers = IsCorrectProgramType AndAlso IsCorrectLOB
                Return AvailabilityByDateOrVersion.AvailabilityByDateOrVersion(quote, SwimmingPoolSettings, AvailabilityByDateOrVersion.VersionTypeToTest.SubquoteFirstVersionId)
            End If
            Return False

        End Function
    End Class

End Namespace
