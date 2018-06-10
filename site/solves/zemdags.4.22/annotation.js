var annotation = {
    frames: {
        base: "zemdags.4.22/6.26%20official%203x3%20average-NA%20",
        fps: 30,
        first: 1936,
        last: 2581,
    },
    person: {
        name: "Felix Zemdags"
    },
    event: {
        name: "Cube For Cambodia 2018"
    },
    scramble: "R2 L' F2 D2 F' D L2 B' D L U B2 U B2 D2 L2 D' F2 D",
    time: 4.22,
    marks: [
        { name: "Inspection", frame: 1985, type: "inspection-start" },
        { name: "Up",         frame: 1996, type: "inspection" },
        { name: "Down",       frame: 2287, type: "inspection" },
        { name: "Solve",      frame: 2352, type: "timer-start" },
        { name: "Grab",       frame: 2356, type: "solve-start" },
        { name: "F'",         frame: 2359, type: "twist" },
        { name: "...",        frame: 2363, type: "idle" },
        { name: "R'",         frame: 2366, type: "twist" },
        { name: "D'",         frame: 2369, type: "twist" },
        { name: "R",          frame: 2372, type: "twist", note: "Pseudo cross" },
        { name: "y",          frame: 2374, type: "rotate" },
        { name: "R",          frame: 2379, type: "twist" },
        { name: "U'",         frame: 2381, type: "twist" },
        { name: "R'",         frame: 2382, type: "twist" },
        { name: "u'",         frame: 2386, type: "twist", note: "XCross" },
        { name: "...",        frame: 2388, type: "idle" },
        { name: "U'",         frame: 2390, type: "twist" },
        { name: "R",          frame: 2392, type: "twist" },
        { name: "U",          frame: 2394, type: "twist" },
        { name: "R'",         frame: 2396, type: "twist", note: "2nd pair" },
        { name: "y'",         frame: 2398, type: "rotate" },
        { name: "L'",         frame: 2403, type: "twist" },
        { name: "U2",         frame: 2405, type: "twist" },
        { name: "L",          frame: 2408, type: "twist" },
        { name: "U'",         frame: 2410, type: "twist" },
        { name: "L'",         frame: 2411, type: "twist" },
        { name: "U",          frame: 2413, type: "twist" },
        { name: "L",          frame: 2414, type: "twist", note: "3rd pair" },
        { name: "d",          frame: 2418, type: "twist" },
        { name: "U",          frame: 2424, type: "twist" },
        { name: "R'",         frame: 2426, type: "twist" },
        { name: "U'",         frame: 2427, type: "twist" },
        { name: "R",          frame: 2429, type: "twist" },
        { name: "U",          frame: 2431, type: "twist" },
        { name: "R'",         frame: 2432, type: "twist" },
        { name: "U'",         frame: 2434, type: "twist" },
        { name: "R",          frame: 2435, type: "twist", note: "4th pair" },
        { name: "U'",         frame: 2438, type: "twist" },
        { name: "...",        frame: 2440, type: "idle" },
        { name: "R",          frame: 2442, type: "twist" },
        { name: "U2'",        frame: 2444, type: "twist" },
        { name: "R'",         frame: 2447, type: "twist" },
        { name: "lockup",     frame: 2450, type: "lockup", note: "Lockup" },
        { name: "R'",         frame: 2457, type: "twist" },
        { name: "F",          frame: 2459, type: "twist" },
        { name: "...",        frame: 2462, type: "idle" },
        { name: "R",          frame: 2464, type: "twist" },
        { name: "F'",         frame: 2466, type: "twist" },
        { name: "R",          frame: 2468, type: "twist" },
        { name: "U2'",        frame: 2470, type: "twist" },
        { name: "R'",         frame: 2473, type: "twist", note: "OLL[CP]" },
        { name: "Release",    frame: 2476, type: "solve-stop" },
        { name: "Stop",       frame: 2479, type: "timer-stop" },
    ]
}
/*
R' R' F R F' R U2' R' // OLL(CP)
This was a nice one. Keyhole extended cross was straightforward, the only issue might have been the rotation. I was also able to see that the green/red pair would end up as a simple insertion case, so made sure to do a u' at the end of the X-Cross to avoid a rotation.

X Cross: F' R' D' R y R U' R' u'
2nd pair: U' R U R'

Here, the green/orange pair can be solved without a rotation, yet I rotated to solve the blue/orange one. This was most likely because I saw the blue/orange F2L pieces during the first two pairs, and perhaps because my thumb would have been blocking the green/orange corner piece.

3rd pair: y' L' U2 L U' L' U L
4th pair: d U R' U' R U R' U' R
OLL: U' R U2' R' R' F R F' R U2' R'

Brest said the OLL lockup cost about 0.26 seconds lol.

4.22 seconds, 38 moves (STM), 9.00 TPS
*/