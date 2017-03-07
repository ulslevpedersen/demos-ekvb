// Copyright: 2015-2017, Copenhagen Business School
// Author: Rasmus Ulslev Pedersen (rup.itm@cbs.dk)
// License: Simplified BSD License
//
// Eaglegen: Create schematics for Eagle

open System.IO

let readFile = File.ReadAllLines(@"xc7a100tcsg324pkg.txt")

let taskStr (instArray : string []) = 
    let mutable taskStr = ""
    for i = 0 to instArray.Length - 1 do
        printfn "%d:%s" i instArray.[i]
    instArray

[<EntryPoint>]
let main argv = 
    printfn "%A" (taskStr (readFile))
    //System.IO.File.WriteAllText(@"C:\Users\test\Desktop\instnames-sv.txt", taskStr (readFile))
    System.Console.ReadKey() |> ignore
    0 // return an integer exit code