Imports IFM.PrimativeExtensions
Imports PopupMessageClass
Imports QuickQuote.CommonMethods
Imports QuickQuote.CommonObjects
Imports System.Web.UI

Namespace IFM.VR.Common.Helpers.CGL
    Public Class CGLMedicalExpensesExcludedClassCodesHelper

        Public Const CGLMedicalExpensesExcludedClassCodesMsg As String = "Medical Expense coverage is excluded by one of the General Liability class codes you selected and has been removed from your quote. Additionally, any General Liability enhancement endorsement is ineligible when Medical Expense coverage is excluded, and if selected, it has also been removed from your quote."

        Public Const CGLMedicalExpensesNonExcludedClassCodesMsg As String = "Medical Expense coverage is no longer required to be excluded based on the class codes selected in your quote. We've updated your quote with a $5,000 limit for this coverage. If you still wish to exclude it, you can go back and manually update the field to exclude the coverage."

        Public Const GeneralLiabilityEnhancementEndorsementMsg As String = "Please note the General Liability Enhancement Endorsement has been removed from your quote, as it is ineligible when Medical Expense coverage is excluded."

        Public Shared Function HasCGLMedicalExpensesExcludedClassCode(ByVal quote As QuickQuoteObject) As Boolean 'added 06/02/2025 BD
            Dim hasIt As Boolean = False

            If quote IsNot Nothing Then
                Dim CLS As List(Of QuickQuoteGLClassification)
                If quote.QuoteTransactionType = QuickQuoteObject.QuickQuoteTransactionType.NewBusinessQuote Then
                    CLS = ClassCodeHelper.GetAllPolicyAndLocationClassCodes(quote)
                Else
                    CLS = ClassCodeHelper.GetAllPolicyAndLocationClassCodes_NewToImage(quote)
                End If
                If CLS IsNot Nothing AndAlso CLS.Count > 0 Then
                    For Each c As QuickQuoteGLClassification In CLS
                        If c IsNot Nothing AndAlso String.IsNullOrWhiteSpace(c.ClassCode) = False AndAlso c.ClassCode.EqualsAny("48924", "48177", "48178", "16670", "47221", "41665", "41666", "45224", "45225", "44222", "43117", "40046", "48252") Then
                            hasIt = True
                            Exit For
                        End If
                    Next
                End If
            End If

            Return hasIt
        End Function

        Public Shared Function HasCGLEnhancementEndorsementOrCPRPackageEnhancementEndorsement(ByVal quote As QuickQuoteObject) As Boolean 'added 06/02/2025 BD
            Dim hasIt As Boolean = False
            If quote IsNot Nothing Then
                Dim qqh As New QuickQuoteHelperClass
                Dim SubQuoteFirst = qqh.MultiStateQuickQuoteObjects(quote).GetItemAtIndex(0)
                If SubQuoteFirst IsNot Nothing AndAlso (SubQuoteFirst.Has_PackageGL_PlusEnhancementEndorsement = True OrElse SubQuoteFirst.Has_PackageGL_EnhancementEndorsement = True OrElse SubQuoteFirst.HasContractorsEnhancement = True OrElse SubQuoteFirst.HasFoodManufacturersEnhancement = True OrElse SubQuoteFirst.Has_PackageCPR_EnhancementEndorsement = True OrElse SubQuoteFirst.Has_PackageCPR_PlusEnhancementEndorsement = True) Then
                    hasIt = True
                End If
            End If
            Return hasIt
        End Function

        Public Shared Sub UpdateAndShowMessagesForMedicalExpensesDropdownForExcludedGLCodes(Quote As QuickQuoteObject, Page As Page)
            If Quote IsNot Nothing Then
                Dim qqh As New QuickQuoteHelperClass
                Dim SubQuoteFirst = qqh.MultiStateQuickQuoteObjects(Quote).GetItemAtIndex(0)
                Dim SubQuotes As List(Of QuickQuote.CommonObjects.QuickQuoteObject) = MultiState.General.SubQuotes(Quote)
                If ((Quote.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialPackage OrElse Quote.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialGeneralLiability) AndAlso (SubQuotes IsNot Nothing AndAlso SubQuotes.Count > 0 AndAlso SubQuoteFirst IsNot Nothing)) Then
                    If HasCGLMedicalExpensesExcludedClassCode(Quote) = True AndAlso SubQuoteFirst.MedicalExpensesLimitId <> "327" Then
                        For Each sq In SubQuotes
                            sq.MedicalExpensesLimitId = "327" 'Set to Excluded
                        Next
                        CGLMedicalExpensesExcludedClassCodesHelper.ShowCGLMedicalExpensesPopupMessage(Page, CGLMedicalExpensesExcludedClassCodesHelper.CGLMedicalExpensesExcludedClassCodesMsg)
                    ElseIf HasCGLMedicalExpensesExcludedClassCode(Quote) = False AndAlso HasCGLEnhancementEndorsementOrCPRPackageEnhancementEndorsement(Quote) = True AndAlso SubQuoteFirst.MedicalExpensesLimitId = "327" Then
                        CGLMedicalExpensesExcludedClassCodesHelper.ShowCGLMedicalExpensesPopupMessage(Page, CGLMedicalExpensesExcludedClassCodesHelper.GeneralLiabilityEnhancementEndorsementMsg)
                    End If
                    If SubQuoteFirst.MedicalExpensesLimitId = "327" Then
                        For Each sq In SubQuotes
                            sq.MedicalExpensesLimitId = "327" 'Set to Excluded
                            sq.Has_PackageGL_PlusEnhancementEndorsement = False
                            sq.Has_PackageGL_EnhancementEndorsement = False
                            sq.HasContractorsEnhancement = False
                            sq.HasFoodManufacturersEnhancement = False
                            sq.Has_PackageCPR_EnhancementEndorsement = False
                            sq.Has_PackageCPR_PlusEnhancementEndorsement = False
                        Next
                    End If
                End If
            End If
        End Sub

        Public Shared Sub ShowCGLMedicalExpensesPopupMessage(page As Page, popupMsg As String)
            Using popup As New PopupMessageObject(page, popupMsg)
                With popup
                    .Title = "Medical Expense Coverage Excluded"
                    .isFixedPositionOnScreen = True
                    .ZIndexOfPopup = 2
                    .isModal = True
                    .Image = PopupMessageObject.ImageOptions.None
                    .hideCloseButton = True
                    .AddButton("OK", True)
                    .CreateDynamicPopUpWindow()
                End With
            End Using
        End Sub

    End Class

End Namespace
