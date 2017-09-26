// Copyright: 2017-, Copenhagen Business School
// Author: Rasmus Ulslev Pedersen (rp.digi@cbs.dk)
// License: Simplified BSD License
// LIPS: Utilities to work with network data

namespace lips
[<AutoOpen>]
module Util =
    // Example: 'prn (sprintf "%d" 123)'
    let prn (str : string) = 
            System.Console.WriteLine(str)
            System.Diagnostics.Debug.WriteLine(str)

    let hex2ascii hx =
        match hx with
        | _ when hx <= 0x9 -> char (hx + 48)
        | _ when hx >= 0xA && hx <= 0xF -> char (hx + 55)
        | _ -> failwith "unexpected input"

    let char2byte ch =
        match ch with
        | _ when ch >= '0' && ch <= '9' -> byte (int ch - 48)
        | _ when ch >= 'A' && ch <= 'F' -> byte (int ch - 55)
        | _ -> failwith "unexpected input"
    
    // example: 0xCA becomes "CA"
    let byte2str by =
        let highby = by >>> 4
        let lowby = by &&& 0x0F
        string (hex2ascii highby) + string (hex2ascii lowby)

    // example: "CA" becomes byte 0xCA
    let strtwo2byte (str:string) =
        let highby = char2byte (char str.[0])
        let lowby = char2byte (char str.[1])
        highby <<< 4 ||| lowby

    let str2bytes (str:string) =
        let mutable ba : byte[] = Array.zeroCreate 0 //TODO: odd 
        for i in 0 ..2.. str.Length-1 do
            let by = strtwo2byte (str.Substring(i,2))
            ba <- Array.append ba [|by|]
        ba
            

        
        