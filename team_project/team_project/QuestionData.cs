using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace team_project
{
    // 1. 문제 유형 Enum (QuestionData.cs 파일에 정의)
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum QuestionType { Short, Ox, Multiple }


    // ==========================================================
    // ⭐ 다형성 직렬화 설정 추가 (핵심)
    // ==========================================================
    [JsonPolymorphic(
        TypeDiscriminatorPropertyName = "$type")] // 타입 식별자 이름을 "$type"으로 설정
    [JsonDerivedType(typeof(OxQuestionData), typeDiscriminator: "Ox")] // OxQuestionData를 Type="Ox"로 저장하도록 설정
    [JsonDerivedType(typeof(ShortQuestionData), typeDiscriminator: "Short")] // ShortQuestionData를 Type="Short"로 저장하도록 설정
    [JsonDerivedType(typeof(MultipleQuestionData), typeDiscriminator: "Multiple")] // MultipleQuestionData를 Type="Multiple"로 저장하도록 설정

    // 2. 기본 문제 데이터 (BaseQuestionData.cs 파일 내용을 여기로 가져옴)
    public abstract class BaseQuestionData
    {
        public QuestionType Type { get; protected set; }
        public int Number { get; set; }
        public string ProblemText { get; set; }

        public BaseQuestionData()
        {
            // 상속받는 클래스 이름에 따라 Type 자동 설정 (예: MultipleQuestionData -> Multiple)
            Type = (QuestionType)Enum.Parse(typeof(QuestionType), this.GetType().Name.Replace("QuestionData", ""));
        }
    }

    // 3. 객관식 옵션 데이터 (OptionData.cs 파일 내용을 여기로 가져옴)
    public class OptionData
    {
        public string Text { get; set; }// 옵션 텍스트
        public bool IsCorrect { get; set; }// 정답 여부
    }

    // 4. 객관식/복수선택 문제 데이터 (MultipleQuestionData.cs 파일 내용을 여기로 가져옴)
    public class MultipleQuestionData : BaseQuestionData
    {
        public MultipleQuestionData() { }
        public bool IsSingleChoice { get; set; }// 단일 선택 여부
        public List<OptionData> Options { get; set; }// 옵션 리스트
    }

    // 5. 주관식 문제 데이터 (ShortQuestionData.cs 파일 내용을 여기로 가져옴)
    public class ShortQuestionData : BaseQuestionData
    {
        public ShortQuestionData() { }
        public string AnswerText { get; set; }// 정답 텍스트
    }

    // 6. O/X 문제 데이터 (OxQuestionData.cs 파일 내용을 여기로 가져옴)
    public class OxQuestionData : BaseQuestionData
    {
        public OxQuestionData() { }
        public bool? AnswerIsO { get; set; }// true: O, false: X, null: 미설정
    }

    public class QuestionSetData
    {
        // JSON 키 이름과 동일하게 소문자 't'를 사용합니다.
        public string title { get; set; }
        public int totalCount { get; set; }

        // ⚠️ 수정: 단일 문자열이 아닌 문자열 리스트로 변경합니다.
        public List<string> types { get; set; }

        // BaseQuestionData의 파생 클래스 리스트는 그대로 유지합니다.
        public List<BaseQuestionData> questions { get; set; }
    }
}