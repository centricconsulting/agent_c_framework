Imports Microsoft.VisualBasic

Namespace QuickQuote.CommonObjects
    <Serializable()>
    Public Class QuickQuoteEndorsementForPolicyIdAndTransactionDateInput
        Public Property PolicyId As Integer
        Public Property TransactionDate As String
        Public Property TransactionReasonId As Integer
        Public Property EndorsementRemarks As String
        Public Property PolicyImageNum As Integer
        Public Property LatestPendingEndorsementImageNum As Integer
        Public Property LatestPendingEndorsementImageTranEffDate As String
        Public Property ValidateTransactionDate As Boolean
        Public Property DaysForward As Integer
        Public Property DaysBack As Integer
        Public Property DevDictionaryKeys As New List(Of QuickQuoteGenericObjectWithTwoStringProperties)
        Public Property ReturnExistingPendingQuickQuoteEndorsement As Boolean
        Public Property OnlyReturnPendingQuickQuoteEndorsementWhenDateMatches As Boolean
        Public Property ErrorMessage As String
        Public Property IsBillingUpdate As Boolean
        Public Property EndorsementOriginType As EndorsementOriginTypes = EndorsementOriginTypes.Velocirater
        Public Property TransactionSourceId As Diamond.Common.Enums.TransSource

        Public Property DelegateMethod As QuickQuote.CommonMethods.QuickQuoteXML.DelegateMethod

        Public Enum EndorsementOriginTypes
            NA = 0
            Velocirater = 1
            MemberPortal = 2
        End Enum
    End Class
End Namespace

