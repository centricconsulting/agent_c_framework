Public Class DiamondSecurityProducer
#Region "Var"
    Private _AgencyID As Integer
    Private _IsPrimaryContact As Boolean
    Private _Prefix As String
    Private _FirstName As String
    Private _MiddleName As String
    Private _LastName As String
    Private _Suffix As String
    Private _EmailAddress As String
    Private _PhoneNumber As String
    Private _ProducerCode As String
    Private _Status As Boolean
#End Region

    Public Sub New()

    End Sub

    Public Sub New(ByVal prodAgencyID As Integer, ByVal prodIsPrimContact As Boolean, ByVal prodPrefix As String, ByVal prodFirstName As String, ByVal prodMiddleName As String, _
                   ByVal prodLastName As String, ByVal prodSuffix As String, ByVal prodEmail As String, ByVal prodPhone As String, ByVal prodCode As String, ByVal prodStatus As Boolean)

        If IsNumeric(prodAgencyID) Then AgencyID = prodAgencyID
        IsPrimaryContact = prodIsPrimContact
        Prefix = prodPrefix
        FirstName = prodFirstName
        MiddleName = prodMiddleName
        LastName = prodLastName
        Suffix = prodSuffix
        EmailAddress = prodEmail
        PhoneNumber = prodPhone
        ProducerCode = prodCode
        Status = prodStatus
    End Sub

#Region "Props"

    Public Property AgencyID() As Integer
        Get
            Return _AgencyID
        End Get
        Set(ByVal value As Integer)
            _AgencyID = value
        End Set
    End Property

    Public Property IsPrimaryContact() As Boolean
        Get
            Return _IsPrimaryContact
        End Get
        Set(ByVal value As Boolean)
            _IsPrimaryContact = value
        End Set
    End Property

    Public Property Prefix() As String
        Get
            Return _Prefix
        End Get
        Set(ByVal value As String)
            _Prefix = value
        End Set
    End Property

    Public Property FirstName() As String
        Get
            Return _FirstName
        End Get
        Set(ByVal value As String)
            _FirstName = value
        End Set
    End Property

    Public Property MiddleName() As String
        Get
            Return _MiddleName
        End Get
        Set(ByVal value As String)
            _MiddleName = value
        End Set
    End Property

    Public Property LastName() As String
        Get
            Return _LastName
        End Get
        Set(ByVal value As String)
            _LastName = value
        End Set
    End Property

    Public Property Suffix() As String
        Get
            Return _Suffix
        End Get
        Set(ByVal value As String)
            _Suffix = value
        End Set
    End Property

    Public Property EmailAddress() As String
        Get
            Return _EmailAddress
        End Get
        Set(ByVal value As String)
            _EmailAddress = value
        End Set
    End Property

    Public Property PhoneNumber() As String
        Get
            Return _PhoneNumber
        End Get
        Set(ByVal value As String)
            _PhoneNumber = value
        End Set
    End Property

    Public Property ProducerCode() As String
        Get
            Return _ProducerCode
        End Get
        Set(ByVal value As String)
            _ProducerCode = value
        End Set
    End Property

    Public Property Status() As Boolean
        Get
            Return _Status
        End Get
        Set(ByVal value As Boolean)
            _Status = value
        End Set
    End Property

#End Region
End Class
