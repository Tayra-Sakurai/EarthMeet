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
        private LowLagMediaRecording? mediaRecording;

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

        [RelayCommand(AllowConcurrentExecutions = false)]
        private async Task RecordAsync()
        {
            await mediaCapture.InitializeAsync();
            StorageFolder folder = ApplicationData.Current.LocalFolder;
            StorageFile file = await folder.CreateFileAsync("audio.mp3", CreationCollisionOption.GenerateUniqueName);
            mediaRecording = await mediaCapture.PrepareLowLagRecordToStorageFileAsync(
                Windows.Media.MediaProperties.MediaEncodingProfile.CreateMp3(Windows.Media.MediaProperties.AudioEncodingQuality.Medium), file);
            await mediaRecording.StartAsync();
            recordData.VoiceDataFile = file;
        }

        [RelayCommand(AllowConcurrentExecutions = false, CanExecute = nameof(CanGetText))]
        private async Task GetTextAsync()
        {
            if (mediaRecording == null)
                return;
            await mediaRecording.StopAsync();
            await recordData.TranscribeAsync();
            StorageFile saveFile = await WeakReferenceMessenger.Default.Send<VoiceTranscribedMessage>();
            await FileIO.WriteTextAsync(saveFile, recordData.Transcript);
        }

        private bool CanGetText()
        {
            if (mediaRecording == null) return false;
            return true;
        }
    }
}
