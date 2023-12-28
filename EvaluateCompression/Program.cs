using System.Reflection;
using CompressionLib;

namespace EvaluateCompression
{
    public class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine("Usage: EvaluateCompression.exe zippedFileName");
                return;
            }

            var referenceZippedFileName = args[0];

            var currentDirectory = Directory.GetCurrentDirectory();
            var referenceDecompressedFileName = Path.Combine(currentDirectory, "reference.txt");
            var compressedFileName = Path.Combine(currentDirectory, "compressed.txt");
            var decompressedFileName = Path.Combine(currentDirectory, "decompressed.txt");

            PrepareUncompressedFile(referenceZippedFileName, referenceDecompressedFileName);

            var compressionInstances = CompressionInstances();
            var results = CompressAndUncompress(compressionInstances, referenceDecompressedFileName, compressedFileName, decompressedFileName);
            
            if (File.Exists(referenceDecompressedFileName))
                File.Delete(referenceDecompressedFileName);
            
            PrintResults(results);   
        }

        private static void PrepareUncompressedFile(string zippedFileName, string uncompressedReferenceFileName)
        {
            if (!File.Exists(zippedFileName))
            {
                Console.WriteLine("The zipped reference file {0} does not exist.", zippedFileName);
                return;
            }   
            
            GZipReference gZipReference = new();
            gZipReference.Decompress(zippedFileName, uncompressedReferenceFileName);
        }

        private static void PrintResults(List<(string ClassName, long CompressedFileSize)> results)
        {
            // Sort results by compressed file size in ascending order
            var sortedResults = results.OrderBy(r => r.CompressedFileSize).Reverse().ToList();

            // Get the best (smallest) compressed file size
            var bestFileSize = sortedResults.First().CompressedFileSize;

            // Print the header
            Console.WriteLine($"{"Class Name",20} | {"File Size",-15} | {"Times Smaller",13}");

            // Print the results with file sizes and times smaller information rounded to one decimal point
            foreach (var (className, compressedFileSize) in sortedResults)
            {
                // Calculate how many times smaller the current result is compared to the best
                var timesSmaller = Math.Round((double)bestFileSize / compressedFileSize, 1);

                // Print the result with file size and times smaller information
                Console.WriteLine($"{className,-20} | {compressedFileSize,-15} | {timesSmaller,13}x smaller");
            }
        }

        private static List<(string ClassName, long CompressedFileSize)> CompressAndUncompress(
            List<ICompression> compressionInstances, string referenceDecompressedFile,
            string compressedFileName, string decompressedFileName)
        {
            List<(string ClassName, long CompressedFileSize)> results = new();
            
            // get reference file size
            var uncompressedFileSize = new FileInfo(referenceDecompressedFile).Length;
            results.Add(("Uncompressed", uncompressedFileSize));

            // Call the Compress and Decompress methods for each instance
            foreach (var compressionInstance in compressionInstances)
            {
                string className;
                try
                {
                    compressionInstance.Compress(referenceDecompressedFile, compressedFileName);

                    // Get the size of the compressed file
                    var compressedFileSize = new FileInfo(compressedFileName).Length;

                    // Get the class name of the compression instance
                    className = compressionInstance.GetType().Name;

                    // Add the results to the list
                    results.Add((className, compressedFileSize));
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    goto cleanupCode;
                }

                try
                {
                    compressionInstance.Decompress(compressedFileName, decompressedFileName);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }

                if (!AssessCorrectness(referenceDecompressedFile, decompressedFileName))
                {
                    Console.WriteLine("{0}: The original file and the decompressed file are not identical.", className);
                }
                
                cleanupCode:
                
                // Clean up: Delete the temporary files
                if (File.Exists(compressedFileName))
                    File.Delete(compressedFileName);
                
                if (File.Exists(decompressedFileName))
                    File.Delete(decompressedFileName);
            }

            return results;
        }

        private static List<ICompression> CompressionInstances()
        {
            // Assuming YourClassLibrary is the name of your class library project
            var assembly = Assembly.Load("CompressionLib");

            // Get all types in the specified assembly that implement ICompression
            var compressionTypes = assembly.GetTypes()
                .Where(t => t.GetInterfaces().Contains(typeof(ICompression)) && t.IsClass);

            // Create instances of the classes implementing ICompression
            List<ICompression> compressionInstances = compressionTypes
                .Select(t => (ICompression)Activator.CreateInstance(t))
                .ToList();

            return compressionInstances;
        }


        private static bool AssessCorrectness(string referenceDecompressedFile, string decompressedFileName)
        {
            // Compare the original file with the decompressed file after sorting the lines
            var originalFile = File.ReadAllLines(referenceDecompressedFile).OrderBy(line => line);
            var decompressedFile = File.ReadAllLines(decompressedFileName).OrderBy(line => line);

            // Find the index of the first mismatched line
            using var originalEnumerator = originalFile.GetEnumerator();
            using var decompressedEnumerator = decompressedFile.GetEnumerator();
            var lineNumber = 1; // 1-based index
            while (originalEnumerator.MoveNext() && decompressedEnumerator.MoveNext())
            {
                var originalLine = originalEnumerator.Current;
                var decompressedLine = decompressedEnumerator.Current;
                    
                if (originalLine != decompressedLine)
                {
                    Console.WriteLine("Sorted Line: {0} mismatch. Ref: {1}  Comp/Decomp: {2}", lineNumber, originalLine, decompressedLine);
                    return false;
                }
                lineNumber++;
            }

            return true;
        }

    }
}