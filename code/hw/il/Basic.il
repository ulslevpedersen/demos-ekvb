// Copyright: 2015-2017, Copenhagen Business School
// Author: Rasmus Ulslev Pedersen (rup.itm@cbs.dk)

// License: Simplified BSD License
//
// IL asm file

.assembly extern mscorlib {}
.assembly BasicTest
{
    .ver 1:0:1:0
}
.module basic.exe

// Notation: heaps is for a static heap frame, stacks is for a static stack frame

// A heap_s frame for BasicClass is created before main() is called.
// Then a stack_s frame is created for main(). 
// newobj triggers a new heap_i frame for BasicClass. That will generate "this"
// Then a stackframe_i is created for .ctor with <int32>5 on the stack as argument 1 and the 
// new this reference as argument 0 on the stack.
// .ctor consumes argument 1 and returns to main() with this on top of the stack.
// Then 10 and 20 are pushed to the stack and Method1(int32, int32) is called.    
// It is called by creating a stack frame for Method1. It will have 2 local variables and
// 2 arguments.

// This is a basic class that sends an int32, 5, to the constructor .ctor.
// 
.class public beforefieldinit BasicClass
	extends [mscorlib]System.Object
{	
	//.field private int32 m_IntField
	// MethodDef[#0x1]
    .method static void main() cil managed
    {
        .entrypoint
        ldc.i4 5
		// Operand for newobj is 06000002, which is 06 (MethodDefTable) and entry index 2.
		// That is .ctor below (following this method)
		newobj instance void BasicClass::.ctor(int32)
		ldc.i4 10
		ldc.i4 20
        call instance void BasicClass::Method1(int32, int32)
        ret
    }
    
	// MethodDef[#0x2]
    .method public hidebysig specialname rtspecialname  
        instance void .ctor(int32) cil managed
    { 
		// ldarg.0 // load this pointer 
        // call instance void [mscorlib]System.Object::.ctor()
		//ldc.i4.s 123
		//ldarg.0
		//stsfld int32 BasicClass::m_IntField 
        ret
    }
    
	// MethodDef[#0x3]
    .method public instance void Method1(int32, int32) cil managed
    {   
		//.locals ([0] int32 l_i)
		.locals ([0] int32 locali, [1] int16 localj)
		ldloc.0
		stloc.0     
        //ldstr "Method1()..."
        //call void [mscorlib]System.Console::WriteLine(string)
        ret
    }

	// MethodDef[#0x4]
	.method public instance void Method2() cil managed
	{
		.locals ([0] int32 localk)
		ldloc.0
		stloc.0     
		ret
	}
}