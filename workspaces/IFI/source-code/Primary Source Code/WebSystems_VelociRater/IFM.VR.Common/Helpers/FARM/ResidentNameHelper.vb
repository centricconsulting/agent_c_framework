Imports IFM.VR.Common.Helpers.MultiState
Imports QuickQuote.CommonObjects

Namespace IFM.VR.Common.Helpers.FARM
    Public Class ResidentNameHelper

        Public Shared Function HasIncompleteResidentNames(topQuote As QuickQuote.CommonObjects.QuickQuoteObject) As Boolean
            If topQuote IsNot Nothing Then
                If topQuote.QuoteLevel <> QuickQuote.CommonMethods.QuickQuoteHelperClass.QuoteLevel.TopLevel Then
                    Throw New NotTopLevelQuoteException()
                End If
                If topQuote.Locations IsNot Nothing Then
                    For Each l In topQuote.Locations
                        If l IsNot Nothing Then
                            If l.ResidentNames IsNot Nothing Then
                                For Each n In l.ResidentNames
                                    If ResidentNameIsIncomplete(n) Then
                                        Return True
                                    End If
                                Next
                            End If
                        End If
                    Next
                End If
            End If
            Return False
        End Function

        Public Shared Sub RemoveIncompleteResidentNames(topQuote As QuickQuote.CommonObjects.QuickQuoteObject, Optional update_NumberOfPersonsReceivingCare As Boolean = False)

            If topQuote IsNot Nothing Then
                If topQuote.QuoteLevel <> QuickQuote.CommonMethods.QuickQuoteHelperClass.QuoteLevel.TopLevel Then
                    Throw New NotTopLevelQuoteException()
                End If
                If topQuote.Locations IsNot Nothing Then
                    Dim removeList As New List(Of QuickQuote.CommonObjects.QuickQuoteResidentName)
                    For Each l In topQuote.Locations
                        If l IsNot Nothing Then
                            If l.ResidentNames IsNot Nothing Then
                                For Each n In l.ResidentNames
                                    If ResidentNameIsIncomplete(n) Then
                                        removeList.Add(n)
                                    End If
                                Next
                            End If
                            For Each r In removeList
                                l.ResidentNames.Remove(r)
                            Next
                        End If
                        removeList.Clear()
                        If update_NumberOfPersonsReceivingCare Then
                            If l.SectionIICoverages IsNot Nothing Then
                                '' update 'NumberOfPersonsReceivingCare' so that if we delete a record that will be reflected on the quote side
                                Dim famMed As QuickQuoteSectionIICoverage = (From cov In l.SectionIICoverages Where cov.CoverageType = QuickQuoteSectionIICoverage.SectionIICoverageType.Named_Persons_Medical_Payments Select cov).FirstOrDefault()
                                If famMed IsNot Nothing Then
                                    famMed.NumberOfPersonsReceivingCare = l.ResidentNames.Count.ToString()
                                End If
                            End If
                        End If
                    Next
                End If
            End If
        End Sub

        ''' <summary>
        ''' Use this to remove all resident names from the quote.
        ''' </summary>
        ''' <param name="topQuote"></param>
        Public Shared Sub RemoveAllResidentNames(topQuote As QuickQuote.CommonObjects.QuickQuoteObject)
            If topQuote IsNot Nothing Then
                If topQuote.QuoteLevel <> QuickQuote.CommonMethods.QuickQuoteHelperClass.QuoteLevel.TopLevel Then
                    Throw New NotTopLevelQuoteException()
                End If
                If topQuote.Locations IsNot Nothing Then
                    For Each l In topQuote.Locations
                        If l IsNot Nothing Then
                            If l.ResidentNames IsNot Nothing Then
                                l.ResidentNames = New List(Of QuickQuoteResidentName)()
                            End If
                        End If
                    Next
                End If
            End If
        End Sub

        Private Shared Function ResidentNameIsIncomplete(n As QuickQuote.CommonObjects.QuickQuoteResidentName) As Boolean
            If n.Name.TypeId = "1" Then
                If String.IsNullOrWhiteSpace(n.Name.FirstName) Or String.IsNullOrWhiteSpace(n.Name.LastName) Or String.IsNullOrWhiteSpace(n.Name.BirthDate) Or String.IsNullOrWhiteSpace(n.Name.Salutation) Then
                    Return True
                End If
            ElseIf n.Name.TypeId = "2" Then
                If String.IsNullOrWhiteSpace(n.Name.CommercialName1) AndAlso String.IsNullOrWhiteSpace(n.Name.CommercialName2) Then
                    Return True
                End If
            End If
            Return False
        End Function

    End Class
End Namespace