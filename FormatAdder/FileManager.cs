using System.Diagnostics;

namespace FormatAdder;

/// <summary>
/// Manages file operations including backups and opening files for editing
/// </summary>
public class FileManager
{
    public async Task CreateBackupAsync(string filePath)
    {
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"File not found: {filePath}");
        }

        var backupPath = $"{filePath}.backup.{DateTime.Now:yyyyMMdd_HHmmss}";
        
        // Use File.Copy instead of File.CopyToAsync which doesn't exist
        File.Copy(filePath, backupPath);
        
        Console.WriteLine($"📦 Backup created: {Path.GetFileName(backupPath)}");
        
        // Add small delay to ensure operation completes
        await Task.Delay(100);
    }

    public async Task OpenFileAsync(string filePath)
    {
        try
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = filePath,
                    UseShellExecute = true
                }
            };
            
            process.Start();
            await Task.Delay(1000); // Give the process time to start
        }
        catch (Exception ex)
        {
            Console.WriteLine($"⚠️  Could not open file automatically: {ex.Message}");
            Console.WriteLine($"📁 Please open manually: {filePath}");
        }
    }

    public async Task WriteFileAsync(string filePath, string content)
    {
        await File.WriteAllTextAsync(filePath, content);
    }

    public async Task<string> ReadFileAsync(string filePath)
    {
        return await File.ReadAllTextAsync(filePath);
    }
}
