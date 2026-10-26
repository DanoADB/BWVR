# Summonable tools and the underside-wrist palette

Roadmap addition, 2026-10-26. Planned, not implemented. The proposed entry gesture
is rotating the left controller to look at the underside of the wrist. Confirm
comfort and tracking on Quest Pro/3 before treating that gesture as final.

## Interaction and presentation

The palette starts hidden. An intentional underside-wrist reveal brings a small
ring or fan of recognizable physical tool silhouettes into reach. Use a brief,
tunable dwell and separate open/close orientation thresholds to avoid flicker or
accidental opening during normal movement. Looking away, returning the wrist or
an explicit dismiss closes it. Keep the existing left menu/X sanctuary entry as a
fallback; provide a desktop equivalent and an alternative reveal for limited
wrist rotation. No forced head movement, full-screen HUD or permanent wrist menu.

The player points to or reaches for a tool, summons it into the free hand, then
uses it in the world. Keep one active tool and one transient placement preview.
Show valid/invalid targets through shape, color and light haptics; minimal text
belongs in optional Help/details. Tool selection and world application are
separate intentional actions. Revealing the palette never casts, places a sign,
edits terrain, spends energy or pauses the simulation.

## Tool families

| Tool | Intended outcome | Integration / prerequisites |
| --- | --- | --- |
| Guidance signs | Place or move a clear pictogram at a particular spot to draw the creature toward helping, investigating, protecting or a supported activity such as building/playing there. | Reuse guidance, affordances, navigation and exclusive action ownership. Signs suggest an outcome; capability, needs and learned preferences still determine the creature's response. No guaranteed RTS orders or villager command panel. |
| Terrain tools | Raise, lower, smooth or flatten a bounded patch with a physical brush and visible footprint/height preview. | Design costs and limits; update rendered terrain, ground collision, navigation and grounded actors together before committing a change. Persistent edits need an explicit bounded save representation. |
| Miracles | Summon a contextual miracle tool, aim at a valid place/person and cast with visible effects and an explicit energy cost. | Adapt existing rain/healing and divine-energy services. The palette is a new access method, not a second miracle/energy system. New miracles remain separate capabilities. |
| Book of Creation | Summon and open the physical book; close it and return naturally to the world. | The book itself remains planned. Start with read-only creature, villager and provision pages grounded in actual state; see BOOK_OF_CREATION.md. Keep the sanctuary entry too. |
| Spirit companion | Summon/dismiss the private orb, inspect contextual help or let it present the selected tool. | Optional presentation link to the existing local companion. No external agent, voice or autonomous code changes are required to use any tool. |

## Guidance signs must be understandable

Use the imported wooden sign models with distinct activity/intent pictograms,
not unexplained colored posts. Separate player-only placement previews from
committed world affordances that the creature can perceive. A placed sign is an
object/cue, not a visible player avatar; villagers continue to regard the creature
as their god. Decide sign visibility to villagers deliberately rather than giving
all actors automatic knowledge of player intentions.

A sign's target point must be reachable and appropriate for its activity. Give the
creature a brief gaze/affect acknowledgment; optional details can explain occupied,
unreachable, unavailable or unsupported requests. Reuse the existing temporary
guidance slot for the first delivery: a new request supersedes the old one safely.
Moving/removing a sign or expiration clears the suggestion without corrupting an
owned action. Persistent multi-sign task queues are later work, not this first step.
Do not advertise a sign activity until an executor and meaningful outcome exist.

## Input ownership and comfort

Audit QuestControls/QuestSlice bindings before assigning selection/application.
Preserve stick locomotion, right side-grip zoom, right front-trigger pickup,
A/B feedback and their desktop counterparts. Do not reveal over an active carry,
zoom, world interaction or tracking loss. An open palette or held tool explicitly
owns its input context; pointing/selecting cannot simultaneously pick up a world
object, praise the creature or cast a miracle. Require release/rearm after context
changes so a held trigger cannot apply an action twice. Existing carried objects
and action reservations must never be silently stolen by summoning a tool.

Tracking loss, suspension, closing, tool switching and canceled placement clear
previews and transient ownership without spending resources or applying changes.
Resuming does not replay an old input. Respect simulation pause and use explicit
save/pause actions; opening the palette or book does not silently freeze actors.
Scale placement, icon readability and reach with the current player zoom.

## Terrain-specific acceptance gate

Start with a small brush on clear ground away from buildings, stores, crops,
construction, rooted trees and actors. Reject unsupported or unsafe edits; do not
leave meshes, colliders and navigation disagreeing or actors floating underground.
Keep slope/height bounds, a bounded edit budget and an explicit confirm/undo design.
Validate paths and affected object grounding before committing the edit; a failed
edit restores the prior geometry and spends nothing. Avoid full-island mesh/nav
rebuilds per controller frame. Quest frame cost, edit persistence and old-save
migration must be proven before terrain editing is called playable.

## Companion connection

An optional visual relationship can make the palette feel like the spirit's
collection: the orb appears by the underside wrist, briefly presents the selected
tool, then gets out of the way. Orb color/shape can acknowledge selection, invalid
targets or insufficient energy without text. The orb remains private to the player
and has no villager/creature perception or physics role.

This does not resume the deferred AI-companion/development-bridge work. If voice or
connected assistance is added later, it should invoke the same validated tool
contracts and respect costs, target feasibility and input ownership; it must not
become a parallel path that bypasses gameplay rules.

## Delivery sequence and validation

1. Keep the current food-priority/balance follow-up ahead of this new interface.
   Then audit controls and prototype the hidden wrist reveal, selection, dismissal,
   accessibility fallback and exclusive input contexts on desktop and Quest.
2. Add physical guidance signs for supported creature activities; prove autonomous
   acknowledgment, safe replacement/cancel and correct placement at a target spot.
3. Add miracle selection over the existing energy/effect pipeline, plus a Book
   shortcut when the first physical read-only book pages are built. The palette
   shell may reserve those slots but must not present them as working tools early.
4. Implement a narrowly bounded terrain brush only after its geometry, collision,
   navigation, undo and save contracts are ready.
5. Optionally connect the orb's local presentation to tool summoning. Connected AI
   and voice remain deferred until separately prioritized.

Validate accidental wrist turns, dwell/hysteresis, zoom scaling, locomotion/carry/
feedback conflicts, held-input rearm, invalid targets, pause/suspend/tracking loss,
old/new saves and hardware readability/reach/frame time. Guidance must remain a
bias rather than overriding creature learning; miracles charge exactly once;
terrain edits are atomic; Book views show actual state. No runtime feature is
claimed by adding this roadmap document.

## First delivery — 2026-10-26

The hidden palette, intentional wrist reveal, sanctuary/desktop fallback, exclusive
selection/use context, physical tool view, validity preview, energy display,
rain/healing adapters and one persistent guidance sign are implemented. See
[GOD_TOOLS.md](GOD_TOOLS.md) for actual controls, supported targets and verification.
This supersedes the planned status for those features only. Terrain brushes, Book,
AI/voice, multiple signs and physical wrist-comfort validation remain outstanding.
The user explicitly moved interactive player functionality ahead of food tuning.
