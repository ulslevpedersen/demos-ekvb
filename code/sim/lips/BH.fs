// Copyright: 2017-, Copenhagen Business School
// Author: Rasmus Ulslev Pedersen (rp.digi@cbs.dk)
// License: Simplified BSD License
// LIPS: Blaa Hund packet

namespace lips

[<AutoOpen>]
module BH =
    open System

    type BHPacket() as __ =
        let mutable data : byte[] = Array.zeroCreate 0
        member __.Data with get() = data and
                            set(value) = data <- value
        member __.Print() =
            printf "BHPacket.Data.Length:%d " __.Data.Length
            __.Data |> Array.iteri (fun i l -> printf "Data[%02d]=0x%02X" i l)

        // Verifying that it contains only hex digits
        static member VerifyBHStr(newdata:string) :bool =
            let mutable res = true
            newdata.ToCharArray() |> Array.iteri (fun i c -> if not <| Uri.IsHexDigit(c) then 
                                                                 res <- false)
            res