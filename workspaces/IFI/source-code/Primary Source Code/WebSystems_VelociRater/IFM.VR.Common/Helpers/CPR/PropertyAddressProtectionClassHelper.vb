Imports QuickQuote.CommonObjects
Imports QuickQuote.CommonMethods
Imports IFM.PrimativeExtensions

Namespace IFM.VR.Common.Helpers.CPR
    Public Class PropertyAddressProtectionClassHelper

        Private Shared _paProtectionClassSettings As NewFlagItem
        Public Shared ReadOnly Property paProtectionClassSettings() As NewFlagItem
            Get
                If _paProtectionClassSettings Is Nothing Then
                    _paProtectionClassSettings = New NewFlagItem("VR_Cpr_Cpp_paProtectionClassUnits_Settings")
                End If
                Return _paProtectionClassSettings
            End Get
        End Property

        'Const paProtectionClassWarningMsg As String = "The number of paProtectionClasss for location one have been set to one. Please update the quote locations page, for all locations, if more than one are present."
        'Const paProtectionClassRemovedMsg As String = "Rating for paProtectionClass Surcharge has changed based on the effective date."
        Public Shared Sub UpdatepaProtectionClassUnits(ByRef Quote As QuickQuoteObject, ByRef CrossDirection As Helper.EnumsHelper.CrossDirectionEnum, Optional ByRef ValidationErrors As List(Of WebValidationItem) = Nothing)
            Dim NeedsWarningMessage As Boolean = False
            Select Case CrossDirection
                Case Helper.EnumsHelper.CrossDirectionEnum.FORWARD
                    'Added Coverage Possibility
                    'For Each l In Quote.Locations
                    '    If l.paProtectionClassSurcharge = True Then
                    '        NeedsWarningMessage = True
                    '        If l.paProtectionClassSurcharge_NumberOfUnits < 1 Then
                    '            l.paProtectionClassSurcharge_NumberOfUnits = 1

                    '        End If
                    '    End If
                    'Next
                    'If NeedsWarningMessage AndAlso ValidationErrors IsNot Nothing Then
                    '    Dim i = New WebValidationItem(paProtectionClassWarningMsg)
                    '    i.IsWarning = True
                    '    ValidationErrors.Add(i)
                    'End If
                Case Helper.EnumsHelper.CrossDirectionEnum.BACK
                    'Remove Coverage Possibility
                    'For Each l In Quote.Locations

                    '    If Quote.Locations.IndexOf(l) > 0 Then
                    '        l.paProtectionClassSurcharge = False
                    '    End If

                    '    l.paProtectionClassSurcharge_NumberOfUnits = 0

                    '    If l.paProtectionClassSurcharge Then
                    '        NeedsWarningMessage = True
                    '    End If
                    'Next
                    'If NeedsWarningMessage AndAlso ValidationErrors IsNot Nothing Then
                    '    Dim i = New WebValidationItem(paProtectionClassRemovedMsg)
                    '    i.IsWarning = True
                    '    ValidationErrors.Add(i)
                    'End If

            End Select
        End Sub

        Public Shared Function ispaProtectionClassUnitsAvailable(quote As QuickQuoteObject) As Boolean
            If quote IsNot Nothing Then
                Dim qqh As New QuickQuoteHelperClass
                Dim SubQuoteFirst = qqh.MultiStateQuickQuoteObjects(quote).GetItemAtIndex(0)


                'paProtectionClassSettings.OtherQualifiers = ""
                Return AvailabilityByDateOrVersion.AvailabilityByDateOrVersion(quote, paProtectionClassSettings, AvailabilityByDateOrVersion.VersionTypeToTest.SubquoteFirstVersionId)
            End If
            Return False

        End Function
    End Class

End Namespace
