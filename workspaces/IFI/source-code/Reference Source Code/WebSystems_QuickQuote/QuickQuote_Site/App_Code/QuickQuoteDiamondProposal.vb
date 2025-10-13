Imports Microsoft.VisualBasic
Imports QuickQuote.CommonMethods 'added 3/29/2017

Namespace QuickQuote.CommonObjects
    <Serializable()>
    Public Class QuickQuoteDiamondProposal 'added 3/27/2017

        Dim qqHelper As New QuickQuote.CommonMethods.QuickQuoteHelperClass

        Public Property DiamondProposalId As Integer = 0
        Public Property DiamondProposalBinaryId As Integer = 0
        Public Property DiamondProposalBytes As Byte() = Nothing
        Public Property Images As List(Of QuickQuoteDiamondProposalImage) = Nothing
        Public Property InsertUserId As Integer = 0
        Public Property InsertDate As String = String.Empty
        Public Property UpdateUserId As Integer = 0
        Public Property UpdateDate As String = String.Empty
        Public Property InsertUsername As String = String.Empty 'added 3/28/2017
        Public Property UpdateUsername As String = String.Empty 'added 3/28/2017

        Public ReadOnly Property PolicyNumberString As String 'added 3/28/2017
            Get
                Dim strPolNums As String = ""

                If _Images IsNot Nothing AndAlso _Images.Count > 0 Then
                    For Each i As QuickQuoteDiamondProposalImage In _Images
                        If i IsNot Nothing AndAlso String.IsNullOrWhiteSpace(i.PolicyNumber) = False Then
                            strPolNums = qqHelper.appendText(strPolNums, i.PolicyNumber, ", ")
                        End If
                    Next
                End If

                Return strPolNums
            End Get
        End Property
        Public ReadOnly Property PolicyNumbers As List(Of String) 'added 3/29/2017
            Get
                Return QuickQuoteHelperClass.ListOfStringFromString(PolicyNumberString, ", ")
            End Get
        End Property
        Public ReadOnly Property QuoteNumberString As String 'added 3/28/2017
            Get
                Dim strQuoteNums As String = ""

                If _Images IsNot Nothing AndAlso _Images.Count > 0 Then
                    For Each i As QuickQuoteDiamondProposalImage In _Images
                        If i IsNot Nothing AndAlso String.IsNullOrWhiteSpace(i.QuoteNumber) = False Then
                            strQuoteNums = qqHelper.appendText(strQuoteNums, i.QuoteNumber, ", ")
                        End If
                    Next
                End If

                Return strQuoteNums
            End Get
        End Property
        Public ReadOnly Property QuoteNumbers As List(Of String) 'added 3/29/2017
            Get
                Return QuickQuoteHelperClass.ListOfStringFromString(QuoteNumberString, ", ")
            End Get
        End Property
        Public ReadOnly Property AgencyIdString As String 'added 3/29/2017
            Get
                Dim strAgIds As String = ""

                If _Images IsNot Nothing AndAlso _Images.Count > 0 Then
                    For Each i As QuickQuoteDiamondProposalImage In _Images
                        If i IsNot Nothing AndAlso i.AgencyId > 0 Then
                            strAgIds = qqHelper.appendText(strAgIds, i.AgencyId.ToString, ", ")
                        End If
                    Next
                End If

                Return strAgIds
            End Get
        End Property
        Public ReadOnly Property AgencyIds As List(Of Integer) 'added 3/29/2017
            Get
                Return QuickQuoteHelperClass.ListOfIntegerFromString(AgencyIdString, ", ")
            End Get
        End Property
        Public ReadOnly Property AgencyCodeString As String 'added 3/29/2017
            Get
                Dim strAgCodes As String = ""

                If _Images IsNot Nothing AndAlso _Images.Count > 0 Then
                    For Each i As QuickQuoteDiamondProposalImage In _Images
                        If i IsNot Nothing AndAlso String.IsNullOrWhiteSpace(i.AgencyCode) = False Then
                            strAgCodes = qqHelper.appendText(strAgCodes, i.AgencyCode, ", ")
                        End If
                    Next
                End If

                Return strAgCodes
            End Get
        End Property
        Public ReadOnly Property AgencyCodes As List(Of String) 'added 3/29/2017
            Get
                Return QuickQuoteHelperClass.ListOfStringFromString(AgencyCodeString, ", ")
            End Get
        End Property

        'added 4/22/2017
        Private _TotalQuotedPremium As String = String.Empty
        Public Property TotalQuotedPremium As String
            Get
                Return qqHelper.QuotedPremiumFormat(_TotalQuotedPremium)
            End Get
            Set(value As String)
                _TotalQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_TotalQuotedPremium)
            End Set
        End Property

        Public Sub Reset()
            _DiamondProposalId = 0
            _DiamondProposalBinaryId = 0
            _DiamondProposalBytes = Nothing
            _Images = Nothing
            _InsertUserId = 0
            _InsertDate = String.Empty
            _UpdateUserId = 0
            _UpdateDate = String.Empty
            _InsertUsername = String.Empty 'added 3/28/2017
            _UpdateUsername = String.Empty 'added 3/28/2017

            _TotalQuotedPremium = String.Empty 'added 4/22/2017
        End Sub
        Public Sub Dispose()
            _DiamondProposalId = Nothing
            _DiamondProposalBinaryId = Nothing
            If _DiamondProposalBytes IsNot Nothing Then
                _DiamondProposalBytes = Nothing
            End If
            If _Images IsNot Nothing Then
                If _Images.Count > 0 Then
                    For Each i As QuickQuoteDiamondProposalImage In _Images
                        If i IsNot Nothing Then
                            i.Dispose()
                            i = Nothing
                        End If
                    Next
                    _Images.Clear()
                End If
                _Images = Nothing
            End If
            _InsertUserId = Nothing
            qqHelper.DisposeString(_InsertDate)
            _UpdateUserId = Nothing
            qqHelper.DisposeString(_UpdateDate)
            qqHelper.DisposeString(_InsertUsername) 'added 3/28/2017
            qqHelper.DisposeString(_UpdateUsername) 'added 3/28/2017

            qqHelper.DisposeString(_TotalQuotedPremium) 'added 4/22/2017
        End Sub
    End Class
End Namespace
