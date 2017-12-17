namespace HotChocolatey.Model
{
    public class ChocoFeature
    {
        public string Name { get; }
        public string Description { get; }
        public bool IsEnabled { get; set; }

        public ChocoFeature(string name, string description, bool isEnabled)
        {
            IsEnabled = isEnabled;
            Description = description;
            Name = name;
        }
    }
}