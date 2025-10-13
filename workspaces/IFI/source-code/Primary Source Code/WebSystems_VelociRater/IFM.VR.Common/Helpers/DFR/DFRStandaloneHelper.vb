Imports QuickQuote.CommonObjects
Imports IFM.PrimativeExtensions

Namespace IFM.VR.Common.Helpers.DFR

    Public Class DFRStandaloneHelper
        'VR_DFR_Standalone_Settings
        Private Shared _StandaloneSettings As NewFlagItem
        Public Shared ReadOnly Property StandaloneSettings() As NewFlagItem
            Get
                If _StandaloneSettings Is Nothing Then
                    _StandaloneSettings = New NewFlagItem("VR_DFR_Standalone_Settings")
                End If
                Return _StandaloneSettings
            End Get
        End Property

        Public Overloads Shared Function isDFRStandaloneAvailable(quote As QuickQuoteObject) As Boolean
            If quote IsNot Nothing Then
                Return AvailabilityByDateOrVersion.AvailabilityByDateOrVersion(quote, StandaloneSettings, AvailabilityByDateOrVersion.VersionTypeToTest.SubquoteFirstVersionId)
            End If
            Return False
        End Function

        ''' <summary>
        ''' Use only when effective date is known and on new business
        ''' </summary>
        ''' <param name="EffectiveDate">The effective date on the quote</param>
        ''' <returns>True if the feature is available</returns>
        Public Overloads Shared Function isDFRStandaloneAvailable(EffectiveDate As String) As Boolean
            If EffectiveDate.HasValue AndAlso EffectiveDate.IsDate Then
                Dim qqo As New QuickQuote.CommonObjects.QuickQuoteObject
                qqo.EffectiveDate = EffectiveDate
                qqo.LobType = QuickQuoteObject.QuickQuoteLobType.DwellingFirePersonal
                Return isDFRStandaloneAvailable(qqo)
            End If
            Return False
        End Function

        Public Shared Sub UpdateDFRStandaloneQuestion(ByRef Quote As QuickQuoteObject, ByRef CrossDirection As Helper.EnumsHelper.CrossDirectionEnum, Optional ByRef ValidationErrors As List(Of WebValidationItem) = Nothing)
            Select Case CrossDirection
                Case Helper.EnumsHelper.CrossDirectionEnum.FORWARD
                    'Update to show new question verbiage, clear out additional info text on "No" response
                    If Quote?.Locations IsNot Nothing AndAlso Quote.Locations.Count > 0 AndAlso Quote.Locations(0).PolicyUnderwritings IsNot Nothing AndAlso Quote.Locations(0).PolicyUnderwritings.Count > 0 Then
                        For Each puw In Quote.Locations(0).PolicyUnderwritings
                            if puw.PolicyUnderwritingCodeId = "9419" AndAlso puw.PolicyUnderwritingAnswer = "-1" AndAlso puw.PolicyUnderwritingExtraAnswerTypeId = "1" Then
                                puw.PolicyUnderwritingExtraAnswer = ""
                            End If
                        Next
                    End If
                Case Helper.EnumsHelper.CrossDirectionEnum.BACK
                    'Update to show old question verbiage
            End Select
        End Sub        
    End Class
End Namespace
