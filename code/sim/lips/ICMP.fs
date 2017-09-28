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

namespace lips

[<AutoOpen>]
module ICMP =
    type ICMPMessage() as __ =
        let mutable header : byte[] = Array.zeroCreate 8 // 8 bytes header
        let icmptype_mask = 0xFF
        let mutable icmptype = 0x08 // 0x08 is echo request and 0x00 is echo reply   
        let code_mask = 0xFF
        let mutable code     = 0x00 // 0x00 for both echo request and echo reply
        let chksum_mask = 0xFFFF
        let mutable chksum  = 0x0000 // sum the 9 16-bit words, keeping hchksum 0, keep 
                                     // adding (and nulling) the top 16 bits in the sum 
                                     // word to the lower 16 bits, until there is no 
                                     // carry, then get the 1-complement (~). Verifying: 
                                     // Add all the 16-bit words and then the carry(s) and
                                     // take the compliment, verify it is 0000.
        let id_mask = 0xFFFF
        let mutable id       = 0x0001 // client process id, pid, default 0x0001
        let seqno_mask = 0xFFFF
        let mutable seqno    = 0x0000 // starting at 0 and incremented for each echo request
        let mutable data : byte [] = Array.zeroCreate 0
        
        static member ICMPECHOREQUEST with get() = 8
        static member ICMPECHOREPLY   with get() = 0

        member __.Header with get()      = header and
                              set(value) = header <- value
                                           // word 0: icmp type, code, checksum
                                           icmptype <- int header.[0]
                                           code     <- int header.[1]
                                           chksum   <- ((int header.[2] <<< 8) &&& 0xFF00) ||| (int header.[3] &&& 0x00FF)
                                           // word 1: identifier, sequence number 
                                           id       <- ((int header.[4] <<< 8) &&& 0xFF00) ||| (int header.[5] &&& 0x00FF)
                                           seqno    <- ((int header.[6] <<< 8) &&& 0xFF00) ||| (int header.[7] &&& 0x00FF)
        member __.Icmptype with get()      = icmptype and
                                set(value) = icmptype <- value
                                             header.[0] <- byte icmptype
        member __.Code with get()      = code and
                            set(value) = code <- value
                                         header.[1] <- byte code
        member __.Chksum with get()      = chksum and
                              set(value) = chksum <- value
                                           header.[2] <- byte(chksum >>> 8)
                                           header.[3] <- byte chksum
        member __.Id with get()      = id and
                          set(value) = id <- value
                                       header.[4] <- byte(id >>> 8)
                                       header.[5] <- byte id                                       
        member __.Seqno with get()      = seqno and
                             set(value) = seqno <- value
                                          header.[6] <- byte(seqno >>> 8)
                                          header.[7] <- byte seqno
        member __.Data with get()      = data and
                            set(value) = data <- value

        member __.Print() =
            prn (sprintf "ICMPPacket.Icmptype:0x%02X" __.Icmptype)
            prn (sprintf "ICMPPacket.Code:0x%02X" __.Code)
            prn (sprintf "ICMPPacket.Chksum:0x%04X" __.Chksum)
            prn (sprintf "ICMPPacket.Id:0x%04X" __.Id)
            prn (sprintf "IPPacket.Seqno:0x%04X" __.Seqno)
            __.Header |> Array.iteri (fun i l -> prn (sprintf "Header[%02d]=0x%02X" i l))
            __.Data |> Array.iteri (fun i l -> prn(sprintf "Data[%02d]=0x%02X" i l))

        member __.CalculateChecksum(_header:byte[], _data:byte[]) =
            let mutable checksum = 0
            _header |> Array.iteri (fun i l -> checksum <- checksum + int l)
            _data |> Array.iteri (fun i l -> checksum <- checksum + int l)
            if (checksum &&& 0xFFFF0000 > 0) then // sign problem?
                 checksum <- (checksum >>> 16) + (checksum &&& 0x0000FFFF)
            if (checksum &&& 0xFFFF0000 > 0) then
                 checksum <- (checksum >>> 16) + (checksum &&& 0x0000FFFF)
            checksum <- ~~~checksum
            checksum