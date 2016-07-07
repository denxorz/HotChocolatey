using System;
using NuGet;

namespace HotChocolatey.View
{
    public class DesignTimeContext
    {
        public Uri Ico { get; } = new Uri("https://gitlab.com/uploads/project/avatar/331561/Hot_Chocolate-100.png");
        public Uri SvgIco { get; } = new Uri("https://upload.wikimedia.org/wikipedia/commons/0/02/SVG_logo.svg");
        public bool IsIcoSvg { get; } = false;
        public string Title { get; } = "Lorem ipsum dolor sit amet.";
        public string Summary { get; } = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Duis rutrum venenatis mattis. Morbi euismod risus vel pharetra ornare. Ut vel velit et odio dignissim ullamcorper at at eros.";
        public bool IsInstalled { get; } = true;
        public bool IsUpgradable { get; } = true;
        public string Authors { get; } = "By Me Myself and I";
        public string ProjectUrl { get; } = "http://test.com";
        public long DownloadCount { get; } = 213456546;
        public bool IsPreRelease { get; } = false;
        public SemanticVersion LatestVersion { get; } = new SemanticVersion(4, 3, 2, 1);
        public SemanticVersion CurrentVersion { get; } = new SemanticVersion(1, 2, 3, 4);
        public string Tags { get; } = "tttt";
        public string DescriptionAsHtml { get; } = "<b>Descripti</b>on";
    }

    public class DesignTimeContext2
    {
        public DesignTimeContext SelectedPackage { get; } = new DesignTimeContext();
    }
}