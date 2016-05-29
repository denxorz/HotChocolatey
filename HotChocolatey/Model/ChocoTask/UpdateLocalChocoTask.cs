namespace HotChocolatey.Model.ChocoTask
{
    internal class UpdateLocalChocoTask : BaseChocoTask
    {
        private readonly bool includePreReleases;

        public UpdateLocalChocoTask(bool includePreReleases)
        {
            this.includePreReleases = includePreReleases;
        }

        protected override string GetCommand()
        {
            return "list";
        }

        protected override string GetParameters()
        {
            var includePreRelease = includePreReleases ? "--prerelease" : string.Empty;
            return $"--localonly {includePreRelease}";
        }
    }
}