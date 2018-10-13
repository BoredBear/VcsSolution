﻿using System;
using System.Collections.Generic;
using System.Text;
using DataAdapter.Inside;
using MySql.Data.MySqlClient;

namespace DataAdapter.Outside
{
    public class MySql
    {

        // Params
        private string host = "5.228.65.163";
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
                    students.Add(new Student((int)reader["idstudent"], (string)reader["firstname"], (string)reader["lastname"], (string)reader["pastname"],(bool) reader["male"], (string)reader["studentgroup"]));
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
                    studentVisits.Add(new StudentVisit((int)reader["idpresense"], (string)reader["firstname"], (string)reader["lastname"], (string)reader["pastname"], (string)reader["studentgroup"], (DateTime)reader["date"], (string)reader["classroom"], (string)reader["subject"], (bool)reader["presense"]));
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
                request += $"AND studentgroup = '{student.Group}' ";
            }
            request += ";"; 
            return request;
        }
        private String StudentVisitsDbRequest(StudentVisitSearchObject studentVisit)
        {
            string request;
            var date = studentVisit.DateTime.ToString("yyyy-MM-dd");
            request = $"SELECT * FROM vcsdb.visits " +
                $"WHERE firstname = '{studentVisit.Student.FirstName}' " +
                $"AND lastname = '{studentVisit.Student.LastName}' " +
                $"AND date = '{date}' ";
            if(studentVisit.Classroom != null)
            {
                request += $"AND classroom = '{studentVisit.Classroom}' ";
            }
            if(studentVisit.Student.Group != null)
            {
                request += $"AND studentgroup = '{studentVisit.Student.Group}' ";
            }
            if(studentVisit.Student.PastName != null)
            {
                request += $"AND pastname = '{studentVisit.Student.PastName}' ";
            }
            if(studentVisit.Subject != null)
            {
                request += $"AND subject = '{studentVisit.Subject}' ";
            }
            request += ";";
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
    }
}
