using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
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

            DisplayFileList(_allQuestionSets);
        }

        // ⭐ C# 7.3 호환: switch 표현식 대신 if-else if 구문 사용
        private static string ConvertTypeToKorean(string type)
        {
            string trimmedType = type.Trim().ToLower();

            if (trimmedType == "short")
            {
                return "단답형";
            }
            else if (trimmedType == "single")
            {
                return "객관형";
            }
            else if (trimmedType == "ox")
            {
                return "OX";
            }
            else if (trimmedType == "multiple")
            {
                return "객관형(복수정답)";
            }
            // 매칭되지 않으면 원래 문자열 반환
            return type;
        }

        // List<string> 영문 유형 목록을 한글 문자열로 결합하는 메서드
        private static string GetProblemTypesString(List<string> types)
        {
            if (types == null || types.Count == 0)
            {
                return "유형 없음";
            }
            // 각 영문 유형을 한글로 변환 후, 콤마와 공백으로 연결
            var koreanTypes = types.Select(ConvertTypeToKorean).ToList();
            return string.Join(", ", koreanTypes);
        }


        // QuestionSetInfo 클래스는 그대로 유지합니다.
        public class QuestionSetInfo
        {
            public string title { get; set; }
            public int totalCount { get; set; }
            public List<string> types { get; set; }
            public string FileName { get; set; }

            public override string ToString()
            {
                string typesString = types != null ? string.Join(", ", types) : "유형 없음";
                return $"{title} | {totalCount}개 | {typesString}";
            }
        }

        // LoadQuestionSetInfos 메서드는 그대로 유지됩니다.
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

        // ListView에 항목을 표시하는 메서드
        private void DisplayFileList(List<QuestionSetInfo> questionSets)
        {
            this.listView1.Items.Clear();

            foreach (QuestionSetInfo info in questionSets)
            {
                // 한글 유형 문자열 사용
                string typesString = GetProblemTypesString(info.types);

                ListViewItem item = new ListViewItem(info.title);

                item.SubItems.Add($"{info.totalCount}개"); // 개수
                item.SubItems.Add(typesString);             // 한글 유형들

                item.Tag = info;

                this.listView1.Items.Add(item);
            }
        }

        // 검색 텍스트 변경 이벤트 핸들러는 그대로 유지됩니다.
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

        // ListView 더블 클릭 이벤트 핸들러
        private void listView1_DoubleClick(object sender, EventArgs e)
        {
            BaseQuestionData firstQuestion = null;

            if (this.listView1.SelectedItems.Count > 0)
            {
                ListViewItem selectedItem = this.listView1.SelectedItems[0];

                if (selectedItem.Tag is QuestionSetInfo selectedInfo)
                {
                    string problemTitle = selectedInfo.title;
                    string selectedFile = selectedInfo.FileName;
                    string fullPath = Path.Combine(_questionFolderPath, selectedFile + ".json");

                    int totalProblems = 0;
                    string problemTypes = "데이터 없음";

                    if (File.Exists(fullPath))
                    {
                        try
                        {
                            string jsonString = File.ReadAllText(fullPath);

                            var options = new JsonSerializerOptions
                            {
                                PropertyNameCaseInsensitive = true
                            };

                            // ⭐ 주의: System.Text.Json의 다형성 처리(BaseQuestionData의 하위 클래스 로드)는 
                            // C# 버전보다는 .NET 런타임 버전(>=.NET 6)에 더 의존합니다. 
                            // 현재 코드가 .NET 6/7/8 환경이라면 문제 없으나, .NET Framework 환경이라면 
                            // 여기에서 문제가 발생할 수 있으며, 이 경우 Newtonsoft.Json 등의 외부 라이브러리를 사용해야 합니다.
                            QuestionSetData data = JsonSerializer.Deserialize<QuestionSetData>(jsonString, options);

                            if (data != null)
                            {
                                totalProblems = data.totalCount;

                                if (data.types != null && data.types.Count > 0)
                                {
                                    problemTypes = GetProblemTypesString(data.types);
                                }

                                if (data.questions != null && data.questions.Count > 0)
                                {
                                    firstQuestion = data.questions[0];
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"문제 로드 중 오류 발생: {ex.Message}", "오류");
                            return;
                        }
                    }

                    // PreviewForm 인스턴스 생성 및 한글 유형 데이터 전달
                    PreviewForm previewForm = new PreviewForm(
                        problemTitle,
                        totalProblems,
                        problemTypes,
                        firstQuestion
                    );

                    // 폼 전환
                    this.Hide();
                    previewForm.ShowDialog();
                    this.Show();
                }
            }
        }

        private void Cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}