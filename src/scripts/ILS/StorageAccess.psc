Scriptname ILS:StorageAccess Extends ObjectReference

; Shared Lodge inventory access for fixed and player-built access points.
; Items remain in the original bedroom safe; animated access points close on exit.
Bool Property CloseDoorOnExit = True Auto Const

; Track inventory-menu events and reject overlapping activation requests.
Bool LastMenuOpened
Bool Opening

Event OnActivate(ObjectReference akActionRef)
    ; Only player activations may open the shared storage.
    Actor player = Game.GetPlayer()
    If akActionRef != player || Opening
        Return
    EndIf
    Opening = True
    LastMenuOpened = False
    ; Resolve the vanilla inventory independently of this mod's load-order slot.
    ObjectReference safe = Game.GetFormFromFile(0x00266E81, "Starfield.esm") as ObjectReference
    If safe == None
        Opening = False
        Debug.MessageBox("Improved Lodge Storage: bedroom safe 00266E81 was not found.")
        Return
    EndIf
    ; Access points without a door opt out of the closing animation.
    If CloseDoorOnExit
        RegisterForMenuOpenCloseEvent("ContainerMenu")
    EndIf
    ; Preserve the player position while leaving a paused menu context.
    If Utility.IsGameMenuPaused()
        player.MoveTo(player)
        player.WaitFor3dLoad()
    EndIf
    ; The source must be blocked while forwarding activation. Otherwise the
    ; focused object's activation route can intercept the container request.
    ; Hide its activation prompt only for this call, then restore both flags.
    BlockActivation(True, True)
    Bool activated = safe.Activate(player)
    BlockActivation(False)
    Opening = False
    If !activated
        Debug.MessageBox("Improved Lodge Storage: the bedroom safe rejected activation.")
    EndIf
EndEvent

Event OnMenuOpenCloseEvent(String asMenuName, Bool abOpening)
    ; Close our own door only after an actual inventory open/close cycle.
    If asMenuName == "ContainerMenu"
        If abOpening
            LastMenuOpened = True
        ElseIf LastMenuOpened
            ; Both the bedroom safe and Captain's Locker use SimpleOpenClose01.
            ; Close their graph without another activation.
            PlayAnimation("Close")
            UnregisterForMenuOpenCloseEvent("ContainerMenu")
        EndIf
    EndIf
EndEvent
