using System;

namespace PoorManDI_MultiArgsConstructorSupports
{
    public class FileMessage : IMessage
    {
        public void Write(string message)
        {
            Console.WriteLine($"訊息: {message} 已經寫入到檔案上了");
        }
    }
}
