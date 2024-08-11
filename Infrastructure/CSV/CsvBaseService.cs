using System.Collections;
using System.Globalization;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;

namespace Infrastructure.CSV;

/// <summary>
/// Represents the CSV base service class.
/// </summary>
public sealed class CsvBaseService
{
    private readonly CsvConfiguration _csvConfiguration;

    /// <summary>
    /// Initialize the new instance of <see cref="CsvBaseService"/> class.
    /// </summary>
    public CsvBaseService()
    {
        _csvConfiguration = GetConfiguration();
    }
    
    /// <summary>
    /// Upload the file with specifier data.
    /// </summary>
    /// <param name="data">The common enumerable.</param>
    /// <returns>Returns the array of bytes.</returns>
    public async Task<byte[]> UploadFile(IEnumerable data)
    {
        using var memoryStream = new MemoryStream();
        await using var streamWriter = new StreamWriter(memoryStream);
        await using var csvWriter = new CsvWriter(streamWriter, _csvConfiguration);
        
        await csvWriter.WriteRecordsAsync(data);
        await streamWriter.FlushAsync();
        return memoryStream.ToArray();
    }
    
    private CsvConfiguration GetConfiguration()
    {
        return new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            Delimiter = ";",
            Encoding = Encoding.UTF8,
            NewLine = "\r\n"
        };
    }
}