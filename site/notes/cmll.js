
function setColors(colors) {
    document.getElementById("b0").setAttribute("fill", colorToCss(colors[0]));
    document.getElementById("b1").setAttribute("fill", colorToCss(colors[1]));
    document.getElementById("b2").setAttribute("fill", colorToCss(colors[2]));
    document.getElementById("l0").setAttribute("fill", colorToCss(colors[3]));
    document.getElementById("u0").setAttribute("fill", colorToCss(colors[4]));
    document.getElementById("u1").setAttribute("fill", colorToCss(colors[5]));
    document.getElementById("u2").setAttribute("fill", colorToCss(colors[6]));
    document.getElementById("r0").setAttribute("fill", colorToCss(colors[7]));
    document.getElementById("l1").setAttribute("fill", colorToCss(colors[8]));
    document.getElementById("u3").setAttribute("fill", colorToCss(colors[9]));
    document.getElementById("u4").setAttribute("fill", colorToCss(colors[10]));
    document.getElementById("u5").setAttribute("fill", colorToCss(colors[11]));
    document.getElementById("r1").setAttribute("fill", colorToCss(colors[12]));
    document.getElementById("l2").setAttribute("fill", colorToCss(colors[13]));
    document.getElementById("u6").setAttribute("fill", colorToCss(colors[14]));
    document.getElementById("u7").setAttribute("fill", colorToCss(colors[15]));
    document.getElementById("u8").setAttribute("fill", colorToCss(colors[16]));
    document.getElementById("r2").setAttribute("fill", colorToCss(colors[17]));
    document.getElementById("f0").setAttribute("fill", colorToCss(colors[18]));
    document.getElementById("f1").setAttribute("fill", colorToCss(colors[19]));
    document.getElementById("f2").setAttribute("fill", colorToCss(colors[20]));
}

