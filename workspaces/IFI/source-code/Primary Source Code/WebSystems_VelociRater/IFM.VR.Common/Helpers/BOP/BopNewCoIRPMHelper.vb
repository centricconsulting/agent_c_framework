Imports QuickQuote.CommonObjects
Imports QuickQuote.CommonMethods
Imports IFM.PrimativeExtensions

Namespace IFM.VR.Common.Helpers
    Public Class BopNewCoIRPMHelper

        Private Shared _BopNewCoIRPMSettings As NewFlagItem
        Public Shared ReadOnly Property BopNewCoIRPMSettings() As NewFlagItem
            Get
                If _BopNewCoIRPMSettings Is Nothing Then
                    _BopNewCoIRPMSettings = New NewFlagItem("VR_NewCo_BOP_IRPM_Settings")
                End If
                Return _BopNewCoIRPMSettings
            End Get
        End Property

        Const BopNewCoIRPMWarningMsg As String = ""
        Const BopNewCoIRPMRemovedMsg As String = ""

        Public Shared Sub UpdateBopNewCoIRPM(ByRef Quote As QuickQuoteObject, ByRef CrossDirection As Helper.EnumsHelper.CrossDirectionEnum, Optional ByRef ValidationErrors As List(Of WebValidationItem) = Nothing)
            Dim NeedsWarningMessage As Boolean = False
            Dim qqHelper = New QuickQuoteHelperClass
            Select Case CrossDirection
                Case Helper.EnumsHelper.CrossDirectionEnum.FORWARD
                    'No Messages
                Case Helper.EnumsHelper.CrossDirectionEnum.BACK
                    'No Messages
            End Select
        End Sub

        Public Shared Function IsBopNewCoIRPMAvailable(quote As QuickQuoteObject) As Boolean

            If quote IsNot Nothing Then

                Dim IsCorrectLOB As Boolean = quote.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialBOP

                BopNewCoIRPMSettings.OtherQualifiers = IsCorrectLOB

                Return AvailabilityByDateOrVersion.AvailabilityByDateOrVersion(quote, BopNewCoIRPMSettings, AvailabilityByDateOrVersion.VersionTypeToTest.SubquoteFirstVersionId)
            End If
            Return False

        End Function

    End Class
End Namespace