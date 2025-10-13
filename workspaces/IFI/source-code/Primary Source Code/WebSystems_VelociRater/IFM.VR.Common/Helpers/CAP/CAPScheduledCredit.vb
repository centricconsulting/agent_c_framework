Imports QuickQuote.CommonObjects
Imports QuickQuote.CommonMethods
Imports IFM.PrimativeExtensions
Imports System.Web.UI.WebControls
Namespace IFM.VR.Common.Helpers.CAP
    'Added 6/28/2022 for task 75037 MLW
    Public Class CAPScheduledCredit

        Private Shared _CAPScheduledCredit As NewFlagItem
        Public Shared ReadOnly Property CAPScheduledCredit() As NewFlagItem
            Get
                If _CAPScheduledCredit Is Nothing Then
                    _CAPScheduledCredit = New NewFlagItem("VR_CAP_ScheduledCredit")
                End If
                Return _CAPScheduledCredit
            End Get
        End Property

        Const CAPScheduledCreditWarningMsg As String = ""
        Public Shared Function CAPScheduledCreditEnabled() As Boolean
            Return CAPScheduledCredit.EnabledFlag
        End Function

        Public Shared Function CAPScheduledCreditEffDate() As Date
            Return CAPScheduledCredit.GetStartDateOrDefault("1/1/1800")
        End Function

        Public Shared Sub UpdateCAPScheduledCredit(ByRef Quote As QuickQuoteObject, ByRef CrossDirection As Helper.EnumsHelper.CrossDirectionEnum, Optional ByRef ValidationErrors As List(Of WebValidationItem) = Nothing)
            Select Case CrossDirection
                Case Helper.EnumsHelper.CrossDirectionEnum.FORWARD
                    'Allow coverage                   
                Case Helper.EnumsHelper.CrossDirectionEnum.BACK
                    'Remove coverage
            End Select
        End Sub

        Public Shared Function IsCAPScheduledCreditAvailable(quote As QuickQuoteObject) As Boolean
            If quote IsNot Nothing Then
                CAPScheduledCredit.OtherQualifiers = quote.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialAuto
                Return AvailabilityByDateOrVersion.AvailabilityByDateOrVersion(quote, CAPScheduledCredit, AvailabilityByDateOrVersion.VersionTypeToTest.SubquoteFirstVersionId)
            End If
            Return False
        End Function
    End Class

End Namespace