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
    public partial class MultipleControl : BaseQuestionControl
    {
        // ⭐ 1. 옵션 관리를 위한 필드 및 상수 정의
        private List<UserControl> optionControls = new List<UserControl>();
        private OptionType currentOptionType = OptionType.None;

        private enum OptionType { None, Single, Multiple }

        private const int MAX_OPTIONS = 6;
        private const int OPTION_SPACING = 5;
        // 문제 입력란(Problem)과 버튼들(y=64)을 고려하여, 옵션 컨트롤이 시작될 Y 좌표를 계산합니다.
        // (버튼 하단 64 + 34) + 여유 공간 약 10 픽셀
        private const int INITIAL_OPTION_Y = 50;
        private const int INITIAL_BUTTON_Y = 64; // 옵션이 없을 때 버튼의 원래 Y 위치

        // ⭐ [추가]: 단일 선택 RadioButton들을 저장할 리스트 추가
        private List<RadioButton> singleRadioButtons = new List<RadioButton>();


        // 📌 1. 사용자 정의 이벤트 선언: 폼에게 높이 변경을 알립니다.
        public event EventHandler HeightChanged;

        public MultipleControl()
        {
            InitializeComponent();
        }

        public override string NumText
        {
            get { return Num.Text; }
            set { Num.Text = value; }
        }


        // ⭐ 2. 단일 선택 버튼 클릭 이벤트 핸들러 (SingleBtn에 연결될 메서드)
        private void SingleOptionAddButton_Click(object sender, EventArgs e)
        {
            // 현재 옵션 타입이 설정되어 있지 않거나 단일 선택 타입인 경우에만 진행
            if (currentOptionType == OptionType.None || currentOptionType == OptionType.Single)
            {
                currentOptionType = OptionType.Single;
                AddOptionControl(currentOptionType);
            }
        }

        // ⭐ 3. 복수 선택 버튼 클릭 이벤트 핸들러 (OptionAddButton_Click에 연결됨)
        // 기존 OptionAddButton_Click을 대신하거나, 해당 메서드 내에서 AddOptionControl을 호출하도록 수정합니다.
        // 여기서는 OptionAddButton_Click이 복수 선택을 담당한다고 가정하고, 타입 설정만 추가합니다.
        private void OptionAddButton_Click(object sender, EventArgs e)
        {
            // 현재 옵션 타입이 설정되어 있지 않거나 복수 선택 타입인 경우에만 진행
            if (currentOptionType == OptionType.None || currentOptionType == OptionType.Multiple)
            {
                currentOptionType = OptionType.Multiple;
                AddOptionControl(currentOptionType);
            }
            // 기존에 있던 this.Height += 50; 로직은 AddOptionControl 내 RelayoutOptions에서 처리됩니다.
        }


        // ⭐ 4. 옵션 추가 및 관리의 핵심 로직 (RadioButton 동적 생성 로직 추가)
        private void AddOptionControl(OptionType type)
        {
            // 1. 최대 개수 확인
            if (optionControls.Count >= MAX_OPTIONS)
            {
                return;
            }

            // 2. 옵션 타입에 따라 컨트롤 인스턴스 생성 (Multiple, Single 클래스가 존재해야 함)
            UserControl newOptionControl = (type == OptionType.Multiple) ?
                (UserControl)new Multiple() : (UserControl)new Single();

            if (newOptionControl == null) return;
            
            // ⭐ [추가]: 단일 선택일 경우 RadioButton을 MultipleControl에 직접 생성
            if (type == OptionType.Single)
            {
                RadioButton newRadio = new RadioButton();
                newRadio.AutoSize = true;
                
                // UserControl과 RadioButton을 연결하기 위해 Tag 사용
                newRadio.Tag = newOptionControl; 
                
                // MultipleControl에 RadioButton 추가 (하나의 그룹으로 묶기 위함)
                this.Controls.Add(newRadio); 
                singleRadioButtons.Add(newRadio); // 리스트에도 추가
            }


            // 3. 버튼 관리: 첫 옵션 추가 시 다른 타입 버튼 숨김
            if (optionControls.Count == 0)
            {
                if (type == OptionType.Multiple) SingleBtn.Visible = false;
                else MultipleBtn.Visible = false;
            }

            // 4. 이벤트 연결: 옵션 컨트롤 내부의 삭제 버튼(Del) 클릭 이벤트를 OptionDel_Click에 연결
            if (newOptionControl is Multiple multiOption)
            {
                multiOption.Del.Click += OptionDel_Click;
            }
            else if (newOptionControl is Single singleOption)
            {
                singleOption.Del.Click += OptionDel_Click;
            }

            // 5. 리스트 및 컨트롤에 추가
            optionControls.Add(newOptionControl);
            this.Controls.Add(newOptionControl);

            // 6. 위치 및 번호 재정렬 및 높이 조정
            RelayoutOptions();

            // 7. 최대 개수 초과 시 추가 버튼 숨기기
            if (optionControls.Count >= MAX_OPTIONS)
            {
                MultipleBtn.Visible = false;
                SingleBtn.Visible = false;
            }

            // 8. 부모 폼에 높이 변경 알림
            HeightChanged?.Invoke(this, EventArgs.Empty);
        }

        // ⭐ 5. 옵션 삭제 로직 (RadioButton 제거 로직 추가)
        private void OptionDel_Click(object sender, EventArgs e)
        {
            Button delButton = sender as Button;
            if (delButton == null) return;

            // 삭제 버튼의 부모(옵션 UserControl)를 찾습니다.
            UserControl controlToDelete = delButton.Parent as UserControl;
            if (controlToDelete == null) return;

            // ⭐ [추가]: Single 타입일 경우, 연결된 RadioButton도 제거
            if (controlToDelete is Single)
            {
                // Tag를 사용하여 연결된 RadioButton을 찾습니다.
                RadioButton radioToDelete = singleRadioButtons.FirstOrDefault(r => r.Tag == controlToDelete);
                
                if (radioToDelete != null)
                {
                    this.Controls.Remove(radioToDelete);
                    singleRadioButtons.Remove(radioToDelete);
                    radioToDelete.Dispose();
                }
            }
            
            // 1. 리스트와 컨트롤에서 제거 및 리소스 해제
            this.Controls.Remove(controlToDelete);
            optionControls.Remove(controlToDelete);
            controlToDelete.Dispose();

            // 2. 위치, 번호, 높이 재정렬 및 버튼 이동
            RelayoutOptions();

            // 3. 버튼 관리: 옵션이 0개가 되면 두 버튼 모두 다시 표시하고 타입 초기화
            if (optionControls.Count == 0)
            {
                MultipleBtn.Visible = true;
                SingleBtn.Visible = true;
                currentOptionType = OptionType.None;
            }
            // 4. 버튼 관리: 옵션이 6개 미만이 되면 현재 타입의 추가 버튼 다시 표시
            else if (optionControls.Count < MAX_OPTIONS)
            {
                if (currentOptionType == OptionType.Multiple) MultipleBtn.Visible = true;
                else if (currentOptionType == OptionType.Single) SingleBtn.Visible = true;
            }

            // 5. 부모 폼에 높이 변경 알림
            HeightChanged?.Invoke(this, EventArgs.Empty);
        }

        // 📌 Note: 이 메서드는 기존의 Del_Click 메서드와 병합되어야 합니다.
        private void Del_Click(object sender, EventArgs e) // 👈 유저 컨트롤 내의 삭제 버튼 이벤트
        {
            // 1. 이벤트 발생 (부모 폼에게 알림)
            OnDeleteRequested();
        }


        // ⭐ 6. 옵션 재배치, 번호 재부여 및 컨트롤 높이 조정 로직 (RadioButton 위치 조정 로직 추가)
        private void RelayoutOptions()
        {
            int currentY = INITIAL_OPTION_Y;

            // 1. 옵션 컨트롤 배치 및 번호 재부여
            for (int i = 0; i < optionControls.Count; i++)
            {
                UserControl control = optionControls[i];

                // 위치 재설정 (Problem TextBox Left인 70에 맞춤)
                control.Location = new Point(70, currentY);

                // ⭐ [추가]: Single 타입일 경우, 연결된 RadioButton의 위치도 조정
                if (control is Single singleOption)
                {
                    // Tag를 사용하여 연결된 RadioButton을 찾습니다.
                    RadioButton radio = singleRadioButtons.FirstOrDefault(r => r.Tag == singleOption);
                    
                    if (radio != null)
                    {
                        // RadioButton을 Single 옵션 컨트롤의 좌측(x=3), 중앙(y)에 배치
                        radio.Location = new Point(72, currentY + (singleOption.Height / 2) - (radio.Height / 2));
                        radio.BringToFront(); // RadioButton이 항상 위에 오도록
                    }
                }

                // 번호 재부여 (Num Label 업데이트)
                string numText = $"{i + 1}.";
                // Multiple과 Single 컨트롤 내의 Num Label 접근자(Modifier)가 public이어야 합니다.
                if (control is Multiple multiple)
                {
                    multiple.Num.Text = numText;
                }
                else if (control is Single single)
                {
                    single.Num.Text = numText;
                }

                // 다음 컨트롤을 위한 Y 위치 업데이트
                currentY = control.Bottom + OPTION_SPACING;
            }

            // 2. ⭐ 추가 버튼들 (MultipleBtn, SingleBtn)을 마지막 옵션 아래 또는 초기 위치로 이동
            int buttonY;
            if (optionControls.Count == 0)
            {
                // 옵션이 없으면 버튼을 원래 위치 (Y=64)로 이동
                buttonY = INITIAL_BUTTON_Y;
            }
            else
            {
                // 옵션이 있으면 마지막 옵션 컨트롤 아래로 이동
                buttonY = currentY + 5;
            }

            MultipleBtn.Location = new Point(MultipleBtn.Location.X, buttonY);
            SingleBtn.Location = new Point(SingleBtn.Location.X, buttonY);

            // 3. ⭐ MultipleControl 자체의 높이 조정
            // 컨트롤의 새 높이 = 가장 낮은 컨트롤 (버튼들)의 Bottom + 여유 공간
            int newHeight = SingleBtn.Bottom + 10;

            if (this.Height != newHeight)
            {
                this.Height = newHeight;
            }

            this.PerformLayout();
        }
        
        // 📌 Note: 기존의 OptionAddButton_Click 로직이 복수 선택 기능으로 대체되었습니다.
        private void OptionAddButton_Click_Old(object sender, EventArgs e)
        {
            // 1. 컨트롤의 높이를 늘리는 로직을 수행합니다. (예시: 50픽셀 증가)
            this.Height += 50;

            // 2. 📌 이벤트 발생: 부모 폼에게 높이가 변경되었음을 알립니다.
            //     이 코드를 통해 ProblemForm의 UserControl_OnHeightChanged 메서드가 호출됩니다.
            HeightChanged?.Invoke(this, EventArgs.Empty);
        }




        // ----------------------------------------------------------------------
        // 📌 핵심: 다형성을 위한 GetQuestionData 메서드 재정의 (새로 추가)
        // ----------------------------------------------------------------------
        public override BaseQuestionData GetQuestionData()
        {
            // MultipleQuestionData DTO를 생성합니다.
            var data = new MultipleQuestionData
            {
                ProblemText = Problem.Text,
                // currentOptionType은 MultipleControl의 내부 상태에 따라 설정
                IsSingleChoice = (currentOptionType == OptionType.Single),
                Options = new List<OptionData>()
            };

            // 1. 동적으로 추가된 옵션 컨트롤들을 순회하며 데이터 추출
            foreach (UserControl control in optionControls)
            {
                OptionData option = new OptionData();

                if (currentOptionType == OptionType.Multiple)
                {
                    // 1-1. 복수 선택 옵션 (Multiple.cs)에서 데이터 추출
                    if (control is Multiple multipleControl)
                    {
                        // CheckBox와 TextBox의 이름을 Answer, AnswerTxt로 가정하고 데이터를 가져옵니다.
                        CheckBox answerCheckbox = multipleControl.Controls.OfType<CheckBox>().FirstOrDefault(c => c.Name == "Answer");
                        TextBox answerTextBox = multipleControl.Controls.OfType<TextBox>().FirstOrDefault(c => c.Name == "AnswerTxt");

                        if (answerTextBox != null) option.Text = answerTextBox.Text;
                        if (answerCheckbox != null) option.IsCorrect = answerCheckbox.Checked;

                        data.Options.Add(option);
                    }
                }
                else if (currentOptionType == OptionType.Single)
                {
                    // 1-2. 단일 선택 옵션 (Single.cs)에서 데이터 추출
                    if (control is Single singleControl)
                    {
                        TextBox answerTextBox = singleControl.Controls.OfType<TextBox>().FirstOrDefault(c => c.Name == "AnswerTxt");

                        int index = optionControls.IndexOf(singleControl);

                        if (answerTextBox != null) option.Text = answerTextBox.Text;

                        // singleRadioButtons 리스트는 이미 클래스 필드에 정의되어 있습니다.
                        if (index >= 0 && index < singleRadioButtons.Count)
                        {
                            option.IsCorrect = singleRadioButtons[index].Checked;
                        }

                        data.Options.Add(option);
                    }
                }
            }

            return data;
        }
    }
}