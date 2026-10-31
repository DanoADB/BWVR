"""Blender --background --python tools/prepare-cathedral-bell.py.
Convert the retained CC0 model and synthesize an original mono bell tone.
"""
import array
import math
from pathlib import Path
import wave
import bpy

root = Path(__file__).resolve().parents[1]
bpy.ops.object.select_all(action='SELECT')
bpy.ops.object.delete(use_global=False)
bpy.ops.import_scene.gltf(filepath=str(root/'ArtSources/CathedralBell/bell.glb'))
bpy.ops.export_scene.fbx(filepath=str(root/'Assets/ThirdParty/Quaternius/CathedralBell/Bell.fbx'),
                         object_types={'MESH'}, add_leaf_bones=False, bake_anim=False,
                         axis_forward='-Z', axis_up='Y')
rate = 22050
seconds = 3.4
samples = []
for i in range(int(rate*seconds)):
    t = i/rate
    tone = sum(amp*math.sin(2*math.pi*220*ratio*t)*math.exp(-decay*t)
               for ratio, amp, decay in [(1, 1, 1.1), (2.706, .45, 1.8),
                                         (3.02, .23, 2.5), (4.11, .12, 3), (5.43, .06, 4)])
    samples.append(tone*min(1, t/.004)*min(1, (seconds-t)/.15))
peak = max(abs(x) for x in samples)
pcm = array.array('h', (round(x/peak*.68*32767) for x in samples))
with wave.open(str(root/'Assets/BWVR/Resources/IslandAudio/bell.wav'), 'wb') as sound:
    sound.setnchannels(1)
    sound.setsampwidth(2)
    sound.setframerate(rate)
    sound.writeframes(pcm.tobytes())
print('BWVR_BELL_SOURCE_READY: CC0 bell FBX and original mono 22050Hz tone')
