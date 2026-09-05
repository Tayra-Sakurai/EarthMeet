// SPDX-FileCopyrightText: 2026 Tayra Sakurai
// SPDX-License-Identifier: GPL-3.0-or-later
using Google.GenAI;
using Google.GenAI.Types;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace EarthMeet.Bus.Models
{
    public class RecordData
    {
        private readonly Client client;
        public string? Transcript { get; set; }
        public StorageFile? VoiceDataFile { get; set; }
        public ICollection<string> Vocaburary { get; set; } = new List<string>();

        public RecordData(Client client)
        {
            this.client = client;
        }

        public async Task TranscribeAsync()
        {
            if (VoiceDataFile is null)
                return;

            File file = await client.Files.UploadAsync(
                (await FileIO.ReadBufferAsync(VoiceDataFile)).ToArray(),
                VoiceDataFile.Name);

            GenerateContentResponse response = await client.Models.GenerateContentAsync(
                "gemini-3.5-transcribe",
                [
                    new()
                    {
                        Role = "user",
                        Parts = [
                            new()
                            {
                                FileData = new()
                                {
                                    FileUri = file.Uri,
                                    MimeType = file.MimeType,
                                },
                            },
                        ],
                    }
                ],
                new()
                {
                    AudioTranscriptionConfig = new()
                    {
                        CustomVocabulary = [..Vocaburary],
                        Mode = AudioTranscriptionConfigMode.Smart,
                    },
                });

            Transcript = response.Text;
        }
    }
}
