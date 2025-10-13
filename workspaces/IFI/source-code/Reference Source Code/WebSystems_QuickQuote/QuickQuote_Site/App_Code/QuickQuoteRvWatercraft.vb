Imports Microsoft.VisualBasic
'added 12/20/2013
Imports QuickQuote.CommonMethods
Imports helper = QuickQuote.CommonMethods.QuickQuoteHelperClass 'added 10/30/2014

Namespace QuickQuote.CommonObjects 'added namespace 12/20/2013
    ''' <summary>
    ''' object used to store RvWatercraft information
    ''' </summary>
    ''' <remarks>currently used as list object under Location object (<see cref="QuickQuoteLocation"/>)</remarks>
    <Serializable()> _
    Public Class QuickQuoteRvWatercraft 'added 8/1/2013 for HOM
        Inherits QuickQuoteBaseGenericObject(Of Object) 'added 8/4/2014
        Implements IDisposable

        Dim qqHelper As New QuickQuoteHelperClass

        Private _PolicyId As String
        Private _PolicyImageNum As String
        Private _AdditionalInterests As List(Of QuickQuoteAdditionalInterest)
        Private _Coverages As List(Of QuickQuoteCoverage)
        Private _Description As String
        'added 8/2/2013
        Private _AddedOperators As List(Of QuickQuoteOperator)
        Private _CostNew As String
        Private _HorsepowerCC As String
        Private _Length As String
        Private _Manufacturer As String
        Private _Model As String
        Private _Name As QuickQuoteName
        Private _OwnerOtherThanInsured As Boolean
        Private _RatedSpeed As String
        Private _RvWatercraftMotors As List(Of QuickQuoteRvWatercraftMotor)
        Private _RvWatercraftTypeId As String 'may need matching RvWatercraftType variable/property
        Private _SerialNumber As String
        Private _Year As String
        'added 8/6/2013
        Private _PropertyDeductibleLimitId As String 'may need matching PropertyDeductibleLimit variable/property
        Private _UninsuredMotoristBodilyInjuryLimitId As String 'may need matching UninsuredMotoristBodilyInjuryLimit variable/property
        Private _HasLiability As Boolean
        Private _HasLiabilityOnly As Boolean

        Private _HasCollision As Boolean 'added 11/29/2017 for HOM 2018 Upgrade

        'added 8/19/2013... testing for DFR; should also work for HOM
        Private _Operators As List(Of QuickQuoteOperator)

        'added 2/18/2014
        Private _HasConvertedCoverages As Boolean

        Private _CanUseAdditionalInterestNumForAdditionalInterestReconciliation As Boolean 'added 4/29/2014

        Private _RvWatercraftNum As String 'added 10/14/2014 for reconciliation
        Private _CanUseRvWatercraftMotorNumForRvWatercraftMotorReconciliation As Boolean 'added 10/15/2014 for reconciliation

        'added 10/29/2014
        Private _AssignedOperatorNums As List(Of Integer)
        Private _HasConvertedAssignedOperators As Boolean
        Private _CanUseOperatorNumForOperatorReconciliation As Boolean

        Private _Premium As String 'added 11/17/2014
        Private _CoveragesPremium As String 'added 11/17/2014

        Private _RemovedOperators As List(Of QuickQuoteOperator) 'added 12/4/2014; not being used for anything but view purposes to see if it's on the xml

        Private _DetailStatusCode As String 'added 5/15/2019

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
        Public Property AdditionalInterests As List(Of QuickQuoteAdditionalInterest)
            Get
                SetParentOfListItems(_AdditionalInterests, "{663B7C7B-F2AC-4BF6-965A-D30F41A05602}")
                Return _AdditionalInterests
            End Get
            Set(value As List(Of QuickQuoteAdditionalInterest))
                _AdditionalInterests = value
                SetParentOfListItems(_AdditionalInterests, "{663B7C7B-F2AC-4BF6-965A-D30F41A05602}")
            End Set
        End Property
        Public Property Coverages As List(Of QuickQuoteCoverage)
            Get
                SetParentOfListItems(_Coverages, "{663B7C7B-F2AC-4BF6-965A-D30F41A05603}")
                Return _Coverages
            End Get
            Set(value As List(Of QuickQuoteCoverage))
                _Coverages = value
                SetParentOfListItems(_Coverages, "{663B7C7B-F2AC-4BF6-965A-D30F41A05603}")
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
        ''' <summary>
        ''' Doesn't appear to be needed
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>Use Operators instead (along w/ Operators at the Policy level [property on QuickQuoteObject])</remarks>
        Public Property AddedOperators As List(Of QuickQuoteOperator)
            Get
                SetParentOfListItems(_AddedOperators, "{663B7C7B-F2AC-4BF6-965A-D30F41A05604}")
                Return _AddedOperators
            End Get
            Set(value As List(Of QuickQuoteOperator))
                _AddedOperators = value
                SetParentOfListItems(_AddedOperators, "{663B7C7B-F2AC-4BF6-965A-D30F41A05604}")
            End Set
        End Property
        Public Property CostNew As String
            Get
                Return _CostNew
                'updated 8/25/2014; won't use for now
                'Return qqHelper.QuotedPremiumFormat(_CostNew)
            End Get
            Set(value As String)
                _CostNew = value
                qqHelper.ConvertToQuotedPremiumFormat(_CostNew)
            End Set
        End Property
        Public Property HorsepowerCC As String
            Get
                Return _HorsepowerCC
            End Get
            Set(value As String)
                _HorsepowerCC = value
            End Set
        End Property
        Public Property Length As String
            Get
                Return _Length
            End Get
            Set(value As String)
                _Length = value
            End Set
        End Property
        Public Property Manufacturer As String
            Get
                Return _Manufacturer
            End Get
            Set(value As String)
                _Manufacturer = value
            End Set
        End Property
        Public Property Model As String
            Get
                Return _Model
            End Get
            Set(value As String)
                _Model = value
            End Set
        End Property
        Public Property Name As QuickQuoteName
            Get
                SetObjectsParent(_Name)
                Return _Name
            End Get
            Set(value As QuickQuoteName)
                _Name = value
                SetObjectsParent(_Name)
            End Set
        End Property
        Public Property OwnerOtherThanInsured As Boolean
            Get
                Return _OwnerOtherThanInsured
            End Get
            Set(value As Boolean)
                _OwnerOtherThanInsured = value
            End Set
        End Property
        Public Property RatedSpeed As String
            Get
                Return _RatedSpeed
            End Get
            Set(value As String)
                _RatedSpeed = value
            End Set
        End Property
        Public Property RvWatercraftMotors As List(Of QuickQuoteRvWatercraftMotor)
            Get
                SetParentOfListItems(_RvWatercraftMotors, "{663B7C7B-F2AC-4BF6-965A-D30F41A05605}")
                Return _RvWatercraftMotors
            End Get
            Set(value As List(Of QuickQuoteRvWatercraftMotor))
                _RvWatercraftMotors = value
                SetParentOfListItems(_RvWatercraftMotors, "{663B7C7B-F2AC-4BF6-965A-D30F41A05605}")
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>uses Diamond's RvWatercraftType table (-1=N/A, 0=N/A, 1=Watercraft, 2=Sailboat, 3=Boat Motor Only, 4=Boat Trailer, 5=Accessories &amp; Equipment, 6=Golf Cart, 7=Jet Skis &amp; Waverunners, 8=4 Wheel All Terrain Vehicle, 9=Snowmobile - Named Perils, 10=Snowmobile - Special Coverage, 11=Snowmobile - Trailer, 12=Other RV)</remarks>
        Public Property RvWatercraftTypeId As String '-1=N/A; 0=N/A; 1=Watercraft; 2=Sailboat; 3=Boat Motor Only; 4=Boat Trailer; 5=Accessories & Equipment; 6=Golf Cart; 7=Jet Skis & Waverunners; 8=4 Wheel All Terrain Vehicle; 9=Snowmobile - Named Perils; 10=Snowmobile - Special Coverage; 11=Snowmobile - Trailer; 12=Other RV
            Get
                Return _RvWatercraftTypeId
            End Get
            Set(value As String)
                _RvWatercraftTypeId = value
            End Set
        End Property
        Public Property SerialNumber As String
            Get
                Return _SerialNumber
            End Get
            Set(value As String)
                _SerialNumber = value
            End Set
        End Property
        Public Property Year As String
            Get
                Return _Year
            End Get
            Set(value As String)
                _Year = value
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 70097</remarks>
        Public Property PropertyDeductibleLimitId As String '
            Get
                Return _PropertyDeductibleLimitId
            End Get
            Set(value As String)
                _PropertyDeductibleLimitId = value
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 294</remarks>
        Public Property UninsuredMotoristBodilyInjuryLimitId As String '
            Get
                Return _UninsuredMotoristBodilyInjuryLimitId
            End Get
            Set(value As String)
                _UninsuredMotoristBodilyInjuryLimitId = value
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 20167</remarks>
        Public Property HasLiability As Boolean
            Get
                Return _HasLiability
            End Get
            Set(value As Boolean)
                _HasLiability = value
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 80149</remarks>
        Public Property HasLiabilityOnly As Boolean
            Get
                Return _HasLiabilityOnly
            End Get
            Set(value As Boolean)
                _HasLiabilityOnly = value
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 80150</remarks>
        Public Property HasCollision As Boolean
            Get
                Return _HasCollision
            End Get
            Set(value As Boolean)
                _HasCollision = value
            End Set
        End Property
        ''' <summary>
        ''' property to assign operators to the RvWatercraft
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>must be used w/ Operators at the Policy level (property on QuickQuoteObject); OperatorNum is the only thing that needs to be set if everything is already populated on corresponding object at policy level</remarks>
        Public Property Operators As List(Of QuickQuoteOperator)
            Get
                SetParentOfListItems(_Operators, "{663B7C7B-F2AC-4BF6-965A-D30F41A05606}")
                Return _Operators
            End Get
            Set(value As List(Of QuickQuoteOperator))
                _Operators = value
                SetParentOfListItems(_Operators, "{663B7C7B-F2AC-4BF6-965A-D30F41A05606}")
            End Set
        End Property

        'added 2/18/2014
        Public Property HasConvertedCoverages As Boolean
            Get
                Return _HasConvertedCoverages
            End Get
            Set(value As Boolean)
                _HasConvertedCoverages = value
            End Set
        End Property

        Public Property CanUseAdditionalInterestNumForAdditionalInterestReconciliation As Boolean 'added 4/29/2014
            Get
                Return _CanUseAdditionalInterestNumForAdditionalInterestReconciliation
            End Get
            Set(value As Boolean)
                _CanUseAdditionalInterestNumForAdditionalInterestReconciliation = value
            End Set
        End Property

        Public Property RvWatercraftNum As String 'added 10/14/2014 for reconciliation
            Get
                Return _RvWatercraftNum
            End Get
            Set(value As String)
                _RvWatercraftNum = value
            End Set
        End Property
        Public Property CanUseRvWatercraftMotorNumForRvWatercraftMotorReconciliation As Boolean 'added 10/15/2014 for reconciliation
            Get
                Return _CanUseRvWatercraftMotorNumForRvWatercraftMotorReconciliation
            End Get
            Set(value As Boolean)
                _CanUseRvWatercraftMotorNumForRvWatercraftMotorReconciliation = value
            End Set
        End Property

        'added 10/29/2014
        Public Property AssignedOperatorNums As List(Of Integer)
            Get
                Return _AssignedOperatorNums
            End Get
            Set(value As List(Of Integer))
                _AssignedOperatorNums = value
            End Set
        End Property
        Public Property HasConvertedAssignedOperators As Boolean
            Get
                Return _HasConvertedAssignedOperators
            End Get
            Set(value As Boolean)
                _HasConvertedAssignedOperators = value
            End Set
        End Property
        Public Property CanUseOperatorNumForOperatorReconciliation As Boolean
            Get
                Return _CanUseOperatorNumForOperatorReconciliation
            End Get
            Set(value As Boolean)
                _CanUseOperatorNumForOperatorReconciliation = value
            End Set
        End Property

        Public Property Premium As String 'added 11/17/2014
            Get
                Return _Premium
            End Get
            Set(value As String)
                _Premium = value
                qqHelper.ConvertToQuotedPremiumFormat(_Premium)
            End Set
        End Property
        Public Property CoveragesPremium As String 'added 11/17/2014
            Get
                Return _CoveragesPremium
            End Get
            Set(value As String)
                _CoveragesPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_CoveragesPremium)
            End Set
        End Property

        Public Property RemovedOperators As List(Of QuickQuoteOperator) 'added 12/4/2014; not being used for anything but view purposes to see if it's on the xml
            Get
                SetParentOfListItems(_RemovedOperators, "{663B7C7B-F2AC-4BF6-965A-D30F41A05607}")
                Return _RemovedOperators
            End Get
            Set(value As List(Of QuickQuoteOperator))
                _RemovedOperators = value
                SetParentOfListItems(_RemovedOperators, "{663B7C7B-F2AC-4BF6-965A-D30F41A05607}")
            End Set
        End Property

        Public Property DetailStatusCode As String 'added 5/15/2019
            Get
                Return _DetailStatusCode
            End Get
            Set(value As String)
                _DetailStatusCode = value
            End Set
        End Property

        Public Sub New()
            MyBase.New() 'added 8/4/2014
            SetDefaults()
        End Sub
        Private Sub SetDefaults()
            _PolicyId = ""
            _PolicyImageNum = ""
            '_AdditionalInterests = New List(Of QuickQuoteAdditionalInterest)
            _AdditionalInterests = Nothing 'added 8/4/2014
            '_Coverages = New List(Of QuickQuoteCoverage)
            _Coverages = Nothing 'added 8/4/2014
            _Description = ""
            '_AddedOperators = New List(Of QuickQuoteOperator)
            _AddedOperators = Nothing 'added 8/4/2014
            _CostNew = ""
            _HorsepowerCC = ""
            _Length = ""
            _Manufacturer = ""
            _Model = ""
            _Name = New QuickQuoteName '8/6/2013 - not sure what NameAddressSourceId should be
            _OwnerOtherThanInsured = False
            _RatedSpeed = ""
            '_RvWatercraftMotors = New List(Of QuickQuoteRvWatercraftMotor)
            _RvWatercraftMotors = Nothing 'added 8/4/2014
            _RvWatercraftTypeId = ""
            _SerialNumber = ""
            _Year = ""
            _PropertyDeductibleLimitId = ""
            _UninsuredMotoristBodilyInjuryLimitId = ""
            _HasLiability = False
            _HasLiabilityOnly = False

            _HasCollision = False 'added 11/30/2017 for HOM 2018 Upgrade

            '_Operators = New List(Of QuickQuoteOperator)
            _Operators = Nothing 'added 8/4/2014

            _HasConvertedCoverages = False 'added 2/18/2014

            _CanUseAdditionalInterestNumForAdditionalInterestReconciliation = False 'added 4/29/2014

            _RvWatercraftNum = "" 'added 10/14/2014 for reconciliation
            _CanUseRvWatercraftMotorNumForRvWatercraftMotorReconciliation = False 'added 10/15/2014 for reconciliation

            'added 10/29/2014
            '_AssignedOperatorNums = New List(Of Integer)
            _AssignedOperatorNums = Nothing
            _HasConvertedAssignedOperators = False
            _CanUseOperatorNumForOperatorReconciliation = False

            _Premium = "" 'added 11/17/2014
            _CoveragesPremium = "" 'added 11/17/2014

            '_RemovedOperators = New List(Of QuickQuoteOperator) 'added 12/4/2014; not being used for anything but view purposes to see if it's on the xml
            _RemovedOperators = Nothing

            _DetailStatusCode = "" 'added 5/15/2019
        End Sub
        'added 8/6/2013
        Public Sub ParseThruCoverages()
            If _Coverages IsNot Nothing AndAlso _Coverages.Count > 0 Then
                For Each c As QuickQuoteCoverage In _Coverages
                    Select Case c.CoverageCodeId
                        Case "70097" 'Combo: Inland_Marine_Watercraft_Property
                            PropertyDeductibleLimitId = c.CoverageLimitId
                        Case "294" 'Combo: Uninsured Bodily Injury
                            UninsuredMotoristBodilyInjuryLimitId = c.CoverageLimitId
                        Case "20167" 'CheckBox: Location - Watercraft Liability
                            _HasLiability = c.Checkbox
                        Case "80149" 'CheckBox: Liability Only
                            _HasLiabilityOnly = c.Checkbox
                        Case "80250" 'CheckBox: Collision
                            _HasCollision = c.Checkbox
                    End Select
                    CoveragesPremium = qqHelper.getSum(_CoveragesPremium, c.FullTermPremium) 'added 11/17/2014
                Next
            End If
        End Sub
        'added 4/29/2014 for additionalInterests reconciliation
        Public Sub ParseThruAdditionalInterests()
            If _AdditionalInterests IsNot Nothing AndAlso _AdditionalInterests.Count > 0 Then
                For Each ai As QuickQuoteAdditionalInterest In _AdditionalInterests
                    'note: should only happen when parsing xml (FinalizeQuickQuote method)... shouldn't happen in FinalizeQuickQuoteLight method
                    If _CanUseAdditionalInterestNumForAdditionalInterestReconciliation = False Then
                        If ai.HasValidAdditionalInterestNum = True Then
                            _CanUseAdditionalInterestNumForAdditionalInterestReconciliation = True
                            Exit For
                        End If
                    End If
                Next
            End If
        End Sub
        Public Function HasValidRvWatercraftNum() As Boolean 'added 10/14/2014 for reconciliation purposes
            Return qqHelper.IsValidQuickQuoteIdOrNum(_RvWatercraftNum)
        End Function
        'added 10/15/2014 for reconciliation
        Public Sub ParseThruRvWatercraftMotors()
            If _RvWatercraftMotors IsNot Nothing AndAlso _RvWatercraftMotors.Count > 0 Then
                For Each m As QuickQuoteRvWatercraftMotor In _RvWatercraftMotors
                    'note: should only happen when parsing xml (FinalizeQuickQuote method)... shouldn't happen in FinalizeQuickQuoteLight method
                    If _CanUseRvWatercraftMotorNumForRvWatercraftMotorReconciliation = False Then
                        If m.HasValidRvWatercraftMotorNum = True Then
                            _CanUseRvWatercraftMotorNumForRvWatercraftMotorReconciliation = True
                            Exit For
                        End If
                    End If
                Next
            End If
        End Sub
        'added 10/29/2014 for reconciliation
        'Public Sub ParseThruOperators()
        'updated 10/30/2014
        Public Sub ParseThruOperators(Optional ByVal policyLevelOperators As List(Of QuickQuoteOperator) = Nothing)
            If _Operators IsNot Nothing AndAlso _Operators.Count > 0 Then
                Dim policyLevelOperatorMatches As New List(Of Integer) 'added 10/30/2014
                Dim okayToCheckPolicyLevelOperators As Boolean = False 'added 10/30/2014
                If policyLevelOperators IsNot Nothing AndAlso policyLevelOperators.Count > 0 Then 'added 10/30/2014
                    okayToCheckPolicyLevelOperators = True
                End If
                '10/30/2014 note: may need to wipe out _AssignedOperatorNums
                For Each opp As QuickQuoteOperator In _Operators
                    'note: should only happen when parsing xml (FinalizeQuickQuote method)... shouldn't happen in FinalizeQuickQuoteLight method
                    If _CanUseOperatorNumForOperatorReconciliation = False Then
                        If opp.HasValidOperatorNum = True Then
                            _CanUseOperatorNumForOperatorReconciliation = True
                            'Exit For 'removed 10/30/2014 so new logic below could still execute
                        End If
                    End If

                    'added 10/30/2014
                    If okayToCheckPolicyLevelOperators = True Then
                        Dim ploCount As Integer = 0
                        Dim hasMatch As Boolean = False
                        For Each plo As QuickQuoteOperator In policyLevelOperators
                            ploCount += 1
                            If policyLevelOperatorMatches.Contains(ploCount) = False Then
                                'not already matched
                                If plo.PolicyLevelAssignmentNum > 0 AndAlso opp.PolicyLevelAssignmentNum > 0 Then
                                    'If opp.PolicyLevelAssignmentNum > 0 Then 'could also just look at this one and compare to count; adjust line below accordingly
                                    If plo.PolicyLevelAssignmentNum = opp.PolicyLevelAssignmentNum Then ' OrElse opp.PolicyLevelAssignmentNum = ploCount Then
                                        hasMatch = True
                                    End If
                                ElseIf plo.OperatorNum <> "" AndAlso opp.OperatorNum <> "" Then
                                    If plo.OperatorNum = opp.OperatorNum Then
                                        hasMatch = True
                                    End If
                                Else
                                    hasMatch = helper.IsQuickQuoteObjectMatch_Name(plo.Name, opp.Name)
                                End If
                                If hasMatch = True Then
                                    If _AssignedOperatorNums Is Nothing Then
                                        _AssignedOperatorNums = New List(Of Integer)
                                    End If
                                    _AssignedOperatorNums.Add(ploCount)
                                    policyLevelOperatorMatches.Add(ploCount)
                                    Exit For
                                End If
                            End If
                        Next
                    End If
                Next
            End If
        End Sub
        Public Overrides Function ToString() As String 'added 6/30/2015
            Dim str As String = ""
            If Me IsNot Nothing Then
                If Me.RvWatercraftTypeId <> "" Then
                    Dim rv As String = ""
                    rv = "RvWatercraftTypeId: " & Me.RvWatercraftTypeId
                    Dim rvType As String = qqHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteRvWatercraft, QuickQuoteHelperClass.QuickQuotePropertyName.RvWatercraftTypeId, Me.RvWatercraftTypeId)
                    If rvType <> "" Then
                        rv &= " (" & rvType & ")"
                    End If
                    str = qqHelper.appendText(str, rv, vbCrLf)
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
                    If _AdditionalInterests IsNot Nothing Then
                        If _AdditionalInterests.Count > 0 Then
                            For Each ai As QuickQuoteAdditionalInterest In _AdditionalInterests
                                ai.Dispose()
                                ai = Nothing
                            Next
                            _AdditionalInterests.Clear()
                        End If
                        _AdditionalInterests = Nothing
                    End If
                    If _Coverages IsNot Nothing Then
                        If _Coverages.Count > 0 Then
                            For Each c As QuickQuoteCoverage In _Coverages
                                c.Dispose()
                                c = Nothing
                            Next
                            _Coverages.Clear()
                        End If
                        _Coverages = Nothing
                    End If
                    If _Description IsNot Nothing Then
                        _Description = Nothing
                    End If
                    If _AddedOperators IsNot Nothing Then
                        If _AddedOperators.Count > 0 Then
                            For Each op As QuickQuoteOperator In _AddedOperators
                                op.Dispose()
                                op = Nothing
                            Next
                            _AddedOperators.Clear()
                        End If
                        _AddedOperators = Nothing
                    End If
                    If _CostNew IsNot Nothing Then
                        _CostNew = Nothing
                    End If
                    If _HorsepowerCC IsNot Nothing Then
                        _HorsepowerCC = Nothing
                    End If
                    If _Length IsNot Nothing Then
                        _Length = Nothing
                    End If
                    If _Manufacturer IsNot Nothing Then
                        _Manufacturer = Nothing
                    End If
                    If _Model IsNot Nothing Then
                        _Model = Nothing
                    End If
                    If _Name IsNot Nothing Then
                        _Name.Dispose()
                        _Name = Nothing
                    End If
                    If _OwnerOtherThanInsured <> Nothing Then
                        _OwnerOtherThanInsured = Nothing
                    End If
                    If _RatedSpeed IsNot Nothing Then
                        _RatedSpeed = Nothing
                    End If
                    If _RvWatercraftMotors IsNot Nothing Then
                        If _RvWatercraftMotors.Count > 0 Then
                            For Each m As QuickQuoteRvWatercraftMotor In _RvWatercraftMotors
                                m.Dispose()
                                m = Nothing
                            Next
                            _RvWatercraftMotors.Clear()
                        End If
                        _RvWatercraftMotors = Nothing
                    End If
                    If _RvWatercraftTypeId IsNot Nothing Then
                        _RvWatercraftTypeId = Nothing
                    End If
                    If _SerialNumber IsNot Nothing Then
                        _SerialNumber = Nothing
                    End If
                    If _Year IsNot Nothing Then
                        _Year = Nothing
                    End If
                    If _PropertyDeductibleLimitId IsNot Nothing Then
                        _PropertyDeductibleLimitId = Nothing
                    End If
                    If _UninsuredMotoristBodilyInjuryLimitId IsNot Nothing Then
                        _UninsuredMotoristBodilyInjuryLimitId = Nothing
                    End If
                    If _HasLiability <> Nothing Then
                        _HasLiability = Nothing
                    End If
                    If _HasLiabilityOnly <> Nothing Then
                        _HasLiabilityOnly = Nothing
                    End If
                    'qqHelper.DisposeString(_HasCollision)
                    'updated 1/4/2019 to fix error occuring since HasCollision is boolean and not string
                    _HasCollision = Nothing
                    If _Operators IsNot Nothing Then
                        If _Operators.Count > 0 Then
                            For Each op As QuickQuoteOperator In _Operators
                                op.Dispose()
                                op = Nothing
                            Next
                            _Operators.Clear()
                        End If
                        _Operators = Nothing
                    End If

                    'added 2/18/2014
                    If _HasConvertedCoverages <> Nothing Then
                        _HasConvertedCoverages = Nothing
                    End If

                    If _CanUseAdditionalInterestNumForAdditionalInterestReconciliation <> Nothing Then 'added 4/29/2014
                        _CanUseAdditionalInterestNumForAdditionalInterestReconciliation = Nothing
                    End If

                    If _RvWatercraftNum IsNot Nothing Then 'added 10/14/2014 for reconciliation
                        _RvWatercraftNum = Nothing
                    End If
                    _CanUseRvWatercraftMotorNumForRvWatercraftMotorReconciliation = Nothing 'added 10/15/2014 for reconciliation

                    'added 10/29/2014
                    If _AssignedOperatorNums IsNot Nothing Then
                        If _AssignedOperatorNums.Count > 0 Then
                            For Each n As Integer In _AssignedOperatorNums
                                n = Nothing
                            Next
                            '_AddedOperators.Clear()
                            'fixed 12/4/2014 to use correct variable
                            _AssignedOperatorNums.Clear()
                        End If
                        _AssignedOperatorNums = Nothing
                    End If
                    _HasConvertedAssignedOperators = Nothing
                    _CanUseOperatorNumForOperatorReconciliation = Nothing

                    If _Premium IsNot Nothing Then 'added 11/17/2014
                        _Premium = Nothing
                    End If
                    If _CoveragesPremium IsNot Nothing Then 'added 11/17/2014
                        _CoveragesPremium = Nothing
                    End If

                    If _RemovedOperators IsNot Nothing Then 'added 12/4/2014; not being used for anything but view purposes to see if it's on the xml
                        If _RemovedOperators.Count > 0 Then
                            For Each op As QuickQuoteOperator In _RemovedOperators
                                op.Dispose()
                                op = Nothing
                            Next
                            _RemovedOperators.Clear()
                        End If
                        _RemovedOperators = Nothing
                    End If

                    qqHelper.DisposeString(_DetailStatusCode) 'added 5/15/2019

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
