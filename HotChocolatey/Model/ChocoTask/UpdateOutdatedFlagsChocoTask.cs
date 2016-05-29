namespace HotChocolatey.Model.ChocoTask
{
    internal class UpdateOutdatedFlagsChocoTask : BaseChocoTask
    {
        protected override string GetCommand()
        {
            return "outdated";
        }

        protected override string GetParameters()
        {
            return string.Empty;
        }
    }
}