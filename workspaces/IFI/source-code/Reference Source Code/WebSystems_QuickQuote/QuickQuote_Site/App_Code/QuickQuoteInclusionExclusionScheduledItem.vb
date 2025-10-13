Imports Microsoft.VisualBasic

Namespace QuickQuote.CommonObjects 'added namespace 12/20/2013
    ''' <summary>
    ''' object used to store inclusion/exclusion scheduled item information
    ''' </summary>
    ''' <remarks>used with QuickQuoteInclusionExclusion (<see cref="QuickQuoteInclusionExclusion"/>)</remarks>
    <Serializable()> _
    Public Class QuickQuoteInclusionExclusionScheduledItem
        Inherits QuickQuoteBaseGenericObject(Of Object) 'added 8/4/2014
        Implements IDisposable

        Dim qqHelper As New QuickQuote.CommonMethods.QuickQuoteHelperClass 'added 5/15/2019

        Private _PolicyId As String
        Private _PolicyImageNum As String
        Private _Name As QuickQuoteName
        Private _Address As QuickQuoteAddress
        Private _NameAddressSourceId As String
        Private _ClueDate As String
        Private _Description As String
        Private _DescriptionOfAccident As String
        Private _InclusionExclusionNum As String
        Private _InclusionExclusionScheduledItemNum As String
        Private _ParagraphReferenceTypeId As String
        Private _PositionTitleTypeId As String
        Private _PropertyExclusionTypeId As String
        Private _PropertyTypeId As String
        Private _TypeId As String
        Private _WaiverOfSubrogationAmountTypeId As String
        Private _MvrDate As String
        Private _MarriageCertificate As Boolean
        Private _Photos As Boolean
        Private _Registration As Boolean
        Private _Salary As String
        Private _VehicleDescription As String
        Private _ViolationDate As String
        Private _WaiverOfSubrogationPercentage As String

        'added 8/13/2012
        Private _Phones As Generic.List(Of QuickQuotePhone)
        Private _Emails As Generic.List(Of QuickQuoteEmail)

        Private _DetailStatusCode As String 'added 5/15/2019

        Public Property PolicyId As String
            Get
                Return _PolicyId
            End Get
            Set(value As String)
                _PolicyId = value
            End Set
        End Property
        Public Property PolicyImageNum As String
            Get
                Return _PolicyImageNum
            End Get
            Set(value As String)
                _PolicyImageNum = value
            End Set
        End Property
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
        Public Property NameAddressSourceId As String
            Get
                Return _NameAddressSourceId
            End Get
            Set(value As String)
                _NameAddressSourceId = value
            End Set
        End Property
        Public Property ClueDate As String
            Get
                Return _ClueDate
            End Get
            Set(value As String)
                _ClueDate = value
            End Set
        End Property
        Public Property Description As String
            Get
                Return _Description
            End Get
            Set(value As String)
                _Description = value
            End Set
        End Property
        Public Property DescriptionOfAccident As String
            Get
                Return _DescriptionOfAccident
            End Get
            Set(value As String)
                _DescriptionOfAccident = value
            End Set
        End Property
        Public Property InclusionExclusionNum As String
            Get
                Return _InclusionExclusionNum
            End Get
            Set(value As String)
                _InclusionExclusionNum = value
            End Set
        End Property
        Public Property InclusionExclusionScheduledItemNum As String
            Get
                Return _InclusionExclusionScheduledItemNum
            End Get
            Set(value As String)
                _InclusionExclusionScheduledItemNum = value
            End Set
        End Property
        Public Property ParagraphReferenceTypeId As String
            Get
                Return _ParagraphReferenceTypeId
            End Get
            Set(value As String)
                _ParagraphReferenceTypeId = value
            End Set
        End Property
        Public Property PositionTitleTypeId As String
            Get
                Return _PositionTitleTypeId
            End Get
            Set(value As String)
                _PositionTitleTypeId = value
            End Set
        End Property
        Public Property PropertyExclusionTypeId As String
            Get
                Return _PropertyExclusionTypeId
            End Get
            Set(value As String)
                _PropertyExclusionTypeId = value
            End Set
        End Property
        Public Property PropertyTypeId As String
            Get
                Return _PropertyTypeId
            End Get
            Set(value As String)
                _PropertyTypeId = value
            End Set
        End Property
        Public Property TypeId As String
            Get
                Return _TypeId
            End Get
            Set(value As String)
                _TypeId = value
            End Set
        End Property
        Public Property WaiverOfSubrogationAmountTypeId As String
            Get
                Return _WaiverOfSubrogationAmountTypeId
            End Get
            Set(value As String)
                _WaiverOfSubrogationAmountTypeId = value
            End Set
        End Property
        Public Property MvrDate As String
            Get
                Return _MvrDate
            End Get
            Set(value As String)
                _MvrDate = value
            End Set
        End Property
        Public Property MarriageCertificate As Boolean
            Get
                Return _MarriageCertificate
            End Get
            Set(value As Boolean)
                _MarriageCertificate = value
            End Set
        End Property
        Public Property Photos As Boolean
            Get
                Return _Photos
            End Get
            Set(value As Boolean)
                _Photos = value
            End Set
        End Property
        Public Property Registration As Boolean
            Get
                Return _Registration
            End Get
            Set(value As Boolean)
                _Registration = value
            End Set
        End Property
        Public Property Salary As String
            Get
                Return _Salary
            End Get
            Set(value As String)
                _Salary = value
            End Set
        End Property
        Public Property VehicleDescription As String
            Get
                Return _VehicleDescription
            End Get
            Set(value As String)
                _VehicleDescription = value
            End Set
        End Property
        Public Property ViolationDate As String
            Get
                Return _ViolationDate
            End Get
            Set(value As String)
                _ViolationDate = value
            End Set
        End Property
        Public Property WaiverOfSubrogationPercentage As String
            Get
                Return _WaiverOfSubrogationPercentage
            End Get
            Set(value As String)
                _WaiverOfSubrogationPercentage = value
            End Set
        End Property

        Public Property Phones As Generic.List(Of QuickQuotePhone)
            Get
                SetParentOfListItems(_Phones, "{663B7C7B-F2AC-4BF6-965A-D30F41A05303}")
                Return _Phones
            End Get
            Set(value As Generic.List(Of QuickQuotePhone))
                _Phones = value
                SetParentOfListItems(_Phones, "{663B7C7B-F2AC-4BF6-965A-D30F41A05303}")
            End Set
        End Property
        Public Property Emails As Generic.List(Of QuickQuoteEmail)
            Get
                SetParentOfListItems(_Emails, "{663B7C7B-F2AC-4BF6-965A-D30F41A05304}")
                Return _Emails
            End Get
            Set(value As Generic.List(Of QuickQuoteEmail))
                _Emails = value
                SetParentOfListItems(_Emails, "{663B7C7B-F2AC-4BF6-965A-D30F41A05304}")
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
            _PolicyId = ""
            _PolicyImageNum = ""
            _Name = New QuickQuoteName
            _Address = New QuickQuoteAddress
            _NameAddressSourceId = ""
            _ClueDate = ""
            _Description = ""
            _DescriptionOfAccident = ""
            _InclusionExclusionNum = ""
            _InclusionExclusionScheduledItemNum = ""
            _ParagraphReferenceTypeId = ""
            _PositionTitleTypeId = ""
            _PropertyExclusionTypeId = ""
            _PropertyTypeId = ""
            _TypeId = ""
            _WaiverOfSubrogationAmountTypeId = ""
            _MvrDate = ""
            _MarriageCertificate = False
            _Photos = False
            _Registration = False
            _Salary = ""
            _VehicleDescription = ""
            _ViolationDate = ""
            _WaiverOfSubrogationPercentage = ""

            '_Phones = New Generic.List(Of QuickQuotePhone)
            _Phones = Nothing 'added 8/4/2014
            '_Emails = New Generic.List(Of QuickQuoteEmail)
            _Emails = Nothing 'added 8/4/2014

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
                    If _PolicyId IsNot Nothing Then
                        _PolicyId = Nothing
                    End If
                    If _PolicyImageNum IsNot Nothing Then
                        _PolicyImageNum = Nothing
                    End If
                    If _Name IsNot Nothing Then
                        _Name.Dispose()
                        _Name = Nothing
                    End If
                    If _Address IsNot Nothing Then
                        _Address.Dispose()
                        _Address = Nothing
                    End If
                    If _NameAddressSourceId IsNot Nothing Then
                        _NameAddressSourceId = Nothing
                    End If
                    If _ClueDate IsNot Nothing Then
                        _ClueDate = Nothing
                    End If
                    If _Description IsNot Nothing Then
                        _Description = Nothing
                    End If
                    If _DescriptionOfAccident IsNot Nothing Then
                        _DescriptionOfAccident = Nothing
                    End If
                    If _InclusionExclusionNum IsNot Nothing Then
                        _InclusionExclusionNum = Nothing
                    End If
                    If _InclusionExclusionScheduledItemNum IsNot Nothing Then
                        _InclusionExclusionScheduledItemNum = Nothing
                    End If
                    If _ParagraphReferenceTypeId IsNot Nothing Then
                        _ParagraphReferenceTypeId = Nothing
                    End If
                    If _PositionTitleTypeId IsNot Nothing Then
                        _PositionTitleTypeId = Nothing
                    End If
                    If _PropertyExclusionTypeId IsNot Nothing Then
                        _PropertyExclusionTypeId = Nothing
                    End If
                    If _PropertyTypeId IsNot Nothing Then
                        _PropertyTypeId = Nothing
                    End If
                    If _TypeId IsNot Nothing Then
                        _TypeId = Nothing
                    End If
                    If _WaiverOfSubrogationAmountTypeId IsNot Nothing Then
                        _WaiverOfSubrogationAmountTypeId = Nothing
                    End If
                    If _MvrDate IsNot Nothing Then
                        _MvrDate = Nothing
                    End If
                    If _MarriageCertificate <> Nothing Then
                        _MarriageCertificate = Nothing
                    End If
                    If _Photos <> Nothing Then
                        _Photos = Nothing
                    End If
                    If _Registration <> Nothing Then
                        _Registration = Nothing
                    End If
                    If _Salary IsNot Nothing Then
                        _Salary = Nothing
                    End If
                    If _VehicleDescription IsNot Nothing Then
                        _VehicleDescription = Nothing
                    End If
                    If _ViolationDate IsNot Nothing Then
                        _ViolationDate = Nothing
                    End If
                    If _WaiverOfSubrogationPercentage IsNot Nothing Then
                        _WaiverOfSubrogationPercentage = Nothing
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
            ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
            Dispose(True)
            GC.SuppressFinalize(Me)
        End Sub
#End Region

    End Class
End Namespace
