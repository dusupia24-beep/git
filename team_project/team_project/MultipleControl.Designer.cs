namespace team_project
{
    partial class MultipleControl
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
            this.Num = new System.Windows.Forms.Label();
            this.Problem = new System.Windows.Forms.TextBox();
            this.Del = new System.Windows.Forms.Button();
            this.SingleBtn = new System.Windows.Forms.Button();
            this.MultipleBtn = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // Num
            // 
            this.Num.AutoSize = true;
            this.Num.Font = new System.Drawing.Font("굴림", 24F);
            this.Num.Location = new System.Drawing.Point(3, 11);
            this.Num.Name = "Num";
            this.Num.Size = new System.Drawing.Size(25, 32);
            this.Num.TabIndex = 1;
            this.Num.Text = ".";
            // 
            // Problem
            // 
            this.Problem.Font = new System.Drawing.Font("굴림", 13F);
            this.Problem.Location = new System.Drawing.Point(70, 17);
            this.Problem.Name = "Problem";
            this.Problem.Size = new System.Drawing.Size(1049, 27);
            this.Problem.TabIndex = 2;
            // 
            // Del
            // 
            this.Del.ForeColor = System.Drawing.Color.Red;
            this.Del.Location = new System.Drawing.Point(1125, 11);
            this.Del.Name = "Del";
            this.Del.Size = new System.Drawing.Size(41, 41);
            this.Del.TabIndex = 3;
            this.Del.Text = "삭제";
            this.Del.UseVisualStyleBackColor = true;
            this.Del.Click += new System.EventHandler(this.OptionDel_Click);
            // 
            // SingleBtn
            // 
            this.SingleBtn.Location = new System.Drawing.Point(70, 64);
            this.SingleBtn.Name = "SingleBtn";
            this.SingleBtn.Size = new System.Drawing.Size(132, 34);
            this.SingleBtn.TabIndex = 4;
            this.SingleBtn.Text = "단일선택 보기 추가";
            this.SingleBtn.UseVisualStyleBackColor = true;
            this.SingleBtn.Click += new System.EventHandler(this.SingleOptionAddButton_Click);
            // 
            // MultipleBtn
            // 
            this.MultipleBtn.Location = new System.Drawing.Point(208, 64);
            this.MultipleBtn.Name = "MultipleBtn";
            this.MultipleBtn.Size = new System.Drawing.Size(132, 34);
            this.MultipleBtn.TabIndex = 5;
            this.MultipleBtn.Text = "복수선택 보기 추가";
            this.MultipleBtn.UseVisualStyleBackColor = true;
            this.MultipleBtn.Click += new System.EventHandler(this.OptionAddButton_Click);
            // 
            // MultipleControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Azure;
            this.Controls.Add(this.MultipleBtn);
            this.Controls.Add(this.SingleBtn);
            this.Controls.Add(this.Del);
            this.Controls.Add(this.Problem);
            this.Controls.Add(this.Num);
            this.Name = "MultipleControl";
            this.Size = new System.Drawing.Size(1198, 117);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label Num;
        private System.Windows.Forms.TextBox Problem;
        private System.Windows.Forms.Button Del;
        private System.Windows.Forms.Button SingleBtn;
        private System.Windows.Forms.Button MultipleBtn;
    }
}
