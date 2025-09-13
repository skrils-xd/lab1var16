using System;
using System.Collections.Generic;

namespace Lab1Variant16
{
    // Базовый класс «Животное»
    class Animal
    {
        public string Name { get; set; }
        public double Weight { get; set; } // вес в кг

        public Animal() { }
        public Animal(string name, double weight)
        {
            Name = name;
            Weight = weight;
        }

        // Виртуальный метод для издания звука
        public virtual void MakeSound()
        {
            Console.WriteLine($"{Name} издает неопределенный звук.");
        }

        // Виртуальный метод для вывода информации
        public virtual void Print(string prefix = "")
        {
            Console.WriteLine($"{prefix}Животное: {Name}, вес: {Weight:0.###} кг");
        }
    }

    // Млекопитающее
    class Mammal : Animal
    {
        public int GestationPeriod { get; set; } // период беременности в днях

        public Mammal() { }
        public Mammal(string name, double weight, int gestationPeriod)
            : base(name, weight)
        {
            GestationPeriod = gestationPeriod;
        }

        public void FeedMilk()
        {
            Console.WriteLine($"{Name} кормит детенышей молоком.");
        }

        public override void Print(string prefix = "")
        {
            Console.WriteLine($"{prefix}Млекопитающее: {Name}, вес: {Weight:0.###} кг, вынашивание: {GestationPeriod} дн.");
        }
    }

    // Птица
    class Bird : Animal
    {
        public double Wingspan { get; set; } // размах крыльев в метрах

        public Bird() { }
        public Bird(string name, double weight, double wingspan)
            : base(name, weight)
        {
            Wingspan = wingspan;
        }

        public virtual void Fly()
        {
            Console.WriteLine($"{Name} летит с размахом крыльев {Wingspan:0.##} м.");
        }

        public override void Print(string prefix = "")
        {
            Console.WriteLine($"{prefix}Птица: {Name}, вес: {Weight:0.###} кг, размах крыльев: {Wingspan:0.##} м");
        }
    }

    // Лев
    class Lion : Mammal
    {
        public string PrideName { get; set; } // название прайда

        public Lion() { }
        public Lion(string name, double weight, int gestationPeriod, string prideName)
            : base(name, weight, gestationPeriod)
        {
            PrideName = prideName;
        }

        public void Hunt()
        {
            Console.WriteLine($"{Name} из прайда '{PrideName}' охотится.");
        }

        public override void MakeSound()
        {
            Console.WriteLine($"{Name} громко рычит: Рррррр!");
        }

        public override void Print(string prefix = "")
        {
            Console.WriteLine($"{prefix}Лев: {Name}, вес: {Weight:0.###} кг, прайд: '{PrideName}'");
        }
    }

    // Дельфин
    class Dolphin : Mammal
    {
        public int PodSize { get; set; } // размер стаи

        public Dolphin() { }
        public Dolphin(string name, double weight, int gestationPeriod, int podSize)
            : base(name, weight, gestationPeriod)
        {
            PodSize = podSize;
        }

        public void Jump()
        {
            Console.WriteLine($"{Name} выпрыгивает из воды!");
        }

        public override void MakeSound()
        {
            Console.WriteLine($"{Name} издает щелчки и свист.");
        }

        public override void Print(string prefix = "")
        {
            Console.WriteLine($"{prefix}Дельфин: {Name}, вес: {Weight:0.###} кг, стая: {PodSize} особ.");
        }
    }

    // Синица
    class Tit : Bird
    {
        public string BeakColor { get; set; } // цвет клюва

        public Tit() { }
        public Tit(string name, double weight, double wingspan, string beakColor)
            : base(name, weight, wingspan)
        {
            BeakColor = beakColor;
        }

        public void Peck()
        {
            Console.WriteLine($"{Name} с {BeakColor} клювом клюет зернышки.");


        }

        public override void Fly()
        {
            Console.WriteLine($"{Name} порхает с ветки на ветку.");
        }

        public override void MakeSound()
        {
            Console.WriteLine($"{Name} выводит: Синь-синь-синь!");
        }

        public override void Print(string prefix = "")
        {
            Console.WriteLine($"{prefix}Синица: {Name}, вес: {Weight:0.###} кг, клюв: {BeakColor}");
        }
    }

    class Program
    {
        static void Main()
        {
            Console.WriteLine("*** Демонстрация иерархии классов животных ***\n");

            // Создаем животных
            var simba = new Lion("Симба", 190.5, 110, "Гордость Скалы");
            var flipper = new Dolphin("Флиппер", 210.0, 365, 12);
            var chirik = new Tit("Чирик", 0.018, 0.22, "серый");

            // Собираем их в список базового типа для демонстрации полиморфизма
            List<Animal> zoo = new List<Animal> { simba, flipper, chirik };

            // Демонстрация работы с каждым объектом
            foreach (var creature in zoo)
            {
                creature.Print();
                creature.MakeSound();

                // Проверяем тип, чтобы вызвать специфичные методы
                if (creature is Lion lion) lion.Hunt();
                if (creature is Dolphin dolphin) dolphin.Jump();
                if (creature is Tit tit) tit.Peck();
                if (creature is Mammal mammal) mammal.FeedMilk();
                if (creature is Bird bird) bird.Fly();

                Console.WriteLine(); // Разделитель между животными
            }

            // Демонстрация прямого вызова специфичных методов
            Console.WriteLine("--- Прямой вызов методов ---");
            simba.Hunt();
            flipper.Jump();
            chirik.Peck();
        }
    }
}