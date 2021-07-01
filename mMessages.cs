using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExRules
{
    
    class MMessages
    {
        /// <summary>
        /// Простое сообщение.
        /// </summary>
        /// <param name="TextMessage">Текст сообщения.</param>
        public void SimpleMessage(string TextMessage)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(TextMessage);

        }
        /// <summary>
        /// Положительное сообщение.
        /// </summary>
        /// <param name="TextMessage">Текст сообщения.</param>
        public void GoodMessage(string TextMessage)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(TextMessage);
            Console.ForegroundColor = ConsoleColor.Yellow;
        }
        /// <summary>
        /// Сообщение об ошибке.
        /// </summary>
        /// <param name="TextMessage">Текст сообщения.</param>
        public void ErrorMessage(string TextMessage)
        {
            //Console.Beep();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(TextMessage);
            Console.ForegroundColor = ConsoleColor.Yellow;
        }
    }


}
