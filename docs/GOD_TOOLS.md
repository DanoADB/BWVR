# Summonable god tools — first playable delivery

The player can summon Help, Protect, Investigate, Rain and Healing without hunting
for the original intention objects around the cistern. Those objects remain usable.
The new interface adapts existing guidance, energy, rain and injury systems.

## Controls

- Quest: intentionally turn the underside of the left controller toward your face
  while looking at it for 0.35 seconds. Point with the right controller and press
  its front/index trigger to select a tool. The wrist gesture remains provisional
  until physical Quest testing; X → God tools is the rotation-free fallback.
- Desktop: T opens/closes the palette. Aim by looking with right mouse; F selects
  and uses a tool. Escape cancels the selected tool. Tab still opens the sanctuary.
- Release the selection trigger, aim into the world, then press it again to use.
  The selected physical symbol remains by the right hand for repeated deliberate
  uses. Green target rings indicate valid targets; red rings indicate invalid ones.
- Open the palette again to change tools, clear the sign, or close. The sanctuary,
  pause, tracking loss and suspend dismiss the transient tool context.

The palette is hidden by default. Left movement, right side-grip zoom and A/B
teaching retain their bindings; tool selection/use owns the interaction context
so it cannot also grab an object, activate a world button or teach the creature.
Carrying either item prevents summoning. Held inputs require release before use.
The palette does not pause the world. No forced camera movement is introduced.

## What to try

Rain costs 35 spiritual energy and targets the cistern or water offering. It uses
the existing localized rain effect and adds 22 water/6 crop condition once per
press. Healing costs 25 and targets an injured villager, removing that injury.
The palette shows available energy and costs. Existing recovery still applies.
This does not turn the rain miracle into a free-target weather or crop-growth brush.

Place an Investigate sign on a reachable dry ground point or an existing world
target. The creature weighs the suggestion against its other interests. Help works
on the cistern/water offering or villagers; Protect requires an injured villager
and asks the creature to attend them, rather than casting healing. Unsupported
requests are rejected before replacing a valid sign. This is not an RTS order.

One imported CC0 wooden sign with an intent pictogram marks the current suggestion.
A replacement supersedes the previous guidance; Clear sign removes the suggestion
and safely ends an associated observation action. Expiration/completion hides the
sign. No player avatar becomes visible to villagers, and the decorative sign does
not obstruct navigation or become a harvestable resource.

## Saves and runtime boundaries

GuidanceState adds optional playerSign/spot fields. Historical saves default to no
placed sign. The existing guidance target, deadline and intent remain authoritative.
A single registered stable point restores a ground sign on reload; invalid or
unreachable saved spots are abandoned. Preview probes are not in actor perception.
Sign positions reject nonfinite data. Energy and injury retain existing persistence.
Only the committed sign persists; palette, tool choice, preview and held presses do
not. New seasons discard the old suggestion through the existing fresh state.

Preview validity is evaluated at 10 Hz and again on application. Mesh icons share
existing meshes/materials; there are no extra lights, physics simulations, packages
or asset downloads. One preview ring and one sign bound world presentation cost.

Terrain editing, Book of Creation, AI/voice companion tools, multi-sign queues and
new miracle types remain queued. Water remains secondary in development focus.

## Validation

Native Play-mode check: BWVR → Verify summonable god tools (Play mode).
Validation results and physical-headset limitations are recorded after the run.

2026-10-26 verification: final GodToolsChecks passed in native Editor Play mode
at frame 2084, including real physics-ray palette selection, press/release rearm,
one rain application per held press, exact energy/resource deltas, healing only
the injured target, invalid/unaffordable/paused rejection, nonmutating previews,
independent guidance ownership, save/restore of ground signs, explicit removal,
right tracking cleanup, left tracking dismissal, pickup exclusion, desktop aiming
anchor and wrist dwell/busy/hysteresis. Run this check from a fresh Play session.

MiracleChecks, HybridChecks.VerifyModel and PopulationChecks also passed during
integration verification. A separate ground-cue check saw the registered point in
the feasible candidates. With building examples already known and competing play
needs low, it completed InvestigateObject on player-guidance-spot with valid state.
With new building examples competing, it instead pursued a bench example and the
cue expired. Do not interpret guidance as guaranteed obedience or interruption of
an unrelated active plan. No decision weights were changed for this feature.

A real game-view render of the five-tool palette was inspected; labels, costs,
energy, symbols and dismiss/remove controls were visible. Live frames advanced
from 2084 to 20534 in the final run. The final console snapshot reported zero
current compilation failures, errors or warnings. Original player save restored
and hash-verified after stopping Play. No APK or physical Quest test was performed;
wrist-axis comfort, reach, readability and device frame time remain unverified.
The older full hybrid water-delivery and DevelopmentFocus state-swap regression
failures from EXPANDED_POPULATION.md remain unresolved; they were not claimed fixed
or rerun as a passing full suite in this interaction-focused update.
