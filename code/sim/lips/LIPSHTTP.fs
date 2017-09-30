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

    // GET
    let myCallbacker (reader:IO.StreamReader option) url = 
        if reader.IsSome then 
            let content = reader.Value.ReadToEnd()
            printfn "%s" content //url
            content
        else
            ""
        
    // Fetch the contents of a web page
    let fetchUrl callback url = 
        try  
            let req = WebRequest.Create(Uri(url)) 
            use resp = req.GetResponse() 
            use stream = resp.GetResponseStream() 
            use reader = new IO.StreamReader(stream) 
            callback (Some reader) url
        with 
        | :? System.Net.WebException as ex -> 
            printfn "Web error, no answer from: %s \n%s" url ex.Message
            callback None url

    // Then listen 
    let listener (handler:(HttpListenerRequest->HttpListenerResponse->Async<unit>),host:string) =
        let hl = new HttpListener()
        hl.Prefixes.Add host
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
        icmpmsg.Header <- bytes.[0..7]
        icmpmsg.Data <- bytes.[8..]
        // Check ICMP checksum
        let _icmpchecksum = icmpmsg.CalculateChecksum()
        if (icmpmsg.Chksum <> _icmpchecksum) then
            prn (sprintf "Wrong Chksum in ICMP message: 0x%X. Expected 0x%X" icmpmsg.Chksum _icmpchecksum)
        // ICMP echo request?
        if (icmpmsg.Icmptype = ICMPMessage.ICMPECHOREQUEST) then
            prn "Received ICMP echo request:"
            icmpmsg.Print()
            Some icmpmsg
         else 
            None

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
        newip.Header <- byta.[0..19]
        newip.Data <- byta.[20..]
        // Check Tlen
        if (newip.Tlen <> newip.Header.Length + newip.Data.Length) then
            prn (sprintf "Wrong Tlen in ip packet: %d. Expected %d" 
                                newip.Tlen (newip.Header.Length+newip.Data.Length))
        // Check Hchksum
        let checkchecksum = newip.CalculateChecksum()
        if (newip.Hchksum <> checkchecksum) then
            prn (sprintf "Wrong Hchksum in ip packet: 0x%X. Expected 0x%X" newip.Hchksum checkchecksum)
        // ICMP ?
        if newip.Prot = IPPacket.ICMPPROTOCOL then
            let icmp = icmpdecode(newip.Data)
            // Now issue ICMP reply to ICMP request?
            if icmp.IsSome && icmp.Value.Icmptype = ICMPMessage.ICMPECHOREQUEST then
                let icmpreply = icmp.Value
                icmpreply.Icmptype <- ICMPMessage.ICMPECHOREPLY
                icmpreply.Chksum <- icmpreply.CalculateChecksum()
                let newsrcip = newip.DstIP
                let newdstip = newip.SrcIP
                newip.DstIP   <- newdstip
                newip.SrcIP   <- newsrcip
                newip.Data    <- Array.concat [icmpreply.Header; icmpreply.Data]
                newip.Hchksum <- newip.CalculateChecksum()
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