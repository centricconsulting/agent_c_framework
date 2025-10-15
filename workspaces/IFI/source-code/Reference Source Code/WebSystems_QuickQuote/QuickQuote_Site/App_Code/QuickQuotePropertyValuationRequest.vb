Imports Microsoft.VisualBasic
Imports helper = QuickQuote.CommonMethods.QuickQuoteHelperClass 'added 8/6/2014
Imports QuickQuote.CommonMethods 'added 8/7/2014

Namespace QuickQuote.CommonObjects
    ''' <summary>
    ''' object used to property valuation request information
    ''' </summary>
    ''' <remarks>currently using e2Value</remarks>
    <Serializable()>
    Public Class QuickQuotePropertyValuationRequest
        Inherits QuickQuoteBaseGenericObject(Of Object)
        Implements IDisposable

        Dim qqHelper As New QuickQuoteHelperClass 'updated 8/7/2014 from QuickQuote.CommonMethods.QuickQuoteHelperClass

        'see e2ValueNotes.txt and Portico Integration Instructions 06-10-2013.pdf

        'preload=1 for e2Value; only valid value; required
        'ccova = Coverage; max 30; must be numeric
        'ccovao = Origin of Coverage A; Must be one of the following (Previous Coverage, Agent's Estimate, Builder's Estimate, Claims Estimate, Insured's Estimate, Market Value, Purchase Price, Unknown)
        'pi_number = Policy Number; max 100
        'pi_carrier = Carrier; max 100
        'pi_agent = Agent; max 100
        'pi_requestor = Requestor; max 100
        'pi_interviewed = Person Interviewed; max 100
        'pi_inspdate = Inspection Date; max 50; must be valid date
        'pi_inspby = Inspected By; max 100
        'pi_effdate = Effective Date; max 50; must be valid date
        'pi_agencycode = Agency Code; max 100
        'pi_agencyName = Agency Name; max 100
        'pi_add1 = Agency Address 1; max 100
        'pi_add2 = Agency Address 2; max 100
        'pi_city = Agency City; max 100
        'pi_state = Agency State; max 2
        'pi_zip = Agency Zip; max 5
        'pi_phone = Agency Phone; max 10
        'Private _Username As String 'username in e2Value auth request 'removed 8/7/2014
        'Private _Password As String 'password in e2Value auth request 'removed 8/7/2014
        'Private _VirtualUsername As String 'vusername in e2Value auth request 'removed 8/7/2014
        Private _db_propertyValuationId As String 'added 8/7/2014
        Private _db_propertyValuationRequestId As String 'added 8/7/2014
        Private _AuthId As String 'authid in e2Value auth response; ad in e2Value redirect URL; return_ad in e2Value return URL
        Private _AuthCode As String 'authcode in e2Value auth response; ac in e2Value redirect
        Private _IFM_UniqueValuationId As String 'id in e2Value redirect URL; return_id in e2Value return URL
        Private _VendorValuationId As String 'propid in e2Value redirect URL; rt_propid in e2Value return URL... if rt_propid = 1 in redirect (default)
        Private _ClientIsBusiness As Boolean 'clientisbusiness in e2Value redirect URL; 1 or 0; default = 1; If value is 1 then the business entity name will be the value specified in cln1
        Private _ClientFirstName As String 'cfn1 in e2Value redirect URL; max 100
        Private _ClientMiddleInitial As String 'cmi1 in e2Value redirect URL; max 1
        Private _ClientLastName As String 'cln1 in e2Value redirect URL; max 100
        Private _ClientAddress1 As String 'paddr1 in e2Value redirect URL; max 150
        Private _ClientAddress2 As String 'paddr2 in e2Value redirect URL; max 150
        Private _ClientCity As String 'pcity in e2Value redirect URL; max 100
        Private _ClientState As String 'pstate in e2Value redirect URL; max 2
        Private _ClientZip As String 'pzip in e2Value redirect URL; max 5
        Private _ReturnUrl As String 'u in e2Value redirect URL; max 7000; A return URL provided by the partner. If this field is provided during the initial request, there will be a link created at the final calculation page with the URL provided by the partner. If no custom fieldnames were used in the return URL, partner should append a character "?" to the end of the URL. (e.g. http://www.returnhere.com/index.asp?)
        'postback_rt_autopostbackurl; only valid for BPR and EXR; max 5000; A return URL provided by the partner. If this field is provided during the initial request, system will automatically call the provided URL behind the scene. If no custom fieldnames were used in the return URL, partner should append a character "?" to the end of the URL. (e.g. http://www.returnhere.com/index.asp?)
        Private _ReturnUrlLinkText As String 'bname in e2Value redirect URL; max 50; This is the text of the return URL link that will be display on the final calculation page. If this is not provided during the initial request, a default name will be used.
        Private _YearBuilt As String 'yrblt in e2Value redirect URL; max 4; must be valid year; rt_yrblt in return URL... if rt_yrblt = 1 in redirect (default). In QCE, CMM, FNR, and HSD, returns the year built of each structure in the property separated by a “|” character in the return URL specified by the user. Value will be returned as rt_yrblt=<yrblt1|yrblt2|yrblt3|etc…>
        Private _ReturnYearBuilt As Boolean 'rt_yrblt in redirect URL; 1 = default
        Private _ConstructionType As String 'consttype in e2Value redirect URL; max 100; see valid EVS values; rt_consttype in return URL... if rt_consttype = 1 in redirect (default). In QCE, CMM, FNR, and HSD, returns the construction type of each structure in the property separated by a “|” character in the return URL specified by the user. Value will be returned as rt_consttype=<consttype1|consttype2|consttype3|etc…>
        Private _ReturnConstructionType As Boolean 'rt_consttype in redirect URL; 1 = default
        Private _SquareFeet As String 'sqft in e2Value redirect URL; max 10; must be valid numeric value; rt_sqft in return URL... if rt_sqft = 1 in redirect (default). In QCE, CMM, FNR, and HSD, returns the square feet of each structure in the property separated by a “|” character in the return URL specified by the user. Value will be returned as rt_sqft=<sqft1|sqft2|sqft3|etc…>
        Private _ReturnSquareFeet As Boolean 'rt_sqft in redirect URL; 1 = default
        Private _RoofType As String 'rooftype in e2Value redirect URL; max 100; see valid EVS values; rt_rooftype in return URL... if rt_rooftype = 1 in redirect URL (default).
        Private _ReturnRoofType As Boolean 'rt_rooftype in redirect URL; 1 = default
        'Private _RoofCovering As String 'only valid for QCE and CMM; roofcov in e2Value redirect URL; max 100; see valid EVS values; rt_roofcov in return URL... if rt_roofcov = 1 in redirect URL (default). Returns the roof covering of each structure in the property separated by a “|” character in the return URL specified by the user. Value will be returned as rt_roofcov=<roofcov1|roofcov2|roofcov3|etc…>
        'Private _StructureType As String 'only valid for QCE and CMM; structtype in e2Value redirect URL; max 100; see valid EVS values; rt_structtype in return URL... if rt_structtype = 1 in redirect URL (default). Returns the structure type of each structure in the property separated by a “|” character in the return URL specified by the user. Value will be returned as rt_structtype=<structtype1|structtype2|structtype3|etc…>
        'Private _NumberOfFloors As String 'only valid for QCE and CMM; numfloors in e2Value redirect URL; max 4; must be valid integer; rt_numfloors in return URL... if rt_numfloors = 1 in redirect URL (default). Returns the number of floors of each structure in the property separated by a “|” character in the return URL specified by the user. Value will be returned as rt_numfloors=<numfloors1|numfloors2|numfloors3|etc…>
        Private _ArchitecturalStyle As String 'archstyle in e2Value redirect URL; max 100; see valid EVS values; rt_archstyle in return URL... if rt_archstyle = 1 in redirect URL (default = 0)
        Private _ReturnArchitecturalStyle As Boolean 'rt_archstyle in redirect URL; 0 = default
        Private _ConstructionQuality As String 'constquality in e2Value redirect URL; max 100; see valid EVS values; rt_constquality in return URL... if rt_constquality = 1 in redirect URL (default = 0)
        Private _ReturnConstructionQuality As Boolean 'rt_constquality in redirect URL; 0 = default
        Private _PhysicalShape As String 'shape in e2Value redirect URL; see valid EVS values; rt_shape in return URL... if rt_shape = 1 in redirect URL (default = 0)
        Private _ReturnPhysicalShape As Boolean 'rt_shape in redirect URL; 0 = default
        Private _PrimaryExterior As String 'exterior1 in e2Value redirect URL; see valid EVS values; rt_exterior1 in return URL... if rt_exterior1 = 1 in redirect URL (default = 0)
        Private _ReturnPrimaryExterior As Boolean 'rt_exterior1 in redirect URL; 0 = default
        Private _ReturnUrlNoPopup As Boolean 'nopopup in e2Value redirect URL; 0 or 1; default = 0; If the nopopup parameter is specified and set to 1, the return URL will open in the same browser window as the estimate rather than in a popup window which is the default behavior.
        'goToSDW; 0 or 1; default = 0. If the goToSDW parameter is specified and set to 1, the system will bypass the estimator and any other specified CML parameters and take the user directly to the Smart Data Warehouse. If the u parameter was also specified, then a return button will appear in the SDW pointing back to the URL that the user specified. The bname and nopopup parameters can also be used in this case to control the settings of the return button.
        Private _ReturnReplacementCostType As Boolean 'rt_rpctype in redirect URL; 0 or 1; default = 0. Returns the Replacement Cost Type in the return URL specified by the user. Value will be returned as rt_rpctype=<replacement cost type> where < replacement cost type> is the actual Replacement Cost Type value of this property. Possible return values for the Replacement Cost Type are “full”, “func” (for “functional”), or an empty value if none exists for this property.
        Private _ReturnAdditionalAreas As Boolean 'rt_areas in redirect URL; 0 or 1; default = 0. Returns the additional areas in the return URL specified by user. Value will be returned as rt_areas=<areas>. For <areas>, Columns are separated by | and rows are separated by a line break. ... For example: garage, attached|1997|500 ; 1/2 Story|1997|600... Note: Column 1 = area name; Column 2 = year built; Column 3 = sqft
        Private _VendorParams As String 'added 8/8/2014
        Private _db_sentToVendor As Boolean 'added 8/19/2014
        'Private _Timestamp As String 'added 8/7/2014; removed 8/7/2014
        Private _db_inserted As String 'added 8/7/2014
        Private _db_updated As String 'added 8/7/2014
        Private _unit As String
        Private _locNum As String
        Private _buildNum As String

        Public Property db_propertyValuationId As String 'added 8/7/2014
            Get
                Return _db_propertyValuationId
            End Get
            Set(value As String)
                _db_propertyValuationId = value
            End Set
        End Property
        Public Property db_propertyValuationRequestId As String 'added 8/7/2014
            Get
                Return _db_propertyValuationRequestId
            End Get
            Set(value As String)
                _db_propertyValuationRequestId = value
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
        Public Property AuthCode As String
            Get
                Return _AuthCode
            End Get
            Set(value As String)
                _AuthCode = value
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
        Public Property ClientIsBusiness As Boolean
            Get
                Return _ClientIsBusiness
            End Get
            Set(value As Boolean)
                _ClientIsBusiness = value
            End Set
        End Property
        Public Property ClientFirstName As String
            Get
                Return _ClientFirstName
            End Get
            Set(value As String)
                _ClientFirstName = value
            End Set
        End Property
        Public Property ClientMiddleInitial As String
            Get
                Return _ClientMiddleInitial
            End Get
            Set(value As String)
                _ClientMiddleInitial = value
            End Set
        End Property
        Public Property ClientLastName As String
            Get
                Return _ClientLastName
            End Get
            Set(value As String)
                _ClientLastName = value
            End Set
        End Property
        Public Property ClientAddress1 As String
            Get
                Return _ClientAddress1
            End Get
            Set(value As String)
                _ClientAddress1 = value
            End Set
        End Property
        Public Property ClientAddress2 As String
            Get
                Return _ClientAddress2
            End Get
            Set(value As String)
                _ClientAddress2 = value
            End Set
        End Property
        Public Property ClientCity As String
            Get
                Return _ClientCity
            End Get
            Set(value As String)
                _ClientCity = value
            End Set
        End Property
        Public Property ClientState As String
            Get
                Return _ClientState
            End Get
            Set(value As String)
                _ClientState = value
            End Set
        End Property
        Public Property ClientZip As String
            Get
                Return _ClientZip
            End Get
            Set(value As String)
                _ClientZip = value
            End Set
        End Property
        Public Property ReturnUrl As String
            Get
                Return _ReturnUrl
            End Get
            Set(value As String)
                _ReturnUrl = value
            End Set
        End Property
        Public Property ReturnUrlLinkText As String
            Get
                Return _ReturnUrlLinkText
            End Get
            Set(value As String)
                _ReturnUrlLinkText = value
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
        Public Property ReturnYearBuilt As Boolean
            Get
                Return _ReturnYearBuilt
            End Get
            Set(value As Boolean)
                _ReturnYearBuilt = value
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
        Public Property ReturnConstructionType As Boolean
            Get
                Return _ReturnConstructionType
            End Get
            Set(value As Boolean)
                _ReturnConstructionType = value
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
        Public Property ReturnSquareFeet As Boolean
            Get
                Return _ReturnSquareFeet
            End Get
            Set(value As Boolean)
                _ReturnSquareFeet = value
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
        Public Property ReturnRoofType As Boolean
            Get
                Return _ReturnRoofType
            End Get
            Set(value As Boolean)
                _ReturnRoofType = value
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
        Public Property ReturnArchitecturalStyle As Boolean
            Get
                Return _ReturnArchitecturalStyle
            End Get
            Set(value As Boolean)
                _ReturnArchitecturalStyle = value
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
        Public Property ReturnConstructionQuality As Boolean
            Get
                Return _ReturnConstructionQuality
            End Get
            Set(value As Boolean)
                _ReturnConstructionQuality = value
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
        Public Property ReturnPhysicalShape As Boolean
            Get
                Return _ReturnPhysicalShape
            End Get
            Set(value As Boolean)
                _ReturnPhysicalShape = value
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
        Public Property ReturnPrimaryExterior As Boolean
            Get
                Return _ReturnPrimaryExterior
            End Get
            Set(value As Boolean)
                _ReturnPrimaryExterior = value
            End Set
        End Property
        Public Property ReturnUrlNoPopup As Boolean
            Get
                Return _ReturnUrlNoPopup
            End Get
            Set(value As Boolean)
                _ReturnUrlNoPopup = value
            End Set
        End Property
        Public Property ReturnReplacementCostType As Boolean
            Get
                Return _ReturnReplacementCostType
            End Get
            Set(value As Boolean)
                _ReturnReplacementCostType = value
            End Set
        End Property
        Public Property ReturnAdditionalAreas As Boolean
            Get
                Return _ReturnAdditionalAreas
            End Get
            Set(value As Boolean)
                _ReturnAdditionalAreas = value
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
        Public Property db_sentToVendor As Boolean 'added 8/19/2014
            Get
                Return _db_sentToVendor
            End Get
            Set(value As Boolean)
                _db_sentToVendor = value
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

        Public Property locNum As String
            Get
                Return _locNum
            End Get
            Set(value As String)
                _locNum = value
            End Set
        End Property

        Public Property buildNum As String
            Get
                Return _buildNum
            End Get
            Set(value As String)
                _buildNum = value
            End Set
        End Property

        Public Property Unit As String
            Get
                Return _unit
            End Get
            Set(value As String)
                _unit = value
            End Set
        End Property

        Public Sub New()
            MyBase.New()
            SetDefaults()
        End Sub
        Private Sub SetDefaults()
            '_Username = "" 'removed 8/7/2014
            '_Password = "" 'removed 8/7/2014
            '_VirtualUsername = "" 'removed 8/7/2014
            _db_propertyValuationId = "" 'added 8/7/2014
            _db_propertyValuationRequestId = "" 'added 8/7/2014
            _AuthId = ""
            _AuthCode = ""
            _IFM_UniqueValuationId = ""
            _VendorValuationId = ""
            _ClientIsBusiness = False 'defaulted to True in e2Value if omitted
            _ClientFirstName = ""
            _ClientMiddleInitial = ""
            _ClientLastName = ""
            _ClientAddress1 = ""
            _ClientAddress2 = ""
            _ClientCity = ""
            _ClientState = ""
            _ClientZip = ""
            _ReturnUrl = ""
            _ReturnUrlLinkText = ""
            _YearBuilt = ""
            _ReturnYearBuilt = True 'to match e2Value default
            _ConstructionType = ""
            _ReturnConstructionType = True 'to match e2Value default
            _SquareFeet = ""
            _ReturnSquareFeet = True 'to match e2Value default
            _RoofType = ""
            _ReturnRoofType = True 'to match e2Value default
            _ArchitecturalStyle = ""
            _ReturnArchitecturalStyle = True 'defaulted to False in e2Value if omitted
            _ConstructionQuality = ""
            _ReturnConstructionQuality = True 'defaulted to False in e2Value if omitted
            _PhysicalShape = ""
            _ReturnPhysicalShape = True 'defaulted to False in e2Value if omitted
            _PrimaryExterior = ""
            _ReturnPrimaryExterior = True 'defaulted to False in e2Value if omitted
            _ReturnUrlNoPopup = True 'to match e2Value default; changed to True 8/28/2014
            _ReturnReplacementCostType = True 'defaulted to False in e2Value if omitted
            _ReturnAdditionalAreas = True 'defaulted to False in e2Value if omitted
            _VendorParams = "" 'added 8/8/2014
            _db_sentToVendor = False 'added 8/19/2014
            '_Timestamp = "" 'added 8/7/2014; removed 8/7/2014
            _db_inserted = "" 'added 8/7/2014
            _db_updated = "" 'added 8/7/2014
            _unit = ""
            _locNum = ""
            _buildNum = ""
        End Sub
        'Public Function QuerystringForE2Value() As String '8/7/2014 - moved to QuickQuotePropertyValuationHelperClass
        '    Dim qs As String = ""

        '    qs = "?ac=" & helper.UrlEncodedValue(AuthCode)
        '    qs &= "&ad=" & helper.UrlEncodedValue(AuthId)
        '    qs &= "&id=" & helper.UrlEncodedValue(IFM_UniqueValuationId)
        '    qs &= "&propid=" & helper.UrlEncodedValue(VendorValuationId)
        '    qs &= "&clientisbusiness=" & helper.UrlEncodedValue(qqHelper.BooleanToInt(ClientIsBusiness).ToString)
        '    qs &= "&cfn1=" & helper.UrlEncodedValue(ClientFirstName)
        '    qs &= "&cmi1=" & helper.UrlEncodedValue(ClientMiddleInitial)
        '    qs &= "&cln1=" & helper.UrlEncodedValue(ClientLastName)
        '    qs &= "&paddr1=" & helper.UrlEncodedValue(ClientAddress1)
        '    qs &= "&paddr2=" & helper.UrlEncodedValue(ClientAddress2)
        '    qs &= "&pcity=" & helper.UrlEncodedValue(ClientCity)
        '    qs &= "&pstate=" & helper.UrlEncodedValue(ClientState)
        '    qs &= "&pzip=" & helper.UrlEncodedValue(ClientZip)
        '    qs &= "&u=" & helper.UrlEncodedValue(ReturnUrl)
        '    qs &= "&bname=" & helper.UrlEncodedValue(ReturnUrlLinkText)
        '    qs &= "&yrblt=" & helper.UrlEncodedValue(YearBuilt)
        '    qs &= "&rt_yrblt=" & helper.UrlEncodedValue(qqHelper.BooleanToInt(ReturnYearBuilt).ToString)
        '    qs &= "&consttype=" & helper.UrlEncodedValue(ConstructionType)
        '    qs &= "&rt_consttype=" & helper.UrlEncodedValue(qqHelper.BooleanToInt(ReturnConstructionType).ToString)
        '    qs &= "&sqft=" & helper.UrlEncodedValue(SquareFeet)
        '    qs &= "&rt_sqft=" & helper.UrlEncodedValue(qqHelper.BooleanToInt(ReturnSquareFeet).ToString)
        '    qs &= "&rooftype=" & helper.UrlEncodedValue(RoofType)
        '    qs &= "&rt_rooftype=" & helper.UrlEncodedValue(qqHelper.BooleanToInt(ReturnRoofType).ToString)
        '    qs &= "&archstyle=" & helper.UrlEncodedValue(ArchitecturalStyle)
        '    qs &= "&rt_archstyle=" & helper.UrlEncodedValue(qqHelper.BooleanToInt(ReturnArchitecturalStyle).ToString)
        '    qs &= "&constquality=" & helper.UrlEncodedValue(ConstructionQuality)
        '    qs &= "&rt_constquality=" & helper.UrlEncodedValue(qqHelper.BooleanToInt(ReturnConstructionQuality).ToString)
        '    qs &= "&shape=" & helper.UrlEncodedValue(PhysicalShape)
        '    qs &= "&rt_shape=" & helper.UrlEncodedValue(qqHelper.BooleanToInt(ReturnPhysicalShape).ToString)
        '    qs &= "&exterior1=" & helper.UrlEncodedValue(PrimaryExterior)
        '    qs &= "&rt_exterior1=" & helper.UrlEncodedValue(qqHelper.BooleanToInt(ReturnPrimaryExterior).ToString)
        '    qs &= "&nopopup=" & helper.UrlEncodedValue(qqHelper.BooleanToInt(ReturnUrlNoPopup).ToString)
        '    qs &= "&rt_rpctype=" & helper.UrlEncodedValue(qqHelper.BooleanToInt(ReturnReplacementCostType).ToString)
        '    qs &= "&rt_areas=" & helper.UrlEncodedValue(qqHelper.BooleanToInt(ReturnAdditionalAreas).ToString)

        '    Return qs

        '    'could also do w/ string builder
        '    'Dim sbQS As New StringBuilder
        '    'sbQS.Append("")
        '    'Return sbQS.ToString
        'End Function

