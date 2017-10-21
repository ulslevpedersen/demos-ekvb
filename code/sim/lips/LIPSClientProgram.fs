// Copyright: 2017-, Copenhagen Business School
// Author: Rasmus Ulslev Pedersen (rp.digi@cbs.dk)
// License: Simplified BSD License
// LIPS: Client

// Client: "1.2.3.4"
// Server: "1.2.3.5"

namespace lips
module MainModule = 

    open System
    open LIPSLIB.Util
    open LIPSLIB
    open LIPSLIB.UDP
    open LIPSLIB.ICMP
    open LIPSLIB.HTTPUTIL

    // LOCAL CLIENT (LIPSClient)
    // ngrok http -subdomain=pingrt -host-header=localhost 8080
    let localaddr = "http://localhost:8080/" 
    
    // REMOTE SERVER 
    //LIPSServer when testing on localhost or second laptop:
    //let remoteaddr = "http://pingrt1.ngrok.io/" //-> localhost:8081
    //ms:
    let remoteaddr = "http://d51eb215.ngrok.io/"
    
    /// Send UDP datagrams to server
    let sendudp() =
        async {
            let mutable i = 0
            while true do
                let udpdatagram = UDPDatagram()
                udpdatagram.IP.Tlen    <- udpdatagram.IP.CalculateTlen()
                udpdatagram.IP.Hchksum <- udpdatagram.IP.CalculateChecksum()
                prn (sprintf "(%d) New IP/UDP to be sent to remote server:" i)
                udpdatagram.IP.Print()
                udpdatagram.Print()
                if txQueue.Put(remoteaddr + udpdatagram.IP.AsStr()) then
                    prn (sprintf "TX queued new UDP datagram %d" i)
                else
                    prn (sprintf "Could not queue new UDP datagram %d" i)
                i <- i + 1
                System.Threading.Thread.Sleep 10000
        }

    // Send pings toward server
    let sendping() = 
        async { 
            let mutable i = 0
            while true do
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
                prn (sprintf "(%d) New IP/ICMP to be sent to remote server:" i)
                ippacket.Print()
                icmpmsg.Print()
                if txQueue.Put(remoteaddr + ippacket.AsStr()) then
                    prn (sprintf "TX queued new ICMP request %d" i)
                else
                    prn (sprintf "Could not queue new ICMP request %d" i)
                i <- i + 1
                System.Threading.Thread.Sleep 10000
        }
 
    [<EntryPoint>]
    let main argv = 
        A.Instance.SetOut(System.Console.Out)
        Async.Start (startFetcher())
        // Listen for server new packets / responses
        startlisterner (localaddr, remoteaddr)
        // Issue PINGs toward the server
        // Async.Start(sendping())
        Async.Start(sendudp())
        System.Console.ReadKey() |> ignore
        0
