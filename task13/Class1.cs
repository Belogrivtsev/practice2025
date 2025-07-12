using System;
using System.Text.Json;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace task13;

public class Subject
{
    public string Name { get; set; } = string.Empty;
    public int Grade { get; set; }

    public Subject() { }

    public Subject(string name, int grade)
    {
        Name = name;
        Grade = grade;
    }
}
public class Student
{
    public string FirstName { get; set; } 
    public string LastName { get; set; }

    [JsonConverter(typeof(CustomDateFormatting))]
    public DateTime BirthDate { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<Subject>? Grades { get; set; }

    public Student() { }

    public Student(string firstName, string lastName, DateTime birthDate, List<Subject> grades)
    {
        FirstName = firstName;
        LastName = lastName;
        BirthDate = birthDate;
        Grades = grades;
    }
}
public class CustomDateFormatting : JsonConverter<DateTime>
{
    public string DateFormat { get; } = "yyyy-MM-dd";
    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return DateTime.ParseExact(reader.GetString(), DateFormat, null);
    }

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString(DateFormat));
    }
}
public static class JsonProcessing
{
    private static readonly JsonSerializerOptions _options = new()
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        WriteIndented = true
    };

    public static string ToJson(Student student)
    {
        return JsonSerializer.Serialize(student, _options);
    }

    public static Student FromJson(string json)
    {
        try
        {
            var student = JsonSerializer.Deserialize<Student>(json, _options);
            if (string.IsNullOrEmpty(student.FirstName))
                throw new JsonException("FirstName required");
            if (string.IsNullOrEmpty(student.LastName))
                throw new JsonException("LastName required");

            return student;
        }
        catch (JsonException ex)
        {
            throw new JsonException("Error json data", ex);
        }
    }
    public static void SaveToFile(string filePath, Student student)
    {
        string json = ToJson(student);
        File.WriteAllText(filePath, json);
    }
    public static Student LoadFromFile(string filePath)
    {
        string json = File.ReadAllText(filePath);
        return FromJson(json);
    }
}
