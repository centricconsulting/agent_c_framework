Imports Microsoft.VisualBasic
Imports DC = Diamond.Common
Imports DCO = Diamond.Common.Objects
Imports DCE = Diamond.Common.Enums
Imports DCS = Diamond.Common.Services
Imports DCSM = Diamond.Common.Services.Messages
Imports DCSP = Diamond.Common.Services.Proxies
Imports IFMS = IFM.DiamondServices.Services.Diamond
Namespace Services.Notes
    Public Module Notes
        Public Function AssignOwnership(NoteId As Integer,
                                        UserId As Integer,
                                        Optional ByRef e As System.Exception = Nothing,
                                        Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim res As New DCSM.NotesService.AssignOwnership.Response
            Dim req As New DCSM.NotesService.AssignOwnership.Request

            With req.RequestData
                .NoteId = NoteId
                .UserId = UserId
            End With

            If IFMS.NotesService.AssignOwnership(res, req, e, dv) Then
                Return res.ResponseData.Success
            End If
            Return Nothing
        End Function

        Public Function CanEditNote(NoteId As Integer,
                                    Optional ByRef e As System.Exception = Nothing,
                                    Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim res As New DCSM.NotesService.CanEditNote.Response
            Dim req As New DCSM.NotesService.CanEditNote.Request

            With req.RequestData
                .NoteId = NoteId
            End With

            If IFMS.NotesService.CanEditNote(res, req, e, dv) Then
                Return res.ResponseData.Success
            End If
            Return Nothing
        End Function

        ''' <summary>
        ''' Creates a not on a policy with defaulted policy level values
        ''' </summary>
        ''' <param name="PolicyID"></param>
        ''' <param name="PolicyImageNum"></param>
        ''' <param name="Note"></param>
        ''' <param name="Title"></param>
        ''' <param name="UserID"></param>
        ''' <param name="IsPrivate"></param>
        ''' <param name="IsUrgent"></param>
        ''' <param name="CheckOnRenewal"></param>
        ''' <param name="ReturnNote"></param>
        ''' <param name="e"></param>
        ''' <param name="dv"></param>
        ''' <returns></returns>
        ''' <remarks>Function will be moved to middle tier later</remarks>
        Public Function CreatePolicyNote(PolicyID As Integer,
                                         PolicyImageNum As Integer,
                                         Note As String,
                                         Title As String,
                                         UserID As Integer,
                                         IsPrivate As Boolean,
                                         IsUrgent As Boolean,
                                         CheckOnRenewal As Boolean,
                                         Optional ByRef ReturnNote As DCO.Notes.NoteKeyStruct = Nothing,
                                         Optional ByRef e As Exception = Nothing,
                                         Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean

            Dim ns As New DCO.Notes.NoteKeyStruct

            With ns
                .Key01 = PolicyID
                .Key02 = PolicyImageNum
                .Key03 = 0
                .Key04 = 0
                With .Note
                    .CreateKey = PolicyID
                    .Note = Note
                    .Title = Title
                    .UserId = UserID
                    .NoteStatus = "Y"
                    .AttachLevelId = DCE.Notes.Level.Policy
                    .IsPrivate = IsPrivate
                    .IsUrgent = IsUrgent
                    .IsSticky = False
                    .CheckOnRenewal = CheckOnRenewal
                    '.NotesTypeId = DCE.Notes.NotesType.NotApplicable
                End With
            End With
            Return CreateNote(ns, ReturnNote, e, dv)
        End Function

        ''' <summary>
        ''' General call to create a not in diamond.
        ''' </summary>
        ''' <param name="NoteStruct"></param>
        ''' <param name="returnNote"></param>
        ''' <param name="e"></param>
        ''' <param name="dv"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function CreateNote(NoteStruct As DCO.Notes.NoteKeyStruct,
                                   ByRef returnNote As DCO.Notes.NoteKeyStruct,
                                   Optional ByRef e As Exception = Nothing,
                                   Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim req As New DCSM.NotesService.CreateNote.Request
            Dim res As New DCSM.NotesService.CreateNote.Response
            req.RequestData.NoteStruct = NoteStruct
            If IFMS.NotesService.CreateNote(res, req, e, dv) Then
                If res.ResponseData IsNot Nothing Then
                    If res.ResponseData.NoteStruct IsNot Nothing Then
                        returnNote = res.ResponseData.NoteStruct
                    End If
                    Return res.ResponseData.Success
                End If
            End If
            Return False

        End Function

        ''' <summary>
        ''' Calls LoadNotes for a policy
        ''' </summary>
        ''' <param name="policyID"></param>
        ''' <param name="policyImageNum"></param>
        ''' <param name="e"></param>
        ''' <param name="dv"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function LoadPolicyNotes(policyID As Integer, policyImageNum As Integer,
                                        Optional ByRef e As System.Exception = Nothing,
                                        Optional ByRef dv As DCO.DiamondValidation = Nothing) As List(Of DCO.Notes.NoteKeyStruct)
            Return LoadNotes(New DCO.Notes.LoadNoteStruct With {
                             .Key01 = policyID,
                             .Key02 = policyImageNum,
                             .Key03 = 0,
                             .Key04 = 0,
                             .CreateKey = policyID,
                             .AttachLevelId = DCE.Notes.Level.Policy}, e, dv)

        End Function

        ''' <summary>
        ''' General call to load diamond notes.
        ''' </summary>
        ''' <param name="loadStruct"></param>
        ''' <param name="e"></param>
        ''' <param name="dv"></param>
        ''' <returns></returns>
        ''' <remarks>Any calls so far only return hard error's</remarks>
        Public Function LoadNotes(loadStruct As DCO.Notes.LoadNoteStruct,
                                 Optional ByRef e As Exception = Nothing,
                                 Optional ByRef dv As DCO.DiamondValidation = Nothing) As List(Of DCO.Notes.NoteKeyStruct)
            Dim req As New DCSM.NotesService.LoadNotes.Request
            Dim res As New DCSM.NotesService.LoadNotes.Response
            req.RequestData.LoadStruct = loadStruct
            If IFMS.NotesService.LoadNotes(res, req, e, dv) Then
                If res.ResponseData.Notes IsNot Nothing Then
                    Return res.ResponseData.Notes.ToList
                End If
            End If
            Return Nothing
        End Function
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="NoteID"></param>
        ''' <param name="e">Exception bubbled from proxy call, will include any proxy validation error messages.</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function DeleteNote(NoteID As Integer,
                                   Optional ByRef e As Exception = Nothing) As Boolean
            Dim req As New DCSM.NotesService.DeleteNote.Request
            Dim res As New DCSM.NotesService.DeleteNote.Response
            req.RequestData.NoteId = NoteID
            If IFMS.NotesService.DeleteNote(res, req, e) Then
                If res.ResponseData IsNot Nothing Then
                    Return res.ResponseData.Success
                End If
            End If
            Return False
        End Function

        Public Function GetNoteCount(IncludeSystemGeneratedNotes As Boolean,
                                     LoadStruct As DCO.Notes.LoadNoteStruct,
                                     Optional ByRef e As System.Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCSM.NotesService.GetNoteCount.ResponseData
            Dim res As New DCSM.NotesService.GetNoteCount.Response
            Dim req As New DCSM.NotesService.GetNoteCount.Request

            With req.RequestData
                .IncludeSystemGeneratedNotes = IncludeSystemGeneratedNotes
                .LoadStruct = LoadStruct
            End With

            If IFMS.NotesService.GetNoteCount(res, req, e, dv) Then
                Return res.ResponseData
            End If
            Return Nothing
        End Function

        Public Function LoadKeys(InitialLevelNoteKeyStruct As DCO.Notes.NoteKeyStruct,
                                 KeysFound As Integer,
                                 Optional ByRef e As System.Exception = Nothing,
                                 Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCSM.NotesService.LoadKeys.ResponseData
            Dim res As New DCSM.NotesService.LoadKeys.Response
            Dim req As New DCSM.NotesService.LoadKeys.Request

            With req.RequestData
                .InitialLevelNoteKeyStruct = InitialLevelNoteKeyStruct
                .KeysFound = KeysFound
            End With

            If IFMS.NotesService.LoadKeys(res, req, e, dv) Then
                Return res.ResponseData
            End If
            Return Nothing
        End Function

        Public Function LoadNotesAtThisLevel(IncludeSystemGeneratedNotes As Boolean,
                                             LoadStruct As DCO.Notes.LoadNoteStruct,
                                             NotesFound As Integer,
                                             Optional ByRef e As System.Exception = Nothing,
                                             Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCSM.NotesService.LoadNotesAtThisLevel.ResponseData
            Dim res As New DCSM.NotesService.LoadNotesAtThisLevel.Response
            Dim req As New DCSM.NotesService.LoadNotesAtThisLevel.Request

            With req.RequestData
                .IncludeSystemGeneratedNotes = IncludeSystemGeneratedNotes
                .LoadStruct = LoadStruct
                .NotesFound = NotesFound
            End With

            If IFMS.NotesService.LoadNotesAtThisLevel(res, req, e, dv) Then
                Return res.ResponseData
            End If
            Return Nothing
        End Function

        Public Function SaveNote(NoteStruct As DCO.Notes.NoteKeyStruct,
                                 Optional ByRef e As System.Exception = Nothing,
                                 Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCO.Notes.NoteKeyStruct
            Dim res As New DCSM.NotesService.SaveNote.Response
            Dim req As New DCSM.NotesService.SaveNote.Request

            With req.RequestData
                .NoteStruct = NoteStruct
            End With

            If IFMS.NotesService.SaveNote(res, req, e, dv) Then
                Return res.ResponseData.NoteStruct
            End If
            Return Nothing
        End Function

        Public Function ToggleStickyNote(Note As DCO.Notes.Note,
                                         Optional ByRef e As System.Exception = Nothing,
                                         Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCO.Notes.Note
            Dim res As New DCSM.NotesService.ToggleStickyNote.Response
            Dim req As New DCSM.NotesService.ToggleStickyNote.Request

            With req.RequestData
                .Note = Note
            End With

            If IFMS.NotesService.ToggleStickyNote(res, req, e, dv) Then
                Return res.ResponseData.Note
            End If
            Return Nothing
        End Function
    End Module
End Namespace

