using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SubtitleBackoffice.JSON.ProgramList
{
    public class ProgramList
    {
        public string Info;
        public string Type;
        public DateTime GenerateDate;
        public int ProgramCount = 0;
        public List<Program> Programs;

        public ProgramList()
        {
            Programs = new List<Program>();
        }

        public void Add(Program program)
        {
            this.Programs.Add(program);
            this.ProgramCount++;
        }
    }
}