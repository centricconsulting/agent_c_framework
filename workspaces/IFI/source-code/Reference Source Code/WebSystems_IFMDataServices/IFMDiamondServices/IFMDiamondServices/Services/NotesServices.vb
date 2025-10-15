Imports Microsoft.VisualBasic
Imports DC = Diamond.Common
Imports DCO = Diamond.Common.Objects
Imports DCE = Diamond.Common.Enums
Imports DCS = Diamond.Common.Services
Imports DSCM = Diamond.Common.Services.Messages.NotesService
Imports DCSP = Diamond.Common.Services.Proxies.NotesServices
Imports IFM.DiamondServices.Services.Common


Namespace Services.Diamond.NotesService
    Public Module NotesService
        Public Function AssignOwnership(ByRef res As DSCM.AssignOwnership.Response,
                                                  ByRef req As DSCM.AssignOwnership.Request,
                                                  Optional ByRef e As Exception = Nothing,
                                                  Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.NotesServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.AssignOwnership
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function CanEditNote(ByRef res As DSCM.CanEditNote.Response,
                                                      ByRef req As DSCM.CanEditNote.Request,
                                                      Optional ByRef e As Exception = Nothing,
                                                      Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.NotesServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.CanEditNote
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function CreateNote(ByRef res As DSCM.CreateNote.Response,
                                            ByRef req As DSCM.CreateNote.Request,
                                            Optional ByRef e As Exception = Nothing,
                                            Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.NotesServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.CreateNote
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function DeleteNote(ByRef res As DSCM.DeleteNote.Response,
                                               ByRef req As DSCM.DeleteNote.Request,
                                               Optional ByRef e As Exception = Nothing,
                                               Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.NotesServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.DeleteNote
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function GetNoteCount(ByRef res As DSCM.GetNoteCount.Response,
                                           ByRef req As DSCM.GetNoteCount.Request,
                                           Optional ByRef e As Exception = Nothing,
                                           Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.NotesServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.GetNoteCount
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function LoadKeys(ByRef res As DSCM.LoadKeys.Response,
                                           ByRef req As DSCM.LoadKeys.Request,
                                           Optional ByRef e As Exception = Nothing,
                                           Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.NotesServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadKeys
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function LoadNotes(ByRef res As DSCM.LoadNotes.Response,
                                    ByRef req As DSCM.LoadNotes.Request,
                                    Optional ByRef e As Exception = Nothing,
                                    Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.NotesServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadNotes
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function LoadNotesAtThisLevel(ByRef res As DSCM.LoadNotesAtThisLevel.Response,
                                  ByRef req As DSCM.LoadNotesAtThisLevel.Request,
                                  Optional ByRef e As Exception = Nothing,
                                  Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.NotesServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadNotesAtThisLevel
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function SaveNote(ByRef res As DSCM.SaveNote.Response,
                                           ByRef req As DSCM.SaveNote.Request,
                                           Optional ByRef e As Exception = Nothing,
                                           Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.NotesServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.SaveNote
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function ToggleStickyNote(ByRef res As DSCM.ToggleStickyNote.Response,
                                                  ByRef req As DSCM.ToggleStickyNote.Request,
                                                  Optional ByRef e As Exception = Nothing,
                                                  Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.NotesServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.ToggleStickyNote
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
    End Module
End Namespace