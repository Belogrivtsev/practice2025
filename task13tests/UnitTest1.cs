namespace task13tests;

using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Xunit;
using task13;
public class JsonSerializationTests
{
    [Fact]
    public void Serialize_Student_ReturnsValidJson()
    {
        var student = new Student
        {
            FirstName = "John",
            LastName = "Snow",
            BirthDate = new DateTime(2000, 1, 1),
            Grades = new List<Subject>
            {
                new Subject("Math", 5),
                new Subject("Physics", 4)
            }
        };

        string json = JsonProcessing.ToJson(student);
        
        Assert.Contains("\"FirstName\": \"John\"", json);
        Assert.Contains("\"LastName\": \"Snow\"", json);
        Assert.Contains("\"BirthDate\": \"2000-01-01\"", json);
        Assert.Contains("\"Grades\"", json);
    }

    [Fact]
    public void Deserialize_ValidJson_ReturnsStudent()
    {
        string json = @"{""FirstName"": ""John"", ""LastName"": ""Snow"", ""BirthDate"": ""2000-01-01"",
        ""Grades"": [{ ""Name"": ""Math"", ""Grade"": 5 },{ ""Name"": ""Physics"", ""Grade"": 4 }]}";

        var student = JsonProcessing.FromJson(json);
        
        Assert.Equal("John", student.FirstName);
        Assert.Equal("Snow", student.LastName);
        Assert.Equal(new DateTime(2000, 1, 1), student.BirthDate);
        Assert.NotNull(student.Grades);
        Assert.Equal(2, student.Grades.Count);
    }

    [Fact]
    public void SaveAndLoad_Student_ReturnsSameData()
    {
        var student = new Student
        {
            FirstName = "Ivan",
            LastName = "Ivanov",
            BirthDate = new DateTime(1999, 5, 15),
            Grades = new List<Subject>
            {
                new Subject("Chemistry", 5)
            }
        };

        string filePath = Path.GetTempFileName();
        
        try
        {
            JsonProcessing.SaveToFile(filePath, student);
            var loadedStudent = JsonProcessing.LoadFromFile(filePath);
            
            Assert.Equal(student.FirstName, loadedStudent.FirstName);
            Assert.Equal(student.LastName, loadedStudent.LastName);
            Assert.Equal(student.BirthDate, loadedStudent.BirthDate);
            Assert.NotNull(student.Grades);
            Assert.Equal(student.Grades[0].Name, loadedStudent.Grades[0].Name);
            Assert.Equal(student.Grades[0].Grade, loadedStudent.Grades[0].Grade);
        }
        finally
        {
            File.Delete(filePath);
        }
    }
}
