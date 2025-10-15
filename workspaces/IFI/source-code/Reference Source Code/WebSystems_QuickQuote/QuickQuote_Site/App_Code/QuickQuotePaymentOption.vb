Imports Microsoft.VisualBasic
Imports System.Collections.Generic
'added 12/20/2013
Imports QuickQuote.CommonMethods

Namespace QuickQuote.CommonObjects 'added namespace 12/20/2013
    ''' <summary>
    ''' object used to store payment option information
    ''' </summary>
    ''' <remarks></remarks>
    <Serializable()>
    Public Class QuickQuotePaymentOption
        Inherits QuickQuoteBaseGenericObject(Of Object) 'added 8/4/2014
        Implements IDisposable

        'added 9/4/2017
        Enum SortBy
            None = 0
            DescriptionAscending = 1
            DescriptionDescending = 2
            NumberOfInstallmentsAscending = 3
            NumberOfInstallmentsDescending = 4
            PayPlanIdAscending = 5
            PayPlanIdDescending = 6
            FriendlyDescriptionAscending = 7
            FriendlyDescriptionDescending = 8
            'added 9/8/2017
            NumberOfInstallmentsAscendingThenDescriptionAscending = 9
            NumberOfInstallmentsDescendingThenDescriptionAscending = 10
            NumberOfInstallmentsAscendingThenDescriptionDescending = 11
            NumberOfInstallmentsDescendingThenDescriptionDescending = 12
            NumberOfInstallmentsAscendingThenFriendlyDescriptionAscending = 13
            NumberOfInstallmentsDescendingThenFriendlyDescriptionAscending = 14
            NumberOfInstallmentsAscendingThenFriendlyDescriptionDescending = 15
            NumberOfInstallmentsDescendingThenFriendlyDescriptionDescending = 16
        End Enum

        'added 9/8/2017
        Enum ListPositionSortBy
            None = 0
            Ascending = 1
            Descending = 2
            MatchInitialSort = 3
            MatchLastSort = 4
        End Enum

        Dim qqHelper As New QuickQuoteHelperClass 'added 5/1/2013

        Private _Description As String
        Private _DepositAmount As String
        Private _InstallmentAmount As String
        Private _NumInstalls As String
        Private _InstallmentFee As String
        Private _DownPaymentPercentage As String
        Private _DefaultPaymentOption As Boolean
        'added 8/28/2017
        Private _Installments As List(Of QuickQuoteInstallment)
        Private _InstallmentInterval As String 'specific to PayPlanPreview
        Private _NextInstallmentDue As String 'specific to PayPlanPreview
        Private _PayPlanId As String 'specific to PayPlanPreview
        Private _TotalInstallmentAmount As String 'specific to PayPlanPreview
        Private _TotalPremiumFeeServiceChargeAmount As String 'specific to PayPlanPreview
        Private _ListPosition As Integer 'added 9/4/2017

        Public Property Description As String
            Get
                Return _Description
            End Get
            Set(value As String)
                _Description = value
            End Set
        End Property
        Public ReadOnly Property FriendlyDescription As String 'added 9/7/2017
            Get
                Return qqHelper.StringWithNumberRemovedFromEnd(_Description)
            End Get
        End Property
        Public Property DepositAmount As String
            Get
                'Return _DepositAmount
                'updated 8/25/2014
                Return qqHelper.QuotedPremiumFormat(_DepositAmount)
            End Get
            Set(value As String)
                _DepositAmount = value
                qqHelper.ConvertToQuotedPremiumFormat(_DepositAmount) 'added 5/1/2013
            End Set
        End Property
        Public Property InstallmentAmount As String
            Get
                'Return _InstallmentAmount
                'updated 8/25/2014
                Return qqHelper.QuotedPremiumFormat(_InstallmentAmount)
            End Get
            Set(value As String)
                _InstallmentAmount = value
                qqHelper.ConvertToQuotedPremiumFormat(_InstallmentAmount) 'added 5/1/2013
            End Set
        End Property
        Public Property NumInstalls As String
            Get
                Return _NumInstalls
            End Get
            Set(value As String)
                _NumInstalls = value
            End Set
        End Property
        Public Property InstallmentFee As String
            Get
                'Return _InstallmentFee
                'updated 8/25/2014
                Return qqHelper.QuotedPremiumFormat(_InstallmentFee)
            End Get
            Set(value As String)
                _InstallmentFee = value
                qqHelper.ConvertToQuotedPremiumFormat(_InstallmentFee) 'added 5/1/2013
            End Set
        End Property
        Public Property DownPaymentPercentage As String
            Get
                Return _DownPaymentPercentage
            End Get
            Set(value As String)
                _DownPaymentPercentage = value
            End Set
        End Property
        Public Property DefaultPaymentOption As Boolean
            Get
                Return _DefaultPaymentOption
            End Get
            Set(value As Boolean)
                _DefaultPaymentOption = value
            End Set
        End Property
        'added 8/28/2017
        Public Property Installments As List(Of QuickQuoteInstallment)
            Get
                SetParentOfListItems(_Installments, "{663B7C7B-F2AC-4BF6-965A-D30F41A05501}")
                Return _Installments
            End Get
            Set(value As List(Of QuickQuoteInstallment))
                _Installments = value
                SetParentOfListItems(_Installments, "{663B7C7B-F2AC-4BF6-965A-D30F41A05501}")
            End Set
        End Property
        Public Property InstallmentInterval As String 'specific to PayPlanPreview
            Get
                Return _InstallmentInterval
            End Get
            Set(value As String)
                _InstallmentInterval = value
            End Set
        End Property
        Public Property NextInstallmentDue As String 'specific to PayPlanPreview
            Get
                Return _NextInstallmentDue
            End Get
            Set(value As String)
                _NextInstallmentDue = value
            End Set
        End Property
        Public Property PayPlanId As String 'specific to PayPlanPreview
            Get
                Return _PayPlanId
            End Get
            Set(value As String)
                _PayPlanId = value
            End Set
        End Property
        Public Property TotalInstallmentAmount As String 'specific to PayPlanPreview
            Get
                If qqHelper.IsPositiveDecimalString(_TotalInstallmentAmount) = False AndAlso (qqHelper.IsPositiveDecimalString(_InstallmentAmount) = True OrElse qqHelper.IsPositiveDecimalString(_InstallmentFee) = True) Then 'added IF 9/4/2017
                    _TotalInstallmentAmount = qqHelper.getSum(_InstallmentAmount, _InstallmentFee)
                End If
                'Return _TotalInstallmentAmount
                'updated 9/16/2017
                Return qqHelper.QuotedPremiumFormat(_TotalInstallmentAmount)
            End Get
            Set(value As String)
                _TotalInstallmentAmount = value
                qqHelper.ConvertToQuotedPremiumFormat(_TotalInstallmentAmount) 'added 9/16/2017
            End Set
        End Property
        Public Property TotalPremiumFeeServiceChargeAmount As String 'specific to PayPlanPreview
            Get
                'Return _TotalPremiumFeeServiceChargeAmount
                'updated 9/16/2017
                Return qqHelper.QuotedPremiumFormat(_TotalPremiumFeeServiceChargeAmount)
            End Get
            Set(value As String)
                _TotalPremiumFeeServiceChargeAmount = value
                qqHelper.ConvertToQuotedPremiumFormat(_TotalPremiumFeeServiceChargeAmount) 'added 9/16/2017
            End Set
        End Property
        Public ReadOnly Property ListPosition As Integer 'added 9/4/2017
            Get
                Return _ListPosition
            End Get
        End Property

        Public Sub New()
            MyBase.New() 'added 8/4/2014
            SetDefaults()
        End Sub
        Private Sub SetDefaults()
            _Description = ""
            _DepositAmount = ""
            _InstallmentAmount = ""
            _NumInstalls = ""
            _InstallmentFee = ""
            _DownPaymentPercentage = ""
            _DefaultPaymentOption = False
            'added 8/28/2017
            _Installments = Nothing
            _InstallmentInterval = "" 'specific to PayPlanPreview
            _NextInstallmentDue = "" 'specific to PayPlanPreview
            _PayPlanId = "" 'specific to PayPlanPreview
            _TotalInstallmentAmount = "" 'specific to PayPlanPreview
            _TotalPremiumFeeServiceChargeAmount = "" 'specific to PayPlanPreview
            _ListPosition = 0 'added 9/4/2017
        End Sub
        Protected Friend Sub Set_ListPosition(ByVal lstPos As Integer)
            _ListPosition = lstPos
        End Sub

        'added 9/8/2017
        Public Overrides Function ToString() As String 'added 6/29/2015
            Dim str As String = ""
            If Me IsNot Nothing Then
                If String.IsNullOrWhiteSpace(Me.Description) = False Then
                    str = qqHelper.appendText(str, Me.Description, vbCrLf)
                End If
                If qqHelper.IsNumericString(Me.NumInstalls) = True Then
                    str = qqHelper.appendText(str, "NumInstalls: " & Me.NumInstalls, vbCrLf)
                End If
                If qqHelper.IsNumericString(Me.DepositAmount) = True Then
                    str = qqHelper.appendText(str, "DepositAmount: " & Me.DepositAmount, vbCrLf)
                End If
                If qqHelper.IsNumericString(Me.InstallmentAmount) = True Then
                    str = qqHelper.appendText(str, "InstallmentAmount: " & Me.InstallmentAmount, vbCrLf)
                End If
                If qqHelper.IsNumericString(Me.InstallmentFee) = True Then
                    str = qqHelper.appendText(str, "InstallmentFee: " & Me.InstallmentFee, vbCrLf)
                End If
            Else
                str = "Nothing"
            End If
            Return str
        End Function

