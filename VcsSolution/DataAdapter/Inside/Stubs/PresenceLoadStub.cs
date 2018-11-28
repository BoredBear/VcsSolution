using System;
using System.Collections.Generic;
using System.Text;

namespace DataAdapter.Inside.Stubs
{
    public static class PresenceLoadStub
    {

        public static StudentVisit GetStudentVisit(string firstName, string lastName, string pastName, string group, string classroom, string subject, DateTime dt, bool presence,string typeOfClass)
        {
            return new StudentVisit(-1, firstName, lastName, pastName, group, dt, classroom, subject, presence, typeOfClass);
        }
    }
}
