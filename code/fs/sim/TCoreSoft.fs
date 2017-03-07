namespace TCore

open System.Windows.Forms

// Soft Core
module TCoreSoft = 
    //// Main Constructs
    // Stack
    let stack : string [] = Array.zeroCreate 8
    // Points to empty slot
    let mutable tos = 0
    // memory
    let mem : string [] = Array.zeroCreate 8
    //// Stackframe
    // arguments
    let stackFrameArguments : string [] = Array.create 8 "0"
    // locals
    let stackFrameLocals : string [] = Array.create 8 "0"

    //// Main program
    // ves 
    // 'line' is 'ldc 2' for example 
    let ves (line : string) = 
        // ldc
        if line.StartsWith "ldc" then 
            let opr = line.Substring("ldc".Length + 1)
            stack.[tos] <- opr
            let stackpushpos = tos
            let stackpushval = stack.[tos]
            tos <- tos + 1
            TCoreGui.pushguistack (stackpushpos, stackpushval)
        // add
        elif line.StartsWith "add" then 
            let stackval = string (int stack.[tos - 1] + int stack.[tos - 2])
            stack.[tos - 2] <- stackval
            stack.[tos - 1] <- null
            let poppos1 = tos - 1
            let poppos2 = tos - 2
            let pushpos = tos - 2
            let pushval = stackval
            tos <- tos - 1
            TCoreGui.popguistack (poppos1)
            TCoreGui.popguistack (poppos2)
            TCoreGui.pushguistack (pushpos, pushval)
        // str
        elif line.StartsWith "str" then 
            // memory is on top of stack
            let mempos = int stack.[tos - 1]
            // value right beneth
            let memval = stack.[tos - 2]
            mem.[mempos] <- memval
            let poppos1 = tos - 1
            let poppos2 = tos - 2
            stack.[tos - 2] <- null
            stack.[tos - 1] <- null
            tos <- tos - 2
            TCoreGui.memshow (mempos, memval)
            TCoreGui.popguistack (poppos1)
            TCoreGui.popguistack (poppos2)
        // ldr
        elif line.StartsWith "ldr" then 
            let poppos = tos - 1
            let pushpos = tos - 1
            let pushval = mem.[int stack.[tos - 1]]
            stack.[pushpos] <- pushval
            TCoreGui.popguistack (poppos)
            TCoreGui.pushguistack (pushpos, pushval)
        // local variables: ldloc
        elif line.StartsWith "ldloc" then
            // load local from tos-1 and pop
            let poppos = tos-1
            let locpos = stack.[poppos]
            TCoreGui.popguistack poppos
            let locval = stackFrameLocals.[int locpos]
            let pushpos = tos-1
            TCoreGui.pushguistack (pushpos, locval)
            printf ""
        // stloc
        elif line.StartsWith "stloc" then
            // store at tos-1 tos-2 as local variable
            let poppos1 = tos - 1
            let locpos = stack.[poppos1]
            let poppos2 = tos - 2
            let locval = stack.[poppos2]
            stackFrameLocals.[int locpos] <- locval
            TCoreGui.popguistack poppos1
            TCoreGui.popguistack poppos2  
            TCoreGui.localshow (int locpos, locval)
            tos <- tos - 2   
        // starg have both pos and val arguments on the 
        // stack
        elif line.StartsWith "starg" then
            let poppos1 = tos - 1
            let argpos = stack.[poppos1]
            let poppos2 = tos - 2
            let argval = stack.[poppos2]
            stackFrameArguments.[int argpos] <- argval
            TCoreGui.popguistack poppos1
            TCoreGui.popguistack poppos2
            TCoreGui.argsShow (int argpos, argval)
            tos <- tos - 2
        // method arguments
        // ldarg has a pos operand on the stack,
        // which is popped
        elif line.StartsWith "ldarg" then
            let poppos1 = tos - 1
            let argpos = stack.[poppos1]
            let argval = stackFrameArguments.[int argpos]
            printf ""
        // decode error
        else 
            MessageBox.Show(line, "Decode error") |> ignore