namespace API.Models
{
    public class Sound : Common
    {
        public int Decibel { get; set; }
        public DateTime Time { get; set; }
        public int SoundUnitId { get; set; }
    }

    public class CreateSoundDTO
    {
        public int Decibel { get; set; }
        public DateTime Time { get; set; }
        public int SoundUnitId { get; set; }
    }

    public class SoundDTO
    {
        public int Decibel { get; set; }
        public DateTime Time { get; set; }
    }
}