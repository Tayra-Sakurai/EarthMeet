// SPDX-FileCopyrightText: 2026 Tayra Sakurai
// SPDX-License-Identifier: GPL-3.0-or-later
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Messaging;
using EarthMeet.Bus.Messages;
using EarthMeet.Bus.ViewModels;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.Windows.Storage.Pickers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace EarthMeet
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class RecordPage : Page, IRecipient<VoiceTranscribedMessage>, IRecipient<FileUploadRequestedMessage>
    {
        private RecordDataViewModel? viewModel;

        public RecordPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            viewModel = Ioc.Default.GetRequiredService<RecordDataViewModel>();
            WeakReferenceMessenger.Default.Register<VoiceTranscribedMessage>(this);
            WeakReferenceMessenger.Default.Register<FileUploadRequestedMessage>(this);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            WeakReferenceMessenger.Default.UnregisterAll(this);
        }

        ~RecordPage()
        {
            WeakReferenceMessenger.Default.UnregisterAll(this);
        }

        public async void Receive(VoiceTranscribedMessage message)
        {
            WindowId? windowId = (App.Current as App)!.WindowId;
            if (windowId is not WindowId wId)
                throw new NullReferenceException("Window ID was null.");
            FileSavePicker fileSavePicker = new(wId)
            {
                Title = "保存先を選択",
                CommitButtonText = "保存",
                DefaultFileExtension = ".md",
                SuggestedFileName = "transcript.md",
                SuggestedStartLocation = PickerLocationId.DocumentsLibrary,
            };
            fileSavePicker.FileTypeChoices.Add(
                "Markdownファイル",
                new List<string> { ".md" });
            PickFileResult result = await fileSavePicker.PickSaveFileAsync();
            StorageFile storageFile = await StorageFile.GetFileFromPathAsync(result.Path);
            message.Reply(storageFile);
        }

        public async void Receive(FileUploadRequestedMessage message)
        {
            WindowId? wId = (App.Current as App)!.WindowId;
            if (wId is not WindowId windowId)
                throw new NullReferenceException("Window id must be specified");

            FileOpenPicker fileOpenPicker = new(windowId)
            {
                CommitButtonText = "開く",
                SuggestedStartLocation = PickerLocationId.VideosLibrary,
            };
            fileOpenPicker.FileTypeChoices.Clear();
            fileOpenPicker.FileTypeChoices.Add(
                "MP3ファイル",
                new List<string>
                {
                    ".mp3",
                });
            PickFileResult result = await fileOpenPicker.PickSingleFileAsync();
            StorageFile storageFile = await StorageFile.GetFileFromPathAsync(result.Path);
            message.Reply(storageFile);
        }
    }
}
