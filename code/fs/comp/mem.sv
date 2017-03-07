// mem.sv memory initialization
// 6:44:47 AM

//--------------------------------------------------------------------------  

// cit records:         [0]CIT_ID_INDEX
//                      [1]CIT_NUMFIELDS_INDEX
//                      [2]CIT_NUMMETHODS_INDEX

// BasicClass
cit[0000] <= 32'h00000002;      // CIT_ID_INDEX
cit[0001] <= 32'h00000000;      // CIT_NUMFIELDS_INDEX
cit[0002] <= 32'h00000004;      // CIT_NUMMETHODS_INDEX

//--------------------------------------------------------------------------  

// cit_mit_name records:[0]CIT_MIT_ID_NAME_INDEX

cit_mit_names[00000000] <= "noname";               	// CIT_MIT_ID_NAME INDEX
cit_mit_names[00000001] <= "BasicClass:main";      	// CIT_MIT_ID_NAME INDEX
cit_mit_names[00000002] <= "BasicClass:.ctor";      	// CIT_MIT_ID_NAME INDEX
cit_mit_names[00000003] <= "BasicClass:Method1";      	// CIT_MIT_ID_NAME INDEX
cit_mit_names[00000004] <= "BasicClass:Method2";      	// CIT_MIT_ID_NAME INDEX

//--------------------------------------------------------------------------  

// cit_mit records:     [0]CIT_MIT_ID_INDEX
//                      [1]CIT_MIT_CITID_INDEX (foreign key)
//                      [2]CIT_MIT_NUMARGS_INDEX
//                      [3]CIT_MIT_NUMLOCALS_INDEX
//                      [4]CIT_MIT_MAXEVALSTACK_INDEX
//                      [5]CIT_MIT_NUMCIL_INDEX
//                      [6]CIT_MIT_FIRSTCIL_INDEX

// BasicClass:main
cit_mit[0000] <= 32'h00000001;      // CIT_MIT_ID_INDEX
cit_mit[0001] <= 32'h00000000;      // CIT_MIT_CITID_INDEX (foreign [key])
cit_mit[0002] <= 32'h00000000;      // CIT_MIT_NUMARGS_INDEX
cit_mit[0003] <= 32'h00000000;      // CIT_MIT_NUMLOCALS_INDEX
cit_mit[0004] <= 32'h00000008;      // CIT_MIT_MAXEVALSTACK_INDEX
cit_mit[0005] <= 32'h00000006;      // CIT_MIT_NUMCIL_INDEX
cit_mit[0006] <= 32'h00000000;      // CIT_MIT_FIRSTCIL_INDEX

// BasicClass:.ctor
cit_mit[0007] <= 32'h00000002;      // CIT_MIT_ID_INDEX
cit_mit[0008] <= 32'h00000000;      // CIT_MIT_CITID_INDEX (foreign [key])
cit_mit[0009] <= 32'h00000001;      // CIT_MIT_NUMARGS_INDEX
cit_mit[0010] <= 32'h00000000;      // CIT_MIT_NUMLOCALS_INDEX
cit_mit[0011] <= 32'h00000008;      // CIT_MIT_MAXEVALSTACK_INDEX
cit_mit[0012] <= 32'h00000001;      // CIT_MIT_NUMCIL_INDEX
cit_mit[0013] <= 32'h00000011;      // CIT_MIT_FIRSTCIL_INDEX

// BasicClass:Method1
cit_mit[0014] <= 32'h00000003;      // CIT_MIT_ID_INDEX
cit_mit[0015] <= 32'h00000000;      // CIT_MIT_CITID_INDEX (foreign [key])
cit_mit[0016] <= 32'h00000002;      // CIT_MIT_NUMARGS_INDEX
cit_mit[0017] <= 32'h00000002;      // CIT_MIT_NUMLOCALS_INDEX
cit_mit[0018] <= 32'h00000008;      // CIT_MIT_MAXEVALSTACK_INDEX
cit_mit[0019] <= 32'h00000003;      // CIT_MIT_NUMCIL_INDEX
cit_mit[0020] <= 32'h00000013;      // CIT_MIT_FIRSTCIL_INDEX

// BasicClass:Method2
cit_mit[0021] <= 32'h00000004;      // CIT_MIT_ID_INDEX
cit_mit[0022] <= 32'h00000000;      // CIT_MIT_CITID_INDEX (foreign [key])
cit_mit[0023] <= 32'h00000000;      // CIT_MIT_NUMARGS_INDEX
cit_mit[0024] <= 32'h00000001;      // CIT_MIT_NUMLOCALS_INDEX
cit_mit[0025] <= 32'h00000008;      // CIT_MIT_MAXEVALSTACK_INDEX
cit_mit[0026] <= 32'h00000003;      // CIT_MIT_NUMCIL_INDEX
cit_mit[0027] <= 32'h0000001B;      // CIT_MIT_FIRSTCIL_INDEX

//--------------------------------------------------------------------------  

// cit_mit_cil records: [0]CIT_MIT_CIL_ID_INDEX
//                      [1]CIT_MIT_CIL_OPCODE
//                      [2]CIT_MIT_CIL_OPERAND

