﻿using Bindables;
using HotChocolatey.Model;

namespace HotChocolatey.View.Main
{
    [DependencyProperty]
    public partial class PackageInfoView
    {
        public Package Package { get; set; }

        public PackageInfoView()
        {
            InitializeComponent();
        }
    }
}
