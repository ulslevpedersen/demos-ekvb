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
            __.Data     <- [|byte 0xAC|] //[|byte 0xAC; byte 0xDC|]
            __.Chksum   <- __.CalculateChecksum()
        
        static member ICMPECHOREQUEST with get() = 8
        static member ICMPECHOREPLY   with get() = 0

        member __.Header with get()        = header and
                              set(value)   = header <- value
        member __.Icmptype with get()      = int header.[0] and
                                set(value) = header.[0] <- byte value
        member __.Code with get()          = int header.[1] and
                            set(value)     = header.[1] <- byte value
        member __.Chksum with get()        = (int header.[2] <<< 8) ||| (int header.[3]) and
                              set(value)   = header.[2] <- byte(value >>> 8)
                                             header.[3] <- byte value
        member __.Id with get()            = (int header.[4] <<< 8) ||| (int header.[5]) and
                          set(value)       = header.[4] <- byte(value >>> 8)
                                             header.[5] <- byte value                                       
        member __.Seqno with get()         = (int header.[6] <<< 8) ||| (int header.[7]) and
                             set(value)    = header.[6] <- byte(value >>> 8)
                                             header.[7] <- byte value
        member __.Data with get()          = data and
                            set(value)     = data <- value

        member __.Print() =
            prn (sprintf "ICMPPacket.Icmptype: 0x%02X            Header[0]:   0x%02X" __.Icmptype __.Header.[0])
            prn (sprintf "ICMPPacket.Code:     0x%02X            Header[1]:   0x%02X" __.Code __.Header.[1])
            prn (sprintf "ICMPPacket.Chksum:   0x%04X          Header[2,3]: 0x%02X 0x%02X" __.Chksum __.Header.[2] __.Header.[3])
            prn (sprintf "ICMPPacket.Id:       0x%04X          Header[4,5]: 0x%02X 0x%02X" __.Id __.Header.[4] __.Header.[5])
            prn (sprintf "ICMPPacket.Seqno:    0x%04X          Header[6,7]: 0x%02X 0x%02X" __.Seqno __.Header.[6] __.Header.[7])
            //__.Header |> Array.iteri (fun i l -> prn (sprintf "Header[%02d]=0x%02X" i l))
            for i in 0 .. data.Length - 1 do
                if i % 4 = 0 then
                    pr (sprintf "ICMPPacket.Data[%02d]: " i)
                pr (sprintf "0x%02X " data.[i])
                if (i + 1) % 4 = 0 then 
                    prn ""

        member __.CalculateChecksum() =
            let mutable checksum = 0
            checksum <- checksum + ((int header.[0] <<< 8) ||| (int header.[1]))
            checksum <- checksum + ((int header.[4] <<< 8) ||| (int header.[5]))
            checksum <- checksum + ((int header.[6] <<< 8) ||| (int header.[7]))
            checksum <- checksum + checksumData(__.Data)
            checksum <- checksumCarryRound(checksum)
            checksum

        member __.CreateICMPEchoReply(icmprequest:ICMPMessage) =
            let icmpreply = new ICMPMessage()
            icmpreply.Header <- Array.copy icmprequest.Header
            icmpreply.Data <- Array.copy icmprequest.Data
            icmpreply.Icmptype <- ICMPMessage.ICMPECHOREPLY
            icmpreply.Chksum <- icmpreply.CalculateChecksum()
            icmpreply

        member __.IsICMPEchoRequest() =
            __.Icmptype = ICMPMessage.ICMPECHOREQUEST

        member __.IsICMPEchoReply() =
            __.Icmptype = ICMPMessage.ICMPECHOREPLY