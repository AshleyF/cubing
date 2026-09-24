export function turn(move){const match=move.match(/^(.*?)(2|')?$/);return{base:match[1],amount:match[2]==='2'?2:match[2]==="'"?3:1}}
export function notation(base,amount){amount=((amount%4)+4)%4;return amount===0?'':base+(amount===1?'':amount===2?'2':"'")}

export const cancellationBases=['U','D','L','R','F','B','M','E','S','x','y','z','u','d','l','r','f','b','Uw','Dw','Lw','Rw','Fw','Bw'];
const cancellationAmounts=[1,2,3];
const layerMoves={
 L:['x',1,0,0],M:['x',0,1,0],R:['x',0,0,1],l:['x',1,1,0],Lw:['x',1,1,0],r:['x',0,3,1],Rw:['x',0,3,1],x:['x',3,3,1],
 D:['y',1,0,0],E:['y',0,1,0],U:['y',0,0,1],d:['y',1,1,0],Dw:['y',1,1,0],u:['y',0,3,1],Uw:['y',0,3,1],y:['y',3,3,1],
 B:['z',1,0,0],S:['z',0,1,0],F:['z',0,0,1],b:['z',1,3,0],Bw:['z',1,3,0],f:['z',0,1,1],Fw:['z',0,1,1],z:['z',3,1,1]
};
const canonicalBases=['L','M','R','l','r','x','D','E','U','d','u','y','B','S','F','b','f','z'];
const vectorKey=(axis,values)=>`${axis}:${values.map(value=>(value%4+4)%4).join('')}`;
const singleMoveByVector=new Map(canonicalBases.flatMap(base=>cancellationAmounts.map(amount=>{const[axis,...values]=layerMoves[base];return[vectorKey(axis,values.map(value=>value*amount)),notation(base,amount)]})));

export function reducePair(left,right){const a=turn(left),b=turn(right),av=layerMoves[a.base],bv=layerMoves[b.base];if(!av||!bv||av[0]!==bv[0])return null;const values=[0,1,2].map(i=>av[i+1]*a.amount+bv[i+1]*b.amount),key=vectorKey(av[0],values);if(values.every(value=>value%4===0))return'';return singleMoveByVector.get(key)??null}

export function cancellationCases(){return cancellationBases.flatMap(leftBase=>cancellationAmounts.flatMap(leftAmount=>cancellationBases.flatMap(rightBase=>cancellationAmounts.map(rightAmount=>{const left=notation(leftBase,leftAmount),right=notation(rightBase,rightAmount),reduced=reducePair(left,right);return reduced===null?null:[`${left} ${right}`,reduced]})))).filter(Boolean)}

const identity=[1,0,0,0,1,0,0,0,1],rotationMatrices={x:[1,0,0,0,0,-1,0,1,0],y:[0,0,1,0,1,0,-1,0,0],z:[0,1,0,-1,0,0,0,0,1]};
const multiply=(a,b)=>[0,1,2].flatMap(row=>[0,1,2].map(column=>[0,1,2].reduce((sum,k)=>sum+a[row*3+k]*b[k*3+column],0)));
const power=(matrix,amount)=>{let result=identity;for(let i=0;i<amount;i++)result=multiply(matrix,result);return result};
const orientationKey=moves=>moves.reduce((matrix,move)=>{const{base,amount}=turn(move);return multiply(power(rotationMatrices[base],amount),matrix)},identity).join(',');
const rotationTokens=['x',"x'",'x2','y',"y'",'y2','z',"z'",'z2'];
const shortestOrientation=new Map([[orientationKey([]),[]]]);
for(const first of rotationTokens){const one=[first];if(!shortestOrientation.has(orientationKey(one)))shortestOrientation.set(orientationKey(one),one)}
for(const first of rotationTokens)for(const second of rotationTokens){const two=[first,second];if(!shortestOrientation.has(orientationKey(two)))shortestOrientation.set(orientationKey(two),two)}
export function simplifyOrientation(moves){return shortestOrientation.get(orientationKey(moves))??moves}

export function simplifyLeadingOrientation(entries){let rotationCount=0;while(rotationCount<entries.length&&rotationMatrices[entries[rotationCount].base])rotationCount++;if(rotationCount>1){const replacement=simplifyOrientation(entries.slice(0,rotationCount).map(entry=>entry.move)),stageIndex=entries[rotationCount-1].stageIndex;entries.splice(0,rotationCount,...replacement.map(move=>({...turn(move),move,stageIndex})))}return entries}

export function cancelMoves(stages){const stack=[];for(const[stageIndex,stage]of stages.entries())for(const move of stage.moves.trim().split(/\s+/).filter(Boolean)){let current=move;while(stack.length){const reduced=reducePair(stack.at(-1).move,current);if(reduced===null)break;stack.pop();current=reduced;if(!current)break}if(current)stack.push({...turn(current),move:current,stageIndex})}return simplifyLeadingOrientation(stack)}
