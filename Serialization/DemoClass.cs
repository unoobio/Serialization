namespace Serialization
{
    public class DemoClass
    {
        public int IntField;
        public bool BoolFeild;
        public double DoubleField;

        public DateTime DateTimeProperty { get; set; }
        public string? StringProperty { get; set; }

        public static DemoClass Get() => 
            new DemoClass() 
            {
                IntField = 1,
                BoolFeild = true,
                DoubleField = 2.5,
                DateTimeProperty = DateTime.Now,
                StringProperty = "DemoClass Test",
            }; 
    }
}
