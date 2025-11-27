using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Windows.Forms;

namespace team_project
{
    public partial class SolveListForm : Form
    {
        private string _questionFolderPath;
        private List<QuestionSetInfo> _allQuestionSets;

        public SolveListForm(string folderPath, List<string> fileNames)
        {
            InitializeComponent();
            this.listView1.View = View.Details;

            _questionFolderPath = folderPath;

            _allQuestionSets = LoadQuestionSetInfos(folderPath, fileNames);

            // ⭐ ListView를 사용하는 새 메서드로 변경
            DisplayFileList(_allQuestionSets);


            // 📝 SearchText_TextChanged 이벤트 핸들러는 그대로 유지됩니다.
        }





        // ⭐ QuestionSetInfo 클래스는 그대로 유지합니다. (ListView의 Tag에 저장할 데이터 모델)
        public class QuestionSetInfo
        {
            public string title { get; set; }
            public int totalCount { get; set; }
            public List<string> types { get; set; }
            public string FileName { get; set; }

            // ListBox가 아닌 ListView를 사용하므로, ToString() 오버라이드는 항목 표시에 사용되지 않습니다.
            // 하지만 디버깅 용도로 유지하거나 삭제할 수 있습니다.
            public override string ToString()
            {
                string typesString = types != null ? string.Join(", ", types) : "유형 없음";
                return $"{title} | {totalCount}개 | {typesString}";
            }
        }





        // ⭐ LoadQuestionSetInfos 메서드는 그대로 유지됩니다. (데이터 로드 로직)
        private List<QuestionSetInfo> LoadQuestionSetInfos(string folderPath, List<string> fileNames)
        {
            var questionSets = new List<QuestionSetInfo>();

            foreach (string fileName in fileNames)
            {
                string fullPath = Path.Combine(folderPath, fileName + ".json");
                if (File.Exists(fullPath))
                {
                    try
                    {
                        string jsonString = File.ReadAllText(fullPath);
                        var info = JsonSerializer.Deserialize<QuestionSetInfo>(jsonString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                        if (info != null)
                        {
                            info.FileName = fileName;
                            questionSets.Add(info);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"파일 로드 오류 ({fileName}.json): {ex.Message}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            return questionSets;
        }

        // 🟢 수정: ListView에 항목을 표시하는 메서드
        private void DisplayFileList(List<QuestionSetInfo> questionSets)
        {
            this.listView1.Items.Clear(); // ❌ listBox1.Items.Clear() -> listView1.Items.Clear()

            foreach (QuestionSetInfo info in questionSets)
            {
                string typesString = info.types != null ? string.Join(", ", info.types) : "유형 없음";

                // 1. 새로운 ListViewItem을 생성하고 첫 번째 열(문제 제목)을 설정
                ListViewItem item = new ListViewItem(info.title);

                // 2. 두 번째 이후의 열(SubItems)을 추가
                item.SubItems.Add($"{info.totalCount}개"); // 개수
                item.SubItems.Add(typesString);             // 유형들

                // 3. QuestionSetInfo 객체 전체를 Tag 속성에 저장 (더블 클릭 시 사용)
                item.Tag = info;

                // 4. ListView에 항목 추가
                this.listView1.Items.Add(item);
            }
            // 모든 항목을 추가한 후 열 너비를 자동으로 조정할 수도 있습니다.
            // this.listView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
        }

        // ⭐ 검색 텍스트 변경 이벤트 핸들러 (ListView에 맞게 로직 수정)
        private void SearchText_TextChanged(object sender, EventArgs e)
        {
            string searchTerm = SearchText.Text.Trim();

            if (string.IsNullOrEmpty(searchTerm))
            {
                DisplayFileList(_allQuestionSets);
            }
            else
            {
                // QuestionSetInfo 목록을 필터링합니다.
                List<QuestionSetInfo> filteredList = _allQuestionSets
                    .Where(info => info.title.IndexOf(searchTerm, StringComparison.OrdinalIgnoreCase) >= 0)
                    .ToList();

                // 필터링된 목록을 ListView에 표시합니다.
                DisplayFileList(filteredList);
            }
        }

        // 🟢 수정: ListView 더블 클릭 이벤트 핸들러
        private void listView1_DoubleClick(object sender, EventArgs e)
        {
            // ListView는 SelectedItems 컬렉션을 사용하며, Details 모드에서는 일반적으로 첫 번째 항목을 사용합니다.
            if (this.listView1.SelectedItems.Count > 0)
            {
                // 선택된 ListViewItem을 가져옵니다.
                ListViewItem selectedItem = this.listView1.SelectedItems[0];

                // Tag 속성에 저장했던 QuestionSetInfo 객체를 가져옵니다.
                if (selectedItem.Tag is QuestionSetInfo selectedInfo)
                {
                    string selectedFile = selectedInfo.FileName;
                    string fullPath = Path.Combine(_questionFolderPath, selectedFile + ".json");

                    MessageBox.Show($"선택된 파일: {selectedFile}.json (제목: {selectedInfo.title})\n이제 이 파일을 로드하여 문제 풀이 폼을 열어야 합니다.", "문제 풀이 시작");
                }
            }
        }

        private void Cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}