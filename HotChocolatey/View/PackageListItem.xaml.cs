using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using NuGet;

namespace HotChocolatey.View
{
    public partial class PackageListItem : UserControl
    {
        public PackageListItem()
        {
            InitializeComponent();
        }

       
    }
    public class DesignTimeContext
    {
        public Uri Ico { get; } = new Uri("http://cdn.sstatic.net/stackoverflow/img/favicon.ico");
        public string Title { get; } = "Test package";
        public string Summary { get; } = "blablalblalbalblabllalblablbalba";
        public bool IsInstalled { get; } = true;
        public bool IsUpgradable { get; } = true;
        public string Authors { get; } = "By Me Myself and I";
        public long DownloadCount { get; } = 213456546;
        public bool IsPreRelease { get; } = false;
        public SemanticVersion LatestVersion { get; } = new SemanticVersion(4, 3, 2, 1);
        public SemanticVersion CurrentVersion { get; } = new SemanticVersion(1, 2, 3, 4);

    }
}
