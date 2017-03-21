// Copyright: 2015-2017, Copenhagen Business School
// Author: Rasmus Ulslev Pedersen (rup.itm@cbs.dk)

// License: Simplified BSD License
//
// Cilcc: CIL codes to runtime structures

namespace Cilcc

module MainModule = 
    open System
    open System.IO
    //open System.Collections.Generic
    
    // Redirecting the output
    // 'prn (sprintf "%d" 123)'
    let prn (str : string) = 
            System.Console.WriteLine(str)
            System.Diagnostics.Debug.WriteLine(str)

    // Redirecting the output
    // 'pr (sprintf "%d" 123)'
    let pr (str : string) = 
            System.Console.Write(str)
            System.Diagnostics.Debug.Write(str)
    
    // CONSTANTS
    // Comes from base of code (0x2000) and file ???
    let rvaOffset = -0x2000 + 0x200
    // GLOBALS
    let mutable stringHeapBytes : byte [] = Array.zeroCreate 0
    // Blob
    // Use: 
    //     blobLookup.Add (0x06, **)
    //     let blobEntryRec = snd (blobLookup.TryGetValue(0x06))
    let blobLookup = System.Collections.Generic.Dictionary<int, BlobEntryRec>()

    // Create a table record type
    let getReader fileName = new BinaryReader(File.Open(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
    // Positions the reader
    let positionReader (indx : int, reader : BinaryReader) = 
        reader.BaseStream.Seek(int64 (indx), SeekOrigin.Begin) |> ignore
    
    let readUInt16Value (indx : int, reader : BinaryReader) = 
        try 
            reader.BaseStream.Seek(int64 (indx), SeekOrigin.Begin) |> ignore
            Some(reader.ReadUInt16())
        with :? System.IO.EndOfStreamException -> 
            prn (sprintf "End of stream!")
            None
    
    let readUInt32Value (indx : int, reader : BinaryReader) = 
        try 
            reader.BaseStream.Seek(int64 (indx), SeekOrigin.Begin) |> ignore
            Some(reader.ReadUInt32())
        with :? System.IO.EndOfStreamException -> 
            prn (sprintf "End of stream!")
            None
    
    let readStringValue (indx : int, len : int, reader : BinaryReader) = 
        try 
            reader.BaseStream.Seek(int64 (indx), SeekOrigin.Begin) |> ignore
            let rec loop acc = 
                let readValue = reader.ReadChar()
                match readValue with
                | System.Char.MinValue -> List.rev acc
                | _ -> loop (readValue :: acc)
            
            let res = System.String.Concat(loop [])
            Some(res)
        with :? System.IO.EndOfStreamException -> 
            prn (sprintf "End of stream!")
            None
    
    // returns a tuple with the given stream header's 
    // offset, size, and name
    let readStreamHeader (metaDataRootStart : int, indx : int, reader : BinaryReader) = 
        reader.BaseStream.Seek(int64 (indx), SeekOrigin.Begin) |> ignore
        let streamHeaderOffset = int (reader.ReadUInt32())
        reader.BaseStream.Seek(int64 (indx + 4), SeekOrigin.Begin) |> ignore
        let streamHeaderSize = int (reader.ReadUInt32())
        reader.BaseStream.Seek(int64 (indx + 8), SeekOrigin.Begin) |> ignore
        let rec loop acc = 
            let readValue = reader.ReadChar()
            match readValue with
            | System.Char.MinValue -> List.rev acc
            | _ -> loop (readValue :: acc)
        
        let streamHeaderName = System.String.Concat(loop [])
        { StreamHeaderStart = metaDataRootStart + streamHeaderOffset
          StreamHeaderOffset = streamHeaderOffset
          StreamHeaderSize = streamHeaderSize
          StreamHeaderName = streamHeaderName 
          StreamBytes = [||]}
    
    // including the 4-byte offset and 4-byte size
    let streamHeaderNameTotalSize streamHeader = 
        let streamHeaderNameLengthWithNull = streamHeader.StreamHeaderName.Length + 1
        if (streamHeaderNameLengthWithNull % 4) = 0 then streamHeaderNameLengthWithNull + 8
        else (4 - streamHeaderNameLengthWithNull % 4) + streamHeaderNameLengthWithNull + 8
    
    let mutable (stringList : string list) = List.empty
    
    // reads one string out of the string heap byte array
    // using the global string heap byte array
    let readString (indx : int) = 
        let mutable keepLooking = true
        let mutable i = indx
        while keepLooking do
            if stringHeapBytes.[i] = 0uy then keepLooking <- false
            else i <- i + 1
        let byteStr = stringHeapBytes.[indx..i - 1]
        let str = System.Text.Encoding.ASCII.GetString(byteStr)
        str
    
    let readStringStream (metaDataRootStart : int, stringStreamRec : StreamHeader, reader : BinaryReader) = 
        prn (sprintf "\nreadStringStream")
        let mutable (xs : string []) = Array.zeroCreate 0
        let mutable indx = metaDataRootStart + stringStreamRec.StreamHeaderOffset
        // first string is null
        xs <- Array.append xs [| "" |]
        indx <- indx + 1
        let mutable keepLooking = true
        while keepLooking do
            let nextStringOption = readStringValue (indx, stringStreamRec.StreamHeaderSize, reader)
            if nextStringOption.IsSome then 
                if nextStringOption.Value.Length > 0 then 
                    prn (sprintf "[0x%08x]string: \"%s\"" indx nextStringOption.Value)
                    prn (sprintf "---length:%d" nextStringOption.Value.Length)
                    xs <- Array.append xs [| nextStringOption.Value |]
                    indx <- indx + nextStringOption.Value.Length
                    indx <- indx + 1 // for the null terminator
                    // still inside the string stream?
                    if indx - (metaDataRootStart + stringStreamRec.StreamHeaderOffset) > stringStreamRec.StreamHeaderSize then 
                        keepLooking <- false
                else 
                    // stop on empty string (the end)
                    keepLooking <- false
            else 
                prn (sprintf "[0x%08x]no string!" indx)
                keepLooking <- false
        List.ofArray xs
    
    //let readStringSH ( indx : int, reader : BinaryReader)
    let decodeModuleRows (count : int, reader : BinaryReader) = 
        // The rows index the String heap using a byte offset and then the 
        // string understood as \0 terminated
        for i in 0..count - 1 do
            prn (sprintf "\nModuleRow[%d]" i)
            let generationVal = reader.ReadUInt16()
            prn (sprintf "generationVal:0x%04x" generationVal)
            let nameSH = int (reader.ReadUInt16())
            prn (sprintf "nameSH:0x%04x" nameSH)
            let nameStr = readString (nameSH)
            prn (sprintf "nameStr:%s" nameStr)
            let mvidGH = reader.ReadUInt16()
            prn (sprintf "mvidGH:0x%04x" mvidGH)
            let encIdGH = reader.ReadUInt16()
            prn (sprintf "encIdGH:%04x" encIdGH)
            let encBaseIdGH = reader.ReadUInt16()
            prn (sprintf "encBaseIdGH:0x%04x" encBaseIdGH)
        0
    
    let decodeTypeRefRows (count : int, reader : BinaryReader) = 
        for i in 1..count do
            prn (sprintf "\nTypeRefRow[%d]" i)
            let resolutionScopeIndex = reader.ReadUInt16()
            prn (sprintf "resolutionScopeIndex:0x%04x" resolutionScopeIndex)
            let typeNameSH = reader.ReadUInt16()
            prn (sprintf "typeNameSH:0x%04x" typeNameSH)
            let typeNameStr = readString (int (typeNameSH))
            prn (sprintf "typeNameStr:%s" typeNameStr)
            let typeNamespaceSH = reader.ReadUInt16()
            prn (sprintf "typeNamespaceSH:0x%04x" typeNamespaceSH)
            let typeNamespaceStr = readString (int (typeNamespaceSH))
            prn (sprintf "typeNamespaceStr:%s" typeNamespaceStr)
        0
    
    // returns a list of TypeDefTavbleRec records
    let decodeTypeDefRows (count : int, reader : BinaryReader) = 
        let typeDefTable = ResizeArray<TypeDefTableRec>()
        for i in 1..count do
            prn (sprintf "\nTypeDefRow[%d]" count)
            let typeAttributesFlag = reader.ReadUInt32()
            prn (sprintf "typeAttributesFlag:0x%08x" typeAttributesFlag)
            let typeNameSH = reader.ReadUInt16()
            prn (sprintf "typeNameSH:0x%04x" typeNameSH)
            let typeNameStr = readString (int (typeNameSH))
            prn (sprintf "typeNameStr:%s" typeNameStr)
            let typeNamespaceSH = reader.ReadUInt16()
            prn (sprintf "typeNamespaceSH:0x%04x" typeNamespaceSH)
            let typeNamespaceStr = readString (int (typeNamespaceSH))
            prn (sprintf "typeNamespaceStr:%s" typeNamespaceStr)
            let extendsCodedIndex = reader.ReadUInt16()
            prn (sprintf "extendsCodedIndex:0x%04x" extendsCodedIndex)
            let fieldListIndex = reader.ReadUInt16()
            prn (sprintf "fieldListIndex:0x%04x" fieldListIndex)
            let methodListIndex = reader.ReadUInt16()
            prn (sprintf "methodListIndex:0x%04x" methodListIndex)
            let typeDefTableRec = 
                { _index = i
                  Flags = typeAttributesFlag
                  TypeName = typeNameSH
                  TypeNameStr = typeNameStr
                  TypeNamespace = typeNamespaceSH
                  TypeNamespaceStr = typeNamespaceStr
                  Extends = extendsCodedIndex
                  FieldList = fieldListIndex
                  MethodList = methodListIndex }
            typeDefTable.Add(typeDefTableRec)
        typeDefTable.ToArray()
    
    let decodeMethodDefRows (count : int, totParams : int, typeDefTable : TypeDefTableRec [], blob : Blob, reader : BinaryReader) = 
        let methodDefRows = new ResizeArray<MethodDefRowType>()
        for i in 1..count do
            prn (sprintf "\nMethodDefRow[%d]" i)
            let rva = int (reader.ReadUInt32()) + rvaOffset // points to the COR_ILMETHOD
            prn (sprintf "ad. rva:0x%08x" rva)
            let implFlags = reader.ReadUInt16()
            prn (sprintf "implFlags:0x%04x" implFlags)
            let flags = reader.ReadUInt16()
            prn (sprintf "flags:0x%04x" flags)
            let nameSH = reader.ReadUInt16()
            prn (sprintf "nameSH:0x%04x" nameSH)
            let nameStr = readString (int (nameSH))
            prn (sprintf "nameStr:%s" nameStr)
            let signatureBH = reader.ReadUInt16()
            prn (sprintf "signatureBH:0x%04x" signatureBH)
            let aBlobRec = blob.GetEntryByOffset(int(signatureBH))
            Blob.PrintBlobEntryRec(aBlobRec)
            // An index into the Param Table. It continues until the 'ParamList' index of the next 'MethodDef'.
            // The Param 0x08 table has three columns:
            //  'Flags' a 2-byte bitmask of 'ParamAttributes'
            //  'Sequence'a 2-byte constant
            //  'Name' an index into the String Heap
            let paramListIndex = reader.ReadUInt16()
            prn (sprintf "paramListIndex:0x%04x" paramListIndex)
            let methodDefRow =
                { _index = i
                  Rva = int (rva)
                  ImplFlags = implFlags
                  Flags = flags
                  NameStr = nameStr
                  SignatureBH = signatureBH
                  BlobRec = aBlobRec
                  ParamListIndex = paramListIndex
                  NumArgs = 0
                  Locals = None
                  TypeDefTableRecOwner = getTypeDefTableRecOwner (i, typeDefTable)
                  // Updated when the cil instructions are read
                  Instructions = Array.zeroCreate 0 }
            methodDefRows.Add(methodDefRow)
            for i in 0 .. methodDefRows.Count-1 do
                // If there is one more methodDef that would define how many params this method has.
                if i < methodDefRows.Count-1 then
                    let paramCnt = methodDefRows.[i+1].ParamListIndex - methodDefRows.[i].ParamListIndex
                    methodDefRows.[i].NumArgs <- int(paramCnt) 
                else
                    // +1 because it is inclusive the last item
                    let paramCnt = totParams - int(methodDefRows.[i].ParamListIndex) + 1
                    methodDefRows.[i].NumArgs <- paramCnt
        methodDefRows.ToArray()

    // The Param table 0x08, has the following columns:
    //     Flags : 2 bytes
    //     Sequence : a 2 byte constant
    //     Name : an index into the String heap
    let decodeParamTableRows(count : int, reader : BinaryReader) =
        prn (sprintf "\ndecodeParamTableRows, count=%d" count)
        let paramTableRows = new ResizeArray<ParamTableRow>()
        for i in 0 .. count-1 do
            prn (sprintf "i=%d:" i)
            let flags = reader.ReadUInt16()
            prn (sprintf "    flags=0x%02X" flags)
            let sequence = reader.ReadUInt16()
            prn (sprintf "   sequence=0x%02X" sequence)
            let name = reader.ReadUInt16()
            prn (sprintf "    name=0x%02X" name)
            let nameStr = readString (int (name))
            prn (sprintf "nameStr=%s" nameStr)
            let paramTableRow = {
                    Flags = flags
                    Sequence = sequence
                    Name = name
                    NameStr = nameStr }
            paramTableRows.Add(paramTableRow)        
        paramTableRows.ToArray()

    // Decode StandAloneSig table rows. 0x11 and .[14]
    //type StandAloneSigRow =
    //        { // Signature is an index into the Blob heap
    //          Signature : uint16 }
    // For each .method with the .locals directive, there exists a row in this 
    // table. Ann example signature is (see II.23.2.6 LocalVarSig)
    // 20->Blob:[|7uy; 2uy; 8uy; 6uy|]
    // LOCAL_SIG: 7
    // Count (compressed): 2
    // See II.23.1.16 Element types used in signatures 
    // Element[0x08]: "ELEMENT_TYPE_I4" (Value:0x8)
    // Element[0x06]: "ELEMENT_TYPE_I2" (Value:0x6)
    let decodeStandAloneSigRows(count : int, blob : Blob, reader : BinaryReader) =
        let standAloneSigRows = new ResizeArray<StandAloneSigRow>()
        prn (sprintf "\ndecodeStandAloneSigRows, count=%d" count)
        for i in 1..count do
            let signature = reader.ReadUInt16()
            //prn (sprintf "    i=%d, signature=0x%04X" i signature)
            let blobEntry = blob.GetEntryByOffset(int(signature))
            let localVarSigRec = (blobEntry.Signature :?> LocalVarSigRec)
            let standAloneSigRow =
                    { // Signature is an index into the Blog heap 
                      Signature = signature 
                      LocalVarSig = localVarSigRec }
            prn (sprintf "    i=%d, standAloneSigRow=%A" i standAloneSigRow)
            standAloneSigRows.Add(standAloneSigRow)
        standAloneSigRows.ToArray()
    
    // The strings referred to by these cil instructions are indexes into the 
    // string heap (which is preprocessed to be available as the list 'xs:string list' 
    let findCIL (methodDefRows : MethodDefRowType [], standAloneSigRows : StandAloneSigRow [], reader : BinaryReader) = 
        for i in 0..methodDefRows.Length - 1 do
            prn ""
            prn (sprintf "methodDef[#0x%x]:%A" (i + 1) methodDefRows.[i])
            // Prepare the cil array
            let cilArray = ResizeArray<CilRec>()
            reader.BaseStream.Seek(int64 (methodDefRows.[i].Rva), SeekOrigin.Begin) |> ignore
            let firstByte = reader.ReadByte()
            prn (sprintf "firstByte=0x%02X" firstByte)
            // Tiny header (first 2 bits are 0x2):  I.25.4.2 
            if firstByte &&& 0x3uy = 0x2uy then 
                let size = int (firstByte >>> 2)
                let mutable pos = 0 // byte position after header 
                prn (sprintf "tiny header code size:%d bytes" size)
                while pos < size do
                    let cilInstOpcode = int (reader.ReadByte())
                    prn (sprintf "pos:0x%x, cilInst:0x%02x" pos cilInstOpcode)
                    let cilInst = Instructions.[cilInstOpcode]
                    prn (sprintf "cil:%A" cilInst)
                    let mutable operandParams = 0UL
                    if cilInst.OperandParamsSize = 1 then 
                        operandParams <- uint64 (reader.ReadByte())
                        prn (sprintf "operandParams:0x%02x" operandParams)
                    else if cilInst.OperandParamsSize = 2 then 
                        operandParams <- uint64 (reader.ReadUInt16())
                        prn (sprintf "operandParams:0x%04x" operandParams)
                    else if cilInst.OperandParamsSize = 4 then 
                        operandParams <- uint64 (reader.ReadUInt32())
                        prn (sprintf "operandParams:0x%08x" operandParams)
                    else 
                        if cilInst.OperandParamsSize = 8 then 
                            operandParams <- reader.ReadUInt64()
                            prn (sprintf "operandParams:0x%016x" operandParams)
                    let cilRec = 
                        { Pos = pos
                          Instruction = cilInst
                          Params = operandParams }
                    cilArray.Add(cilRec)
                    pos <- pos + 1 // the instruction
                    pos <- pos + cilInst.OperandParamsSize
            elif firstByte &&& 0x3uy = 0x3uy then
                // fat method header
                let secondByte = reader.ReadByte()
                prn ( sprintf "secondByte=0x%02X" secondByte)
                let firstTwoBytes = (int(secondByte)<<<8) ||| int(firstByte)
                prn ( sprintf "firstTwoBytes=0x%04X" firstTwoBytes)
                let maxStack = int(reader.ReadUInt16())
                let codeSizeBytes = int(reader.ReadUInt32())
                let localVarSigTok = int(reader.ReadUInt32())
                let size = codeSizeBytes
                let mutable pos = 0 // byte position after header 
                prn (sprintf "fat header code size:%d bytes" size)
                while pos < size do
                    let cilInstOpcode = int (reader.ReadByte())
                    prn (sprintf "pos:0x%x, cilInst:0x%02x" pos cilInstOpcode)
                    let cilInst = Instructions.[cilInstOpcode]
                    prn (sprintf "cil:%A" cilInst)
                    let mutable operandParams = 0UL
                    if cilInst.OperandParamsSize = 1 then 
                        operandParams <- uint64 (reader.ReadByte())
                        prn (sprintf "operandParams:0x%02x" operandParams)
                    else if cilInst.OperandParamsSize = 2 then 
                        operandParams <- uint64 (reader.ReadUInt16())
                        prn (sprintf "operandParams:0x%04x" operandParams)
                    else if cilInst.OperandParamsSize = 4 then 
                        operandParams <- uint64 (reader.ReadUInt32())
                        prn (sprintf "operandParams:0x%08x" operandParams)
                    else 
                        if cilInst.OperandParamsSize = 8 then 
                            operandParams <- reader.ReadUInt64()
                            prn (sprintf "operandParams:0x%016x" operandParams)
                    let cilRec = 
                        { Pos = pos
                          Instruction = cilInst
                          Params = operandParams }
                    cilArray.Add(cilRec)
                    pos <- pos + 1 // the instruction
                    pos <- pos + cilInst.OperandParamsSize
                // The localVarSigTok is 0x11000001, which is a "pointer"
                // to the StandAloneSigTable 0x11, 1st entry
                // The StandAloneSigTable has only one column, which points to
                // a Signature in the Blob heap 
                // TODO: Remember that the 'Param'table (0x08) needs to be read before 
                // the one row in the StandAloneSigTable can be read
                prn (sprintf "localVarSigTok:0x%08X" localVarSigTok)
                if localVarSigTok > 0 then
                    let localIndex = 0x0000FFFF &&& localVarSigTok
                    let locals = standAloneSigRows.[localIndex-1].LocalVarSig
                    methodDefRows.[i].Locals <- Some(locals)
                    ()
                ()
            methodDefRows.[i].Instructions <- cilArray.ToArray()
        0

    let writeToMem (typeDefTable : TypeDefTableRec [], methodDefRows : MethodDefRowType []) =
        writeToVerilog(typeDefTable, methodDefRows) 

    let readTablesStream (metaDataRootStart : int, tableStreamRec : StreamHeader, blob : Blob, reader : BinaryReader) = 
        reader.BaseStream.Seek(int64 (metaDataRootStart + tableStreamRec.StreamHeaderOffset), SeekOrigin.Begin) |> ignore
        let reservedVal = reader.ReadUInt32()
        prn (sprintf "reservedVal=0x%08x" reservedVal)
        let majorVersionVal = reader.ReadByte()
        prn (sprintf "majorVersionVal=0x%02x" majorVersionVal)
        let minorVersionVal = reader.ReadByte()
        prn (sprintf "minorVersionVal=0x%02x" minorVersionVal)
        let heapSizesVal = reader.ReadByte()
        prn (sprintf "heapSizesVal=0x%02x" heapSizesVal)
        let reservedVal = reader.ReadByte()
        prn (sprintf "reservedVal=0x%02x" reservedVal)
        prn ""
        prn ( "--Decoding Tables--")
        let validVal = reader.ReadUInt64()
        prn (sprintf "validVal=0x%016x" validVal)
        let sortedVal = reader.ReadUInt64()
        prn (sprintf "sortedVal=0x%016x" sortedVal)
        // Save the table row count using the table id as key
        let rowCounts = new ResizeArray<int>()
        for i in 0..tableListOrdered.Length - 1 do
            let validBit = 
                if (validVal &&& (1UL <<< tableListOrdered.[i].Number)) > 0UL then 1
                else 0
            
            let sortedBit = 
                if (sortedVal &&& (1UL <<< tableListOrdered.[i].Number)) > 0UL then 1
                else 0
            
            // One 4-byte row count for each valid table
            if validBit = 1 then 
                rowCounts.Add(int (reader.ReadUInt32()))
            else
                rowCounts.Add(0)
            
            prn (sprintf "{valid:%d}{rows:%d}{sorted:%d}[%2d]%s:0x%02x" validBit rowCounts.[i] sortedBit i tableListOrdered.[i].Name 
                     tableListOrdered.[i].Number)
        // Module table 0x00 on .[0]
        decodeModuleRows (rowCounts.[0], reader) |> ignore
        // TypeRef table 0x01 on .[1]
        decodeTypeRefRows (rowCounts.[1], reader) |> ignore
        // TypeDef table 0x02 .[2]
        let typeDefTable = decodeTypeDefRows (rowCounts.[2], reader)
        // MethodDef table 0x06 on .[4]
        // Send in the number of rows in the Param table as well
        let methodDefRows = decodeMethodDefRows (rowCounts.[4], rowCounts.[5], typeDefTable, blob, reader)
        // Param table 0x08 on .[5]
        let paramTableRows = decodeParamTableRows(rowCounts.[5], reader)
        // StandAloneSigTable 0x11, .[14]
        let standAloneSigRows = decodeStandAloneSigRows(rowCounts.[14], blob, reader)
        // Finds CIL, and (via the FAT header, CorILMethod_FatFormat, II.25.4.3 Fat format
        // Finds the localVarSigTok, which via the StandAloneSig.Signature points to the LocalVarSig
        // in the Blob.
        findCIL (methodDefRows, standAloneSigRows, reader) |> ignore
        // Now we can emit mem.sv file because we have the class(es) with the methods with the CIL code
        writeToMem (typeDefTable, methodDefRows)
        0
    
    let readGuidStream (metaDataRootStart : int, guidStreamRec : StreamHeader, reader : BinaryReader) = 
        // The “#GUID” header points to a sequence of 128-bit GUIDs
        prn (sprintf "readGuidStream")
        prn (sprintf "length:%d" guidStreamRec.StreamHeaderSize)
        prn (sprintf "num. of 128 bit (16 byte) GUIDs: %d" (guidStreamRec.StreamHeaderSize / 16))
        if guidStreamRec.StreamHeaderSize > 0 then 
            let mutable indx = metaDataRootStart + guidStreamRec.StreamHeaderOffset
            reader.BaseStream.Seek(int64 (indx), SeekOrigin.Begin) |> ignore
            let guidBytes = reader.ReadBytes(guidStreamRec.StreamHeaderSize)
            prn (sprintf "first guid:%A" guidBytes)
        else prn (sprintf "no guids")
        0
    
    let readUsStream (metaDataRootStart : int, usStreamRec : StreamHeader, reader : BinaryReader) = 
        prn (sprintf "\nreadUsStream")
        // If the first one byte of the 'blob' is 0 bbbbbbb2, then the rest of the 'blob' contains the 
        //     bbbbbbb2 bytes of actual data. 
        // If the first two bytes of the 'blob' are 10 bbbbbb2 and x, then the rest of the 'blob' 
        //     contains the (bbbbbb2 << 8 + x) bytes of actual data. 
        // If the first four bytes of the 'blob' are 110 bbbbb2, x, y, and z, then the rest of the 
        //     'blob' contains the (bbbbb2 << 24 + x << 16 + y << 8 + z) bytes of actual data. 
        // The first entry in both these heaps is the empty 'blob' that consists of the single byte 0x00.  
        let mutable indx = metaDataRootStart + usStreamRec.StreamHeaderOffset
        reader.BaseStream.Seek(int64 (indx), SeekOrigin.Begin) |> ignore
        let firstByte = reader.ReadByte()
        prn (sprintf "firstByte=0=0x%02x" firstByte)
        prn (sprintf "%s" (sprintf "firstByte=0=0x%02x" firstByte))
        // if first byte defines the US
        let secondByte = reader.ReadByte()
        prn (sprintf "secondByte=0x%02x" secondByte)
        if secondByte &&& 0x80uy = 0uy then 
            let usLen = int (secondByte)
            prn (sprintf "%d bytes will follow" usLen)
            let rawBytes = reader.ReadBytes(usLen)
            prn (sprintf "%d bytes: %A" usLen rawBytes)
        let thirdByte = reader.ReadByte()
        prn (sprintf "thirdByte=0x%02x" thirdByte)
        let forthByte = reader.ReadByte()
        prn (sprintf "forthByte=0x%02x" forthByte)
        // first string is null
        //us <- Array.append xs [|""|]
        0

    // BLOB DECODING -----------------------------------------------------------
    // MethodRefSig (differs from a MethodDefSig only for VARARG calls) 
    // MethodDefSig 
    // FieldSig 
    // PropertySig 
    // LocalVarSig 
    // TypeSpec 
    // MethodSpec 

    // First byte:
    // One of: 
    //      *C, 
    //      *DEFAULT        =   0x0, 
    //      *FASTCALL, 
    //      *STDCALL, 
    //      *THISCALL, or 
    //      *VARARG=0x5

    // MethodDefSig
    //     First byte:
    //         HASTHIS = 0x20 | EXPLICITTHIS = 0x40
    //         DEFAULT = 0x0 or VARARG = 0x5 or GENERIC 0x10
    //     Paramcount; number of parameters: Compressed unsigned integer

    // This metod will read the Blob heap. It creates an array of "raw" byte arrays. Each of those individual 
    // signatures is encoded according to II.23.2 "Blobs and signatures".
    // It will create a BlobStream class, which has methods for accessing the individual blobs and their types.   
    // TODO: Will the blob be accessed using a byte-offset or a logical index. If it is a byte-based offset, then 
    //     ...
    let readBlobStream (metaDataRootStart : int, blobStreamRec : StreamHeader, reader : BinaryReader) = 
        let blob = Blob()
        prn (sprintf "readBlobStream@0x%x" (metaDataRootStart + blobStreamRec.StreamHeaderOffset))
        let blobArray : byte [][] = Array.zeroCreate 0
        let mutable indx = metaDataRootStart + blobStreamRec.StreamHeaderOffset
        reader.BaseStream.Seek(int64 (indx), SeekOrigin.Begin) |> ignore
        // The entire blob
        blobStreamRec.StreamBytes <- reader.ReadBytes(blobStreamRec.StreamHeaderSize)
        blob.RawBlobBytes <- blobStreamRec.StreamBytes 
        prn (sprintf "Raw blob bytes:\n%A" blobStreamRec.StreamBytes)
        // II.24.2.4
        //  If the first one byte of the 'blob' is 0bbbbbbb2, then the rest of the 'blob' contains the 
        //  bbbbbbb2 bytes of actual data.
        let sb = blobStreamRec.StreamBytes
        let mutable blobs : byte [][] = Array.zeroCreate 0
        if sb.[0] <> 0x00uy then
            prn ( "Error: First byte is not zero")
        else
            blobs <- Array.append blobs [|[|0x00uy|]|]
        // Decode blobs
        let mutable i = 1
        let mutable len = -1
        while i < sb.Length do
            let offset = i
            // Is it one byte compression?
            if sb.[i] &&& 0x80uy = 0x00uy then
                // Get the len
                len <- int(sb.[i] &&& 0x7Fuy)
                i <- i + 1
            // Is it two byte compression?
            else if sb.[i] &&& 0xC0uy = 0x80uy then
                len <- int((sb.[i] &&& 0x3Fuy)<<<8) + int(sb.[i+1])
                i <- i + 2
            // Is it four byte compression?
            else if sb.[i] &&& 0xE0uy = 0xC0uy then
                len <- int((sb.[i] &&& 0x1Fuy)<<<24) + int(sb.[i+1]<<<16) + int(sb.[i+2]<<<8) + int(sb.[i+3])
                i <- i + 4
            // now read the len bytes blob
            let aBlob = sb.[i..i+len-1]
            blob.AddRawBlobEntry(offset, aBlob) |> ignore
            blobs <- Array.append blobs [|aBlob|]
            i <- i + len
        blob

    [<EntryPoint>]
    let main argv = 
        let reader = getReader "basic.exe"
        // find index of magic value 
        let mutable indx = 0
        let mutable aVal = readUInt32Value (indx, reader)
        let mutable continueLoop = aVal.IsSome
        while continueLoop = true do
            if aVal.Value = uint32 (0x424A5342) then 
                prn "::Metadata root"
                prn "::  -Signature, version info, stream header count, and array of each present stream header."
                prn "::  -nActual encoded tables and heaps immediately follow the stream headers."
                prn (sprintf "aVal[indx=x%x]:x%08x!!!" indx (aVal.Value))
                continueLoop <- false
            else 
                //prn (sprintf "aVal[indx=x%x]:x%08x" indx (aVal.Value)
                indx <- indx + 1
                aVal <- readUInt32Value (indx, reader)
        // Index of the magic signature 0x424A5342
        let metaDataRootStart = indx
        // majorVersion
        let majorVersionIndex = metaDataRootStart + 4
        let majorVersionValue = readUInt16Value (majorVersionIndex, reader)
        prn (sprintf "majorVersion[majorVsersionIndex=x%x]:x%04x" majorVersionIndex majorVersionValue.Value)
        // minorVersion
        let minorVersionIndex = metaDataRootStart + 6
        let minorVersionValue = readUInt16Value (minorVersionIndex, reader)
        prn (sprintf "minorVersion[minorVersionIndex=x%x]:x%04x" minorVersionIndex minorVersionValue.Value)
        // Reserved
        let reservedIndex = metaDataRootStart + 8
        let reservedValue = readUInt32Value (reservedIndex, reader)
        prn (sprintf "reservedValue[reservedIndex=x%x]:x%04x" reservedIndex reservedValue.Value)
        // Length
        let lengthIndex = metaDataRootStart + 12
        let lengthValue = readUInt32Value (lengthIndex, reader)
        prn (sprintf "lengthValue[lengthIndex=x%x]:x%04x" lengthIndex lengthValue.Value)
        let m = 
            if lengthValue.Value % 4ul = 0ul then int (lengthValue.Value)
            else int (4ul - lengthValue.Value % 4ul + lengthValue.Value)
        prn (sprintf "m:x%04x" m)
        // Version string
        let versionIndex = int (metaDataRootStart + 16)
        prn (sprintf "versionIndex:0x%x" versionIndex)
        let versionString = readStringValue (versionIndex, m, reader)
        let versionString2 = versionString.Value
        prn (sprintf "versionString2=%s" versionString2)
        // No padding
        // Flags
        let flagsIndex = versionIndex + 12
        let flagsValue = (readUInt16Value(flagsIndex, reader)).Value
        prn (sprintf "flagsValue[flagsIndex:0x%x]:0x%04x" flagsIndex flagsValue)
        // Streams
        let streamsIndex = flagsIndex + 2
        let streamsValue = (readUInt16Value(streamsIndex, reader)).Value
        prn (sprintf "streamsValue[flagsIndex:0x%x]:0x%04x" streamsIndex streamsValue)
        let streamsHeadersOffset = streamsIndex + 2
        prn ("")
        // #~: (tables)
        // Stream 0
        let tablesStreamStart = streamsHeadersOffset
        let tablesStreamRec = readStreamHeader (metaDataRootStart, tablesStreamStart, reader)
        let tablesStreamHeaderBytes = streamHeaderNameTotalSize tablesStreamRec
        prn 
            (sprintf "Tables stream: StreamHeaderOffset=0x%08x, StreamHeaderSize:0x%08x, StreamHeaderName:%s" 
                 tablesStreamRec.StreamHeaderOffset tablesStreamRec.StreamHeaderSize tablesStreamRec.StreamHeaderName)
        prn (sprintf "tablesStreamHeaderBytes=%d" tablesStreamHeaderBytes)
        prn ("")
        // #Strings
        // Stream 1
        let stringStreamStart = tablesStreamStart + tablesStreamHeaderBytes
        prn (sprintf "stringStreamStart=0x%08x" stringStreamStart)
        let stringStreamRec = readStreamHeader (metaDataRootStart, stringStreamStart, reader)
        let stringStreamHeaderBytes = streamHeaderNameTotalSize stringStreamRec
        prn 
            (sprintf "Strings stream: StreamHeaderOffset=0x%08x, StreamHeaderSize:0x%08x, StreamHeaderName:%s" 
                 stringStreamRec.StreamHeaderOffset stringStreamRec.StreamHeaderSize stringStreamRec.StreamHeaderName)
        prn (sprintf "stringStreamHeaderBytes=%d" stringStreamHeaderBytes)
        prn ("")
        positionReader (metaDataRootStart + stringStreamRec.StreamHeaderOffset, reader)
        stringHeapBytes <- reader.ReadBytes(stringStreamRec.StreamHeaderSize)
        stringList <- readStringStream (metaDataRootStart, stringStreamRec, reader)
        prn (sprintf "stringList:%A" stringList)
        prn (sprintf "stringList[2]:%s" stringList.[2])
        prn ("")
        // #US
        // Stream 2
        let usStreamStart = stringStreamStart + stringStreamHeaderBytes
        prn (sprintf "usStreamStart=0x%08x" usStreamStart)
        let usStreamRec = readStreamHeader (metaDataRootStart, usStreamStart, reader)
        let usStreamHeaderBytes = streamHeaderNameTotalSize usStreamRec
        prn 
            (sprintf "US stream: StreamHeaderOffset=0x%08x, StreamHeaderSize:0x%08x, StreamHeaderName:%s" 
                 usStreamRec.StreamHeaderOffset usStreamRec.StreamHeaderSize usStreamRec.StreamHeaderName)
        prn (sprintf "usStreamHeaderBytes=%d" usStreamHeaderBytes)
        prn ("")
        // #GUID
        // Stream 3
        let guidStreamStart = usStreamStart + usStreamHeaderBytes
        prn (sprintf "guidStreamStart=0x%08x" guidStreamStart)
        let guidStreamRec = readStreamHeader (metaDataRootStart, guidStreamStart, reader)
        let guidStreamHeaderBytes = streamHeaderNameTotalSize guidStreamRec
        prn 
            (sprintf "guidStream: StreamHeaderOffset=0x%08x, StreamHeaderSize:0x%08x, StreamHeaderName:%s" 
                 guidStreamRec.StreamHeaderOffset guidStreamRec.StreamHeaderSize guidStreamRec.StreamHeaderName)
        prn (sprintf "guidStreamHeaderBytes=%d" guidStreamHeaderBytes)
        prn ("")
        // #Blob
        // Stream 4
        let blobStreamStart = guidStreamStart + guidStreamHeaderBytes
        prn (sprintf "blobStreamStart=0x%08x" blobStreamStart)
        let blobStreamRec = readStreamHeader (metaDataRootStart, blobStreamStart, reader)
        let blobStreamHeaderBytes = streamHeaderNameTotalSize blobStreamRec
        prn 
            (sprintf "blobStream: StreamHeaderOffset=0x%08x, StreamHeaderSize:0x%08x, StreamHeaderName:%s" 
                 blobStreamRec.StreamHeaderOffset blobStreamRec.StreamHeaderSize blobStreamRec.StreamHeaderName)
        prn (sprintf "blobStreamHeaderBytes=%d" blobStreamHeaderBytes)
        prn ""
        // Get the blobs first?
        // Analyze Tables
        let blob = readBlobStream (metaDataRootStart, blobStreamRec, reader)
        for i = 0 to blob.Count()-1 do
            prn( sprintf("Blob entry [%d]:") i)
            Blob.PrintBlobEntryRec(blob.GetEntryByIndex(i))            
        readTablesStream (metaDataRootStart, tablesStreamRec, blob, reader) |> ignore
        readUsStream (metaDataRootStart, usStreamRec, reader) |> ignore
        readGuidStream (metaDataRootStart, guidStreamRec, reader) |> ignore
        
        //System.Console.ReadKey() |> ignore
        0
