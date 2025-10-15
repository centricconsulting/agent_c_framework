Imports Microsoft.VisualBasic
Imports QuickQuote.CommonMethods

Namespace QuickQuote.CommonObjects
    ''' <summary>
    ''' object used to hold additional interest list information
    ''' </summary>
    ''' <remarks>AIs can be tied to vehicles, buildings, etc.; aiList info is for the name/address/etc. stuff</remarks>
    <Serializable()>
    Public Class QuickQuoteAdditionalInterestList 'added 4/1/2020
        Inherits QuickQuoteBaseGenericObject(Of Object)
        Implements IDisposable

        Dim qqHelper As New QuickQuoteHelperClass
        Dim chc As New CommonHelperClass

        Private _Name As QuickQuoteName
        Private _Address As QuickQuoteAddress
        Private _Phones As Generic.List(Of QuickQuotePhone)
        Private _Emails As Generic.List(Of QuickQuoteEmail)

        'AdditionalInterestList nodes
        Private _GroupTypeId As String 'Diamond table AdditionalInterestGroupType
        Private _ListId As String 'also at AdditionalInterest level
        Private _AgencyId As String
        Private _SingleEntry As Boolean
        Private _StatusCode As String

        Private _HasAdditionalInterestListNameChanged As Boolean 'added 4/29/2014
        Private _OverwriteAdditionalInterestListInfoForDiamondId As Boolean 'added 5/6/2014
        Private _HasAdditionalInterestListIdChanged As Boolean 'added 5/6/2014

        Public Property Name As QuickQuoteName
            Get
                SetObjectsParent(_Name)
                Return _Name
            End Get
            Set(value As QuickQuoteName)
                _Name = value
                If _Name IsNot Nothing AndAlso _Name.NameAddressSourceId = "" Then
                    _Name.NameAddressSourceId = "12" 'Additional Interest
                    'SetObjectsParent(_Name)
                End If
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
                SetParentOfListItems(_Phones, "{32BB5FB9-093F-42E5-8373-3CB836D45A41}")
                Return _Phones
            End Get
            Set(value As Generic.List(Of QuickQuotePhone))
                _Phones = value
                SetParentOfListItems(_Phones, "{32BB5FB9-093F-42E5-8373-3CB836D45A41}")
            End Set
        End Property
        Public Property Emails As Generic.List(Of QuickQuoteEmail)
            Get
                SetParentOfListItems(_Emails, "{32BB5FB9-093F-42E5-8373-3CB836D45A42}")
                Return _Emails
            End Get
            Set(value As Generic.List(Of QuickQuoteEmail))
                _Emails = value
                SetParentOfListItems(_Emails, "{32BB5FB9-093F-42E5-8373-3CB836D45A42}")
            End Set
        End Property

        Public Property GroupTypeId As String
            Get
                Return _GroupTypeId
            End Get
            Set(value As String)
                _GroupTypeId = value
            End Set
        End Property
        Public Property ListId As String
            Get
                Return _ListId
            End Get
            Set(value As String)
                _ListId = value
            End Set
        End Property
        Public Property AgencyId As String
            Get
                Return _AgencyId
            End Get
            Set(value As String)
                _AgencyId = value
            End Set
        End Property
        Public Property SingleEntry As Boolean
            Get
                Return _SingleEntry
            End Get
            Set(value As Boolean)
                _SingleEntry = value
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

        Public Property HasAdditionalInterestListNameChanged As Boolean 'added 4/29/2014
            Get
                Return _HasAdditionalInterestListNameChanged
            End Get
            Set(value As Boolean)
                _HasAdditionalInterestListNameChanged = value
            End Set
        End Property
        Public Property OverwriteAdditionalInterestListInfoForDiamondId As Boolean 'added 5/6/2014
            Get
                Return _OverwriteAdditionalInterestListInfoForDiamondId
            End Get
            Set(value As Boolean)
                _OverwriteAdditionalInterestListInfoForDiamondId = value
            End Set
        End Property
        Public Property HasAdditionalInterestListIdChanged As Boolean 'added 5/6/2014
            Get
                Return _HasAdditionalInterestListIdChanged
            End Get
            Set(value As Boolean)
                _HasAdditionalInterestListIdChanged = value
            End Set
        End Property


        Public Sub New()
            MyBase.New() 'added 8/4/2014
            SetDefaults()
        End Sub
        Private Sub SetDefaults()
            _Name = New QuickQuoteName
            '_Name.NameAddressSourceId = "12" 'Additional Interest '5/12/2014 note: may need to update to use static data list... done
            _Name.NameAddressSourceId = qqHelper.GetStaticDataValueForText(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteName, QuickQuoteHelperClass.QuickQuotePropertyName.NameAddressSourceId, "Additional Interest")
            _Address = New QuickQuoteAddress
            '_Phones = New Generic.List(Of QuickQuotePhone)
            '_Emails = New Generic.List(Of QuickQuoteEmail)

            _GroupTypeId = "0" 'defaulted to 0 (Not Assigned) on 8/23/2012
            _ListId = ""
            _AgencyId = ""
            _SingleEntry = True 'False'defaulted to True on 8/23/2012
            _StatusCode = "1" 'defaulted to 1 on 8/23/2012

            _HasAdditionalInterestListNameChanged = False 'added 4/29/2014
            _OverwriteAdditionalInterestListInfoForDiamondId = False 'added 5/6/2014
            _HasAdditionalInterestListIdChanged = False 'added 5/6/2014
        End Sub
        Public Function HasValidAdditionalInterestListId() As Boolean 'added 4/27/2014
            Return qqHelper.IsValidQuickQuoteIdOrNum(_ListId)
        End Function
        Public Function HasInfo() As Boolean 'added 4/1/2020
            If (_Name IsNot Nothing AndAlso _Name.HasData = True) OrElse (_Address IsNot Nothing AndAlso _Address.HasData = True) OrElse (_Phones IsNot Nothing AndAlso _Phones.Count > 0) OrElse (_Emails IsNot Nothing AndAlso _Emails.Count > 0) OrElse qqHelper.IsPositiveIntegerString(_GroupTypeId) = True OrElse HasValidAdditionalInterestListId() = True OrElse qqHelper.IsPositiveIntegerString(_AgencyId) = True Then
                Return True
            Else
                Return False
            End If
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
                If Me.ListId <> "" Then
                    str = qqHelper.appendText(str, "ListId: " & Me.ListId, vbCrLf)
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

                    If _GroupTypeId IsNot Nothing Then
                        _GroupTypeId = Nothing
                    End If
                    If _ListId IsNot Nothing Then
                        _ListId = Nothing
                    End If
                    If _AgencyId IsNot Nothing Then
                        _AgencyId = Nothing
                    End If
                    If _SingleEntry <> Nothing Then
                        _SingleEntry = Nothing
                    End If
                    If _StatusCode IsNot Nothing Then
                        _StatusCode = Nothing
                    End If

                    If _HasAdditionalInterestListNameChanged <> Nothing Then
                        _HasAdditionalInterestListNameChanged = Nothing
                    End If
                    If _OverwriteAdditionalInterestListInfoForDiamondId <> Nothing Then
                        _OverwriteAdditionalInterestListInfoForDiamondId = Nothing
                    End If
                    If _HasAdditionalInterestListIdChanged <> Nothing Then
                        _HasAdditionalInterestListIdChanged = Nothing
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
