using System;

namespace DataAdapter.Inside
{
    public class PresenseSetRequest : IDataObject
    {
        public string StudentId { get; set; }
        public string Classroom { get; set; }
        public int Dayoftheweek { get; set; }
        public int PairNumber { get; set; }
        
        public void ValidateData()
        {
            DataValidator.ValidateFieldTextRequired(StudentId, "CardNumber");
            DataValidator.ValidateFieldTextRequired(Classroom, "Classroom");
        }
    }
}
