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
//Pseudo-header prepended UDP header for checksum purposes
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
        let mutable data : byte []  = Array.zeroCreate 0
        let mutable ip              = IPPacket()

        do  // Test values so __.Checksum = 0x14de holds 
            ip.SrcIP   <- 0x9801331B //152.1.51.27
            ip.DstIP   <- 0x980E5E4B //152.14.94.75
            ip.Prot    <- 0x11       // udp 17
            // Some UDP test values
            __.SrcPort <- 0xA08F
            __.DstPort <- 0x2694
            // Chksum below
            // __.Len is updated when Data is set
            __.Data    <- [|0x62uy; 0x62uy|]
            __.Chksum  <- __.CalculateChecksum()
        
        member __.Header with get()      = header and
                              set(value) = header <- value
        member __.SrcPort with get()      = (int header.[0] <<< 8) ||| (int header.[1]) and
                             set(value)  = header.[0] <- byte(value >>> 8)
                                           header.[1] <- byte value
        member __.DstPort with get()     = (int header.[2] <<< 8) ||| (int header.[3]) and
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
                                           __.Len <- 0x8 + data.Length
        member __.IP with get()          = ip and 
                          set(value)     = ip <- value

        member __.Print() =
            prn (sprintf "UDP.SPort:           0x%04X          Header[0,1]:         0x%02X 0x%02X" __.SrcPort  __.Header.[0] __.Header.[1])
            prn (sprintf "UDP.DPort:           0x%04X          Header[2,3]:         0x%02X 0x%02X" __.DstPort  __.Header.[2] __.Header.[3])
            prn (sprintf "UDP.Len:             0x%04X          Header[4,5]:         0x%02X 0x%02X" __.Len    __.Header.[4] __.Header.[5])
            prn (sprintf "UDP.Chksum:          0x%04X          Header[6,7]:         0x%02X 0x%02X" __.Chksum __.Header.[6] __.Header.[7])
            for i in 0 .. data.Length - 1 do
                if i % 4 = 0 then
                    pr (sprintf "UDP.Data[%02d]:        " i)
                pr (sprintf "0x%02X " data.[i])
                if (i + 1) % 4 = 0 then 
                    prn ""
            if data.Length > 0 & data.Length % 4 <> 0 then
                prn ""

        /// Update length and checksum of the IP packet when the (UDP) data changes
        member __.UpdateIPOnData() =
            __.IP.Data    <- Array.concat[__.Header; __.Data] 
            __.IP.Tlen    <- __.IP.CalculateTlen()
            __.IP.Hchksum <- __.IP.CalculateChecksum()
        
        member __.PrintIP() =
            __.IP.Print()

        member __.CalculateChecksum() =
            let mutable checksum = 0
            // pseudo header
            checksum <- checksum + ((__.IP.SrcIP >>> 16) &&& 0xFFFF) + (__.IP.SrcIP &&& 0xFFFF)
            checksum <- checksum + ((__.IP.DstIP >>> 16) &&& 0xFFFF) + (__.IP.DstIP &&& 0xFFFF)
            checksum <- checksum + __.IP.Prot
            checksum <- checksum + __.Len 
            // UDP header
            checksum <- checksum + __.SrcPort
            checksum <- checksum + __.DstPort
            checksum <- checksum + __.Len 
            checksum <- checksum + checksumData(__.Data)
            checksum <- checksumCarryRound(checksum)
            checksum
