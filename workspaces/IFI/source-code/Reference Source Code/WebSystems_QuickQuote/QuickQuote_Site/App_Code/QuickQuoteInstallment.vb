Imports Microsoft.VisualBasic
Imports QuickQuote.CommonMethods

Namespace QuickQuote.CommonObjects
    <Serializable()>
    Public Class QuickQuoteInstallment 'added 8/26/2017

        Dim qqHelper As New QuickQuote.CommonMethods.QuickQuoteHelperClass

        Public Property Amount As String = String.Empty
        Public Property BillingInstallmentNum As String = String.Empty
        Public Property DueDate As String = String.Empty
        Public Property DueDays As String = String.Empty
        Public Property PolicyId As String = String.Empty
        Public Property PolicyImageNum As String = String.Empty
        Public Property RenewalVer As String = String.Empty
        Public Property RollDate As String = String.Empty
        Public Property ServiceChargeAmount As String = String.Empty
        Public Property StatusCode As String = String.Empty
        Public Property TaxAmount As String = String.Empty
        Public Property TaxRate As String = String.Empty
        Public Property UserSelected As Boolean = False

        Public Sub Reset()
            _Amount = String.Empty
            _BillingInstallmentNum = String.Empty
            _DueDate = String.Empty
            _DueDays = String.Empty
            _PolicyId = String.Empty
            _PolicyImageNum = String.Empty
            _RenewalVer = String.Empty
            _RollDate = String.Empty
            _ServiceChargeAmount = String.Empty
            _StatusCode = String.Empty
            _TaxAmount = String.Empty
            _TaxRate = String.Empty
            _UserSelected = False
        End Sub
        Public Sub Dispose()
            qqHelper.DisposeString(_Amount)
            qqHelper.DisposeString(_BillingInstallmentNum)
            qqHelper.DisposeString(_DueDate)
            qqHelper.DisposeString(_DueDays)
            qqHelper.DisposeString(_PolicyId)
            qqHelper.DisposeString(_PolicyImageNum)
            qqHelper.DisposeString(_RenewalVer)
            qqHelper.DisposeString(_RollDate)
            qqHelper.DisposeString(_ServiceChargeAmount)
            qqHelper.DisposeString(_StatusCode)
            qqHelper.DisposeString(_TaxAmount)
            qqHelper.DisposeString(_TaxRate)
            _UserSelected = Nothing
        End Sub

    End Class
End Namespace
