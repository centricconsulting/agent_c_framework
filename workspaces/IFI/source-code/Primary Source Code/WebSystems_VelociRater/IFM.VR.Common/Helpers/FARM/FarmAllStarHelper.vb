Imports QuickQuote.CommonMethods
Imports QuickQuote.CommonObjects
Imports IFM.PrimativeExtensions


Namespace IFM.VR.Common.Helpers.FARM
    Public Class FarmAllStarHelper
        Private Shared _FarmAllStarSettings As NewFlagItem
        Public Shared ReadOnly Property FarmAllStarSettings() As NewFlagItem
            Get
                If _FarmAllStarSettings Is Nothing Then
                    _FarmAllStarSettings = New NewFlagItem("VR_Far_FarmAllStar_Settings")
                End If
                Return _FarmAllStarSettings
            End Get
        End Property

        Const FarmAllStarForwardMsg As String = "Updated Farm All Star Endorsement is available effective 02/01/2024. Coverage will be amended to the Updated Farm All Star Endorsement for quotes or endorsements amended to an effective date of 02/01/2024 or greater."
        Const FarmAllStarBackMsg As String = "Updated Farm All Star Endorsement is not available prior to 02/01/2024. Coverage will be amended to the existing Farm All Star Endorsement for quotes or endorsements amended to an effective date prior to 02/01/2024."
        Public Shared Function FarmAllStarEnabled() As Boolean
            Return FarmAllStarSettings.EnabledFlag
        End Function

        Public Shared Function FarmAllStarEffDate() As Date
            Return FarmAllStarSettings.GetStartDateOrDefault("2/1/2024")
        End Function

        Public Shared Sub UpdateFarmAllStar(ByRef Quote As QuickQuoteObject, ByRef CrossDirection As Helper.EnumsHelper.CrossDirectionEnum, Optional ByRef ValidationErrors As List(Of WebValidationItem) = Nothing)
            Dim NeedsWarningMessage As Boolean = False
            Dim qqh As New QuickQuoteHelperClass
            Dim SubQuoteFirst = qqh.MultiStateQuickQuoteObjects(Quote).GetItemAtIndex(0)
            Select Case CrossDirection
                Case Helper.EnumsHelper.CrossDirectionEnum.FORWARD
                    If qqh.IsPositiveIntegerString(SubQuoteFirst.FarmAllStarLimitId) Then
                        NeedsWarningMessage = True
                        SubQuoteFirst.HasFarmAllStar = True
                        SubQuoteFirst.FarmAllStarWaterBackupLimitId = SubQuoteFirst.FarmAllStarLimitId
                        SubQuoteFirst.FarmAllStarLimitId = ""
                        SubQuoteFirst.FarmAllStarWaterDamageLimitId = "15" '5,000 [inc]
                    End If
                    If NeedsWarningMessage AndAlso ValidationErrors IsNot Nothing Then
                        Dim i = New WebValidationItem(FarmAllStarForwardMsg)
                        i.IsWarning = True
                        ValidationErrors.Add(i)
                    End If
                Case Helper.EnumsHelper.CrossDirectionEnum.BACK
                    If SubQuoteFirst.HasFarmAllStar Then
                        NeedsWarningMessage = True
                        SubQuoteFirst.HasFarmAllStar = False
                        SubQuoteFirst.FarmAllStarLimitId = SubQuoteFirst.FarmAllStarWaterBackupLimitId
                        SubQuoteFirst.FarmAllStarWaterBackupLimitId = ""
                        SubQuoteFirst.FarmAllStarWaterDamageLimitId = ""
                    End If
                    If NeedsWarningMessage AndAlso ValidationErrors IsNot Nothing Then
                        Dim i = New WebValidationItem(FarmAllStarBackMsg)
                        i.IsWarning = True
                        ValidationErrors.Add(i)
                    End If
            End Select
        End Sub

        Public Shared Function IsFarmAllStarAvailable(quote As QuickQuoteObject) As Boolean
            If quote IsNot Nothing Then
                Return AvailabilityByDateOrVersion.AvailabilityByDateOrVersion(quote, FarmAllStarSettings, AvailabilityByDateOrVersion.VersionTypeToTest.SubquoteFirstVersionId)
            End If
            Return False
        End Function
    End Class
End Namespace
