using Bindables;
using HotChocolatey.Model;

namespace HotChocolatey.View.Settings
{
    [DependencyProperty]
    public partial class SettingsFeatureListItem
    {
       public ChocoFeature Feature { get; set; }

        public SettingsFeatureListItem()
        {
            InitializeComponent();
        }
    }
}
