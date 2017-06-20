namespace BachelorThesis.Abstractions.Models
{
    public sealed class TrainingModel
    {
        public string UserId { get; set; }

        public string UserQuestion { get; set; }

        public string KbQuestion { get; set; }

        public string KbAnswer { get; set; }
    }
}
