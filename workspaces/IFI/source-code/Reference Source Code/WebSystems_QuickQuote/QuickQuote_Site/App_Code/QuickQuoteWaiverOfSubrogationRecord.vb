Imports Microsoft.VisualBasic
Imports QuickQuote.CommonMethods 'added 6/30/2015

Namespace QuickQuote.CommonObjects 'added namespace 12/20/2013
    ''' <summary>
    ''' object used to store waiver of subrogation
    ''' </summary>
    ''' <remarks>used with QuickQuoteInclusionExclusion (<see cref="QuickQuoteInclusionExclusion"/>) and QuickQuoteInclusionExclusionScheduledItem (<see cref="QuickQuoteInclusionExclusionScheduledItem"/>) objects</remarks>
    <Serializable()> _
    Public Class QuickQuoteWaiverOfSubrogationRecord
        Inherits QuickQuoteBaseGenericObject(Of Object) 'added 8/4/2014
        Implements IDisposable

        Dim qqHelper As New QuickQuoteHelperClass 'added 6/30/2015

        Private _Name As QuickQuoteName
        Private _Address As QuickQuoteAddress
        Private _Phones As Generic.List(Of QuickQuotePhone)
        Private _Emails As Generic.List(Of QuickQuoteEmail)
        Private _Premium As String
        Private _PremiumId As String

        Public Property Name As QuickQuoteName
            Get
                SetObjectsParent(_Name)
                Return _Name
            End Get
            Set(value As QuickQuoteName)
                _Name = value
                SetObjectsParent(_Name)
            End Set
        End Property
        Public Property Address As QuickQuoteAddress
            Get
                SetObjectsParent(_Address)
                Return _Address
            End Get
            Set(value As QuickQuoteAddress)
                _Address = value
                SetObjectsParent(_Address)
            End Set
        End Property
        Public Property Phones As Generic.List(Of QuickQuotePhone)
            Get
                SetParentOfListItems(_Phones, "{663B7C7B-F2AC-4BF6-965A-D30F41A05671}")
                Return _Phones
            End Get
            Set(value As Generic.List(Of QuickQuotePhone))
                _Phones = value
                SetParentOfListItems(_Phones, "{663B7C7B-F2AC-4BF6-965A-D30F41A05671}")
            End Set
        End Property
        Public Property Emails As Generic.List(Of QuickQuoteEmail)
            Get
                SetParentOfListItems(_Emails, "{663B7C7B-F2AC-4BF6-965A-D30F41A05672}")
                Return _Emails
            End Get
            Set(value As Generic.List(Of QuickQuoteEmail))
                _Emails = value
                SetParentOfListItems(_Emails, "{663B7C7B-F2AC-4BF6-965A-D30F41A05672}")
            End Set
        End Property
        Public Property Premium As String
            Get
                Return _Premium
            End Get
            Set(value As String)
                _Premium = value
                Select Case _Premium
                    Case "Not Assigned"
                        _PremiumId = "0"
                    Case "0"
                        _PremiumId = "1"
                    Case "25"
                        _PremiumId = "2"
                    Case "50"
                        _PremiumId = "3"
                    Case "75"
                        _PremiumId = "4"
                    Case "100"
                        _PremiumId = "5"
                    Case "150"
                        _PremiumId = "6"
                    Case "200"
                        _PremiumId = "7"
                    Case "250"
                        _PremiumId = "8"
                    Case "300"
                        _PremiumId = "9"
                    Case "400"
                        _PremiumId = "10"
                    Case "500"
                        _PremiumId = "11"
                    Case Else
                        _PremiumId = ""
                End Select
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>uses Diamond's InclusionExclusionScheduledItemWaiverOfSubrogationAmountType table</remarks>
        Public Property PremiumId As String
            Get
                Return _PremiumId
            End Get
            Set(value As String)
                _PremiumId = value
                _Premium = ""
                If IsNumeric(_PremiumId) = True Then
                    Select Case _PremiumId
                        Case "0"
                            _Premium = "Not Assigned"
                        Case "1"
                            _Premium = "0"
                        Case "2"
                            _Premium = "25"
                        Case "3"
                            _Premium = "50"
                        Case "4"
                            _Premium = "75"
                        Case "5"
                            _Premium = "100"
                        Case "6"
                            _Premium = "150"
                        Case "7"
                            _Premium = "200"
                        Case "8"
                            _Premium = "250"
                        Case "9"
                            _Premium = "300"
                        Case "10"
                            _Premium = "400"
                        Case "11"
                            _Premium = "500"
                    End Select
                End If
            End Set
        End Property

        Public Sub New()
            MyBase.New() 'added 8/4/2014
            SetDefaults()
        End Sub
        Private Sub SetDefaults()
            _Name = New QuickQuoteName
            _Address = New QuickQuoteAddress
            '_Phones = New Generic.List(Of QuickQuotePhone)
            _Phones = Nothing 'added 8/4/2014
            '_Emails = New Generic.List(Of QuickQuoteEmail)
            _Emails = Nothing 'added 8/4/2014
            _Premium = ""
            _PremiumId = ""
        End Sub
        Public Overrides Function ToString() As String 'added 6/29/2015
            Dim str As String = ""
            If Me IsNot Nothing Then
                If Me.Name IsNot Nothing Then
                    str = qqHelper.appendText(str, "DisplayName: " & Me.Name.DisplayName, vbCrLf)
                End If
                If Me.Address IsNot Nothing Then
                    str = qqHelper.appendText(str, "DisplayAddress: " & Me.Address.DisplayAddress, vbCrLf)
                End If
            Else
                str = "Nothing"
            End If
            Return str
        End Function

        'added 10/10/2017
        Public Function HasData() As Boolean
            If (_Name IsNot Nothing AndAlso _Name.HasData = True) OrElse (_Address IsNot Nothing AndAlso _Address.HasData = True) OrElse (_Phones IsNot Nothing AndAlso _Phones.Count > 0) OrElse (_Emails IsNot Nothing AndAlso _Emails.Count > 0) OrElse qqHelper.IsPositiveIntegerString(_PremiumId) = True Then
                Return True
            Else
                Return False
            End If
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
                    If _Name IsNot Nothing Then
                        _Name.Dispose()
                        _Name = Nothing
                    End If
                    If _Address IsNot Nothing Then
                        _Address.Dispose()
                        _Address = Nothing
                    End If
                    If _Phones IsNot Nothing Then
                        If _Phones.Count > 0 Then
                            For Each ph As QuickQuotePhone In _Phones
                                ph.Dispose()
                                ph = Nothing
                            Next
                            _Phones.Clear()
                        End If
                        _Phones = Nothing
                    End If
                    If _Emails IsNot Nothing Then
                        If _Emails.Count > 0 Then
                            For Each em As QuickQuoteEmail In _Emails
                                em.Dispose()
                                em = Nothing
                            Next
                            _Emails.Clear()
                        End If
                        _Emails = Nothing
                    End If
                    If _Premium IsNot Nothing Then
                        _Premium = Nothing
                    End If
                    If _PremiumId IsNot Nothing Then
                        _PremiumId = Nothing
                    End If

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
End Namespace
