Scriptname ILS:StorageAccess Extends ObjectReference

; Shared Lodge inventory access for fixed and player-built access points.
; Items remain in the original bedroom safe; animated access points close on exit.
ObjectReference Property LodgeInventory Auto Const Mandatory
Bool Property CloseDoorOnExit = True Auto Const

; Only player-built access points participate in local crafting and building.
ILS:WorkshopCrafting Property Controller Auto Const

; Track inventory-menu events and reject overlapping activation requests.
Bool LastMenuOpened
Bool Opening
ObjectReference Workshop

Event OnLoad()
    WatchWorkshop()
EndEvent

Event OnWorkshopObjectPlaced(ObjectReference akReference)
    WatchWorkshop(True)
EndEvent

Event OnUnload()
    If Workshop != None && Controller != None
        Controller.RefreshWorkshop(Workshop)
    EndIf
EndEvent

; Fixed access points only open storage; player-built ones also enable local resources.
Function WatchWorkshop(Bool placed = False)
    If Controller == None
        Return
    EndIf
    Workshop = Controller.GetWorkshopFor(Self)
    Controller.WatchWorkshop(Workshop, Self, placed)
EndFunction

Event OnActivate(ObjectReference akActionRef)
    ; Only player activations may open the shared storage.
    Actor player = Game.GetPlayer()
    If akActionRef != player || Opening
        Return
    EndIf
    Opening = True
    LastMenuOpened = False
    If LodgeInventory == None
        Opening = False
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
    LodgeInventory.Activate(player)
    BlockActivation(False)
    Opening = False
EndEvent

Event OnMenuOpenCloseEvent(String asMenuName, Bool abOpening)
    ; Close our own door only after an actual inventory open/close cycle.
    If abOpening
        LastMenuOpened = True
    ElseIf LastMenuOpened
        ; Both the bedroom safe and Captain's Locker use SimpleOpenClose01.
        ; Close their graph without another activation.
        PlayAnimation("Close")
        UnregisterForMenuOpenCloseEvent("ContainerMenu")
    EndIf
EndEvent
