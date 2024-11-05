// /***************************************************************************
//  *
//  * $Author: Turley
//  * 
//  * "THE BEER-WARE LICENSE"
//  * As long as you retain this notice you can do whatever you want with 
//  * this stuff. If we meet some day, and you think this stuff is worth it,
//  * you can buy me a beer in return.
//  *
//  ***************************************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace UoFiddler.Plugin.FontSeaHats.QuestSH
{
    public class AddResult
    {
        public bool Success { get; set; } = false;
        public QuestDataInfo QuestData { get; set; } = null;
    }

    public class QuestSeaHatsManager
    {
        public event EventHandler<AddResult> ChangeDataRequest;
        public IReadOnlyDictionary<ushort, QuestDataInfo> Quest => _questDatas;
        private Dictionary<ushort, QuestDataInfo> _questDatas { get; set; } = new Dictionary<ushort, QuestDataInfo>();
        public string FileName => "Quest.JSON";
        public QuestSeaHatsManager(string path)
        {
            if (!Directory.Exists(path))
            {
                MessageBox.Show($"Path is not exist: {path}");
            }

            string file = Path.Combine(path, FileName);

            if (!File.Exists(file))
            {
                File.Create(file).Close();
                MessageBox.Show($"File is created: {file}");
            }
        }

        public QuestDataInfo GetQuest(string title)
        {
            var result = Quest.Where(t => t.Value.QuestName == title).FirstOrDefault();

            return result.Value;
        }

        public ushort GetFreeId()
        {
            ushort result = 0;
            try
            {
                result = (ushort)(_questDatas.Keys.Max() + 1);
            }
            catch(Exception ex)
            {
                result = 1;
            }

            return result;
        }

        public void ChangeTitleName(string title, ushort index)
        {




        }

        public bool AddStep(ushort questIndex)
        {
            if(_questDatas.TryGetValue(questIndex, out var questData))
            {
                questData.AddStep(new QuestDataStep(questData));

                ChangeDataRequest.Invoke(this, new AddResult() {QuestData = questData});

                return true;
            }

            return false;
        }

        public AddResult AddQuest()
        {
            ushort newId = GetFreeId();

            QuestDataInfo questDataInfo = new QuestDataInfo()
            {
                ID = newId
            };

            if (_questDatas.TryAdd(newId, questDataInfo))
            {
                return new AddResult()
                {
                    Success = true,
                    QuestData = questDataInfo
                };
            }

            return null;
        }
    }
}
