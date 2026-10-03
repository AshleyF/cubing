const pieceColors = [
  ['#f5f3e9','#ef7d16'], // UL
  ['#f5f3e9','#d72d2d'], // UR
  ['#f5f3e9','#16a05d'], // UF
  ['#f5f3e9','#2474d8'], // UB
  ['#f4d928','#16a05d'], // DF
  ['#f4d928','#2474d8']  // DB
];
const pieceNames = ['UL','UR','UF','UB','DF','DB'];

function edgeColors(state, position) {
  const colors=[...pieceColors[state.pieces[position]]];
  return state.flips[position] ? colors.reverse() : colors;
}

function polygon(points,fill,highlight=false) {
  return `<polygon points="${points}" fill="${fill}" stroke="${highlight?'#080808':'#2b2b2b'}" stroke-width="${highlight?'.035':'.012'}" stroke-linejoin="round"/>`;
}

// Perspective geometry adapted from BriefCubing's Display.diagramUF renderer.
const topFace = {
  bl:'-.476,-.691 -.192,-.691 -.199,-.511 -.497,-.511',
  b:'-.141,-.691 .142,-.691 .149,-.511 -.148,-.511',
  br:'.194,-.691 .477,-.691 .498,-.511 .201,-.511',
  l:'-.499,-.477 -.202,-.477 -.210,-.278 -.522,-.278',
  c:'-.148,-.477 .149,-.477 .157,-.278 -.156,-.278',
  r:'.203,-.477 .501,-.477 .524,-.278 .211,-.278',
  fl:'-.525,-.241 -.212,-.241 -.221,-.020 -.551,-.020',
  f:'-.156,-.241 .157,-.241 .165,-.020 -.164,-.020',
  fr:'.214,-.241 .527,-.241 .552,-.020 .222,-.020'
};
const front = {
  tl:'-.552,.020 -.222,.020 -.214,.241 -.527,.241',
  t:'-.166,.020 .164,.020 .156,.241 -.157,.241',
  tr:'.221,.020 .551,.020 .525,.241 .212,.241',
  l:'-.524,.278 -.211,.278 -.203,.477 -.501,.477',
  c:'-.157,.278 .156,.278 .148,.477 -.149,.477',
  r:'.210,.278 .522,.278 .499,.477 .202,.477',
  bl:'-.498,.511 -.201,.511 -.194,.691 -.477,.691',
  b:'-.149,.511 .148,.511 .141,.691 -.142,.691',
  br:'.199,.511 .497,.511 .476,.691 .192,.691'
};

function centerColors(offset) {
  return [
    ['#f5f3e9','#16a05d'], ['#2474d8','#f5f3e9'],
    ['#f4d928','#2474d8'], ['#16a05d','#f4d928']
  ][offset];
}

function diagram(state,label) {
  const colors=[0,1,2,3,4,5].map(position=>edgeColors(state,position));
  const centers=centerColors(state.center);
  const highlight=position=>state.pieces[position]<2;
  const gray='#d1cec4', dark='#aaa69b';
  return `<svg class="lse-diagram" viewBox="-.82 -.87 1.64 1.75" role="img" aria-label="${label}">
    <g>${polygon('-.547,0 .547,0 .47,.665 -.47,.665','#171717')}${polygon('-.47,-.665 .47,-.665 .547,0 -.547,0','#171717')}</g>
    <g>
      ${polygon(topFace.bl,dark)}${polygon(topFace.b,colors[3][0],highlight(3))}${polygon(topFace.br,dark)}
      ${polygon(topFace.l,colors[0][0],highlight(0))}${polygon(topFace.c,centers[0])}${polygon(topFace.r,colors[1][0],highlight(1))}
      ${polygon(topFace.fl,dark)}${polygon(topFace.f,colors[2][0],highlight(2))}${polygon(topFace.fr,dark)}
      ${polygon(front.tl,dark)}${polygon(front.t,colors[2][1],highlight(2))}${polygon(front.tr,dark)}
      ${polygon(front.l,gray)}${polygon(front.c,centers[1])}${polygon(front.r,gray)}
      ${polygon(front.bl,dark)}${polygon(front.b,colors[4][1],highlight(4))}${polygon(front.br,dark)}
      ${polygon('-.620,-.477 -.552,-.477 -.576,-.279 -.647,-.278',colors[0][1],highlight(0))}
      ${polygon('.553,-.477 .623,-.477 .650,-.278 .578,-.278',colors[1][1],highlight(1))}
      ${polygon('-.149,.708 .141,.708 .132,.806 -.137,.806',colors[4][0],highlight(4))}
    </g>
    <g class="back-edge" aria-label="DB edge shown behind cube">
      <text x="0" y="-.795" text-anchor="middle">DB</text>
      ${polygon('-.105,-.845 0,-.812 .105,-.845 0,-.878',colors[5][1],highlight(5))}
      ${polygon('-.105,-.845 0,-.812 0,-.755 -.105,-.787',colors[5][0],highlight(5))}
    </g>
    <g class="position-labels"><text x="-.66" y="-.37">UL</text><text x=".66" y="-.37">UR</text><text x="0" y=".79">DF</text></g>
  </svg>`;
}

function friendlier(text) {
  return text.replaceAll('U-side','side of U').replaceAll('U-M','M slots of U').replaceAll('; ', ' · ');
}

function card(example,index) {
  return `<article class="rule-card">
    <header class="rule-title"><h2>${index+1}. ${example.schema}</h2><span>covers ${example.representedStates} concrete states</span></header>
    <div class="rule-visual">
      <section class="state"><h3>Recognize</h3>${diagram(example.before,'Before '+example.action)}<p class="state-meta">center ${example.before.center} · AUF ${example.before.auf} · ${example.before.distance} moves away</p></section>
      <div class="transition"><strong>${example.action}</strong><span>exact reduction →</span></div>
      <section class="state"><h3>Continue from</h3>${diagram(example.after,'After '+example.action)}<p class="state-meta">center ${example.after.center} · AUF ${example.after.auf} · ${example.after.distance} moves away</p></section>
    </div>
    <div class="rule-summary">
      <p><b>Source family</b>${friendlier(example.source)}</p>
      <p><b>Target family</b>${friendlier(example.target)}</p>
      <details><summary>What is being proved here?</summary><p>For every one of the ${example.representedStates} states sharing the source description, at least one choice of powers in <b>${example.schema}</b> stays on a shortest path and reaches the target description. This card shows one concrete choice: <b>${example.action}</b>, reducing exact distance from ${example.before.distance} to ${example.after.distance}.</p></details>
    </div>
  </article>`;
}

fetch('lse-rule-examples.json')
  .then(response => { if(!response.ok) throw new Error(`${response.status} ${response.statusText}`); return response.json(); })
  .then(examples => { document.querySelector('#rule-examples').innerHTML=examples.map(card).join(''); })
  .catch(error => { document.querySelector('#rule-examples').innerHTML=`<p>Could not load examples: ${error.message}</p>`; });
