Imports QuickQuote.CommonObjects

Namespace IFM.VR.Common.Helpers.FARM
    Public Class AddlResidenceRentedToOthersHelper
        Public Shared Function GetAddlResidenceRentedToOthersCoverages(ByVal quote As QuickQuoteObject) As List(Of QuickQuote.CommonObjects.QuickQuoteSectionIICoverage)
            Dim covs As New List(Of QuickQuote.CommonObjects.QuickQuoteSectionIICoverage)

            If quote IsNot Nothing OrElse quote.Locations IsNot Nothing OrElse quote.Locations.Count > 0 Then 
                For Each l As QuickQuoteLocation In quote.Locations
                    If l.SectionIICoverages IsNot Nothing AndAlso l.SectionIICoverages.Count > 0 Then
                        For Each c As QuickQuote.CommonObjects.QuickQuoteSectionIICoverage In l.SectionIICoverages
                            If c.CoverageType = QuickQuoteSectionIICoverage.SectionIICoverageType.OptionalLiability_AdditionalResidencesOrFarmsRentedtoOthers Then
                                covs.Add(c)
                            End If
                        Next
                    End If
                Next
            End If

            Return covs
        End Function

    End Class
End Namespace
