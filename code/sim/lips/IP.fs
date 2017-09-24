// Copyright: 2017-, Copenhagen Business School
// Author: Rasmus Ulslev Pedersen (rp.digi@cbs.dk)

// License: Simplified BSD License
//
// LIPS: IP packet

namespace lips

[<AutoOpen>]
module IP =
    // Redirecting the output
    // 'prn (sprintf "%d" 123)'
    let prn (str : string) = 
            System.Console.WriteLine(str)
            System.Diagnostics.Debug.WriteLine(str)

    // length in bits
    //let ver = 4 //IPv4
    let ver_width = 4
    let ver_mask = 0b1111
    //let hdr = 5 // no options
    let hdr_width = 4
    let hdr_mask = 0b1111
    ///let tos = 0
    let tos_width = 8
    let tos_mask = 0b1111_1111
    ///let tlen = 0 // mutable, length in bytes bytes of IP header, 20, plus data
    let tlen_width = 16
    let tlen_mask = 0xFFFF
    ///let id = 1 // mutable
    let id_width = 16
    let id_mask = 0xFFFF
    ///let flags = 0b000
    let flags_width = 3
    let flags_mask = 0b111
    ///let foff = 0
    let foff_width = 13
    let foff_mask = 0b0_0000_0000_0000
    ///let ttl = 1 // same LAN
    let ttl_width = 8
    let ttl_mask = 0xFF
    //let prot = 1 // ICMP
    let prot_width = 8
    let prot_mask = 0xFF
    //let hchksum = 0; // mutable
    let hchksum_width = 16
    let hchksum_mask = 0xFFFF
    ///let srcip = 0x01_02_03_04 // local, mutable
    let srcip_width = 32
    let srcip_mask = 0xFFFFFFFF
    //let dstip = 0x01_02_03_05
    let dstip_width = 32
    let dstip_mask = 0xFFFFFFFF
    //let data = 0; // default value
    let data_width = 1024
    let data_width_bytes = 256

    type IPPacket() as __ =
      let mutable header : byte[] = Array.zeroCreate 20 // 20 bytes header
      let mutable ver     = 4 // version, 4, IPv4
      let mutable hdr     = 0 // num
      let mutable tos     = 0 // type of service
      let mutable tlen    = 0 // total length in bytes
      let mutable id      = 0 // identification
      let mutable flags   = 0 // fragment flags, 0
      let mutable foff    = 0 // flags offset, 0
      let mutable ttl     = 1 // time to live, 1
      let mutable prot    = 1 // ICMP = 1
      let mutable hchksum = 1 // sum the 9 16-bit words, keeping hchksum 0, keep adding (and nulling) 
                              // the top 16 bits in the sum word to the lower 16 bits, until there is 
                              // no carry, then get the 1-complement (~). Verifying: Add all the 16-bit 
                              // words and then the carry(s) and take the compliment, verify it is 0000.
      let mutable srcip   = 0x01_02_03_04 // default local 
      let mutable dstip  = 0x01_02_03_05 // dafault remote
      let mutable data : byte [] = Array.zeroCreate 0 
      do __.UpdateHeader()
      member __.Header with get() = header and
                            set(value) = header <- value
                                         __.UpdateHeader()
      member __.Ver with get() = ver and
                         set(value) = ver <- value
                                      __.UpdateHeader()
      member __.Hdr with get () = hdr and
                         set(value) = hdr <- value
                                      __.UpdateHeader()
      member __.Tos with get () = tos and
                         set(value) = tos <- value
                                      __.UpdateHeader()
      member __.Tlen with get () = tlen and
                          set(value) = tlen <- value
                                       __.UpdateHeader()
      member __.Id with get () = id and
                        set(value) = id <- value
                                     __.UpdateHeader()
      member __.Flags with get () = flags and
                           set(value) = flags <- value
                                        __.UpdateHeader()
      member __.Foff with get () = foff and
                          set(value) = foff <- value
                                       __.UpdateHeader()
      member __.Ttl with get () = ttl and
                         set(value) = ttl <- value
                                      __.UpdateHeader()
      member __.Prot with get () = prot and
                          set(value) = prot <- value
                                       __.UpdateHeader()
      member __.Hchksum with get () = hchksum and
                             set(value) = hchksum <- value // allow insertion of arbitrary checksum for testing purposes
      member __.SrcIP with get () = srcip and
                           set(value) = srcip <- value
                                        __.UpdateHeader()
      member __.DstIP with get () = dstip and
                            set(value) = dstip <- value
                                         __.UpdateHeader()
      member __.Data with get () = data and
                          set(value) = data <- value
                                       __.UpdateHeader()
      member __.Print() =
          prn (sprintf "IPPacket.Ver:%d" __.Ver)
          prn (sprintf "IPPacket.Hdr:%d" __.Hdr)
          prn (sprintf "IPPacket.Tos:%d" __.Tos)
          prn (sprintf "IPPacket.Id:%d" __.Id)
          prn (sprintf "IPPacket.Flags:%d" __.Flags)
          prn (sprintf "IPPacket.Offset:%d" __.Foff)
          prn (sprintf "IPPacket.Ttl:%d" __.Ttl)
          prn (sprintf "IPPacket.Prot:%d" __.Prot)
          prn (sprintf "IPPacket.Hchksum:%d" __.Hchksum)
          prn (sprintf "IPPacket.SrcIP:0x%08X" __.SrcIP)
          prn (sprintf "IPPacket.DestIP:0x%08X" __.DstIP)
          __.Header |> Array.iteri (fun i l -> prn (sprintf "Header[%02d]=0x%02X" i l))
          __.Data |> Array.iteri (fun i l -> prn(sprintf "Data[%02d]=0x%02X" i l))

      member __.UpdateHeader() =
          // word 0: version, header length, total length
          header.[0]  <- byte ((ver &&& ver_mask) <<< 4 ||| (hdr &&& hdr_mask)) 
          header.[1]  <- byte (tos &&& tos_mask)
          header.[2]  <- byte ((tlen &&& tlen_mask) >>> 8)
          header.[3]  <- byte (tlen &&& tlen_mask)
          // word 1: identification, fragment flags, fragment offset 
          header.[4]  <- byte ((id &&& id_mask) >>> 8)
          header.[5]  <- byte (id &&& id_mask)
          header.[6]  <- byte ((flags &&& flags_mask) <<< 5 ||| (foff &&& foff_mask) >>> 8)
          header.[7]  <- byte (foff &&& foff_mask)
          // word 2: time to live, protocol, header checksum
          header.[8]  <- byte (ttl &&& ttl_mask)
          header.[9]  <- byte (prot &&& prot_mask)
          header.[10] <- byte ((hchksum &&& hchksum_mask) >>> 8)
          header.[11] <- byte (hchksum &&& hchksum_mask)
          // word 3: source address
          header.[12] <- byte ((srcip &&& srcip_mask) >>> 24)
          header.[13] <- byte ((srcip &&& srcip_mask) >>> 16)
          header.[14] <- byte ((srcip &&& srcip_mask) >>> 8)
          header.[15] <- byte (srcip &&& srcip_mask)
          // word 4: destination address
          header.[16] <- byte ((dstip &&& dstip_mask) >>> 24)
          header.[17] <- byte ((dstip &&& dstip_mask) >>> 16)
          header.[18] <- byte ((dstip &&& dstip_mask) >>> 8)
          header.[19] <- byte (dstip &&& dstip_mask)
          __.CalculateChecksum()

      member __.CalculateChecksum() =
          let mutable checksum = 0
          checksum <- checksum + int header.[0] <<< 8 + int header.[1]
          checksum <- checksum + int header.[2] <<< 8 + int header.[3]
          checksum <- checksum + int header.[4] <<< 8 + int header.[5]
          checksum <- checksum + int header.[6] <<< 8 + int header.[7]
          checksum <- checksum + int header.[8] <<< 8 + int header.[9]
          checksum <- checksum + int header.[12] <<< 8 + int header.[13]
          checksum <- checksum + int header.[14] <<< 8 + int header.[15]
          checksum <- checksum + int header.[16] <<< 8 + int header.[17]
          checksum <- checksum + int header.[18] <<< 8 + int header.[19]
          if (checksum &&& 0xFFFF0000 > 0) then // sign problem?
              checksum <- checksum >>> 16 + (checksum &&& 0x0000FFFF)
          if (checksum &&& 0xFFFF0000 > 0) then
              checksum <- checksum >>> 16 + (checksum &&& 0x0000FFFF)
          checksum <- ~~~checksum
          header.[2]  <- byte ((checksum &&& hchksum_mask) >>> 8)
          header.[3]  <- byte (checksum &&& hchksum_mask)