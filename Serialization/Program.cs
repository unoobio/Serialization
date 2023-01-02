// See https://aka.ms/new-console-template for more information
using BenchmarkDotNet.Running;
using Serialization;
using Serialization.Serializators;

BenchmarkRunner.Run<Benchmark>();

string serializedObject = ReflectionSerializer.SerializeToCsvFormat(new List<DemoClass> { DemoClass.Get() });
Console.WriteLine($"Serialized object by custom reflection: {serializedObject}");

serializedObject = Newtonsoft.Json.JsonConvert.SerializeObject(DemoClass.Get());
Console.WriteLine($"Serialized object by newtonsoft.json: {serializedObject}");
