// Rasterize the pinned, attributed SVGs; no artwork is fetched or code executed here.
// Requires Node.js and sharp. The original SVGs remain unchanged in ArtSources.
const fs=require('fs'),path=require('path'),sharp=require('sharp');
(async()=>{const source=path.resolve(__dirname,'../ArtSources/GodToolIcons');const manifest=JSON.parse(fs.readFileSync(path.join(source,'manifest.json'),'utf8'));
 const tiles=[];for(let i=0;i<manifest.icons.length;i++){const item=manifest.icons[i];let svg=fs.readFileSync(path.join(source,item.tool+'.svg'),'utf8');const background='<path d="M0 0h512v512H0z"/>';if(!svg.includes(background))throw new Error('Unexpected SVG background: '+item.tool);svg=svg.replace(background,'');
  const input=await sharp(Buffer.from(svg)).resize(224,224).extend({top:16,bottom:16,left:16,right:16,background:{r:0,g:0,b:0,alpha:0}}).png().toBuffer();tiles.push({input,left:(i%5)*256,top:Math.floor(i/5)*256});
 }const dest=path.resolve(__dirname,'../Assets/BWVR/Resources/GodToolIcons');fs.mkdirSync(dest,{recursive:true});await sharp({create:{width:1280,height:512,channels:4,background:{r:0,g:0,b:0,alpha:0}}}).composite(tiles).png().toFile(path.join(dest,'atlas.png'));console.log('Ten transparent icons -> one 1280 x 512 atlas');
})().catch(error=>{console.error(error);process.exitCode=1;});
