// Adapt Kutejnikov's CC0 blank open scroll, keeping painted curls outside the control area.
const fs=require('fs'),path=require('path'),sharp=require('sharp');
(async()=>{const source=path.resolve(__dirname,'../ArtSources/GodToolScroll/01_papers.png');
const bands=[{top:367,height:85,h:155},{top:452,height:85,h:714},{top:537,height:114,h:155}];let y=0,parts=[];
for(const b of bands){const input=await sharp(source).extract({left:72,top:b.top,width:223,height:b.height}).resize(1024,b.h,{fit:'fill'}).png().toBuffer();parts.push({input,left:0,top:y});y+=b.h;}
await sharp({create:{width:1024,height:1024,channels:4,background:{r:0,g:0,b:0,alpha:0}}}).composite(parts).png().toFile(path.resolve(__dirname,'../Assets/BWVR/Resources/GodToolScroll/parchment.png'));
})();
