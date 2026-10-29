# CC0 toy, goal, puzzle and construction art

Integrated 2026-10-29. The gameplay roots, stable IDs, save fields, physics scenes and action controllers remain in place. Sourced meshes supply presentation; invisible colliders and simulation meshes remain technical geometry.

| Use | Source and adaptation |
| --- | --- |
| Large beachball | Kenney Minigolf ball-blue; centered and fitted to the existing seven-unit diameter. Existing striped beachball texture and spherical collision retained. |
| Rhythm instrument | Innerscene Xylophone: four frame pieces and eight existing rosewood bars, decimated to 492 triangles total. Original walnut/redwood textures retained; two separately tappable input groups and existing echo timing retained. |
| Bowling pins | Tomcat94 Bowling Pin, decimated to 343 imported triangles per pin. Original white body/red collar retained, fitted to the existing capsule dimensions; scoring/reset/imitative play retained. |
| Movable goals | Quaternius flat wooden door trim fitted around the existing 20-by-16 opening. Kenney round studs/jewel supply feet, colored cue strip, score badges and turn beads. A Survival floor rotated into a backboard replaces net artwork, matching the existing solid catch collider. |
| Together gate puzzle | Fitted Quaternius trim and original wooden door, Kenney Platformer key/jewel, Minigolf bordered pad and Brick round studs. Player key, matching socket, creature pressure pad, lifted shutter and ball passage retain their existing behavior. |
| Eight construction shapes | Kenney beveled bricks, beam, round brick, plate and slope; fitted Quaternius trim for the arch. Survival floor for tray/foundation artwork. Existing dimensions, ramp direction, physics meshes, snapping, detachable joints and persisted support graph retained. |

The old decorative turning-block toy was already retired; the shared construction yard supplies block play. Physical shapes are deliberately retained for compatibility. Brick bevels/studs and the door/frame surface detail are visual; they do not introduce new collision teeth or change placement rules. Imported art prefabs contain no AI, colliders, rigidbodies or carry controllers.

## Sources and licenses

All new models in this batch are CC0-1.0. This does not change the licenses of the purchased gorilla, original terrain, or existing attributed UI icons.

- [Kenney Minigolf Kit](https://kenney.nl/assets/minigolf-kit): selected original FBX, palette and bundled license in Assets/ThirdParty/KenneyPlay.
- [Kenney Platformer Kit](https://kenney.nl/assets/platformer-kit): key/jewel, palette and bundled license in Assets/ThirdParty/KenneyPlatformer.
- [Kenney Brick Kit](https://kenney.nl/assets/brick-kit): selected originals and license in Assets/ThirdParty/Kenney/ConstructionBlocks; round brick, slope and plate added to the existing 2x2/2x6 sources.
- [Kenney Survival Kit](https://kenney.nl/assets/survival-kit): structure-floor added to the existing source folder/license.
- [Quaternius Medieval Village](https://quaternius.com/packs/medievalvillage.html): DoorFrame_Flat_WoodDark and Door_1_Flat from the downloaded Standard archive. Its bundled License_Standard.txt explicitly states CC0.
- [Innerscene Xylophone](https://www.innerscene.com/tools/library/3d-parts/xylophone-5da5d32e): original GLB and source/license record retained in ArtSources/Play.
- [Tomcat94 Bowling Pin](https://opengameart.org/content/bowling-pin): original Blender file and source/license record retained in ArtSources/Play.

ArtSources/Play/SOURCES.json records the source pages, authors, selected filenames and SHA256 hashes. No signed download URLs or paid source files are included.

## Rebuild

1. Run Blender 5.1 in background with --python tools/prepare-play-art.py from the repository. It uses retained originals, fits existing frame vertices, extracts the original instrument textures and exports selected/decimated FBX. No primitive art is authored.
2. In Unity Edit mode use BWVR > Art > Rebuild sourced play artwork, or call BWVR.Editor.PlayArtImporter.Build(). It normalizes centered mesh bounds, retains material slots/UVs, orients the slope to the existing physics ramp and creates Resources/PlayArt prefabs. Save materials before each subsequent source reimport.
3. Recheck rendered proportions, frame openings, slope direction, chime rays and existing interaction suites before committing regenerated assets.

## Validation

Unity 6000.6.7f1 desktop Play mode checks passed: physical chime rays and exact creature echo; cooperative key/pad/ball solve and cancellation; physical goal scoring and witnessed imitation; shared goal turns; bowling, reset and creature imitation; native ball pickup/drop; spin/chase; all eight construction types, gravity, snapping, creature arch assembly, stability, save/resume, detach and bridge use. Source prefabs were checked for centered unit bounds, provenance and absence of art-owned colliders. Model-sheet and runtime visual reviews caught and corrected source-material suffixes and ramp orientation.

Quest headset readability, controller hand poses and device frame time remain unverified. No APK was built for this art batch. User save and XR initialization settings are restored after QA.
