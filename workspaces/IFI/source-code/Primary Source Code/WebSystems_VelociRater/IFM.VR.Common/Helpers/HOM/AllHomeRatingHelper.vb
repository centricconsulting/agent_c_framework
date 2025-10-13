Imports QuickQuote.CommonObjects
Namespace IFM.VR.Common.Helpers.HOM
    Public Class AllHomeRatingHelper
        Private Shared _AllHomeRatingSettings As NewFlagItem
        Private Shared chc As New CommonHelperClass
        Public Shared ReadOnly Property AllHomeRatingSettings() As NewFlagItem
            Get
                If _AllHomeRatingSettings Is Nothing Then
                    _AllHomeRatingSettings = New NewFlagItem("VR_HOM_PreLoadIntegrationCall_Settings")
                End If
                Return _AllHomeRatingSettings
            End Get
        End Property

        Public Shared Function IsAllHomeRatingAvailable(quote As QuickQuoteObject) As Boolean
            If quote IsNot Nothing Then
                Return AvailabilityByDateOrVersion.AvailabilityByDateOrVersion(quote, AllHomeRatingSettings, AvailabilityByDateOrVersion.VersionTypeToTest.SubquoteFirstVersionId)
            End If
            Return False
        End Function
    End Class
End Namespace
