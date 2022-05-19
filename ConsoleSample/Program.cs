using System;
using System.Collections.Generic;

namespace ConsoleSample
{
    internal class Program
    {
        static void Main(string[] args)
        {

            Console.WriteLine("Hello World!");

            IEmployee employee = new Employee();

            List<string> list1 = new List<string>();
            list1.Add("Hello!");

            List<int> list2 = new List<int>();
            list2.Add(1);

            int[] arr = new int[5];

            Int32 i = 5;

            MyCollection<int> myCollection = new MyCollection<int>();


        }
    }

    class MyCollection<TMy> where TMy : struct
    {
        TMy[] arr;

        public MyCollection()
        {
            arr = new TMy[1000];

        }

    }

    interface IEmployee
    {

    }

    class Employee : Person, IEmployee
    {

    }

    class EmployeeV2 : IEmployee
    {

    }

    class Person
    {

    }
}
