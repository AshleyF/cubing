
function setColors(colors) {
    document.getElementById("u0").setAttribute("fill", colorToCss(colors[0]));
    document.getElementById("u1").setAttribute("fill", colorToCss(colors[1]));
    document.getElementById("u2").setAttribute("fill", colorToCss(colors[2]));
    document.getElementById("u3").setAttribute("fill", colorToCss(colors[3]));
    document.getElementById("u4").setAttribute("fill", colorToCss(colors[4]));
    document.getElementById("u5").setAttribute("fill", colorToCss(colors[5]));
    document.getElementById("u6").setAttribute("fill", colorToCss(colors[6]));
    document.getElementById("u7").setAttribute("fill", colorToCss(colors[7]));
    document.getElementById("u8").setAttribute("fill", colorToCss(colors[8]));
    document.getElementById("f0").setAttribute("fill", colorToCss(colors[9]));
    document.getElementById("f1").setAttribute("fill", colorToCss(colors[10]));
    document.getElementById("f2").setAttribute("fill", colorToCss(colors[11]));
    document.getElementById("f3").setAttribute("fill", colorToCss(colors[12]));
    document.getElementById("f4").setAttribute("fill", colorToCss(colors[13]));
    document.getElementById("f5").setAttribute("fill", colorToCss(colors[14]));
    document.getElementById("f6").setAttribute("fill", colorToCss(colors[15]));
    document.getElementById("f7").setAttribute("fill", colorToCss(colors[16]));
    document.getElementById("f8").setAttribute("fill", colorToCss(colors[17]));
}

function diagram() {
    // generated initially by Conrads VisualCube - http://cube.crider.co.uk/visualcube.php
    return '<svg version="1.1" xmlns="http://www.w3.org/2000/svg"' +
                'width="600" height="600"' +
                'viewBox="-0.9 -0.9 1.8 1.8">' +
                '<rect fill="#000" x="-0.9" y="-0.9" width="1.8" height="1.8"/>' +
                '<g style="stroke-width:0.1;stroke-linejoin:round;opacity:1">' +
                    '<polygon fill="#000" stroke="#000000" points="-0.547416364726,6.07754252175E-17 0.547416364726,6.07754252175E-17 0.47,0.664680374315 -0.47,0.664680374315"/>' +
                    '<polygon fill="#000" stroke="#000000" points="-0.47,-0.664680374315 0.47,-0.664680374315 0.547416364726,6.07754252175E-17 -0.547416364726,6.07754252175E-17"/>' +
                '</g>' +
                '<g style="opacity:1;stroke-opacity:0.5;stroke-width:0;stroke-linejoin:round">' +
                    '<polygon id="f0" fill="#333" stroke="#000000" points="-0.552482185737,0.0195178280008 -0.222479412675,0.0195178280008 -0.21389149619,0.240719878676 -0.52671843628,0.240719878676"/>' +
                    '<polygon id="f1" fill="#333" stroke="#000000" points="-0.165759143868,0.0195178280008 0.164243629194,0.0195178280008 0.155655712708,0.240719878676 -0.157171227382,0.240719878676"/>' +
                    '<polygon id="f2" fill="#333" stroke="#000000" points="0.220963898001,0.0195178280008 0.550966671063,0.0195178280008 0.525202921606,0.240719878676 0.212375981516,0.240719878676"/>' +
                    '<polygon id="f3" fill="#333" stroke="#000000" points="-0.523762383403,0.277824338758 -0.210935443313,0.277824338758 -0.203197260173,0.477139502342 -0.500547833981,0.477139502342"/>' +
                    '<polygon id="f4" fill="#333" stroke="#000000" points="-0.15709625091,0.277824338758 0.15573068918,0.277824338758 0.147992506039,0.477139502342 -0.14935806777,0.477139502342"/>' +
                    '<polygon id="f5" fill="#333" stroke="#000000" points="0.209569881583,0.277824338758 0.522396821673,0.277824338758 0.49918227225,0.477139502342 0.201831698442,0.477139502342"/>' +
                    '<polygon id="f6" fill="#333" stroke="#000000" points="-0.497881083717,0.51065468293 -0.200530509908,0.51065468293 -0.193521889671,0.691178232679 -0.476855223004,0.691178232679"/>' +
                    '<polygon id="f7" fill="#333" stroke="#000000" points="-0.149293694572,0.51065468293 0.148056879236,0.51065468293 0.141048258999,0.691178232679 -0.142285074335,0.691178232679"/>' +
                    '<polygon id="f8" fill="#333" stroke="#000000" points="0.199293694572,0.51065468293 0.496644268381,0.51065468293 0.475618407668,0.691178232679 0.192285074335,0.691178232679"/>' +
                    '<polygon id="u0" fill="#333" stroke="#000000" points="-0.475618407668,-0.691178232679 -0.192285074335,-0.691178232679 -0.199293694572,-0.51065468293 -0.496644268381,-0.51065468293"/>' +
                    '<polygon id="u1" fill="#333" stroke="#000000" points="-0.141048258999,-0.691178232679 0.142285074335,-0.691178232679 0.149293694572,-0.51065468293 -0.148056879236,-0.51065468293"/>' +
                    '<polygon id="u2" fill="#333" stroke="#000000" points="0.193521889671,-0.691178232679 0.476855223004,-0.691178232679 0.497881083717,-0.51065468293 0.200530509908,-0.51065468293"/>' +
                    '<polygon id="l"  fill="#000" stroke="#000000" points="-0.60018227225,-0.477139502342 -0.551831698442,-0.477139502342 -0.575569881583,-0.278824338758 -0.627396821673,-0.277824338758"/>' +
                    '<polygon id="u3" fill="#333" stroke="#000000" points="-0.49918227225,-0.477139502342 -0.201831698442,-0.477139502342 -0.209569881583,-0.277824338758 -0.522396821673,-0.277824338758"/>' +
                    '<polygon id="u4" fill="#333" stroke="#000000" points="-0.147992506039,-0.477139502342 0.14935806777,-0.477139502342 0.15709625091,-0.277824338758 -0.15573068918,-0.277824338758"/>' +
                    '<polygon id="u5" fill="#333" stroke="#000000" points="0.203197260173,-0.477139502342 0.500547833981,-0.477139502342 0.523762383403,-0.277824338758 0.210935443313,-0.277824338758"/>' +
                    '<polygon id="r"  fill="#000" stroke="#000000" points="0.553197260173,-0.477139502342 0.603197260173,-0.477139502342 0.629935443313,-0.277824338758 0.577935443313,-0.277824338758"/>' +
                    '<polygon id="u6" fill="#333" stroke="#000000" points="-0.525202921606,-0.240719878676 -0.212375981516,-0.240719878676 -0.220963898001,-0.0195178280008 -0.550966671063,-0.0195178280008"/>' +
                    '<polygon id="u7" fill="#333" stroke="#000000" points="-0.155655712708,-0.240719878676 0.157171227382,-0.240719878676 0.165759143868,-0.0195178280008 -0.164243629194,-0.0195178280008"/>' +
                    '<polygon id="u8" fill="#333" stroke="#000000" points="0.21389149619,-0.240719878676 0.52671843628,-0.240719878676 0.552482185737,-0.0195178280008 0.222479412675,-0.0195178280008"/>' +
                '</g>' +
            '</svg>';
}