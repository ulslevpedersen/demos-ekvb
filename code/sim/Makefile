all: clean build run

build: TcoreGui.fs TCoreSoft.fs TCoreApp.fs
	fsc TcoreGui.fs TCoreSoft.fs TCoreApp.fs

run: TCoreApp.exe
	./TCoreApp

clean:
	-rm -f TCoreApp.exe

help:
	#Makefile for TCore sim	