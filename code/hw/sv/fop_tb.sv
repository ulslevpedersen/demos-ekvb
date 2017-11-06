// Copyright: 2015-, Copenhagen Business School
// Author: Rasmus Ulslev Pedersen (rp.digi@cbs.dk)
// License: Simplified BSD License
//
// fop testbench

module fop_tb; 
    timeunit 1ns; timeprecision 1ns;

    // testbench clock
    logic tb_clk;

    logic fop_reset;
    logic fop_enable;
    fop fop_ (
                  .clk ( tb_clk),
                  .reset ( fop_reset ),
                  .enable ( fop_enable )    
             );

    initial fop_reset  = 1'b0;
    initial fop_enable = 1'b0;
             
    // init and generate 100 Mhz clock
    initial begin
        tb_clk = 1'b0;
        forever #5 tb_clk = ~tb_clk;
    end		
    
    initial begin
	       @(posedge tb_clk);
        fop_reset  =#1 1'b1;
        $display("fop_tb: resetting ...");
        @(posedge tb_clk);
        fop_reset  =#1 1'b0;
        fop_enable =#1 1'b1;
        $display("fop_tb: enabled ...");
        @(posedge tb_clk);
        #100 $finish;
    end
endmodule