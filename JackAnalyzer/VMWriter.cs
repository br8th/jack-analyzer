using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JackAnalyzer
{
    internal class VMWriter
    {
        private StreamWriter writer;
        private StringBuilder sb;

        public enum Segment
        {
            CONST,
            ARG,
            LOCAL,
            STATIC,
            THIS,
            THAT,
            POINTER,
            TEMP
        }

        public enum Command
        {
            ADD,
            SUB,
            NEG,
            EQ,
            GT,
            LT,
            AND,
            OR,
            NOT
        }

        public VMWriter(string outputFile)
        {
            this.writer = new StreamWriter(outputFile);
            this.sb = new StringBuilder();
        }

        // Writes a VM push command
        private void WritePush(Segment s, int index)
        {

        }

        // Writes a VM pop command
        private void WritePop(Segment s, int index)
        {

        }

        private void WriteArithmetic(Command command)
        {

        }

        private void WriteLabel(String label)
        {

        }

        // Writes a VM if-goto command
        private void WriteGoto(String label)
        {

        }

        private void WriteIf(String label)
        {

        }

        private void WriteCall(String name, int nArgs)
        {

        }

        private void WriteFunction(String name, int nLocals)
        {

        }

        private void WriteReturn()
        {

        }


        // Closes the output file
        public void Close()
        {
            writer.Dispose();
        }
    }
}
