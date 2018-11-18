namespace UpdateFromGithub
{
    public class Payload
    {
        public class Repository_
        {
            public string Name { get; set; }
        }

        public Repository_ Repository { get; set; }
    }
}
