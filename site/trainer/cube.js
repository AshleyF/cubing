/*
Cube representation. Eight corners (c) with three orientations. Twelve edges (e) which may be flipped.
Separately (not strictly part of the cube), a view (v) specifying U, D, L, R, F, B. This controls the "camera" when displaying.

Corners (above view);

  Permutation: Top: 1 4  Bottom: 5 8
                    2 3          6 7

  Orientation: 1 = U/D on F/B
               2 = U/D on L/R
               3 = U/D on U/D (oriented)

     1   2   3
  1 LUB BLU UBL (last to front)
  2 LFU FUL ULF (first to back)
  3 RUF FRU UFR (last to front)
  4 RBU BUR URB (first to back)
  5 LBD BDL DLB (first to back)
  6 LDF FLD DFL (last to front)
  7 RFD FDR DRF (first to back)
  8 RDB BRD DBR (last to front)

Edges (above view):

  Permutation: Top:   1    Middle: 5  8  Bottom:    9
                    2   4                        10  12
                      3            6  7            11

  Orientation: F/B moves flip (not F2/B2, which is like F F; canceling)

Default cube orientation: green top/white front
*/

var solved = {
    c: [{ p: 1, o: 3 },
        { p: 2, o: 3 },
        { p: 3, o: 3 },
        { p: 4, o: 3 },
        { p: 5, o: 3 },
        { p: 6, o: 3 },
        { p: 7, o: 3 },
        { p: 8, o: 3 }],
    e: [{ p: 1, o: 1 },
        { p: 2, o: 1 },
        { p: 3, o: 1 },
        { p: 4, o: 1 },
        { p: 5, o: 1 },
        { p: 6, o: 1 },
        { p: 7, o: 1 },
        { p: 8, o: 1 },
        { p: 9, o: 1 },
        { p: 10, o: 1 },
        { p: 11, o: 1 },
        { p: 12, o: 1 }],
    v: [0, 1, 2, 3, 4, 5]
}

