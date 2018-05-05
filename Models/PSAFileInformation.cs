using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SearchBotUpdated.Models
{
    public class PSAFileInformation
    {
        // File attributes
        public string Domain { get; set; }
        public string storageType { get; set; }
        public string fileLocation { get; set; }
        public string fileName { get; set; }
        public string fileTags { get; set; }

        // This method is used to parse information from .csv files into instantiated PSAFileInformation object's attributes
        public static PSAFileInformation FromCsv(string csvLine)
        {
            string[] values = csvLine.Split(',');
            PSAFileInformation fileInformation = new PSAFileInformation();
            fileInformation.Domain = values[0];
            fileInformation.storageType = values[1];
            fileInformation.fileLocation = values[2];
            fileInformation.fileName = values[3];
            fileInformation.fileTags = values[4];
            return fileInformation;
        }
    }
}