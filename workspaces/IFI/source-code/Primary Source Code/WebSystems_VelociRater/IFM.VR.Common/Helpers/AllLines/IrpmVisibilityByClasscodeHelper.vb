Imports QuickQuote.CommonObjects
Imports QuickQuote.CommonMethods
Imports IFM.PrimativeExtensions

Namespace IFM.VR.Common.Helpers
    Public Class IrpmVisibilityByClasscodeHelper

        Private Shared _IrpmVisibilitySettings As NewFlagItem
        Public Shared ReadOnly Property IrpmVisibilitySettings() As NewFlagItem
            Get
                If _IrpmVisibilitySettings Is Nothing Then
                    _IrpmVisibilitySettings = New NewFlagItem("VR_AllLines_IrpmVisibilityByClasscode_Settings")
                End If
                Return _IrpmVisibilitySettings
            End Get
        End Property

        Const IrpmVisibilityWarningMsg As String = ""
        Const IrpmVisibilityRemovedMsg As String = ""

        Public Shared Sub UpdateIrpmVisibility(ByRef Quote As QuickQuoteObject, ByRef CrossDirection As Helper.EnumsHelper.CrossDirectionEnum, Optional ByRef ValidationErrors As List(Of WebValidationItem) = Nothing)
            Dim NeedsWarningMessage As Boolean = False
            Select Case CrossDirection
                Case Helper.EnumsHelper.CrossDirectionEnum.FORWARD
                    'No Messages
                Case Helper.EnumsHelper.CrossDirectionEnum.BACK
                    'No Messages
            End Select
        End Sub

        Public Shared Function IsIrpmVisibilityAvailable(quote As QuickQuoteObject) As Boolean

            If quote IsNot Nothing Then

                Dim IsCorrectLOB As Boolean = quote.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialProperty _
                    OrElse quote.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialPackage _
                    OrElse quote.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialGeneralLiability _
                    OrElse quote.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialBOP

                IrpmVisibilitySettings.OtherQualifiers = IsCorrectLOB

                Return AvailabilityByDateOrVersion.AvailabilityByDateOrVersion(quote, IrpmVisibilitySettings, AvailabilityByDateOrVersion.VersionTypeToTest.SubquoteFirstVersionId)
            End If
            Return False

        End Function

    End Class
End Namespace