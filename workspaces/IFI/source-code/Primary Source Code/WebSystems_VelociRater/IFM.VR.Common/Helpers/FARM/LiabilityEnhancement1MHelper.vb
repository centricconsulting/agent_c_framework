Imports QuickQuote.CommonObjects
Imports QuickQuote.CommonMethods
Imports IFM.PrimativeExtensions
Imports System.Web.UI.WebControls

Namespace IFM.VR.Common.Helpers.FARM
    Public Class LiabilityEnhancement1MHelper
        'Added 8/8/2022 for task 76030 MLW
        Const LiabilityEnhancement1MWarningMsg As String = "The limit of $1,000,000 for Liability Enhancement Endorsement is not available prior to 09/01/22. The coverage has been reset to the included limit of $25,000."
        Public Shared Function LiabilityEnhancement1MEnabled() As Boolean
            Dim LiabilityEnhancement1MSettings As NewFlagItem = New NewFlagItem("VR_Far_LiabilityEnhancement1M_Settings")
            Return LiabilityEnhancement1MSettings.EnabledFlag
        End Function

        Public Shared Function LiabilityEnhancement1MEffDate() As Date
            Dim LiabilityEnhancement1MSettings As NewFlagItem = New NewFlagItem("VR_Far_LiabilityEnhancement1M_Settings")
            Return LiabilityEnhancement1MSettings.GetStartDateOrDefault("9/1/2022")
        End Function

        Public Shared Sub UpdateLiabilityEnhancement1M(ByRef Quote As QuickQuoteObject, ByRef CrossDirection As Helper.EnumsHelper.CrossDirectionEnum, Optional ByRef ValidationErrors As List(Of WebValidationItem) = Nothing)
            Select Case CrossDirection
                Case Helper.EnumsHelper.CrossDirectionEnum.FORWARD
                    'Allow 1M limit                   
                Case Helper.EnumsHelper.CrossDirectionEnum.BACK
                    'If 1M selected, set Liability Enhancement Endorsement Limit to 25,000 included limit
                    Dim NeedsWarningMessage As Boolean = False
                    Dim MyLocation As QuickQuoteLocation = Quote.Locations(0)                   
                    For Each sc As QuickQuote.CommonObjects.QuickQuoteSectionIICoverage In MyLocation.SectionIICoverages
                        If sc.CoverageCodeId = "80094" AndAlso sc.IncreasedLimitId = "444" Then '444 = 1,000,000
			                sc.IncreasedLimitId = "" '25,000 [INC]
                            sc.TotalLimit = "25,000"
                            NeedsWarningMessage = True
                            Exit For
                        End If
                    Next
                    
                    If NeedsWarningMessage AndAlso ValidationErrors IsNot Nothing Then
                        Dim i = New WebValidationItem(LiabilityEnhancement1MWarningMsg)
                        i.IsWarning = True
                        ValidationErrors.Add(i)
                    End If
            End Select
        End Sub      

        Public Shared Function IsLiabilityEnhancement1MAvailable(quote As QuickQuoteObject) As Boolean
            If quote IsNot Nothing Then
                Dim LiabilityEnhancement1MSettings As NewFlagItem = New NewFlagItem("VR_Far_LiabilityEnhancement1M_Settings")
                Return AvailabilityByDateOrVersion.AvailabilityByDateOrVersion(quote, LiabilityEnhancement1MSettings, AvailabilityByDateOrVersion.VersionTypeToTest.SubquoteFirstVersionId)
            End If
            Return False
        End Function
    End Class
End Namespace
