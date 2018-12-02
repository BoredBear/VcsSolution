using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAdapter.Outside;
using DataAdapter.Inside;

namespace ARDIS
{
    public class ArdisWorker
    {
        MySql sql = new MySql();
        Task shedulePlanner;

        public ArdisWorker()
        {
            shedulePlanner = new Task(() => FillSchedule());
            shedulePlanner.Start();
        }

        private void FillSchedule()
        {
            int pairNum = 1;
            while(true)
            {
                if(GetPairNumber() != pairNum)
                {
                    pairNum = GetPairNumber();
                    var idClass = sql.GetIdClassForGenerateSchedule(pairNum);
                    foreach(var item in idClass)
                    {
                        if (!sql.CheckExistsVisit(item.Idclass))
                        {
                            var students = sql.GetStudentsFromGroup(item.idstudentgroup);
                            foreach (string studentid in students)
                            {
                                sql.NewStudentVisit(new StudentVisitRequest()
                                {
                                    idclass = item.Idclass,
                                    idstudent = studentid
                                });
                            }
                        }
                    }
                }
                System.Threading.Thread.Sleep(10000);
            }
        }

        public void Send(string cardNumber, string classroom)
        {
            try
            {
                string studentId = sql.GetStudentIdByCardNumber(cardNumber);
                if(studentId.Length < 1)
                {
                    throw new Exception("Студент не найден в базе");
                }
                var request = new PresenseSetRequest()
                {
                    Classroom = classroom,
                    Dayoftheweek = (int)DateTime.Now.DayOfWeek,
                    StudentId = studentId,
                    PairNumber = GetPairNumber()
                };
                if (!sql.SetStudentVisit(request))
                {
                    throw new Exception("Не удалось отметить посещение");
                }
            }
            catch
            {
                throw;
            }
        }

        private int GetPairNumber()
        {
            int pairNum = 0;
            var time = new TimeOfDay()
            {
                Hour = DateTime.Now.Hour,
                Minute = DateTime.Now.Minute
            };
            if(time.Hour < 11 && time.Minute < 30)
            {
                pairNum = 1;
            } else if((time.Hour < 13) || (time.Hour == 13 && time.Minute < 10))
            {
                pairNum = 2;
            } else if(time.Hour < 15 || (time.Hour == 15 && time.Minute < 30))
            {
                pairNum = 3;
            } else if (time.Hour < 16 || (time.Hour == 16 && time.Minute < 40))
            {
                pairNum = 4;
            } else if (time.Hour < 18 || (time.Hour == 18 && time.Minute < 50))
            {
                pairNum = 5;
            } else
            {
                pairNum = 6;
            }

            return pairNum;
        }
    }

    public class TimeOfDay
    {
        public int Hour { get; set; }
        public int Minute { get; set; }
    }
}
