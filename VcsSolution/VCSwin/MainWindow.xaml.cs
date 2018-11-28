using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using DataAdapter.Inside;
using DataAdapter.Exceptions;
using DataAdapter.Outside;
using DataAdapter;

namespace VCSwin
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        StudentInfoPage studentInfoPage;
        EditVisitPage editVisitPage;
        Student Student;
        List<StudentVisit> studentVisits;
        int visitId;

        public MainWindow()
        {
            InitializeComponent();
            MySql mySql1 = new MySql();
            var clasrooms = mySql1.GetClassrooms();
            var subjects = mySql1.GetSubjects();
            var types = mySql1.GetTypeOfClass();
            foreach(var c in clasrooms)
            {
                cmbClassroom.Items.Add(c);
            }
            foreach(var s in subjects)
            {
                cmbSubject.Items.Add(s);
            }
            foreach(var t in types)
            {
                cmbType.Items.Add(t);
            }
        }

        private void PickStudentClickEvent(object sender, RoutedEventArgs e)
        {
            studentInfoPage = new StudentInfoPage(StudentPicked);
            studentInfoPage.Show();
        }

        private void LoadInfoClickEvent(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Student == null)
                {
                    throw new ValidationErrorException("Студент", "Выберите студента!");
                }
                if(cmbClassroom.Text.Length < 1)
                {
                    throw new ValidationErrorException("Аудитория", "Выберите аудиторию!");
                }
                if (cmbSubject.Text.Length < 1)
                {
                    throw new ValidationErrorException("Предмет", "Выберите предмет!");
                }
                DataValidator.ValidateDateTime(dpDate.SelectedDate, "Дата");

                var sql = new MySql();
                studentVisits = sql.GetStudentVisits(new StudentVisitSearchObject(
                    Student,
                    (DateTime)dpDate.SelectedDate,
                    cmbClassroom.Text,
                    cmbSubject.Text,
                    cmbType.Text));
                grdPresenseInfoTable.ItemsSource = studentVisits;
            } catch (NullReferenceException)
            {
                MessageBox.Show($"Не заполнено одно из обязательных полей!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            } catch (ValidationErrorException ex)
            {
                MessageBox.Show($"Ошибка валидации данных! Поле '{ex.FieldName}' {ex.ErrorMessage}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            MySql mysql = new MySql();
            mysql.GetStudentVisits(new StudentVisitSearchObject(new Student(1, "Егор", "Петров", "Михайлович", true, "ИВБО-06-16"), new DateTime(2018, 10, 05, 18, 00, 00), "А5", "Архитектура вычислительных машин и систем", "Лекция"));
        }

        private void StudentPicked(Student student)
        {
            // > Отобразить студента
            tbStudent.Text = $"{student.FirstName} {student.LastName} {student.PastName} | {student.Group}";
            Student = student;
        }

        private void ChangeVisitState(object sender, MouseButtonEventArgs e)
        {
            visitId = grdPresenseInfoTable.SelectedIndex;
            editVisitPage = new EditVisitPage(studentVisits[visitId], ChangedVisitInfo);
            editVisitPage.Show();
        }

        public void ChangedVisitInfo(StudentVisit studentVisit)
        {
            MySql mySql = new MySql();
            var result = mySql.SetStudentVisit(studentVisit);
            if(!result)
            {
                MessageBox.Show("Ошибка при изменении значения!", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            studentVisits[visitId] = studentVisit;
            grdPresenseInfoTable.ItemsSource = null;
            grdPresenseInfoTable.ItemsSource = studentVisits;         
        }

        /// <summary>
        /// Метод для автотестов
        /// </summary>
        /// <param name="studentSearch">Студент</param>
        /// <param name="date">Дата занятия</param>
        /// <param name="classroom">Аудитория</param>
        /// <param name="subject">Предмет</param>
        /// <returns></returns>
        public MainWindow FillPage(StudentSearchObject studentSearch, DateTime date, string classroom, string subject)
        {
            PickStudentClickEvent(null, null);
            studentInfoPage.FillPage(studentSearch).PickFirstStudent();
            dpDate.SelectedDate = date;
            cmbClassroom.SelectedIndex = cmbClassroom.Items.IndexOf(classroom);
            cmbSubject.SelectedIndex = cmbSubject.Items.IndexOf(subject);
            return this;
        }

        /// <summary>
        /// Метод для автотестов
        /// </summary>
        /// <returns></returns>
        public MainWindow ClickSearch()
        {
            LoadInfoClickEvent(null, null);
            return this;
        }
    }
}
