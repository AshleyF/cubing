// This is a modified version of lgarron's giiker.js from https://github.com/cubing/cuble.js

var Giiker = (function () {
    function giikerTwist(i, giikerState) {
        var twists = "BDLURF";
        var twist = giikerState[32 + i * 2] - 1;
        var amount = giikerState[32 + 1 + i * 2];
        return twists[twist] + (amount == 2 ? "2" : amount == 3 ? "'" : "");
    }

    const UUIDs = {
        cubeService: "0000aadb-0000-1000-8000-00805f9b34fb",
        cubeCharacteristic: "0000aadc-0000-1000-8000-00805f9b34fb" };

    async function connect(connected, callback) {
        try {
            console.log("Attempting to pair.")
            this.device = await navigator.bluetooth.requestDevice({
                filters: [{ namePrefix: "GiC" }],
                optionalServices: [
                    "00001530-1212-efde-1523-785feabcd123",
                    "0000aaaa-0000-1000-8000-00805f9b34fb",
                    "0000aadb-0000-1000-8000-00805f9b34fb",
                    "0000180f-0000-1000-8000-00805f9b34fb",
                    "0000180a-0000-1000-8000-00805f9b34fb"
                ]
            });
            console.log("Device:", this.device);
            this.server = await this.device.gatt.connect();
            console.log("Server:", this.server);
            this.cubeService = await this.server.getPrimaryService(this.UUIDs.cubeService);
            console.log("Service:", this.cubeService);
            this.cubeCharacteristic = await this.cubeService.getCharacteristic(this.UUIDs.cubeCharacteristic);
            console.log(this.cubeCharacteristic);
            await this.cubeCharacteristic.startNotifications();
            this.cubeCharacteristic.addEventListener("characteristicvaluechanged", this.onCubeCharacteristicChanged.bind(this));
            connected(true);
        } catch(ex) {
            connected(false, ex);
        }
    }

    function onCubeCharacteristicChanged(event) {
        var val = event.target.value;
        console.log(val);
        console.log(event);
        var giikerState = [];
        for (var i = 0; i < 20; i++) {
            giikerState.push(Math.floor(val.getUint8(i) / 16));
            giikerState.push(val.getUint8(i) % 16);
        }
        var str = "";
        str += giikerState.slice(0, 8).join(".");
        str += "\n"
        str += giikerState.slice(8, 16).join(".");
        str += "\n"
        str += giikerState.slice(16, 28).join(".");
        str += "\n"
        str += giikerState.slice(28, 32).join(".");
        str += "\n"
        str += giikerState.slice(32, 40).join(".");
        console.log(str);

        callback(["?", "B", "D", "L", "U", "R", "F"][face] + ["", "", "2", "'"][amount == 9 ? 2 : amount]);
      }

      return {
          connect: connect
      };
}());