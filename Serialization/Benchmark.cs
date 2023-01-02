using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using Serialization.Serializators;

namespace Serialization
{
    [MemoryDiagnoser]
    [Orderer(BenchmarkDotNet.Order.SummaryOrderPolicy.SlowestToFastest)]
    [RankColumn]
    [SimpleJob(RunStrategy.Monitoring, warmupCount: 5, launchCount: 1, targetCount: 1000)]
    public class Benchmark
    {
        public Benchmark()
        {
            _testClass = DemoClass.Get();
            _customSerializedObject = ReflectionSerializer.SerializeToCsvFormat(new List<DemoClass> { _testClass });
            _newtonsoftJSONSerializedObject = Newtonsoft.Json.JsonConvert.SerializeObject(_testClass);
        }

        private readonly DemoClass _testClass;
        private readonly string _customSerializedObject;
        private readonly string _newtonsoftJSONSerializedObject;

        [Benchmark]
        public void CustomSerializationWithOutput()
        {
            string serializedObject = ReflectionSerializer.SerializeToCsvFormat(new List<DemoClass> { _testClass });
            Console.WriteLine(serializedObject);
        }

        [Benchmark]
        public void NewtonSoftJsonSerializationWithOutput()
        {
            string str = Newtonsoft.Json.JsonConvert.SerializeObject(_testClass);
            Console.WriteLine(str);
        }

        [Benchmark]
        public void CustomSerialization()
        {
            ReflectionSerializer.SerializeToCsvFormat(new List<DemoClass> { _testClass });
        }

        [Benchmark]
        public void NewtonSoftJsonSerialization()
        {
            Newtonsoft.Json.JsonConvert.SerializeObject(_testClass);
        }

        [Benchmark]
        public void CustomDeserialization()
        {
            ReflectionSerializer.Deserialize<DemoClass>(_customSerializedObject);
        }

        [Benchmark]
        public void NewtonSoftJsonDeserialization()
        {
            Newtonsoft.Json.JsonConvert.DeserializeObject<DemoClass>(_newtonsoftJSONSerializedObject);
        }
    }
}
