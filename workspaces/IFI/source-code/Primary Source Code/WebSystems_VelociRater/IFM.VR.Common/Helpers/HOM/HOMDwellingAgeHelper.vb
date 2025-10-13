Imports QuickQuote.CommonObjects
Imports IFM.PrimativeExtensions
Imports System.Configuration

Namespace IFM.VR.Common.Helpers.HOM
    Public Class HOMDwellingAgeHelper

        Private Shared _HomeDwellingTextSettings As NewFlagItem
        Public Shared ReadOnly Property HomeDwellingTextSettings() As NewFlagItem
            Get
                If _HomeDwellingTextSettings Is Nothing Then
                    _HomeDwellingTextSettings = New NewFlagItem("VR_Home_DwellingTextSettings")
                End If
                Return _HomeDwellingTextSettings
            End Get
        End Property

        Public Shared Function HomeDwellingTextEnabled() As Boolean
            Return HomeDwellingTextSettings.EnabledFlag
        End Function

        Public Shared Function HomeDwellingTextEffDate() As Date
            Return HomeDwellingTextSettings.GetStartDateOrDefault("1/1/1800")
        End Function

        Public Shared Function IsHomeDwellingTextAvailable(quote As QuickQuoteObject) As Boolean
            If quote IsNot Nothing Then
                Return AvailabilityByDateOrVersion.AvailabilityByDateOrVersion(quote, HomeDwellingTextSettings, AvailabilityByDateOrVersion.VersionTypeToTest.SubquoteFirstVersionId)
            End If
            Return False
        End Function
    End Class

End Namespace
