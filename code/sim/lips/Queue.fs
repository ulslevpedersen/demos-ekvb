// Copyright: 2017-, Copenhagen Business School
// Author: Rasmus Ulslev Pedersen (rp.digi@cbs.dk)
// License: Simplified BSD License
// LIPS: Queue

namespace LIPSLIB

[<AutoOpen>]
module QueueModule = 
    type Queue(cap:int) as __ =
        let capacity = cap
        let mutable free = cap
        let mutable enqptr = 0
        let mutable deqptr = 0
        let data : byte[][] = [| for a in 0 .. cap - 1  do yield [||] |]

        // Read if not empty
        member __.Dequeue() : byte[] option=
            let mutable res = None
            if free = cap then // not empty?
                res <- Some (Array.copy data.[deqptr])
                deqptr <- (deqptr + 1) % cap
                free <- free + 1
            res
            
        // Write if not full
        member __.Enqueue(msg : byte[]) : bool = 
            if free > 0 then
                data.[enqptr] <- Array.copy msg
                enqptr <- (enqptr + 1) % cap
                free <- free - 1
                true
            else
                false