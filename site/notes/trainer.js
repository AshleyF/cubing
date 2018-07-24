function load() {
    document.getElementById('diagram').innerHTML = diagram();
}

var cube = new GiikerCube();

function connect() {
    cube.connect();
}

var stage = 1; // HACK: giiker.js patchTwists() depends on this
var alg, expectedAlg, auf, expectedAuf, colors, hint, started, recoStart, algStart, manualAdvance;

function init() {
    alg = ""; 
    expectedAlg = "";
    auf = null;
    expectedAuf = [];
    colors = ".....................";
    hint = "";
    started = false;
    recoStart = null;
    algStart = null;
    manualAdvance = false;
}
init();
manualAdvance = true;

function setManualAdvance() {
    manualAdvance = true;
}

function render() {
    var match = "#08F"; // blue
    var aufError = " ";
    var time = "";
    if (alg != "") {
        if (alg == expectedAlg) match = "#0C0"; // green
        else if (expectedAlg.indexOf(alg.substring(0, alg.length - 1)) != -1) match = "#FF0";
        else if (alg.indexOf("U' ") == alg.length - 3 && expectedAlg.indexOf(alg.substring(0, alg.length - 2)) != -1) match = "#FF0";
        else match = "#F10"; // red
    }
    var goodAuf = expectedAuf.length == 0;
    for (var a in expectedAuf) {
        var e = expectedAuf[a];
        if ((auf == null && alg == "") || (auf == null && e == 0) || auf == e || (e == 2 && auf == 0)) goodAuf = true;
    }
    if (!goodAuf) {
        match = "#F10"; // red
        aufError = "(AUF?) ";
    }
    if (match == "#0C0") {
        window.setTimeout(setManualAdvance, 500); // auto-advance
        var elapsedReco = (((new Date()) - recoStart) - ((new Date()) - algStart)) / 1000.0;
        var elapsedAlg = ((new Date()) - algStart) / 1000.0;
        time = "(" + elapsedReco + "/" + elapsedAlg + ")";
    }
    document.getElementById("alg").innerHTML = '<span style="color:' + match + '">' + (auf == -1 ? "(AUF: U') " : auf == 1 ? "(AUF: U) " : auf == 2 ? "(AUF: U2) " : aufError) + alg + '</span>' + time;
    setColors(colors);
    document.getElementById("hint").innerText = hint;
}

function reset() {
    init();
    render();
}

function setCase(c) {
    colors = c.colors;
    expectedAlg = c.alg + ' ';
    expectedAuf = c.auf;
    hint = "Hint: " + c.name + " - " + (c.display || c.alg);
    render();
    recoStart = new Date();
}

function cloneCase(c) {
    return {
        name: c.name,
        colors: c.colors,
        alg: c.alg,
        auf: c.auf,
        display: c.display
    };
}

var current = 0;
function next() {
    init();
    setCase(cases[current++]);
    if (current >= cases.length) current = 0;
}

function onGiikerChanged(colors, twist) {
    if (manualAdvance) {
        manualAdvance = false;
        nextCase();
        return;
    }
    if (!started) {
        started = true;
        algStart = new Date();
    }
    console.log(twist);
    if (alg == "" && (twist == "U" || twist == "U'")) {
        auf += twist == "U" ? 1 : -1;
        if (auf == 3) auf = -1;
        else if (auf == -2) auf = 2;
        else if (auf == -3) auf = 1;
        console.log(auf);
    } else {
        alg = patchTwists(alg + twist + " ", true);
    }
    render();
}

function colorToCss(c) {
    switch (c) {
        case 'R': return "#F10";
        case 'O': return "#F90";
        case 'Y': return "#EF0";
        case 'W': return "#FFF";
        case 'B': return "#08F";
        case 'G': return "#0C0";
        case 'P': return "#B4F";
        case '*': return "#000";
        default: return "#333";
    }
}