Imports QuickQuote.CommonMethods
Imports QuickQuote.CommonObjects
Imports IFM.PrimativeExtensions

Namespace IFM.VR.Common.Helpers.PPA
    Public Class PolicyTrainingFeeHelper
        Private Shared _PoliceTrainingFeeSettings As NewFlagItem
        Public Shared ReadOnly Property PoliceTrainingFeeSettings() As NewFlagItem
            Get
                If _PoliceTrainingFeeSettings Is Nothing Then
                    _PoliceTrainingFeeSettings = New NewFlagItem("VR_PPA_PoliceTrainingFee_Settings")
                End If
                Return _PoliceTrainingFeeSettings
            End Get
        End Property

        Public Shared Sub UpdatePoliceTrainingFee(ByRef Quote As QuickQuoteObject, ByRef CrossDirection As Helper.EnumsHelper.CrossDirectionEnum, Optional ByRef ValidationErrors As List(Of WebValidationItem) = Nothing)
            Select Case CrossDirection
                Case Helper.EnumsHelper.CrossDirectionEnum.FORWARD
                    'No Change       
                Case Helper.EnumsHelper.CrossDirectionEnum.BACK
                    'No Change
            End Select
        End Sub

        Public Shared Function IsPoliceTrainingFeeAvailable(quote As QuickQuoteObject) As Boolean
            If quote IsNot Nothing Then
                Dim IsCorrectState As Boolean = quote.QuickQuoteState = QuickQuoteHelperClass.QuickQuoteState.Illinois
                PoliceTrainingFeeSettings.OtherQualifiers = IsCorrectState
                Return AvailabilityByDateOrVersion.AvailabilityByDateOrVersion(quote, PoliceTrainingFeeSettings, AvailabilityByDateOrVersion.VersionTypeToTest.SubquoteFirstVersionId)
            End If
            Return False
        End Function
    End Class
End Namespace