function twist(notation, cube) {
    function map(mapping, cube) {
        // corners
        function co(m, o) {
            if (m) {
                switch (m) {
                    case 0: return o;
                    case 1: return o == 1 ? 1 : o == 2 ? 3 : 2;
                    case 2: return o == 2 ? 2 : o == 1 ? 3 : 1;
                    case 3: return o == 3 ? 3 : o == 2 ? 1 : 2;
                }
            } else return o;
        }
        var cs = cube.c.slice(0);
        if (mapping.c) {
            for (var j in mapping.c) {
                var m = mapping.c[j];
                var i = m.i;
                var n = m.p || i;
                var c = cube.c[n - 1];
                c.o = co(m.o, c.o);
                cs[i - 1] = c;
            }
        }
        // edges
        function eo(m, o) {
            if (m) {
                return m == 1 ? o == 1 ? 2 : 1 : o;
            } else return o;
        }
        var es = cube.e.slice(0);
        if (mapping.e) {
            for (var j in mapping.e) {
                var m = mapping.e[j];
                var i = m.i;
                var n = m.p || i;
                var e = cube.e[n - 1];
                e.o = eo(m.o, e.o);
                es[i - 1] = e;
            }
        }
        // view
        var vs = cube.v.slice(0);
        if (mapping.v) {
            var m = mapping.v;
            for (var i = 0; i < 6; i++) {
                vs[i] = cube.v[m[i]];
            }
        }
        return { c: cs, e: es, v: vs };
    }
    var maps = [ // U, D, L, R, F, B twists in original orientation (remapped as necessary below)
        { c: [{ i: 1, p: 2, o: 3 }, { i: 2, p: 3, o: 3 }, { i: 3, p: 4, o: 3 }, { i: 4, p: 1, o: 3 }],
        e: [{ i: 1, p: 2 }, { i: 2, p: 3 }, { i: 3, p: 4 }, { i: 4, p: 1 }]},
        { c: [{ i: 5, p: 8, o: 3 }, { i: 6, p: 5, o: 3 }, { i: 7, p: 6, o: 3 }, { i: 8, p: 7, o: 3 }],
        e: [{ i: 9, p: 12 }, { i: 10, p: 9 }, { i: 11, p: 10 }, { i: 12, p: 11 }]},
        { c: [{ i: 1, p: 5, o: 2 }, { i: 2, p: 1, o: 2 }, { i: 5, p: 6, o: 2 }, { i: 6, p: 2, o: 2 }],
        e: [{ i: 2, p: 5 }, { i: 6, p: 2 }, { i: 10, p: 6 }, { i: 5, p: 10 }]},
        { c: [{ i: 3, p: 7, o: 2 }, { i: 4, p: 3, o: 2 }, { i: 7, p: 8, o: 2 }, { i: 8, p: 4, o: 2 }],
        e: [{ i: 4, p: 7 }, { i: 7, p: 12 }, { i: 8, p: 4 }, { i: 12, p: 8 }]},
        { c: [{ i: 2, p: 6, o: 1 }, { i: 3, p: 2, o: 1 }, { i: 6, p: 7, o: 1 }, { i: 7, p: 3, o: 1 }],
        e: [{ i: 3, p: 6, o: 1 }, { i: 6, p: 11, o: 1 }, { i: 7, p: 3, o: 1 }, { i: 11, p: 7, o: 1 }]},
        { c: [{ i: 1, p: 4, o: 1 }, { i: 4, p: 8, o: 1 }, { i: 5, p: 1, o: 1 }, { i: 8, p: 5, o: 1 }],
        e: [{ i: 1, p: 8, o: 1 }, { i: 5, p: 1, o: 1 }, { i: 8, p: 9, o: 1 }, { i: 9, p: 5, o: 1 }]}];
    var u = maps[cube.v[0]];
    var d = maps[cube.v[1]];
    var l = maps[cube.v[2]];
    var r = maps[cube.v[3]];
    var f = maps[cube.v[4]];
    var b = maps[cube.v[5]];
    var x = { v: [4, 5, 2, 3, 1, 0] };
    var y = { v: [0, 1, 4, 5, 3, 2] };
    var z = { v: [2, 3, 1, 0, 4, 5] };
    switch (notation) {
        case "U": return map(u, cube);
        case "U2": return map(u, map(u, cube));
        case "U'": return map(u, map(u, map(u, cube)));
        case "D": return map(d, cube);
        case "D2": return map(d, map(d, cube));
        case "D'": return map(d, map(d, map(d, cube)));
        case "L": return map(l, cube);
        case "L2": return map(l, map(l, cube));
        case "L'": return map(l, map(l, map(l, cube)));
        case "R": return map(r, cube);
        case "R2": return map(r, map(r, cube));
        case "R'": return map(r, map(r, map(r, cube)));
        case "F": return map(f, cube);
        case "F2": return map(f, map(f, cube));
        case "F'": return map(f, map(f, map(f, cube)));
        case "B": return map(b, cube);
        case "B2": return map(b, map(b, cube));
        case "B'": return map(b, map(b, map(b, cube)));
        case "M": return map(l, map(l, map(l, map(r, map(x, map(x, map(x, cube)))))));
        case "M2": return map(l, map(l, map(r, map(r, map(x, map(x, cube))))));
        case "M'": return map(l, map(r, map(r, map(r, map(x, cube)))));
        case "E": return map(u, map(d, map(d, map(d, map(y, map(y, map(y, cube)))))));
        case "E2": return map(u, map(u, map(d, map(d, map(y, map(y, cube))))));
        case "E'": return map(u, map(u, map(u, map(d, map(y, cube)))));
        case "S": return map(f, map(f, map(f, map(b, map(z, cube)))));
        case "S2": return map(f, map(f, map(b, map(b, map(z, map(z, cube))))));
        case "S'": return map(f, map(b, map(b, map(b, map(z, map(z, map(z, cube)))))));
        // relative cube orientations
        case "x": return map(x, cube);
        case "x2": return map(x, map(x, cube));
        case "x'": return map(x, map(x, map(x, cube)));
        case "y": return map(y, cube);
        case "y2": return map(y, map(y, cube));
        case "y'": return map(y, map(y, map(y, cube)));
        case "z": return map(z, cube);
        case "z2": return map(z, map(z, cube));
        case "z'": return map(z, map(z, map(z, cube)));
        // absolute cube orientations (U/F color pair) // TODO: Handle these some other way
        case "yr": return map({ v: [0, 1, 2, 3, 4, 5] }, cube);
        case "ry": return map({ v: [4, 5, 3, 2, 0, 1] }, cube);
        case "yg": return map({ v: [0, 1, 4, 5, 3, 2] }, cube);
        case "gy": return map({ v: [3, 2, 5, 4, 0, 1] }, cube);
        case "yo": return map({ v: [0, 1, 3, 2, 5, 4] }, cube);
        case "oy": return map({ v: [5, 4, 2, 3, 0, 1] }, cube);
        case "yb": return map({ v: [0, 1, 5, 4, 2, 3] }, cube);
        case "by": return map({ v: [2, 3, 4, 5, 0, 1] }, cube);
        case "gr": return map({ v: [3, 2, 0, 1, 4, 5] }, cube);
        case "rg": return map({ v: [4, 5, 1, 0, 3, 2] }, cube);
        case "og": return map({ v: [5, 4, 0, 1, 3, 2] }, cube);
        case "go": return map({ v: [3, 2, 1, 0, 5, 4] }, cube);
        case "bo": return map({ v: [2, 3, 0, 1, 5, 4] }, cube);
        case "ob": return map({ v: [5, 4, 1, 0, 2, 3] }, cube);
        case "rb": return map({ v: [4, 5, 0, 1, 2, 3] }, cube);
        case "br": return map({ v: [2, 3, 1, 0, 4, 5] }, cube);
        case "wr": return map({ v: [1, 0, 3, 2, 4, 5] }, cube);
        case "rw": return map({ v: [4, 5, 2, 3, 1, 0] }, cube);
        case "wb": return map({ v: [1, 0, 4, 5, 2, 3] }, cube);
        case "bw": return map({ v: [2, 3, 5, 4, 1, 0] }, cube);
        case "wo": return map({ v: [1, 0, 2, 3, 5, 4] }, cube);
        case "ow": return map({ v: [5, 4, 3, 2, 1, 0] }, cube);
        case "wg": return map({ v: [1, 0, 5, 4, 3, 2] }, cube);
        case "gw": return map({ v: [3, 2, 4, 5, 1, 0] }, cube);
    }
}

function apply(alg, cube) {
    var twists = alg.split(' ');
    for (var t in twists) {
        cube = twist(twists[t], cube);
    }
    return cube;
}