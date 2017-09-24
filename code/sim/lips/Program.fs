// Copyright: 2017-, Copenhagen Business School
// Author: Rasmus Ulslev Pedersen (rp.digi@cbs.dk)

// License: Simplified BSD License
//
// LIPS: Little IP Stack testing program

namespace lips

module MainModule = 

  open System
  open System.Net
  open System.Text
  open System.IO
 
  let siteRoot = @"C:\Users\ulslevpedersen\Desktop\www"
  let host = "http://localhost:8080/"
 
  // GET
  // Fetch the contents of a web page
  let fetchUrl callback url =        
    let req = WebRequest.Create(Uri(url)) 
    use resp = req.GetResponse() 
    use stream = resp.GetResponseStream() 
    use reader = new IO.StreamReader(stream) 
    callback reader url

  let myCallback (reader:IO.StreamReader) url = 
    let html = reader.ReadToEnd()
    printfn "Downloaded %s. First 1000 is %s" url html
    html      // return all the html
  
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
 
  [<EntryPoint>]
  let main argv = 
      // GET
      let ippacket = IPPacket()
      //ippacket.Print()
      let icmpmsg = ICMPMessage()
      icmpmsg.Data <- [|byte 0xCA; byte 0xFE; byte 0xBA; byte 0xBE|]

      ippacket.Data <- Array.concat [icmpmsg.Header; icmpmsg.Data]

      let iptxt = Array.concat [ippacket.Header;ippacket.Data] |>
                  Array.map (fun b -> byte2ascstr(int b)) 

      let mutable blaaurl = "http://dbaa8026.ngrok.io/" 
                            + String.Join("", iptxt)

      let blaahund = fetchUrl myCallback blaaurl

      listener (fun req resp ->
          async {
              //let txt = Encoding.ASCII.GetBytes(output req)
              let datetxt = System.DateTime.Now.ToString();
              let txt = Encoding.ASCII.
                          GetBytes("<html><head></head><body><h2>" +
                          "Hello Ping-RT in the Real-Time IoT World!</h2>" +
                          "Your package is (perhaps) a Blaa Hund packet: " + 
                          "<tt>" + req.Url.LocalPath.ToString().[1..] + "</tt><p>"+     "Served by LIPS "+ datetxt +  
                          "</body></html>")
              resp.ContentType <- "text/html"
              resp.OutputStream.Write(txt, 0, txt.Length)
              resp.OutputStream.Close()
          })
      System.Console.ReadKey() |> ignore
      0