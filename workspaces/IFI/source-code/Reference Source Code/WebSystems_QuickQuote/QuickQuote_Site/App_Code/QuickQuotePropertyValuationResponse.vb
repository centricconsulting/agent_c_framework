Imports Microsoft.VisualBasic
Imports helper = QuickQuote.CommonMethods.QuickQuoteHelperClass 'added 8/6/2014
Imports QuickQuote.CommonMethods 'added 8/7/2014

Namespace QuickQuote.CommonObjects
    ''' <summary>
    ''' object used to property valuation response information
    ''' </summary>
    ''' <remarks>currently using e2Value</remarks>
    <Serializable()> _
    Public Class QuickQuotePropertyValuationResponse
        Inherits QuickQuoteBaseGenericObject(Of Object)
        Implements IDisposable

        Dim qqHelper As New QuickQuoteHelperClass 'added 8/7/2014
        Dim pvHelper As New QuickQuotePropertyValuationHelperClass 'added 7/29/2015

        'see e2ValueNotes.txt and Portico Integration Instructions 06-10-2013.pdf

        Private _db_propertyValuationId As String 'added 8/7/2014
        Private _db_propertyValuationResponseId As String 'added 8/7/2014
        Private _AuthId As String 'authid in e2Value auth response; ad in e2Value redirect URL; return_ad in e2Value return URL
        Private _IFM_UniqueValuationId As String 'id in e2Value redirect URL; return_id in e2Value return URL
        Private _VendorValuationId As String 'propid in e2Value redirect URL; rt_propid in e2Value return URL
        Private _YearBuilt As String 'yrblt in e2Value redirect URL; max 4; must be valid year; rt_yrblt in return URL... if rt_yrblt = 1 in redirect (default). In QCE, CMM, FNR, and HSD, returns the year built of each structure in the property separated by a “|” character in the return URL specified by the user. Value will be returned as rt_yrblt=<yrblt1|yrblt2|yrblt3|etc…>
        Private _ConstructionType As String 'consttype in e2Value redirect URL; max 100; see valid EVS values; rt_consttype in return URL... if rt_consttype = 1 in redirect (default). In QCE, CMM, FNR, and HSD, returns the construction type of each structure in the property separated by a “|” character in the return URL specified by the user. Value will be returned as rt_consttype=<consttype1|consttype2|consttype3|etc…>
        Private _SquareFeet As String 'sqft in e2Value redirect URL; max 10; must be valid numeric value; rt_sqft in return URL... if rt_sqft = 1 in redirect (default). In QCE, CMM, FNR, and HSD, returns the square feet of each structure in the property separated by a “|” character in the return URL specified by the user. Value will be returned as rt_sqft=<sqft1|sqft2|sqft3|etc…>
        Private _RoofType As String 'rooftype in e2Value redirect URL; max 100; see valid EVS values; rt_rooftype in return URL... if rt_rooftype = 1 in redirect URL (default).
        'Private _RoofCovering As String 'only valid for QCE and CMM; roofcov in e2Value redirect URL; max 100; see valid EVS values; rt_roofcov in return URL... if rt_roofcov = 1 in redirect URL (default). Returns the roof covering of each structure in the property separated by a “|” character in the return URL specified by the user. Value will be returned as rt_roofcov=<roofcov1|roofcov2|roofcov3|etc…>
        'Private _StructureType As String 'only valid for QCE and CMM; structtype in e2Value redirect URL; max 100; see valid EVS values; rt_structtype in return URL... if rt_structtype = 1 in redirect URL (default). Returns the structure type of each structure in the property separated by a “|” character in the return URL specified by the user. Value will be returned as rt_structtype=<structtype1|structtype2|structtype3|etc…>
        'Private _NumberOfFloors As String 'only valid for QCE and CMM; numfloors in e2Value redirect URL; max 4; must be valid integer; rt_numfloors in return URL... if rt_numfloors = 1 in redirect URL (default). Returns the number of floors of each structure in the property separated by a “|” character in the return URL specified by the user. Value will be returned as rt_numfloors=<numfloors1|numfloors2|numfloors3|etc…>
        Private _ArchitecturalStyle As String 'archstyle in e2Value redirect URL; max 100; see valid EVS values; rt_archstyle in return URL... if rt_archstyle = 1 in redirect URL (default = 0)
        Private _ConstructionQuality As String 'constquality in e2Value redirect URL; max 100; see valid EVS values; rt_constquality in return URL... if rt_constquality = 1 in redirect URL (default = 0)
        Private _PhysicalShape As String 'shape in e2Value redirect URL; see valid EVS values; rt_shape in return URL... if rt_shape = 1 in redirect URL (default = 0)
        Private _PrimaryExterior As String 'exterior1 in e2Value redirect URL; see valid EVS values; rt_exterior1 in return URL... if rt_exterior1 = 1 in redirect URL (default = 0)
        Private _ReplacementCostValue As String 'return_rpc in e2Value return URL; Returns the calculated replacement cost value.
        Private _ReplacementCostType As String 'rt_rpctype in e2Value return URL... if rt_rpctype = 1 in redirect URL (default 0). Returns the Replacement Cost Type in the return URL specified by the user. Value will be returned as rt_rpctype=<replacement cost type> where < replacement cost type> is the actual Replacement Cost Type value of this property. Possible return values for the Replacement Cost Type are “full”, “func” (for “functional”), or an empty value if none exists for this property.
        Private _AdditionalAreas As String 'rt_areas in e2Value return URL... if rt_areas = 1 in redirect URL (default 0). Returns the additional areas in the return URL specified by user. Value will be returned as rt_areas=<areas>. For <areas>, Columns are separated by | and rows are separated by a line break. ... For example: garage, attached|1997|500 ; 1/2 Story|1997|600... Note: Column 1 = area name; Column 2 = year built; Column 3 = sqft
        Private _VendorParams As String 'added 8/8/2014
        Private _db_loadedBackIntoQuote As Boolean 'added 8/8/2014
        'Private _Timestamp As String 'added 8/7/2014; removed 8/7/2014
        Private _db_inserted As String 'added 8/7/2014
        Private _db_updated As String 'added 8/7/2014
        Private _StructuresReplacementCostValue As String 'added 7/29/2015 for Farm; return_structuresrpc in e2Value return URL; Returns the calculated replacement cost value of other structures.
        Private _StructuresReplacementCostValues As List(Of String) 'added 7/29/2015 for Farm; list representation of split values from _StructuresReplacementCostValue
        Private _StructuresReplacementCostValueTotal As String 'added 7/29/2015 for Farm; return_structuresrpc_total in e2Value return URL
        Private _UniqueItemsTotalValue As String 'added 7/29/2015 for Farm; return_unique in e2Value return URL; Returns the total value of any unique items entered.
        Private _ActualCashValue As String 'added 7/29/2015 for Farm; return_ACV in e2Value return URL; Returns the ACV value.
        Private _ActualCashValues As List(Of String) 'added 7/29/2015 for Farm; list representation of split values from _ActualCashValue

        Public Property db_propertyValuationId As String 'added 8/7/2014
            Get
                Return _db_propertyValuationId
            End Get
            Set(value As String)
                _db_propertyValuationId = value
            End Set
        End Property
        Public Property db_propertyValuationResponseId As String 'added 8/7/2014
            Get
                Return _db_propertyValuationResponseId
            End Get
            Set(value As String)
                _db_propertyValuationResponseId = value
            End Set
        End Property
        Public Property AuthId As String
            Get
                Return _AuthId
            End Get
            Set(value As String)
                _AuthId = value
            End Set
        End Property
        Public Property IFM_UniqueValuationId As String
            Get
                Return _IFM_UniqueValuationId
            End Get
            Set(value As String)
                _IFM_UniqueValuationId = value
            End Set
        End Property
        Public Property VendorValuationId As String
            Get
                Return _VendorValuationId
            End Get
            Set(value As String)
                _VendorValuationId = value
            End Set
        End Property
        Public Property YearBuilt As String
            Get
                Return _YearBuilt
            End Get
            Set(value As String)
                _YearBuilt = value
            End Set
        End Property
        Public Property ConstructionType As String
            Get
                Return _ConstructionType
            End Get
            Set(value As String)
                _ConstructionType = value
            End Set
        End Property
        Public Property SquareFeet As String
            Get
                Return _SquareFeet
            End Get
            Set(value As String)
                _SquareFeet = value
            End Set
        End Property
        Public Property RoofType As String
            Get
                Return _RoofType
            End Get
            Set(value As String)
                _RoofType = value
            End Set
        End Property
        Public Property ArchitecturalStyle As String
            Get
                Return _ArchitecturalStyle
            End Get
            Set(value As String)
                _ArchitecturalStyle = value
            End Set
        End Property
        Public Property ConstructionQuality As String
            Get
                Return _ConstructionQuality
            End Get
            Set(value As String)
                _ConstructionQuality = value
            End Set
        End Property
        Public Property PhysicalShape As String
            Get
                Return _PhysicalShape
            End Get
            Set(value As String)
                _PhysicalShape = value
            End Set
        End Property
        Public Property PrimaryExterior As String
            Get
                Return _PrimaryExterior
            End Get
            Set(value As String)
                _PrimaryExterior = value
            End Set
        End Property
        Public Property ReplacementCostValue As String
            Get
                'Return _ReplacementCostValue
                'updated 8/26/2014
                Return qqHelper.QuotedPremiumFormat(_ReplacementCostValue)
            End Get
            Set(value As String)
                _ReplacementCostValue = value
                qqHelper.ConvertToQuotedPremiumFormat(_ReplacementCostValue)
            End Set
        End Property
        Public Property ReplacementCostType As String
            Get
                Return _ReplacementCostType
            End Get
            Set(value As String)
                _ReplacementCostType = value
            End Set
        End Property
        Public Property AdditionalAreas As String
            Get
                Return _AdditionalAreas
            End Get
            Set(value As String)
                _AdditionalAreas = value
            End Set
        End Property
        Public Property VendorParams As String 'added 8/8/2014
            Get
                Return _VendorParams
            End Get
            Set(value As String)
                _VendorParams = value
            End Set
        End Property
        Public Property db_loadedBackIntoQuote As Boolean 'added 8/8/2014
            Get
                Return _db_loadedBackIntoQuote
            End Get
            Set(value As Boolean)
                _db_loadedBackIntoQuote = value
            End Set
        End Property
        'Public Property Timestamp As String 'added 8/7/2014; removed 8/7/2014
        '    Get
        '        Return _Timestamp
        '    End Get
        '    Set(value As String)
        '        _Timestamp = value
        '    End Set
        'End Property
        Public Property db_inserted As String 'added 8/7/2014
            Get
                Return _db_inserted
            End Get
            Set(value As String)
                _db_inserted = value
            End Set
        End Property
        Public Property db_updated As String 'added 8/7/2014
            Get
                Return _db_updated
            End Get
            Set(value As String)
                _db_updated = value
            End Set
        End Property
        Public Property StructuresReplacementCostValue As String 'added 7/29/2015 for Farm
            Get
                Return _StructuresReplacementCostValue
            End Get
            Set(value As String)
                _StructuresReplacementCostValue = value
                _StructuresReplacementCostValues = pvHelper.ListFromDelimitedParam(_StructuresReplacementCostValue, "|", True)
            End Set
        End Property
        Public ReadOnly Property StructuresReplacementCostValues As List(Of String) 'added 7/29/2015 for Farm
            Get
                Return _StructuresReplacementCostValues
            End Get
        End Property
        Public Property StructuresReplacementCostValueTotal As String 'added 7/29/2015 for Farm
            Get
                'Return _StructuresReplacementCostValueTotal
                Return qqHelper.QuotedPremiumFormat(_StructuresReplacementCostValueTotal)
            End Get
            Set(value As String)
                _StructuresReplacementCostValueTotal = value
                qqHelper.ConvertToQuotedPremiumFormat(_StructuresReplacementCostValueTotal)
            End Set
        End Property
        Public Property UniqueItemsTotalValue As String 'added 7/29/2015 for Farm
            Get
                Return qqHelper.QuotedPremiumFormat(_UniqueItemsTotalValue)
            End Get
            Set(value As String)
                _UniqueItemsTotalValue = value
                qqHelper.ConvertToQuotedPremiumFormat(_UniqueItemsTotalValue)
            End Set
        End Property
        Public Property ActualCashValue As String 'added 7/29/2015 for Farm
            Get
                Return _ActualCashValue
            End Get
            Set(value As String)
                _ActualCashValue = value
                _ActualCashValues = pvHelper.ListFromDelimitedParam(_StructuresReplacementCostValue, "|", False)
            End Set
        End Property
        Public ReadOnly Property ActualCashValues As List(Of String) 'added 7/29/2015 for Farm
            Get
                Return _ActualCashValues
            End Get
        End Property

        Public Sub New()
            MyBase.New()
            SetDefaults()
        End Sub
        Private Sub SetDefaults()
            _db_propertyValuationId = "" 'added 8/7/2014
            _db_propertyValuationResponseId = "" 'added 8/7/2014
            _AuthId = ""
            _IFM_UniqueValuationId = ""
            _VendorValuationId = ""
            _YearBuilt = ""
            _ConstructionType = ""
            _SquareFeet = ""
            _RoofType = ""
            _ArchitecturalStyle = ""
            _ConstructionQuality = ""
            _PhysicalShape = ""
            _PrimaryExterior = ""
            _ReplacementCostValue = ""
            _ReplacementCostType = ""
            _AdditionalAreas = ""
            _VendorParams = "" 'added 8/8/2014
            _db_loadedBackIntoQuote = False 'added 8/8/2014
            '_Timestamp = "" 'added 8/7/2014; removed 8/7/2014
            _db_inserted = "" 'added 8/7/2014
            _db_updated = "" 'added 8/7/2014
            _StructuresReplacementCostValue = "" 'added 7/29/2015 for Farm
            _StructuresReplacementCostValues = Nothing 'added 7/29/2015 for Farm
            _StructuresReplacementCostValueTotal = "" 'added 7/29/2015 for Farm
            _UniqueItemsTotalValue = "" 'added 7/29/2015 for Farm
            _ActualCashValue = "" 'added 7/29/2015 for Farm
            _ActualCashValues = Nothing 'added 7/29/2015 for Farm
        End Sub
        'Public Sub LoadFromNameValueCollection(ByVal nvc As NameValueCollection) 'System.Collections.Specialized.NameValueCollection (i.e. QueryString); 8/7/2014 - moved to QuickQuotePropertyValuationHelperClass
        '    'If nvc IsNot Nothing AndAlso nvc.Count > 0 Then 'works but will use nvc.Keys since that's what the code below uses
        '    If nvc IsNot Nothing AndAlso nvc.Keys IsNot Nothing AndAlso nvc.Keys.Count > 0 Then 'System.Collections.Specialized.NameObjectCollectionBase.KeysCollection
        '        'For Each kvp As KeyValuePair(Of String, String) In nvc 'didn't work... invalid cast
        '        '    Dim keyValueString As String = kvp.Key & " - " & kvp.Value
        '        'Next

        '        'For Each key As String In nvc.Keys 'this works
        '        '    For Each value As String In nvc.GetValues(key)
        '        '        Dim keyValueString As String = key & " - " & value
        '        '    Next
        '        'Next

        '        'Dim key As String = ""
        '        'Dim value As String = ""
        '        'Dim keyValueString As String = ""
        '        'Dim keyValueString2 As String = ""
        '        'For Each key In nvc.Keys
        '        '    For Each value In nvc.GetValues(key)
        '        '        keyValueString = key & " - " & value
        '        '    Next
        '        '    keyValueString2 = key & " - " & nvc(key)
        '        'Next
        '        '?key1=a&key2=b&key1=aa&key3=c&key4=d
        '        '4 total keys (nvc.Count or nvc.Keys.Count); key1 had 2 values
        '        'keyValueString = aa 'after last iteration
        '        'keyValueString2 = a,aa 'combines multiple values into 1 string

        '        '?key1=&key1=a&key2=b&key1=aa&key3=c&key4=d&key1=
        '        '4 total keys (nvc.Count or nvc.Keys.Count); key1 had 2 values
        '        'keyValueString =  'after last iteration
        '        'keyValueString2 = ,a,aa, 'combines multiple values into 1 string

        '        For Each key As String In nvc.Keys
        '            Dim vals As String() = nvc.GetValues(key)
        '            If vals IsNot Nothing AndAlso vals.Count > 0 Then
        '                Dim valCounter As Integer = 0
        '                For Each value As String In vals
        '                    value = helper.UrlDecodedValue(value)
        '                    valCounter += 1
        '                    Select Case UCase(key)
        '                        Case UCase("return_ad")
        '                            'AuthId
        '                            If valCounter < 2 Then
        '                                '1st instance
        '                                AuthId = value
        '                            Else
        '                                'at least 2nd instance
        '                                If AuthId = "" OrElse String.IsNullOrWhiteSpace(value) = False Then
        '                                    AuthId = value
        '                                End If
        '                            End If
        '                        Case UCase("return_id")
        '                            'IFM_UniqueValuationId
        '                            If valCounter < 2 Then
        '                                '1st instance
        '                                IFM_UniqueValuationId = value
        '                            Else
        '                                'at least 2nd instance
        '                                If IFM_UniqueValuationId = "" OrElse String.IsNullOrWhiteSpace(value) = False Then
        '                                    IFM_UniqueValuationId = value
        '                                End If
        '                            End If
        '                        Case UCase("rt_propid")
        '                            'VendorValuationId
        '                            If valCounter < 2 Then
        '                                '1st instance
        '                                VendorValuationId = value
        '                            Else
        '                                'at least 2nd instance
        '                                If VendorValuationId = "" OrElse String.IsNullOrWhiteSpace(value) = False Then
        '                                    VendorValuationId = value
        '                                End If
        '                            End If
        '                        Case UCase("rt_yrblt")
        '                            'YearBuilt
        '                            If valCounter < 2 Then
        '                                '1st instance
        '                                YearBuilt = value
        '                            Else
        '                                'at least 2nd instance
        '                                If YearBuilt = "" OrElse String.IsNullOrWhiteSpace(value) = False Then
        '                                    YearBuilt = value
        '                                End If
        '                            End If
        '                        Case UCase("rt_consttype")
        '                            'ConstructionType
        '                            If valCounter < 2 Then
        '                                '1st instance
        '                                ConstructionType = value
        '                            Else
        '                                'at least 2nd instance
        '                                If ConstructionType = "" OrElse String.IsNullOrWhiteSpace(value) = False Then
        '                                    ConstructionType = value
        '                                End If
        '                            End If
        '                        Case UCase("rt_sqft")
        '                            'SquareFeet
        '                            If valCounter < 2 Then
        '                                '1st instance
        '                                SquareFeet = value
        '                            Else
        '                                'at least 2nd instance
        '                                If SquareFeet = "" OrElse String.IsNullOrWhiteSpace(value) = False Then
        '                                    SquareFeet = value
        '                                End If
        '                            End If
        '                        Case UCase("rt_rooftype")
        '                            'RoofType
        '                            If valCounter < 2 Then
        '                                '1st instance
        '                                RoofType = value
        '                            Else
        '                                'at least 2nd instance
        '                                If RoofType = "" OrElse String.IsNullOrWhiteSpace(value) = False Then
        '                                    RoofType = value
        '                                End If
        '                            End If
        '                        Case UCase("rt_archstyle")
        '                            'ArchitecturalStyle
        '                            If valCounter < 2 Then
        '                                '1st instance
        '                                ArchitecturalStyle = value
        '                            Else
        '                                'at least 2nd instance
        '                                If ArchitecturalStyle = "" OrElse String.IsNullOrWhiteSpace(value) = False Then
        '                                    ArchitecturalStyle = value
        '                                End If
        '                            End If
        '                        Case UCase("rt_constquality")
        '                            'ConstructionQuality
        '                            If valCounter < 2 Then
        '                                '1st instance
        '                                ConstructionQuality = value
        '                            Else
        '                                'at least 2nd instance
        '                                If ConstructionQuality = "" OrElse String.IsNullOrWhiteSpace(value) = False Then
        '                                    ConstructionQuality = value
        '                                End If
        '                            End If
        '                        Case UCase("rt_shape")
        '                            'PhysicalShape
        '                            If valCounter < 2 Then
        '                                '1st instance
        '                                PhysicalShape = value
        '                            Else
        '                                'at least 2nd instance
        '                                If PhysicalShape = "" OrElse String.IsNullOrWhiteSpace(value) = False Then
        '                                    PhysicalShape = value
        '                                End If
        '                            End If
        '                        Case UCase("rt_exterior1")
        '                            'PrimaryExterior
        '                            If valCounter < 2 Then
        '                                '1st instance
        '                                PrimaryExterior = value
        '                            Else
        '                                'at least 2nd instance
        '                                If PrimaryExterior = "" OrElse String.IsNullOrWhiteSpace(value) = False Then
        '                                    PrimaryExterior = value
        '                                End If
        '                            End If
        '                        Case UCase("return_rpc")
        '                            'ReplacementCostValue
        '                            If valCounter < 2 Then
        '                                '1st instance
        '                                ReplacementCostValue = value
        '                            Else
        '                                'at least 2nd instance
        '                                If ReplacementCostValue = "" OrElse String.IsNullOrWhiteSpace(value) = False Then
        '                                    ReplacementCostValue = value
        '                                End If
        '                            End If
        '                        Case UCase("rt_rpctype")
        '                            'ReplacementCostType
        '                            If valCounter < 2 Then
        '                                '1st instance
        '                                ReplacementCostType = value
        '                            Else
        '                                'at least 2nd instance
        '                                If ReplacementCostType = "" OrElse String.IsNullOrWhiteSpace(value) = False Then
        '                                    ReplacementCostType = value
        '                                End If
        '                            End If
        '                        Case UCase("rt_areas")
        '                            'AdditionalAreas
        '                            If valCounter < 2 Then
        '                                '1st instance
        '                                AdditionalAreas = value
        '                            Else
        '                                'at least 2nd instance
        '                                If AdditionalAreas = "" OrElse String.IsNullOrWhiteSpace(value) = False Then
        '                                    AdditionalAreas = value
        '                                End If
        '                            End If
        '                    End Select
        '                Next
        '            End If
        '        Next

        '    End If

        'End Sub

