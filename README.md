
# Introduction

This repository contains 4-bit CPU design files for my Computer Architecture
Lab.


CPU E-bity means 4-bit CPU in Persian. E stands for number 4 , because I think
the E letter is the most similar letter to number 4
(![Arabic 4](Images/Arabic-4.jpg?raw=true "Arabic 4")) in Arabic font :)


![Block Diagram](Images/BlockDiagram.png?raw=true "Block Diagram")

![Proteus Simulation](Images/CPU.png?raw=true "Proteus Simulation")



# Instruction Set

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



# Example Assembly Code

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

