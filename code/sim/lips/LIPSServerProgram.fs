// Copyright: 2017-, Copenhagen Business School
// Author: Rasmus Ulslev Pedersen (rp.digi@cbs.dk)
// License: Simplified BSD License
// LIPS: Server

namespace lips
module MainModule = 

    open System
    open LIPSLIB.HTTPUTIL

    // LOCAL SERVER //
    // ngrok http -subdomain=pingrt1 -host-header=localhost 8081
    let localaddr = "http://localhost:8081/"

    // REMOTE (LIPSClient when testing on localhost) //
    let remoteaddr = "http://pingrt.ngrok.io/" //->localhost:8080
    //let remoteaddr = "http://87cbaa5a.ngrok.io/"
        
    [<EntryPoint>]
    let main argv = 
        // Listen for http requests
        Async.Start (startFetcher())
        startlisterner (localaddr, remoteaddr)
        System.Console.ReadKey() |> ignore
        0
