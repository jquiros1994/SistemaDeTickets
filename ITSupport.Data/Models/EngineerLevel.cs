namespace ITSupport.Models
{
    public class EngineerLevel
    {
        public int LevelId { get; set; }
        public string LevelName { get; set; }
        public string Description { get; set; }

        public EngineerLevel() { }

        public EngineerLevel(int levelId, string levelName, string description)
        {
            LevelId = levelId;
            LevelName = levelName;
            Description = description;
        }
    }
}
