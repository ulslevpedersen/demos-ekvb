// Copyright: 2015-2017, Copenhagen Business School
// Author: Rasmus Ulslev Pedersen (rup.itm@cbs.dk)
// License: Simplified BSD License
//
// Cilcc: CIL codes to runtime structures

// TODO: Code that can write 'cilFile.xml' based on 'typeDefTable : TypeDefTableRec list'
namespace Cilcc

open System.Collections.Generic;

//open FSharp.Data

// The module uses an xml typeprovider. It reads the cilFile.xml to generate the 
// verilog mem.sv file. That file is used by Vivado to get the cil instructions.
// mem.sv: There are the 'cit', 'cit_mit', and 'cit_mit_cil' tables. They are for the 
// classes, methods and cil code respectively. The table record formats are:
//
// cit record:          [0]CIT_ID_INDEX, 
//                      [1]CIT_NUMFIELDS_INDEX, 
//                      [2]CIT_NUMMETHODS_INDEX
//
// cit_mit record:      [0]CIT_MIT_ID_INDEX
//                      [1]CIT_MIT_NUMARGS_INDEX
//                      [2]CIT_MIT_NUMLOCALS_INDEX
//                      [3]CIT_MIT_MAXEVALSTACK_INDEX
//                      [4]CIT_MIT_NUMCIL_INDEX
//                      [5]CIT_MIT_FIRSTCIL_INDEX
//
// cit_mit_cil record:  [0]CIT_MIT_CIL_ID_INDEX
//                      [1]CIT_MIT_CIL_OPCODE
//                      [2]CIT_MIT_CIL_OPERAND
[<AutoOpen>]
module CilXmlProgram = 
    // The record types for the cit, cit_mit, and cit_mit_cil tables
    type CitMemTableRec = 
        { CIT_ID_INDEX : uint32
          CIT_NUMFIELDS_INDEX : uint32
          CIT_NUMMETHODS_INDEX : uint32 }
    
    type CitMitMemTableRec = 
        { CIT_MIT_ID_INDEX : uint32
          CIT_MIT_CITID_INDEX : uint32 // Foreign key
          CIT_MIT_NUMARGS_INDEX : uint32
          CIT_MIT_NUMLOCALS_INDEX : uint32
          CIT_MIT_MAXEVALSTACK_INDEX : uint32
          CIT_MIT_NUMCIL_INDEX : uint32
          CIT_MIT_FIRSTCIL_INDEX : uint32 }
    
    type CitMitCilTableRec = 
        { CIT_MIT_CIL_ID_INDEX : uint32
          CIT_MIT_CIL_OPCODE : uint32
          CIT_MIT_CIL_OPERAND : uint32 }
    
    //type CilXML = XmlProvider< "CilXMLFileProvider.xml" >
    
    let citHeader = """// CIT: Class Information Table
    // parameter logic [31:0] CIT_ID_INDEX              = 32'h00; // Class id
    // parameter logic [31:0] CIT_NUMFIELDS_INDEX       = 32'h01; // Number of fields
    // parameter logic [31:0] CIT_NUMMETHODS_INDEX      = 32'h02; // Number of metods"""
    //let vivadoPath = ""
    let vivadoPath = "C:/DB/Dropbox/40-sw/vivado/tcltest/"
    let mutable citStr = ""
    let mutable citMitStr = ""
    let mutable citMitCilStr = ""
    let mutable citMitAdr = 0
    let mutable citMitCilAdr = 0
    
    let asm2hex (asmStr : string) = 
        let mutable indx = 0
        for i = 0 to asmStr.Length - 1 do
            if asmStr.[i] = 'x' then indx <- i
        "0000" + asmStr.Substring(indx + 1, asmStr.Length - (indx + 1))
    
    // This method emits the three memory tables
    let writeToVerilog (typeDefTable : TypeDefTableRec [], methodDefRows : MethodDefRowType []) = 
        // cit //
        // Map of CIT primary key (here row index) to the actual CIT_ID
        let dictMitCit = new Dictionary<int, int>();
        let mutable typeDefStr = 
            "//--------------------------------------------------------------------------  \n\n" +
            "// cit records:         [0]CIT_ID_INDEX\n" + 
            "//                      [1]CIT_NUMFIELDS_INDEX\n" + 
            "//                      [2]CIT_NUMMETHODS_INDEX\n"
        // skip pseudo class
        let mutable typeDefMemWordCnt = 0
        for i in 1..typeDefTable.Length - 1 do
            let typeDefRec = typeDefTable.[i]
            // Enables type look up when heap frame is created from NEWOBJ inst
            dictMitCit.Add(typeDefRec._index, typeDefMemWordCnt);
            typeDefStr <- typeDefStr + "\n" + "// " + typeDefRec.TypeNameStr + "\n" + 
                        "cit[" + (sprintf "%04d" (typeDefMemWordCnt+0)) + "]" + " <= " + "32'h" + (sprintf "%08X" typeDefRec._index) + ";" 
                          + "      // CIT_ID_INDEX" + "\n" + 
                        "cit[" + (sprintf "%04d" (typeDefMemWordCnt + 1)) + "]" + " <= " + "32'h" + (sprintf "%08X" 0) + ";" 
                          + "      // CIT_NUMFIELDS_INDEX" + "\n" + 
                        "cit[" + (sprintf "%04d" (typeDefMemWordCnt + 2)) + "]" + " <= " + "32'h" + (sprintf "%08X" methodDefRows.Length) + ";" 
                          + "      // CIT_NUMMETHODS_INDEX" + "\n"
            typeDefMemWordCnt <- typeDefMemWordCnt + 3
        
        // cit_mit_names
        // Enable better debugging with method names available in Verilog
        let mutable methodNamesDefStr =
            "//--------------------------------------------------------------------------  \n\n" +
            "// cit_mit_name records:[0]CIT_MIT_ID_NAME_INDEX\n"
        // Not using [0] for a name
        methodNamesDefStr <- methodNamesDefStr + "\n" + "cit_mit_names[00000000] <= \"noname\";" + "               \t// CIT_MIT_ID_NAME INDEX\n"

        // cit_mit //
        let mutable methodDefStr = 
            "//--------------------------------------------------------------------------  \n\n" +
            "// cit_mit records:     [0]CIT_MIT_ID_INDEX\n" + 
            "//                      [1]CIT_MIT_CITID_INDEX (foreign key)\n" +
            "//                      [2]CIT_MIT_NUMARGS_INDEX\n" + 
            "//                      [3]CIT_MIT_NUMLOCALS_INDEX\n" + 
            "//                      [4]CIT_MIT_MAXEVALSTACK_INDEX\n" +
            "//                      [5]CIT_MIT_NUMCIL_INDEX\n" + 
            "//                      [6]CIT_MIT_FIRSTCIL_INDEX\n"
        let mutable methodDefMemWordCnt = 0
        let mutable firstCilOffset = 0
        for i in 0..methodDefRows.Length - 1 do
            let methodDefRec = methodDefRows.[i]
            methodNamesDefStr <- methodNamesDefStr + "cit_mit_names[" + (sprintf "%08X" methodDefRec._index) + "]" + " <= " + "\"" + methodDefRec.TypeDefTableRecOwner.TypeNameStr
                                + ":" + methodDefRec.NameStr + "\";" + "      \t// CIT_MIT_ID_NAME INDEX\n"
            let mutable numLocals = 0
            // Points to the key index in the cit table
            let mitCitKey = dictMitCit.[methodDefRec.TypeDefTableRecOwner._index];
            if methodDefRec.Locals.IsSome then
                numLocals <- methodDefRec.Locals.Value.Count
            methodDefStr <- methodDefStr + "\n" + "// " + methodDefRec.TypeDefTableRecOwner.TypeNameStr 
                                + ":" + methodDefRec.NameStr + "\n" + 
                            "cit_mit[" + (sprintf "%04d" (methodDefMemWordCnt + 0)) + "]" + " <= " + "32'h" + (sprintf "%08X" methodDefRec._index) + ";" 
                                + "      // CIT_MIT_ID_INDEX\n" +
                            "cit_mit[" + (sprintf "%04d" (methodDefMemWordCnt + 1)) + "]" + " <= " + "32'h" + (sprintf "%08X" mitCitKey) + ";" 
                                + "      // CIT_MIT_CITID_INDEX (foreign [key])" + "\n" + 
                            "cit_mit[" + (sprintf "%04d" (methodDefMemWordCnt + 2)) + "]" + " <= " + "32'h" + (sprintf "%08X" methodDefRec.NumArgs) + ";" 
                                + "      // CIT_MIT_NUMARGS_INDEX\n" +
                            "cit_mit[" + (sprintf "%04d" (methodDefMemWordCnt + 3)) + "]" + " <= " + "32'h" + (sprintf "%08X" numLocals) + ";" 
                                + "      // CIT_MIT_NUMLOCALS_INDEX\n" +
                            "cit_mit[" + (sprintf "%04d" (methodDefMemWordCnt + 4)) + "]" + " <= " + "32'h" + (sprintf "%08X" 8) + ";" 
                                + "      // CIT_MIT_MAXEVALSTACK_INDEX\n" +
                            "cit_mit[" + (sprintf "%04d" (methodDefMemWordCnt + 5)) + "]" + " <= " + "32'h" + (sprintf "%08X" methodDefRec.Instructions.Length) + ";" 
                                + "      // CIT_MIT_NUMCIL_INDEX\n" +
                            "cit_mit[" + (sprintf "%04d" (methodDefMemWordCnt + 6)) + "]" + " <= " + "32'h" + (sprintf "%08X" firstCilOffset) + ";" 
                                + "      // CIT_MIT_FIRSTCIL_INDEX\n"
            // 7 is hardcoded
            methodDefMemWordCnt <- methodDefMemWordCnt + 7
            firstCilOffset <- firstCilOffset + ((methodDefRec.Instructions.Length * 3) - 1)

        // cit_mit_cil //
        let mutable instDefStr = 
            "//--------------------------------------------------------------------------  \n\n" +
            "// cit_mit_cil records: [0]CIT_MIT_CIL_ID_INDEX\n" + 
            "//                      [1]CIT_MIT_CIL_OPCODE\n" + 
            "//                      [2]CIT_MIT_CIL_OPERAND\n"
        let mutable methodDefMemWordCnt = 0
        let mutable cilIndexCnt = 0
        for i in 0..methodDefRows.Length - 1 do
            let methodDefRec = methodDefRows.[i]
            for j in 0..methodDefRec.Instructions.Length - 1 do
                let instDefRec = methodDefRec.Instructions.[j]
                instDefStr <- instDefStr + "\n" + "// " + methodDefRec.TypeDefTableRecOwner.TypeNameStr 
                                + " : " + methodDefRec.NameStr + " : rva " + (sprintf "0x%08x" methodDefRec.Rva) + "\n" + 
                                "cit_mit_cil[" + (sprintf "%04d" (cilIndexCnt+0)) + "]" + " <= " + "32'h" + (sprintf "%08X" instDefRec.Pos) + ";" 
                                    + "      // CIT_MIT_CIL_ID_INDEX" + "\n" + 
                                "cit_mit_cil[" + (sprintf "%04d" (cilIndexCnt + 1)) + "]" + " <= " + "32'h" + (sprintf "%08X" instDefRec.Instruction.b2) + ";" 
                                    + "      // CIT_MIT_CIL_OPCODE: \t\t" + instDefRec.Instruction.StringName + "\n" + 
                                "cit_mit_cil[" + (sprintf "%04d" (cilIndexCnt + 2)) + "]" + " <= " + "32'h" + (sprintf "%08X" instDefRec.Params) + ";" 
                                    + "      // CIT_MIT_CIL_OPERAND: \t" + (sprintf "%d" instDefRec.Params) + "\n"
                cilIndexCnt <- cilIndexCnt + 3
        System.IO.File.WriteAllText
            ("C:/Dropbox/40-sw/VisualStudio/ConsoleApplication1/ConsoleApplication1/mem.sv", 
             ("// mem.sv memory initialization\n" + "// " + System.DateTime.Now.ToLongTimeString() + "\n\n" + typeDefStr + "\n" + methodNamesDefStr + "\n" 
             + methodDefStr + "\n" + instDefStr))
    
