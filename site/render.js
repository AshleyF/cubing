
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
        render('Solved', note, annotation.time * annotation.frames.fps, solveStopFrame - solveStartFrame);
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
        lockupFrames = 0,
        idleFrames = 0,
        solving = false,
        last = null;
    for (var i in annotation.marks) {
        var e = annotation.marks[i];
        if (e.frame > f) break;
        if (solving) {
            var frames = e.frame - last.frame;
            switch (e.type) {
                case 'twist': twistFrames += frames; twistCount++; break;
                case 'rotate': rotationFrames += frames; rotationCount++; break;
                case 'idle': idleFrames += frames; break;
                case 'lockup': lockupFrames += frames; break;
                case 'solve-stop': solving = false; break;
                default: throw "Unknown mark type during solving.";
            }
        }
        if (e.type == 'solve-start') {
            solving = true;
        }
        last = e;
    }
    var twists = 'Twists: ' + getTime(twistFrames) + ' (' + twistCount + ')';
    var rotations = 'Rotations: ' + getTime(rotationFrames) + ' (' + rotationCount + ')';
    var lockups = 'Lockups: ' + getTime(lockupFrames);
    var idle = 'Idle: ' + getTime(idleFrames);
    document.getElementById('stats').innerHTML = twists + '<br />' + rotations + '<br />' + lockups + '<br />' + idle;
}

function renderFrame(f) {
    frame.src = 'solves/' + annotation.frames.base + f + '.jpg';
    document.getElementById('debug').innerText = 'Frame: ' + f;
    renderTime(f);
    renderStats(f);
}