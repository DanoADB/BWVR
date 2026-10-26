"""Original BWVR sound sketches, generated from noise and oscillators; no sampled media.
Regenerate with Python 3 standard library: python tools/prepare-audio.py
WAVs are mono 22050 Hz / PCM16. Loops use a wrap crossfade, effects fade to zero.
"""
import array, json, math, random, wave
from pathlib import Path
RATE=22050
OUT=Path(__file__).resolve().parents[1]/'Assets/BWVR/Resources/IslandAudio'
OUT.mkdir(parents=True,exist_ok=True)
report={}
def save(name,data,loop=False):
    peak=max(abs(x) for x in data) or 1
    data=[x*.68/peak for x in data]
    if not loop:
        fade=min(256,len(data)//8)
        for i in range(fade):
            data[i]*=i/fade;data[-i-1]*=i/fade
    pcm=array.array('h',(round(max(-1,min(1,x))*32767) for x in data))
    with wave.open(str(OUT/(name+'.wav')),'wb') as f:
        f.setnchannels(1);f.setsampwidth(2);f.setframerate(RATE);f.writeframes(pcm.tobytes())
    report[name]={'seconds':len(data)/RATE,'peak':max(abs(x) for x in data),'rms':math.sqrt(sum(x*x for x in data)/len(data)),'boundaryDelta':abs(data[0]-data[-1]),'loop':loop}
def bed(name,seconds,cutoff,seed):
    rng=random.Random(seed);n=int(seconds*RATE);fade=RATE
    low=slow=0;data=[];a=1-math.exp(-2*math.pi*cutoff/RATE)
    for i in range(n+fade):
        white=rng.uniform(-1,1);low+=a*(white-low);slow+=.025*(white-slow);t=i/RATE
        if name=='surf':
            swell=.22+.78*(.5+.5*math.sin(2*math.pi*t/8))**2
            x=(low*.8+(white-slow)*.035)*swell
        elif name=='wind':x=low*(.65+.18*math.sin(2*math.pi*t/6)+.12*math.sin(2*math.pi*t/4))+(white-slow)*.018
        else:x=(white-low)*.2+low*.3
        data.append(x)
    for i in range(fade):
        p=i/fade;data[i]=data[n+i]*(1-p)+data[i]*p
    save(name,data[:n],True)
def effect(name,seconds,freq,decay,noise=.1,chirp=0,seed=1):
    rng=random.Random(seed);data=[];phase=low=0
    for i in range(int(seconds*RATE)):
        t=i/RATE;p=t/seconds;phase+=2*math.pi*(freq+chirp*p)/RATE
        low+=.16*(rng.uniform(-1,1)-low)
        tone=math.sin(phase)+.25*math.sin(phase*2.03)+.1*math.sin(phase*3.97)
        envelope=(1-math.exp(-t*120))*math.exp(-t*decay)*(1-p)**2
        if name=='bird': envelope*=max(0,math.sin(p*math.pi*4))**2
        data.append((tone*(1-noise)+low*noise*5)*envelope)
    save(name,data)
bed('surf',16,700,101);bed('wind',12,240,102);bed('rain',12,1800,103)
for args in [
 ('step',.18,110,26,.85,0),('grasp',.13,220,28,.65,-80),('place',.23,130,18,.7,-60),
 ('snap',.30,520,13,.3,-250),('ball',.28,180,17,.3,-110),('pour',.85,480,2,.95,-180),
 ('harvest',.24,200,20,.8,-80),('notice',.35,300,7,.05,200),('content',.6,190,3,.08,80),
 ('uneasy',.48,200,4,.2,-100),('miracle',1.2,580,2,.04,300),('discovery',.8,440,3,.05,350),
 ('chime',.6,660,5,.02,0),('bird',.85,1800,1,.01,1600),('thunder',2.6,48,1.3,.9,-20),('gust',.48,90,3,.99,-20)]:effect(*args)
(OUT/'analysis.json').write_text(json.dumps(report,indent=2)+'\n')
print(json.dumps({'clips':len(report),'pcmBytes':sum((OUT/(k+'.wav')).stat().st_size for k in report),'maxLoopBoundary':max(v['boundaryDelta'] for v in report.values() if v['loop'])}))
