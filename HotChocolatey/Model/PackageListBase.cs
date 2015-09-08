using System.Collections.Generic;
using System.Threading.Tasks;

namespace HotChocolatey.Model
{
    public abstract class PackageListBase
    {
        protected string searchFor;
        protected int skipped;

        public abstract bool HasMore { get; }
        public abstract Task Refresh();
        public abstract Task<IEnumerable<ChocoItem>> GetMore(int numberOfItems);

        protected bool PackageSearchComparer(ChocoItem package, string searchFor)
        {
            return package.Tags.Contains(searchFor) || package.Title.Contains(searchFor);
        }

        public virtual async Task ApplySearch(string searchFor)
        {
            this.searchFor = searchFor.ToLower();
            skipped = 0;
        }
    }
}