//    let main2 = 
//        let cilcode = CilXML.Load("../../cilFile.xml")
//        for i = 0 to cilcode.Classes.Length - 1 do
//            let clas = cilcode.Classes.[i]
//            printfn "#fields: %d" clas.Cilfields.Length
//            citStr <- citStr + sprintf "cit[%d] <= 32'h%08x;\t\t// CIT_ID_INDEX\n" (i * 3) i 
//                      + sprintf "cit[%d] <= 32'h%08x;\t\t// CIT_NUMFIELDS_INDEX\n" (i * 3 + 1) clas.Cilfields.Length 
//                      + sprintf "cit[%d] <= 32'h%08x;\t\t// CIT_NUMMETHODS_INDEX\n" (i * 3 + 2) clas.Methods.Length
//            printfn "citStr:%s" citStr
//            for f = 0 to clas.Cilfields.Length - 1 do
//                let cilField = clas.Cilfields.[f]
//                printfn "field: %s" cilField.Name
//            printfn "len %d" clas.Methods.Length
//            for j = 0 to clas.Methods.Length - 1 do
//                let meth = clas.Methods.[j]
//                citMitStr <- citMitStr + sprintf "cit_mit[%d] <= 32'h%08x;\t\t// CIT_MIT_ID_INDEX\n" citMitAdr j 
//                             + sprintf "cit_mit[%d] <= 32'h%08x;\t\t// CIT_MIT_NUMARGS_INDEX\n" (citMitAdr + 1) 
//                                   meth.Numargs 
//                             + sprintf "cit_mit[%d] <= 32'h%08x;\t\t// CIT_MIT_NUMLOCALS_INDEX\n" (citMitAdr + 2) 
//                                   meth.Numlocals 
//                             + sprintf "cit_mit[%d] <= 32'h%08x;\t\t// CIT_MIT_MAXEVALSTACK_INDEX\n" (citMitAdr + 3) 
//                                   meth.Maxevalstack 
//                             + sprintf "cit_mit[%d] <= 32'h%08x;\t\t// CIT_MIT_NUMCIL_INDEX\n" (citMitAdr + 4) 
//                                   meth.Instructions.Length 
//                             + sprintf "cit_mit[%d] <= 32'h%08x;\t\t// CIT_MIT_FIRSTCIL_INDEX\n" (citMitAdr + 5) 
//                                   citMitCilAdr
//                citMitAdr <- citMitAdr + 6
//                printfn "citMitStr:%s" citMitStr
//                citMitCilStr <- citMitCilStr + "// " + meth.Name + "\n"
//                for k = 0 to meth.Instructions.Length - 1 do
//                    let inst = meth.Instructions.[k]
//                    let citMitCilStrNew = 
//                        sprintf "// %s %d\n" inst.Name inst.Operand 
//                        + sprintf "cit_mit_cil[%d] <= 32'h%08x;\t\t// CIT_MIT_CIL_ID_INDEX\n" citMitCilAdr k 
//                        + sprintf "cit_mit_cil[%d] <= 32'h%8s;\t\t// CIT_MIT_CIL_OPCODE\n" (citMitCilAdr + 1) 
//                              (asm2hex (inst.Name)) 
//                        + sprintf "cit_mit_cil[%d] <= 32'h%08x;\t\t// CIT_MIT_CIL_OPERAND\n" (citMitCilAdr + 2) 
//                              inst.Operand
//                    citMitCilStr <- citMitCilStr + citMitCilStrNew
//                    citMitCilAdr <- citMitCilAdr + 3
//                    printfn "citMitCilStr:%s" citMitCilStr
//        System.IO.File.WriteAllText
//            (vivadoPath + "tmp.srcs/sources_1/imports/new/mem.sv", 
//             ("// mem.sv memory initialization\n" + citStr + "\n//cit_mit init\n" + citMitStr + "\n//cit_mit_cil init\n" 
//              + citMitCilStr))
//        0 // return an integer exit code
// Example initialization
// System.IO.File.WriteAllText(vivadoPath + "tmp.srcs/sources_1/imports/new/mem.sv", "cit[0] <= 32'h1234;\n")
//System.IO.File.WriteAllText(vivadoPath + "cil.hex", citStr)
//System.IO.File.WriteAllText(vivadoPath + "cil_mit.hex", citMitStr)
//System.IO.File.WriteAllText(vivadoPath + "cil_mit_cil.hex", citMitCilStr)
//// mem.sv memory initialization
//cit[0] <= 32'h00000000;		// CIT_ID_INDEX
//cit[1] <= 32'h00000001;		// CIT_NUMFIELDS_INDEX
//cit[2] <= 32'h00000002;		// CIT_NUMMETHODS_INDEX
//
////cit_mit init
//cit_mit[0] <= 32'h00000000;		// CIT_MIT_ID_INDEX
//cit_mit[1] <= 32'h00000000;		// CIT_MIT_NUMARGS_INDEX
//cit_mit[2] <= 32'h00000001;		// CIT_MIT_NUMLOCALS_INDEX
//cit_mit[3] <= 32'h00000008;		// CIT_MIT_MAXEVALSTACK_INDEX
//cit_mit[4] <= 32'h0000000d;		// CIT_MIT_NUMCIL_INDEX
//cit_mit[5] <= 32'h00000000;		// CIT_MIT_FIRSTCIL_INDEX
//cit_mit[6] <= 32'h00000001;		// CIT_MIT_ID_INDEX
//cit_mit[7] <= 32'h00000001;		// CIT_MIT_NUMARGS_INDEX
//cit_mit[8] <= 32'h00000000;		// CIT_MIT_NUMLOCALS_INDEX
//cit_mit[9] <= 32'h00000008;		// CIT_MIT_MAXEVALSTACK_INDEX
//cit_mit[10] <= 32'h00000006;		// CIT_MIT_NUMCIL_INDEX
//cit_mit[11] <= 32'h00000027;		// CIT_MIT_FIRSTCIL_INDEX
//
////cit_mit_cil init
//// method0
//// LDCI4x0020 7
//cit_mit_cil[0] <= 32'h00000000;		// CIT_MIT_CIL_ID_INDEX
//cit_mit_cil[1] <= 32'h00000020;		// CIT_MIT_CIL_OPCODE
//cit_mit_cil[2] <= 32'h00000007;		// CIT_MIT_CIL_OPERAND
//// CALLx0028 1
//cit_mit_cil[3] <= 32'h00000001;		// CIT_MIT_CIL_ID_INDEX
//cit_mit_cil[4] <= 32'h00000028;		// CIT_MIT_CIL_OPCODE
//cit_mit_cil[5] <= 32'h00000001;		// CIT_MIT_CIL_OPERAND
//// STLOCxFE0E 0
//cit_mit_cil[6] <= 32'h00000002;		// CIT_MIT_CIL_ID_INDEX
//cit_mit_cil[7] <= 32'h0000FE0E;		// CIT_MIT_CIL_OPCODE
//cit_mit_cil[8] <= 32'h00000000;		// CIT_MIT_CIL_OPERAND
//// LDCI4x0020 0
//cit_mit_cil[9] <= 32'h00000003;		// CIT_MIT_CIL_ID_INDEX
//cit_mit_cil[10] <= 32'h00000020;		// CIT_MIT_CIL_OPCODE
//cit_mit_cil[11] <= 32'h00000000;		// CIT_MIT_CIL_OPERAND
//// LDCI4x0020 8
//cit_mit_cil[12] <= 32'h00000004;		// CIT_MIT_CIL_ID_INDEX
//cit_mit_cil[13] <= 32'h00000020;		// CIT_MIT_CIL_OPCODE
//cit_mit_cil[14] <= 32'h00000008;		// CIT_MIT_CIL_OPERAND
//// STFLDx007D 0
//cit_mit_cil[15] <= 32'h00000005;		// CIT_MIT_CIL_ID_INDEX
//cit_mit_cil[16] <= 32'h0000007D;		// CIT_MIT_CIL_OPCODE
//cit_mit_cil[17] <= 32'h00000000;		// CIT_MIT_CIL_OPERAND
//// LDCI4x0020 0
//cit_mit_cil[18] <= 32'h00000006;		// CIT_MIT_CIL_ID_INDEX
//cit_mit_cil[19] <= 32'h00000020;		// CIT_MIT_CIL_OPCODE
//cit_mit_cil[20] <= 32'h00000000;		// CIT_MIT_CIL_OPERAND
//// LDCI4x0020 0
//cit_mit_cil[21] <= 32'h00000007;		// CIT_MIT_CIL_ID_INDEX
//cit_mit_cil[22] <= 32'h00000020;		// CIT_MIT_CIL_OPCODE
//cit_mit_cil[23] <= 32'h00000000;		// CIT_MIT_CIL_OPERAND
//// LDFLDx007B 0
//cit_mit_cil[24] <= 32'h00000008;		// CIT_MIT_CIL_ID_INDEX
//cit_mit_cil[25] <= 32'h0000007B;		// CIT_MIT_CIL_OPCODE
//cit_mit_cil[26] <= 32'h00000000;		// CIT_MIT_CIL_OPERAND
//// LDCI4x0020 0
//cit_mit_cil[27] <= 32'h00000009;		// CIT_MIT_CIL_ID_INDEX
//cit_mit_cil[28] <= 32'h00000020;		// CIT_MIT_CIL_OPCODE
//cit_mit_cil[29] <= 32'h00000000;		// CIT_MIT_CIL_OPERAND
//// LDCFLDx007B 0
//cit_mit_cil[30] <= 32'h0000000a;		// CIT_MIT_CIL_ID_INDEX
//cit_mit_cil[31] <= 32'h0000007B;		// CIT_MIT_CIL_OPCODE
//cit_mit_cil[32] <= 32'h00000000;		// CIT_MIT_CIL_OPERAND
//// ADDx0058 99999999
//cit_mit_cil[33] <= 32'h0000000b;		// CIT_MIT_CIL_ID_INDEX
//cit_mit_cil[34] <= 32'h00000058;		// CIT_MIT_CIL_OPCODE
//cit_mit_cil[35] <= 32'h05f5e0ff;		// CIT_MIT_CIL_OPERAND
//// STFLDx007D 0
//cit_mit_cil[36] <= 32'h0000000c;		// CIT_MIT_CIL_ID_INDEX
//cit_mit_cil[37] <= 32'h0000007D;		// CIT_MIT_CIL_OPCODE
//cit_mit_cil[38] <= 32'h00000000;		// CIT_MIT_CIL_OPERAND
//// mymethod1
//// LDCI4x0020 16
//cit_mit_cil[39] <= 32'h00000000;		// CIT_MIT_CIL_ID_INDEX
//cit_mit_cil[40] <= 32'h00000020;		// CIT_MIT_CIL_OPCODE
//cit_mit_cil[41] <= 32'h00000010;		// CIT_MIT_CIL_OPERAND
//// LDARGxFE09 0
//cit_mit_cil[42] <= 32'h00000001;		// CIT_MIT_CIL_ID_INDEX
//cit_mit_cil[43] <= 32'h0000FE09;		// CIT_MIT_CIL_OPCODE
//cit_mit_cil[44] <= 32'h00000000;		// CIT_MIT_CIL_OPERAND
//// ADDx0058 99999999
//cit_mit_cil[45] <= 32'h00000002;		// CIT_MIT_CIL_ID_INDEX
//cit_mit_cil[46] <= 32'h00000058;		// CIT_MIT_CIL_OPCODE
//cit_mit_cil[47] <= 32'h05f5e0ff;		// CIT_MIT_CIL_OPERAND
//// LDCI4x0020 1
//cit_mit_cil[48] <= 32'h00000003;		// CIT_MIT_CIL_ID_INDEX
//cit_mit_cil[49] <= 32'h00000020;		// CIT_MIT_CIL_OPCODE
//cit_mit_cil[50] <= 32'h00000001;		// CIT_MIT_CIL_OPERAND
//// SHLx0062 99999999
//cit_mit_cil[51] <= 32'h00000004;		// CIT_MIT_CIL_ID_INDEX
//cit_mit_cil[52] <= 32'h00000062;		// CIT_MIT_CIL_OPCODE
//cit_mit_cil[53] <= 32'h05f5e0ff;		// CIT_MIT_CIL_OPERAND
//// RETx002A 99999999
//cit_mit_cil[54] <= 32'h00000005;		// CIT_MIT_CIL_ID_INDEX
//cit_mit_cil[55] <= 32'h0000002A;		// CIT_MIT_CIL_OPCODE
//cit_mit_cil[56] <= 32'h05f5e0ff;		// CIT_MIT_CIL_OPERAND
//<!--This file has the CIL-->
//<cilcode topic="Program 1">
//	<classes>
//		<class name="Turbo">
//			<cilfields>
//				<cilfield name="field0"/>
//			</cilfields>
//			<methods>
//				<method name="method0" numargs="0" numlocals="1" maxevalstack="8">
//					<instructions>
//						<instruction name="LDCI4x0020" operand="7" />
//						<instruction name="CALLx0028" operand="1" />
//						<instruction name="STLOCxFE0E" operand="0" />
//						<instruction name="LDCI4x0020" operand="0" />
//						<instruction name="LDCI4x0020" operand="8" />
//						<instruction name="STFLDx007D" operand="0" />
//						<instruction name="LDCI4x0020" operand="0" />
//						<instruction name="LDCI4x0020" operand="0" />
//						<instruction name="LDFLDx007B" operand="0" />
//						<instruction name="LDCI4x0020" operand="0" />
//						<instruction name="LDCFLDx007B" operand="0" />
//						<instruction name="ADDx0058" operand="99999999" />
//						<instruction name="STFLDx007D" operand="0" />
//					</instructions>
//				</method>
//				<method name="mymethod1" numargs="1" numlocals="0" maxevalstack="8">
//					<instructions>
//						<instruction name="LDCI4x0020" operand="16" />
//						<instruction name="LDARGxFE09" operand="0" />
//						<instruction name="ADDx0058" operand="99999999" />
//						<instruction name="LDCI4x0020" operand="1" />
//						<instruction name="SHLx0062" operand="99999999" />
//						<instruction name="RETx002A" operand="99999999" />
//					</instructions>
//				</method>
//			</methods>
//		</class>
//	</classes>
//</cilcode>
