Imports QuickQuote.CommonObjects
Namespace IFM.VR.Common.Helpers.FARM
    Public Class FarmBlanketAcreageHelper
        Private Shared _FarmBlanketAcreageSettings As NewFlagItem
        Public Shared ReadOnly Property FarmBlanketAcreageSettings() As NewFlagItem
            Get
                If _FarmBlanketAcreageSettings Is Nothing Then
                    _FarmBlanketAcreageSettings = New NewFlagItem("VR_FAR_BlanketAcreage_Settings")
                End If
                Return _FarmBlanketAcreageSettings
            End Get
        End Property

        Public Shared Function FarmBlanketAcreageEnabled() As Boolean
            Return FarmBlanketAcreageSettings.EnabledFlag
        End Function

        Public Shared Function FarmBlanketAcreageEffDate() As Date
            Return FarmBlanketAcreageSettings.GetStartDateOrDefault("09/02/2025")
        End Function

        Public Shared Sub UpdateFarmBlanketAcreage(ByRef Quote As QuickQuoteObject, ByRef CrossDirection As Helper.EnumsHelper.CrossDirectionEnum, Optional ByRef ValidationErrors As List(Of WebValidationItem) = Nothing)
            Select Case CrossDirection
                Case Helper.EnumsHelper.CrossDirectionEnum.FORWARD
                    'Show coverage                  
                Case Helper.EnumsHelper.CrossDirectionEnum.BACK
                    'Hide coverage
            End Select
        End Sub

        Public Shared Function IsFarmBlanketAcreageAvailable(quote As QuickQuoteObject) As Boolean
            If quote IsNot Nothing Then
                Return AvailabilityByDateOrVersion.AvailabilityByDateOrVersion(quote, FarmBlanketAcreageSettings, AvailabilityByDateOrVersion.VersionTypeToTest.SubquoteFirstVersionId)
            End If
            Return False
        End Function
    End Class
End Namespace