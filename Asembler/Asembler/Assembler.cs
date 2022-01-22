using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Assembler
{
    struct Error
    {
        public int Address;
        public string Description;

        public Error(int Address, string Desc)
        {
            this.Address = Address;
            this.Description = Desc;
        }

        public override string ToString()
        {
            return "Line : " + Address + " -> " + Description;
        }
    }
    struct OpCode
    {
        public int Length;
        public int MachineCode;

        public OpCode(int Length, int MashineCode)
        {
            this.Length = Length;
            this.MachineCode = MashineCode;
        }
    }
    struct Symbol
    {
        public int Address;
        public Types Type;
        public enum Types
        {
            Instruction,
            Lable,
            Variable
        }
        public Symbol(int Address,Types Type)
        {
            this.Address = Address;
            this.Type = Type;
        }
    }
    class Assembler
    {
        int LocCounter;
        int LineCounter;
        int VarLocCounter;

        Dictionary<string, OpCode> OpTable = new Dictionary<string,OpCode>();
        Dictionary<string, Symbol> SymTable = new Dictionary<string,Symbol>();
        List<Error> ErrorList = new List<Error>();

        StreamReader sReader;
        StreamWriter sWriter;
        StreamWriter sAddressHexWriter;

        public Assembler()
        {

            OpTable.Add("MOV" , new OpCode(1, 0));
            OpTable.Add("ADD" , new OpCode(1, 1));
            OpTable.Add("SUB" , new OpCode(1, 2));
            OpTable.Add("OR" , new OpCode(1, 3));
            OpTable.Add("AND" , new OpCode(1, 4));
            OpTable.Add("XOR" , new OpCode(1, 5));
            OpTable.Add("NOT" , new OpCode(1, 6));
            OpTable.Add("SHL" , new OpCode(1, 7));
            OpTable.Add("SHR" , new OpCode(1, 8));
            OpTable.Add("JMP" , new OpCode(1, 9));
            OpTable.Add("BRZ" , new OpCode(1, 10));
            OpTable.Add("BRC" , new OpCode(1, 11));
            OpTable.Add("BNZ" , new OpCode(1, 12));
            OpTable.Add("BNC" , new OpCode(1, 13));
            OpTable.Add("CLF" , new OpCode(1, 14));
            OpTable.Add("CLA" , new OpCode(1, 15));
        }
        public void Assemble(string SourcePath ,string OutputPath)
        {
            //SourcePath = "Source.txt";
            //OutputPath = "source";

            string ObjectPath = OutputPath + ".o";
            string IntermadiateFile = OutputPath + ".Intr";
            string ListingPath = OutputPath + ".lst";
            string OpcHexPath = OutputPath + "_OPC.hex";
            string AdrHexPath = OutputPath + "_ADR.hex";

            sReader = new StreamReader(SourcePath);
            sWriter = new StreamWriter(IntermadiateFile);
            SymTable.Clear();

            Pass1();
            sReader.Close();
            sWriter.Close();

            sReader = new StreamReader(SourcePath);
            sWriter = new StreamWriter(ObjectPath);

            Pass2();
            sReader.Close();
            sWriter.Close();

            if (ErrorList.Count != 0) // There is Error in source code
            {
                sWriter.Close();
                string Errors = "";
                for (int i = 0; i < ErrorList.Count; ++i)
                {
                    Errors += ErrorList[i].ToString() + "\r\n";
                }
                File.WriteAllText("ErrorList.txt", Errors); // write errors in a file
                if (File.Exists(ObjectPath))
                    File.Delete(ObjectPath); // delete object file
            }
            else
            {
                File.WriteAllText("ErrorList.txt", "No Error");

                sReader = new StreamReader(ObjectPath);
                sWriter = new StreamWriter(OpcHexPath);
                sAddressHexWriter = new StreamWriter(AdrHexPath);
                Ascii2Hex();
                sAddressHexWriter.Close();
            }
            sReader.Close();
            sWriter.Close();

        }
        public void ReadNextInputLine(out string InputLine)
        {
            InputLine = sReader.ReadLine().Trim().ToUpper();
            LineCounter++;
        }
        public void ReadNextInputLine(out string[] InputLine)
        {
            string LABLE;   // Lable field
            string OPCODE;  // Instruction filed
            string OPR1;    // Operand1 field
            string OPR2;    // Operand2 field


            InputLine = sReader.ReadLine().Trim().ToUpper().Split(new char[] { ' ', '\t' });
            LineCounter++;
        }
        public string Int2Hex(int Address)
        {
            string address = "";
            if (((Address >> 4) & 0x0F) < 10) // high nibble 0 to 9
            {
                int ad = ((Address >> 4) & 0x0F);
                ad += '0';
                char ch = Convert.ToChar(ad);
                address += ch;
            }
            else  // A to F
            {
                int ad = ((Address >> 4) & 0x0F);
                ad -= 10;
                ad += 'A';
                char ch = Convert.ToChar(ad);
                address += ch;
            }

            if ((Address & 0x0F) < 10) // low nibble 0 to 9
            {
                int ad = (Address & 0x0F);
                ad += '0';
                char ch = Convert.ToChar(ad);
                address += ch;
            }
            else  // A to F
            {
                int ad = (Address & 0x0F);
                ad -= 10;
                ad += 'A';
                char ch = Convert.ToChar(ad);
                address += ch;
            }

            return address;
        }

        public void Pass1()
        {
            string Line;
            LineCounter = 0;
            LocCounter = 0;
            VarLocCounter = 0;
            Line = sReader.ReadLine().Trim().ToUpper(); // Read Line
            LineCounter++;
            while (Line != "END")
            {
                if (!Line.StartsWith(";")) // if this is not a comment line
                {
                    // search SYMTAB for LABLE
                    if (Line.Contains(":")) // if there is Symbole in LABLE field
                    {
                        string LABLE = Line.Split(':')[0];
                        if (SymTable.ContainsKey(LABLE))
                        {
                            // if found set duplicate symbol definition error
                            //SymTable[LABLE].SymFlag = Symbol.SymFlags.DuplicateSymbol;
                            ErrorList.Add(new Error(LineCounter, "Duplicate Symbol"));
                        }
                        else
                        {
                            // if not found Inser (LABLE,LOCCTR) into SYMTBL
                            SymTable.Add(LABLE, new Symbol(LocCounter,Symbol.Types.Lable));
                        }
                        // remove lable to read opcode
                        Line = Line.Remove(0, Line.IndexOf(':') + 1).TrimStart();
                    }
                    // search OPTBL for OPCODE
                    string OPCODE = Line.Split(' ')[0];
                    // if found
                    if (OpTable.ContainsKey(OPCODE))
                    {
                        // add instruction length to location counter
                        LocCounter += OpTable[OPCODE].Length;
                    }
                    else
                    {

                        //if (OPCODE == "WORD")
                        //{
                        //    LocCounter += 1;
                        //}
                        //else
                        if (OPCODE == "BYTE")
                        {
                            string identifier = Line.Remove(0, 5);
                            SymTable.Add(identifier.Split(' ')[0], new Symbol(VarLocCounter, Symbol.Types.Variable));
                            VarLocCounter += 1;
                        }
                        else // Invalid opration code
                        {
                            // set error flag
                            ErrorList.Add(new Error(LineCounter, "Invalid opration code"));
                        }
                    }
                }
                // Write Line to intermadiate file
                sWriter.WriteLine(Line);
                // read next input line
                ReadNextInputLine(out Line);
            }// While (not END)
            // Write Last Line To Intermadiate File
            sWriter.WriteLine(Line);

        }// {PASS1}

        public void Pass2()
        {
            //read first input line {from intermadiat file}
            string Line;
            LocCounter = 0;
            LineCounter = 0;
            VarLocCounter = 0;
            ReadNextInputLine(out Line);
            while (Line != "END")
            {
                // if this is not acomment line
                if (!Line.StartsWith(";"))
                {
                    // Get Lable field
                    string LABLE = Line.Split(':')[0];
                    if (LABLE != null && LABLE != "")
                    {
                        // remove lable field
                        Line = Line.Remove(0, Line.IndexOf(':') + 1).TrimStart();
                    }
                    // Get OPCODE Field
                    string OPCODE = Line.Split(' ')[0];
                    string OPERAND = "";
                    if (
                        OPCODE != "NOT" &&
                        OPCODE != "CLF" &&
                        OPCODE != "CLA" &&
                        OPCODE != "SHL" &&
                        OPCODE != "SHR")
                    {
                        // Get OPERAND field
                        try
                        {
                            OPERAND = Line.Split(' ')[1];
                        }
                        catch (Exception exp)
                        {
                            ErrorList.Add(new Error(LineCounter , exp.Message));
                        }
                    }
                    // search OPTAB for OPCODE
                    if (OpTable.ContainsKey(OPCODE))
                    {
                        string OperandAddress = "FF";
                        string MashineCode = "";
                        // if there is a SYMBOL in operand Field
                        if (!OPERAND.StartsWith("0X"))// && !OPERAND.StartsWith("#"))
                        {
                            // search SYMTBL for OPERAND
                            if(OPERAND != "")
                                if (SymTable.ContainsKey(OPERAND))
                                {
                                    // if found
                                    // store symbol value as operand address
                                    OperandAddress = Int2Hex(SymTable[OPERAND].Address);
                                }
                                else
                                {
                                    // store 0 as operand address
                                    // set error flag (Undefined Symbol)
                                    OperandAddress = "00";
                                    ErrorList.Add(new Error(LineCounter, "Undefined Symbol"));
                                }
                        } // {if symbol}
                        else
                        {
                            // store 0 as operand address
                            OperandAddress = OPERAND.Substring(OPERAND.Length - 2, 2);
                        }
                        // asemble the object code istruction
                        MashineCode = Int2Hex(OpTable[OPCODE].MachineCode);
                        string objCode = MashineCode
                            + ' '
                            + OperandAddress;
                        sWriter.WriteLine(objCode);
                    }// {if opcode found}
                    else
                    {
                        // if OPCODE = 'BYTE' or 'WORD'
                        // Convert constant to object code

                        // if object code will not fit into the current text record
                        // write text record to object program
                        // initialize new text record

                    }
                    // add object code to text record

                } // {if not comment}
                // write listing line
                // Read Next Input Line
                ReadNextInputLine(out Line);
            } // {while not END}

            // write last text record to object program
            // write END record to object program
            // write last listing line

        }// {Pass2}

        public void Ascii2Hex()
        {
            int RecLen = 0;
            int LoadAddress = 0x0000;
            int OpcodeCheckSum = 0;
            int AddressCheckSum = 0;

            string OpcodeRecord = ":";
            string OpcodeData = "";

            string AddressRecord = ":";
            string AddressData = "";

            string OPCODE;
            do
            {
                do
                {
                    OPCODE = sReader.ReadLine(); // Read input
                    OpcodeCheckSum += Hex2Int(OPCODE.Split(' ')[0]); // add to checksum
                    AddressCheckSum += Hex2Int(OPCODE.Split(' ')[1]);

                    OpcodeData += OPCODE.Split(' ')[0]; // add to data field in hex format
                    AddressData += OPCODE.Split(' ')[1];
                    RecLen++; // increase Data Lenth
                }
                while (RecLen < 16 && !sReader.EndOfStream);
                OpcodeCheckSum += LoadAddress + RecLen;
                AddressCheckSum += LoadAddress + RecLen;

                OpcodeRecord +=
                    Int2Hex(RecLen) +
                    Int2Hex(LoadAddress >> 8) +
                    Int2Hex(LoadAddress >> 0) +
                    "00" +
                    OpcodeData +
                    Int2Hex(-OpcodeCheckSum);

                AddressRecord +=
                    Int2Hex(RecLen) +
                    Int2Hex(LoadAddress >> 8) +
                    Int2Hex(LoadAddress >> 0) +
                    "00" +
                    AddressData +
                    Int2Hex(-AddressCheckSum);

                sWriter.WriteLine(OpcodeRecord);
                sAddressHexWriter.WriteLine(AddressRecord);

                OpcodeRecord = ":";
                OpcodeData = "";
                OpcodeCheckSum = 0;

                AddressRecord = ":";
                AddressData = "";
                AddressCheckSum = 0;

                RecLen = 0;
                LoadAddress += 16;
            }
            while (!sReader.EndOfStream);
            // end of record
            sWriter.WriteLine(":00000001FF");
            sAddressHexWriter.WriteLine(":00000001FF");
        }

        int Hex2Int(string Hex)
        {
            Hex = Hex.ToUpper();
            if (Hex.StartsWith("0X"))
                Hex = Hex.Remove(0, 2);

            int num = 0;

            if (Hex[0] >= '0' && Hex[0] <= '9')
                num += Hex[0] - '0';
            else
                num += Hex[0] - 'A' + 10;

            num <<= 4;

            if (Hex[1] >= '0' && Hex[1] <= '9')
                num += Hex[1] - '0';
            else
                num += Hex[1] - 'A' + 10;

            return num;
        }
    }
}
