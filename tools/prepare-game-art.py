"""Prepare a local-only Unity import bundle from Shade's converted game art."""
import argparse, json, math
from pathlib import Path
parser=argparse.ArgumentParser();parser.add_argument('source',type=Path);parser.add_argument('--output',type=Path,default=Path('.local/game-art.json'));args=parser.parse_args()
source=json.loads(args.source.read_text());models=[]
# Bake the original single-joint-per-vertex CBN channels into blend shapes.
# Bone orientations in the export are column-major; CBN Euler axes are row-vector.
def mul(a,b):return [[sum(a[i][k]*b[k][j] for k in range(3)) for j in range(3)] for i in range(3)]
def mv(a,v):return [sum(a[i][k]*v[k] for k in range(3)) for i in range(3)]
def transpose(a):return [list(v) for v in zip(*a)]
def euler(v):
 x,y,z=v;cx,sx=math.cos(x),math.sin(x);cy,sy=math.cos(y),math.sin(y);cz,sz=math.cos(z),math.sin(z)
 return transpose(mul(mul([[1,0,0],[0,cx,-sx],[0,sx,cx]],[[cy,0,sy],[0,1,0],[-sy,0,cy]]),[[cz,-sz,0],[sz,cz,0],[0,0,1]]))
motion_path=args.source.with_name('cow-animation-raw.json')
if not motion_path.exists():raise FileNotFoundError('Original cow standing/walking cache missing: '+str(motion_path))
motion=json.loads(motion_path.read_text())
baked=[]
if motion:
 stand=next(c for c in motion['clips'] if c['name']=='stand');bones=source['bones']
 base_r=[[0,0,0] for _ in bones];base_p=[b['position'] for b in bones]
 for j,v in zip(stand['rotated'],stand['frames'][0]['rotations']):base_r[j]=v
 for j,v in zip(stand['translated'],stand['frames'][0]['translations']):base_p[j]=v
 for clip in motion['clips']:
  if clip['name'] not in ['stand','walk']:continue
  for f,frame in enumerate(clip['frames']):
   angles=list(base_r);positions=list(base_p)
   for j,v in zip(clip['rotated'],frame['rotations']):angles[j]=v
   for j,v in zip(clip['translated'],frame['translations']):positions[j]=v
   matrices=[];offsets=[]
   for i,b in enumerate(bones):
    bind=[[b['rotation'][col*3+row] for col in range(3)] for row in range(3)];r=mul(bind,euler(angles[i]));p=list(positions[i]);parent=b['parent']
    if clip['name']=='walk' and parent==4294967295:p[0]=base_p[i][0];p[2]=base_p[i][2]
    if parent!=4294967295:
     if parent>=i:raise ValueError('Invalid skeleton hierarchy')
     p=[a+b for a,b in zip(mv(matrices[parent],p),offsets[parent])];r=mul(matrices[parent],r)
    matrices.append(r);offsets.append(p)
   parts=[];floor=float('inf')
   for primitive in source['primitives']:
    raw=primitive['bindVertices'];out=[]
    for i,j in enumerate(primitive['joints']):
     if j<0 or j>=len(bones):raise ValueError('Invalid vertex joint')
     v=raw[i*8:i*8+8];pos=[a+b for a,b in zip(mv(matrices[j],v[:3]),offsets[j])];normal=mv(matrices[j],v[5:8]);floor=min(floor,pos[1]);out.extend(pos+v[3:5]+normal)
    parts.append(out)
   for vertices in parts:
    for i in range(1,len(vertices),8):vertices[i]-=floor
   baked.append(dict(name=clip['name']+str(f),duration=clip['header'][0]/1000,parts=parts))
 print('Baked original cow motion:',len(baked),'poses')

for name,model in [('cow',source)]+[(k,v) for k,v in source['village'].items() if k!='grass']:
 textures=[]
 for t in model['textures']:
  count=len(t['rgba'])//4;width=t.get('width',math.isqrt(count));height=t.get('height',width)
  if width*height!=count:raise ValueError('Texture dimensions do not match')
  textures.append(dict(id=t['id'],width=width,height=height,rgba=t['rgba']))
 primitives=[]
 for p in model['primitives']:
  vertices=p['vertices'];indices=p['indices']
  if len(vertices)%8 or len(indices)%3 or not all(math.isfinite(x) for x in vertices):raise ValueError('Invalid mesh')
  if indices and (min(indices)<0 or max(indices)>=len(vertices)//8):raise ValueError('Invalid triangle')
  result={k:p[k] for k in ['vertices','indices','skin','color']}
  if name=='cow' and baked:
   part=len(primitives);result['vertices']=baked[0]['parts'][part]
   result['poses']=[dict(name=f['name'],vertices=f['parts'][part]) for f in baked]
  primitives.append(result)
 models.append(dict(name=name,primitives=primitives,textures=textures))
g=source['village']['grass'];bundle=dict(models=models,grass=dict(id=g['id'],width=g['width'],height=g['height'],rgba=g['rgba']))
args.output.parent.mkdir(parents=True,exist_ok=True);args.output.write_text(json.dumps(bundle,separators=(',',':')))
print('Prepared',len(models),'models at',args.output,'(local only)')
