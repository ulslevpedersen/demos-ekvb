// Copyright: 2017-, Copenhagen Business School
// Author: Rasmus Ulslev Pedersen (rp.digi@cbs.dk)
// License: Simplified BSD License
// LIPS: Server

// Server "1.2.3.5"
// localhost:8085

namespace lips
module MainModule = 

    open System
    open System.Net
    open System.IO
    open System.Text
    open LIPSLIB.Util
    open LIPSLIB.BH
    open LIPSLIB.IP
    open LIPSLIB.HTTPUTIL

    // ngrok http -subdomain=pingrt -host-header=localhost 8080
    let host = "http://localhost:8085/"

    let startlisterner =
       listener ((fun req resp ->
            async {
                //let txt = Encoding.ASCII.GetBytes(output req)
                let mutable dosend = false
                let iptxtin = req.Url.LocalPath.ToString().[1..]
                prn (sprintf "\nLIPS SERVER received: %s" iptxtin)
                let isBlaaHund = iptxtin.Length > 0 && 
                                 BHPacket.VerifyBHStr(iptxtin)
                let mutable iptxtout = ""
                if isBlaaHund then
                    let reply = ipdecode(iptxtin)
                    if reply.IsSome then
                        let ipreply = reply.Value 
                        iptxtout <- bytearray2str(Array.concat [ipreply.Header;ipreply.Data])
                        dosend <- true

                // TODO: Need to decode iptextin and see if it a valid IP packet
                // and a valid ICMP message
                // If it is that then I need to send a valid echo back to the
                // sender
                let datetxt = System.DateTime.Now.ToString();
                let txt = Encoding.ASCII.GetBytes(
                              "(Info from LIPS server: \"" + datetxt + "\")\n"            +
                              "(             Received: \"" + iptxtin + "\")\n"            +
                              "(            BlaaHund?: \"" + string(isBlaaHund) + "\")\n" +
                              "(       Server replied: \"" + iptxtout + "\")\n")
                resp.ContentType <- "text/plain"
                resp.OutputStream.Write(txt, 0, txt.Length)
                resp.OutputStream.Close()

                if dosend then
                    //let blaaurlA = "http://pingrt.ngrok.io/" + String.Join("", iptxtout)
                    //let blaahundA = fetchUrl myCallbacker blaaurlA
                    //let blaaurlB = "http://dbaa8026.ngrok.io/" + String.Join("", iptxtout)
                    //let blaahundB = fetchUrl myCallbacker blaaurlB
                    prn (sprintf "\nIP reply to client:     \"%s\" " iptxtout)
                    let blaaurlC = "http://localhost:8084/" + String.Join("", iptxtout)
                    let blaahundC = fetchUrl myCallbacker blaaurlC
                    prn "" //TODO                    

            }), host)
        
    [<EntryPoint>]
    let main argv = 
        // Listen for http requests
        startlisterner
        System.Console.ReadKey() |> ignore
        0 // return an integer exit code
