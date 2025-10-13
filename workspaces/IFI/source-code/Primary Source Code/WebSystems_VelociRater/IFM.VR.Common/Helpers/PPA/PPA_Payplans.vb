
Imports IFM.VR.Common.Helpers.MultiState
Imports QuickQuote.CommonMethods.QuickQuoteHelperClass
Imports QuickQuote.CommonObjects

Namespace IFM.VR.Common.Helpers.PPA
    Public Class PPA_Payplans

        Public Shared Function GetPayplanFactors(IsParachute As Boolean, qqState As QuickQuoteState) As Dictionary(Of Int32, Double)
            ' for DirectBill only -  12 = annual 2, 13 = semi-annual 2, 14 = quarterly 2, 15 = monthly 2,18 = renewal credit card monthly 2, 19 = renewal eft monthly 2, 
            'If IsParachute Then
            '    Return New Dictionary(Of Int32, Double) From {{12, 0.93}, {13, 0.97}, {14, 1.0}, {15, 1.05}, {18, 0.98}, {19, 0.96}, {20, 0.93}, {21, 0.97}, {22, 1.0}}
            'Else
            '    Return New Dictionary(Of Int32, Double) From {{12, 1}, {13, 1}, {14, 1.0}, {15, 1}, {18, 1}, {19, 1}, {20, 1}, {21, 1}, {22, 1}}
            'End If
            'updated 9/26/2021 for missing payplans (1=Annual [Direct Bill], 3=Quarterly [Direct Bill], 23=Annual MTG [Direct Bill], 24=Account Bill Monthly [Direct Bill]
            ', 25=Account Bill Credit Card Monthly [Direct Bill], 26=Account Bill EFT Monthly [Direct Bill], 27=Annual [Direct Bill], 28=Semi-Annual [Direct Bill], 29=Quarterly [Direct Bill]
            ', 30=Monthly [Direct Bill], 31=Renewal Credit Card Monthly [Direct Bill], 32=Renewal EFT Monthly [Direct Bill], 33=Annual [Agency Bill], 34=Semi Annual [Agency Bill]
            ', 35=Quarterly [Agency Bill], 36=Annual MTG [Direct Bill], 37=Account Bill Monthly [Direct Bill], 38=Account Bill Credit Card Monthly [Direct Bill], 39=Account Bill EFT Monthly [Direct Bill])
            '9/26/2021 note: making assumptions for the Factor based on existing ones
            '9/29/2021 note: would need additional updates as new payplans are added to Diamond
            '6/27/2025 updated rates, new pay plans added 62=Annual [Direct Bill], 63=Semi Annual [Direct Bill], 64=Annual MTG [Direct Bill], 65=Quarterly [Direct Bill], 66=Monthly [Direct Bill]
            If IsParachute Then
                'Return New Dictionary(Of Int32, Double) From {{1, 0.93}, {3, 1.0}, {12, 0.93}, {13, 0.97}, {14, 1.0}, {15, 1.05}, {18, 0.98}, {19, 0.96}, {20, 0.93}, {21, 0.97}, {22, 1.0}, {23, 0.93}, {24, 1.05}, {25, 0.98}, {26, 0.96}, {27, 0.93}, {28, 0.97}, {29, 1.0}, {30, 1.05}, {31, 0.98}, {32, 0.96}, {33, 0.93}, {34, 0.97}, {35, 1.0}, {36, 0.93}, {37, 1.05}, {38, 0.98}, {39, 0.96}}
                If qqState = QuickQuoteState.Ohio Then
                    Return New Dictionary(Of Int32, Double) From {{1, 0.93}, {3, 1.0}, {12, 0.93}, {13, 0.97}, {14, 1.0}, {15, 1.05}, {18, 0.98}, {19, 0.96}, {20, 0.93}, {21, 0.97}, {22, 1.0}, {23, 0.93}, {24, 1.05}, {25, 0.98}, {26, 0.96}, {27, 0.93}, {28, 0.97}, {29, 1.0}, {30, 1.05}, {31, 0.98}, {32, 0.96}, {33, 0.93}, {34, 0.97}, {35, 1.0}, {36, 0.93}, {37, 1.05}, {38, 0.98}, {39, 0.96}, {62, 0.93}, {63, 0.97}, {64, 0.93}, {65, 1.0}, {66, 1.05}}
                Else
                    Return New Dictionary(Of Int32, Double) From {{1, 0.94}, {3, 1.0}, {12, 0.94}, {13, 0.95}, {14, 1.0}, {15, 1.05}, {18, 0.98}, {19, 0.95}, {20, 0.94}, {21, 0.95}, {22, 1.0}, {23, 0.94}, {24, 1.05}, {25, 0.98}, {26, 0.95}, {27, 0.94}, {28, 0.95}, {29, 1.0}, {30, 1.05}, {31, 0.98}, {32, 0.95}, {33, 0.94}, {34, 0.95}, {35, 1.0}, {36, 0.94}, {37, 1.05}, {38, 0.98}, {39, 0.95}, {62, 0.94}, {63, 0.95}, {64, 0.94}, {65, 1.0}, {66, 1.05}}
                End If
            Else
                Return New Dictionary(Of Int32, Double) From {{1, 1}, {3, 1.0}, {12, 1}, {13, 1}, {14, 1.0}, {15, 1}, {18, 1}, {19, 1}, {20, 1}, {21, 1}, {22, 1}, {23, 1}, {24, 1}, {25, 1}, {26, 1}, {27, 1}, {28, 1}, {29, 1.0}, {30, 1}, {31, 1}, {32, 1}, {33, 1}, {34, 1}, {35, 1}, {36, 1}, {37, 1}, {38, 1}, {39, 1}}
            End If

        End Function

        Public Shared Function GetAllPaymentOptions(topQuote As QuickQuote.CommonObjects.QuickQuoteObject) As IEnumerable(Of PayPlanOptions)
            If topQuote.QuoteLevel <> QuickQuote.CommonMethods.QuickQuoteHelperClass.QuoteLevel.TopLevel Then
                Throw New NotTopLevelQuoteException()
            End If

            'Added 09/26/2019 for bug 40515 MLW
            Dim quotePayPlanId As Int32 = topQuote.BillingPayPlanId
            If (topQuote.QuoteTransactionType = QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote OrElse topQuote.QuoteTransactionType = QuickQuoteObject.QuickQuoteTransactionType.ReadOnlyImage) Then
                quotePayPlanId = topQuote.CurrentPayplanId
            End If
            'Update 09/26/2019 for bug 40515 MLW
            Dim options As New List(Of PayPlanOptions)
            options = GetPaymentOptions(quotePayPlanId, topQuote.TotalQuotedPremium, Helpers.PPA.PPA_General.IsParachuteQuote(topQuote), topQuote.QuickQuoteState)
            options.AddRange(GetAgencyBillPaymentOptions(quotePayPlanId, topQuote.TotalQuotedPremium, Helpers.PPA.PPA_General.IsParachuteQuote(topQuote), topQuote.QuickQuoteState))

            Return options
        End Function

        Public Shared Function GetPaymentOptionsBasedOnBillMethod(topQuote As QuickQuote.CommonObjects.QuickQuoteObject) As IEnumerable(Of PayPlanOptions)
            If topQuote.BillMethodId = "1" Then
                Return GetAgencyBillPaymentOptions(topQuote)
            Else
                Return GetPaymentOptions(topQuote)
            End If
        End Function

        Public Shared Function GetPaymentOptions(topQuote As QuickQuote.CommonObjects.QuickQuoteObject) As IEnumerable(Of PayPlanOptions)
            If topQuote.QuoteLevel <> QuickQuote.CommonMethods.QuickQuoteHelperClass.QuoteLevel.TopLevel Then
                Throw New NotTopLevelQuoteException()
            End If
            'Added 09/26/2019 for bug 40515 MLW
            Dim quotePayPlanId As Int32 = topQuote.BillingPayPlanId
            If (topQuote.QuoteTransactionType = QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote OrElse topQuote.QuoteTransactionType = QuickQuoteObject.QuickQuoteTransactionType.ReadOnlyImage) Then
                quotePayPlanId = topQuote.CurrentPayplanId
            End If
            'Updated 09/26/2019 for bug 40515 MLW
            Return GetPaymentOptions(quotePayPlanId, topQuote.TotalQuotedPremium, Helpers.PPA.PPA_General.IsParachuteQuote(topQuote), topQuote.QuickQuoteState)
        End Function

        ''' <summary>        ''' 
        ''' There is an assumption here that Quarterly is always rate neutral 1.0 and that the quote was rated with a quarterly payplan
        ''' </summary>
        ''' <param name="TotalQuotedPremium"></param>
        ''' <returns></returns>
        Public Shared Function GetPaymentOptions(RatedWithPayPlanId As Int32, TotalQuotedPremium As Double, IsParachute As Boolean, qqState As QuickQuoteState) As IEnumerable(Of PayPlanOptions)
            Dim options As New List(Of PayPlanOptions)
            If TotalQuotedPremium > 0 Then
                Dim factors As Dictionary(Of Int32, Double) = GetPayplanFactors(IsParachute, qqState)
                Dim qqHelper As New QuickQuote.CommonMethods.QuickQuoteHelperClass

                '9/29/2021 note: may need to be updated if we ever start loading payplans based on effDate (would rely on VR_Default_PayPlanIds and VR_ConvertPayPlanIdsIfNeeded config keys until then)

                'If RatedWithPayPlanId = 12 Then
                'updated 9/26/2021 for missing payplans (1=Annual, 23=Annual MTG, 27=Annual, 36=Annual MTG)
                If RatedWithPayPlanId = 12 OrElse RatedWithPayPlanId = 1 OrElse RatedWithPayPlanId = 23 OrElse RatedWithPayPlanId = 27 OrElse RatedWithPayPlanId = 36 Then
                    options.Add(New PayPlanOptions() With {.PayPlanId = 12, .DownPayment = TotalQuotedPremium, .TotalPayments = TotalQuotedPremium})
                Else
                    options.Add(New PayPlanOptions() With {.PayPlanId = 12, .DownPayment = (TotalQuotedPremium / factors(RatedWithPayPlanId)) * factors(12), .TotalPayments = (TotalQuotedPremium / factors(RatedWithPayPlanId)) * factors(12)})
                End If

                'If RatedWithPayPlanId = 13 Then
                'updated 9/26/2021 for missing payplans (28=Semi-Annual)
                If RatedWithPayPlanId = 13 OrElse RatedWithPayPlanId = 28 Then
                    options.Add(New PayPlanOptions() With {.PayPlanId = 13, .DownPayment = qqHelper.getDivisionQuotient(TotalQuotedPremium, "2"), .TotalPayments = TotalQuotedPremium})
                Else
                    options.Add(New PayPlanOptions() With {.PayPlanId = 13, .DownPayment = qqHelper.getDivisionQuotient((TotalQuotedPremium / factors(RatedWithPayPlanId)) * factors(13), "2"), .TotalPayments = (TotalQuotedPremium / factors(RatedWithPayPlanId)) * factors(13)})
                End If

                'If RatedWithPayPlanId = 14 Then
                'updated 9/26/2021 for missing payplans (3=Quarterly, 29=Quarterly)
                If RatedWithPayPlanId = 14 OrElse RatedWithPayPlanId = 3 OrElse RatedWithPayPlanId = 29 Then
                    options.Add(New PayPlanOptions() With {.PayPlanId = 14, .DownPayment = qqHelper.getDivisionQuotient(TotalQuotedPremium, "4"), .TotalPayments = (TotalQuotedPremium / factors(RatedWithPayPlanId)) * factors(14)})
                Else
                    options.Add(New PayPlanOptions() With {.PayPlanId = 14, .DownPayment = qqHelper.getDivisionQuotient((TotalQuotedPremium / factors(RatedWithPayPlanId)) * factors(14), "4"), .TotalPayments = (TotalQuotedPremium / factors(RatedWithPayPlanId)) * factors(14)})
                End If

                'If RatedWithPayPlanId = 15 Then
                'updated 9/26/2021 for missing payplans (24=Account Bill Monthly, 30=Monthly, 37=Account Bill Monthly)
                If RatedWithPayPlanId = 15 OrElse RatedWithPayPlanId = 24 OrElse RatedWithPayPlanId = 30 OrElse RatedWithPayPlanId = 37 Then
                    options.Add(New PayPlanOptions() With {.PayPlanId = 15, .DownPayment = qqHelper.getDivisionQuotient(TotalQuotedPremium, "12"), .TotalPayments = TotalQuotedPremium})
                Else
                    options.Add(New PayPlanOptions() With {.PayPlanId = 15, .DownPayment = qqHelper.getDivisionQuotient((TotalQuotedPremium / factors(RatedWithPayPlanId)) * factors(15), "12"), .TotalPayments = ((TotalQuotedPremium / factors(RatedWithPayPlanId)) * factors(15))}) ' + (3 * 11)})
                End If

                'If RatedWithPayPlanId = 18 Then
                'updated 9/26/2021 for missing payplans (25=Account Bill Credit Card Monthly, 31=Renewal Credit Card Monthly, 38=Account Bill Credit Card Monthly)
                If RatedWithPayPlanId = 18 OrElse RatedWithPayPlanId = 25 OrElse RatedWithPayPlanId = 31 OrElse RatedWithPayPlanId = 38 Then
                    options.Add(New PayPlanOptions() With {.PayPlanId = 18, .DownPayment = qqHelper.getDivisionQuotient(TotalQuotedPremium, "12"), .TotalPayments = TotalQuotedPremium})
                Else
                    options.Add(New PayPlanOptions() With {.PayPlanId = 18, .DownPayment = qqHelper.getDivisionQuotient((TotalQuotedPremium / factors(RatedWithPayPlanId)) * factors(18), "12"), .TotalPayments = (TotalQuotedPremium / factors(RatedWithPayPlanId)) * factors(18)})
                End If

                'If RatedWithPayPlanId = 19 Then
                'updated 9/26/2021 for missing payplans (26=Account Bill EFT Monthly, 32=Renewal EFT Monthly, 39=Account Bill EFT Monthly)
                If RatedWithPayPlanId = 19 OrElse RatedWithPayPlanId = 26 OrElse RatedWithPayPlanId = 32 OrElse RatedWithPayPlanId = 39 Then
                    options.Add(New PayPlanOptions() With {.PayPlanId = 19, .DownPayment = qqHelper.getDivisionQuotient(TotalQuotedPremium, "12"), .TotalPayments = TotalQuotedPremium})
                Else
                    options.Add(New PayPlanOptions() With {.PayPlanId = 19, .DownPayment = qqHelper.getDivisionQuotient((TotalQuotedPremium / factors(RatedWithPayPlanId)) * factors(19), "12"), .TotalPayments = (TotalQuotedPremium / factors(RatedWithPayPlanId)) * factors(19)})
                End If

                For Each op In options
                    op.DownPayment = Math.Ceiling(op.DownPayment)
                    op.TotalPayments = Math.Ceiling(op.TotalPayments)
                Next

            End If
            Return options
        End Function

        Public Shared Function GetAgencyBillPaymentOptions(topQuote As QuickQuote.CommonObjects.QuickQuoteObject) As IEnumerable(Of PayPlanOptions)
            If topQuote.QuoteLevel <> QuickQuote.CommonMethods.QuickQuoteHelperClass.QuoteLevel.TopLevel Then
                Throw New NotTopLevelQuoteException()
            End If
            'Added 09/26/2019 for bug 40515 MLW
            Dim quotePayPlanId As Int32 = topQuote.BillingPayPlanId
            If (topQuote.QuoteTransactionType = QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote OrElse topQuote.QuoteTransactionType = QuickQuoteObject.QuickQuoteTransactionType.ReadOnlyImage) Then
                quotePayPlanId = topQuote.CurrentPayplanId
            End If
            'Update 09/26/2019 for bug 40515 MLW
            Return GetAgencyBillPaymentOptions(quotePayPlanId, topQuote.TotalQuotedPremium, Helpers.PPA.PPA_General.IsParachuteQuote(topQuote), topQuote.QuickQuoteState)
        End Function

        Public Shared Function GetAgencyBillPaymentOptions(RatedWithPayPlanId As Int32, TotalQuotedPremium As Double, IsParachute As Boolean, qqState As QuickQuoteState) As IEnumerable(Of PayPlanOptions)
            Dim options As New List(Of PayPlanOptions)
            If TotalQuotedPremium > 0 Then
                Dim factors As Dictionary(Of Int32, Double) = GetPayplanFactors(IsParachute, qqState)
                Dim qqHelper As New QuickQuote.CommonMethods.QuickQuoteHelperClass

                '9/29/2021 note: may need to be updated if we ever start loading payplans based on effDate (would rely on VR_Default_PayPlanIds and VR_ConvertPayPlanIdsIfNeeded config keys until then)

                'If RatedWithPayPlanId = 20 Then
                'updated 9/26/2021 for missing payplans (33=Annual)
                If RatedWithPayPlanId = 20 OrElse RatedWithPayPlanId = 33 Then
                    options.Add(New PayPlanOptions() With {.PayPlanId = 20, .DownPayment = TotalQuotedPremium, .TotalPayments = TotalQuotedPremium})
                Else
                    options.Add(New PayPlanOptions() With {.PayPlanId = 20, .DownPayment = (TotalQuotedPremium / factors(RatedWithPayPlanId)) * factors(20), .TotalPayments = (TotalQuotedPremium / factors(RatedWithPayPlanId)) * factors(20)})
                End If

                'If RatedWithPayPlanId = 21 Then
                'updated 9/26/2021 for missing payplans (34=Semi Annual)
                If RatedWithPayPlanId = 21 OrElse RatedWithPayPlanId = 34 Then
                    options.Add(New PayPlanOptions() With {.PayPlanId = 21, .DownPayment = qqHelper.getDivisionQuotient(TotalQuotedPremium, "2"), .TotalPayments = TotalQuotedPremium})
                Else
                    options.Add(New PayPlanOptions() With {.PayPlanId = 21, .DownPayment = qqHelper.getDivisionQuotient((TotalQuotedPremium / factors(RatedWithPayPlanId)) * factors(21), "2"), .TotalPayments = (TotalQuotedPremium / factors(RatedWithPayPlanId)) * factors(21)})
                End If

                'If RatedWithPayPlanId = 22 Then
                'updated 9/26/2021 for missing payplans (35=Quarterly)
                If RatedWithPayPlanId = 22 OrElse RatedWithPayPlanId = 35 Then
                    options.Add(New PayPlanOptions() With {.PayPlanId = 22, .DownPayment = qqHelper.getDivisionQuotient(TotalQuotedPremium, "4"), .TotalPayments = (TotalQuotedPremium / factors(RatedWithPayPlanId)) * factors(22)})
                Else
                    options.Add(New PayPlanOptions() With {.PayPlanId = 22, .DownPayment = qqHelper.getDivisionQuotient((TotalQuotedPremium / factors(RatedWithPayPlanId)) * factors(22), "4"), .TotalPayments = (TotalQuotedPremium / factors(RatedWithPayPlanId)) * factors(22)})
                End If

                For Each op In options
                    op.DownPayment = Math.Ceiling(op.DownPayment)
                    op.TotalPayments = Math.Ceiling(op.TotalPayments)
                Next

            End If
            Return options
        End Function

        Public Shared Function GetCurrentPayPlanName(Quote As QuickQuote.CommonObjects.QuickQuoteObject, Optional RemoveExtraPlanNameNumber As Boolean = True) As String
            Dim returnVar As String = ""
            If Quote IsNot Nothing Then
                Dim qqhelper As New QuickQuote.CommonMethods.QuickQuoteHelperClass

                'Added 09/26/2019 for bug 40515 MLW
                'Dim quotePayPlanId As Int32 = Quote.BillingPayPlanId
                'updated 1/5/2021 to just use a string since that's what the staticData call expects, and that's what the properties already are
                Dim quotePayPlanId As String = Quote.BillingPayPlanId
                If (Quote.QuoteTransactionType = QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote OrElse Quote.QuoteTransactionType = QuickQuoteObject.QuickQuoteTransactionType.ReadOnlyImage) Then
                    quotePayPlanId = Quote.CurrentPayplanId
                End If
                'Update 09/26/2019 for bug 40515 MLW
                returnVar = qqhelper.GetStaticDataTextForValue(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuotePropertyName.BillingPayPlanId, quotePayPlanId, QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.AutoPersonal, QuickQuote.CommonMethods.QuickQuoteHelperClass.PersOrComm.Pers)
                If RemoveExtraPlanNameNumber = True Then
                    returnVar = returnVar.Replace(" 2", "")
                End If
            End If
            Return returnVar
        End Function
    End Class

    Public Class PayPlanOptions
        'qqHelper.ConvertToQuotedPremiumFormat(op.TotalInstallmentAmount)
        Public Property PayPlanId As Int32
        Public Property DownPayment As Double
        Public Property TotalPayments As Double

    End Class
End Namespace

