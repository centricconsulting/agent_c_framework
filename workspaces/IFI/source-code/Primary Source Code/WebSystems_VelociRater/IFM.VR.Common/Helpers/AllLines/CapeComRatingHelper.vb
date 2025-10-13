Imports QuickQuote.CommonObjects
Imports IFM.PrimativeExtensions

Namespace IFM.VR.Common.Helpers.AllLines
    Public Class CapeComRatingHelper
        Inherits FeatureFlagBase
        Private Shared _CapeComRatingSettings As NewFlagItem

        Const CapeComLobsAllowedKey As String = "VR_AllLines_CapeCom_LobsAllowedKey"
        Public Shared ReadOnly Property CapeComRatingSettings() As NewFlagItem
            Get
                If _CapeComRatingSettings Is Nothing Then
                    _CapeComRatingSettings = New NewFlagItem("VR_AllLines_CapeCom_PreLoadIntegrationCall_Settings")
                End If
                Return _CapeComRatingSettings
            End Get
        End Property

        Public Shared Function IsCapeComRatingAvailable(quote As QuickQuoteObject) As Boolean
            If quote IsNot Nothing Then
                'CapeComRatingSettings.OtherQualifiers = DoesQuoteQualifyByLob(quote)
                'Remove the above and uncomment below when Snap and Diamond are ready CAH 10/3/2024
                CapeComRatingSettings.OtherQualifiers = DoesQuoteQualifyByLob(quote) _
                    AndAlso IsNewCompany(quote)
                Return AvailabilityByDateOrVersion.AvailabilityByDateOrVersion(quote, CapeComRatingSettings, AvailabilityByDateOrVersion.VersionTypeToTest.SubquoteFirstVersionId)
            End If
            Return False
        End Function

        Private Shared Function LobsAllowed() As List(Of Integer)
            Return GetListofIntegersFromAppSettingsKey(CapeComLobsAllowedKey)
        End Function

        Private Shared Function GetListofIntegersFromAppSettingsKey(key As String) As List(Of Integer)
            Dim c As New CommonHelperClass
            Dim integerList As List(Of Integer) = New List(Of Integer)
            Dim integerString As String = c.ConfigurationAppSettingValueAsString(key)
            If Not String.IsNullOrWhiteSpace(integerString) Then
                integerList = c.ListOfIntegerFromString(integerString, ",")
            End If
            Return integerList
        End Function
        Private Shared Function DoesQuoteQualifyByLob(quote As QuickQuoteObject) As Boolean
            Return quote?.LobId IsNot Nothing AndAlso LobsAllowed().Contains(quote.LobId.TryToGetInt32)
        End Function
    End Class
End Namespace
