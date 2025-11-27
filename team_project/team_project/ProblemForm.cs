using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace team_project
{
    public partial class ProblemForm : Form
    {
        // 콤보박스 선택 항목에 따라 생성될 UserControl 클래스 이름이
        // ShortControl, MultipleControl, OxControl 이라고 가정합니다.

        // 📌 상수 정의: 초기 위치와 컨트롤 간의 간격
        private const int INITIAL_TOP = 13;
        private const int INITIAL_LEFT = 18;
        private const int SPACING = 10;

        // 폼 내부에 오토스크롤이 활성화된 패널의 이름이 'panel1'이라고 가정합니다.
        // private System.Windows.Forms.Panel panel1; 
        // -> 디자이너 파일에 이미 선언되어 있으므로 여기서는 사용만 합니다.

        public ProblemForm()
        {
            InitializeComponent();
            this.Type.SelectedIndex = 0;
        }

        private void Cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Create_Click(object sender, EventArgs e)
        {
            Panel targetPanel = this.panel1; // 유저 컨트롤을 추가할 대상 패널 (디자이너에서 선언되어 있다고 가정)
            UserControl newControl = null;

            // 1. ComboBox 선택 값에 따라 생성할 컨트롤 결정
            if (this.Type.SelectedItem == null) return;
            string selectedType = this.Type.SelectedItem.ToString();

            switch (selectedType)
            {
                case "단답형":
                    newControl = new ShortControl(); // ShortControl 클래스가 존재한다고 가정
                    break;
                case "객관형":
                    newControl = new MultipleControl();
                    // 📌 중요: 객관형 컨트롤의 높이 변경 이벤트를 구독하여 레이아웃 재계산 함수와 연결합니다.
                    ((MultipleControl)newControl).HeightChanged += UserControl_OnHeightChanged;
                    break;
                case "O / X":
                    newControl = new OxControl(); // OxControl 클래스가 존재한다고 가정
                    break;
                default:
                    return;
            }

            // 2. 다음 위치 (Y 좌표) 계산
            int nextY = INITIAL_TOP;

            if (targetPanel.Controls.Count > 0)
            {
                // 가장 마지막에 추가된 컨트롤의 하단 위치를 기준으로 다음 위치를 계산
                Control lastControl = targetPanel.Controls[targetPanel.Controls.Count - 1];
                nextY = lastControl.Bottom + SPACING;
            }

            // 3. 위치 설정 및 패널에 추가
            newControl.Location = new Point(INITIAL_LEFT, nextY);


            if (newControl is IQuestionControl questionControl)
            {
                // 현재 패널에 있는 컨트롤의 개수 + 1을 번호로 설정
                int questionNumber = targetPanel.Controls.Count + 1;

                // 텍스트 설정 (예: "1번", "2번" 등)
                questionControl.NumText = $"{questionNumber}.";
            }




            targetPanel.Controls.Add(newControl);


            // 📌 모든 유저 컨트롤의 삭제 이벤트를 구독 (새로 추가/수정된 핵심 로직)
            // ----------------------------------------------------------------------
            if (newControl is MultipleControl mc)
            {
                // 이미 위에서 HeightChanged와 함께 구독되었으므로 DeleteRequested는 다시 구독할 필요 없음
                mc.DeleteRequested += UserControl_OnDeleteRequested;
            }
            else if (newControl is ShortControl sc)
            {
                sc.DeleteRequested += UserControl_OnDeleteRequested;
            }
            else if (newControl is OxControl oc)
            {
                oc.DeleteRequested += UserControl_OnDeleteRequested;
            }
            // ----------------------------------------------------------------------

            // 스크롤을 맨 아래로 이동
            targetPanel.ScrollControlIntoView(newControl);
        }

        // ----------------------------------------------------
        // 레이아웃 관리 메서드
        // ----------------------------------------------------

        /// <summary>
        /// 객관형 컨트롤의 높이가 변했을 때 호출되어 전체 컨트롤의 위치를 재배치합니다.
        /// </summary>
        private void UserControl_OnHeightChanged(object sender, EventArgs e)
        {
            RelayoutControls(this.panel1);
        }

        /// <summary>
        /// 패널 내의 모든 컨트롤을 위에서부터 순차적으로 재배치합니다. (겹침 방지)
        /// </summary>
        private void RelayoutControls(Panel targetPanel)
        {
            int currentY = INITIAL_TOP;
            int spacing = SPACING;



            //현재 스크롤위치 저장
            Point savedScrollPosition;


            // 📌 현재 위치를 저장하는 코드
            Panel targetPanel1 = this.panel1;


            // X와 Y 값을 양수로 변환하여 저장합니다.
            savedScrollPosition = new Point(Math.Abs(targetPanel1.AutoScrollPosition.X),Math.Abs(targetPanel1.AutoScrollPosition.Y));



            targetPanel1.AutoScrollPosition = new Point(0, 0);// 스크롤 위치 맨위로 올려서  공백버그 제거

            // Controls 컬렉션을 순회하며 재배치
            // ----------------------------------------------------------------------
            // [수정]: 번호 재부여를 위해 foreach를 for 루프로 변경했습니다.
            for (int i = 0; i < targetPanel.Controls.Count; i++)
            {
                Control control = targetPanel.Controls[i];

                // 1. 위치 재설정 (기존 로직)
                control.Location = new Point(control.Left, currentY);

                // 2. ⭐ 추가된 로직: 번호 재부여 (NumText 업데이트) ⭐
                // 삭제 후 남은 컨트롤들에 대해 순번(1, 2, 3...)을 다시 매깁니다.
                if (control is IQuestionControl questionControl) // IQuestionControl 인터페이스가 구현되어 있어야 합니다.
                {
                    int questionNumber = i + 1;
                    questionControl.NumText = $"{questionNumber}.";
                }

                // 3. 다음 컨트롤을 위한 Y 위치 업데이트 (기존 로직)
                currentY = control.Bottom + spacing;
            }
            // ----------------------------------------------------------------------

            // 레이아웃 변경 사항을 강제로 적용하여 스크롤이 제대로 동작하게 함
            targetPanel.PerformLayout();
            targetPanel.AutoScrollPosition = savedScrollPosition;// 이전 스크롤 위치 복원
        }

        /// <summary>
        /// 유저 컨트롤로부터 삭제 요청을 받았을 때 실행되는 함수
        /// </summary>
        private void UserControl_OnDeleteRequested(object sender, EventArgs e)
        {
            // 이벤트 발생시킨 객체(유저 컨트롤)를 가져옵니다.
            UserControl controlToDelete = sender as UserControl;

            if (controlToDelete != null && controlToDelete.Parent != null)
            {
                Panel targetPanel = controlToDelete.Parent as Panel;

                // 1. 부모 Controls 컬렉션에서 제거
                controlToDelete.Parent.Controls.Remove(controlToDelete);

                // 2. 리소스 해제
                controlToDelete.Dispose();

                // 3. 📌 중요: 컨트롤이 제거된 후, 나머지 컨트롤들을 위로 당겨 재배치합니다.
                if (targetPanel != null)
                {
                    RelayoutControls(targetPanel);
                }
            }
        }

        private void Save_Click(object sender, EventArgs e)
        {


            string rawName = textBox1.Text.Trim();

            // 1. 제목이 비었는지 체크
            if (string.IsNullOrWhiteSpace(rawName))
            {
                MessageBox.Show("파일 제목을 입력하세요.", "경고",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 2. 파일 이름에 사용할 수 없는 문자 포함 여부 체크
            char[] invalidChars = Path.GetInvalidFileNameChars();
            if (rawName.Any(c => invalidChars.Contains(c)))
            {
                MessageBox.Show("파일 이름에 사용할 수 없는 문자가 포함되어 있습니다.\n" +
                                "사용할 수 없는 문자:  \\ / : * ? \" < > |",
                                "잘못된 파일 이름",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
                return;
            }

            // ⚡ 여기서부터는 파일 이름이 정상일 때 진행
            string fileName = rawName + ".json";



            // 1. 모든 문제 데이터를 담을 리스트 생성
            List<BaseQuestionData> allQuestions = new List<BaseQuestionData>();

            // 2. panel1에 있는 모든 컨트롤을 순회하며 데이터 추출
            // Controls를 역순으로 순회하여 문제 번호 순서(오름차순)대로 데이터를 추출합니다.
            // (컨트롤은 보통 아래에 추가되므로 역순이 번호 순서에 맞을 가능성이 높습니다.)
            // 만약 추가된 순서가 곧 번호 순서(1, 2, 3...)라면 순방향으로 순회합니다.
            // ProblemForm의 RelayoutControls()를 확인했을 때, 컨트롤은 번호가 매겨져 있으므로,
            // Controls를 순서대로 순회하면 됩니다.

            foreach (Control control in panel1.Controls.OfType<BaseQuestionControl>())
            {
                // BaseQuestionControl 타입으로 캐스팅하여 GetQuestionData()를 호출합니다.
                BaseQuestionControl questionControl = (BaseQuestionControl)control;
                BaseQuestionData data = questionControl.GetQuestionData();

                // 문제 번호는 폼에서 관리하는 것이 좋습니다.
                // NumText는 "1." 형태이므로, 숫자만 추출하여 DTO에 저장합니다.
                if (int.TryParse(questionControl.NumText.TrimEnd('.'), out int number))
                {
                    data.Number = number;
                }

                allQuestions.Add(data);
            }

            // 3. (옵션) 유효성 검사 (생략 가능하나 권장)
            if (allQuestions.Count == 0)
            {
                MessageBox.Show("저장할 문제가 없습니다.", "경고", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 4. JSON 직렬화 설정
            var options = new JsonSerializerOptions
            {
                WriteIndented = true, // 가독성을 위해 예쁘게 출력
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping, // 한글 깨짐 방지

                // ⭐ 중요: BaseQuestionData 추상 클래스의 파생 클래스들을 처리할 수 있도록 JsonConverter를 사용하거나, 
                // JsonTypeInfoResolver를 사용해야 하지만, 여기서는 가장 간단한 방법으로 
                // 직렬화 시 타입 정보를 포함하도록 설정합니다. (단, .NET 5.0 이상에서는 복잡해질 수 있음)
                // 현재 .NET Framework를 사용한다고 가정하고, 가장 쉬운 방법을 사용합니다.

                // .NET Framework에서 다형성 직렬화를 위해 일반적으로 NewtonSoft.Json 라이브러리를 사용하지만, 
                // System.Text.Json을 사용하려면, DTO에 [JsonDerivedType] 속성을 사용하거나, 
                // 직접 코드를 복잡하게 작성해야 합니다.
                // 여기서는 프로젝트에 외부 라이브러리(NewtonSoft.Json)를 추가하는 방법을 권장합니다.

                // 만약 System.Text.Json으로 진행한다면, 일단 기본 직렬화만 진행합니다.
                // (이 경우, 역직렬화 시 BaseQuestionData로만 인식되어 파생 클래스의 속성을 잃을 수 있습니다.)
            };


            // 문제 유형 리스트 생성
            List<string> types = new List<string>();

            foreach (var q in allQuestions)
            {
                if (q is MultipleQuestionData m)
                {
                    // Multiple 문제라면 단일/복수 여부에 따라 추가
                    string multipleType = m.IsSingleChoice ? "Single" : "Multiple";
                    if (!types.Contains(multipleType))
                        types.Add(multipleType);
                }
                else
                {
                    // Multiple이 아닌 문제는 기존 Type 이름 그대로
                    string typeName = q.Type.ToString();
                    if (!types.Contains(typeName))
                        types.Add(typeName);
                }
            }


            // JSON 최상단 패키지 생성
            var package = new
            {
                title = rawName,
                totalCount = allQuestions.Count,
                types = types,
                questions = allQuestions
            };


            string jsonString = JsonSerializer.Serialize(package, options);



            // 5. SaveFileDialog를 통해 저장 경로 지정
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "JSON 파일 (*.json)|*.json";
                saveFileDialog.Title = "문제 파일 저장";
                saveFileDialog.FileName = fileName;

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string filePath = saveFileDialog.FileName;

                    try
                    {
                        // 6. 파일 저장
                        File.WriteAllText(filePath, jsonString, Encoding.UTF8);
                        MessageBox.Show("문제가 성공적으로 저장되었습니다.", "저장 완료", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        this.Close();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"파일 저장 중 오류가 발생했습니다: {ex.Message}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
    }
}