let solved = [0; 1; 2; 3; 4; 5; 6; 7], [0; 0; 0; 0; 0; 0; 0; 0]

type Twist = U | D | L | R | F | B

let twist = function
    | U -> [1; 2; 3; 0; 4; 5; 6; 7], [0; 0; 0; 0; 0; 0; 0; 0]
    | D -> [0; 1; 2; 3; 5; 6; 7; 4], [0; 0; 0; 0; 0; 0; 0; 0]
    | L -> [0; 1; 6; 2; 4; 3; 5; 7], [0; 0; 2; 1; 0; 2; 1; 0]
    | R -> [4; 0; 2; 3; 7; 5; 6; 1], [2; 1; 0; 0; 1; 0; 0; 2]
    | F -> [3; 1; 2; 5; 0; 4; 6; 7], [1; 0; 0; 2; 2; 1; 0; 0]
    | B -> [0; 7; 1; 3; 4; 5; 2; 6], [0; 2; 1; 0; 0; 0; 2; 1]

type Rotation = X | Y | Z

let rotation = function
    | Z -> [3; 2; 6; 5; 0; 4; 7; 1], [1; 2; 1; 2; 2; 1; 2; 1]
    | X -> [4; 0; 3; 5; 7; 6; 2; 1], [2; 1; 2; 1; 1; 2; 1; 2]
    | Y -> [1; 2; 3; 0; 7; 4; 5; 6], [0; 0; 0; 0; 0; 0; 0; 0]

type Move = Twist of Twist | Rotation of Rotation

let move = function
    | Twist t -> twist t
    | Rotation r -> rotation r

// let apply transform cube = function

// let identity =

// let invert t =

// let combine t0 t1 =

// let multiply t n =
// < 0 -> invert t -n
// = 0 -> identity
// = 1 -> t
//   2 -> combine t t
// > 2 -> recurse