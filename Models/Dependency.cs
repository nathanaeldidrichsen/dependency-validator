namespace DependencyValidator.Models
{
    public record Dependency(Package Parent, Package DependencyPackage);
}
