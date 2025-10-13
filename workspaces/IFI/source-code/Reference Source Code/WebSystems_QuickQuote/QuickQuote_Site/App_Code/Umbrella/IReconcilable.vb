Imports System.Runtime.CompilerServices
Imports Diamond.Common.Objects

Namespace QuickQuote.CommonObjects.Umbrella
    Public Interface IReconcilable
        Property ItemNumber As String
        ReadOnly Property HasValidItemNumber As Boolean
        Sub CopyFrom(src As IReconcilable, Optional setItemNumbers As Boolean = True, Optional canUseItemNumberForPolicyItemReconcilliation As Boolean = False)
        Sub ConvertToDiamondItem(ByRef diaItem As InsTableObject, ByRef innerDiamondItemDeletedFlag As Boolean, ByRef innerDiamondItemAddedFlag As Boolean, Optional canUseItemNumberForPolicyItemReconcilliation As Boolean = False)
        Function MatchesByItemNumber(src As IReconcilable) As Boolean
        Function IsMatchFor(src As IReconcilable, Optional canUseItemNumberForPolicyItemReconcilliation As Boolean = False) As Boolean
        Function IsMatchForDiamondItem(diaItem As InsTableObject, Optional canUseItemNumberForPolicyItemReconcilliation As Boolean = False) As Boolean
        Function EnsureSameType(src As IReconcilable, Optional throwException As Boolean = True) As Boolean
        Sub ParseThroughCollectionsAndSetFlags()
    End Interface

    Module IReconcilableExtensions
        <Extension>
        Public Sub CopyFromCollection(ByRef policyItems As IEnumerable(Of IReconcilable), ByVal srcPolicyItems As IEnumerable(Of IReconcilable),
                            Optional ByVal setItemNumbers As Boolean = True, Optional ByRef canUseItemNumberForPolicyItemReconcilliation As Boolean = False)

            If policyItems?.Any() AndAlso srcPolicyItems?.Any() Then
                'Only process lists of the same generic type!
                If policyItems.FirstOrDefault()?.EnsureSameType(srcPolicyItems?.FirstOrDefault(), False) = False Then
                    Throw New ArgumentException("Cannot process Lists of different generic types", NameOf(srcPolicyItems))
                End If

                If policyItems?.Any() AndAlso srcPolicyItems?.Count() = policyItems.Count() Then
                    For index As Integer = 0 To srcPolicyItems.Count - 1

                        If setItemNumbers Then
                            If policyItems(index).MatchesByItemNumber(srcPolicyItems(index)) = False Then
                                policyItems(index).ItemNumber = srcPolicyItems(index).ItemNumber
                                canUseItemNumberForPolicyItemReconcilliation = True
                            End If
                        End If
                        policyItems(index).CopyFrom(srcPolicyItems(index), setItemNumbers, canUseItemNumberForPolicyItemReconcilliation)
                    Next
                End If
            End If
        End Sub
        <Extension>
        Public Sub ParseThroughCollection_SetReconcilliationFlag(Of T As IReconcilable)(ByRef policyItems As IEnumerable(Of T), ByRef canUseIfItemNumberForReconciliationFlag As Boolean, Optional overrideTest As Func(Of T, Boolean) = Nothing)
            If policyItems?.Any() Then
                For Each item As T In policyItems
                    'allow the test to be overriden using supplied code if needed
                    If canUseIfItemNumberForReconciliationFlag = False AndAlso
                       If(overrideTest?.Invoke(item), item.HasValidItemNumber) Then
                        canUseIfItemNumberForReconciliationFlag = True
                    End If
                    item.ParseThroughCollectionsAndSetFlags()
                Next
            End If
        End Sub

        <Extension>
        Public Function AsInstance(Of T)(ByRef obj As IReconcilable) As T
            Return DirectCast(obj, T)
        End Function

        <Extension>
        Public Sub ConvertToDiamondItemCollection(Of D As {InsTableObject, Interfaces.IInsPolicyDetailObject})(ByRef policyItems As IEnumerable(Of IReconcilable), ByRef diaItems As InsCollection(Of D), ByRef diamondItemDeletedFlag As Boolean, ByRef diamondItemAddedFlag As Boolean, Optional ByVal useItemNumberForReconciliation As Boolean = False)
            If policyItems?.Any() Then
                Dim allNew As Boolean = False
                Dim hasAnyMatches As Boolean = False
                If diaItems Is Nothing Then
                    diaItems = New InsCollection(Of D)
                    allNew = True
                ElseIf diaItems.IsEmpty OrElse diaItems.Any() = False Then
                    allNew = True
                End If

                'remove any Diamond objects that aren't in our list
                If allNew = False Then
                    Dim matchNums As New List(Of Integer)
                    For Each dpi As D In diaItems
                        Dim hasMatch As Boolean = False
                        Dim itemCounter As Integer = 0
                        If dpi.IsMarkedForDeletion = False Then
                            For Each qpi In policyItems
                                itemCounter += 1
                                If matchNums.Contains(itemCounter) = False Then
                                    hasMatch = qpi.IsMatchForDiamondItem(dpi, useItemNumberForReconciliation)
                                    If hasMatch = True Then
                                        hasAnyMatches = True
                                        matchNums.Add(itemCounter)
                                        Exit For 'note: may need to exit every time to maintain order
                                    End If
                                    'Exit For 'will make sure records stay in order; these don't need to stay in order... especially since we convert them back and forth
                                    'note: may want to maintain order for setDiamondNum logic... in case they would ever be out of order there; note: verify CopyRatedQuoteInformationToQuoteObject method
                                End If
                            Next
                        End If
                        If hasMatch = False Then
                            dpi.MarkForDeletion
                            diamondItemDeletedFlag = True
                        End If
                    Next
                End If
                Dim diaMatchNums As New List(Of Integer) 'for reconciliation
                Dim currentDiaItemCount As Integer = diaItems.Count
                For Each pItem In policyItems
                    Dim diaItem As D = Nothing
                    Dim needsToAdd As Boolean = True 'will be switched to False on edit
                    'see if matching Diamond object already exists... if so, edit it; else, instantiate new and set; object method instantiates so that's not really needed here
                    If allNew = False AndAlso hasAnyMatches = True Then
                        Dim hasMatch As Boolean = False
                        Dim diaItemCounter As Integer = 0
                        For Each dI In diaItems
                            diaItemCounter += 1
                            'If helper.IsValidDiamondNum(diaPi.PolicyInfoNum) = True Then 'note: could use this to verify it's an existing one instead of comparing before and after count

                            'End If
                            If diaItemCounter <= currentDiaItemCount Then 'to not even check ones added in previous iteration (per logic added for coverage reconciliation); was previously happening every time... works correctly but may be easier to just check diamondNum (helper.IsValidDiamondNum)
                                If dI.IsMarkedForDeletion = False Then 'make sure we're not even looking at ones already flagged for delete
                                    If diaMatchNums.Contains(diaItemCounter) = False Then
                                        hasMatch = pItem.IsMatchForDiamondItem(dI, useItemNumberForReconciliation)
                                        If hasMatch = True Then
                                            diaItem = dI
                                            needsToAdd = False
                                            diaMatchNums.Add(diaItemCounter)
                                            Exit For
                                        End If
                                    End If
                                End If
                            Else
                                'current object was just added in previous iteration; wasn't there before qq loop
                            End If
                        Next
                    End If
                    pItem.ConvertToDiamondItem(diaItem, diamondItemDeletedFlag, diamondItemAddedFlag, useItemNumberForReconciliation)
                    If needsToAdd = True Then
                        diamondItemAddedFlag = True 'so quote xml will always have nums needed for reconciliation
                        diaItems.Add(diaItem)
                    End If
                Next
            ElseIf diaItems IsNot Nothing Then
                'need to remove all
                For Each dI In diaItems
                    dI.MarkForDeletion
                Next
                diamondItemDeletedFlag = True
            End If
        End Sub
        <Extension>
        Public Sub MarkForDeletion(Of D As {InsTableObject, Interfaces.IInsPolicyDetailObject})(ByRef diaItem As D)
            diaItem.DetailStatusCode = Diamond.Common.Enums.StatusCode.Deleted
        End Sub
        <Extension>
        Public Function IsMarkedForDeletion(Of D As {InsTableObject, Interfaces.IInsPolicyDetailObject})(ByRef diaItem As D) As Boolean
            Return (diaItem.DetailStatusCode = Diamond.Common.Enums.StatusCode.Deleted)
        End Function

        'TODO: we probably need to move this to a general purpose area and probably drop the IReconcilable constraint
        <Extension>
        Public Function InitIfNothing(Of T As {IReconcilable, New})(ByRef obj As T) As T
            If obj Is Nothing Then
                obj = New T
            End If
            Return obj 'we return this to make it fluent :-) 
        End Function
        <Extension>
        Public Function InitIfNothing(Of T As {IReconcilable, New})(ByRef objList As List(Of T)) As List(Of T)
            If objList Is Nothing Then
                objList = New List(Of T)
            End If
            Return objList 'we return this to make it fluent :-) 
        End Function
    End Module
End Namespace