using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SeaHatsExternal.Quests;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;
using static UoFiddler.Plugin.FontSeaHats.UserControls.QuestControl;

namespace UoFiddler.Plugin.FontSeaHats.UserControls
{
    public partial class Parameters : UserControl
    {
        public Parameters()
        {
            InitializeComponent();
        }
        QuestSeaHatsManager _manager { get; set; } = null;
        Form _parent { get; set; } = null;
        short _stepId { get; set; } = -1;
        ushort _questId { get; set; } = 0;
        IStepDataContainer _stepDataContainer { get; set; } = null;
        public event EventHandler<StepCreatedEventArgs> StepCreated;
        int _idCreated { get; set; } = 0;

        Dictionary<string, string> propertyValues = new Dictionary<string, string>();

        public void Initialize(Form thisForm, SeaHatsExternal.Quests.QuestSeaHatsManager manager, short step, ushort questId, QuestType_T containerType, int id, IStepDataContainer toCopy = null)
        {
            _manager = manager;
            _parent = thisForm;
            _stepId = step;
            _questId = questId;
            _idCreated = id;

            if ((int)containerType < 1 && toCopy is null)
            {
                throw new Exception("No valid container type");
            }

            if (toCopy is not null)
            {
                _stepDataContainer = _manager.CreateContainer(toCopy.Type);
            }
            else
                _stepDataContainer = _manager.CreateContainer(containerType);

            var props = _stepDataContainer.GetType().GetProperties();

            int x = 10;
            int y = 10;

            foreach (var prop in props)
            {
                if (prop.Name == "Completed" || prop.Name == "Type")
                {
                    continue;
                }

                Label name = new Label()
                {
                    Text = $"{prop.Name}",
                    Location = new Point(x, y)
                };

                TextBox value = new TextBox()
                {
                    Location = new Point(x + name.Width + 10, y),
                    Name = prop.Name
                };


                if (toCopy is not null)
                {
                    value.Text = prop.GetValue(toCopy).ToString();
                    propertyValues[prop.Name] = value.Text;
                }

                value.TextChanged += (s, e) =>
                {
                    var textBox = s as TextBox;
                    if (textBox != null)
                    {
                        propertyValues[prop.Name] = textBox.Text;
                    }

                    //prop.SetValue(_stepDataContainer, value.Text);
                };

                y += 30;

                this.Controls.Add(name);
                this.Controls.Add(value);
            }
        }


        public void ClearData()
        {
            foreach (var control in Controls)
            {
                if (control is TextBox tx)
                {
                    tx.Dispose();
                }
            }

            Controls.Clear();
        }
        private void AcceptBtn_Click(object sender, EventArgs e)
        {
            ClearData();

            var props = _stepDataContainer.GetType().GetProperties();

            foreach (var prop in props)
            {
                if (prop.Name == "Completed" || prop.Name == "Type")
                {
                    continue;
                }

                prop.SetValue(_stepDataContainer, propertyValues[prop.Name]);
            }

            StepCreated?.Invoke(this, new StepCreatedEventArgs() { Container = _stepDataContainer, IdCreated = _idCreated });
            _parent.Close();

        }

        private void CloseBtn_Click(object sender, EventArgs e)
        {
            ClearData();
            _parent.Close();
        }
    }
}
