# Shrine gathering bell

A completed shrine or any later cathedral level adds a small CC0 bell beside the
forecourt. Aim at its stand and tap the right front trigger (desktop F) to ring.
The bell swings, plays an original spatial bell tone, and emits a brief visual
ripple. It is a fixed interaction, not a carryable toy or a new God Tool menu.
Menu/tool ownership, carrying, zoom, suspension and pause guard the input.

A successful ring opens a 30-simulation-second invitation for up to four nearby
adults. Faith must be at least 25%, distance at most 160 game units, and the
villager must be free of productive work, carrying, social/play reservations,
household visits, injury and fright. Attendance is considered by the existing
worship scheduler, not forced by an independent movement controller. No one is
promised to attend. Audio volume/muting does not determine gameplay eligibility.

Accepted visitors use the existing forecourt positions and navigation. They
face the creature god and pray briefly after arrival, then return to normal
routines. Bell visits last four seconds; arrival has a bounded 90-second timeout.
One adult can accept once per invitation. A completed visit increments its saved
ritual and bell-visit counts; interrupted or unreachable visits earn nothing.
Player pickup cancels attendance. Fanaticism cancels the ordinary bell activity
and retains its existing scheduler behavior. A ring grants no faith, resources,
energy, creature experience or teaching receipt.

Rings have a saved 90-second cooldown. An already active invitation or unfinished
bell congregation also prevents reinviting. No new gathering is offered during
fanaticism or its recovery cooldown. Pause freezes simulation timers and motion.
Save/resume retains rings, cooldown and confirmed individual visits, while
clearing transient reservations, swing and invitations without replaying audio.
Higher cathedral faith thresholds, labor, stock and construction remain unchanged.

The fixed stand is built from existing Kenney CC0 planks/floor art; its clear
forecourt position and a native navigation footprint keep it outside the building.
The bell itself is Quaternius's 779-triangle Medieval Village model, explicitly
CC0 on its model and pack pages:
https://poly.pizza/m/cILuaO115Y
https://poly.pizza/bundle/Medieval-Village-Pack-NsHhjhlrfY
The retained GLB, hash and source/license are under ArtSources/CathedralBell.
`tools/prepare-cathedral-bell.py` converts it through Blender and synthesizes the
original 3.4-second mono 22050Hz bell tone. `CathedralBellImporter.Import` makes
the normalized prefab and material. The existing eight-voice audio pool is reused.

`CathedralBellChecks` model/runtime checks cover legacy and bounded saves,
completed-building and input gates, physical raycast/activation, pause, cooldown,
four-person acceptance cap, busy/held/injured exclusions, staged real-arrival
completion, duplicate/interrupted visits, unchanged faith, native Save/Load and
rearming. The visit fixture stages actors/destinations; it does not establish
natural full-village attendance frequency. Existing cathedral construction,
progress, religion and audio regressions pass. The bell and ring pose were
rendered and reviewed. Quest targeting, sound, comfort and performance remain
hardware playtest gates. No APK is built in this increment.
