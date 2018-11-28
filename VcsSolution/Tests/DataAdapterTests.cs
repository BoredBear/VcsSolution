using Microsoft.VisualStudio.TestTools.UnitTesting;
using DataAdapter.Outside;
using DataAdapter.Inside;
using DataAdapter.Exceptions;
using System;

namespace Tests
{
    [TestClass]
    public class DataAdapterTests
    {
        /// <summary> Тест обычного получения студента из базы </summary>
        [TestMethod]
        public void MySql_GetStudent()
        {
            var sql = new MySql();
            var result = sql.GetStudent(new StudentSearchObject("Егор", "Петров", null, true, "ИВБО-06-16"));

            Assert.IsTrue(result.Count > 0);
            Assert.AreEqual("Егор", result[0].FirstName);
        }


        /// <summary> Тест - Поле Имя содержит "-" </summary>
        [TestMethod]
        public void MySql_GetStudent_1()
        {
            var sql = new MySql();
            var result = sql.GetStudent(new StudentSearchObject("Егор-Артём", "Петров", null, true, "ИВБО-06-16"));
        }

        /// <summary> Негативный тест - группа не указана </summary>
        [TestMethod]
        public void MySql_GetStudent_Negative()
        {
            //var sql = new MySql();
            //var result = sql.GetStudent(new StudentSearchObject("Егор", "Петров", null, true, null));

            //Assert.IsTrue(result.Count > 0);
            //Assert.AreEqual("Егор", result[0].FirstName);

            bool isExeption = false;
            var sql = new MySql();
            try
            {
                var result = sql.GetStudent(new StudentSearchObject("Егор", "Петров", null, true, null));
            }
            catch (ValidationErrorException ex)
            {
                Assert.AreEqual("Группа", ex.FieldName);
                isExeption = true;
            }
            Assert.IsTrue(isExeption);
        }

        /// <summary> Негативный тест - Имя с запретными символами </summary>
        [TestMethod]
        public void MySql_GetStudent_Negative_2()
        {
            bool isExeption = false;
            try
            {
                var sql = new MySql();
                var result = sql.GetStudent(new StudentSearchObject("Ег;ор", "Петров", null, true, "ИВБО-06-16"));
            } catch(ValidationErrorException ex)
            {
                Assert.AreEqual("Имя", ex.FieldName);
                isExeption = true;
            }
            Assert.IsTrue(isExeption);
        }

        /// <summary> Негативный тест - Фамилия с запретными символами </summary>
        [TestMethod]
        public void MySql_GetStudent_Negative_3()
        {
            bool isExeption = false;
            try
            {
                var sql = new MySql();
                var result = sql.GetStudent(new StudentSearchObject("Егор", "Пе%тров", null, true, "ИВБО-06-16"));
            }
            catch (ValidationErrorException ex)
            {
                Assert.AreEqual("Фамилия", ex.FieldName);
                isExeption = true;
            }
            Assert.IsTrue(isExeption);
        }

        /// <summary> Негативный тест - Имя с запретными символами </summary>
        [TestMethod]
        public void MySql_GetStudent_Negative_4()
        {
            bool isExeption = false;
            try
            {
                var sql = new MySql();
                var result = sql.GetStudent(new StudentSearchObject("Ег/ор", "Петров", null, true, "ИВБО-06-16"));
            }
            catch (ValidationErrorException ex)
            {
                Assert.AreEqual("Имя", ex.FieldName);
                isExeption = true;
            }
            Assert.IsTrue(isExeption);
        }

        /// <summary> Негативный тест - Фамилия с запретными символами </summary>
        [TestMethod]
        public void MySql_GetStudent_Negative_6()
        {
            bool isExeption = false;
            try
            {
                var sql = new MySql();
                var result = sql.GetStudent(new StudentSearchObject("Егор", "Пе/тров", null, true, "ИВБО-06-16"));
            }
            catch (ValidationErrorException ex)
            {
                Assert.AreEqual("Фамилия", ex.FieldName);
                isExeption = true;
            }
            Assert.IsTrue(isExeption);
        }
        /// <summary> Негативный тест - Фамилия с запретными символами </summary>
        [TestMethod]
        public void MySql_GetStudent_Negative_7()
        {
            bool isExeption = false;
            try
            {
                var sql = new MySql();
                var result = sql.GetStudent(new StudentSearchObject("Егор", "Абдул,Маликов", null, true, "ИВБО-06-16"));
            }
            catch (ValidationErrorException ex)
            {
                Assert.AreEqual("Фамилия", ex.FieldName);
                isExeption = true;
            }
            Assert.IsTrue(isExeption);
        }

        /// <summary> Негативный тест - не указано имя </summary>
        [TestMethod]
        public void MySql_GetStudent_Negative_8()
        {
            bool isExeption = false;
            var sql = new MySql();
            try
            {
                var result = sql.GetStudent(new StudentSearchObject(null, "Петров", null, true, "ИВБО-06-16"));
            } catch(ValidationErrorException ex)
            {
                Assert.AreEqual("Имя", ex.FieldName);
                isExeption = true;
            }
            Assert.IsTrue(isExeption);
        }

        // Положительный тест на получение данных визита
        [TestMethod]
        public void MySql_GetStudentVisit()
        {
            var sql = new MySql();
            var result = sql.GetStudentVisits(new StudentVisitSearchObject(new Student(1, "Егор", "Петров", "Михайлович", true, "ИВБО-06-16"), new DateTime(2018, 10, 28), "А-1", "Английский язык", "Лабораторная"));
            Assert.IsTrue(result.Count > 0);
            Assert.AreEqual(10, result[0].Id);
        }

