// Copyright: 2017-, Copenhagen Business School
// Author: Rasmus Ulslev Pedersen (rp.digi@cbs.dk)
// License: Simplified BSD License
// LIPS: ICMP message

//RFC 792
//Echo or Echo Reply Message
//    0                   1                   2                   3
//    0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1
//   +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
//   |     Type      |     Code      |          Checksum             |
//   +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
//   |           Identifier          |        Sequence Number        |
//   +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
//   |     Data ...
//   +-+-+-+-+-

namespace LIPSLIB

[<AutoOpen>]
module ICMP =
    type ICMPMessage() as __ =
        let mutable header : byte[] = Array.zeroCreate 8 // 8 bytes header
        let icmptype_mask = 0xFF
        let code_mask = 0xFF
        let chksum_mask = 0xFFFF
        let id_mask = 0xFFFF
        let seqno_mask = 0xFFFF
        let mutable data : byte [] = Array.zeroCreate 0

        do
            __.Icmptype <- ICMPMessage.ICMPECHOREQUEST // 8-bit
            __.Code     <- 0x00
            __.Id       <- 0x0001
            // Checksum later
            __.Seqno    <- 0x0000
            __.Data     <- [|byte 0xAC; byte 0xDC|]
            __.Chksum   <- __.CalculateChecksum(__.Header, __.Data)
        
        static member ICMPECHOREQUEST with get() = 8
        static member ICMPECHOREPLY   with get() = 0

        member __.Header with get()      = header and
                              set(value) = header <- value
        member __.Icmptype with get()      = int header.[0] and
                                set(value) = header.[0] <- byte value
        member __.Code with get()      = int header.[1] and
                            set(value) = header.[1] <- byte value
        member __.Chksum with get()      = (int header.[2] <<< 8) ||| (int header.[3]) and
                              set(value) = header.[2] <- byte(value >>> 8)
                                           header.[3] <- byte value
        member __.Id with get()      = (int header.[4] <<< 8) ||| (int header.[5]) and
                          set(value) = header.[4] <- byte(value >>> 8)
                                       header.[5] <- byte value                                       
        member __.Seqno with get()      = (int header.[6] <<< 8) ||| (int header.[7]) and
                             set(value) = header.[6] <- byte(value >>> 8)
                                          header.[7] <- byte value
        member __.Data with get()      = data and
                            set(value) = data <- value

        member __.Print() =
            prn (sprintf "ICMPPacket.Icmptype:0x%02X" __.Icmptype)
            prn (sprintf "ICMPPacket.Code:    0x%02X" __.Code)
            prn (sprintf "ICMPPacket.Chksum:0x%04X" __.Chksum)
            prn (sprintf "ICMPPacket.Id:    0x%04X" __.Id)
            prn (sprintf "IPPacket.Seqno:   0x%04X" __.Seqno)
            __.Header |> Array.iteri (fun i l -> prn (sprintf "Header[%02d]=0x%02X" i l))
            __.Data |> Array.iteri (fun i l -> prn(sprintf "Data[%02d]=  0x%02X" i l))

        member __.CalculateChecksum(_header:byte[], _data:byte[]) =
            let mutable checksum = ((int _header.[0] <<< 8) ||| (int _header.[1]))
            checksum <- checksum + ((int _header.[4] <<< 8) ||| (int _header.[5]))
            checksum <- checksum + ((int _header.[6] <<< 8) ||| (int _header.[7]))
            for i in 0 .. 2 .. _data.Length - 1 do
                checksum <- checksum + ((int _data.[i] <<< 8) ||| (int _data.[i+1]))
            if (checksum &&& 0xFFFF0000 > 0) then 
                 checksum <- (checksum >>> 16) + (checksum &&& 0x0000FFFF)
            if (checksum &&& 0xFFFF0000 > 0) then
                 checksum <- (checksum >>> 16) + (checksum &&& 0x0000FFFF)
            checksum <- ~~~checksum
            checksum &&& 0x0000FFFF