// Copyright: 2017-, Copenhagen Business School
// Author: Rasmus Ulslev Pedersen (rp.digi@cbs.dk)

// License: Simplified BSD License
//
// LIPS: Utilities to work with network data

namespace lips

[<AutoOpen>]
module Util =
    let hex2ascii hx =
      match hx with
      | _ when hx <= 0x9 -> char (hx + 48)
      | _ when hx >= 0xA && hx <= 0xF -> char (hx + 55)
      | _ -> failwith "unexpected input"

    let byte2ascstr by =
      let highby = by >>> 4
      let lowby = by &&& 0x0F
      string (hex2ascii highby) + string (hex2ascii lowby)