using System;
using System.Collections.Generic;

namespace Tenacious.Data
{
    [Serializable]
    public class SaveMetadata
    {
        public string lastSave;
        public List<string> fileNames;

        public SaveMetadata()
        {
            fileNames = new List<string>();
        }
    }
}