#Region "IDisposable Support"
        Private disposedValue As Boolean ' To detect redundant calls

        ' IDisposable
        'Protected Overridable Sub Dispose(disposing As Boolean)
        'updated 8/4/2014 w/ QuickQuoteBaseObject inheritance
        Protected Overloads Sub Dispose(disposing As Boolean)
            If Not Me.disposedValue Then
                If disposing Then
                    ' TODO: dispose managed state (managed objects).
                    If _Description IsNot Nothing Then
                        _Description = Nothing
                    End If
                    If _DepositAmount IsNot Nothing Then
                        _DepositAmount = Nothing
                    End If
                    If _InstallmentAmount IsNot Nothing Then
                        _InstallmentAmount = Nothing
                    End If
                    If _NumInstalls IsNot Nothing Then
                        _NumInstalls = Nothing
                    End If
                    If _InstallmentFee IsNot Nothing Then
                        _InstallmentFee = Nothing
                    End If
                    If _DownPaymentPercentage IsNot Nothing Then
                        _DownPaymentPercentage = Nothing
                    End If
                    If _DefaultPaymentOption <> Nothing Then
                        _DefaultPaymentOption = Nothing
                    End If
                    'added 8/28/2017
                    If _Installments IsNot Nothing Then
                        If _Installments.Count > 0 Then
                            For Each i As QuickQuoteInstallment In _Installments
                                If i IsNot Nothing Then
                                    i.Dispose()
                                    i = Nothing
                                End If
                            Next
                            _Installments.Clear()
                        End If
                        _Installments = Nothing
                    End If
                    qqHelper.DisposeString(_InstallmentInterval) 'specific to PayPlanPreview
                    qqHelper.DisposeString(_NextInstallmentDue) 'specific to PayPlanPreview
                    qqHelper.DisposeString(_PayPlanId) 'specific to PayPlanPreview
                    qqHelper.DisposeString(_TotalInstallmentAmount) 'specific to PayPlanPreview
                    qqHelper.DisposeString(_TotalPremiumFeeServiceChargeAmount) 'specific to PayPlanPreview
                    _ListPosition = Nothing 'added 9/4/2017

                    MyBase.Dispose() 'added 8/4/2014
                End If

                ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
                ' TODO: set large fields to null.
            End If
            Me.disposedValue = True
        End Sub

        ' TODO: override Finalize() only if Dispose(ByVal disposing As Boolean) above has code to free unmanaged resources.
        'Protected Overrides Sub Finalize()
        '    ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
        '    Dispose(False)
        '    MyBase.Finalize()
        'End Sub

        ' This code added by Visual Basic to correctly implement the disposable pattern.
        'Public Sub Dispose() Implements IDisposable.Dispose
        'updated 8/4/2014 w/ QuickQuoteBaseObject inheritance
        Public Overrides Sub Dispose() 'Implements IDisposable.Dispose
            ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
            Dispose(True)
            GC.SuppressFinalize(Me)
        End Sub
