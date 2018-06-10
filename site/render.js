
function renderInfo() {
    var person = annotation.person.name;
    var event = annotation.event.name;
    document.getElementById('info').innerHTML = person + ', ' + event;
}

function renderScramble() {
    document.getElementById('scramble').innerHTML = 'Scramble: ' + annotation.scramble;
}

function renderMarks() {
    var htm = '';
    for (var i in annotation.marks) {
        var e = annotation.marks[i];
        htm += '<a href="javascript:jump(' + e.frame + ')">' + e.name + '</a> '
    }
    document.getElementById('marks').innerHTML = htm;
}

var frame;

function init() {
    frame = document.getElementById('frame');
    jump(annotation.frames.first);
    renderInfo();
    renderScramble();
    renderMarks();
}

function loadSolve(name) {
    var script = document.getElementById('annotation'); // document.createElement('script');
    document.head.removeChild(script);
    script = document.createElement('script');
    script.id = 'annotation';
    document.head.appendChild(script);
    script.onload = init;
    script.src = './solves/' + name + '/annotation.js';
}

function getTime(frames) {
    return (frames / annotation.frames.fps).toFixed(2);
}

function renderTime(f) {
    function render(label, note, timerFrames, solveFrames) {
        var solve = solveFrames != null ? ' (' + getTime(solveFrames) + ')' : '';
        document.getElementById('time').innerHTML = label + ': ' + note + ' - ' + getTime(timerFrames) + solve;
    }
    var inspectionFrame = 0,
        timerStartFrame = 0,
        timerStopFrame = 0,
        solveStartFrame = 0,
        solveStopFrame = 0,
        note = '';
    for (var i in annotation.marks) {
        var e = annotation.marks[i];
        if (f >= e.frame) note = e.name + (e.note ? ' (' + e.note + ')' : '');
        switch (e.type) {
            case 'inspection-start': inspectionFrame = e.frame; break;
            case 'timer-start': timerStartFrame = e.frame; break;
            case 'timer-stop': timerStopFrame = e.frame; break;
            case 'solve-start': solveStartFrame = e.frame; break;
            case 'solve-stop': solveStopFrame = e.frame; break;
        }
    }
    if (f >= timerStopFrame) {
        render('Solved', note, (annotation.time ? annotation.time * annotation.frames.fps : timerStopFrame - timerStartFrame), solveStopFrame - solveStartFrame);
    } else if (f >= timerStartFrame) {
        render('Solving', note, f - timerStartFrame, f - solveStartFrame);
    } else if (f >= inspectionFrame) {
        render('Inspection', note, f - inspectionFrame, null); // TODO: hilight 8 & 12 sec.
    } else {
        document.getElementById('time').innerHTML = 'Pre-solve ' + note;
    }
}

function renderStats(f) {
    var twistFrames = 0,
        twistCount = 0,
        rotationFrames = 0,
        rotationCount = 0,
        regripFrames = 0,
        lockupFrames = 0,
        idleFrames = 0,
        solving = false,
        last = null;
    for (var i in annotation.marks) {
        var e = annotation.marks[i];
        if (solving) {
            var frames = (e.frame > f ? f : e.frame) - last.frame;
            switch (last.type) {
                case 'twist': twistFrames += frames; twistCount++; break;
                case 'rotate': rotationFrames += frames; rotationCount++; break;
                case 'idle': idleFrames += frames; break;
                case 'regrip': regripFrames += frames; break;
                case 'lockup': lockupFrames += frames; break;
                case 'solve-start': break;
                case 'solve-stop': solving = false; break;
                default: throw 'Unknown mark type during solving: ' + last.type;
            }
        }
        if (e.type == 'solve-start') {
            solving = true;
        }
        last = e;
        if (e.frame > f) break;
    }
    var twists = 'Twists: ' + getTime(twistFrames) + ' (' + twistCount + ')';
    var rotations = 'Rotations: ' + getTime(rotationFrames) + ' (' + rotationCount + ')';
    var regrips = 'Regrips: ' + getTime(regripFrames);
    var lockups = 'Lockups: ' + getTime(lockupFrames);
    var idle = 'Idle/Looking: ' + getTime(idleFrames);
    document.getElementById('stats').innerHTML = twists + '<br />' + rotations + '<br />' + regrips + '<br />' + lockups + '<br />' + idle;
}

function renderFrame(f) {
    frame.src = 'solves/' + annotation.frames.base + ('000' + f).slice(-4) + '.jpg';
    document.getElementById('debug').innerText = 'Frame: ' + f;
    renderTime(f);
    renderStats(f);
}