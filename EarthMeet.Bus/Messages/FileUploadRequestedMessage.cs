// SPDX-FileCopyrightText: 2026 Tayra Sakurai
// SPDX-License-Identifier: GPL-3.0-or-later
using CommunityToolkit.Mvvm.Messaging.Messages;
using System;
using System.Collections.Generic;
using System.Text;
using Windows.Storage;

namespace EarthMeet.Bus.Messages
{
    public class FileUploadRequestedMessage : AsyncRequestMessage<StorageFile>
    {
    }
}
