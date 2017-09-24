// Copyright: 2017-, Copenhagen Business School
// Author: Rasmus Ulslev Pedersen (rp.digi@cbs.dk)

// License: Simplified BSD License
//
// LIPS: ICMP message

namespace lips

[<AutoOpen>]
module ICMP =
    let prn (str : string) = 
            System.Console.WriteLine(str)
            System.Diagnostics.Debug.WriteLine(str)

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
        do __.UpdateHeader()
        member __.Header with get() = header and
                              set(value) = header <- value
                                           __.UpdateHeader()
        member __.Icmptype with get() = icmptype and
                                set(value) = icmptype <- value
                                             __.UpdateHeader()
        member __.Code with get() = code and
                            set(value) = code <- value
                                         __.UpdateHeader()
        member __.Chksum with get() = chksum and
                              set(value) = chksum <- value
                                           __.UpdateHeader()
        member __.Id with get() = id and
                          set(value) = id <- value
                                       __.UpdateHeader()
        member __.Seqno with get() = seqno and
                             set(value) = seqno <- value
                                          __.UpdateHeader()
        member __.Data with get() = data and
                            set(value) = data <- value
                                         __.UpdateHeader()
        member __.Print() =
            prn (sprintf "ICMPPacket.Icmptype:0x%02X" __.Icmptype)
            prn (sprintf "ICMPPacket.Code:0x%02X" __.Code)
            prn (sprintf "ICMPPacket.Chksum:0x%04X" __.Chksum)
            prn (sprintf "ICMPPacket.Id:0x%04X" __.Id)
            prn (sprintf "IPPacket.Seqno:0x%04X" __.Seqno)
            __.Header |> Array.iteri (fun i l -> prn (sprintf "Header[%02d]=0x%02X" i l))
            __.Data |> Array.iteri (fun i l -> prn(sprintf "Data[%02d]=0x%02X" i l))

        member __.UpdateHeader() =
            // word 0: icmp type, code, checksum
            header.[0]  <- byte (icmptype &&& icmptype_mask) 
            header.[1]  <- byte (code &&& code_mask)
            header.[2]  <- byte ((chksum &&& chksum_mask) >>> 8)
            header.[3]  <- byte (chksum &&& chksum_mask)
            // word 1: identifier, sequence number 
            header.[4]  <- byte ((id &&& id_mask) >>> 8)
            header.[5]  <- byte (id &&& id_mask)
            header.[6]  <- byte ((seqno &&& seqno_mask) >>> 8)
            header.[7]  <- byte (seqno &&& seqno_mask)
            // data
            __.CalculateChecksum()

        member __.CalculateChecksum() =
            let mutable checksum = 0
            checksum <- checksum + int header.[0] <<< 8 + int header.[1]
            checksum <- checksum + int header.[4] <<< 8 + int header.[5]
            checksum <- checksum + int header.[6] <<< 8 + int header.[7]
            // include data in the ICMP checksum
            __.Data |> Array.iteri (fun i l -> checksum <- checksum + int l)
            if (checksum &&& 0xFFFF0000 > 0) then // sign problem?
                 checksum <- checksum >>> 16 + (checksum &&& 0x0000FFFF)
            if (checksum &&& 0xFFFF0000 > 0) then
                 checksum <- checksum >>> 16 + (checksum &&& 0x0000FFFF)
            checksum <- ~~~checksum
            header.[2]  <- byte ((checksum &&& chksum_mask) >>> 8)
            header.[3]  <- byte (checksum &&& chksum_mask)