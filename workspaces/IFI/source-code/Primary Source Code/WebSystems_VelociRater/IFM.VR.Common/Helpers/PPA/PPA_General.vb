Imports IFM.VR.Common.Helpers.MultiState

Namespace IFM.VR.Common.Helpers.PPA
    Public Class PPA_General

        Public Shared Function IsParachuteQuote(topQuote As QuickQuote.CommonObjects.QuickQuoteObject) As Boolean
            If topQuote.QuoteLevel <> QuickQuote.CommonMethods.QuickQuoteHelperClass.QuoteLevel.TopLevel Then
                Throw New NotTopLevelQuoteException()
            End If
            If IsDate(topQuote.EffectiveDate) Then
                If (CDate(topQuote.EffectiveDate) >= Date.Parse(System.Configuration.ConfigurationManager.AppSettings("parachute_EffectiveDate"))) Then
                    'Return True
                    'updated 6/20/2019 for ReadOnly/Endorsements
                    If topQuote.QuoteTransactionType <> QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteTransactionType.ReadOnlyImage AndAlso topQuote.QuoteTransactionType <> QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote Then
                        'New Business quoting
                        Return True
                    Else
                        'ReadOnly or Endorsement
                        Dim qqHelper As New QuickQuote.CommonMethods.QuickQuoteHelperClass
                        Dim versionId As Integer = qqHelper.IntegerForString(topQuote.VersionId)
                        If versionId >= 128 Then 'initial Parachute version
                            Return True
                        End If
                        'try to handle for Parachute renewalEffDate (1 month after newBus)
                        If CDate(topQuote.EffectiveDate) >= DateAdd(DateInterval.Month, 1, qqHelper.DateForString(QuickQuote.CommonMethods.QuickQuoteHelperClass.configAppSettingValueAsString("parachute_EffectiveDate"))) Then
                            Return True
                        End If
                    End If
                End If
            End If
            Return False
        End Function

    End Class
End Namespace