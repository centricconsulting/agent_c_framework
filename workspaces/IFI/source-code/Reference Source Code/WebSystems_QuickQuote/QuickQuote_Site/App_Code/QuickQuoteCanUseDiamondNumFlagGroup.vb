Imports Microsoft.VisualBasic
Imports QuickQuote.CommonMethods

Namespace QuickQuote.CommonObjects
    ''' <summary>
    ''' object used to store Diamond Num info for an object; allows for us to use the same object at different levels and set diamondNum for object on each packagePart
    ''' </summary>
    ''' <remarks></remarks>
    <Serializable()>
    Public Class QuickQuoteCanUseDiamondNumFlagGroup 'added 10/29/2018
        Inherits QuickQuoteBaseGenericObject(Of Object)
        Implements IDisposable

        Dim qqHelper As New QuickQuoteHelperClass

        Private _CanUseDiamondNumForReconciliation As Boolean
        Private _CanUseDiamondNumForMasterPartReconciliation As Boolean
        Private _CanUseDiamondNumForCGLPartReconciliation As Boolean
        Private _CanUseDiamondNumForCPRPartReconciliation As Boolean
        Private _CanUseDiamondNumForCIMPartReconciliation As Boolean
        Private _CanUseDiamondNumForCRMPartReconciliation As Boolean
        Private _CanUseDiamondNumForGARPartReconciliation As Boolean

        Public Property CanUseDiamondNumForReconciliation As Boolean
            Get
                Return _CanUseDiamondNumForReconciliation
            End Get
            Set(value As Boolean)
                _CanUseDiamondNumForReconciliation = value
            End Set
        End Property
        Public Property CanUseDiamondNumForMasterPartReconciliation As Boolean
            Get
                Return _CanUseDiamondNumForMasterPartReconciliation
            End Get
            Set(value As Boolean)
                _CanUseDiamondNumForMasterPartReconciliation = value
            End Set
        End Property
        Public Property CanUseDiamondNumForCGLPartReconciliation As Boolean
            Get
                Return _CanUseDiamondNumForCGLPartReconciliation
            End Get
            Set(value As Boolean)
                _CanUseDiamondNumForCGLPartReconciliation = value
            End Set
        End Property
        Public Property CanUseDiamondNumForCPRPartReconciliation As Boolean
            Get
                Return _CanUseDiamondNumForCPRPartReconciliation
            End Get
            Set(value As Boolean)
                _CanUseDiamondNumForCPRPartReconciliation = value
            End Set
        End Property
        Public Property CanUseDiamondNumForCIMPartReconciliation As Boolean
            Get
                Return _CanUseDiamondNumForCIMPartReconciliation
            End Get
            Set(value As Boolean)
                _CanUseDiamondNumForCIMPartReconciliation = value
            End Set
        End Property
        Public Property CanUseDiamondNumForCRMPartReconciliation As Boolean
            Get
                Return _CanUseDiamondNumForCRMPartReconciliation
            End Get
            Set(value As Boolean)
                _CanUseDiamondNumForCRMPartReconciliation = value
            End Set
        End Property
        Public Property CanUseDiamondNumForGARPartReconciliation As Boolean
            Get
                Return _CanUseDiamondNumForGARPartReconciliation
            End Get
            Set(value As Boolean)
                _CanUseDiamondNumForGARPartReconciliation = value
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
            _CanUseDiamondNumForReconciliation = False
            _CanUseDiamondNumForMasterPartReconciliation = False
            _CanUseDiamondNumForCGLPartReconciliation = False
            _CanUseDiamondNumForCPRPartReconciliation = False
            _CanUseDiamondNumForCIMPartReconciliation = False
            _CanUseDiamondNumForCRMPartReconciliation = False
            _CanUseDiamondNumForGARPartReconciliation = False
        End Sub
        Public Function CanUseDiamondNumFlagForPackagePartType(ByVal packagePartType As QuickQuoteXML.QuickQuotePackagePartType) As Boolean
            Select Case packagePartType
                Case QuickQuoteXML.QuickQuotePackagePartType.Package
                    Return CanUseDiamondNumForMasterPartReconciliation
                Case QuickQuoteXML.QuickQuotePackagePartType.GeneralLiability
                    Return CanUseDiamondNumForCGLPartReconciliation
                Case QuickQuoteXML.QuickQuotePackagePartType.CommercialProperty
                    Return CanUseDiamondNumForCPRPartReconciliation
                Case QuickQuoteXML.QuickQuotePackagePartType.InlandMarine
                    Return CanUseDiamondNumForCIMPartReconciliation
                Case QuickQuoteXML.QuickQuotePackagePartType.Crime
                    Return CanUseDiamondNumForCRMPartReconciliation
                Case QuickQuoteXML.QuickQuotePackagePartType.Garage
                    Return CanUseDiamondNumForGARPartReconciliation
                Case Else
                    Return CanUseDiamondNumForReconciliation
            End Select
        End Function
        Public Sub SetCanUseDiamondNumFlagForPackagePartType(ByVal canUse As Boolean, ByVal packagePartType As QuickQuoteXML.QuickQuotePackagePartType)
            Select Case packagePartType
                Case QuickQuoteXML.QuickQuotePackagePartType.Package
                    CanUseDiamondNumForMasterPartReconciliation = canUse
                Case QuickQuoteXML.QuickQuotePackagePartType.GeneralLiability
                    CanUseDiamondNumForCGLPartReconciliation = canUse
                Case QuickQuoteXML.QuickQuotePackagePartType.CommercialProperty
                    CanUseDiamondNumForCPRPartReconciliation = canUse
                Case QuickQuoteXML.QuickQuotePackagePartType.InlandMarine
                    CanUseDiamondNumForCIMPartReconciliation = canUse
                Case QuickQuoteXML.QuickQuotePackagePartType.Crime
                    CanUseDiamondNumForCRMPartReconciliation = canUse
                Case QuickQuoteXML.QuickQuotePackagePartType.Garage
                    CanUseDiamondNumForGARPartReconciliation = canUse
                Case Else
                    CanUseDiamondNumForReconciliation = canUse
            End Select
        End Sub
        Public Overrides Function ToString() As String
            Dim str As String = ""
            If Me IsNot Nothing Then
                str = qqHelper.appendText(str, "CanUseDiamondNumForReconciliation: " & _CanUseDiamondNumForReconciliation.ToString, vbCrLf)
                str = qqHelper.appendText(str, "CanUseDiamondNumForMasterPartReconciliation: " & _CanUseDiamondNumForMasterPartReconciliation.ToString, vbCrLf)
                str = qqHelper.appendText(str, "CanUseDiamondNumForCGLPartReconciliation: " & _CanUseDiamondNumForCGLPartReconciliation.ToString, vbCrLf)
                str = qqHelper.appendText(str, "CanUseDiamondNumForCPRPartReconciliation: " & _CanUseDiamondNumForCPRPartReconciliation.ToString, vbCrLf)
                str = qqHelper.appendText(str, "CanUseDiamondNumForCIMPartReconciliation: " & _CanUseDiamondNumForCIMPartReconciliation.ToString, vbCrLf)
                str = qqHelper.appendText(str, "CanUseDiamondNumForCRMPartReconciliation: " & _CanUseDiamondNumForCRMPartReconciliation.ToString, vbCrLf)
                str = qqHelper.appendText(str, "CanUseDiamondNumForGARPartReconciliation: " & _CanUseDiamondNumForGARPartReconciliation.ToString, vbCrLf)
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
                    _CanUseDiamondNumForReconciliation = Nothing
                    _CanUseDiamondNumForMasterPartReconciliation = Nothing
                    _CanUseDiamondNumForCGLPartReconciliation = Nothing
                    _CanUseDiamondNumForCPRPartReconciliation = Nothing
                    _CanUseDiamondNumForCIMPartReconciliation = Nothing
                    _CanUseDiamondNumForCRMPartReconciliation = Nothing
                    _CanUseDiamondNumForGARPartReconciliation = Nothing

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