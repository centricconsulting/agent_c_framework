Imports QuickQuote.CommonObjects
Imports QuickQuote.CommonMethods
Imports IFM.PrimativeExtensions
Imports System.Web.UI.WebControls

Namespace IFM.VR.Common.Helpers.FARM
    Public Class PollutionLiability1MHelper
        'Added 6/21/2022 for task 71215 MLW
        Const PollutionLiability1MWarningMsg As String = "The limit of $1,000,000 for Limited Farm Pollution is not available prior to 09/01/2022. The coverage has been removed."
        Public Shared Function PollutionLiability1MEnabled() As Boolean
            Dim PollutionLiability1MSettings As NewFlagItem = New NewFlagItem("VR_Far_PollutionLiability1M_Settings")
            Return PollutionLiability1MSettings.EnabledFlag
        End Function

        Public Shared Function PollutionLiability1MEffDate() As Date
            Dim PollutionLiability1MSettings As NewFlagItem = New NewFlagItem("VR_Far_PollutionLiability1M_Settings")
            Return PollutionLiability1MSettings.GetStartDateOrDefault("9/1/2022")
        End Function

        Public Shared Sub UpdatePollutionLiability1M(ByRef Quote As QuickQuoteObject, ByRef CrossDirection As Helper.EnumsHelper.CrossDirectionEnum, Optional ByRef ValidationErrors As List(Of WebValidationItem) = Nothing)
            Select Case CrossDirection
                Case Helper.EnumsHelper.CrossDirectionEnum.FORWARD
                    'Allow 1M limit                   
                Case Helper.EnumsHelper.CrossDirectionEnum.BACK
                    'If 1M selected, remove Pollution Liability Coverage
                    Dim NeedsWarningMessage As Boolean = False
                    Dim MyLocation As QuickQuoteLocation = Quote.Locations(0)                   
                    For Each sc As QuickQuote.CommonObjects.QuickQuoteSectionIICoverage In MyLocation.SectionIICoverages
                        If sc.CoverageCodeId = "40054" AndAlso sc.IncreasedLimitId = "444" Then
                            MyLocation.SectionIICoverages.Remove(sc)
                            NeedsWarningMessage = True
                            Exit For
                        End If
                    Next
                    
                    If NeedsWarningMessage AndAlso ValidationErrors IsNot Nothing Then
                        Dim i = New WebValidationItem(PollutionLiability1MWarningMsg)
                        i.IsWarning = True
                        ValidationErrors.Add(i)
                    End If
            End Select
        End Sub        

        Public Shared Function IsPollutionLiability1MAvailable(quote As QuickQuoteObject) As Boolean
            If quote IsNot Nothing Then
                Dim PollutionLiability1MSettings As NewFlagItem = New NewFlagItem("VR_Far_PollutionLiability1M_Settings")
                Return AvailabilityByDateOrVersion.AvailabilityByDateOrVersion(quote, PollutionLiability1MSettings, AvailabilityByDateOrVersion.VersionTypeToTest.SubquoteFirstVersionId)
            End If
            Return False
        End Function
    End Class
End Namespace
