Imports IFM.VR.Common.Helpers.MultiState
Imports IFM.PrimativeExtensions
Imports QuickQuote.CommonObjects

Namespace IFM.VR.Common.Helpers.HOM
    Public Class HOM_General
        Public Shared Function IsHomMortgageFreeEligable(topQuote As QuickQuote.CommonObjects.QuickQuoteObject) As Boolean
            Dim checkDate As Date
            Dim qqHelper As New QuickQuote.CommonMethods.QuickQuoteHelperClass
            If topQuote.QuoteLevel <> QuickQuote.CommonMethods.QuickQuoteHelperClass.QuoteLevel.TopLevel Then
                Throw New NotTopLevelQuoteException()
            End If
            If topQuote.Locations(0).FormTypeId.EqualsAny("22", "23", "24") AndAlso topQuote.Locations(0).StructureTypeId <> "2" Then
                If IsDate(topQuote.EffectiveDate) AndAlso Date.TryParse(System.Configuration.ConfigurationManager.AppSettings("MortgageFreeUpgrade_EffectiveDate"), checkDate) Then
                    If (CDate(topQuote.EffectiveDate) >= checkDate) Then
                        'Return True
                        If topQuote.QuoteTransactionType <> QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteTransactionType.ReadOnlyImage AndAlso topQuote.QuoteTransactionType <> QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote Then
                            'New Business quoting
                            Return True
                        Else
                            'ReadOnly or Endorsement
                            Dim versionId As Integer = qqHelper.IntegerForString(topQuote.VersionId)
                            If versionId > 175 OrElse (versionId = 175 AndAlso qqHelper.IntegerForString(topQuote.RatingVersionId) >= 384) Then ' need HOM version?
                                Return True
                            End If
                            'try to handle for MortgageFree renewalEffDate (1 month after newBus)
                            If CDate(topQuote.EffectiveDate) >= DateAdd(DateInterval.Month, 1, qqHelper.DateForString(QuickQuote.CommonMethods.QuickQuoteHelperClass.configAppSettingValueAsString("HOM_MortgageFreeEligible_EffectiveDate"))) Then
                                Return True
                            End If
                        End If
                    End If
                End If
            End If
            Return False
        End Function

        'Added 4/29/2022 for tasks 74106, 74136 and 74145 MLW
        Public Shared Function HPEEWaterBUEnabled() As Boolean
            Dim chc As New CommonHelperClass       
            Return chc.ConfigurationAppSettingValueAsBoolean("VR_HOM_HPEE_WaterBU_Enabled")
            End Function

        'Added 4/29/2022 for tasks 74106, 74136 and 74145 MLW
        Public Shared Function HPEEWaterBUEffDate() As Date
            Dim QQHelper As New QuickQuote.CommonMethods.QuickQuoteHelperClass
            If System.Configuration.ConfigurationManager.AppSettings("VR_HOM_HPEE_WaterBU_EffectiveDate") IsNot Nothing AndAlso
                QQHelper.IsValidDateString(System.Configuration.ConfigurationManager.AppSettings("VR_HOM_HPEE_WaterBU_EffectiveDate").ToString) Then
                Return CDate(System.Configuration.ConfigurationManager.AppSettings("VR_HOM_HPEE_WaterBU_EffectiveDate").ToString)
            End If
            Return CDate("9/1/2022")
        End Function

        'Added 5/27/2022 for tasks 74136 and 74145 MLW, Updated 6/13/2022 for task 74187 MLW
        Public Shared Function OkayForHPEEWaterBU(quote As QuickQuoteObject) As Boolean
            Dim QQHelper As New QuickQuote.CommonMethods.QuickQuoteHelperClass            
            If quote IsNot Nothing AndAlso QQHelper.IsValidDateString(quote.EffectiveDate) Then
                Dim effectiveDate As Date = CDate(quote.EffectiveDate)
                If HPEEWaterBUEnabled() AndAlso (effectiveDate >= HPEEWaterBUEffDate() OrElse HOM_General.HPEEWaterBackupValidForEndorsements(quote.QuoteTransactionType, QQHelper.IntegerForString(quote.RatingVersionId))) Then
                    Return True
                Else
                    Return False
                End If
            Else
                Return False
            End If
        End Function

        'Added 6/9/2022 for task 74187 MLW
        Public Shared Function HPEEWaterBackupValidForEndorsements(TranType As QuickQuoteObject.QuickQuoteTransactionType, RatingVersionId As Integer) As Boolean
            If (TranType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote OrElse TranType = QuickQuoteObject.QuickQuoteTransactionType.ReadOnlyImage) AndAlso RatingVersionId >= 482 Then        
                Return True
            Else
                Return false
            End If
        End Function

    End Class

End Namespace
