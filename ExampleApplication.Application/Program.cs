// See https://aka.ms/new-console-template for more information
using ExampleApplication.Database.Helpers;
using ExampleApplication.Database.Models;
using System;
using System.Collections.Generic;

Console.WriteLine("Hello, World!");

ITestDatabaseHelper databaseHelper = new TestDatabaseHelper("<<Connection String>>");

TestDatabaseModel test = databaseHelper.GetTestQuery();

Console.WriteLine("Test1");
Console.WriteLine(test.Name);
Console.WriteLine(test.Age);

string test2 = databaseHelper.GetTestQuery2();

Console.WriteLine("Test2");
Console.WriteLine(test2);

List<TestDatabaseModel> test3 = databaseHelper.GetTestQuery3();

Console.WriteLine("Test3");
foreach (TestDatabaseModel testModel3 in test3)
{
    Console.WriteLine(testModel3.Name);
    Console.WriteLine(testModel3.Age);
}

string? test4 = databaseHelper.GetTestQuery4();

Console.WriteLine("Test4");
Console.WriteLine(test4);

string? test5 = databaseHelper.GetTestQuery5();

Console.WriteLine("Test5");
Console.WriteLine(test5);