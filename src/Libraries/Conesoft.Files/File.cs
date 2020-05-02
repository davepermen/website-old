using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using IO = System.IO;

namespace Conesoft.Files
{
    public class File : Directory
    {
        public File(Directory directoryAsFile) : base(directoryAsFile)
        {
        }

        public async Task<string> ReadTextAsync() => await IO.File.ReadAllTextAsync(path);
        public async Task<string[]> ReadLinesAsync() => await IO.File.ReadAllLinesAsync(path);
        public async Task<T> ReadFromJsonAsync<T>(JsonSerializerOptions? options = null)
        {
            using var stream = IO.File.OpenRead(path);
            return await JsonSerializer.DeserializeAsync<T>(stream, options);
        }

        public async Task WriteTextAsync(string content)
        {
            Parent.Create();
            await IO.File.WriteAllTextAsync(path, content);
        }

        public async Task AppendTextAsync(string content)
        {
            Parent.Create();
            await IO.File.AppendAllTextAsync(path, content);
        }

        public bool Exists => IO.File.Exists(path);

        public IO.FileInfo Info => new IO.FileInfo(path);

        public async Task AppendLineAsync(string content) => await AppendTextAsync(content + Environment.NewLine);

        public async Task WriteAsJsonAsync<T>(T content, bool pretty = false)
        {
            Parent.Create();
            using var stream = IO.File.Create(path);
            await JsonSerializer.SerializeAsync(stream, content, new JsonSerializerOptions { WriteIndented = pretty });
        }

        public static Filename Name(string filename, string extension) => new Filename(filename, extension);

        public async Task WriteLinesAsync(IEnumerable<string> contents)
        {
            Parent.Create();
            await IO.File.WriteAllLinesAsync(path, contents);
        }
    }
}
