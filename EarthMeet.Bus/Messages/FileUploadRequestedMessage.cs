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