#Region "IDisposable Support"
        Private disposedValue As Boolean ' To detect redundant calls

        ' IDisposable
        'Protected Overridable Sub Dispose(disposing As Boolean)
        'updated for QuickQuoteBaseObject inheritance
        Protected Overloads Sub Dispose(disposing As Boolean)
            If Not Me.disposedValue Then
                If disposing Then
                    ' TODO: dispose managed state (managed objects).
                    'If _Username IsNot Nothing Then 'removed 8/7/2014
                    '    _Username = Nothing
                    'End If
                    'If _Password IsNot Nothing Then 'removed 8/7/2014
                    '    _Password = Nothing
                    'End If
                    'If _VirtualUsername IsNot Nothing Then 'removed 8/7/2014
                    '    _VirtualUsername = Nothing
                    'End If
                    If _db_propertyValuationId IsNot Nothing Then 'added 8/7/2014
                        db_propertyValuationId = Nothing
                    End If
                    If _db_propertyValuationRequestId IsNot Nothing Then 'added 8/7/2014
                        _db_propertyValuationRequestId = Nothing
                    End If
                    If _AuthId IsNot Nothing Then
                        _AuthId = Nothing
                    End If
                    If _AuthCode IsNot Nothing Then
                        _AuthCode = Nothing
                    End If
                    If _IFM_UniqueValuationId IsNot Nothing Then
                        _IFM_UniqueValuationId = Nothing
                    End If
                    If _VendorValuationId IsNot Nothing Then
                        _VendorValuationId = Nothing
                    End If
                    _ClientIsBusiness = Nothing
                    If _ClientFirstName IsNot Nothing Then
                        _ClientFirstName = Nothing
                    End If
                    If _ClientMiddleInitial IsNot Nothing Then
                        _ClientMiddleInitial = Nothing
                    End If
                    If _ClientLastName IsNot Nothing Then
                        _ClientLastName = Nothing
                    End If
                    If _ClientAddress1 IsNot Nothing Then
                        _ClientAddress1 = Nothing
                    End If
                    If _ClientAddress2 IsNot Nothing Then
                        _ClientAddress2 = Nothing
                    End If
                    If _ClientCity IsNot Nothing Then
                        _ClientCity = Nothing
                    End If
                    If _ClientState IsNot Nothing Then
                        _ClientState = Nothing
                    End If
                    If _ClientZip IsNot Nothing Then
                        _ClientZip = Nothing
                    End If
                    If _ReturnUrl IsNot Nothing Then
                        _ReturnUrl = Nothing
                    End If
                    If _ReturnUrlLinkText IsNot Nothing Then
                        _ReturnUrlLinkText = Nothing
                    End If
                    If _YearBuilt IsNot Nothing Then
                        _YearBuilt = Nothing
                    End If
                    _ReturnYearBuilt = Nothing
                    If _ConstructionType IsNot Nothing Then
                        _ConstructionType = Nothing
                    End If
                    _ReturnConstructionType = Nothing
                    If _SquareFeet IsNot Nothing Then
                        _SquareFeet = Nothing
                    End If
                    _ReturnSquareFeet = Nothing
                    If _RoofType IsNot Nothing Then
                        _RoofType = Nothing
                    End If
                    _ReturnRoofType = Nothing
                    If _ArchitecturalStyle IsNot Nothing Then
                        _ArchitecturalStyle = Nothing
                    End If
                    _ReturnArchitecturalStyle = Nothing
                    If _ConstructionQuality IsNot Nothing Then
                        _ConstructionQuality = Nothing
                    End If
                    _ReturnConstructionQuality = Nothing
                    If _PhysicalShape IsNot Nothing Then
                        _PhysicalShape = Nothing
                    End If
                    _ReturnPhysicalShape = Nothing
                    If _PrimaryExterior IsNot Nothing Then
                        _PrimaryExterior = Nothing
                    End If
                    _ReturnPrimaryExterior = Nothing
                    _ReturnUrlNoPopup = Nothing
                    _ReturnReplacementCostType = Nothing
                    _ReturnAdditionalAreas = Nothing
                    If _VendorParams IsNot Nothing Then 'added 8/8/2014
                        _VendorParams = Nothing
                    End If
                    _db_sentToVendor = Nothing 'added 8/19/2014
                    'If _Timestamp IsNot Nothing Then 'added 8/7/2014; removed 8/7/2014
                    '    _Timestamp = Nothing
                    'End If
                    If _db_inserted IsNot Nothing Then 'added 8/7/2014
                        _db_inserted = Nothing
                    End If
                    If _db_updated IsNot Nothing Then 'added 8/7/2014
                        _db_updated = Nothing
                    End If
                    If _unit IsNot Nothing Then
                        _unit = Nothing
                    End If
                    If _locNum IsNot Nothing Then
                        _locNum = Nothing
                    End If
                    If _buildNum IsNot Nothing Then
                        _buildNum = Nothing
                    End If
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
