using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace FileApp
{
    class Program
    {
        static void Main(string[] args)
        {
            List<Institute> institutes = LoadData();

            var institutesWithoutPoorStudents = institutes
                .Where(inst => inst.Courses
                    .All(course => course.Groups
                        .All(group => group.Students
                            .All(student => !student.Grades.Contains(2)))))
                .ToList();

            string outputPath = Path.Combine(Path.GetTempPath(), "result.txt");

            using (StreamWriter sw = new StreamWriter(outputPath, false, new UTF8Encoding(true)))
            {
                if (institutesWithoutPoorStudents.Any())
                {
                    sw.WriteLine("Институты без двоечников:");
                    foreach (var inst in institutesWithoutPoorStudents)
                    {
                        sw.WriteLine(inst.Name);
                    }
                }
                else
                {
                    sw.WriteLine("Нет институтов без двоечников.");
                }
            }

            Console.WriteLine($"Результат записан в файл: {outputPath}");

            Console.WriteLine("\nСодержимое файла:");
            string fileContent = File.ReadAllText(outputPath, Encoding.UTF8);
            Console.WriteLine(fileContent);

            Console.ReadLine();
        }

        static List<Institute> LoadData()
        {
            return new List<Institute>
            {
                new Institute
                {
                    Name = "Институт информационных технологий",
                    Courses = new List<Course>
                    {
                        new Course
                        {
                            Number = 1,
                            Groups = new List<Group>
                            {
                                new Group
                                {
                                    Name = "Группа 1",
                                    Students = new List<Student>
                                    {
                                        new Student { LastName = "Иванов", Grades = new List<int> { 5, 5, 5 } },
                                        new Student { LastName = "Петров", Grades = new List<int> { 4, 4, 4 } }
                                    }
                                }
                            }
                        }
                    }
                }
            };
        }
    }

    class Institute { public string Name { get; set; } public List<Course> Courses { get; set; } }
    class Course { public int Number { get; set; } public List<Group> Groups { get; set; } }
    class Group { public string Name { get; set; } public List<Student> Students { get; set; } }
    class Student { public string LastName { get; set; } public List<int> Grades { get; set; } }
}