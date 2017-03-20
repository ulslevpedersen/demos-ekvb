# DEMOS simulation makefile

all: compme simme

compme: clean
	vlib work
	vlog -work work -sv -lint fop.sv
	vlog -work work -sv -lint fop_tb.sv

simme:
	vsim -do "run 100ns; quit -f" -c work.fop_tb -hazards
	#vsim -do "run 100ns" -c work.fop_tb -hazards

clean:
	rm -rf transscript
	rm -rf work	
	
help:
	#Makefile for demos
	#use target 'compme' to compile
	#use target 'simme' to simulate