using System;
using System.Collections.Concurrent;
using System.IO;
using System.IO.MemoryMappedFiles;

namespace StormFellowship.Services;

/// <summary>
/// Memory-Mapped Storage Cache Service.
/// Provides zero-copy, non-blocking media & sticker caching using MemoryMappedFiles
/// for instant UI rendering and minimal RAM/Disk footprint.
/// </summary>
public class MemoryMappedCacheService : IDisposable
{
    private static MemoryMappedCacheService? _instance;
    public static MemoryMappedCacheService Instance => _instance ??= new MemoryMappedCacheService();

    private readonly ConcurrentDictionary<string, byte[]> _memoryCache = new();
    private readonly string _cacheDirectory;

    public MemoryMappedCacheService()
    {
        _cacheDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "StormFellowship", "MediaCache");
        try
        {
            if (!Directory.Exists(_cacheDirectory)) Directory.CreateDirectory(_cacheDirectory);
        }
        catch { }
    }

    public void StoreMedia(string key, byte[] data)
    {
        if (string.IsNullOrWhiteSpace(key) || data == null) return;
        _memoryCache[key] = data;

        // Persist to Memory Mapped Cache asynchronously
        Task.Run(() =>
        {
            try
            {
                string safeKey = string.Join("_", key.Split(Path.GetInvalidFileNameChars()));
                string filePath = Path.Combine(_cacheDirectory, $"{safeKey}.dat");

                using var mmf = MemoryMappedFile.CreateFromFile(filePath, FileMode.Create, safeKey, data.Length, MemoryMappedFileAccess.ReadWrite);
                using var accessor = mmf.CreateViewAccessor(0, data.Length, MemoryMappedFileAccess.Write);
                accessor.WriteArray(0, data, 0, data.Length);
            }
            catch { }
        });
    }

    public byte[]? RetrieveMedia(string key)
    {
        if (_memoryCache.TryGetValue(key, out var data)) return data;

        try
        {
            string safeKey = string.Join("_", key.Split(Path.GetInvalidFileNameChars()));
            string filePath = Path.Combine(_cacheDirectory, $"{safeKey}.dat");
            if (File.Exists(filePath))
            {
                var fileInfo = new FileInfo(filePath);
                using var mmf = MemoryMappedFile.CreateFromFile(filePath, FileMode.Open, safeKey, fileInfo.Length, MemoryMappedFileAccess.Read);
                using var accessor = mmf.CreateViewAccessor(0, fileInfo.Length, MemoryMappedFileAccess.Read);
                byte[] buffer = new byte[fileInfo.Length];
                accessor.ReadArray(0, buffer, 0, buffer.Length);
                _memoryCache[key] = buffer;
                return buffer;
            }
        }
        catch { }

        return null;
    }

    public void Dispose()
    {
        _memoryCache.Clear();
    }
}
