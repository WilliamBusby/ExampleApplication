// See https://aka.ms/new-console-template for more information
using ExampleApplication.Application.Helpers.Database;
using ExampleApplication.Application.Models.Database;
using System;
using System.Collections.Generic;

Console.WriteLine("Hello, World!");


TestDatabaseModel test = TestDatabaseHelper.GetTestQuery();

Console.WriteLine(test.Name);
Console.WriteLine(test.Age);

string test2 = TestDatabaseHelper.GetTestQuery2();

Console.WriteLine(test2);

List<TestDatabaseModel> test3 = TestDatabaseHelper.GetTestQuery3();

foreach(TestDatabaseModel testModel3 in test3)
{
    Console.WriteLine(testModel3.Name);
    Console.WriteLine(testModel3.Age);
}