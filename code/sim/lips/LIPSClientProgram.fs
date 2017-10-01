// Copyright: 2017-, Copenhagen Business School
// Author: Rasmus Ulslev Pedersen (rp.digi@cbs.dk)
// License: Simplified BSD License
// LIPS: Client

// Client: "1.2.3.4"
// localhost:8084

namespace lips
module MainModule = 

    open System
    open LIPSLIB.Util
    open LIPSLIB.ICMP
    open LIPSLIB.IP
    open LIPSLIB.HTTPUTIL

    // LOCAL CLIENT/HOST
    // ngrok http -subdomain=pingrt -host-header=localhost 8080
    let localaddr = "http://localhost:8084/"
    
    // REMOTE SERVER
    //let blaaurl = "http://pingrt.ngrok.io/"
    //let blaaurl = "http://dbaa8026.ngrok.io/"
    let remoteaddr = "http://localhost:8085/"
 
    [<EntryPoint>]
    let main argv = 
        // Listen for server responses
        startlisterner (localaddr, remoteaddr)
        // Issue PINGs toward the server
        let mutable i = 0
        while i < 1 do // true do
            // ICMP echo request
            let icmpmsg = ICMPMessage()
            icmpmsg.Seqno <- i
            icmpmsg.Data <- [|byte 0xCA; byte 0xFE; byte 0xBA; byte 0xBE|]
            icmpmsg.Chksum <- icmpmsg.CalculateChecksum()
            // IP packet
            let ippacket = IPPacket()
            // Test
            //ippacket.Tlen <- 0x0073
            //ippacket.Flags <- 0b010
            //ippacket.Ttl <- 0x40
            //ippacket.Prot <- 0x11
            //ippacket.SrcIP <- 0xC0A80001
            //ippacket.DstIP <- 0xC0A800C7
            //ippacket.UpdateHchksum() // 0xB861
            // Insert ICMP rtequest in IP
            ippacket.Data    <- Array.concat [icmpmsg.Header; icmpmsg.Data]
            ippacket.Tlen    <- ippacket.CalculateTlen()
            ippacket.Hchksum <- ippacket.CalculateChecksum()
            prn "New IP/ICMP to be sent to remote server:"
            ippacket.Print()
            icmpmsg.Print()
            // Issue GET to server
            let iptxt = bytearray2str(Array.concat [ippacket.Header;ippacket.Data])
            prn "Done sending request"
            let blaahund = Async.RunSynchronously (fetchUrl (remoteaddr + iptxt))
            i <- i + 1
            System.Threading.Thread.Sleep 5000

        let e = IPPacket.ICMPPROTOCOL
        System.Console.ReadKey() |> ignore
        0 // return an integer exit code
