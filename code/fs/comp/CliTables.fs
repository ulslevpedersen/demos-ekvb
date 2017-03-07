// Copyright: 2015-17, Copenhagen Business School
// Author: Rasmus Ulslev Pedersen (rup.itm@cbs.dk)
// License: Simplified BSD License
//
// Cilcc: CIL codes to runtime structures
namespace Cilcc

open System.Collections.Generic

[<AutoOpen>]
module CilTables = 

    // Redirecting the output
    // 'prn (sprintf "%d" 123)'
    let prn (str : string) = 
            System.Console.WriteLine(str)
            System.Diagnostics.Debug.WriteLine(str)

    // TABLES
    let AssemblyTable = 0x20
    // *HashAlgId (a 4-byte constant of type AssemblyHashAlgorithm, § II.23.1.1) 
    // *MajorVersion, MinorVersion, BuildNumber, RevisionNumber (each being 2-byte constants) 
    // *Flags (a 4-byte bitmask of type AssemblyFlags, § II.23.1.2) 
    // *PublicKey (an index into the Blob heap) 
    // *Name (an index into the String heap)
    let AssemblyOSTable = 0x22
    // *OSPlatformID (a 4-byte constant) 
    // *OSMajorVersion (a 4-byte constant) 
    // *OSMinorVersion (a 4-byte constant) 
    let AssemblyProcessorTable = 0x21
    // *Processor (a 4-byte constant)
    let AssemblyRefTable = 0x23
    // *MajorVersion, *MinorVersion, *BuildNumber, *RevisionNumber (each being 2-byte constants) 
    // *Flags (a 4-byte bitmask of type AssemblyFlags, § II.23.1.2) 
    // *PublicKeyOrToken (an index into the Blob heap, indicating the public key or token 
    //     that identifies the author of this Assembly) 
    // *Name (an index into the String heap) 
    // *Culture (an index into the String heap) 
    // *HashValue (an index into the Blob heap)         
    let AssemblyRefOSTable = 0x25
    // *OSPlatformId (a 4-byte constant) 
    // *OSMajorVersion (a 4-byte constant) 
    // *OSMinorVersion (a 4-byte constant) 
    // *AssemblyRef (an index into the AssemblyRef table)      
    let AssemblyRefProcessorTable = 0x24
    // *Processor (a 4-byte constant) 
    // *AssemblyRef (an index into the AssemblyRef table) 
    let ClassLayoutTable = 0x0F
    // *PackingSize (a 2-byte constant) 
    // *ClassSize (a 4-byte constant) 
    // *Parent (an index into the TypeDef table) 
    let ConstantTable = 0x0B
    // *Type (a 1-byte constant, followed by a 1 -byte padding zero); see §II.23.1.16 . The 
    //      encoding of Type for the nullref value for FieldInit in ilasm (§II.16.2) is 
    //      ELEMENT_TYPE_CLASS with a Value of a 4-byte zero. Unlike uses of 
    //      ELEMENT_TYPE_CLASS in signatures, this one is not followed by a type token. 
    // *Parent (an index into the Param, Field, or Property table; more precisely, a 
    // *HasConstant (§II.24.2.6) coded index) 
    // *Value (an index into the Blob heap) 
    let CustomAttributeTable = 0x0C
    // *Parent (an index into a metadata table that has an associated HasCustomAttribute 
    //      (§II.24.2.6) coded index). 
    // *Type (an index into the MethodDef or MemberRef table; more precisely, a 
    //      CustomAttributeType (§II.24.2.6) coded index). 
    // *Value (an index into the Blob heap). 
    let DeclSecurityTable = 0x0E
    // *Action (a 2-byte value) 
    // *Parent (an index into the TypeDef, MethodDef, or Assembly table; more precisely, a 
    // *HasDeclSecurity (§II.24.2.6) coded index) 
    // *PermissionSet (an index into the Blob heap) 
    let EventMapTable = 0x12
    // *Parent (an index into the TypeDef table) 
    // *EventList (an index into the Event table). It marks the first of a contiguous run of 
    //      Events owned by this Type. That run continues to the smaller of: 
    //          o the last row of the Event table 
    //          o the next run of Events, found by inspecting the EventList of the next row 
    //              in the EventMap table 
    let EventTable = 0x14
    // *EventFlags (a 2-byte bitmask of type EventAttributes, §II.23.1.4) 
    // *Name (an index into the String heap) 
    // *EventType (an index into a TypeDef, a TypeRef, or TypeSpec table; more precisely, a 
    //      TypeDefOrRef (§II.24.2.6) coded index) (This corresponds to the Type of the 
    //      Event; it is not the Type that owns this event.) 
    let ExportedTypeTable = 0x27
    // *Flags (a 4-byte bitmask of type TypeAttributes, §II.23.1.15) 
    // *TypeDefId (a 4-byte index into a TypeDef table of another module in this Assembly). 
    //      This column is used as a hint only. If the entry in the target TypeDef table matches 
    //      the TypeName and TypeNamespace entries in this table, resolution has succeeded. 
    //      But if there is a mismatch, the CLI shall fall back to a search of the target TypeDef 
    //      table. Ignored and should be zero if Flags has IsTypeForwarder set. 
    // *TypeName (an index into the String heap) 
    // *TypeNamespace (an index into the String heap) 
    // *Implementation. This is an index (more precisely, an Implementation (§II.24.2.6) 
    //  coded index) into either of the following table s: 
    //      o File table, where that entry says which module in the current assembly 
    //          holds the TypeDef 
    //      o ExportedType table, where that entry is the enclosing Type of the current 
    //          nested Type 
    //      o AssemblyRef table, where that entry says in which assembly the type may 
    //          now be found (Flags must have the IsTypeForwarder flag set). 
    let FieldTable = 0x04
    // *Flags (a 2-byte bitmask of type FieldAttributes, §II.23.1.5) 
    // *Name (an index into the String heap) 
    // *Signature (an index into the Blob heap) 
    let FieldLayoutTable = 0x10
    // *Offset (a 4-byte constant) 
    // *Field (an index into the Field table) 
    let FieldMarshalTable = 0x0D
    // *Parent (an index into Field or Param table; more precisely, a HasFieldMarshal 
    //      (§II.24.2.6) coded index) 
    // *NativeType (an index into the Blob heap) 
    let FieldRVATable = 0x1D
    // *RVA (a 4-byte constant) 
    // *Field (an index into Field table)
    let FileTable = 0x026
    // *Flags (a 4-byte bitmask of type FileAttributes, § II.23.1.6) 
    // *Name (an index into the String heap) 
    // *HashValue (an index into the Blob heap) 
    let GenericParam = 0x2A
    // *Number (the 2-byte index of the generic parameter, numbered left -to-right, from zero) 
    // *Flags (a 2-byte bitmask of type GenericParamAttributes, §II.23.1.7) 
    // *Owner (an index into the TypeDef or MethodDef table, specifying the Type or 
    // *Method to which this generic parameter applies; more precisely, a 
    // *TypeOrMethodDef (§II.24.2.6) coded index) 
    // *Name (a non-null index into the String heap, giving the name for the generic 
    //      parameter. This is purely descriptive and is used only by source language compilers 
    //      and by Reflection) 
    let GenericParamConstraintTable = 0x2C
    // *Owner (an index into the GenericParam table, specifying to which generic 
    //      parameter this row refers) 
    // *Constraint (an index into the TypeDef, TypeRef, or TypeSpec tables, specifying from 
    //      which class this generic parameter is constrained to de rive; or which interface this 
    //      generic parameter is constrained to implement; more precisely, a TypeDefOrRef 
    //      (§II.24.2.6) coded index) 
    let ImplMapTable = 0x1C
    // *MappingFlags (a 2-byte bitmask of type PInvokeAttributes, §23.1.8) 
    // *MemberForwarded (an index into the Field or MethodDef table; more precisely, a 
    //      MemberForwarded (§II.24.2.6) coded index). However, it only ever indexes the 
    //      MethodDef table, since Field export is not supp orted. 
    // *ImportName (an index into the String heap) 
    // *ImportScope (an index into the ModuleRef table) 
    let InterfaceImplTable = 0x09
    // *Class (an index into the TypeDef table) 
    // *Interface (an index into the TypeDef, TypeRef, or TypeSpec table; more precisely, a 
    //      TypeDefOrRef (§II.24.2.6) coded index) 
    let ManifestResourceTable = 0x28
    // *Offset (a 4-byte constant) 
    // *Flags (a 4-byte bitmask of type ManifestResourceAttributes, §II.23.1.9) 
    // *Name (an index into the String heap) 
    // *Implementation (an index into a File table, a AssemblyRef table, or null; more 
    //      precisely, an Implementation (§II.24.2.6) coded index) 
    let MemberRefTable = 0x0A
    // *Class (an index into the MethodDef, ModuleRef,TypeDef, TypeRef, or TypeSpec 
    //      tables; more precisely, a MemberRefParent (§II.24.2.6) coded index) 
    // *Name (an index into the String heap) 
    // *Signature (an index into the Blob heap) 
    let MethodDefTable = 0x06
    // *RVA (a 4-byte constant) 
    // *ImplFlags (a 2-byte bitmask of type MethodImplAttributes, §II.23.1.10) 
    // *Flags (a 2-byte bitmask of type MethodAttributes, §II.23.1.10) 
    // *Name (an index into the String heap) 
    // *Signature (an index into the Blob heap) 
    // *ParamList (an index into the Param table). It marks the first of a contiguous run of 
    //      Parameters owned by this method. The run continues to the smaller of: 
    //          o the last row of the Param table 
    //          o the next run of Parameters, found by inspecting the ParamList of the next 
    //              row in the MethodDef table 
    let MethodImplTable = 0x019
    // *Class (an index into the TypeDef table) 
    // *MethodBody (an index into the MethodDef or MemberRef table; more precisely, a 
    //      MethodDefOrRef (§II.24.2.6) coded index) 
    // *MethodDeclaration (an index into the MethodDef or MemberRef table; more 
    //      precisely, a MethodDefOrRef (§II.24.2.6) coded index) 
    let MethodSemanticsTable = 0x18
    // *Semantics (a 2-byte bitmask of type MethodSemanticsAttributes, §II.23.1.12) 
    // *Method (an index into the MethodDef table) 
    // *Association (an index into the Event or Property table; more precisely, a 
    //      HasSemantics (§II.24.2.6) coded index) 
    let MethodSpecTable = 0x2B
    // *Method (an index into the MethodDef or MemberRef table, specifying to which 
    //      generic method this row refers; that is, which generic method this row is an 
    //      instantiation of; more precisely, a MethodDefOrRef (§II.24.2.6) coded index) 
    // *Instantiation (an index into the Blob heap (§II.23.2.15), holding the signature of 
    //      this instantiation) 
    let ModuleTable = 0x00
    // *Generation (a 2-byte value, reserved, shall be zero) 
    // *Name (an index into the String heap) 
    // *Mvid (an index into the Guid heap; simply a Guid used to distinguish between two 
    //      versions of the same module) 
    // *EncId (an index into the Guid heap; reserved, shall be zero) 
    // *EncBaseId (an index into the Guid heap; reserved, shall be zero) 
    let ModuleRefTable = 0x1A
    // *Name (an index into the String he ap) 
    let NestedClassTable = 0x29
    // *NestedClass (an index into the TypeDef table) 
    // *EnclosingClass (an index into the TypeDef table) 
    let ParamTable = 0x08
    // *Flags (a 2-byte bitmask of type ParamAttributes, §II.23.1.13) 
    // *Sequence (a 2-byte constant) 
    // *Name (an index into the String heap) 
    let PropertyTable = 0x17
    // *Flags (a 2-byte bitmask of type Pro pertyAttributes, §II.23.1.14) 
    // *Name (an index into the String heap) 
    // *Type (an index into the Blob heap) (The name of this column is misleading. It does 
    //      not index a TypeDef or TypeRef table—instead it indexes the signature in the Blob 
    //      heap of the Property) 
    let PropertyMapTable = 0x15
    // *Parent (an index into the TypeDef table) 
    // *PropertyList (an index into the Property table). It marks the first of a contiguous 
    //      run of Properties owned by Parent. The run continues to the smaller of: 
    //          o the last row of the Property table 
    //          o the next run of Properties, foun d by inspecting the PropertyList of the 
    //              next row in this PropertyMap table 
    let StandAloneSigTable = 0x11
    // *Signature (an index into the Blob heap) 
    let TypeDefTable = 0x02
    // *Flags (a 4-byte bitmask of type TypeAttributes, §II.23.1.15) 
    // *TypeName (an index into the String heap) 
    // *TypeNamespace (an index into the String heap) 
    // *Extends (an index into the TypeDef, TypeRef, or TypeSpec table; more precisely, a 
    //      TypeDefOrRef (§II.24.2.6) coded index) 
    // *FieldList (an index into the Field table; it marks the first of a contiguous run of 
    //      Fields owned by this Type). The run continues to the smaller of: 
    //      o the last row of the Field table 
    //      o the next run of Fields, found by inspecting the FieldList of the next row 
    //          in this TypeDef table 
    // * MethodList (an index into the MethodDef table; it marks the first of a continguous 
    //      run of Methods owned by this Type ). The run continues to the smaller of: 
    //      o the last row of the MethodDef table 
    //      o the next run of Methods, found by inspecting the MethodList of the next 
    //          row in this TypeDef table 
    let TypeRefTable = 0x01
    // *ResolutionScope (an index into a Module, ModuleRef, AssemblyRef or TypeRef table, 
    //      or null; more precisely, a ResolutionScope (§II.24.2.6) coded index) 
    // *TypeName (an index into the String heap) 
    // *TypeNamespace (an index into the String heap)
    let TypeSpecTable = 0x1B
    
    // *Signature (index into the Blob heap, where the blob is formatted as specified 
    //      in §II.23.2.14) 
    // Create a table record type
    type TableRec = 
        { Name : string
          Number : int }
    
    // A table of these records will be made when the TypeDefTable is read from CLI header? 
    type TypeDefTableRec = 
        { _index : int // index in the TypeDefTable (0 is the pseudo class)
          Flags : uint32
          TypeName : uint16
          TypeNameStr : string
          TypeNamespace : uint16
          TypeNamespaceStr : string
          Extends : uint16
          FieldList : uint16
          MethodList : uint16 }
    
    let getTypeDefTableRecOwner (methodDefIndex : int, typeDefTable : TypeDefTableRec []) = 
        let typeDefTableRecOwnerIndex = 
            typeDefTable |> Seq.tryFindIndex (fun typeDefRec -> 
                                // don't look in the pseudo class row
                                // but start with the 3rd row (if there)
                                if typeDefRec._index > 1 && typeDefRec.MethodList > uint16 (methodDefIndex) then true
                                else false)
        if typeDefTableRecOwnerIndex.IsSome then typeDefTable.[typeDefTableRecOwnerIndex.Value]
        else 
            // then the last row must be the owner
            typeDefTable.[typeDefTable.Length - 1]
    
    // CIL instruction to be included in an array of CIL instructions for
    // each method
    type CilRec = 
        { Pos : int
          Instruction : InstRec
          Params : uint64 }
    // --BLOB--
    type BlobSignature(raw : byte []) = 
        member this.Raw = raw 
    
    // --BLOBS--
    // Types II.23.1.16
        // TYPES
    type ElementTypeRec = {
        Name : string
        Value : int
    }

    type MethodDefSigRec(raw : byte [], paramCount : int, paramTypes : ElementTypeRec [], returnType : ElementTypeRec) = 
       inherit BlobSignature(raw)
        member this.ParamCount = paramCount
        member this.ParamTypes = paramTypes 
        member this.ReturnType = returnType
    
    type LocalVarSigRec(raw : byte [], count : int, types : ElementTypeRec []) = 
        inherit BlobSignature(raw) 
        member this.Count = count
        member this.Types = types

    type BlobEntryRec = 
         { // Index (as it would be used from say LocalVarSig
           StartIndx : int
           // The blob signature holds either the MethodDefSigRec or the LocalVarSigRec
           Signature : BlobSignature }

    // StandAloneSig table 0x11
    type StandAloneSigRow =
            { // Signature is an index into the Blog heap
              Signature : uint16 
              LocalVarSig : LocalVarSigRec }

    // Table types
    type MethodDefRowType = 
        { _index : int
          Rva : int
          ImplFlags : uint16
          Flags : uint16
          NameStr : string
          SignatureBH : uint16
          BlobRec : BlobEntryRec
          ParamListIndex : uint16
          mutable NumArgs : int
          mutable Locals : LocalVarSigRec option // = ref None
          TypeDefTableRecOwner : TypeDefTableRec
          mutable Instructions : CilRec [] }
    
    /// Param table 0x08
    type ParamTableRow =
        { Flags : uint16 // 'ParamAttributes', see II.23.1.13
          // 2-byte constant
          Sequence : uint16 
          // Index into String heap
          Name : uint16 
          // from the String heap
          NameStr : string  }

    //// StandAloneSig table 0x11
    ////type StandAloneSigRow =
    ////        { // Signature is an index into the Blog heap
    ////          Signature : uint16 
    ////          LocalVarSigRecLink : LocalVarSigRec}

    type StreamHeader = 
        { StreamHeaderStart : int
          StreamHeaderOffset : int
          StreamHeaderSize : int
          StreamHeaderName : string 
          mutable StreamBytes : byte []}
    
    // Table in alphabetical order
    let tableList = 
        [ { Name = "AssemblyTable"
            Number = 0x20 }
          { Name = "AssemblyOSTable"
            Number = 0x22 }
          { Name = "AssemblyProcessorTable"
            Number = 0x21 }
          { Name = "AssemblyRefTable"
            Number = 0x23 }
          { Name = "AssemblyRefOSTable"
            Number = 0x25 }
          { Name = "AssemblyRefProcessorTable"
            Number = 0x24 }
          { Name = "ClassLayoutTable"
            Number = 0x0F }
          { Name = "ConstantTable"
            Number = 0x0B }
          { Name = "CustomAttributeTable"
            Number = 0x0C }
          { Name = "DeclSecurityTable"
            Number = 0x0E }
          { Name = "EventMapTable"
            Number = 0x12 }
          { Name = "EventTable"
            Number = 0x14 }
          { Name = "ExportedTypeTable"
            Number = 0x27 }
          { Name = "FieldTable"
            Number = 0x04 }
          { Name = "FieldLayoutTable"
            Number = 0x10 }
          { Name = "FieldMarshalTable"
            Number = 0x0D }
          { Name = "FieldRVATable"
            Number = 0x1D }
          { Name = "FileTable"
            Number = 0x26 }
          { Name = "GenericParam"
            Number = 0x2A }
          { Name = "GenericParamConstraintTable"
            Number = 0x2C }
          { Name = "ImplMapTable"
            Number = 0x1C }
          { Name = "InterfaceImplTable"
            Number = 0x09 }
          { Name = "ManifestResourceTable"
            Number = 0x28 }
          { Name = "MemberRefTable"
            Number = 0x0A }
          { Name = "MethodDefTable"
            Number = 0x06 }
          { Name = "MethodImplTable"
            Number = 0x19 }
          { Name = "MethodSemanticsTable"
            Number = 0x18 }
          { Name = "MethodSpecTable"
            Number = 0x2B }
          { Name = "ModuleTable"
            Number = 0x00 }
          { Name = "ModuleRefTable"
            Number = 0x1A }
          { Name = "NestedClassTable"
            Number = 0x29 }
          { Name = "ParamTable"
            Number = 0x08 }
          { Name = "PropertyTable"
            Number = 0x17 }
          { Name = "PropertyMapTable"
            Number = 0x15 }
          { Name = "StandAloneSigTable"
            Number = 0x11 }
          { Name = "TypeDefTable"
            Number = 0x02 }
          { Name = "TypeRefTable"
            Number = 0x01 }
          { Name = "TypeSpecTable"
            Number = 0x1B } ]
    
    let tableListOrdered = 
        [ { Name = "ModuleTable"            //.[0]
            Number = 0x00 }
          { Name = "TypeRefTable"           // .[1]
            Number = 0x01 }
          { Name = "TypeDefTable"           // .[2]
            Number = 0x02 }
          { Name = "FieldTable"             // .[3]
            Number = 0x04 }
          { Name = "MethodDefTable"         // .[4]
            Number = 0x06 }
          { Name = "ParamTable"             // .[5]
            Number = 0x08 }
          { Name = "InterfaceImplTable"     // .[6]
            Number = 0x09 }
          { Name = "MemberRefTable"         // .[7]
            Number = 0x0A }
          { Name = "ConstantTable"          // .[8]
            Number = 0x0B }
          { Name = "CustomAttributeTable"   // .[9]
            Number = 0x0C }
          { Name = "FieldMarshalTable"      // .[10]
            Number = 0x0D }
          { Name = "DeclSecurityTable"      // .[11]
            Number = 0x0E }
          { Name = "ClassLayoutTable"       // .[12]
            Number = 0x0F }
          { Name = "FieldLayoutTable"       // .[13]
            Number = 0x10 }
          { Name = "StandAloneSigTable"     // .[14]
            Number = 0x11 }
          { Name = "EventMapTable"          // .[15]
            Number = 0x12 }
          { Name = "EventTable"             // .[16]
            Number = 0x14 }
          { Name = "PropertyMapTable"       // .[17]
            Number = 0x15 }
          { Name = "PropertyTable"          // .[18]
            Number = 0x17 }
          { Name = "MethodSemanticsTable"   // .[19]
            Number = 0x18 }
          { Name = "MethodImplTable"        // .[20]
            Number = 0x19 }
          { Name = "ModuleRefTable"         // .[21]
            Number = 0x1A }
          { Name = "TypeSpecTable"          // .[22]
            Number = 0x1B }
          { Name = "ImplMapTable"           // .[23]
            Number = 0x1C }
          { Name = "FieldRVATable"          // .[24]
            Number = 0x1D }
          { Name = "AssemblyTable"          // .[25]
            Number = 0x20 }
          { Name = "AssemblyProcessorTable" // .[26]
            Number = 0x21 }
          { Name = "AssemblyOSTable"        // .[27]
            Number = 0x22 }
          { Name = "AssemblyRefTable"       // .[28]
            Number = 0x23 }
          { Name = "AssemblyRefProcessorTable" // .[29]
            Number = 0x24 }
          { Name = "AssemblyRefOSTable"     // .[30]
            Number = 0x25 }
          { Name = "FileTable"              // .[31]
            Number = 0x26 }
          { Name = "ExportedTypeTable"      // .[32]
            Number = 0x27 }
          { Name = "ManifestResourceTable"  // .[33]
            Number = 0x28 }
          { Name = "NestedClassTable"       // .[34]
            Number = 0x29 }
          { Name = "GenericParam"           // .[35]
            Number = 0x2A }
          { Name = "MethodSpecTable"        // .[36]
            Number = 0x2B }
          { Name = "GenericParamConstraintTable" // .[37]
            Number = 0x2C } ]
    


    let ElementTypes =  
        [   { Name = "ELEMENT_TYPE_END" // Marks end of a list
              Value = 0x00}
            { Name = "ELEMENT_TYPE_VOID"
              Value = 0x01 }
            { Name = "ELEMENT_TYPE_BOOLEAN"
              Value =  0x02 }
            { Name = "ELEMENT_TYPE_CHAR"
              Value = 0x03 }
            { Name = "ELEMENT_TYPE_I1"
              Value = 0x04 }
            { Name = "ELEMENT_TYPE_U1"
              Value = 0x05 }
            { Name = "ELEMENT_TYPE_I2"
              Value = 0x06 }
            { Name = "ELEMENT_TYPE_U2"
              Value = 0x07 }
            { Name = "ELEMENT_TYPE_I4"
              Value = 0x08 }
            { Name = "ELEMENT_TYPE_U4"
              Value = 0x09 }
            { Name = "ELEMENT_TYPE_I8"
              Value = 0x0a }
            { Name = "ELEMENT_TYPE_U8"
              Value = 0x0b }
            { Name = "ELEMENT_TYPE_R4"
              Value = 0x0c }
            { Name = "ELEMENT_TYPE_R8"
              Value = 0x0d}
            { Name = "ELEMENT_TYPE_STRING"
              Value = 0x0e }
            { Name = "ELEMENT_TYPE_PTR" // Followed by type 
              Value = 0x0f }
            { Name = "ELEMENT_TYPE_BYREF" // Followed by type
              Value = 0x10 }
            { Name = "ELEMENT_TYPE_VALUETYPE" // Followed by TypeDef or TypeRef token
              Value = 0x11 }
            { Name = "ELEMENT_TYPE_CLASS" // Followed by TypeDef or TypeRef token
              Value = 0x12 }
            { Name = "ELEMENT_TYPE_VAR" // Generic parameter in a generic type
              Value = 0x13 }
            { Name = "ELEMENT_TYPE_ARRAY"
              Value = 0x14 }
            { Name = "ELEMENT_TYPE_GENERICINST"
              Value = 0x15}
            { Name = "ELEMENT_TYPE_TYPEDBYREF"
              Value = 0x16}
            { Name = "_"
              Value = 0x17 }  
            { Name = "ELEMENT_TYPE_I"
              Value = 0x18}
            { Name = "ELEMENT_TYPE_U"
              Value = 0x19}
            { Name = "_"
              Value = 0x1a }
            { Name = "ELEMENT_TYPE_FNPTR"
              Value = 0x1b }
            { Name = "ELEMENT_TYPE_OBJECT"
              Value = 0x1c} 
            { Name = "ELEMENT_TYPE_SZARRAY"
              Value = 0x1d }     
            { Name = "ELEMENT_TYPE_MVAR"
              Value = 0x1e }
            { Name = "ELEMENT_TYPE_CMOD_REQD"
              Value = 0x1f } 
            { Name = "ELEMENT_TYPE_CMOD_OPT"
              Value = 0x20 }  
            { Name = "ELEMENT_TYPE_INTERNAL"
              Value = 0x21 } 
            { Name = "_"
              Value = 0x22 }
            { Name = "_"
              Value = 0x23 }
            { Name = "_"
              Value = 0x24 }
            { Name = "_"
              Value = 0x25 }
            { Name = "_"
              Value = 0x26 }
            { Name = "_"
              Value = 0x27 }
            { Name = "_"
              Value = 0x28 }
            { Name = "_"
              Value = 0x29 }
            { Name = "_"
              Value = 0x2A }
            { Name = "_"
              Value = 0x2B }
            { Name = "_"
              Value = 0x2C }
            { Name = "_"
              Value = 0x2D }
            { Name = "_"
              Value = 0x2E }
            { Name = "_"
              Value = 0x2F }
            { Name = "_"
              Value = 0x30 }
            { Name = "_"
              Value = 0x31 } 
            { Name = "_"
              Value = 0x32 }
            { Name = "_"
              Value = 0x33 }
            { Name = "_"
              Value = 0x34 }
            { Name = "_"
              Value = 0x35 }
            { Name = "_"
              Value = 0x36 }
            { Name = "_"
              Value = 0x37 }
            { Name = "_"
              Value = 0x38 }
            { Name = "_"
              Value = 0x39 }
            { Name = "_"
              Value = 0x3A }
            { Name = "_"
              Value = 0x3B }
            { Name = "_"
              Value = 0x3C }
            { Name = "_"
              Value = 0x3D }
            { Name = "_"
              Value = 0x3E }
            { Name = "_"
              Value = 0x3F }
            { Name = "ELEMENT_TYPE_MODIFIER"
              Value = 0x40  } 
            { Name = "ELEMENT_TYPE_SENTINEL"
              Value = 0x41  } 
            { Name = "_"
              Value = 0x42 }
            { Name = "_"
              Value = 0x43 }
            { Name = "_"
              Value = 0x44 }
            { Name = "ELEMENT_TYPE_PINNED"
              Value = 0x45  } 
            { Name = "_"
              Value = 0x46 }
            { Name = "_"
              Value = 0x47 }
            { Name = "_"
              Value = 0x48 }
            { Name = "_"
              Value = 0x49 }
            { Name = "SYSTEM_TYPE"
              Value = 0x50 } 
            { Name = "CUST_ATTR_BOXED_OBJECT"
              Value = 0x51 } 
            { Name = "RESERVED"
              Value = 0x52 } 
            { Name = "CUST_ATTR_FIELD"
              Value = 0x53 } ]
    
    let DEFAULT   = 0x00uy 
    let C         = 0x01uy
    let STDCALL   = 0x02uy
    let THISCALL  = 0x03uy
    let FASTCALL  = 0x04uy
    let VARARG    = 0x05uy
    // LocalVarSig [II.23.2.6]
    let LOCALSIG = 0x07uy



