Imports QuickQuote.CommonObjects
Imports QuickQuote.CommonMethods
Imports IFM.PrimativeExtensions
Imports System.Web.UI.WebControls
Namespace IFM.VR.Common.Helpers.CGL
    'Added 6/28/2022 for task 75037 MLW
    Public Class CGL_GenLiabPlusEnhancementEndorsement

        Private Shared _GenLiabPlusEnhancementSettings As NewFlagItem
        Public Shared ReadOnly Property GenLiabPlusEnhancementSettings() As NewFlagItem
            Get
                If _GenLiabPlusEnhancementSettings Is Nothing Then
                    _GenLiabPlusEnhancementSettings = New NewFlagItem("VR_CGL_GenLiabPlusEnhancementEndorsement_Settings")
                End If
                Return _GenLiabPlusEnhancementSettings
            End Get
        End Property

        Const GenLiabPlusEnhancementWarningMsg As String = "General Liability PLUS Enhancement Endorsement is available as of (01/01/2023). The coverage has been removed."
        Public Shared Function GlPlusEnhancementEnabled() As Boolean
            Return GenLiabPlusEnhancementSettings.EnabledFlag
        End Function

        Public Shared Function GlPlusEnhancementEffDate() As Date
            Return GenLiabPlusEnhancementSettings.GetStartDateOrDefault("1/1/2023")
        End Function

        Public Shared Sub UpdateGlPlusEnhancement(ByRef Quote As QuickQuoteObject, ByRef CrossDirection As Helper.EnumsHelper.CrossDirectionEnum, Optional ByRef ValidationErrors As List(Of WebValidationItem) = Nothing)
            Select Case CrossDirection
                Case Helper.EnumsHelper.CrossDirectionEnum.FORWARD
                    'Allow coverage                   
                Case Helper.EnumsHelper.CrossDirectionEnum.BACK
                    'Remove coverage
                    Dim NeedsWarningMessage As Boolean = False
                    Dim SubQuotes As List(Of QuickQuote.CommonObjects.QuickQuoteObject) = MultiState.General.SubQuotes(Quote)
                    If SubQuotes IsNot Nothing And SubQuotes.Count > 0 Then
                        For Each sq In SubQuotes
                            If sq.Has_PackageGL_PlusEnhancementEndorsement = True Then
                                sq.Has_PackageGL_PlusEnhancementEndorsement = False
                                NeedsWarningMessage = True
                            End If
                        Next
                    End If
                    If NeedsWarningMessage AndAlso ValidationErrors IsNot Nothing Then
                        Dim i = New WebValidationItem(GenLiabPlusEnhancementWarningMsg)
                        i.IsWarning = True
                        ValidationErrors.Add(i)
                    End If
            End Select
        End Sub

        Public Shared Function IsGlPlusEnhancementAvailable(quote As QuickQuoteObject) As Boolean
            If quote IsNot Nothing Then
                Return AvailabilityByDateOrVersion.AvailabilityByDateOrVersion(quote, GenLiabPlusEnhancementSettings, AvailabilityByDateOrVersion.VersionTypeToTest.CppPackagePartForCpr)
            End If
            Return False
        End Function
    End Class

End Namespace