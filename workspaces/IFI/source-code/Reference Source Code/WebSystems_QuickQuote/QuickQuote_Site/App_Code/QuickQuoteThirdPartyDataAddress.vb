Imports Microsoft.VisualBasic
'added 12/20/2013
Imports QuickQuote.CommonMethods

Namespace QuickQuote.CommonObjects 'added namespace 12/20/2013
    ''' <summary>
    ''' object used to store current/prior address information
    ''' </summary>
    ''' <remarks>related to 3rd party information</remarks>
    <Serializable()> _
    Public Class QuickQuoteThirdPartyDataAddress 'added 9/18/2013; Diamond.Common.Objects.ThirdParty.AddressLocationInformation
        Inherits QuickQuoteBaseGenericObject(Of Object) 'added 8/4/2014
        Implements IDisposable

        Dim qqHelper As New QuickQuoteHelperClass

        Private _AddressId As String
        Private _AddressNum As String
        Private _ApartmentNum As String
        Private _ApartmentNumberFsi As String
        Private _City As String
        Private _CityFsi As String
        Private _Classification As String
        Private _County As String
        Private _DateFirstAtAddress As String
        Private _DateLastAtAddress As String
        Private _GroupUsageIndicator As String
        Private _HouseNum As String
        Private _HouseNumberFsi As String
        Private _LocationNum As String
        Private _NameAddressSourceId As String
        Private _Other As String
        Private _PersonalLiabilityNum As String
        Private _PoBox As String
        Private _StateAbbreviation As String
        Private _StateCode As String
        Private _StateFsi As String
        Private _StateId As String
        Private _StatusCode As String
        Private _StreetName As String
        Private _StreetNameFsi As String
        Private _TimeAtAddressMonths As String
        Private _TimeAtAddressYears As String
        Private _Verified As Boolean
        Private _Zip As String
        Private _ZipFsi As String
        Private _ZipPlus4 As String
        Private _ZipPlus4Fsi As String

        Public Property AddressId As String
            Get
                Return _AddressId
            End Get
            Set(value As String)
                _AddressId = value
            End Set
        End Property
        Public Property AddressNum As String
            Get
                Return _AddressNum
            End Get
            Set(value As String)
                _AddressNum = value
            End Set
        End Property
        Public Property ApartmentNum As String
            Get
                Return _ApartmentNum
            End Get
            Set(value As String)
                _ApartmentNum = value
            End Set
        End Property
        Public Property ApartmentNumberFsi As String
            Get
                Return _ApartmentNumberFsi
            End Get
            Set(value As String)
                _ApartmentNumberFsi = value
            End Set
        End Property
        Public Property City As String
            Get
                Return _City
            End Get
            Set(value As String)
                _City = value
            End Set
        End Property
        Public Property CityFsi As String
            Get
                Return _CityFsi
            End Get
            Set(value As String)
                _CityFsi = value
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
        Public Property County As String
            Get
                Return _County
            End Get
            Set(value As String)
                _County = value
            End Set
        End Property
        Public Property DateFirstAtAddress As String
            Get
                Return _DateFirstAtAddress
            End Get
            Set(value As String)
                _DateFirstAtAddress = value
                qqHelper.ConvertToShortDate(_DateFirstAtAddress)
            End Set
        End Property
        Public Property DateLastAtAddress As String
            Get
                Return _DateLastAtAddress
            End Get
            Set(value As String)
                _DateLastAtAddress = value
                qqHelper.ConvertToShortDate(_DateLastAtAddress)
            End Set
        End Property
        Public Property GroupUsageIndicator As String
            Get
                Return _GroupUsageIndicator
            End Get
            Set(value As String)
                _GroupUsageIndicator = value
            End Set
        End Property
        Public Property HouseNum As String
            Get
                Return _HouseNum
            End Get
            Set(value As String)
                _HouseNum = value
            End Set
        End Property
        Public Property HouseNumberFsi As String
            Get
                Return _HouseNumberFsi
            End Get
            Set(value As String)
                _HouseNumberFsi = value
            End Set
        End Property
        Public Property LocationNum As String
            Get
                Return _LocationNum
            End Get
            Set(value As String)
                _LocationNum = value
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
        Public Property Other As String
            Get
                Return _Other
            End Get
            Set(value As String)
                _Other = value
            End Set
        End Property
        Public Property PersonalLiabilityNum As String
            Get
                Return _PersonalLiabilityNum
            End Get
            Set(value As String)
                _PersonalLiabilityNum = value
            End Set
        End Property
        Public Property PoBox As String
            Get
                Return _PoBox
            End Get
            Set(value As String)
                _PoBox = value
            End Set
        End Property
        Public Property StateAbbreviation As String
            Get
                Return _StateAbbreviation
            End Get
            Set(value As String)
                _StateAbbreviation = value
            End Set
        End Property
        Public Property StateCode As String
            Get
                Return _StateCode
            End Get
            Set(value As String)
                _StateCode = value
            End Set
        End Property
        Public Property StateFsi As String
            Get
                Return _StateFsi
            End Get
            Set(value As String)
                _StateFsi = value
            End Set
        End Property
        Public Property StateId As String
            Get
                Return _StateId
            End Get
            Set(value As String)
                _StateId = value
            End Set
        End Property
        Public Property StatusCode As String
            Get
                Return _StatusCode
            End Get
            Set(value As String)
                _StatusCode = value
            End Set
        End Property
        Public Property StreetName As String
            Get
                Return _StreetName
            End Get
            Set(value As String)
                _StreetName = value
            End Set
        End Property
        Public Property StreetNameFsi As String
            Get
                Return _StreetNameFsi
            End Get
            Set(value As String)
                _StreetNameFsi = value
            End Set
        End Property
        Public Property TimeAtAddressMonths As String
            Get
                Return _TimeAtAddressMonths
            End Get
            Set(value As String)
                _TimeAtAddressMonths = value
            End Set
        End Property
        Public Property TimeAtAddressYears As String
            Get
                Return _TimeAtAddressYears
            End Get
            Set(value As String)
                _TimeAtAddressYears = value
            End Set
        End Property
        Public Property Verified As Boolean
            Get
                Return _Verified
            End Get
            Set(value As Boolean)
                _Verified = value
            End Set
        End Property
        Public Property Zip As String
            Get
                Return _Zip
            End Get
            Set(value As String)
                _Zip = value
            End Set
        End Property
        Public Property ZipFsi As String
            Get
                Return _ZipFsi
            End Get
            Set(value As String)
                _ZipFsi = value
            End Set
        End Property
        Public Property ZipPlus4 As String
            Get
                Return _ZipPlus4
            End Get
            Set(value As String)
                _ZipPlus4 = value
            End Set
        End Property
        Public Property ZipPlus4Fsi As String
            Get
                Return _ZipPlus4Fsi
            End Get
            Set(value As String)
                _ZipPlus4Fsi = value
            End Set
        End Property

        Public Sub New()
            MyBase.New() 'added 8/4/2014
            SetDefaults()
        End Sub
        Private Sub SetDefaults()
            _AddressId = ""
            _AddressNum = ""
            _ApartmentNum = ""
            _ApartmentNumberFsi = ""
            _City = ""
            _CityFsi = ""
            _Classification = ""
            _County = ""
            _DateFirstAtAddress = ""
            _DateLastAtAddress = ""
            _GroupUsageIndicator = ""
            _HouseNum = ""
            _HouseNumberFsi = ""
            _LocationNum = ""
            _NameAddressSourceId = ""
            _Other = ""
            _PersonalLiabilityNum = ""
            _PoBox = ""
            _StateAbbreviation = ""
            _StateCode = ""
            _StateFsi = ""
            _StateId = ""
            _StatusCode = ""
            _StreetName = ""
            _StreetNameFsi = ""
            _TimeAtAddressMonths = ""
            _TimeAtAddressYears = ""
            _Verified = False
            _Zip = ""
            _ZipFsi = ""
            _ZipPlus4 = ""
            _ZipPlus4Fsi = ""
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
                    If _AddressId IsNot Nothing Then
                        _AddressId = Nothing
                    End If
                    If _AddressNum IsNot Nothing Then
                        _AddressNum = Nothing
                    End If
                    If _ApartmentNum IsNot Nothing Then
                        _ApartmentNum = Nothing
                    End If
                    If _ApartmentNumberFsi IsNot Nothing Then
                        _ApartmentNumberFsi = Nothing
                    End If
                    If _City IsNot Nothing Then
                        _City = Nothing
                    End If
                    If _CityFsi IsNot Nothing Then
                        _CityFsi = Nothing
                    End If
                    If _Classification IsNot Nothing Then
                        _Classification = Nothing
                    End If
                    If _County IsNot Nothing Then
                        _County = Nothing
                    End If
                    If _DateFirstAtAddress IsNot Nothing Then
                        _DateFirstAtAddress = Nothing
                    End If
                    If _DateLastAtAddress IsNot Nothing Then
                        _DateLastAtAddress = Nothing
                    End If
                    If _GroupUsageIndicator IsNot Nothing Then
                        _GroupUsageIndicator = Nothing
                    End If
                    If _HouseNum IsNot Nothing Then
                        _HouseNum = Nothing
                    End If
                    If _HouseNumberFsi IsNot Nothing Then
                        _HouseNumberFsi = Nothing
                    End If
                    If _LocationNum IsNot Nothing Then
                        _LocationNum = Nothing
                    End If
                    If _NameAddressSourceId IsNot Nothing Then
                        _NameAddressSourceId = Nothing
                    End If
                    If _Other IsNot Nothing Then
                        _Other = Nothing
                    End If
                    If _PersonalLiabilityNum IsNot Nothing Then
                        _PersonalLiabilityNum = Nothing
                    End If
                    If _PoBox IsNot Nothing Then
                        _PoBox = Nothing
                    End If
                    If _StateAbbreviation IsNot Nothing Then
                        _StateAbbreviation = Nothing
                    End If
                    If _StateCode IsNot Nothing Then
                        _StateCode = Nothing
                    End If
                    If _StateFsi IsNot Nothing Then
                        _StateFsi = Nothing
                    End If
                    If _StateId IsNot Nothing Then
                        _StateId = Nothing
                    End If
                    If _StatusCode IsNot Nothing Then
                        _StatusCode = Nothing
                    End If
                    If _StreetName IsNot Nothing Then
                        _StreetName = Nothing
                    End If
                    If _StreetNameFsi IsNot Nothing Then
                        _StreetNameFsi = Nothing
                    End If
                    If _TimeAtAddressMonths IsNot Nothing Then
                        _TimeAtAddressMonths = Nothing
                    End If
                    If _TimeAtAddressYears IsNot Nothing Then
                        _TimeAtAddressYears = Nothing
                    End If
                    If _Verified <> Nothing Then
                        _Verified = Nothing
                    End If
                    If _Zip IsNot Nothing Then
                        _Zip = Nothing
                    End If
                    If _ZipFsi IsNot Nothing Then
                        _ZipFsi = Nothing
                    End If
                    If _ZipPlus4 IsNot Nothing Then
                        _ZipPlus4 = Nothing
                    End If
                    If _ZipPlus4Fsi IsNot Nothing Then
                        _ZipPlus4Fsi = Nothing
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
