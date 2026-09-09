// SPDX-FileCopyrightText: 2026 Tayra Sakurai
// SPDX-License-Identifier: GPL-3.0-or-later
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.Windows.Storage;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace EarthMeet
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Activated += MainWindow_Activated;
            MainNav.ItemInvoked += MainNav_ItemInvoked;
        }

        private void MainNav_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            if (args.InvokedItem == Rec.Content)
                PageFrame.Navigate(typeof(RecordPage));
            else if (args.IsSettingsInvoked)
                PageFrame.Navigate(typeof(SettingsPage));
        }

        private void MainWindow_Activated(object sender, WindowActivatedEventArgs args)
        {
            if (PageFrame.SourcePageType is null)
            {
                if (!string.IsNullOrWhiteSpace(ApplicationData.GetDefault().LocalSettings.Values["API_KEY"] as string))
                    PageFrame.Navigate(typeof(RecordPage));
                else
                {
                    PageFrame.Navigate(typeof(SettingsPage));
                    Rec.IsEnabled = false;
                }
            }
        }
    }
}
