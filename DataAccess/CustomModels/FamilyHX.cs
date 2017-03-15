using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.CustomModels
{
    public class FamilyHX
    {
        public long familyHXItemsID { get; set; }
        public string name { get; set; }
    }

    public class UpdateFamilyHX
    {
        public long patientfamilyHXID { get; set; }
        public long patientID { get; set; }
        public string relationship { get; set; }
    }

    public class RelationshipModel
    {
        public long relationshipID { get; set; }
        public string name { get; set; }
        
    }

    public class PFamilyList
    {
        public Nullable<long> fhxID { get; set; }
        public long familyHXItemsID { get; set; }
        public Nullable<long> patientID { get; set; }
        public string name { get; set; }
        public string relationship { get; set; }
    }

}
