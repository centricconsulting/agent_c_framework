Imports Microsoft.VisualBasic
'added 12/20/2013
Imports QuickQuote.CommonMethods

Namespace QuickQuote.CommonObjects 'added namespace 12/20/2013
    ''' <summary>
    ''' object used to store email information
    ''' </summary>
    ''' <remarks>used w/ several objects; usually as a list such as w/ agencies, policyholders, clients, applicants, drivers, etc.</remarks>
    <Serializable()> _
    Public Class QuickQuoteEmail
        Inherits QuickQuoteBaseGenericObject(Of Object) 'added 8/4/2014
        Implements IDisposable

        Dim qqHelper As New QuickQuoteHelperClass 'added 12/11/2013

        Private _PolicyId As String
        Private _PolicyImageNum As String
        Private _DetailStatusCode As String 'added 6/25/2014 since Emails always seem to stay on rated image after flagging for delete
        Private _Address As String
        Private _EmailId As String
        Private _NameAddressSourceId As String
        Private _TypeId As String
        Private _Type As String 'added 5/2/2013

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
        Public Property DetailStatusCode As String 'added 6/25/2014 since Emails always seem to stay on rated image after flagging for delete
            Get
                Return _DetailStatusCode
            End Get
            Set(value As String)
                _DetailStatusCode = value
            End Set
        End Property
        Public Property Address As String
            Get
                Return _Address
            End Get
            Set(value As String)
                _Address = value
            End Set
        End Property
        Public Property EmailId As String
            Get
                Return _EmailId
            End Get
            Set(value As String)
                _EmailId = value
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
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>uses Diamond's EMailType table (0=N/A, 1=Home, 2=Business, 3=Other, 4=Commissions, 5=Documents)</remarks>
        Public Property TypeId As String
            Get
                Return _TypeId
            End Get
            Set(value As String)
                _TypeId = value
                '5/2/2013 - added logic to update new property
                '_Type = ""
                'If IsNumeric(_TypeId) = True Then
                '    Select Case _TypeId
                '        Case "0"
                '            _Type = "N/A"
                '        Case "1"
                '            _Type = "Home"
                '        Case "2"
                '            _Type = "Business"
                '        Case "3"
                '            _Type = "Other"
                '    End Select
                'End If
                'updated 12/11/2013
                _Type = qqHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteEmail, QuickQuoteHelperClass.QuickQuotePropertyName.TypeId, _TypeId)
            End Set
        End Property
        Public Property Type As String
            Get
                Return _Type
            End Get
            Set(value As String)
                _Type = value
                'Select Case _Type
                '    Case "N/A"
                '        _TypeId = "0"
                '    Case "Home"
                '        _TypeId = "1"
                '    Case "Business"
                '        _TypeId = "2"
                '    Case "Other"
                '        _TypeId = "3"
                '    Case Else
                '        _TypeId = ""
                'End Select
                'updated 12/11/2013
                _TypeId = qqHelper.GetStaticDataValueForText(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteEmail, QuickQuoteHelperClass.QuickQuotePropertyName.TypeId, _Type)
            End Set
        End Property

        Public Sub New()
            MyBase.New() 'added 8/4/2014
            SetDefaults()
        End Sub
        Private Sub SetDefaults()
            _PolicyId = ""
            _PolicyImageNum = ""
            _DetailStatusCode = "" 'added 6/25/2014 since Emails always seem to stay on rated image after flagging for delete
            _Address = ""
            _EmailId = ""
            _NameAddressSourceId = ""
            _TypeId = "" '*0=N/A; 1=Home; 2=Business; 3=Other
            _Type = ""
        End Sub
        Public Overrides Function ToString() As String 'added 6/29/2015
            Dim str As String = ""
            If Me IsNot Nothing Then
                If Me.Address <> "" Then
                    str = qqHelper.appendText(str, "Address: " & Me.Address, vbCrLf)
                End If
                If Me.TypeId <> "" Then
                    Dim t As String = ""
                    t = "TypeId: " & Me.TypeId
                    Dim eType As String = qqHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteEmail, QuickQuoteHelperClass.QuickQuotePropertyName.TypeId, Me.TypeId)
                    If eType <> "" Then
                        t &= " (" & eType & ")"
                    End If
                    str = qqHelper.appendText(str, t, vbCrLf)
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
                    If _DetailStatusCode IsNot Nothing Then 'added 6/25/2014 since Emails always seem to stay on rated image after flagging for delete
                        _DetailStatusCode = Nothing
                    End If
                    If _Address IsNot Nothing Then
                        _Address = Nothing
                    End If
                    If _EmailId IsNot Nothing Then
                        _EmailId = Nothing
                    End If
                    If _NameAddressSourceId IsNot Nothing Then
                        _NameAddressSourceId = Nothing
                    End If
                    If _TypeId IsNot Nothing Then
                        _TypeId = Nothing
                    End If
                    If _Type IsNot Nothing Then
                        _Type = Nothing
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
