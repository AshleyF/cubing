var f = annotation.frames.first,
    to = annotation.frames.last,
    playing = false,
    auto = false;

function next() {
    if (playing && f < to && step(1)) {
        playing = true;
        window.setTimeout(next, 1000 / annotation.frames.fps);
    }
}

function step(i) {
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

function playTo(f) {
    playing = true;
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

function playToNextMark() {
    for (var i in annotation.marks) {
        var e = annotation.marks[i];
        if (e.frame > f) {
            playTo(e.frame);
            return true;
        }
    }
    playTo(annotation.frames.last);
    return false;
}

function playToNextMarksAuto() {
    function next() {
        if (playing && playToNextMark()) {
            window.setTimeout(next, 3000);
        }
    }
    playing = true;
    auto = true;
    next();
}


function play() {
    playTo(annotation.frames.last);
}

function rewind() {
    playing = false;
    auto = false;
    f = annotation.frames.first;
    renderFrame(f);
}

function pause() {
    playing = false;
    auto = false;
}

function jump(i) {
    playing = false;
    auto = false;
    f = i;
    renderFrame(f);
}