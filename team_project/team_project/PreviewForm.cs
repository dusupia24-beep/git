using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace team_project
{
    public partial class PreviewForm : Form
    {
        private BaseQuestionData _firstQuestion;
        public PreviewForm(string title, int totalCount, string types, BaseQuestionData questionData)
        {
            InitializeComponent();
            this.PreviewPanel.BorderStyle = BorderStyle.FixedSingle;

            this._firstQuestion = questionData; // 객체 저장

            // 1. 문제 제목 설정
            this.Problem.Text = title;

            // 2. 미리보기 정보 설정
            this.PreviewLabel.Text = $"총 {totalCount}문항 | 유형 : {types}";

            // 3. 1번 문제 내용 설정 (새로운 메서드 호출)
            DisplayQuestionContent();
        }


        private void DisplayQuestionContent()
        {
            if (_firstQuestion == null) return;

            this.PreviewPanel.Controls.Clear();

            // 1. PreviewPanel의 전체 크기를 차지하는 FlowLayoutPanel 생성
            FlowLayoutPanel contentContainer = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                AutoScroll = true,
                Padding = new Padding(10),
                BorderStyle = BorderStyle.None
            };
            this.PreviewPanel.Controls.Add(contentContainer);

            // 2. 문제 텍스트를 담는 Label 생성
            // 폰트 크기 12pt * 2배 = 24pt
            int questionFontSize = 12 * 2;
            Label questionTextDisplay = new Label
            {
                Text = $"1. {_firstQuestion.ProblemText}",
                AutoSize = true, // 폰트 크기에 따라 높이 자동 조절
                Font = new Font("굴림", questionFontSize),
                BorderStyle = BorderStyle.None,

                Width = this.PreviewPanel.ClientSize.Width - 40,
                Margin = new Padding(0, 0, 0, 20) // 문제와 보기 사이 마진 증가 (겹침 방지)
            };
            contentContainer.Controls.Add(questionTextDisplay);

            // 3. 문제 유형에 따른 UI 구현

            int controlWidth = this.PreviewPanel.ClientSize.Width - 40;

            switch (_firstQuestion)
            {
                case OxQuestionData oxData:
                    // 🟥 O/X 문제: RadioButton (AutoCheck=false)
                    // 폰트 크기 14pt * 2배 = 28pt
                    int oxFontSize = 14 * 2;
                    // 라디오 버튼 높이 계산 (폰트 크기에 따라 자동)
                    int radioOxHeight = TextRenderer.MeasureText("O", new Font("굴림", oxFontSize)).Height + 10; // 폰트 높이 + 여백

                    Panel oxPanel = new Panel { Height = radioOxHeight + 10, Width = controlWidth, Margin = new Padding(0) }; // 패널 높이도 조정
                    contentContainer.Controls.Add(oxPanel);

                    RadioButton radioO = new RadioButton
                    {
                        Text = "O",
                        Location = new Point(5, 5),
                        AutoSize = true,
                        Font = new Font("굴림", oxFontSize),
                        AutoCheck = false,
                        ForeColor = SystemColors.ControlText
                    };

                    RadioButton radioX = new RadioButton
                    {
                        Text = "X",
                        Location = new Point(95, 5),
                        AutoSize = true,
                        Font = new Font("굴림", oxFontSize),
                        AutoCheck = false,
                        ForeColor = SystemColors.ControlText
                    };

                    oxPanel.Controls.Add(radioO);
                    oxPanel.Controls.Add(radioX);

                    break;

                case ShortQuestionData shortData:
                    // 🟨 단답형 문제
                    // TextBox 높이 계산 (폰트 크기에 따라 자동)
                    int answerBoxHeight = TextRenderer.MeasureText("답안 입력", new Font("굴림", questionFontSize)).Height + 10; // 폰트 높이 + 여백

                    TextBox answerBox = new TextBox
                    {
                        Text = "답안 입력",
                        Width = controlWidth,
                        Height = answerBoxHeight, // 높이 자동 조절
                        ReadOnly = true,
                        BackColor = SystemColors.Window,
                        Font = new Font("굴림", questionFontSize), // 문제 폰트와 동일하게 24pt
                        Margin = new Padding(0, 10, 0, 10) // 상하 마진 증가
                    };
                    contentContainer.Controls.Add(answerBox);
                    break;

                case MultipleQuestionData multipleData:
                    // 🟦 객관식/복수선택 문제
                    // 폰트 크기 11pt * 2배 = 22pt
                    int optionFontSize = 11 * 2;
                    // 각 옵션의 높이 계산 (폰트 크기에 따라 자동)
                    int optionControlHeight = TextRenderer.MeasureText("1. 보기 텍스트", new Font("굴림", optionFontSize)).Height + 10; // 폰트 높이 + 여백

                    Type controlType = multipleData.IsSingleChoice ? typeof(RadioButton) : typeof(CheckBox);
                    char optionChar = '1';

                    foreach (var option in multipleData.Options)
                    {
                        Control control = (Control)Activator.CreateInstance(controlType);

                        control.Text = $"{optionChar++}. {option.Text}";

                        control.AutoSize = false;
                        control.Width = controlWidth;
                        control.Height = optionControlHeight; // 높이 자동 조절

                        control.Font = new Font("굴림", optionFontSize, FontStyle.Regular);
                        control.ForeColor = SystemColors.ControlText;

                        if (control is CheckBox checkBox)
                        {
                            checkBox.AutoCheck = false;
                        }
                        else if (control is RadioButton radioButton)
                        {
                            radioButton.AutoCheck = false;
                        }

                        control.Margin = new Padding(0, 5, 0, 5); // 마진 조정

                        contentContainer.Controls.Add(control);
                    }
                    break;
            }
        }

        private void Cancel_Click(object sender, EventArgs e)
        {
            this.Close();   
        }

        private void SolveBtn_Click(object sender, EventArgs e)
        {
            QuizForm quizForm = new QuizForm();
            quizForm.ShowDialog();
        }
    }
}
