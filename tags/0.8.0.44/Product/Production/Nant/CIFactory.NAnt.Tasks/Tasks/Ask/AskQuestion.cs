using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace CIFactory.NAnt.Tasks.Ask
{
    public partial class AskQuestion : Form
    {
        public AskQuestion()
        {
            InitializeComponent();
        }

        private List<RadioButton> _RadioButtons;
        public List<RadioButton> RadioButtons
        {
            get
            {
                if (_RadioButtons == null)
                {
                    _RadioButtons = new List<RadioButton>();
                    _RadioButtons.Add(this.radioButton1);
                    _RadioButtons.Add(this.radioButton2);
                    _RadioButtons.Add(this.radioButton3);
                    _RadioButtons.Add(this.radioButton4);
                    _RadioButtons.Add(this.radioButton5);
                    _RadioButtons.Add(this.radioButton6);
                }
                return _RadioButtons;
            }
            set
            {
                _RadioButtons = value;
            }
        }

        private int _AvailibleIndex = 0;
        public int AvailibleIndex
        {
            get
            {
                return _AvailibleIndex;
            }
            set
            {
                _AvailibleIndex = value;
            }
        }

        public void AddChoice(string choice)
        {
            this.RadioButtons[this.AvailibleIndex].Text = choice;
            this.RadioButtons[this.AvailibleIndex].Visible = true;
            this.AvailibleIndex += 1;
        }

        public void StateQuestion(string question)
        {
            this.textBox1.Text = question;
            this.textBox1.Select(0, 0);
        }

        public void SetCaption(string caption)
        {
            this.Text = caption;
        }

        public string GetAnswer()
        {
            foreach (RadioButton Choice in this.RadioButtons)
            {
                if (Choice.Checked)
                    return Choice.Text;
            }
            return string.Empty;
        }
    }
}