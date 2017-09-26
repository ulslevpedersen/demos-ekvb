// Copyright: 2017-, Copenhagen Business School
// Author: Rasmus Ulslev Pedersen (rp.digi@cbs.dk)
// License: Simplified BSD License
// LIPS: IP packet

namespace lips
[<AutoOpen>]
module IP =
    open System

    type IPPacket() as __ =
        let ver_mask     = 0b1111
        let hdr_mask     = 0b1111
        let tos_mask     = 0b1111_1111
        let tlen_mask    = 0xFFFF
        let id_mask      = 0xFFFF
        let flags_mask   = 0b111
        let foff_mask    = 0b1_1111_1111_1111
        let ttl_mask     = 0xFF
        let prot_mask    = 0xFF
        let hchksum_mask = 0xFFFF
        let srcip_mask   = 0xFFFFFFFF
        let dstip_mask   = 0xFFFFFFFF
        
        let mutable header : byte[] = Array.zeroCreate 20 // 20 bytes header
        let mutable ver     = 0 // version
        let mutable hdr     = 0 // num of 32-bit words
        let mutable tos     = 0 // type of service
        let mutable tlen    = 0 // total length in bytes
        let mutable id      = 0 // identification
        let mutable flags   = 0 // fragment flags
        let mutable foff    = 0 // flags offset
        let mutable ttl     = 0 // time to live
        let mutable prot    = 0 // ICMP = 1
        let mutable hchksum = 0 // sum the 9 16-bit words, keeping hchksum 0, 
                                // keep adding (and nulling) 
                                // the top 16 bits in the sum word to the lower 16 
                                // bits, until there is 
                                // no carry, then get the 1-complement (~). 
                                // Verifying: Add all the 16-bit 
                                // words and then the carry(s) and take the 
                                // compliment, verify it is 0000.
        let mutable srcip   = 0  // default local 
        let mutable dstip   = 0  // dafault remote
        let mutable data : byte array = Array.zeroCreate 0 

        do
            __.Ver     <-  4 // version, 4, IPv4
            __.Hdr     <-  5 // num of 32-bit words (5)
            __.Tos     <-  0 // type of service
            // Tlen, later
            __.Id      <-  0 // identification
            __.Flags   <-  0 // fragment flags, 0
            __.Foff    <-  0 // flags offset, 0
            __.Ttl     <-  1 // time to live, 1
            __.Prot    <-  1 // ICMP = 1
            // Hchksum, later
            __.SrcIP   <- 0x01020304 // default local 1.2.3.4
            __.DstIP   <- 0x01020305 // dafault remote 1.2.3.5
            __.Data    <- Array.zeroCreate 0 // can be actual testing data 
            __.Tlen    <- 20 // total length in bytes
            __.Hchksum <- __.CalculateChecksum(__.Header, true)

        /// Version 4-bits .[0H]
        member __.Ver with get()      = ver and
                           set(value) = ver <- value
                                        header.[0] <- byte (((ver &&& ver_mask) <<< 4) 
                                                           ||| (hdr &&& hdr_mask)) 
        /// Header 4-bits .[0L], number of 32-bit words
        member __.Hdr with get ()     = hdr and
                           set(value) = hdr <- value
                                        header.[0] <- byte (((ver &&& ver_mask) <<< 4) 
                                                           ||| (hdr &&& hdr_mask)) 
        /// Type of service 8-bits .[1]
        member __.Tos with get ()     = tos and
                           set(value) = tos <- value
                                        header.[1] <- byte (tos &&& tos_mask)
        /// Total length in byte, 16-bits .[2] .[3]         
        member __.Tlen with get ()     = tlen and
                            set(value) = tlen <- value
                                         header.[2] <- byte ((tlen &&& tlen_mask) >>> 8)
                                         header.[3] <- byte (tlen &&& tlen_mask)
        /// Identification, 16-bits, .[4] .[5]
        member __.Id with get ()     = id and
                          set(value) = id <- value
                                       header.[4] <- byte ((id &&& id_mask) >>> 8)
                                       header.[5] <- byte (id &&& id_mask)      
        /// Flags, 3-bits, .[6H]
        member __.Flags with get ()     = flags and
                             set(value) = flags <- value
                                          header.[6] <- byte (((flags &&& flags_mask) <<< 5) 
                                                        ||| (int header.[6] &&& 0b11111))              
        /// Fragment offset, 13-bits, .[6L] .[7]
        member __.Foff with get ()     = foff and
                            set(value) = foff <- value
                                         header.[6] <- byte ((int header.[6] &&& 0b11100000) 
                                                       ||| ((foff &&& foff_mask) >>> 8))
                                         header.[7] <- byte (foff &&& foff_mask)
        /// Time to live, 8-bits, .[8]
        member __.Ttl with get ()     = ttl and
                           set(value) = ttl <- value
                                        header.[8] <- byte (ttl &&& ttl_mask)
        /// Protocol, 8-bits, .[9]
        member __.Prot with get ()     = prot and
                            set(value) = prot <- value
                                         header.[9] <- byte (prot &&& prot_mask)
        /// Checksum, 16-bits, .[10] .[11]
        /// Sum the 9 16-bit words, keeping hchksum 0, 
        /// keep adding (and nulling) 
        /// the top 16 bits in the sum word to the lower 16 
        /// bits, until there is 
        /// no carry, then get the 1-complement (~). 
        /// Verifying: Add all the 16-bit 
        /// words and then the carry(s) and take the 
        /// compliment, verify it is 0000.
        member __.Hchksum with get ()     = hchksum and
                               set(value) = hchksum <- value
                                            header.[10] <- byte ((hchksum &&& hchksum_mask) >>> 8)
                                            header.[11] <- byte (hchksum &&& hchksum_mask)
        /// Source IP, 32-bits, .[12] .[13] .[14] .[15]
        member __.SrcIP with get ()     = srcip and
                             set(value) = srcip <- value
                                          header.[12] <- byte ((srcip &&& srcip_mask) >>> 24)
                                          header.[13] <- byte ((srcip &&& srcip_mask) >>> 16)
                                          header.[14] <- byte ((srcip &&& srcip_mask) >>> 8)
                                          header.[15] <- byte (srcip &&& srcip_mask)
        /// Destination IP, 32-bits, .[16] .[17] .[18] .[19] 
        member __.DstIP with get ()     = dstip and
                             set(value) = dstip <- value
                                          header.[16] <- byte ((dstip &&& dstip_mask) >>> 24)
                                          header.[17] <- byte ((dstip &&& dstip_mask) >>> 16)
                                          header.[18] <- byte ((dstip &&& dstip_mask) >>> 8)
                                          header.[19] <- byte (dstip &&& dstip_mask)
        member __.Header with get()      = header and
                              set(value) = header <- value
                                           ver    <- (int header.[0] &&& ver_mask) <<< 4
                                           hdr    <- int header.[0] &&& hdr_mask 
                                           tos    <- int header.[1] &&& tos_mask
                                           tlen   <- (int header.[2] <<< 8 
                                                      ||| int header.[3]) &&& tlen_mask
                                           id     <- (int header.[4] <<< 8 
                                                      ||| int header.[5]) &&& id_mask
                                           flags  <- (int header.[6] >>> 5) &&& flags_mask
                                           foff   <- (((int header.[6] &&& 0b11111) <<< 8) 
                                                        ||| int header.[7]) &&& foff_mask
                                           ttl    <- int header.[7] &&& ttl_mask
                                           prot   <- int header.[8] &&& prot_mask
                                           hchksum<- ((int header.[10] <<< 8) &&& int header.[1]) 
                                                       &&& hchksum 
                                           srcip  <- (int header.[12] <<< 24) |||
                                                     (int header.[13] <<< 16) |||
                                                     (int header.[14] <<< 8)  |||
                                                      int header.[15] 
                                           dstip  <- (int header.[16] <<< 24) |||
                                                     (int header.[17] <<< 16) |||
                                                     (int header.[18] <<< 8)  |||
                                                      int header.[19] 

        /// Data, byte array, such an an ICMP message
        /// Manually update TLen and Hchksum
        member __.Data with get ()     = data and
                            set(value) = data <- value
        
        /// Update Tlen
        /// Use after updating payload Data
        member __.UpdateTlen() =
            tlen <- hdr * 4 + data.Length

        /// Update Hchksum
        /// Use after updating any header field, but usually Data
        member __.UpdateHchksum() =
            __.Hchksum <- __.CalculateChecksum(header, true) 
                          &&& hchksum_mask
      
        /// Header checksum
        /// If zero then zero out the checksum fields in .[10] and .[11]
        /// Zero is false when verifying a checksum and true when calculating a new one
        member __.CalculateChecksum(_hdr:byte[], zero:bool) : int =
            // Test: 4500 0073 0000 4000 4011 *b861* c0a8 0001 c0a8 00c7
            let mutable checksum = 0
            checksum <- checksum + (int _hdr.[0] <<< 8) + int _hdr.[1]
            checksum <- checksum + (int _hdr.[2] <<< 8) + int _hdr.[3]
            checksum <- checksum + (int _hdr.[4] <<< 8) + int _hdr.[5]
            checksum <- checksum + (int _hdr.[6] <<< 8) + int _hdr.[7]
            checksum <- checksum + (int _hdr.[8] <<< 8) + int _hdr.[9]
            if (not zero) then
                checksum <- checksum + (int _hdr.[10] <<< 8) + int _hdr.[11]
            checksum <- checksum + (int _hdr.[12] <<< 8) + int _hdr.[13]
            checksum <- checksum + (int _hdr.[14] <<< 8) + int _hdr.[15]
            checksum <- checksum + (int _hdr.[16] <<< 8) + int _hdr.[17]
            checksum <- checksum + (int _hdr.[18] <<< 8) + int _hdr.[19]
            if (checksum &&& 0xFFFF0000 > 0) then // sign problem?
                checksum <- (checksum >>> 16) + (checksum &&& 0x0000FFFF)
            if (checksum &&& 0xFFFF0000 > 0) then
                checksum <- (checksum >>> 16) + (checksum &&& 0x0000FFFF)
            checksum <- ~~~checksum &&& hchksum_mask
            checksum

        member __.Print() =
            // word 0: version, header length, total length
            // word 1: identification, fragment flags, fragment offset 
            // word 2: time to live, protocol, header checksum
            // word 3: source address
            // word 4: destination address
            prn (sprintf "IPPacket.Ver:    0x%X,             Header[0 ]         =0x%02X" 
                          __.Ver header.[0])
            prn (sprintf "IPPacket.Hdr:    0x%X,             Header[ 0]         =0x%02X" 
                          __.Hdr header.[0])
            prn (sprintf "IPPacket.Tos:    0x%02X,            Header[1]          =0x%02X" 
                          __.Tos header.[1])
            prn (sprintf "IPPacket.Tlen:   0x%04X,          Header[2,3]        =0x%02X 0x%02X" 
                          __.Tlen header.[2] header.[3])
            prn (sprintf "IPPacket.Id:     0x%04X,          Header[4,5]        =0x%02X 0x%02X" 
                          __.Id header.[4] header.[5])
            prn (sprintf "IPPacket.Flags:  0b%s,           Header[6]          =0x%02X"
                          (Convert.ToString(__.Flags,2).PadLeft(3, '0')) header.[6])
            prn (sprintf "IPPacket.Offset: 0b%s, Header[6,7]        =0x%02X 0x%02X" 
                          (Convert.ToString(__.Foff,2).PadLeft(13, '0')) (header.[6] &&& byte 0b11111) header.[7])
            prn (sprintf "IPPacket.Ttl:    0x%02X,            Header[8]          =0x%02X" 
                          __.Ttl header.[8])
            prn (sprintf "IPPacket.Prot:   0x%02X,            Header[09]         =0x%02X" 
                          __.Prot header.[9])
            prn (sprintf "IPPacket.Hchksum:0x%04X,          Header[10,11]      =0x%02X 0x%02X" 
                          __.Hchksum header.[10] header.[11])
            prn (sprintf "IPPacket.SrcIP:  0x%08X,      Header[12,13,14,15]=0x%02X 0x%02X 0x%02X 0x%02X"
                          __.SrcIP header.[12] header.[13] header.[14] header.[15])
            prn (sprintf "IPPacket.DestIP: 0x%08X,      Header[16,17,18,19]=0x%02X 0x%02X 0x%02X 0x%02X"
                         __.DstIP header.[12] header.[13] header.[14] header.[15])
            //__.Header |> Array.iteri (fun i l -> prn (sprintf "Header[%02d]=0x%02X" i l))
            __.Data   |> Array.iteri (fun i l -> prn(sprintf  "Data[%02d]  =0x%02X" i l))