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
        function check() {
            var rotations = ["", "x", "x y", "x y'", "x y2", "x z", "x z'", "x z2", "x'", "x' y", "x' y'", "x' z", "x' z'", "x2", "x2 y", "x2 y'", "x2 z", "x2 z'", "y", "y'", "y2", "z", "z'", "z2"];
            for (var i = 0; i < rotations.length; i++) {
                var rot = rotations[i];
                // apply rotation, alg, inverse rotation
                var result = Cube.alg(rot, Cube.alg(alg, Cube.alg(rot, instance)), true);
                if (verify(result)) {
                    document.getElementById("status").innerText = "Well done!";
                    document.body.style.backgroundColor = "green";
                    document.getElementById("retry").disabled = true;
                    window.setTimeout(next, 3000);
                    return true;
                }
            }
        }
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
        document.getElementById("retry").disabled = false;
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

    var settings = {};
    var instance = Cube.solved;
    var alg = "";
    var solution = "";
    var verify;

    function next() {
        function randomElement(arr) {
            return arr[Math.floor(Math.random() * arr.length)];
        }
        function challenge(alg, expected) {
            verify = expected;
            var auf = settings.auf ? randomElement(["", "U ", "U' ", "U2 "]) : "";
            solution = auf + alg;
            instance = Cube.solved;
            // up color
            var rot = [];
            if (settings.yellow) rot.push("");
            if (settings.white) rot.push("x2");
            if (settings.red) rot.push("x");
            if (settings.orange) rot.push("x'");
            if (settings.green) rot.push("z'");
            if (settings.blue) rot.push("z");
            instance = Cube.random(rot, 1, instance);
            if (settings.method == "roux") {
                var upColor = Cube.faceColor("U", Cube.faces(instance));
                // scramble M-slice with U-layer
                instance = Cube.random(["U", "U'", "U2", "M", "M'", "M2"], 100, instance);
                var numColors = (settings.yellow ? 1 : 0) + (settings.white ? 1 : 0) + (settings.red ? 1 : 0) + (settings.orange ? 1 : 0) + (settings.green ? 1 : 0) + (settings.blue ? 1 : 0);
                if (numColors > 1) {
                    // adjust M-slite so center top indicates color (too confusing otherwise!)
                    while (Cube.faceColor("U", Cube.faces(instance)) != upColor) {
                        instance = Cube.alg("M", instance);
                    }
                }
            }
            // scramble corners and edges
            var jperm_b = "R U R' F' R U R' U' R' F R2 U' R' U'";
            var yperm = "F R U' R' U' R U R' F' R U R' U' R' F R F'";
            for (var i = 0; i < 10; i++) {
                instance = Cube.random(["", "U", "U'", "U2"], 1, instance);
                instance = Cube.random([jperm_b, yperm], 1, instance);
            }
            instance = Cube.alg(solution, instance, true);
        }
        alg = "";
        challenge(randomElement(settings.algs), verifyCornersOriented);
        update(instance);
        document.getElementById("status").innerText = "Have fun!";
        document.body.style.backgroundColor = "black";
        document.getElementById("retry").disabled = true;
    }

    function retry() {
        alg = "";
        update(instance);
        document.getElementById("status").innerText = "Try again";
        document.body.style.backgroundColor = "black";
        document.getElementById("retry").disabled = true;
    }

    return {
        displayAlg: displayAlg,
        twist: twist,
        giiker: giiker,
        next: next,
        retry: retry,
        settings: settings
    };
}());