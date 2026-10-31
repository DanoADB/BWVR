# Lightning — charge, aim, release

Select Lightning from the God Tools scroll. Point at a standing tree or building,
hold the right front trigger (desktop F) until the electric halo fills and the
ready chime sounds, then release on that same target. Charging takes 1.2 seconds
of simulation time. A successful strike costs 20 energy and has a three-second
cooldown. The current unlimited-energy testing setting still applies.

A short tap does nothing. Retargeting starts a fresh charge; losing aim cancels
it. Tracking loss, menus, pause, suspension, switching tools and an eight-second
hold timeout clear the charge. There is no charge stored in the season save.
The selection click must be released before casting. A held trigger never
repeats a strike and repeated release frames cannot spend twice.

The existing IslandWeather strike supplies the bolt, spatial thunder, heat and
ignition. Dry fuel can catch fire; soaked material heats but resists ignition.
The existing material tick consumes fuel, damages and spreads fire. Existing
rain/dousing cools and extinguishes it. Existing local villager fire reactions
remain in effect; this does not add a new global firefighter assignment system.

Targets must be live registered scenery: no held/uprooted/harvested tree,
destroyed scenery, unregistered proxy or villager. Already burning targets and
saturated fire/material pools reject a cast. Preview and charging do not spend
energy, initialize target material records or change fire state. The completed
strike records player-miracle evidence and invalidates creature teaching, so
player lightning is not attributed to the creature. Existing fire and evidence
save data retain the consequences; a held charge is not replayed on reload.

The stone tool uses Lorc's unchanged Power lightning icon, CC BY 3.0:
https://game-icons.net/1x1/lorc/power-lightning.html
Source/license are included under Resources/GodToolIcons. The icon imports at
256px with transparency and mipmaps. The charge halo is a bounded 48-point
presentation effect; no new physics scene, light or particle pool is added.

Quest control feel, readability, sound and standalone performance remain
hardware playtest gates. No APK is requested for this increment.

## Verification

`LightningChecks.Verify` exercises actual charge/release and material changes:
short tap, retargeting, pause, cooldown, duplicate release, charge timeout,
tracking loss, real palette selection/hold/release, dry ignition, consumed fuel
and damage, rain extinguishing, wet resistance, finite/unlimited energy,
registered scenery-only targets and player evidence. Native Save/Load retains
wetness and cast evidence while discarding an in-flight charge. Existing Rain,
Wind, Whirlwind and God Tools checks pass together. Palette, charged halo and
released bolt views were rendered for visual review; the halo is placed above
the visible mesh bounds so it clears tree crowns.
