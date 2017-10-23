// Copyright: 2017-, Copenhagen Business School
// Author: Rasmus Ulslev Pedersen (rp.digi@cbs.dk)
// License: Simplified BSD License
// LIPS: HTTP Client/Server

namespace LIPSLIB
[<AutoOpen>]
module HTTPUTIL =
    open System
    open System.Net
    open System.IO
    open System.Text

    // Queue for sending "real" responses
    let txQueue = new Queue(2)
        
    // Fetch the contents of a web page
    let fetchUrlAsync url = 
        async {
            try  
                let req = HttpWebRequest.Create(Uri(url))
                let httpreq = req :?> HttpWebRequest
                httpreq.KeepAlive <- false
                use! resp = req.AsyncGetResponse() 
                use stream = resp.GetResponseStream() 
                use reader = new IO.StreamReader(stream) 
                let content = reader.ReadToEnd()//callback (Some reader) url
                printfn "::HTTP RESPONSE START::\n%s\n::HTTP RESPONSE END::" content //url
            with 
            | :? System.Net.WebException as ex -> 
                printfn "Web error, no answer from: %s \n%s" url ex.Message
            }

    // Then listen 
    let listener (handler:(HttpListenerRequest->HttpListenerResponse->Async<unit>), 
                      localaddr:string, remoteaddr:string) =
        let hl = new HttpListener()
        hl.Prefixes.Add localaddr
        hl.Start()
        let task = Async.FromBeginEnd(hl.BeginGetContext, hl.EndGetContext)
        async {
            while true do
                let! context = task
                Async.Start(handler context.Request context.Response)
        } |> Async.Start
 
    // Decoding
    let icmpreply(icmprequest:ICMPMessage) =
        let icmpreplymsg = ICMPMessage()
        icmpreplymsg.Header <- Array.copy icmprequest.Header
        icmpreplymsg.Data <- Array.copy icmprequest.Data
        icmpreplymsg.Icmptype <- ICMPMessage.ICMPECHOREPLY
        icmpreplymsg.Chksum <- icmpreplymsg.CalculateChecksum()
        icmpreplymsg

    let icmpdecode(bytes:byte[]) : ICMPMessage option=
        let icmpmsg = ICMPMessage()
        if bytes.Length >= 8 then
            icmpmsg.Header <- bytes.[0..7]
            if bytes.Length > 8 then
                icmpmsg.Data <- bytes.[8..]
            // Check ICMP checksum
            let _icmpchecksum = icmpmsg.CalculateChecksum()
            if (icmpmsg.Chksum <> _icmpchecksum) then
                prn (sprintf "Wrong Chksum in ICMP message: 0x%X. Expected 0x%X" icmpmsg.Chksum _icmpchecksum)
            Some icmpmsg
        else
            None
        // ICMP echo request?
        //if (icmpmsg.Icmptype = ICMPMessage.ICMPECHOREQUEST) then
        //    prn "Received ICMP echo request:"
        //    icmpmsg.Print()
        //    Some icmpmsg
        // else 
        //    None

    let icmpprint(bytes:byte[]) : bool=
        let mutable res = false
        let icmpmsg = ICMPMessage()
        icmpmsg.Header <- bytes.[0..7]
        icmpmsg.Data <- bytes.[8..]
        // Check ICMP checksum
        if (icmpmsg.Chksum <> icmpmsg.CalculateChecksum()) then
            prn (sprintf "Wrong Chksum in ICMP message: 0x%X. Expected 0x%X" icmpmsg.Chksum (icmpmsg.CalculateChecksum()))
        else
            res <- true
            icmpmsg.Print()
        true


    // Return Option IPPacket
    let ipdecode(iptxt:string) :IPPacket option = 
        let mutable res = None //IPPacket
        let byta = str2bytearray(iptxt)
        let newip = IPPacket()
        if byta.Length >= 20 then
            newip.Header <- byta.[0..19]
            if byta.Length > 20 then
                newip.Data <- byta.[20..]
            // Check Tlen
            if (newip.Tlen <> newip.Header.Length + newip.Data.Length) then
                prn (sprintf "Wrong Tlen in ip packet: %d. Expected %d" 
                                    newip.Tlen (newip.Header.Length+newip.Data.Length))
            // Check Hchksum
            let checkchecksum = newip.CalculateChecksum()
            if (newip.Hchksum <> checkchecksum) then
                prn (sprintf "Wrong Hchksum in ip packet: 0x%X. Expected 0x%X" newip.Hchksum checkchecksum)
            res <- Some newip
        res

    // Return Option IPPacket
    let ipprint(iptxt:string) :bool = 
        let mutable res = false 
        let byta = str2bytearray(iptxt)
        let newip = IPPacket()    
        newip.Header <- byta.[0..19]
        newip.Data <- byta.[20..]
        // Check Tlen
        if (newip.Tlen <> newip.Header.Length + newip.Data.Length) then
            prn (sprintf "Wrong Tlen in IP packet: %d. Expected %d" 
                                newip.Tlen (newip.Header.Length+newip.Data.Length))
        else 
            // Check Hchksum
            let checkchecksum = newip.CalculateChecksum()
            if (newip.Hchksum <> checkchecksum) then
                prn (sprintf "Wrong Hchksum in IP packet: 0x%X. Expected 0x%X" newip.Hchksum checkchecksum)
            else
                res <- true
                newip.Print()
        // ICMP ?
        if newip.Prot = IPPacket.ICMPPROTOCOL then
            icmpprint(newip.Data) |> ignore
        res

    let startlisterner (localaddr:string, remoteaddr:string) =
       listener ((fun req resp ->
            async {
                //let txt = Encoding.ASCII.GetBytes(output req)
                let mutable dosend = false
                let iptxtin = req.Url.LocalPath.ToString().[1..]
                prn (sprintf "\nLIPS received: %s" iptxtin)
                //prn (sprintf "From: %s" (req.RemoteEndPoint.ToString()))
                let isBlaaHund = iptxtin.Length > 0 && 
                                 BHPacket.VerifyBHStr(iptxtin)
                let mutable isIP = false
                let mutable isICMP = false
                let mutable isICMPEchoReply = false
                let mutable isICMPEchoRequest = false
                let mutable isUDP = false
                let mutable iptxtout = ""
                if isBlaaHund then
                    let ipin = ipdecode(iptxtin)
                    if ipin.IsSome then
                        isIP <- true
                        ipin.Value.Print()
                        if ipin.Value.IsICMP() then
                            isICMP <- true
                            let icmpin = icmpdecode(ipin.Value.Data)
                            if icmpin.IsSome then
                                icmpin.Value.Print()
                                if icmpin.Value.IsICMPEchoRequest() then
                                    isICMPEchoRequest <- true
                                    let icmpReply = icmpin.Value.CreateICMPEchoReply(icmpin.Value)
                                    let ipReply = ipin.Value
                                    ipReply.Data <- Array.concat [icmpReply.Header; icmpReply.Data]
                                    ipReply.SwitchSrcDstIP()
                                    ipReply.Hchksum <- ipReply.CalculateChecksum()
                                    iptxtout <- bytearray2str (Array.concat [ipReply.Header;ipReply.Data])
                                    dosend <- true
                                else if icmpin.Value.IsICMPEchoReply() then
                                    isICMPEchoReply <- true
                        elif ipin.Value.IsUDP() then
                            isUDP <- true
                            let udp = UDPDatagram()
                            udp.Header <- ipin.Value.Data.[0..7]
                            udp.Data   <- ipin.Value.Data.[8..]
                            udp.Print()
                let datetxt = System.DateTime.Now.ToString();
                let txt =     "(      Informal 'debug' info on the latest packet:\n"             +
                              "(      Remote LIPS info: \"" + datetxt + "\"\n"                   +
                              "(              Received: \"" + iptxtin + "\"\n"                   +
                              "(           Is BlaaHund: \"" + string(isBlaaHund) + "\"\n"        +
                              "(                 Is IP: \"" + string(isIP) + "\"\n"              +
                              "(                Is UDP: \"" + string(isUDP) + "\"\n"             +
                              "(               Is ICMP: \"" + string(isICMP) + "\"\n"            +   
                              "(  Is ICMP echo request: \"" + string(isICMPEchoRequest) + "\"\n" +
                              "(    Is ICMP echo reply: \"" + string(isICMPEchoReply) + "\"\n"   +
                              "( Reply (if applicable): \"" + iptxtout + "\""
                resp.ContentType <- "text/plain"
                let txtBytes = Encoding.ASCII.GetBytes(txt)
                resp.OutputStream.Write(txtBytes, 0, txtBytes.Length)
                resp.OutputStream.Close()
                resp.Close();
                prn (sprintf "Sent this informal 'debug' info back:\n%s" txt)

                if dosend then
                    prn ""
                    if txQueue.Put(remoteaddr + iptxtout) then 
                        prn (sprintf "LIPS tx queued: %s" (remoteaddr + iptxtout))
                    else
                        prn (sprintf "LIPS not queued: %s" (remoteaddr + iptxtout))
                    
                    //let blaahund = Async.RunSynchronously (fetchUrl (remoteaddr + iptxtout))
                    ()   
            }), localaddr, remoteaddr)

    // TX SENDER

    // Takes items fromt the txqueue every PX milliseconds
    let startFetcher() =
        async {
            let px = 1000 //ms
            while true do
                let aurl = txQueue.Take()  
                if aurl.IsSome then
                    prn (sprintf "Took packet \"%s\" from tx queue" aurl.Value)
                    do! fetchUrlAsync(aurl.Value) 
                else
                    prn "Noting in tx queue"
                do! Async.Sleep px
        }