using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.CustomModels
{
    public class MessageFileModel
    {
        public long msgFileID { get; set; }
        public Nullable<long> msgID { get; set; }
        public string fileName { get; set; }
        public byte[] fileContent { get; set; }
        
        
    }
}
