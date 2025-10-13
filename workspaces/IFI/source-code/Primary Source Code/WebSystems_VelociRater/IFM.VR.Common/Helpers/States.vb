Option Explicit On
Option Strict On

Namespace IFM.VR.Common.Helpers
    Public Structure States
        Private Shared ReadOnly Property EmptyState As States
            Get
                Return New States(0, "", "")
            End Get
        End Property
        Public Enum Abbreviations
            None = 0
            AK = 1
            AL = 2
            AR = 3
            AZ = 4
            CA = 5
            CO = 6
            CT = 7
            DC = 8
            DE = 9
            FL = 10
            GA = 11
            HI = 12
            IA = 13
            ID = 14
            IL = 15
            [IN] = 16
            KS = 17
            KY = 18
            LA = 19
            MA = 20
            MD = 21
            [ME] = 22
            MI = 23
            MN = 24
            MO = 25
            MS = 26
            MT = 27
            NC = 28
            ND = 29
            NE = 30
            NH = 31
            NJ = 32
            NM = 33
            NV = 34
            NY = 35
            OH = 36
            OK = 37
            [OR] = 38
            PA = 39
            RI = 40
            SC = 41
            SD = 42
            TN = 43
            TX = 44
            UT = 45
            VA = 46
            VT = 47
            WA = 48
            WI = 49
            WV = 50
            WY = 51
        End Enum
        Public Enum StateNames
            None = 0
            Alaska = 1
            Alabama = 2
            Arkansas = 3
            Arizona = 4
            California = 5
            Colorado = 6
            Connecticut = 7
            District_Of_Columbia = 8
            Delaware = 9
            Florida = 10
            Georgia = 11
            Hawaii = 12
            Iowa = 13
            Idaho = 14
            Illinois = 15
            Indiana = 16
            Kansas = 17
            Kentucky = 18
            Louisiana = 19
            Massachusetts = 20
            Maryland = 21
            Maine = 22
            Michigan = 23
            Minnesota = 24
            Missouri = 25
            Mississippi = 26
            Montana = 27
            North_Carolina = 28
            North_Dakota = 29
            Nebraska = 30
            New_Hampshire = 31
            New_Jersey = 32
            New_Mexico = 33
            Nevada = 34
            New_York = 35
            Ohio = 36
            Oklahoma = 37
            Oregon = 38
            Pennsylvania = 39
            Rhode_Island = 40
            South_Carolina = 41
            South_Dakota = 42
            Tennessee = 43
            Texas = 44
            Utah = 45
            Virginia = 46
            Vermont = 47
            Washington = 48
            Wisconsin = 49
            West_Virginia = 50
            Wyoming = 51
        End Enum

        Private Shared _statesList As New List(Of States)
        Public Shared ReadOnly Property StatesList As System.Collections.ObjectModel.ReadOnlyCollection(Of States)
            Get
                Return New System.Collections.ObjectModel.ReadOnlyCollection(Of States)(_statesList)
            End Get
        End Property

        Public ReadOnly Property StateId As Int32
        Public ReadOnly Property StateName As String
        Public ReadOnly Property Abbreviation As String

        Public ReadOnly Property AbbreviationEnum As Abbreviations
            Get
                Return DirectCast(Me.StateId, Abbreviations)

            End Get
        End Property

        Public ReadOnly Property StateNameEnum As StateNames
            Get
                Return DirectCast(Me.StateId, StateNames)
            End Get
        End Property

        Public ReadOnly Property IsEmpty As Boolean
            Get
                Return StateId = 0
            End Get
        End Property

        Private Sub New(stateId As Int32, abbrev As String, stateName As String)
            Me.StateId = stateId
            Me.StateName = stateName
            Me.Abbreviation = abbrev
        End Sub
        Shared Sub New()
            _statesList.Add(New States(1, "AK", "Alaska"))
            _statesList.Add(New States(2, "AL", "Alabama"))
            _statesList.Add(New States(3, "AR", "Arkansas"))
            _statesList.Add(New States(4, "AZ", "Arizona"))
            _statesList.Add(New States(5, "CA", "California"))
            _statesList.Add(New States(6, "CO", "Colorado"))
            _statesList.Add(New States(7, "CT", "Connecticut"))
            _statesList.Add(New States(8, "DC", "District of Columbia"))
            _statesList.Add(New States(9, "DE", "Delaware"))
            _statesList.Add(New States(10, "FL", "Florida"))
            _statesList.Add(New States(11, "GA", "Georgia"))
            _statesList.Add(New States(12, "HI", "Hawaii"))
            _statesList.Add(New States(13, "IA", "Iowa"))
            _statesList.Add(New States(14, "ID", "Idaho"))
            _statesList.Add(New States(15, "IL", "Illinois"))
            _statesList.Add(New States(16, "IN", "Indiana"))
            _statesList.Add(New States(17, "KS", "Kansas"))
            _statesList.Add(New States(18, "KY", "Kentucky"))
            _statesList.Add(New States(19, "LA", "Louisiana"))
            _statesList.Add(New States(20, "MA", "Massachusetts"))
            _statesList.Add(New States(21, "MD", "Maryland"))
            _statesList.Add(New States(22, "ME", "Maine"))
            _statesList.Add(New States(23, "MI", "Michigan"))
            _statesList.Add(New States(24, "MN", "Minnesota"))
            _statesList.Add(New States(25, "MO", "Missouri"))
            _statesList.Add(New States(26, "MS", "Mississippi"))
            _statesList.Add(New States(27, "MT", "Montana"))
            _statesList.Add(New States(28, "NC", "North Carolina"))
            _statesList.Add(New States(29, "ND", "North Dakota"))
            _statesList.Add(New States(30, "NE", "Nebraska"))
            _statesList.Add(New States(31, "NH", "New Hampshire"))
            _statesList.Add(New States(32, "NJ", "New Jersey"))
            _statesList.Add(New States(33, "NM", "New Mexico"))
            _statesList.Add(New States(34, "NV", "Nevada"))
            _statesList.Add(New States(35, "NY", "New York"))
            _statesList.Add(New States(36, "OH", "Ohio"))
            _statesList.Add(New States(37, "OK", "Oklahoma"))
            _statesList.Add(New States(38, "OR", "Oregon"))
            _statesList.Add(New States(39, "PA", "Pennsylvania"))
            _statesList.Add(New States(40, "RI", "Rhode Island"))
            _statesList.Add(New States(41, "SC", "South Carolina"))
            _statesList.Add(New States(42, "SD", "South Dakota"))
            _statesList.Add(New States(43, "TN", "Tennessee"))
            _statesList.Add(New States(44, "TX", "Texas"))
            _statesList.Add(New States(45, "UT", "Utah"))
            _statesList.Add(New States(46, "VA", "Virginia"))
            _statesList.Add(New States(47, "VT", "Vermont"))
            _statesList.Add(New States(48, "WA", "Washington"))
            _statesList.Add(New States(49, "WI", "Wisconsin"))
            _statesList.Add(New States(50, "WV", "West Virginia"))
            _statesList.Add(New States(51, "WY", "Wyoming"))
        End Sub

        Public Shared Function GetStateInfoFromId(stateId As Int32) As States
            Return (From s In _statesList Where stateId = s.StateId Select s).DefaultIfEmpty(EmptyState).FirstOrDefault()
        End Function

        Public Shared Function DoesStateAbbreviationExist(abbrev As String) As Boolean
            If Not String.IsNullOrWhiteSpace(abbrev) Then
                Return (From s In States.StatesList Where s.Abbreviation = abbrev.Trim().ToUpper() Select s).Any()
            End If
            Return False
        End Function

        Public Shared Function DoesStateIdExist(id As Int32) As Boolean
            If id > 0 Then
                Return (From s In States.StatesList Where s.StateId = id Select s).Any()
            End If
            Return False
        End Function

        Public Shared Function GetStateInfoFromAbbreviation(abbrev As String) As States
            Return (From s In _statesList Where s.Abbreviation = abbrev.Trim().ToUpper() Select s).FirstOrDefault()
        End Function

        Public Shared Function GetStateInfosFromIds(stateIds As IEnumerable(Of Int32)) As List(Of States)
            If stateIds IsNot Nothing Then
                Dim results = From s In _statesList Where stateIds.Contains(s.StateId) Select s
                If results IsNot Nothing Then
                    Return results.ToList()
                End If
            End If
            Return New List(Of States)()
        End Function

        Public Shared Function GetStateAbbreviationsFromStateIds(stateIds As IEnumerable(Of Int32)) As IEnumerable(Of String)
            Return From sInfo In GetStateInfosFromIds(stateIds) Select sInfo.Abbreviation
        End Function

        Public Shared Function GetStateNamesFromStateIds(stateIds As IEnumerable(Of Int32)) As IEnumerable(Of String)
            Return From sInfo In GetStateInfosFromIds(stateIds) Select sInfo.StateName
        End Function



    End Structure
End Namespace