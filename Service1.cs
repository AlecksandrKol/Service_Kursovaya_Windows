using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace ServiceKursovaya
{
    public partial class Service1 : ServiceBase
    {
        private FileSystemWatcher fileWatcher;
        private string lastRenamedMessage;

        public Service1()
        {
            InitializeComponent();
            lastRenamedMessage = null;
        }

        protected override async void OnStart(string[] args)
        {
            // Задаем путь к каталогу для мониторинга
            string directoryPath = @"D:\wiser";

            // Инициализация FileSystemWatcher
            fileWatcher = new FileSystemWatcher();
            fileWatcher.Path = directoryPath;
            fileWatcher.Filter = "*.*"; // Мониторим все файлы

            // Подписываемся на события изменения файлов
            fileWatcher.Created += OnFileChanged;
            fileWatcher.Deleted += OnFileChanged;
            fileWatcher.Changed += OnFileChanged;
            fileWatcher.Renamed += OnFileRenamed;

            // Включаем мониторинг
            fileWatcher.EnableRaisingEvents = true;

            string logMessage = $"Мониторинг каталога {directoryPath} запущен.";
            EventLog.WriteEntry("ServiceKursovaya", logMessage);
            AppendLogToFile(logMessage);
            

        }
        protected override async void OnStop()
        {
            string logMessage = "Служба остановлена.";
            EventLog.WriteEntry("ServiceKursovaya", logMessage);
            AppendLogToFile(logMessage);

            // Выключаем мониторинг
            fileWatcher.EnableRaisingEvents = false;
        }
        private void OnFileChanged(object sender, FileSystemEventArgs e)
        {
            // Код, который будет выполняться при создании, удалении или изменении файла
            string changeType = e.ChangeType.ToString();
            string fileName = Path.GetFileNameWithoutExtension(e.Name);
            string logMessage = $"{changeType}: Файл \"{fileName}\": Log в каталоге {fileWatcher.Path}";
            // Проверяем, было ли уже отображено сообщение о переименовании
            if (lastRenamedMessage != logMessage && !logMessage.Contains("Changed"))
            {
                EventLog.WriteEntry("ServiceKursovaya", logMessage);
                AppendLogToFile(logMessage);
                lastRenamedMessage = logMessage;
            }
            else
            {
                // Если сообщение о переименовании уже было отображено или содержит "Changed", устанавливаем lastRenamedMessage в null
                lastRenamedMessage = null;
            }
            
        }

        private void OnFileRenamed(object sender, RenamedEventArgs e)
        {
            // Код, который будет выполняться при переименовании файла
            string oldFileName = Path.GetFileNameWithoutExtension(e.OldName);
            string newFileName = Path.GetFileNameWithoutExtension(e.Name);
            string logMessage = $"Файл переименован: {oldFileName} => {newFileName}";

            // Проверяем, было ли уже отображено сообщение о переименовании
            if (lastRenamedMessage != logMessage && !logMessage.Contains("Changed"))
            {
                EventLog.WriteEntry("ServiceKursovaya", logMessage);
                AppendLogToFile(logMessage);
                lastRenamedMessage = logMessage;
            }
            else
            {
                // Если сообщение о переименовании уже было отображено или содержит "Changed", устанавливаем lastRenamedMessage в null
                lastRenamedMessage = null;
            }
        }
        private void AppendLogToFile(string logMessage)
        {
            string logFilePath = "D:\\wiser\\Log.txt";
            File.AppendAllText(logFilePath, $"{DateTime.Now}: {logMessage}\n");
        }
    }
}
