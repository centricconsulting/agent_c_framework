Imports QuickQuote.CommonMethods
Imports QuickQuote.CommonObjects
Imports IFM.PrimativeExtensions

Public Class AvailabilityByDateOrVersion
    Public Shared Function AvailabilityByDateOrVersion(quote As QuickQuoteObject, enabledOptions As Boolean, startDate As String, version As String) As Boolean
        Dim qqh As New QuickQuoteHelperClass
        Dim SubQuoteFirst = qqh.MultiStateQuickQuoteObjects(quote).GetItemAtIndex(0)
        Dim isNewBusiness As Boolean = quote.QuoteTransactionType = QuickQuoteObject.QuickQuoteTransactionType.NewBusinessQuote
        Dim QuoteVersion As String = SubQuoteFirst.VersionId
        Dim QuoteEffectiveDate As String = quote.EffectiveDate

        If enabledOptions Then
            If isNewBusiness Then
                If qqh.IsDateString(QuoteEffectiveDate) AndAlso qqh.IsDateString(startDate) Then
                    If CDate(QuoteEffectiveDate) >= startDate Then Return True
                End If
            Else
                If QuoteVersion.TryToGetInt32 >= version.TryToGetInt32 Then Return True
            End If
        End If
        Return False
    End Function

    Public Shared Function AvailabilityByDateOrVersion(quote As QuickQuoteObject, FlagClass As NewFlagItem, VersionType As VersionTypeToTest) As Boolean
        Dim qqh As New QuickQuoteHelperClass
        Dim SubQuoteFirst = qqh.MultiStateQuickQuoteObjects(quote).GetItemAtIndex(0)
        Dim isNewBusiness As Boolean = quote.QuoteTransactionType = QuickQuoteObject.QuickQuoteTransactionType.NewBusinessQuote
        Dim QuoteVersion As String = SubQuoteFirst.VersionId
        Dim QuoteEffectiveDate As String = quote.EffectiveDate

        Select Case VersionType
            Case VersionTypeToTest.SubquoteFirstVersionId
                QuoteVersion = SubQuoteFirst.VersionId
            Case VersionTypeToTest.SubquoteFirstRatingVersionId
                QuoteVersion = SubQuoteFirst.RatingVersionId
            Case VersionTypeToTest.CppPackagePartForCpr
                Dim CprPackageParts = qqh.PackagePartForLobType(quote.PackageParts, QuickQuoteObject.QuickQuoteLobType.CommercialProperty)
                If CprPackageParts Is Nothing Then
                    CprPackageParts = qqh.PackagePartForLobType(quote.TopLevelQuoteInfo.OriginalPackageParts, QuickQuoteObject.QuickQuoteLobType.CommercialProperty)
                End If
                If CprPackageParts IsNot Nothing Then
                    QuoteVersion = CprPackageParts.VersionId
                End If
        End Select

        If FlagClass.EnabledFlag AndAlso FlagClass.OtherQualifiers AndAlso qqh.IsDateString(QuoteEffectiveDate) Then
            If isNewBusiness Then
                If QuoteEffectiveDate.ToDateTime >= FlagClass.StartDate Then Return True
            Else 'Is Endorsement
                If FlagClass.HasVersionNumber Then
                    If QuoteVersion.TryToGetInt32 >= FlagClass.VersionNumber Then Return True
                ElseIf FlagClass.HasRenewalDate Then
                    If qqh.IsPositiveIntegerString(quote.RenewalVersion) AndAlso quote.RenewalVersion.TryToGetInt32 > 1 Then
                        If QuoteEffectiveDate.ToDateTime >= FlagClass.RenewalDate Then Return True
                    Else
                        If QuoteEffectiveDate.ToDateTime >= FlagClass.StartDate Then Return True
                    End If
                Else
                    Return True
                End If
            End If
        End If
        Return False
    End Function

    Enum VersionTypeToTest
        SubquoteFirstVersionId
        SubquoteFirstRatingVersionId
        CppPackagePartForCpr
    End Enum
End Class
