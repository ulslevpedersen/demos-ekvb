// Copyright: 2017-, Copenhagen Business School
// Author: Rasmus Ulslev Pedersen (rp.digi@cbs.dk)
// License: Simplified BSD License
// LIPS: Server

namespace lips
module MainModule = 

    open System
    open LIPSLIB.HTTPUTIL

    // LOCAL (:8080 also when it is server in LIPSServer)
    // ngrok http -subdomain=pingrt -host-header=localhost 8080
    let localaddr = "http://localhost:8080/"

    // REMOTE (LIPSClient when testing on localhost)
    let remoteaddr = "http://localhost:8084/"
    //let remoteaddr = "http://pingrt.ngrok.io/"
    //let remoteaddr = "http://87cbaa5a.ngrok.io/"
    
        
    [<EntryPoint>]
    let main argv = 
        // Listen for http requests
        Async.Start (startFetcher())
        startlisterner (localaddr, remoteaddr)
        System.Console.ReadKey() |> ignore
        0 // return an integer exit code
