using AutoDependencyPropertyMarker;
using HotChocolatey.Model;

namespace HotChocolatey.View.Settings
{
    public partial class SettingsFeatureListItem
    {
        [AutoDependencyProperty]
        public ChocoFeature Feature { get; set; }

        public SettingsFeatureListItem()
        {
            InitializeComponent();
        }
    }
}
