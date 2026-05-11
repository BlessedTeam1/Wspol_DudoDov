using System;
using System.Collections.Concurrent;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Data
{
    internal class Logger : IDisposable
    {
        private readonly BlockingCollection<string> _logBuffer;
        private readonly Task _writerTask;
        private readonly string _filePath;
        private readonly CancellationTokenSource _cts;

        public Logger(string filePath = "diagnostics.log")
        {
            _filePath = filePath;
            // Ограничиваем буфер, чтобы избежать утечек памяти
            _logBuffer = new BlockingCollection<string>(5000);
            _cts = new CancellationTokenSource();
            _writerTask = Task.Run(WriteToFileAsync);
        }

        public void Log(IBalls ball)
        {
            if (!_logBuffer.IsAddingCompleted)
            {
                string logEntry = $"{{\"Time\":\"{DateTime.Now:O}\", \"X\":{ball.X:F2}, \"Y\":{ball.Y:F2}, \"VelX\":{ball.VelX:F2}, \"VelY\":{ball.VelY:F2}}}";
                _logBuffer.TryAdd(logEntry);
            }
        }

        private async Task WriteToFileAsync()
        {
            try
            {
                // ИСПРАВЛЕНИЕ ЗДЕСЬ: Классический using с фигурными скобками для C# 7.3
                using (StreamWriter writer = new StreamWriter(_filePath, append: false, Encoding.ASCII))
                {
                    foreach (var log in _logBuffer.GetConsumingEnumerable(_cts.Token))
                    {
                        await writer.WriteLineAsync(log);
                    }
                }
            }
            catch (OperationCanceledException) { }
        }

        public void Dispose()
        {
            _logBuffer.CompleteAdding();
            _cts.Cancel();
            try { _writerTask.Wait(1000); } catch { }
            _logBuffer.Dispose();
            _cts.Dispose();
        }
    }
}