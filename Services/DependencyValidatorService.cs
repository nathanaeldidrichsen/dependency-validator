using DependencyValidator.Models;

namespace DependencyValidator.Services
{

    /// <summary>
    /// Author: Nathanael
    /// Date: 27-10-2024
    /// 
    /// The DependencyValidatorService class is responsible for validating a set of packages
    /// and their dependencies to ensure that all required dependencies can be installed 
    /// without conflicts. It checks for version conflicts and circular dependencies during 
    /// the validation process.
    /// </summary>
    /// 
    public class DependencyValidatorService
    {
        private readonly HashSet<Package> _packagesToInstall; // Collection of packages to install
        private readonly Dictionary<string, string> _packageVersionMap; // Map of package names to their versions

        public DependencyValidatorService(IEnumerable<Package> packagesToInstall, IEnumerable<Dependency> dependencies)
        {
            _packagesToInstall = new HashSet<Package>(packagesToInstall);
            _packageVersionMap = packagesToInstall.ToDictionary(p => p.Name, p => p.Version);
        }

        // Validate all packages to ensure their dependencies can be installed
        public bool ValidateInstallation()
        {
            foreach (var package in _packagesToInstall)
            {
                var visitedPackages = new HashSet<Package>(); // Track visited packages to avoid cycles
                if (!ValidatePackageDependencies(package, visitedPackages))
                    return false; // If any package is invalid, installation fails
            }
            return true; // All packages are valid
        }

        // Recursively validate the dependencies for a given package
        private bool ValidatePackageDependencies(Package package, HashSet<Package> visitedPackages)
        {
            if (visitedPackages.Contains(package))
                return true; // Prevent circular dependency by returning true if already visited

            visitedPackages.Add(package); // Mark the package as visited

            // Loop over each dependency of the current package
            foreach (var dependency in package.Dependencies.Select(d => d.DependencyPackage))
            {
                // Check if the dependency exists in the map with a version
                if (_packageVersionMap.TryGetValue(dependency.Name, out var requiredVersion))
                {
                    // If there’s a version conflict, return false
                    if (requiredVersion != dependency.Version)
                        return false; // Conflict detected
                }
                else
                {
                    // Check for circular dependencies
                    if (dependency.Name == package.Name)
                    {
                        // If the dependency is the same package but with a different version
                        if (dependency.Version != package.Version)
                            return false; // Conflict detected with the same package
                    }
                    // Add the new dependency version if not already in the map
                    _packageVersionMap[dependency.Name] = dependency.Version;
                }

                // Recursively check dependencies of this dependency
                if (!ValidatePackageDependencies(dependency, visitedPackages))
                    return false; // If any sub-dependency is invalid, return false
            }

            visitedPackages.Remove(package); // Clean up visited packages for further checks
            return true; // All dependencies are valid
        }
    }
}
