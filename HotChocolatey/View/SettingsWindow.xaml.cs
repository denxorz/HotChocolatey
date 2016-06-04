﻿using System.ComponentModel;
using System.Windows;
using MahApps.Metro.Controls;

namespace HotChocolatey.View
{
    public partial class SettingsWindow : MetroWindow
    {
        public SettingsWindow()
        {
            InitializeComponent();
        }

        private async void OnLoaded(object sender, RoutedEventArgs e)
        {
            // TODO : Not really neat, but good enough for now
            await ((ViewModel.SettingsWindowsViewModel)DataContext).Loaded();
        }

        private async void OnClosing(object sender, CancelEventArgs e)
        {
            // TODO : Not really neat, but good enough for now
            await ((ViewModel.SettingsWindowsViewModel)DataContext).Closing();
        }
    }
}
