using System;
using System.Collections.Generic;
using System.IO;

namespace edu_simple
{
    // ==================== ИНТЕРФЕЙСЫ ====================
    public interface IHasId
    {
        string GetId();
    }

    public interface IPrintable
    {
        void Print();
    }

    public interface IMarkEditable
    {
        void PutMark(string subject, int mark);
        void ClearMarks();
    }

    // - интерфейс для работы с финансами
    public interface IFinancialOperations
    {
        decimal Balance { get; }
        void Deposit(decimal amount);
        void Withdraw(decimal amount);
    }

    // НОВЫЙ ИНТЕРФЕЙС для варианта 16 - интерфейс для уведомлений
    public interface INotifiable
    {
        string Email { get; set; }
        void SendNotification(string message);
    }

    // ==================== СИСТЕМА ОБРАБОТКИ ОШИБОК ====================

    // Аргументы события про ошибку
    public class ErrorEventArgs : EventArgs
    {
        public string Kind;   // тип ошибки (кратко)
        public string Text;   // подробности (ex.Message)
        public DateTime TimeOccurred { get; }

        public ErrorEventArgs(string kind, string text)
        {
            Kind = kind;
            Text = text;
            TimeOccurred = DateTime.Now;
        }
    }

    // Базовый класс, у которого есть виртуальное событие про ошибку
    public class ErrorNotifierBase
    {
        // делегат стандартный: EventHandler<T>
        public virtual event EventHandler<ErrorEventArgs> ErrorHappened;

        // защищённый метод - вызвать событие
        protected virtual void OnError(string kind, string text)
        {
            var handler = ErrorHappened;
            if (handler != null)
                handler(this, new ErrorEventArgs(kind, text));
        }

        // Публичный помощник
        public void RaiseError(string kind, string text)
        {
            OnError(kind, text);
        }
    }

    // Производный класс переопределяет событие (через override add/remove)
    public class ErrorNotifierDerived : ErrorNotifierBase
    {
        private EventHandler<ErrorEventArgs> _backing; // внутр. список подписчиков

        public override event EventHandler<ErrorEventArgs> ErrorHappened
        {
            add { _backing += value; }
            remove { _backing -= value; }
        }

        protected override void OnError(string kind, string text)
        {
            // добавим префикс, чтобы было видно, что событие "переопределено"
            var handler = _backing;
            if (handler != null)
            {
                var msg = $"[DerivedEvent {DateTime.Now:HH:mm:ss}] {text}";
                handler(this, new ErrorEventArgs(kind, msg));
            }
        }
    }

    // ==================== МОДЕЛИ ====================
    public class Student : IHasId, IPrintable, IMarkEditable, IFinancialOperations, INotifiable
    {
        public string Id;
        public string FullName;
        public Dictionary<string, int> Marks = new Dictionary<string, int>();
        private decimal _balance;
        public string Email { get; set; }

        // Событие для уведомления об ошибках операций со студентом
        public event EventHandler<ErrorEventArgs> StudentOperationError;

        public Student(string id, string fullName)
        {
            Id = id;
            FullName = fullName;
            _balance = 0;
            Email = "";
        }

        public bool IsExcellent()
        {
            try
            {
                if (Marks.Count == 0) return false;
                foreach (var kv in Marks)
                    if (kv.Value != 5)
                        return false;
                return true;
            }
            catch (Exception ex)
            {
                StudentOperationError?.Invoke(this, new ErrorEventArgs("IsExcellentError", ex.Message));
                return false;
            }
        }

        public override string ToString() => $"{FullName} (ID:{Id}), отличник: {IsExcellent()}, баланс: {_balance:C}";

        // --- Реализация интерфейсов ---
        public string GetId() { return Id; }

        public void Print()
        {
            Console.WriteLine(ToString());
        }

        public void PutMark(string subject, int mark)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(subject))
                    throw new ArgumentException("Название предмета не может быть пустым");

