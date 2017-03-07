namespace TCore

open System
open System.Drawing
open System.Threading
open System.Windows.Forms

// todo - method w. arguments
// make room for 8 arguments
// make a type for them in TCoreSoft
// init to [0] - and so on
// remember that [0] is "this"
// create the ves instructions "'pos' 'val' starg"
// and                         "'pos' ldarg"
// make the guiupdate instructions in TCoreGui
// make the gui for 8 args
// argshow(pos:int, value:string)

// Gui for TCore
module TCoreGui = 
    // Program counter
    // TODO: Should it be in TCoreSoft.fs?
    let mutable pc = 0

    // An instruction with OpCode such as 'ldc'
    // and optional Operand such as '2'
    type IlRec = 
        { OpCode : string
          Operand : string }
    
    // The "program"    
    let IlList = 
        [ { OpCode = "ldc"
            Operand = "1" }
          { OpCode = "ldc"
            Operand = "2" }
          { OpCode = "str"
            Operand = "" }
          { OpCode = "ldc"
            Operand = "4" }
          { OpCode = "ldc"
            Operand = "6" }
          { OpCode = "add"
            Operand = "" }
          { OpCode = "ldc"
            Operand = "1" }
          { OpCode = "stloc"
            Operand = "" }
          { OpCode = "ldc"
            Operand = "1" }
          { OpCode = "ldloc"
            Operand = "" } ]
   
    // GUI creation
    let font = new Font("Consolas", 12.0f, GraphicsUnit.Point)
    // Main Form
    let fGui = new Form(Text = "TCore App", Font = font, Height = 380, Width = 480)
    // Stack
    let lStack = new Label(Text = "stack", Font = font, Top = 20, Left = 20, Height = 15)
    let lbStack = new ListBox(Font = font, Top = 40, Left = 20, Height = 8 * 20, Width = 100)
    // Memory
    let lMem = new Label(Text = "mem", Font = font, Top = 20, Left = 120, Height = 15)
    let lbMem = new ListBox(Font = font, Top = 40, Left = 120, Height = 8 * 20, Width = 100)
    // Locals
    let lStackFrameLocals = new Label(Text = "stack frm.\nlocals", Font = font, Top = 0, Left = 220, Height = 40, Width = 100)
    let lbStackFrameLocals = new ListBox(Font = font, Top = 40, Left = 220, Height = 8 * 20, Width = 100)
    // Stack frame arguments
    let lStackFrameArguments = new Label(Text = "stack frm.\narguments", Font = font, Top = 0, Left = 320, Height = 40, Width = 100)
    let lbStackFrameArguments = new ListBox(Font = font, Top = 40, Left = 320, Height = 8 * 20, Width = 100) 
    // Command and stepping
    let bStep = new Button(Text = "Step", Font = font, Top = 200, Left = 20, Height = 30, Width = 100)
    let tCmd = new RichTextBox(Font = font, Top = 200, Left = 120, Height = 30, Width = 100)
    let bHelp = new Button(Text = "Help", Font = font, Top = 250, Left = 20, Height = 30, Width = 100)
    let bTodo = new Button(Text = "Todo", Font = font, Top = 300, Left = 20, Height = 30, Width = 100)
    
    // memory is 8 items top down:
    // [0] 123, [1] 1, and so on    
    let memshow (pos : int, value : string) = 
        let listmemval = "[" + string pos + "] " + value
        lbMem.Items.RemoveAt(pos)
        lbMem.Items.Insert(pos, listmemval) |> ignore
        lbMem.SelectedIndex <- pos
    
    // pop from gui stack
    let popguistack (pos) = 
        lbStack.Items.RemoveAt(7-pos)
        lbStack.Items.Insert(7-pos,"-") |> ignore
    
    // push to gui stack
    // The gui stack is an 8 item stack with "-" depicting empty slots. Gui index [0] is on top and stack tos is on the bottom
    let pushguistack (pos : int, value : string) = 
        lbStack.Items.RemoveAt (7-pos)
        lbStack.Items.Insert(7-pos, value) |> ignore

    // Local variable update on pos
    // [0] 0, [1] 1, etc.
    let localshow(pos : int, value : string) =
        let localval = "[" + string pos + "] " + value
        lbStackFrameLocals.Items.RemoveAt pos
        lbStackFrameLocals.Items.Insert(pos, localval) |> ignore
        lbStackFrameLocals.SelectedIndex <- pos

    // Arguments varible update on pos
    let argsShow(pos : int, value : string) =
        let argVal = "[" + string pos + "] " + value
        lbStackFrameArguments.Items.RemoveAt pos
        lbStackFrameArguments.Items.Insert(pos, argVal) |> ignore
        lbStackFrameArguments.SelectedIndex <- pos

    let setStephandler (f) = bStep.Click.Add(fun _ -> f() |> ignore)
    // Gui components
    // Initialization for memory, stack, and locals
    let guiInit() = 
        for i in 0..7 do
            lbMem.Items.Add("[" + string i + "] null") |> ignore 
        // Init 8 item stack with "-"
        for i in 0..7 do
            lbStack.Items.Add("-") |> ignore
        // locals stackframe
        for i in 0..7 do
            lbStackFrameLocals.Items.Add("[" + string i + "] -") |> ignore
        // Arguments stackframe
        for i in 0..7 do
            lbStackFrameArguments.Items.Add("[" + string i + "] -") |> ignore
        tCmd.HideSelection <- false
        fGui.Controls.Add(lStack)
        fGui.Controls.Add(lbStack)
        fGui.Controls.Add(lMem)
        fGui.Controls.Add(lbMem)
        fGui.Controls.Add(lStackFrameLocals)
        fGui.Controls.Add(lbStackFrameLocals)
        fGui.Controls.Add(lStackFrameArguments)
        fGui.Controls.Add(lbStackFrameArguments)
        fGui.Controls.Add(bStep)
        fGui.Controls.Add(tCmd)
        fGui.Controls.Add(bHelp)
        bHelp.Click.Add(fun _ -> MessageBox.Show("'x' 'y' add \nldc 'x' \n'pos' 'val' str \n'pos' ldr \n'loc' 'val' stloc \n'loc' ldloc", "TCore Help") |> ignore)
        fGui.Controls.Add(bTodo)
        bTodo.Click.Add(fun _ -> MessageBox.Show("stackframe: arguments", "TCore Todo") |> ignore)
        fGui.AcceptButton <- bStep
        tCmd.Focus() |> ignore

    // Update current instruction
    let guiUpdate() = 
        if pc < List.length IlList then
            tCmd.Text <- IlList.[pc].OpCode + " " + IlList.[pc].Operand
            bStep.Text <- "Step " + string pc
        else
            bStep.Text <- "Step"
        tCmd.SelectionStart <- 0
        tCmd.SelectionLength <- tCmd.Text.Length
        tCmd.Focus()    
    
    // Show gui
    let startGui() = 
        fGui.Show()
        tCmd.Focus() |> ignore   
        guiUpdate() |> ignore     
        Application.Run(fGui)