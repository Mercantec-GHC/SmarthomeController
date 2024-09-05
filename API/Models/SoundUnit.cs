namespace API.Models
{
    public class SoundUnit : Common
    {
        public string UnitName { get; set; }
        public List<Sound> DecibelData { get; set; } = new List<Sound>();
    }

    public class CreateSoundUnitDTO
    {
        public string UnitName { get; set; }
    }

    public class SoundUnitDTO
    {
        public int Id { get; set; }
        public string UnitName { get; set; }
        public List<SoundDTO> DecibelData { get; set; } = new List<SoundDTO>();
    }
}