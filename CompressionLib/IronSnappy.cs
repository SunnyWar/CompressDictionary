using System.IO.Compression;
using IronCompress;

namespace CompressionLib
{
    public class IronSnappy : ICompression
    {
        public void Compress(string unCompressedFileName, string compressedFileName)
        {
            var input = File.ReadAllBytes(unCompressedFileName);
            var iron = new Iron();

            // Assuming iron.Compress returns IronCompressResult
            var compressed = iron.Compress(Codec.Snappy, input.AsSpan(), null, CompressionLevel.SmallestSize);

            using var compressedFileStream = File.Create(compressedFileName);
            
            // Use the implicit conversion to ReadOnlySpan<byte>
            compressedFileStream.Write(compressed);
        }


        public void Decompress(string compressedFileName, string unCompressedFileName)
        {
            var iron = new Iron();

            // Read the compressed data from the file
            var compressedData = File.ReadAllBytes(compressedFileName);

            // Assuming iron.Decompress returns IronDecompressResult
            var decompressed = iron.Decompress(Codec.Snappy, compressedData.AsSpan());

            // Write the uncompressed data to a new file
            File.WriteAllBytes(unCompressedFileName, decompressed.AsSpan().ToArray());
        }
    }
}
