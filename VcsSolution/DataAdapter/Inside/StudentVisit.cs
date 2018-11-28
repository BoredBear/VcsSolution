using System;
using System.Collections.Generic;
using System.Text;

namespace DataAdapter.Inside
{
    public class StudentVisit : IDataObject
    {
        public int Id { get; }

        public string FirstName { get; }

        public string LastName { get; }

        public string PastName { get; }

        public string Group { get; }

        public DateTime DateTime { get; }

        public string Classroom { get; }

        public string Subject { get; }

        public bool Presense { get; private set; }

        public string TypeOfClass { get; }

        /// <summary></summary>
        /// <param name="id">Если нет -1</param>
        /// <param name="fName"></param>
        /// <param name="lName"></param>
        /// <param name="pName"></param>
        /// <param name="group"></param>
        /// <param name="dt"></param>
        /// <param name="classroom"></param>
        /// <param name="subject"></param>
        /// <param name="presense"></param>
        public StudentVisit(int id, string fName, string lName, string pName, string group, DateTime dt, string classroom, string subject, bool presense, string typeofclass)
        {
            Id = id;
            FirstName = fName;
            LastName = lName;
            PastName = pName;
            Group = group;
            DateTime = dt;
            Classroom = classroom;
            Presense = presense;
            Subject = subject;
            TypeOfClass = typeofclass;
            ValidateData();
        }

        public void SetPresense(bool presense)
        {
            Presense = presense;
        }

        public void ValidateData()
        {
            DataValidator.ValidateFieldTextRequired(FirstName, "Имя");
            DataValidator.ValidateFieldTextRequired(LastName, "Имя");
            DataValidator.ValidateFieldText(PastName, "Имя");
            DataValidator.ValidateFieldTextAdvanced(Group, "Группа");
            DataValidator.ValidateDateTime(DateTime, "Дата занятия");
            DataValidator.ValidateFieldTextAdvanced(Classroom, "Группа");
            DataValidator.ValidateFieldTextAdvanced(Subject, "Предмет");
            DataValidator.ValidateFieldTextAdvanced(TypeOfClass, "Тип занятия");
        }    
    }
}