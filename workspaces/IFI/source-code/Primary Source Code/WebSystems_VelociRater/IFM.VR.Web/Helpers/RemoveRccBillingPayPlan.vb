Imports QuickQuote.CommonMethods

Public Class RemoveRccBillingPayPlan
    Public Function RemoveRCCBillingPayPlan() As Boolean
        Dim hideRccBillingPayPlan As Boolean = False
        Dim qqHelper As New QuickQuoteHelperClass
        Dim strRemoveRccBillingPayPlan As String = QuickQuoteHelperClass.configAppSettingValueAsString("RemoveRCCBillingPayPlanCalendarDateKey")
        If qqHelper.IsValidDateString(strRemoveRccBillingPayPlan, mustBeGreaterThanDefaultDate:=True) = True AndAlso Date.Today >= CDate(strRemoveRccBillingPayPlan) Then
            hideRccBillingPayPlan = True
        End If
        Return hideRccBillingPayPlan
    End Function

End Class
