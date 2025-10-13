Imports QuickQuote.CommonObjects
'Imports QuickQuote.CommonMethods
Namespace IFM.VR.Common.Helpers.FARM
    Public Class IndividualLegalEntityHelper
        Private Shared _IndividualLegalEntitySettings As NewFlagItem
        Public Shared ReadOnly Property IndividualLegalEntitySettings() As NewFlagItem
            Get
                If _IndividualLegalEntitySettings Is Nothing Then
                    _IndividualLegalEntitySettings = New NewFlagItem("VR_FAR_IndividualLegalEntity_Settings")
                End If
                Return _IndividualLegalEntitySettings
            End Get
        End Property

        Const IndividualLegalEntityWarningMsg As String = "N/A"
        Const IndividualLegalEntityRemovedMsg As String = "N/A"
        Public Shared Function IndividualLegalEntityEnabled() As Boolean
            Return IndividualLegalEntitySettings.EnabledFlag
        End Function

        Public Shared Function IndividualLegalEntityEffDate() As Date
            Return IndividualLegalEntitySettings.StartDate
        End Function

        Public Shared Sub UpdateIndividualLegalEntity(ByRef Quote As QuickQuoteObject, ByRef CrossDirection As Helper.EnumsHelper.CrossDirectionEnum, Optional ByRef ValidationErrors As List(Of WebValidationItem) = Nothing)
            Dim NeedsWarningMessage As Boolean = False
            Select Case CrossDirection
                Case Helper.EnumsHelper.CrossDirectionEnum.FORWARD
                    'Not Needed
                Case Helper.EnumsHelper.CrossDirectionEnum.BACK
                    'Not Needed
            End Select
        End Sub

        Public Shared Function IsIndividualLegalEntityAvailable(quote As QuickQuoteObject) As Boolean
            If quote IsNot Nothing Then
                Dim IsCorrectLOB As Boolean = quote.LobType = QuickQuoteObject.QuickQuoteLobType.Farm
                IndividualLegalEntitySettings.OtherQualifiers = IsCorrectLOB
                Return AvailabilityByDateOrVersion.AvailabilityByDateOrVersion(quote, IndividualLegalEntitySettings, AvailabilityByDateOrVersion.VersionTypeToTest.SubquoteFirstVersionId)  
            End If
            Return False

        End Function
    End Class
End Namespace
