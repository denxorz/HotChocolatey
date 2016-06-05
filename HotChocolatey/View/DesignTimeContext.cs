using System;
using NuGet;

namespace HotChocolatey.View
{
    public class DesignTimeContext
    {
        public Uri Ico { get; } = new Uri("https://gitlab.com/uploads/project/avatar/331561/Hot_Chocolate-100.png");
        public string Title { get; } = "Lorem ipsum dolor sit amet.";
        public string Summary { get; } = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Duis rutrum venenatis mattis. Morbi euismod risus vel pharetra ornare. Ut vel velit et odio dignissim ullamcorper at at eros.";
        public bool IsInstalled { get; } = true;
        public bool IsUpgradable { get; } = true;
        public string Authors { get; } = "By Me Myself and I";
        public long DownloadCount { get; } = 213456546;
        public bool IsPreRelease { get; } = false;
        public SemanticVersion LatestVersion { get; } = new SemanticVersion(4, 3, 2, 1);
        public SemanticVersion CurrentVersion { get; } = new SemanticVersion(1, 2, 3, 4);
    }
}