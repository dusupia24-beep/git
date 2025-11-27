using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace team_project
{
    public partial class ShortControl : BaseQuestionControl
    {
        public ShortControl()
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


        // GetQuestionData() 메서드 추가 (다형성 구현)
        public override BaseQuestionData GetQuestionData()
        {
            // ShortQuestionData DTO를 생성하여 반환합니다.
            return new ShortQuestionData
            {
                ProblemText = Problem.Text, // 문제 입력 필드
                AnswerText = Answer.Text    // 정답 입력 필드
            };
        }
    }
}
