using System;
using System.Collections.Generic;
using System.IO;

namespace edu_simple
{
    // модели
    public class Student
    {
        public string Id;
        public string FullName;
        public Dictionary<string, int> Marks = new Dictionary<string, int>(); // subject -> mark (2..5)

        public Student(string id, string fullName) { Id = id; FullName = fullName; }

        public bool IsExcellent()
        {
            if (Marks.Count == 0) return false;
            foreach (var kv in Marks) if (kv.Value != 5) return false;
            return true;
        }

        public override string ToString() => $"{FullName} (ID:{Id}), отличник: {IsExcellent()}";
    }

    public class Group
    {
        public string Name;
        public List<Student> Students = new List<Student>();
        public Group(string name) { Name = name; }
        public override string ToString() => $"Группа {Name}, студентов: {Students.Count}";
    }

    public class Course
    {
        public int Number; // 1..6
        public List<Group> Groups = new List<Group>();
        public Course(int number) { Number = number; }
        public override string ToString() => $"Курс {Number}, групп: {Groups.Count}";
    }

    public class Institute
    {
        public string Name;
        public List<string> Subjects = new List<string>(); // просто имена предметов
        public List<Course> Courses = new List<Course>();
        public Institute(string name) { Name = name; }

        public int CountExcellent()
        {
            int c = 0;
            foreach (var crs in Courses)
                foreach (var g in crs.Groups)
                    foreach (var st in g.Students)
                        if (st.IsExcellent()) c++;
            return c;
        }

        public override string ToString() => $"Институт: {Name}, курсов: {Courses.Count}, предметов: {Subjects.Count}, отличников: {CountExcellent()}";
    }

    class Program
    {
        static List<Institute> institutes = new List<Institute>();
        static int autoId = 1; // простая авто-нумерация студентов

        static void Main()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Seed();

            while (true)
            {
                Menu();
                Console.Write("Выбор: ");
                var cmd = Console.ReadLine();
                Console.WriteLine();

                if (cmd == "0") break;
                switch (cmd)
                {
                    case "1": AddInstitute(); break;
                    case "2": RenameInstitute(); break;
                    case "3": DeleteInstitute(); break;
                    case "4": AddSubject(); break;
                    case "5": AddCourseAndGroup(); break;
                    case "6": AddStudent(); break;
                    case "7": PutMark(); break;
                    case "8": ListAll(); break;
                    case "9": QueryAndSave(); break;
                    default: Console.WriteLine("Нет такого пункта.\n"); break;
                }
            }
        }

        static void Menu()
        {
            Console.WriteLine("=== МЕНЮ ===");
            Console.WriteLine("1) Добавить институт");
            Console.WriteLine("2) Переименовать институт");
            Console.WriteLine("3) Удалить институт");
            Console.WriteLine("4) Добавить предмет в институт");
            Console.WriteLine("5) Добавить курс и/или группу");
            Console.WriteLine("6) Добавить студента в группу");
            Console.WriteLine("7) Поставить/изменить оценку студенту");
            Console.WriteLine("8) Показать все данные");
            Console.WriteLine("9) Запрос: институт с наибольшим числом отличников (в файл)");
            Console.WriteLine("0) Выход\n");
        }

        // Выборы
        static Institute PickInstitute()


        {
            if (institutes.Count == 0) { Console.WriteLine("Институтов нет.\n"); return null; }
            for (int i = 0; i < institutes.Count; i++) Console.WriteLine($"{i + 1}. {institutes[i].Name}");
            Console.Write("Номер института: ");
            if (int.TryParse(Console.ReadLine(), out int iidx) && iidx >= 1 && iidx <= institutes.Count) return institutes[iidx - 1];
            Console.WriteLine("Неверно.\n"); return null;
        }

        static Course PickCourse(Institute inst)
        {
            if (inst.Courses.Count == 0) { Console.WriteLine("Курсов нет.\n"); return null; }
            for (int i = 0; i < inst.Courses.Count; i++) Console.WriteLine($"{i + 1}. Курс {inst.Courses[i].Number}");
            Console.Write("Номер курса: ");
            if (int.TryParse(Console.ReadLine(), out int iidx) && iidx >= 1 && iidx <= inst.Courses.Count) return inst.Courses[iidx - 1];
            Console.WriteLine("Неверно.\n"); return null;
        }

