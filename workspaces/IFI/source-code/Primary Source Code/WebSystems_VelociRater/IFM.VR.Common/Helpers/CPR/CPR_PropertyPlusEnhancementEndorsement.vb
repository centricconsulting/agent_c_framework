Imports QuickQuote.CommonObjects
Imports QuickQuote.CommonMethods
Imports IFM.PrimativeExtensions
Imports System.Web.UI.WebControls
Namespace IFM.VR.Common.Helpers.CPR
    'Added 6/28/2022 for task 75037 MLW
    Public Class CPR_PropertyPlusEnhancementEndorsement

        Private Shared _PropPlusEnhancementSettings As NewFlagItem
        Public Shared ReadOnly Property PropPlusEnhancementSettings() As NewFlagItem
            Get
                If _PropPlusEnhancementSettings Is Nothing Then
                    _PropPlusEnhancementSettings = New NewFlagItem("VR_CPR_CPP_PlusEnhancementEndorsement_Settings")
                End If
                Return _PropPlusEnhancementSettings
            End Get
        End Property

        Const PropPlusEnhancementWarningMsg As String = "Property PLUS Enhancement Endorsement is not available prior to 09/01/2022. The coverage has been removed."
        Public Shared Function PropPlusEnhancementEnabled() As Boolean
            Return PropPlusEnhancementSettings.EnabledFlag
        End Function

        Public Shared Function PropPlusEnhancementEffDate() As Date
            Return PropPlusEnhancementSettings.GetStartDateOrDefault("9/1/2022")
        End Function

        Public Shared Sub UpdatePropPlusEnhancement(ByRef Quote As QuickQuoteObject, ByRef CrossDirection As Helper.EnumsHelper.CrossDirectionEnum, Optional ByRef ValidationErrors As List(Of WebValidationItem) = Nothing)
            Select Case CrossDirection
                Case Helper.EnumsHelper.CrossDirectionEnum.FORWARD
                    'Allow coverage                   
                Case Helper.EnumsHelper.CrossDirectionEnum.BACK
                    'Remove coverage
                    Dim NeedsWarningMessage As Boolean = False
                    Dim SubQuotes As List(Of QuickQuote.CommonObjects.QuickQuoteObject) = MultiState.General.SubQuotes(Quote)
                    If SubQuotes IsNot Nothing And SubQuotes.Count > 0 Then
                        For Each sq In SubQuotes
                            If sq.Has_PackageCPR_PlusEnhancementEndorsement = True Then
                                sq.Has_PackageCPR_PlusEnhancementEndorsement = False
                                NeedsWarningMessage = True
                            End If
                        Next
                    End If
                    If NeedsWarningMessage AndAlso ValidationErrors IsNot Nothing Then
                        Dim i = New WebValidationItem(PropPlusEnhancementWarningMsg)
                        i.IsWarning = True
                        ValidationErrors.Add(i)
                    End If
            End Select
        End Sub

        Public Shared Function IsPropPlusEnhancementAvailable(quote As QuickQuoteObject) As Boolean
            If quote IsNot Nothing Then
                Return AvailabilityByDateOrVersion.AvailabilityByDateOrVersion(quote, PropPlusEnhancementSettings, AvailabilityByDateOrVersion.VersionTypeToTest.CppPackagePartForCpr)
            End If
            Return False
        End Function
    End Class

End Namespace