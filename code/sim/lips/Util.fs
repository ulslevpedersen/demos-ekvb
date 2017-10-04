// Copyright: 2017-, Copenhagen Business School
// Author: Rasmus Ulslev Pedersen (rp.digi@cbs.dk)
// License: Simplified BSD License
// LIPS: Utilities to work with network data

namespace LIPSLIB
[<AutoOpen>]
module Util =
    open System
    open System.Threading

    // Example: 'prn (sprintf "%d" 123)'
    let prn (str : string) = 
        System.Console.WriteLine(str)
        System.Diagnostics.Debug.WriteLine(str)

    let pr (str : string) =
        System.Console.Write(str)
        System.Diagnostics.Debug.Write(str)
    
    // I.e.: 0xB -> 'B'
    let hex2ascii (hx:int) =
        match hx with
        | _ when hx <= 0x9 -> char (hx + 48)
        | _ when hx >= 0xA && hx <= 0xF -> char (hx + 55)
        | _ -> failwith "unexpected input" 

    // I.e.: 'C' -> 0xC
    let char2byte (ch:char) =
        match ch with
        | _ when ch >= '0' && ch <= '9' -> byte (int ch - 48)
        | _ when ch >= 'a' && ch <= 'f' -> byte (int ch - 87)
        | _ when ch >= 'A' && ch <= 'F' -> byte (int ch - 55)
        | _ -> failwith (sprintf "unexpected input: '%c'" ch)
    
    // example: 0xCA becomes "CA"
    let byte2str (by:int) =
        let highby = by >>> 4
        let lowby = by &&& 0x0F
        string (hex2ascii highby) + string (hex2ascii lowby)

    // example: "CA" becomes byte 0xCA
    let strtwo2byte (str:string) =
        let highby = char2byte (char str.[0])
        let lowby = char2byte (char str.[1])
        highby <<< 4 ||| lowby
    
    // "CAFEBABE" -> [|0xCA;0xFE;0xBA;0xBE|] (of half the length)
    let str2bytearray (str:string) =
        let mutable evenstr = str
        if str.Length % 2 <> 0 then
            evenstr <- "0" + evenstr
        let mutable ba : byte[] = Array.zeroCreate 0 //TODO: odd 
        for i in 0 ..2.. evenstr.Length-1 do
            let by = strtwo2byte (evenstr.Substring(i,2))
            ba <- Array.append ba [|by|]
        ba

    // [|0xCA;0xFE;0xBA;0xBE|] -> "CAFEBABE" (twice the length)
    let bytearray2str (bytes:byte[]) =
        let strarray = Array.zeroCreate<string> (bytes.Length * 2)
        for i in 0 .. bytes.Length-1 do
            let strtwo = byte2str(int bytes.[i])
            strarray.[i*2] <- string(strtwo.[0])
            strarray.[i*2 + 1] <- string(strtwo.[1])
        String.Join ("", strarray)

    // https://msdn.microsoft.com/en-us/library/system.threading.readerwriterlock(v=vs.110).aspx?cs-save-lang=1&cs-lang=fsharp#code-snippet-1
    let readLock (rwlock : ReaderWriterLock) f  =
        rwlock.AcquireReaderLock(Timeout.Infinite)
        try
            f()
        finally
            rwlock.ReleaseReaderLock()
 
    let writeLock (rwlock : ReaderWriterLock) f  =
        rwlock.AcquireWriterLock(Timeout.Infinite)
        try 
            Thread.MemoryBarrier()
            f()
            //Thread.MemoryBarrier() //TODO
        finally
            rwlock.ReleaseWriterLock()