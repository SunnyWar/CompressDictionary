using System.IO.Compression;
using IronCompress;

namespace CompressionLib
{
    public class Snappy_Iron : ICompression
    {
        public void Compress(string unCompressedFileName, string compressedFileName)
        {
            IronPackage.Compress(unCompressedFileName, compressedFileName, Codec.Snappy);
        }

        public void Decompress(string compressedFileName, string unCompressedFileName)
        {
            IronPackage.Decompress(compressedFileName, unCompressedFileName, Codec.Snappy);
        }
    }

    public class Zstd_Iron : ICompression
    {
        public void Compress(string unCompressedFileName, string compressedFileName)
        {
            IronPackage.Compress(unCompressedFileName, compressedFileName, Codec.Zstd);
        }

        public void Decompress(string compressedFileName, string unCompressedFileName)
        {
            IronPackage.Decompress(compressedFileName, unCompressedFileName, Codec.Zstd);
        }
    }

    public class Gzip_Iron : ICompression
    {
        public void Compress(string unCompressedFileName, string compressedFileName)
        {
            IronPackage.Compress(unCompressedFileName, compressedFileName, Codec.Gzip);
        }

        public void Decompress(string compressedFileName, string unCompressedFileName)
        {
            IronPackage.Decompress(compressedFileName, unCompressedFileName, Codec.Gzip);
        }
    }

    public class Brotli_Iron : ICompression
    {
        public void Compress(string unCompressedFileName, string compressedFileName)
        {
            IronPackage.Compress(unCompressedFileName, compressedFileName, Codec.Brotli);
        }

        public void Decompress(string compressedFileName, string unCompressedFileName)
        {
            IronPackage.Decompress(compressedFileName, unCompressedFileName, Codec.Brotli);
        }
    }
    
    public static class IronPackage
    {
        public static void Compress(string unCompressedFileName, string compressedFileName, Codec codec)
        {
            var input = File.ReadAllBytes(unCompressedFileName);
            var iron = new Iron();

            // Assuming iron.Compress returns IronCompressResult
            var compressed = iron.Compress(codec, input.AsSpan(), null, CompressionLevel.SmallestSize);

            using var compressedFileStream = File.Create(compressedFileName);
            
            // Use the implicit conversion to ReadOnlySpan<byte>
            compressedFileStream.Write(compressed);
        }


        public static void Decompress(string compressedFileName, string unCompressedFileName, Codec codec)
        {
            var iron = new Iron();

            // Read the compressed data from the file
            var compressedData = File.ReadAllBytes(compressedFileName);

            // Assuming iron.Decompress returns IronDecompressResult
            var decompressed = iron.Decompress(codec, compressedData.AsSpan());

            // Write the uncompressed data to a new file
            File.WriteAllBytes(unCompressedFileName, decompressed.AsSpan().ToArray());
        }
    }
}
