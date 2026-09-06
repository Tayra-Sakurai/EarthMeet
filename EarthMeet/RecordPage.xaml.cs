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

        public void Receive(VoiceTranscribedMessage message)
        {
            message.Reply(GetSaveFileAsync());
        }

        public void Receive(FileUploadRequestedMessage message)
        {
            message.Reply(GetUploadFileAsync());
        }

        private static async Task<StorageFile> GetUploadFileAsync()
        {
            WindowId? wId = (App.Current as App)!.WindowId;
            if (wId is not WindowId windowId)
                throw new NullReferenceException("The value 'null' is not valid.");

            FileOpenPicker fileOpenPicker = new(windowId)
            {
                CommitButtonText = "選択",
                SuggestedStartLocation = PickerLocationId.VideosLibrary,
            };
            fileOpenPicker.FileTypeChoices.Clear();
            fileOpenPicker.FileTypeChoices.Add(
                new(
                    "MP3ファイル",
                    new List<string> { ".mp3" }));
            fileOpenPicker.FileTypeChoices.Add(
                new(
                    "WAVファイル",
                    new List<string> { ".wav" }));

            PickFileResult pickFileResult = await fileOpenPicker.PickSingleFileAsync();

            if (string.IsNullOrEmpty(pickFileResult.Path))
                throw new NullReferenceException("You must pick a file.");
            else
            {
                StorageFile storageFile = await StorageFile.GetFileFromPathAsync(pickFileResult.Path);
                return storageFile;
            }
        }

        private static async Task<StorageFile> GetSaveFileAsync()
        {
            WindowId? wId = (App.Current as App)!.WindowId;
            if (wId is not WindowId windowId)
                throw new NullReferenceException();

            FileSavePicker fileSavePicker = new(windowId)
            {
                CommitButtonText = "保存",
                DefaultFileExtension = ".md",
                ShowOverwritePrompt = false,
                SuggestedFileName = "output.md",
                SuggestedStartLocation = PickerLocationId.DocumentsLibrary,
            };
            fileSavePicker.FileTypeChoices.Clear();
            fileSavePicker.FileTypeChoices.Add(
                new(
                    "Markdownファイル",
                    new List<string> { ".md" }));

            PickFileResult pickFileResult = await fileSavePicker.PickSaveFileAsync();
            if (string.IsNullOrEmpty(pickFileResult.Path))
                throw new NotImplementedException();

            StorageFile storageFile = await StorageFile.GetFileFromPathAsync(pickFileResult.Path);
            return storageFile;
        }
    }
}
