
var GiikerCube = function() {
  this.listeners = [];
}

var debug = console.info ? console.log.bind(console) : console.info.bind(console);

GiikerCube.prototype = {
  UUIDs: {
    cubeService: "0000aadb-0000-1000-8000-00805f9b34fb",
    cubeCharacteristic: "0000aadc-0000-1000-8000-00805f9b34fb"
  },

  connect: async function() {
    console.log("Attempting to pair.")
    this.device = await navigator.bluetooth.requestDevice({
      filters: [{
        namePrefix: "GiC"
      }],
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
    // TODO: Can we safely save the async promise instead of waiting for the response?
    this._originalValue = await this.cubeCharacteristic.readValue();
    debug("Original value:", this._originalValue);
    this.cubeCharacteristic.addEventListener("characteristicvaluechanged",
      this.onCubeCharacteristicChanged.bind(this));
  },

  giikerMoveToAlgMove(face, amount) {
    if (amount == 9) {
      console.err("Encountered 9", face, amount);
      amount = 2;
    }

    return {
      type: "move",
      base: ["?", "B", "D", "L", "U", "R", "F"][face],
      amount: [0, 1, 2, -1][amount]
    }
  },

  onCubeCharacteristicChanged(event) {
    var val = event.target.value;

    if (this._originalValue) {
      debug("Comparing against original value.")
      var same = true;
      for (var i = 0; i < 20; i++) {
         if (this._originalValue.getUint8(i) != val.getUint8(i)) {
          debug("Different at index ", i);
          same = false;
          break;
         }
      }
      this._originalValue = null;
      if (same) {
        debug("Skipping extra first event.")
        return;
      }
    }

    console.log(val);
    // console.log(event.target);
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

    // play

    const DFR = 0;
    const UFR = 1;
    const UFL = 2;
    const DFL = 3;
    const DBR = 4;
    const UBR = 5;
    const UBL = 6;
    const DBL = 7;

    function giikerCornerColor(cornerNum, i) {
      var corners = ["GYR", "GRW", "GWO", "GOY", "BRY", "BWR", "BOW", "BYO"];
      var corner = giikerState[cornerNum] - 1;
      var orientation = giikerState[cornerNum + 8] - 1; // 0 CW, 1 CCW, 2 solved
      var colors = corners[corner];
      var reverse = cornerNum == 0 || cornerNum == 2 || cornerNum == 5 || cornerNum == 7; // TODO: understand this!
      var index = (i + (reverse ? 3 - orientation : orientation)) % 3;
      return colors[index]
    }

    const DF = 0;
    const FR = 1;
    const UF = 2;
    const FL = 3;
    const DR = 4;
    const UR = 5;
    const UL = 6;
    const DL = 7;
    const DB = 8;
    const BR = 9;
    const UB = 10;
    const BL = 11;

    function giikerEdgeColor(edgeNum, i) {
      var edges = ["YG", "RG", "WG", "OG", "RY", "RW", "OW", "OY", "YB", "RB", "WB", "OB"]
      var edge = giikerState[16 + edgeNum] - 1;
      var byte = giikerState[28 + Math.floor(edgeNum / 4)];
      var mask = Math.pow(2, (3 - (edgeNum % 4)));
      var flip = (byte & mask) != 0;
      return edges[edge][flip ? (i == 0 ? 1 : 0) : i];
    }

    function giikerTwist(i) {
      var twists = ["B", "D", "L", "U", "R", "F"];
      var twist = giikerState[32 + i] - 1;
      var amount = giikerState[32 + 1 + i];
      return twists[twist] + (amount == 2 ? "2" : amount == 3 ? "'" : "");
    }

    var s = "";
    // B
    s += giikerCornerColor(DBL, 2);
    s += giikerEdgeColor(DB, 1);
    s += giikerCornerColor(DBR, 1);
    s += giikerEdgeColor(BL, 1);
    s += "B";
    s += giikerEdgeColor(BR, 1);
    s += giikerCornerColor(UBL, 1);
    s += giikerEdgeColor(UB, 1);
    s += giikerCornerColor(UBR, 2);
    // U
    s += giikerCornerColor(UBL, 0);
    s += giikerEdgeColor(UB, 0)
    s += giikerCornerColor(UBR, 0);
    s += giikerEdgeColor(UL, 1);
    s += "W";
    s += giikerEdgeColor(UR, 1);
    s += giikerCornerColor(UFL, 0);
    s += giikerEdgeColor(UF, 0);
    s += giikerCornerColor(UFR, 0);
    // L0
    s += giikerCornerColor(UBL, 2);
    s += giikerEdgeColor(UL, 0);
    s += giikerCornerColor(UFL, 1);
    // F0
    s += giikerCornerColor(UFL, 2);
    s += giikerEdgeColor(UF, 1);
    s += giikerCornerColor(UFR, 1);
    // R0
    s += giikerCornerColor(UFR, 2);
    s += giikerEdgeColor(UR, 0);
    s += giikerCornerColor(UBR, 1);
    // L1
    s += giikerEdgeColor(BL, 0);
    s += "O";
    s += giikerEdgeColor(FL, 0);
    // F1
    s += giikerEdgeColor(FL, 1);
    s += "G";
    s += giikerEdgeColor(FR, 1);
    // R1
    s += giikerEdgeColor(FR, 0);
    s += "R";
    s += giikerEdgeColor(BR, 0);
    // L2
    s += giikerCornerColor(DBL, 1);
    s += giikerEdgeColor(DL, 0);
    s += giikerCornerColor(DFL, 2);
    // F2
    s += giikerCornerColor(DFL, 1);
    s += giikerEdgeColor(DF, 1);
    s += giikerCornerColor(DFR, 2);
    // R2
    s += giikerCornerColor(DFR, 1);
    s += giikerEdgeColor(DR, 0);
    s += giikerCornerColor(DBR, 2);
    // D
    s += giikerCornerColor(DFL, 0);
    s += giikerEdgeColor(DF, 0);
    s += giikerCornerColor(DFR, 0);
    s += giikerEdgeColor(DL, 1);
    s += "Y";
    s += giikerEdgeColor(DR, 1);
    s += giikerCornerColor(DBL, 0);
    s += giikerEdgeColor(DB, 0);
    s += giikerCornerColor(DBR, 0);
    console.log(s);

    var m = giikerTwist(0);
    m += " " + giikerTwist(1);
    m += " " + giikerTwist(2);
    m += " " + giikerTwist(3);
    console.log(m);

    function matchPattern(pattern, state) {
      for (var i in pattern) {
        var p = pattern[i];
        if (p != '.' && p != state[i]) {
          return false;
        }
      }
      return true;
    }

    // BBBBBBBBBWWWWWWWWWOOOGGGRRROOOGGGRRROOOGGGRRRYYYYYYYYY
    // BBBBBBBBBRRRWWWRRRWOWGGGYRYWOWGGGYRYWOWGGGYRYOOOYYYOOO
    // BBBBBBBBBYYYWWWYYYRORGGGORORORGGGORORORGGGOROWWWYYYWWW
    // BBBBBBBBBOOOWWWOOOYOYGGGWRWYOYGGGWRWYOYGGGWRWRRRYYYRRR
    // *********   ***    * ***    * *** *  * *** *    ***
    // BBBBBBBBBUUUUUUUUULLLFFFRRRLLLFFFRRRLLLFFFRRRDDDDDDDDD

    if (matchPattern("BBBBBBBBBWWWWWWWWWOOOGGGRRROOOGGGRRROOOGGGRRRYYYYYYYYY", s)) {
      console.log("CONGRATS! SOLVED!");
    } else if (matchPattern("B.BBBBBBBWWW......O.......RO.......RO.OG.GR.RY.Y...Y.Y", s) ||
               matchPattern("R.RBBBBBBWWW......O.......RO.......RB.BO.OG.GY.Y...Y.Y", s) ||
               matchPattern("G.GBBBBBBWWW......O.......RO.......RR.RB.BO.OY.Y...Y.Y", s) ||
               matchPattern("O.OBBBBBBWWW......O.......RO.......RG.GR.RB.BY.Y...Y.Y", s)) {
               matchPattern("BBBBB.BBBRRR.....RW.....Y.YW........W.....Y.Y......OO.", s) ||
      console.log("CORNERS PERMUTED - CO/CP!")
    } else if (matchPattern("...BBBBBBWWW......O.......RO.......R.........Y.Y...Y.Y", s) ||
               matchPattern("BB.BB.BB.RR.......W.....Y.YW........W.....Y.Y......OO.", s) ||
               matchPattern("BBBBBB...Y.Y...Y.Y.........R.......OR.......O......WWW", s) ||
               matchPattern(".BB.BB.BB.OO......Y.Y.....W........WY.Y.....W.......RR", s)) {
      console.log("CORNERS ORIENTED - CO!")
    } else if (matchPattern("...BBBBBBWWW......O.OGGGR.RO.OGGGR.R..................", s) ||
               matchPattern("BB.BB.BB.RR....RR.W.WGG....W.WGG....W.WGG....OO....OO.", s) ||
               matchPattern("BBBBBB.....................R.RGGGO.OR.RGGGO.OWWW...WWW", s) ||
               matchPattern(".BB.BB.BB.OO....OO....GGW.W....GGW.W....GGW.W.RR....RR", s)) {
      console.log("SECOND BLOCK - F2B!")
    } else if (matchPattern("...BBBBBBWWW......O.......RO.......R..................", s) ||
               matchPattern("BB.BB.BB.RR.......W........W........W..............OO.", s) ||
               matchPattern("BBBBBB.....................R.......OR.......O......WWW", s) ||
               matchPattern(".BB.BB.BB.OO..............W........W........W.......RR", s)) {
      console.log("FIRST BLOCK - FB!")
    }

    // endplay

    for (var l of this.listeners) {
      l({
        latestMove: this.giikerMoveToAlgMove(giikerState[32], giikerState[33]),
        timeStamp: event.timeStamp,
        stateStr: str
      });
    }
  },

  addEventListener(listener) {
    this.listeners.push(listener);
  }
}
