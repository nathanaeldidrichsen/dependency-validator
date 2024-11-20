# dependency-validator
 A small C# console app to validate package dependencies

DependencyValidator is a C# .NET application designed to validate package installations based on dependency requirements. Given a list of packages and dependencies in a specific format, the application checks for conflicts and circular dependencies to ensure reliable installation.

Features
Dependency Validation: Ensures that all specified dependencies are compatible with the required versions.
Conflict Detection: Identifies and reports version conflicts between dependencies.
Circular Dependency Prevention: Detects circular dependencies, preventing infinite loops during installation.
File-Based Input: Reads data from .txt files with a defined structure for packages and dependencies.

Usage
Prepare Input Files:

Create .txt files with the following format:
First line: Number of packages to install.
Next lines: Package name and version (e.g., PackageA,1.0).
Followed by: Number of dependencies.
Remaining lines: Dependency pairs in the format ParentPackage,ParentVersion,DependentPackage,DependentVersion.

Example input file:
2
PackageA,1.0
PackageB,2.0
1
PackageA,1.0,PackageB,2.0

Run the Application:
open cmd at the root folder of the project and type `dotnet run`
input the location of the folder with your test data and press enter.

