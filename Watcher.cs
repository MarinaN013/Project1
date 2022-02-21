using System;
using System.Diagnostics;
using System.Threading;

namespace ProcessWatcher
{
    /// <summary>
    /// Сущность, следящая за процессом и убивающая его
    /// </summary>
    public class Watcher
    {
        //Имя процесса, за которым нужно следить
        private readonly string processName;
        //Все процессы
        private Process[] processes;
        //Время допустимой жизни (в минутах)
        private readonly byte aliveTime;
        //Интервал проверки (в минутах)
        private readonly byte checkTime;

        /// <param name="processName">Имя процесса, за которым нужно следить</param>
        /// <param name="aliveTime">Время допустимой жизни (в минутах)</param>
        /// <param name="checkTime">Интервал проверки (в минутах)</param>
        public Watcher(string processName = "Spotify", string aliveTime = "1", string checkTime = "1")
        {
            this.processName = processName;

            bool convertResult1 = byte.TryParse(aliveTime, out this.aliveTime);
            bool convertResult2 = byte.TryParse(checkTime, out this.checkTime);

            if (convertResult1 && convertResult2)
            {
                WriteToConsole("Наблюдение запущено успешно.");
                Watch();
            }
            else
            {
                WriteToConsole("Введены некорректные данные.");
                WriteToConsole("Нажмите Esc для выхода");
                if (Console.ReadKey(true).Key == ConsoleKey.Escape) 
                {
                    Process.GetCurrentProcess().Kill(); 
                }
            }
        }

        //Для записи логов в консоль
        private void WriteToConsole(string text)
        {
            Console.WriteLine("{0:t}\t{1}", DateTime.Now, text);
        }

        //Наблюдение
        private void Watch()
        {
            try
            {
                processes = Process.GetProcessesByName(processName);
                if (processes.Length == 0)
                    WriteToConsole("Процесс не найден.");
                byte watchingMinutes = 0;

                while (processes.Length != 0)
                {
                    if (watchingMinutes >= aliveTime)
                    {
                        Kill();
                        WriteToConsole("Процесс завершен принудительно.");
                        break;
                    }

                    Thread.Sleep(60000 * checkTime);
                    watchingMinutes++;
                    WriteToConsole(string.Format("Процесс в работе уже {0} из {1} минут.", watchingMinutes, aliveTime));
                    processes = Process.GetProcessesByName(processName);
                }
            }
            catch (Exception ex)
            {
                WriteToConsole(string.Format("Исключение при наблюдении: {0}", ex.Message));
            }
            finally
            {
                WriteToConsole("Наблюдение завершено");
                WriteToConsole("Нажмите Esc для выхода");
                if (Console.ReadKey(true).Key == ConsoleKey.Escape) { Process.GetCurrentProcess().Kill(); }                    
            }
        }

        //Завершение процесса
        private void Kill()
        {
            foreach (Process item in processes)
            {
                item.Kill();
            }
        }
    }
}
