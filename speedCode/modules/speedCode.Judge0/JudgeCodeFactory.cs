using speedCode.codeTesting;
using speedCode.Core.judge0.dto;
using speedCode.Judge0.attributes;

namespace speedCode.Judge0;

using System.Reflection;

public static class JudgeCodeFactory
{
    private static readonly Dictionary<TestType, Type> _typeMap;

    static JudgeCodeFactory()
    {
        string targetNamespace = "speedCode.Judge0";

        _typeMap = Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(t =>
                t.Namespace == targetNamespace &&
                typeof(JudgeCode).IsAssignableFrom(t) &&
                !t.IsAbstract)
            .Select(t => new
            {
                Type = t,
                Attr = t.GetCustomAttribute<JudgeTypesAttributes>()
            })
            .ToDictionary(x => x.Attr.TestType, x => x.Type);
    }

    public static JudgeCode CreateJudgeCode(TestType testType)
    {
        if (_typeMap.TryGetValue(testType, out var type))
        {
            return (JudgeCode)Activator.CreateInstance(type)!;
        }

        throw new ArgumentException($"No JudgeCode class found for TestType: {testType}");
    }
}