using System;

namespace Tenacious.Data
{
    [Serializable]
    public class SaveData
    {
        public string fileName;

        public SaveData(string fileName = null)
        {
            this.fileName = fileName == null || fileName.Trim().Equals("") ? DateTime.Now.ToString("yyyyMMdd_HHmmssfff") : fileName;
        }
    }
}
