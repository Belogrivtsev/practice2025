namespace task02;
public class Student
{
    public string Name { get; set; }
    public string Faculty { get; set; }
    public List<int> Grades { get; set; }
}
public class StudentService
{
    private readonly List<Student> _students;

    public StudentService(List<Student> students) => _students = students;

    public IEnumerable<Student> GetStudentsByFaculty(string faculty) 
    => _students.Where(s => s.Faculty == faculty);

    public IEnumerable<Student> GetStudentsWithMinAverageGrade(double minAverageGrade) 
    => _students.Where(s => (s.Grades.Average() >= minAverageGrade) && s.Grades.Any());

    public IEnumerable<Student> GetStudentsOrderedByName() 
    => _students.OrderBy(s => s.Name);

    public ILookup<string, Student> GroupStudentsByFaculty()
        => _students.ToLookup(s => s.Faculty);

    public string GetFacultyWithHighestAverageGrade()
        => _students.GroupBy(s => s.Faculty).OrderByDescending
        (g => g.Average(s => s.Grades.DefaultIfEmpty().Average())).FirstOrDefault()?.Key;
}