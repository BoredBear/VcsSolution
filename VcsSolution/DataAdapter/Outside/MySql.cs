using System;
using System.Collections.Generic;
using System.Text;
using DataAdapter.Inside;
using MySql.Data.MySqlClient;

namespace DataAdapter.Outside
{
    public class MySql
    {

        // Params
        private string host = "37.204.36.2";
        private int port = 3306;
        private string dataBase = "vcsdb";
        private string userName = "root";
        private string password = "1234";
        public MySqlConnection conn;

        // Func public
        public MySql()
        {
            conn = GetDBConnection();
        }
        /// <summary>
        /// Метод, который получает всех студентов по заданным параметрам
        /// </summary>
        /// <param name="student"></param>
        /// <returns></returns>
        public List<Student> GetStudent(StudentSearchObject student)
        {
            var students = new List<Student>();
            conn.Open();
            try
            {
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = conn;
                cmd.CommandText = StudentsDbRequest(student);
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    students.Add(new Student((int)reader["idstudent"], (string)reader["firstname"], (string)reader["lastname"], (string)reader["pastname"],(bool) reader["male"], student.Group));
                }
            }
            catch (NullReferenceException)
            {
                throw;
            }
            finally
            {
                conn.Close();
                conn.Dispose();
            }
            return students;
        }
        /// <summary>
        /// Метод, который получает все посещения по заданным параметрам
        /// </summary>
        /// <param name="studentVisit"></param>
        /// <returns></returns>
        public List<StudentVisit> GetStudentVisits(StudentVisitSearchObject studentVisit)
        {
            var studentVisits = new List<StudentVisit>();
            conn.Open();
            try
            {
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = conn;
                cmd.CommandText = StudentVisitsDbRequest(studentVisit);
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    studentVisits.Add(new StudentVisit((int)reader["idpresense"], studentVisit.Student.FirstName, studentVisit.Student.LastName, studentVisit.Student.PastName, studentVisit.Student.Group, (DateTime)reader["date"], studentVisit.Classroom, studentVisit.Subject, (bool)reader["presense"], studentVisit.TypeOfClass));
                }
            }
            catch (NullReferenceException)
            {
                throw;
            }
            finally
            {
                conn.Close();
                conn.Dispose();
            }

