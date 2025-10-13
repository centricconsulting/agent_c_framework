Imports Microsoft.VisualBasic
Imports QuickQuote.CommonMethods

Namespace QuickQuote.CommonObjects
    ''' <summary>
    ''' object used to store Diamond Num info for an object; allows for us to use the same object at different levels and set diamondNum for object on each packagePart
    ''' </summary>
    ''' <remarks></remarks>
    <Serializable()>
    Public Class QuickQuoteDiamondNumGroup 'added 10/29/2018
        Inherits QuickQuoteBaseGenericObject(Of Object)
        Implements IDisposable

        Dim qqHelper As New QuickQuoteHelperClass

        Private _DiamondNum As String
        Private _DiamondNum_MasterPart As String
        Private _DiamondNum_CGLPart As String
        Private _DiamondNum_CPRPart As String
        Private _DiamondNum_CIMPart As String
        Private _DiamondNum_CRMPart As String
        Private _DiamondNum_GARPart As String

        Public Property DiamondNum As String
            Get
                Return _DiamondNum
            End Get
            Set(value As String)
                _DiamondNum = value
            End Set
        End Property
        Public Property DiamondNum_MasterPart As String
            Get
                Return _DiamondNum_MasterPart
            End Get
            Set(value As String)
                _DiamondNum_MasterPart = value
            End Set
        End Property
        Public Property DiamondNum_CGLPart As String
            Get
                Return _DiamondNum_CGLPart
            End Get
            Set(value As String)
                _DiamondNum_CGLPart = value
            End Set
        End Property
        Public Property DiamondNum_CPRPart As String
            Get
                Return _DiamondNum_CPRPart
            End Get
            Set(value As String)
                _DiamondNum_CPRPart = value
            End Set
        End Property
        Public Property DiamondNum_CIMPart As String
            Get
                Return _DiamondNum_CIMPart
            End Get
            Set(value As String)
                _DiamondNum_CIMPart = value
            End Set
        End Property
        Public Property DiamondNum_CRMPart As String
            Get
                Return _DiamondNum_CRMPart
            End Get
            Set(value As String)
                _DiamondNum_CRMPart = value
            End Set
        End Property
        Public Property DiamondNum_GARPart As String
            Get
                Return _DiamondNum_GARPart
            End Get
            Set(value As String)
                _DiamondNum_GARPart = value
            End Set
        End Property

        Public Sub New()
            MyBase.New()
            SetDefaults()
        End Sub
        Public Sub New(Parent As Object)
            MyBase.New()
            SetDefaults()
            Me.SetParent = Parent
        End Sub
        Private Sub SetDefaults()
            _DiamondNum = ""
            _DiamondNum_MasterPart = ""
            _DiamondNum_CGLPart = ""
            _DiamondNum_CPRPart = ""
            _DiamondNum_CIMPart = ""
            _DiamondNum_CRMPart = ""
            _DiamondNum_GARPart = ""
        End Sub
        Public Function DiamondNumForPackagePartType(ByVal packagePartType As QuickQuoteXML.QuickQuotePackagePartType) As String
            Select Case packagePartType
                Case QuickQuoteXML.QuickQuotePackagePartType.Package
                    Return _DiamondNum_MasterPart
                Case QuickQuoteXML.QuickQuotePackagePartType.GeneralLiability
                    Return _DiamondNum_CGLPart
                Case QuickQuoteXML.QuickQuotePackagePartType.CommercialProperty
                    Return _DiamondNum_CPRPart
                Case QuickQuoteXML.QuickQuotePackagePartType.InlandMarine
                    Return _DiamondNum_CIMPart
                Case QuickQuoteXML.QuickQuotePackagePartType.Crime
                    Return _DiamondNum_CRMPart
                Case QuickQuoteXML.QuickQuotePackagePartType.Garage
                    Return _DiamondNum_GARPart
                Case Else
                    Return _DiamondNum
            End Select
        End Function
        Public Function HasValidDiamondNum(Optional ByVal packagePartType As QuickQuoteXML.QuickQuotePackagePartType = QuickQuoteXML.QuickQuotePackagePartType.None) As Boolean
            Return qqHelper.IsValidQuickQuoteIdOrNum(DiamondNumForPackagePartType(packagePartType))
        End Function
        Public Sub SetDiamondNumForPackagePartType(ByVal diaNum As String, ByVal packagePartType As QuickQuoteXML.QuickQuotePackagePartType)
            Select Case packagePartType
                Case QuickQuoteXML.QuickQuotePackagePartType.Package
                    _DiamondNum_MasterPart = diaNum
                Case QuickQuoteXML.QuickQuotePackagePartType.GeneralLiability
                    _DiamondNum_CGLPart = diaNum
                Case QuickQuoteXML.QuickQuotePackagePartType.CommercialProperty
                    _DiamondNum_CPRPart = diaNum
                Case QuickQuoteXML.QuickQuotePackagePartType.InlandMarine
                    _DiamondNum_CIMPart = diaNum
                Case QuickQuoteXML.QuickQuotePackagePartType.Crime
                    _DiamondNum_CRMPart = diaNum
                Case QuickQuoteXML.QuickQuotePackagePartType.Garage
                    _DiamondNum_GARPart = diaNum
                Case Else
                    _DiamondNum = diaNum
            End Select
        End Sub
        Public Overrides Function ToString() As String
            Dim str As String = ""
            If Me IsNot Nothing Then
                If String.IsNullOrWhiteSpace(_DiamondNum) = False Then
                    str = qqHelper.appendText(str, "DiamondNum: " & _DiamondNum, vbCrLf)
                End If
                If String.IsNullOrWhiteSpace(_DiamondNum_MasterPart) = False Then
                    str = qqHelper.appendText(str, "DiamondNum_MasterPart: " & _DiamondNum_MasterPart, vbCrLf)
                End If
                If String.IsNullOrWhiteSpace(_DiamondNum_CGLPart) = False Then
                    str = qqHelper.appendText(str, "DiamondNum_CGLPart: " & _DiamondNum_CGLPart, vbCrLf)
                End If
                If String.IsNullOrWhiteSpace(_DiamondNum_CPRPart) = False Then
                    str = qqHelper.appendText(str, "DiamondNum_CPRPart: " & _DiamondNum_CPRPart, vbCrLf)
                End If
                If String.IsNullOrWhiteSpace(_DiamondNum_CIMPart) = False Then
                    str = qqHelper.appendText(str, "DiamondNum_CIMPart: " & _DiamondNum_CIMPart, vbCrLf)
                End If
                If String.IsNullOrWhiteSpace(_DiamondNum_CRMPart) = False Then
                    str = qqHelper.appendText(str, "DiamondNum_CRMPart: " & _DiamondNum_CRMPart, vbCrLf)
                End If
                If String.IsNullOrWhiteSpace(_DiamondNum_GARPart) = False Then
                    str = qqHelper.appendText(str, "DiamondNum_GARPart: " & _DiamondNum_GARPart, vbCrLf)
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
        'updated w/ QuickQuoteBaseObject inheritance
        Protected Overloads Sub Dispose(disposing As Boolean)
            If Not Me.disposedValue Then
                If disposing Then
                    ' TODO: dispose managed state (managed objects).
                    qqHelper.DisposeString(_DiamondNum)
                    qqHelper.DisposeString(_DiamondNum_MasterPart)
                    qqHelper.DisposeString(_DiamondNum_CGLPart)
                    qqHelper.DisposeString(_DiamondNum_CPRPart)
                    qqHelper.DisposeString(_DiamondNum_CIMPart)
                    qqHelper.DisposeString(_DiamondNum_CRMPart)
                    qqHelper.DisposeString(_DiamondNum_GARPart)

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
        'updated  w/ QuickQuoteBaseObject inheritance
        Public Overrides Sub Dispose() 'Implements IDisposable.Dispose
            ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
            Dispose(True)
            GC.SuppressFinalize(Me)
        End Sub
#End Region

    End Class
End Namespace
