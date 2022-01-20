using NSubstitute;
using NUnit.Framework;
using ResharperToolsLib;
using System.Collections.Generic;
using System.IO;

namespace ResharperToolsLibTests
{
    public class CommandBuilderTests
    {
        private IList<string> Paths;

        private string Solution;

        [SetUp]
        public void Setup()
        {
            // set up solution mock
            Solution = ".\\ResharperCLITools.sln";

            // set up some file mocks
            var path1 = "ExampleApp\\OtherClass.cs";

            var path2 = "ExampleApp\\Program.cs";

            Paths = new[] { path1, path2 };
        }

        [Test]
        public void Clean_WithMultiplePaths_ReturnsValid()
        {
            // Arrange
            var validString = $"jb CleanUpCode \"{Solution}\" --include=\"{Paths[0]};{Paths[1]}\"";
            var commandBuilder = new CommandBuilder(Solution);

            // Act
            var command = commandBuilder.Clean(Paths);

            // Assert
            Assert.AreEqual(command, validString);
        }
    }
}