function diagram() {
    // generated initially by Conrads VisualCube - http://cube.crider.co.uk/visualcube.php
    return '<svg version="1.1" xmlns="http://www.w3.org/2000/svg"' +
               'width="600" height="600"' +
               'viewBox="-0.9 -0.9 1.8 1.8">' +
               '<rect fill="#000" x="-0.9" y="-0.9" width="1.8" height="1.8"/>' +
               '<g style="stroke-width:0.1;stroke-linejoin:round;opacity:1">' +
                   '<polygon fill="#000000" stroke="#000000" points="-0.522222222222,-0.522222222222 0.522222222222,-0.522222222222 0.522222222222,0.522222222222 -0.522222222222,0.522222222222"/>' +
               '</g>' +
               '<g style="opacity:1;stroke-opacity:0.5;stroke-width:0;stroke-linejoin:round">' +
                   '<polygon id="u0" fill="#333" stroke="#000000" points="-0.527777777778,-0.527777777778 -0.212962962963,-0.527777777778 -0.212962962963,-0.212962962963 -0.527777777778,-0.212962962963"/>' +
                   '<polygon id="u1" fill="#333" stroke="#000000" points="-0.157407407407,-0.527777777778 0.157407407407,-0.527777777778 0.157407407407,-0.212962962963 -0.157407407407,-0.212962962963"/>' +
                   '<polygon id="u2" fill="#333" stroke="#000000" points="0.212962962963,-0.527777777778 0.527777777778,-0.527777777778 0.527777777778,-0.212962962963 0.212962962963,-0.212962962963"/>' +
                   '<polygon id="u3" fill="#333" stroke="#000000" points="-0.527777777778,-0.157407407407 -0.212962962963,-0.157407407407 -0.212962962963,0.157407407407 -0.527777777778,0.157407407407"/>' +
                   '<polygon id="u4" fill="#333" stroke="#000000" points="-0.157407407407,-0.157407407407 0.157407407407,-0.157407407407 0.157407407407,0.157407407407 -0.157407407407,0.157407407407"/>' +
                   '<polygon id="u5" fill="#333" stroke="#000000" points="0.212962962963,-0.157407407407 0.527777777778,-0.157407407407 0.527777777778,0.157407407407 0.212962962963,0.157407407407"/>' +
                   '<polygon id="u6" fill="#333" stroke="#000000" points="-0.527777777778,0.212962962963 -0.212962962963,0.212962962963 -0.212962962963,0.527777777778 -0.527777777778,0.527777777778"/>' +
                   '<polygon id="u7" fill="#333" stroke="#000000" points="-0.157407407407,0.212962962963 0.157407407407,0.212962962963 0.157407407407,0.527777777778 -0.157407407407,0.527777777778"/>' +
                   '<polygon id="u8" fill="#333" stroke="#000000" points="0.212962962963,0.212962962963 0.527777777778,0.212962962963 0.527777777778,0.527777777778 0.212962962963,0.527777777778"/>' +
               '</g>' +
               '<g style="opacity:1;stroke-opacity:1;stroke-width:0.02;stroke-linejoin:round">' +
                   '<polygon id="b0" fill="#333" stroke="#000000" points="-0.195146871009,-0.554406130268 -0.543295019157,-0.554406130268 -0.507279693487,-0.718390804598 -0.183141762452,-0.718390804598"/>' +
                   '<polygon id="b1" fill="#333" stroke="#000000" points="0.174457215837,-0.554406130268 -0.173690932312,-0.554406130268 -0.161685823755,-0.718390804598 0.16245210728,-0.718390804598"/>' +
                   '<polygon id="b2" fill="#333" stroke="#000000" points="0.544061302682,-0.554406130268 0.195913154534,-0.554406130268 0.183908045977,-0.718390804598 0.508045977011,-0.718390804598"/>' +
                   '<polygon id="l0" fill="#333" stroke="#000000" points="-0.554406130268,-0.544061302682 -0.554406130268,-0.195913154534 -0.718390804598,-0.183908045977 -0.718390804598,-0.508045977011"/>' +
                   '<polygon id="l1" fill="#333" stroke="#000000" points="-0.554406130268,-0.174457215837 -0.554406130268,0.173690932312 -0.718390804598,0.161685823755 -0.718390804598,-0.16245210728"/>' +
                   '<polygon id="l2" fill="#333" stroke="#000000" points="-0.554406130268,0.195146871009 -0.554406130268,0.543295019157 -0.718390804598,0.507279693487 -0.718390804598,0.183141762452"/>' +
                   '<polygon id="r0" fill="#333" stroke="#000000" points="0.554406130268,-0.195146871009 0.554406130268,-0.543295019157 0.718390804598,-0.507279693487 0.718390804598,-0.183141762452"/>' +
                   '<polygon id="r1" fill="#333" stroke="#000000" points="0.554406130268,0.174457215837 0.554406130268,-0.173690932312 0.718390804598,-0.161685823755 0.718390804598,0.16245210728"/>' +
                   '<polygon id="r2" fill="#333" stroke="#000000" points="0.554406130268,0.544061302682 0.554406130268,0.195913154534 0.718390804598,0.183908045977 0.718390804598,0.508045977011"/>' +
                   '<polygon id="f0" fill="#333" stroke="#000000" points="-0.544061302682,0.554406130268 -0.195913154534,0.554406130268 -0.183908045977,0.718390804598 -0.508045977011,0.718390804598"/>' +
                   '<polygon id="f1" fill="#333" stroke="#000000" points="-0.174457215837,0.554406130268 0.173690932312,0.554406130268 0.161685823755,0.718390804598 -0.16245210728,0.718390804598"/>' +
                   '<polygon id="f2" fill="#333" stroke="#000000" points="0.195146871009,0.554406130268 0.543295019157,0.554406130268 0.507279693487,0.718390804598 0.183141762452,0.718390804598"/>' +
               '</g>' +
           '</svg>';
}