// Copyright: 2017-, Copenhagen Business School
// Author: Rasmus Ulslev Pedersen (rp.digi@cbs.dk)
// License: Simplified BSD License
// LIPS: Server

// Server "1.2.3.5"
// localhost:8085

namespace lips
module MainModule = 

    open System
    open LIPSLIB.HTTPUTIL

    // LOCAL 
    // ngrok http -subdomain=pingrt -host-header=localhost 8080
    let localaddr = "http://localhost:8085/"

    // REMOTE 
    //let blaaurl = "http://pingrt.ngrok.io/"
    //let blaaurlB = "http://dbaa8026.ngrok.io/"
    let remoteaddr = "http://localhost:8084/"

        
    [<EntryPoint>]
    let main argv = 
        // Listen for http requests
        startlisterner (localaddr, remoteaddr)
        System.Console.ReadKey() |> ignore
        0 // return an integer exit code
