using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace team_project
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ProblemForm problemForm = new ProblemForm();
            problemForm.ShowDialog();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            using (var fbd = new FolderBrowserDialog())
            {
                // 초기 디렉토리 설정 (선택 사항)
                fbd.SelectedPath = AppDomain.CurrentDomain.BaseDirectory;

                // 사용자가 폴더를 선택하고 확인 버튼을 눌렀는지 확인
                if (fbd.ShowDialog() == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    string selectedFolderPath = fbd.SelectedPath;

                    // 2. 선택된 폴더에서 JSON 파일 목록 가져오기
                    // 파일명만 필요한 경우 (확장자 제외)
                    List<string> jsonFileNames = new List<string>();

                    try
                    {
                        // 폴더 내의 모든 *.json 파일의 경로를 가져옵니다.
                        string[] files = Directory.GetFiles(selectedFolderPath, "*.json");

                        foreach (string filePath in files)
                        {
                            // 파일 경로에서 파일 이름(확장자 포함)만 추출
                            string fileNameWithExt = Path.GetFileName(filePath);
                            // 파일 이름에서 확장자(.json)를 제거하고 목록에 추가
                            string fileNameWithoutExt = Path.GetFileNameWithoutExtension(filePath);
                            jsonFileNames.Add(fileNameWithoutExt);
                        }

                        // 3. 목록을 보여줄 새로운 폼을 생성하고 데이터 전달
                        if (jsonFileNames.Count > 0)
                        {
                            // 💡 SolveListForm은 아직 정의되지 않았으므로 임시로 MessageBox를 사용합니다.
                            // 다음 단계에서 이 코드를 SolveListForm 호출로 변경해야 합니다.
                            MessageBox.Show($"선택된 폴더: {selectedFolderPath}\n\n발견된 JSON 파일 목록:\n{string.Join("\n", jsonFileNames)}", "문제 목록 로드 성공");
                            SolveListForm solveListForm = new SolveListForm(selectedFolderPath, jsonFileNames);
                            solveListForm.ShowDialog();
                        }
                        else
                        {
                            MessageBox.Show("선택된 폴더에 *.json 확장자를 가진 파일이 없습니다.", "알림");
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"파일을 읽는 도중 오류가 발생했습니다: {ex.Message}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            MessageBox.Show("문제를 생성하기 전에 문제들을 저장할 새로운 폴더를 만들어주세요.", "도움말");
        }
    }
}
