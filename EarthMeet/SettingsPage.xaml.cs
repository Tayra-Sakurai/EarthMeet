// SPDX-FileCopyrightText: 2026 Tayra Sakurai
// SPDX-License-Identifier: GPL-3.0-or-later

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.Windows.AppLifecycle;
using Microsoft.Windows.Storage;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SettingsPage : Page
    {
        public SettingsPage()
        {
            InitializeComponent();
            APIKey.LostFocus += APIKey_LostFocus;
            RestartButton.Click += RestartButton_Click;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            Debug.WriteLine(ApplicationData.GetDefault().LocalSettings.Values["API_KEY"]);

            APIKey.Text = string.IsNullOrEmpty(ApplicationData.GetDefault().LocalSettings.Values["API_KEY"] as string) ? (Environment.GetEnvironmentVariable("GOOGLE_API_KEY") ?? (Environment.GetEnvironmentVariable("GEMINI_API_KEY") ?? string.Empty)) : (ApplicationData.GetDefault().LocalSettings.Values["API_KEY"] as string);
        }

        private void RestartButton_Click(object sender, RoutedEventArgs e)
        {
            ApplicationData.GetDefault().LocalSettings.Values["API_KEY"] = APIKey.Text;
            AppInstance.Restart(string.Empty);
        }

        private void APIKey_LostFocus(object sender, RoutedEventArgs e)
        {
            ApplicationData.GetDefault().LocalSettings.Values["API_KEY"] = APIKey.Text;
        }
    }
}
