Imports Microsoft.VisualBasic
Namespace QuickQuote.CommonObjects
    <Serializable()>
    Public Class QuickQuoteDevDictionaryList
        Inherits List(Of QuickQuoteDevDictionaryItem)
        Implements IReadOnlyList(Of QuickQuoteDevDictionaryItem)

        Protected Friend Sub AddDevDictionaryItem(page As String, key As String, value As String, listControlIndex As Integer, state As QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState)
            If String.IsNullOrWhiteSpace(key) = False AndAlso String.IsNullOrWhiteSpace(value) = False Then
                listControlIndex = CheckListControlIndexInput(listControlIndex)
                page = CheckPageInput(page)

                Dim newItem As New QuickQuoteDevDictionaryItem
                newItem.SetKey(key)
                newItem.SetValue(value)
                newItem.SetPage(page)
                newItem.SetState(state)
                newItem.SetListControlIndex(listControlIndex)
                Me.Add(newItem)
            End If
        End Sub

        Protected Friend Function HasDevDictionaryItem(page As String, key As String, listControlIndex As Integer, state As QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState) As Boolean
            If Me.Count > 0 Then
                Return Me.GetDevDictionaryItem(page, key, listControlIndex, state) IsNot Nothing
            Else
                Return False
            End If
        End Function

        Protected Friend Function HasDevDictionaryItem(page As String, key As String, listControlIndex As Integer, state As QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState, ByRef devDictionaryItem As QuickQuoteDevDictionaryItem) As Boolean
            If Me.Count > 0 Then
                devDictionaryItem = Me.GetDevDictionaryItem(page, key, listControlIndex, state)
                Return devDictionaryItem IsNot Nothing
            Else
                Return False
            End If
        End Function

        Protected Friend Function GetDevDictionaryItem(page As String, key As String, listControlIndex As Integer, state As QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState) As QuickQuoteDevDictionaryItem
            If Me.Count > 0 Then
                listControlIndex = CheckListControlIndexInput(listControlIndex)
                page = CheckPageInput(page)
                Return Me.Find(Function(x) x.Key.Equals(key, StringComparison.OrdinalIgnoreCase) AndAlso x.Page.Equals(page, StringComparison.OrdinalIgnoreCase) AndAlso x.State = state AndAlso listControlIndex = x.ListControlIndex)
            Else
                Return Nothing
            End If
        End Function

        Protected Friend Function GetDevDictionaryValue(page As String, key As String, listControlIndex As Integer, state As QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState) As String
            Dim returnVar As String = ""

            If Me.Count > 0 Then
                Dim devDictionaryItem As QuickQuoteDevDictionaryItem = Nothing
                If HasDevDictionaryItem(page, key, listControlIndex, state, devDictionaryItem) Then
                    returnVar = devDictionaryItem.Value
                End If
            End If

            Return returnVar
        End Function

        Protected Friend Sub RemoveDevDictionaryItem(page As String, key As String, listControlIndex As Integer, state As QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState)
            If Me.Count > 0 Then
                Dim devDictionaryItem As QuickQuoteDevDictionaryItem = Nothing
                If HasDevDictionaryItem(page, key, listControlIndex, state, devDictionaryItem) Then
                    Me.Remove(devDictionaryItem)
                    RenumberListControlItems(page, key, listControlIndex, state)
                End If
            End If
        End Sub

        Protected Friend Sub SetDevDictionaryItem(page As String, key As String, listControlIndex As Integer, state As QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState, value As String)
            If Me.Count > 0 Then
                Dim devDictionaryItem As QuickQuoteDevDictionaryItem = Nothing
                If HasDevDictionaryItem(page, key, listControlIndex, state, devDictionaryItem) Then
                    devDictionaryItem.SetValue(value)
                End If
            End If
        End Sub

        Protected Friend Sub SetDevDictionaryItem(devDictionaryItem As QuickQuoteDevDictionaryItem, newValue As String)
            If Me.Count > 0 Then
                If devDictionaryItem IsNot Nothing Then
                    devDictionaryItem.SetValue(newValue)
                End If
            End If
        End Sub

        Private Sub RenumberListControlItems(page As String, key As String, listControlIndex As Integer, state As QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState)
            If Me.Count > 0 Then
                If listControlIndex >= 0 Then
                    listControlIndex += 1
                    Dim numListItems As Integer = GetNumberOfControlListItems(page, key, state)
                    For i As Integer = listControlIndex To numListItems
                        Dim devDictionaryItem As QuickQuoteDevDictionaryItem = Nothing
                        If HasDevDictionaryItem(page, key, i, state, devDictionaryItem) Then
                            ReIndexDevDictionaryItem(devDictionaryItem, i - 1)
                        End If
                    Next
                End If
            End If
        End Sub

        Private Function GetNumberOfControlListItems(page As String, key As String, state As QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState) As Integer
            If Me.Count > 0 Then
                page = CheckPageInput(page)
                Return Me.FindAll(Function(x) x.Key.Equals(key, StringComparison.OrdinalIgnoreCase) AndAlso x.Page.Equals(page, StringComparison.OrdinalIgnoreCase) AndAlso x.State = state AndAlso x.ListControlIndex >= 0).Count
            Else
                Return 0
            End If
        End Function

        Private Sub ReIndexDevDictionaryItem(page As String, key As String, listControlIndex As Integer, state As QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState, newIndex As Integer)
            If Me.Count > 0 Then
                Dim devDictionaryItem As QuickQuoteDevDictionaryItem = Nothing
                If HasDevDictionaryItem(page, key, listControlIndex, state, devDictionaryItem) Then
                    ReIndexDevDictionaryItem(devDictionaryItem, newIndex)
                End If
            End If
        End Sub

        Private Sub ReIndexDevDictionaryItem(devDictionaryItem As QuickQuoteDevDictionaryItem, newIndex As Integer)
            If Me.Count > 0 Then
                If devDictionaryItem IsNot Nothing Then
                    devDictionaryItem.SetListControlIndex(newIndex)
                End If
            End If
        End Sub

        Protected Friend Sub ClearListControlDevDictionaryItems(page As String, key As String, state As QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState)
            If Me.Count > 0 Then
                Me.RemoveAll(Function(x) x.Key.Equals(key, StringComparison.OrdinalIgnoreCase) AndAlso x.Page.Equals(page, StringComparison.OrdinalIgnoreCase) AndAlso x.State = state AndAlso x.ListControlIndex >= 0)
            End If
        End Sub

        Private Function CheckPageInput(page As String) As String
            If String.IsNullOrWhiteSpace(page) Then
                page = "global"
            End If
            Return page
        End Function

        Private Function CheckListControlIndexInput(listControlIndex As Integer) As Integer
            If listControlIndex < 0 Then
                listControlIndex = -1
            End If
            Return listControlIndex
        End Function

        Protected Friend Function JoinKeys(seperator As Char) As String
            Dim returnVar As String = ""

            If Me.Count > 0 Then
                Dim mySeperator As String = ""
                Dim first As Boolean = True
                For Each Item As QuickQuoteDevDictionaryItem In Me
                    returnVar &= mySeperator & Item.Key
                    If first = True Then
                        mySeperator = seperator
                        first = False
                    End If
                Next
            End If

            Return returnVar
        End Function

        Protected Friend Function JoinValues(seperator As Char) As String
            Dim returnVar As String = ""

            If Me.Count > 0 Then
                Dim mySeperator As String = ""
                Dim first As Boolean = True
                For Each Item As QuickQuoteDevDictionaryItem In Me
                    returnVar &= mySeperator & Item.Value
                    If first = True Then
                        mySeperator = seperator
                        first = False
                    End If
                Next
            End If

            Return returnVar
        End Function

        Protected Friend Function JoinPages(seperator As Char) As String
            Dim returnVar As String = ""

            If Me.Count > 0 Then
                Dim mySeperator As String = ""
                Dim first As Boolean = True
                For Each Item As QuickQuoteDevDictionaryItem In Me
                    returnVar &= mySeperator & Item.Page
                    If first = True Then
                        mySeperator = seperator
                        first = False
                    End If
                Next
            End If

            Return returnVar
        End Function

        Protected Friend Function JoinStates(seperator As Char) As String
            Dim returnVar As String = ""

            If Me.Count > 0 Then
                Dim mySeperator As String = ""
                Dim first As Boolean = True
                For Each Item As QuickQuoteDevDictionaryItem In Me
                    If Item.State = CommonMethods.QuickQuoteHelperClass.QuickQuoteState.None Then
                        returnVar &= mySeperator & "ALL"
                    Else
                        returnVar &= mySeperator & QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteStateForStateAbbreviation(Item.State)
                    End If

                    If first = True Then
                        mySeperator = seperator
                        first = False
                    End If
                Next
            End If

            Return returnVar
        End Function

        Protected Friend Function JoinListControlIndices(seperator As Char) As String
            Dim returnVar As String = ""

            If Me.Count > 0 Then
                Dim mySeperator As String = ""
                Dim first As Boolean = True
                For Each Item As QuickQuoteDevDictionaryItem In Me
                    If Item.ListControlIndex < 0 Then
                        returnVar &= mySeperator & "-1"
                    Else
                        returnVar &= mySeperator & Item.ListControlIndex.ToString()
                    End If

                    If first = True Then
                        mySeperator = seperator
                        first = False
                    End If
                Next
            End If

            Return returnVar
        End Function
    End Class
End Namespace