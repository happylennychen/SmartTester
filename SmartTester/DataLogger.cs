using System.IO;
using System.Threading.Tasks;

namespace SmartTester
{
    public class DataLogger : IDataLogger
    {
        public string FilePath { get; set; }
        public int Id { get; set; }
        private int bufferSize { get; set; }

        private FileStream fileStream;
        private StreamWriter streamWriter;
        public DataLogger(int id, string filePath)
        {
            this.Id = id;
            this.FilePath = filePath;
            fileStream = new FileStream(FilePath, FileMode.Create);
            streamWriter = new StreamWriter(fileStream);
        }

        public void AddData(string log)
        {
            Task t1 = WriteData(log);
            bufferSize++;
            if (bufferSize >= 20)
            {
                t1.Wait();
                Task t2 = FlushData();
                bufferSize = 0;
            }
        }

        public void Flush()
        {
            Task t = FlushData();
        }

        private async Task WriteData(string log)
        {
            try
            {
                await streamWriter.WriteAsync(log);
                //await streamWriter.FlushAsync();
            }
            catch
            {
            }
        }

        private async Task FlushData()
        {
            try
            {
                await streamWriter.FlushAsync();
            }
            catch
            {
            }
        }

        public void Close()
        {
            Task task = CloseDataLogger();
        }

        private async Task CloseDataLogger()
        {
            await streamWriter.FlushAsync();
            streamWriter.Close();
            fileStream.Close();
        }
    }
}