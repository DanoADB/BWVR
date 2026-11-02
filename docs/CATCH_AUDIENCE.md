# Villagers watching catch games

Nearby free villagers can watch a player/creature catch game through the existing ball spectator routine. They turn toward the ball, briefly clap after a sufficiently witnessed catch-and-roll, and then resume their normal routines. This is an extension of local observation and ball confidence, not a command or a guaranteed crowd.

## What counts

A native catch attempt opens one transient observation session. Existing distance, private-physics sight, facing, injury, carry, resource work, household, cathedral and social-task guards determine who can watch. Catch spectators additionally need the idle routine gate used by the gathering bell; working, loaded, requesting and worshipping villagers are not interrupted. The empty water bucket is put aside for a hands-free audience and restored when ordinary routines resume.

A villager must accumulate at least 1.5 seconds of visible, facing observation before the real return-roll launch to clap. Anticipation alone cannot create applause or a lesson. Applause lasts 1.2 simulation seconds, once per witness per session, using existing hand poses and a quiet content cue. Sight loss or arriving late can produce different reactions between villagers. Private guides do not teach anyone.

Applause acknowledges the observed catch and launch. **It does not certify the later roll as safe.** The existing ball outcome window supplies that separate evidence: a completed harm-free creature throw can add a good observation, while a witnessed harmful impact can add a bad one and stop cheering. The existing 12-second learning cooldown prevents repeated rapid bouts farming confidence. A later event outside that observation window is not retroactively known to a former spectator.

Only actual witnesses update their existing `BallLearning` memory, attributed to the creature. Confidence influences the existing peer-play drive and voluntary ball participation; this does not teach villagers to catch or guarantee an imitation. No village-wide faith, morality, resource reward or forced job change is added.

## Ownership, lifetime and saves

The existing spectator phase owns gaze and posture; it does not acquire the ball or add a second actor controller. The audience closes after its outcome or on a new throw, reset, player pickup, wind impulse, failed/interrupted catch, unavailable ball, season change or timeout. The catch's internal grounded handoff preserves the same session. Pause freezes observation, cheering, physics and lessons. Cleanup clears hand poses and returns the ordinary routine, including its bucket.

Audience tickets, applause and unconfirmed outcomes are transient. Existing individual confidence and good/bad observation counts save through `SocialMemory.BallLearning`; old saves need no new schema. Reloading does not replay a crowd or count the same display again. At most the existing resident array is consulted at show start and outcome events, with the existing spectator sight cadence during play. No new meshes, physical actors, materials, lights or particles are introduced.

## Validation

Passed in Unity Editor Play mode: native catch/roll with one clear viewer, a real private-physics occluder, a carried villager and a construction-busy villager; pause, once-only applause, harm-free flight confirmation, a staged harmful-impact receipt, confidence/peer-drive changes, duplicate prevention, cleanup and native Save/Load. The bad-impact fixture exercises the authoritative impact entry point and is not a claim of a naturally occurring hazardous roll. Existing catch ownership/training/save, fetch, ball consequences, rallies and observational-learning checks passed. Disabled-camera review renders were captured and inspected; headset-scale readability remains unverified. Editor compilation and Android player-script compilation passed (43 assemblies); the original season save and XR startup settings were restored. Physical Quest crowd readability, natural participation, performance and subjective enjoyment remain open. No APK is included.
