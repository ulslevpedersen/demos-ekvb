#demos-ekvb make
all: allil allfs allsv
	
clean: cleanil cleanfs cleansv
	
help: helpil helpfs helpsv
	

#basic.il
allil: cleanil buildil helpil runil
	
buildil: 
	cd il && \
	ilasm Basic.il
runil: 
	cd il && \
	./Basic
docil: buildil
	cd il && \
	ildasm.exe /out=Basic.html /html /adv /all /metadata=MDHEADER /metadata=HEX /metadata=CSV /metadata=UNREX /metadata=SCHEMA /metadata=RAW /metadata=HEAPS /metadata=VALIDATE Basic.exe
cleanil:
	cd il && \
	rm -f Basic.exe Basic.html
helpil:
	#basic.il:
	#'allil' to clean, make basic.exe
	#'docil' to produce IL html overview
	#'runil' to run basic.exe

#Ciller compiler (.fs)
allfs:cleanfs buildfs runfs
	
buildfs: cleanfs
	cd fs && \
	cp ../il/Basic.exe Basic.exe
	cd fs && \
	fsc CilStuff.fs CliTables.fs CilXml.fs Ciller.fs --out:Ciller.exe
runfs: buildfs
	cd fs && \
	./Ciller	
cleanfs:
	cd fs && \
	rm -f Ciller.exe
	cd fs && \
	rm -f mem.sv
	cd fs && \
	rm -f Basic.exe
helpfs:
	#Ciller.exe:
	#'allfs' to do all
	#'buildfs' to build
	#'runfs' to run

#fop.sv
allsv: cleansv compme simme
	
compme: cleansv
	cd sv && \
	cp ../fs/mem.sv mem.sv
	cd sv && \
	vlib work
	cd sv && \
	vlog -work work -sv -lint fop.sv
	cd sv && \
	vlog -work work -sv -lint fop_tb.sv
simme:
	cd sv && \
	vsim -do "run 100ns; quit -f" -c work.fop_tb -hazards
	#vsim -do "run 100ns" -c work.fop_tb -hazards
cleansv:
	cd sv && \
	rm -f transcript
	cd sv && \
	rm -rf work	
	cd sv && \
	rm -f mem.sv
helpsv:
	#fop.sv:
	#'compme' to compile
	#'simme' to simulate