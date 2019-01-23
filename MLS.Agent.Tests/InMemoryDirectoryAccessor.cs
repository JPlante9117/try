﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MLS.Agent.Markdown;
using WorkspaceServer.Servers.Roslyn;

namespace MLS.Agent.Tests
{
    internal class InMemoryDirectoryAccessor : IDirectoryAccessor, IEnumerable
    {
        private readonly DirectoryInfo _rootDirToAddFiles;
        private readonly DirectoryInfo _workingDirectory;
        private Dictionary<string, string> _files;

        public InMemoryDirectoryAccessor(DirectoryInfo workingDirectory, DirectoryInfo rootDirectoryToAddFiles = null)
        {
            _rootDirToAddFiles = rootDirectoryToAddFiles ?? TestAssets.SampleConsole;
            _workingDirectory = workingDirectory;
            _files = new Dictionary<string, string>();
        }

        public void Add((string path, string content) file)
        {
            _files.Add(
                new FileInfo(Path.Combine(_rootDirToAddFiles.FullName, file.path)).FullName, file.content);
        }

        public bool FileExists(RelativeFilePath filePath)
        { 
            return _files.ContainsKey(GetFullyQualifiedPath(filePath).FullName);
        }

        public string ReadAllText(RelativeFilePath filePath)
        {
            _files.TryGetValue(GetFullyQualifiedPath(filePath).FullName, out var value);
            return value;
        }

        public FileSystemInfo GetFullyQualifiedPath(RelativePath path)
        {
            if (path == null)
            {
                throw new ArgumentNullException();
            }

            switch (path)
            {
                case RelativeFilePath rfp:
                    return _workingDirectory.Combine(rfp);
                case RelativeDirectoryPath rdp:
                    return _workingDirectory.Combine(rdp);
                default:
                    throw new NotSupportedException();
            }
        }

        public IEnumerator GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public IDirectoryAccessor GetDirectoryAccessorForRelativePath(RelativeDirectoryPath relativePath)
        {
            var newPath = _workingDirectory.Combine(relativePath);
            return new InMemoryDirectoryAccessor(newPath)
            {
                _files = _files
            };
        }

        public IEnumerable<RelativeFilePath> GetAllFilesRecursively()
        {
            return _files.Keys.Select(key => new RelativeFilePath(
                Path.GetRelativePath(_workingDirectory.FullName, key)));
        }
    }
}