Imports Microsoft.VisualBasic
Imports QuickQuote.CommonObjects

Public Class SummaryHelperClass

    Public ReadOnly Property LinesInPage As Integer 'added 6/28/2013
        Get
            Return 60
        End Get
    End Property

    Public Function GetAgencyName(ByVal name As QuickQuote.CommonObjects.QuickQuoteName) As String 'added 6/10/2013
        Dim agName As String = ""

        If name IsNot Nothing Then
            If name.CommercialDBAname <> "" Then
                agName = name.CommercialDBAname
            ElseIf name.DoingBusinessAsName <> "" Then 'this probably won't ever have a value
                agName = name.DoingBusinessAsName
            ElseIf name.CommercialIRSname <> "" Then
                agName = name.CommercialIRSname
            Else
                agName = name.DisplayNameForWeb 'just in case nothing is found in any of the above, which should mean that this one is blank too
            End If
        End If

        Return agName
    End Function

    Public Sub AddPageBreakToPlaceholder(ByRef ph As PlaceHolder) 'added 6/28/2013 (taken from code on DiamondQuoteProposal.aspx.vb)
        Dim pageBreak As New Panel
        pageBreak.CssClass = "page-break"
        Dim pageBreakSection As New Panel
        pageBreakSection.CssClass = "page-break-section"
        'pageBreak.Controls.Add(pageBreakSection)'would only do this if other div needs to wrap around it
        ph.Controls.Add(pageBreak)
        ph.Controls.Add(pageBreakSection)
    End Sub

    ''TODO - Extract functions like these into a resuable libary or add to QuickQuote
    Public Shared Function Find_First_PolicyLevelCoverage(quoteList As List(Of QuickQuoteObject), coverageCodeId As String) As QuickQuoteCoverage
        'check parameters
        If quoteList Is Nothing Then Throw New ArgumentNullException(NameOf(quoteList))
        If String.IsNullOrWhiteSpace(coverageCodeId) OrElse Integer.TryParse(coverageCodeId, 0) = False Then
            Throw New ArgumentException("The supplied coverage code id is not a valid value or is an empty string", NameOf(coverageCodeId))
        End If

        Dim found As Boolean = False
        Dim retval As QuickQuoteCoverage = Nothing

        '' this is faster than foreach or equivalent LINQ expression because it doesn't incur the overhead of enumerators
        '' the Find function predates linq and uses a regular for loop internally - it is used for readability and conciseness here
        If quoteList.Any() Then
            Dim indx = 0
            Do
                retval = quoteList(indx).PolicyCoverages?.Find(Function(c As QuickQuoteCoverage) c.CoverageCodeId = coverageCodeId)
                indx += 1
            Loop While retval Is Nothing AndAlso indx < quoteList.Count
        End If
        Return retval
    End Function

    Public Shared Function Find_First_PackageLevelCoverage(packagePart As QuickQuotePackagePart, coverageCodeId As String) As QuickQuoteCoverage
        'check parameters
        If packagePart Is Nothing Then Throw New ArgumentNullException(NameOf(packagePart))
        If String.IsNullOrWhiteSpace(coverageCodeId) OrElse Integer.TryParse(coverageCodeId, 0) = False Then
            Throw New ArgumentException("The supplied coverage code id is not a valid value or is an empty string", NameOf(coverageCodeId))
        End If

        Dim found As Boolean = False
        Dim retval As QuickQuoteCoverage = Nothing

        '' this is faster than foreach or equivalent LINQ expression because it doesn't incur the overhead of enumerators
        '' the Find function predates linq and uses a regular for loop internally - it is used for readability and conciseness here

        retval = packagePart.Coverages?.Find(Function(c As QuickQuoteCoverage) c.CoverageCodeId = coverageCodeId)

        Return retval
    End Function
End Class