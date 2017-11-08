// Copyright: 2015-, Copenhagen Business School
// Author: Rasmus Ulslev Pedersen (rp.digi@cbs.dk)
// License: Simplified BSD License
//
// fop 

// vlog -work work .\fop.sv
// vlog -work work .\fop_tb.sv
// vsim -c -do "run -all;quit" fop_tb
module fop
(
  input clk,
  input reset,
  input enable 
);
    timeunit 1ns; timeprecision 1ns;
          
    enum logic [31:0] {
        NOPx00         = 32'h0000, // ->
        BREAKx01       = 32'h0001, // ->
        LDARG0x02      = 32'h0002, // -> value
        LDARG1x03      = 32'h0003, // -> value
        LDARG2x04      = 32'h0004, // -> value
        LDARG3x05      = 32'h0005, // -> value
        LDLOC0x06      = 32'h0006, // -> value
        LDLOC1x07      = 32'h0007, // -> value
        LDLOC2x08      = 32'h0008, // -> value
        LDLOC3x09      = 32'h0009, // -> value
        STLOC0x0A      = 32'h000A, // ., value -> .
        STLOC1x0B      = 32'h000B, // ., value -> .
        STLOC2x0C      = 32'h000C, // ., value -> .
        STLOC3x0D      = 32'h000D, // ., value -> .
        STARGSx10      = 32'h0010, // ., value -> ., 
        LDLOCSx11      = 32'h0011, // -> value
        LDLOCASx12     = 32'h0012, // -> address
        STLOCSx13      = 32'h0013, // ., value -> .
        LDNULLx14      = 32'h0014, // -> null, value
        LDCI4M1x15     = 32'h0015, // -> num
        LDCI40x16      = 32'h0016, // -> num
        LDCI41x17      = 32'h0017, // -> num
        LDCI42x18      = 32'h0018, // -> num
        LDCI43x19      = 32'h0019, // -> num
        LDCI44x1A      = 32'h001A, // -> num
        LDCI45x1B      = 32'h001B, // -> num
        LDCI46x1C      = 32'h001C, // -> num
        LDCI47x1D      = 32'h001D, // -> num
        LDCI48x1E      = 32'h001E, // -> num
        LDCI44Sx1F     = 32'h001F, // -> num
        LDARGSx0E      = 32'h000E, // -> value
        LDARGASx0F     = 32'h000F, // -> -> address of argument number argNum
        LDCI4x20       = 32'h0020, // -> num
        LDCI8x21       = 32'h0021, // -> num
        LDCR4x22       = 32'h0022, // -> num
        LDCR8x23       = 32'h0023, // -> num
        DUPx25         = 32'h0025, // value -> value, value
        POPx26         = 32'h0026, // ., value -> .
        JMPx27         = 32'h0027, // ->
        CALLx28        = 32'h0028, // ?                  -> .
        CALLIx29       = 32'h0029, // arg0, arg1, . , argn, ftn -> retVal (sometimes)
        //RETx2A       = 32'h002A, // -> (???)
        BRSx2B         = 32'h002B, // ->
        BRFALSESx2C    = 32'h002C, // value ->
        BRTRUESx2D     = 32'h002D, // value ->
        BEQSx2E        = 32'h002E, // value1, value2 ->
        BGESx2F        = 32'h002F, // value1, value2 ->
        BGTSx30        = 32'h0030, // value1, value2 ->
        BLESx31        = 32'h0031, // value1, value2 ->
        BLTSx32        = 32'h0032, // value1, value2 ->
        BNEUNSx33      = 32'h0033, // value1, value2 ->
        BGEUNSx34      = 32'h0034, // value1, value2 ->
        BGTUNSx35      = 32'h0035, // value1, value2 ->
        BLEUNSx36      = 32'h0036, // value1, value2 ->
        BLTUNSx37      = 32'h0037, // value1, value2 ->
        BRx38          = 32'h0038, // ?                  -> ?
        BRFALSEx39     = 32'h0039, // value ->
        BRTRUEx3A      = 32'h003A, // value ->
        //BRINSTx3A    = 32'h003A, // value ->
        BEQx3B         = 32'h003B, // value1, value2 ->
        BGEx3C         = 32'h003C, // value1, value2 ->
        //BGTx3D       = 32'h002E, // value1, value2 ->
        BLEx3E         = 32'h003E, // value1, value2 ->
        BLTx3F         = 32'h003F, // ?                  -> ?
        BNEUNx40       = 32'h0040, // value1, value2 ->
        BGEUNx41       = 32'h0041, // value1, value2 ->
        BGTUNx42       = 32'h0042, // value1, value2 ->
        BLEUNx43       = 32'h0043, // value1, value2 -> 
        BLTUNx44       = 32'h0044, // value1, value2 ->
        SWITCHx45      = 32'h0045, // ., value -> .
        LDINDI1x46     = 32'h0046, // addr -> value
        LDINDU1x47     = 32'h0047, // addr -> value
        LDINDI2x48     = 32'h0048, // addr -> value
        LDINDU2x49     = 32'h0049, // addr -> value
        LDINDI4x4A     = 32'h004A, // addr -> value
        LDINDU4x4B     = 32'h004B, // addr -> value
        LDINDI8x4C     = 32'h004C, // addr -> value
        //LDINDU8x4C   = 32'h004C, // addr -> value
        LDINDIx4D      = 32'h004D, // addr -> value
        LDINDR4x4E     = 32'h004E, // addr -> value
        LDINDR8x4F     = 32'h004F, // addr -> value
        LDINDREFx50    = 32'h0050, // addr -> value
        STINDREFx51    = 32'h0051, // ., addr, val -> .
        STNDI1x52      = 32'h0052, // ., addr, val -> .
        STINDI2x53     = 32'h0053, // ., addr, val -> .
        STINDI4x54     = 32'h0054, // ., addr, val -> .
        STINDI8x55     = 32'h0055, // ., addr, val -> .
        STINDR4x56     = 32'h0056, // ., addr, val -> .
        STINDR8x57     = 32'h0057, // ., addr, val -> .
        ADDx58         = 32'h0058, // x,y -> z
        SUBx59         = 32'h0059, // ., value1, value2 -> ., result
        MULx5A         = 32'h005A, // ., value1, value2 -> ., result
        DIVx5B         = 32'h005B, // value1, value2 -> result
        DIVUNx5C       = 32'h005C, // value1, value2 -> result
        REMx5D         = 32'h005D, // ., value1, value2 -> ., result
        REMUNx5E       = 32'h005E, // ., value1, value2 -> ., result
        ANDx5F         = 32'h005F, // ?, value1, value2 -> ?, result
        ORx60          = 32'h0060, // ., value1, value2 -> ., result
        XORx61         = 32'h0061, // ., value1, value2 -> ., result
        SHLx62         = 32'h0062, // value, shiftAmount -> result
        SHRx63         = 32'h0063, // value, shiftAmount -> result
        SHRUNx64       = 32'h0064, // value, shiftAmount -> result    
        NEGx65         = 32'h0065, // ., value -> ., result
        NOTx66         = 32'h0066, // ., value -> ., result 
        CONVI1x67      = 32'h0067, // value -> result
        CONVI2x68      = 32'h0068, // value -> result
        CONVI4x69      = 32'h0069, // value -> result
        RETx2A         = 32'h002A, // ?     -> . (empty except for returned value)
        CONVI8x6A      = 32'h006A, // value -> result
        CONVR4x6B      = 32'h006B, // value -> result
        CONVR8x6C      = 32'h006C, // value -> result
        CONVU4x6D      = 32'h006D, // value -> result
        CONVU8x6E      = 32'h006E, // value -> result
        CONVRUNx76     = 32'h0076, // value -> result
        LDFLDx7B       = 32'h007B, // ., obj             -> ., value
        CONVOVFI1UNx82 = 32'h0082, // value -> result
        CONVOVFI2UNx83 = 32'h0083, // value -> result
        CONVOVFI4UNx84 = 32'h0084, // value -> result
        CONVOVFI8UNx85 = 32'h0085, // value -> result
        CONVOVFU1UNx86 = 32'h0086, // value -> result
        CONVOVFU2UNx87 = 32'h0087, // value -> result
        CONVOVFU4UNx88 = 32'h0088, // value -> result
        CONVOVFU8UNx89 = 32'h0089, // value -> result
        CONVOVFIUNx8A  = 32'h008A, // value -> result
        CONVOVFUUNx8B  = 32'h008B, // value -> result
        CONVOVFI1xB3   = 32'h00B3, // value -> result
        CONVOVFU1xB4   = 32'h00B4, // value -> result
        CONVOVFI2xB5   = 32'h00B5, // value -> result
        CONVOVFU2xB6   = 32'h00B6, // value -> result
        CONVOVFI4xB7   = 32'h00B7, // value -> result
        CONVOVFI8xB9   = 32'h00B9, // value -> result
        CONVOVFI2xBA   = 32'h00BA, // value -> result
        CKFINITExC3    = 32'h00C3, // value -> value
        CONVU2xD1      = 32'h00D1, // value -> result
        CONVU1xD2      = 32'h00D2, // value -> result
        CONVIxD3       = 32'h00D3, // value -> result
        CONVOVFUxD5    = 32'h00D5, // value -> result
        ADDOVFxD6      = 32'h00D6, // ?, value1, value2 -> ?, result
        ADDOVFUNxD7    = 32'h00D7, // ?, value1, value2 -> ?, result
        MULOVFxD8      = 32'h00D8, // ., value1, value2 -> ., result
        MULOVFUNxD9    = 32'h00D9, // ., value1, value2 -> ., result
        SUBOVFxDA      = 32'h00DA, // ., value1, value2 -> ., result
        SUBOVFUNxDB    = 32'h00DB, // ., value1, value2 -> ., result
        ENDFAULTxDC    = 32'h00DC, // -> 
        //ENDFINALLYxDC= 32'h00DC, // -> 
        LEAVExDD       = 32'h00DD, // . ->
        LEAVESxDE      = 32'h00DE, // . ->
        STINDIxDF      = 32'h00DF, // ., addr, val -> .
        CONVUxE0       = 32'h00E0, // value -> result
        CEQxFE01       = 32'hFE01,  // value1, value2 -> result
        ARGLISTxFE00   = 32'hFE00, // -> argListHandle
        CGTxFE02       = 32'hFE02, // value1, value2 -> result
        CGTUNxFE03     = 32'hFE03, // value1, value2 -> result
        CLTxFE04       = 32'hFE04, // value1, value2 -> result
        CLTUNxFE05     = 32'hFE05, // value1, value2 -> result
        LDFTNxFE06     = 32'hFE06, // -> ftn
        LDARGxFE09     = 32'hFE09, // -> value
        LDARGAxFE0A    = 32'hFE0A, // -> -> address of argument number argNum
        STARGxFE0B     = 32'hFE0B, // ., value -> ., 
        ENDFILTERxFE11 = 32'hFE11, // value ->
        CPBLKxFE17     = 32'hFE17, // destaddr, srcaddr, size -> 
        INITBLKxFE18   = 32'hFE18, // addr, value, size -> 
        LDLOCxFE0C     = 32'hFE0C, // -> value
        LDLOCAxFE0D    = 32'hFE0D, // -> address
        STLOCxFE0E     = 32'hFE0E, // z                  -> .
        LOCALLOCxFE0F  = 32'hFE0F, // size -> address  
        // OBJECT MODEL INSTRUCTIONS
        CALLVIRTx6F    = 32'h006F, // ., obj, arg, ., argN -> ., returnVal (not always returned)
        CPOBJx70       = 32'h0070, // ., dest, src -> ., 
        LDOBJx71       = 32'h0071, // ., src -> ., val
        LDSTRx72       = 32'h0072, // ., -> ., string
        NEWOBJx73      = 32'h0073, // ?, arg1, ? argN    -> ?, obj
        CASTCLASSx74   = 32'h0074, // ., obj -> ., obj2
        LDSFLDx7E      = 32'h007E, // ., -> ., value
        LDSFLDAx7F     = 32'h007F, // ., -> ., address
        BOXx8C         = 32'h008C, // ., val -> ., obj
        ISINSTx75      = 32'h0075, // ., obj -> ., result
        UNBOXx79       = 32'h0079, // ., obj -> ., valueTypePtr
        THROWx7A       = 32'h007A, // ., object -> .,
        //LDFLDx7B     = 32'h007B, // ., obj -> ., value
        LDFLDAx7C      = 32'h007C, // ., obj -> ., address
        STFLDx7D       = 32'h007D, // ., obj, val -> .
        STSFLDx80      = 32'h0080, // ., val -> .
        STOBJx81       = 32'h0081, // ., dest, src -> .,
        NEWARRx8D      = 32'h008D, // ., numElems -> ., array
        LDLENx8E       = 32'h008E, // ., array -> ., length
        LDELEMAx8F     = 32'h008F, // ., array, index -> ., value
        LDELEMI1x90    = 32'h0090, // ., array, index -> ., value
        LDELEMU1x91    = 32'h0091, // ., array, index -> ., value
        LDELEMI2x92    = 32'h0092, // ., array, index -> ., value
        LDELEMU2x93    = 32'h0093, // ., array, index -> ., value
        LDELEMI4x94    = 32'h0094, // ., array, index -> ., value
        LDELEMU4x95    = 32'h0095, // ., array, index -> ., value
        LDELEMI8x96    = 32'h0096, // ., array, index -> ., value
        //LDELEMU8x96  = 32'h0096, // ., array, index -> ., value
        LDELEMIx97     = 32'h0097, // ., array, index -> ., value
        LDELEMR4x98    = 32'h0098, // ., array, index -> ., value
        LDELEMR8x99    = 32'h0099, // ., array, index -> ., value
        LDELEMREFx9A   = 32'h009A, // ., array, index -> ., value
        STELEMIx9B     = 32'h009B, // ., array, index, value -> ., 
        STELEMI1x9C    = 32'h009C, // ., array, index, value -> ., 
        STELEMI2x9D    = 32'h009D, // ., array, index, value -> ., 
        STELEMI4x9E    = 32'h009E, // ., array, index, value -> ., 
        STELEMR4xA0    = 32'h00A0, // ., array, index, value -> .,
        STELEMR8xA1    = 32'h00A1, // ., array, index, value -> .,  
        STELEMREFxA2   = 32'h00A2, // ., array, index, value -> ., 
        LDELEMxA3      = 32'h00A3, // ., array, index -> ., value
        STELEMxA4      = 32'h00A4, // ., array, index, value -> .
        UNBOXANYxA5    = 32'h00A5, // ., obj -> ., value or obj
        REFANYVALxC2   = 32'h00C2, // ., TypedRef -> ., address
        MKREFANYxC6    = 32'h00C6, // ., ptr -> ., typedRef
        LDTOKENxD0     = 32'h00D0, // ., -> ., RuntimeHandle
        LDVIRTFNxFE07  = 32'hFE07, // ., object -> ., ftn
        INITOBJxFE15   = 32'hFE15, // ., dest -> ., 
        RETHROWxFE1A   = 32'hFE1A, // ., -> ., 
        SIZEOFxFE1C    = 32'hFE1C, // ., -> ., size (4 bytes, unsigned)
        REFANYTYPExFE1D= 32'hFE1D // ., TypedRef -> ., type        
    } CILOP;
   
    // stack: slots, pointers, stack frame information
    typedef struct {
        // The registers below (until 'slot[]') fill the slots
        // Id of owner method
        logic [31:0] SF_MIT_ID;       // 32'h00
        // Instance id (if not static => 0)
        logic [31:0] SF_THIS_ID;      // 32'h01
        // Class id of owner class      
        logic [31:0] SF_CIT_ID;       // 32'h02
        // Number of arguments to method (fixed to 4)
        logic [31:0] SF_ARGS;         // 32'h03
        // Number of local variables in method (fixed to 4)
        logic [31:0] SF_LOCS;         // 32'h04
        // Link register: Address (index to an CIL record entry) of caller's next cil instruction
        logic [31:0] SF_LR;           // 32'h05   
        // Fixed args and locals to 4 + 4
        logic [31:0] SF_ARG_0;        // 32'h06
        logic [31:0] SF_ARG_1;        // 32'h07
        logic [31:0] SF_ARG_2;        // 32'h08
        logic [31:0] SF_ARG_3;        // 32'h09
        logic [31:0] SF_LOC_0;        // 32'h0A
        logic [31:0] SF_LOC_1;        // 32'h0B
        logic [31:0] SF_LOC_2;        // 32'h0C
        logic [31:0] SF_LOC_3;        // 32'h0D
        // Stack frame size (based on the above slots)
        logic [31:0] SF_SIZE;         // 32'h0E
        // stack slots
        logic [31:0] slot[0:255];
        // stack state
        logic [31:0] sfp;
        logic [31:0] nsfp;
        logic [31:0] mit_id;          // Current mit_id. Matches stack.sfp.SF_MIT_ID
        logic [31:0] pc;              // Program counter
        logic [31:0] lr;              // Link register. Matches previous stack frame (the next CIL to be executed)
    } stack_t;  
    
    stack_t stack;
 
    ///////////////////////////////////////////////////////////////////////////
    //  COMPILE-TIME INFORMATION TABLES (e.g. mem.sv)
    ///////////////////////////////////////////////////////////////////////////
    
    struct {
        // Class information table 
        logic [31:0] cit [0:127];       // Class information table
        logic [31:0] CIT_ID;            // Class id
        logic [31:0] CIT_NUMFIELDS;     // Number of fields
        logic [31:0] CIT_NUMMETHODS;    // Number of metods
        logic [31:0] CIT_SIZE;          // Number of entries
        
        // Class field information table
        logic [31:0] fit [0:127];       // Field information table
        logic [31:0] FIT_ID;            // Field id
        logic [31:0] FIT_FIELDTYPE;     // Field type
        logic [31:0] FIT_SIZE;          // Number of FIT entries
        
        // Method information table
        logic [31:0] mit [0:127];       // Method information table
        logic [31:0] MIT_ID;            // Method id
        logic [31:0] MIT_CIT_ID;        // Foreign key to CIT_ID (the class owning the method)
        logic [31:0] MIT_NUMARGS;       // Number of arguments
        logic [31:0] MIT_NUMLOCALS;     // Number of locals
        logic [31:0] MIT_MAXEVALSTACK;  // Max. eval. stack
        logic [31:0] MIT_NUMCIL;        // Num. CIL of instruc.
        logic [31:0] MIT_FIRSTCIL;      // First CIL record index for this particular method
        logic [31:0] MIT_SIZE;          // Size of one method entry record
        logic [8*20:1] mit_names [0:63];// class:method names concatinated (debug help)
        
        // CIT_MIT_CIL: Class Method CIL Information Table
        logic [31:0] cil [0:127];       // cil instructions information table
        logic [31:0] CIL_ID;            // CIL id (per method)
        logic [31:0] CIL_OPCODE;        // CIL opcode
        logic [31:0] CIL_OPERAND;       // CIL opcode
        logic [31:0] CIL_SIZE;          // Size of one cil entry record
        logic [8*20:1] cil_names [0:65535];// cil opnames (debug help)
    } tables;
    
    ///////////////////////////////////////////////////////////////////////////
    // RUNTIME STRUCTS
    ///////////////////////////////////////////////////////////////////////////
           
    // heap: slots, heap frame pointer, and combined static frame info/instance frame info.
    struct {
        // Set to 0 for static heap frames
        logic [31:0] HF_THIS_ID;//  = 32'h00;
        // Heap frame record definition
        logic [31:0] HF_CIT_ID;//   = 32'h01;
        // Fixed static or instance fields to 4
        logic [31:0] HF_FLD_0;//    = 32'h02; 
        logic [31:0] HF_FLD_1;//    = 32'h03; 
        logic [31:0] HF_FLD_2;//    = 32'h04; 
        logic [31:0] HF_FLD_3;//    = 32'h05; 
        logic [31:0] HF_SIZE;//     = 32'h06;
        // heap slots
        logic [31:0] slot[0:127];
        // heap state
        logic [31:0] hfp;
        logic [31:0] nhfp;
        logic [31:0] cit_id; // Current cit_id. Matches heap.hfp.HF_CIT_ID and stack.sfp.SF_CIT_ID
        logic [31:0] this_id;   // Current this.   Matches heap.hfp.HF_THIS and stack.sfp.SF_THIS  
    } heap;

    // evalstack: slots and eval stack pointer
    struct {
        // evaluation stack
        logic [31:0] slot[0:127];
        // eval stack state
        logic [31:0] esp; // Points to top of the evaluation stack
    } evalstack;

    //==32-bit TEXT constants==
    struct {
        logic [31:0] THIS_74_68_69_73;//          = 32'h74_68_69_73;           
        logic [31:0] NSF_6E_73_66_5F; //          = 32'h6E_73_66_5F;      
    } text;
    
    initial begin
      text.THIS_74_68_69_73 <= 32'h74_68_69_73;
      text.NSF_6E_73_66_5F  <= 32'h6E_73_66_5F;
    end
    
    ///////////////////////////////////////////////////////////////////////////
    // EXECUTION 
    ///////////////////////////////////////////////////////////////////////////
    
    // main loop boot sequencer
    typedef enum logic [1:0] {BOOTRESET, BOOTREADY, BOOTSET, BOOTGO} bootState_t;
    bootState_t current_boot_state, next_boot_state;
    always_comb begin
        case (current_boot_state)
            BOOTRESET : next_boot_state = BOOTREADY;
            BOOTREADY : next_boot_state = BOOTSET;
            BOOTSET   : next_boot_state = BOOTGO;
            BOOTGO    : next_boot_state = BOOTGO;
        endcase
    end
    
    always_ff @(posedge clk) begin
        $display("////////////////////////////////////////////////////////////////////////////////");
        $display("%0d is current time in main loop", $time);
        $display("////////////////////////////////////////////////////////////////////////////////");
        if (reset) 
            begin
                $display("Reset()");
                current_boot_state <=#1 BOOTRESET;
            end
        else if (enable)
            begin
                case (current_boot_state)
                   BOOTRESET : preInitOnReset();
                   BOOTREADY : initOnReset();
                   BOOTSET   : load(); 
                   BOOTGO    : cilSwitch(); 
                endcase
                current_boot_state <=#1 next_boot_state; // next bootstage
            end 
    end
    
    // Called from main loop
    // Using: stack.pc
    task cilSwitch();
        begin
            $display("cilSwitch():");
            $display("Decoding: pc=0x%02x", stack.pc);
             
            // Take the opcode from current instruction
            unique case (tables.cil[stack.pc + 32'h1])
                //==BASIC TASKS==                     
                NOPx00           : NOPx00_task();
                BREAKx01         : BREAKx01_task();
                LDARG0x02        : LDARG0x02_task();
                LDARG1x03        : LDARG1x03_task();
                LDARG2x04        : LDARG2x04_task();
                LDARG3x05        : LDARG3x05_task();
                LDLOC0x06        : LDLOC0x06_task();
                LDLOC1x07        : LDLOC1x07_task();
                LDLOC2x08        : LDLOC2x08_task();
                LDLOC3x09        : LDLOC3x09_task();
                STLOC0x0A        : STLOC0x0A_task();
                STLOC1x0B        : STLOC1x0B_task();
                STLOC2x0C        : STLOC2x0C_task();
                STLOC3x0D        : STLOC3x0D_task();
                STARGSx10        : STARGSx10_task(tables.cil[stack.pc + 8'h2]);
                LDLOCSx11        : LDLOCSx11_task(tables.cil[stack.pc + 8'h2]);
                LDLOCASx12       : LDLOCASx12_task(tables.cil[stack.pc + 8'h2]);
                STLOCSx13        : STLOCSx13_task(tables.cil[stack.pc + 8'h2]);
                LDNULLx14        : LDNULLx14_task();
                LDCI4M1x15       : LDCI4M1x15_task();
                LDCI40x16        : LDCI40x16_task();
                LDCI41x17        : LDCI41x17_task();
                LDCI42x18        : LDCI42x18_task();
                LDCI43x19        : LDCI43x19_task();
                LDCI44x1A        : LDCI44x1A_task();
                LDCI45x1B        : LDCI45x1B_task();
                LDCI46x1C        : LDCI46x1C_task();
                LDCI47x1D        : LDCI47x1D_task();
                LDCI48x1E        : LDCI48x1E_task();
                LDCI44Sx1F       : LDCI44Sx1F_task(tables.cil[stack.pc + 8'h2]);
                LDARGSx0E        : LDARGSx0E_task(tables.cil[stack.pc + 8'h2]);
                LDARGASx0F       : LDARGASx0F_task(tables.cil[stack.pc + 8'h2]);
                LDCI4x20         : LDCI4x20_task(tables.cil[stack.pc + 8'h2]); 
                LDCI8x21         : LDCI8x21_task(tables.cil[stack.pc + 8'h2]);
                LDCR4x22         : LDCR4x22_task(tables.cil[stack.pc + 8'h2]);
                LDCR8x23         : LDCR8x23_task(tables.cil[stack.pc + 8'h2]);
                DUPx25           : DUPx25_task();
                POPx26           : POPx26_task();
                JMPx27           : JMPx27_task(tables.cil[stack.pc + 8'h2]);
                CALLx28          : CALLx28_task(tables.cil[stack.pc + 8'h2]);
                CALLIx29         : CALLIx29_task(tables.cil[stack.pc + 8'h2]);
                RETx2A           : RETx2A_task();
                BRSx2B           : BRSx2B_task(tables.cil[stack.pc + 8'h2]);
                BRFALSESx2C      : BRFALSESx2C_task(tables.cil[stack.pc + 8'h2]);
                BRTRUESx2D       : BRTRUESx2D_task(tables.cil[stack.pc + 8'h2]);
                BEQSx2E          : BEQSx2E_task(tables.cil[stack.pc + 8'h2]);
                BGESx2F          : BGESx2F_task(tables.cil[stack.pc + 8'h2]);
                BGTSx30          : BGTSx30_task(tables.cil[stack.pc + 8'h2]);
                BLESx31          : BLESx31_task(tables.cil[stack.pc + 8'h2]);
                BLTSx32          : BLTSx32_task(tables.cil[stack.pc + 8'h2]);
                BNEUNSx33        : BNEUNSx33_task(tables.cil[stack.pc + 8'h2]);
                BGEUNSx34        : BGEUNSx34_task(tables.cil[stack.pc + 8'h2]);
                BGTUNSx35        : BGTUNSx35_task(tables.cil[stack.pc + 8'h2]);
                BLEUNSx36        : BLEUNSx36_task(tables.cil[stack.pc + 8'h2]);
                BLTUNSx37        : BLTUNSx37_task(tables.cil[stack.pc + 8'h2]);
                BRx38            : BRx38_task(tables.cil[stack.pc + 8'h2]); 
                BRFALSEx39       : BRFALSEx39_task(tables.cil[stack.pc + 8'h2]);
                BRTRUEx3A        : BRTRUEx3A_task(tables.cil[stack.pc + 8'h2]);
                //BRINSTx3A      : BRINSTx3A_task(cit_mit_cil[pc + 8'h2]);
                BEQx3B           : BEQx3B_task(tables.cil[stack.pc + 8'h2]);
                BGEx3C           : BGEx3C_task(tables.cil[stack.pc + 8'h2]);
                //BGTx3D           : BGTx3D_task(cit_mit_cil[pc+8'h2]);
                BLEx3E           : BLEx3E_task(tables.cil[stack.pc + 8'h2]);
                BLTx3F           : BLTx3F_task(tables.cil[stack.pc + 8'h2]);
                BNEUNx40         : BNEUNx40_task(tables.cil[stack.pc + 8'h2]);
                BGEUNx41         : BGEUNx41_task(tables.cil[stack.pc + 8'h2]);
                BGTUNx42         : BGTUNx42_task(tables.cil[stack.pc + 8'h2]);
                BLEUNx43         : BLEUNx43_task(tables.cil[stack.pc + 8'h2]);
                BLTUNx44         : BLTUNx44_task(tables.cil[stack.pc + 8'h2]);
                SWITCHx45        : SWITCHx45_task();
                LDINDI1x46       : LDINDI1x46_task(tables.cil[stack.pc + 8'h2]);
                LDINDU1x47       : LDINDU1x47_task(tables.cil[stack.pc + 8'h2]);
                LDINDI2x48       : LDINDI2x48_task(tables.cil[stack.pc + 8'h2]);
                LDINDU2x49       : LDINDU2x49_task(tables.cil[stack.pc + 8'h2]);
                LDINDI4x4A       : LDINDI4x4A_task(tables.cil[stack.pc + 8'h2]);
                LDINDU4x4B       : LDINDU4x4B_task(tables.cil[stack.pc + 8'h2]);
                LDINDI8x4C       : LDINDI8x4C_task(tables.cil[stack.pc + 8'h2]);
                //LDINDU8x4C     : LDINDU8x4C_task(cit_mit_cil[pc + 8'h2]);
                LDINDIx4D        : LDINDIx4D_task(tables.cil[stack.pc + 8'h2]);
                LDINDR4x4E       : LDINDR4x4E_task(tables.cil[stack.pc + 8'h2]);
                LDINDR8x4F       : LDINDR8x4F_task(tables.cil[stack.pc + 8'h2]);
                LDINDREFx50      : LDINDREFx50_task(tables.cil[stack.pc + 8'h2]);
                STINDREFx51      : STINDREFx51_task(tables.cil[stack.pc + 8'h2]);
                STNDI1x52        : STNDI1x52_task(tables.cil[stack.pc + 8'h2]);
                STINDI2x53       : STINDI2x53_task(tables.cil[stack.pc + 8'h2]);
                STINDI4x54       : STINDI4x54_task(tables.cil[stack.pc + 8'h2]);
                STINDI8x55       : STINDI8x55_task(tables.cil[stack.pc + 8'h2]);
                STINDR4x56       : STINDR4x56_task(tables.cil[stack.pc + 8'h2]);
                STINDR8x57       : STINDR8x57_task(tables.cil[stack.pc + 8'h2]);
                ADDx58           : ADDx58_task(1'b0);
                SUBx59           : SUBx59_task();
                MULx5A           : MULx5A_task();
                DIVx5B           : DIVx5B_task();
                DIVUNx5C         : DIVUNx5C_task();
                REMx5D           : REMx5D_task();
                REMUNx5E         : REMUNx5E_task();
                ANDx5F           : ANDx5F_task();
                ORx60            : ORx60_task();
                XORx61           : XORx61_task();
                SHLx62           : SHLx62_task();
                SHRx63           : SHRx63_task();
                SHRUNx64         : SHRUNx64_task();
                NEGx65           : NEGx65_task();
                NOTx66           : NOTx66_task();
                CONVI1x67        : CONVI1x67_task();
                CONVI2x68        : CONVI2x68_task();
                CONVI4x69        : CONVI4x69_task();
                CONVI8x6A        : CONVI8x6A_task();
                CONVR4x6B        : CONVR4x6B_task();
                CONVR8x6C        : CONVR8x6C_task();
                CONVU4x6D        : CONVU4x6D_task();
                CONVU8x6E        : CONVU8x6E_task();
                CONVRUNx76       : CONVRUNx76_task(); 
                LDFLDx7B         : LDFLDx7B_task(tables.cil[stack.pc + 8'h2]); // field index/token
                CONVOVFI1UNx82   : CONVOVFI1UNx82_task();
                CONVOVFI2UNx83   : CONVOVFI2UNx83_task();
                CONVOVFI4UNx84   : CONVOVFI4UNx84_task();
                CONVOVFI8UNx85   : CONVOVFI8UNx85_task();
                CONVOVFU1UNx86   : CONVOVFU1UNx86_task();
                CONVOVFU2UNx87   : CONVOVFU2UNx87_task();
                CONVOVFU4UNx88   : CONVOVFU4UNx88_task();
                CONVOVFU8UNx89   : CONVOVFU8UNx89_task();
                CONVOVFIUNx8A    : CONVOVFIUNx8A_task();
                CONVOVFUUNx8B    : CONVOVFUUNx8B_task();
                CONVOVFI1xB3     : CONVOVFI1xB3_task();
                CONVOVFU1xB4     : CONVOVFU1xB4_task();
                CONVOVFI2xB5     : CONVOVFI2xB5_task();
                CONVOVFU2xB6     : CONVOVFU2xB6_task();
                CONVOVFI4xB7     : CONVOVFI4xB7_task();
                CONVOVFI8xB9     : CONVOVFI8xB9_task();
                CONVOVFI2xBA     : CONVOVFI2xBA_task();
                CKFINITExC3      : CKFINITExC3_task();
                CONVU2xD1        : CONVU2xD1_task();
                CONVU1xD2        : CONVU1xD2_task();
                CONVIxD3         : CONVIxD3_task();
                CONVOVFUxD5      : CONVOVFUxD5_task();
                ADDOVFxD6        : ADDOVFxD6_task(tables.cil[stack.pc + 8'h2]);
                ADDOVFUNxD7      : ADDOVFUNxD7_task(tables.cil[stack.pc + 8'h2]);
                MULOVFxD8        : MULOVFxD8_task();
                MULOVFUNxD9      : MULOVFUNxD9_task();
                SUBOVFxDA        : SUBOVFxDA_task();
                SUBOVFUNxDB      : SUBOVFUNxDB_task();
                ENDFAULTxDC      : ENDFAULTxDC_task();
                //ENDFINALLYxDC    : ENDFINALLYxDC_task();
                LEAVExDD         : LEAVExDD_task(tables.cil[stack.pc + 8'h2]);
                LEAVESxDE        : LEAVESxDE_task(tables.cil[stack.pc + 8'h2]);
                STINDIxDF        : STINDIxDF_task();
                CONVUxE0         : CONVUxE0_task();
                CEQxFE01         : CEQxFE01_task();
                ARGLISTxFE00     : ARGLISTxFE00_task();
                CGTxFE02         : CGTxFE02_task();
                CGTUNxFE03       : CGTUNxFE03_task();
                CLTxFE04         : CLTxFE04_task();
                CLTUNxFE05       : CLTUNxFE05_task();
                LDFTNxFE06       : LDFTNxFE06_task();
                LDARGxFE09       : LDARGxFE09_task(tables.cil[stack.pc + 8'h2]);
                LDARGAxFE0A      : LDARGAxFE0A_task(tables.cil[stack.pc + 8'h2]);
                STARGxFE0B       : STARGxFE0B_task(tables.cil[stack.pc + 8'h2]);
                ENDFILTERxFE11   : ENDFILTERxFE11_task();
                CPBLKxFE17       : CPBLKxFE17_task();
                INITBLKxFE18     : INITBLKxFE18_task();
                LDLOCxFE0C       : LDLOCxFE0C_task(tables.cil[stack.pc + 8'h2]);
                LDLOCAxFE0D      : LDLOCAxFE0D_task(tables.cil[stack.pc + 8'h2]);
                STLOCxFE0E       : STLOCxFE0E_task(tables.cil[stack.pc + 8'h2]);
                LOCALLOCxFE0F    : LOCALLOCxFE0F_task();
                //==OBJECT TASKS==
                CALLVIRTx6F      : CALLVIRTx6F_task(tables.cil[stack.pc + 8'h2]);
                CPOBJx70         : CPOBJx70_task();
                LDOBJx71         : LDOBJx71_task();
                LDSTRx72         : LDSTRx72_task();
                NEWOBJx73        : NEWOBJx73_task(tables.cil[stack.pc + 8'h2]); // ctor token: methodref or methoddef
                CASTCLASSx74     : CASTCLASSx74_task();
                LDSFLDx7E        : LDSFLDx7E_task(tables.cil[stack.pc + 8'h2]);
                LDSFLDAx7F       : LDSFLDAx7F_task(tables.cil[stack.pc + 8'h2]);
                BOXx8C           : BOXx8C_task();
                ISINSTx75        : ISINSTx75_task(tables.cil[stack.pc + 8'h2]);
                UNBOXx79         : UNBOXx79_task();
                THROWx7A         : THROWx7A_task();
                //LDFLDx7B       : LDFLDx7B_task();
                LDFLDAx7C        : LDFLDAx7C_task(tables.cil[stack.pc + 8'h2]);
                STFLDx7D         : STFLDx7D_task(tables.cil[stack.pc + 8'h2]); // field index/token
                STSFLDx80        : STSFLDx80_task(tables.cil[stack.pc + 8'h2]);
                STOBJx81         : STOBJx81_task(tables.cil[stack.pc + 8'h2]);
                NEWARRx8D        : NEWARRx8D_task(tables.cil[stack.pc + 8'h2]);
                LDLENx8E         : LDLENx8E_task();
                //LDELEMAx8F     : LDELEMAx8F_task(cil[stack.pc + 8'h2]);
                LDELEMI1x90      : LDELEMI1x90_task();
                LDELEMU1x91      : LDELEMU1x91_task();
                LDELEMI2x92      : LDELEMI2x92_task();
                LDELEMU2x93      : LDELEMU2x93_task();
                LDELEMI4x94      : LDELEMI4x94_task();
                LDELEMU4x95      : LDELEMU4x95_task();
                LDELEMI8x96      : LDELEMI8x96_task();
                //LDELEMU8x96    : LDELEMU8x96_task();
                LDELEMIx97       : LDELEMIx97_task();
                LDELEMR4x98      : LDELEMR4x98_task();
                LDELEMR8x99      : LDELEMR8x99_task();
                LDELEMREFx9A     : LDELEMREFx9A_task();
                STELEMIx9B       : STELEMIx9B_task();
                STELEMI1x9C      : STELEMI1x9C_task();
                STELEMI2x9D      : STELEMI2x9D_task();
                STELEMI4x9E      : STELEMI4x9E_task();
                STELEMR4xA0      : STELEMR4xA0_task();
                STELEMR8xA1      : STELEMR8xA1_task();
                STELEMREFxA2     : STELEMREFxA2_task();
                LDELEMxA3        : LDELEMxA3_task();
                STELEMxA4        : STELEMxA4_task();
                UNBOXANYxA5      : UNBOXANYxA5_task();
                REFANYVALxC2     : REFANYVALxC2_task(tables.cil[stack.pc + 8'h2]);
                MKREFANYxC6      : MKREFANYxC6_task(tables.cil[stack.pc + 8'h2]);
                LDTOKENxD0       : LDTOKENxD0_task(tables.cil[stack.pc + 8'h2]);
                LDVIRTFNxFE07    : LDVIRTFNxFE07_task(tables.cil[stack.pc + 8'h2]);
                INITOBJxFE15     : INITOBJxFE15_task();
                RETHROWxFE1A     : RETHROWxFE1A_task();
                SIZEOFxFE1C      : SIZEOFxFE1C_task(tables.cil[stack.pc + 8'h2]);
                REFANYTYPExFE1D  : REFANYTYPExFE1D_task();                     
                default ;//decodeError();
             endcase    
        end
    endtask

    task preInitOnReset();
       begin
           tables.cil_names[16'h0000] <= "NOPx00";
           tables.cil_names[16'h0001] <= "BREAKx01";
           tables.cil_names[16'h0002] <= "LDARG0x02";
           tables.cil_names[16'h0003] <= "LDARG1x03";
           tables.cil_names[16'h0004] <= "LDARG2x04";
           tables.cil_names[16'h0005] <= "LDARG3x05";
           tables.cil_names[16'h0006] <= "LDLOC0x06";
           tables.cil_names[16'h0007] <= "LDLOC1x07";
           tables.cil_names[16'h0008] <= "LDLOC2x08";
           tables.cil_names[16'h0009] <= "LDLOC3x09";
           tables.cil_names[16'h000A] <= "STLOC0x0A";
           tables.cil_names[16'h000B] <= "STLOC1x0B";
           tables.cil_names[16'h000C] <= "STLOC2x0C";
           tables.cil_names[16'h000D] <= "STLOC3x0D";
           tables.cil_names[16'h0010] <= "STARGSx10";
           tables.cil_names[16'h0011] <= "LDLOCSx11";
           tables.cil_names[16'h0012] <= "LDLOCASx12";
           tables.cil_names[16'h0013] <= "STLOCSx13";
           tables.cil_names[16'h0014] <= "LDNULLx14";
           tables.cil_names[16'h0015] <= "LDCI4M1x15";
           tables.cil_names[16'h0016] <= "LDCI40x16";
           tables.cil_names[16'h0017] <= "LDCI41x17";
           tables.cil_names[16'h0018] <= "LDCI42x18";
           tables.cil_names[16'h0019] <= "LDCI43x19";
           tables.cil_names[16'h001A] <= "LDCI44x1A";
           tables.cil_names[16'h001B] <= "LDCI45x1B";
           tables.cil_names[16'h001C] <= "LDCI46x1C";
           tables.cil_names[16'h001D] <= "LDCI47x1D";
           tables.cil_names[16'h001E] <= "LDCI48x1E";
           tables.cil_names[16'h001F] <= "LDCI44Sx1F";
           tables.cil_names[16'h000E] <= "LDARGSx0E";
           tables.cil_names[16'h000F] <= "LDARGASx0F";
           tables.cil_names[16'h0020] <= "LDCI4x20";
           tables.cil_names[16'h0021] <= "LDCI8x21";
           tables.cil_names[16'h0022] <= "LDCR4x22";
           tables.cil_names[16'h0023] <= "LDCR8x23";
           tables.cil_names[16'h0025] <= "DUPx25";
           tables.cil_names[16'h0026] <= "POPx26";
           tables.cil_names[16'h0027] <= "JMPx27";
           tables.cil_names[16'h0028] <= "CALLx28";
           tables.cil_names[16'h0029] <= "CALLIx29";
           tables.cil_names[16'h002A] <= "RETx2A";
           tables.cil_names[16'h002B] <= "BRSx2B";
           tables.cil_names[16'h002C] <= "BRFALSESx2C";
           tables.cil_names[16'h002D] <= "BRTRUESx2D";
           tables.cil_names[16'h002E] <= "BEQSx2E";
           tables.cil_names[16'h002F] <= "BGESx2F";
           tables.cil_names[16'h0030] <= "BGTSx30";
           tables.cil_names[16'h0031] <= "BLESx31";
           tables.cil_names[16'h0032] <= "BLTSx32";
           tables.cil_names[16'h0033] <= "BNEUNSx33";
           tables.cil_names[16'h0034] <= "BGEUNSx34";
           tables.cil_names[16'h0035] <= "BGTUNSx35";
           tables.cil_names[16'h0036] <= "BLEUNSx36";
           tables.cil_names[16'h0037] <= "BLTUNSx37";
           tables.cil_names[16'h0038] <= "BRx38";
           tables.cil_names[16'h0039] <= "BRFALSEx39";
           tables.cil_names[16'h003A] <= "BRTRUEx3A";
           tables.cil_names[16'h003A] <= "BRINSTx3A";
           tables.cil_names[16'h003B] <= "BEQx3B";
           tables.cil_names[16'h003C] <= "BGEx3C";
           tables.cil_names[16'h002E] <= "BGTx3D";
           tables.cil_names[16'h003E] <= "BLEx3E";
           tables.cil_names[16'h003F] <= "BLTx3F";
           tables.cil_names[16'h0040] <= "BNEUNx40";
           tables.cil_names[16'h0041] <= "BGEUNx41";
           tables.cil_names[16'h0042] <= "BGTUNx42";
           tables.cil_names[16'h0043] <= "BLEUNx43";
           tables.cil_names[16'h0044] <= "BLTUNx44";
           tables.cil_names[16'h0045] <= "SWITCHx45";
           tables.cil_names[16'h0046] <= "LDINDI1x46";
           tables.cil_names[16'h0047] <= "LDINDU1x47";
           tables.cil_names[16'h0048] <= "LDINDI2x48";
           tables.cil_names[16'h0049] <= "LDINDU2x49";
           tables.cil_names[16'h004A] <= "LDINDI4x4A";
           tables.cil_names[16'h004B] <= "LDINDU4x4B";
           tables.cil_names[16'h004C] <= "LDINDI8x4C";
           tables.cil_names[16'h004C] <= "LDINDU8x4C";
           tables.cil_names[16'h004D] <= "LDINDIx4D";
           tables.cil_names[16'h004E] <= "LDINDR4x4E";
           tables.cil_names[16'h004F] <= "LDINDR8x4F";
           tables.cil_names[16'h0050] <= "LDINDREFx50";
           tables.cil_names[16'h0051] <= "STINDREFx51";
           tables.cil_names[16'h0052] <= "STNDI1x52";
           tables.cil_names[16'h0053] <= "STINDI2x53";
           tables.cil_names[16'h0054] <= "STINDI4x54";
           tables.cil_names[16'h0055] <= "STINDI8x55";
           tables.cil_names[16'h0056] <= "STINDR4x56";
           tables.cil_names[16'h0057] <= "STINDR8x57";
           tables.cil_names[16'h0058] <= "ADDx58";
           tables.cil_names[16'h0059] <= "SUBx59";
           tables.cil_names[16'h005A] <= "MULx5A";
           tables.cil_names[16'h005B] <= "DIVx5B";
           tables.cil_names[16'h005C] <= "DIVUNx5C";
           tables.cil_names[16'h005D] <= "REMx5D";
           tables.cil_names[16'h005E] <= "REMUNx5E";
           tables.cil_names[16'h005F] <= "ANDx5F";
           tables.cil_names[16'h0060] <= "ORx60";
           tables.cil_names[16'h0061] <= "XORx61";
           tables.cil_names[16'h0062] <= "SHLx62";
           tables.cil_names[16'h0063] <= "SHRx63";
           tables.cil_names[16'h0064] <= "SHRUNx64";
           tables.cil_names[16'h0065] <= "NEGx65";
           tables.cil_names[16'h0066] <= "NOTx66";
           tables.cil_names[16'h0067] <= "CONVI1x67";
           tables.cil_names[16'h0068] <= "CONVI2x68";
           tables.cil_names[16'h0069] <= "CONVI4x69";
           tables.cil_names[16'h002A] <= "RETx2A";
           tables.cil_names[16'h006A] <= "CONVI8x6A";
           tables.cil_names[16'h006B] <= "CONVR4x6B";
           tables.cil_names[16'h006C] <= "CONVR8x6C";
           tables.cil_names[16'h006D] <= "CONVU4x6D";
           tables.cil_names[16'h006E] <= "CONVU8x6E";
           tables.cil_names[16'h0076] <= "CONVRUNx76";
           tables.cil_names[16'h007B] <= "LDFLDx7B";
           tables.cil_names[16'h0082] <= "CONVOVFI1UNx82";
           tables.cil_names[16'h0083] <= "CONVOVFI2UNx83";
           tables.cil_names[16'h0084] <= "CONVOVFI4UNx84";
           tables.cil_names[16'h0085] <= "CONVOVFI8UNx85";
           tables.cil_names[16'h0086] <= "CONVOVFU1UNx86";
           tables.cil_names[16'h0087] <= "CONVOVFU2UNx87";
           tables.cil_names[16'h0088] <= "CONVOVFU4UNx88";
           tables.cil_names[16'h0089] <= "CONVOVFU8UNx89";
           tables.cil_names[16'h008A] <= "CONVOVFIUNx8A";
           tables.cil_names[16'h008B] <= "CONVOVFUUNx8B";
           tables.cil_names[16'h00B3] <= "CONVOVFI1xB3";
           tables.cil_names[16'h00B4] <= "CONVOVFU1xB4";
           tables.cil_names[16'h00B5] <= "CONVOVFI2xB5";
           tables.cil_names[16'h00B6] <= "CONVOVFU2xB6";
           tables.cil_names[16'h00B7] <= "CONVOVFI4xB7";
           tables.cil_names[16'h00B9] <= "CONVOVFI8xB9";
           tables.cil_names[16'h00BA] <= "CONVOVFI2xBA";
           tables.cil_names[16'h00C3] <= "CKFINITExC3";
           tables.cil_names[16'h00D1] <= "CONVU2xD1";
           tables.cil_names[16'h00D2] <= "CONVU1xD2";
           tables.cil_names[16'h00D3] <= "CONVIxD3";
           tables.cil_names[16'h00D5] <= "CONVOVFUxD5";
           tables.cil_names[16'h00D6] <= "ADDOVFxD6";
           tables.cil_names[16'h00D7] <= "ADDOVFUNxD7";
           tables.cil_names[16'h00D8] <= "MULOVFxD8";
           tables.cil_names[16'h00D9] <= "MULOVFUNxD9";
           tables.cil_names[16'h00DA] <= "SUBOVFxDA";
           tables.cil_names[16'h00DB] <= "SUBOVFUNxDB";
           tables.cil_names[16'h00DC] <= "ENDFAULTxDC";
           tables.cil_names[16'h00DC] <= "ENDFINALLYxDC";
           tables.cil_names[16'h00DD] <= "LEAVExDD";
           tables.cil_names[16'h00DE] <= "LEAVESxDE";
           tables.cil_names[16'h00DF] <= "STINDIxDF";
           tables.cil_names[16'h00E0] <= "CONVUxE0";
           tables.cil_names[16'hFE01] <= "CEQxFE01";
           tables.cil_names[16'hFE00] <= "ARGLISTxFE00";
           tables.cil_names[16'hFE02] <= "CGTxFE02";
           tables.cil_names[16'hFE03] <= "CGTUNxFE03";
           tables.cil_names[16'hFE04] <= "CLTxFE04";
           tables.cil_names[16'hFE05] <= "CLTUNxFE05";
           tables.cil_names[16'hFE06] <= "LDFTNxFE06";
           tables.cil_names[16'hFE09] <= "LDARGxFE09";
           tables.cil_names[16'hFE0A] <= "LDARGAxFE0A";
           tables.cil_names[16'hFE0B] <= "STARGxFE0B";
           tables.cil_names[16'hFE11] <= "ENDFILTERxFE11";
           tables.cil_names[16'hFE17] <= "CPBLKxFE17";
           tables.cil_names[16'hFE18] <= "INITBLKxFE18";
           tables.cil_names[16'hFE0C] <= "LDLOCxFE0C";
           tables.cil_names[16'hFE0D] <= "LDLOCAxFE0D";
           tables.cil_names[16'hFE0E] <= "STLOCxFE0E";
           tables.cil_names[16'hFE0F] <= "LOCALLOCxFE0F";
           tables.cil_names[16'h006F] <= "CALLVIRTx6F";
           tables.cil_names[16'h0070] <= "CPOBJx70";
           tables.cil_names[16'h0071] <= "LDOBJx71";
           tables.cil_names[16'h0072] <= "LDSTRx72";
           tables.cil_names[16'h0073] <= "NEWOBJx73";
           tables.cil_names[16'h0074] <= "CASTCLASSx74";
           tables.cil_names[16'h007E] <= "LDSFLDx7E";
           tables.cil_names[16'h007F] <= "LDSFLDAx7F";
           tables.cil_names[16'h008C] <= "BOXx8C";
           tables.cil_names[16'h0075] <= "ISINSTx75";
           tables.cil_names[16'h0079] <= "UNBOXx79";
           tables.cil_names[16'h007A] <= "THROWx7A";
           tables.cil_names[16'h007B] <= "LDFLDx7B";
           tables.cil_names[16'h007C] <= "LDFLDAx7C";
           tables.cil_names[16'h007D] <= "STFLDx7D";
           tables.cil_names[16'h0080] <= "STSFLDx80";
           tables.cil_names[16'h0081] <= "STOBJx81";
           tables.cil_names[16'h008D] <= "NEWARRx8D";
           tables.cil_names[16'h008E] <= "LDLENx8E";
           tables.cil_names[16'h008F] <= "LDELEMAx8F";
           tables.cil_names[16'h0090] <= "LDELEMI1x90";
           tables.cil_names[16'h0091] <= "LDELEMU1x91";
           tables.cil_names[16'h0092] <= "LDELEMI2x92";
           tables.cil_names[16'h0093] <= "LDELEMU2x93";
           tables.cil_names[16'h0094] <= "LDELEMI4x94";
           tables.cil_names[16'h0095] <= "LDELEMU4x95";
           tables.cil_names[16'h0096] <= "LDELEMI8x96";
           tables.cil_names[16'h0096] <= "LDELEMU8x96";
           tables.cil_names[16'h0097] <= "LDELEMIx97";
           tables.cil_names[16'h0098] <= "LDELEMR4x98";
           tables.cil_names[16'h0099] <= "LDELEMR8x99";
           tables.cil_names[16'h009A] <= "LDELEMREFx9A";
           tables.cil_names[16'h009B] <= "STELEMIx9B";
           tables.cil_names[16'h009C] <= "STELEMI1x9C";
           tables.cil_names[16'h009D] <= "STELEMI2x9D";
           tables.cil_names[16'h009E] <= "STELEMI4x9E";
           tables.cil_names[16'h00A0] <= "STELEMR4xA0";
           tables.cil_names[16'h00A1] <= "STELEMR8xA1";
           tables.cil_names[16'h00A2] <= "STELEMREFxA2";
           tables.cil_names[16'h00A3] <= "LDELEMxA3";
           tables.cil_names[16'h00A4] <= "STELEMxA4";
           tables.cil_names[16'h00A5] <= "UNBOXANYxA5";
           tables.cil_names[16'h00C2] <= "REFANYVALxC2";
           tables.cil_names[16'h00C6] <= "MKREFANYxC6";
           tables.cil_names[16'h00D0] <= "LDTOKENxD0";
           tables.cil_names[16'hFE07] <= "LDVIRTFNxFE07"; // FE07
           tables.cil_names[16'hFE15] <= "INITOBJxFE15"; // FE15
           tables.cil_names[16'hFE1A] <= "RETHROWxFE1A"; // FE1A
           tables.cil_names[16'hFE1C] <= "SIZEOFxFE1C"; // FE1C
           tables.cil_names[16'hFE1D] <= "REFANYTYPExFE1D"; // FE1D

           // TABLE HELPER FIELDS from 'tables' struct
           // Class information table 
           tables.CIT_ID           <= 32'h00; // Class id
           tables.CIT_NUMFIELDS    <= 32'h01; // Number of fields
           tables.CIT_NUMMETHODS   <= 32'h02; // Number of metods
           tables.CIT_SIZE         <= 32'h03; // Number of entries
           // Class field information table
           tables.FIT_ID           <= 32'h00; // Field id
           tables.FIT_FIELDTYPE    <= 32'h01; // Field type
           tables.FIT_SIZE         <= 32'h02; // Number of FIT entries
           // Method information table
           tables.MIT_ID           <= 32'h00; // Method id
           tables.MIT_CIT_ID       <= 32'h01; // Foreign key to CIT_ID (the class owning the method)
           tables.MIT_NUMARGS      <= 32'h02; // Number of arguments
           tables.MIT_NUMLOCALS    <= 32'h03; // Number of locals
           tables.MIT_MAXEVALSTACK <= 32'h04; // Max. eval. stack
           tables.MIT_NUMCIL       <= 32'h05; // Num. CIL of instruc.
           tables.MIT_FIRSTCIL     <= 32'h06; // First CIL record index for this particular method
           tables.MIT_SIZE         <= 32'h07; // Size of one method entry record
           // CIT_MIT_CIL: Class Method CIL Information Table
           tables.CIL_ID           <= 32'h00; // CIL id (per method)
           tables.CIL_OPCODE       <= 32'h01; // CIL opcode
           tables.CIL_OPERAND      <= 32'h02; // CIL opcode
           tables.CIL_SIZE         <= 32'h03; // Size of one cil entry record
       end
    endtask
    
    // INITIALIZATION
    task initOnReset();
        begin
            $display("initOnReset()");
            // Program state: pc init (points to first CIL instruction in cit_mit_cil)
            stack.pc     <=#1 32'h0;
            // The instruction after stack.pc for the calling method
            stack.lr     <=#1 32'h0;
            // Set "current" instance to 0, but is it does not mean static
            // Just mean that we have to add one to generate first instance id
            heap.this_id <=#1 32'h0;
            
            // stack init
            stack.sfp     <=#1 32'h0;
            stack.nsfp    <=#1 32'h0;
            stack.slot[0] <=#1 32'h73_66_70_5F; //"sfp_"
            
            // evaluation stack
            evalstack.esp     <=#1 32'h0;
            evalstack.slot[0] <=#1 32'h65_73_70_5F; //"esp_"
            
            // heap init
            heap.hfp     <=#1 32'h0;
            heap.nhfp    <=#1 32'h0;
            heap.slot[0] <=#1 32'h68_70_5F_5F; //"hp__"

            // Create first (static) heap frame
            // First cit is 0 and "this" is also 0
            createHeapFrame(32'h0, 32'h0);
            
            // Create first static stack frame
            // First (static) method is main which is mit_id 0 and "this" is also still 0
            createStackFrame(32'h0, 32'h0);
        end
    endtask

    // "Read" memory files
    task load();        
        begin
            // PROGRAM LOADING    
            //`include "mem.sv" // or load directly in test
            // cit records:         [0]CIT_ID
                                 // [1]CIT_NUMFIELDS
                                 // [2]CIT_NUMMETHODS
            tables.cit[0000] <=#1 32'h00000000;      // CIT_ID. NOTE: This used to be 2, which came from the assembler.
            tables.cit[0001] <=#1 32'h00000000;      // CIT_NUMFIELDS
            tables.cit[0002] <=#1 32'h00000004;      // CIT_NUMMETHODS
            
            // --------------------------------------------------------------------------  
            // mit records are constant size
            // mit records:     [0]MIT_ID
                             // [1]MIT_CIT_ID (foreign key)
                             // [2]MIT_NUMARGS
                             // [3]MIT_NUMLOCALS
                             // [4]MIT_MAXEVALSTACK
                             // [5]MIT_NUMCIL
                             // [6]MIT_FIRSTCIL: Points to the index in the cil table for the first CIL record for this method.
            // BasicClass:main
            tables.mit[0000] <=#1 32'h00000000; // MIT_ID: Changed from 1 to 0 to give direct index multiplied with MIT_SIZE
            tables.mit[0001] <=#1 32'h00000000; // MIT_CIT_ID (foreign key: index into cit[XX * CIT_SIZE])
            tables.mit[0002] <=#1 32'h00000000; // MIT_NUMARGS
            tables.mit[0003] <=#1 32'h00000000; // MIT_NUMLOCALS
            tables.mit[0004] <=#1 32'h00000008; // MIT_MAXEVALSTACK
            tables.mit[0005] <=#1 32'h00000006; // MIT_NUMCIL
            tables.mit[0006] <=#1 32'h00000000; // MIT_FIRSTCIL
            // BasicClass:.ctor
            tables.mit[0007] <=#1 32'h00000001; // MIT_ID
            tables.mit[0008] <=#1 32'h00000000; // MIT_CIT_ID (foreign [key])
            tables.mit[0009] <=#1 32'h00000001; // MIT_NUMARGS
            tables.mit[0010] <=#1 32'h00000000; // MIT_NUMLOCALS
            tables.mit[0011] <=#1 32'h00000008; // MIT_MAXEVALSTACK
            tables.mit[0012] <=#1 32'h00000001; // MIT_NUMCIL
            tables.mit[0013] <=#1 32'h00000012; // MIT_FIRSTCIL
            // BasicClass:Method1
            tables.mit[0014] <=#1 32'h00000002; // MIT_ID
            tables.mit[0015] <=#1 32'h00000000; // MIT_CIT_ID (foreign [key])
            tables.mit[0016] <=#1 32'h00000002; // MIT_NUMARGS
            tables.mit[0017] <=#1 32'h00000002; // MIT_NUMLOCALS
            tables.mit[0018] <=#1 32'h00000008; // MIT_MAXEVALSTACK
            tables.mit[0019] <=#1 32'h00000003; // MIT_NUMCIL
            tables.mit[0020] <=#1 32'h00000015; // MIT_FIRSTCIL
            // BasicClass:Method2
            tables.mit[0021] <=#1 32'h00000003; // MIT_ID
            tables.mit[0022] <=#1 32'h00000000; // MIT_CIT_ID (foreign [key])
            tables.mit[0023] <=#1 32'h00000000; // MIT_NUMARGS
            tables.mit[0024] <=#1 32'h00000001; // MIT_NUMLOCALS
            tables.mit[0025] <=#1 32'h00000008; // MIT_MAXEVALSTACK
            tables.mit[0026] <=#1 32'h00000003; // MIT_NUMCIL
            tables.mit[0027] <= 32'h0000001E; // MIT_FIRSTCIL
            // mit names records:[0]MIT_ID_NAME
            tables.mit_names[00000000] <=#1 "noname";                   // MIT_ID_NAME
            tables.mit_names[00000001] <=#1 "BasicClass:main";          // MIT_ID_NAME
            tables.mit_names[00000002] <=#1 "BasicClass:.ctor";         // MIT_ID_NAME
            tables.mit_names[00000003] <=#1 "BasicClass:Method1";       // MIT_ID_NAME
            tables.mit_names[00000004] <=#1 "BasicClass:Method2";       // MIT_ID_NAME
            
            // --------------------------------------------------------------------------
            // cil records: [0]CIL_ID
                         // [1]CIL_OPCODE
                         // [2]CIL_OPERAND
            // BasicClass : main : rva 0x00000250
            tables.cil[0000] <=#1 32'h00000000;      // CIL_ID
            tables.cil[0001] <=#1 32'h00000020;      // CIL_OPCODE:      ldc.i4
            tables.cil[0002] <=#1 32'h00000005;      // CIL_OPERAND:     5
            // BasicClass : main : rva 0x00000250
            tables.cil[0003] <=#1 32'h00000001;      // CIL_ID
            tables.cil[0004] <=#1 32'h00000073;      // CIL_OPCODE:      newobj
            tables.cil[0005] <=#1 32'h06000002;      // CIL_OPERAND:     100663298
            // BasicClass : main : rva 0x00000250
            tables.cil[0006] <=#1 32'h00000002;      // CIL_ID
            tables.cil[0007] <=#1 32'h00000020;      // CIL_OPCODE:      ldc.i4
            tables.cil[0008] <=#1 32'h0000000A;      // CIL_OPERAND:     10
            // BasicClass : main : rva 0x00000250
            tables.cil[0009] <=#1 32'h00000003;      // IL_ID
            tables.cil[0010] <=#1 32'h00000020;      // CIL_OPCODE:      ldc.i4
            tables.cil[0011] <=#1 32'h00000014;      // CIL_OPERAND:     20
            // BasicClass : main : rva 0x00000250
            tables.cil[0012] <=#1 32'h00000004;      // CIL_ID
            tables.cil[0013] <=#1 32'h00000028;      // CIL_OPCODE:      call
            tables.cil[0014] <=#1 32'h06000003;      // CIL_OPERAND:     100663299
            // BasicClass : main : rva 0x00000250
            tables.cil[0015] <=#1 32'h00000005;      // CIL_ID
            tables.cil[0016] <=#1 32'h0000002A;      // CIL_OPCODE:      ret
            tables.cil[0017] <=#1 32'h00000000;      // CIL_OPERAND:     0
            // BasicClass : .ctor : rva 0x0000026b
            tables.cil[0018] <=#1 32'h00000006;      // CIL_ID
            tables.cil[0019] <=#1 32'h0000002A;      // CIL_OPCODE:      ret
            tables.cil[0020] <=#1 32'h00000000;      // CIL_OPERAND:     0
            // BasicClass : Method1 : rva 0x00000270
            tables.cil[0021] <=#1 32'h00000007;      // CIL_ID
            tables.cil[0022] <=#1 32'h00000006;      // CIL_OPCODE:      ldloc.0
            tables.cil[0023] <=#1 32'h00000000;      // CIL_OPERAND:     0
            // BasicClass : Method1 : rva 0x00000270
            tables.cil[0024] <=#1 32'h00000008;      // CIL_ID
            tables.cil[0025] <=#1 32'h0000000A;      // CIL_OPCODE:      stloc.0
            tables.cil[0026] <=#1 32'h00000000;      // CIL_OPERAND:     0
            // BasicClass : Method1 : rva 0x00000270
            tables.cil[0027] <=#1 32'h00000009;      // CIL_ID
            tables.cil[0028] <=#1 32'h0000002A;      // CIL_OPCODE:      ret
            tables.cil[0029] <=#1 32'h00000000;      // CIL_OPERAND:     0
            // BasicClass : Method2 : rva 0x00000280
            tables.cil[0030] <=#1 32'h0000000A;      // CIL_ID
            tables.cil[0031] <=#1 32'h00000006;      // CIL_OPCODE:      ldloc.0
            tables.cil[0032] <=#1 32'h00000000;      // CIL_OPERAND:     0
            // BasicClass : Method2 : rva 0x00000280
            tables.cil[0033] <=#1 32'h0000000B;      // CIL_ID
            tables.cil[0034] <=#1 32'h0000000A;      // CIL_OPCODE:      stloc.0
            tables.cil[0035] <=#1 32'h00000000;      // CIL_OPERAND:     0
            // BasicClass : Method2 : rva 0x00000280
            tables.cil[0036] <=#1 32'h0000000C;      // CIL_ID
            tables.cil[0037] <=#1 32'h0000002A;      // CIL_OPCODE:      ret
            tables.cil[0038] <=#1 32'h00000000;      // CIL_OPERAND:     0
        end  
    endtask        

    // CREATE STACK FRAME
    // mitId is the method id, and "this" is the identifier for the new instance (or just 0 if it is a static method)
    task createStackFrame(input [31:0] mit_id, input [31:0] this_id);
        begin
            stack.slot[stack.nsfp + stack.SF_MIT_ID]  <=#1 mit_id;
            stack.slot[stack.nsfp + stack.SF_CIT_ID]  <=#1 32'h0; // Only one class so far
            stack.slot[stack.nsfp + stack.SF_THIS_ID] <=#1 this_id; // 0 if static method          
            stack.slot[stack.nsfp + stack.SF_ARGS]    <=#1 32'h4;   // 4 hardcoded
            stack.slot[stack.nsfp + stack.SF_LOCS]    <=#1 32'h4;   // 4 hardcoded
            stack.slot[stack.nsfp + stack.SF_LR]      <=#1 stack.lr;
            stack.slot[stack.nsfp + stack.SF_ARG_0]   <=#1 32'h0;
            stack.slot[stack.nsfp + stack.SF_ARG_1]   <=#1 32'h0;          
            stack.slot[stack.nsfp + stack.SF_ARG_2]   <=#1 32'h0;          
            stack.slot[stack.nsfp + stack.SF_ARG_3]   <=#1 32'h0;          
            stack.slot[stack.nsfp + stack.SF_LOC_0]   <=#1 32'h0;          
            stack.slot[stack.nsfp + stack.SF_LOC_1]   <=#1 32'h0;          
            stack.slot[stack.nsfp + stack.SF_LOC_2]   <=#1 32'h0;          
            stack.slot[stack.nsfp + stack.SF_LOC_3]   <=#1 32'h0;          

            // Arguments slots
            copyWords(evalstack.esp, stack.nsfp, 32'h4);
            // Locals slots
            copyWords(evalstack.esp + 32'h4, stack.nsfp + 32'h4, 32'h4);
            // Increment stack frame pointers
            stack.sfp     <=#1 stack.nsfp;
            stack.nsfp    <=#1 stack.nsfp + stack.SF_SIZE;
            // esp reset (but it is a full stack)
            evalstack.esp <=#1 32'h0;
        end
    endtask    

    // Called when a method returns control to its caller
    // i.e. the previous stack frame
    task popStackFrame();
        begin
        end
    endtask
    
    // Create heap frame for a static class or new instance/object
    // If this = 0, then it is a static class, otherwise an instance.
    task createHeapFrame(input [31:0] cit_id, input [31:0] this_id);
        begin
            heap.slot[heap.nhfp + heap.HF_THIS_ID]  <=#1 this_id; // 0 means static
            heap.slot[heap.nhfp + heap.HF_CIT_ID]   <=#1 cit_id; 
            heap.slot[heap.nhfp + heap.HF_FLD_0]    <=#1 32'h0;
            heap.slot[heap.nhfp + heap.HF_FLD_1]    <=#1 32'h0;
            heap.slot[heap.nhfp + heap.HF_FLD_2]    <=#1 32'h0;
            heap.slot[heap.nhfp + heap.HF_FLD_3]    <=#1 32'h0;
            heap.hfp  <= heap.nhfp;
            heap.nhfp <= heap.nhfp + heap.HF_SIZE;
        end
    endtask
    
    task showEvalStack();
        begin
            $display("showEvalStack();");            
                $display("eval stack (top)                    --------------");
            if (evalstack.esp >= 3) begin
                $display("                              esp[3] | 0x%000008x |", evalstack.slot[3]);
                $display("                                     --------------");
            end
            if (evalstack.esp >= 2) begin
                $display("                              esp[2] | 0x%000008x |", evalstack.slot[2]);
                $display("                                     --------------");
            end
            if (evalstack.esp >= 1) begin
                $display("                              esp[1] | 0x%000008x |", evalstack.slot[1]);
                $display("                                     --------------");
            end
            if (evalstack.esp >= 0) begin
                $display("                              esp[0] | 0x%000008x |", evalstack.slot[0]);
            end
                $display("eval stack (start)                   --------------");    
        end
    endtask
    
    task showStackFrame();
        begin
        end
    endtask

    task showHeapFrame();
        begin
        end
    endtask
    
    // Show the contents of the cit, mit, fit, etc. tables that were loaded (originally) from mem.sv
    task showMemSv();
        begin
          `ifdef MODEL_TECH // Quartus can't synthesize the foreach statements (just needed for sim)
            $display("\nshowMemSv()");
            foreach (tables.mit_names[i])
                // Name and method for debugging
                if (tables.mit_names[i] >= 0)
                    $display("mit_names[0x%4x] %0s", i, tables.mit_names[i]);
                else
                    break;
            
            $display("");
            $display("cit records:         [0]CIT_ID");
            $display("                     [1]CIT_NUMFIELDS");
            $display("                     [2]CIT_NUMMETHODS");
            foreach (tables.cit[i])
                // Only show if defined (ie. how many classes are defined in the IL code)
                if (tables.cit[i] >= 0)
                    $display("        cit[0x%4x] 0x%000008x", i, tables.cit[i]);
                else
                    break;
                    
            $display("");       
            $display("mit records:         [0]MIT_ID");
            $display("                     [1]MIT_CIT_ID (foreign key)");
            $display("                     [2]MIT_NUMARGS");
            $display("                     [3]MIT_NUMLOCALS");
            $display("                     [4]MIT_MAXEVALSTACK");
            $display("                     [5]MIT_NUMCIL");
            $display("                     [6]MIT_FIRSTCIL");
            foreach (tables.mit[i])
                // Only show if defined (ie. how many methods are defined in the F#/IL code)
                if (tables.mit[i] >= 0)
                    $display("    mit[0x%4x] 0x%000008x", i, tables.mit[i]);
                else
                    break;
                    
            $display("");
            $display("cil records:");
            $display("    [0]CIL_ID");
            $display("    [1]CIL_OPCODE");
            $display("    [2]CIL_OPERAND");
            foreach (tables.cil[i])
                // Only show if defined (ie. how many methods are defined in the F#/IL code)
                if (tables.cil[i] >= 0)
                    $display("    cil[0x%4x] 0x%000008x", i, tables.cil[i]);
                else
                    break;
          `endif
        end
    endtask     
    
    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // CIL TASKS    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    
    //==BASE INSTRUCTIONS TASKS==
    // NOP
    task NOPx00_task();
        begin
            $display("NOPx00_task()");
            nextInstruction();
        end
    endtask
    
    // BREAK
    task BREAKx01_task();
        begin
            $display("BREAKx01_task()");
        end
    endtask
    
    // LDARG0
    task LDARG0x02_task();
        begin
            $display("LDARG0x02_task()");
            LDARGxFE09_task(32'h0);
        end
    endtask
    
    // LDARG1
    task LDARG1x03_task();
        begin
            $display("LDARG1x03_task()");
            LDARGxFE09_task(32'h1);
        end
    endtask
    
    // LDARG2
    task LDARG2x04_task();
        begin
            $display("LDARG2x04_task()");
            LDARGxFE09_task(32'h2);
        end
    endtask
    
    // LDARG3
    task LDARG3x05_task();
        begin
            $display("LDARG3x05_task()");
            LDARGxFE09_task(32'h3);
        end
    endtask
    
    // LDLOC0
    task LDLOC0x06_task();
        begin
            $display("LDLOC0x06_task()");
            LDLOCxFE0C_task(32'h0);
        end
    endtask
    
    // LDLOC1
    task LDLOC1x07_task();
        begin
            $display("LDLOC1x07_task()");
            LDLOCxFE0C_task(32'h1);
        end
    endtask
    
    // LDLOC2
    task LDLOC2x08_task();
        begin
            $display("LDLOC2x08_task()");
            LDLOCxFE0C_task(32'h2);
        end
    endtask
    
    // LDLOC3
    task LDLOC3x09_task();
        begin
            $display("LDLOC3x09_task()");
            LDLOCxFE0C_task(32'h3);
        end
    endtask
    
    // STLOC0
    task STLOC0x0A_task();
        begin
            $display("STLOC0x0A_task()");
            STLOCxFE0E_task(32'h0);
        end
    endtask
    
    // STLOC1
    task STLOC1x0B_task();
        begin
            $display("STLOC1x0B_task()");
            STLOCxFE0E_task(32'h1);
        end
    endtask
    
    // STLOC2
    task STLOC2x0C_task();
        begin
            $display("STLOC2x0C_task()");
            STLOCxFE0E_task(32'h2);
        end
    endtask
    
    // STLOC3
    task STLOC3x0D_task();
        begin
            $display("STLOC3x0D_task()");
            STLOCxFE0E_task(32'h3);
        end
    endtask
    
    // STARGS
    task STARGSx10_task(input [31:0] argIndex);
        begin
            $display("STARGSx10_task()");
            STARGxFE0B_task(argIndex);
        end
    endtask
    
    // LDLOCS
    task LDLOCSx11_task(input [31:0] locIndx);
        begin
            $display("LDLOCSx11_task(input [31:0] locIndx=%d)", locIndx);
            LDLOCxFE0C_task(locIndx);
        end
    endtask
    
    // LDLOCAS
    task LDLOCASx12_task(input [31:0] locIndx);
        begin
            $display("LDLOCASx12_task(input [31:0] locIndx=%d)", locIndx);
            LDLOCAxFE0D_task(locIndx);
        end
    endtask
    
    // STLOCS
    task STLOCSx13_task(input [31:0] locIndx);
        begin
            $display("STLOCSx13_task()");
            STLOCxFE0E_task(locIndx);
        end
    endtask
    
    // LDNULL
    task LDNULLx14_task();
        begin
            $display("LDNULLx14_task(), evalstack[esp=%d]=0x%x", evalstack.esp, evalstack.slot[evalstack.esp]); 
            // The offset is also dependent on the number of arguments for this mid
            evalstack.slot[evalstack.esp + 32'h1] <=#1 32'h0; // 0 ok as null?
            evalstack.esp <=#1 evalstack.esp + 32'h1;
            nextInstruction();
        end
    endtask
    
    // LDCI4M1
    task LDCI4M1x15_task();
        begin
            $display("LDCI4M1x15_task(), evalstack[esp=%d]=0x%x", evalstack.esp, evalstack.slot[evalstack.esp]); 
            // The offset is also dependent on the number of arguments for this mid
            evalstack.slot[evalstack.esp + 32'h1] <=#1 32'hFFFFFFFF; // -1?
            evalstack.esp <=#1 evalstack.esp + 32'h1;
            nextInstruction();
        end
    endtask
    
    // LDCI40
    task LDCI40x16_task();
        begin
            $display("LDCI40x16_task()");
            LDCI4x20_task(32'h0);
        end
    endtask
    
    // LDCI41
    task LDCI41x17_task();
        begin
            $display("LDCI41x17_task()");
            LDCI4x20_task(32'h1);
        end
    endtask
    
    // LDCI42
    task LDCI42x18_task();
        begin
            $display("LDCI42x18_task()");
            LDCI4x20_task(32'h2);
        end
    endtask
    
    // LDCI43
    task LDCI43x19_task();
        begin
            $display("LDCI43x19_task()");
            LDCI4x20_task(32'h3);
        end
    endtask
    
    // LDCI44
    task LDCI44x1A_task();
        begin
            $display("LDCI44x1A_task()");
            LDCI4x20_task(32'h4);
        end
    endtask
    
    // LDCI45
    task LDCI45x1B_task();
        begin
            $display("LDCI45x1B_task()");
            LDCI4x20_task(32'h5);
        end
    endtask
    
    // LDCI46
    task LDCI46x1C_task();
        begin
            $display("LDCI46x1C_task()");
            LDCI4x20_task(32'h6);
        end
    endtask
    
    // LDCI47
    task LDCI47x1D_task();
        begin
            $display("LDCI47x1D_task()");
            LDCI4x20_task(32'h7);
        end
    endtask
    
    // LDCI48
    task LDCI48x1E_task();
        begin
            $display("LDCI48x1E_task()");
            LDCI4x20_task(32'h8);
        end
    endtask
    
    // LDCI44S
    task LDCI44Sx1F_task(input [31:0] in);
        begin
            $display("LDCI44Sx1F_task(input [31:0] in=%d)", in);
            LDCI4x20_task(in);
        end
    endtask
    
    // LDARGS
    task LDARGSx0E_task(input [31:0] argIndex);
        begin
            $display("LDARGSx0E_task(input [31:0] argIndex=%d)", argIndex);
            LDARGxFE09_task(argIndex);
        end
    endtask
    
    // LDARGAS
    task LDARGASx0F_task(input [31:0] argIndx);
        begin
            $display("LDARGASx0F_task(input [31:0] argIndex=%d)", argIndx);
            LDARGAxFE0A_task(argIndx);
        end
    endtask
    
    // LDC.I4
    // push the value
    task LDCI4x20_task(input [31:0] in);
        begin
           $display("LDCI4x20_task(input [31:0] in=0x%x), esp=0x%x", in, evalstack.esp);
           // push i4 value to new tos
           evalstack.slot[evalstack.esp + 32'h1] <=#1 in;
           // adjust esp (??? <=)
           evalstack.esp <=#1 evalstack.esp + 32'h1;
           nextInstruction();
        end            
    endtask
    
    // LDCI8
    task LDCI8x21_task(input [31:0] in);
        begin
            $display("LDCI8x21_task(input [31:0] in=0x%x), esp=0x%x", in, evalstack.esp);
            // push "i8" value to new tos
            evalstack.slot[evalstack.esp + 32'h2]  <=#1 32'h0;
            evalstack.slot[evalstack.esp + 32'h1]  <=#1 in;
            // adjust esp
            evalstack.esp <= evalstack.esp + 32'h2;
            nextInstruction();
        end
    endtask
    
    // LDCR4
    task LDCR4x22_task(input [31:0] r4);
        begin
            $display("LDCR4x22_task(input [31:0] r4=0x%x), esp=0x%x", r4, evalstack.esp);
            // push r4 value to new tos
            evalstack.slot[evalstack.esp + 32'h1]  <=#1 r4;
            // adjust esp
            evalstack.esp <=#1 evalstack.esp + 32'h1;
            nextInstruction();
        end
    endtask
    
    // LDCR8
    task LDCR8x23_task(input [31:0] r8);
        begin
            $display("LDCR8x23_task(input [31:0] r8=0x%x), esp=0x%x", r8, evalstack.esp);
            // push "r8" value to new tos
            evalstack.slot[evalstack.esp + 32'h2]  <=#1 32'h0;
            evalstack.slot[evalstack.esp + 32'h1]  <=#1 r8;
            // adjust esp
            evalstack.esp <= evalstack.esp + 32'h2;
            nextInstruction();
        end
    endtask
    
    // DUP
    task DUPx25_task();
        begin
            $display("DUPx25_task()");
            evalstack.slot[evalstack.esp + 32'h1]  <=#1 evalstack.slot[evalstack.esp];
            // adjust esp
            evalstack.esp <=#1 evalstack.esp + 32'h1;
            nextInstruction();
        end
    endtask
    
    // POP
    task POPx26_task();
        begin
            $display("POPx26_task()");
            evalstack.slot[evalstack.esp] <=#1 32'bx;
            // adjust esp
            evalstack.esp <=#1 evalstack.esp - 32'h1;
            nextInstruction();            
        end
    endtask
    
    // JMP III.3.37
    task JMPx27_task(input [31:0] mid);
        begin
            $display("JMPx27_task(input [31:0] mid=%d)", mid);
            // Also adjusts pc             
            createStackFrame(mid, heap.this_id);
        end
    endtask
    
    // CALLI
    task CALLIx29_task(input [31:0] mid);
        begin
            $display("CALLIx29_task(input [31:0] mid=0x%0x)", mid);
            // Also adjusts pc             
            createStackFrame(mid, heap.this_id);
        end
    endtask
    
    // RET x2A
    // Return from the current method, possibly with a value
    task RETx2A_task();
        begin
            $display("RETx2A_task()");
            // also sets pc to lr
            popStackFrame();    
        end
    endtask
    
    // BRS
    task BRSx2B_task(input signed [31:0] target);
        begin
            $display("BRSx2B_task(input signed [31:0] target=0x%x), pc=0x%x", target, stack.pc);
            BRx38_task(target);
        end
    endtask
    
    // BRFALSES
    task BRFALSESx2C_task(input signed [31:0] target);
        begin
            $display("BRFALSESx2C_task(input signed [31:0] target=%d), evalstack[esp=%d]=0x%x", target, evalstack.esp, evalstack.slot[evalstack.esp]);
            if (evalstack.slot[evalstack.esp] == 32'h0)
               begin
                   $display("BRFALSE: Calling BRx38_task");
                   evalstack.slot[evalstack.esp] <=#1 32'bx;
                   evalstack.esp <=#1 evalstack.esp - 32'h1;
                   BRx38_task(target); 
               end 
            evalstack.slot[evalstack.esp] <=#1 32'bx;
            evalstack.esp <=#1 evalstack.esp - 32'h1;
            nextInstruction();
        end
    endtask
    
    // BRTRUES
    task BRTRUESx2D_task(input signed [31:0] target);
        begin
            $display("BRTRUESx2D_task(input signed [31:0] target=%d), evalstack[esp=%d]=0x%x", target, evalstack.esp, evalstack.slot[evalstack.esp]);
            if (evalstack.slot[evalstack.esp] != 32'h0)
               begin
                   $display("BRTRUE: Calling BRx38_task");
                   evalstack.slot[evalstack.esp] <=#1 32'bx;
                   evalstack.esp <=#1 evalstack.esp - 32'h1;
                   BRx38_task(target); 
               end 
            evalstack.slot[evalstack.esp] <=#1 32'bx;
            evalstack.esp <=#1 evalstack.esp - 32'h1;
            nextInstruction();
        end
    endtask
    
    // BEQS
    task BEQSx2E_task(input signed [31:0] target);
        begin
            $display("BEQSx2E_task(input signed [31:0] target=%d), evalstack[esp=%d]=0x%x", target, evalstack.esp, evalstack.slot[evalstack.esp]);
            if (evalstack.slot[evalstack.esp] == evalstack.slot[evalstack.esp-32'h1])
               begin
                   $display("BEQS: Calling BRx38_task");
                   evalstack.slot[evalstack.esp] <=#1 32'bx;
                   evalstack.slot[evalstack.esp-32'h1] <=#1 32'bx;
                   evalstack.esp <=#1 evalstack.esp - 32'h2;
                   BRx38_task(target); 
               end 
            evalstack.slot[evalstack.esp] <=#1 32'bx;
            evalstack.slot[evalstack.esp-32'h1] <=#1 32'bx;
            evalstack.esp <=#1 evalstack.esp - 32'h2;
            nextInstruction();
        end
    endtask
    
    // BGES
    task BGESx2F_task(input signed [31:0] target);
        begin
            $display("BGESx2F_task(input signed [31:0] target=%d), evalstack[esp=%d]=0x%x", target, evalstack.esp, evalstack.slot[evalstack.esp]);
            if (evalstack.slot[evalstack.esp-32'h1] >= evalstack.slot[evalstack.esp])
               begin
                   $display("BEQS: Calling BRx38_task");
                   evalstack.slot[evalstack.esp] <=#1 32'bx;
                   evalstack.slot[evalstack.esp-32'h1] <=#1 32'bx;
                   evalstack.esp <=#1 evalstack.esp - 32'h2;
                   BRx38_task(target); 
               end 
            evalstack.slot[evalstack.esp] <=#1 32'bx;
            evalstack.slot[evalstack.esp-32'h1] <=#1 32'bx;
            evalstack.esp <=#1 evalstack.esp - 32'h2;
            nextInstruction();            
        end
    endtask
    
    // BGTS
    task BGTSx30_task(input signed [31:0] target);
        begin
            $display("BGTSx30_task(input signed [31:0] target=%d), evalstack[esp=%d]=0x%x", target, evalstack.esp, evalstack.slot[evalstack.esp]);
            if (evalstack.slot[evalstack.esp-32'h1] > evalstack.slot[evalstack.esp])
               begin
                   $display("BGT: Calling BRx38_task");
                   evalstack.slot[evalstack.esp] <=#1 32'bx;
                   evalstack.slot[evalstack.esp-32'h1] <=#1 32'bx;
                   evalstack.esp <=#1 evalstack.esp - 32'h2;
                   BRx38_task(target); 
               end 
            evalstack.slot[evalstack.esp] <=#1 32'bx;
            evalstack.slot[evalstack.esp-32'h1] <=#1 32'bx;
            evalstack.esp <=#1 evalstack.esp - 32'h2;
            nextInstruction();  
        end
    endtask
    
    // BLES
    task BLESx31_task(input signed [31:0] target);
        begin
            $display("BLESx31_task(input signed [31:0] target=%d), evalstack[esp=%d]=0x%x", target, evalstack.esp, evalstack.slot[evalstack.esp]);
            if (evalstack.slot[evalstack.esp-32'h1] <= evalstack.slot[evalstack.esp])
               begin
                   $display("BLES: Calling BRx38_task");
                   evalstack.slot[evalstack.esp] <=#1 32'bx;
                   evalstack.slot[evalstack.esp-32'h1] <=#1 32'bx;
                   evalstack.esp <=#1 evalstack.esp - 32'h2;
                   BRx38_task(target); 
               end 
            evalstack.slot[evalstack.esp] <=#1 32'bx;
            evalstack.slot[evalstack.esp-32'h1] <=#1 32'bx;
            evalstack.esp <=#1 evalstack.esp - 32'h2;
            nextInstruction(); 
        end
    endtask
    
    // BLTS
    task BLTSx32_task(input signed [31:0] target);
        begin
            $display("BLTSx32_task(input signed [31:0] target=%d), evalstack[esp=%d]=0x%x", target, evalstack.esp, evalstack.slot[evalstack.esp]);
            if (evalstack.slot[evalstack.esp-32'h1] < evalstack.slot[evalstack.esp])
               begin
                   $display("BLTS: Calling BRx38_task");
                   evalstack.slot[evalstack.esp] <=#1 32'bx;
                   evalstack.slot[evalstack.esp-32'h1] <=#1 32'bx;
                   evalstack.esp <=#1 evalstack.esp - 32'h2;
                   BRx38_task(target); 
               end 
            evalstack.slot[evalstack.esp] <=#1 32'bx;
            evalstack.slot[evalstack.esp-32'h1] <=#1 32'bx;
            evalstack.esp <=#1 evalstack.esp - 32'h2;
            nextInstruction(); 
        end
    endtask
    
    // BNEUNS
    task BNEUNSx33_task(input signed [31:0] target);
        begin
            $display("BNEUNSx33_task(input signed [31:0] target=%d), evalstack[esp=%d]=0x%x", target, evalstack.esp, evalstack.slot[evalstack.esp]);
            if (evalstack.slot[evalstack.esp-32'h1] != evalstack.slot[evalstack.esp])
               begin
                   $display("BNEUNS: Calling BRx38_task");
                   evalstack.slot[evalstack.esp] <=#1 32'bx;
                   evalstack.slot[evalstack.esp-32'h1] <=#1 32'bx;
                   evalstack.esp <=#1 evalstack.esp - 32'h2;
                   BRx38_task(target); 
               end 
            evalstack.slot[evalstack.esp] <=#1 32'bx;
            evalstack.slot[evalstack.esp-32'h1] <=#1 32'bx;
            evalstack.esp <=#1 evalstack.esp - 32'h2;
            nextInstruction(); 
        end
    endtask
    
    // BGEUNS
    task BGEUNSx34_task(input signed [31:0] target);
        begin
            $display("BGEUNSx34_task(input signed [31:0] target=%d), evalstack[esp=%d]=0x%x", target, evalstack.esp, evalstack.slot[evalstack.esp]);
            if (evalstack.slot[evalstack.esp-32'h1] >= evalstack.slot[evalstack.esp])
               begin
                   $display("BGEUNS: Calling BRx38_task");
                   evalstack.slot[evalstack.esp] <=#1 32'bx;
                   evalstack.slot[evalstack.esp-32'h1] <=#1 32'bx;
                   evalstack.esp <=#1 evalstack.esp - 32'h2;
                   BRx38_task(target); 
               end 
            evalstack.slot[evalstack.esp] <=#1 32'bx;
            evalstack.slot[evalstack.esp-32'h1] <=#1 32'bx;
            evalstack.esp <=#1 evalstack.esp - 32'h2;
            nextInstruction(); 
        end
    endtask
    
    // BGTUNS
    task BGTUNSx35_task(input signed [31:0] target);
        begin
            $display("BGTUNSx35_task(input signed [31:0] target=%d), evalstack[esp=%d]=0x%x", target, evalstack.esp, evalstack.slot[evalstack.esp]);
            if (evalstack.slot[evalstack.esp-32'h1] > evalstack.slot[evalstack.esp])
               begin
                   $display("BGTUNS: Calling BRx38_task");
                   evalstack.slot[evalstack.esp] <=#1 32'bx;
                   evalstack.slot[evalstack.esp-32'h1] <=#1 32'bx;
                   evalstack.esp <=#1 evalstack.esp - 32'h2;
                   BRx38_task(target); 
               end 
            evalstack.slot[evalstack.esp] <=#1 32'bx;
            evalstack.slot[evalstack.esp-32'h1] <=#1 32'bx;
            evalstack.esp <=#1 evalstack.esp - 32'h2;
            nextInstruction();             
        end
    endtask
    
    // BLEUNS
    task BLEUNSx36_task(input signed [31:0] target);
        begin
            $display("BLEUNSx36_task(input signed [31:0] target=%d), evalstack[esp=%d]=0x%x", target, evalstack.esp, evalstack.slot[evalstack.esp]);
            if (evalstack.slot[evalstack.esp-32'h1] <= evalstack.slot[evalstack.esp])
               begin
                   $display("BLEUNS: Calling BRx38_task");
                   evalstack.slot[evalstack.esp] <=#1 32'bx;
                   evalstack.slot[evalstack.esp-32'h1] <=#1 32'bx;
                   evalstack.esp <=#1 evalstack.esp - 32'h2;
                   BRx38_task(target); 
               end 
            evalstack.slot[evalstack.esp] <=#1 32'bx;
            evalstack.slot[evalstack.esp-32'h1] <=#1 32'bx;
            evalstack.esp <=#1 evalstack.esp - 32'h2;
            nextInstruction(); 
        end
    endtask
    
    // BLTUNS
    task BLTUNSx37_task(input signed [31:0] target);
        begin
            $display("BLTUNSx36_task(input signed [31:0] target=%d), evalstack[esp=%d]=0x%x", target, evalstack.esp, evalstack.slot[evalstack.esp]);
            if (evalstack.slot[evalstack.esp-32'h1] < evalstack.slot[evalstack.esp])
               begin
                   $display("BLTUNS: Calling BRx38_task");
                   evalstack.slot[evalstack.esp] <=#1 32'bx;
                   evalstack.slot[evalstack.esp-32'h1] <=#1 32'bx;
                   evalstack.esp <=#1 evalstack.esp - 32'h2;
                   BRx38_task(target); 
               end 
            evalstack.slot[evalstack.esp] <=#1 32'bx;
            evalstack.slot[evalstack.esp-32'h1] <=#1 32'bx;
            evalstack.esp <=#1 evalstack.esp - 32'h2;
            nextInstruction();
        end
    endtask
    
     
    // BR x38
    // unconditional (signed) <int32> branch
   task BRx38_task(input signed [31:0] target);
       begin
           $display("BRx38_task(input signed [31:0] target=0x%x), pc=0x%x", target, stack.pc);
           stack.pc <=#1 stack.pc + target;
       end
   endtask
    
    // BRFALSE
    task BRFALSEx39_task(input signed [31:0] target);
        begin
            $display("BRFALSEx39_task(input signed [31:0] target=%d)", target);
            BRFALSESx2C_task(target);
        end
    endtask
    
    // BRTRUE
    task BRTRUEx3A_task(input signed [31:0] target);
        begin
            $display("BRTRUEx3A_task(input signed [31:0] target=%d)", target);
            BRTRUESx2D_task(target);
        end
    endtask
    
    // BRINST
    task BRINSTx3A_task(input signed [31:0] target);
        begin
            $display("BRINSTx3A_task(input signed [31:0] target=%d)", target);
            BRTRUESx2D_task(target);
        end
    endtask
    
    // BEQ
    task BEQx3B_task(input signed [31:0] target);
        begin
            $display("BEQx3B_task(input signed [31:0] target=%d)", target);
            BEQSx2E_task(target);
        end
    endtask
    
    // BGE
    task BGEx3C_task(input signed [31:0] target);
        begin
            $display("BGEx3C_task(input signed [31:0] target=%d)", target);
            BGESx2F_task(target);
        end
    endtask
    
    // BGT
    task BGTx3D_task(input signed [31:0] target);
        begin
            $display("BGTx3D_task(input signed [31:0] target=%d)", target);
            BGTSx30_task(target);
        end
    endtask
    
    // BLE
    task BLEx3E_task(input signed [31:0] target);
        begin
            $display("BLEx3E_task(input signed [31:0] target=%d)", target);
            BLESx31_task(target);        
        end
    endtask
    
    // BLT
    task BLTx3F_task(input signed [31:0] target);
        begin
            $display("BLTx3F_task(input signed [31:0] target=%d)", target);
            BLTSx32_task(target);
        end
    endtask
    
    // BNEUN
    task BNEUNx40_task(input signed [31:0] target);
        begin
            $display("BNEUNx40_task(input signed [31:0] target=%d)", target);
            BNEUNSx33_task(target);
        end
    endtask
    
    // BGEUN
    task BGEUNx41_task(input signed [31:0] target);
        begin
            $display("BGEUNx41_task(input signed [31:0] target=%d)", target);
            BGEUNSx34_task(target);
        end
    endtask
    
    // BGTUN
    task BGTUNx42_task(input signed [31:0] target);
        begin
            $display("BGTUNx42_task(input signed [31:0] target=%d)", target);
            BGTUNSx35_task(target);
        end
    endtask
    
    // BLEUN
    task BLEUNx43_task(input signed [31:0] target);
        begin
            $display("BLEUNx43_task(input signed [31:0] target=%d)", target);
            BLEUNSx36_task(target);
        end
    endtask
    
    // BLTUN
    task BLTUNx44_task(input signed [31:0] target);
        begin
            $display("BLTUNx44_task(input signed [31:0] target=%d)", target);
            BLTUNSx37_task(target);
        end
    endtask
    
    // SWITCH
    task SWITCHx45_task();
        begin
            $display("SWITCHx45_task()");   
            nextInstruction();
        end
    endtask
    
    // LDINDI1
    task LDINDI1x46_task(input [31:0] addr);
        begin
            $display("LDINDI1x46_task(input [31:0] addr=%d)", addr);
            LDINDIx4D_task(addr);
        end
    endtask
    
    // LDINDU1
    task LDINDU1x47_task(input [31:0] addr);
        begin
            $display("LDINDU1x47_task(input [31:0] addr=%d)", addr);
            LDINDIx4D_task(addr);
        end
    endtask
    
    // LDINDI2
    task LDINDI2x48_task(input [31:0] addr);
        begin
            $display("LDINDI2x48_task(input [31:0] addr=%d)", addr);
            LDINDIx4D_task(addr);
        end
    endtask
    
    // LDINDU2
    task LDINDU2x49_task(input [31:0] addr);
        begin
            $display("LDINDU2x49_task(input [31:0] addr=%d)", addr);
            LDINDIx4D_task(addr);
        end
    endtask
    
    // LDINDI4
    task LDINDI4x4A_task(input [31:0] addr);
        begin
            $display("LDINDI4x4A_task(input [31:0] addr=%d)", addr);
            LDINDIx4D_task(addr);
        end
    endtask
    
    // LDINDU4
    task LDINDU4x4B_task(input [31:0] addr);
        begin
            $display("LDINDU4x4B_task(input [31:0] addr=%d)", addr);
            LDINDIx4D_task(addr);
        end
    endtask
    
    // LDINDI8
    task LDINDI8x4C_task(input [31:0] addr);
        begin
            $display("LDINDI8x4C_task(input [31:0] addr=%d)", addr);
            LDINDIx4D_task(addr);
        end
    endtask
    
    // LDINDU8
    task LDINDU8x4C_task(input [31:0] addr);
        begin
            $display("LDINDU8x4C_task(input [31:0] addr=%d)", addr);
            LDINDIx4D_task(addr);
        end
    endtask
    
    // LDINDI
    task LDINDIx4D_task(input [31:0] addr);
        begin
            $display("LDINDIx4D_task(input [31:0] addr=%d)", addr);
            nextInstruction();
        end
    endtask
    
    // LDINDR4
    task LDINDR4x4E_task(input [31:0] addr);
        begin
            $display("LDINDR4x4E_task(input [31:0] addr=%d)", addr);
            LDINDIx4D_task(addr);
        end
    endtask
    
    // LDINDR8
    task LDINDR8x4F_task(input [31:0] addr);
        begin
            $display("LDINDR8x4F_task(input [31:0] addr=%d)", addr);
            LDINDIx4D_task(addr);
        end
    endtask
    
    // LDINDREF
    task LDINDREFx50_task(input [31:0] addr);
        begin
            $display("LDINDREFx50_task(input [31:0] addr=%d)", addr);
            LDINDIx4D_task(addr);
        end
    endtask
    
    // STINDREF
    task STINDREFx51_task(input [31:0] addr);
        begin
            $display("STINDREFx51_task(input [31:0] addr=%d)", addr);
            STINDIxDF_task();
        end
    endtask
    
    // STNDI1
    task STNDI1x52_task(input [31:0] addr);
        begin
            $display("STNDI1x52_task(input [31:0] addr=%d)", addr);
            STINDIxDF_task();
        end
    endtask
    
    // STINDI2
    task STINDI2x53_task(input [31:0] addr);
        begin
            $display("STINDI2x53_task(input [31:0] addr=%d)", addr);
            STINDIxDF_task();
        end
    endtask
    
    // STINDI4
    task STINDI4x54_task(input [31:0] addr);
        begin
            $display("STINDI4x54_task(input [31:0] addr=%d)", addr);
            STINDIxDF_task();
        end
    endtask
    
    // STINDI8
    task STINDI8x55_task(input [31:0] addr);
        begin
            $display("STINDI8x55_task(input [31:0] addr=%d)", addr);
            STINDIxDF_task();
        end
    endtask
    
    // STINDR4
    task STINDR4x56_task(input [31:0] addr);
        begin
            $display("STINDR4x56_task(input [31:0] addr=%d)", addr);
            STINDIxDF_task();
        end
    endtask
    
    // STINDR8
    task STINDR8x57_task(input [31:0] addr);
        begin
            $display("STINDR8x57_task(input [31:0] addr=%d)", addr);
            STINDIxDF_task();
        end
    endtask
    
    // ADD x58
    // add the two numbers, pop the operands, push the result
    task ADDx58_task(input carryIn);
        begin
            $display("ADDx58_task(input carryIn)=%d), esp=0x%x", carryIn, evalstack.esp);
            evalstack.slot[evalstack.esp - 1'b1] <=#1 carryIn + evalstack.slot[evalstack.esp] + evalstack.slot[evalstack.esp - 1'b1]; 
            evalstack.slot[evalstack.esp] <=#1 32'bx;
            evalstack.esp <=#1 evalstack.esp - 1'b1;
            nextInstruction();
        end
    endtask
    
    // SUB
    task SUBx59_task();
        begin
            $display("SUBx59_task()");
            evalstack.slot[evalstack.esp - 32'h1] <=#1 evalstack.slot[evalstack.esp] + evalstack.slot[evalstack.esp - 32'h1]; 
            evalstack.slot[evalstack.esp] <=#1 32'bx;
            evalstack.esp <=#1 evalstack.esp - 32'h1;
            nextInstruction();
        end
    endtask
    
    // MUL
    task MULx5A_task();
        begin
            $display("MULx5A_task()");
            evalstack.slot[evalstack.esp - 32'h1] <=#1 evalstack.slot[evalstack.esp] * evalstack.slot[evalstack.esp - 32'h1]; 
            evalstack.slot[evalstack.esp] <=#1 32'bx;
            evalstack.esp <=#1 evalstack.esp - 32'h1;
            nextInstruction();
        end
    endtask
    
    // DIV
    task DIVx5B_task();
        begin
            $display("DIVx5B_task()");
            evalstack.slot[evalstack.esp - 32'h1] <=#1 evalstack.slot[evalstack.esp] / evalstack.slot[evalstack.esp - 32'h1]; 
            evalstack.slot[evalstack.esp] <=#1 32'bx;
            evalstack.esp <=#1 evalstack.esp - 32'h1;
            nextInstruction();
        end
    endtask
    
    // DIVUN
    task DIVUNx5C_task();
        begin
            $display("DIVUNx5C_task()");
            DIVx5B_task();
        end
    endtask
    
    // REM
    task REMx5D_task();
        begin
            $display("REMx5D_task()");
            evalstack.slot[evalstack.esp - 32'h1] <=#1 evalstack.slot[evalstack.esp]-(evalstack.slot[evalstack.esp]*(evalstack.slot[evalstack.esp] / evalstack.slot[evalstack.esp - 32'h1])); 
            evalstack.slot[evalstack.esp] <=#1 32'bx;
            evalstack.esp <=#1 evalstack.esp - 32'h1;
            nextInstruction();
        end
    endtask
    
    // REMUN
    task REMUNx5E_task();
        begin
            $display("REMUNx5E_task()");
            REMx5D_task();
        end
    endtask
    
    // AND
    task ANDx5F_task();
        begin
            $display("ANDx5F_task()");
            evalstack.slot[evalstack.esp - 32'h1] <=#1 evalstack.slot[evalstack.esp] & evalstack.slot[evalstack.esp - 32'h1]; 
            evalstack.slot[evalstack.esp] <=#1 32'bx;
            evalstack.esp <=#1 evalstack.esp - 32'h1;
            nextInstruction();
        end
    endtask
    
    // OR
    task ORx60_task();
        begin
            $display("ORx60_task()");
            evalstack.slot[evalstack.esp - 32'h1] <=#1 evalstack.slot[evalstack.esp] | evalstack.slot[evalstack.esp - 32'h1]; 
            evalstack.slot[evalstack.esp] <=#1 32'bx;
            evalstack.esp <=#1 evalstack.esp - 32'h1;
            nextInstruction();
        end
    endtask
    
    // XOR
    task XORx61_task();
        begin
            $display("XORx61_task()");
            evalstack.slot[evalstack.esp - 32'h1] <=#1 evalstack.slot[evalstack.esp] ^ evalstack.slot[evalstack.esp - 32'h1]; 
            evalstack.slot[evalstack.esp] <=#1 32'bx;
            evalstack.esp <=#1 evalstack.esp - 32'h1;
            nextInstruction();
        end
    endtask
    
    // SHL x62
    // value, shiftAmount -> result
    task SHLx62_task();
        begin
            $display("SHLx62_task()");
            $display("shiftAmount=evalstack[esp=%d]=0x%x, value=evalstack[esp-1=%d]=0x%x", evalstack.esp, evalstack.slot[evalstack.esp], evalstack.esp-32'h1, evalstack.slot[evalstack.esp-32'h1]);
            evalstack.slot[evalstack.esp-32'h1] <=#1 evalstack.slot[evalstack.esp-32'h1] << evalstack.slot[evalstack.esp];
            evalstack.slot[evalstack.esp] <=#1 32'bx;
            evalstack.esp <=#1 evalstack.esp - 32'h1;
            nextInstruction();
        end
    endtask
    
    // SHR
    task SHRx63_task();
        begin
            $display("SHRx63_task()");
            $display("shiftAmount=evalstack[esp=%d]=0x%x, value=evalstack[esp-1=%d]=0x%x", evalstack.esp, evalstack.slot[evalstack.esp], evalstack.esp-32'h1, evalstack.slot[evalstack.esp-32'h1]);
            evalstack.slot[evalstack.esp-32'h1] <=#1 evalstack.slot[evalstack.esp-32'h1] >> evalstack.slot[evalstack.esp];
            evalstack.slot[evalstack.esp] <=#1 32'bx;
            evalstack.esp <=#1 evalstack.esp - 32'h1;
            nextInstruction();
        end
    endtask
    
    // SHRUN
    task SHRUNx64_task();
        begin
            $display("SHRUNx64_task()");
            SHRx63_task();
        end
    endtask
    
    // NEG
    task NEGx65_task();
        begin
            $display("NEGx65_task()");
            evalstack.slot[evalstack.esp] <=#1 0 - evalstack.slot[evalstack.esp];
            nextInstruction();
        end
    endtask
    
    // NOT
    task NOTx66_task();
        begin
            $display("NOTx66_task()");
            evalstack.slot[evalstack.esp] <=#1 !evalstack.slot[evalstack.esp];
            nextInstruction();
        end
    endtask
    
    // CONVI1
    task CONVI1x67_task();
        begin
            $display("CONVI1x67_task()");
            nextInstruction();
        end
    endtask
    
    // CONVI2
    task CONVI2x68_task();
        begin
            $display("CONVI2x68_task()");
            nextInstruction();
        end
    endtask
    
    // CONVI4
    task CONVI4x69_task();
        begin
            $display("CONVI4x69_task()");
            nextInstruction();
        end
    endtask
    
    // CONVI8
    task CONVI8x6A_task();
        begin
            $display("CONVI8x6A_task()");
            nextInstruction();
        end
    endtask
    
    // CONVR4
    task CONVR4x6B_task();
        begin
            $display("CONVR4x6B_task()");
            nextInstruction();
        end
    endtask
    
    // CONVR8
    task CONVR8x6C_task();
        begin
            $display("CONVR8x6C_task()");
            nextInstruction();
        end
    endtask
    
    // CONVU4
    task CONVU4x6D_task();
        begin
            $display("CONVU4x6D_task()");
            nextInstruction();
        end
    endtask
    
    // CONVU8
    task CONVU8x6E_task();
        begin
            $display("CONVU8x6E_task()");
            nextInstruction();
        end
    endtask
    
    // CONVRUN
    task CONVRUNx76_task();
        begin
            $display("CONVRUNx76_task()");
            nextInstruction();
        end
    endtask
    
    // obj -> value
    task LDFLDx7B_task(input [31:0] fldIndx);
        begin
            // pop and push
            evalstack.slot[evalstack.esp+32'h0] <=#1 heap.slot[heap.hfp + heap.HF_FLD_0 + fldIndx];
            nextInstruction();
        end
    endtask 
    
    // CONVOVFI1UN
    task CONVOVFI1UNx82_task();
        begin
            $display("CONVOVFI1UNx82_task()");
            nextInstruction();
        end
    endtask
    
    // CONVOVFI2UN
    task CONVOVFI2UNx83_task();
        begin
            $display("CONVOVFI2UNx83_task()");
            nextInstruction();
        end
    endtask
    
    // CONVOVFI4UN
    task CONVOVFI4UNx84_task();
        begin
            $display("CONVOVFI4UNx84_task()");
            nextInstruction();
        end
    endtask
    
    // CONVOVFI8UN
    task CONVOVFI8UNx85_task();
        begin
            $display("CONVOVFI8UNx85_task()");
            nextInstruction();
        end
    endtask
    
    // CONVOVFU1UN
    task CONVOVFU1UNx86_task();
        begin
            $display("CONVOVFU1UNx86_task()");
            nextInstruction();
        end
    endtask
    
    // CONVOVFU2UN
    task CONVOVFU2UNx87_task();
        begin
            $display("CONVOVFU2UNx87_task()");
            nextInstruction();
        end
    endtask
    
    // CONVOVFU4UN
    task CONVOVFU4UNx88_task();
        begin
            $display("CONVOVFU4UNx88_task()");
            nextInstruction();
        end
    endtask
    
    // CONVOVFU8UN
    task CONVOVFU8UNx89_task();
        begin
            $display("CONVOVFU8UNx89_task()");
            nextInstruction();
        end
    endtask
    
    // CONVOVFIUN
    task CONVOVFIUNx8A_task();
        begin
            $display("CONVOVFIUNx8A_task()");
            nextInstruction();
        end
    endtask
    
    // CONVOVFUUN
    task CONVOVFUUNx8B_task();
        begin
            $display("CONVOVFUUNx8B_task()");
            nextInstruction();
        end
    endtask
    
    // CONVOVFI1
    task CONVOVFI1xB3_task();
        begin
            $display("CONVOVFI1xB3_task()");
            nextInstruction();
        end
    endtask
    
    // CONVOVFU1
    task CONVOVFU1xB4_task();
        begin
            $display("CONVOVFU1xB4_task()");
            nextInstruction();
        end
    endtask
    
    // CONVOVFI2
    task CONVOVFI2xB5_task();
        begin
            $display("CONVOVFI2xB5_task()");
            nextInstruction();
        end
    endtask
    
    // CONVOVFU2
    task CONVOVFU2xB6_task();
        begin
            $display("CONVOVFU2xB6_task()");
            nextInstruction();
        end
    endtask
    
    // CONVOVFI4
    task CONVOVFI4xB7_task();
        begin
            $display("CONVOVFI4xB7_task()");
            nextInstruction();
        end
    endtask
    
    // CONVOVFI8
    task CONVOVFI8xB9_task();
        begin
            $display("CONVOVFI8xB9_task()");
            nextInstruction();
        end
    endtask
    
    // CONVOVFI2
    task CONVOVFI2xBA_task();
        begin
            $display("CONVOVFI2xBA_task()");
            nextInstruction();
        end
    endtask
    
    // CKFINITE
    task CKFINITExC3_task();
        begin
            $display("CKFINITExC3_task()");
            nextInstruction();
        end
    endtask
    
    // CONVU2
    task CONVU2xD1_task();
        begin
            $display("CONVU2xD1_task()");
            nextInstruction();
        end
    endtask
    
    // CONVU1
    task CONVU1xD2_task();
        begin
            $display("CONVU1xD2_task()");
            nextInstruction();
        end
    endtask
    
    // CONVI
    task CONVIxD3_task();
        begin
            $display("CONVIxD3_task()");
            nextInstruction();
        end
    endtask
    
    // CONVOVFU
    task CONVOVFUxD5_task();
        begin
            $display("CONVOVFUxD5_task()");
            nextInstruction();
        end
    endtask
    
    // ADDOVF
    task ADDOVFxD6_task(input carryIn);
        begin
            $display("ADDOVFxD6_task()");
            ADDx58_task(carryIn);
        end
    endtask
    
    // ADDOVFUN
    task ADDOVFUNxD7_task(input carryIn);
        begin
            $display("ADDOVFUNxD7_task()");
            ADDx58_task(carryIn);
        end
    endtask
    
    // MULOVF
    task MULOVFxD8_task();
        begin
            $display("MULOVFxD8_task()");
            MULx5A_task();
        end
    endtask
    
    // MULOVFUN
    task MULOVFUNxD9_task();
        begin
            $display("MULOVFUNxD9_task()");
            MULx5A_task();
        end
    endtask
    
    // SUBOVF
    task SUBOVFxDA_task();
        begin
            $display("SUBOVFxDA_task()");
            SUBx59_task();
        end
    endtask
    
    // SUBOVFUN
    task SUBOVFUNxDB_task();
        begin
            $display("SUBOVFUNxDB_task()");
            SUBx59_task();
        end
    endtask
    
    // ENDFAULT
    task ENDFAULTxDC_task();
        begin
            $display("ENDFAULTxDC_task()");
            nextInstruction();
        end
    endtask
    
    // ENDFINALLY
    task ENDFINALLYxDC_task();
        begin
            $display("ENDFINALLYxDC_task()");
            evalstack.esp <= 32'h0;
            nextInstruction();
        end
    endtask
    
    // LEAVE
    task LEAVExDD_task(input signed [31:0] target);
        begin
            $display("LEAVExDD_task()");
            BRx38_task(target);
        end
    endtask
    
    // LEAVES
    task LEAVESxDE_task(input signed [31:0] target);
        begin
            $display("LEAVESxDE_task()");
            BRx38_task(target);
        end
    endtask
    
    // STINDI
    task STINDIxDF_task();
        begin
            $display("STINDIxDF_task()");
            evalstack.slot[evalstack.slot[evalstack.esp-32'h1]] <=#1 evalstack.slot[evalstack.esp];
            evalstack.slot[evalstack.esp] <=#1 32'bx;
            evalstack.slot[evalstack.esp-32'h1] <=#1 32'bx;
            evalstack.esp <=#1 evalstack.esp - 32'h2;
            nextInstruction();
        end
    endtask
    
    // CONVU
    task CONVUxE0_task();
        begin
            $display("CONVUxE0_task()");
            nextInstruction();
        end
    endtask
    
    // CEQ
    task CEQxFE01_task();
        begin
            $display("CEQxFE01_task()");
            if (evalstack.slot[evalstack.esp-32'h1] == evalstack.slot[evalstack.esp])
                begin
                    evalstack.slot[evalstack.esp-32'h1] <=#1 32'h1;
                end
            else
                begin
                    evalstack.slot[evalstack.esp-32'h1] <=#1 32'h0;
                end
            evalstack.slot[evalstack.esp] <=#1 32'bx;
            evalstack.esp <=#1 evalstack.esp - 32'h1;
            nextInstruction();
        end
    endtask
    
    // ARGLIST
    task ARGLISTxFE00_task();
        begin
            $display("ARGLISTxFE00_task()");
            evalstack.slot[evalstack.esp+32'h1] <=#1 32'h99999999;
            evalstack.esp <=#1 evalstack.esp + 32'h1;
            nextInstruction();
        end
    endtask
    
    // CGT
    task CGTxFE02_task();
        begin
            $display("CGTxFE02_task()");
            if (evalstack.slot[evalstack.esp-32'h1] > evalstack.slot[evalstack.esp])
                begin
                    evalstack.slot[evalstack.esp-32'h1] <=#1 32'h1;
                end
            else
                begin
                    evalstack.slot[evalstack.esp-32'h1] <=#1 32'h0;
                end
            evalstack.slot[evalstack.esp] <=#1 32'bx;
            evalstack.esp <=#1 evalstack.esp - 32'h1;
            nextInstruction();
        end
    endtask
    
    // CGTUN
    task CGTUNxFE03_task();
        begin
            $display("CGTUNxFE03_task()");
            CGTxFE02_task();
        end
    endtask
    
    // CLT
    task CLTxFE04_task();
        begin
            $display("CLTxFE04_task()");
            if (evalstack.slot[evalstack.esp-32'h1] < evalstack.slot[evalstack.esp])
                begin
                    evalstack.slot[evalstack.esp-32'h1] <=#1 32'h1;
                end
            else
                begin
                    evalstack.slot[evalstack.esp-32'h1] <=#1 32'h0;
                end
            evalstack.slot[evalstack.esp] <=#1 32'bx;
            evalstack.esp <=#1 evalstack.esp - 32'h1;
            nextInstruction();            
        end
    endtask
    
    // CLTUN
    task CLTUNxFE05_task();
        begin
            $display("CLTUNxFE05_task()");
            CLTxFE04_task();            
        end
    endtask
    
    // LDFTN
    task LDFTNxFE06_task();
        begin
            $display("LDFTNxFE06_task()");
            evalstack.slot[evalstack.esp+32'h1] <=#1 32'h99999999;
            evalstack.esp <=#1 evalstack.esp + 32'h1;
            nextInstruction();
        end
    endtask
  
    // LDARG FE09
    // Load the n'th argument onto the evaluation stack
    task LDARGxFE09_task(input [31:0] argIndex);
        begin
           $display("LDARGxFE09_task(input [31:0] argIndex=%d), stack[%d]=0x%x, esp=0x%x", argIndex, argIndex, stack.slot[stack.sfp + stack.SF_SIZE + argIndex], evalstack.esp); 
           evalstack.slot[evalstack.esp + 32'h1] <=#1 stack.slot[stack.sfp + stack.SF_SIZE + argIndex];
           evalstack.esp <=#1 evalstack.esp + 32'h1;
           nextInstruction();
        end
    endtask  
    
    // LDARGA
    task LDARGAxFE0A_task(input [31:0] argIndex);
        begin
            $display("LDARGAxFE0A_task(input [31:0] argIndex=%d), stack[%d]=0x%x, esp=0x%x", argIndex, argIndex, stack.slot[stack.sfp + stack.SF_SIZE + argIndex], evalstack.esp); 
            evalstack.slot[evalstack.esp + 32'h1] <=#1 stack.sfp + stack.SF_SIZE + argIndex;
            evalstack.esp <=#1 evalstack.esp + 32'h1;
            nextInstruction();
        end
    endtask
    
    // STARG
    task STARGxFE0B_task(input [31:0] argIndex);
        begin
            $display("STARGxFE0B_task(input [31:0] argIndex=%d), stack[%d]=0x%x, esp=0x%x", argIndex, argIndex, stack.slot[stack.sfp + stack.SF_SIZE + argIndex], evalstack.esp); 
            stack.slot[stack.sfp + stack.SF_SIZE + argIndex] <=#1 evalstack.slot[evalstack.esp];
            evalstack.slot[evalstack.esp] <=#1 32'bx;
            evalstack.esp <=#1 evalstack.esp - 32'h1;
            nextInstruction();
        end
    endtask
    
    // ENDFILTER
    task ENDFILTERxFE11_task();
        begin
            $display("ENDFILTERxFE11_task()");
            evalstack.slot[evalstack.esp] <=#1 32'bx;
            evalstack.esp <=#1 evalstack.esp - 32'h1;
            nextInstruction();
        end
    endtask
    
    // CPBLK
    task CPBLKxFE17_task();
        begin
            $display("CPBLKxFE17_task()");
            stack.slot[evalstack.slot[evalstack.esp-32'h2]] <=#1 stack.slot[evalstack.slot[evalstack.esp-32'h1]];
            evalstack.slot[evalstack.esp] <=#1 32'bx;
            evalstack.slot[evalstack.esp-32'h1] <=#1 32'bx;
            evalstack.slot[evalstack.esp-32'h2] <=#1 32'bx;
            evalstack.esp <=#1 evalstack.esp - 32'h3;
            nextInstruction();
        end
    endtask
    
    // INITBLK
    task INITBLKxFE18_task();
        begin
            $display("INITBLKxFE18_task()");
            stack.slot[evalstack.slot[evalstack.esp-32'h2]] <=#1 evalstack.slot[evalstack.esp-32'h1];
            evalstack.slot[evalstack.esp] <=#1 32'bx;
            evalstack.slot[evalstack.esp-32'h1] <=#1 32'bx;
            evalstack.slot[evalstack.esp-32'h2] <=#1 32'bx;
            evalstack.esp <=#1 evalstack.esp - 32'h3;
            nextInstruction();
        end
    endtask
    
    // LDLOC
    task LDLOCxFE0C_task(input [31:0] locIndx);//, input [31:0] nextmid);
        begin
            logic [31:0] mit_numargs; // Number of arguments
            logic [31:0] mit_numlocals; // Number of locals
            $display("LDLOCxFE0C_task(input login [31:0] locIndx=%d), evalstack[esp=%d]=0x%x", locIndx, evalstack.esp, evalstack.slot[evalstack.esp]); 
            // The offset is also dependent on the number of arguments for this mid
            mit_numargs <=#1 tables.mit[stack.mit_id * tables.MIT_SIZE + tables.MIT_NUMARGS]; // Number of arguments
            mit_numlocals <=#1 tables.mit[stack.mit_id * tables.MIT_SIZE + tables.MIT_NUMLOCALS]; // Number of locals
            evalstack.slot[evalstack.esp + 32'h1] <=#1 stack.slot[stack.sfp + stack.SF_SIZE + mit_numargs + locIndx] ;
            evalstack.esp <=#1 evalstack.esp + 32'h1;
            nextInstruction();
        end
    endtask
    
    // LDLOCA
    task LDLOCAxFE0D_task(input [31:0] locIndx);//, input [31:0] nextmid);
        begin
            logic [31:0] mit_numargs; // Number of arguments
            logic [31:0] mit_numlocals; // Number of locals
            $display("LDLOCAxFE0D_task(input login [31:0] locIndx=%d), evalstack[esp=%d]=0x%x", locIndx, evalstack.esp, evalstack.slot[evalstack.esp]);
            // The offset is also dependent on the number of arguments for this mid
            mit_numargs <=#1 tables.mit[stack.mit_id * tables.MIT_SIZE + tables.MIT_NUMARGS]; // Number of arguments
            mit_numlocals <=#1 tables.mit[stack.mit_id * tables.MIT_SIZE + tables.MIT_NUMLOCALS]; // Number of locals
            evalstack.slot[evalstack.esp + 32'h1] <=#1 stack.sfp + stack.SF_SIZE + mit_numargs + locIndx;
            evalstack.esp <=#1 evalstack.esp + 32'h1;
            nextInstruction();
        end
    endtask
    
    // STLOC FE0E
    // Pop and store the n'th local
    task STLOCxFE0E_task(input [31:0] locIndx);
        begin
           logic [31:0] mit_numargs; // Number of arguments
           logic [31:0] mit_numlocals; // Number of locals
           $display("STLOCxFE0E_task(input login [31:0] locIndx=%d), evalstack[esp=%d]=0x%x", locIndx, evalstack.esp, evalstack.slot[evalstack.esp]); 
           // The offset is also dependent on the number of arguments for this mid
           mit_numargs <=#1 tables.mit[stack.mit_id * tables.MIT_SIZE + tables.MIT_NUMARGS]; // Number of arguments
           mit_numlocals <=#1 tables.mit[stack.mit_id * tables.MIT_SIZE + tables.MIT_NUMLOCALS]; // Number of locals
           stack.slot[stack.sfp + stack.SF_SIZE + mit_numargs + locIndx] <=#1 evalstack.slot[evalstack.esp];
           evalstack.slot[evalstack.esp] <=#1 32'bx;
           evalstack.esp <=#1 evalstack.esp - 32'h1;
           nextInstruction();
        end
    endtask
    
    // LOCALLOC
    task LOCALLOCxFE0F_task();
        begin
            $display("LOCALLOCxFE0F_task()");
            evalstack.slot[evalstack.esp] <=#1 evalstack.slot[evalstack.esp] + stack.sfp + 32'h99999999;
            nextInstruction();
        end
    endtask
    
    // OBJECT TASKS
    
    // Calls the method id on top of evalstack 
    task CALLx28_task(input [31:0] mid);
        begin
             $display("CALLx28_task(input [31:0] mid=%d)", mid);
             // Also adjusts pc             
             createStackFrame(mid, heap.this_id);
        end
    endtask
    
    // CALLVIRT
    task CALLVIRTx6F_task(input [31:0] mid);
        begin
            $display("CALLVIRTx6F_task()");
            createStackFrame(mid, heap.this_id);
        end
    endtask
    
    // CPOBJ
    task CPOBJx70_task();
        begin
            $display("CPOBJx70_task()");
            nextInstruction();            
        end
    endtask
    
    // LDOBJ
    task LDOBJx71_task();
        begin
            $display("LDOBJx71_task()");
            nextInstruction();
        end
    endtask
    
    // LDSTR
    task LDSTRx72_task();
        begin
            $display("LDSTRx72_task()");
            nextInstruction();
        end
    endtask
    
    // NEWOBJx73
    task NEWOBJx73_task(input [31:0] ctorToken);
        begin
            // .ctor is no. 2 in the methoddef table
            logic [31:0] ctor_mit_id;
            logic [31:0] ctor_cit_id;
            logic [31:0] citMitCitIdKeyValue;
            logic [31:0] citIdValue;
            // ctorToken = x06000002
            $display("NEWOBJx73(input [31:0] ctorToken=0x%08x)", ctorToken);
            ctor_mit_id <=#1 (32'h00FFFFFF & ctorToken);
            ctor_cit_id <=#1 tables.mit[ctor_mit_id * tables.MIT_SIZE + tables.MIT_CIT_ID];
            
            // Create the new instance heap frame with incremented "this"
            heap.this_id <=#1 heap.this_id + 32'h1;
            createHeapFrame(32'h0, heap.this_id);            
            
            // Create the stackframe for .ctor
            // point to the first .ctor slot
            createStackFrame(ctor_mit_id, heap.this_id);
            
            nextInstruction();
        end    
    endtask
    
    // CASTCLASS
    task CASTCLASSx74_task();
        begin
            $display("CASTCLASSx74_task()");
            nextInstruction();
        end
    endtask
    
        // obj, value on top of stack
    task STFLDx7D_task(input [31:0] fldIndx);
        begin
            $display("STFLDx7D_task(input [31:0] fldIndx=%d), obj=%d", fldIndx, evalstack.slot[evalstack.esp]);
            $display("val=evalstack[esp=%d]=0x%x, obj=evalstack[esp-1=%d]=0x%x", evalstack.esp, evalstack.slot[evalstack.esp], evalstack.esp-32'h1, evalstack.slot[evalstack.esp-32'h1]);
//TODO: Create lookup table obj=evalstack[esp-32'h1] -> hfp
            // Store val into the object field            
            heap.slot[heap.hfp + heap.HF_FLD_0 + fldIndx] <=#1 evalstack.slot[evalstack.esp];
            evalstack.slot[evalstack.esp] <=#1 32'bx;
            evalstack.slot[evalstack.esp - 32'h1] <=#1 32'bx;
            evalstack.esp <=#1 evalstack.esp - 32'h2;
            nextInstruction();
        end
    endtask
    
    // LDSFLD
    task LDSFLDx7E_task(input [31:0] fldIndx);
        begin // TODO: Wrong
            $display("LDSFLDx7E_task(input [31:0] fldIndx=%d), obj=%d", fldIndx, evalstack.slot[evalstack.esp]);
            $display("val=evalstack[esp=%d]=0x%x, obj=evalstack[esp-1=%d]=0x%x", evalstack.esp, evalstack.slot[evalstack.esp], evalstack.esp-32'h1, evalstack.slot[evalstack.esp-32'h1]);
//TODO: Create lookup table obj=evalstack[esp-32'h1] -> hfp
            // Store val into the object field            
            evalstack.slot[evalstack.esp] <=#1 32'bx;
            evalstack.slot[evalstack.esp - 32'h1] <=#1 heap.slot[heap.hfp + heap.HF_FLD_0 + fldIndx];
            evalstack.esp <=#1 evalstack.esp - 32'h1;
            nextInstruction();
        end
    endtask
    
    // LDSFLDA
    task LDSFLDAx7F_task(input [31:0] fldIndx);
        begin
            $display("LDSFLDAx7F_task(input [31:0] fldIndx=%d), obj=%d", fldIndx, evalstack.slot[evalstack.esp]);
            $display("val=evalstack[esp=%d]=0x%x, obj=evalstack[esp-1=%d]=0x%x", evalstack.esp, evalstack.slot[evalstack.esp], evalstack.esp-32'h1, evalstack.slot[evalstack.esp-32'h1]);
//TODO: Create lookup table obj=evalstack[esp-32'h1] -> hfp
            // Store val into the object field            
            evalstack.slot[evalstack.esp] <=#1 32'bx;
            evalstack.slot[evalstack.esp - 32'h1] <=#1 heap.hfp + heap.HF_FLD_0 + fldIndx;
            evalstack.esp <=#1 evalstack.esp - 32'h1;
            nextInstruction();
        end
    endtask
    
    // BOX
    task BOXx8C_task();
        begin
            $display("BOXx8C_task()");
            nextInstruction();
        end
    endtask
    
    // ISINST
    task ISINSTx75_task(input [31:0] inst);
        begin
            $display("ISINSTx75_task()");
            evalstack.slot[evalstack.esp] <=#1 inst; // wrong
            nextInstruction();
        end
    endtask
    
    // UNBOX
    task UNBOXx79_task();
        begin
            $display("UNBOXx79_task()");
            nextInstruction();
        end
    endtask
    
    // THROW
    task THROWx7A_task();
        begin
            $display("THROWx7A_task()");
            evalstack.slot[evalstack.esp] <=#1 32'bx;
            evalstack.esp <=#1 evalstack.esp - 32'h1;
            nextInstruction();  
        end
    endtask
    
    // LDFLDA
    task LDFLDAx7C_task(input [31:0] fldIndx);
        begin
            $display("LDFLDAx7C_task(input [31:0] fldIndx=%d), obj=%d", fldIndx, evalstack.slot[evalstack.esp]);
            $display("val=evalstack[esp=%d]=0x%x, obj=evalstack[esp-1=%d]=0x%x", evalstack.esp, evalstack.slot[evalstack.esp], evalstack.esp-32'h1, evalstack.slot[evalstack.esp-32'h1]);
//TODO: Create lookup table obj=evalstack[esp-32'h1] -> hfp
            // Store val into the object field            
            evalstack.slot[evalstack.esp + 32'h1] <=#1 heap.slot[heap.hfp + heap.HF_FLD_0 + fldIndx];
            evalstack.esp <=#1 evalstack.esp + 32'h1;
            nextInstruction();
        end
    endtask
    
    // STSFLD
    task STSFLDx80_task(input [31:0] fldIndx);
        begin
            $display("STSFLDx80_task");
            //evalstack.slot[evalstack.esp] =evalstack[esp=%d]=0x%x, obj=evalstack[esp-1=%d]=0x%x", esp, evalstack.slot[evalstack.esp], esp-32'h1, evalstack[esp-32'h1]);
//TODO: Create lookup table obj=evalstackevalstack.slot[evalstack.esp] hfp
            // Store val into the object field            
            evalstack.slot[evalstack.esp] <=#1 32'bx;
            heap.slot[heap.hfp + heap.HF_FLD_0 + fldIndx] <=#1 evalstack.slot[evalstack.esp - 32'h1] <= heap.slot[heap.hfp + heap.HF_FLD_0 + fldIndx];
            evalstack.esp <=#1 evalstack.esp - 32'h1;
            nextInstruction();
        end
    endtask
    
    // STOBJ
    task STOBJx81_task(input [31:0] typeTok);
        begin
            $display("STOBJx81_task()");
            evalstack.slot[evalstack.esp-32'h1] <=#1 evalstack.slot[evalstack.esp];
            evalstack.slot[evalstack.esp] <=#1 32'bx;
            evalstack.esp <=#1 evalstack.esp - 32'h1;
            nextInstruction();
        end
    endtask
    
    // NEWARR: ?, numElems -> ?, array
    task NEWARRx8D_task(input [31:0] etype);
        begin
            $display("NEWARRx8D_task(etype=%d)", etype);
            // aid is on top of stack after this
            //createArrayFrame(evalstack.slot[evalstack.esp], etype);
            //nextevalstack.slot[evalstack.esp]
        end
    endtask
    
    // LDLEN
    task LDLENx8E_task();
        begin
            $display("LDLENx8E_task()");
            //evalstack.slot[evalstack.esp] <= heap.aheap[heap.ahfp + heap.AHF_FLDS];
            nextInstruction();
        end
    endtask
    
    // LDELEMA: ?, array, index -> ?, addresseval
    task LDEMAx8F_task();
        begin
            $display("LDELEMAx8F_task()");
            //evalstack.slot[evalstack.esp-32'h1] <= heap.ahfp + heap.AHF_FLDS + evalstack.slot[evalstack.esp];
            //evalstack.slot[evalstack.esp] <= 32'bx;
            //esp <= -32'h1; 
            nextInstruction();            
        end
    endtask
    
    // LDELEMI1
    task LDELEMI1x90_task();
        begin
            $display("LDELEMI1x90_task()");
            LDELEMIx97_task();
        end
    endtask
    
    // LDELEMU1
    task LDELEMU1x91_task();
        begin
            $display("LDELEMU1x91_task()");
            LDELEMIx97_task();
        end
    endtask
    
    // LDELEMI2
    task LDELEMI2x92_task();
        begin
            $display("LDELEMI2x92_task()");
            LDELEMIx97_task();
        end
    endtask
    
    // LDELEMU2
    task LDELEMU2x93_task();
        begin
            $display("LDELEMU2x93_task()");
            LDELEMIx97_task();
        end
    endtask
    
    // LDELEMI4
    task LDELEMI4x94_task();
        begin
            $display("LDELEMI4x94_task()");
            LDELEMIx97_task();
        end
    endtask
    
    // LDELEMU4
    task LDELEMU4x95_task();
        begin
            $display("LDELEMU4x95_task()");
            LDELEMIx97_task();
        end
    endtask
    
    // LDELEMI8
    task LDELEMI8x96_task();
        begin
            $display("LDELEMI8x96_task()");
            LDELEMIx97_task();
        end
    endtask
    
    // LDELEMU8
    task LDELEMU8x96_task();
        begin
            $display("LDELEMU8x96_task()");
            LDELEMIx97_task();
        end
    endtask
    
    task LDELEMIx97_task();
        begin
            $display("LDELEMIx97_task()");
            //evalstack.slot[evalstack.esp-32'h1] <= heap.aheap[heap.ahfp + heap.AHF_FLDS + evalstack.slot[evalstack.esp]];
            //evalstack.slot[evalstack.esp] <= 32'bx;
            //evalstack.esp <= -32'h1; 
            nextInstruction();
        end
    endtask
    
    // LDELEMR4
    task LDELEMR4x98_task();
        begin
            $display("LDELEMR4x98_task()");
            LDELEMIx97_task();
        end
    endtask
    
    // LDELEMR8
    task LDELEMR8x99_task();
        begin
            $display("LDELEMR8x99_task()");
            LDELEMIx97_task();
        end
    endtask
    
    // LDELEMREF
    task LDELEMREFx9A_task();
        begin
            $display("LDELEMREFx9A_task()");
            LDELEMIx97_task();
        end
     endtask
         
    //evalstack.slot[evalstack.esp]array, index, value -> ?,
    task STELEMIx9B_task();
        begin
            $display("STELEMIx9B_task()");
            //heap.aheap[heap.ahfp + heap.AHF_FLDS + evalstack.slot[evalstack.esp-32'h1]] <= evalstack.slot[evalstack.esp]; 
            //evalstack.slot[evalstack.esp] <= 32'bx;
            //evalstack.slot[evalstack.esp-32'h1] <= 32'bx;
            //evalstack.slot[evalstack.esp-32'h2] <= 32'bx;
            //evalstack.esp <= evalstack.esp - 32'h3; 
            nextInstruction();
        end
    endtask
    
    // STELEMI1
    task STELEMI1x9C_task();
        begin
            $display("STELEMI1x9C_task()");
            STELEMIx9B_task();
        end
    endtask
    
    // STELEMI2
    task STELEMI2x9D_task();
        begin
            $display("STELEMI2x9D_task()");
            STELEMIx9B_task();
        end
    endtask
    
    // STELEMI4
    task STELEMI4x9E_task();
        begin
            $display("STELEMI4x9E_task()");
            STELEMIx9B_task();
        end
    endtask
    
    // STELEMR4
    task STELEMR4xA0_task();
        begin
            $display("STELEMR4xA0_task()");
            STELEMIx9B_task();
        end
    endtask
    
    // STELEMR8
    task STELEMR8xA1_task();
        begin
            $display("STELEMR8xA1_task()");
            STELEMIx9B_task();
        end
    endtask
    
    // STELEMREF
    task STELEMREFxA2_task();
        begin
            $display("STELEMREFxA2_task()");
            STELEMIx9B_task();
        end
    endtask
    
    // LDELEM
    task LDELEMxA3_task();
        begin
            $display("LDELEMxA3_task()");
            LDELEMIx97_task();
        end
    endtask
    
    // STELEM
    task STELEMxA4_task();
        begin
            $display("STELEMxA4_task()");
            STELEMIx9B_task();
        end
    endtask
    
    // UNBOXANY
    task UNBOXANYxA5_task();
        begin
            $display("UNBXANYxA5_task()");
            nextInstruction();
        end
    endtask
    
    // REFANYVAL: ?, TypedRef -> ?, address
    task REFANYVALxC2_task(input [31:0] aType);
        begin
            $display("REFANYVALxC2_task()");
            evalstack.slot[evalstack.esp] <=#1 aType;
            nextInstruction();
        end
    endtask
    
    // MKREFANY: ?, ptr -> ?, typedRef
    task MKREFANYxC6_task(input [31:0] aClass);
        begin
            $display("MKREFANYxC6_task()");
            evalstack.slot[evalstack.esp] <=#1 aClass;
            nextInstruction();
        end
    endtask
    
    // LDTOKEN: ? -> ?, RuntimeHandle
    task LDTOKENxD0_task(input [31:0] aToken);
        begin
            $display("LDTOKENxD0_task()");
            evalstack.slot[evalstack.esp + 32'h1] <=#1 aToken;
            evalstack.esp <=#1 evalstack.esp + 32'h1;
            nextInstruction();
        end
    endtask
    
    // LDVIRTFN: ? object -> ?, ftn
    task LDVIRTFNxFE07_task(input [31:0] aMethod);
        begin
            $display("LDVIRTFNxFE07_task()");
            evalstack.slot[evalstack.esp] <=#1 aMethod;
            nextInstruction();
        end
    endtask
    
    // INITOBJ
    task INITOBJxFE15_task();
        begin
            $display("INITOBJxFE15_task()");
            nextInstruction();
        end
    endtask
    
    // RETHROW
    task RETHROWxFE1A_task();
        begin
            $display("RETHROWxFE1A_task()");
            nextInstruction();
        end
    endtask
    
    // SIZEOF: ?, -> ?, size (4 bytes, unsigned)
    task SIZEOFxFE1C_task(input [31:0] aTypeTok);
        begin
            $display("SIevalstack.slot[evalstack.esp]()");
            // ???
            evalstack.slot[evalstack.esp+32'h1] <=#1 4;
            evalstack.esp <=#1 evalstack.esp + 32'h1;
            nextInstruction();
        end
    endtask
    
    // REFANYTYPE: ?, TypedRef -> ?, type
    task REFANYTYPExFE1D_task();
        begin
            $display("REFANYTYPExFE1D_task()");
            evalstack.slot[evalstack.esp] <=#1 evalstack.slot[evalstack.esp];
            nextInstruction();
        end
    endtask
    
    
    // SUPPORT TASKS
    
    // increment pc
    task nextInstruction();
        begin
            $display("");
            //$display("nextInstruction(): cid=%0d, mid=%0d", heap.cit_id, cls.prevcid, cls.mid);
            //$display("                   old pc=0x%0x, new pc=0x%0x", stack.pc, stack.pc + tables.CIL_SIZE);
            stack.pc <=#1 stack.pc + tables.CIL_SIZE;
        end
    endtask
    
    task decodeError();
        begin
            $display("decodeError()");
            $display("Could not decode opcode: cit_mit_cil[pc=0x%0x]=0x%0x", stack.pc, tables.cil[stack.pc]);
            $finish;
        end
    endtask
    
    // Primitive memory copying op to 4 words from eval stack to stack
    // when setting up a new stack frame
    task copyWords(input [31:0] fromEIndex, input [31:0] toSIndex, input [31:0] cnt);
        begin
            $display("copyWords(input [31:0] fromEIndex=%0d, input [31:0] toSIndex=%0d, input [31:0] cnt=%0d)", fromEIndex, toSIndex, cnt);
            if(cnt == 1)
                begin
                    $display("Copying evalstack[fromEIndex=%0d]=0x%0x", fromEIndex, evalstack.slot[fromEIndex]);
                    stack.slot[toSIndex] <=#1 evalstack.slot[fromEIndex];
                end 
                  
            if(cnt == 2)
                begin
                    $display("Copying evalstack[fromEIndex=%0d+32'h1]=0x%0x", fromEIndex, evalstack.slot[fromEIndex+32'h1]);
                    stack.slot[toSIndex+32'h1] <=#1 evalstack.slot[fromEIndex+32'h1];
                end
              
            if(cnt == 3)
                begin
                    $display("Copying evalstack[fromEIndex=%0d+32'h2]=0x%0x", fromEIndex, evalstack.slot[fromEIndex+32'h2]);
                    evalstack.slot[toSIndex+32'h2] <=#1 evalstack.slot[fromEIndex+32'h2];
                end
            
            if(cnt == 4)
                begin
                    $display("Copying evalstack[fromEIndex=%0d+32'h3]=0x%0x", fromEIndex, evalstack.slot[fromEIndex+32'h3]);
                    evalstack.slot[toSIndex+32'h3] <=#1 evalstack.slot[fromEIndex+32'h3];
                end
        end
    endtask
    
    // Set some value on the stack
    task setValues(input [31:0] startIndx, input [31:0] cnt, input [31:0] val);
        begin
         $display("setValues(input [31:0] startIndx=%0d, input [31:0] cnt=%0d, input [31:0] val=%0d)", startIndx, cnt, val);
         if(cnt == 1)
             begin
                 $display("Setting stack[startIndx=%d]=x%0x", startIndx, val);
                 stack.slot[startIndx] <=#1 val;
             end 
               
         if(cnt == 2)
             begin
                 $display("Setting stack[startIndx=%d]=0x%0x", startIndx+32'h1, val);
                 stack.slot[startIndx+32'h1] <=#1 val;
             end
           
         if(cnt == 3)
             begin
                 $display("Setting stack[startIndx=%0d]=0x%0x", startIndx+32'h2, val);
                 stack.slot[startIndx+32'h2] <=#1 val;
             end
         
         if(cnt == 4)
             begin
                 $display("Setting stack[startIndx=%0d]=0x%0x", startIndx+32'h3, val);
                 stack.slot[startIndx+32'h3] <=#1 val;
             end
        end
    endtask
     
endmodule

/*******************************************************************************
Information:

Definitions
Class life: Classes are instanciated on the heap. Classes have fields as state.
            The (*)this pointer is managed. The GC takes care of removing dead
            class instances. 
Methods:    Metods are instanciated on the stack. Methods take arguments and
            have local variables as state. The *SP pointer and *FP are associa-
            with the method. 
CIL code:   The CIL code belongs to a method. The *PC points to the current CIL
            instruction in the current method. There are three types of instruc-
            tions: Sequential (arithmetic), branching (if), and OO (new, field
            loads, and method calls) instructions.  

Only one instruction in one method in one class is run at a time.
Only one instruction in one stack frame is run at a time. 
Only one instruction in the top stack frame is run at the time. 
At start, one class is instanciated and a static metod is called automatically. 
After start, either a new class/new method or a member method is called.
Method frames follow class life.
Heap frames follow class life.
At the end the (static) first method will exit.           

Source code is compiled into a class information table (CIT) and method information
tables (MIT). At runtime a class is instanciated as a heap frame (HF) identified
by a class instance id (CIID). Then a stack frame (SF) is created for the method
call and it is identified by a method invocation id (MIID). The MIID has a
reference to the CIID via an indirection table (used by the garbage collector
(GCID)).

Class information table (CIT):
It holds general class information, such as the class name, the number and type
of the fields. In addition, there is information about the methods of this
class. 

Method information table (MIT):
The method information table (MIT) is owned by a class. It has general compile-
time information such as number of arguments, local variables and the CIL in-
structions. Furthermore, it has information about the maximum size of the local
evaluation stack. 

At runtime, these two static structures - CIT and MIT - are the basis of the dy-
namic behaviour of the system. The three main dynamic structures are the class
instance identification table (CIID), the metod invocation identification table
(MIID) and the link between the two is the method invocation identifier to
class instance identifier reference->heap frame (HF) address to CIID table.

 STATIC        RUNTIME
 =======  ==================
 | CIT |  |    CIID_HF     |
 |     |  ==================
 =======  | MIID_CIID -> & |
 | MIT |  ==================
 |     |  |     MIID_SF    |
 =======  ==================

CIT: Class information table
The CIT has information regarding the class itself and its fields.
=CITID
=CNAME
=NUM_FIELDS
{*}FIELD_NAME/FIELD_TYPE

MIT: Method information table
The MIT has information about the number of locals/arguments as well as max.
(local) eval. stack.
=NUM_LOCALS
=NUM_ARGUMENTS
=MAX_STACK
{*}LOCAL_NAME/LOCAL_TYPE
{*}ARGUMENT_NAME/ARGUMENT_TYPE

CIID_HF: Class instance identification heap frame.
The instanciated class has a dynamic class instance reference id (CIRID).
=CIRID (PK)
=CIT
=(NUM_FIELDS)
*(FIELD_TYPE)

MID_CIRID-> &: Mapping from Method identification stack frame to the actual
memory address of the CIID_HF
The MID_CIRID table has the mapping to the CIRID. 
=CIRID
=HFADR
=(GCSTATUS)

MID_SF: Method identifier stack frame.
The instanciated method stack frame is identified by the MIID. (It makes sense
to call it method instance to capture the fact the each method can be recursive
/reentrant). 
=MID
=MIT
=CIRID->
=CILENTRY
=NUM_LOCALS
=LOCALS_OFFSET
=NUM_ARGUMENTS
=ARGUMENTS_PTR
=SIZE_EVALSTACK
=EVALSTACL_PTR
=LR
=(PC)
=(SP/TOS)
*LOCALS
*ARGS
*EVAL_STACK

Tokens
The CIL tokens are composed of a 1-byte CIL table id, and then followed by a
3-byte releative index identifier (RID). 

*******************************************************************************/


/*******************************************************************************
VERSION (Year.Month)
15.6: Changed ves.v to ves.sv with task driven opcodes, always_, and logic type.
*******************************************************************************/


/*******************************************************************************
TODO
*******************************************************************************/

// * FOP is a multiparadigm processor

//// Typedefs

//// MIT: Method information table
//typedef struct {
//    logic [7:0]  mid;               // method id
//    logic [7:0]  numLocals;         // number of locals
//    logic [7:0]  numArguments;      // number of arguments
//    logic [7:0]  maxEvalStack;      // max. evaluation stack height
//// This should be replaced with a CIL start index
//// for the method into the opcode and operand tables
//// In principle, I could still use the cit_t structure and control
//// the active class and metod index. It would make debugging easier?        
//    cil_t    cil[0:3];              // CIL opcodes
//} mit_t;

//// CIT: Class information table
//typedef struct { 
//    logic [7:0] cid;                // class id
//    logic [7:0] numFields;          // number of class fields
//    logic [7:0] numMethods;         // number of methods
//    logic [7:1] fieldType[0:3];     // field types
//    mit_t       mit[0:0];              // methods        
//} cit_t;  
    
//// CIL: One CIL instruction
//typedef struct {
//    logic [15:0] cilOpcode;
//    logic [31:0] cilOperand;
//} cil_t;

    // Load local variable onto the stack
    // [local x] -> tos
    // Partition 3.43 
    // ldlocx11
    // ldloc.s
    // This requires that there is (1) method speciffic information regarding the relative start posistion of the locals, (2) there must 
    //     there must also be a frame pointer that points to the first location of the method. It can be pointing to tos if there are no
    //     arguments or locals. 
    //
    // A MIT (Method Information Table) can look like this:
    // [U + 4]: Number of locals
    // [U + 0]: Number of arguments
    //
    // A SF (Stack Frame) can look like this:
    //       [Y + 16]: tos
    //       [Y + 12]: tos - 1
    //         ...
    //       [Y + 8]: Local.[0] (if any)
    //         ...
    // FP -> [Y + 0]: Argument.[0] (if any)
    //
    // There must also (at some point) be a method indirection table. It can be indexed by a MID (Method ID) which (via a "struct"?) provides information 
    //     regarding (1) the MIT and (2) the FP for that particular method. 
    // 
    // *First implementation:* 
    // I pretend that I am in a method that has no arguments and just one local. 
    // Initially: 
    // [SP = FP + 4]    tos = empty
    // [FP + 4]         the local 123
    // [FP + 0]         "this"
    // 
    // Then (after the load local instruction):
    // [SP = FP + 8]    tos = 123
    // [FP + 4]         the local 123 
    // [FP + 0]         "this"
    // 
    // Then (after the load constant "246" instruction):
    // [SP = FP + 12]   tos = 246
    // [SP = FP + 8]    123
    // [FP + 4]         the local 123 
    // [FP + 0]         "this"
    // 
    // Then (after the add instruction):
    // [SP = FP + 8]    tos = 369
    // [FP + 4]         the local 123 
    // [FP + 0]         "this"   
    // 
    // Then (after the save local.[0] instruction):
    // [SP = FP + 4]    tos = empty
    // [FP + 4]         the local 369 
    // [FP + 0]         "this"   
      
    // So I need to know the memory index of the local (in an offset to the FP), but since there are no arguments and this is the only local, then 
    // the FP points to this particular local with offset 0. 
    
// Array of classes
//        cit_t cit[0:2] = '{default:0};
//        // Active class
//    // This one must be mapped when the there is a method call
//        logic [7:0] aC = 8'hFF;     // FF means that there is no active class
//        // Active method
//    // ... and so must this one
//        logic [7:0] aM = 8'hFF;     // FF means that there is no active method

//    initial begin
//        // Class 0
//        cit[0].cid                        = 8'h0;
//        cit[0].numFields                  = 8'h1;
//        cit[0].numMethods                 = 8'h1;
//        cit[0].fieldType[0]               = 8'h1; // int
//        cit[0].mit[0].mid                 = 8'h0;
//        cit[0].mit[0].numLocals           = 8'h1;
//        cit[0].mit[0].numArguments        = 8'h1;
//        cit[0].mit[0].maxEvalStack        = 8'h1;
//        cit[0].mit[0].cil[0].cilOpcode    = 16'h20;   
//        cit[0].mit[0].cil[0].cilOperand   = 32'h0000_0012;
//        cit[0].mit[0].cil[1].cilOpcode    = 16'h20;   
//        cit[0].mit[0].cil[1].cilOperand   = 32'h0000_0013;
//        cit[0].mit[0].cil[2].cilOpcode    = 16'h58;   
//        cit[0].mit[0].cil[2].cilOperand   = NOPx00;
//        cit[0].mit[0].cil[3].cilOpcode    = 16'h38;   
//        cit[0].mit[0].cil[3].cilOperand   = -32'h0000_0003;
//        cit[0].mit[0].cil[4].cilOpcode    = 16'h00;   
//        cit[0].mit[0].cil[4].cilOperand   = 32'h0000_0000;                    
//    end
//           // ldc i4 -> x12
//            opcmem[0] <= 16'h0020;
//            oprmem[0] <= 32'h12;
//            // ldc i4 -> x13
//            opcmem[1] <= 16'h0020;
//            oprmem[1] <= 32'h13;
//            // add tos0, tos1 -> res
//            opcmem[2] <= 16'h0058;
//            oprmem[2] <= 32'h0;
//            // branch back
//            opcmem[3] <= 16'h0038;
//            oprmem[3] <= -32'h3;
//            // nop
//            opcmem[4] <= 16'h0000;
//            oprmem[4] <= 32'h0;             
    // 16-bit instructions indexed by pc
//logic [15:0] opcmem[0:255];
// and the "optional" 32-bit operand
//logic [31:0] oprmem[0:255];

/* PLANNING */
/*
    FOP.sv: Possible to do initialize memory and do a method call and return. 
    
    PCB Design 1: Artix-7 connected to FTDI 4232 with header pins.
*/

//    initial begin
//        // Class 0
////        cit[cit_index + CIT_ID_INDEX]                               = 32'h0;
////        cit[cit_index + CIT_NUMFIELDS_INDEX]                        = 32'h1;
////        cit[cit_index + CIT_NUMMETHODS_INDEX]                       = 32'h1;
////        cit_index = cit_index + CIT_SIZE;
////        // Class Fields
////        cit_fit[cit_fit_index + CIT_FIT_ID_INDEX]                   = 32'h0;
////        cit_fit[cit_fit_index + CIT_FIT_FIELDTYPE_INDEX]            = 32'h2;
////        cit_fit_index = cit_fit_index + CIT_FIT_SIZE;
////        // Class Methods
////        cit_mit[cit_mit_index + CIT_MIT_ID_INDEX]                   = 32'h0;
////        cit_mit[cit_mit_index + CIT_MIT_NUMARGS_INDEX]              = 32'h1;
////        cit_mit[cit_mit_index + CIT_MIT_NUMLOCALS_INDEX]            = 32'h1;
////        cit_mit[cit_mit_index + CIT_MIT_MAXEVALSTACK_INDEX]         = 32'h3;
////        cit_mit[cit_mit_index + CIT_MIT_NUMCIL_INDEX]               = 32'h2;
////        cit_mit_index = cit_mit_index + CIT_MIT_SIZE;
////        // Class Method Arguments
////        cit_mit_at[cit_mit_at_index + CIT_MIT_AT_ID_INDEX]          = 32'h0;
////        cit_mit_at[cit_mit_at_index + CIT_MIT_AT_TYPE_INDEX]        = 32'h4;
////        cit_mit_at_index = cit_mit_at_index + CIT_MIT_AT_SIZE;
////        // Class Method Locals
////        cit_mit_lt[cit_mit_lt_index + CIT_MIT_LT_ID_INDEX]          = 32'h0;
////        cit_mit_lt[cit_mit_lt_index + CIT_MIT_LT_TYPE_INDEX]        = 32'h5;
////        cit_mit_lt_index = cit_mit_lt_index + CIT_MIT_LT_SIZE; 
////        // Class Method CIL
////        cit_mit_cil[cit_mit_cil_index + CIT_MIT_CIL_ID_INDEX]       = 32'h0;
////        cit_mit_cil[cit_mit_cil_index + CIT_MIT_CIL_OPCODE_INDEX]   = LDCI4x20;
////        cit_mit_cil[cit_mit_cil_index + CIT_MIT_CIL_OPERAND_INDEX]  = 32'h5;
////        cit_mit_cil_index = cit_mit_cil_index + CIT_MIT_CIL_SIZE;
////        cit_mit_cil[cit_mit_cil_index + CIT_MIT_CIL_ID_INDEX]       = 32'h1;
////        cit_mit_cil[cit_mit_cil_index + CIT_MIT_CIL_OPCODE_INDEX]   = BRx38;
////        cit_mit_cil[cit_mit_cil_index + CIT_MIT_CIL_OPERAND_INDEX]  = -32'h3;
////        cit_mit_cil_index = cit_mit_cil_index + CIT_MIT_CIL_SIZE;
//    end

//       Decimal   Octal   Hex    Binary     Value
//       -------   -----   ---    ------     -----
//         000      000    000   00000000      NUL    (Null char.)
//         001      001    001   00000001      SOH    (Start of Header)
//         002      002    002   00000010      STX    (Start of Text)
//         003      003    003   00000011      ETX    (End of Text)
//         004      004    004   00000100      EOT    (End of Transmission)
//         005      005    005   00000101      ENQ    (Enquiry)
//         006      006    006   00000110      ACK    (Acknowledgment)
//         007      007    007   00000111      BEL    (Bell)
//         008      010    008   00001000       BS    (Backspace)
//         009      011    009   00001001       HT    (Horizontal Tab)
//         010      012    00A   00001010       LF    (Line Feed)
//         011      013    00B   00001011       VT    (Vertical Tab)
//         012      014    00C   00001100       FF    (Form Feed)
//         013      015    00D   00001101       CR    (Carriage Return)
//         014      016    00E   00001110       SO    (Shift Out)
//         015      017    00F   00001111       SI    (Shift In)
//         016      020    010   00010000      DLE    (Data Link Escape)
//         017      021    011   00010001      DC1    (XON) (Device Control 1)
//         018      022    012   00010010      DC2          (Device Control 2)
//         019      023    013   00010011      DC3    (XOFF)(Device Control 3)
//         020      024    014   00010100      DC4          (Device Control 4)
//         021      025    015   00010101      NAK    (Negative Acknowledgement)
//         022      026    016   00010110      SYN    (Synchronous Idle)
//         023      027    017   00010111      ETB    (End of Trans. Block)
//         024      030    018   00011000      CAN    (Cancel)
//         025      031    019   00011001       EM    (End of Medium)
//         026      032    01A   00011010      SUB    (Substitute)
//         027      033    01B   00011011      ESC    (Escape)
//         028      034    01C   00011100       FS    (File Separator)
//         029      035    01D   00011101       GS    (Group Separator)
//         030      036    01E   00011110       RS    (Request to Send)(Record Separator)
//         031      037    01F   00011111       US    (Unit Separator)
//         032      040    020   00100000       SP    (Space)
//         033      041    021   00100001        !    (exclamation mark)
//         034      042    022   00100010        "    (double quote)
//         035      043    023   00100011        #    (number sign)
//         036      044    024   00100100        $    (dollar sign)
//         037      045    025   00100101        %    (percent)
//         038      046    026   00100110        &    (ampersand)
//         039      047    027   00100111        '    (single quote)
//         040      050    028   00101000        (    (left/opening parenthesis)
//         041      051    029   00101001        )    (right/closing parenthesis)
//         042      052    02A   00101010        *    (asterisk)
//         043      053    02B   00101011        +    (plus)
//         044      054    02C   00101100        ,    (comma)
//         045      055    02D   00101101        -    (minus or dash)
//         046      056    02E   00101110        .    (dot)
//         047      057    02F   00101111        /    (forward slash)
//         048      060    030   00110000        0
//         049      061    031   00110001        1
//         050      062    032   00110010        2
//         051      063    033   00110011        3
//         052      064    034   00110100        4
//         053      065    035   00110101        5
//         054      066    036   00110110        6
//         055      067    037   00110111        7
//         056      070    038   00111000        8
//         057      071    039   00111001        9
//         058      072    03A   00111010        :    (colon)
//         059      073    03B   00111011        ;    (semi-colon)
//         060      074    03C   00111100        <    (less than)
//         061      075    03D   00111101        =    (equal sign)
//         062      076    03E   00111110        >    (greater than)
//         063      077    03F   00111111        ?    (question mark)
//         064      100    040   01000000        @    (AT symbol)
//         065      101    041   01000001        A
//         066      102    042   01000010        B
//         067      103    043   01000011        C
//         068      104    044   01000100        D
//         069      105    045   01000101        E
//         070      106    046   01000110        F
//         071      107    047   01000111        G
//         072      110    048   01001000        H
//         073      111    049   01001001        I
//         074      112    04A   01001010        J
//         075      113    04B   01001011        K
//         076      114    04C   01001100        L
//         077      115    04D   01001101        M
//         078      116    04E   01001110        N
//         079      117    04F   01001111        O
//         080      120    050   01010000        P
//         081      121    051   01010001        Q
//         082      122    052   01010010        R
//         083      123    053   01010011        S
//         084      124    054   01010100        T
//         085      125    055   01010101        U
//         086      126    056   01010110        V
//         087      127    057   01010111        W
//         088      130    058   01011000        X
//         089      131    059   01011001        Y
//         090      132    05A   01011010        Z
//         091      133    05B   01011011        [    (left/opening bracket)
//         092      134    05C   01011100        \    (back slash)
//         093      135    05D   01011101        ]    (right/closing bracket)
//         094      136    05E   01011110        ^    (caret/circumflex)
//         095      137    05F   01011111        _    (underscore)
//         096      140    060   01100000        `
//         097      141    061   01100001        a
//         098      142    062   01100010        b
//         099      143    063   01100011        c
//         100      144    064   01100100        d
//         101      145    065   01100101        e
//         102      146    066   01100110        f
//         103      147    067   01100111        g
//         104      150    068   01101000        h
//         105      151    069   01101001        i
//         106      152    06A   01101010        j
//         107      153    06B   01101011        k
//         108      154    06C   01101100        l
//         109      155    06D   01101101        m
//         110      156    06E   01101110        n
//         111      157    06F   01101111        o
//         112      160    070   01110000        p
//         113      161    071   01110001        q
//         114      162    072   01110010        r
//         115      163    073   01110011        s
//         116      164    074   01110100        t
//         117      165    075   01110101        u
//         118      166    076   01110110        v
//         119      167    077   01110111        w
//         120      170    078   01111000        x
//         121      171    079   01111001        y
//         122      172    07A   01111010        z
//         123      173    07B   01111011        {    (left/opening brace)
//         124      174    07C   01111100        |    (vertical bar)
//         125      175    07D   01111101        }    (right/closing brace)
//         126      176    07E   01111110        ~    (tilde)
//         127      177    07F   01111111      DEL    (delete)
      //$readmemh("D:/Dropbox/sw/w3op/Vivado/tcltest/cit.hex", cit);
//$readmemh("D:/Dropbox/sw/w3op/Vivado/tcltest/cit_fit.hex", cit_fit);
//$readmemh("D:/Dropbox/sw/w3op/Vivado/tcltest/cit_mit.hex", cit_mit);
//$readmemh("D:/Dropbox/sw/w3op/Vivado/tcltest/cit_mit_at.hex", cit_mit_at);
//$readmemh("D:/Dropbox/sw/w3op/Vivado/tcltest/cit_mit_lt.hex", cit_mit_lt);
//$readmemh("D:/Dropbox/sw/w3op/Vivado/tcltest/cit_mit_cil.hex", cit_mit_cil);
//$readmemh("cit.hex", cit);
//$readmemh("cit_fit.hex", cit_fit);
//$readmemh("cit_mit.hex", cit_mit);
//$readmemh("cit_mit_at.hex", cit_mit_at);
//$readmemh("cit_mit_lt.hex", cit_mit_lt);
//$readmemh("cit_mit_cil.hex", cit_mit_cil);

//    // Index of next unused slot in each table
//    logic [7:0] cit_index           = 8'h0;
//    logic [7:0] cit_fit_index       = 8'h0;
//    logic [7:0] cit_mit_index       = 8'h0;
//    logic [7:0] cit_mit_at_index    = 8'h0;
//    logic [7:0] cit_mit_lt_index    = 8'h0;
//    logic [7:0] cit_mit_cil_index   = 8'h0;

    //==CIL parameter encodings==                                                    //
    //parameter [31:0] NOPx00    = 32'h0000; // ->
    //parameter [31:0] BREAKx01  = 32'h0001; // ->
//    parameter [31:0] LDARG0x02 = 32'h0002; // -> value
//    parameter [31:0] LDARG1x03 = 32'h0003; // -> value
//    parameter [31:0] LDARG2x04 = 32'h0004; // -> value
//    parameter [31:0] LDARG3x05 = 32'h0005; // -> value
//    parameter [31:0] LDLOC0x06 = 32'h0006; // -> value
//    parameter [31:0] LDLOC1x07 = 32'h0007; // -> value
//    parameter [31:0] LDLOC2x08 = 32'h0008; // -> value
//    parameter [31:0] LDLOC3x09 = 32'h0009; // -> value
//    parameter [31:0] STLOC0x0A = 32'h000A; // ., value -> .
//    parameter [31:0] STLOC1x0B = 32'h000B; // ., value -> .
//    parameter [31:0] STLOC2x0C = 32'h000C; // ., value -> .
//    parameter [31:0] STLOC3x0D = 32'h000D; // ., value -> .
//    parameter [31:0] STARGSx10 = 32'h0010; // ., value -> ., 
//    parameter [31:0] LDLOCSx11 = 32'h0011; // -> value
//    parameter [31:0] LDLOCASx12= 32'h0012; // -> address
//    parameter [31:0] STLOCSx13 = 32'h0013; // ., value -> .
//    parameter [31:0] LDNULLx14 = 32'h0014; // -> null, value
//    parameter [31:0] LDCI4M1x15= 32'h0015; // -> num

//    parameter [31:0] LDCI40x16 = 32'h0016; // -> num
//    parameter [31:0] LDCI41x17 = 32'h0017; // -> num
//    parameter [31:0] LDCI42x18 = 32'h0018; // -> num
//    parameter [31:0] LDCI43x19 = 32'h0019; // -> num
//    parameter [31:0] LDCI44x1A = 32'h001A; // -> num
//    parameter [31:0] LDCI45x1B = 32'h001B; // -> num
//    parameter [31:0] LDCI46x1C = 32'h001C; // -> num
//    parameter [31:0] LDCI47x1D = 32'h001D; // -> num
//    parameter [31:0] LDCI48x1E = 32'h001E; // -> num
//    parameter [31:0] LDCI44Sx1F = 32'h001F; // -> num
    
//    parameter [31:0] LDARGSx0E = 32'h000E; // -> value
//    parameter [31:0] LDARGASx0F= 32'h000F; // -> -> address of argument number argNum
//    parameter [31:0] LDCI4x20  = 32'h0020; // -> num
//    parameter [31:0] LDCI8x21  = 32'h0021; // -> num
//    parameter [31:0] LDCR4x22  = 32'h0022; // -> num
//    parameter [31:0] LDCR8x23  = 32'h0023; // -> num
//    parameter [31:0] DUPx25    = 32'h0025; // value -> value, value
//    parameter [31:0] POPx26    = 32'h0026; // ., value -> .
//    parameter [31:0] JMPx27    = 32'h0027; // ->
//    parameter [31:0] CALLx28   = 32'h0028; // ?                  -> .
//    parameter [31:0] CALLIx29  = 32'h0029; // arg0, arg1, . , argn, ftn -> retVal (sometimes)
//    parameter [31:0] BRSx2B    = 32'h002B; // ->
//    parameter [31:0] BRFALSESx2C= 32'h002C; // value ->
//    parameter [31:0] BRTRUESx2D= 32'h002D; // value ->
//    parameter [31:0] BEQSx2E   = 32'h002E; // value1, value2 ->
//    parameter [31:0] BGESx2F   = 32'h002F; // value1, value2 ->
//    parameter [31:0] BGTSx30   = 32'h0030; // value1, value2 ->
//    parameter [31:0] BLESx31   = 32'h0031; // value1, value2 ->
//    parameter [31:0] BLTSx32   = 32'h0032; // value1, value2 ->
//    parameter [31:0] BNEUNSx33 = 32'h0033; // value1, value2 ->
//    parameter [31:0] BGEUNSx34 = 32'h0034; // value1, value2 ->
//    parameter [31:0] BGTUNSx35 = 32'h0035; // value1, value2 ->
//    parameter [31:0] BLEUNSx36 = 32'h0036; // value1, value2 ->
//    parameter [31:0] BLTUNSx37 = 32'h0037; // value1, value2 ->
//    parameter [31:0] BRx38     = 32'h0038; // ?                  -> ?
//    parameter [31:0] BRFALSEx39= 32'h0039; // value ->
//    parameter [31:0] BRTRUEx3A = 32'h003A; // value ->
//    parameter [31:0] BRINSTx3A = 32'h003A; // value ->
//    parameter [31:0] BEQx3B    = 32'h003B; // value1, value2 ->
//    parameter [31:0] BGEx3C    = 32'h003C; // value1, value2 ->
//    parameter [31:0] BGTx3D    = 32'h002E; // value1, value2 ->
//    parameter [31:0] BLEx3E    = 32'h003E; // value1, value2 ->
//    parameter [31:0] BLTx3F    = 32'h003F; // ?                  -> ?
//    parameter [31:0] BNEUNx40  = 32'h0040; // value1, value2 ->
//    parameter [31:0] BGEUNx41  = 32'h0041; // value1, value2 ->
//    parameter [31:0] BGTUNx42  = 32'h0042; // value1, value2 ->
//    parameter [31:0] BLEUNx43  = 32'h0043; // value1, value2 -> 
//    parameter [31:0] BLTUNx44  = 32'h0044; // value1, value2 ->
//    parameter [31:0] SWITCHx45 = 32'h0045; // ., value -> .
//    parameter [31:0] LDINDI1x46= 32'h0046; // addr -> value
//    parameter [31:0] LDINDU1x47= 32'h0047; // addr -> value
//    parameter [31:0] LDINDI2x48= 32'h0048; // addr -> value
//    parameter [31:0] LDINDU2x49= 32'h0049; // addr -> value
//    parameter [31:0] LDINDI4x4A= 32'h004A; // addr -> value
//    parameter [31:0] LDINDU4x4B= 32'h004B; // addr -> value
//    parameter [31:0] LDINDI8x4C= 32'h004C; // addr -> value
//    parameter [31:0] LDINDU8x4C= 32'h004C; // addr -> value
//    parameter [31:0] LDINDIx4D = 32'h004D; // addr -> value
//    parameter [31:0] LDINDR4x4E= 32'h004E; // addr -> value
//    parameter [31:0] LDINDR8x4F= 32'h004F; // addr -> value
//    parameter [31:0] LDINDREFx50= 32'h0050; // addr -> value
//    parameter [31:0] STINDREFx51= 32'h0051; // ., addr, val -> .
//    parameter [31:0] STNDI1x52 = 32'h0052; // ., addr, val -> .
//    parameter [31:0] STINDI2x53= 32'h0053; // ., addr, val -> .
//    parameter [31:0] STINDI4x54= 32'h0054; // ., addr, val -> .
//    parameter [31:0] STINDI8x55= 32'h0055; // ., addr, val -> .
//    parameter [31:0] STINDR4x56= 32'h0056; // ., addr, val -> .
//    parameter [31:0] STINDR8x57= 32'h0057; // ., addr, val -> .
//    parameter [31:0] ADDx58   = 32'h0058; // x,y -> z
//    parameter [31:0] SUBx59   = 32'h0059; // ., value1, value2 -> ., result
//    parameter [31:0] MULx5A   = 32'h005A; // ., value1, value2 -> ., result
//    parameter [31:0] DIVx5B   = 32'h005B; // value1, value2 -> result
//    parameter [31:0] DIVUNx5C = 32'h005C; // value1, value2 -> result
//    parameter [31:0] REMx5D   = 32'h005D; // ., value1, value2 -> ., result
//    parameter [31:0] REMUNx5E = 32'h005E; // ., value1, value2 -> ., result
//    parameter [31:0] ANDx5F   = 32'h005F; // ?, value1, value2 -> ?, result
//    parameter [31:0] ORx60    = 32'h0060; // ., value1, value2 -> ., result
//    parameter [31:0] XORx61   = 32'h0061; // ., value1, value2 -> ., result
//    parameter [31:0] SHLx62   = 32'h0062; // value, shiftAmount -> result
//    parameter [31:0] SHRx63   = 32'h0063; // value, shiftAmount -> result
//    parameter [31:0] SHRUNx64 = 32'h0064; // value, shiftAmount -> result    
//    parameter [31:0] NEGx65   = 32'h0065; // ., value -> ., result
//    parameter [31:0] NOTx66   = 32'h0066; // ., value -> ., result 
//    parameter [31:0] CONVI1x67= 32'h0067; // value -> result
//    parameter [31:0] CONVI2x68= 32'h0068; // value -> result
//    parameter [31:0] CONVI4x69= 32'h0069; // value -> result
//    parameter [31:0] RETx2A   = 32'h002A; // ?                  -> . (empty except for returned value)
//    parameter [31:0] CONVI8x6A= 32'h006A; // value -> result
//    parameter [31:0] CONVR4x6B= 32'h006B; // value -> result
//    parameter [31:0] CONVR8x6C= 32'h006C; // value -> result
//    parameter [31:0] CONVU4x6D= 32'h006D; // value -> result
//    parameter [31:0] CONVU8x6E= 32'h006E; // value -> result
    
//    parameter [31:0] CONVRUNx76 = 32'h0076; // value -> result
//    parameter [31:0] LDFLDx7B = 32'h007B; // ., obj             -> ., value
    
//    parameter [31:0] CONVOVFI1UNx82 = 32'h0082; // value -> result
//    parameter [31:0] CONVOVFI2UNx83 = 32'h0083; // value -> result
//    parameter [31:0] CONVOVFI4UNx84 = 32'h0084; // value -> result
//    parameter [31:0] CONVOVFI8UNx85 = 32'h0085; // value -> result
//    parameter [31:0] CONVOVFU1UNx86 = 32'h0086; // value -> result
//    parameter [31:0] CONVOVFU2UNx87 = 32'h0087; // value -> result
//    parameter [31:0] CONVOVFU4UNx88 = 32'h0088; // value -> result
//    parameter [31:0] CONVOVFU8UNx89 = 32'h0089; // value -> result
//    parameter [31:0] CONVOVFIUNx8A = 32'h008A; // value -> result
//    parameter [31:0] CONVOVFUUNx8B = 32'h008B; // value -> result
//    parameter [31:0] CONVOVFI1xB3 = 32'h00B3; // value -> result
//    parameter [31:0] CONVOVFU1xB4 = 32'h00B4; // value -> result
//    parameter [31:0] CONVOVFI2xB5 = 32'h00B5; // value -> result
//    parameter [31:0] CONVOVFU2xB6 = 32'h00B6; // value -> result
//    parameter [31:0] CONVOVFI4xB7 = 32'h00B7; // value -> result
//    parameter [31:0] CONVOVFI8xB9 = 32'h00B9; // value -> result
//    parameter [31:0] CONVOVFI2xBA = 32'h00BA; // value -> result
//    parameter [31:0] CKFINITExC3  = 32'h00C3; // value -> value
//    parameter [31:0] CONVU2xD1 = 32'h00D1; // value -> result
//    parameter [31:0] CONVU1xD2 = 32'h00D2; // value -> result
//    parameter [31:0] CONVIxD3  = 32'h00D3; // value -> result
//    parameter [31:0] CONVOVFUxD5 = 32'h00D5; // value -> result
//    parameter [31:0] ADDOVFxD6 = 32'h00D6; // ?, value1, value2 -> ?, result
//    parameter [31:0] ADDOVFUNxD7= 32'h00D7; // ?, value1, value2 -> ?, result
//    parameter [31:0] MULOVFxD8 = 32'h00D8; // ., value1, value2 -> ., result
//    parameter [31:0] MULOVFUNxD9 = 32'h00D9; // ., value1, value2 -> ., result
//    parameter [31:0] SUBOVFxDA = 32'h00DA; // ., value1, value2 -> ., result
//    parameter [31:0] SUBOVFUNxDB = 32'h00DB; // ., value1, value2 -> ., result
//    parameter [31:0] ENDFAULTxDC = 32'h00DC; // -> 
//    parameter [31:0] ENDFINALLYxDC = 32'h00DC; // -> 
//    parameter [31:0] LEAVExDD = 32'h00DD; // . ->
//    parameter [31:0] LEAVESxDE = 32'h00DE; // . ->
//    parameter [31:0] STINDIxDF = 32'h00DF; // ., addr, val -> .
//    parameter [31:0] CONVUxE0   = 32'h00E0; // value -> result
//    parameter [31:0] CEQxFE01  = 32'hFE01;  // value1, value2 -> result
//    parameter [31:0] ARGLISTxFE00=32'hFE00; // -> argListHandle
//    parameter [31:0] CGTxFE02   = 32'hFE02; // value1, value2 -> result
//    parameter [31:0] CGTUNxFE03 = 32'hFE03; // value1, value2 -> result
//    parameter [31:0] CLTxFE04   = 32'hFE04; // value1, value2 -> result
//    parameter [31:0] CLTUNxFE05 = 32'hFE05; // value1, value2 -> result
//    parameter [31:0] LDFTNxFE06 = 32'hFE06; // -> ftn
//    parameter [31:0] LDARGxFE09 = 32'hFE09; // -> value
//    parameter [31:0] LDARGAxFE0A = 32'hFE0A; // -> -> address of argument number argNum
//    parameter [31:0] STARGxFE0B = 32'hFE0B; // ., value -> ., 
//    parameter [31:0] ENDFILTERxFE11=32'hFE11; // value ->
//    parameter [31:0] CPBLKxFE17 = 32'hFE17; // destaddr, srcaddr, size -> 
//    parameter [31:0] INITBLKxFE18 = 32'hFE18; // addr, value, size -> 
//    parameter [31:0] LDLOCxFE0C = 32'hFE0C; // -> value
//    parameter [31:0] LDLOCAxFE0D = 32'hFE0D; // -> address
//    parameter [31:0] STLOCxFE0E = 32'hFE0E; // z                  -> .
//    parameter [31:0] LOCALLOCxFE0F = 32'hFE0F; // size -> address
    
//    // OBJECT MODEL INSTRUCTIONS
//    parameter [31:0] CALLVIRTx6F = 32'h006F; // ., obj, arg, ., argN -> ., returnVal (not always returned)
//    parameter [31:0] CPOBJx70 = 32'h0070; // ., dest, src -> ., 
//    parameter [31:0] LDOBJx71 = 32'h0071; // ., src -> ., val
//    parameter [31:0] LDSTRx72 = 32'h0072; // ., -> ., string
//    parameter [31:0] NEWOBJx73  = 32'h0073; // ?, arg1, ? argN    -> ?, obj
//    parameter [31:0] CASTCLASSx74 = 32'h0074; // ., obj -> ., obj2
//    parameter [31:0] LDSFLDx7E = 32'h007E; // ., -> ., value
//    parameter [31:0] LDSFLDAx7F = 32'h007F; // ., -> ., address
//    parameter [31:0] BOXx8C = 32'h008C; // ., val -> ., obj
//    parameter [31:0] ISINSTx75 = 32'h0075; // ., obj -> ., result
//    parameter [31:0] UNBOXx79 = 32'h0079; // ., obj -> ., valueTypePtr
//    parameter [31:0] THROWx7A = 32'h007A; // ., object -> .,
//    //parameter [31:0] LDFLDx7B = 32'h007B; // ., obj -> ., value
//    parameter [31:0] LDFLDAx7C = 32'h007C; // ., obj -> ., address
//    parameter [31:0] STFLDx7D   = 32'h007D; // ., obj, val -> .
//    parameter [31:0] STSFLDx80 = 32'h0080; // ., val -> .
//    parameter [31:0] STOBJx81 = 32'h0081; // ., dest, src -> .,
//    parameter [31:0] NEWARRx8D = 32'h008D; // ., numElems -> ., array
//    parameter [31:0] LDLENx8E = 32'h008E; // ., array -> ., length
//    parameter [31:0] LDELEMAx8F = 32'h008F; // ., array, index -> ., value
//    parameter [31:0] LDELEMI1x90 = 32'h0090; // ., array, index -> ., value
//    parameter [31:0] LDELEMU1x91 = 32'h0091; // ., array, index -> ., value
//    parameter [31:0] LDELEMI2x92 = 32'h0092; // ., array, index -> ., value
//    parameter [31:0] LDELEMU2x93 = 32'h0093; // ., array, index -> ., value
//    parameter [31:0] LDELEMI4x94 = 32'h0094; // ., array, index -> ., value
//    parameter [31:0] LDELEMU4x95 = 32'h0095; // ., array, index -> ., value
//    parameter [31:0] LDELEMI8x96 = 32'h0096; // ., array, index -> ., value
//    parameter [31:0] LDELEMU8x96 = 32'h0096; // ., array, index -> ., value
//    parameter [31:0] LDELEMIx97 = 32'h0097; // ., array, index -> ., value
//    parameter [31:0] LDELEMR4x98 = 32'h0098; // ., array, index -> ., value
//    parameter [31:0] LDELEMR8x99 = 32'h0099; // ., array, index -> ., value
//    parameter [31:0] LDELEMREFx9A = 32'h009A; // ., array, index -> ., value
//    parameter [31:0] STELEMIx9B = 32'h009B; // ., array, index, value -> ., 
//    parameter [31:0] STELEMI1x9C = 32'h009C; // ., array, index, value -> ., 
//    parameter [31:0] STELEMI2x9D = 32'h009D; // ., array, index, value -> ., 
//    parameter [31:0] STELEMI4x9E = 32'h009E; // ., array, index, value -> ., 
//    parameter [31:0] STELEMR4xA0 = 32'h00A0; // ., array, index, value -> .,
//    parameter [31:0] STELEMR8xA1 = 32'h00A1; // ., array, index, value -> .,  
//    parameter [31:0] STELEMREFxA2 = 32'h00A2; // ., array, index, value -> ., 
//    parameter [31:0] LDELEMxA3 = 32'h00A3; // ., array, index -> ., value
//    parameter [31:0] STELEMxA4 = 32'h00A4; // ., array, index, value -> .
//    parameter [31:0] UNBOXANYxA5 = 32'h00A5; // ., obj -> ., value or obj
//    parameter [31:0] REFANYVALxC2 = 32'h00C2; // ., TypedRef -> ., address
//    parameter [31:0] MKREFANYxC6 = 32'h00C6; // ., ptr -> ., typedRef
//    parameter [31:0] LDTOKENxD0 = 32'h00D0; // ., -> ., RuntimeHandle
//    parameter [31:0] LDVIRTFNxFE07 = 32'hFE07; // ., object -> ., ftn
//    parameter [31:0] INITOBJxFE15 = 32'hFE15; // ., dest -> ., 
//    parameter [31:0] RETHROWxFE1A = 32'hFE1A; // ., -> ., 
//    parameter [31:0] SIZEOFxFE1C = 32'hFE1C; // ., -> ., size (4 bytes, unsigned)
//    parameter [31:0] REFANYTYPExFE1D = 32'hFE1D; // ., TypedRef -> ., type


    // Called when a method returns control to its caller
    // i.e. the previous stack frame
    // task popStackFrame();
        // begin
            // Clean away the old stack frame to make debugging easier
            // stack.slot[stack.sfp + stack.SF_NEXT_SF_OFFSET] = text.NSF_6E_73_66_5F; // "nsf_"
            // stack.slot[stack.nsfp] = 32'bx; // "nsf_"
            // stack.slot[stack.sfp + stack.SF_PREV_SF_OFFSET] = 32'bx;            
            // stack.slot[stack.sfp + stack.SF_MIT_KEY_OFFSET] = 32'bx; 
            // stack.slot[stack.sfp + stack.SF_CIT_KEY_OFFSET] = 32'bx;
            // stack.slot[stack.sfp + stack.SF_THIS_OFFSET] = 32'bx; 
            // stack.slot[stack.sfp + stack.SF_ARGS_OFFSET] = 32'bx; 
            // stack.slot[stack.sfp + stack.SF_LOCS_OFFSET] = 32'bx; 
            // stack.slot[stack.sfp + stack.SF_LR_OFFSET] = 32'bx;  

            //Stack frame pointer from old next stack frame pointer
            // stack.nsfp = stack.sfp;
            //Previous stack frame pointer from previous stack frame pointer
            // stack.sfp = stack.psfp; 
            //PC from LR
            // stack.pc = state.lr;
            //esp reset to "empty" (but there are possible return arguments?)
            //no, too "brutal" esp = 32'h0;
            
            //Active and previous class
            // cls.cid = 32'h0;
            // cls.prevcid = 32'bx;//?
            //Active and previous method
            // cls.mid = cls.prevmid;
            // cls.prevmid = 32'bx; // What to do?
            // $display("cid=%0d, prevcid=%0d, mid=%0d, prevmid=%0d", cls.cid, cls.prevcid, cls.mid, cls.prevmid);
        // end
    // endtask
    
        // task showStackFrame();
        // begin
            // $display("Stackframe setup:");
            // $display("                                     --------------------------------------------------------");
            // $display("                   stack[0x%000008x] | lr                = 0x%000008x                       |", stack.sfp + stack.SF_LR_OFFSET, state.lr);
            // $display("                                     --------------------------------------------------------");          
            // $display("                   stack[0x%000008x] | cit_mit_numlocals = 0x%000008x                       |", stack.sfp + stack.SF_LOCS_OFFSET, cit_mit_numlocals);
            // $display("                                     --------------------------------------------------------");          
            // $display("                   stack[0x%000008x] | cit_mit_numargs   = 0x%000008x                       |", stack.sfp + stack.SF_ARGS_OFFSET, cit_mit_numargs);
            // $display("                                     --------------------------------------------------------");          
            // $display("                   stack[0x%000008x] | this_id           = 0x%000008x                       | (0 is static method & type)", stack.sfp + stack.SF_THIS_OFFSET, thisCnt); // 0 is static
            // $display("                                     --------------------------------------------------------");          
            // $display("                   stack[0x%000008x] | citIdKey          = 0x%000008x (cidKey)              | (the method's owner's type id)"
                                                                                                                        // , stack.sfp + stack.SF_CIT_KEY_OFFSET, tables.cit[cls.cid]);
            // $display("                                     --------------------------------------------------------");          
            // $display("                   stack[0x%000008x] | citMitIdKey       = 0x%000008x (midKey)              | %0s"
                                                                                                                        // , stack.sfp + stack.SF_MIT_KEY_OFFSET, cls.mid
                                                                                                                        // , tables.cit_mit_names[cls.mid]);
            // $display("                                     --------------------------------------------------------");          
            // $display("                   stack[0x%000008x] | psfp              = 0x%000008x                       |", stack.sfp + stack.SF_PREV_SF_OFFSET, stack.psfp);
            // $display("                                     --------------------------------------------------------");          
            // $display(" sfp=0x%000008x -> stack[0x%000008x] | nsfp              = 0x%000008x                       |", stack.sfp, stack.sfp + stack.SF_NEXT_SF_OFFSET, stack.nsfp);
            // $display("                                     --------------------------------------------------------");  
            // $display(""); 
        // end
    // endtask
    //task showHeap();
      //  begin
            // $display("Heapframe setup:");
            // $display("                                     --------------------------------------------------------");
            // $display("                    heap[0x%000008x] | citNumfieldsValue    = 0x%000008x                        |", heap.hfp + heap.HF_FLDS, citNumfieldsValue);
            // $display("                                     --------------------------------------------------------");
            // $display("                    heap[0x%000008x] | this_id          = 0x%000008x                        | (0: not an instance)"
                                                                                                      // ,heap.hfp + heap.HF_THIS_OFFSET, thisCnt);
            // $display("                                     --------------------------------------------------------");
            // $display("hfp=0x%000008x ->   heap[0x%000008x] | citIdValue           = 0x%000008x                        |", heap.hfp, heap.hfp + heap.HF_CID_OFFSET, citIdValue);
            // $display("                                     --------------------------------------------------------");  
            // $display("");
    //  end
    //endtask
    
        // Argument information table
        // logic [31:0] at                  [0:127];  // Argument information table 
        // logic [31:0] AT_ID               = 32'h00; // Argument id
        // logic [31:0] AT_TYPE             = 32'h01; // Argument type
        // logic [31:0] AT_SIZE             = 32'h02; // Size of one argument entry record
        
        // Locals information table
        // logic [31:0] lt                  [0:127];  // 
        // logic [31:0] LT_ID               = 32'h00; // Local var. id
        // logic [31:0] LT_TYPE             = 32'h01; // Local var. type
        // logic [31:0] LT_SIZE             = 32'h02; // Size of local entry record
        
    
