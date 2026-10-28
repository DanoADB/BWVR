# Villagers join player-led music

Play four notes on the chime while the creature watches, using the existing right front trigger (desktop F). Up to two nearby, free villagers can notice the opportunity, walk to separate listening spots, and clap on the creature's echo. A completed, witnessed performance can give them a happy reaction and strengthen their enjoyment of music, trust in the creature, and bonds with another listener.

This extends [player rhythm echoes](PLAYER_RHYTHM_ECHO.md); no new player controls, menus, artwork or jobs. The player stays unseen and the creature remains the performer.

## Joining and reacting

- Villagers must be healthy, unheld, recently unfrightened, free of competing activities, within 24 game units, and able to see the chime. Existing music interest/boldness and a music cooldown affect willingness. Critical water work outside development focus takes precedence.
- Two bounded reservations provide reachable, separated spots away from the creature's approach. Each villager uses its existing exploration motor and actor avoidance, not another AI controller. Recording an ignored phrase can attract a small audience, but expiration grants no visit or sentiment reward.
- Routine loads are stowed using the existing hands-free activity hook. Beats close/open the hands through Humanoid IK plus the same after-animation arm adjustment used for carrying. Pause freezes the beat pulse and movement; cleanup clears the pose and restores routine presentation.
- A listener must arrive and witness at least two actual creature notes, then still see a successfully completed echo. Proximity alone is insufficient. Pickup, interruption, expired/superseded invitations and target loss release reservations without completion credit. No late recruitment after the second echoed note.
- Existing rain/shelter comfort affects the experience: exposed rain may produce disappointment instead of enjoyment. Only comfortable co-listeners can gain positive peer and creature bonds. Existing bounded memories and cooldowns prevent repeated credit.

## Persistence and integration

`EchoAudience` extends the existing PlayerRhythm and VillagerExploration classes. The latter adds one activity state with existing scheduler ownership. PlayerRhythm caches nearby actors once and looks for listeners at approximately 2 Hz; listener visibility refreshes at approximately 4 Hz, with fresh checks for meaningful beats/outcomes. No scene scans every rendered frame, extra physics scene or decorative bodies.

Saved state reuses each villager's music visits, taste, trust and peer memories. Echo history adds a bounded audience-visit total, capped at two visits per completed echo; historical saves default to zero. Temporary spots, clapping and audience sessions are discarded on reload. Spectators remain separate from the creature's typed echo/training receipt and are never credited as instrument performers. Existing Book music-visit totals reflect the new visits without a new HUD.

## Verification

Editor model/runtime checks passed for legacy saves, visit bounds/cooldowns, two visible healthy villagers routing to distinct spots, hands-free clapping, pause, once-only music/sentiment/peer/creature memories and save round trip. Occluded/injured actors were excluded; picking up a listener freed its slot; chime pickup and invitation expiry cleared the audience without rewards. The fixture stages positions, needs and other opportunities to isolate the feature; it does not establish unattended attendance frequency.

Close-up Unity renders exposed an unstowed routine bucket and inconsistent native IK. The existing hands-free hook and post-animation arm adjustment corrected them. The final render check verifies hand approach near the clap peak and inspects the actual pose. Existing player echo, villager duet, chase, sanctuary, native pickup/drop and fetch checks pass after those corrections. Compilation reports zero errors/warnings.

Physical Quest readability, animation feel and performance remain unverified. No APK was built. The original saved season and XR startup settings are restored after verification. Water and food work remain deprioritized in development focus. The next playful candidates are player-built physical challenges and further creature tricks; headset testing of the current activities remains pending.
