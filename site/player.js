var f = annotation.frames.first,
    to = annotation.frames.last,
    playing = false,
    auto = false,
    speed = 1,
    restart = false;

function next() {
    playing = !restart && playing && f < to && step(1);
    if (playing) {
        playing = true;
        window.setTimeout(next, 1000 / annotation.frames.fps * speed);
    }
    restart = false;
}

function step(i) {
    restart = playing;
    playing = false;
    auto = false;
    f += i;
    var success = (f >= annotation.frames.first && f <= annotation.frames.last);
    if (!success) {
        f = Math.min(annotation.frames.last, Math.max(annotation.frames.first, f));
    }
    renderFrame(f);
    return success;
}

function playTo(f, s) {
    restart = playing;
    playing = true;
    speed = s;
    auto = false;
    to = f;
    next();
}

function jumpToPreviousMark() {
    var n = annotation.frames.first;
    for (var i in annotation.marks) {
        var e = annotation.marks[i];
        if (f - 1 > e.frame) n = e.frame; else break;
    }
    jump(n);
}

function playToNextMark(movesOnly) {
    for (var i in annotation.marks) {
        var e = annotation.marks[i];
        if (e.frame > f && (!movesOnly || (e.type != 'idle' && e.type != 'regrip'))) {
            playTo(e.frame, 1);
            return true;
        }
    }
    playTo(annotation.frames.last, 1);
    return false;
}

function playToNextMarksAuto() {
    function nextMark() {
        if (playing && playToNextMark(true)) {
            window.setTimeout(nextMark, 3000);
        }
    }
    restart = playing;
    playing = true;
    auto = true;
    speed = 1;
    next();
}


function play(speed) {
    playTo(annotation.frames.last, speed);
}

function rewind() {
    playing = restart = false;
    auto = false;
    f = annotation.frames.first;
    renderFrame(f);
}

function pause() {
    playing = restart = false;
    auto = false;
}

function jump(i) {
    playing = restart = false;
    auto = false;
    f = i;
    renderFrame(f);
}