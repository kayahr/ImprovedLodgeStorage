# Manual test checklists

Each checklist starts from its own named save and can be run independently.

## Prepare once

All four starting saves must contain **no player-built Lodge Storage access points**. The fixed devices placed by ILS in the Lodge and at the Spaceport are expected to be present.

Keep test materials **only in Lodge Storage**, with enough for unlocked crafting and building recipes. Choose materials used by the different station types. Use the existing contents to distinguish Lodge Storage from ship cargo.

Use stations without old storage links for the resource checks. Links from other mods are deliberately preserved. Prepare these saves with all menus closed, running each command at the corresponding location:

| Save command | Starting point |
| --- | --- |
| `save ILS_Lodge` | Lodge basement. |
| `save ILS_Home` | Player home with crafting/research stations. |
| `save ILS_Outpost` | Outpost with crafting/research stations. |
| `save ILS_Ship` | Inside your home ship **in space**, with fixed hab stations and a player-built station. |

These commands overwrite existing saves with the same names. Use **F5/F9** for temporary reload checks during testing, keeping the four named starting saves reusable.

Throughout testing, menus should open once per interaction, close normally and show no ILS debug messages. Known issue: build recipes may initially look unavailable despite sufficient Lodge materials. Construction must work; switching tabs refreshes the display.

## 1. Lodge (`load ILS_Lodge`)

1. [ ] Open the basement Lodge Storage locker. **Expected:** Lodge inventory opens; door closes after leaving the inventory.
2. [ ] Open each basement station: weapon, spacesuit, pharmaceutical, cooking, industrial and research. **Expected:** Lodge materials available immediately at all six stations, without building a locker.
3. [ ] Open the basement Ship Cargo panel. **Expected:** the current home ship's cargo inventory opens.
4. [ ] Visit the bedroom safe. **Expected:** exactly one visible safe, the same Lodge contents, no quick-loot list when targeting it, and its door closes afterwards. Check that the basement locker also has no quick-loot list.
5. [ ] Open the Spaceport container. **Expected:** Lodge inventory opens.
6. [ ] Use a public workbench outside the Lodge and your workshops. **Expected:** normal operation, without additional Lodge materials.

## 2. Home (`load ILS_Home`)

1. [ ] Open a station, close it and open the build menu. **Expected:** neither has access to the Lodge-only test materials.
2. [ ] Under **Misc**, build only a **Ship Cargo** panel and open it. **Expected:** free wall placement, correct model preview with transparent background, and access to home-ship cargo. Reopen the station: Lodge materials are still unavailable.
3. [ ] Build a **Lodge Storage** locker under **Misc** and open it. **Expected:** free wall placement, correct preview and orientation, shared Lodge contents, and the door closes afterwards.
4. [ ] Open and close every available crafting/research station. **Expected:** Lodge materials available on the first opening.
5. [ ] Open the build menu and construct an item using Lodge-only materials. **Expected:** construction succeeds. Move the locker and check a station again: access still works.
6. [ ] Build a second locker and a new station, then remove the first locker. **Expected:** the new station and build menu still have access through the remaining locker.
7. [ ] Leave and re-enter the home. Immediately open a station and the build menu. **Expected:** Lodge materials are still available without rebuilding the remaining locker.
8. [ ] Close all menus, press F5 to save and F9 to reload. Open a station and the build menu again. **Expected:** Lodge materials are still available without rebuilding the locker.
9. [ ] Remove the last locker and reopen a station and the build menu. **Expected:** Lodge-only materials are unavailable again. Press F5/F9 and repeat: access remains off.

## 3. Outpost (`load ILS_Outpost`)

1. [ ] Open a station and the build menu. **Expected:** no Lodge-only test materials are available.
2. [ ] Build and open a **Ship Cargo** panel from **Misc**, then reopen the station. **Expected:** free wall placement and correct cargo access, without enabling Lodge resources.
3. [ ] Build a **Lodge Storage** locker and open each available crafting/research station. **Expected:** Lodge resources available on the first opening; storage access and door closing also work.
4. [ ] Open the build menu, construct an item using Lodge materials, then build a new station. **Expected:** construction succeeds and the new station also has access.
5. [ ] Move the locker, build a second one and remove the first. **Expected:** resource access continues through the remaining locker.
6. [ ] Leave the outpost's buildable area and return, then press F5/F9 with menus closed. **Expected:** station and build-menu access still work without rebuilding the locker.
7. [ ] If a second outpost is available, travel there while keeping the first locker. Check resources before building a locker there, then build one and check again. Return to the first outpost. **Expected:** each outpost gains access only after its own locker is built; both continue to work when revisited.
8. [ ] Remove the last locker in the first outpost. Reopen a station and the build menu, then repeat after F5/F9. **Expected:** Lodge materials remain unavailable there, even if the second outpost still has a locker.

## 4. Ship in space (`load ILS_Ship`)

Keep the ship in space for these checks.

1. [ ] Open a fixed hab station, the player-built station and the Decorate menu. **Expected:** no Lodge-only test materials in any of them.
2. [ ] Build a **Ship Cargo** panel under **Misc** and open it. **Expected:** free wall placement and correct home-ship cargo. Reopen both stations: Lodge resources are still unavailable.
3. [ ] Build and open a **Lodge Storage** locker. **Expected:** correct wall placement and shared Lodge contents; door closes afterwards.
4. [ ] Open each available **fixed hab station**, including a fixed kitchen if present. **Expected:** Lodge materials available immediately, with normal opening and closing.
5. [ ] Open the **player-built station**, then build another station through Decorate and open that too. **Expected:** both have access. Construct an item in Decorate using Lodge-only materials: construction succeeds.
6. [ ] If an NPC is using a station, activate it, then another station. Open and close a station twice in quick succession. **Expected:** normal NPC dismissal, no stuck Busy state, no extra menu openings, and resource access still works on later activations.
7. [ ] Build a second locker and remove the first. Press F5/F9 with menus closed. **Expected:** both fixed and player-built stations still have access without rebuilding the remaining locker.
8. [ ] Remove the last locker. Check a fixed station, the player-built station and Decorate, then repeat after F5/F9. **Expected:** all lose Lodge access.
9. [ ] If another ship can be boarded, use a station aboard it. **Expected:** normal interaction without added ILS resources.

## Uninstall test (`load ILS_Lodge`)

1. [ ] Close all menus and press F5. Disable ILS, restart and load the quicksave.
2. [ ] Check the bedroom safe, basement stations and added devices/recipes. **Expected:** the original bedroom safe is visible and opens normally; added devices and recipes are gone; basement stations no longer get Lodge materials from ILS.
3. [ ] Re-enable ILS and restart before repeating any other checklist.