#End Region

    End Class

    'added 9/4/2017; completed logic 9/6/2017
    Public Class QuickQuotePaymentOptionComparer

        Dim qqHelper As New QuickQuoteHelperClass 'added 9/6/2017

        'Public Function Compare(ByVal x As QuickQuotePaymentOption, ByVal y As QuickQuotePaymentOption, ByVal sortBy As QuickQuotePaymentOption.SortBy) As Integer
        'updated 9/8/2017 for optional backupListPositionSortBy
        Public Function Compare(ByVal x As QuickQuotePaymentOption, ByVal y As QuickQuotePaymentOption, ByVal sortBy As QuickQuotePaymentOption.SortBy, Optional ByVal backupListPositionSortBy As QuickQuotePaymentOption.ListPositionSortBy = QuickQuotePaymentOption.ListPositionSortBy.Ascending) As Integer
            If sortBy = Nothing OrElse sortBy = QuickQuotePaymentOption.SortBy.None Then
                sortBy = QuickQuotePaymentOption.SortBy.DescriptionAscending
            End If

            'Dim isAscendingSort As Boolean = If(sortBy = QuickQuotePaymentOption.SortBy.DescriptionAscending OrElse sortBy = QuickQuotePaymentOption.SortBy.NumberOfInstallmentsAscending OrElse sortBy = QuickQuotePaymentOption.SortBy.PayPlanIdAscending OrElse sortBy = QuickQuotePaymentOption.SortBy.FriendlyDescriptionAscending, True, False)
            'updated 9/8/2017
            Dim isAscendingSort As Boolean = If(sortBy = QuickQuotePaymentOption.SortBy.DescriptionAscending OrElse sortBy = QuickQuotePaymentOption.SortBy.NumberOfInstallmentsAscending OrElse sortBy = QuickQuotePaymentOption.SortBy.PayPlanIdAscending OrElse sortBy = QuickQuotePaymentOption.SortBy.FriendlyDescriptionAscending OrElse sortBy = QuickQuotePaymentOption.SortBy.NumberOfInstallmentsAscendingThenDescriptionAscending OrElse sortBy = QuickQuotePaymentOption.SortBy.NumberOfInstallmentsAscendingThenDescriptionDescending OrElse sortBy = QuickQuotePaymentOption.SortBy.NumberOfInstallmentsAscendingThenFriendlyDescriptionAscending OrElse sortBy = QuickQuotePaymentOption.SortBy.NumberOfInstallmentsAscendingThenFriendlyDescriptionDescending, True, False)

            'added 9/8/2017
            If backupListPositionSortBy = Nothing OrElse backupListPositionSortBy = QuickQuotePaymentOption.ListPositionSortBy.None Then
                backupListPositionSortBy = QuickQuotePaymentOption.ListPositionSortBy.Ascending
            End If
            Dim isBackupListPositionAscendingSort As Boolean = If(backupListPositionSortBy = QuickQuotePaymentOption.ListPositionSortBy.Ascending, True, False)
            If backupListPositionSortBy = QuickQuotePaymentOption.ListPositionSortBy.MatchInitialSort OrElse backupListPositionSortBy = QuickQuotePaymentOption.ListPositionSortBy.MatchLastSort Then
                isBackupListPositionAscendingSort = isAscendingSort
            End If

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
                'both objects are something
                Select Case sortBy
                    Case QuickQuotePaymentOption.SortBy.DescriptionAscending, QuickQuotePaymentOption.SortBy.DescriptionDescending
                        If String.IsNullOrWhiteSpace(x.Description) = True AndAlso String.IsNullOrWhiteSpace(y.Description) = True Then
                            'same so far; continue below
                        ElseIf String.IsNullOrWhiteSpace(x.Description) = True Then
                            If isAscendingSort = True Then
                                'asc
                                Return -1
                            Else
                                'desc
                                Return 1
                            End If
                        ElseIf String.IsNullOrWhiteSpace(y.Description) = True Then
                            If isAscendingSort = True Then
                                'asc
                                Return 1
                            Else
                                'desc
                                Return -1
                            End If
                        Else
                            'Description is something for both
                            If UCase(x.Description) = UCase(y.Description) Then
                                'same so far; continue below
                            Else
                                If isAscendingSort = True Then
                                    'string text asc
                                    Return UCase(x.Description).CompareTo(UCase(y.Description))
                                Else
                                    'string text desc
                                    Return UCase(y.Description).CompareTo(UCase(x.Description))
                                End If
                            End If
                        End If
                        'Case QuickQuotePaymentOption.SortBy.NumberOfInstallmentsAscending, QuickQuotePaymentOption.SortBy.NumberOfInstallmentsDescending
                        'updated 9/8/2017
                    Case QuickQuotePaymentOption.SortBy.NumberOfInstallmentsAscending, QuickQuotePaymentOption.SortBy.NumberOfInstallmentsDescending, QuickQuotePaymentOption.SortBy.NumberOfInstallmentsAscendingThenDescriptionAscending, QuickQuotePaymentOption.SortBy.NumberOfInstallmentsDescendingThenDescriptionAscending, QuickQuotePaymentOption.SortBy.NumberOfInstallmentsAscendingThenDescriptionDescending, QuickQuotePaymentOption.SortBy.NumberOfInstallmentsDescendingThenDescriptionDescending, QuickQuotePaymentOption.SortBy.NumberOfInstallmentsAscendingThenFriendlyDescriptionAscending, QuickQuotePaymentOption.SortBy.NumberOfInstallmentsDescendingThenFriendlyDescriptionAscending, QuickQuotePaymentOption.SortBy.NumberOfInstallmentsAscendingThenFriendlyDescriptionDescending, QuickQuotePaymentOption.SortBy.NumberOfInstallmentsDescendingThenFriendlyDescriptionDescending
                        Dim xNumInstalls As Integer = qqHelper.IntegerForString(x.NumInstalls)
                        Dim yNumInstalls As Integer = qqHelper.IntegerForString(y.NumInstalls)
                        If xNumInstalls = yNumInstalls Then
                            'same so far; continue below
                        Else
                            If isAscendingSort = True Then
                                'string text asc
                                Return xNumInstalls.CompareTo(yNumInstalls)
                            Else
                                'string text desc
                                Return yNumInstalls.CompareTo(xNumInstalls)
                            End If
                        End If
                    Case QuickQuotePaymentOption.SortBy.PayPlanIdAscending, QuickQuotePaymentOption.SortBy.PayPlanIdDescending
                        Dim xPayPlanId As Integer = qqHelper.IntegerForString(x.PayPlanId)
                        Dim yPayPlanId As Integer = qqHelper.IntegerForString(y.PayPlanId)
                        If xPayPlanId = yPayPlanId Then
                            'same so far; continue below
                        Else
                            If isAscendingSort = True Then
                                'string text asc
                                Return xPayPlanId.CompareTo(yPayPlanId)
                            Else
                                'string text desc
                                Return yPayPlanId.CompareTo(xPayPlanId)
                            End If
                        End If
                    Case QuickQuotePaymentOption.SortBy.FriendlyDescriptionAscending, QuickQuotePaymentOption.SortBy.FriendlyDescriptionDescending
                        Dim xFriendlyDesc As String = x.FriendlyDescription
                        Dim yFriendlyDesc As String = y.FriendlyDescription
                        If String.IsNullOrWhiteSpace(xFriendlyDesc) = True AndAlso String.IsNullOrWhiteSpace(yFriendlyDesc) = True Then
                            'same so far; continue below
                        ElseIf String.IsNullOrWhiteSpace(xFriendlyDesc) = True Then
                            If isAscendingSort = True Then
                                'asc
                                Return -1
                            Else
                                'desc
                                Return 1
                            End If
                        ElseIf String.IsNullOrWhiteSpace(yFriendlyDesc) = True Then
                            If isAscendingSort = True Then
                                'asc
                                Return 1
                            Else
                                'desc
                                Return -1
                            End If
                        Else
                            'Description is something for both
                            If UCase(xFriendlyDesc) = UCase(yFriendlyDesc) Then
                                'same so far; continue below
                            Else
                                If isAscendingSort = True Then
                                    'string text asc
                                    Return UCase(xFriendlyDesc).CompareTo(UCase(yFriendlyDesc))
                                Else
                                    'string text desc
                                    Return UCase(yFriendlyDesc).CompareTo(UCase(xFriendlyDesc))
                                End If
                            End If
                        End If
                End Select

                'added 9/8/2017 for secondary sort
                Select Case sortBy
                    Case QuickQuotePaymentOption.SortBy.NumberOfInstallmentsAscendingThenDescriptionAscending, QuickQuotePaymentOption.SortBy.NumberOfInstallmentsDescendingThenDescriptionAscending, QuickQuotePaymentOption.SortBy.NumberOfInstallmentsAscendingThenDescriptionDescending, QuickQuotePaymentOption.SortBy.NumberOfInstallmentsDescendingThenDescriptionDescending, QuickQuotePaymentOption.SortBy.NumberOfInstallmentsAscendingThenFriendlyDescriptionAscending, QuickQuotePaymentOption.SortBy.NumberOfInstallmentsDescendingThenFriendlyDescriptionAscending, QuickQuotePaymentOption.SortBy.NumberOfInstallmentsAscendingThenFriendlyDescriptionDescending, QuickQuotePaymentOption.SortBy.NumberOfInstallmentsDescendingThenFriendlyDescriptionDescending
                        Dim isAscendingSort2 As Boolean = If(sortBy = QuickQuotePaymentOption.SortBy.NumberOfInstallmentsAscendingThenDescriptionAscending OrElse sortBy = QuickQuotePaymentOption.SortBy.NumberOfInstallmentsDescendingThenDescriptionAscending OrElse sortBy = QuickQuotePaymentOption.SortBy.NumberOfInstallmentsAscendingThenFriendlyDescriptionAscending OrElse sortBy = QuickQuotePaymentOption.SortBy.NumberOfInstallmentsDescendingThenFriendlyDescriptionAscending, True, False)

                        If backupListPositionSortBy = QuickQuotePaymentOption.ListPositionSortBy.MatchLastSort Then
                            isBackupListPositionAscendingSort = isAscendingSort2
                        End If

                        Select Case sortBy
                            Case QuickQuotePaymentOption.SortBy.NumberOfInstallmentsAscendingThenDescriptionAscending, QuickQuotePaymentOption.SortBy.NumberOfInstallmentsDescendingThenDescriptionAscending, QuickQuotePaymentOption.SortBy.NumberOfInstallmentsAscendingThenDescriptionDescending, QuickQuotePaymentOption.SortBy.NumberOfInstallmentsDescendingThenDescriptionDescending
                                If String.IsNullOrWhiteSpace(x.Description) = True AndAlso String.IsNullOrWhiteSpace(y.Description) = True Then
                                    'same so far; continue below
                                ElseIf String.IsNullOrWhiteSpace(x.Description) = True Then
                                    If isAscendingSort2 = True Then
                                        'asc
                                        Return -1
                                    Else
                                        'desc
                                        Return 1
                                    End If
                                ElseIf String.IsNullOrWhiteSpace(y.Description) = True Then
                                    If isAscendingSort2 = True Then
                                        'asc
                                        Return 1
                                    Else
                                        'desc
                                        Return -1
                                    End If
                                Else
                                    'Description is something for both
                                    If UCase(x.Description) = UCase(y.Description) Then
                                        'same so far; continue below
                                    Else
                                        If isAscendingSort2 = True Then
                                            'string text asc
                                            Return UCase(x.Description).CompareTo(UCase(y.Description))
                                        Else
                                            'string text desc
                                            Return UCase(y.Description).CompareTo(UCase(x.Description))
                                        End If
                                    End If
                                End If
                            Case QuickQuotePaymentOption.SortBy.NumberOfInstallmentsAscendingThenFriendlyDescriptionAscending, QuickQuotePaymentOption.SortBy.NumberOfInstallmentsDescendingThenFriendlyDescriptionAscending, QuickQuotePaymentOption.SortBy.NumberOfInstallmentsAscendingThenFriendlyDescriptionDescending, QuickQuotePaymentOption.SortBy.NumberOfInstallmentsDescendingThenFriendlyDescriptionDescending
                                Dim xFriendlyDesc As String = x.FriendlyDescription
                                Dim yFriendlyDesc As String = y.FriendlyDescription
                                If String.IsNullOrWhiteSpace(xFriendlyDesc) = True AndAlso String.IsNullOrWhiteSpace(yFriendlyDesc) = True Then
                                    'same so far; continue below
                                ElseIf String.IsNullOrWhiteSpace(xFriendlyDesc) = True Then
                                    If isAscendingSort2 = True Then
                                        'asc
                                        Return -1
                                    Else
                                        'desc
                                        Return 1
                                    End If
                                ElseIf String.IsNullOrWhiteSpace(yFriendlyDesc) = True Then
                                    If isAscendingSort2 = True Then
                                        'asc
                                        Return 1
                                    Else
                                        'desc
                                        Return -1
                                    End If
                                Else
                                    'Description is something for both
                                    If UCase(xFriendlyDesc) = UCase(yFriendlyDesc) Then
                                        'same so far; continue below
                                    Else
                                        If isAscendingSort2 = True Then
                                            'string text asc
                                            Return UCase(xFriendlyDesc).CompareTo(UCase(yFriendlyDesc))
                                        Else
                                            'string text desc
                                            Return UCase(yFriendlyDesc).CompareTo(UCase(xFriendlyDesc))
                                        End If
                                    End If
                                End If
                        End Select
                End Select

                'looks the same so far; try to compare ListPosition number
                'If isAscendingSort = True Then
                'updated 9/8/2017
                If isBackupListPositionAscendingSort = True Then
                    'ListPosition int asc
                    Return x.ListPosition.CompareTo(y.ListPosition)
                Else
                    'ListPosition int desc
                    Return y.ListPosition.CompareTo(x.ListPosition)
                End If
            End If
        End Function
    End Class

    Public Class QuickQuotePaymentOptionComparer_DescriptionAscending
        Inherits QuickQuotePaymentOptionComparer
        Implements IComparer(Of QuickQuotePaymentOption)

        Public Function Compare1(x As QuickQuotePaymentOption, y As QuickQuotePaymentOption) As Integer Implements System.Collections.Generic.IComparer(Of QuickQuotePaymentOption).Compare
            Return MyBase.Compare(x, y, QuickQuotePaymentOption.SortBy.DescriptionAscending)
        End Function
    End Class

    Public Class QuickQuotePaymentOptionComparer_DescriptionDescending
        Inherits QuickQuotePaymentOptionComparer
        Implements IComparer(Of QuickQuotePaymentOption)

        Public Function Compare1(x As QuickQuotePaymentOption, y As QuickQuotePaymentOption) As Integer Implements System.Collections.Generic.IComparer(Of QuickQuotePaymentOption).Compare
            Return MyBase.Compare(x, y, QuickQuotePaymentOption.SortBy.DescriptionDescending)
        End Function
    End Class

    Public Class QuickQuotePaymentOptionComparer_NumberOfInstallmentsAscending
        Inherits QuickQuotePaymentOptionComparer
        Implements IComparer(Of QuickQuotePaymentOption)

        Public Function Compare1(x As QuickQuotePaymentOption, y As QuickQuotePaymentOption) As Integer Implements System.Collections.Generic.IComparer(Of QuickQuotePaymentOption).Compare
            Return MyBase.Compare(x, y, QuickQuotePaymentOption.SortBy.NumberOfInstallmentsAscending)
        End Function
    End Class

    Public Class QuickQuotePaymentOptionComparer_NumberOfInstallmentsDescending
        Inherits QuickQuotePaymentOptionComparer
        Implements IComparer(Of QuickQuotePaymentOption)

        Public Function Compare1(x As QuickQuotePaymentOption, y As QuickQuotePaymentOption) As Integer Implements System.Collections.Generic.IComparer(Of QuickQuotePaymentOption).Compare
            Return MyBase.Compare(x, y, QuickQuotePaymentOption.SortBy.NumberOfInstallmentsDescending)
        End Function
    End Class

    Public Class QuickQuotePaymentOptionComparer_PayPlanIdAscending
        Inherits QuickQuotePaymentOptionComparer
        Implements IComparer(Of QuickQuotePaymentOption)

        Public Function Compare1(x As QuickQuotePaymentOption, y As QuickQuotePaymentOption) As Integer Implements System.Collections.Generic.IComparer(Of QuickQuotePaymentOption).Compare
            Return MyBase.Compare(x, y, QuickQuotePaymentOption.SortBy.PayPlanIdAscending)
        End Function
    End Class

    Public Class QuickQuotePaymentOptionComparer_PayPlanIdDescending
        Inherits QuickQuotePaymentOptionComparer
        Implements IComparer(Of QuickQuotePaymentOption)

        Public Function Compare1(x As QuickQuotePaymentOption, y As QuickQuotePaymentOption) As Integer Implements System.Collections.Generic.IComparer(Of QuickQuotePaymentOption).Compare
            Return MyBase.Compare(x, y, QuickQuotePaymentOption.SortBy.PayPlanIdDescending)
        End Function
    End Class

    'added 9/7/2017
    Public Class QuickQuotePaymentOptionComparer_FriendlyDescriptionAscending
        Inherits QuickQuotePaymentOptionComparer
        Implements IComparer(Of QuickQuotePaymentOption)

        Public Function Compare1(x As QuickQuotePaymentOption, y As QuickQuotePaymentOption) As Integer Implements System.Collections.Generic.IComparer(Of QuickQuotePaymentOption).Compare
            Return MyBase.Compare(x, y, QuickQuotePaymentOption.SortBy.FriendlyDescriptionAscending)
        End Function
    End Class

    Public Class QuickQuotePaymentOptionComparer_FriendlyDescriptionDescending
        Inherits QuickQuotePaymentOptionComparer
        Implements IComparer(Of QuickQuotePaymentOption)

        Public Function Compare1(x As QuickQuotePaymentOption, y As QuickQuotePaymentOption) As Integer Implements System.Collections.Generic.IComparer(Of QuickQuotePaymentOption).Compare
            Return MyBase.Compare(x, y, QuickQuotePaymentOption.SortBy.FriendlyDescriptionDescending)
        End Function
    End Class

    'added 9/8/2017
    Public Class QuickQuotePaymentOptionComparer_NumberOfInstallmentsAscendingThenDescriptionAscending
        Inherits QuickQuotePaymentOptionComparer
        Implements IComparer(Of QuickQuotePaymentOption)

        Public Function Compare1(x As QuickQuotePaymentOption, y As QuickQuotePaymentOption) As Integer Implements System.Collections.Generic.IComparer(Of QuickQuotePaymentOption).Compare
            Return MyBase.Compare(x, y, QuickQuotePaymentOption.SortBy.NumberOfInstallmentsAscendingThenDescriptionAscending)
        End Function
    End Class

    Public Class QuickQuotePaymentOptionComparer_NumberOfInstallmentsDescendingThenDescriptionAscending
        Inherits QuickQuotePaymentOptionComparer
        Implements IComparer(Of QuickQuotePaymentOption)

        Public Function Compare1(x As QuickQuotePaymentOption, y As QuickQuotePaymentOption) As Integer Implements System.Collections.Generic.IComparer(Of QuickQuotePaymentOption).Compare
            Return MyBase.Compare(x, y, QuickQuotePaymentOption.SortBy.NumberOfInstallmentsDescendingThenDescriptionAscending)
        End Function
    End Class

    Public Class QuickQuotePaymentOptionComparer_NumberOfInstallmentsAscendingThenDescriptionDescending
        Inherits QuickQuotePaymentOptionComparer
        Implements IComparer(Of QuickQuotePaymentOption)

        Public Function Compare1(x As QuickQuotePaymentOption, y As QuickQuotePaymentOption) As Integer Implements System.Collections.Generic.IComparer(Of QuickQuotePaymentOption).Compare
            Return MyBase.Compare(x, y, QuickQuotePaymentOption.SortBy.NumberOfInstallmentsAscendingThenDescriptionDescending)
        End Function
    End Class

    Public Class QuickQuotePaymentOptionComparer_NumberOfInstallmentsDescendingThenDescriptionDescending
        Inherits QuickQuotePaymentOptionComparer
        Implements IComparer(Of QuickQuotePaymentOption)

        Public Function Compare1(x As QuickQuotePaymentOption, y As QuickQuotePaymentOption) As Integer Implements System.Collections.Generic.IComparer(Of QuickQuotePaymentOption).Compare
            Return MyBase.Compare(x, y, QuickQuotePaymentOption.SortBy.NumberOfInstallmentsDescendingThenDescriptionDescending)
        End Function
    End Class

    Public Class QuickQuotePaymentOptionComparer_NumberOfInstallmentsAscendingThenFriendlyDescriptionAscending
        Inherits QuickQuotePaymentOptionComparer
        Implements IComparer(Of QuickQuotePaymentOption)

        Public Function Compare1(x As QuickQuotePaymentOption, y As QuickQuotePaymentOption) As Integer Implements System.Collections.Generic.IComparer(Of QuickQuotePaymentOption).Compare
            Return MyBase.Compare(x, y, QuickQuotePaymentOption.SortBy.NumberOfInstallmentsAscendingThenFriendlyDescriptionAscending)
        End Function
    End Class

    Public Class QuickQuotePaymentOptionComparer_NumberOfInstallmentsDescendingThenFriendlyDescriptionAscending
        Inherits QuickQuotePaymentOptionComparer
        Implements IComparer(Of QuickQuotePaymentOption)

        Public Function Compare1(x As QuickQuotePaymentOption, y As QuickQuotePaymentOption) As Integer Implements System.Collections.Generic.IComparer(Of QuickQuotePaymentOption).Compare
            Return MyBase.Compare(x, y, QuickQuotePaymentOption.SortBy.NumberOfInstallmentsDescendingThenFriendlyDescriptionAscending)
        End Function
    End Class

    Public Class QuickQuotePaymentOptionComparer_NumberOfInstallmentsAscendingThenFriendlyDescriptionDescending
        Inherits QuickQuotePaymentOptionComparer
        Implements IComparer(Of QuickQuotePaymentOption)

        Public Function Compare1(x As QuickQuotePaymentOption, y As QuickQuotePaymentOption) As Integer Implements System.Collections.Generic.IComparer(Of QuickQuotePaymentOption).Compare
            Return MyBase.Compare(x, y, QuickQuotePaymentOption.SortBy.NumberOfInstallmentsAscendingThenFriendlyDescriptionDescending)
        End Function
    End Class

    Public Class QuickQuotePaymentOptionComparer_NumberOfInstallmentsDescendingThenFriendlyDescriptionDescending
        Inherits QuickQuotePaymentOptionComparer
        Implements IComparer(Of QuickQuotePaymentOption)

        Public Function Compare1(x As QuickQuotePaymentOption, y As QuickQuotePaymentOption) As Integer Implements System.Collections.Generic.IComparer(Of QuickQuotePaymentOption).Compare
            Return MyBase.Compare(x, y, QuickQuotePaymentOption.SortBy.NumberOfInstallmentsDescendingThenFriendlyDescriptionDescending)
        End Function
    End Class

    'added 9/8/2017 for backup listPosition sort
    Public Class QuickQuotePaymentOptionComparer_DescriptionAscending_WithBackupListPositionAscending
        Inherits QuickQuotePaymentOptionComparer
        Implements IComparer(Of QuickQuotePaymentOption)

        Public Function Compare1(x As QuickQuotePaymentOption, y As QuickQuotePaymentOption) As Integer Implements System.Collections.Generic.IComparer(Of QuickQuotePaymentOption).Compare
            Return MyBase.Compare(x, y, QuickQuotePaymentOption.SortBy.DescriptionAscending, backupListPositionSortBy:=QuickQuotePaymentOption.ListPositionSortBy.Ascending)
        End Function
    End Class
    Public Class QuickQuotePaymentOptionComparer_DescriptionDescending_WithBackupListPositionAscending
        Inherits QuickQuotePaymentOptionComparer
        Implements IComparer(Of QuickQuotePaymentOption)

        Public Function Compare1(x As QuickQuotePaymentOption, y As QuickQuotePaymentOption) As Integer Implements System.Collections.Generic.IComparer(Of QuickQuotePaymentOption).Compare
            Return MyBase.Compare(x, y, QuickQuotePaymentOption.SortBy.DescriptionDescending, backupListPositionSortBy:=QuickQuotePaymentOption.ListPositionSortBy.Ascending)
        End Function
    End Class
    Public Class QuickQuotePaymentOptionComparer_NumberOfInstallmentsAscending_WithBackupListPositionAscending
        Inherits QuickQuotePaymentOptionComparer
        Implements IComparer(Of QuickQuotePaymentOption)

        Public Function Compare1(x As QuickQuotePaymentOption, y As QuickQuotePaymentOption) As Integer Implements System.Collections.Generic.IComparer(Of QuickQuotePaymentOption).Compare
            Return MyBase.Compare(x, y, QuickQuotePaymentOption.SortBy.NumberOfInstallmentsAscending, backupListPositionSortBy:=QuickQuotePaymentOption.ListPositionSortBy.Ascending)
        End Function
    End Class
    Public Class QuickQuotePaymentOptionComparer_NumberOfInstallmentsDescending_WithBackupListPositionAscending
        Inherits QuickQuotePaymentOptionComparer
        Implements IComparer(Of QuickQuotePaymentOption)

        Public Function Compare1(x As QuickQuotePaymentOption, y As QuickQuotePaymentOption) As Integer Implements System.Collections.Generic.IComparer(Of QuickQuotePaymentOption).Compare
            Return MyBase.Compare(x, y, QuickQuotePaymentOption.SortBy.NumberOfInstallmentsDescending, backupListPositionSortBy:=QuickQuotePaymentOption.ListPositionSortBy.Ascending)
        End Function
    End Class
    Public Class QuickQuotePaymentOptionComparer_PayPlanIdAscending_WithBackupListPositionAscending
        Inherits QuickQuotePaymentOptionComparer
        Implements IComparer(Of QuickQuotePaymentOption)

        Public Function Compare1(x As QuickQuotePaymentOption, y As QuickQuotePaymentOption) As Integer Implements System.Collections.Generic.IComparer(Of QuickQuotePaymentOption).Compare
            Return MyBase.Compare(x, y, QuickQuotePaymentOption.SortBy.PayPlanIdAscending, backupListPositionSortBy:=QuickQuotePaymentOption.ListPositionSortBy.Ascending)
        End Function
    End Class
    Public Class QuickQuotePaymentOptionComparer_PayPlanIdDescending_WithBackupListPositionAscending
        Inherits QuickQuotePaymentOptionComparer
        Implements IComparer(Of QuickQuotePaymentOption)

        Public Function Compare1(x As QuickQuotePaymentOption, y As QuickQuotePaymentOption) As Integer Implements System.Collections.Generic.IComparer(Of QuickQuotePaymentOption).Compare
            Return MyBase.Compare(x, y, QuickQuotePaymentOption.SortBy.PayPlanIdDescending, backupListPositionSortBy:=QuickQuotePaymentOption.ListPositionSortBy.Ascending)
        End Function
    End Class
    Public Class QuickQuotePaymentOptionComparer_FriendlyDescriptionAscending_WithBackupListPositionAscending
        Inherits QuickQuotePaymentOptionComparer
        Implements IComparer(Of QuickQuotePaymentOption)

        Public Function Compare1(x As QuickQuotePaymentOption, y As QuickQuotePaymentOption) As Integer Implements System.Collections.Generic.IComparer(Of QuickQuotePaymentOption).Compare
            Return MyBase.Compare(x, y, QuickQuotePaymentOption.SortBy.FriendlyDescriptionAscending, backupListPositionSortBy:=QuickQuotePaymentOption.ListPositionSortBy.Ascending)
        End Function
    End Class
    Public Class QuickQuotePaymentOptionComparer_FriendlyDescriptionDescending_WithBackupListPositionAscending
        Inherits QuickQuotePaymentOptionComparer
        Implements IComparer(Of QuickQuotePaymentOption)

        Public Function Compare1(x As QuickQuotePaymentOption, y As QuickQuotePaymentOption) As Integer Implements System.Collections.Generic.IComparer(Of QuickQuotePaymentOption).Compare
            Return MyBase.Compare(x, y, QuickQuotePaymentOption.SortBy.FriendlyDescriptionDescending, backupListPositionSortBy:=QuickQuotePaymentOption.ListPositionSortBy.Ascending)
        End Function
    End Class
    Public Class QuickQuotePaymentOptionComparer_NumberOfInstallmentsAscendingThenDescriptionAscending_WithBackupListPositionAscending
        Inherits QuickQuotePaymentOptionComparer
        Implements IComparer(Of QuickQuotePaymentOption)

        Public Function Compare1(x As QuickQuotePaymentOption, y As QuickQuotePaymentOption) As Integer Implements System.Collections.Generic.IComparer(Of QuickQuotePaymentOption).Compare
            Return MyBase.Compare(x, y, QuickQuotePaymentOption.SortBy.NumberOfInstallmentsAscendingThenDescriptionAscending, backupListPositionSortBy:=QuickQuotePaymentOption.ListPositionSortBy.Ascending)
        End Function
    End Class
    Public Class QuickQuotePaymentOptionComparer_NumberOfInstallmentsDescendingThenDescriptionAscending_WithBackupListPositionAscending
        Inherits QuickQuotePaymentOptionComparer
        Implements IComparer(Of QuickQuotePaymentOption)

        Public Function Compare1(x As QuickQuotePaymentOption, y As QuickQuotePaymentOption) As Integer Implements System.Collections.Generic.IComparer(Of QuickQuotePaymentOption).Compare
            Return MyBase.Compare(x, y, QuickQuotePaymentOption.SortBy.NumberOfInstallmentsDescendingThenDescriptionAscending, backupListPositionSortBy:=QuickQuotePaymentOption.ListPositionSortBy.Ascending)
        End Function
    End Class
    Public Class QuickQuotePaymentOptionComparer_NumberOfInstallmentsAscendingThenDescriptionDescending_WithBackupListPositionAscending
        Inherits QuickQuotePaymentOptionComparer
        Implements IComparer(Of QuickQuotePaymentOption)

        Public Function Compare1(x As QuickQuotePaymentOption, y As QuickQuotePaymentOption) As Integer Implements System.Collections.Generic.IComparer(Of QuickQuotePaymentOption).Compare
            Return MyBase.Compare(x, y, QuickQuotePaymentOption.SortBy.NumberOfInstallmentsAscendingThenDescriptionDescending, backupListPositionSortBy:=QuickQuotePaymentOption.ListPositionSortBy.Ascending)
        End Function
    End Class
    Public Class QuickQuotePaymentOptionComparer_NumberOfInstallmentsDescendingThenDescriptionDescending_WithBackupListPositionAscending
        Inherits QuickQuotePaymentOptionComparer
        Implements IComparer(Of QuickQuotePaymentOption)

        Public Function Compare1(x As QuickQuotePaymentOption, y As QuickQuotePaymentOption) As Integer Implements System.Collections.Generic.IComparer(Of QuickQuotePaymentOption).Compare
            Return MyBase.Compare(x, y, QuickQuotePaymentOption.SortBy.NumberOfInstallmentsDescendingThenDescriptionDescending, backupListPositionSortBy:=QuickQuotePaymentOption.ListPositionSortBy.Ascending)
        End Function
    End Class
    Public Class QuickQuotePaymentOptionComparer_NumberOfInstallmentsAscendingThenFriendlyDescriptionAscending_WithBackupListPositionAscending
        Inherits QuickQuotePaymentOptionComparer
        Implements IComparer(Of QuickQuotePaymentOption)

        Public Function Compare1(x As QuickQuotePaymentOption, y As QuickQuotePaymentOption) As Integer Implements System.Collections.Generic.IComparer(Of QuickQuotePaymentOption).Compare
            Return MyBase.Compare(x, y, QuickQuotePaymentOption.SortBy.NumberOfInstallmentsAscendingThenFriendlyDescriptionAscending, backupListPositionSortBy:=QuickQuotePaymentOption.ListPositionSortBy.Ascending)
        End Function
    End Class
    Public Class QuickQuotePaymentOptionComparer_NumberOfInstallmentsDescendingThenFriendlyDescriptionAscending_WithBackupListPositionAscending
        Inherits QuickQuotePaymentOptionComparer
        Implements IComparer(Of QuickQuotePaymentOption)

        Public Function Compare1(x As QuickQuotePaymentOption, y As QuickQuotePaymentOption) As Integer Implements System.Collections.Generic.IComparer(Of QuickQuotePaymentOption).Compare
            Return MyBase.Compare(x, y, QuickQuotePaymentOption.SortBy.NumberOfInstallmentsDescendingThenFriendlyDescriptionAscending, backupListPositionSortBy:=QuickQuotePaymentOption.ListPositionSortBy.Ascending)
        End Function
    End Class
    Public Class QuickQuotePaymentOptionComparer_NumberOfInstallmentsAscendingThenFriendlyDescriptionDescending_WithBackupListPositionAscending
        Inherits QuickQuotePaymentOptionComparer
        Implements IComparer(Of QuickQuotePaymentOption)

        Public Function Compare1(x As QuickQuotePaymentOption, y As QuickQuotePaymentOption) As Integer Implements System.Collections.Generic.IComparer(Of QuickQuotePaymentOption).Compare
            Return MyBase.Compare(x, y, QuickQuotePaymentOption.SortBy.NumberOfInstallmentsAscendingThenFriendlyDescriptionDescending, backupListPositionSortBy:=QuickQuotePaymentOption.ListPositionSortBy.Ascending)
        End Function
    End Class
    Public Class QuickQuotePaymentOptionComparer_NumberOfInstallmentsDescendingThenFriendlyDescriptionDescending_WithBackupListPositionAscending
        Inherits QuickQuotePaymentOptionComparer
        Implements IComparer(Of QuickQuotePaymentOption)

        Public Function Compare1(x As QuickQuotePaymentOption, y As QuickQuotePaymentOption) As Integer Implements System.Collections.Generic.IComparer(Of QuickQuotePaymentOption).Compare
            Return MyBase.Compare(x, y, QuickQuotePaymentOption.SortBy.NumberOfInstallmentsDescendingThenFriendlyDescriptionDescending, backupListPositionSortBy:=QuickQuotePaymentOption.ListPositionSortBy.Ascending)
        End Function
    End Class

    Public Class QuickQuotePaymentOptionComparer_DescriptionAscending_WithBackupListPositionDescending
        Inherits QuickQuotePaymentOptionComparer
        Implements IComparer(Of QuickQuotePaymentOption)

        Public Function Compare1(x As QuickQuotePaymentOption, y As QuickQuotePaymentOption) As Integer Implements System.Collections.Generic.IComparer(Of QuickQuotePaymentOption).Compare
            Return MyBase.Compare(x, y, QuickQuotePaymentOption.SortBy.DescriptionAscending, backupListPositionSortBy:=QuickQuotePaymentOption.ListPositionSortBy.Descending)
        End Function
    End Class
    Public Class QuickQuotePaymentOptionComparer_DescriptionDescending_WithBackupListPositionDescending
        Inherits QuickQuotePaymentOptionComparer
        Implements IComparer(Of QuickQuotePaymentOption)

        Public Function Compare1(x As QuickQuotePaymentOption, y As QuickQuotePaymentOption) As Integer Implements System.Collections.Generic.IComparer(Of QuickQuotePaymentOption).Compare
            Return MyBase.Compare(x, y, QuickQuotePaymentOption.SortBy.DescriptionDescending, backupListPositionSortBy:=QuickQuotePaymentOption.ListPositionSortBy.Descending)
        End Function
    End Class
    Public Class QuickQuotePaymentOptionComparer_NumberOfInstallmentsAscending_WithBackupListPositionDescending
        Inherits QuickQuotePaymentOptionComparer
        Implements IComparer(Of QuickQuotePaymentOption)

        Public Function Compare1(x As QuickQuotePaymentOption, y As QuickQuotePaymentOption) As Integer Implements System.Collections.Generic.IComparer(Of QuickQuotePaymentOption).Compare
            Return MyBase.Compare(x, y, QuickQuotePaymentOption.SortBy.NumberOfInstallmentsAscending, backupListPositionSortBy:=QuickQuotePaymentOption.ListPositionSortBy.Descending)
        End Function
    End Class
    Public Class QuickQuotePaymentOptionComparer_NumberOfInstallmentsDescending_WithBackupListPositionDescending
        Inherits QuickQuotePaymentOptionComparer
        Implements IComparer(Of QuickQuotePaymentOption)

        Public Function Compare1(x As QuickQuotePaymentOption, y As QuickQuotePaymentOption) As Integer Implements System.Collections.Generic.IComparer(Of QuickQuotePaymentOption).Compare
            Return MyBase.Compare(x, y, QuickQuotePaymentOption.SortBy.NumberOfInstallmentsDescending, backupListPositionSortBy:=QuickQuotePaymentOption.ListPositionSortBy.Descending)
        End Function
    End Class
    Public Class QuickQuotePaymentOptionComparer_PayPlanIdAscending_WithBackupListPositionDescending
        Inherits QuickQuotePaymentOptionComparer
        Implements IComparer(Of QuickQuotePaymentOption)

        Public Function Compare1(x As QuickQuotePaymentOption, y As QuickQuotePaymentOption) As Integer Implements System.Collections.Generic.IComparer(Of QuickQuotePaymentOption).Compare
            Return MyBase.Compare(x, y, QuickQuotePaymentOption.SortBy.PayPlanIdAscending, backupListPositionSortBy:=QuickQuotePaymentOption.ListPositionSortBy.Descending)
        End Function
    End Class
    Public Class QuickQuotePaymentOptionComparer_PayPlanIdDescending_WithBackupListPositionDescending
        Inherits QuickQuotePaymentOptionComparer
        Implements IComparer(Of QuickQuotePaymentOption)

        Public Function Compare1(x As QuickQuotePaymentOption, y As QuickQuotePaymentOption) As Integer Implements System.Collections.Generic.IComparer(Of QuickQuotePaymentOption).Compare
            Return MyBase.Compare(x, y, QuickQuotePaymentOption.SortBy.PayPlanIdDescending, backupListPositionSortBy:=QuickQuotePaymentOption.ListPositionSortBy.Descending)
        End Function
    End Class
    Public Class QuickQuotePaymentOptionComparer_FriendlyDescriptionAscending_WithBackupListPositionDescending
        Inherits QuickQuotePaymentOptionComparer
        Implements IComparer(Of QuickQuotePaymentOption)

        Public Function Compare1(x As QuickQuotePaymentOption, y As QuickQuotePaymentOption) As Integer Implements System.Collections.Generic.IComparer(Of QuickQuotePaymentOption).Compare
            Return MyBase.Compare(x, y, QuickQuotePaymentOption.SortBy.FriendlyDescriptionAscending, backupListPositionSortBy:=QuickQuotePaymentOption.ListPositionSortBy.Descending)
        End Function
    End Class
    Public Class QuickQuotePaymentOptionComparer_FriendlyDescriptionDescending_WithBackupListPositionDescending
        Inherits QuickQuotePaymentOptionComparer
        Implements IComparer(Of QuickQuotePaymentOption)

        Public Function Compare1(x As QuickQuotePaymentOption, y As QuickQuotePaymentOption) As Integer Implements System.Collections.Generic.IComparer(Of QuickQuotePaymentOption).Compare
            Return MyBase.Compare(x, y, QuickQuotePaymentOption.SortBy.FriendlyDescriptionDescending, backupListPositionSortBy:=QuickQuotePaymentOption.ListPositionSortBy.Descending)
        End Function
    End Class
    Public Class QuickQuotePaymentOptionComparer_NumberOfInstallmentsAscendingThenDescriptionAscending_WithBackupListPositionDescending
        Inherits QuickQuotePaymentOptionComparer
        Implements IComparer(Of QuickQuotePaymentOption)

        Public Function Compare1(x As QuickQuotePaymentOption, y As QuickQuotePaymentOption) As Integer Implements System.Collections.Generic.IComparer(Of QuickQuotePaymentOption).Compare
            Return MyBase.Compare(x, y, QuickQuotePaymentOption.SortBy.NumberOfInstallmentsAscendingThenDescriptionAscending, backupListPositionSortBy:=QuickQuotePaymentOption.ListPositionSortBy.Descending)
        End Function
    End Class
    Public Class QuickQuotePaymentOptionComparer_NumberOfInstallmentsDescendingThenDescriptionAscending_WithBackupListPositionDescending
        Inherits QuickQuotePaymentOptionComparer
        Implements IComparer(Of QuickQuotePaymentOption)

        Public Function Compare1(x As QuickQuotePaymentOption, y As QuickQuotePaymentOption) As Integer Implements System.Collections.Generic.IComparer(Of QuickQuotePaymentOption).Compare
            Return MyBase.Compare(x, y, QuickQuotePaymentOption.SortBy.NumberOfInstallmentsDescendingThenDescriptionAscending, backupListPositionSortBy:=QuickQuotePaymentOption.ListPositionSortBy.Descending)
        End Function
    End Class
    Public Class QuickQuotePaymentOptionComparer_NumberOfInstallmentsAscendingThenDescriptionDescending_WithBackupListPositionDescending
        Inherits QuickQuotePaymentOptionComparer
        Implements IComparer(Of QuickQuotePaymentOption)

        Public Function Compare1(x As QuickQuotePaymentOption, y As QuickQuotePaymentOption) As Integer Implements System.Collections.Generic.IComparer(Of QuickQuotePaymentOption).Compare
            Return MyBase.Compare(x, y, QuickQuotePaymentOption.SortBy.NumberOfInstallmentsAscendingThenDescriptionDescending, backupListPositionSortBy:=QuickQuotePaymentOption.ListPositionSortBy.Descending)
        End Function
    End Class
    Public Class QuickQuotePaymentOptionComparer_NumberOfInstallmentsDescendingThenDescriptionDescending_WithBackupListPositionDescending
        Inherits QuickQuotePaymentOptionComparer
        Implements IComparer(Of QuickQuotePaymentOption)

        Public Function Compare1(x As QuickQuotePaymentOption, y As QuickQuotePaymentOption) As Integer Implements System.Collections.Generic.IComparer(Of QuickQuotePaymentOption).Compare
            Return MyBase.Compare(x, y, QuickQuotePaymentOption.SortBy.NumberOfInstallmentsDescendingThenDescriptionDescending, backupListPositionSortBy:=QuickQuotePaymentOption.ListPositionSortBy.Descending)
        End Function
    End Class
    Public Class QuickQuotePaymentOptionComparer_NumberOfInstallmentsAscendingThenFriendlyDescriptionAscending_WithBackupListPositionDescending
        Inherits QuickQuotePaymentOptionComparer
        Implements IComparer(Of QuickQuotePaymentOption)

        Public Function Compare1(x As QuickQuotePaymentOption, y As QuickQuotePaymentOption) As Integer Implements System.Collections.Generic.IComparer(Of QuickQuotePaymentOption).Compare
            Return MyBase.Compare(x, y, QuickQuotePaymentOption.SortBy.NumberOfInstallmentsAscendingThenFriendlyDescriptionAscending, backupListPositionSortBy:=QuickQuotePaymentOption.ListPositionSortBy.Descending)
        End Function
    End Class
    Public Class QuickQuotePaymentOptionComparer_NumberOfInstallmentsDescendingThenFriendlyDescriptionAscending_WithBackupListPositionDescending
        Inherits QuickQuotePaymentOptionComparer
        Implements IComparer(Of QuickQuotePaymentOption)

        Public Function Compare1(x As QuickQuotePaymentOption, y As QuickQuotePaymentOption) As Integer Implements System.Collections.Generic.IComparer(Of QuickQuotePaymentOption).Compare
            Return MyBase.Compare(x, y, QuickQuotePaymentOption.SortBy.NumberOfInstallmentsDescendingThenFriendlyDescriptionAscending, backupListPositionSortBy:=QuickQuotePaymentOption.ListPositionSortBy.Descending)
        End Function
    End Class
    Public Class QuickQuotePaymentOptionComparer_NumberOfInstallmentsAscendingThenFriendlyDescriptionDescending_WithBackupListPositionDescending
        Inherits QuickQuotePaymentOptionComparer
        Implements IComparer(Of QuickQuotePaymentOption)

        Public Function Compare1(x As QuickQuotePaymentOption, y As QuickQuotePaymentOption) As Integer Implements System.Collections.Generic.IComparer(Of QuickQuotePaymentOption).Compare
            Return MyBase.Compare(x, y, QuickQuotePaymentOption.SortBy.NumberOfInstallmentsAscendingThenFriendlyDescriptionDescending, backupListPositionSortBy:=QuickQuotePaymentOption.ListPositionSortBy.Descending)
        End Function
    End Class
    Public Class QuickQuotePaymentOptionComparer_NumberOfInstallmentsDescendingThenFriendlyDescriptionDescending_WithBackupListPositionDescending
        Inherits QuickQuotePaymentOptionComparer
        Implements IComparer(Of QuickQuotePaymentOption)

        Public Function Compare1(x As QuickQuotePaymentOption, y As QuickQuotePaymentOption) As Integer Implements System.Collections.Generic.IComparer(Of QuickQuotePaymentOption).Compare
            Return MyBase.Compare(x, y, QuickQuotePaymentOption.SortBy.NumberOfInstallmentsDescendingThenFriendlyDescriptionDescending, backupListPositionSortBy:=QuickQuotePaymentOption.ListPositionSortBy.Descending)
        End Function
    End Class

    Public Class QuickQuotePaymentOptionComparer_DescriptionAscending_WithMatchingSortOnBackupListPosition
        Inherits QuickQuotePaymentOptionComparer
        Implements IComparer(Of QuickQuotePaymentOption)

        Public Function Compare1(x As QuickQuotePaymentOption, y As QuickQuotePaymentOption) As Integer Implements System.Collections.Generic.IComparer(Of QuickQuotePaymentOption).Compare
            Return MyBase.Compare(x, y, QuickQuotePaymentOption.SortBy.DescriptionAscending, backupListPositionSortBy:=QuickQuotePaymentOption.ListPositionSortBy.MatchLastSort)
        End Function
    End Class
    Public Class QuickQuotePaymentOptionComparer_DescriptionDescending_WithMatchingSortOnBackupListPosition
        Inherits QuickQuotePaymentOptionComparer
        Implements IComparer(Of QuickQuotePaymentOption)

        Public Function Compare1(x As QuickQuotePaymentOption, y As QuickQuotePaymentOption) As Integer Implements System.Collections.Generic.IComparer(Of QuickQuotePaymentOption).Compare
            Return MyBase.Compare(x, y, QuickQuotePaymentOption.SortBy.DescriptionDescending, backupListPositionSortBy:=QuickQuotePaymentOption.ListPositionSortBy.MatchLastSort)
        End Function
    End Class
    Public Class QuickQuotePaymentOptionComparer_NumberOfInstallmentsAscending_WithMatchingSortOnBackupListPosition
        Inherits QuickQuotePaymentOptionComparer
        Implements IComparer(Of QuickQuotePaymentOption)

        Public Function Compare1(x As QuickQuotePaymentOption, y As QuickQuotePaymentOption) As Integer Implements System.Collections.Generic.IComparer(Of QuickQuotePaymentOption).Compare
            Return MyBase.Compare(x, y, QuickQuotePaymentOption.SortBy.NumberOfInstallmentsAscending, backupListPositionSortBy:=QuickQuotePaymentOption.ListPositionSortBy.MatchLastSort)
        End Function
    End Class
    Public Class QuickQuotePaymentOptionComparer_NumberOfInstallmentsDescending_WithMatchingSortOnBackupListPosition
        Inherits QuickQuotePaymentOptionComparer
        Implements IComparer(Of QuickQuotePaymentOption)

        Public Function Compare1(x As QuickQuotePaymentOption, y As QuickQuotePaymentOption) As Integer Implements System.Collections.Generic.IComparer(Of QuickQuotePaymentOption).Compare
            Return MyBase.Compare(x, y, QuickQuotePaymentOption.SortBy.NumberOfInstallmentsDescending, backupListPositionSortBy:=QuickQuotePaymentOption.ListPositionSortBy.MatchLastSort)
        End Function
    End Class
    Public Class QuickQuotePaymentOptionComparer_PayPlanIdAscending_WithMatchingSortOnBackupListPosition
        Inherits QuickQuotePaymentOptionComparer
        Implements IComparer(Of QuickQuotePaymentOption)

        Public Function Compare1(x As QuickQuotePaymentOption, y As QuickQuotePaymentOption) As Integer Implements System.Collections.Generic.IComparer(Of QuickQuotePaymentOption).Compare
            Return MyBase.Compare(x, y, QuickQuotePaymentOption.SortBy.PayPlanIdAscending, backupListPositionSortBy:=QuickQuotePaymentOption.ListPositionSortBy.MatchLastSort)
        End Function
    End Class
    Public Class QuickQuotePaymentOptionComparer_PayPlanIdDescending_WithMatchingSortOnBackupListPosition
        Inherits QuickQuotePaymentOptionComparer
        Implements IComparer(Of QuickQuotePaymentOption)

        Public Function Compare1(x As QuickQuotePaymentOption, y As QuickQuotePaymentOption) As Integer Implements System.Collections.Generic.IComparer(Of QuickQuotePaymentOption).Compare
            Return MyBase.Compare(x, y, QuickQuotePaymentOption.SortBy.PayPlanIdDescending, backupListPositionSortBy:=QuickQuotePaymentOption.ListPositionSortBy.MatchLastSort)
        End Function
    End Class
    Public Class QuickQuotePaymentOptionComparer_FriendlyDescriptionAscending_WithMatchingSortOnBackupListPosition
        Inherits QuickQuotePaymentOptionComparer
        Implements IComparer(Of QuickQuotePaymentOption)

        Public Function Compare1(x As QuickQuotePaymentOption, y As QuickQuotePaymentOption) As Integer Implements System.Collections.Generic.IComparer(Of QuickQuotePaymentOption).Compare
            Return MyBase.Compare(x, y, QuickQuotePaymentOption.SortBy.FriendlyDescriptionAscending, backupListPositionSortBy:=QuickQuotePaymentOption.ListPositionSortBy.MatchLastSort)
        End Function
    End Class
    Public Class QuickQuotePaymentOptionComparer_FriendlyDescriptionDescending_WithMatchingSortOnBackupListPosition
        Inherits QuickQuotePaymentOptionComparer
        Implements IComparer(Of QuickQuotePaymentOption)

        Public Function Compare1(x As QuickQuotePaymentOption, y As QuickQuotePaymentOption) As Integer Implements System.Collections.Generic.IComparer(Of QuickQuotePaymentOption).Compare
            Return MyBase.Compare(x, y, QuickQuotePaymentOption.SortBy.FriendlyDescriptionDescending, backupListPositionSortBy:=QuickQuotePaymentOption.ListPositionSortBy.MatchLastSort)
        End Function
    End Class

    Public Class QuickQuotePaymentOptionComparer_NumberOfInstallmentsAscendingThenDescriptionAscending_WithBackupListPositionMatchingInitialSort
        Inherits QuickQuotePaymentOptionComparer
        Implements IComparer(Of QuickQuotePaymentOption)

        Public Function Compare1(x As QuickQuotePaymentOption, y As QuickQuotePaymentOption) As Integer Implements System.Collections.Generic.IComparer(Of QuickQuotePaymentOption).Compare
            Return MyBase.Compare(x, y, QuickQuotePaymentOption.SortBy.NumberOfInstallmentsAscendingThenDescriptionAscending, backupListPositionSortBy:=QuickQuotePaymentOption.ListPositionSortBy.MatchInitialSort)
        End Function
    End Class
    Public Class QuickQuotePaymentOptionComparer_NumberOfInstallmentsDescendingThenDescriptionAscending_WithBackupListPositionMatchingInitialSort
        Inherits QuickQuotePaymentOptionComparer
        Implements IComparer(Of QuickQuotePaymentOption)

        Public Function Compare1(x As QuickQuotePaymentOption, y As QuickQuotePaymentOption) As Integer Implements System.Collections.Generic.IComparer(Of QuickQuotePaymentOption).Compare
            Return MyBase.Compare(x, y, QuickQuotePaymentOption.SortBy.NumberOfInstallmentsDescendingThenDescriptionAscending, backupListPositionSortBy:=QuickQuotePaymentOption.ListPositionSortBy.MatchInitialSort)
        End Function
    End Class
    Public Class QuickQuotePaymentOptionComparer_NumberOfInstallmentsAscendingThenDescriptionDescending_WithBackupListPositionMatchingInitialSort
        Inherits QuickQuotePaymentOptionComparer
        Implements IComparer(Of QuickQuotePaymentOption)

        Public Function Compare1(x As QuickQuotePaymentOption, y As QuickQuotePaymentOption) As Integer Implements System.Collections.Generic.IComparer(Of QuickQuotePaymentOption).Compare
            Return MyBase.Compare(x, y, QuickQuotePaymentOption.SortBy.NumberOfInstallmentsAscendingThenDescriptionDescending, backupListPositionSortBy:=QuickQuotePaymentOption.ListPositionSortBy.MatchInitialSort)
        End Function
    End Class
    Public Class QuickQuotePaymentOptionComparer_NumberOfInstallmentsDescendingThenDescriptionDescending_WithBackupListPositionMatchingInitialSort
        Inherits QuickQuotePaymentOptionComparer
        Implements IComparer(Of QuickQuotePaymentOption)

        Public Function Compare1(x As QuickQuotePaymentOption, y As QuickQuotePaymentOption) As Integer Implements System.Collections.Generic.IComparer(Of QuickQuotePaymentOption).Compare
            Return MyBase.Compare(x, y, QuickQuotePaymentOption.SortBy.NumberOfInstallmentsDescendingThenDescriptionDescending, backupListPositionSortBy:=QuickQuotePaymentOption.ListPositionSortBy.MatchInitialSort)
        End Function
    End Class
    Public Class QuickQuotePaymentOptionComparer_NumberOfInstallmentsAscendingThenFriendlyDescriptionAscending_WithBackupListPositionMatchingInitialSort
        Inherits QuickQuotePaymentOptionComparer
        Implements IComparer(Of QuickQuotePaymentOption)

        Public Function Compare1(x As QuickQuotePaymentOption, y As QuickQuotePaymentOption) As Integer Implements System.Collections.Generic.IComparer(Of QuickQuotePaymentOption).Compare
            Return MyBase.Compare(x, y, QuickQuotePaymentOption.SortBy.NumberOfInstallmentsAscendingThenFriendlyDescriptionAscending, backupListPositionSortBy:=QuickQuotePaymentOption.ListPositionSortBy.MatchInitialSort)
        End Function
    End Class
    Public Class QuickQuotePaymentOptionComparer_NumberOfInstallmentsDescendingThenFriendlyDescriptionAscending_WithBackupListPositionMatchingInitialSort
        Inherits QuickQuotePaymentOptionComparer
        Implements IComparer(Of QuickQuotePaymentOption)

        Public Function Compare1(x As QuickQuotePaymentOption, y As QuickQuotePaymentOption) As Integer Implements System.Collections.Generic.IComparer(Of QuickQuotePaymentOption).Compare
            Return MyBase.Compare(x, y, QuickQuotePaymentOption.SortBy.NumberOfInstallmentsDescendingThenFriendlyDescriptionAscending, backupListPositionSortBy:=QuickQuotePaymentOption.ListPositionSortBy.MatchInitialSort)
        End Function
    End Class
    Public Class QuickQuotePaymentOptionComparer_NumberOfInstallmentsAscendingThenFriendlyDescriptionDescending_WithBackupListPositionMatchingInitialSort
        Inherits QuickQuotePaymentOptionComparer
        Implements IComparer(Of QuickQuotePaymentOption)

        Public Function Compare1(x As QuickQuotePaymentOption, y As QuickQuotePaymentOption) As Integer Implements System.Collections.Generic.IComparer(Of QuickQuotePaymentOption).Compare
            Return MyBase.Compare(x, y, QuickQuotePaymentOption.SortBy.NumberOfInstallmentsAscendingThenFriendlyDescriptionDescending, backupListPositionSortBy:=QuickQuotePaymentOption.ListPositionSortBy.MatchInitialSort)
        End Function
    End Class
    Public Class QuickQuotePaymentOptionComparer_NumberOfInstallmentsDescendingThenFriendlyDescriptionDescending_WithBackupListPositionMatchingInitialSort
        Inherits QuickQuotePaymentOptionComparer
        Implements IComparer(Of QuickQuotePaymentOption)

        Public Function Compare1(x As QuickQuotePaymentOption, y As QuickQuotePaymentOption) As Integer Implements System.Collections.Generic.IComparer(Of QuickQuotePaymentOption).Compare
            Return MyBase.Compare(x, y, QuickQuotePaymentOption.SortBy.NumberOfInstallmentsDescendingThenFriendlyDescriptionDescending, backupListPositionSortBy:=QuickQuotePaymentOption.ListPositionSortBy.MatchInitialSort)
        End Function
    End Class

    Public Class QuickQuotePaymentOptionComparer_NumberOfInstallmentsAscendingThenDescriptionAscending_WithBackupListPositionMatchingLastSort
        Inherits QuickQuotePaymentOptionComparer
        Implements IComparer(Of QuickQuotePaymentOption)

        Public Function Compare1(x As QuickQuotePaymentOption, y As QuickQuotePaymentOption) As Integer Implements System.Collections.Generic.IComparer(Of QuickQuotePaymentOption).Compare
            Return MyBase.Compare(x, y, QuickQuotePaymentOption.SortBy.NumberOfInstallmentsAscendingThenDescriptionAscending, backupListPositionSortBy:=QuickQuotePaymentOption.ListPositionSortBy.MatchLastSort)
        End Function
    End Class
    Public Class QuickQuotePaymentOptionComparer_NumberOfInstallmentsDescendingThenDescriptionAscending_WithBackupListPositionMatchingLastSort
        Inherits QuickQuotePaymentOptionComparer
        Implements IComparer(Of QuickQuotePaymentOption)

        Public Function Compare1(x As QuickQuotePaymentOption, y As QuickQuotePaymentOption) As Integer Implements System.Collections.Generic.IComparer(Of QuickQuotePaymentOption).Compare
            Return MyBase.Compare(x, y, QuickQuotePaymentOption.SortBy.NumberOfInstallmentsDescendingThenDescriptionAscending, backupListPositionSortBy:=QuickQuotePaymentOption.ListPositionSortBy.MatchLastSort)
        End Function
    End Class
    Public Class QuickQuotePaymentOptionComparer_NumberOfInstallmentsAscendingThenDescriptionDescending_WithBackupListPositionMatchingLastSort
        Inherits QuickQuotePaymentOptionComparer
        Implements IComparer(Of QuickQuotePaymentOption)

        Public Function Compare1(x As QuickQuotePaymentOption, y As QuickQuotePaymentOption) As Integer Implements System.Collections.Generic.IComparer(Of QuickQuotePaymentOption).Compare
            Return MyBase.Compare(x, y, QuickQuotePaymentOption.SortBy.NumberOfInstallmentsAscendingThenDescriptionDescending, backupListPositionSortBy:=QuickQuotePaymentOption.ListPositionSortBy.MatchLastSort)
        End Function
    End Class
    Public Class QuickQuotePaymentOptionComparer_NumberOfInstallmentsDescendingThenDescriptionDescending_WithBackupListPositionMatchingLastSort
        Inherits QuickQuotePaymentOptionComparer
        Implements IComparer(Of QuickQuotePaymentOption)

        Public Function Compare1(x As QuickQuotePaymentOption, y As QuickQuotePaymentOption) As Integer Implements System.Collections.Generic.IComparer(Of QuickQuotePaymentOption).Compare
            Return MyBase.Compare(x, y, QuickQuotePaymentOption.SortBy.NumberOfInstallmentsDescendingThenDescriptionDescending, backupListPositionSortBy:=QuickQuotePaymentOption.ListPositionSortBy.MatchLastSort)
        End Function
    End Class
    Public Class QuickQuotePaymentOptionComparer_NumberOfInstallmentsAscendingThenFriendlyDescriptionAscending_WithBackupListPositionMatchingLastSort
        Inherits QuickQuotePaymentOptionComparer
        Implements IComparer(Of QuickQuotePaymentOption)

        Public Function Compare1(x As QuickQuotePaymentOption, y As QuickQuotePaymentOption) As Integer Implements System.Collections.Generic.IComparer(Of QuickQuotePaymentOption).Compare
            Return MyBase.Compare(x, y, QuickQuotePaymentOption.SortBy.NumberOfInstallmentsAscendingThenFriendlyDescriptionAscending, backupListPositionSortBy:=QuickQuotePaymentOption.ListPositionSortBy.MatchLastSort)
        End Function
    End Class
    Public Class QuickQuotePaymentOptionComparer_NumberOfInstallmentsDescendingThenFriendlyDescriptionAscending_WithBackupListPositionMatchingLastSort
        Inherits QuickQuotePaymentOptionComparer
        Implements IComparer(Of QuickQuotePaymentOption)

        Public Function Compare1(x As QuickQuotePaymentOption, y As QuickQuotePaymentOption) As Integer Implements System.Collections.Generic.IComparer(Of QuickQuotePaymentOption).Compare
            Return MyBase.Compare(x, y, QuickQuotePaymentOption.SortBy.NumberOfInstallmentsDescendingThenFriendlyDescriptionAscending, backupListPositionSortBy:=QuickQuotePaymentOption.ListPositionSortBy.MatchLastSort)
        End Function
    End Class
    Public Class QuickQuotePaymentOptionComparer_NumberOfInstallmentsAscendingThenFriendlyDescriptionDescending_WithBackupListPositionMatchingLastSort
        Inherits QuickQuotePaymentOptionComparer
        Implements IComparer(Of QuickQuotePaymentOption)

        Public Function Compare1(x As QuickQuotePaymentOption, y As QuickQuotePaymentOption) As Integer Implements System.Collections.Generic.IComparer(Of QuickQuotePaymentOption).Compare
            Return MyBase.Compare(x, y, QuickQuotePaymentOption.SortBy.NumberOfInstallmentsAscendingThenFriendlyDescriptionDescending, backupListPositionSortBy:=QuickQuotePaymentOption.ListPositionSortBy.MatchLastSort)
        End Function
    End Class
    Public Class QuickQuotePaymentOptionComparer_NumberOfInstallmentsDescendingThenFriendlyDescriptionDescending_WithBackupListPositionMatchingLastSort
        Inherits QuickQuotePaymentOptionComparer
        Implements IComparer(Of QuickQuotePaymentOption)

        Public Function Compare1(x As QuickQuotePaymentOption, y As QuickQuotePaymentOption) As Integer Implements System.Collections.Generic.IComparer(Of QuickQuotePaymentOption).Compare
            Return MyBase.Compare(x, y, QuickQuotePaymentOption.SortBy.NumberOfInstallmentsDescendingThenFriendlyDescriptionDescending, backupListPositionSortBy:=QuickQuotePaymentOption.ListPositionSortBy.MatchLastSort)
        End Function
    End Class

End Namespace
