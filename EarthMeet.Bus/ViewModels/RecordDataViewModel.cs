// SPDX-FileCopyrightText: 2026 Tayra Sakurai
// SPDX-License-Identifier: GPL-3.0-or-later
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using EarthMeet.Bus.Messages;
using EarthMeet.Bus.Models;
using Google.GenAI;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Windows.Media.Capture;
using Windows.Storage;

namespace EarthMeet.Bus.ViewModels
{
    public partial class RecordDataViewModel : ObservableObject
    {
        private readonly RecordData recordData;
        private readonly MediaCapture mediaCapture;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(RecordCommand))]
        [NotifyCanExecuteChangedFor(nameof(GetTextCommand))]
        public partial LowLagMediaRecording? MediaRecording { get; set; }

        public string Vocaburary
        {
            get => string.Join(Environment.NewLine, recordData.Vocaburary);
            set => SetProperty(string.Join(Environment.NewLine, recordData.Vocaburary), value, recordData, (m, v) => m.Vocaburary = v.Split(Environment.NewLine));
        }

        public RecordDataViewModel(Client client, MediaCapture mediaCapture)
        {
            recordData = new(client);
            this.mediaCapture = mediaCapture;
        }

        [RelayCommand(AllowConcurrentExecutions = false, CanExecute = nameof(CanRecord))]
        private async Task RecordAsync()
        {
            await mediaCapture.InitializeAsync();
            StorageFolder folder = ApplicationData.Current.LocalFolder;
            StorageFile file = await folder.CreateFileAsync("audio.mp3", CreationCollisionOption.GenerateUniqueName);
            MediaRecording = await mediaCapture.PrepareLowLagRecordToStorageFileAsync(
                Windows.Media.MediaProperties.MediaEncodingProfile.CreateMp3(Windows.Media.MediaProperties.AudioEncodingQuality.Medium), file);
            await MediaRecording.StartAsync();
            recordData.VoiceDataFile = file;
        }

        private bool CanRecord()
        {
            return MediaRecording is null;
        }

        [RelayCommand(AllowConcurrentExecutions = false, CanExecute = nameof(CanGetText))]
        private async Task GetTextAsync()
        {
            if (MediaRecording == null)
                return;
            await MediaRecording.StopAsync();
            MediaRecording = null;
            await recordData.TranscribeAsync();
            StorageFile saveFile = await WeakReferenceMessenger.Default.Send<VoiceTranscribedMessage>();
            await FileIO.WriteTextAsync(saveFile, recordData.Transcript);
        }

        private bool CanGetText()
        {
            if (MediaRecording == null) return false;
            return true;
        }
    }
}
