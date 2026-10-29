"""Convert selected CC0 source geometry with Blender; no primitive meshes are authored.
Run: blender --background --python tools/prepare-play-art.py
Original downloads and license/source records live in ArtSources/Play.
"""
import bpy, math, json, struct
from pathlib import Path
from mathutils import Vector, Matrix

ROOT=Path(__file__).resolve().parents[1]
OUT=ROOT/'Assets/ThirdParty/PlayDerivatives/Models'
OUT.mkdir(parents=True,exist_ok=True)

# Retain the original CC0 walnut/redwood textures embedded in the GLB.
source=(ROOT/'ArtSources/Play/Xylophone.glb').read_bytes()
length=struct.unpack_from('<I',source,12)[0]
document=json.loads(source[20:20+length]);binary=source[28+length:]
textures=OUT.parent/'Textures';textures.mkdir(exist_ok=True)
for label,index in [('music-walnut',1),('music-redwood',5)]:
    image=document['images'][document['textures'][index]['source']]
    view=document['bufferViews'][image['bufferView']];offset=view.get('byteOffset',0)
    (textures/(label+'.png')).write_bytes(binary[offset:offset+view['byteLength']])


def clear():
    bpy.ops.object.select_all(action='SELECT');bpy.ops.object.delete(use_global=False)

def export(name, objects, simplify=False):
    bpy.ops.object.select_all(action='DESELECT')
    for obj in objects:
        obj.select_set(True);bpy.context.view_layer.objects.active=obj
        bpy.ops.object.transform_apply(location=False,rotation=False,scale=True)
        if simplify and len(obj.data.polygons)>80:
            mod=obj.modifiers.new('Quest source simplification','DECIMATE');mod.ratio=.22
            bpy.ops.object.modifier_apply(modifier=mod.name)
    bpy.context.view_layer.objects.active=objects[0]
    bpy.ops.object.join();obj=bpy.context.object;obj.name=name
    bpy.ops.export_scene.fbx(filepath=str(OUT/(name+'.fbx')),use_selection=True,object_types={'MESH'},add_leaf_bones=False,bake_anim=False,axis_forward='-Z',axis_up='Y')
    print('BWVR_PLAY_EXPORT',name,'triangles',sum(len(p.vertices)-2 for p in obj.data.polygons))

# Fit authored rectangular trim around the unchanged logical/physical openings.
for name,width,height,opening_w,opening_h in [('goal-frame',24,18,20,16),('puzzle-frame',28,20,21,18),('block-arch',6,3.6,2,2.4)]:
    clear();bpy.ops.import_scene.fbx(filepath=str(ROOT/'Assets/ThirdParty/Quaternius/Village/Models/DoorFrame_Flat_WoodDark.fbx'))
    objs=[o for o in bpy.data.objects if o.type=='MESH']
    for o in objs:
        o.data.transform(o.matrix_world);o.matrix_world=Matrix.Identity(4)
        vs=[v.co.copy() for v in o.data.vertices];inner=min(abs(v.x) for v in vs)
        outer_l=max(-v.x for v in vs);outer_r=max(v.x for v in vs)
        floor=min(v.z for v in vs);top=max(v.z for v in vs);lintel=min(v.z for v in vs if v.z>1.5)
        front=min(v.y for v in vs);back=max(v.y for v in vs)
        for vertex in o.data.vertices:
            x,y,z=vertex.co;outer=outer_r if x>=0 else outer_l;a=abs(x)
            px=a/inner*opening_w*.5 if a<=inner else opening_w*.5+(a-inner)/(outer-inner)*(width-opening_w)*.5
            pz=(z-floor)/(lintel-floor)*opening_h if z<=lintel else opening_h+(z-lintel)/(top-lintel)*(height-opening_h)
            vertex.co=(math.copysign(px,x)/width,(y-front)/(back-front)-.5,pz/height-.5)
    export(name,objs)

clear();bpy.ops.import_scene.gltf(filepath=str(ROOT/'ArtSources/Play/Xylophone.glb'))
sources=[o for o in bpy.data.objects if o.type=='MESH']
groups={
 'chime-body':[o for o in sources if o.name.startswith('Xylophone frame rail') or o.name.startswith('Accidental frame end crossmember')],
 'chime-low':[o for o in sources if o.name in {'Tuned rosewood bar '+str(i).zfill(2) for i in (0,2,4,5)}],
 'chime-high':[o for o in sources if o.name in {'Tuned rosewood bar '+str(i).zfill(2) for i in (29,31,33,35)}]
}
for name,originals in groups.items():
    if len(originals)!=4:raise RuntimeError('Source part names changed: '+name)
    copies=[]
    for o in originals:
        n=o.copy();n.data=o.data.copy();bpy.context.collection.objects.link(n);copies.append(n)
    export(name,copies,True)

bpy.ops.wm.open_mainfile(filepath=str(ROOT/'ArtSources/Play/BowlingPin.blend'))
pins=[o for o in bpy.data.objects if o.type=='MESH']
for o in pins:
    for mat in o.data.materials:
        if mat is not None:
            color=mat.diffuse_color[:];mat.use_nodes=True
            diffuse=next((n for n in mat.node_tree.nodes if n.type=='BSDF_DIFFUSE'),None)
            if diffuse is not None:color=diffuse.inputs['Color'].default_value[:]
            mat.node_tree.nodes.clear();shader=mat.node_tree.nodes.new('ShaderNodeBsdfPrincipled');shader.inputs['Base Color'].default_value=color
            output=mat.node_tree.nodes.new('ShaderNodeOutputMaterial');mat.node_tree.links.new(shader.outputs['BSDF'],output.inputs['Surface'])
export('bowling-pin',pins,True)

clear();bpy.ops.import_scene.fbx(filepath=str(ROOT/'Assets/ThirdParty/KenneySurvival/Models/structure-floor.fbx'))
objects=[o for o in bpy.data.objects if o.type=='MESH']
for obj in objects:
    obj.data.transform(Matrix.Rotation(math.pi/2,4,'X')@obj.matrix_world);obj.matrix_world=Matrix.Identity(4)
export('backstop',objects)
