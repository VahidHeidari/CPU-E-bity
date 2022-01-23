using System;
using System.Collections.Generic;
using System.Text;

namespace Assembler
{
    class Program
    {
        static void Main(string[] args)
        {
            Assembler asm = new Assembler();

            string Source,Output;
            Console.WriteLine("Enter Source and Output Name : ");
            Source = Console.ReadLine();
            Output = Console.ReadLine();
            asm.Assemble(Source, Output);
        }
    }
}
