# Undo the latest terrain stroke

God Tools → **Terraform → Undo stroke** reverses only the latest confirmed Raise, Lower or Smooth stroke. It is a rounded stone utility button with the existing Lucide rotate-ccw glyph; the sculpt tools remain circular. A dim button means no reversal is recorded or ground preparation is in progress. The usual selection press is consumed; holding the button cannot repeat the operation.

Undo preserves all earlier strokes. The last stroke's changed cells and their before/after heights are saved with the season, so the option survives save/resume. Older saves remain usable and simply have no undo until the next sculpt. A new confirmed stroke replaces the single history entry; a successful undo consumes it. There is no redo or unlimited history. New season clears both edits and history.

## Safety and accounting

Undo waits for the existing 1.5 simulation-second settling period. It does not spend additional energy or refund the original miracle. The forward stroke count is retained. It records one player-miracle reversal receipt and cannot reinforce a creature action.

The source hash, grid dimensions, bounded finite deltas, matching current heights and every cell's brush footprint are checked. The same actor/tree/building/resource/construction/physics protection is rechecked before publication. If someone has entered the patch, undo is refused and its history remains available for a later attempt.

Undo uses the existing background navigation preparation and main/private physics publication path. Pending work changes neither save data nor terrain. Pause, tracking loss, menu/tool cancellation, season change, geometry revision changes or timeout cancel preparation without consuming history. On success, visible mesh, ground collision, ball/tree physics terrain and both navigation profiles are updated through the existing transaction. Imported terrain remains untouched.

The button reverses terrain only. It does not rewind villagers, buildings, weather, resources, time, or the simulation's other outcomes. Existing terrain publication performance limits still apply; see [terrain shaping](TERRAIN_SHAPING.md).

## Validation

Verified in Unity 6000.6.7f1 Editor on 2026-10-31: model/legacy save guards; native save/load before and after undo; actual dock trigger with a held press; exact previous heights; finite zero-energy behavior; collision/private physics/two navigation profiles; occupancy, pause, explicit cancel and tracking-loss cancellation. The existing sculpt, navigation, god-tool, sanctuary and dock regressions also pass. Idle/expanded Terraform presentation was rendered and inspected. Staged multi-frame cases hold simulation time at zero while native navigation jobs finish, then restore the original time scale; these fixtures do not establish natural full-island play. Desktop Editor checks cannot establish Quest comfort or performance. Android player scripts also compiled successfully (43 assemblies). No APK or physical headset test is included in this increment.
