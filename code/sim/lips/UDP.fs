// Copyright: 2017-, Copenhagen Business School
// Author: Rasmus Ulslev Pedersen (rp.digi@cbs.dk)
// License: Simplified BSD License
// LIPS: UDP datagram

//RFC 768
//User Datagram Header Format
//  0      7 8     15 16    23 24    31
//  +--------+--------+--------+--------+
//  |     Source      |   Destination   |
//  |      Port       |      Port       |
//  +--------+--------+--------+--------+
//  |                 |                 |
//  |     Length      |    Checksum     |
//  +--------+--------+--------+--------+
//  |
//  |          data octets ...
//  +---------------- ...
//
//Pseudo-header for checksum
//  0      7 8     15 16    23 24    31
//  +--------+--------+--------+--------+
//  |          source address           |
//  +--------+--------+--------+--------+
//  |        destination address        |
//  +--------+--------+--------+--------+
//  |  zero  |protocol|   UDP length    |
//  +--------+--------+--------+--------+

namespace LIPSLIB

[<AutoOpen>]
module UDP =
    type UDPDatagram() as __ =
        let mutable header : byte[] = Array.zeroCreate 8 // 8 bytes header
        let sport_mask = 0xFFFF
        let dport_mask = 0xFFFF
        let length     = 0xFFFF
        let chksum     = 0xFFFF
        let mutable data : byte [] = Array.zeroCreate 0

        do  // Some values
            __.SPort    <- 0x2345
            __.DPort    <- 0x6789
            __.Len      <- 0x0008 + 0x0003 // 0xB
            // Chksum below
            __.Data     <- [|byte 0xC0; byte 0xFF; byte 0xEE|]
            __.Chksum   <- __.CalculateChecksum()
        
        member __.Header with get()      = header and
                              set(value) = header <- value
        member __.SPort with get()       = (int header.[0] <<< 8) ||| (int header.[1]) and
                             set(value)  = header.[0] <- byte(value >>> 8)
                                           header.[1] <- byte value
        member __.DPort with get()       = (int header.[2] <<< 8) ||| (int header.[3]) and
                             set(value)  = header.[2] <- byte(value >>> 8)
                                           header.[3] <- byte value
        member __.Len    with get()      = (int header.[4] <<< 8) ||| (int header.[5]) and
                              set(value) = header.[4] <- byte(value >>> 8)
                                           header.[5] <- byte value
        member __.Chksum with get()      = (int header.[6] <<< 8) ||| (int header.[7]) and
                              set(value) = header.[6] <- byte(value >>> 8)
                                           header.[7] <- byte value
        member __.Data with get()        = data and
                            set(value)   = data <- value

        member __.Print() =
            prn (sprintf "UDP.SPort:    0x%04X          Header[0,1]: 0x%02X 0x%02X" __.SPort  __.Header.[0] __.Header.[1])
            prn (sprintf "UDP.DPort:    0x%04X          Header[2,3]: 0x%02X 0x%02X" __.DPort  __.Header.[2] __.Header.[3])
            prn (sprintf "UDP.Len:      0x%04X          Header[4,5]: 0x%02X 0x%02X" __.Len    __.Header.[4] __.Header.[5])
            prn (sprintf "UDP.Chksum:   0x%04X          Header[6,7]: 0x%02X 0x%02X" __.Chksum __.Header.[6] __.Header.[7])
            for i in 0 .. data.Length - 1 do
                if i % 4 = 0 then
                    pr (sprintf "UDP.Data[%02d]: " i)
                pr (sprintf "0x%02X " data.[i])
                if (i + 1) % 4 = 0 then 
                    prn ""

//TODO
        member __.CalculateChecksum() =
            let mutable checksum = ((int header.[0] <<< 8) ||| (int header.[1]))
            checksum <- checksum + ((int header.[4] <<< 8) ||| (int header.[5]))
            checksum <- checksum + ((int header.[6] <<< 8) ||| (int header.[7]))
            if data.Length = 1 then
                checksum <- checksum + (int data.[0] <<< 8) // Padding with a "zero"
            else
                for i in 0 .. 2 .. data.Length - 2 do // there will be one or two bytes left
                    checksum <- checksum + ((int data.[i] <<< 8) ||| (int data.[i+1]))
                if data.Length % 2 = 0 then // two bytes left
                    checksum <- checksum + ((int data.[data.Length - 2] <<< 8) ||| (int data.[data.Length - 1]))
                else // one byte left
                    checksum <- checksum + (int data.[data.Length - 1] <<< 8) // Padding with "zero"
            if (checksum &&& 0xFFFF0000 > 0) then 
                 checksum <- (checksum >>> 16) + (checksum &&& 0x0000FFFF)
            if (checksum &&& 0xFFFF0000 > 0) then
                 checksum <- (checksum >>> 16) + (checksum &&& 0x0000FFFF)
            checksum <- ~~~checksum
            checksum &&& 0x0000FFFF