#Region "IDisposable Support"
        Private disposedValue As Boolean ' To detect redundant calls

        ' IDisposable
        'Protected Overridable Sub Dispose(disposing As Boolean)
        'updated for QuickQuoteBaseObject inheritance
        Protected Overloads Sub Dispose(disposing As Boolean)
            If Not Me.disposedValue Then
                If disposing Then
                    ' TODO: dispose managed state (managed objects).
                    If _db_propertyValuationId IsNot Nothing Then 'added 8/7/2014
                        db_propertyValuationId = Nothing
                    End If
                    If _db_propertyValuationResponseId IsNot Nothing Then 'added 8/7/2014
                        _db_propertyValuationResponseId = Nothing
                    End If
                    If _AuthId IsNot Nothing Then
                        _AuthId = Nothing
                    End If
                    If _IFM_UniqueValuationId IsNot Nothing Then
                        _IFM_UniqueValuationId = Nothing
                    End If
                    If _VendorValuationId IsNot Nothing Then
                        _VendorValuationId = Nothing
                    End If
                    If _YearBuilt IsNot Nothing Then
                        _YearBuilt = Nothing
                    End If
                    If _ConstructionType IsNot Nothing Then
                        _ConstructionType = Nothing
                    End If
                    If _SquareFeet IsNot Nothing Then
                        _SquareFeet = Nothing
                    End If
                    If _RoofType IsNot Nothing Then
                        _RoofType = Nothing
                    End If
                    If _ArchitecturalStyle IsNot Nothing Then
                        _ArchitecturalStyle = Nothing
                    End If
                    If _ConstructionQuality IsNot Nothing Then
                        _ConstructionQuality = Nothing
                    End If
                    If _PhysicalShape IsNot Nothing Then
                        _PhysicalShape = Nothing
                    End If
                    If _PrimaryExterior IsNot Nothing Then
                        _PrimaryExterior = Nothing
                    End If
                    If _ReplacementCostValue IsNot Nothing Then
                        _ReplacementCostValue = Nothing
                    End If
                    If _ReplacementCostType IsNot Nothing Then
                        _ReplacementCostType = Nothing
                    End If
                    If _AdditionalAreas IsNot Nothing Then
                        _AdditionalAreas = Nothing
                    End If
                    If _VendorParams IsNot Nothing Then 'added 8/8/2014
                        _VendorParams = Nothing
                    End If
                    _db_loadedBackIntoQuote = Nothing 'added 8/8/2014
                    'If _Timestamp IsNot Nothing Then 'added 8/7/2014; removed 8/7/2014
                    '    _Timestamp = Nothing
                    'End If
                    If _db_inserted IsNot Nothing Then 'added 8/7/2014
                        _db_inserted = Nothing
                    End If
                    If _db_updated IsNot Nothing Then 'added 8/7/2014
                        _db_updated = Nothing
                    End If
                    qqHelper.DisposeString(_StructuresReplacementCostValue) 'added 7/29/2015 for Farm
                    qqHelper.DisposeStrings(_StructuresReplacementCostValues)
                    qqHelper.DisposeString(_StructuresReplacementCostValueTotal) 'added 7/29/2015 for Farm
                    qqHelper.DisposeString(_UniqueItemsTotalValue) 'added 7/29/2015 for Farm
                    qqHelper.DisposeString(_ActualCashValue) 'added 7/29/2015 for Farm
                    qqHelper.DisposeStrings(_ActualCashValues)
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
        'updated for QuickQuoteBaseObject inheritance
        Public Overrides Sub Dispose()
            ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
            Dispose(True)
            GC.SuppressFinalize(Me)
        End Sub
#End Region

    End Class
End Namespace
