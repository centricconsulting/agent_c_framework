Imports Microsoft.VisualBasic
Imports QuickQuote.CommonMethods 'added 5/19/2014

Namespace QuickQuote.CommonObjects 'added namespace 12/20/2013
    ''' <summary>
    ''' object used to store prior carrier information
    ''' </summary>
    ''' <remarks></remarks>
    <Serializable()> _
    Public Class QuickQuotePriorCarrier
        Inherits QuickQuoteBaseGenericObject(Of Object) 'added 8/4/2014
        Implements IDisposable

        Dim qqHelper As New QuickQuoteHelperClass 'added 5/19/2014

        Private _Name As QuickQuoteName
        Private _Address As QuickQuoteAddress
        Private _Phones As Generic.List(Of QuickQuotePhone)
        Private _Emails As Generic.List(Of QuickQuoteEmail)
        Private _PolicyId As String
        Private _PolicyImageNum As String
        Private _Amount As String
        Private _PreviousInsurerTypeId As String
        Private _PriorDurationTypeId As String
        Private _RolloverId As String
        Private _TypeId As String
        Private _PriorDurationWithCompany As String
        Private _PriorExpirationDate As String
        Private _PriorPolicy As String
        Private _ProofOfPriorInsurance As Boolean
        Private _Remarks As String

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
                SetParentOfListItems(_Phones, "{663B7C7B-F2AC-4BF6-965A-D30F41A05510}")
                Return _Phones
            End Get
            Set(value As Generic.List(Of QuickQuotePhone))
                _Phones = value
                SetParentOfListItems(_Phones, "{663B7C7B-F2AC-4BF6-965A-D30F41A05510}")
            End Set
        End Property
        Public Property Emails As Generic.List(Of QuickQuoteEmail)
            Get
                SetParentOfListItems(_Emails, "{663B7C7B-F2AC-4BF6-965A-D30F41A05511}")
                Return _Emails
            End Get
            Set(value As Generic.List(Of QuickQuoteEmail))
                _Emails = value
                SetParentOfListItems(_Emails, "{663B7C7B-F2AC-4BF6-965A-D30F41A05511}")
            End Set
        End Property
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
        Public Property Amount As String
            Get
                Return _Amount
            End Get
            Set(value As String)
                _Amount = value
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>uses Diamond's PreviousInsurer table</remarks>
        Public Property PreviousInsurerTypeId As String '12/18/2013 note: not in xml yet; 83 rows
            Get
                Return _PreviousInsurerTypeId
            End Get
            Set(value As String)
                _PreviousInsurerTypeId = value
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>uses Diamond's PriorCarrierDurationType table (-1=None, 0=N/A, 1=Years, 2=Months, 3=Weeks, 4=Days)</remarks>
        Public Property PriorDurationTypeId As String
            Get
                Return _PriorDurationTypeId
            End Get
            Set(value As String)
                _PriorDurationTypeId = value
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>uses Diamond's PriorCarrierRollover table (0=N/A, 1=Rollover1, 2=Rollover2, 3=Rollover3, 4=Rollover4, 5=Rollover5, 6=Rollover6, 7=Yes, 8=No)</remarks>
        Public Property RolloverId As String
            Get
                Return _RolloverId
            End Get
            Set(value As String)
                _RolloverId = value
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>uses Diamond's PriorCarrierType table (0=N/A, 1=Preferred, 2=Standard, 3=Non-Standard, 4=Multi-Market, 5=Assigned Risk, 6=Unknown)</remarks>
        Public Property TypeId As String
            Get
                Return _TypeId
            End Get
            Set(value As String)
                _TypeId = value
            End Set
        End Property
        Public Property PriorDurationWithCompany As String
            Get
                Return _PriorDurationWithCompany
            End Get
            Set(value As String)
                _PriorDurationWithCompany = value
            End Set
        End Property
        Public Property PriorExpirationDate As String
            Get
                Return _PriorExpirationDate
            End Get
            Set(value As String)
                _PriorExpirationDate = value
                qqHelper.ConvertToShortDate(_PriorExpirationDate) 'added 5/19/2014
            End Set
        End Property
        Public Property PriorPolicy As String
            Get
                Return _PriorPolicy
            End Get
            Set(value As String)
                _PriorPolicy = value
            End Set
        End Property
        Public Property ProofOfPriorInsurance As Boolean
            Get
                Return _ProofOfPriorInsurance
            End Get
            Set(value As Boolean)
                _ProofOfPriorInsurance = value
            End Set
        End Property
        Public Property Remarks As String
            Get
                Return _Remarks
            End Get
            Set(value As String)
                _Remarks = value
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
            _Name.NameAddressSourceId = "19" 'Prior Carrier
            _Address = New QuickQuoteAddress
            '_Phones = New Generic.List(Of QuickQuotePhone)
            _Phones = Nothing 'added 8/4/2014
            '_Emails = New Generic.List(Of QuickQuoteEmail)
            _Emails = Nothing 'added 8/4/2014
            _PolicyId = ""
            _PolicyImageNum = ""
            _Amount = ""
            _PreviousInsurerTypeId = ""
            _PriorDurationTypeId = ""
            _RolloverId = ""
            _TypeId = ""
            _PriorDurationWithCompany = ""
            _PriorExpirationDate = ""
            _PriorPolicy = ""
            _ProofOfPriorInsurance = False
            _Remarks = ""

            _DetailStatusCode = "" 'added 5/15/2019
        End Sub
        Public Function HasData() As Boolean 'added 12/20/2018
            Dim hasIt As Boolean = False

            If (_Name IsNot Nothing AndAlso _Name.HasData = True) OrElse (_Address IsNot Nothing AndAlso _Address.HasData = True) OrElse (_Phones IsNot Nothing AndAlso _Phones.Count > 0) OrElse (_Emails IsNot Nothing AndAlso _Emails.Count > 0) OrElse qqHelper.IsPositiveDecimalString(_Amount) = True OrElse qqHelper.IsPositiveIntegerString(_PreviousInsurerTypeId) = True OrElse qqHelper.IsPositiveIntegerString(_PriorDurationTypeId) = True OrElse qqHelper.IsPositiveIntegerString(_RolloverId) = True OrElse qqHelper.IsPositiveIntegerString(_TypeId) = True OrElse qqHelper.IsPositiveIntegerString(_PriorDurationWithCompany) = True OrElse qqHelper.IsValidDateString(_PriorExpirationDate, mustBeGreaterThanDefaultDate:=True) = True OrElse String.IsNullOrWhiteSpace(_PriorPolicy) = False OrElse _ProofOfPriorInsurance = True OrElse String.IsNullOrWhiteSpace(_Remarks) = False Then
                hasIt = True
            End If

            Return hasIt
        End Function
        Public Overrides Function ToString() As String 'added 6/30/2015
            Dim str As String = ""
            If Me IsNot Nothing Then
                If Me.Name IsNot Nothing Then
                    str = qqHelper.appendText(str, "DisplayName: " & Me.Name.DisplayName, vbCrLf)
                End If
                If Me.Address IsNot Nothing Then
                    str = qqHelper.appendText(str, "DisplayAddress: " & Me.Address.DisplayAddress, vbCrLf)
                End If
                If Me.PreviousInsurerTypeId <> "" Then
                    Dim i As String = ""
                    i = "PreviousInsurerTypeId: " & Me.PreviousInsurerTypeId
                    Dim iType As String = qqHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuotePriorCarrier, QuickQuoteHelperClass.QuickQuotePropertyName.PreviousInsurerTypeId, Me.PreviousInsurerTypeId)
                    If iType <> "" Then
                        i &= " (" & iType & ")"
                    End If
                    str = qqHelper.appendText(str, i, vbCrLf)
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
                    If _PolicyId IsNot Nothing Then
                        _PolicyId = Nothing
                    End If
                    If _PolicyImageNum IsNot Nothing Then
                        _PolicyImageNum = Nothing
                    End If
                    If _Amount IsNot Nothing Then
                        _Amount = Nothing
                    End If
                    If _PreviousInsurerTypeId IsNot Nothing Then
                        _PreviousInsurerTypeId = Nothing
                    End If
                    If _PriorDurationTypeId IsNot Nothing Then
                        _PriorDurationTypeId = Nothing
                    End If
                    If _RolloverId IsNot Nothing Then
                        _RolloverId = Nothing
                    End If
                    If _TypeId IsNot Nothing Then
                        _TypeId = Nothing
                    End If
                    If _PriorDurationWithCompany IsNot Nothing Then
                        _PriorDurationWithCompany = Nothing
                    End If
                    If _PriorExpirationDate IsNot Nothing Then
                        _PriorExpirationDate = Nothing
                    End If
                    If _PriorPolicy IsNot Nothing Then
                        _PriorPolicy = Nothing
                    End If
                    If _ProofOfPriorInsurance <> Nothing Then
                        _ProofOfPriorInsurance = Nothing
                    End If
                    If _Remarks IsNot Nothing Then
                        _Remarks = Nothing
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
