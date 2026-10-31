# Whirlwind — physical god toy

Select Whirlwind from the God Tools scroll (desktop T, or the underside of the
left wrist). Aim at island ground near a free beachball or loose construction
pieces. Hold the right front trigger (desktop F) to lift and spin them. Move the
aim to move the column. Release to let the pieces fall with their current momentum.

The whirlwind can gather up to eight objects in a 16-game-unit radius and a
22-unit-high column. It applies bounded inward, tangential and lifting velocity
changes at 10 Hz. The three animated air ribbons reuse the existing effect
material; the circular stone token reuses the credited game-icons wind artwork.
No new placeholder item model, rigidbody scene or global physics setting is added.

Physics operates in the existing beachball and construction private scenes.
Block velocity is rotated into its construction site's coordinates. A pulse
costs 1 energy; the existing unlimited testing setting still applies. A stroke
lasts at most eight seconds and must be released to rearm after exhaustion or
its time limit. Invalid aim, pause, suspend, tool changes and closing the tool
stop the effect. Release never teleports an object, deletes its velocity or
changes gravity.

Held objects, active creature/social reservations, attached pieces, bridges,
locked or occupied construction sites, and active ball games are excluded.
The tool does not take locomotion from an actor or uproot scenery. Ordinary
physical contacts can disturb free construction; this is not a new building-
destruction, injury or creature-learning system. Environmental ball motion keeps
its existing player/environment attribution and does not reward creature throws.
The existing bounded evidence journal records player miracle activity.

Runtime vortex fields are transient. Existing ball reset and construction-save
rules remain authoritative; this does not introduce exact physics replay or
resumable midair vortices. Quest gesture feel and device performance require a
headset playtest; no APK is requested for this increment.

## Editor verification

`WhirlwindChecks.Verify` exercises native ball lift/orbit and gravity after
release, paid nonstacking pulses, object reservations, locked/snapped pieces,
pause, invalid points, exhaustion/rearm, the stroke cap, unlimited energy,
selection-click gating, actual palette hold/release, tracking loss and valid
player-attributed saved evidence. Wind, Rain and God Tools checks also pass.
Rendered palette and active vortex views were reviewed. Air ribbons use 192
positions total and add no particles. These controlled Editor checks do not
measure natural full-village behavior or standalone Quest frame time.
