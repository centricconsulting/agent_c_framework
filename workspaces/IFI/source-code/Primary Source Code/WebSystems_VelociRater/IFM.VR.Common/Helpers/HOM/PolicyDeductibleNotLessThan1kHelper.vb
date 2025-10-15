Imports QuickQuote.CommonObjects

Namespace IFM.VR.Common.Helpers.HOM
    Public Class PolicyDeductibleNotLessThan1kHelper
        Private Shared _PolicyDeductibleNotLessThan1kSettings As NewFlagItem
        Public Shared ReadOnly Property PolicyDeductibleNotLessThan1kSettings() As NewFlagItem
            Get
                If _PolicyDeductibleNotLessThan1kSettings Is Nothing Then
                    _PolicyDeductibleNotLessThan1kSettings = New NewFlagItem("VR_HOM_PolicyDeductibleNotLessThan1k_Settings")
                End If
                Return _PolicyDeductibleNotLessThan1kSettings
            End Get
        End Property

        Const deductibleUpdatedMsg As String = "We've updated the deductible to meet our minimum requirement of $1,000."

        Public Shared Function PolicyDeductibleNotLessThan1kEnabled() As Boolean
            Return PolicyDeductibleNotLessThan1kSettings.EnabledFlag
        End Function

        Public Shared Function PolicyDeductibleNotLessThan1kEffDate() As Date
            Return PolicyDeductibleNotLessThan1kSettings.GetStartDateOrDefault("9/1/2024")
        End Function

        Public Shared Sub UpdatePolicyDeductibleNotLessThan1k(ByRef Quote As QuickQuoteObject, ByRef CrossDirection As Helper.EnumsHelper.CrossDirectionEnum, Optional ByRef ValidationErrors As List(Of WebValidationItem) = Nothing)
            Select Case CrossDirection
                Case Helper.EnumsHelper.CrossDirectionEnum.FORWARD
                    'Default deductibles lower than 1k to 1k and show message
                    Dim NeedsWarningMessage As Boolean = False
                    Dim MyLocation = Quote.Locations(0)
                    Select Case MyLocation.DeductibleLimitId
                        Case "21", "22", "23"
                            '21 = 250, 22 = 500, 23 = 750
                            'Forces the deductible to be 1,000 for deductibles lower than 1,000
                            MyLocation.DeductibleLimitId = "24" '1,000
                            NeedsWarningMessage = True
                    End Select
                    If NeedsWarningMessage AndAlso ValidationErrors IsNot Nothing Then
                        Dim i = New WebValidationItem(deductibleUpdatedMsg)
                        i.IsWarning = True
                        ValidationErrors.Add(i)
                    End If
                Case Helper.EnumsHelper.CrossDirectionEnum.BACK
                    'Allow deductibles lower than 1k
            End Select
        End Sub

        Public Shared Function IsPolicyDeductibleNotLessThan1kAvailable(quote As QuickQuoteObject) As Boolean
            If quote IsNot Nothing Then
                Return AvailabilityByDateOrVersion.AvailabilityByDateOrVersion(quote, PolicyDeductibleNotLessThan1kSettings, AvailabilityByDateOrVersion.VersionTypeToTest.SubquoteFirstVersionId)
            End If
            Return False
        End Function
    End Class
End Namespace
