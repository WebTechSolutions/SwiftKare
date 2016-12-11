using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.CustomModels
{
    public class GetPatientUserFiles
    {
        public long fileID { get; set; }
        public string documentType { get; set; }
        public string FileName { get; set; }
        public byte[] fileContent { get; set; }
        public Nullable<long> patientID { get; set; }
        public Nullable<long> doctorID { get; set; }

        /// <summary>
        /// Returns file size
        /// </summary>
        public string FileSize
        {
            get
            {
                string[] sizes = { "B", "KB", "MB", "GB" };
                double len = fileContent.Length;
                int order = 0;
                while (len >= 1024 && ++order < sizes.Length)
                {
                    len = len / 1024;
                }

                // Adjust the format string to your preferences. For example "{0:0.#}{1}" would
                // show a single decimal place, and no space.
                return String.Format("{0:0.##} {1}", len, sizes[order]);
            }
        }

    }
}
