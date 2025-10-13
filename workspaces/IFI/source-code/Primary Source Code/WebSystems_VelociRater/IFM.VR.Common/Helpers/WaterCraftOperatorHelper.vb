Imports IFM.PrimativeExtensions

Namespace IFM.VR.Common.Helpers
    Public Module WaterCraftOperatorHelper

        Public Function GetAssignedOperators(quote As QuickQuote.CommonObjects.QuickQuoteObject, watercraft As QuickQuote.CommonObjects.QuickQuoteRvWatercraft) As Dictionary(Of QuickQuote.CommonObjects.QuickQuoteOperator, Boolean)
            Dim list As New Dictionary(Of QuickQuote.CommonObjects.QuickQuoteOperator, Boolean)
            If quote?.Operators IsNot Nothing AndAlso watercraft IsNot Nothing Then
                For Each o In quote.Operators
                    list.Add(o, IsOperatorAssignedToWatercraft(quote, o, watercraft))
                Next
            End If
            Return list
        End Function

        Public Function IsOperatorAssignedToWatercraft(quote As QuickQuote.CommonObjects.QuickQuoteObject, op As QuickQuote.CommonObjects.QuickQuoteOperator, watercraft As QuickQuote.CommonObjects.QuickQuoteRvWatercraft) As Boolean
            Dim opIndex = Coalesce(quote?.Operators?.IndexOf(op), -1)
            If opIndex >= 0 Then
                Dim opNum = opIndex + 1
                Return Coalesce(watercraft?.AssignedOperatorNums?.Contains(opNum), False)
            End If
            Return False
        End Function

        Public Sub RemoveWatercraftOperator(quote As QuickQuote.CommonObjects.QuickQuoteObject, wcOperator As QuickQuote.CommonObjects.QuickQuoteOperator)
            If Coalesce(quote?.Operators?.Any(), False) Then
                Dim opereatorIndex = quote.Operators.IndexOf(wcOperator)
                If opereatorIndex >= 0 Then
                    Dim operatorNumber = opereatorIndex + 1
                    quote.Operators.Remove(wcOperator)
                    RemoveWatercraftOperatorAssignment(quote, operatorNumber)
                End If
            End If
        End Sub

        Private Sub RemoveWatercraftOperatorAssignment(quote As QuickQuote.CommonObjects.QuickQuoteObject, operatorNumber As Int32)
            Dim ShiftWatercraftOperatorAssignment As New Action(Of List(Of Int32))(Sub(assignedOperatorNumberList As List(Of Int32))
                                                                                       If assignedOperatorNumberList IsNot Nothing Then
                                                                                           For Each num In assignedOperatorNumberList
                                                                                               If num > operatorNumber Then
                                                                                                   num = num - 1
                                                                                               End If
                                                                                           Next
                                                                                       End If
                                                                                   End Sub)
            Dim watercrafts = quote?.Locations.GetItemAtIndex(0)?.RvWatercrafts
            If watercrafts IsNot Nothing Then
                For Each wc In watercrafts
                    If wc.AssignedOperatorNums IsNot Nothing Then
                        wc.AssignedOperatorNums.Remove(operatorNumber)
                        ShiftWatercraftOperatorAssignment(wc.AssignedOperatorNums)
                    End If
                Next
            End If
        End Sub


        Public Function GetYouthfulOperators(quote As QuickQuote.CommonObjects.QuickQuoteObject) As IEnumerable(Of QuickQuote.CommonObjects.QuickQuoteOperator)
            If quote IsNot Nothing Then
                Return quote.Operators.FindAll(Function(op)
                                                   Dim effDate = If(IsDate(quote.EffectiveDate), CDate(quote.EffectiveDate), DateTime.Now)
                                                   Dim opDOB = DateTime.MinValue
                                                   If (DateTime.TryParse(Coalesce(op?.Name?.BirthDate, String.Empty), opDOB)) Then
                                                       Return effDate.AddYears(-25) < opDOB
                                                   End If
                                                   Return False
                                               End Function)
            End If
            Return New List(Of QuickQuote.CommonObjects.QuickQuoteOperator)
        End Function

        Public Function GetYouthfulOperatorNums(quote As QuickQuote.CommonObjects.QuickQuoteObject) As IEnumerable(Of Int32)
            If quote?.Operators IsNot Nothing Then
                Dim youthfulOperators = GetYouthfulOperators(quote)
                Return From o In youthfulOperators Select quote.Operators.IndexOf(o) + 1
            End If
            Return New List(Of Int32)
        End Function


    End Module

End Namespace
