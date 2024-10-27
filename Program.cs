using DependencyValidator.Models;
using DependencyValidator.Services;

namespace DependencyValidator
{
    /// <summary>
    /// Author: Nathanael
    /// Date: 27-10-2024
    /// 
    /// The Program class serves as the entry point for the Dependency Validator application. 
    /// It prompts the user for a folder path containing text files with package and dependency 
    /// information, processes each file, and validates the packages and their dependencies 
    /// using the DependencyValidatorService. Results are outputted to the console indicating 
    /// whether the validation passed or failed for each file.
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            var folderPath = PromptForFolderPath();
            if (string.IsNullOrWhiteSpace(folderPath) || !Directory.Exists(folderPath))
            {
                Console.WriteLine("Invalid folder path. Exiting.");
                return;
            }

            ProcessFilesInFolder(folderPath);
        }

        static string PromptForFolderPath()
        {
            Console.WriteLine("Enter the path to the test data folder:");
            var input = Console.ReadLine();
            return input ?? string.Empty; // Return an empty string if input is null
        }

        static void ProcessFilesInFolder(string folderPath)
        {
            foreach (var filePath in Directory.GetFiles(folderPath, "*.txt"))
            {
                Console.WriteLine($"Processing {Path.GetFileName(filePath)}");
                ProcessFile(filePath);
            }
        }

        static void ProcessFile(string filePath)
        {
            try
            {
                var (packagesToInstall, dependencies) = ParseFile(filePath);
                var validator = new DependencyValidatorService(packagesToInstall, dependencies);
                bool isValid = validator.ValidateInstallation();
                Console.WriteLine(isValid ? "PASS" : "FAIL");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing {filePath}: {ex.Message}");
            }
        }

        static (IEnumerable<Package>, IEnumerable<Dependency>) ParseFile(string filePath)
        {
            var lines = File.ReadAllLines(filePath);
            var packagesToInstall = new Dictionary<string, Package>();
            var dependencies = new List<Dependency>();

            int currentLine = 0;
            if (!TryParseNumPackages(lines, ref currentLine, out int numPackagesToInstall)) return (Array.Empty<Package>(), Array.Empty<Dependency>());
            ReadPackages(lines, ref currentLine, numPackagesToInstall, packagesToInstall);
            ReadDependencies(lines, ref currentLine, packagesToInstall, dependencies);

            return (packagesToInstall.Values, dependencies);
        }

        static bool TryParseNumPackages(string[] lines, ref int currentLine, out int numPackagesToInstall)
        {
            if (currentLine >= lines.Length || !int.TryParse(lines[currentLine++], out numPackagesToInstall))
            {
                Console.WriteLine("Invalid format for number of packages to install.");
                numPackagesToInstall = 0;
                return false;
            }
            return true;
        }

        static void ReadPackages(string[] lines, ref int currentLine, int numPackagesToInstall, Dictionary<string, Package> packagesToInstall)
        {
            for (int i = 0; i < numPackagesToInstall; i++)
            {
                if (currentLine >= lines.Length) throw new FormatException("Unexpected end of file while reading packages.");
                var packageParts = lines[currentLine++].Split(',');

                if (packageParts.Length != 2)
                {
                    throw new FormatException($"Invalid package format on line {currentLine}: {lines[currentLine - 1]}");
                }

                var packageKey = GetPackageKey(packageParts[0], packageParts[1]);
                var package = new Package(packageParts[0], packageParts[1]);

                if (!packagesToInstall.ContainsKey(packageKey))
                {
                    packagesToInstall[packageKey] = package;
                }
            }
        }

        static void ReadDependencies(string[] lines, ref int currentLine, Dictionary<string, Package> packagesToInstall, List<Dependency> dependencies)
        {
            if (currentLine < lines.Length && int.TryParse(lines[currentLine], out int numDependencies))
            {
                currentLine++; // Move to dependency lines
                for (int i = 0; i < numDependencies; i++)
                {
                    if (currentLine >= lines.Length) throw new FormatException("Unexpected end of file while reading dependencies.");

                    var dependencyParts = lines[currentLine++].Split(',');
                    var parentKey = GetPackageKey(dependencyParts[0], dependencyParts[1]);
                    var dependentKey = GetPackageKey(dependencyParts[2], dependencyParts[3]);

                    if (packagesToInstall.TryGetValue(parentKey, out var parentPackage))
                    {
                        var dependentPackage = new Package(dependencyParts[2], dependencyParts[3]);
                        parentPackage.Dependencies.Add(new Dependency(parentPackage, dependentPackage));
                        dependencies.Add(new Dependency(parentPackage, dependentPackage));
                    }
                }
            }
        }

        static string GetPackageKey(string name, string version) => $"{name},{version}";
    }
}
