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
    public partial class OxControl : BaseQuestionControl
    {
        public OxControl()
        {
            InitializeComponent();
        }
        public override string NumText
        {
            get { return Num.Text; }
            set { Num.Text = value; }
        }
        private void Del_Click(object sender, EventArgs e) // 👈 유저 컨트롤 내의 삭제 버튼 이벤트
        {
            // 이벤트 발생 (부모 폼에게 알림)
            OnDeleteRequested();
        }

        // 다형성을 위한 GetQuestionData 메서드 재정의
        public override BaseQuestionData GetQuestionData()
        {
            // 1. OxQuestionData DTO를 생성합니다.
            var data = new OxQuestionData
            {
                ProblemText = Problem.Text, // 문제 내용 입력 필드
            };

            // 2. O 또는 X RadioButton 중 무엇이 체크되었는지 확인하여 정답을 설정합니다.
            // OxControl.Designer.cs에 RadioO와 RadioX가 있다고 가정합니다.
            if (RadioO.Checked)
            {
                data.AnswerIsO = true; // O가 정답
            }
            else if (RadioX.Checked)
            {
                data.AnswerIsO = false; // X가 정답
            }
            // 둘 다 체크 안 된 경우(data.IsAnswerO는 기본값 false)는 문제의 유효성 검사 로직에서 처리해야 합니다.

            return data;
        }
    }
}
