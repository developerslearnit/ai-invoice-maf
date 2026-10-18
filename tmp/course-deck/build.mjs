import fs from "node:fs/promises";
import path from "node:path";
import {fileURLToPath,pathToFileURL} from "node:url";
import {createCanvas} from "@napi-rs/canvas";
import {Presentation,PresentationFile,FileBlob} from "@oai/artifact-tool";
import {deck,modules,sources,repo} from "./content.mjs";

const dir=path.dirname(fileURLToPath(import.meta.url));
const root=path.resolve(dir,"../..");
const skill="C:/Users/Mark/.codex/plugins/cache/openai-primary-runtime/presentations/26.1015.12040/skills/presentations";
const runtime="C:/Users/Mark/.cache/codex-runtimes/codex-primary-runtime/dependencies";
process.env.RUNTIME_NODE_MODULES=path.join(runtime,"node/node_modules");
process.env.RUNTIME_NODE=process.execPath;
process.env.RUNTIME_PYTHON=path.join(runtime,"python/python.exe");
const stage=path.join(dir,"staging");
await fs.mkdir(stage,{recursive:true});
await fs.mkdir(path.join(dir,"renders"),{recursive:true});
const finalPath=path.join(root,"output/presentations/invoiceai-microsoft-agent-framework-course.pptx");
const candidatePath=path.join(stage,"candidate.pptx");
const p=Presentation.create({slideSize:{width:1280,height:720}});
p.comments.setSelf({displayName:"Course Author",initials:"CA"});
const col={bg:"#F6F8FA",navy:"#112C43",teal:"#007F83",body:"#34495B",muted:"#617384",line:"#DCE5EA",pale:"#EAF1F3",white:"#FFFFFF",cyan:"#73DEDC"};
const font="Segoe UI", mono="Consolas";
const ctx=createCanvas(1,1).getContext("2d");
const issues=[], nativeTables=[];
const noteCounts=[];
const n=(s)=>String(s).replace(/[“”]/g,'"').replace(/[‘’]/g,"'").replace(/[–—‑]/g,"-");
function lineCount(text,size,width,typeface=font,bold=false){
  ctx.font=(bold?"bold ":"")+size+'px "'+typeface+'"';
  let count=0;
  for(const para of n(text).split("\n")){
    let line="";
    for(const word of para.split(/\s+/)){
      const trial=line?line+" "+word:word;
      if(line&&ctx.measureText(trial).width>width){count++;line=word;}else line=trial;
    }
    count++;
  }
  return count;
}
function text(slide,value,x,y,w,h,size=26,{bold=false,color=col.body,typeface=font,name="body",align="left"}={}){
  value=n(value);
  const need=lineCount(value,size,w,typeface,bold)*size*1.21;
  if(need>h+3)issues.push({slide:p.slides.items.length,name,need:Math.ceil(need),height:h,text:value.slice(0,100)});
  const s=slide.shapes.add({geometry:"textbox",name,position:{left:x,top:y,width:w,height:h},fill:"none",line:{fill:"none",width:0}});
  s.text=value;
  s.text.style={fontSize:size,typeface,bold,color,alignment:align,verticalAlignment:"top",insets:0,wrap:"square",autoFit:"none"};
  return s;
}
const assetCache=new Map();
async function img(slide,name,x,y,w,h,fit="contain"){
  if(!assetCache.has(name))assetCache.set(name,await fs.readFile(path.join(dir,"assets",name)));
  const bytes=assetCache.get(name);
  slide.images.add({blob:bytes.buffer.slice(bytes.byteOffset,bytes.byteOffset+bytes.byteLength),contentType:"image/png",alt:name==="cover.png"?"Abstract invoice and software integration background":"Conceptual InvoiceAI architecture illustration",position:{left:x,top:y,width:w,height:h},fit});
}
function header(slide,d,index){
  text(slide,d.status??"Engineering design",72,28,1100,25,16,{color:col.teal,bold:true,name:"context"});
  const titleSize=d.title.length>61?42:44;
  text(slide,d.title,72,72,1136,110,titleSize,{color:col.navy,bold:true,name:"title"});
  const module=d.module?String(d.module).padStart(2,"0")+"  "+modules[d.module-1]:"InvoiceAI capstone course";
  text(slide,module,72,678,1050,22,14,{color:col.muted,name:"footer"});
  text(slide,String(index+1).padStart(2,"0"),1160,678,48,22,14,{color:col.muted,name:"page",align:"right"});
}
async function addNotes(slide,d,index){
  const citations=[];
  for(const key of d.docs??[]){
    if(!sources[key])throw Error("Unknown doc key "+key);
    citations.push(sources[key][0]+"\n"+sources[key][1]);
  }
  for(const key of d.refs??[]){
    if(!repo[key])throw Error("Unknown repo key "+key);
    const [rel,needle]=repo[key];
    const body=await fs.readFile(path.join(root,rel),"utf8");
    const line=body.split(/\r?\n/).findIndex(l=>l.includes(needle)&&!l.trimStart().startsWith("///"))+1;
    if(line<1)throw Error("Unresolved code reference "+key);
    citations.push(rel+":"+line);
  }
  let notes=d.notes;
  if(d.type==="chapter")notes+="\nMODULE OUTCOME\n"+d.data[0];
  if(d.type==="lab")notes+="\nIMPLEMENTATION TASKS\n"+d.data.tasks.map((x,i)=>(i+1)+". "+x).join("\n")+"\nACCEPTANCE EVIDENCE\n"+d.data.accept.map((x,i)=>(i+1)+". "+x).join("\n");
  notes+="\n\nCOURSE STATUS\n"+(d.status??"Engineering design")+". ";
  if((d.status??"").includes("pattern")||d.type==="code")notes+="Code is a condensed teaching pattern. Supply the named host dependencies and use the pinned SDK's exact overloads in the lab project. ";
  if((d.status??"").includes("target")||d.type==="lab")notes+="This describes course work, not a claim that the baseline repository already implements it. ";
  if(citations.length)notes+="\n\nSOURCES\n"+citations.join("\n\n")+"\n\nPrimary documentation checked 17 October 2026. Local line references describe the reviewed workspace.";
  slide.speakerNotes.text=n(notes);
  slide.speakerNotes.setVisible(true);
  noteCounts.push(notes.split(/\s+/).length);
  const comment=d.comment??(
    d.type==="chapter"?"Instructor: This module ends with a reviewable implementation increment. Use the speaker notes for pacing and the module lab for acceptance.":
    d.type==="code"?"Instructor: This is a condensed code pattern. Compile it against the course-pinned SDK and explain the application-owned dependencies before the demo.":
    (d.status??"").startsWith("Existing")?"Instructor: Show the executable source behind this observation and distinguish it from the proposed capstone behavior.":
    "Instructor: "+d.notes.split("\n").filter(l=>l&&l!=="AUTHOR COMMENTARY")[0].slice(0,340)
  );
  p.comments.addThread({slide},n(comment),{position:{x:1200,y:70}});
}
function nativeTable(slide,d,index){
  const {headers,rows}=d.data;
  const tracks=d.widths??Array(headers.length).fill(1/headers.length);
  const widths=tracks.map(v=>1136*v);
  const bodySize=23;
  const rowHeights=[58,...rows.map(row=>Math.max(62,...row.map((v,c)=>lineCount(v,bodySize,widths[c]-28)*bodySize*1.22+24)))];
  const height=rowHeights.reduce((a,b)=>a+b,0);
  if(height>470)issues.push({slide:index+1,name:"table",height,text:d.title});
  const t=slide.tables.add({rows:rows.length+1,columns:headers.length,left:72,top:190,width:1136,height,columnWidths:widths,values:[headers,...rows].map(r=>r.map(n))});
  t.borders.assign({style:"solid",fill:col.line,width:0.6});
  t.cells.block({row:0,column:0,rowCount:rows.length+1,columnCount:headers.length}).assign({textStyle:{fontSize:bodySize,typeface:font,color:col.body},margins:{left:14,right:14,top:12,bottom:10},anchor:"top"});
  for(let r=0;r<=rows.length;r++){
    t.rows[r].height=rowHeights[r];
    for(let c=0;c<headers.length;c++){
      const cell=t.getCell(r,c);
      cell.fill=r===0?col.navy:(r%2?col.pale:col.bg);
      cell.text.style={fontSize:bodySize,typeface:font,color:r===0?col.white:col.body,bold:r===0};
    }
  }
  nativeTables.push(index+1);
}
for(let i=0;i<deck.length;i++){
  const d=deck[i],s=p.slides.add();
  s.background.fill=col.bg;
  if(["cover","chapter","closing"].includes(d.type)){
    s.background.fill="#06182B";
    await img(s,"cover.png",0,0,1280,720,"cover");
    const label=d.type==="chapter"?"MODULE "+String(d.module).padStart(2,"0"):(d.type==="cover"?"ADVANCED .NET ENGINEERING":"COURSE CAPSTONE");
    text(s,label,68,100,710,35,20,{color:col.cyan,bold:true,name:"context"});
    const title=d.type==="chapter"?d.title:d.title;
    text(s,title,68,190,720,d.type==="chapter"?185:235,d.type==="cover"?55:52,{color:col.white,bold:true,name:"title"});
    if(d.type==="chapter")text(s,d.data[0],68,420,650,120,27,{color:"#D9E6F0",name:"subtitle"});
    else text(s,d.data.join("\n"),68,d.type==="cover"?480:458,650,125,26,{color:"#D9E6F0",name:"subtitle"});
    text(s,d.type==="chapter"?"3 contact hours including the module lab":"Instructor edition  /  October 2026",68,646,690,28,17,{color:"#ABC0D2",name:"footer"});
    text(s,String(i+1).padStart(2,"0"),1160,678,48,22,14,{color:"#ABC0D2",name:"page",align:"right"});
  }else if(d.type==="image"){
    text(s,d.status,72,28,1100,25,16,{color:col.teal,bold:true,name:"context"});
    text(s,d.title,72,69,1136,70,43,{color:col.navy,bold:true,name:"title"});
    await img(s,d.data,25,140,1230,520,"contain");
    text(s,String(d.module).padStart(2,"0")+"  "+modules[d.module-1],72,678,1050,22,14,{color:col.muted,name:"footer"});
    text(s,String(i+1).padStart(2,"0"),1160,678,48,22,14,{color:col.muted,name:"page",align:"right"});
  }else{
    header(s,d,i);
    if(d.type==="points"){
      const start=205,gap=106;
      for(const [j,[label,detail]] of d.data.entries()){
        text(s,label,72,start+j*gap,240,80,28,{bold:true,color:col.teal,name:"label-"+j});
        text(s,detail,350,start+j*gap,858,84,27,{name:"point-"+j});
      }
    }else if(d.type==="table")nativeTable(s,d,i);
    else if(d.type==="code"){
      const lines=[];
      for(const raw of d.data.lines){
        const line=n(raw);
        if(line.length>65&&line.trimStart().startsWith("//")){
          const words=line.trim().slice(2).trim().split(/\s+/);let current="//";
          for(const word of words){if(current.length+word.length+1>65){lines.push(current);current="// "+word;}else current+=" "+word;}lines.push(current);
        }else lines.push(line);
      }
      text(s,lines.join("\n"),72,195,790,446,23,{typeface:mono,color:col.navy,name:"code"});
      for(const [j,line] of d.data.explain.entries()){
        text(s,line,915,210+j*126,293,110,24,{name:"explanation-"+j,color:j===0?col.teal:col.body});
      }
    }else if(d.type==="lab"){
      text(s,"Implementation",72,195,640,42,28,{color:col.teal,bold:true,name:"implementation"});
      text(s,"Acceptance",785,195,423,42,28,{color:col.teal,bold:true,name:"acceptance"});
      for(const [j,v] of d.data.tasks.entries())text(s,(j+1)+". "+v,72,260+j*107,634,92,26,{name:"task-"+j});
      for(const [j,v] of d.data.accept.entries())text(s,v,785,260+j*107,423,92,25,{name:"criterion-"+j});
      text(s,"Suggested lab work: 70 minutes. Review the evidence before continuing.",72,618,1136,35,20,{color:col.muted,name:"lab-time"});
    }
  }
  await addNotes(s,d,i);
}
await fs.writeFile(path.join(stage,"text-fit-preflight.json"),JSON.stringify(issues,null,2));
await fs.writeFile(path.join(stage,"manifest.json"),JSON.stringify({count:deck.length,titles:deck.map(d=>d.title),nativeTables,noteCounts},null,2));
console.log("Slides authored:",deck.length,"; tables:",nativeTables.length,"; note words:",noteCounts.reduce((a,b)=>a+b,0));
if(issues.length){console.log(JSON.stringify(issues,null,2));throw Error("Fix text-fit preflight before export");}
await (await PresentationFile.exportPptx(p)).save(candidatePath);
console.log("Candidate saved");
const {finalizePresentation}=await import(pathToFileURL(path.join(skill,"container_tools/artifact_tool_utils.mjs")).href);
const result=await finalizePresentation({
 workspaceDir:root,candidatePath,finalPath,
 explicitTotalSlideCount:deck.length,
 requiredNativeTableOwnerSlides:nativeTables,
 requiredNativeChartOwnerSlides:[],
 pythonExecutable:path.join(runtime,"python/python.exe"),
 integrityValidatorPath:path.join(skill,"container_tools/inspect_presentation_package_integrity.py"),
 layoutValidatorPath:path.join(skill,"container_tools/inspect_presentation_layout_geometry.py"),
 layoutArgs:["--expected-slide-size-emu","12192000,6858000","--validate-bullet-geometry","--validate-heading-fit",...nativeTables.flatMap(n=>["--require-native-table-slide",String(n)])],
 fontPolicy:{basis:"design",families:[font,mono]},
 verifyArtifactToolImport:true,
 receiptPath:path.join(stage,"final.validation.json")
});
console.log("Finalized:",result.finalPath);
// Render from the final artifact, rather than relying on pre-export previews.
const reviewed=await PresentationFile.importPptx(await FileBlob.load(finalPath));
const start=Number(process.env.COURSE_RENDER_START??1)-1;
const end=Number(process.env.COURSE_RENDER_END??deck.length);
for(let i=start;i<Math.min(end,reviewed.slides.items.length);i++){
  const blob=await reviewed.export({slide:reviewed.slides.items[i],format:"png",scale:1});
  await fs.writeFile(path.join(dir,"renders","slide-"+String(i+1).padStart(3,"0")+".png"),new Uint8Array(await blob.arrayBuffer()));
  if((i+1)%5===0)console.log("Rendered",i+1);
}
console.log("All final slides rendered.");
process.exit(0);
