Imports Microsoft.VisualBasic
'added 12/20/2013
Imports QuickQuote.CommonMethods

Namespace QuickQuote.CommonObjects 'added namespace 12/20/2013
    ''' <summary>
    ''' object used to store operator information
    ''' </summary>
    ''' <remarks></remarks>
    <Serializable()> _
    Public Class QuickQuoteOperator 'added 8/1/2013
        Inherits QuickQuoteBaseGenericObject(Of Object) 'added 8/4/2014
        Implements IDisposable

        Dim qqHelper As New QuickQuoteHelperClass

        Private _PolicyId As String
        Private _PolicyImageNum As String

        Private _Name As QuickQuoteName
        Private _Address As QuickQuoteAddress
        Private _Phones As Generic.List(Of QuickQuotePhone) 'no longer sending to Diamond as-of 12/22/2014; re-rate can result in db key constraint error since image doesn't load them back in and our logic tries to add them new since it appears that nothing is there
        Private _Emails As Generic.List(Of QuickQuoteEmail) 'no longer sending to Diamond as-of 12/22/2014; re-rate can result in db key constraint error since image doesn't load them back in and our logic tries to add them new since it appears that nothing is there
        'added 8/6/2013
        Private _RelationshipTypeId As String 'may need matching RelationshipType variable/property
        'added 8/19/2013
        Private _OperatorNum As String

        'added 10/29/2014
        Private _IsForPolicyLevelAssignment As Boolean
        Private _PolicyLevelAssignmentNum As Integer

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
        Public Property Phones As Generic.List(Of QuickQuotePhone)
            Get
                SetParentOfListItems(_Phones, "{663B7C7B-F2AC-4BF6-965A-D30F41A05401}")
                Return _Phones
            End Get
            Set(value As Generic.List(Of QuickQuotePhone))
                _Phones = value
                SetParentOfListItems(_Phones, "{663B7C7B-F2AC-4BF6-965A-D30F41A05401}")
            End Set
        End Property
        Public Property Emails As Generic.List(Of QuickQuoteEmail)
            Get
                SetParentOfListItems(_Emails, "{663B7C7B-F2AC-4BF6-965A-D30F41A05402}")
                Return _Emails
            End Get
            Set(value As Generic.List(Of QuickQuoteEmail))
                _Emails = value
                SetParentOfListItems(_Emails, "{663B7C7B-F2AC-4BF6-965A-D30F41A05402}")
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>uses Diamond's RelationshipType table</remarks>
        Public Property RelationshipTypeId As String '0=None; 8=Policyholder; 5=Policyholder #2; etc.; added more 8/19/2013: 10=Relationship Not Known; 11=Not Related to Policyholder
            Get
                Return _RelationshipTypeId
            End Get
            Set(value As String)
                _RelationshipTypeId = value
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>must be set to avoid SQL foreign key error; links available operator to assigned operator</remarks>
        Public Property OperatorNum As String
            Get
                Return _OperatorNum
            End Get
            Set(value As String)
                _OperatorNum = value
            End Set
        End Property

        'added 10/29/2014
        Public Property IsForPolicyLevelAssignment As Boolean
            Get
                Return _IsForPolicyLevelAssignment
            End Get
            Set(value As Boolean)
                _IsForPolicyLevelAssignment = value
            End Set
        End Property
        Public Property PolicyLevelAssignmentNum As Integer
            Get
                Return _PolicyLevelAssignmentNum
            End Get
            Set(value As Integer)
                _PolicyLevelAssignmentNum = value
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
            _Name.NameAddressSourceId = "60" 'Operator
            _Address = New QuickQuoteAddress
            '_Phones = New Generic.List(Of QuickQuotePhone)
            _Phones = Nothing 'added 8/4/2014
            '_Emails = New Generic.List(Of QuickQuoteEmail)
            _Emails = Nothing 'added 8/4/2014
            _RelationshipTypeId = ""
            _OperatorNum = ""

            'added 10/29/2014
            _IsForPolicyLevelAssignment = False
            _PolicyLevelAssignmentNum = 0

            _DetailStatusCode = "" 'added 5/15/2019
        End Sub
        Public Function HasValidOperatorNum() As Boolean 'added 10/29/2014 for reconciliation purposes
            Return qqHelper.IsValidQuickQuoteIdOrNum(_OperatorNum)
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
                If Me.RelationshipTypeId <> "" Then
                    Dim rel As String = ""
                    rel = "RelationshipTypeId: " & Me.RelationshipTypeId
                    Dim relType As String = qqHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteOperator, QuickQuoteHelperClass.QuickQuotePropertyName.UserAgencyRelationshipTypeId, Me.RelationshipTypeId)
                    If relType <> "" Then
                        rel &= " (" & relType & ")"
                    End If
                    str = qqHelper.appendText(str, rel, vbCrLf)
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
                    If _RelationshipTypeId IsNot Nothing Then
                        _RelationshipTypeId = Nothing
                    End If
                    If _OperatorNum IsNot Nothing Then
                        _OperatorNum = Nothing
                    End If

                    'added 10/29/2014
                    _IsForPolicyLevelAssignment = Nothing
                    _PolicyLevelAssignmentNum = Nothing

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
