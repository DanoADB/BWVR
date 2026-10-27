# Gathering-place construction contributors

S4 increment, 2026-10-26. Open the wrist sanctuary, then Book of Creation > Places. Each known gathering site has a use-summary page and a second page naming recent block-placement contributors.

## Recorded behavior

Confirmed physical snaps at the shared yard and two former clearings record the stable site, block index, actor, simulation time and whether this was a villager repair. Native player release, creature construction and individual villager construction/repair supply actor identity at their existing completion boundaries. This does not change action selection, reward, teaching or construction physics. Bridge construction is outside this gathering-site record.

The history retains the latest 48 placements across these sites. Each contributor page shows up to three most recent contributor identities and their counts within that retained history. Repairs are a subset of snaps. These are neither lifetime counts nor unique blocks: a block can be moved and placed again. Existing geometry restored from a save creates no new credit.

Failed placements, anonymous cleanup/cancellation drops, paused operations and unchanged repeated drops of already-attached geometry do not record credit. Old saves load without this optional record; reading the Book does not create it. Values and actor/site/block IDs are validated during save loading, with bounded IDs and chronological simulation timestamps.

Placement does not prove useful construction. Existing visit/comfort memories remain separate evidence of use. Earlier contributors, deliveries, planting and local-rain authorship are not inferred from nearby geometry, accepted requests or general island outcomes. The page explicitly retains unknown-history and retention notices.

## Verification

Unity 6000.6.7f1, FirstQuestSlice, temporary DX11 Editor. Final C# compilation passed without errors or warnings.

- PlaceContributionChecks model checks cover source validity, canonical actor IDs, bounds, duplicate rejection, latest-48 retention, save round trip, old saves and unchanged state under repeated Book projection.
- Runtime checks use actual QuestSlice pickup/release and a physical yard ray for player credit. They also exercise the creature placement boundary, failed/paused/anonymous placement, unchanged attached drops and geometry save/restore without new records.
- VillageBuildingChecks exercises native cooperative villagers and creature construction, confirms their actor records, and retains partial-project save/resume checks. VillagerRepairChecks confirms native completed repair attribution. Fixtures cancel prior resource jobs before claiming work, matching action ownership requirements.
- CreationBookChecks model/runtime, FeedbackChecks, DevelopmentFocusChecks and CreatureCarryChecks pass. Live contributor-page rendering was captured and visually inspected; page controls, read-only navigation, pause/run behavior and pickup protections remain covered. Editor frame progression was observed.

The original user season save was restored with matching SHA256 after testing. Standalone/Android XR startup settings were restored and the temporary Editor closed. No APK or headset result is claimed; wrist readability, comfort and device performance remain physical Quest checks.

## Remaining S4 gate

This completes block-placement attribution, not the whole shared gathering-place milestone. Still demonstrate three different useful player contributions, a declined creature suggestion, villager initiation, autonomous progress, weather-sensitive utility and mixed social outcomes together. Expand other contribution types only when their actual completion and use can be established.
