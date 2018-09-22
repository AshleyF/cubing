/*
Roofpig integration
Assumes an element with ID of "cube" in which to render.
*/

var corners = ["UBL", "ULF", "UFR", "URB", "DLB", "DFL", "DRF", "DBR"]
var edges = ["UB", "UL", "UF", "UR", "BL", "FL", "FR", "BR", "DB", "DL", "DF", "DR"];

function display(cube) {
    function twistColors(colors, corner, twist) {
        var c0 = colors[0];
        var c1 = colors[1];
        var c2 = colors[2];
        var lastToFront = corner == 0 || corner == 2 || corner == 5 || corner == 7;
        switch (twist) {
            case 1: return lastToFront ? c2 + c0 + c1 : c1 + c2 + c0;
            case 2: return lastToFront ? c1 + c2 + c0 : c2 + c0 + c1;
            case 3: return colors; // no twist
        }
    }
    function side(s) {
        return "udlrfb"[s];
    }
    function render(config) {
        document.getElementById("cube").innerHTML = "";
        CubeAnimation.create_in_dom('#cube', config, "class='roofpig'");
    }

    var tweaks = "flags=canvas|pov=" + side(cube.v[0]).toUpperCase() + side(cube.v[4]) + side(cube.v[3]) + "|hover=3|solved=*|tweaks=";

    // centers
    tweaks += "U:U D:D L:L R:R F:F B:B "
    // corners
    for (var c = 0; c < 8; c++) {
        var p = cube.c[c].p;
        if (p != 0) {
            var colors = twistColors(corners[p - 1], c, cube.c[c].o);
            var targets = corners[c].toLowerCase();
            var c0 = colors[0];
            var c1 = colors[1];
            var c2 = colors[2];
            var t0 = targets[0];
            var t1 = targets[1];
            var t2 = targets[2];
            tweaks += c0 + ':' + t0.toUpperCase() + t1 + t2 + ' ';
            tweaks += c1 + ':' + t0 + t1.toUpperCase() + t2 + ' ';
            tweaks += c2 + ':' + t0 + t1 + t2.toUpperCase() + ' ';
        }
    }
    // edges
    for (var e = 0; e < 12; e++) {
        var p = cube.e[e].p;
        if (p != 0) {
            var colors = edges[p - 1];
            var flipped = cube.e[e].o == 2;
            var c0 = flipped ? colors[1] : colors[0];
            var c1 = flipped ? colors[0] : colors[1];
            var edge = edges[e].toLowerCase();
            tweaks += c0 + ':' + edge[0].toUpperCase() + edge[1] + ' ';
            tweaks += c1 + ':' + edge[0] + edge[1].toUpperCase() + ' ';
        }
    }
    render(tweaks);
}