Imports Microsoft.VisualBasic
'added 12/20/2013
Imports QuickQuote.CommonMethods

Namespace QuickQuote.CommonObjects 'added namespace 12/20/2013
    ''' <summary>
    ''' object used to store static data option (text and value)
    ''' </summary>
    ''' <remarks>used w/ Static Data xml file(s)</remarks>
    <Serializable()> _
    Public Class QuickQuoteStaticDataOption 'added 11/19/2013; 8/4/2014 note: will not be inheriting QuickQuoteBaseObject
        'Implements IDisposable
        'Implements IComparable(Of QuickQuoteStaticDataOption) 'testing 12/23/2013

        'added 12/23/2013
        Enum SortBy
            None = 0
            TextAscending = 1
            ValueAscending = 2
            TextDescending = 3
            ValueDescending = 4
        End Enum
        'added 12/24/2013
        Enum EmptyZeroOrNoneEvaluationType
            HandleAsText = 1
            HandleAsNothing = 2
            MaintainEmptiesAtBeginningAndHandleOthersAsText = 3
            MaintainEmptiesAtBeginningAndHandleOthersAsNothing = 4
        End Enum
        Enum ComparerEmptyZeroOrNoneEvaluationType
            HandleAsText = 1
            HandleAsNothing = 2
        End Enum

        Public Property Text As String = String.Empty
        Public Property Value As String = String.Empty
        Public Property lobs As String = String.Empty
        Public Property persOrComm As QuickQuoteHelperClass.PersOrComm = QuickQuoteHelperClass.PersOrComm.None
        'Public Property NameValuePairs As List(Of QuickQuoteStaticDataNameValuePair) = Nothing 'added 11/26/2013; removed 8/15/2014
        Public Property MiscellaneousElements As List(Of QuickQuoteStaticDataElement) = Nothing 'added 8/15/2014
        Public Property MiscellaneousAttributes As List(Of QuickQuoteStaticDataAttribute) = Nothing 'added 8/15/2014
        Public Property ignoreForLists As Boolean = False 'added 12/6/2013

        'testing 12/23/2013
        'Public Function CompareToText(comparePart As QuickQuoteStaticDataOption) As Integer _
        '    Implements IComparable(Of QuickQuoteStaticDataOption).CompareTo
        '    ' A null value means that this object is greater. 
        '    If comparePart Is Nothing Then
        '        Return 1
        '    Else

        '        Return Me.Text.CompareTo(comparePart.Text)
        '    End If
        'End Function
        'Public Function CompareToValue(comparePart As QuickQuoteStaticDataOption) As Integer _
        '    'Implements IComparable(Of QuickQuoteStaticDataOption).CompareTo
        '    ' A null value means that this object is greater. 
        '    If comparePart Is Nothing Then
        '        Return 1
        '    Else

        '        Return Me.Value.CompareTo(comparePart.Value)
        '    End If
        'End Function

        '#Region "IDisposable Support"
        '    Private disposedValue As Boolean ' To detect redundant calls

        '    ' IDisposable
        '    Protected Overridable Sub Dispose(disposing As Boolean)
        '        If Not Me.disposedValue Then
        '            If disposing Then
        '                ' TODO: dispose managed state (managed objects).
        '                Text = Nothing
        '                Value = Nothing
        '                lobs = Nothing
        '                persOrComm = Nothing
        '                ignoreForLists = Nothing
        '            End If

        '            ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
        '            ' TODO: set large fields to null.
        '        End If
        '        Me.disposedValue = True
        '    End Sub

        '    ' TODO: override Finalize() only if Dispose(ByVal disposing As Boolean) above has code to free unmanaged resources.
        '    'Protected Overrides Sub Finalize()
        '    '    ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
        '    '    Dispose(False)
        '    '    MyBase.Finalize()
        '    'End Sub

        '    ' This code added by Visual Basic to correctly implement the disposable pattern.
        '    Public Sub Dispose() Implements IDisposable.Dispose
        '        ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
        '        Dispose(True)
        '        GC.SuppressFinalize(Me)
        '    End Sub
        '#End Region

    End Class
    'added 12/24/2013
    'Public Interface IQuickQuoteStaticDataOptionComparer
    '    Sub Compare()
    'End Interface
    Public Class QuickQuoteStaticDataOptionComparer
        'Implements IQuickQuoteStaticDataOptionComparer
        'Implements IComparer(Of QuickQuoteStaticDataOption)

        'Public Sub Compare() Implements IQuickQuoteStaticDataOptionComparer.Compare

        'End Sub

        'Public Function Compare1(x As QuickQuoteStaticDataOption, y As QuickQuoteStaticDataOption) As Integer Implements System.Collections.Generic.IComparer(Of QuickQuoteStaticDataOption).Compare

        'End Function

        Public Function Compare(ByVal x As QuickQuoteStaticDataOption, ByVal y As QuickQuoteStaticDataOption, ByVal sortBy As QuickQuoteStaticDataOption.SortBy, ByVal EmptyZeroOrNoneEvaluationType As QuickQuoteStaticDataOption.ComparerEmptyZeroOrNoneEvaluationType) As Integer
            If sortBy = Nothing OrElse sortBy = QuickQuoteStaticDataOption.SortBy.None Then
                sortBy = QuickQuoteStaticDataOption.SortBy.TextAscending
            End If
            If EmptyZeroOrNoneEvaluationType = Nothing Then
                EmptyZeroOrNoneEvaluationType = QuickQuoteStaticDataOption.ComparerEmptyZeroOrNoneEvaluationType.HandleAsText
            End If

            Dim isTextSort As Boolean = If(sortBy = QuickQuoteStaticDataOption.SortBy.TextAscending OrElse sortBy = QuickQuoteStaticDataOption.SortBy.TextDescending, True, False)
            Dim isAscendingSort As Boolean = If(sortBy = QuickQuoteStaticDataOption.SortBy.TextAscending OrElse sortBy = QuickQuoteStaticDataOption.SortBy.ValueAscending, True, False)

            If x Is Nothing AndAlso y Is Nothing Then
                Return 0
            ElseIf x Is Nothing Then
                If isAscendingSort = True Then
                    'asc
                    Return -1
                Else
                    'desc
                    Return 1
                End If
            ElseIf y Is Nothing Then
                If isAscendingSort = True Then
                    'asc
                    Return 1
                Else
                    'desc
                    Return -1
                End If
            Else
                If (isTextSort = True AndAlso x.Text Is Nothing AndAlso y.Text Is Nothing) OrElse (isTextSort = False AndAlso x.Value Is Nothing AndAlso y.Value Is Nothing) Then
                    Return 0
                ElseIf (isTextSort = True AndAlso x.Text Is Nothing) OrElse (isTextSort = False AndAlso x.Value Is Nothing) Then
                    If isAscendingSort = True Then
                        'asc
                        Return -1
                    Else
                        'desc
                        Return 1
                    End If
                ElseIf (isTextSort = True AndAlso y.Text Is Nothing) OrElse (isTextSort = False AndAlso y.Value Is Nothing) Then
                    If isAscendingSort = True Then
                        'asc
                        Return 1
                    Else
                        'desc
                        Return -1
                    End If
                Else
                    If EmptyZeroOrNoneEvaluationType = QuickQuoteStaticDataOption.ComparerEmptyZeroOrNoneEvaluationType.HandleAsNothing Then
                        If (isTextSort = True AndAlso IsEmptyZeroOrNone(x.Text) = True AndAlso IsEmptyZeroOrNone(y.Text) = True) OrElse (isTextSort = False AndAlso IsEmptyZeroOrNone(x.Value) = True AndAlso IsEmptyZeroOrNone(y.Value) = True) Then
                            'continue so it sorts by text
                        ElseIf (isTextSort = True AndAlso IsEmptyZeroOrNone(x.Text) = True) OrElse (isTextSort = False AndAlso IsEmptyZeroOrNone(x.Value) = True) Then
                            If isAscendingSort = True Then
                                'asc
                                Return -1
                                Exit Function
                            Else
                                'desc
                                Return 1
                                Exit Function
                            End If
                        ElseIf (isTextSort = True AndAlso IsEmptyZeroOrNone(y.Text) = True) OrElse (isTextSort = False AndAlso IsEmptyZeroOrNone(y.Value) = True) Then
                            If isAscendingSort = True Then
                                'asc
                                Return 1
                                Exit Function
                            Else
                                'desc
                                Return -1
                                Exit Function
                            End If
                        End If
                    End If
                    If isTextSort = True Then
                        If IsNumeric(x.Text) = True AndAlso IsNumeric(y.Text) = True Then
                            If isAscendingSort = True Then
                                'numeric text asc
                                Return CType(x.Text, Double).CompareTo(CType(y.Text, Double))
                            Else
                                'numeric text desc
                                Return CType(y.Text, Double).CompareTo(CType(x.Text, Double))
                            End If
                        Else
                            If isAscendingSort = True Then
                                'string text asc
                                Return x.Text.CompareTo(y.Text)
                            Else
                                'string text desc
                                Return y.Text.CompareTo(x.Text)
                            End If
                        End If
                    Else
                        If IsNumeric(x.Value) = True AndAlso IsNumeric(y.Value) = True Then
                            If isAscendingSort = True Then
                                'numeric value asc
                                Return CType(x.Value, Double).CompareTo(CType(y.Value, Double))
                            Else
                                'numeric value desc
                                Return CType(y.Value, Double).CompareTo(CType(x.Value, Double))
                            End If
                        Else
                            If isAscendingSort = True Then
                                'string value asc
                                Return x.Value.CompareTo(y.Value)
                            Else
                                'string value desc
                                Return y.Value.CompareTo(x.Value)
                            End If
                        End If
                    End If
                End If
            End If
        End Function
        Public Function IsEmptyZeroOrNone(ByVal x As String) As Boolean
            If x Is Nothing Then
                Return True
            Else
                Select Case Trim(UCase(x))
                    Case "", "N/A", "NONE", "0", "-1"
                        Return True
                    Case Else
                        Return False
                End Select
            End If
        End Function
    End Class
    Public Class QuickQuoteStaticDataOptionComparer_TextAscending
        Inherits QuickQuoteStaticDataOptionComparer
        Implements IComparer(Of QuickQuoteStaticDataOption)

        Public Function Compare1(x As QuickQuoteStaticDataOption, y As QuickQuoteStaticDataOption) As Integer Implements System.Collections.Generic.IComparer(Of QuickQuoteStaticDataOption).Compare
            Return MyBase.Compare(x, y, QuickQuoteStaticDataOption.SortBy.TextAscending, QuickQuoteStaticDataOption.ComparerEmptyZeroOrNoneEvaluationType.HandleAsText)
        End Function
    End Class
    Public Class QuickQuoteStaticDataOptionComparer_ValueAscending
        Inherits QuickQuoteStaticDataOptionComparer
        Implements IComparer(Of QuickQuoteStaticDataOption)

        Public Function Compare1(x As QuickQuoteStaticDataOption, y As QuickQuoteStaticDataOption) As Integer Implements System.Collections.Generic.IComparer(Of QuickQuoteStaticDataOption).Compare
            Return MyBase.Compare(x, y, QuickQuoteStaticDataOption.SortBy.ValueAscending, QuickQuoteStaticDataOption.ComparerEmptyZeroOrNoneEvaluationType.HandleAsText)
        End Function
    End Class
    Public Class QuickQuoteStaticDataOptionComparer_TextDescending
        Inherits QuickQuoteStaticDataOptionComparer
        Implements IComparer(Of QuickQuoteStaticDataOption)

        Public Function Compare1(x As QuickQuoteStaticDataOption, y As QuickQuoteStaticDataOption) As Integer Implements System.Collections.Generic.IComparer(Of QuickQuoteStaticDataOption).Compare
            Return MyBase.Compare(x, y, QuickQuoteStaticDataOption.SortBy.TextDescending, QuickQuoteStaticDataOption.ComparerEmptyZeroOrNoneEvaluationType.HandleAsText)
        End Function
    End Class
    Public Class QuickQuoteStaticDataOptionComparer_ValueDescending
        Inherits QuickQuoteStaticDataOptionComparer
        Implements IComparer(Of QuickQuoteStaticDataOption)

        Public Function Compare1(x As QuickQuoteStaticDataOption, y As QuickQuoteStaticDataOption) As Integer Implements System.Collections.Generic.IComparer(Of QuickQuoteStaticDataOption).Compare
            Return MyBase.Compare(x, y, QuickQuoteStaticDataOption.SortBy.ValueDescending, QuickQuoteStaticDataOption.ComparerEmptyZeroOrNoneEvaluationType.HandleAsText)
        End Function
    End Class
    'added later on 12/24/2013 for EmptiesNonesAndZeros
    Public Class QuickQuoteStaticDataOptionComparer_TextAscending_EmptiesNonesAndZerosAreNothing
        Inherits QuickQuoteStaticDataOptionComparer
        Implements IComparer(Of QuickQuoteStaticDataOption)

        Public Function Compare1(x As QuickQuoteStaticDataOption, y As QuickQuoteStaticDataOption) As Integer Implements System.Collections.Generic.IComparer(Of QuickQuoteStaticDataOption).Compare
            Return MyBase.Compare(x, y, QuickQuoteStaticDataOption.SortBy.TextAscending, QuickQuoteStaticDataOption.ComparerEmptyZeroOrNoneEvaluationType.HandleAsNothing)
        End Function
    End Class
    Public Class QuickQuoteStaticDataOptionComparer_ValueAscending_EmptiesNonesAndZerosAreNothing
        Inherits QuickQuoteStaticDataOptionComparer
        Implements IComparer(Of QuickQuoteStaticDataOption)

        Public Function Compare1(x As QuickQuoteStaticDataOption, y As QuickQuoteStaticDataOption) As Integer Implements System.Collections.Generic.IComparer(Of QuickQuoteStaticDataOption).Compare
            Return MyBase.Compare(x, y, QuickQuoteStaticDataOption.SortBy.ValueAscending, QuickQuoteStaticDataOption.ComparerEmptyZeroOrNoneEvaluationType.HandleAsNothing)
        End Function
    End Class
    Public Class QuickQuoteStaticDataOptionComparer_TextDescending_EmptiesNonesAndZerosAreNothing
        Inherits QuickQuoteStaticDataOptionComparer
        Implements IComparer(Of QuickQuoteStaticDataOption)

        Public Function Compare1(x As QuickQuoteStaticDataOption, y As QuickQuoteStaticDataOption) As Integer Implements System.Collections.Generic.IComparer(Of QuickQuoteStaticDataOption).Compare
            Return MyBase.Compare(x, y, QuickQuoteStaticDataOption.SortBy.TextDescending, QuickQuoteStaticDataOption.ComparerEmptyZeroOrNoneEvaluationType.HandleAsNothing)
        End Function
    End Class
    Public Class QuickQuoteStaticDataOptionComparer_ValueDescending_EmptiesNonesAndZerosAreNothing
        Inherits QuickQuoteStaticDataOptionComparer
        Implements IComparer(Of QuickQuoteStaticDataOption)

        Public Function Compare1(x As QuickQuoteStaticDataOption, y As QuickQuoteStaticDataOption) As Integer Implements System.Collections.Generic.IComparer(Of QuickQuoteStaticDataOption).Compare
            Return MyBase.Compare(x, y, QuickQuoteStaticDataOption.SortBy.ValueDescending, QuickQuoteStaticDataOption.ComparerEmptyZeroOrNoneEvaluationType.HandleAsNothing)
        End Function
    End Class
End Namespace
