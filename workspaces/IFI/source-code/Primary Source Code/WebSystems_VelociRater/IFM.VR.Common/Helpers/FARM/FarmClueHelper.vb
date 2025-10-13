Imports QuickQuote.CommonObjects

Namespace IFM.VR.Common.Helpers.FARM
    Public Class FarmClueHelper
        Private Shared _FarClueSettings As NewFlagItem
        Public Shared ReadOnly Property FarClueSettings() As NewFlagItem
            Get
                If _FarClueSettings Is Nothing Then
                    _FarClueSettings = New NewFlagItem("VR_FAR_Clue_Settings")
                End If
                Return _FarClueSettings
            End Get
        End Property

        Public Shared Function FarClueEnabled() As Boolean
            Return FarClueSettings.EnabledFlag
        End Function

        Public Shared Function FarClueEffDate() As Date
            Return FarClueSettings.GetStartDateOrDefault("1/1/2024")
        End Function

        Public Shared Sub UpdateFarClue(ByRef Quote As QuickQuoteObject, ByRef CrossDirection As Helper.EnumsHelper.CrossDirectionEnum, Optional ByRef ValidationErrors As List(Of WebValidationItem) = Nothing)
            Select Case CrossDirection
                Case Helper.EnumsHelper.CrossDirectionEnum.FORWARD
                    'Show on Policy Level Coverages page                   
                Case Helper.EnumsHelper.CrossDirectionEnum.BACK
                    'Show on Location page under building
            End Select
        End Sub

        Public Shared Function IsFarClueAvailable(quote As QuickQuoteObject) As Boolean
            If quote IsNot Nothing Then
                Return AvailabilityByDateOrVersion.AvailabilityByDateOrVersion(quote, FarClueSettings, AvailabilityByDateOrVersion.VersionTypeToTest.SubquoteFirstVersionId)
            End If
            Return False
        End Function
    End Class
End Namespace

