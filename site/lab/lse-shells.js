const pieceColors=[['#f5f3e9','#ef7d16'],['#f5f3e9','#d72d2d'],['#f5f3e9','#16a05d'],['#f5f3e9','#2474d8'],['#f4d928','#16a05d'],['#f4d928','#2474d8']];
function edgeColors(state,position){const colors=[...pieceColors[state.pieces[position]]];return state.flips[position]?colors.reverse():colors;}
function polygon(points,fill,highlight=false){return `<polygon points="${points}" fill="${fill}" stroke="${highlight?'#080808':'#2b2b2b'}" stroke-width="${highlight?'.035':'.012'}" stroke-linejoin="round"/>`;}
// Perspective geometry adapted from BriefCubing's Display.diagramUF renderer.
const topFace={bl:'-.476,-.691 -.192,-.691 -.199,-.511 -.497,-.511',b:'-.141,-.691 .142,-.691 .149,-.511 -.148,-.511',br:'.194,-.691 .477,-.691 .498,-.511 .201,-.511',l:'-.499,-.477 -.202,-.477 -.210,-.278 -.522,-.278',c:'-.148,-.477 .149,-.477 .157,-.278 -.156,-.278',r:'.203,-.477 .501,-.477 .524,-.278 .211,-.278',fl:'-.525,-.241 -.212,-.241 -.221,-.020 -.551,-.020',f:'-.156,-.241 .157,-.241 .165,-.020 -.164,-.020',fr:'.214,-.241 .527,-.241 .552,-.020 .222,-.020'};
const frontFace={tl:'-.552,.020 -.222,.020 -.214,.241 -.527,.241',t:'-.166,.020 .164,.020 .156,.241 -.157,.241',tr:'.221,.020 .551,.020 .525,.241 .212,.241',l:'-.524,.278 -.211,.278 -.203,.477 -.501,.477',c:'-.157,.278 .156,.278 .148,.477 -.149,.477',r:'.210,.278 .522,.278 .499,.477 .202,.477',bl:'-.498,.511 -.201,.511 -.194,.691 -.477,.691',b:'-.149,.511 .148,.511 .141,.691 -.142,.691',br:'.199,.511 .497,.511 .476,.691 .192,.691'};
function centerColors(offset){return [["#f5f3e9","#16a05d"],["#2474d8","#f5f3e9"],["#f4d928","#2474d8"],["#16a05d","#f4d928"]][offset];}
function diagram(state){
  const colors=[0,1,2,3,4,5].map(position=>edgeColors(state,position)),centers=centerColors(state.center),highlight=position=>state.pieces[position]<2,gray='#d1cec4',dark='#aaa69b';
  return `<svg class="lse-diagram" viewBox="-.82 -.87 1.64 1.75" role="img" aria-label="LSE state solved by ${state.solution||'no moves'}">
    ${polygon('-.547,0 .547,0 .47,.665 -.47,.665','#171717')}${polygon('-.47,-.665 .47,-.665 .547,0 -.547,0','#171717')}
    ${polygon(topFace.bl,dark)}${polygon(topFace.b,colors[3][0],highlight(3))}${polygon(topFace.br,dark)}
    ${polygon(topFace.l,colors[0][0],highlight(0))}${polygon(topFace.c,centers[0])}${polygon(topFace.r,colors[1][0],highlight(1))}
    ${polygon(topFace.fl,dark)}${polygon(topFace.f,colors[2][0],highlight(2))}${polygon(topFace.fr,dark)}
    ${polygon(frontFace.tl,dark)}${polygon(frontFace.t,colors[2][1],highlight(2))}${polygon(frontFace.tr,dark)}
    ${polygon(frontFace.l,gray)}${polygon(frontFace.c,centers[1])}${polygon(frontFace.r,gray)}
    ${polygon(frontFace.bl,dark)}${polygon(frontFace.b,colors[4][1],highlight(4))}${polygon(frontFace.br,dark)}
    ${polygon('-.620,-.477 -.552,-.477 -.576,-.279 -.647,-.278',colors[0][1],highlight(0))}
    ${polygon('.553,-.477 .623,-.477 .650,-.278 .578,-.278',colors[1][1],highlight(1))}
    ${polygon('-.149,.708 .141,.708 .132,.806 -.137,.806',colors[4][0],highlight(4))}
    <g aria-label="DB edge shown behind cube"><text x="0" y="-.795" text-anchor="middle">DB</text>${polygon('-.105,-.845 0,-.812 .105,-.845 0,-.878',colors[5][1],highlight(5))}${polygon('-.105,-.845 0,-.812 0,-.755 -.105,-.787',colors[5][0],highlight(5))}</g>
  </svg>`;
}
function card(state,index){const next=state.distance?`<p class="next-move"><span>Next</span><b>${state.optimalFirstMoves.join(' or ')}</b><small>→ distance ${state.distance-1}</small></p>`:'';return `<article class="shell-card"><h3>${state.solution||'Solved'}</h3>${diagram(state)}${next}<p class="setup">${state.distance?`setup <b>${state.setup}</b> · solution <b>${state.solution}</b>`:'solved state'}</p><footer><span>case ${index+1}</span><span>center ${state.center} · AUF ${state.auf}</span></footer></article>`;}
const explanations={
  0:[['SOLVED','No case','The cube is done.',null]],
  1:[
    ['3 STATES · M','Align the M slice','The fixed U-layer frame—the X outside the moving slice—is intact. Turn the M slice until its completed colors align.',['M / M\' / M2\'','0']],
    ['3 STATES · U','Align the top','The M slice is complete. Turn U until the completed top aligns with the side colors.',['U / U\' / U2\'','0']]
  ],
  2:[
    ['9 STATES · M THEN U','Bring the M-slice bar to U','Find the completed bar somewhere around the middle slice—even on the bottom. Turn M just far enough to bring that bar to the top.',['M / M\' / M2\'','1 · align the top']],
    ['9 STATES · U THEN M','Align the U bar with M','A completed bar is on top but offset from its continuation in the middle slice. Turn U until the two parts join.',['U / U\' / U2\'','1 · align the M slice']]
  ],
  3:[
    ['27 STATES · M–U–M','Fill the matching U bookends','The two fixed stickers on opposite sides of U match. Turn M until the matching center sticker sits between those bookends.',['M / M\' / M2\'','2 · align the U bar']],
    ['27 STATES · U–M–U','Attach the top edge to the M-slice stem','A partial bar is fixed in the middle slice. Turn U until the matching top edge attaches to that stem.',['U / U\' / U2\'','2 · bring the M-slice bar to U']]
  ],
  4:[
    ['81 PATHS · M–U–M–U','Expose the M-slice stem','Check the three M alignments. Choose the M power that makes a partial bar visible in the middle slice. Stop there; the new position is the distance-three stem case.',['M / M\' / M2\'','3 · attach the top edge']],
    ['81 PATHS · U–M–U–M','Expose the matching U bookends','Check the three U alignments. Choose the U power that makes matching opposite stickers visible on U. Stop there; the new position is the distance-three bookend case.',['U / U\' / U2\'','3 · fill the bookends']],
    ['1 SHARED STATE · 161 DISTINCT TOTAL','The half-turn overlap',`The symmetric state may take either next move: <code>M2'</code> reaches the stem family and <code>U2'</code> reaches the bookend family. Its full reductions are <code>M2' U2' M2' U2'</code> and <code>U2' M2' U2' M2'</code>.`,['M2\' or U2\'','3']]
  ],
  5:[
    ['236 STATES · M–U–M–U–M','Set up the U-bookend reduction',`Check the three M alignments. Choose the M power whose resulting position has a U alignment that reveals matching bookends. Stop after M; you are now in the distance-four U-first family. Example setup: <code>M' U' M' U' M'</code>.`,['M / M\' / M2\'','4 · expose the U bookends']],
    ['236 STATES · U–M–U–M–U','Set up the M-stem reduction',`Check the three U alignments. Choose the U power whose resulting position has an M alignment that reveals a partial middle-slice stem. Stop after U; you are now in the distance-four M-first family. Example setup: <code>U' M' U' M' U'</code>.`,['U / U\' / U2\'','4 · expose the M-slice stem']]
  ],
  6:[
    ['696 STATES · M–U–M–U–M–U','Enter the U-first distance-five family',`Check the three M alignments. Choose the M power whose resulting position has the distance-five U-first relationship: a U alignment can set up the M-stem reduction. Example setup: <code>U' M' U' M' U' M'</code>.`,['M / M\' / M2\'','5 · set up the M stem']],
    ['696 STATES · U–M–U–M–U–M','Enter the M-first distance-five family',`Check the three U alignments. Choose the U power whose resulting position has the distance-five M-first relationship: an M alignment can set up the U-bookend reduction. Example setup: <code>M' U' M' U' M' U'</code>.`,['U / U\' / U2\'','5 · set up the U bookends']],
    ['46 SHARED STATES · 1,346 DISTINCT TOTAL','Either axis can step down',`Both axes contain a move that reaches distance five. Example setup: <code>U' M2' U M2' U M2'</code>. Its next move may be <code>M2'</code> or <code>U</code>; either one reduces the exact distance from six to five.`,['an M or U power','5']]
  ],
  7:[
    ['1,826 STATES · M-FIRST','Enter a U-first distance-six family',`Choose the M power that leaves a distance-six state with an optimal U-axis continuation. Example setup: <code>M' U' M' U' M' U' M'</code>; its next move is <code>M</code>.`,['M / M\' / M2\'','6']],
    ['1,802 STATES · U-FIRST','Enter an M-first distance-six family',`Choose the U power that leaves a distance-six state with an optimal M-axis continuation. Example setup: <code>U' M' U' M' U' M' U'</code>; its next move is <code>U</code>.`,['U / U\' / U2\'','6']],
    ['105 SHARED STATES · 3,523 DISTINCT TOTAL','Either axis can step down',`Example setup: <code>M2' U2' M2' U' M' U' M'</code>. Its exact next moves are <code>M</code> or <code>U2'</code>; both reach distance six.`,['an M or U power','6']]
  ],
  8:[
    ['4,483 STATES · M-FIRST','Enter a U-first distance-seven family',`Choose the M power that leaves an exact distance-seven state with a U-first reduction. Example setup: <code>U' M U' M' U' M' U' M'</code>; its next move is <code>M</code>.`,['M / M\' / M2\'','7']],
    ['4,483 STATES · U-FIRST','Enter an M-first distance-seven family',`Choose the U power that leaves an exact distance-seven state with an M-first reduction. Example setup: <code>M U' M' U' M' U' M' U'</code>; its next move is <code>U</code>.`,['U / U\' / U2\'','7']],
    ['592 SHARED STATES · 8,374 DISTINCT TOTAL','Either axis can step down',`Example setup: <code>U' M' U' M' U' M' U' M'</code>. Its exact next moves are <code>M</code> or <code>U'</code>; both reach distance seven.`,['an M or U power','7']]
  ],
  9:[
    ['8,961 STATES · M-FIRST','Enter a U-first distance-eight family',`Choose the M power that leaves an exact distance-eight state with a U-first reduction. Example setup: <code>M' U' M U' M' U' M' U' M'</code>; its next move is <code>M</code>.`,['M / M\' / M2\'','8']],
    ['8,897 STATES · U-FIRST','Enter an M-first distance-eight family',`Choose the U power that leaves an exact distance-eight state with an M-first reduction. Example setup: <code>U' M U' M' U' M' U' M' U'</code>; its next move is <code>U</code>.`,['U / U\' / U2\'','8']],
    ['604 SHARED STATES · 17,254 DISTINCT TOTAL','Either axis can step down',`Example setup: <code>M' U2' M U' M' U' M' U' M'</code>. Its exact next moves are <code>M</code> or <code>U2'</code>; both reach distance eight.`,['an M or U power','8']]
  ],
  10:[
    ['18,007 STATES · M-FIRST','Enter a U-first distance-nine family',`Choose the M power that leaves an exact distance-nine state with a U-first reduction. Example setup: <code>U' M' U' M U' M' U' M' U' M'</code>; its next move is <code>M</code>.`,['M / M\' / M2\'','9']],
    ['18,007 STATES · U-FIRST','Enter an M-first distance-nine family',`Choose the U power that leaves an exact distance-nine state with an M-first reduction. Example setup: <code>M' U' M U' M' U' M' U' M' U'</code>; its next move is <code>U</code>.`,['U / U\' / U2\'','9']],
    ['4,439 SHARED STATES · 31,575 DISTINCT TOTAL','Either axis can step down',`Example setup: <code>U' M U' M U' M' U' M' U' M'</code>. Its exact next moves are <code>M</code> or <code>U</code>; both reach distance nine.`,['an M or U power','9']]
  ]
};
const outerShells={
  11:{total:40622,m:23400,u:22976,both:5754,mExample:[`M' U' M' U' M U' M' U' M' U' M'`,['M']],uExample:[`U' M' U' M U' M' U' M' U' M' U'`,['U']],bothExample:[`M2' U' M' U' M U' M' U' M' U' M'`,['M',`U2'`]]},
  12:{total:40200,m:25298,u:25298,both:10396,mExample:[`U' M' U' M' U' M U' M' U' M' U' M'`,['M',`M'`]],uExample:[`M' U' M' U' M U' M' U' M' U' M' U'`,['U',`U'`]],bothExample:[`U M U' M' U' M U' M' U' M' U' M'`,['M','U']]},
  13:{total:25959,m:16058,u:16038,both:6137,mExample:[`M' U' M' U' M' U' M U' M' U' M' U' M'`,['M',`M'`]],uExample:[`U2' M' U' M' U' M U' M' U' M' U' M' U'`,['U',`U'`]],bothExample:[`M2' U' M U' M' U' M U' M' U' M' U' M'`,['M','U',`U2'`]]},
  14:{total:10643,m:7715,u:7715,both:4787,mExample:[`U2' M' U2' M' U' M' U' M U' M' U' M' U' M'`,['M',`M'`]],uExample:[`M' U2' M' U' M' U' M U' M' U' M' U' M' U'`,['U',`U'`]],bothExample:[`U' M U' M' U' M' U' M U' M' U' M' U' M'`,['M',`M'`,'U',`U2'`]]},
  15:{total:2816,m:1736,u:1592,both:512,mExample:[`M' U2' M' U2' M' U' M' U' M U' M' U' M' U' M'`,['M',`M'`]],uExample:[`U2' M' U2' M' U' M' U' M U' M' U' M' U' M' U'`,['U',`U'`,`U2'`]],bothExample:[`M' U M' U2' M' U2' M' U' M U' M' U' M' U' M'`,['M',`M2'`,'U',`U'`,`U2'`]]},
  16:{total:882,m:560,u:560,both:238,mExample:[`U M' U2' M' U2' M' U' M' U' M U' M' U' M' U' M'`,['M',`M'`]],uExample:[`M U M U M' U2' M' U M' U M' U' M' U' M' U'`,['U',`U'`,`U2'`]],bothExample:[`U' M U2' M' U2' M' U' M' U' M U' M' U' M' U' M'`,['M',`M'`,'U',`U'`,`U2'`]]},
  17:{total:320,m:214,u:118,both:12,mExample:[`M' U M' U2' M' U2' M' U' M' U' M U' M' U' M' U' M'`,['M',`M'`]],uExample:[`U2' M U M U M' U2' M' U M' U M' U' M' U' M' U'`,['U',`U'`,`U2'`]],bothExample:[`M' U' M' U M U M' U M U M2' U' M' U' M' U' M'`,['M',`M'`,'U',`U'`]]},
  18:{total:80,m:62,u:62,both:44,mExample:[`U M' U2' M U' M U' M' U2' M' U' M2' U' M' U' M' U' M'`,['M',`M'`,`M2'`]],uExample:[`M' U2' M U M U M' U2' M' U M' U M' U' M' U' M' U'`,['U',`U'`,`U2'`]],bothExample:[`U' M' U M' U2' M' U2' M' U' M' U' M U' M' U' M' U' M'`,['M',`M'`,'U',`U'`,`U2'`]]},
  19:{total:12,m:6,u:6,both:0,mExample:[`M U M' U2' M U' M U' M' U2' M' U' M2' U' M' U' M' U' M'`,['M',`M'`,`M2'`]],uExample:[`U' M' U2' M U M U M' U2' M' U M' U M' U' M' U' M' U'`,['U',`U'`,`U2'`]]},
  20:{total:2,m:2,u:2,both:2,bothExample:[`U M U M' U2' M U' M U' M' U2' M' U' M2' U' M' U' M' U' M'`,['M',`M'`,`M2'`,'U',`U'`,`U2'`]]}
};
for(const [distanceText,shell] of Object.entries(outerShells)){
  const distance=Number(distanceText),rows=[];
  if(shell.m>shell.both){const [setup,next]=shell.mExample;rows.push([`${shell.m.toLocaleString()} STATES · M-FIRST`,`Step into distance ${distance-1} on M`,`Choose an M power that reaches the exact shell immediately below. Example setup: <code>${setup}</code>; its optimal M-axis choice${next.length===1?' is':'s are'} <code>${next.join(' or ')}</code>.`,[`M / M' / M2'`,String(distance-1)]]);}
  if(shell.u>shell.both){const [setup,next]=shell.uExample;rows.push([`${shell.u.toLocaleString()} STATES · U-FIRST`,`Step into distance ${distance-1} on U`,`Choose a U power that reaches the exact shell immediately below. Example setup: <code>${setup}</code>; its optimal U-axis choice${next.length===1?' is':'s are'} <code>${next.join(' or ')}</code>.`,[`U / U' / U2'`,String(distance-1)]]);}
  if(shell.both){const [setup,next]=shell.bothExample;rows.push([`${shell.both.toLocaleString()} SHARED STATES · ${shell.total.toLocaleString()} DISTINCT TOTAL`,'Either axis can step down',`Example setup: <code>${setup}</code>. Its exact optimal next moves are <code>${next.join(' or ')}</code>; every one reaches distance ${distance-1}.`,['an M or U power',String(distance-1)]]);}
  explanations[distance]=rows;
}
const unclassifiedShellCounts={
  7:[3523,1826,1802,105],8:[8374,4483,4483,592],9:[17254,8961,8897,604],10:[31575,18007,18007,4439],
  11:[40622,23400,22976,5754],12:[40200,25298,25298,10396],13:[25959,16058,16038,6137],14:[10643,7715,7715,4787],
  15:[2816,1736,1592,512],16:[882,560,560,238],17:[320,214,118,12],18:[80,62,62,44],19:[12,6,6,0],20:[2,2,2,2]
};
for(const [distanceText,[total]] of Object.entries(unclassifiedShellCounts)){
  const distance=Number(distanceText);
  explanations[distance]=[[
    `${total.toLocaleString()} UNCLASSIFIED STATES`,
    'The visual goal families have not been mined yet',
    `This shell almost certainly contains several distinct things to recognize and several different one-move goals—not merely “M-first” and “U-first.” We still need to cluster the cases by visible relationships such as bars, bookends, edge orientation, LR-edge placement, center alignment, adjacency, and relative color. Until those predicates are found and validated, the exact next moves below are lookup evidence rather than human rules.`,
    null
  ]];
}
explanations[7]=[
  ['160 OF 3,523 STATES · 4.54%','Six exact visual families mined so far',`These rules were tested against all distance-seven states. “Turn M until” means inspect the three M alignments and stop when the named visible EO/LR picture appears. Every move that creates the stated picture is optimal for every state admitted by that rule. The families overlap; together they cover 160 distinct states. The remaining 3,363 states are still explicitly unclassified.`,null],
  ['64 STATES','Clear the one-flipped LR case',`When exactly one of the two LR edges is flipped, turn M until EO is <code>3/1</code>, the LR edges are split between U and D, and neither LR edge is flipped. Example setup: <code>M U M' U2' M U' M'</code>; play <code>M</code>.`,['M / M\' / M2\'','6']],
  ['32 NEW STATES','Break up adjacent LR edges',`When the LR edges are adjacent on U, turn M until EO is <code>3/1</code>, the LR edges are split between U and D, and neither LR edge is flipped. Example setup: <code>M U M U2' M U M2'</code>; play <code>M2'</code>.`,['M / M\' / M2\'','6']],
  ['16 STATES','Separate opposite LR edges',`When the LR edges are opposite on U, turn M until EO is adjacent-two-on-U (<code>2a/0</code>), the LR edges are split between U and D, and exactly one LR edge is flipped. Example setup: <code>M' U' M' U' M' U' M'</code>; play <code>M</code>.`,['M / M\' / M2\'','6']],
  ['16 STATES','Lift one LR edge from D',`When both LR edges are on D, turn M until EO is adjacent-two-on-U (<code>2a/0</code>), the LR edges are split between U and D, and exactly one LR edge is flipped. Example setup: <code>M' U' M' U' M' U' M</code>; play <code>M'</code>.`,['M / M\' / M2\'','6']],
  ['16 STATES','Convert opposite EO into the one-one picture',`In a <code>2o/0</code> EO case, turn M until EO becomes <code>1/1</code>, the LR edges are adjacent on U, and exactly one LR edge is flipped. Example setup: <code>M' U' M U' M' U M</code>; play <code>M'</code>.`,['M / M\' / M2\'','6']],
  ['16 STATES','Lift the two D flips into the one-one picture',`In a <code>0/2</code> EO case, turn M until EO becomes <code>1/1</code>, the LR edges are adjacent on U, and exactly one LR edge is flipped. Example setup: <code>M' U' M U' M' U M'</code>; play <code>M</code>.`,['M / M\' / M2\'','6']]
];
function explanation(distance){return `<div class="group-explanation">${explanations[distance].map(([count,title,text,next])=>`<article><span>${count}</span><h3>${title}</h3><p>${text}</p>${next?`<p class="rule-next"><b>Next:</b> <code>${next[0]}</code> <i>→ distance ${next[1]}</i></p>`:''}</article>`).join('')}</div>`;}
function group(distance,count){return `<section class="shell-group" id="distance-${distance}"><header class="shell-title"><h2>Distance ${distance}</h2><span>${count} ${count===1?'state':'states'}</span></header>${explanation(distance)}<details class="shell-cases" data-distance="${distance}" data-count="${count}"><summary><span class="show-label">Show ${count} ${count===1?'diagram':'diagrams'}</span><span class="hide-label">Hide diagrams</span></summary><div class="shell-grid"></div><div class="shell-load" hidden><button type="button">Show next 100</button><span></span></div></details></section>`;}
async function loadShell(details){
  if(details.dataset.loaded)return;
  details.dataset.loaded='loading';
  const distance=Number(details.dataset.distance),grid=details.querySelector('.shell-grid'),controls=details.querySelector('.shell-load'),button=controls.querySelector('button'),status=controls.querySelector('span');
  status.textContent='Loading cases…'; controls.hidden=false;
  try{
    const response=await fetch(`lse-shells-${distance}.json?v=18`);
    if(!response.ok)throw new Error(`${response.status} ${response.statusText}`);
    const states=await response.json();
    let shown=0;
    const renderNext=()=>{
      const end=Math.min(shown+100,states.length);
      grid.insertAdjacentHTML('beforeend',states.slice(shown,end).map((state,index)=>card(state,shown+index)).join(''));
      shown=end;
      status.textContent=`${shown.toLocaleString()} of ${states.length.toLocaleString()} shown`;
      button.textContent=`Show next ${Math.min(100,states.length-shown).toLocaleString()}`;
      if(shown===states.length)button.hidden=true;
    };
    button.addEventListener('click',renderNext);
    details.dataset.loaded='true';
    renderNext();
  }catch(error){details.dataset.loaded='';controls.innerHTML=`<span>Could not load cases: ${error.message}</span>`;}
}
document.querySelector('#shells').addEventListener('toggle',event=>{const details=event.target;if(details.matches?.('.shell-cases')&&details.open)loadShell(details);},true);
fetch('lse-shells-index.json?v=18').then(response=>{if(!response.ok)throw new Error(`${response.status} ${response.statusText}`);return response.json();}).then(index=>{document.querySelector('#shells').innerHTML=index.map(shell=>group(shell.distance,shell.count)).join('');}).catch(error=>{document.querySelector('#shells').innerHTML=`<p>Could not load shell index: ${error.message}</p>`;});
