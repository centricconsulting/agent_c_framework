Imports PublicQuotingLib.Models
Imports QuickQuote.CommonObjects

Public Class AddressOtherField


    Private _input As String
    Public Property Input() As String
        Get
            Return _input
        End Get
        Set(ByVal value As String)
            _input = value
        End Set
    End Property

    Private _PrefixType As Helpers.AddressOtherFieldPrefixHelper.OtherFieldPrefix
    Public ReadOnly Property PrefixType() As Helpers.AddressOtherFieldPrefixHelper.OtherFieldPrefix
        Get
            Return _PrefixType
        End Get
    End Property

    Private _prefixText As String
    Public ReadOnly Property PrefixText() As String
        Get
            Return _prefixText
        End Get
    End Property

    Private _NameWithoutPrefix As String
    Public ReadOnly Property NameWithoutPrefix() As String
        Get
            Return _NameWithoutPrefix
        End Get
    End Property

    Private _NameWithPrefix As String
    Public ReadOnly Property NameWithPrefix() As String
        Get
            Return _NameWithPrefix
        End Get
    End Property


    Public Sub New()
        _input = String.Empty
        _prefixText = String.Empty
        _NameWithoutPrefix = String.Empty
        _NameWithPrefix = String.Empty

    End Sub

    Public Sub New(input As String)
        _input = input
        _prefixText = String.Empty
        _NameWithoutPrefix = String.Empty
        _NameWithPrefix = String.Empty
        UpdateData()
    End Sub

    Public Sub UpdateData()
        If String.IsNullOrWhiteSpace(Input) = False Then
            _PrefixType = Helpers.AddressOtherFieldPrefixHelper.GetPrefix(Input)
            _prefixText = Helpers.AddressOtherFieldPrefixHelper.GetPrefixStringFromInput(Input)
            _NameWithoutPrefix = Helpers.AddressOtherFieldPrefixHelper.GetNameWithoutPrefix(Input)
            _NameWithPrefix = Helpers.AddressOtherFieldPrefixHelper.GetNameWithPrefix(Input)
        End If
    End Sub

    Public Sub UpdateType(type As Helpers.AddressOtherFieldPrefixHelper.OtherFieldPrefix)
        If String.IsNullOrWhiteSpace(Input) = False Then
            _input = Helpers.AddressOtherFieldPrefixHelper.ChangeType(_input, type)
            UpdateData()
        End If
    End Sub

    Public Sub UpdateNameAndType(name As String, type As Helpers.AddressOtherFieldPrefixHelper.OtherFieldPrefix)
        _input = Helpers.AddressOtherFieldPrefixHelper.ChangeType(name, type)
        UpdateData()
    End Sub

    'Dim OtherFieldItem As AddressOtherField = New AddressOtherField(" C/O Name")

    'Dim OtherFieldItem2 As AddressOtherField = New AddressOtherField(" C/O Name")
    'OtherFieldItem2.UpdateData()
    'OtherFieldItem2.UpdateType(Helpers.AddressOtherFieldPrefixHelper.OtherFieldPrefix.Attention)
    'OtherFieldItem2.Input = "Fred Jones"
    'OtherFieldItem2.UpdateType(Helpers.AddressOtherFieldPrefixHelper.OtherFieldPrefix.CareOf)
    'OtherFieldItem2.UpdateNameAndType("Bobby Frank", Helpers.AddressOtherFieldPrefixHelper.OtherFieldPrefix.Attention)

    'Dim OFI As AddressOtherField = New AddressOtherField()
    'OFI.UpdateNameAndType("John John", Helpers.AddressOtherFieldPrefixHelper.OtherFieldPrefix.CareOf)

End Class
