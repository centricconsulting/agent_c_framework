Imports QuickQuote.CommonObjects
Imports QuickQuote.CommonMethods

Namespace IFM.VR.Common.Helpers.FARM

    Public Class FarmPersonalLiabilityGL9Helper

        Public Structure IFMSectionII_structure
            Public SectionIndex As Integer
            Public SectionIIItem As QuickQuote.CommonObjects.QuickQuoteSectionIICoverage
        End Structure

        ''' <summary>
        ''' Returns TRUE if a quote is eligible for GL-9, FALSE if not.
        ''' A Farm quote is eligible for GL-9 when program type is 6 or 8 and quote type is commercial
        ''' </summary>
        ''' <param name="qt"></param>
        ''' <returns></returns>
        Public Shared Function QuoteIsEligibleForGL9(ByVal qt As QuickQuoteObject) As Boolean
            If qt Is Nothing Then Return False
            If qt.Locations Is Nothing Then Return False
            If qt.Locations.Count <= 0 Then Return False

            If qt.Locations(0).ProgramTypeId = "6" OrElse qt.Locations(0).ProgramTypeId = "8" Then
                If qt.Policyholder IsNot Nothing Then
                    If qt.Policyholder.Name IsNot Nothing Then
                        If qt.Policyholder.Name.TypeId = "2" Then Return True   ' typeid 2 is commercial
                    End If
                End If
            End If

            Return False
        End Function

        ''' <summary>
        ''' Returns TRUE if any location on the quote has a GL-9 coverage.  Returns FALSE if not.
        ''' </summary>
        ''' <param name="qt"></param>
        ''' <returns></returns>
        Public Shared Function QuoteHasGL9(ByVal qt As QuickQuoteObject) As Boolean
            If qt Is Nothing Then Return False
            If qt.Locations Is Nothing Then Return False
            If qt.Locations.Count <= 0 Then Return False

            For Each loc As QuickQuoteLocation In qt.Locations
                If loc.SectionIICoverages IsNot Nothing AndAlso loc.SectionIICoverages.Count > 0 Then
                    For Each sc As QuickQuoteSectionIICoverage In loc.SectionIICoverages
                        If sc.CoverageType = QuickQuoteSectionIICoverage.SectionIICoverageType.Location_Farm_Personal_Liability_GL9 Then Return True
                    Next
                End If
            Next

            Return False
        End Function

        ''' <summary>
        ''' Returns a list of all GL-9's on the quote - pulls from all locations
        ''' </summary>
        ''' <param name="qt"></param>
        ''' <returns></returns>
        Public Shared Function GetAllQuoteGL9s(ByVal qt As QuickQuoteObject) As List(Of QuickQuoteSectionIICoverage)
            Dim GL9s As New List(Of QuickQuoteSectionIICoverage)

            If qt Is Nothing OrElse qt.Locations Is Nothing OrElse qt.Locations.Count <= 0 Then Return GL9s

            For Each loc As QuickQuoteLocation In qt.Locations
                If loc.SectionIICoverages IsNot Nothing AndAlso loc.SectionIICoverages.Count > 0 Then
                    For Each sc As QuickQuoteSectionIICoverage In loc.SectionIICoverages
                        If sc.CoverageType = QuickQuoteSectionIICoverage.SectionIICoverageType.Location_Farm_Personal_Liability_GL9 Then GL9s.Add(sc)
                    Next
                End If
            Next

            Return GL9s
        End Function

        ''' <summary>
        ''' Returns a list of all GL-9's and their Section II indexes in the passed location
        ''' </summary>
        ''' <param name="MyLoc"></param>
        ''' <returns></returns>
        Public Shared Function GetAllLocationGL9s(ByVal MyLoc As QuickQuoteLocation, ByRef LocationGL9Count As Integer) As List(Of IFMSectionII_structure)
            Dim GL9List As New List(Of IFMSectionII_structure)
            Dim ndx As Integer = -1
            LocationGL9Count = 0

            If MyLoc Is Nothing Then Return GL9List
            If MyLoc.SectionIICoverages Is Nothing Then Return GL9List

            For Each sc As QuickQuoteSectionIICoverage In MyLoc.SectionIICoverages
                ndx += 1
                If sc.CoverageType = QuickQuoteSectionIICoverage.SectionIICoverageType.Location_Farm_Personal_Liability_GL9 Then
                    LocationGL9Count += 1
                    Dim newitem As New IFMSectionII_structure()
                    newitem.SectionIndex = ndx
                    newitem.SectionIIItem = sc
                    GL9List.Add(newitem)
                End If
            Next

            Return GL9List
        End Function

        ''' <summary>
        ''' Returns the number of GL9's on the passed location
        ''' </summary>
        ''' <param name="loc"></param>
        ''' <returns></returns>
        Public Shared Function NumberOfGL9sOnALocation(ByVal loc As QuickQuoteLocation) As Integer
            Dim cnt As Integer = 0
            If loc Is Nothing Then Return 0
            If loc.SectionIICoverages Is Nothing OrElse loc.SectionIICoverages.Count <= 0 Then Return 0

            For Each sc As QuickQuoteSectionIICoverage In loc.SectionIICoverages
                If sc.CoverageType = QuickQuoteSectionIICoverage.SectionIICoverageType.Location_Farm_Personal_Liability_GL9 Then cnt += 1
            Next
            Return cnt
        End Function

        ''' <summary>
        ''' Returns the total number of GL9's on a quote
        ''' </summary>
        ''' <param name="qt"></param>
        ''' <returns></returns>
        Public Shared Function NumberOfGL9sOnAQuote(ByVal qt As QuickQuoteObject) As Integer
            Dim cnt As Integer = 0
            If qt Is Nothing Then Return 0
            If qt.Locations Is Nothing OrElse qt.Locations.Count <= 0 Then Return 0

            For Each loc As QuickQuoteLocation In qt.Locations
                If loc.SectionIICoverages IsNot Nothing AndAlso loc.SectionIICoverages.Count > 0 Then
                    For Each sc As QuickQuoteSectionIICoverage In loc.SectionIICoverages
                        If sc.CoverageType = QuickQuoteSectionIICoverage.SectionIICoverageType.Location_Farm_Personal_Liability_GL9 Then cnt += 1
                    Next
                End If
            Next

            Return cnt
        End Function

        ''' <summary>
        ''' If the quote location at the passed index is the only location on the quote with a GL-9 coverage returns true, otherwise returns false
        ''' </summary>
        ''' <param name="qt"></param>
        ''' <param name="LocIndex"></param>
        ''' <returns></returns>
        Public Shared Function LocationIsTheOnlyLocationWithGL9(ByVal qt As QuickQuoteObject, ByVal LocIndex As Integer) As Boolean
            Dim ndx As Integer = -1

            If qt Is Nothing Or qt.Locations Is Nothing OrElse qt.Locations.Count <= 0 Then Return False
            If LocIndex < 0 Then Return False

            For Each loc As QuickQuoteLocation In qt.Locations
                ndx += 1
                If ndx <> LocIndex Then
                    If loc.SectionIICoverages IsNot Nothing AndAlso loc.SectionIICoverages.Count > 0 Then
                        For Each sc As QuickQuoteSectionIICoverage In loc.SectionIICoverages
                            If sc.CoverageType = QuickQuoteSectionIICoverage.SectionIICoverageType.Location_Farm_Personal_Liability_GL9 Then Return False
                        Next
                    End If
                End If
            Next

            Return True
        End Function

    End Class

End Namespace
