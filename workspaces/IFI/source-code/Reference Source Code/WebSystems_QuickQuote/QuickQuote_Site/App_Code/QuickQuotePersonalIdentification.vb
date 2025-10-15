Imports Microsoft.VisualBasic
'added 12/20/2013
Imports QuickQuote.CommonMethods

Namespace QuickQuote.CommonObjects 'added namespace 12/20/2013
    ''' <summary>
    ''' object used to store personal identification information
    ''' </summary>
    ''' <remarks>related to 3rd party information</remarks>
    <Serializable()> _
    Public Class QuickQuotePersonalIdentification 'added 9/18/2013
        Inherits QuickQuoteBaseGenericObject(Of Object) 'added 8/4/2014
        Implements IDisposable

        Dim qqHelper As New QuickQuoteHelperClass

        Private _AddressAssociationIndicator As String
        Private _BirthDate As String
        Private _BirthDateFsi As String
        Private _Classification As String
        Private _DisplayName As String
        Private _DisplayNum As String
        Private _DriverNum As String
        Private _FirstName As String
        Private _FirstNameFsi As String
        Private _LastName As String
        Private _LastNameFsi As String
        Private _MaritalStatusId As String
        Private _MiddleName As String
        Private _MiddleNameFsi As String
        Private _NameAddressSourceId As String
        Private _NameId As String
        Private _NameNum As String
        Private _PhoneAreaCodeFsi As String
        Private _PhoneExchangeFsi As String
        Private _PhoneNumberFsi As String
        Private _PrefixName As String
        Private _PrefixNameFsi As String
        Private _Relationship As String
        Private _RelationshipDescription As String
        Private _RelationshipTypeId As String
        Private _SSN As String
        Private _SexId As String
        Private _SexIdFsi As String
        Private _SuffixName As String
        Private _SuffixNameFsi As String
        Private _TaxNumberFsi As String

        Public Property AddressAssociationIndicator As String
            Get
                Return _AddressAssociationIndicator
            End Get
            Set(value As String)
                _AddressAssociationIndicator = value
            End Set
        End Property
        Public Property BirthDate As String
            Get
                Return _BirthDate
            End Get
            Set(value As String)
                _BirthDate = value
                qqHelper.ConvertToShortDate(_BirthDate)
            End Set
        End Property
        Public Property BirthDateFsi As String
            Get
                Return _BirthDateFsi
            End Get
            Set(value As String)
                _BirthDateFsi = value
                qqHelper.ConvertToShortDate(_BirthDateFsi)
            End Set
        End Property
        Public Property Classification As String
            Get
                Return _Classification
            End Get
            Set(value As String)
                _Classification = value
            End Set
        End Property
        Public Property DisplayName As String
            Get
                Return _DisplayName
            End Get
            Set(value As String)
                _DisplayName = value
            End Set
        End Property
        Public Property DisplayNum As String
            Get
                Return _DisplayNum
            End Get
            Set(value As String)
                _DisplayNum = value
            End Set
        End Property
        Public Property DriverNum As String
            Get
                Return _DriverNum
            End Get
            Set(value As String)
                _DriverNum = value
            End Set
        End Property
        Public Property FirstName As String
            Get
                Return _FirstName
            End Get
            Set(value As String)
                _FirstName = value
            End Set
        End Property
        Public Property FirstNameFsi As String
            Get
                Return _FirstNameFsi
            End Get
            Set(value As String)
                _FirstNameFsi = value
            End Set
        End Property
        Public Property LastName As String
            Get
                Return _LastName
            End Get
            Set(value As String)
                _LastName = value
            End Set
        End Property
        Public Property LastNameFsi As String
            Get
                Return _LastNameFsi
            End Get
            Set(value As String)
                _LastNameFsi = value
            End Set
        End Property
        Public Property MaritalStatusId As String
            Get
                Return _MaritalStatusId
            End Get
            Set(value As String)
                _MaritalStatusId = value
            End Set
        End Property
        Public Property MiddleName As String
            Get
                Return _MiddleName
            End Get
            Set(value As String)
                _MiddleName = value
            End Set
        End Property
        Public Property MiddleNameFsi As String
            Get
                Return _MiddleNameFsi
            End Get
            Set(value As String)
                _MiddleNameFsi = value
            End Set
        End Property
        Public Property NameAddressSourceId As String
            Get
                Return _NameAddressSourceId
            End Get
            Set(value As String)
                _NameAddressSourceId = value
            End Set
        End Property
        Public Property NameId As String
            Get
                Return _NameId
            End Get
            Set(value As String)
                _NameId = value
            End Set
        End Property
        Public Property NameNum As String
            Get
                Return _NameNum
            End Get
            Set(value As String)
                _NameNum = value
            End Set
        End Property
        Public Property PhoneAreaCodeFsi As String
            Get
                Return _PhoneAreaCodeFsi
            End Get
            Set(value As String)
                _PhoneAreaCodeFsi = value
            End Set
        End Property
        Public Property PhoneExchangeFsi As String
            Get
                Return _PhoneExchangeFsi
            End Get
            Set(value As String)
                _PhoneExchangeFsi = value
            End Set
        End Property
        Public Property PhoneNumberFsi As String
            Get
                Return _PhoneNumberFsi
            End Get
            Set(value As String)
                _PhoneNumberFsi = value
            End Set
        End Property
        Public Property PrefixName As String
            Get
                Return _PrefixName
            End Get
            Set(value As String)
                _PrefixName = value
            End Set
        End Property
        Public Property PrefixNameFsi As String
            Get
                Return _PrefixNameFsi
            End Get
            Set(value As String)
                _PrefixNameFsi = value
            End Set
        End Property
        Public Property Relationship As String
            Get
                Return _Relationship
            End Get
            Set(value As String)
                _Relationship = value
            End Set
        End Property
        Public Property RelationshipDescription As String
            Get
                Return _RelationshipDescription
            End Get
            Set(value As String)
                _RelationshipDescription = value
            End Set
        End Property
        Public Property RelationshipTypeId As String
            Get
                Return _RelationshipTypeId
            End Get
            Set(value As String)
                _RelationshipTypeId = value
            End Set
        End Property
        Public Property SSN As String
            Get
                Return _SSN
            End Get
            Set(value As String)
                _SSN = value
            End Set
        End Property
        Public Property SexId As String
            Get
                Return _SexId
            End Get
            Set(value As String)
                _SexId = value
            End Set
        End Property
        Public Property SexIdFsi As String
            Get
                Return _SexIdFsi
            End Get
            Set(value As String)
                _SexIdFsi = value
            End Set
        End Property
        Public Property SuffixName As String
            Get
                Return _SuffixName
            End Get
            Set(value As String)
                _SuffixName = value
            End Set
        End Property
        Public Property SuffixNameFsi As String
            Get
                Return _SuffixNameFsi
            End Get
            Set(value As String)
                _SuffixNameFsi = value
            End Set
        End Property
        Public Property TaxNumberFsi As String
            Get
                Return _TaxNumberFsi
            End Get
            Set(value As String)
                _TaxNumberFsi = value
            End Set
        End Property

        Public Sub New()
            MyBase.New() 'added 8/4/2014
            SetDefaults()
        End Sub
        Private Sub SetDefaults()
            _AddressAssociationIndicator = ""
            _BirthDate = ""
            _BirthDateFsi = ""
            _Classification = ""
            _DisplayName = ""
            _DisplayNum = ""
            _DriverNum = ""
            _FirstName = ""
            _FirstNameFsi = ""
            _LastName = ""
            _LastNameFsi = ""
            _MaritalStatusId = ""
            _MiddleName = ""
            _MiddleNameFsi = ""
            _NameAddressSourceId = ""
            _NameId = ""
            _NameNum = ""
            _PhoneAreaCodeFsi = ""
            _PhoneExchangeFsi = ""
            _PhoneNumberFsi = ""
            _PrefixName = ""
            _PrefixNameFsi = ""
            _Relationship = ""
            _RelationshipDescription = ""
            _RelationshipTypeId = ""
            _SSN = ""
            _SexId = ""
            _SexIdFsi = ""
            _SuffixName = ""
            _SuffixNameFsi = ""
            _TaxNumberFsi = ""
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
                    If _AddressAssociationIndicator IsNot Nothing Then
                        _AddressAssociationIndicator = Nothing
                    End If
                    If _BirthDate IsNot Nothing Then
                        _BirthDate = Nothing
                    End If
                    If _BirthDateFsi IsNot Nothing Then
                        _BirthDateFsi = Nothing
                    End If
                    If _Classification IsNot Nothing Then
                        _Classification = Nothing
                    End If
                    If _DisplayName IsNot Nothing Then
                        _DisplayName = Nothing
                    End If
                    If _DisplayNum IsNot Nothing Then
                        _DisplayNum = Nothing
                    End If
                    If _DriverNum IsNot Nothing Then
                        _DriverNum = Nothing
                    End If
                    If _FirstName IsNot Nothing Then
                        _FirstName = Nothing
                    End If
                    If _FirstNameFsi IsNot Nothing Then
                        _FirstNameFsi = Nothing
                    End If
                    If _LastName IsNot Nothing Then
                        _LastName = Nothing
                    End If
                    If _LastNameFsi IsNot Nothing Then
                        _LastNameFsi = Nothing
                    End If
                    If _MaritalStatusId IsNot Nothing Then
                        _MaritalStatusId = Nothing
                    End If
                    If _MiddleName IsNot Nothing Then
                        _MiddleName = Nothing
                    End If
                    If _MiddleNameFsi IsNot Nothing Then
                        _MiddleNameFsi = Nothing
                    End If
                    If _NameAddressSourceId IsNot Nothing Then
                        _NameAddressSourceId = Nothing
                    End If
                    If _NameId IsNot Nothing Then
                        _NameId = Nothing
                    End If
                    If _NameNum IsNot Nothing Then
                        _NameNum = Nothing
                    End If
                    If _PhoneAreaCodeFsi IsNot Nothing Then
                        _PhoneAreaCodeFsi = Nothing
                    End If
                    If _PhoneExchangeFsi IsNot Nothing Then
                        _PhoneExchangeFsi = Nothing
                    End If
                    If _PhoneNumberFsi IsNot Nothing Then
                        _PhoneNumberFsi = Nothing
                    End If
                    If _PrefixName IsNot Nothing Then
                        _PrefixName = Nothing
                    End If
                    If _PrefixNameFsi IsNot Nothing Then
                        _PrefixNameFsi = Nothing
                    End If
                    If _Relationship IsNot Nothing Then
                        _Relationship = Nothing
                    End If
                    If _RelationshipDescription IsNot Nothing Then
                        _RelationshipDescription = Nothing
                    End If
                    If _RelationshipTypeId IsNot Nothing Then
                        _RelationshipTypeId = Nothing
                    End If
                    If _SSN IsNot Nothing Then
                        _SSN = Nothing
                    End If
                    If _SexId IsNot Nothing Then
                        _SexId = Nothing
                    End If
                    If _SexIdFsi IsNot Nothing Then
                        _SexIdFsi = Nothing
                    End If
                    If _SuffixName IsNot Nothing Then
                        _SuffixName = Nothing
                    End If
                    If _SuffixNameFsi IsNot Nothing Then
                        _SuffixNameFsi = Nothing
                    End If
                    If _TaxNumberFsi IsNot Nothing Then
                        _TaxNumberFsi = Nothing
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
