Imports QuickQuote.CommonObjects
Imports QuickQuote.CommonMethods

Namespace IFM.VR.Common.Helpers.FARM

    Public Class FarmAdditionalDwellingHelper

        ''' <summary>
        ''' Removes any dummy 'Additional Residence Rented to Others" coverages from the SectionIICoverages on Location 0
        ''' This needs to be executed prior to rating.  The dummy coverage is required to make the UI work correctly.
        ''' </summary>
        ''' <param name="QuoteId"></param>  Pass in the quote ID for the quote to be checked
        ''' <param name="Quote"></param>  The quote object that will be updated
        ''' <param name="NumRemoved"></param>  By Reference. Returns the number of dummy records removed.  Will be -1 if error.
        ''' <param name="ErrMsg"></param>  Returns any error messages.
        ''' <returns></returns>
        ''' Returns True if the function succeeded, false if not
        Public Shared Function RemoveAnyFARMDummyAdditionalResidences(ByVal QuoteId As String, ByRef Quote As QuickQuoteObject, ByRef NumRemoved As Integer, ByRef ErrMsg As String) As Boolean
            NumRemoved = -1

            If Quote Is Nothing Then
                ErrMsg = "Quote is nothing"
                Return False
            End If
            If QuoteId Is Nothing OrElse QuoteId.Trim = String.Empty OrElse (Not IsNumeric(QuoteId)) Then
                ErrMsg = "Quote Id id nothing or invalid"
                Return False
            End If
            If Quote.Locations Is Nothing OrElse Quote.Locations.Count = 0 Then
                ErrMsg = "Quote contains no locations"
                Return False
            End If

            NumRemoved = 0

            If Quote.Locations(0).SectionIICoverages Is Nothing OrElse Quote.Locations(0).SectionIICoverages.Count = 0 Then Return True

            Dim DummyRemoved As Boolean = False
            Dim err As String = ""
coverageLoop:
            For Each SC In Quote.Locations(0).SectionIICoverages
                If SC.CoverageType = QuickQuoteSectionIICoverage.SectionIICoverageType.OptionalLiability_AdditionalResidencesOrFarmsRentedtoOthers Then
                    If AdditionalResidenceIsDummy(SC) Then
                        Quote.Locations(0).SectionIICoverages.Remove(SC)
                        DummyRemoved = True
                        NumRemoved += 1
                        GoTo coverageLoop
                    End If
                End If
            Next
            If DummyRemoved Then IFM.VR.Common.QuoteSave.QuoteSaveHelpers.SaveQuote(QuoteId, Quote, err)

            Return True
        End Function

        ''' <summary>
        ''' Checks to see if the passed section coverage is a dummy record
        ''' </summary>
        ''' <param name="SC"></param>
        ''' <returns></returns>
        Public Shared Function AdditionalResidenceIsDummy(ByVal SC As QuickQuote.CommonObjects.QuickQuoteSectionIICoverage) As Boolean
            If SC Is Nothing Then Return False
            If SC.Description = "DUMMY ITEM" AndAlso
                SC.Address.HouseNum = "999" AndAlso
                SC.Address.StreetName = "DUMMY" AndAlso
                SC.Address.Zip = "99999" OrElse SC.Address.Zip = "99999-0000" Then
                Return True
            End If
            Return False
        End Function

    End Class

End Namespace

