using Xunit;
using Moq;
using task05;

namespace task05tests
{
    public class TestClass
    {
        public int PublicField;
#pragma warning disable CS0169
        private string? _privateField;
#pragma warning restore CS0169
        public int Property { get; set; }

        public void Method() { }
        public void MethodWithParams(int a, string b) { }
    }

    [Serializable]
    public class AttributedClass { }

    public class ClassAnalyzerTests
    {
        [Fact]
        public void GetPublicMethods_ReturnsCorrectMethods()
        {
            var analyzer = new ClassAnalyzer(typeof(TestClass));
            var methods = analyzer.GetPublicMethods().ToList();

            Assert.Contains("Method", methods);
            Assert.Contains("MethodWithParams", methods);
            Assert.Equal(2, methods.Count);
        }

        [Fact]
        public void GetAllFields_IncludesPrivateFields()
        {
            var analyzer = new ClassAnalyzer(typeof(TestClass));
            var fields = analyzer.GetAllFields();

            Assert.Contains("_privateField", fields);
            Assert.Contains("PublicField", fields);
        }

        [Fact]
        public void GetProperties_ReturnsCorrectProperties()
        {
            var analyzer = new ClassAnalyzer(typeof(TestClass));
            var properties = analyzer.GetProperties().ToList();

            Assert.Contains("Property", properties);
            Assert.Single(properties);
        }

        [Fact]
        public void GetMethodParams_ReturnsCorrectParameters()
        {
            var analyzer = new ClassAnalyzer(typeof(TestClass));
            var parameters = analyzer.GetMethodParams("MethodWithParams").ToList();

            Assert.Equal(new[] { "a", "b" }, parameters);
        }

        [Fact]
        public void HasAttribute_ReturnsTrueForClassWithAttribute()
        {
            var analyzer = new ClassAnalyzer(typeof(AttributedClass));
            Assert.True(analyzer.HasAttribute<SerializableAttribute>());
        }
    }
}
