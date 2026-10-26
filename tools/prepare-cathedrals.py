"""Assemble five CC0 cathedral stages. Run with Blender --background --python tools/prepare-cathedrals.py -- <kit folder>.
Authoring units are SigilsVault's eight-unit modules. Unity applies one shared 0.6 fit.
"""
import bpy, math, sys, json, shutil, hashlib
from pathlib import Path
from mathutils import Vector, Matrix
root=Path(__file__).resolve().parents[1]
source=Path(sys.argv[sys.argv.index('--')+1])
art=root/'ArtSources/Cathedrals';out=root/'Assets/ThirdParty/SigilsVault/Cathedrals'
art.mkdir(parents=True,exist_ok=True);out.mkdir(parents=True,exist_ok=True)
for f in ['LICENSE.txt','README.txt']:shutil.copy2(source/f,art/f)
bpy.ops.object.select_all(action='SELECT');bpy.ops.object.delete(use_global=False)
cache={};used={};manifest=[]
def piece(key,x=0,y=0,z=0,angle=0,scale=(1,1,1)):
 if key not in cache:
  paths=list(source.rglob(key+'.glb'));p=paths[0]
  before=set(bpy.data.objects);bpy.ops.import_scene.gltf(filepath=str(p));objects=[o for o in bpy.data.objects if o not in before and o.type=='MESH']
  cache[key]=objects
  for o in objects:o.hide_render=True;o.hide_set(True)
  shutil.copy2(p,art/p.name);used[p.name]=hashlib.sha256(p.read_bytes()).hexdigest()
 for o in cache[key]:
  n=o.copy();n.data=o.data.copy();bpy.context.collection.objects.link(n);n.hide_render=False;n.hide_set(False)
  # Bake imported transforms first; GLB models arrive in Blender Z-up coordinates.
  n.data.transform(o.matrix_world);n.matrix_world=Matrix.Identity(4);n.scale=scale;n.rotation_mode='XYZ';n.rotation_euler=(0,0,angle);n.location=(x,y,z)
  parts.append(n)
def hall(width,length,rose=False):
 # Width is 8 or 24. Front door remains the same size through every upgrade.
 for x in range(-width//2,width//2,8):
  piece('doorway_01' if x==-4 else 'wall_rose_window_01' if rose else 'wall_window_01',x,-length/2)
  piece('wall_window_01',x+8,length/2,angle=math.pi)
 for y in range(-length//2,length//2,8):
  piece('wall_window_large_01',width/2,y,angle=math.pi/2)
  piece('wall_window_large_01',-width/2,y+8,angle=-math.pi/2)
 for y in range(-length//2+4,length//2,8):piece('rooftop_01',0,y,16,scale=(width/24,1,width/24))
 # Closed gable ends reuse the kit's stone trim; no view through the roof.
 for y in [-length/2,length/2]:
  mesh=bpy.data.meshes.new('Stone gable');mesh.from_pydata([(-width/2,y,16),(width/2,y,16),(0,y,16+14.2073*width/24)],[],[(0,1,2)]);mesh.materials.append(bpy.data.materials['trim sheet']);uv=mesh.uv_layers.new()
  for loop,coord in zip(uv.data,[(.05,.12),(.95,.12),(.5,.35)]):loop.uv=coord
  cap=bpy.data.objects.new('Stone gable',mesh);bpy.context.collection.objects.link(cap);parts.append(cap)
 for x in range(-width//2+4,width//2,8):
  for y in range(-length//2+4,length//2,8):piece('floor_01',x,y,.05)
 piece('door_01',-4,-length/2-.05,scale=(1,1,1));piece('door_01',4,-length/2-.05,angle=math.pi)
 piece('altar_01',0,length/2-4)
def tower(x,y,height=32):
 piece('tower_base_01',x+4,y-4)
 piece('tower_cap_01',x+4,y-4,height)
for stage,name in enumerate(['Shrine','Chapel','VillageChurch','GothicChurch','GrandCathedral'],1):
 parts=[]
 if stage==1:
  piece('pedestal_01');piece('altar_01',0,0,1)
  for x in [-3,3]:piece('column_small_01',x,2,1,scale=(.3,.3,.6))
  piece('rooftop_01',0,2,6.4,scale=(.32,.6,.28))
 elif stage==2:hall(8,16)
 elif stage==3:
  hall(24,24,True);tower(-8,-8,32)
 elif stage==4:
  hall(24,32,True)
  for x in [-16,16]:tower(x,-10)
  for y in [0,8]:
   piece('flying_buttress_01',-12,y,scale=(-1,1,1));piece('flying_buttress_01',12,y)
 else:
  hall(24,48,True)
  for x in [-16,16]:tower(x,-18)
  for y in [-4,8,20]:
   piece('flying_buttress_01',-12,y,scale=(-1,1,1));piece('flying_buttress_01',12,y)
  # The rear sanctuary expands the silhouette without introducing interior gameplay.
  piece('sanctum_base_01',0,20,scale=(.65,.65,.65))
  piece('sanctum_upper_01',0,20,15.6,scale=(.65,.65,.65))
  piece('sanctum_roof_01',0,20,36.2,scale=(.65,.65,.65))
  piece('spire_01',0,11,30,scale=(.65,.65,.65))
 bpy.ops.object.select_all(action='DESELECT')
 for o in parts:o.select_set(True)
 bpy.context.view_layer.objects.active=parts[0];bpy.ops.object.convert(target='MESH');bpy.ops.object.join();model=bpy.context.object
 # Merge duplicate source material slots to the two shared atlases.
 for mat in list(model.data.materials):
  if mat:
   canonical=mat.name.split('.')[0]

   base=bpy.data.materials.get(canonical)
   for slot in model.material_slots:
    if slot.material==mat:slot.material=base
 bpy.ops.object.transform_apply(location=True,rotation=True,scale=True)
 tris=sum(len(p.vertices)-2 for p in model.data.polygons)
 model.name=name
 bpy.ops.export_scene.fbx(filepath=str(out/(name+'.fbx')),use_selection=True,object_types={'MESH'},add_leaf_bones=False,bake_anim=False,axis_forward='-Z',axis_up='Y',use_mesh_modifiers=True)
 manifest.append(dict(stage=stage,name=name,triangles=tris))
 bpy.data.objects.remove(model,do_unlink=True)
for texture in ['trim_sheet_512x512.png','window_diffuse_512x512.png','door_diffuse_512x512.png']:
 p=next(source.rglob(texture));shutil.copy2(p,out/p.name)
(art/'manifest.json').write_text(json.dumps(dict(source='https://sigilsvault.itch.io/modular-cathedral-kit',license='CC0-1.0',author='Kevin Barany / SigilsVault',files=used,stages=manifest),indent=2)+'\n')
print(json.dumps(manifest))
