using PoorManDI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoorManDI_MultiArgsConstructorSupports
{
    // 這個類別用來測試多參數建構式DI
    class CustomMessage : IMessage
    {
        private readonly ConsoleMessage _consoleMessage;
        private readonly FileMessage _fileMessage;
        private readonly string _prefixMessage;

        // 多參數建構式
        public CustomMessage(
            ConsoleMessage consoleMessage,
            FileMessage fileMessage,
            string prefixMessage = "Custom Prefix :" // 預設參數值
        )
        {
            _consoleMessage = consoleMessage;
            _fileMessage = fileMessage;
            _prefixMessage = prefixMessage;
        }

        public void Write(string message)
        {
            Console.WriteLine($"{_prefixMessage}");
            _consoleMessage.Write(message);
            _fileMessage.Write(message);
        }
    }

    class Program
    {
        static void Main(string[] args)
        {

            var diContainer = new MyDIWithNamed(); //new MyDI();

            diContainer.Register<ConsoleMessage, ConsoleMessage>();
            diContainer.Register<FileMessage, FileMessage>();
            diContainer.Register<IMessage, CustomMessage>();

            diContainer.Register<IMessage, FileMessage>(nameof(FileMessage));

            Console.WriteLine("Test IMessage without named:");

            IMessage message = diContainer.Resolve<IMessage>();
            message.Write("Without Named");

            Console.WriteLine("===========================================================");

            Console.WriteLine($"Test IMessage with named {nameof(FileMessage)}:");
            IMessage message1 = diContainer.Resolve<IMessage>(nameof(FileMessage));
            message1.Write("With Named");

            Console.ReadKey(true);
        }
    }
}
