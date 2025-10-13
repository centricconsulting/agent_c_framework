Imports System.Xml
Imports System.Xml.Schema
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic
Imports QuickQuote.CommonMethods

Namespace QuickQuote.CommonObjects.Umbrella
    ''' <summary>
    ''' objects used to store underlying policy information (on Umbrella Policies)
    ''' </summary>
    ''' <remarks>typically found under QuickQuoteObject object (<see cref="QuickQuoteObject"/>) as a list</remarks>
    <Serializable()>
    Public Class QuickQuoteUnderlyingPolicy 'added 4/20/2020 for PUP/FUP
        Inherits QuickQuoteBaseGenericObject(Of Object)
        Implements IDisposable

        Dim qqHelper As New QuickQuoteHelperClass

        Private _DetailStatusCode As String
        Private _UnderlyingPolicyNum As String 'for reconciliation
        Private _Company As String
        Private _CompanyTypeId As String 'static data
        Private _EffectiveDate As String
        Private _ExpirationDate As String
        Private _LobId As String
        Private _PrimaryPolicyNumber As String
        Private _PolicyInfos As new List(Of PolicyInfo)
        Private _CanUsePolicyInfoNumForPolicyInfoReconciliation As Boolean

        Public Property DetailStatusCode As String
            Get
                Return _DetailStatusCode
            End Get
            Set(value As String)
                _DetailStatusCode = value
            End Set
        End Property
        Public Property UnderlyingPolicyNum As String 'for reconciliation
            Get
                Return _UnderlyingPolicyNum
            End Get
            Set(value As String)
                _UnderlyingPolicyNum = value
            End Set
        End Property
        Public Property Company As String
            Get
                Return _Company
            End Get
            Set(value As String)
                _Company = value
            End Set
        End Property
        Public Property CompanyTypeId As String 'static data
            Get
                Return _CompanyTypeId
            End Get
            Set(value As String)
                _CompanyTypeId = value
            End Set
        End Property
        Public Property EffectiveDate As String
            Get
                Return _EffectiveDate
            End Get
            Set(value As String)
                _EffectiveDate = value
                qqHelper.ConvertToShortDate(_EffectiveDate)
            End Set
        End Property
        Public Property ExpirationDate As String
            Get
                Return _ExpirationDate
            End Get
            Set(value As String)
                _ExpirationDate = value
                qqHelper.ConvertToShortDate(_ExpirationDate)
            End Set
        End Property
        Public Property LobId As String
            Get
                Return _LobId
            End Get
            Set(value As String)
                _LobId = value
            End Set
        End Property
        Public Property PrimaryPolicyNumber As String
            Get
                Return _PrimaryPolicyNumber
            End Get
            Set(value As String)
                _PrimaryPolicyNumber = value
            End Set
        End Property
        Public Property PolicyInfos As List(Of PolicyInfo)
            Get
                SetParentOfListItems(_PolicyInfos, "{A4T6E852-F2AC-4V81-765N-13A1E9E2B5E1}")
                If _PolicyInfos Is Nothing Then
                    _PolicyInfos = New List(Of PolicyInfo)
                End If
                Return _PolicyInfos
            End Get
            Set(value As List(Of PolicyInfo))
                _PolicyInfos = value
                SetParentOfListItems(_PolicyInfos, "{A4T6E852-F2AC-4V81-765N-13A1E9E2B5E1}")
            End Set
        End Property
        Public Property CanUsePolicyInfoNumForPolicyInfoReconciliation As Boolean
            Get
                Return _CanUsePolicyInfoNumForPolicyInfoReconciliation
            End Get
            Set(value As Boolean)
                _CanUsePolicyInfoNumForPolicyInfoReconciliation = value
            End Set
        End Property

        Public Property PolicyId As String
        Public Property PolicyImageNum As String

        Public Sub New()
            MyBase.New()
            SetDefaults()
        End Sub
        Private Sub SetDefaults()
            _DetailStatusCode = ""
            _UnderlyingPolicyNum = "" 'for reconciliation
            _Company = ""
            _CompanyTypeId = "" 'static data
            _EffectiveDate = ""
            _ExpirationDate = ""
            _LobId = ""
            _PrimaryPolicyNumber = ""
            _PolicyInfos = Nothing
            _CanUsePolicyInfoNumForPolicyInfoReconciliation = False
        End Sub
        Public Function HasValidUnderlyingPolicyNum() As Boolean 'added for reconciliation purposes
            Return qqHelper.IsValidQuickQuoteIdOrNum(_UnderlyingPolicyNum)
        End Function
        Public Sub RunParseMethods()
        '    ParseThruPolicyInfos()
        End Sub
        'Private Sub ParseThruPolicyInfos()
        '    If _PolicyInfos IsNot Nothing AndAlso _PolicyInfos.Count > 0 Then
        '        For Each i As PolicyInfo In _PolicyInfos
        '            'note: should only happen when parsing xml (FinalizeQuickQuote method)... shouldn't happen in FinalizeQuickQuoteLight method
        '            If _CanUsePolicyInfoNumForPolicyInfoReconciliation = False Then
        '                If i.HasValidPolicyInfoNum = True Then
        '                    _CanUsePolicyInfoNumForPolicyInfoReconciliation = True
        '                    'Exit For 'needs to keep going so child Parse routines can be called
        '                End If
        '            End If
        '            i.RunParseMethods()
        '        Next
        '    End If
        'End Sub
        Public Overrides Function ToString() As String
            Dim str As String = ""
            If Me IsNot Nothing Then
                If String.IsNullOrWhiteSpace(Me.PrimaryPolicyNumber) = False Then
                    str = qqHelper.appendText(str, "PrimaryPolicyNumber: " & Me.PrimaryPolicyNumber, vbCrLf)
                End If
                If Me.PolicyInfos IsNot Nothing AndAlso Me.PolicyInfos.Count > 0 Then
                    str = qqHelper.appendText(str, Me.PolicyInfos.Count.ToString & " PolicyInfos", vbCrLf)
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
        Protected Overloads Sub Dispose(disposing As Boolean)
            If Not Me.disposedValue Then
                If disposing Then
                    ' TODO: dispose managed state (managed objects).
                    qqHelper.DisposeString(_DetailStatusCode)
                    qqHelper.DisposeString(_UnderlyingPolicyNum) 'for reconciliation
                    qqHelper.DisposeString(_Company)
                    qqHelper.DisposeString(_CompanyTypeId) 'static data
                    qqHelper.DisposeString(_EffectiveDate)
                    qqHelper.DisposeString(_ExpirationDate)
                    qqHelper.DisposeString(_LobId)
                    qqHelper.DisposeString(_PrimaryPolicyNumber)
                    If _PolicyInfos IsNot Nothing Then
                        If _PolicyInfos.Count > 0 Then
                            For Each pi As PolicyInfo In _PolicyInfos
                                If pi IsNot Nothing Then
                                    pi.Dispose()
                                    pi = Nothing
                                End If
                            Next
                            _PolicyInfos.Clear()
                        End If
                        _PolicyInfos = Nothing
                    End If
                    _CanUsePolicyInfoNumForPolicyInfoReconciliation = Nothing

                    MyBase.Dispose()
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
        Public Overrides Sub Dispose()
            ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
            Dispose(True)
            GC.SuppressFinalize(Me)
        End Sub
#End Region

    End Class
End Namespace
