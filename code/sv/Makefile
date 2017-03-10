.PHONY: compme
compme:
	if [ -d "work" ]; then vdel -all; fi
	vlib work
	vlog -work work -sv -lint fop.sv
	vlog -work work -sv -lint fop_tb.sv

simme:
	vsim -do "run 100ns; quit -f" -c work.fop_tb -hazards
