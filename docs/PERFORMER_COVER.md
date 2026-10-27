# Sheltered chime performers

At a familiar gathering place, a villager inviting the creature to a chime duet now prefers a nearby safe roof when rain is at least 0.5. Arrange the chime beside a supported canopy, close enough that the performer remains within reach of the bars. The instrument stays where the player placed it; the villager walks to cover using the existing social motor.

Cover must be within 4.5 navigation units of the chime, grounded and braced, tall enough, reachable, clear of bodies, and in an unlocked/unpaused site with no visitor, requester or repairer. A short clear lane between the instrument and covered spot is required. Candidates use the existing construction geometry checks. When no suitable cover exists, the previous exposed performance position remains available; dry weather preserves the previous choice.

The social controller borrows the existing site's Visitor lease using its own VillagerExploration identity. That prevents a resting villager or audience shelter claim from taking the performer's roof during approach or play. It does not start the exploration controller or add a movement owner. Normal completion, pickup, moved instrument, unanswered invitation, interruption and disable release the lease through social ResetTask. Roof validity is rechecked at 4 Hz. Losing a roof releases the lease and leaves the performer at its actual position; a completed exposed performance retains the existing uncomfortable rainy place memory.

The creature still chooses whether to accept the invitation. No utility weights, hunger priorities, water/food development settings or guaranteed commands were added. No new save fields, UI or controller bindings were introduced.

## Verification — 2026-10-27

Compilation passed with no reported errors or warnings. `PerformerCoverChecks.Verify`, `VerifyRemoval`, `VerifyDry` and `VerifyOccupied` passed through the existing native invitation/approach and eight-beat duet fixture. They check covered arrival, exclusion of other shelter users, occupied-cover preservation, actual weather-sensitive place memories and released reservations. The base fixture also exercises player pickup, moving the instrument, pause, unanswered-invitation timeout and valid saved state.

`MusicCoverChecks.Verify` passed: the same performance can still yield separate covered/exposed audience outcomes. The five-minute whole-scheduler regression passed with 12 villagers, five independent bench uses/visitors, and four creature completions across four categories, plus pause and valid save roundtrip.

These are distinct evidence levels. The duet fixtures begin a villager invitation and temporarily isolate the chime candidate; they prove execution and cleanup, not its natural occurrence rate. An exploratory 900-second normal-scheduler run before the change produced 19 bench uses and two chime invitations but no completed duet. The full combined three-contribution scenario, natural music pacing and physical Quest checks remain open. Do not mark S4 complete from these checks alone.
