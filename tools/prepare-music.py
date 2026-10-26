"""Render BWVR's original Quiet Island sketch with Python 3's standard library.
No sampled instruments or borrowed melody. 6/8, quarter=72, D major, 16 bars.
Plucked chords, airy lead, deliberate rests; stereo PCM16/22050 Hz source.
"""
from pathlib import Path
import array, json, math, random, wave
RATE=22050;BEAT=60/72/2;BAR=BEAT*6;SECONDS=46
OUT=Path(__file__).resolve().parents[1]/'Assets/BWVR/Resources/IslandMusic'
OUT.mkdir(parents=True,exist_ok=True)
left=[0.0]*int(RATE*SECONDS);right=[0.0]*len(left)
def note(start,midi,duration,gain,pan,flute=False):
    rng=random.Random(round(start*1000)+midi);frequency=440*2**((midi-69)/12);phase=breath=0
    for i in range(min(int(duration*RATE),len(left)-round(start*RATE))):
        t=i/RATE;phase+=2*math.pi*frequency/RATE*(1+.0018*math.sin(t*math.pi*9))
        if flute:
            breath+=.08*(rng.uniform(-1,1)-breath)
            attack=min(1,t/.12);release=min(1,(duration-t)/.24)
            value=(math.sin(phase)+.13*math.sin(phase*2)+.025*math.sin(phase*3)+breath*.18)*attack*release*(.9+.1*math.sin(t*math.pi*3))
        else:
            value=sum(math.sin(phase*h)*math.exp(-t*(1.8+h*.7))/(h*h) for h in range(1,7))*(1-math.exp(-t*170))*min(1,(duration-t)/.2)
        index=round(start*RATE)+i;value*=gain
        left[index]+=value*math.sqrt((1-pan)/2);right[index]+=value*math.sqrt((1+pan)/2)
# A light rolling accompaniment, with entire bars left empty between phrases.
chords=[(50,57,62),(55,59,62),(47,54,59),(45,52,57)]
for bar in [0,1,2,3,6,7,8,9,12,13,14,15]:
    chord=chords[(bar//2)%4]
    for j,pitch in enumerate(chord):note(bar*BAR+j*BEAT*.55,pitch,3.4,.075 if j else .09,(-.3,0,.3)[j])
    if bar in (1,3,7,9,13):note(bar*BAR+3*BEAT,chord[1]+12,2.3,.045,.25)
# Three related but distinct phrases. All pitches and rhythm below are original.
phrases=[(0,[(0,69,1.6),(2,71,.8),(3,74,1.5),(5,73,.8),(6,71,2),(9,69,1),(10.5,66,1),(12,67,2),(15,66,1),(16,64,1.5),(18,62,3)]),
 (6,[(0,66,1),(1.5,69,1),(3,71,2),(6,74,1),(7.5,76,1),(9,74,2),(12,73,1),(13.5,71,1),(15,69,2),(18,66,1.5),(20,64,2)]),
 (12,[(0,67,1.5),(2,69,1),(3.5,71,1.5),(6,69,2),(9,66,1),(10.5,64,1),(12,62,2.5),(16,66,1),(18,64,1.5),(21,62,2.5)])]
for bar,phrase in phrases:
    for offset,pitch,length in phrase:note(bar*BAR+offset*BEAT,pitch,length*BEAT,.115,.08,True)
# Small, sparse reflection taps; deterministic and cheap at runtime (baked offline).
for channel in (left,right):
    original=channel[:]
    for delay,gain in ((.13,.10),(.29,.065)):
        n=round(delay*RATE)
        for i in range(n,len(channel)):channel[i]+=original[i-n]*gain
peak=max(max(abs(x) for x in left),max(abs(x) for x in right));scale=.58/max(peak,1e-9)
pcm=array.array('h')
for i,(a,b) in enumerate(zip(left,right)):
    t=i/RATE;fade=min(1,t/2,(SECONDS-t)/4);pcm.extend((round(a*scale*fade*32767),round(b*scale*fade*32767)))
with wave.open(str(OUT/'QuietIsland.wav'),'wb') as f:
    f.setnchannels(2);f.setsampwidth(2);f.setframerate(RATE);f.writeframes(pcm.tobytes())
report={'title':'Quiet Island','seconds':SECONDS,'tempoQuarter':72,'meter':'6/8','channels':2,'sampleRate':RATE,'peak':max(abs(x) for x in pcm)/32768,'rms':math.sqrt(sum(x*x for x in pcm)/len(pcm))/32768,'sourceBytes':len(pcm)*2,'melodyRestBars':[4,5,10,11],'samplesUsed':False}
(OUT/'analysis.json').write_text(json.dumps(report,indent=2)+'\n');print(json.dumps(report))
