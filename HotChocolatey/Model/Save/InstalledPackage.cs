namespace HotChocolatey.Model.Save
{
    public class InstalledPackage
    {
        public string Id { get; set; }
        public int WorkStationId { get; set; }
        public WorkStation WorkStation { get; set; }
    }
}
