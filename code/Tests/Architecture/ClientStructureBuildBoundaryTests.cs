using NUnit.Framework;
using System.Text.RegularExpressions;

namespace CosmosCasino.Tests.Architecture
{
    [TestFixture]
    internal sealed class ClientStructureBuildBoundaryTests
    {
        #region Legacy Build Contexts

        [Test]
        public void LegacyFloorWallContexts_DoNotCreateStructureBuildIntents()
        {
            string floorContext = ReadRepositoryFile(
                "code",
                "Client",
                "Game",
                "Build",
                "BuildContext",
                "BuildContexts",
                "FloorBuildContext.cs");
            string wallContext = ReadRepositoryFile(
                "code",
                "Client",
                "Game",
                "Build",
                "BuildContext",
                "BuildContexts",
                "WallBuildContext.cs");

            AssertLegacyContextDoesNotUseStructurePipeline(floorContext);
            AssertLegacyContextDoesNotUseStructurePipeline(wallContext);
        }

        #endregion

        #region Definition Mapping

        [Test]
        public void ClientSource_DoesNotDefineFakeFloorWallStructureDefinitions()
        {
            string clientSource = ReadClientSource();

            Assert.That(clientSource, Does.Not.Contain(Forbidden("Build", "Structure", "Definitions")));
            Assert.That(clientSource, Does.Not.Contain(Forbidden("Floor", "Definition", "Id")));
            Assert.That(clientSource, Does.Not.Contain(Forbidden("Wall", "Definition", "Id")));
            Assert.That(clientSource, Does.Not.Contain(Forbidden("Try", "Get", "Build", "Kind")));
            AssertNoMatch(clientSource, @"new\s+StructureDefinitionId\s*\(\s*[12]\s*\)");
            AssertNoMatch(clientSource, @"StructureDefinitionId\s*\(\s*[12]\s*\)");
        }

        #endregion

        #region Helpers

        private static void AssertLegacyContextDoesNotUseStructurePipeline(string source)
        {
            Assert.That(source, Does.Not.Contain("BuildIntent.PlaceStructures"));
            Assert.That(source, Does.Not.Contain("BuildIntent.RemoveStructuresAt"));
            Assert.That(source, Does.Not.Contain("StructureDefinition"));
            Assert.That(source, Does.Not.Contain(Forbidden("Build", "Structure", "Definitions")));
        }

        private static void AssertNoMatch(string source, string pattern)
        {
            Assert.That(
                Regex.IsMatch(source, pattern, RegexOptions.CultureInvariant),
                Is.False,
                $"Unexpected match for pattern: {pattern}");
        }

        private static string Forbidden(params string[] parts)
        {
            return string.Concat(parts);
        }

        private static string ReadClientSource()
        {
            string clientRoot = GetRepositoryPath("code", "Client");
            IEnumerable<string> sourceFiles = Directory.EnumerateFiles(
                clientRoot,
                "*.cs",
                SearchOption.AllDirectories);

            return string.Join(
                Environment.NewLine,
                sourceFiles.Select(File.ReadAllText));
        }

        private static string ReadRepositoryFile(params string[] pathParts)
        {
            return File.ReadAllText(GetRepositoryPath(pathParts));
        }

        private static string GetRepositoryPath(params string[] pathParts)
        {
            string repositoryRoot = GetRepositoryRoot();
            return Path.Combine(new[] { repositoryRoot }.Concat(pathParts).ToArray());
        }

        private static string GetRepositoryRoot()
        {
            var directory = new DirectoryInfo(TestContext.CurrentContext.TestDirectory);

            while (directory != null)
            {
                if (File.Exists(Path.Combine(directory.FullName, "CosmosCasino.sln")))
                {
                    return directory.FullName;
                }

                directory = directory.Parent;
            }

            Assert.Fail("Could not locate repository root from test directory.");
            return string.Empty;
        }

        #endregion
    }
}