        static Group PickGroup(Course course)
        {
            if (course.Groups.Count == 0) { Console.WriteLine("Групп нет.\n"); return null; }
            for (int i = 0; i < course.Groups.Count; i++) Console.WriteLine($"{i + 1}. {course.Groups[i].Name}");
            Console.Write("Номер группы: ");
            if (int.TryParse(Console.ReadLine(), out int iidx) && iidx >= 1 && iidx <= course.Groups.Count) return course.Groups[iidx - 1];
            Console.WriteLine("Неверно.\n"); return null;
        }

        static Student PickStudent(Group group)
        {
            if (group.Students.Count == 0) { Console.WriteLine("Студентов нет.\n"); return null; }
            for (int i = 0; i < group.Students.Count; i++) Console.WriteLine($"{i + 1}. {group.Students[i]}");
            Console.Write("Номер студента: ");
            if (int.TryParse(Console.ReadLine(), out int iidx) && iidx >= 1 && iidx <= group.Students.Count) return group.Students[iidx - 1];
            Console.WriteLine("Неверно.\n"); return null;
        }

        // CRUD
        static void AddInstitute()
        {
            Console.Write("Название: ");
            var name = Console.ReadLine()?.Trim();
            if (string.IsNullOrWhiteSpace(name)) { Console.WriteLine("Пусто.\n"); return; }
            institutes.Add(new Institute(name));
            Console.WriteLine("Добавлено.\n");
        }

        static void RenameInstitute()
        {
            var inst = PickInstitute(); if (inst == null) return;
            Console.Write("Новое название: ");
            var name = Console.ReadLine()?.Trim();
            if (string.IsNullOrWhiteSpace(name)) { Console.WriteLine("Пусто.\n"); return; }
            inst.Name = name; Console.WriteLine("Ок.\n");
        }

        static void DeleteInstitute()
        {
            var inst = PickInstitute(); if (inst == null) return;
            institutes.Remove(inst); Console.WriteLine("Удалено.\n");
        }

        static void AddSubject()
        {
            var inst = PickInstitute(); if (inst == null) return;
            Console.Write("Название предмета: ");
            var s = Console.ReadLine()?.Trim();
            if (string.IsNullOrWhiteSpace(s)) { Console.WriteLine("Пусто.\n"); return; }
            if (!inst.Subjects.Contains(s)) inst.Subjects.Add(s);
            Console.WriteLine("Добавлено.\n");
        }

        static void AddCourseAndGroup()
        {
            var inst = PickInstitute(); if (inst == null) return;

            Console.Write("Номер курса (1..6): ");
            if (!int.TryParse(Console.ReadLine(), out int num) || num < 1 || num > 6) { Console.WriteLine("Неверно.\n"); return; }

            var course = inst.Courses.Find(c => c.Number == num);
            if (course == null) { course = new Course(num); inst.Courses.Add(course); Console.WriteLine("Курс создан."); }

            Console.Write("Название группы: ");
            var


gname = Console.ReadLine()?.Trim();
            if (string.IsNullOrWhiteSpace(gname)) { Console.WriteLine("Пусто.\n"); return; }
            if (course.Groups.Exists(g => string.Equals(g.Name, gname, StringComparison.OrdinalIgnoreCase)))
            { Console.WriteLine("Уже есть.\n"); return; }

            course.Groups.Add(new Group(gname));
            Console.WriteLine("Группа добавлена.\n");
        }

        static void AddStudent()
        {
            var inst = PickInstitute(); if (inst == null) return;
            var course = PickCourse(inst); if (course == null) return;
            var group = PickGroup(course); if (group == null) return;

            var id = $"S{autoId++:000}";
            Console.Write("ФИО: ");
            var fio = Console.ReadLine()?.Trim();
            if (string.IsNullOrWhiteSpace(fio)) { Console.WriteLine("Пусто.\n"); return; }

            group.Students.Add(new Student(id, fio));
            Console.WriteLine($"Добавлен студент {fio} с ID {id}.\n");
        }

