Imports System.Runtime.CompilerServices
Imports QuickQuote.CommonObjects
Imports QQ = QuickQuote.CommonObjects
Public Module QuickQuoteExtensions 'added 10/9/2014
    <Extension>
    Public Function GetAllGLClassificationCodes(quote As QQ.QuickQuoteObject) As IList(Of String)
        Dim quoteClassCodes, locationClassCodes, rtn As IEnumerable(Of String)
        quoteClassCodes = Nothing
        locationClassCodes = Nothing
        rtn = Nothing
        If quote.GLClassifications IsNot Nothing Then
            quoteClassCodes = From a In quote.GLClassifications
                              Select a.ClassCode
        End If
        If quote.Locations IsNot Nothing Then
            locationClassCodes = From a In quote.Locations
                                 Where a.GLClassifications IsNot Nothing
                                 From c In a.GLClassifications
                                 Select c.ClassCode
        End If
        If quoteClassCodes IsNot Nothing Then
            rtn = quoteClassCodes
        End If
        If rtn IsNot Nothing AndAlso locationClassCodes IsNot Nothing Then
            rtn = rtn.Union(locationClassCodes)
        ElseIf rtn Is Nothing AndAlso locationClassCodes IsNot Nothing Then
            rtn = locationClassCodes
        End If
        If rtn IsNot Nothing Then
            Return rtn.ToList
        Else
            'Return Nothing
            'updated 10/10/2014
            Return New List(Of String)
        End If
    End Function

    ''' <summary>
    ''' Use Me.GoverningStateQuote if available.
    ''' </summary>
    ''' <param name="topQuote"></param>
    ''' <param name="subQuotes"></param>
    ''' <returns></returns>
    <Extension>
    Public Function GoverningStateQuoteFor(topQuote As QuickQuote.CommonObjects.QuickQuoteObject) As QuickQuote.CommonObjects.QuickQuoteObject
        Dim QQHelper As New QuickQuote.CommonMethods.QuickQuoteHelperClass()
        Return QQHelper.GoverningStateQuote(topQuote)
    End Function

    ''' <summary>
    ''' Use Me.SubQuotes if available.
    ''' </summary>
    ''' <param name="qqo"></param>
    ''' <returns></returns>
    <Extension>
    Public Function SubQuotesFor(qqo As QuickQuote.CommonObjects.QuickQuoteObject) As List(Of QuickQuote.CommonObjects.QuickQuoteObject)
        Dim QQHelper As New QuickQuote.CommonMethods.QuickQuoteHelperClass()
        Return QQHelper.MultiStateQuickQuoteObjects(qqo)
    End Function
    <Extension>
    Public Function GetExtraElementOrNothing(sdo As QuickQuoteStaticDataOption, elementName As String) As QuickQuoteStaticDataElement
        Dim retval As QuickQuoteStaticDataElement = Nothing

        if sdo?.MiscellaneousElements?.Any() then
            retval = sdo.MiscellaneousElements.FirstOrDefault(Function(misc) misc.nvp_name.Equals(elementName, StringComparison.CurrentCultureIgnoreCase))                                                                                                  
        End If

        Return retval
    End Function
End Module

