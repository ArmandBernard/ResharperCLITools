using NSubstitute;
using NUnit.Framework;
using ResharperToolsLib;
using System.Collections.Generic;

namespace ResharperToolsLibTests
{
    public class CommandBuilderTests
    {
        private readonly string Solution = ".\\ResharperCLITools.sln";

        private readonly IList<string> Paths = new List<string>()
        {
            "ExampleApp\\OtherClass.cs",
            "ExampleApp\\Program.cs"
        };

        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Clean_NoPathsSpecified_ReturnsWithNoFilter()
        {
            // Arrange
            var validString = $"CleanUpCode \"{Solution}\"";
            var commandBuilder = new CommandBuilder(Solution);

            // Act
            var command = commandBuilder.Clean();

            // Assert
            Assert.AreEqual(validString, command);
        }

        [Test]
        public void Clean_OnePath_ReturnsWithValidFilter()
        {
            // Arrange
            var validString = $"CleanUpCode \"{Solution}\" --include=\"{Paths[0]}\"";
            var commandBuilder = new CommandBuilder(Solution);

            // Act
            var command = commandBuilder.Clean(new[] { Paths[0] });

            // Assert
            Assert.AreEqual(validString, command);
        }

        [Test]
        public void Clean_WithMultiplePaths_ReturnsWithValidFilter()
        {
            // Arrange
            var validString = $"CleanUpCode \"{Solution}\" --include=\"{Paths[0]};{Paths[1]}\"";
            var commandBuilder = new CommandBuilder(Solution);

            // Act
            var command = commandBuilder.Clean(Paths);

            // Assert
            Assert.AreEqual(validString, command);
        }
    }
}