            return studentVisits;
        }
        public List<string> GetClassrooms()
        {
            var classrooms = new List<string>();
            conn.Open();
            try
            {
                var cmd = new MySqlCommand();
                cmd.Connection = conn;
                cmd.CommandText = ClassroomsDbRequest();
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    classrooms.Add((string)reader["classroom"]);
                }
            }
            catch(NullReferenceException)
            {
                throw;
            }
            finally
            {
                conn.Close();
                conn.Dispose();
            }
            return classrooms;
        }
        public List<string> GetSubjects()
        {
            var subjects = new List<string>();
            conn.Open();
            try
            {
                var cmd = new MySqlCommand();
                cmd.Connection = conn;
                cmd.CommandText = SubjectsDbRequest();
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    subjects.Add((string)reader["subject"]);
                }
            }
            catch (NullReferenceException)
            {
                throw;
            }
            finally
            {
                conn.Close();
                conn.Dispose();
            }
            return subjects;
        }
        /// <summary>
        /// Метод, который получает все группы
        /// </summary>
        /// <returns></returns>
        public List<string> GetStudentGroups()
        {
            var groups = new List<string>();
            conn.Open();
            try
            {
                var cmd = new MySqlCommand();
                cmd.Connection = conn;
                cmd.CommandText = GroupDbRequest();
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    groups.Add((string)reader["studentgroup"]);
                }
            }
            catch (NullReferenceException)
            {
                throw;
            }
            finally
            {
                conn.Close();
                conn.Dispose();
            }
            return groups;
        }

        public List<string> GetTypeOfClass()
        {
            var groups = new List<string>();
            conn.Open();
            try
            {
                var cmd = new MySqlCommand();
                cmd.Connection = conn;
                cmd.CommandText = TypeOfClassDbRequest();
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    groups.Add((string)reader["typeofclass"]);
                }
            }
            catch (NullReferenceException)
            {
                throw;
            }
            finally
            {
                conn.Close();
                conn.Dispose();
            }
            return groups;
        }
        
        // TODO: ЗАМЕНИТЬ НА СОЗДАНИЕ НОВОГО ПОСЕЩЕНИЯ с 0
        /// <summary>
        /// Метод, который создает новое посещение с переменной Присутствие=0
        /// </summary>
        /// <param name="studentVisit"></param>
        /// <returns></returns>
        public bool NewStudentVisit(StudentVisit studentVisit)
        {
            bool success = true;
            conn.Open();
            try
            {
                var cmd = new MySqlCommand();
                cmd.Connection = conn;
                cmd.CommandText = NewStudentVisitsDBRequest(studentVisit);
                var reader = cmd.ExecuteReader();
            }
            catch (NullReferenceException)
            {
                success = false;
                throw;
            }
            finally
            {
                conn.Close();
                conn.Dispose();
            }
            return success;
        }
        /// <summary>
        /// Метод, который устанавливает значение присутствия в новое, которое нужно ему задавать в StudentVisit.Presense по id
        /// </summary>
        /// <param name="studentVisit"></param>
        /// <returns></returns>
        public bool SetStudentVisit(StudentVisit studentVisit)
        {
            bool success = true;
            conn.Open();
            try
            {
                var cmd = new MySqlCommand();
                cmd.Connection = conn;
                cmd.CommandText = SetStudentVisitPresenseDBRequest(studentVisit);
                var qr  = cmd.ExecuteNonQuery();
                if(qr < 1)
                {
                    throw new Exception("Не удалось отметить посещение");
                }
            }
            catch (Exception)
            {
                success = false;
                throw;
            }
            finally
            {
                conn.Close();
                conn.Dispose();
            }
            return success;
        }

        public string GetStudentIdByCardNumber(string cardNumber)
        {
            conn.Open();
            try
            {
                var cmd = new MySqlCommand();
                cmd.Connection = conn;
                cmd.CommandText = StudentByCardDbRequest(cardNumber);
                var reader = cmd.ExecuteReader();
                if(reader.Read())
                {
                    return reader.GetString(0);
                }
            }
            catch (NullReferenceException)
            {
                throw;
            }
            finally
            {
                conn.Close();
                conn.Dispose();
            }
            return "";
        }

        // Func private
        private MySqlConnection GetDBConnection()
        {
            String connString = "Server=" + host + ";Database=" + dataBase
               + ";port=" + port + ";User Id=" + userName + ";password=" + password;

            MySqlConnection conn = new MySqlConnection(connString);

            return conn;
        }
        private String StudentsDbRequest(StudentSearchObject student)
        {
            string request;
            var male = student.Male ? 1 : 0;
            request = "SELECT * FROM vcsdb.students " +
                    $"WHERE firstname = '{student.FirstName}' " +
                    $"AND lastname = '{student.LastName}' " +
                    $"AND male = '{male}' ";
            if(student.PastName != null)
            {
                request += $"AND pastname = '{student.PastName}' ";
            }
            if(student.Group != null)
            {
                request += $"AND idstudentgroup = (select idstudentgroup from vcsdb.studentgroups where studentgroup = '{student.Group}');";
            }
            request += ";"; 
            return request;
        }
        private String StudentVisitsDbRequest(StudentVisitSearchObject studentVisit)
        {
            string request;
            var issecond = false;
            var date = studentVisit.DateTime.ToString("yyyy-MM-dd");
            var selectIdStudent = $"select idstudent from vcsdb.students where firstname = '{studentVisit.Student.FirstName}' and lastname = '{studentVisit.Student.LastName}' and pastname = '{studentVisit.Student.PastName}'";
            var selectIdSubject = $"select idsubject from vcsdb.subjects where subject = '{studentVisit.Subject}'";
            var selectIdClassroom = $"select idclassroom from vcsdb.classrooms where classroom = '{studentVisit.Classroom}'";
            var selectIdStudentGroup = $"select idstudentgroup from vcsdb.studentgroups where studentgroup = '{studentVisit.Student.Group}'";
            var selectTypeOfClass = $"select idtypeofclass from vcsdb.typeofclass where typeofclass = '{studentVisit.TypeOfClass}'";
            var selectIdClass = $"select idclass from vcsdb.shedule where ";
            request = $"SELECT * FROM vcsdb.visits " +
                $"WHERE idstudent =({selectIdStudent}) " +
                $"AND date = '{date}' ";
            
            if(studentVisit.Classroom != null)
            {
                if(issecond)
                {
                    selectIdClass += "and ";
                }
                else issecond = true;
                selectIdClass += $"idclassroom = ({selectIdClassroom}) ";
            }
            if(studentVisit.Student.Group != null)
            {
                if (issecond)
                {
                    selectIdClass += "and ";
                }
                else issecond = true;
                selectIdClass += $"idstudentgroup = ({selectIdStudentGroup}) ";
            }
            if(studentVisit.Subject != null)
            {
                if (issecond)
                {
                    selectIdClass += "and ";
                }
                else issecond = true;
                selectIdClass += $"idsubject = ({selectIdSubject}) ";
            }
            if (studentVisit.TypeOfClass != null)
            {
                if (issecond)
                {
                    selectIdClass += "and ";
                }
                else issecond = true;
                selectIdClass += $"idtypeofclass = ({selectTypeOfClass}) ";
            }
                if (studentVisit.Classroom != null || studentVisit.Student.Group != null || studentVisit.Subject != null)
            {
                request += $"and idclass = ({selectIdClass})";
            }
            
                request += ";";
            return request;
        }
        private string NewStudentVisitsDBRequest(StudentVisit studentVisit)
        {
            string request;
            var date = studentVisit.DateTime.ToString("yyyy-MM-dd");
            var selectIdStudent = $"select idstudent from vcsdb.students where firstname = '{studentVisit.FirstName}' and lastname = '{studentVisit.LastName}' and pastname = '{studentVisit.PastName}'";
            var selectIdSubject = $"select idsubject from vcsdb.subjects where subject = '{studentVisit.Subject}'";
            var selectIdClassroom = $"select idclassroom from vcsdb.classrooms where classroom = '{studentVisit.Classroom}'";
            var selectIdStudentGroup = $"select idstudentgroup from vcsdb.studentgroups where studentgroup = '{studentVisit.Group}'";
            var selectIdClass = $"select idclass from vcsdb.shedule where idsubject = ({selectIdSubject}) and idclassroom = ({selectIdClassroom}) and idstudentgroup = ({selectIdStudentGroup})";
            request = $"insert into vcsdb.visits (idstudent, date, idclass, presense) " +
                $"value(({selectIdStudent})" +
                $", '{date}'" +
                $", ({selectIdClass})" +
                $", '0');";
            //request = $"UPDATE vcsdb.visits " +
            //    $"SET firstname = '{studentVisit.FirstName}' " +
            //    $", lastname = '{studentVisit.LastName}' " +
            //    $", pastname = '{studentVisit.PastName}' " +
            //    $", date = '{date}' " +
            //    $", classroom = '{studentVisit.Classroom}' " +
            //    $", studentgroup = '{studentVisit.Group}' " +
            //    $", subject = '{studentVisit.Subject}' " +
            //    $", presense = '0' " +
            //    $"WHERE idpresense = '{studentVisit.Id}';";
            return request;
        }

        private string SetStudentVisitPresenseDBRequest(StudentVisit studentVisit)
        {
            string request;
            var presense = studentVisit.Presense ? 1 : 0;
            var date = studentVisit.DateTime.ToString("yyyy-MM-dd");
            var selectIdStudent = $"select idstudent from vcsdb.students where firstname = '{studentVisit.FirstName}' and lastname = '{studentVisit.LastName}' and pastname = '{studentVisit.PastName}'";
            var selectIdSubject = $"select idsubject from vcsdb.subjects where subject = '{studentVisit.Subject}'";
            var selectIdClassroom = $"select idclassroom from vcsdb.classrooms where classroom = '{studentVisit.Classroom}'";
            var selectIdStudentGroup = $"select idstudentgroup from vcsdb.studentgroups where studentgroup = '{studentVisit.Group}'";
            var selectIdClass = $"select idclass from vcsdb.shedule where idsubject = ({selectIdSubject}) and idclassroom = ({selectIdClassroom}) and idstudentgroup = ({selectIdStudentGroup})";

            request = $"UPDATE vcsdb.visits " +
                $"SET presense = '{presense}' " +
                $"WHERE idpresense = '{studentVisit.Id}' " +
                $"AND idclass = ({selectIdClass});";
            return request;
        }

        private string StudentByCardDbRequest(string studentCard)
        {
            string request;
            request = $"SELECT studentid FROM vcsdb.studentscards " +
                $"WHERE cardnumber = '{studentCard}';";            
            return request;
        }

        private String SubjectsDbRequest()
        {
            string request = "SELECT * FROM vcsdb.subjects ;";
            return request;
        }

        private String ClassroomsDbRequest()
        {
            string request = "SELECT * FROM vcsdb.classrooms;";
            return request;
        }
        private string GroupDbRequest()
        {
            string request = "SELECT * FROM vcsdb.studentgroups;";
            return request;
        }
        private String TypeOfClassDbRequest()
        {
            string request = "SELECT * FROM vcsdb.typeofclass;";
            return request;
        }
    }
}
