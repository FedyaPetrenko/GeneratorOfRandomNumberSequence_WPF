namespace GeneratorOfRandomNumber
{
    //Task 1. Static class with a method that extends the type object
    public static class MyExtensionClass
    {
        //The method extends the type object and conducts cloning process
        public static object Clone(this object obj)
        {
            return obj;
        }
    }
}
