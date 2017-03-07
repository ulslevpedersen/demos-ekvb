set systemTime [clock seconds]
set echo off
puts "============================================================================================="
puts "::CILOP COMPILE START; [clock format $systemTime -format %H:%M:%S]"
puts "============================================================================================="

set ConsoleAppHome "C:/Dropbox/40-sw/VisualStudio/ConsoleApplication1/ConsoleApplication1/"
set ConsoleAppHomeDebug "C:/Dropbox/40-sw/VisualStudio/ConsoleApplication1/ConsoleApplication1/bin/Debug"
set VivadoTcltestHome "C:/Dropbox/40-sw/Vivado/tcltest" 

puts "::Compiling basic.il to basic.exe (using ilasm.exe) and producing basic.html (using ildasm.exe)"
puts "::  Location:$ConsoleAppHomeDebug"
cd $ConsoleAppHomeDebug
exec ilasm.exe basic.il >@stdout
exec ildasm.exe /out=basic.html /html /adv /all /metadata=MDHEADER /metadata=HEX /metadata=CSV /metadata=UNREX /metadata=SCHEMA /metadata=RAW /metadata=HEAPS /metadata=VALIDATE basic.exe >@stdout

puts ""
puts "::F# compiler (fsc.exe) compiling to ConsoleApplication1.exe"
cd $ConsoleAppHome
exec fsc.exe CilStuff.fs CliTables.fs CilXmlProgram.fs Program.fs --out:bin/Debug/ConsoleApplication1.exe >@stdout 

puts ""
puts "::Running ConsoleApplication1.exe to produce mem.sv (copied to Vivado)"
cd $ConsoleAppHomeDebug
exec ConsoleApplication1.exe >@stdout
cd $ConsoleAppHome
file delete $VivadoTcltestHome/tmp.srcs/sources_1/imports/new/mem.sv
file copy mem.sv $VivadoTcltestHome/tmp.srcs/sources_1/imports/new

puts "::Launch Vivado Simulation"
cd $VivadoTcltestHome
close_project -quiet
open_project tmp
reset_simulation -quiet
launch_simulation
reset_simulation -quiet
close_project -quiet

set systemEndTime [clock seconds]
puts "============================================================================================="
puts "::CILOP COMPILE END; [clock format $systemEndTime -format %H:%M:%S]"
puts "============================================================================================="
