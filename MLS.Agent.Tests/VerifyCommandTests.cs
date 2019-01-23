﻿using System;
using System.CommandLine;
using System.IO;
using System.Threading.Tasks;
using FluentAssertions;
using MLS.Project.Generators;
using Xunit;
using Xunit.Abstractions;

namespace MLS.Agent.Tests
{
    public class VerifyCommandTests
    {
        private readonly ITestOutputHelper _output;

        public VerifyCommandTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public async Task Errors_are_written_to_std_out()
        {
            var workingDirectory = new DirectoryInfo(".");

            var directoryAccessor = new InMemoryDirectoryAccessor(workingDirectory, workingDirectory)
                                    {
                                        ("doc.md", @"
This is some sample code:
```cs Program.cs
```
")
                                    };

            var console = new TestConsole();

            await VerifyCommand.Do(
                workingDirectory,
                console,
                () => directoryAccessor);

            console.Out
                   .ToString()
                   .Should()
                   .Contain("doc.md (line 3): File not found: ./Program.cs");
        }

        [Fact]
        public async Task Files_are_listed()
        {
            var root = new DirectoryInfo(Directory.GetDirectoryRoot(Directory.GetCurrentDirectory()));

            var directoryAccessor = new InMemoryDirectoryAccessor(root, root)
                                    {
                                        ("some.csproj", ""),
                                        ("Program.cs", ""),
                                        ("doc.md", @"
```cs Program.cs
```
")
                                    };

            var console = new TestConsole();

            await VerifyCommand.Do(
                root,
                console,
                () => directoryAccessor);

            _output.WriteLine(console.Out.ToString());

            console.Out
                   .ToString()
                   .Trim()
                   .EnforceLF()
                   .Should()
                   .Match(
                       $@"{root}doc.md
  {root}Program.cs (in project {root}some.csproj)".EnforceLF());
        }

        [Fact]
        public async Task When_there_are_no_errors_then_return_code_is_0()
        {
            var workingDirectory = new DirectoryInfo(".");

            var directoryAccessor = new InMemoryDirectoryAccessor(workingDirectory)
                                    {
                                        ("some.csproj", ""),
                                        ("Program.cs", ""),
                                        ("doc.md", @"
```cs Program.cs
```
")
                                    };

            var console = new TestConsole();

            var resultCode = await VerifyCommand.Do(
                                 workingDirectory,
                                 console,
                                 () => directoryAccessor);

            _output.WriteLine(console.Out.ToString());

            resultCode.Should().Be(0);
        }

        [Fact]
        public async Task When_there_are_errors_then_return_code_is_1()
        {
            var workingDirectory = new DirectoryInfo(".");

            var directoryAccessor = new InMemoryDirectoryAccessor(workingDirectory)
                                    {
                                        ("doc.md", @"
```cs Program.cs
```
")
                                    };

            var console = new TestConsole();

            var resultCode = await VerifyCommand.Do(
                                 workingDirectory,
                                 console,
                                 () => directoryAccessor);

            resultCode.Should().Be(1);
        }
    }
}