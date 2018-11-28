using System;

namespace DataAdapter.Inside
{
    public class StudentVisitSearchObject : IDataObject
    {

        public Student Student { get; }

        public DateTime DateTime { get; }

        public string Classroom { get; }

        public string Subject { get; }

        public string TypeOfClass { get; }

        public StudentVisitSearchObject(Student student, DateTime dateTime, string classroom, string subject, string typeofclass)
        {
            Student = student;
            DateTime = dateTime;
            Classroom = classroom;
            Subject = subject;
            TypeOfClass = typeofclass;

            ValidateData();
        }

        public void ValidateData()
        {
            DataValidator.ValidateDateTime(DateTime, "Дата");
            DataValidator.ValidateFieldTextAdvanced(Classroom, "Аудитория");
            DataValidator.ValidateFieldTextAdvanced(Subject, "Предмет");
            DataValidator.ValidateFieldTextAdvanced(TypeOfClass, "Тип занятия");
        }
    }
}
