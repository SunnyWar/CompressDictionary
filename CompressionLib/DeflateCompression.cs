﻿using System.IO.Compression;

namespace CompressionLib
{
    public class DeflateReference : ICompression
    {
        public void Compress(string unCompressedFileName, string compressedFileName)
        {
            using var originalFileStream = File.Open(unCompressedFileName, FileMode.Open);
            using var compressedFileStream = File.Create(compressedFileName);
            using var compressor = new DeflateStream(compressedFileStream, CompressionLevel.SmallestSize);
            originalFileStream.CopyTo(compressor);
        }

        public void Decompress(string compressedFileName, string unCompressedFileName)
        {
            using var compressedFileStream = File.Open(compressedFileName, FileMode.Open);
            using var outputFileStream = File.Create(unCompressedFileName);
            using var decompressor = new DeflateStream(compressedFileStream, CompressionMode.Decompress);
            decompressor.CopyTo(outputFileStream);
        }
    }
}