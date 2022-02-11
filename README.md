
# Introduction

This repository contains 4-bit CPU design files for my Computer Architecture
Lab.

CPU E-bity means 4-bit CPU in Persian. E stands for number 4 , because I think
the E letter is the most similar letter to number 4
(![Arabic 4](Images/Arabic-4.jpg?raw=true "Arabic 4")) in Arabic font :)



# Architecture

I tried to use least possible components. This processor has an accumulator, a
data address counter, and a program pointer registers.

It is based on "Harvard" architecture, which means that is has separate address
spaces for program and data. This architecture allows different word size for
data and code. Here, data word is 4-bit wide, and instructions word size is
12-bit wide. This processor can address 256 words of code and 256 words of data,
because it has a 8-bit address bus. The block diagram of this CPU is as follows:

![Block Diagram](Images/BlockDiagram.png?raw=true "Block Diagram")



## Instruction Set

This processor has arithmetic (add and subtract), logical (complement of
accumulator, logical "or", logical "and", logical mutual exclusive "or", and
left and right shift), register move, unconditional jump, conditional jump
operations according to zero and carry flags, accumulator and flags clear
operations. For the sake of simplicity, input/output instructions are dropped.

All operations are done in 3 clock pulses, this means that each instruction
cycle takes 3 clock, then the ring counter resets to zero after 3 clocks.

In the __first clock__ the CPU sends the content of PC to code address bus and
reads and decodes the current instruction, at the same time it puts address of
data into data address register.

In the __second clock__ the CPU executes the instruction and the PC increments
by one.

In the __third clock__ the CPU saves the result and ring counter becomes zero to
begin next cycle.

The opcode format is as follows:

	xxxx dddd dddd

* __x__: 4 bit instruction
* __d__: 8 bit data address

The following table contains the instruction set for this processor:


| Description									| Opcode | Instruction |
| ----------------------------------------------|:------:|:-----------:|
| Move Accumulator to memory					|  0000  |     MOV     |
| Add Accumulator with data in memory			|  0001  |     ADD     |
| Subtract Accumulator with data in memory		|  0010  |     SUB     |
| Or Accumulator with data in memory			|  0011  |     OR      |
| And Accumulator with data in memory			|  0100  |     AND     |
| Exclusive Or Accumulator with data in memory	|  0101  |     XOR     |
| Complement Accumulator						|  0110  |     NOT     |
| Shift Accumulator left						|  0111  |     SHL     |
| Shift Accumulator right						|  1000  |     SHR     |
| Unconditional jump							|  1001  |     JMP     |
| Branch if zero								|  1010  |     BRZ     |
| Branch if carry								|  1011  |     BRC     |
| Branch if not zero							|  1100  |     BNZ     |
| Branch if carry clear							|  1101  |     BNC     |
| Clear Flags									|  1110  |     CLF     |
| Clear Accumulator								|  1111  |     CLA     |



## Proteus Implementation and Simulation

The Proteus 7.6 SP4 (build 8741) is used for implementation and simulation of
CPU. Discrete components are used to implement different blocks of CPU according
to the block diagram that depicted above. The final design is as follows:


![Proteus Simulation](Images/CPU.png?raw=true "Proteus Simulation")




# Example Assembly Code

A two-pass assembler is written to translate assembly language into machine
codes, for example consider the following assembly code:



```assembly
		BYTE A
		BYTE B
		BYTE C
		SHL
		SHL
		SHL
		ADD A
		SUB B
		CLA
		END
```


After running assembler on this source code, two Intel-Hex formatted outputs
will be created as follows:


<table>
<tr>
	<th>Opcode Hex output file</th>
	<th>Data Address Hex output file</th>
</tr>
<tr><td>

```hex
:0600000007070701020FD3
:00000001FF
```

</td><td>

```hex
:06000000FFFFFF0001FFFD
:00000001FF
```

</td></tr>
</table>


These files can be used to test the CPU functionality.

