# Following a player-held beachball

Pick up the beachball with the normal controls, hold it in front of an idle creature, and slowly move it over nearby dry ground. The creature may investigate and follow while looking at the toy. The player retains the ball throughout; release or toss it to return to the existing placed-ball, fetch and catch activities.

This is a short candidate in the existing utility chooser, executed by its existing Observe movement owner. It is not a guaranteed command, a second AI controller, an automatic grab or a new learned trick. Other feasible interests, hunger and critical needs still matter. A nearby toy does not interrupt an already owned action.

## Reach, ownership and timing

The normal hand/controller loop reports only a beachball actually in the player's carry slots. Menus, active god tools, zoom, lost tracking and application suspension withdraw that input. A held villager, tree, resource or other prop does not offer this activity. Reports do not select or begin a motor action directly.

Perception requires the creature to see the ball in its forward field, within forty horizontal game units. The ball must be above the projected ground and within 250 vertical game units; this allows an elevated hand without treating it as a destination in the air. A dry, sampled and reachable ground point nine game units short of the ball's horizontal position supplies the route. Within ten units, it can stay and watch. Navigation and actor avoidance remain authoritative; no straight-line teleport or terrain mutation is used.

The candidate uses ordinary interest, contextual preferences and bounded utility. During execution, sight/ground/reach are refreshed at about 4 Hz, and the existing navigator handles movement and collisions. Following ends after eight simulation seconds, on a blocked route, stale report, lost sight/reach, release, ownership/input loss or interruption. A twelve-second follow cooldown leaves room for other activities. Pause freezes execution; suspension and player pickup of the creature use existing cleanup.

An ended follow is an interrupted investigation, not a confirmed completed play action. It awards no catch practice, faith, resources, object discoveries or training receipt and creates no target-failure learning. Releasing a real toss withdraws following before decision selection so the normal catch/fetch opportunities can be considered. Actual later activities retain their own requirements and outcomes.

## Persistence, cost and validation

Carry reports, routes, cooldown and current follow flags are transient; no save schema is added. A reload has no saved player hand or follow motor to replay. The existing bounded action journal/evidence records can explain an attempted investigation without claiming a learned or completed behavior. No additional art, colliders, rigidbodies, particles or scene-wide frame scans are introduced.

Unity Editor compilation passed without errors or warnings. `HeldBallChecks.Verify` passed native pickup and utility choice, actual routed movement toward a moving held toy, exclusive player ownership, pause, input/report loss, stale reports, duration/cooldown, no fabricated completion, unrelated-owner preservation and native Save/Load. A staged native toss after following was selected by the existing catch motor. A real blocking collider made the held cue invisible and infeasible; a disabled-camera render was inspected and the live Editor frame count was confirmed advancing. Existing catch, fetch, physical ball and music-invitation regressions passed. Android player-script compilation passed with 43 assemblies. The original season save was restored byte-for-byte and desktop/Android XR startup settings restored; the Editor was left out of Play mode.

The fixture stages a visible dry lane, constrains competing opportunities and separates repeated bouts after the creature resumes other activities. It verifies the interaction boundaries, not natural participation rates or unrestricted island navigation. Physical Quest reach, motion/readability and enjoyment remain unverified; no APK is included.
