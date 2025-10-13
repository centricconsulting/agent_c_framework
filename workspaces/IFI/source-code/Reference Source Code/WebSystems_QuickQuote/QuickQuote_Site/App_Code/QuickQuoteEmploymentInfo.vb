Imports Microsoft.VisualBasic
Imports QuickQuote.CommonMethods

Namespace QuickQuote.CommonObjects
    <Serializable()> _
    Public Class QuickQuoteEmploymentInfo 'added 5/12/2014
        Inherits QuickQuoteBaseGenericObject(Of Object) 'added 8/4/2014
        Implements IDisposable

        Dim qqHelper As New QuickQuoteHelperClass

        Private _Name As QuickQuoteName
        Private _Address As QuickQuoteAddress
        Private _Phones As Generic.List(Of QuickQuotePhone)
        Private _Emails As Generic.List(Of QuickQuoteEmail)
        'AnnualIncomeTypeId
        Private _Duration As String
        Private _EmploymentDurationTypeId As String
        'EmploymentNum
        'Occupation
        Private _OccupationTypeId As String
        Private _PreviousDuration As String
        Private _PreviousEmploymentDurationTypeId As String
        'Remarks
        'YearsEmployed

        Private _DetailStatusCode As String 'added 5/15/2019

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
                SetParentOfListItems(_Phones, "{663B7C7B-F2AC-4BF6-965A-D30F41A05207}")
                Return _Phones
            End Get
            Set(value As Generic.List(Of QuickQuotePhone))
                _Phones = value
                SetParentOfListItems(_Phones, "{663B7C7B-F2AC-4BF6-965A-D30F41A05207}")
            End Set
        End Property
        Public Property Emails As Generic.List(Of QuickQuoteEmail)
            Get
                SetParentOfListItems(_Emails, "{663B7C7B-F2AC-4BF6-965A-D30F41A05208}")
                Return _Emails
            End Get
            Set(value As Generic.List(Of QuickQuoteEmail))
                _Emails = value
                SetParentOfListItems(_Emails, "{663B7C7B-F2AC-4BF6-965A-D30F41A05208}")
            End Set
        End Property
        Public Property Duration As String
            Get
                Return _Duration
            End Get
            Set(value As String)
                _Duration = value
            End Set
        End Property
        Public Property EmploymentDurationTypeId As String
            Get
                Return _EmploymentDurationTypeId
            End Get
            Set(value As String)
                _EmploymentDurationTypeId = value
            End Set
        End Property
        Public Property OccupationTypeId As String
            Get
                Return _OccupationTypeId
            End Get
            Set(value As String)
                _OccupationTypeId = value
            End Set
        End Property
        Public Property PreviousDuration As String
            Get
                Return _PreviousDuration
            End Get
            Set(value As String)
                _PreviousDuration = value
            End Set
        End Property
        Public Property PreviousEmploymentDurationTypeId As String
            Get
                Return _PreviousEmploymentDurationTypeId
            End Get
            Set(value As String)
                _PreviousEmploymentDurationTypeId = value
            End Set
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
            '_Name.NameAddressSourceId = "16" 'Driver Employment
            _Name.NameAddressSourceId = qqHelper.GetStaticDataValueForText(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteName, QuickQuoteHelperClass.QuickQuotePropertyName.NameAddressSourceId, "Driver Employment")
            _Address = New QuickQuoteAddress
            '_Phones = New Generic.List(Of QuickQuotePhone)
            _Phones = Nothing 'added 8/4/2014
            '_Emails = New Generic.List(Of QuickQuoteEmail)
            _Emails = Nothing 'added 8/4/2014
            _Duration = ""
            _EmploymentDurationTypeId = ""
            _OccupationTypeId = ""
            _PreviousDuration = ""
            _PreviousEmploymentDurationTypeId = ""

            _DetailStatusCode = "" 'added 5/15/2019
        End Sub

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
                    If _Duration IsNot Nothing Then
                        _Duration = Nothing
                    End If
                    If _EmploymentDurationTypeId IsNot Nothing Then
                        _EmploymentDurationTypeId = Nothing
                    End If
                    If _OccupationTypeId IsNot Nothing Then
                        _OccupationTypeId = Nothing
                    End If
                    If _PreviousDuration IsNot Nothing Then
                        _PreviousDuration = Nothing
                    End If
                    If _PreviousEmploymentDurationTypeId IsNot Nothing Then
                        _PreviousEmploymentDurationTypeId = Nothing
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
