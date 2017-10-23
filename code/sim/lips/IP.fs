// Copyright: 2017-, Copenhagen Business School
// Author: Rasmus Ulslev Pedersen (rp.digi@cbs.dk)
// License: Simplified BSD License
// LIPS: IP packet

// RFC 791:  Internet Header Format
//  0                   1                   2                   3
//  0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1
// +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
// |Version|  IHL  |Type of Service|          Total Length         |
// +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
// |         Identification        |Flags|      Fragment Offset    |
// +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
// |  Time to Live |    Protocol   |         Header Checksum       |
// +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
// |                       Source Address                          |
// +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
// |                    Destination Address                        |
// +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
// |                    Options                    |    Padding    |
// +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+

namespace LIPSLIB
[<AutoOpen>]
module IP =
    open System
    /// LIPS IPPacket
    type IPPacket() as __ =
        let ver_mask     = 0b1111_0000
        let hdr_mask     = 0b0000_1111
        let flags_mask   = 0b111
       
        let mutable header : byte[] = Array.zeroCreate 20 // 20 bytes header
        let mutable data : byte array = Array.zeroCreate 0 

        do  // Create "standard" packet
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
            __.Tlen    <- __.CalculateTlen() // total length in bytes
            __.Hchksum <- __.CalculateChecksum()

        static member ICMPPROTOCOL = 0x01 // .Prot for ICMP = 1
        static member UDPPROTOCOL  = 0x11 // .Prot for UDP = 17 

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
        
        /// Calculate Tlen [bytes]
        /// Use after updating payload Data
        member __.CalculateTlen() =
            __.Header.Length + __.Data.Length
        
        /// Header checksum
        /// Sum the 9 16-bit words, keeping hchksum 0, keep adding (and nulling) 
        /// the top 16 bits in the sum word to the lower 16 bits, until there is 
        /// no carry, then get the 1-complement (~). Verifying: Add all the 16-bit 
        /// words and then the carry(s) and take the compliment, verify it is 0000.
        member __.CalculateChecksum () =
            // Test: 4500 0073 0000 4000 4011 *b861* c0a8 0001 c0a8 00c7
            let mutable checksum = 0
            checksum <- checksum + (int header.[0] <<< 8) + int header.[1]
            checksum <- checksum + (int header.[2] <<< 8) + int header.[3]
            checksum <- checksum + (int header.[4] <<< 8) + int header.[5]
            checksum <- checksum + (int header.[6] <<< 8) + int header.[7]
            checksum <- checksum + (int header.[8] <<< 8) + int header.[9]
            // skip header.[10] and header.[11]
            checksum <- checksum + (int header.[12] <<< 8) + int header.[13]
            checksum <- checksum + (int header.[14] <<< 8) + int header.[15]
            checksum <- checksum + (int header.[16] <<< 8) + int header.[17]
            checksum <- checksum + (int header.[18] <<< 8) + int header.[19]
            checksum <- checksumCarryRound(checksum)
            checksum
        
        member __.Print() =
            // word 0: version, header length, total length
            // word 1: identification, fragment flags, fragment offset 
            // word 2: time to live, protocol, header checksum
            // word 3: source address
            // word 4: destination address
            prn (sprintf "IPPacket.Ver:        0x%X             Header[0 ]:          0x%02X (<- 4-bit)" 
                          __.Ver (header.[0] &&& byte 0xF0))
            prn (sprintf "IPPacket.Hdr:        0x%X             Header[ 0]:          0x%02X (4-bit ->)" 
                          __.Hdr (header.[0] &&& byte 0x0F))
            prn (sprintf "IPPacket.Tos:        0x%02X            Header[1]:           0x%02X" 
                          __.Tos header.[1])
            prn (sprintf "IPPacket.Tlen:       0x%04X          Header[2,3]:         0x%02X 0x%02X" 
                          __.Tlen header.[2] header.[3])
            prn (sprintf "IPPacket.Id:         0x%04X          Header[4,5]:         0x%02X 0x%02X" 
                          __.Id header.[4] header.[5])
            prn (sprintf "IPPacket.Flags:      0b%s           Header[6]:           0x%02X"
                          (Convert.ToString(__.Flags,2).PadLeft(3, '0')) header.[6])
            prn (sprintf "IPPacket.Offset:     0b%s Header[6,7]:         0x%02X 0x%02X" 
                          (Convert.ToString(__.Foff,2).PadLeft(13, '0')) (header.[6] &&& byte 0b11111) header.[7])
            prn (sprintf "IPPacket.Ttl:        0x%02X            Header[8]:           0x%02X" 
                          __.Ttl header.[8])
            prn (sprintf "IPPacket.Prot:       0x%02X            Header[9]:           0x%02X" 
                          __.Prot header.[9])
            prn (sprintf "IPPacket.Hchksum:    0x%04X          Header[10,11]:       0x%02X 0x%02X" 
                          __.Hchksum header.[10] header.[11])
            prn (sprintf "IPPacket.SrcIP:      0x%08X      Header[12,13,14,15]: 0x%02X 0x%02X 0x%02X 0x%02X"
                          __.SrcIP header.[12] header.[13] header.[14] header.[15])
            prn (sprintf "IPPacket.DestIP:     0x%08X      Header[16,17,18,19]: 0x%02X 0x%02X 0x%02X 0x%02X"
                         __.DstIP header.[16] header.[17] header.[18] header.[19])
            for i in 0 .. data.Length - 1 do
                if i % 4 = 0 then
                    pr (sprintf "IPPacket.Data[%02d]:   " i)
                pr (sprintf  "0x%02X " data.[i])
                if (i + 1) % 4 = 0 then prn ""
            if data.Length % 4 <> 0 then prn ""
            //__.Data   |> Array.iteri (fun i l -> prn(sprintf  "Data[%02d]: 0x%02X" i l))

        /// Convert or "raise" the IP header and data bytes to ascii and return as a string
        member __.AsStr() =
            bytearray2str(Array.concat [__.Header; __.Data])

        member __.IsICMP() =
            __.Prot = IPPacket.ICMPPROTOCOL

        member __.IsUDP() =
            __.Prot = IPPacket.UDPPROTOCOL

        member __.SwitchSrcDstIP() =
            let srcip = __.SrcIP
            let dstip = __.DstIP
            __.SrcIP <- dstip
            __.DstIP <- srcip   
            ()
