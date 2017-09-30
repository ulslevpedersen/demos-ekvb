// Copyright: 2017-, Copenhagen Business School
// Author: Rasmus Ulslev Pedersen (rp.digi@cbs.dk)
// License: Simplified BSD License
// LIPS: Client

// Client: "1.2.3.4"
// localhost:8084

namespace lips
module MainModule = 

    open System
    open System.Net
    open System.IO
    open System.Text
    open LIPSLIB.Util
    open LIPSLIB.BH
    open LIPSLIB.ICMP
    open LIPSLIB.IP
    open LIPSLIB.HTTPUTIL

    // ngrok http -subdomain=pingrt -host-header=localhost 8080
    let host = "http://localhost:8080/"
 
    // startlistener
    let startlistener =
        // Listen for http requests
        listener ((fun req resp ->
            async {
                //let txt = Encoding.ASCII.GetBytes(output req)
                let iptxtin = req.Url.LocalPath.ToString().[1..]
                prn (sprintf "\nLIPS CLIENT received  : \"%s\"" iptxtin)
                let isBlaaHund = BHPacket.VerifyBHStr(iptxtin)
                if isBlaaHund then
                    ipprint(iptxtin) |> ignore
                let reply = ipdecode(iptxtin)
                
                let mutable iptxtout = ""
                if reply.IsSome then
                    let ipreply = reply.Value 
                    iptxtout <- bytearray2str(Array.concat [ipreply.Header;ipreply.Data])
                    // blaaurl is server
                    //let blaaurl = "http://pingrt.ngrok.io/" + String.Join("", iptxtout)
                    let blaaurl = "http://dbaa8026.ngrok.io/" + String.Join("", iptxtout)
                    //let blaaurl = "http://localhost:8085/" + String.Join("", iptxtout)
                    let blaahund = fetchUrl myCallbacker blaaurl
                    prn (sprintf "This IP reply was sent:%s " iptxtout)

                // TODO: Need to decode iptextin and see if it a valid IP packet
                // and a valid ICMP message
                // If it is that then I need to send a valid echo back to the
                // sender
                let datetxt = System.DateTime.Now.ToString();
                let txt = Encoding.ASCII.GetBytes(
                              "(Info from LIPS client: \"" + datetxt + "\")\n"            +
                              "(             Received: \"" + iptxtin + "\")\n"            +
                              "(            BlaaHund?: \"" + string(isBlaaHund) + "\")\n" +
                              "(       Client replied: \"" + iptxtout + "\")\n")
                resp.ContentType <- "text/plain"
                resp.OutputStream.Write(txt, 0, txt.Length)
                resp.OutputStream.Close()
            }), host)

    [<EntryPoint>]
    let main argv = 
        // Listen for server responses
        startlistener
        // Issue PINGs toward the server
        let mutable i = 0
        while true do
            // ICMP echo request
            let icmpmsg = ICMPMessage()
            icmpmsg.Seqno <- i
            icmpmsg.Data <- [|byte 0xCA; byte 0xFE; byte 0xBA; byte 0xBE|]
            icmpmsg.Chksum <- icmpmsg.CalculateChecksum()
            prn (sprintf "\nICMP request %d to be sent to LIPS server:" i)
            icmpmsg.Print()
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
            prn "IP to be sent:"
            ippacket.Print()
            // Issue GET to server
            let iptxt = Array.concat [ippacket.Header;ippacket.Data] |>
                        Array.map (fun b -> byte2str(int b)) 
            //let blaaurl = "http://pingrt.ngrok.io/" + String.Join("", iptxt)
            let blaaurl = "http://dbaa8026.ngrok.io/" + String.Join("", iptxt)
            //let blaaurl = "http://localhost:8085/" + String.Join("", iptxt)
            prn "Done sending request"
            let blaahund = fetchUrl myCallbacker blaaurl
            i <- i + 1
            System.Threading.Thread.Sleep 5000

        let e = IPPacket.ICMPPROTOCOL
        System.Console.ReadKey() |> ignore
        0 // return an integer exit code
