Imports QuickQuote.CommonObjects

Namespace IFM.VR.Common.Helpers.HOM
    Public Class CovALimitIncreaseHelper
        Private Shared _CovALimitIncreaseHelper As NewFlagItem
        Public Shared ReadOnly Property _CovALimitIncreaseSettings() As NewFlagItem
            Get
                If _CovALimitIncreaseHelper Is Nothing Then
                    _CovALimitIncreaseHelper = New NewFlagItem("VR_HOM_CovALimitIncrease")
                End If
                Return _CovALimitIncreaseHelper
            End Get
        End Property

        Public Shared Function CovALimitIncreaseEnabled() As Boolean
            Return _CovALimitIncreaseSettings.EnabledFlag
        End Function

        Public Shared Function CovALimitIncreaseEffectiveDate() As Date
            Return _CovALimitIncreaseSettings.GetStartDateOrDefault("10/1/2024")
        End Function
        Public Shared Sub UpdateCovALimit(ByRef Quote As QuickQuoteObject, ByRef CrossDirection As Helper.EnumsHelper.CrossDirectionEnum, Optional ByRef ValidationErrors As List(Of WebValidationItem) = Nothing)
            Select Case CrossDirection
                Case Helper.EnumsHelper.CrossDirectionEnum.FORWARD
                    'No Change       
                Case Helper.EnumsHelper.CrossDirectionEnum.BACK
                    'No Change
            End Select
        End Sub

        Public Shared Function IsCovALimitIncreaseAvailable(quote As QuickQuoteObject) As Boolean
            If quote IsNot Nothing Then
                Return AvailabilityByDateOrVersion.AvailabilityByDateOrVersion(quote, _CovALimitIncreaseSettings, AvailabilityByDateOrVersion.VersionTypeToTest.SubquoteFirstVersionId)
            End If
            Return False
        End Function
    End Class
End Namespace
