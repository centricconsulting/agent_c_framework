Imports QuickQuote.CommonObjects
Namespace IFM.VR.Common.Helpers.FARM
    Public Class FarmCustomFeeding
        Private Shared _FARCustomFeedingSettings As NewFlagItem
        Public Shared ReadOnly Property FARCustomFeedingSettings() As NewFlagItem
            Get
                If _FARCustomFeedingSettings Is Nothing Then
                    _FARCustomFeedingSettings = New NewFlagItem("VR_FAR_CustomFeeding_Settings")
                End If
                Return _FARCustomFeedingSettings
            End Get
        End Property

        Public Shared Function FARCustomFeedingEnabled() As Boolean
            Return FARCustomFeedingSettings.EnabledFlag
        End Function

        Public Shared Function FARCustomFeedingEffDate() As Date
            Return FARCustomFeedingSettings.GetStartDateOrDefault("1/1/1800")
        End Function

        Public Shared Sub UpdateFARCustomFeeding(ByRef Quote As QuickQuoteObject, ByRef CrossDirection As Helper.EnumsHelper.CrossDirectionEnum, Optional ByRef ValidationErrors As List(Of WebValidationItem) = Nothing)
            Select Case CrossDirection
                Case Helper.EnumsHelper.CrossDirectionEnum.FORWARD
                    'Show on Policy Level Coverages page                   
                Case Helper.EnumsHelper.CrossDirectionEnum.BACK
                    'Show on Location page under building
            End Select
        End Sub

        Public Shared Function IsFARCustomFeedingAvailable(quote As QuickQuoteObject) As Boolean
            If quote IsNot Nothing Then
                Return AvailabilityByDateOrVersion.AvailabilityByDateOrVersion(quote, FARCustomFeedingSettings, AvailabilityByDateOrVersion.VersionTypeToTest.SubquoteFirstVersionId)
            End If
            Return False
        End Function
    End Class
End Namespace