                if (mark < 2 || mark > 5)
                    throw new ArgumentOutOfRangeException("Оценка должна быть от 2 до 5");

                Marks[subject] = mark;
            }
            catch (Exception ex)
            {
                StudentOperationError?.Invoke(this, new ErrorEventArgs("PutMarkError", ex.Message));
            }
        }

        public void ClearMarks()
        {
            Marks.Clear();
        }

        // Реализация IFinancialOperations
        public decimal Balance => _balance;

        public void Deposit(decimal amount)
        {
            try
            {
                if (amount <= 0)
                    throw new ArgumentException("Сумма пополнения должна быть положительной");

                _balance += amount;
                Console.WriteLine($"Пополнение: +{amount:C}. Новый баланс: {_balance:C}");
            }
            catch (Exception ex)
            {
                StudentOperationError?.Invoke(this, new ErrorEventArgs("DepositError", ex.Message));
            }
        }

        public void Withdraw(decimal amount)
        {
            try
            {
                if (amount <= 0)
                    throw new ArgumentException("Сумма снятия должна быть положительной");

                if (amount > _balance)
                    throw new InvalidOperationException("Недостаточно средств на счете");

                _balance -= amount;
                Console.WriteLine($"Снятие: -{amount:C}. Новый баланс: {_balance:C}");
            }
            catch (Exception ex)
            {
                StudentOperationError?.Invoke(this, new ErrorEventArgs("WithdrawError", ex.Message));
            }
        }

        // Реализация INotifiable
        public void SendNotification(string message)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(message))
                    throw new ArgumentException("Сообщение не может быть пустым");

                if (!string.IsNullOrEmpty(Email))
                {
                    Console.WriteLine($"Уведомление для {FullName} ({Email}): {message}");
                }
                else
                {
                    Console.WriteLine($"Уведомление для {FullName} (email не указан): {message}");
                }
            }
            catch (Exception ex)
            {
                StudentOperationError?.Invoke(this, new ErrorEventArgs("SendNotificationError", ex.Message));
            }
        }
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
        public int Number;
        public List<Group> Groups = new List<Group>();
        public Course(int number) { Number = number; }
        public override string ToString() => $"Курс {Number}, групп: {Groups.Count}";
    }

    public class Institute
    {
        public string Name;
        public List<string> Subjects = new List<string>();
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

    // ==================== ДЕМОНСТРАЦИЯ ИСКЛЮЧЕНИЙ ====================
    public class ExceptionDemo
    {
        private readonly ErrorNotifierBase _notifier;

        public ExceptionDemo(ErrorNotifierBase notifier)
        {
            _notifier = notifier;
        }

        public void Demo_DivideByZero()
        {
            try
            {
                int a = 1;
                int b = 0;
                int c = a / b; // DivideByZeroException
            }
            catch (DivideByZeroException ex)
            {
                _notifier.RaiseError("DivideByZeroException", ex.Message);
            }
        }

        public void Demo_IndexOutOfRange()
        {
            try
            {
                int[] arr = new int[2];
                int x = arr[5]; // IndexOutOfRangeException
            }
            catch (IndexOutOfRangeException ex)
            {
                _notifier.RaiseError("IndexOutOfRangeException", ex.Message);
            }
        }

        public void Demo_ArrayTypeMismatch()
        {
            try
            {
                string[] sarr = new string[2];
                object[] oarr = sarr;
                oarr[0] = 123; // ArrayTypeMismatchException
            }
            catch (ArrayTypeMismatchException ex)
            {
                _notifier.RaiseError("ArrayTypeMismatchException", ex.Message);
            }
        }

        public void Demo_InvalidCast()
        {
            try
            {
                object obj = "hello";
                Student st = (Student)obj; // InvalidCastException
            }
            catch (InvalidCastException ex)
            {
                _notifier.RaiseError("InvalidCastException", ex.Message);
            }
        }

        public void Demo_Overflow()
        {
            try
            {
                checked
                {
                    int big = int.MaxValue;
                    big = big + 1; // OverflowException
                }
            }
            catch (OverflowException ex)
            {
                _notifier.RaiseError("OverflowException", ex.Message);
            }
        }

        public void Demo_OutOfMemory()
        {
            try
            {
                // Симуляция для демонстрации
                throw new OutOfMemoryException("Симулированная нехватка памяти");
            }
            catch (OutOfMemoryException ex)
            {
                _notifier.RaiseError("OutOfMemoryException", ex.Message);
            }
        }

        public void Demo_StackOverflow()
        {
            try
            {
                // Симуляция для демонстрации
                throw new StackOverflowException("Симулированное переполнение стека");
            }
            catch (StackOverflowException ex)
            {
                _notifier.RaiseError("StackOverflowException", ex.Message);
            }
        }
    }

    // ==================== ОСНОВНАЯ ПРОГРАММА ====================
    class Program
    {
        static List<Institute> institutes = new List<Institute>();
        static int autoId = 1;

        // Многоадресный делегат для тестирования всех методов студента
        delegate void StudentAction(Student s);

        // Общий объект-нотификатор для обработки исключений
        static ErrorNotifierDerived notifier = new ErrorNotifierDerived();

        static void Main()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            // Подписка на событие об ошибке
            notifier.ErrorHappened += (sender, e) =>
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[ОШИБКА] Время: {e.TimeOccurred:HH:mm:ss}");
                Console.WriteLine($"Тип: {e.Kind}");
                Console.WriteLine($"Сообщение: {e.Text}\n");
                Console.ResetColor();

                // Логирование в файл
                try
                {
                    File.AppendAllText("errors.log",
                        $"{e.TimeOccurred:yyyy-MM-dd HH:mm:ss} | {e.Kind} | {e.Text}\n");
                }
                catch
                {
                    // Игнорируем ошибки записи в лог
                }
            };

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
                    case "10": TestStudentInterfacesWithDelegate(); break;
                    case "11": TestFinancialOperations(); break;
                    case "12": DemoExceptions(); break; // НОВЫЙ ПУНКТ - демонстрация исключений
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
            Console.WriteLine("10) Тест методов студента (интерфейсы + многоадресный делегат)");
            Console.WriteLine("11) Тест финансовых операций и уведомлений");
            Console.WriteLine("12) Демонстрация обработки исключений (7 типов)"); // НОВЫЙ ПУНКТ
            Console.WriteLine("0) Выход\n");
        }

        // НОВЫЙ МЕТОД - демонстрация всех 7 исключений
        static void DemoExceptions()
        {
            Console.WriteLine("=== Демонстрация обработки исключений через событие (наследование) ===");
            var demo = new ExceptionDemo(notifier);

            demo.Demo_StackOverflow();      // 1) StackOverflowException
            demo.Demo_ArrayTypeMismatch();  // 2) ArrayTypeMismatchException
            demo.Demo_DivideByZero();       // 3) DivideByZeroException
            demo.Demo_IndexOutOfRange();    // 4) IndexOutOfRangeException
            demo.Demo_InvalidCast();        // 5) InvalidCastException
            demo.Demo_OutOfMemory();        // 6) OutOfMemoryException
            demo.Demo_Overflow();           // 7) OverflowException

            Console.WriteLine("=== Демонстрация завершена ===\n");
        }

        // Методы выбора
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

        // CRUD методы
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
            var gname = Console.ReadLine()?.Trim();
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

            var student = new Student(id, fio);

            // Подписываемся на события ошибок студента
            student.StudentOperationError += (sender, e) =>
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"[ОШИБКА СТУДЕНТА] {student.FullName}: {e.Kind} - {e.Text}");
                Console.ResetColor();
            };

            Console.Write("Email студента: ");
            student.Email = Console.ReadLine()?.Trim();

            group.Students.Add(student);
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

            st.PutMark(subj, m);
            Console.WriteLine("Сохранено.\n");
        }

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

        // Тест всех интерфейсов через многоадресный делегат
        static void TestStudentInterfacesWithDelegate()
        {
            var inst = PickInstitute(); if (inst == null) return;
            var course = PickCourse(inst); if (course == null) return;
            var group = PickGroup(course); if (group == null) return;
            var st = PickStudent(group); if (st == null) return;

            StudentAction tests = null;

            // IHasId
            tests += s => Console.WriteLine("ID студента: " + s.GetId());

            // IPrintable
            tests += s => { Console.Write("Информация о студенте: "); s.Print(); };

            // IMarkEditable
            tests += s => {
                s.PutMark("Тестовый предмет", 5);
                Console.WriteLine("Добавлена оценка по 'Тестовый предмет': 5");
            };
            tests += s => { Console.Write("После добавления оценки: "); s.Print(); };
            tests += s => { s.ClearMarks(); Console.WriteLine("Оценки очищены"); };
            tests += s => { Console.Write("После очистки оценок: "); s.Print(); };

            // IFinancialOperations
            tests += s => s.Deposit(1000);
            tests += s => s.Withdraw(300);
            tests += s => { Console.Write("После финансовых операций: "); s.Print(); };

            // INotifiable
            tests += s => s.SendNotification("Тестовое уведомление о успеваемости");

            Console.WriteLine("=== Запуск комплексного теста всех интерфейсов ===");
            tests(st);
            Console.WriteLine("=== Тесты завершены ===\n");
        }

        // Тест финансовых операций и уведомлений
        static void TestFinancialOperations()
        {
            var inst = PickInstitute(); if (inst == null) return;
            var course = PickCourse(inst); if (course == null) return;
            var group = PickGroup(course); if (group == null) return;
            var st = PickStudent(group); if (st == null) return;

            Console.WriteLine("=== Тест финансовых операций и уведомлений ===");

            // Тестируем IFinancialOperations
            Console.WriteLine($"Текущий баланс: {st.Balance:C}");
            st.Deposit(1500);
            st.Withdraw(500);
            st.Withdraw(1200); // попытка снять больше чем есть

            // Тестируем INotifiable
            st.SendNotification("Ваш баланс был изменен");

            // Комбинированная операция
            if (st.Balance > 0)
            {
                st.SendNotification($"На вашем счету осталось {st.Balance:C}");
            }

            Console.WriteLine("=== Тест завершен ===\n");
        }

        // начальное наполнение 
        static void Seed()
        {
            var i1 = new Institute("ИТ-институт");
            i1.Subjects.AddRange(new[] { "Программирование", "Математика" });
            var c1 = new Course(1);
            var g1 = new Group("ФИ22");
            var s1 = new Student("S001", "Козлоу Максон");
            s1.Marks["Программирование"] = 5;
            s1.Marks["Математика"] = 5;
            s1.Email = "kozvovskiy@edu.ru";
            var s2 = new Student("S002", "Мармок");
            s2.Marks["Программирование"] = 4;
            s2.Marks["Математика"] = 5;
            s2.Email = "marmok@edu.ru";
            g1.Students.Add(s1); g1.Students.Add(s2);
            c1.Groups.Add(g1); i1.Courses.Add(c1);
            institutes.Add(i1);

            var i2 = new Institute("Инженерный институт");
            i2.Subjects.Add("Физика");
            var c2 = new Course(1);
            var g2 = new Group("ФИ21");
            var s3 = new Student("S003", "Армяне Рекордс");
            s3.Marks["Физика"] = 5;
            s3.Email = "arm9ne@edu.ru";
            g2.Students.Add(s3); c2.Groups.Add(g2); i2.Courses.Add(c2);
            institutes.Add(i2);

            autoId = 4;
        }
    }
}