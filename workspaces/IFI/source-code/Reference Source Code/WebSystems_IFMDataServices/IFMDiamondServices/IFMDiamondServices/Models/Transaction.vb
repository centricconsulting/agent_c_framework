Imports Microsoft.VisualBasic
Namespace Models
    <Serializable>
        Public Class Transaction
        Property PolicyID As Integer
        Property PolicyImageNum As Integer
        Property TransactionEffectiveDate As Date
        Property TransactionReasonID As Integer
        Property TransactionSourceID As Integer
        Property TransactionTypeID As Integer
        Property TransactionUserID As Integer
        Property Remark As String

    End Class
End Namespace
