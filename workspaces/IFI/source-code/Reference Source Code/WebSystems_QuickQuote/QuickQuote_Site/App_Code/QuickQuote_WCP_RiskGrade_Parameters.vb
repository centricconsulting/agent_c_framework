Imports Microsoft.VisualBasic

Namespace QuickQuote.CommonObjects 'added namespace 8/19/2017
    <Serializable()>
    Public Class QuickQuote_WCP_RiskGrade_Parameters 'added 8/17/2017
        Public Property OriginalRiskGradeLookupId As String = String.Empty
        Public Property CurrentRiskGradeLookupId As String = String.Empty
        Public Property ClientDateBusinessStarted As String = String.Empty
        Public Property OriginalRiskGradeLookupId_isSpecialFarmOperationsRiskGrade As Boolean = False
        Public Property CurrentRiskGradeLookupId_isSpecialFarmOperationsRiskGrade As Boolean = False
        Public Property OriginalRiskGradeLookupId_isSpecialMeatOperationsRiskGrade As Boolean = False
        Public Property CurrentRiskGradeLookupId_isSpecialMeatOperationsRiskGrade As Boolean = False
        Public Property OriginalRiskGradeLookupId_isSpecialRpgContractorRiskGrade As Boolean = False
        Public Property CurrentRiskGradeLookupId_isSpecialRpgContractorRiskGrade As Boolean = False
        Public Property HasFivePercentIrpmDebitOnClassificationPeculiarities As Boolean = False
        Public Property DateBusStartedIsLessThanOneYear As Boolean = False
        Public Property DateBusStartedIsLessThanTwoYears As Boolean = False
        Public Property DateBusStartedIsAtLeastTwoYears As Boolean = False
    End Class
End Namespace
