Scriptname ILS:ShipCargo Extends ObjectReference

; Shared by fixed and player-built Ship Cargo panels. Resolve the home ship
; on each activation so changing ships requires no relinking.
Bool Opening

Event OnActivate(ObjectReference akActionRef)
    Actor player = Game.GetPlayer()
    ; Only player activations may open cargo; reject overlapping requests.
    If akActionRef != player || Opening
        Return
    EndIf
    Opening = True
    ; Panels hold no inventory themselves; open the current home ship's cargo.
    SpaceshipReference homeShip = Game.GetPlayerHomeSpaceShip()
    If homeShip == None
        Opening = False
        Debug.MessageBox("Ship Cargo: no home ship is available.")
        Return
    EndIf
    ; Leave a paused menu context while preserving the player's position.
    If Utility.IsGameMenuPaused()
        player.MoveTo(player)
        player.WaitFor3dLoad()
    EndIf
    ; Block the focused panel briefly so it cannot intercept the cargo request.
    BlockActivation(True, True)
    homeShip.OpenInventory()
    BlockActivation(False)
    Opening = False
EndEvent
