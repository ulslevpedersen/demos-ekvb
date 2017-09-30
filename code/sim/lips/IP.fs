// Copyright: 2017-, Copenhagen Business School
// Author: Rasmus Ulslev Pedersen (rp.digi@cbs.dk)
// License: Simplified BSD License
// LIPS: IP packet

//RFC 791:  Internet Header Format
//   0                   1                   2                   3
//   0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1
//  +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
//  |Version|  IHL  |Type of Service|          Total Length         |
//  +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
//  |         Identification        |Flags|      Fragment Offset    |
//  +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
//  |  Time to Live |    Protocol   |         Header Checksum       |
//  +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
//  |                       Source Address                          |
//  +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
//  |                    Destination Address                        |
//  +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
//  |                    Options                    |    Padding    |
//  +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+

namespace LIPSLIB
[<AutoOpen>]
module IP =
    open System
    /// LIPS IPPacket
    type IPPacket() as __ =
        let ver_mask     = 0b1111_0000
        let hdr_mask     = 0b0000_1111
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
        let mutable data : byte array = Array.zeroCreate 0 

        do
            __.Ver     <-  4 // version, 4, IPv4
            __.Hdr     <-  5 // num of 32-bit words (5)
            __.Tos     <-  0 // type of service
            // Tlen, later
            __.Id      <-  0 // identification
            __.Flags   <-  0 // fragment flags, 0
            __.Foff    <-  0 // flags offset, 0
            __.Ttl     <- 10 // time to live, 1
            __.Prot    <-  1 // ICMP = 1
            // Hchksum, later
            __.SrcIP   <- 0x01020304 // default local 1.2.3.4
            __.DstIP   <- 0x01020305 // dafault remote 1.2.3.5
            __.Data    <- Array.zeroCreate 0 // can be actual testing data 
            __.Tlen    <- 20 // total length in bytes
            __.Hchksum <- __.CalculateChecksum(__.Header)
        static member ICMPPROTOCOL  = 1 // .Prot
        member __.Header with get()      = header and
                              set(value) = header <- value
        /// Version 4-bits .[V0]
        member __.Ver with get()      = int (header.[0] >>> 4) and
                           set(value) = header.[0] <- byte (value <<< 4) 
                                                      ||| byte (int header.[0] &&& hdr_mask)
        /// Header 4-bits .[0H], number of 32-bit words
        member __.Hdr with get ()     = int header.[0] &&& hdr_mask and
                           set(value) = header.[0] <- byte (int header.[0] &&& ver_mask) 
                                                      ||| byte (value &&& hdr_mask) 
        /// Type of service 8-bits .[1]
        member __.Tos with get ()     = int header.[1] and
                           set(value) = header.[1] <- byte value
        /// Total length in byte, 16-bits .[2] .[3]         
        member __.Tlen with get ()     = (int header.[2] <<< 8) ||| int header.[3] and
                            set(value) = header.[2] <- byte (value >>> 8) 
                                         header.[3] <- byte value
        /// Identification, 16-bits, .[4] .[5]
        member __.Id with get ()     = (int header.[4] <<< 8) ||| int header.[5] and
                          set(value) = header.[4] <- byte (value >>> 8)
                                       header.[5] <- byte value      
        /// Flags, 3-bits, .[6H]
        member __.Flags with get ()     = int header.[6] >>> 5 and
                             set(value) = header.[6] <- byte (((value &&& flags_mask) <<< 5) 
                                                        ||| (int header.[6] &&& 0b11111))              
        /// Fragment offset, 13-bits, .[6L] .[7]
        member __.Foff with get ()     = ((int header.[6] &&& 0b11111) <<< 8) ||| int header.[7] and
                            set(value) = header.[6] <- byte (int header.[6] &&& 0b11100000) ||| byte (value >>> 8)
                                         header.[7] <- byte value
        /// Time to live, 8-bits, .[8]
        member __.Ttl with get ()     = int header.[8] and
                           set(value) = header.[8] <- byte value
        /// Protocol, 8-bits, .[9]
        member __.Prot with get ()     = int header.[9] and
                            set(value) = header.[9] <- byte value
        /// Checksum, 16-bits, .[10] .[11]
        member __.Hchksum with get ()     = (int header.[10] <<< 8) ||| (int header.[11]) and
                               set(value) = header.[10] <- byte (value >>> 8)
                                            header.[11] <- byte value
        /// Source IP, 32-bits, .[12] .[13] .[14] .[15]
        member __.SrcIP with get ()     = (int header.[12] <<< 24) ||| (int header.[13] <<< 16) |||
                                          (int header.[14] <<<  8) ||| (int header.[15]) and
                             set(value) = header.[12] <- byte (value >>> 24) // check the shift
                                          header.[13] <- byte (value >>> 16)
                                          header.[14] <- byte (value >>> 8)
                                          header.[15] <- byte value 
        /// Destination IP, 32-bits, .[16] .[17] .[18] .[19] 
        member __.DstIP with get ()     = (int header.[16] <<< 24) ||| (int header.[17] <<< 16) |||
                                          (int header.[18] <<<  8) ||| (int header.[19]) and
                             set(value) = header.[16] <- byte (value >>> 24)
                                          header.[17] <- byte (value >>> 16)
                                          header.[18] <- byte (value >>> 8)
                                          header.[19] <- byte value
        /// Data, byte array, such an an ICMP message
        /// Manually update TLen and Hchksum
        member __.Data with get ()     = data and
                            set(value) = data <- value
                                         //__.UpdateTlen()
                                         //__.Hchksum <- __.CalculateChecksum(__.Header)
        /// Calculate Tlen
        /// Use after updating payload Data
        member __.CalculateTlen() =
            __.Header.Length + __.Data.Length
        /// Header checksum
        /// Sum the 9 16-bit words, keeping hchksum 0, keep adding (and nulling) 
        /// the top 16 bits in the sum word to the lower 16 bits, until there is 
        /// no carry, then get the 1-complement (~). Verifying: Add all the 16-bit 
        /// words and then the carry(s) and take the compliment, verify it is 0000.
        member __.CalculateChecksum (_hdr:byte[]) =
            // Test: 4500 0073 0000 4000 4011 *b861* c0a8 0001 c0a8 00c7
            let mutable checksum = 0
            checksum <- checksum + (int _hdr.[0] <<< 8) + int _hdr.[1]
            checksum <- checksum + (int _hdr.[2] <<< 8) + int _hdr.[3]
            checksum <- checksum + (int _hdr.[4] <<< 8) + int _hdr.[5]
            checksum <- checksum + (int _hdr.[6] <<< 8) + int _hdr.[7]
            checksum <- checksum + (int _hdr.[8] <<< 8) + int _hdr.[9]
            // skip header.[10] and header.[11]
            checksum <- checksum + (int _hdr.[12] <<< 8) + int _hdr.[13]
            checksum <- checksum + (int _hdr.[14] <<< 8) + int _hdr.[15]
            checksum <- checksum + (int _hdr.[16] <<< 8) + int _hdr.[17]
            checksum <- checksum + (int _hdr.[18] <<< 8) + int _hdr.[19]
            if (checksum &&& 0xFFFF0000 > 0) then // sign problem?
                checksum <- (checksum >>> 16) + (checksum &&& 0x0000FFFF)
            if (checksum &&& 0xFFFF0000 > 0) then
                checksum <- (checksum >>> 16) + (checksum &&& 0x0000FFFF) //TODO: enough?
            checksum <- ~~~checksum 
            checksum &&& hchksum_mask
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
                         __.DstIP header.[16] header.[17] header.[18] header.[19])
            __.Data   |> Array.iteri (fun i l -> prn(sprintf  "Data[%02d]  =0x%02X" i l))