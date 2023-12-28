namespace CompressionLib
{
    public interface ICompression
    {
        void Compress(string unCompressedFileName, string compressedFileName);
        void Decompress(string compressedFileName, string unCompressedFileName);
    }
}
