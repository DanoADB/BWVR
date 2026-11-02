# Following a player-held beachball

Pick up the beachball with the usual controls, show it in front of an idle creature for about three quarters of a second, then move it slowly over nearby dry ground. The creature can choose to follow using the existing ball-chase activity. It never takes the ball from the player. Release nearby after moving together to finish; A/B feedback (desktop 1/2) can teach its preference for that completed game.

## One activity and one owner

The separate Observe-based path from commit 57519f5 has been consolidated into the established `BallChase` service and `CreatureCuriosity.ChaseBall` executor. There is one chase candidate for a held ball, with the existing utility model, learned preferences, saved play history, affect, audio, navigation and exclusive coordinator ownership. No second controller, new component or frame-level carry-report scan remains.

Normal hand slots and input ownership identify a player-held ball. A visible presentation must be within forty horizontal game units, above its projected surface and no more than 250 vertical units above it. Reachable sampled dry ground provides a stand-off goal. Sight and goals refresh around 4 Hz; the navigator retains route and actor avoidance. Rapid tracking jumps, unseen toys, unsafe ground, hunger and other tasks can prevent acceptance.

A bout lasts at most eight simulation seconds with a ten-second cooldown. Menus, tools, zoom, tracking/input loss, distant/unseen targets, suspension and cancellation withdraw the activity. Ordinary withdrawal and stationary bait do not create a failed-attempt lesson; a genuinely blocked route still uses the existing failure/backoff path. Pause freezes movement and timers.

## Actual play, training and release

A completed game requires at least three seconds, eight game units of actual creature travel, six units of horizontal toy travel, and a nearby recently seen release. Stationary holding and vertical bobbing cannot satisfy the toy-travel requirement. An invitation, approach, canceled attempt or expired bout creates no completion or training receipt.

The normal release handler settles an active chase exactly once before dispatching the ball's placement or measured throw. Eligible play retains its own typed chase receipt and learned preference. A gentle throw can immediately become a separate catch opportunity; it does not count as a caught ball merely because the chase completed. Slow release still uses normal placement. Existing fetch, bowling and catch rules remain authoritative.

Play counts, best distance, enjoyment and trained preference use the existing bounded `ChaseMemory` save format. Hand ownership, warm-up, active movement and motor state are transient. Reload does not replay a held-ball activity. No new art, physics actors, lights, particles, UI or controller bindings are introduced.

## Verification

Unity Editor compilation passed without errors/warnings. Consolidated checks passed: one held-ball candidate and one curiosity owner; actual chase duration and creature/toy travel; native measured throw/release; exactly one chase outcome and contextual feedback; immediate separate catch selection; native Save/Load; stationary-bait rejection; and staged input loss, zoom and timeout without carry loss, reward or failed-attempt learning. Existing slow-release chase, spin, catch, fetch, ball-impact and music-invitation regressions passed. A staged disabled-camera render was inspected and the live Editor frame count advanced. Android player-script compilation passed with 43 assemblies. The original saved season was restored byte-for-byte, desktop/Android XR startup restored, and the Editor left out of Play mode. Fixtures stage reachable actors and constrain competing opportunities; they do not establish natural participation, headset reach, animation quality, comfort or subjective fun. No APK is included; physical Quest validation remains deferred.
