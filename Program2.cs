using System;
using System.Collections.Generic;

namespace Lab2Variant16
{
    // Интерфейс 1: Может издавать звуки
    public interface ISoundMaker
    {
        void MakeSound();
    }

    // Интерфейс 2: Может двигаться
    public interface IMovable
    {
        void Move();
    }

    // Интерфейс 3: Имеет среду обитания
    public interface IHabitat
    {
        string GetHabitat();
    }

    // Абстрактный базовый класс Животное
    public abstract class Animal : ISoundMaker, IMovable, IHabitat
    {
        public string Name { get; set; }
        public double Weight { get; set; }

        protected Animal(string name, double weight)
        {
            Name = name;
            Weight = weight;
        }

        // Абстрактные методы (должны быть реализованы в производных классах)
        public abstract void MakeSound();
        public abstract void Move();
        public abstract string GetHabitat();

        // Виртуальный метод (может быть переопределен)
        public virtual void PrintInfo()
        {
            Console.WriteLine($"Животное: {Name}, Вес: {Weight} кг");
        }
    }

    // Абстрактный класс Млекопитающее
    public abstract class Mammal : Animal
    {
        public int GestationPeriod { get; set; }

        protected Mammal(string name, double weight, int gestationPeriod)
            : base(name, weight)
        {
            GestationPeriod = gestationPeriod;
        }

        public void FeedMilk()
        {
            Console.WriteLine($"{Name} кормит детенышей молоком");
        }

        public override void PrintInfo()
        {
            base.PrintInfo();
            Console.WriteLine($"Период беременности: {GestationPeriod} дней");
        }
    }

    // Абстрактный класс Птица
    public abstract class Bird : Animal
    {
        public double Wingspan { get; set; }

        protected Bird(string name, double weight, double wingspan)
            : base(name, weight)
        {
            Wingspan = wingspan;
        }

        public virtual void Fly()
        {
            Console.WriteLine($"{Name} летит с размахом крыльев {Wingspan} м");
        }

        public override void PrintInfo()
        {
            base.PrintInfo();
            Console.WriteLine($"Размах крыльев: {Wingspan} м");
        }
    }

    // Класс Лев
    public class Lion : Mammal
    {
        public string PrideName { get; set; }

        public Lion(string name, double weight, int gestationPeriod, string prideName)
            : base(name, weight, gestationPeriod)
        {
            PrideName = prideName;
        }

        public override void MakeSound()
        {
            Console.WriteLine($"{Name} громко рычит: Рррррр!");
        }

        public override void Move()
        {
            Console.WriteLine($"{Name} бежит по саванне");
        }

        public override string GetHabitat()
        {
            return "Саванна";
        }

        public void Hunt()
        {
            Console.WriteLine($"{Name} из прайда '{PrideName}' охотится");
        }

        public override void PrintInfo()
        {
            base.PrintInfo();
            Console.WriteLine($"Прайд: {PrideName}, Среда обитания: {GetHabitat()}");
        }
    }

    // Класс Дельфин
    public class Dolphin : Mammal
    {
        public int PodSize { get; set; }

        public Dolphin(string name, double weight, int gestationPeriod, int podSize)
            : base(name, weight, gestationPeriod)
        {
            PodSize = podSize;
        }

        public override void MakeSound()
        {
            Console.WriteLine($"{Name} издает щелчки и свист");
        }

        public override void Move()
        {
            Console.WriteLine($"{Name} плавает в океане");
        }

        public override string GetHabitat()
        {
            return "Океан";
        }

        public void Jump()
        {
            Console.WriteLine($"{Name} выпрыгивает из воды!");
        }

        public override void PrintInfo()
        {
            base.PrintInfo();
            Console.WriteLine($"Размер стаи: {PodSize}, Среда обитания: {GetHabitat()}");
        }
    }

    // Класс Синица (конкретная птица)
    public class Tit : Bird
    {
        public string BeakColor { get; set; }

        public Tit(string name, double weight, double wingspan, string beakColor)
            : base(name, weight, wingspan)
        {
            BeakColor = beakColor;
        }

        public override void MakeSound()
        {
            Console.WriteLine($"{Name} выводит: Синь-синь-синь!");
        }

        public override void Move()
        {
            Console.WriteLine($"{Name} перелетает с ветки на ветку");
        }

        public override string GetHabitat()
        {
            return "Лес";
        }

        public override void Fly()
        {
            Console.WriteLine($"{Name} порхает среди деревьев");
        }

        public void Peck()
        {
            Console.WriteLine($"{Name} с {BeakColor} клювом клюет зернышки");
        }

        public override void PrintInfo()
        {
            base.PrintInfo();
            Console.WriteLine($"Цвет клюва: {BeakColor}, Среда обитания: {GetHabitat()}");
        }
    }

    class Program
    {
        static void Main()
        {
            Console.WriteLine("*** Демонстрация полиморфизма - Вариант 16 ***\n");

            // Создаем объекты разных классов
            var simba = new Lion("Симба", 190.5, 110, "Гордость Скалы");
            var flipper = new Dolphin("Флиппер", 210.0, 365, 12);
            var chirik = new Tit("Чирик", 0.018, 0.22, "серый");

            // Собираем в список базового типа Animal для демонстрации полиморфизма
            List<Animal> animals = new List<Animal> { simba, flipper, chirik };

            Console.WriteLine("=== Демонстрация полиморфизма через базовый класс ===");
            foreach (var animal in animals)
            {
                Console.WriteLine("\n--- Общая информация ---");
                animal.PrintInfo();

                Console.WriteLine("--- Полиморфные методы ---");
                animal.MakeSound();    // Полиморфизм: каждый объект вызывает свою реализацию
                animal.Move();         // Полиморфизм: каждый объект вызывает свою реализацию

                Console.WriteLine($"Среда обитания: {animal.GetHabitat()}"); // Полиморфизм

                // Вызов специфичных методов через проверку типа
                if (animal is Lion lion) lion.Hunt();
                if (animal is Dolphin dolphin) dolphin.Jump();
                if (animal is Tit tit) tit.Peck();
                if (animal is Mammal mammal) mammal.FeedMilk();
                if (animal is Bird bird) bird.Fly();

                Console.WriteLine(new string('-', 40));
            }

            // Демонстрация работы через интерфейсы
            Console.WriteLine("\n=== Демонстрация работы через интерфейсы ===");

            ISoundMaker[] soundMakers = { simba, flipper, chirik };
            foreach (var soundMaker in soundMakers)
            {
                soundMaker.MakeSound(); // Полиморфизм через интерфейс
            }

            Console.WriteLine("\n=== Прямой вызов методов конкретных классов ===");
            simba.Hunt();
            flipper.Jump();
            chirik.Peck();
        }
    }
}


