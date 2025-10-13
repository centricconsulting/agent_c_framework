Imports Microsoft.VisualBasic
'added 12/20/2013
Imports QuickQuote.CommonMethods

Namespace QuickQuote.CommonObjects 'added namespace 12/20/2013
    ''' <summary>
    ''' object used to store modifier information
    ''' </summary>
    ''' <remarks></remarks>
    <Serializable()> _
    Public Class QuickQuoteModifier
        Inherits QuickQuoteBaseGenericObject(Of Object) 'added 8/4/2014
        Implements IDisposable

        Dim qqHelper As New QuickQuoteHelperClass

        Private _PolicyId As String
        Private _PolicyImageNum As String
        Private _DetailStatusCode As String 'added 6/25/2014 since Modifiers always seem to stay on rated image after flagging for delete
        Private _CheckboxSelected As Boolean
        Private _ModifierAmount As String
        Private _ModifierGroupId As String
        Private _ModifierLevelId As String
        Private _ModifierOptionDate As String
        Private _ModifierOptionDescription As String
        Private _ModifierOptionId As String
        Private _ModifierTypeId As String
        Private _ParentModifierTypeId As String
        'added 10/16/2014
        Private _ModifierType As String
        Private _ParentModifierType As String
        Private _ModifierGroup As String

        'added 10/19/2018
        Private _ModifierNum As String
        Private _ModifierNum_MasterPart As String
        Private _ModifierNum_CGLPart As String
        Private _ModifierNum_CPRPart As String
        Private _ModifierNum_CIMPart As String
        Private _ModifierNum_CRMPart As String
        Private _ModifierNum_GARPart As String
        Private _PackagePartNum As String

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
        Public Property DetailStatusCode As String 'added 6/25/2014 since Modifiers always seem to stay on rated image after flagging for delete
            Get
                Return _DetailStatusCode
            End Get
            Set(value As String)
                _DetailStatusCode = value
            End Set
        End Property
        Public Property CheckboxSelected As Boolean
            Get
                Return _CheckboxSelected
            End Get
            Set(value As Boolean)
                _CheckboxSelected = value
            End Set
        End Property
        Public Property ModifierAmount As String
            Get
                Return _ModifierAmount
            End Get
            Set(value As String)
                _ModifierAmount = value
            End Set
        End Property
        Public Property ModifierGroupId As String
            Get
                Return _ModifierGroupId
            End Get
            Set(value As String)
                _ModifierGroupId = value
                'added 10/16/2014
                _ModifierGroup = qqHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteModifier, QuickQuoteHelperClass.QuickQuotePropertyName.ModifierGroupId, _ModifierGroupId)
            End Set
        End Property
        Public Property ModifierLevelId As String
            Get
                Return _ModifierLevelId
            End Get
            Set(value As String)
                _ModifierLevelId = value
            End Set
        End Property
        Public Property ModifierOptionDate As String
            Get
                Return _ModifierOptionDate
            End Get
            Set(value As String)
                _ModifierOptionDate = value
                qqHelper.ConvertToShortDate(_ModifierOptionDate)
            End Set
        End Property
        Public Property ModifierOptionDescription As String
            Get
                Return _ModifierOptionDescription
            End Get
            Set(value As String)
                _ModifierOptionDescription = value
            End Set
        End Property
        Public Property ModifierOptionId As String
            Get
                Return _ModifierOptionId
            End Get
            Set(value As String)
                _ModifierOptionId = value
            End Set
        End Property
        Public Property ModifierTypeId As String
            Get
                Return _ModifierTypeId
            End Get
            Set(value As String)
                _ModifierTypeId = value
                'added 10/16/2014
                _ModifierType = qqHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteModifier, QuickQuoteHelperClass.QuickQuotePropertyName.ModifierTypeId, _ModifierTypeId)
            End Set
        End Property
        Public Property ParentModifierTypeId As String
            Get
                Return _ParentModifierTypeId
            End Get
            Set(value As String)
                _ParentModifierTypeId = value
                'added 10/16/2014
                _ParentModifierType = qqHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteModifier, QuickQuoteHelperClass.QuickQuotePropertyName.ParentModifierTypeId, _ParentModifierTypeId)
            End Set
        End Property
        'added 10/16/2014
        Public ReadOnly Property ModifierType As String
            Get
                Return _ModifierType
            End Get
        End Property
        Public ReadOnly Property ParentModifierType As String
            Get
                Return _ParentModifierType
            End Get
        End Property
        Public ReadOnly Property ModifierGroup As String
            Get
                Return _ModifierGroup
            End Get
        End Property
        Public ReadOnly Property IsDiscount As Boolean
            Get
                Return Me.ModifierGroupId = "1" 'could also check static data for Credits
            End Get
        End Property
        Public ReadOnly Property IsSurcharge As Boolean
            Get
                Return Me.ModifierGroupId = "2" 'could also check static data for Surcharges/Fees
            End Get
        End Property

        'added 10/19/2018
        Public Property ModifierNum As String
            Get
                Return _ModifierNum
            End Get
            Set(value As String)
                _ModifierNum = value
            End Set
        End Property
        Public Property ModifierNum_MasterPart As String
            Get
                Return _ModifierNum_MasterPart
            End Get
            Set(value As String)
                _ModifierNum_MasterPart = value
            End Set
        End Property
        Public Property ModifierNum_CGLPart As String
            Get
                Return _ModifierNum_CGLPart
            End Get
            Set(value As String)
                _ModifierNum_CGLPart = value
            End Set
        End Property
        Public Property ModifierNum_CPRPart As String
            Get
                Return _ModifierNum_CPRPart
            End Get
            Set(value As String)
                _ModifierNum_CPRPart = value
            End Set
        End Property
        Public Property ModifierNum_CIMPart As String
            Get
                Return _ModifierNum_CIMPart
            End Get
            Set(value As String)
                _ModifierNum_CIMPart = value
            End Set
        End Property
        Public Property ModifierNum_CRMPart As String
            Get
                Return _ModifierNum_CRMPart
            End Get
            Set(value As String)
                _ModifierNum_CRMPart = value
            End Set
        End Property
        Public Property ModifierNum_GARPart As String
            Get
                Return _ModifierNum_GARPart
            End Get
            Set(value As String)
                _ModifierNum_GARPart = value
            End Set
        End Property
        Public Property PackagePartNum As String
            Get
                Return _PackagePartNum
            End Get
            Set(value As String)
                _PackagePartNum = value
            End Set
        End Property


        Public Sub New()
            MyBase.New() 'added 8/4/2014
            SetDefaults()
        End Sub
        Private Sub SetDefaults()
            _PolicyId = ""
            _PolicyImageNum = ""
            _DetailStatusCode = "" 'added 6/25/2014 since Modifiers always seem to stay on rated image after flagging for delete
            _CheckboxSelected = False
            _ModifierAmount = ""
            _ModifierGroupId = ""
            _ModifierLevelId = ""
            _ModifierOptionDate = ""
            _ModifierOptionDescription = ""
            _ModifierOptionId = ""
            _ModifierTypeId = ""
            _ParentModifierTypeId = ""
            'added 10/16/2014
            _ModifierType = ""
            _ParentModifierType = ""
            _ModifierGroup = ""

            'added 10/19/2018
            _ModifierNum = ""
            _ModifierNum_MasterPart = ""
            _ModifierNum_CGLPart = ""
            _ModifierNum_CPRPart = ""
            _ModifierNum_CIMPart = ""
            _ModifierNum_CRMPart = ""
            _ModifierNum_GARPart = ""
            _PackagePartNum = ""
        End Sub

        'added 10/19/2018
        Public Function ModifierNumForPackagePartType(ByVal packagePartType As QuickQuoteXML.QuickQuotePackagePartType) As String
            Select Case packagePartType
                Case QuickQuoteXML.QuickQuotePackagePartType.Package
                    Return _ModifierNum_MasterPart
                Case QuickQuoteXML.QuickQuotePackagePartType.GeneralLiability
                    Return _ModifierNum_CGLPart
                Case QuickQuoteXML.QuickQuotePackagePartType.CommercialProperty
                    Return _ModifierNum_CPRPart
                Case QuickQuoteXML.QuickQuotePackagePartType.InlandMarine
                    Return _ModifierNum_CIMPart
                Case QuickQuoteXML.QuickQuotePackagePartType.Crime
                    Return _ModifierNum_CRMPart
                Case QuickQuoteXML.QuickQuotePackagePartType.Garage
                    Return _ModifierNum_GARPart
                Case Else
                    Return _ModifierNum
            End Select
        End Function
        Public Function HasValidModifierNum(Optional ByVal packagePartType As QuickQuoteXML.QuickQuotePackagePartType = QuickQuoteXML.QuickQuotePackagePartType.None) As Boolean
            Return qqHelper.IsValidQuickQuoteIdOrNum(ModifierNumForPackagePartType(packagePartType))
        End Function
        Public Sub SetModifierNumForPackagePartType(ByVal modNum As String, ByVal packagePartType As QuickQuoteXML.QuickQuotePackagePartType)
            Select Case packagePartType
                Case QuickQuoteXML.QuickQuotePackagePartType.Package
                    _ModifierNum_MasterPart = modNum
                Case QuickQuoteXML.QuickQuotePackagePartType.GeneralLiability
                    _ModifierNum_CGLPart = modNum
                Case QuickQuoteXML.QuickQuotePackagePartType.CommercialProperty
                    _ModifierNum_CPRPart = modNum
                Case QuickQuoteXML.QuickQuotePackagePartType.InlandMarine
                    _ModifierNum_CIMPart = modNum
                Case QuickQuoteXML.QuickQuotePackagePartType.Crime
                    _ModifierNum_CRMPart = modNum
                Case QuickQuoteXML.QuickQuotePackagePartType.Garage
                    _ModifierNum_GARPart = modNum
                Case Else
                    _ModifierNum = modNum
            End Select
        End Sub

        Public Overrides Function ToString() As String 'added 6/30/2015
            Dim str As String = ""
            If Me IsNot Nothing Then
                If Me.ModifierTypeId <> "" Then
                    Dim t As String = ""
                    t = "ModifierTypeId: " & Me.ModifierTypeId
                    If Me.ModifierType <> "" Then
                        t &= " (" & Me.ModifierType & ")"
                    End If
                    str = qqHelper.appendText(str, t, vbCrLf)
                End If
                If Me.ParentModifierTypeId <> "" Then
                    Dim t As String = ""
                    t = "ParentModifierTypeId: " & Me.ParentModifierTypeId
                    If Me.ParentModifierType <> "" Then
                        t &= " (" & Me.ParentModifierType & ")"
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
                    If _DetailStatusCode IsNot Nothing Then 'added 6/25/2014 since Modifiers always seem to stay on rated image after flagging for delete
                        _DetailStatusCode = Nothing
                    End If
                    If _CheckboxSelected <> Nothing Then
                        _CheckboxSelected = Nothing
                    End If
                    If _ModifierAmount IsNot Nothing Then
                        _ModifierAmount = Nothing
                    End If
                    If _ModifierGroupId IsNot Nothing Then
                        _ModifierGroupId = Nothing
                    End If
                    If _ModifierLevelId IsNot Nothing Then
                        _ModifierLevelId = Nothing
                    End If
                    If _ModifierOptionDate IsNot Nothing Then
                        _ModifierOptionDate = Nothing
                    End If
                    If _ModifierOptionDescription IsNot Nothing Then
                        _ModifierOptionDescription = Nothing
                    End If
                    If _ModifierOptionId IsNot Nothing Then
                        _ModifierOptionId = Nothing
                    End If
                    If _ModifierTypeId IsNot Nothing Then
                        _ModifierTypeId = Nothing
                    End If
                    If _ParentModifierTypeId IsNot Nothing Then
                        _ParentModifierTypeId = Nothing
                    End If
                    'added 10/16/2014
                    If _ModifierType IsNot Nothing Then
                        _ModifierType = Nothing
                    End If
                    If _ParentModifierType IsNot Nothing Then
                        _ParentModifierType = Nothing
                    End If
                    If _ModifierGroup IsNot Nothing Then
                        _ModifierGroup = Nothing
                    End If

                    'added 10/19/2018
                    qqHelper.DisposeString(_ModifierNum)
                    qqHelper.DisposeString(_ModifierNum_MasterPart)
                    qqHelper.DisposeString(_ModifierNum_CGLPart)
                    qqHelper.DisposeString(_ModifierNum_CPRPart)
                    qqHelper.DisposeString(_ModifierNum_CIMPart)
                    qqHelper.DisposeString(_ModifierNum_CRMPart)
                    qqHelper.DisposeString(_ModifierNum_GARPart)
                    qqHelper.DisposeString(_PackagePartNum)

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