        static void PutMark()
        {
            var inst = PickInstitute(); if (inst == null) return;
            if (inst.Subjects.Count == 0) { Console.WriteLine("Нет предметов.\n"); return; }
            var course = PickCourse(inst); if (course == null) return;
            var group = PickGroup(course); if (group == null) return;
            var st = PickStudent(group); if (st == null) return;

            for (int i = 0; i < inst.Subjects.Count; i++) Console.WriteLine($"{i + 1}. {inst.Subjects[i]}");
            Console.Write("Номер предмета: ");
            if (!int.TryParse(Console.ReadLine(), out int sidx) || sidx < 1 || sidx > inst.Subjects.Count) { Console.WriteLine("Неверно.\n"); return; }
            var subj = inst.Subjects[sidx - 1];

            Console.Write("Оценка (2..5): ");
            if (!int.TryParse(Console.ReadLine(), out int m) || m < 2 || m > 5) { Console.WriteLine("Неверно.\n"); return; }

            st.Marks[subj] = m;
            Console.WriteLine("Сохранено.\n");
        }

        // Вывод и запрос
        static void ListAll()
        {
            if (institutes.Count == 0) { Console.WriteLine("Данных нет.\n"); return; }

            foreach (var inst in institutes)
            {
                Console.WriteLine(inst);
                if (inst.Subjects.Count > 0)
                {
                    Console.Write("  Предметы: ");
                    for (int i = 0; i < inst.Subjects.Count; i++)
                    {
                        Console.Write(inst.Subjects[i]);
                        if (i < inst.Subjects.Count - 1) Console.Write(", ");
                    }
                    Console.WriteLine();
                }
                foreach (var c in inst.Courses)
                {
                    Console.WriteLine($"  {c}");
                    foreach (var g in c.Groups)
                    {
                        Console.WriteLine($"    {g}");
                        foreach (var s in g.Students)
                        {
                            Console.WriteLine($"      {s}");
                            if (s.Marks.Count > 0)
                            {
                                Console.Write("        Оценки: ");
                                int k = 0; foreach (var kv in s.Marks)
                                {
                                    Console.Write($"{kv.Key}:{kv.Value}");
                                    if (++k < s.Marks.Count) Console.Write(", ");
                                }
                                Console.WriteLine();
                            }
                        }
                    }
                }
                Console.WriteLine();
            }
        }

        static void QueryAndSave()
        {
            if (institutes.Count == 0) { Console.WriteLine("Нет данных.\n"); return; }

            Institute best = null; int bestCnt = -1;
            foreach (var inst in institutes)
            {
                int c = inst.CountExcellent();
                if (c > bestCnt) { bestCnt = c; best = inst; }
            }

            string text = (best == null)
                ? "Нет данных."
                : $"Институт с наибольшим числом отличников: {best.Name}. Количество отличников: {bestCnt}.";

            Console.WriteLine(text + "\n");
            try { File.WriteAllText("result.txt", text); Console.WriteLine("Сохранено в result.txt\n"); }
            catch (Exception ex) { Console.WriteLine("Ошибка записи файла: " + ex.Message + "\n"); }
        }

        // начальное наполнеие 
        static void Seed()
        {
            var i1 = new Institute("ИТ-институт");
            i1.Subjects.AddRange(new[] { "Программирование", "Математика" });
            var c1 = new Course(1);
            var g1 = new Group("ФИ22");
            var s1 = new Student("S001", "Иванов Иван");
            s1.Marks["Программирование"] = 5;
            s1.Marks["Математика"] = 5;
            var s2 = new Student("S002", "Кай Шик");
            s2.Marks["Программирование"] = 4;
            s2.Marks["Математика"] = 5;
            g1.Students.Add(s1); g1.Students.Add(s2);
            c1.Groups.Add(g1); i1.Courses.Add(c1);
            institutes.Add(i1);

            var i2 = new Institute("Инженерный институт");
            i2.Subjects.Add("Физика");
            var c2 = new Course(1);
            var g2 = new Group("ФИ21");
            var s3 = new Student("S003", "Анова Анна");
            s3.Marks["Физика"] = 5;
            g2.Students.Add(s3); c2.Groups.Add(g2); i2.Courses.Add(c2);
            institutes.Add(i2);

            autoId = 4;
        }
    }
}

