Imports QuickQuote.CommonObjects

Namespace IFM.VR.Common.Helpers.CPP
    Public Class CyberCoverageHelper
        Private Shared _CyberCoverageSettings As NewFlagItem
        Public Shared ReadOnly Property CyberCoverageSettings() As NewFlagItem
            Get
                If _CyberCoverageSettings Is Nothing Then
                    _CyberCoverageSettings = New NewFlagItem("VR_CPP_CGL_CyberCov_Settings")
                End If
                Return _CyberCoverageSettings
            End Get
        End Property

        Public Shared Function CyberCoverageEnabled() As Boolean
            Return CyberCoverageSettings.EnabledFlag
        End Function

        Public Shared Function CyberCoverageEffDate() As Date
            Return CyberCoverageSettings.GetStartDateOrDefault("6/1/2024")
        End Function

        Public Shared Sub UpdateCyberCoverage(ByRef Quote As QuickQuoteObject, ByRef CrossDirection As Helper.EnumsHelper.CrossDirectionEnum, Optional ByRef ValidationErrors As List(Of WebValidationItem) = Nothing)
            Select Case CrossDirection
                Case Helper.EnumsHelper.CrossDirectionEnum.FORWARD
                    'Label is Cyber Coverage       
                Case Helper.EnumsHelper.CrossDirectionEnum.BACK
                    'Label is Cyber Liability
            End Select
        End Sub

        Public Shared Function IsCyberCoverageAvailable(quote As QuickQuoteObject) As Boolean
            If quote IsNot Nothing Then
                Dim IsCorrectLOB As Boolean = quote.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialPackage
                CyberCoverageSettings.OtherQualifiers = IsCorrectLOB
                Return AvailabilityByDateOrVersion.AvailabilityByDateOrVersion(quote, CyberCoverageSettings, AvailabilityByDateOrVersion.VersionTypeToTest.SubquoteFirstVersionId)
            End If
            Return False
        End Function
    End Class
End Namespace