// BasicClass : main : rva 0x00000250
cit_mit_cil[0000] <= 32'h00000000;      // CIT_MIT_CIL_ID_INDEX
cit_mit_cil[0001] <= 32'h00000020;      // CIT_MIT_CIL_OPCODE: 		ldc.i4
cit_mit_cil[0002] <= 32'h00000005;      // CIT_MIT_CIL_OPERAND: 	5

// BasicClass : main : rva 0x00000250
cit_mit_cil[0003] <= 32'h00000005;      // CIT_MIT_CIL_ID_INDEX
cit_mit_cil[0004] <= 32'h00000073;      // CIT_MIT_CIL_OPCODE: 		newobj
cit_mit_cil[0005] <= 32'h06000002;      // CIT_MIT_CIL_OPERAND: 	100663298

// BasicClass : main : rva 0x00000250
cit_mit_cil[0006] <= 32'h0000000A;      // CIT_MIT_CIL_ID_INDEX
cit_mit_cil[0007] <= 32'h00000020;      // CIT_MIT_CIL_OPCODE: 		ldc.i4
cit_mit_cil[0008] <= 32'h0000000A;      // CIT_MIT_CIL_OPERAND: 	10

// BasicClass : main : rva 0x00000250
cit_mit_cil[0009] <= 32'h0000000F;      // CIT_MIT_CIL_ID_INDEX
cit_mit_cil[0010] <= 32'h00000020;      // CIT_MIT_CIL_OPCODE: 		ldc.i4
cit_mit_cil[0011] <= 32'h00000014;      // CIT_MIT_CIL_OPERAND: 	20

// BasicClass : main : rva 0x00000250
cit_mit_cil[0012] <= 32'h00000014;      // CIT_MIT_CIL_ID_INDEX
cit_mit_cil[0013] <= 32'h00000028;      // CIT_MIT_CIL_OPCODE: 		call
cit_mit_cil[0014] <= 32'h06000003;      // CIT_MIT_CIL_OPERAND: 	100663299

// BasicClass : main : rva 0x00000250
cit_mit_cil[0015] <= 32'h00000019;      // CIT_MIT_CIL_ID_INDEX
cit_mit_cil[0016] <= 32'h0000002A;      // CIT_MIT_CIL_OPCODE: 		ret
cit_mit_cil[0017] <= 32'h00000000;      // CIT_MIT_CIL_OPERAND: 	0

// BasicClass : .ctor : rva 0x0000026b
cit_mit_cil[0018] <= 32'h00000000;      // CIT_MIT_CIL_ID_INDEX
cit_mit_cil[0019] <= 32'h0000002A;      // CIT_MIT_CIL_OPCODE: 		ret
cit_mit_cil[0020] <= 32'h00000000;      // CIT_MIT_CIL_OPERAND: 	0

// BasicClass : Method1 : rva 0x00000270
cit_mit_cil[0021] <= 32'h00000000;      // CIT_MIT_CIL_ID_INDEX
cit_mit_cil[0022] <= 32'h00000006;      // CIT_MIT_CIL_OPCODE: 		ldloc.0
cit_mit_cil[0023] <= 32'h00000000;      // CIT_MIT_CIL_OPERAND: 	0

// BasicClass : Method1 : rva 0x00000270
cit_mit_cil[0024] <= 32'h00000001;      // CIT_MIT_CIL_ID_INDEX
cit_mit_cil[0025] <= 32'h0000000A;      // CIT_MIT_CIL_OPCODE: 		stloc.0
cit_mit_cil[0026] <= 32'h00000000;      // CIT_MIT_CIL_OPERAND: 	0

// BasicClass : Method1 : rva 0x00000270
cit_mit_cil[0027] <= 32'h00000002;      // CIT_MIT_CIL_ID_INDEX
cit_mit_cil[0028] <= 32'h0000002A;      // CIT_MIT_CIL_OPCODE: 		ret
cit_mit_cil[0029] <= 32'h00000000;      // CIT_MIT_CIL_OPERAND: 	0

// BasicClass : Method2 : rva 0x00000280
cit_mit_cil[0030] <= 32'h00000000;      // CIT_MIT_CIL_ID_INDEX
cit_mit_cil[0031] <= 32'h00000006;      // CIT_MIT_CIL_OPCODE: 		ldloc.0
cit_mit_cil[0032] <= 32'h00000000;      // CIT_MIT_CIL_OPERAND: 	0

// BasicClass : Method2 : rva 0x00000280
cit_mit_cil[0033] <= 32'h00000001;      // CIT_MIT_CIL_ID_INDEX
cit_mit_cil[0034] <= 32'h0000000A;      // CIT_MIT_CIL_OPCODE: 		stloc.0
cit_mit_cil[0035] <= 32'h00000000;      // CIT_MIT_CIL_OPERAND: 	0

// BasicClass : Method2 : rva 0x00000280
cit_mit_cil[0036] <= 32'h00000002;      // CIT_MIT_CIL_ID_INDEX
cit_mit_cil[0037] <= 32'h0000002A;      // CIT_MIT_CIL_OPCODE: 		ret
cit_mit_cil[0038] <= 32'h00000000;      // CIT_MIT_CIL_OPERAND: 	0
