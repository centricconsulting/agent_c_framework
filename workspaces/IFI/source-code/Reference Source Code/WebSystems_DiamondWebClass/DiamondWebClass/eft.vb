Imports Microsoft.VisualBasic

Public Class eft
    Implements IDisposable

    Private _ABAroutingNumber As String
    Private _AccountNumber As String
    Private _BankAccountType As Integer
    Private _DeductionDay As Integer

    Private _PolicyImageNumber As Integer
    Private _PolicyID As Integer

    Public Function insertEFTaccount() As Boolean

        Dim req As New Diamond.Common.Services.Messages.EFTService.SavePolicyEftInfo.Request
        Dim res As New Diamond.Common.Services.Messages.EFTService.SavePolicyEftInfo.Response

        With req.RequestData
            If .EFTAccountPolicy Is Nothing Then
                .EFTAccountPolicy = New Diamond.Common.Objects.EFT.EftAccountPolicy
            End If
            .EFTAccountPolicy.EftAccount.RoutingNumber = ABAroutingNumber
            .EFTAccountPolicy.EftAccount.AccountNumber = AccountNumber
            .EFTAccountPolicy.EftAccount.BankAccountType = BankAccountType '1 checking, 2 savings
            .EFTAccountPolicy.EftAccount.DeductionDay = DeductionDay

            .EFTAccountPolicy.EftAccount.IsECheck = True
            '9/26/2019 note: may need to make the following update for 534
            '.EFTAccountPolicy.EftAccount.EFTAccountUsageType = Diamond.Common.Enums.Eft.AccountUsageType.ECheck
            .EFTAccountPolicy.EftAccount.EftTransactionType = 1 '1 debit, 2 credit

            .EFTAccountPolicy.EftAccount.PolicyImageNumber = PolicyImageNumber
            .EFTAccountPolicy.PolicyId = PolicyID

        End With

        Using proxy As New Proxies.EFTServiceProxy
            res = proxy.SavePolicyEftInfo(req)
            Return 1
        End Using

        Return 0
    End Function

    Public Sub New()
        setDefaults()
    End Sub

    Public Sub setDefaults()
        ABAroutingNumber = String.Empty
        AccountNumber = String.Empty
        BankAccountType = Nothing
        DeductionDay = Nothing
        PolicyImageNumber = Nothing
        PolicyID = Nothing

    End Sub

    Public Property ABAroutingNumber() As String
        Get
            Return _ABAroutingNumber
        End Get
        Set(ByVal value As String)
            _ABAroutingNumber = value
        End Set
    End Property

    Public Property AccountNumber() As String
        Get
            Return _AccountNumber
        End Get
        Set(ByVal value As String)
            _AccountNumber = value
        End Set
    End Property

    Public Property BankAccountType() As Integer
        Get
            Return _BankAccountType
        End Get
        Set(ByVal value As Integer)
            _BankAccountType = value
        End Set
    End Property

    Public Property DeductionDay() As Integer
        Get
            Return _DeductionDay
        End Get
        Set(ByVal value As Integer)
            _DeductionDay = value
        End Set
    End Property

    Public Property PolicyImageNumber() As Integer
        Get
            Return _PolicyImageNumber
        End Get
        Set(ByVal value As Integer)
            _PolicyImageNumber = value
        End Set
    End Property

    Public Property PolicyID() As Integer
        Get
            Return _PolicyID
        End Get
        Set(ByVal value As Integer)
            _PolicyID = value
        End Set
    End Property

#Region "IDisposable Support"
    Private disposedValue As Boolean ' To detect redundant calls

    ' IDisposable
    Protected Overridable Sub Dispose(ByVal disposing As Boolean)
        If Not Me.disposedValue Then
            If disposing Then
                ' TODO: dispose managed state (managed objects).
                ABAroutingNumber = String.Empty
                AccountNumber = String.Empty
                BankAccountType = Nothing
                DeductionDay = Nothing
                PolicyImageNumber = Nothing
                PolicyID = Nothing
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
    Public Sub Dispose() Implements IDisposable.Dispose
        ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
        Dispose(True)
        GC.SuppressFinalize(Me)
    End Sub
#End Region

End Class