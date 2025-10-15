Imports Microsoft.VisualBasic
Imports QuickQuote.CommonMethods

Namespace QuickQuote.CommonObjects
    <Serializable()> _
    Public Class QuickQuoteBillingAddressee 'added 5/19/2014
        Inherits QuickQuoteBaseGenericObject(Of Object) 'added 8/4/2014
        Implements IDisposable

        Dim qqHelper As New QuickQuoteHelperClass

        Private _Name As QuickQuoteName
        Private _Address As QuickQuoteAddress
        Private _Phones As List(Of QuickQuotePhone) 'not used in Diamond
        Private _Emails As List(Of QuickQuoteEmail) 'not used in Diamond

        Private _DetailStatusCode As String 'added 5/15/2019

        Public Property Name As QuickQuoteName
            Get
                SetObjectsParent(_Name)
                Return _Name
            End Get
            Set(value As QuickQuoteName)
                _Name = value
                If _Name IsNot Nothing AndAlso _Name.NameAddressSourceId = "" Then
                    _Name.NameAddressSourceId = "5" 'Billing Addressee
                    SetObjectsParent(_Name)
                End If
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
        Public Property Phones As List(Of QuickQuotePhone)
            Get
                SetParentOfListItems(_Phones, "{32BB5FB9-093F-42E5-8373-3CB836D45A50}")
                Return _Phones
            End Get
            Set(value As List(Of QuickQuotePhone))
                _Phones = value
                SetParentOfListItems(_Phones, "{32BB5FB9-093F-42E5-8373-3CB836D45A50}")
            End Set
        End Property
        Public Property Emails As List(Of QuickQuoteEmail)
            Get
                SetParentOfListItems(_Emails, "{32BB5FB9-093F-42E5-8373-3CB836D45A51}")
                Return _Emails
            End Get
            Set(value As List(Of QuickQuoteEmail))
                _Emails = value
                SetParentOfListItems(_Emails, "{32BB5FB9-093F-42E5-8373-3CB836D45A51}")
            End Set
        End Property
        'added 5/21/2014
        Public ReadOnly Property HasData As Boolean
            Get
                If (_Name IsNot Nothing AndAlso _Name.HasData = True) OrElse (_Address IsNot Nothing AndAlso _Address.HasData = True) OrElse (_Phones IsNot Nothing AndAlso _Phones.Count > 0) OrElse (_Emails IsNot Nothing AndAlso _Emails.Count > 0) Then
                    Return True
                Else
                    Return False
                End If
            End Get
        End Property

        Public Property DetailStatusCode As String 'added 5/15/2019
            Get
                Return _DetailStatusCode
            End Get
            Set(value As String)
                _DetailStatusCode = value
            End Set
        End Property

        Public Sub New()
            MyBase.New() 'added 8/4/2014
            SetDefaults()
        End Sub
        Private Sub SetDefaults()
            _Name = New QuickQuoteName
            '_Name.NameAddressSourceId = "5" 'Billing Addressee
            _Name.NameAddressSourceId = qqHelper.GetStaticDataValueForText(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteName, QuickQuoteHelperClass.QuickQuotePropertyName.NameAddressSourceId, "Billing Addressee")
            _Address = New QuickQuoteAddress
            '_Phones = New List(Of QuickQuotePhone)
            _Phones = Nothing 'added 8/4/2014
            '_Emails = New List(Of QuickQuoteEmail)
            _Emails = Nothing 'added 8/4/2014

            _DetailStatusCode = "" 'added 5/15/2019
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

                    qqHelper.DisposeString(_DetailStatusCode) 'added 5/15/2019

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
            ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
            Dispose(True)
            GC.SuppressFinalize(Me)
        End Sub
#End Region

    End Class
End Namespace
