namespace DependencyValidator.Models
{
    public record Package(string Name, string Version)
    {
        public List<Dependency> Dependencies { get; } = new List<Dependency>();
    }
}