//==============================SIGNATURE CLASS
    
    // The Blob class will hold the individual blobs. A blob is stored with its length encoded in the first byte(s). 
    // Most of them use only one byte, which means between 0 and 127 bytes for the length.
    // The element types are described in II.23.1.16 'Element types used in signatures'. For example, ELEMENT_TYPE_I4 = 0x08
    // The class uses both an array and a dictioary to store the entries
    type Blob() =
        let mutable rawBlobBytes : byte [] = Array.zeroCreate 0
        let blobArray = new ResizeArray<BlobEntryRec>()
        let blobDict = new Dictionary<int,BlobEntryRec>()

        static member PrintBlobEntryRec(blobEntry : BlobEntryRec) =
            prn (sprintf "BlobEntryRec.StartIndx:%d" blobEntry.StartIndx)
            prn (sprintf "BlobEntryRec.Signature:%A" blobEntry.Signature)
            if blobEntry.Signature:?LocalVarSigRec then
                let lvsr = blobEntry.Signature:?>LocalVarSigRec
                prn (sprintf "BlobEntryRec.Signature.LocalVarSig.Count:%d" lvsr.Count)
                prn (sprintf "BlobEntryRec.Signature.LocalVarSig.Types:%A" lvsr.Types) 
            else if blobEntry.Signature:?MethodDefSigRec then
                let mdsr = blobEntry.Signature:?>MethodDefSigRec
                prn (sprintf "BlobEntryRec.Signature.MethodDefSigRec.ParamCount:%d" mdsr.ParamCount)
                prn (sprintf "BlobEntryRec.Signature.MethodDefSigRec.ParamTypes:%A" mdsr.ParamTypes)

        /// The underlying blob array bytes
        member this.RawBlobBytes with get() = rawBlobBytes and
                                      set(rb) = rawBlobBytes <- rb
        /// Adds a BlobEntryRec by appending it to an array and a dictionary (using an offset)
        member this.AddBlobEntry (offsKey : int, blobEntryRec : BlobEntryRec) =
            blobArray.Add(blobEntryRec)
            blobDict.Add(offsKey, blobEntryRec)            

        /// Add a "raw" entry, which is parsed to a BlobEntryRec, and then ....
        member this.AddRawBlobEntry(offsKey : int, rawBlob : byte []) =
            // Now classify the blobs
            // Create the BlobEntryRecs from the raw 
            prn (sprintf "%d->Blob:%A" offsKey rawBlob)
            if rawBlob.Length > 1 then
                let sigTypeRaw = rawBlob.[0] &&& 0x0Fuy
                // MethodDefSig [II.23.2]
                // The lowest 4 bits determine which type of signature            
                // Find the signature type
                if sigTypeRaw = DEFAULT then
                    // The DEFAULT calling convention is the typical one
                    // See II.23.2.3 and II.15.3. It is used to encode the keyword 'default'in the calling convention
                    // Then comes the (compressed) 'ParamCount'. 
                    // Note: The compressed number of parameters is a "straight" uncompressed encoding for counts between 
                    // 0 and 127.
                    // Then follows the 'RetType' encoding.  
                    prn "MethodDefSig: DEFAULT"
                    // 1 means one parameter
                    let paramCount = int(rawBlob.[1])
                    // 1 means void return type
                    let returnType = int(rawBlob.[2])
                    // 14 (0xE) means String
                    //let firstParam = int(rawBlob.[3])
                    // So in the MethodSignature record I need int:ParamCount, array of ElemTypes, return type
                    //     type LocalVarSigRec = 
                    //      {   Raw : byte [] 
                    //          Count : int 
                    //          Types : ElementTypeRec [] }
                    //localVarSigRec = 
                    //    { Raw = rawBlob
                    //      Count = paramCount
                    //      }
                    let localParams = new ResizeArray<ElementTypeRec>()
                    for indx in 0 .. paramCount - 1 do
                        let paramTypeId = int(rawBlob.[3 + indx])
                        prn (sprintf "Param[0x%02X]: \"%s\" (Value:0x%X)" paramTypeId ElementTypes.[paramTypeId].Name ElementTypes.[paramTypeId].Value)
                        let paramElemType = 
                            {   Name = ElementTypes.[paramTypeId].Name
                                Value = ElementTypes.[paramTypeId].Value }
                        localParams.Add(paramElemType)
                    let returnTypeElem =
                        {   Name = ElementTypes.[returnType].Name
                            Value = ElementTypes.[returnType].Value }
                    // type MethodDefSigRec = 
                    //          {   // Holds the length number of bytes
                    //              raw : byte [] 
                    //              ParamCount : int
                    //              ParamTypes : ElementTypeRec [] 
                    //              ReturnType : ElementTypeRec }
                    // TODO: The same as on line 737
                    let methodDefSigRec =
                        new MethodDefSigRec (rawBlob, paramCount, localParams.ToArray(), returnTypeElem )
                    let blobMethodEntry =
                        {   StartIndx = offsKey
                            Signature = methodDefSigRec }
                    this.AddBlobEntry(offsKey, blobMethodEntry)
                elif sigTypeRaw = C then
                    prn "C"
                elif sigTypeRaw = STDCALL then
                    prn "MethodDefSig: STDCALL"
                elif sigTypeRaw = THISCALL then
                    prn "MethodDefSig: THISCALL"
                elif sigTypeRaw = FASTCALL then
                    prn "MethodDefSig: FASTCALL"
                elif sigTypeRaw = VARARG then
                    prn "MethodDefSig: VARARG"
                elif sigTypeRaw = LOCALSIG then
                    prn "LocalVarSig"
                    // Type count is in the first byte after the length
                    let typeCount = int(rawBlob.[1])
                    prn (sprintf " Count=%d" typeCount)
                    let elementTypes = new ResizeArray<ElementTypeRec>()           
                    for indx in 0 .. typeCount - 1 do
                        let tpe = int(rawBlob.[2 + indx])
                        prn (sprintf "Element[0x%02X]: \"%s\" (Value:0x%X)" tpe ElementTypes.[tpe].Name ElementTypes.[tpe].Value)
                        let elementType = 
                            {   Name = ElementTypes.[tpe].Name
                                Value = ElementTypes.[tpe].Value }
                        elementTypes.Add(elementType)
                    let localVarSigRec = new LocalVarSigRec(rawBlob, typeCount, elementTypes.ToArray())
                    let blobEntry = 
                        {   StartIndx = offsKey
                            Signature = localVarSigRec }
                    this.AddBlobEntry(offsKey, blobEntry)
                else prn (sprintf "Unknown signature type: 0x%02X" rawBlob.[0])
                ()
            else
                prn ("zero blob")

        /// This will return the BlobEntryRec by the logical index
        member this.GetEntryByIndex(indx : int) =
            blobArray.[indx]
        /// This will return the BlobEntryRec from the given offset
        member this.GetEntryByOffset(offsKey : int) =
            blobDict.[offsKey]

        // Get number of entries
        member this.Count() =
            blobArray.Count
