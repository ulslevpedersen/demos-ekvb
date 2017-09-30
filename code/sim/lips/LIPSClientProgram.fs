// Copyright: 2017-, Copenhagen Business School
// Author: Rasmus Ulslev Pedersen (rp.digi@cbs.dk)
// License: Simplified BSD License
// LIPS: Server

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

    // ngrok http -subdomain=pingrt -host-header=localhost 8080
    let siteRoot = @"C:\Users\ulslevpedersen\Desktop\www"
    let host = "http://localhost:8080/"
 
    // GET
    let myCallbacker (reader:IO.StreamReader option) url = 
        if reader.IsSome then 
            let html = reader.Value.ReadToEnd()
            printfn "Downloaded %s. Html is %s" url html
            html
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
          printfn "Web error, no answer from: %s (%s)" url ex.Message
          callback None url

  // return all the html
  
    // Then listen 
    let listener (handler:(HttpListenerRequest->HttpListenerResponse->Async<unit>)) =
        let hl = new HttpListener()
        hl.Prefixes.Add host
        hl.Start()
        let task = Async.FromBeginEnd(hl.BeginGetContext, hl.EndGetContext)
        async {
            while true do
                let! context = task
                Async.Start(handler context.Request context.Response)
        } |> Async.Start
 
    let output (req:HttpListenerRequest) =
        let file = Path.Combine(siteRoot,
                                Uri(host).MakeRelativeUri(req.Url).OriginalString)
        printfn "Requested : '%s'" file
        if (File.Exists file)
            then File.ReadAllText(file)
            else "File does not exist!"

    // Decoding
    let icmpreply(icmprequest:ICMPMessage) =
        let icmpreplymsg = ICMPMessage()
        icmpreplymsg.Header <- Array.copy icmprequest.Header
        icmpreplymsg.Data <- Array.copy icmprequest.Data
        icmpreplymsg.Icmptype <- ICMPMessage.ICMPECHOREPLY
        icmpreplymsg.Chksum <- icmpreplymsg.CalculateChecksum(icmpreplymsg.Header,
                                                               icmpreplymsg.Data)
        icmpreplymsg

    let icmpdecode(bytes:byte[]) =
        let icmpmsg = ICMPMessage()
        icmpmsg.Header <- bytes.[0..7]
        icmpmsg.Data <- bytes.[8..]
        // Check ICMP checksum
        let _icmpchecksum = icmpmsg.CalculateChecksum(icmpmsg.Header, icmpmsg.Data)
        if (icmpmsg.Chksum <> _icmpchecksum) then
            prn (sprintf "Wrong Chksum in ICMP message: 0x%X. Expected 0x%X" icmpmsg.Chksum _icmpchecksum)
        // ICMP echo request?
        if (icmpmsg.Icmptype = ICMPMessage.ICMPECHOREQUEST) then
            prn "Received ICMP echo request:"
            icmpmsg.Print()
            let icmpreplymsg = icmpreply(icmpmsg)
            Some icmpreplymsg
        else 
            None
    // Return Option IPPacket
    let ipdecode(iptxt:string)= 
        let mutable res = None
        let byta = str2bytearray(iptxt)
        let newip = IPPacket()    
        newip.Header <- byta.[0..19]
        newip.Data <- byta.[20..]
        // Check Tlen
        if (newip.Tlen <> newip.Header.Length + newip.Data.Length) then
            prn (sprintf "Wrong Tlen in ip packet: %d. Expected %d" 
                               newip.Tlen (newip.Header.Length+newip.Data.Length))
        // Check Hchksum
        let checkchecksum = newip.CalculateChecksum(newip.Header)
        if (newip.Hchksum <> checkchecksum) then
            prn (sprintf "Wrong Hchksum in ip packet: 0x%X. Expected 0x%X" newip.Hchksum checkchecksum)
        // ICMP ?
        if newip.Prot = IPPacket.ICMPPROTOCOL then
            let icmp = icmpdecode(newip.Data)
            // Now issue ICMP reply?
            if icmp.IsSome then
                let icmpreply = icmp.Value
                let newsrcip = newip.DstIP
                let newdstip = newip.SrcIP
                newip.DstIP <- newsrcip
                newip.SrcIP <- newdstip
                newip.Data <- Array.concat [icmpreply.Header; icmpreply.Data]
                newip.Hchksum <- newip.CalculateChecksum(newip.Header)
                res <- Some newip
        res    
    [<EntryPoint>]
    let main argv = 

        // Listen for responses
        listener (fun req resp ->
            async {
                //let txt = Encoding.ASCII.GetBytes(output req)
                let iptxtin = req.Url.LocalPath.ToString().[1..]
                let isBlaaHund = BHPacket.VerifyBHStr(iptxtin)
                let reply = ipdecode(iptxtin)
                
                let mutable iptxtout = ""
                if reply.IsSome then
                    let ipreply = reply.Value 
                    iptxtout <- bytearray2str(Array.concat [ipreply.Header;ipreply.Data])
                    let blaaurlA = "http://pingrt.ngrok.io/" + String.Join("", iptxtout)
                    //let blaaurlB = "http://dbaa8026.ngrok.io/" + String.Join("", iptxtout)
                    let blaahundC = fetchUrl myCallbacker blaaurlA
                    //let blaahundD = fetchUrl myCallback blaaurlB
                    prn (sprintf "This IP reply was sent:%s " iptxtout)

                // TODO: Need to decode iptextin and see if it a valid IP packet
                // and a valid ICMP message
                // If it is that then I need to send a valid echo back to the
                // sender
                let datetxt = System.DateTime.Now.ToString();
                let txt = Encoding.ASCII.GetBytes(
                              "<html><head></head><body><h2>" +
                              "Hello Ping-RT in the Real-Time IoT World!</h2>" +
                              "Your package is (" + string(isBlaaHund) +
                              ") a Blaa Hund packet: " + 
                              "<tt>" + iptxtin + "</tt>" + 
                              "<p>Reply (if any):<tt>" + iptxtout + "</tt>" +
                              "<p>"+ "Served by LIPS "+ 
                              datetxt + "</body></html>")
                resp.ContentType <- "text/html"
                resp.OutputStream.Write(txt, 0, txt.Length)
                resp.OutputStream.Close()
            })

        let mutable i = 0
        let mutable blaaurl1 = ""
        let mutable blaaurl2 = ""
        while true do
            // ICMP echo request
            let icmpmsg = ICMPMessage()
            icmpmsg.Data <- [|byte 0xCA; byte 0xFE; byte 0xBA; byte 0xBE; byte 0x00; byte i|]
            icmpmsg.Chksum <- icmpmsg.CalculateChecksum(icmpmsg.Header, icmpmsg.Data)
            prn "ICMP:->"
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
            ippacket.Data <- Array.concat [icmpmsg.Header; icmpmsg.Data]
            prn "IP:->"
            ippacket.Print()
            // Issue GET to ms
            let iptxt = Array.concat [ippacket.Header;ippacket.Data] |>
                        Array.map (fun b -> byte2str(int b)) 
            //blaaurl1 <- "http://pingrt.ngrok.io/" + String.Join("", iptxt)
            blaaurl2 <- "http://dbaa8026.ngrok.io/" + String.Join("", iptxt)
            //let blaahund1 = fetchUrl myCallbacker blaaurl1
            let blaahund2 = fetchUrl myCallbacker blaaurl2
            prn (sprintf "Sendt packet no. %d" i)
            i <- i + 1
            System.Threading.Thread.Sleep 5000

        let e = IPPacket.ICMPPROTOCOL
        System.Console.ReadKey() |> ignore
        0 // return an integer exit code
