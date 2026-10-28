# Villager responses to creature tricks

Nearby villagers can notice a creature spin, pause their ordinary routine, watch in place and applaud its completion. The creature still chooses whether to accept the player's held-beachball circle; see [TOY_SPIN.md](TOY_SPIN.md). There are no new buttons, signs or menus.

## What to try

Make a slow circle with the held beachball near the creature's head in a clear space with villagers nearby. Up to two healthy, available villagers already facing the performance can watch from outside the creature's turning footprint. After watching at least 1.5 simulation seconds and seeing the finish, they clap briefly, show the existing affection icon and remember enjoying a trick. Repeated enjoyment gradually increases their interest and their relationship with the creature. Each villager's 90-second memory cooldown limits rewards.

Busy, frightened, injured, carried, distant or occluded villagers stay out. Spectators do not walk into the turning area or take the player's toy. They use the existing exploration intention and hands-free workload stow, yielding safely to pickup, injury, fright or urgent worship/building activity. Pause freezes both watching and applause. Interrupted turns earn no audience enjoyment. Repeated completion notifications cannot give additional credit. Audience enjoyment does not create or consume a player-training receipt.

## Integration and persistence

`SpinAudience.cs` extends the existing `BallSpin` and `VillagerExploration` systems. The service has two exclusive spectator slots; the established villager motor owns watching/applause. Visibility uses the existing facing and obstruction checks, refreshed at 4 Hz with a fresh check at completion. The resident list is cached at first use, and no scene search runs each frame. Social memory adds bounded `tricksEnjoyed`, `trickInterest` and `nextTrickAt` fields. Historical saves start without invented enjoyment; JSON round-trip and validation cover the new values. Active audience intentions remain transient and are cleaned up on interruption/reset.

## Verification — 2026-10-28

Unity 6000.6.7f1 Editor compile passed. `SpinAudienceChecks.VerifyModel/Verify` cover historical data, saved enjoyment, cooldown, malformed values, two visible available witnesses, duplicate/overflow slot rejection, work-load stow, pause, applause, exactly-once credit, no training receipt, blocked sight, pickup/injury interruption and partial-turn cleanup. The scene fixture deliberately stages actors and offers a pending spin to exercise audience behavior; it does not establish natural recruitment frequency.

Existing `BallSpinChecks` independently verify actual native toy pickup, circle recognition, utility selection, grounded stepping, contextual teaching and input-loss cleanup. Echo-audience, shared-goal, pickup/drop, chase and sanctuary regressions also passed. A staged offscreen render was inspected for presentation. Hardware gesture feel, applause readability, natural audience frequency and Quest performance remain unverified. No APK was built. The user's original save and XR startup settings were restored after testing.

Next playful increment: a simple cooperative toy puzzle with a visible shared outcome.
