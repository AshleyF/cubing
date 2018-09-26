// This is a modified version of lgarron's giiker.js from https://github.com/cubing/cuble.js

var Ui = (function () {
    function displayAlg(alg) {
        var cube = Cube.alg(alg, Cube.solved);
        document.getElementById("debug").innerText = Cube.toString(cube);
        function delay() {
            Display.display3DCube(Cube.faces(cube), "cube");
            Display.displayLL(Cube.faces(cube), "ll");
            Display.displayUF(Cube.faces(cube), "uf");
        }
        window.setTimeout(delay, 1);
    }

    function twist(t) {
        if (t == "") return;
        alg += t + ' ';
        var result = Cube.alg(alg, instance);
        var len = alg.split(' ').length;
        var progress = "";
        for (var i = 1; i < len; i++) {
            progress += "&bull; ";
        }
        document.getElementById("status").innerHTML = progress;
        document.body.style.backgroundColor = "#333";
        check();
    }

    function connected() {
        var btn = document.getElementById("giiker");
        btn.disabled = false;
        btn.innerText = "Disconnect";
    }

    function disconnect() {
        var btn = document.getElementById("giiker");
        btn.disabled = false;
        btn.innerText = "Connect";
        Giiker.disconnect();
    }

    function error(ex) {
        var btn = document.getElementById("giiker");
        btn.disabled = false;
        btn.innerText = "Connect";
        alert("Error: " + ex.message);
    }

    function giiker() {
        var btn = document.getElementById("giiker");
        if (Giiker.connected()) {
            btn.disabled = false;
            btn.innerText = "Connect";
            Giiker.disconnect();
        } else {
            btn.disabled = true;
            btn.innerText = "Connecting...";
            Giiker.connect(connected, twist, error);
        }
    }

    return {
        displayAlg: displayAlg,
        twist: twist,
        giiker: giiker
    };
}());