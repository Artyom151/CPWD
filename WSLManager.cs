using System.Diagnostics;
using System.Text;

namespace CPWD
{
    public class WSLManager : IDisposable
    {
        private bool isDisposed;

        public WSLManager()
        {
            isDisposed = false;
        }

        public async Task<List<WSLDistribution>> GetDistributionsAsync()
        {
            var distributions = new List<WSLDistribution>();
            
            try
            {
                var output = await ExecuteCommandAsync("wsl.exe", "--list --verbose");
                var lines = output.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

                // Пропускаем заголовок
                foreach (var line in lines.Skip(1))
                {
                    var parts = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length >= 3)
                    {
                        distributions.Add(new WSLDistribution
                        {
                            Name = parts[0],
                            State = parts[1],
                            Version = parts[2]
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка при получении списка дистрибутивов: {ex.Message}");
            }

            return distributions;
        }

        public async Task<bool> StartDistributionAsync(string name)
        {
            try
            {
                await ExecuteCommandAsync("wsl.exe", $"--distribution {name}");
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка при запуске дистрибутива {name}: {ex.Message}");
            }
        }

        public async Task<bool> StopDistributionAsync(string name)
        {
            try
            {
                await ExecuteCommandAsync("wsl.exe", $"--terminate {name}");
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка при остановке дистрибутива {name}: {ex.Message}");
            }
        }

        public async Task<bool> SetDefaultVersionAsync(int version)
        {
            try
            {
                await ExecuteCommandAsync("wsl.exe", $"--set-default-version {version}");
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка при установке версии WSL по умолчанию: {ex.Message}");
            }
        }

        private async Task<string> ExecuteCommandAsync(string command, string arguments)
        {
            using var process = new Process();
            process.StartInfo = new ProcessStartInfo
            {
                FileName = command,
                Arguments = arguments,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true,
                StandardOutputEncoding = Encoding.UTF8,
                StandardErrorEncoding = Encoding.UTF8
            };

            var output = new StringBuilder();
            var error = new StringBuilder();

            process.OutputDataReceived += (sender, e) =>
            {
                if (e.Data != null)
                {
                    output.AppendLine(e.Data);
                }
            };

            process.ErrorDataReceived += (sender, e) =>
            {
                if (e.Data != null)
                {
                    error.AppendLine(e.Data);
                }
            };

            try
            {
                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
                await process.WaitForExitAsync();

                if (process.ExitCode != 0)
                {
                    throw new Exception($"Команда завершилась с ошибкой (код {process.ExitCode}): {error}");
                }

                return output.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка выполнения команды: {ex.Message}");
            }
        }

        public void Dispose()
        {
            if (!isDisposed)
            {
                isDisposed = true;
                GC.SuppressFinalize(this);
            }
        }
    }

    public class WSLDistribution
    {
        public string Name { get; set; } = "";
        public string State { get; set; } = "";
        public string Version { get; set; } = "";
    }
} 