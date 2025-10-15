Imports Microsoft.VisualBasic
'added 12/20/2013
Imports QuickQuote.CommonMethods

Namespace QuickQuote.CommonObjects 'added namespace 12/20/2013
    ''' <summary>
    ''' object used to store accident/violation information
    ''' </summary>
    ''' <remarks>related to 3rd party information</remarks>
    <Serializable()> _
    Public Class QuickQuoteAccidentViolation 'added 9/17/2013
        Inherits QuickQuoteBaseGenericObject(Of Object) 'added 8/4/2014
        Implements IDisposable

        Dim qqHelper As New QuickQuoteHelperClass

        Private _PolicyId As String
        Private _PolicyImageNum As String
        'added 1/14/2014
        Private _AccidentSurchargeTypeId As String 'list; not used in UI
        Private _AccidentsViolationsTypeId As String 'list
        Private _AmountProperty As String
        Private _AvDate As String 'date
        Private _BiDeath As Boolean
        Private _Comments As String
        Private _ConvictionDate As String 'date
        Private _Description As String
        Private _ExposureLevel As String 'not used in UI
        Private _LicenseSuspended As Boolean
        Private _MajorSurchargeTypeId As String 'list; not used in UI
        Private _ManualOverride As Boolean 'appears to be grayed out in UI
        Private _MinorSurchargeTypeId As String 'list; not used in UI
        Private _Mph As String
        Private _PaidDate As String 'date
        Private _Place As String
        Private _Points As String
        Private _PostDate As String 'date
        Private _Printed As Boolean 'not used in UI
        Private _SpeedLimit As String
        Private _Surcharge As Boolean
        Private _ViolationConvictionTypeId As String 'list
        Private _ViolationSourceId As String 'list; doesn't appear to be active in UI; defaults to Manual

        Private _ViolationNum As String 'added 4/23/2014 for reconciliation

        'added 7/7/2014 to make sure we're capturing everything that could come back in 3rd party reports
        Private _ExternalCode As String
        Private _ExternalId As String
        Private _FirstAddedDate As String 'Date
        Private _GuaranteedRatePeriodEffectiveDate As String 'Date
        'Private _LocationNum As String 'IdValue
        'Private _PackagePartNum As String
        'Private _RvWatercraftNum As String 'IdValue
        Private _SVCUnderwritingIndicatorCode As String
        'Private _UnitNum As String 'IdValue

        'added 7/15/2014 for surcharge logic
        Private _AccidentsViolationsCategoryId As String
        Private _AccidentsViolationsCategory As String

        Private _DetailStatusCode As String 'added 5/15/2019

        'added 7/24/2019
        Private _AddedDate As String
        Private _AddedImageNum As String

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
        'added 1/14/2014
        Public Property AccidentSurchargeTypeId As String 'list; not used in UI
            Get
                Return _AccidentSurchargeTypeId
            End Get
            Set(value As String)
                _AccidentSurchargeTypeId = value
            End Set
        End Property
        Public Property AccidentsViolationsTypeId As String 'list
            Get
                Return _AccidentsViolationsTypeId
            End Get
            Set(value As String)
                'added 7/15/2014
                If _AccidentsViolationsCategoryId <> "" OrElse _AccidentsViolationsCategory <> "" Then
                    Dim oldAccViolTypeId As String = _AccidentsViolationsTypeId
                    If oldAccViolTypeId <> value Then
                        _AccidentsViolationsCategoryId = ""
                        _AccidentsViolationsCategory = ""
                    End If
                End If

                _AccidentsViolationsTypeId = value
            End Set
        End Property
        Public Property AmountProperty As String
            Get
                Return _AmountProperty
            End Get
            Set(value As String)
                _AmountProperty = value
            End Set
        End Property
        Public Property AvDate As String 'date
            Get
                Return _AvDate
            End Get
            Set(value As String)
                _AvDate = value
                qqHelper.ConvertToShortDate(_AvDate)
            End Set
        End Property
        Public Property BiDeath As Boolean
            Get
                Return _BiDeath
            End Get
            Set(value As Boolean)
                _BiDeath = value
            End Set
        End Property
        Public Property Comments As String
            Get
                Return _Comments
            End Get
            Set(value As String)
                _Comments = value
            End Set
        End Property
        Public Property ConvictionDate As String 'date
            Get
                Return _ConvictionDate
            End Get
            Set(value As String)
                _ConvictionDate = value
                qqHelper.ConvertToShortDate(_ConvictionDate)
            End Set
        End Property
        Public Property Description As String
            Get
                Return _Description
            End Get
            Set(value As String)
                _Description = value
            End Set
        End Property
        Public Property ExposureLevel As String 'not used in UI
            Get
                Return _ExposureLevel
            End Get
            Set(value As String)
                _ExposureLevel = value
            End Set
        End Property
        Public Property LicenseSuspended As Boolean
            Get
                Return _LicenseSuspended
            End Get
            Set(value As Boolean)
                _LicenseSuspended = value
            End Set
        End Property
        Public Property MajorSurchargeTypeId As String 'list; not used in UI
            Get
                Return _MajorSurchargeTypeId
            End Get
            Set(value As String)
                _MajorSurchargeTypeId = value
            End Set
        End Property
        Public Property ManualOverride As Boolean 'appears to be grayed out in UI
            Get
                Return _ManualOverride
            End Get
            Set(value As Boolean)
                _ManualOverride = value
            End Set
        End Property
        Public Property MinorSurchargeTypeId As String 'list; not used in UI
            Get
                Return _MinorSurchargeTypeId
            End Get
            Set(value As String)
                _MinorSurchargeTypeId = value
            End Set
        End Property
        Public Property Mph As String
            Get
                Return _Mph
            End Get
            Set(value As String)
                _Mph = value
            End Set
        End Property
        Public Property PaidDate As String 'date
            Get
                Return _PaidDate
            End Get
            Set(value As String)
                _PaidDate = value
                qqHelper.ConvertToShortDate(_PaidDate) 'updated 7/9/2014 from _ConvictionDate
            End Set
        End Property
        Public Property Place As String
            Get
                Return _Place
            End Get
            Set(value As String)
                _Place = value
            End Set
        End Property
        Public Property Points As String
            Get
                Return _Points
            End Get
            Set(value As String)
                _Points = value
            End Set
        End Property
        Public Property PostDate As String 'date
            Get
                Return _PostDate
            End Get
            Set(value As String)
                _PostDate = value
                qqHelper.ConvertToShortDate(_PostDate)
            End Set
        End Property
        Public Property Printed As Boolean 'not used in UI
            Get
                Return _Printed
            End Get
            Set(value As Boolean)
                _Printed = value
            End Set
        End Property
        Public Property SpeedLimit As String
            Get
                Return _SpeedLimit
            End Get
            Set(value As String)
                _SpeedLimit = value
            End Set
        End Property
        Public Property Surcharge As Boolean
            Get
                Return _Surcharge
            End Get
            Set(value As Boolean)
                _Surcharge = value
            End Set
        End Property
        Public Property ViolationConvictionTypeId As String 'list
            Get
                Return _ViolationConvictionTypeId
            End Get
            Set(value As String)
                _ViolationConvictionTypeId = value
            End Set
        End Property
        Public Property ViolationSourceId As String 'list; doesn't appear to be active in UI; defaults to Manual
            Get
                Return _ViolationSourceId
            End Get
            Set(value As String)
                _ViolationSourceId = value
            End Set
        End Property

        Public Property ViolationNum As String 'added 4/23/2014 for reconciliation
            Get
                Return _ViolationNum
            End Get
            Set(value As String)
                _ViolationNum = value
            End Set
        End Property

        'added 7/7/2014
        Public Property ExternalCode As String
            Get
                Return _ExternalCode
            End Get
            Set(value As String)
                _ExternalCode = value
            End Set
        End Property
        Public Property ExternalId As String
            Get
                Return _ExternalId
            End Get
            Set(value As String)
                _ExternalId = value
            End Set
        End Property
        Public Property FirstAddedDate As String 'Date
            Get
                Return _FirstAddedDate
            End Get
            Set(value As String)
                _FirstAddedDate = value
                qqHelper.ConvertToShortDate(_FirstAddedDate)
            End Set
        End Property
        Public Property GuaranteedRatePeriodEffectiveDate As String 'Date
            Get
                Return _GuaranteedRatePeriodEffectiveDate
            End Get
            Set(value As String)
                _GuaranteedRatePeriodEffectiveDate = value
                qqHelper.ConvertToShortDate(_GuaranteedRatePeriodEffectiveDate)
            End Set
        End Property
        'Public Property LocationNum As String 'IdValue
        'Public Property PackagePartNum As String
        'Public Property RvWatercraftNum As String 'IdValue
        Public Property SVCUnderwritingIndicatorCode As String
            Get
                Return _SVCUnderwritingIndicatorCode
            End Get
            Set(value As String)
                _SVCUnderwritingIndicatorCode = value
            End Set
        End Property
        'Public Property UnitNum As String 'IdValue

        'added 7/15/2014 for surcharge logic
        Public ReadOnly Property AccidentsViolationsCategoryId As String
            Get
                If _AccidentsViolationsCategoryId = "" AndAlso _AccidentsViolationsTypeId <> "" Then
                    _AccidentsViolationsCategoryId = qqHelper.GetRelatedStaticDataValueForOptionValue(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteAccidentViolation, QuickQuoteHelperClass.QuickQuotePropertyName.AccidentsViolationsTypeId, _AccidentsViolationsTypeId, QuickQuoteHelperClass.QuickQuotePropertyName.AccidentsViolationsCategoryId)
                End If
                Return _AccidentsViolationsCategoryId
            End Get
        End Property
        Public ReadOnly Property AccidentsViolationsCategory As String
            Get
                If _AccidentsViolationsCategory = "" AndAlso _AccidentsViolationsTypeId <> "" Then 'AccidentsViolationsCategoryId would be empty string if _AccidentsViolationsTypeId is empty string
                    'this uses the property for AccidentsViolationsCategoryId so it will pull the related value if it's not already set
                    _AccidentsViolationsCategory = qqHelper.GetStaticDataTextForValue(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteAccidentViolation, QuickQuoteHelperClass.QuickQuotePropertyName.AccidentsViolationsCategoryId, AccidentsViolationsCategoryId)
                End If
                Return _AccidentsViolationsCategory
            End Get
        End Property

        Public Property DetailStatusCode As String 'added 5/15/2019
            Get
                Return _DetailStatusCode
            End Get
            Set(value As String)
                _DetailStatusCode = value
            End Set
        End Property

        'added 7/24/2019
        Public Property AddedDate As String
            Get
                Return _AddedDate
            End Get
            Set(value As String)
                _AddedDate = value
            End Set
        End Property
        Public Property AddedImageNum As String
            Get
                Return _AddedImageNum
            End Get
            Set(value As String)
                _AddedImageNum = value
            End Set
        End Property

        Public Sub New()
            MyBase.New() 'added 8/4/2014
            SetDefaults()
        End Sub
        Private Sub SetDefaults()
            _PolicyId = ""
            _PolicyImageNum = ""
            'added 1/14/2014
            _AccidentSurchargeTypeId = ""
            _AccidentsViolationsTypeId = ""
            _AmountProperty = ""
            _AvDate = ""
            _BiDeath = False
            _Comments = ""
            _ConvictionDate = ""
            _Description = ""
            _ExposureLevel = ""
            _LicenseSuspended = False
            _MajorSurchargeTypeId = ""
            _ManualOverride = False
            _MinorSurchargeTypeId = ""
            _Mph = ""
            _PaidDate = ""
            _Place = ""
            _Points = ""
            _PostDate = ""
            _Printed = False
            _SpeedLimit = ""
            _Surcharge = False
            _ViolationConvictionTypeId = ""
            _ViolationSourceId = ""

            _ViolationNum = "" 'added 4/23/2014 for reconciliation

            'added 7/7/2014
            _ExternalCode = ""
            _ExternalId = ""
            _FirstAddedDate = "" 'Date
            _GuaranteedRatePeriodEffectiveDate = "" 'Date
            '_LocationNum = "" 'IdValue
            '_PackagePartNum = ""
            '_RvWatercraftNum = "" 'IdValue
            _SVCUnderwritingIndicatorCode = ""
            '_UnitNum = "" 'IdValue

            'added 7/15/2014 for surcharge logic
            _AccidentsViolationsCategoryId = ""
            _AccidentsViolationsCategory = ""

            _DetailStatusCode = "" 'added 5/15/2019

            'added 7/24/2019
            _AddedDate = ""
            _AddedImageNum = ""
        End Sub
        Public Function HasValidViolationNum() As Boolean 'added 4/23/2014 for reconciliation purposes
            'If _ViolationNum <> "" AndAlso IsNumeric(_ViolationNum) = True AndAlso CInt(_ViolationNum) > 0 Then
            '    Return True
            'Else
            '    Return False
            'End If
            'updated 4/27/2014 to use common method
            Return qqHelper.IsValidQuickQuoteIdOrNum(_ViolationNum)
        End Function
        Public Overrides Function ToString() As String 'added 6/30/2015
            Dim str As String = ""
            If Me IsNot Nothing Then
                If Me.AccidentsViolationsTypeId <> "" Then
                    Dim av As String = ""
                    av = "AccidentsViolationsTypeId: " & Me.AccidentsViolationsTypeId
                    Dim avType As String = qqHelper.GetStaticDataTextForValue(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteAccidentViolation, QuickQuoteHelperClass.QuickQuotePropertyName.AccidentsViolationsTypeId, Me.AccidentsViolationsTypeId)
                    If avType <> "" Then
                        av &= " (" & avType & ")"
                    End If
                    str = qqHelper.appendText(str, av, vbCrLf)
                End If
                If Me.AvDate <> "" Then
                    str = qqHelper.appendText(str, "AvDate: " & Me.AvDate, vbCrLf)
                End If
                If Me.AccidentsViolationsCategoryId <> "" Then
                    Dim c As String = ""
                    c = "AccidentsViolationsCategoryId: " & Me.AccidentsViolationsCategoryId
                    Dim catType As String = Me.AccidentsViolationsCategory 'qqHelper.GetStaticDataTextForValue(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteAccidentViolation, QuickQuoteHelperClass.QuickQuotePropertyName.AccidentsViolationsCategoryId, Me.AccidentsViolationsCategoryId)
                    If catType <> "" Then
                        c &= " (" & catType & ")"
                    End If
                    str = qqHelper.appendText(str, c, vbCrLf)
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
                    'added 1/14/2014
                    If _AccidentSurchargeTypeId IsNot Nothing Then
                        _AccidentSurchargeTypeId = Nothing
                    End If
                    If _AccidentsViolationsTypeId IsNot Nothing Then
                        _AccidentsViolationsTypeId = Nothing
                    End If
                    If _AmountProperty IsNot Nothing Then
                        _AmountProperty = Nothing
                    End If
                    If _AvDate IsNot Nothing Then
                        _AvDate = Nothing
                    End If
                    If _BiDeath <> Nothing Then
                        _BiDeath = Nothing
                    End If
                    If _Comments IsNot Nothing Then
                        _Comments = Nothing
                    End If
                    If _ConvictionDate IsNot Nothing Then
                        _ConvictionDate = Nothing
                    End If
                    If _Description IsNot Nothing Then
                        _Description = Nothing
                    End If
                    If _ExposureLevel IsNot Nothing Then
                        _ExposureLevel = Nothing
                    End If
                    If _LicenseSuspended <> Nothing Then
                        _LicenseSuspended = Nothing
                    End If
                    If _MajorSurchargeTypeId IsNot Nothing Then
                        _MajorSurchargeTypeId = Nothing
                    End If
                    If _ManualOverride <> Nothing Then
                        _ManualOverride = Nothing
                    End If
                    If _MinorSurchargeTypeId IsNot Nothing Then
                        _MinorSurchargeTypeId = Nothing
                    End If
                    If _Mph IsNot Nothing Then
                        _Mph = Nothing
                    End If
                    If _PaidDate IsNot Nothing Then
                        _PaidDate = Nothing
                    End If
                    If _Place IsNot Nothing Then
                        _Place = Nothing
                    End If
                    If _Points IsNot Nothing Then
                        _Points = Nothing
                    End If
                    If _PostDate IsNot Nothing Then
                        _PostDate = Nothing
                    End If
                    If _Printed <> Nothing Then
                        _Printed = Nothing
                    End If
                    If _SpeedLimit IsNot Nothing Then
                        _SpeedLimit = Nothing
                    End If
                    If _Surcharge <> Nothing Then
                        _Surcharge = Nothing
                    End If
                    If _ViolationConvictionTypeId IsNot Nothing Then
                        _ViolationConvictionTypeId = Nothing
                    End If
                    If _ViolationSourceId IsNot Nothing Then
                        _ViolationSourceId = Nothing
                    End If

                    If _ViolationNum IsNot Nothing Then 'added 4/23/2014 for reconciliation
                        _ViolationNum = Nothing
                    End If

                    'added 7/7/2014
                    If _ExternalCode IsNot Nothing Then
                        _ExternalCode = Nothing
                    End If
                    If _ExternalId IsNot Nothing Then
                        _ExternalId = Nothing
                    End If
                    If _FirstAddedDate IsNot Nothing Then
                        _FirstAddedDate = Nothing
                    End If
                    If _GuaranteedRatePeriodEffectiveDate IsNot Nothing Then
                        _GuaranteedRatePeriodEffectiveDate = Nothing
                    End If
                    '_LocationNum = "" 'IdValue
                    '_PackagePartNum = ""
                    '_RvWatercraftNum = "" 'IdValue
                    If _SVCUnderwritingIndicatorCode IsNot Nothing Then
                        _SVCUnderwritingIndicatorCode = Nothing
                    End If
                    '_UnitNum = "" 'IdValue

                    'added 7/15/2014 for surcharge logic
                    If _AccidentsViolationsCategoryId IsNot Nothing Then
                        _AccidentsViolationsCategoryId = Nothing
                    End If
                    If _AccidentsViolationsCategory IsNot Nothing Then
                        _AccidentsViolationsCategory = Nothing
                    End If

                    qqHelper.DisposeString(_DetailStatusCode) 'added 5/15/2019

                    'added 7/24/2019
                    qqHelper.DisposeString(_AddedDate)
                    qqHelper.DisposeString(_AddedImageNum)

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