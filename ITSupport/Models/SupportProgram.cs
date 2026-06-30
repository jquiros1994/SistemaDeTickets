namespace ITSupport.Models
{
    public class SupportProgram
    {
        public int ProgramId { get; set; }
        public string ProgramName { get; set; }
        public string SupportLevel { get; set; }
        public string Description { get; set; }

        public SupportProgram() { }

        public SupportProgram(int programId, string programName, string supportLevel, string description)
        {
            ProgramId = programId;
            ProgramName = programName;
            SupportLevel = supportLevel;
            Description = description;
        }
    }
}
