const rows=document.getElementById('reportRows');
const format=value=>Number(value).toFixed(2);
fetch('benchmark-results.json').then(async response=>{if(!response.ok)throw Error('Benchmark results are not ready');return response.json()}).then(data=>{
 const first=data.rows[0],last=data.rows.at(-1),span=first.average-last.average||1;
 document.getElementById('startAverage').textContent=format(first.average);
 document.getElementById('bestAverage').textContent=format(last.average);
 document.getElementById('methodText').textContent=`Every row uses the same ${data.sampleSize.toLocaleString()} deterministic 25-move scrambles (seed ${data.seed}). Each optimization is cumulative except the explicitly labeled center comparison. Move counts are STM; cancellations combine adjacent turns of the same slice.`;
 rows.innerHTML=data.rows.map((row,index)=>`<article class="report-row"><div><b>${index+1}</b><span><strong>${row.name}</strong><small>${row.note}</small></span></div><strong class="average">${format(row.average)}</strong><strong class="saved">${index?`−${format(row.saved)}`:'—'}</strong><span class="range">${row.minimum}–${row.maximum}</span><i style="--width:${Math.max(2,(first.average-row.average)/span*100)}%"></i></article>`).join('');
}).catch(error=>{rows.innerHTML=`<p class="report-error">${error.message}</p>`});
