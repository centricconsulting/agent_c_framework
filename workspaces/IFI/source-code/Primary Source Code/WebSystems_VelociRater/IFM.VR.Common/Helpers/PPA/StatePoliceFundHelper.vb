Imports QuickQuote.CommonMethods
Imports QuickQuote.CommonObjects
Imports IFM.PrimativeExtensions

Namespace IFM.VR.Common.Helpers.PPA
    Public Class StatePoliceFundHelper
        Private Shared _StatePoliceFundSettings As NewFlagItem
        Public Shared ReadOnly Property StatePoliceFundSettings() As NewFlagItem
            Get
                If _StatePoliceFundSettings Is Nothing Then
                    _StatePoliceFundSettings = New NewFlagItem("VR_PPA_StatePoliceFund_Settings")
                End If
                Return _StatePoliceFundSettings
            End Get
        End Property

        Public Shared Sub UpdateStatePoliceFundSettings(ByRef Quote As QuickQuoteObject, ByRef CrossDirection As Helper.EnumsHelper.CrossDirectionEnum, Optional ByRef ValidationErrors As List(Of WebValidationItem) = Nothing)
            Select Case CrossDirection
                Case Helper.EnumsHelper.CrossDirectionEnum.FORWARD
                    'No Change       
                Case Helper.EnumsHelper.CrossDirectionEnum.BACK
                    'No Change
            End Select
        End Sub

        Public Shared Function IsStatePoliceFundLabelAvailable(quote As QuickQuoteObject) As Boolean
            If quote IsNot Nothing Then
                Dim IsCorrectState As Boolean = quote.QuickQuoteState = QuickQuoteHelperClass.QuickQuoteState.Illinois
                StatePoliceFundSettings.OtherQualifiers = IsCorrectState
                Return AvailabilityByDateOrVersion.AvailabilityByDateOrVersion(quote, StatePoliceFundSettings, AvailabilityByDateOrVersion.VersionTypeToTest.SubquoteFirstVersionId)
            End If
            Return False
        End Function
    End Class
End Namespace
