Imports QuickQuote.CommonObjects
Imports IFM.PrimativeExtensions
Imports System.Configuration

Namespace IFM.VR.Common.Helpers.HOM
    Public Class FamilyCyberDateHelper
        Public Shared Function IsPostCyberDate(quote As QuickQuoteObject) As Boolean
            Dim CyberEffDate As DateTime = DateTime.MinValue
            Dim QuoteEffDate As DateTime = DateTime.MinValue
            If ConfigurationManager.AppSettings("VR_Home_Cyber_EffDate") IsNot Nothing AndAlso IsDate(ConfigurationManager.AppSettings("VR_Home_Cyber_EffDate")) Then CyberEffDate = CDate(ConfigurationManager.AppSettings("VR_Home_Cyber_EffDate"))
            If quote IsNot Nothing AndAlso quote.EffectiveDate IsNot Nothing AndAlso IsDate(quote.EffectiveDate) Then QuoteEffDate = CDate(quote.EffectiveDate)
            If QuoteEffDate <> DateTime.MinValue AndAlso CyberEffDate <> DateTime.MinValue AndAlso QuoteEffDate >= CyberEffDate Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Shared Function IsPreCyberDate(quote As QuickQuoteObject) As Boolean
            Dim CyberEffDate As DateTime = DateTime.MinValue
            Dim QuoteEffDate As DateTime = DateTime.MinValue
            If ConfigurationManager.AppSettings("VR_Home_Cyber_EffDate") IsNot Nothing AndAlso IsDate(ConfigurationManager.AppSettings("VR_Home_Cyber_EffDate")) Then CyberEffDate = CDate(ConfigurationManager.AppSettings("VR_Home_Cyber_EffDate"))
            If quote IsNot Nothing AndAlso quote.EffectiveDate IsNot Nothing AndAlso IsDate(quote.EffectiveDate) Then QuoteEffDate = CDate(quote.EffectiveDate)
            If QuoteEffDate <> DateTime.MinValue AndAlso CyberEffDate <> DateTime.MinValue AndAlso QuoteEffDate < CyberEffDate Then
                Return True
            Else
                Return False
            End If
        End Function
    End Class
End Namespace
