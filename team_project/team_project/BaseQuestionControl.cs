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
    // 📌 추상 클래스: 모든 문제 컨트롤의 부모
    public abstract partial class BaseQuestionControl : UserControl, IQuestionControl
    {
        // IQuestionControl 구현 (문제 번호)
        public abstract string NumText { get; set; }

        // 📌 핵심: 다형성을 위한 추상 메서드 - 자식 클래스는 반드시 재정의해야 합니다.
        // 반환 타입이 상속받은 BaseQuestionData이므로 다형성을 활용하기 좋습니다.
        public abstract BaseQuestionData GetQuestionData();


        // 삭제 요청 이벤트 (기존 코드에서 OxControl, ShortControl 등에 이미 존재)
        public event EventHandler DeleteRequested;

        protected void OnDeleteRequested()
        {
            DeleteRequested?.Invoke(this, EventArgs.Empty);
        }
    }
}