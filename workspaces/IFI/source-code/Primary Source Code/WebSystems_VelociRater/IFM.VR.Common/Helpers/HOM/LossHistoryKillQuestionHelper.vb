Imports QuickQuote.CommonObjects

Namespace IFM.VR.Common.Helpers.HOM
    Public Class LossHistoryKillQuestionHelper
        Private Shared _LossHistoryKillQuestionSettings As NewFlagItem
        Public Shared ReadOnly Property LossHistoryKillQuestionSettings() As NewFlagItem
            Get
                If _LossHistoryKillQuestionSettings Is Nothing Then
                    _LossHistoryKillQuestionSettings = New NewFlagItem("VR_HOM_LossHistoryKillQuestion_Settings")
                End If
                Return _LossHistoryKillQuestionSettings
            End Get
        End Property

        Public Shared Function LossHistoryKillQuestionEnabled() As Boolean
            Return LossHistoryKillQuestionSettings.EnabledFlag
        End Function

        Public Shared Function LossHistoryKillQuestionEffDate() As Date
            Return LossHistoryKillQuestionSettings.GetStartDateOrDefault("10/01/2024")
        End Function

        Public Shared Sub UpdateLossHistoryKillQuestion(ByRef Quote As QuickQuoteObject, ByRef CrossDirection As Helper.EnumsHelper.CrossDirectionEnum, Optional ByRef ValidationErrors As List(Of WebValidationItem) = Nothing)
            Select Case CrossDirection
                Case Helper.EnumsHelper.CrossDirectionEnum.FORWARD

                Case Helper.EnumsHelper.CrossDirectionEnum.BACK

            End Select
        End Sub

        Public Shared Function IsLossHistoryKillQuestionAvailable(ByRef Quote As QuickQuoteObject)
            If Quote IsNot Nothing Then
                Return AvailabilityByDateOrVersion.AvailabilityByDateOrVersion(Quote, LossHistoryKillQuestionSettings, AvailabilityByDateOrVersion.VersionTypeToTest.SubquoteFirstVersionId)
            End If
            Return False
        End Function
    End Class
End Namespace
