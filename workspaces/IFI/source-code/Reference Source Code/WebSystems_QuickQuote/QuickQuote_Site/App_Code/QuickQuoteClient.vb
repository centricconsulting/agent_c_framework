Imports Microsoft.VisualBasic
'added 12/20/2013
Imports QuickQuote.CommonMethods

Namespace QuickQuote.CommonObjects 'added namespace 12/20/2013
    ''' <summary>
    ''' object used to store client information
    ''' </summary>
    ''' <remarks></remarks>
    <Serializable()> _
    Public Class QuickQuoteClient
        Inherits QuickQuoteBaseGenericObject(Of Object) 'added 8/4/2014
        Implements IDisposable

        Dim qqHelper As New QuickQuoteHelperClass

        Private _IsNew As Boolean
        Private _AddedDate As String
        Private _LastModifiedDate As String
        Private _PCAddedDate As String

        Private _Name As QuickQuoteName
        Private _Address As QuickQuoteAddress
        Private _ClientId As String
        Private _Name2 As QuickQuoteName

        Private _Phones As Generic.List(Of QuickQuotePhone)

        Private _Emails As Generic.List(Of QuickQuoteEmail) 'added 7/23/2012

        'added 5/2/2013
        Private _PrimaryPhone As String
        Private _PrimaryEmail As String
        'added 10/7/2017
        Private _PrimaryPhoneTypeId As String
        Private _PrimaryEmailTypeId As String

        Private _OverwriteClientInfoForDiamondId As Boolean 'added 5/6/2014

        Public Property IsNew As Boolean
            Get
                Return _IsNew
            End Get
            Set(value As Boolean)
                _IsNew = value
            End Set
        End Property
        Public Property AddedDate As String
            Get
                Return _AddedDate
            End Get
            Set(value As String)
                _AddedDate = value
                qqHelper.ConvertToShortDate(_AddedDate)
            End Set
        End Property
        Public Property LastModifiedDate As String
            Get
                Return _LastModifiedDate
            End Get
            Set(value As String)
                _LastModifiedDate = value
                qqHelper.ConvertToTimeStamp(_LastModifiedDate)
            End Set
        End Property
        Public Property PCAddedDate As String
            Get
                Return _PCAddedDate
            End Get
            Set(value As String)
                _PCAddedDate = value
                qqHelper.ConvertToTimeStamp(_PCAddedDate)
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
        Public Property ClientId As String
            Get
                Return _ClientId
            End Get
            Set(value As String)
                _ClientId = value
            End Set
        End Property
        Public Property Name2 As QuickQuoteName
            Get
                SetObjectsParent(_Name2)
                Return _Name2
            End Get
            Set(value As QuickQuoteName)
                _Name2 = value
                SetObjectsParent(_Name2)
            End Set
        End Property

        Public Property Phones As Generic.List(Of QuickQuotePhone)
            Get
                SetParentOfListItems(_Phones, "{32BB5FB9-093F-42E5-8373-3CB836D45A39}")
                Return _Phones
            End Get
            Set(value As Generic.List(Of QuickQuotePhone))
                _Phones = value
                SetParentOfListItems(_Phones, "{32BB5FB9-093F-42E5-8373-3CB836D45A39}")
            End Set
        End Property

        Public Property Emails As Generic.List(Of QuickQuoteEmail)
            Get
                SetParentOfListItems(_Emails, "{32BB5FB9-093F-42E5-8373-3CB836D45A40}")
                Return _Emails
            End Get
            Set(value As Generic.List(Of QuickQuoteEmail))
                _Emails = value
                SetParentOfListItems(_Emails, "{32BB5FB9-093F-42E5-8373-3CB836D45A40}")
            End Set
        End Property

        ''' <summary>
        ''' uses Phones to determine the main one
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>may also take into account logic specific to this entity</remarks>
        Public Property PrimaryPhone As String
            Get
                SetPrimaryPhone()
                Return _PrimaryPhone
            End Get
            Set(value As String)
                _PrimaryPhone = value
            End Set
        End Property
        ''' <summary>
        ''' uses Emails to determine the main one
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>may also take into account logic specific to this entity</remarks>
        Public Property PrimaryEmail As String
            Get
                SetPrimaryEmail()
                Return _PrimaryEmail
            End Get
            Set(value As String)
                _PrimaryEmail = value
            End Set
        End Property
        'added 10/7/2017
        Public ReadOnly Property PrimaryPhoneTypeId As String
            Get
                Return _PrimaryPhoneTypeId
            End Get
        End Property
        Public ReadOnly Property PrimaryEmailTypeId As String
            Get
                Return _PrimaryEmailTypeId
            End Get
        End Property
        Public ReadOnly Property FirstPhoneWithNumber As QuickQuotePhone
            Get
                Return QuickQuoteHelperClass.FirstPhoneWithNumber(_Phones)
            End Get
        End Property
        Public ReadOnly Property FirstPhoneNumber As String
            Get
                Dim pNum As String = ""

                Dim firstPhone As QuickQuotePhone = FirstPhoneWithNumber
                If firstPhone IsNot Nothing Then
                    pNum = firstPhone.Number
                End If

                Return pNum
            End Get
        End Property
        Public ReadOnly Property FirstPhoneExt As String
            Get
                Dim pExt As String = ""

                Dim firstPhone As QuickQuotePhone = FirstPhoneWithNumber
                If firstPhone IsNot Nothing Then
                    pExt = firstPhone.Extension
                End If

                Return pExt
            End Get
        End Property
        Public ReadOnly Property FirstPhoneTypeId As String
            Get
                Dim pTypeId As String = ""

                Dim firstPhone As QuickQuotePhone = FirstPhoneWithNumber
                If firstPhone IsNot Nothing Then
                    pTypeId = firstPhone.TypeId
                End If

                Return pTypeId
            End Get
        End Property
        Public ReadOnly Property FirstPhoneNumberWithExtension As String
            Get
                Dim pNumWithExt As String = ""

                Dim firstPhone As QuickQuotePhone = FirstPhoneWithNumber
                If firstPhone IsNot Nothing Then
                    pNumWithExt = qqHelper.appendText(firstPhone.Number, If(qqHelper.IsPositiveIntegerString(firstPhone.Extension) = True, firstPhone.Extension, ""), " ")
                End If

                Return pNumWithExt
            End Get
        End Property
        Public ReadOnly Property FirstEmailWithAddress As QuickQuoteEmail
            Get
                Return QuickQuoteHelperClass.FirstEmailWithAddress(_Emails)
            End Get
        End Property
        Public ReadOnly Property FirstEmailAddress As String
            Get
                Dim eAdd As String = ""

                Dim firstEmail As QuickQuoteEmail = FirstEmailWithAddress
                If firstEmail IsNot Nothing Then
                    eAdd = firstEmail.Address
                End If

                Return eAdd
            End Get
        End Property
        Public ReadOnly Property FirstEmailTypeId As String
            Get
                Dim eTypeId As String = ""

                Dim firstEmail As QuickQuoteEmail = FirstEmailWithAddress
                If firstEmail IsNot Nothing Then
                    eTypeId = firstEmail.TypeId
                End If

                Return eTypeId
            End Get
        End Property
        'added 4/26/2014 like HasData prop on Policyholder object
        Public ReadOnly Property HasPrimaryData As Boolean
            Get
                If (_Name IsNot Nothing AndAlso _Name.HasData = True) OrElse (_Address IsNot Nothing AndAlso _Address.HasData = True) OrElse (_Phones IsNot Nothing AndAlso _Phones.Count > 0) OrElse (_Emails IsNot Nothing AndAlso _Emails.Count > 0) Then
                    Return True
                Else
                    Return False
                End If
            End Get
        End Property
        Public ReadOnly Property HasAnyData As Boolean '5/21/2014 note: may need to take into account clientId
            Get
                If HasPrimaryData = True OrElse (_Name2 IsNot Nothing AndAlso _Name2.HasData = True) Then
                    Return True
                Else
                    Return False
                End If
            End Get
        End Property

        Public Property OverwriteClientInfoForDiamondId As Boolean 'added 5/6/2014
            Get
                Return _OverwriteClientInfoForDiamondId
            End Get
            Set(value As Boolean)
                _OverwriteClientInfoForDiamondId = value
            End Set
        End Property

        Public Sub New()
            MyBase.New() 'added 8/4/2014
            SetDefaults()
        End Sub
        Private Sub SetDefaults()
            _IsNew = False
            _AddedDate = ""
            _LastModifiedDate = ""
            _PCAddedDate = ""

            _Name = New QuickQuoteName
            _Address = New QuickQuoteAddress
            _ClientId = ""
            _Name2 = New QuickQuoteName

            '_Phones = New Generic.List(Of QuickQuotePhone)
            _Phones = Nothing 'added 8/4/2014

            '_Emails = New Generic.List(Of QuickQuoteEmail)
            _Emails = Nothing 'added 8/4/2014

            _PrimaryPhone = ""
            _PrimaryEmail = ""
            'added 10/7/2017
            _PrimaryPhoneTypeId = ""
            _PrimaryEmailTypeId = ""

            _OverwriteClientInfoForDiamondId = False 'added 5/6/2014
        End Sub

        Private Sub SetPrimaryPhone()
            _PrimaryPhone = ""
            _PrimaryPhoneTypeId = "" 'added 10/7/2017
            Dim NaPhone As String = ""
            Dim HomePhone As String = ""
            Dim BusinessPhone As String = ""
            Dim FaxPhone As String = ""
            Dim CellularPhone As String = ""
            Dim PagerPhone As String = ""
            Dim OtherPhone As String = ""
            Dim UnknownPhone As String = ""

            qqHelper.SetPhoneVariables(_Phones, NaPhone, HomePhone, BusinessPhone, FaxPhone, CellularPhone, PagerPhone, OtherPhone, UnknownPhone)

            ''can optionally use list if you want to set more than 1 phone based on priority
            'Dim phoneList As New Generic.List(Of String)
            'phoneList.Add(HomePhone)
            'phoneList.Add(CellularPhone)
            ''...
            'For Each p As String In phoneList
            '    If p <> "" Then
            '        If _PrimaryPhone = "" Then
            '            _PrimaryPhone = p
            '        End If
            '        If _SecondaryPhone = "" Then
            '            _SecondaryPhone = p
            '            Exit For 'would exit once last phone is set
            '        End If
            '    End If
            'Next

            'added for reference 10/7/2017
            '0=N/A; 1=Home; 2=Business; 3=Fax; 4=Cellular; 5=Pager; 6=Other

            If HomePhone <> "" Then
                _PrimaryPhone = HomePhone
                'added 10/7/2017
                _PrimaryPhoneTypeId = qqHelper.GetStaticDataValueForText(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuotePhone, QuickQuoteHelperClass.QuickQuotePropertyName.TypeId, "Home")
            ElseIf CellularPhone <> "" Then
                _PrimaryPhone = CellularPhone
                'added 10/7/2017
                _PrimaryPhoneTypeId = qqHelper.GetStaticDataValueForText(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuotePhone, QuickQuoteHelperClass.QuickQuotePropertyName.TypeId, "Cellular")
            ElseIf PagerPhone <> "" Then
                _PrimaryPhone = PagerPhone
                'added 10/7/2017
                _PrimaryPhoneTypeId = qqHelper.GetStaticDataValueForText(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuotePhone, QuickQuoteHelperClass.QuickQuotePropertyName.TypeId, "Pager")
            ElseIf BusinessPhone <> "" Then
                _PrimaryPhone = BusinessPhone
                'added 10/7/2017
                _PrimaryPhoneTypeId = qqHelper.GetStaticDataValueForText(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuotePhone, QuickQuoteHelperClass.QuickQuotePropertyName.TypeId, "Business")
            ElseIf FaxPhone <> "" Then
                _PrimaryPhone = FaxPhone
                'added 10/7/2017
                _PrimaryPhoneTypeId = qqHelper.GetStaticDataValueForText(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuotePhone, QuickQuoteHelperClass.QuickQuotePropertyName.TypeId, "Fax")
            ElseIf OtherPhone <> "" Then
                _PrimaryPhone = OtherPhone
                'added 10/7/2017
                _PrimaryPhoneTypeId = qqHelper.GetStaticDataValueForText(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuotePhone, QuickQuoteHelperClass.QuickQuotePropertyName.TypeId, "Other")
            ElseIf NaPhone <> "" Then
                _PrimaryPhone = NaPhone
                'added 10/7/2017
                _PrimaryPhoneTypeId = qqHelper.GetStaticDataValueForText(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuotePhone, QuickQuoteHelperClass.QuickQuotePropertyName.TypeId, "N/A")
            ElseIf UnknownPhone <> "" Then
                _PrimaryPhone = UnknownPhone
                'added 10/7/2017
                _PrimaryPhoneTypeId = ""
            End If
        End Sub
        Private Sub SetPrimaryEmail()
            _PrimaryEmail = ""
            _PrimaryEmailTypeId = "" 'added 10/7/2017
            Dim NaEmail As String = ""
            Dim HomeEmail As String = ""
            Dim BusinessEmail As String = ""
            Dim OtherEmail As String = ""
            Dim UnKnownEmail As String = ""

            qqHelper.SetEmailVariables(_Emails, NaEmail, HomeEmail, BusinessEmail, OtherEmail, UnKnownEmail)

            'added for reference 10/7/2017
            '0=N/A; 1=Home; 2=Business; 3=Other

            'can optionally use list if you want to set more than 1 email based on priority (like commented phone logic above)
            If HomeEmail <> "" Then
                _PrimaryEmail = HomeEmail
                'added 10/7/2017
                _PrimaryEmailTypeId = qqHelper.GetStaticDataValueForText(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteEmail, QuickQuoteHelperClass.QuickQuotePropertyName.TypeId, "Home")
            ElseIf BusinessEmail <> "" Then
                _PrimaryEmail = BusinessEmail
                'added 10/7/2017
                _PrimaryEmailTypeId = qqHelper.GetStaticDataValueForText(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteEmail, QuickQuoteHelperClass.QuickQuotePropertyName.TypeId, "Business")
            ElseIf OtherEmail <> "" Then
                _PrimaryEmail = OtherEmail
                'added 10/7/2017
                _PrimaryEmailTypeId = qqHelper.GetStaticDataValueForText(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteEmail, QuickQuoteHelperClass.QuickQuotePropertyName.TypeId, "Other")
            ElseIf NaEmail <> "" Then
                _PrimaryEmail = NaEmail
                'added 10/7/2017
                _PrimaryEmailTypeId = qqHelper.GetStaticDataValueForText(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteEmail, QuickQuoteHelperClass.QuickQuotePropertyName.TypeId, "N/A")
            ElseIf UnKnownEmail <> "" Then
                _PrimaryEmail = UnKnownEmail
                'added 10/7/2017
                _PrimaryEmailTypeId = ""
            End If
        End Sub
        Public Function HasValidClientId() As Boolean 'added 4/27/2014
            Return qqHelper.IsValidQuickQuoteIdOrNum(_ClientId)
        End Function
        Public Overrides Function ToString() As String 'added 6/29/2015
            Dim str As String = ""
            If Me IsNot Nothing Then
                If Me.Name IsNot Nothing Then
                    str = qqHelper.appendText(str, "DisplayName: " & Me.Name.DisplayName, vbCrLf)
                End If
                If Me.Address IsNot Nothing Then
                    str = qqHelper.appendText(str, "DisplayAddress: " & Me.Address.DisplayAddress, vbCrLf)
                End If
                If Me.ClientId <> "" Then
                    str = qqHelper.appendText(str, "ClientId: " & Me.ClientId, vbCrLf)
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
                    If _IsNew <> Nothing Then
                        _IsNew = Nothing
                    End If
                    If _AddedDate IsNot Nothing Then
                        _AddedDate = Nothing
                    End If
                    If _LastModifiedDate IsNot Nothing Then
                        _LastModifiedDate = Nothing
                    End If
                    If _PCAddedDate IsNot Nothing Then
                        _PCAddedDate = Nothing
                    End If

                    If _Name IsNot Nothing Then
                        _Name.Dispose()
                        _Name = Nothing
                    End If
                    If _Address IsNot Nothing Then
                        _Address.Dispose()
                        _Address = Nothing
                    End If
                    If _ClientId IsNot Nothing Then
                        _ClientId = Nothing
                    End If
                    If _Name2 IsNot Nothing Then
                        _Name2.Dispose()
                        _Name2 = Nothing
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

                    If _PrimaryPhone IsNot Nothing Then
                        _PrimaryPhone = Nothing
                    End If
                    If _PrimaryEmail IsNot Nothing Then
                        _PrimaryEmail = Nothing
                    End If
                    'added 10/7/2017
                    qqHelper.DisposeString(_PrimaryPhoneTypeId)
                    qqHelper.DisposeString(_PrimaryEmailTypeId)

                    If _OverwriteClientInfoForDiamondId <> Nothing Then
                        _OverwriteClientInfoForDiamondId = Nothing
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