        [TestMethod]
        public void MySql_SetStudentVisit_SetFalse()
        {
            var sql = new MySql();
            var result = sql.SetStudentVisit(new StudentVisit(10, "Егор", "Петров", "Михайлович", "ИВБО-06-16", new DateTime(2018,10,28), "А-1", "Английский язык", false, "Лабораторная"));
            Assert.IsTrue(result);
        }
        [TestMethod]
        public void MySql_SetStudentVisit_SetAndGetFalse()
        {
            var sql = new MySql();
            var result = sql.SetStudentVisit(new StudentVisit(10, "Егор", "Петров", "Михайлович", "ИВБО-06-16", new DateTime(2018, 10, 28), "А-1", "Английский язык", false, "Лабораторная"));
            var result2 = sql.GetStudentVisits(new StudentVisitSearchObject(new Student(1, "Егор", "Петров", "Михайлович", true, "ИВБО-06-16"), new DateTime(2018, 10, 28), "А-1", "Английский язык", "Лабораторная"));
            Assert.AreEqual(result2[0].Presense, false);
        }
        [TestMethod]
        public void MySql_SetStudentVisit_SetAndGetTrue()
        {
            var sql = new MySql();
            var result = sql.SetStudentVisit(new StudentVisit(10, "Егор", "Петров", "Михайлович", "ИВБО-06-16", new DateTime(2018, 10, 28), "А-1", "Английский язык", true, "Лабораторная"));
            var result2 = sql.GetStudentVisits(new StudentVisitSearchObject(new Student(1, "Егор", "Петров", "Михайлович", true, "ИВБО-06-16"), new DateTime(2018, 10, 28), "А-1", "Английский язык", "Лабораторная"));
            Assert.AreEqual(result2[0].Presense, true);
        }

        /// <summary> Негативный тест - не указана фамилия </summary>
        [TestMethod]
        public void MySql_GetStudent_LastNameIsNull()
        {
            bool isExeption = false;
            var sql = new MySql();
            try
            {
                var result = sql.GetStudent(new StudentSearchObject("Егор", null, null, true, "ИВБО-06-16"));
            }
            catch (ValidationErrorException ex)
            {
                Assert.AreEqual("Фамилия", ex.FieldName);
                isExeption = true;
            }
            Assert.IsTrue(isExeption);
        }

        [TestMethod]
        public void MySql_GetSubject()
        {
            var sql = new MySql();
            var result = sql.GetSubjects();

            Assert.IsTrue(result.Count > 0);
            Assert.AreEqual("Алгебра и геометрия", result[0]);
        }
        [TestMethod]
        public void mySql_GetClassrooms()
        {
            var sql = new MySql();
            var result = sql.GetClassrooms();

            Assert.IsTrue(result.Count > 0);
            Assert.AreEqual("А-1", result[0]);
        }

        [TestMethod]
        public void MySql_GetStudentGroups()
        {
            var sql = new MySql();
            var result = sql.GetStudentGroups();

            Assert.IsTrue(result.Count > 0);
            Assert.AreEqual("ИВБО-01-16", result[0]);
        }

        //Негативный тест - неправильно введено какое-то значение 
        [TestMethod]
        public void MySql_GetStudentVisit_WrongFirstName()
        {
            var sql = new MySql();
            var result = sql.GetStudentVisits(new StudentVisitSearchObject(new Student(1, "ЕЙор", "Петров", "Михайлович", true, "ИВБО-06-16"), new DateTime(2018, 10, 28), "А-1", "Английский язык", "Лабораторная"));
            Assert.IsTrue(result.Count < 1);
        }

        //Негативный тест - имя содержит цифры
        [TestMethod]
        public void MySql_SetStudentVisit_NumbersInTheFirstName()
        {
            bool isExseption = false;
            var sql = new MySql();
            try
            {
                var result = sql.SetStudentVisit(new StudentVisit(10, "Ег3ор", "Петров", "Михайлович", "ИВБО-06-16", new DateTime(2018, 10, 28), "А-1", "Английский язык", true, "Лабораторная"));
            }
            catch(ValidationErrorException ex)
            {
                Assert.AreEqual("Имя", ex.FieldName);
                isExseption = true;
            }
            Assert.IsTrue(isExseption);
        }

        //Негативный тест - имя содержит символы
        [TestMethod]
        public void MySql_SetStudentVisit_FirstNameValidationError()
        {
            bool isException = false;
            var sql = new MySql();
            try
            {
                var result = sql.SetStudentVisit(new StudentVisit(10, "Ег%ор", "Петров", "Михайлович", "ИВБО-06-16", new DateTime(2018, 10, 28), "А-1", "Английский язык", true, "Лабораторная"));
            }
            catch(ValidationErrorException ex)
            {
                Assert.AreEqual("Имя", ex.FieldName);
                isException = true;
            }
            Assert.IsTrue(isException);
        }
        //Негативный тест - неправильно написана группа
        [TestMethod]
        public void MySql_SetStudentVisit_WrongGroup()
        {
            bool isExseption = false;
            var sql = new MySql();
            try
            {
                var result = sql.SetStudentVisit(new StudentVisit(10, "Егор", "Петров", "Михайлович", "ИВБО-04-16", new DateTime(2018, 10, 28), "А-1", "Английский язык", true, "Лабораторная"));

            } 
            catch (ValidationErrorException ex)
            {
                Assert.AreEqual("Группа", ex.FieldName);
                isExseption = true;
            }
            Assert.IsTrue(isExseption);
        }
    }
}


