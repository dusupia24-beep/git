namespace team_project
{
    partial class Multiple
    {
        /// <summary> 
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 구성 요소 디자이너에서 생성한 코드

        /// <summary> 
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
        /// </summary>
        private void InitializeComponent()
        {
            this.Del = new System.Windows.Forms.Button();
            this.Answer = new System.Windows.Forms.CheckBox();
            this.AnswerTxt = new System.Windows.Forms.TextBox();
            this.Num = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // Del
            // 
            this.Del.ForeColor = System.Drawing.Color.Red;
            this.Del.Location = new System.Drawing.Point(987, 3);
            this.Del.Name = "Del";
            this.Del.Size = new System.Drawing.Size(59, 24);
            this.Del.TabIndex = 3;
            this.Del.Text = "취소";
            this.Del.UseVisualStyleBackColor = true;
            // 
            // Answer
            // 
            this.Answer.AutoSize = true;
            this.Answer.Location = new System.Drawing.Point(3, 8);
            this.Answer.Name = "Answer";
            this.Answer.Size = new System.Drawing.Size(15, 14);
            this.Answer.TabIndex = 4;
            this.Answer.UseVisualStyleBackColor = true;
            // 
            // AnswerTxt
            // 
            this.AnswerTxt.Font = new System.Drawing.Font("굴림", 10F);
            this.AnswerTxt.Location = new System.Drawing.Point(50, 4);
            this.AnswerTxt.Name = "AnswerTxt";
            this.AnswerTxt.Size = new System.Drawing.Size(931, 23);
            this.AnswerTxt.TabIndex = 7;
            // 
            // Num
            // 
            this.Num.AutoSize = true;
            this.Num.Font = new System.Drawing.Font("굴림", 10F);
            this.Num.Location = new System.Drawing.Point(24, 8);
            this.Num.Name = "Num";
            this.Num.Size = new System.Drawing.Size(12, 14);
            this.Num.TabIndex = 8;
            this.Num.Text = ".";
            // 
            // Multiple
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.Num);
            this.Controls.Add(this.AnswerTxt);
            this.Controls.Add(this.Answer);
            this.Controls.Add(this.Del);
            this.Name = "Multiple";
            this.Size = new System.Drawing.Size(1049, 31);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.Button Del;
        private System.Windows.Forms.CheckBox Answer;
        private System.Windows.Forms.TextBox AnswerTxt;
        public System.Windows.Forms.Label Num;
    }
}
