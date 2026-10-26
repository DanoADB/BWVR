# Practical tools, technology and discovery

First capability slice, 2026-10-25. Extend the existing resource workers and personal memories; no separate AI framework or player research menu.

## Two useful designs

Each of the three stable adult villagers keeps personal practice, knowledge and owned equipment. A third confirmed timber-felling delivery reveals a resin-bound axe design. A third confirmed wheat harvest delivery reveals a cultivating hoe design. Carrying already-felled timber, resin gathering, failed/interrupted work and undelivered cargo do not fabricate these lessons. Counts remain bounded and persist across save/resume. These are authored capabilities discovered through practical outcomes, not claims of unrestricted invention.

A villager who knows a design can choose to craft it at the existing covered store. Both tools cost two real timber; the bound axe costs two resin, the hoe one. One crafter owns the work space at a time. The worker carries a visible timber bundle, works for 12 unpaused simulation seconds, and briefly shows the finished implement. Inputs are reserved during the trip/work and charged once on successful completion. Canceling or loading an unfinished job grants no tool and spends no resources. A completed tool survives cancellation of its final celebration. At most six owned tools exist: one of each type per adult.

Nearby villagers can learn a design by observing a confirmed craft, provided they are on the ground, within 18 game units, facing the crafter and have a clear navigation sight segment to the work area. Watching teaches knowledge, not ownership, fabricated practice or free materials. Knowledge remains personal; there is no instantaneous village-wide unlock. Diagnostic insight/craft/observation receipts use the existing bounded journal and never become creature-training completions.

## Physical benefits

- Resin-bound axe: living-tree felling takes 5.5 rather than 8 seconds. An amber binding appears on the worker's existing axe.
- Cultivating hoe: wheat harvesting takes 3.5 rather than 5 seconds; sowing takes 9 rather than 12; preparing a new farm takes 13.5 rather than 18. A distinct long-handled implement appears during the work.

Yields, cargo capacity, growth rate and inventory accounting stay unchanged. Props share materials, have no blocking colliders or Rigidbodies, and use existing work/carry animations. No new paid art or packages are required.

Food emergencies and owned actions take precedence. The idle scheduler tries reserve work before optional toolmaking. Tool costs compete with storage and farm timber through the same spendable-stock checks. Basic work remains available before discovery or when no resin is in stock. Water remains secondary in development focus.

## Boundaries

This delivery establishes practical discovery, resource-funded crafting, a visible capability change and local social transmission. It does not add a full technology tree, autonomous arbitrary recipe generation, trade, durability, tool theft, creature tool use, children crafting, or the Book of Creation interface. Those can build on this saved foundation. The creature's current curiosity and feedback systems remain separate.

## Verification

`ToolTechnologyChecks` passed model and native checks for personal practice, exactly-once costs, canceled/paused crafting, observation without free ownership, exclusion of held observers, delivered-only learning, native hoe/axe speed benefits and valid saved ownership. The farm expansion, food reserve, farming, native resource and actor movement checks passed, as did all fourteen broader gameplay regressions.

In an unattended one-hour clear-weather run from 18 grain/18 timber, two villagers independently learned and crafted hoes, spending four timber and two resin. No axe design or observational transmission occurred naturally during that run; both were verified separately in the staged native checks. State remained valid, 89 deliveries completed, and stores ended with 22.95 grain. Meal-ready stocks were empty for 45.7 seconds after the initial five-minute warm-up, so this does not establish a balanced economy. This is a single observed run, not a guarantee for other seeds or weather.

Observer acknowledgment gestures are limited to idle villagers so knowledge transmission cannot replace an active carrying animation. The working implement is hidden immediately when the worker switches to carrying harvested cargo. No APK or physical headset validation was requested.

The final native tool checks passed after the carrying-animation safeguards. Staged working-pose previews of the hoe and bound axe were inspected. The live scene advanced to frame 2209 with zero current console errors and two warnings. The original player save was restored and hash-verified after testing.
